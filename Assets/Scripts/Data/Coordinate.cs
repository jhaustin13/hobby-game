using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public readonly static Coordinate PlusXYZ = new Coordinate(1, 1, 1);
    public readonly static Coordinate NegXYZ = new Coordinate(-1, -1, -1);

    public readonly static Coordinate PlusXZ = new Coordinate(1, 0, 1);
    public readonly static Coordinate PlusXY = new Coordinate(1, 1, 0); 
    public readonly static Coordinate PlusYZ = new Coordinate(0, 1, 1);

    public readonly static Coordinate PlusX = new Coordinate(1, 0, 0);
    public readonly static Coordinate PlusY = new Coordinate(0, 1, 0);
    public readonly static Coordinate PlusZ = new Coordinate(0, 0, 1);

    public readonly static Coordinate NegX = new Coordinate(-1, 0, 0);
    public readonly static Coordinate NegY = new Coordinate(0, -1, 0);
    public readonly static Coordinate NegZ = new Coordinate(0, 0, -1);

    public readonly static Coordinate NegXZ = new Coordinate(-1, 0, -1);
    public readonly static Coordinate NegXY = new Coordinate(-1, -1, 0);
    public readonly static Coordinate NegYZ = new Coordinate(0, -1, -1);

    public readonly static Coordinate PlusXNegZ = new Coordinate(1, 0, -1);
    public readonly static Coordinate PlusXNegY = new Coordinate(1, -1, 0);
    public readonly static Coordinate PlusYNegZ = new Coordinate(0, 1, -1);

    public readonly static Coordinate PlusYNegX = new Coordinate(-1, 1, 0);
    public readonly static Coordinate PlusZNegX = new Coordinate(-1, 0, 1);
    public readonly static Coordinate PlusZNegY = new Coordinate(0, -1, 1);

    public readonly static Coordinate PlusXZNegY = new Coordinate(1, -1, 1);
    public readonly static Coordinate PlusXYNegZ = new Coordinate(1, 1, -1);
    public readonly static Coordinate PlusYZNegX = new Coordinate(-1, 1, 1);

    public readonly static Coordinate PlusXNegYZ = new Coordinate(1, -1, -1);
    public readonly static Coordinate PlusYNegXZ = new Coordinate(-1, 1, -1);
    public readonly static Coordinate PlusZNegXY = new Coordinate(-1, -1, 1);


    public Coordinate(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }


}

