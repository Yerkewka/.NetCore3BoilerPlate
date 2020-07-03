using Domain.Base;
using MongoDB.Entities;

namespace Domain.Entities.Address
{
    [Name("Districts")]
    public class District : DictionaryBase
    {
        public One<City> City { get; set; }
        public Many<Street> Streets { get; set; }

        public District()
        {
            this.InitOneToMany(() => Streets);
        }
    }
}
