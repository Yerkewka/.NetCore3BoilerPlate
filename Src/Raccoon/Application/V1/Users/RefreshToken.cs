using System;
using MediatR;
using Resources;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Application.Common.Interfaces.Services;
using Domain.Models.Configuration;

namespace Application.V1.Users
{
    public class RefreshToken
    {
        public class RefreshTokenCommand : IRequest<Login.AuthenticationResponse>
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }

        public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
        {
            public RefreshTokenCommandValidator()
            {
                RuleFor(x => x.Token)
                    .NotEmpty();

                RuleFor(x => x.RefreshToken)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<RefreshTokenCommand, Login.AuthenticationResponse>
        {
            private readonly IIdentityService _identityService;
            private readonly IStringLocalizer<SharedResource> _sharedResource;

            public Handler(IIdentityService identityService, IStringLocalizer<SharedResource> sharedResource)
            {
                _identityService = identityService;
                _sharedResource = sharedResource;
            }

            public async Task<Login.AuthenticationResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            {
                var result = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
                if (result == null)
                    throw new Exception(_sharedResource["InternalServerError"]);

                return new Login.AuthenticationResponse
                {
                    Token = result.Token,
                    RefreshToken = result.RefreshToken
                };
            }
        }
    }
}
