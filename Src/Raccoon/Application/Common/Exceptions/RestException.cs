using System;
using System.Net;

namespace Application.Common.Exceptions
{
    public class RestException : Exception
    {
        public HttpStatusCode Code { get; set; }

        public RestException(HttpStatusCode code) : base()
        {
            Code = code;
        }

        public RestException(HttpStatusCode code, string message) : base(message)
        {
            Code = code;
        }
    }
}
