using System;
using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.Mongo.Model;

namespace Domain.Entities.Indentity
{
    public class ApplicationUser : MongoUser
    {
        [MaxLength(50)]
        public string Firstname { get; set; }
        [MaxLength(50)]
        public string Lastname { get; set; }
        [MaxLength(50)]
        public string Patronymic { get; set; }

        [StringLength(12)]
        public string IndentityNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DateModifiedOn { get; set; } = DateTime.UtcNow;

        public string ConfirmationCode { get; set; }
        public DateTime? DateOfConfirmationCodeExpiration { get; set; }
    }
}
