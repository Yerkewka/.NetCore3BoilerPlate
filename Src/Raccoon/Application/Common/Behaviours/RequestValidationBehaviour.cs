using MediatR;
using System.Linq;
using System.Threading;
using FluentValidation;
using System.Threading.Tasks;
using System.Collections.Generic;
using Resources;
using Microsoft.Extensions.Localization;

namespace Application.Common.Behaviours
{
    public class RequestValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly IStringLocalizer<SharedResource> _sharedResource;

        public RequestValidationBehaviour(
            IEnumerable<IValidator<TRequest>> validators,
            IStringLocalizer<SharedResource> sharedResource)
        {
            _validators = validators;
            _sharedResource = sharedResource;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);

            var failures = _validators
                .Select(x => x.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(x => x != null)
                .ToList();

            if (failures.Any())
                throw new Exceptions.ValidationException(_sharedResource["ValidationErrorOccured"], failures);

            return next();
        }
    }
}
