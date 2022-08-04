namespace netDxf.Header
{
    using netDxf;
    using netDxf.Entities;
    using netDxf.Units;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class HeaderVariables
    {
        private readonly Dictionary<string, HeaderVariable> variables;

        public HeaderVariables()
        {
            Dictionary<string, HeaderVariable> dictionary1 = new Dictionary<string, HeaderVariable> {
                { 
                    "$ACADVER",
                    new HeaderVariable("$ACADVER", DxfVersion.AutoCad2000)
                },
                { 
                    "$DWGCODEPAGE",
                    new HeaderVariable("$DWGCODEPAGE", "ANSI_" + Encoding.Default.WindowsCodePage)
                },
                { 
                    "$LASTSAVEDBY",
                    new HeaderVariable("$LASTSAVEDBY", Environment.UserName)
                },
                { 
                    "$HANDSEED",
                    new HeaderVariable("$HANDSEED", "1")
                },
                { 
                    "$ANGBASE",
                    new HeaderVariable("$ANGBASE", 0.0)
                },
                { 
                    "$ANGDIR",
                    new HeaderVariable("$ANGDIR", AngleDirection.CCW)
                },
                { 
                    "$ATTMODE",
                    new HeaderVariable("$ATTMODE", netDxf.Header.AttMode.Normal)
                },
                { 
                    "$AUNITS",
                    new HeaderVariable("$AUNITS", AngleUnitType.DecimalDegrees)
                },
                { 
                    "$AUPREC",
                    new HeaderVariable("$AUPREC", (short) 0)
                },
                { 
                    "$CECOLOR",
                    new HeaderVariable("$CECOLOR", AciColor.ByLayer)
                },
                { 
                    "$CELTSCALE",
                    new HeaderVariable("$CELTSCALE", 1.0)
                },
                { 
                    "$CELTYPE",
                    new HeaderVariable("$CELTYPE", "ByLayer")
                },
                { 
                    "$CELWEIGHT",
                    new HeaderVariable("$CELWEIGHT", Lineweight.ByLayer)
                },
                { 
                    "$CLAYER",
                    new HeaderVariable("$CLAYER", "0")
                },
                { 
                    "$CMLJUST",
                    new HeaderVariable("$CMLJUST", MLineJustification.Top)
                },
                { 
                    "$CMLSCALE",
                    new HeaderVariable("$CMLSCALE", 20.0)
                },
                { 
                    "$CMLSTYLE",
                    new HeaderVariable("$CMLSTYLE", "Standard")
                },
                { 
                    "$DIMSTYLE",
                    new HeaderVariable("$DIMSTYLE", "Standard")
                },
                { 
                    "$TEXTSIZE",
                    new HeaderVariable("$TEXTSIZE", 2.5)
                },
                { 
                    "$TEXTSTYLE",
                    new HeaderVariable("$TEXTSTYLE", "Standard")
                },
                { 
                    "$LUNITS",
                    new HeaderVariable("$LUNITS", LinearUnitType.Decimal)
                },
                { 
                    "$LUPREC",
                    new HeaderVariable("$LUPREC", (short) 4)
                },
                { 
                    "$EXTNAMES",
                    new HeaderVariable("$EXTNAMES", true)
                },
                { 
                    "$INSBASE",
                    new HeaderVariable("$INSBASE", Vector3.Zero)
                },
                { 
                    "$INSUNITS",
                    new HeaderVariable("$INSUNITS", DrawingUnits.Unitless)
                },
                { 
                    "$LTSCALE",
                    new HeaderVariable("$LTSCALE", 1.0)
                },
                { 
                    "$LWDISPLAY",
                    new HeaderVariable("$LWDISPLAY", false)
                },
                { 
                    "$PDMODE",
                    new HeaderVariable("$PDMODE", PointShape.Dot)
                },
                { 
                    "$PDSIZE",
                    new HeaderVariable("$PDSIZE", 0.0)
                },
                { 
                    "$PLINEGEN",
                    new HeaderVariable("$PLINEGEN", (short) 0)
                },
                { 
                    "$PSLTSCALE",
                    new HeaderVariable("$PSLTSCALE", (short) 1)
                },
                { 
                    "$TDCREATE",
                    new HeaderVariable("$TDCREATE", DateTime.Now)
                },
                { 
                    "$TDUCREATE",
                    new HeaderVariable("$TDUCREATE", DateTime.UtcNow)
                },
                { 
                    "$TDUPDATE",
                    new HeaderVariable("$TDUPDATE", DateTime.Now)
                },
                { 
                    "$TDUUPDATE",
                    new HeaderVariable("$TDUUPDATE", DateTime.UtcNow)
                },
                { 
                    "$TDINDWG",
                    new HeaderVariable("$TDINDWG", new TimeSpan())
                }
            };
            this.variables = dictionary1;
        }

        public DxfVersion AcadVer
        {
            get => 
                ((DxfVersion) this.variables["$ACADVER"].Value);
            set
            {
                if (value < DxfVersion.AutoCad2000)
                {
                    throw new NotSupportedException("Only AutoCad2000 and newer dxf versions are supported.");
                }
                this.variables["$ACADVER"].Value = value;
            }
        }

        public string HandleSeed
        {
            get => 
                ((string) this.variables["$HANDSEED"].Value);
            internal set => 
                (this.variables["$HANDSEED"].Value = value);
        }

        public double Angbase
        {
            get => 
                ((double) this.variables["$ANGBASE"].Value);
            internal set => 
                (this.variables["$ANGBASE"].Value = value);
        }

        public AngleDirection Angdir
        {
            get => 
                ((AngleDirection) this.variables["$ANGDIR"].Value);
            internal set => 
                (this.variables["$ANGDIR"].Value = value);
        }

        public netDxf.Header.AttMode AttMode
        {
            get => 
                ((netDxf.Header.AttMode) this.variables["$ATTMODE"].Value);
            set => 
                (this.variables["$ATTMODE"].Value = value);
        }

        public AngleUnitType AUnits
        {
            get => 
                ((AngleUnitType) this.variables["$AUNITS"].Value);
            set => 
                (this.variables["$AUNITS"].Value = value);
        }

        public short AUprec
        {
            get => 
                ((short) this.variables["$AUPREC"].Value);
            set
            {
                if ((value < 0) || (value > 8))
                {
                    throw new ArgumentOutOfRangeException("value", "Valid values are integers from 0 to 8.");
                }
                this.variables["$AUPREC"].Value = value;
            }
        }

        public AciColor CeColor
        {
            get => 
                ((AciColor) this.variables["$CECOLOR"].Value);
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.variables["$CECOLOR"].Value = value;
            }
        }

        public double CeLtScale
        {
            get => 
                ((double) this.variables["$CELTSCALE"].Value);
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The current entity line type scale must be greater than zero.");
                }
                this.variables["$CELTSCALE"].Value = value;
            }
        }

        public Lineweight CeLweight
        {
            get => 
                ((Lineweight) this.variables["$CELWEIGHT"].Value);
            set => 
                (this.variables["$CELWEIGHT"].Value = value);
        }

        public string CeLtype
        {
            get => 
                ((string) this.variables["$CELTYPE"].Value);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "The current entity line type name should be at least one character long.");
                }
                this.variables["$CELTYPE"].Value = value;
            }
        }

        public string CLayer
        {
            get => 
                ((string) this.variables["$CLAYER"].Value);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "The current layer name should be at least one character long.");
                }
                this.variables["$CLAYER"].Value = value;
            }
        }

        public MLineJustification CMLJust
        {
            get => 
                ((MLineJustification) this.variables["$CMLJUST"].Value);
            set => 
                (this.variables["$CMLJUST"].Value = value);
        }

        public double CMLScale
        {
            get => 
                ((double) this.variables["$CMLSCALE"].Value);
            set => 
                (this.variables["$CMLSCALE"].Value = value);
        }

        public string CMLStyle
        {
            get => 
                ((string) this.variables["$CMLSTYLE"].Value);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "The current multiline style name should be at least one character long.");
                }
                this.variables["$CMLSTYLE"].Value = value;
            }
        }

        public string DimStyle
        {
            get => 
                ((string) this.variables["$DIMSTYLE"].Value);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "The current dimension style name should be at least one character long.");
                }
                this.variables["$DIMSTYLE"].Value = value;
            }
        }

        public double TextSize
        {
            get => 
                ((double) this.variables["$TEXTSIZE"].Value);
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The default text height must be greater than zero.");
                }
                this.variables["$TEXTSIZE"].Value = value;
            }
        }

        public string TextStyle
        {
            get => 
                ((string) this.variables["$TEXTSTYLE"].Value);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "The current text style name should be at least one character long.");
                }
                this.variables["$TEXTSTYLE"].Value = value;
            }
        }

        public LinearUnitType LUnits
        {
            get => 
                ((LinearUnitType) this.variables["$LUNITS"].Value);
            set
            {
                if ((value == LinearUnitType.Architectural) || (value == LinearUnitType.Engineering))
                {
                    this.InsUnits = DrawingUnits.Inches;
                }
                this.variables["$LUNITS"].Value = value;
            }
        }

        public short LUprec
        {
            get => 
                ((short) this.variables["$LUPREC"].Value);
            set
            {
                if ((value < 0) || (value > 8))
                {
                    throw new ArgumentOutOfRangeException("value", "Valid values are integers from 0 to 8.");
                }
                this.variables["$LUPREC"].Value = value;
            }
        }

        public string DwgCodePage
        {
            get => 
                ((string) this.variables["$DWGCODEPAGE"].Value);
            internal set => 
                (this.variables["$DWGCODEPAGE"].Value = value);
        }

        public bool Extnames
        {
            get => 
                ((bool) this.variables["$EXTNAMES"].Value);
            internal set => 
                (this.variables["$EXTNAMES"].Value = value);
        }

        public Vector3 InsBase
        {
            get => 
                ((Vector3) this.variables["$INSBASE"].Value);
            set => 
                (this.variables["$INSBASE"].Value = value);
        }

        public DrawingUnits InsUnits
        {
            get => 
                ((DrawingUnits) this.variables["$INSUNITS"].Value);
            set => 
                (this.variables["$INSUNITS"].Value = value);
        }

        public string LastSavedBy
        {
            get => 
                ((string) this.variables["$LASTSAVEDBY"].Value);
            set => 
                (this.variables["$LASTSAVEDBY"].Value = value);
        }

        public double LtScale
        {
            get => 
                ((double) this.variables["$LTSCALE"].Value);
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The global line type scale must be greater than zero.");
                }
                this.variables["$LTSCALE"].Value = value;
            }
        }

        public bool LwDisplay
        {
            get => 
                ((bool) this.variables["$LWDISPLAY"].Value);
            set => 
                (this.variables["$LWDISPLAY"].Value = value);
        }

        public PointShape PdMode
        {
            get => 
                ((PointShape) this.variables["$PDMODE"].Value);
            set => 
                (this.variables["$PDMODE"].Value = value);
        }

        public double PdSize
        {
            get => 
                ((double) this.variables["$PDSIZE"].Value);
            set => 
                (this.variables["$PDSIZE"].Value = value);
        }

        public short PLineGen
        {
            get => 
                ((short) this.variables["$PLINEGEN"].Value);
            set
            {
                if ((value != 0) && (value != 1))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted values are 0 or 1.");
                }
                this.variables["$PLINEGEN"].Value = value;
            }
        }

        public short PsLtScale
        {
            get => 
                ((short) this.variables["$PSLTSCALE"].Value);
            set
            {
                if ((value != 0) && (value != 1))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted values are 0 or 1.");
                }
                this.variables["$PSLTSCALE"].Value = value;
            }
        }

        public DateTime TdCreate
        {
            get => 
                ((DateTime) this.variables["$TDCREATE"].Value);
            set => 
                (this.variables["$TDCREATE"].Value = value);
        }

        public DateTime TduCreate
        {
            get => 
                ((DateTime) this.variables["$TDUCREATE"].Value);
            set => 
                (this.variables["$TDUCREATE"].Value = value);
        }

        public DateTime TdUpdate
        {
            get => 
                ((DateTime) this.variables["$TDUPDATE"].Value);
            set => 
                (this.variables["$TDUPDATE"].Value = value);
        }

        public DateTime TduUpdate
        {
            get => 
                ((DateTime) this.variables["$TDUUPDATE"].Value);
            set => 
                (this.variables["$TDUUPDATE"].Value = value);
        }

        public TimeSpan TdinDwg
        {
            get => 
                ((TimeSpan) this.variables["$TDINDWG"].Value);
            set => 
                (this.variables["$TDINDWG"].Value = value);
        }

        internal ICollection<HeaderVariable> Values =>
            this.variables.Values;
    }
}

