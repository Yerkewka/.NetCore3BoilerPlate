using Domain.Base;
using MongoDB.Entities;

namespace Domain.Entities.Address
{
    [Name("Streets")]
    public class Street : DictionaryBase
    {
        public One<District> District { get; set; }
        public Many<House> Houses { get; set; }

        public Street()
        {
            this.InitOneToMany(() => Houses);
        }
    }
}
