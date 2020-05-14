using System;
using System.Collections.Generic;
using System.Text;

namespace EagleServer.Exceptions
{
    public class HttpStatusAwareException : Exception
    {

        public HttpStatusAwareException(int statusCode, string message, string contentType = "")
        {
            this.StatusCode = statusCode;
            this.Body = message;
            this.ContentType = contentType;
        }

        public HttpStatusAwareException(int statusCode, object errorObject, string contentType = "application/json")
        {
            this.StatusCode = statusCode;
            this.Body = Newtonsoft.Json.JsonConvert.SerializeObject(errorObject);
            this.ContentType = contentType;
        }

        public int StatusCode { get; set; }

        public string Body { get; set; }

        public string ContentType { get; set; }

    }
}
