using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.MyExceptions
{
    public class NoCardException : System.Exception
    {
        public override string Message
        {
            get
            {
                return "No Carderino";
            }
        }

    }


}
