namespace netDxf.Entities
{
    using System;

    [Flags]
    public enum HatchBoundaryPathTypeFlags
    {
        Default = 0,
        External = 1,
        Polyline = 2,
        Derived = 4,
        Textbox = 8,
        Outermost = 0x10
    }
}

