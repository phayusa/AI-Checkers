using System.Collections.Generic;
using System.Drawing;

namespace AICheckers
{
    class Move
    {
        public Move()
        {
        }

        public Move(Point source, Point destination)
        {
            _source = source;
            _destination = destination;
        }

        //Convenience constructors
        public Move(Point source, int destinationX, int destinationY)
            : this(source, new Point(destinationX, destinationY))
        {
        }

        //public Move(int sourceX, int sourceY, Point destination)
        //    : this(new Point(sourceX, sourceY), destination)
        //{
        //}

        public Move(int sourceX, int sourceY, int destinationX, int destinationY)
            : this(new Point(sourceX, sourceY), new Point(destinationX, destinationY))
        {
        }

        private readonly Point _source = new Point(-1, -1);
        private readonly Point _destination = new Point(-1, -1);

        public Point Source => _source;

        public Point Destination => _destination;

        public List<Point> Captures { get; } = new List<Point>();

        public int Score { get; set; }

        public override string ToString()
        {
            return $"Source: {_source}, Dest: {_destination}";
        }
    }
}
