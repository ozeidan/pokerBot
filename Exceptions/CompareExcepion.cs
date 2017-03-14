using System;

namespace Bot.Exceptions
{
    internal class CompareExcepion : Exception
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