using System;
using System.Collections.Generic;
using System.Text;

namespace EagleServer.Exceptions
{
    class HttpListenerNotSupported : Exception
    {

        public HttpListenerNotSupported() : base("HttpListner not supported") { }

    }
}
