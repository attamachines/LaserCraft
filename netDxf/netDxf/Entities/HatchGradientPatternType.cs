namespace netDxf.Entities
{
    using netDxf;
    using System;

    public enum HatchGradientPatternType
    {
        [StringValue("LINEAR")]
        Linear = 0,
        [StringValue("CYLINDER")]
        Cylinder = 1,
        [StringValue("INVCYLINDER")]
        InvCylinder = 2,
        [StringValue("SPHERICAL")]
        Spherical = 3,
        [StringValue("INVSPHERICAL")]
        InvSpherical = 4,
        [StringValue("HEMISPHERICAL")]
        Hemispherical = 5,
        [StringValue("INVHEMISPHERICAL")]
        InvHemispherical = 6,
        [StringValue("CURVED")]
        Curved = 7,
        [StringValue("INVCURVED")]
        InvCurved = 8
    }
}

