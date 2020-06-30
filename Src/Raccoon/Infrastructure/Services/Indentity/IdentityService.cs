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

namespace Infrastructure.Services.Indentity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService;
        private readonly IStringLocalizer<SharedResource> _sharedResource;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<ApplicationUser> userManager, 
            ISmsService smsService, 
            IStringLocalizer<SharedResource> sharedResource,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _smsService = smsService;
            _sharedResource = sharedResource;
            _logger = logger;
        }

        public async Task<bool> SendCodeAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, _sharedResource["UserNotFound"].Value.Replace("{0}", username));

            user.ConfirmationCode = GenerateRandomFourDigitCode();
            user.DateOfConfirmationCodeExpiration = DateTime.UtcNow.AddMinutes(2);
            var content = $"Eknot: Tekseru kody/proverochniy kod: {user.ConfirmationCode}";

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return false;

            return await _smsService.SendAsync(username, content, cancellationToken);
        }

        public Task<AuthenticationResult> LoginAsync(string username, string code, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAsync(string username, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
                return false;

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
        public Task<bool> RemoveAsync(string username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private string GenerateRandomFourDigitCode() => new Random().Next(1000, 9999).ToString();

    }
}
