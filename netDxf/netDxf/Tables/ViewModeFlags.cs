namespace netDxf.Tables
{
    using System;

    [Flags]
    public enum ViewModeFlags
    {
        Off = 0,
        Perspective = 1,
        FrontClippingPlane = 2,
        BackClippingPlane = 4,
        UCSFollow = 8,
        FrontClipNotAtEye = 0x10
    }
}

