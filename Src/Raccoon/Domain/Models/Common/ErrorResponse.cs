using System.Collections.Generic;

namespace Domain.Models.Common
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
}
