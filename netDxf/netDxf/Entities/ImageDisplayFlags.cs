namespace netDxf.Entities
{
    using System;

    [Flags]
    public enum ImageDisplayFlags
    {
        ShowImage = 1,
        ShowImageWhenNotAlignedWithScreen = 2,
        UseClippingBoundary = 4,
        TransparencyOn = 8
    }
}

