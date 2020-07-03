using System;
using Domain.Base;
using MongoDB.Entities;

namespace Domain.Entities.Indentity
{
    [Name("RefreshTokens")]
    public class RefreshToken : EntityBase
    {
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime DateOfExpiration { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
        public string Username { get; set; }
    }
}
