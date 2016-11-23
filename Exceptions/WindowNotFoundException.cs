using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Bot.MyExceptions
{
    public class WindowNotFoundException : System.Exception
    {
        public WindowNotFoundException()
        { }

        public WindowNotFoundException(string message)
            : base(message)
        { }

        public WindowNotFoundException(string message, System.Exception innerException)
            : base(message, innerException)
        { }

    }
}
