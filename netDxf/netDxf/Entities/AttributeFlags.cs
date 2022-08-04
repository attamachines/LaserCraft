namespace netDxf.Entities
{
    using System;

    [Flags]
    public enum AttributeFlags
    {
        Visible = 0,
        Hidden = 1,
        Constant = 2,
        Verify = 4,
        Predefined = 8
    }
}

