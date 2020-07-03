using Application.Common.Extensions;
using Application.Common.Interfaces.Services.Indentity;
using Domain.Entities.Indentity;
using Domain.Models.Configuration;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Resources;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.V1.Users
{
    public class SendCode
    {
        public class SendCodeCommand : IRequest<Unit>
        {
            public string Username { get; set; }

            [SwaggerIgnoreProperty]
            public bool Debug { get; set; }
        }

        public class SendCodeCommandValidator : AbstractValidator<SendCodeCommand>
        {
            public SendCodeCommandValidator()
            {
                RuleFor(x => x.Username).MobileNumberInKazakhstan();
            }
        }

        public class Handler : IRequestHandler<SendCodeCommand, Unit>
        {
            private readonly IIdentityService _identityService;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IStringLocalizer<SharedResource> _sharedResource;

            public Handler(
                IIdentityService identityService, 
                UserManager<ApplicationUser> userManager,
                IStringLocalizer<SharedResource> sharedResource)
            {
                _identityService = identityService;
                _userManager = userManager;
                _sharedResource = sharedResource;
            }

            public async Task<Unit> Handle(SendCodeCommand request, CancellationToken cancellationToken)
            {
                var existingUser = await _userManager.FindByNameAsync(request.Username);
                if (existingUser == null)
                    await _identityService.RegisterAsync(request.Username, cancellationToken);

                var smsSendResult = await _identityService.SendCodeAsync(request.Username, request.Debug, cancellationToken);
                if (!smsSendResult)
                    throw new Exception(_sharedResource["SmsSendError"]);

                return Unit.Value;
            }
        }
    }
}
