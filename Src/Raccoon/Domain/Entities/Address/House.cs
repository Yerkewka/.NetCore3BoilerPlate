using Domain.Base;
using MongoDB.Entities;

namespace Domain.Entities.Address
{
    public class House : DictionaryBase
    {
        public One<Street> Street { get; set; }
    }
}
