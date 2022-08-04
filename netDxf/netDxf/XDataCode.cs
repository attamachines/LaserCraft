namespace netDxf
{
    using System;

    public enum XDataCode
    {
        String = 0x3e8,
        AppReg = 0x3e9,
        ControlString = 0x3ea,
        LayerName = 0x3eb,
        BinaryData = 0x3ec,
        DatabaseHandle = 0x3ed,
        RealX = 0x3f2,
        RealY = 0x3fc,
        RealZ = 0x406,
        WorldSpacePositionX = 0x3f3,
        WorldSpacePositionY = 0x3fd,
        WorldSpacePositionZ = 0x407,
        WorldSpaceDisplacementX = 0x3f4,
        WorldSpaceDisplacementY = 0x3fe,
        WorldSpaceDisplacementZ = 0x408,
        WorldDirectionX = 0x3f5,
        WorldDirectionY = 0x3ff,
        WorldDirectionZ = 0x409,
        Real = 0x410,
        Distance = 0x411,
        ScaleFactor = 0x412,
        Int16 = 0x42e,
        Int32 = 0x42f
    }
}

