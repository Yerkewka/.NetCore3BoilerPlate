using Application.Common.Extensions;
using Application.Common.Interfaces.Services;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.V1.Users
{
    public class SendCode
    {
        public class SendCodeCommand : IRequest<bool>
        {
            public string Username { get; set; }
        }

        public class SendCodeCommandValidator : AbstractValidator<SendCodeCommand>
        {
            public SendCodeCommandValidator()
            {
                RuleFor(x => x.Username).MobileNumberInKazakhstan();
            }
        }

        public class Handler : IRequestHandler<SendCodeCommand, bool>
        {
            private readonly IIdentityService _identityService;

            public Handler(IIdentityService identityService)
            {
                _identityService = identityService;
            }

            public async Task<bool> Handle(SendCodeCommand request, CancellationToken cancellationToken)
            {
                await _identityService.RegisterAsync(request.Username, cancellationToken);

                return await _identityService.SendCodeAsync(request.Username, cancellationToken);
            }
        }
    }
}
