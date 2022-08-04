namespace netDxf.Entities
{
    using System;

    [Flags]
    internal enum MLineFlags
    {
        Has = 1,
        Closed = 2,
        NoStartCaps = 4,
        NoEndCaps = 8
    }
}

