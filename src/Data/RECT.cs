namespace Bot.Data
{
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;


        public override string ToString()
        {
            return "Left: " + Left + " Right: " + Right + " Top: " + Top + " Bottom: " + Bottom;
        }
    }
}