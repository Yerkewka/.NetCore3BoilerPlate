using Application.Common.Extensions;
using Application.Common.Interfaces.Services.Indentity;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using Resources;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Application.V1.Users
{
    public class Login
    {
        public class AuthenticationResponse
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }

        public class LoginCommand : IRequest<AuthenticationResponse>
        {
            public string Username { get; set; }
            public string Code { get; set; }
        }

        public class LoginCommandValidator : AbstractValidator<LoginCommand>
        {
            public LoginCommandValidator()
            {
                RuleFor(x => x.Username).MobileNumberInKazakhstan();
                RuleFor(x => x.Code).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<LoginCommand, AuthenticationResponse>
        {
            private readonly IIdentityService _identityService;
            private readonly IStringLocalizer<SharedResource> _sharedResource;

            public Handler(IIdentityService identityService, IStringLocalizer<SharedResource> sharedResource)
            {
                _identityService = identityService;
                _sharedResource = sharedResource;
            }

            public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var result = await _identityService.LoginAsync(request.Username, request.Code, cancellationToken);
                if (result == null)
                    throw new Exception(_sharedResource["InternalServerError"]);

                return new AuthenticationResponse
                {
                    Token = result.Token,
                    RefreshToken = result.RefreshToken
                };
            }
        }
    }
}
