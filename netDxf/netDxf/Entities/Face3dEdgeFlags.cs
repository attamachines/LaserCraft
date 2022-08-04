namespace netDxf.Entities
{
    using System;

    [Flags]
    public enum Face3dEdgeFlags
    {
        Visibles = 0,
        First = 1,
        Second = 2,
        Third = 4,
        Fourth = 8
    }
}

