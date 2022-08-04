namespace netDxf.Objects
{
    using System;

    [Flags]
    public enum MLineStyleFlags
    {
        None = 0,
        FillOn = 1,
        DisplayMiters = 2,
        StartSquareEndCap = 0x10,
        StartInnerArcsCap = 0x20,
        StartRoundCap = 0x40,
        EndSquareCap = 0x100,
        EndInnerArcsCap = 0x200,
        EndRoundCap = 0x400
    }
}

