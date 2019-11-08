using System;
using System.Collections.Generic;
using System.Text;

namespace EagleServer.Exceptions
{
    class ServerNotStartedException : Exception
    {
       public ServerNotStartedException(): base("EagleServer is not Started")
       {
       }

        public ServerNotStartedException(string message) : base(message)
        {
        }

        public ServerNotStartedException(string message, Exception inner) : base(message, inner)
        {

        }

    }
}
