using System;
using System.Collections.Generic;
using System.Text;

namespace EagleServer.Exceptions
{
    public class HttpStatusAwareException : Exception
    {

        public HttpStatusAwareException(int statusCode, string message)
        {
            this.StatusCode = statusCode;
            this.Body = message;
        }

        public int StatusCode { get; set; }

        public string Body { get; set; }

    }
}
