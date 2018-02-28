
namespace AICheckers
{
    interface IAI
    {
        CheckerColour Colour { get; set; }
        Move Process(Square[,] board);
    }
}
