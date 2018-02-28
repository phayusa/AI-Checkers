namespace AICheckers
{
    public enum CheckerColour
    {
        Empty,
        Red,
        Black
    }

    class Square
    {
        public CheckerColour Colour = CheckerColour.Empty;
        public bool King = false;
    }
}
