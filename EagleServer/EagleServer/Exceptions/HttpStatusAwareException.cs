using System;
using System.Collections.Generic;
using System.Text;

namespace EagleServer.Exceptions
{
    abstract class HttpStatusAwareException : Exception
    {
        public abstract int getStatusCode();

        public abstract string getBody();
    }
}
