namespace netDxf.Header
{
    using netDxf;
    using System;

    public enum DxfVersion
    {
        [StringValue("Unknown")]
        Unknown = 0,
        [StringValue("AC1006")]
        AutoCad10 = 1,
        [StringValue("AC1009")]
        AutoCad12 = 2,
        [StringValue("AC1012")]
        AutoCad13 = 3,
        [StringValue("AC1014")]
        AutoCad14 = 4,
        [StringValue("AC1015")]
        AutoCad2000 = 5,
        [StringValue("AC1018")]
        AutoCad2004 = 6,
        [StringValue("AC1021")]
        AutoCad2007 = 7,
        [StringValue("AC1024")]
        AutoCad2010 = 8,
        [StringValue("AC1027")]
        AutoCad2013 = 9
    }
}

