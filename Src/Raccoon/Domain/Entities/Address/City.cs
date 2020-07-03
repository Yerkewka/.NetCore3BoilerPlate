using Domain.Base;
using MongoDB.Entities;

namespace Domain.Entities.Address
{
    [Name("Cities")]
    public class City : DictionaryBase
    {
        public Many<District> Districts { get; set; }

        public City()
        {
            this.InitOneToMany(() => Districts);
        }
    }
}
