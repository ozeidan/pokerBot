using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.MyExceptions
{
    class CompareExcepion : System.Exception
    {
        public CompareExcepion()
        {

        }

        public CompareExcepion(string message) : base(message)
        {
            
        }
        public CompareExcepion(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
