using Domain.Models.Common;

namespace Domain.Base
{
    public abstract class DictionaryBase : EntityBase
    {
        public string Code { get; set; }
        public int ExtraId { get; set; }
        public string ExtraCode { get; set; }
        public Name Name { get; set; }
        public Name ShortName { get; set; }
        public string ParentId { get; set; }
        public bool Active { get; set; }
        public int? Sort { get; set; }
    }
}
