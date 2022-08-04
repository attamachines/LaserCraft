namespace netDxf.Entities
{
    using System;

    [Flags]
    internal enum DimensionTypeFlags
    {
        Linear = 0,
        Aligned = 1,
        Angular = 2,
        Diameter = 3,
        Radius = 4,
        Angular3Point = 5,
        Ordinate = 6,
        BlockReference = 0x20,
        OrdinteType = 0x40,
        UserTextPosition = 0x80
    }
}

