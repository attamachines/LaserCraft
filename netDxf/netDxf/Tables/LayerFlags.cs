namespace netDxf.Tables
{
    using System;

    [Flags]
    internal enum LayerFlags
    {
        None = 0,
        Frozen = 1,
        FrozenNewViewports = 2,
        Locked = 4,
        XrefDependent = 0x10,
        XrefResolved = 0x20,
        Referenced = 0x40
    }
}

