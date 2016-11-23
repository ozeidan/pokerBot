namespace Bot
{
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;


        public override string ToString()
        {
            return "left: " + left + " right: " + right + " top: " + top + " bottom: " + bottom;

        }
    }
}
