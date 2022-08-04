namespace netDxf.Header
{
    using System;

    public enum PointShape
    {
        Dot = 0,
        Empty = 1,
        Plus = 2,
        Cross = 3,
        Line = 4,
        CircleDot = 0x20,
        CircleEmpty = 0x21,
        CirclePlus = 0x22,
        CircleCross = 0x23,
        CircleLine = 0x24,
        SquareDot = 0x40,
        SquareEmpty = 0x41,
        SquarePlus = 0x42,
        SquareCross = 0x43,
        SquareLine = 0x44,
        CircleSquareDot = 0x60,
        CircleSquareEmpty = 0x61,
        CircleSquarePlus = 0x62,
        CircleSquareCross = 0x63,
        CircleSquareLine = 100
    }
}

