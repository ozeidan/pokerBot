namespace Bot.Exceptions
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
