namespace Bot.Data
{
    internal class Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position()
        {
            X = 0;
            Y = 0;
        }
    }
}