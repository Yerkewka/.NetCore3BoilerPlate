using Domain.Entities.Address;
using Domain.Models.Common;
using MediatR;
using MongoDB.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.V1.Test
{
    public class Dictionary
    {
        public class DictionaryQuery : IRequest<Unit>
        {
        }

        public class Handler : IRequestHandler<DictionaryQuery, Unit>
        {
            public async Task<Unit> Handle(DictionaryQuery request, CancellationToken cancellationToken)
            {
                var city = new City
                {
                    ExtraId = 1,
                    ExtraCode = "Nur-Sultan",
                    Name = new Name
                    {
                        Kk = "Нұр-Сұлтан",
                        Ru = "Нур-Султан",
                        En = "Nur-Sultan"
                    }
                };
                await city.SaveAsync();                

                var street = new Street
                {
                    ExtraId = 1,
                    ExtraCode = "Pertov",
                    Name = new Name
                    {
                        Kk = "Петров",
                        Ru = "Петорова",
                        En = "Pertov"
                    },
                };
                await street.SaveAsync();

                return Unit.Value;
            }
        }
    }
}
