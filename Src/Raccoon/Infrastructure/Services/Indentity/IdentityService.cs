using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models.Indentity;
using Domain.Entities.Indentity;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Localization;
using Resources;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Linq;
using Application.Common.Extensions;
using Domain.Models.Configuration.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Application.Common.Interfaces.Repositories;

namespace Infrastructure.Services.Indentity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService;
        private readonly JwtOptions _jwtOptions;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IStringLocalizer<SharedResource> _sharedResource;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<ApplicationUser> userManager, 
            ISmsService smsService,
            JwtOptions jwtOptions,
            IRepositoryWrapper repositoryWrapper,
            TokenValidationParameters tokenValidationParameters,
            IStringLocalizer<SharedResource> sharedResource,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _smsService = smsService;
            _jwtOptions = jwtOptions;
            _repositoryWrapper = repositoryWrapper;
            _tokenValidationParameters = tokenValidationParameters;
            _sharedResource = sharedResource;
            _logger = logger;
        }

        public async Task<bool> SendCodeAsync(string username, bool debug = false, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["InvalidCredentials"]);

            user.AccessFailedCount = 0;
            user.ConfirmationCode = GenerateRandomFourDigitCode();
            user.DateOfConfirmationCodeExpiration = DateTime.UtcNow.AddMinutes(2);
            var content = $"Eknot: Tekseru kody/proverochniy kod: {user.ConfirmationCode}";

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception(_sharedResource["SaveEntityError"]);

            return debug ? true : await _smsService.SendAsync(username, content, cancellationToken);
        }

        public async Task<AuthenticationResult> LoginAsync(string username, string code, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["InvalidCredentials"]);

            var userSpecifiedValidCode = await _userManager.CheckSmsCodeAsync(user, code);
            if (!userSpecifiedValidCode)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["InvalidCode"]);

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
                throw new RestException(HttpStatusCode.Unauthorized, _sharedResource["InvalidToken"]);

            var expiryDateUnix = long.Parse(validatedToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["TokenNotExpiredYetError"]);

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = (await _repositoryWrapper.RefreshTokenRepository.FindAsync(x => x.Token == refreshToken)).FirstOrDefault();
            
            if (storedRefreshToken == null) 
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["RefreshTokenDoesNotExist"]);

            if (DateTime.UtcNow > storedRefreshToken.DateOfExpiration)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["TokenExpired"]);

            if (storedRefreshToken.Invalidated)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["InvalidToken"]);

            if (storedRefreshToken.Used)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["TokenUsed"]);

            if (storedRefreshToken.JwtId != jti)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["RefreshTokenDoesNotMatch"]);

            storedRefreshToken.Used = true;
            await _repositoryWrapper.RefreshTokenRepository.SaveAsync(storedRefreshToken);

            var user = await _userManager.FindByNameAsync(storedRefreshToken.Username);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<bool> RegisterAsync(string username, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["InvalidCredentials"]);

            var user = new ApplicationUser
            {
                UserName = username
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                _logger.LogError(createResult.Errors.Select(x => x.Description).FirstOrDefault());
                throw new RestException(HttpStatusCode.InternalServerError, _sharedResource["UserCreationError"]);
            }

            var addClaimResult = await _userManager.AddClaimAsync(user, new Claim("news.city", "true"));
            if (!addClaimResult.Succeeded)
            {
                _logger.LogError(createResult.Errors.Select(x => x.Description).FirstOrDefault());
                throw new RestException(HttpStatusCode.InternalServerError, _sharedResource["UserAddClaimError"]);
            }

            return addClaimResult.Succeeded;
        }

        public async Task<bool> RemoveAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, _sharedResource["InvalidCredentials"]);

            var removeResult = await _userManager.DeleteAsync(user);
            return removeResult.Succeeded;
        }

        private string GenerateRandomFourDigitCode() => new Random().Next(1000, 9999).ToString();

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtOptions.TokenLifetime),
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                Username = user.UserName,
                DateOfCreation = DateTime.UtcNow,
                DateOfExpiration = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString()
            };

            await _repositoryWrapper.RefreshTokenRepository.SaveAsync(refreshToken);

            return new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationTokenParams = _tokenValidationParameters.Clone();
                validationTokenParams.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, validationTokenParams, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
