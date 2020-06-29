using MongoDB.Entities.Core;
using System;

namespace Domain.Base
{
    /// <summary>
    /// Base class for all entities 
    /// 
    /// If switched to SQL you have to add ID field and exclude inheritance from Entity
    /// </summary>
    public class EntityBase : Entity
    {
        public DateTime? DateOfCreation { get; set; } = DateTime.UtcNow;
    }
}
