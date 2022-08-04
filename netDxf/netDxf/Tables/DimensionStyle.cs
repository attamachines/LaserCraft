namespace netDxf.Tables
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Units;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class DimensionStyle : TableObject
    {
        private AciColor dimclrd;
        private Linetype dimltype;
        private Lineweight dimlwd;
        private bool dimsd;
        private double dimdle;
        private double dimdli;
        private AciColor dimclre;
        private Linetype dimltex1;
        private Linetype dimltex2;
        private Lineweight dimlwe;
        private bool dimse1;
        private bool dimse2;
        private double dimexo;
        private double dimexe;
        private double dimasz;
        private double dimcen;
        private Block dimldrblk;
        private Block dimblk1;
        private Block dimblk2;
        private netDxf.Tables.TextStyle dimtxsty;
        private AciColor dimclrt;
        private double dimtxt;
        private short dimjust;
        private short dimtad;
        private double dimgap;
        private double dimscale;
        private short dimtih;
        private short dimtoh;
        private short dimadec;
        private short dimdec;
        private string dimPrefix;
        private string dimSuffix;
        private char dimdsep;
        private double dimlfac;
        private LinearUnitType dimlunit;
        private AngleUnitType dimaunit;
        private FractionFormatType dimfrac;
        private bool suppressLinearLeadingZeros;
        private bool suppressLinearTrailingZeros;
        private bool suppressAngularLeadingZeros;
        private bool suppressAngularTrailingZeros;
        private bool suppressZeroFeet;
        private bool suppressZeroInches;
        private double dimrnd;
        public const string DefaultName = "Standard";

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event BlockChangedEventHandler BlockChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LinetypeChangedEventHandler LinetypeChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event TextStyleChangedEventHandler TextStyleChanged;

        public DimensionStyle(string name) : this(name, true)
        {
        }

        internal DimensionStyle(string name, bool checkName) : base(name, "DIMSTYLE", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The dimension style name should be at least one character long.");
            }
            base.IsReserved = name.Equals("Standard", StringComparison.OrdinalIgnoreCase);
            this.dimclrd = AciColor.ByBlock;
            this.dimltype = Linetype.ByBlock;
            this.dimlwd = Lineweight.ByBlock;
            this.dimsd = false;
            this.dimdli = 0.38;
            this.dimdle = 0.0;
            this.dimclre = AciColor.ByBlock;
            this.dimltex1 = Linetype.ByBlock;
            this.dimltex2 = Linetype.ByBlock;
            this.dimlwe = Lineweight.ByBlock;
            this.dimse1 = false;
            this.dimse2 = false;
            this.dimexo = 0.0625;
            this.dimexe = 0.18;
            this.dimasz = 0.18;
            this.dimcen = 0.09;
            this.dimldrblk = null;
            this.dimblk1 = null;
            this.dimblk2 = null;
            this.dimtxsty = netDxf.Tables.TextStyle.Default;
            this.dimclrt = AciColor.ByBlock;
            this.dimtxt = 0.18;
            this.dimtad = 1;
            this.dimjust = 0;
            this.dimgap = 0.09;
            this.dimscale = 1.0;
            this.dimdec = 2;
            this.dimadec = 0;
            this.dimPrefix = string.Empty;
            this.dimSuffix = string.Empty;
            this.dimtih = 0;
            this.dimtoh = 0;
            this.dimdsep = '.';
            this.dimlfac = 1.0;
            this.dimaunit = AngleUnitType.DecimalDegrees;
            this.dimlunit = LinearUnitType.Decimal;
            this.dimfrac = FractionFormatType.Horizontal;
            this.suppressLinearLeadingZeros = false;
            this.suppressLinearTrailingZeros = false;
            this.suppressAngularLeadingZeros = false;
            this.suppressAngularTrailingZeros = false;
            this.suppressZeroFeet = true;
            this.suppressZeroInches = true;
            this.dimrnd = 0.0;
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName)
        {
            DimensionStyle style = new DimensionStyle(newName) {
                DimLineColor = (AciColor) this.dimclrd.Clone(),
                DimLineLinetype = (Linetype) this.dimltype.Clone(),
                DimLineLineweight = this.dimlwd,
                DimLineOff = this.dimsd,
                DimBaselineSpacing = this.dimdli,
                DimLineExtend = this.dimdle,
                ExtLineColor = (AciColor) this.dimclre.Clone(),
                ExtLine1Linetype = (Linetype) this.dimltex1.Clone(),
                ExtLine2Linetype = (Linetype) this.dimltex2.Clone(),
                ExtLineLineweight = this.dimlwe,
                ExtLine1Off = this.dimse1,
                ExtLine2Off = this.dimse2,
                ExtLineOffset = this.dimexo,
                ExtLineExtend = this.dimexe,
                ArrowSize = this.dimasz,
                CenterMarkSize = this.dimcen,
                DimScaleOverall = this.dimscale,
                DIMTIH = this.dimtih,
                DIMTOH = this.dimtoh,
                TextStyle = (netDxf.Tables.TextStyle) this.dimtxsty.Clone(),
                TextColor = (AciColor) this.dimclrt.Clone(),
                TextHeight = this.dimtxt,
                DIMJUST = this.dimjust,
                DIMTAD = this.dimtad,
                TextOffset = this.dimgap,
                AngularPrecision = this.dimadec,
                LengthPrecision = this.dimdec,
                DimPrefix = this.dimPrefix,
                DimSuffix = this.dimSuffix,
                DecimalSeparator = this.dimdsep,
                DimAngularUnits = this.dimaunit
            };
            if (this.dimldrblk > null)
            {
                style.LeaderArrow = (Block) this.dimldrblk.Clone();
            }
            if (this.dimblk1 > null)
            {
                style.DimArrow1 = (Block) this.dimblk1.Clone();
            }
            if (this.dimblk2 > null)
            {
                style.DimArrow2 = (Block) this.dimblk2.Clone();
            }
            return style;
        }

        protected virtual Block OnBlockChangedEvent(Block oldBlock, Block newBlock)
        {
            BlockChangedEventHandler blockChanged = this.BlockChanged;
            if (blockChanged > null)
            {
                TableObjectChangedEventArgs<Block> e = new TableObjectChangedEventArgs<Block>(oldBlock, newBlock);
                blockChanged(this, e);
                return e.NewValue;
            }
            return newBlock;
        }

        protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            LinetypeChangedEventHandler linetypeChanged = this.LinetypeChanged;
            if (linetypeChanged > null)
            {
                TableObjectChangedEventArgs<Linetype> e = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
                linetypeChanged(this, e);
                return e.NewValue;
            }
            return newLinetype;
        }

        protected virtual netDxf.Tables.TextStyle OnTextStyleChangedEvent(netDxf.Tables.TextStyle oldTextStyle, netDxf.Tables.TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler textStyleChanged = this.TextStyleChanged;
            if (textStyleChanged > null)
            {
                TableObjectChangedEventArgs<netDxf.Tables.TextStyle> e = new TableObjectChangedEventArgs<netDxf.Tables.TextStyle>(oldTextStyle, newTextStyle);
                textStyleChanged(this, e);
                return e.NewValue;
            }
            return newTextStyle;
        }

        public static DimensionStyle Default =>
            new DimensionStyle("Standard");

        public AciColor DimLineColor
        {
            get => 
                this.dimclrd;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimclrd = value;
            }
        }

        public Linetype DimLineLinetype
        {
            get => 
                this.dimltype;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimltype = this.OnLinetypeChangedEvent(this.dimltype, value);
            }
        }

        public Lineweight DimLineLineweight
        {
            get => 
                this.dimlwd;
            set => 
                (this.dimlwd = value);
        }

        public bool DimLineOff
        {
            get => 
                this.dimsd;
            set => 
                (this.dimsd = value);
        }

        public double DimLineExtend
        {
            get => 
                this.dimdle;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMDLE must be equals or greater than zero.");
                }
                this.dimdle = value;
            }
        }

        public double DimBaselineSpacing
        {
            get => 
                this.dimdli;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMDLI must be equals or greater than zero.");
                }
                this.dimdli = value;
            }
        }

        public AciColor ExtLineColor
        {
            get => 
                this.dimclre;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimclre = value;
            }
        }

        public Linetype ExtLine1Linetype
        {
            get => 
                this.dimltex1;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimltex1 = this.OnLinetypeChangedEvent(this.dimltex1, value);
            }
        }

        public Linetype ExtLine2Linetype
        {
            get => 
                this.dimltex2;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimltex2 = this.OnLinetypeChangedEvent(this.dimltex2, value);
            }
        }

        public Lineweight ExtLineLineweight
        {
            get => 
                this.dimlwe;
            set => 
                (this.dimlwe = value);
        }

        public bool ExtLine1Off
        {
            get => 
                this.dimse1;
            set => 
                (this.dimse1 = value);
        }

        public bool ExtLine2Off
        {
            get => 
                this.dimse2;
            set => 
                (this.dimse2 = value);
        }

        public double ExtLineOffset
        {
            get => 
                this.dimexo;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMEXO must be equals or greater than zero.");
                }
                this.dimexo = value;
            }
        }

        public double ExtLineExtend
        {
            get => 
                this.dimexe;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMEXE must be equals or greater than zero.");
                }
                this.dimexe = value;
            }
        }

        public double ArrowSize
        {
            get => 
                this.dimasz;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMASZ must be equals or greater than zero.");
                }
                this.dimasz = value;
            }
        }

        public double CenterMarkSize
        {
            get => 
                this.dimcen;
            set => 
                (this.dimcen = value);
        }

        public Block LeaderArrow
        {
            get => 
                this.dimldrblk;
            set => 
                (this.dimldrblk = (value == null) ? null : this.OnBlockChangedEvent(this.dimldrblk, value));
        }

        public Block DimArrow1
        {
            get => 
                this.dimblk1;
            set => 
                (this.dimblk1 = (value == null) ? null : this.OnBlockChangedEvent(this.dimblk1, value));
        }

        public Block DimArrow2
        {
            get => 
                this.dimblk2;
            set => 
                (this.dimblk2 = (value == null) ? null : this.OnBlockChangedEvent(this.dimblk2, value));
        }

        public netDxf.Tables.TextStyle TextStyle
        {
            get => 
                this.dimtxsty;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimtxsty = this.OnTextStyleChangedEvent(this.dimtxsty, value);
            }
        }

        public AciColor TextColor
        {
            get => 
                this.dimclrt;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.dimclrt = value;
            }
        }

        public double TextHeight
        {
            get => 
                this.dimtxt;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMTXT must be greater than zero.");
                }
                this.dimtxt = value;
            }
        }

        internal short DIMJUST
        {
            get => 
                this.dimjust;
            set => 
                (this.dimjust = value);
        }

        internal short DIMTAD
        {
            get => 
                this.dimtad;
            set => 
                (this.dimtad = value);
        }

        public double TextOffset
        {
            get => 
                this.dimgap;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMGAP must be equals or greater than zero.");
                }
                this.dimgap = value;
            }
        }

        public double DimScaleOverall
        {
            get => 
                this.dimscale;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMSCALE must be greater than zero.");
                }
                this.dimscale = value;
            }
        }

        internal short DIMTIH
        {
            get => 
                this.dimtih;
            set => 
                (this.dimtih = value);
        }

        internal short DIMTOH
        {
            get => 
                this.dimtoh;
            set => 
                (this.dimtoh = value);
        }

        public short AngularPrecision
        {
            get => 
                this.dimadec;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMADEC must be greater than -1.");
                }
                this.dimadec = value;
            }
        }

        public short LengthPrecision
        {
            get => 
                this.dimdec;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The DIMDEC must be equals or greater than zero.");
                }
                this.dimdec = value;
            }
        }

        public string DimPrefix
        {
            get => 
                this.dimPrefix;
            set => 
                (this.dimPrefix = value ?? string.Empty);
        }

        public string DimSuffix
        {
            get => 
                this.dimSuffix;
            set => 
                (this.dimSuffix = value ?? string.Empty);
        }

        public char DecimalSeparator
        {
            get => 
                this.dimdsep;
            set => 
                (this.dimdsep = value);
        }

        public double DimScaleLinear
        {
            get => 
                this.dimlfac;
            set
            {
                if (MathHelper.IsZero(value))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The scale factor cannot be zero.");
                }
                this.dimlfac = value;
            }
        }

        public LinearUnitType DimLengthUnits
        {
            get => 
                this.dimlunit;
            set => 
                (this.dimlunit = value);
        }

        public AngleUnitType DimAngularUnits
        {
            get => 
                this.dimaunit;
            set
            {
                if (value == AngleUnitType.SurveyorUnits)
                {
                    throw new ArgumentException("Surveyor's units are not applicable in angular dimensions.");
                }
                this.dimaunit = value;
            }
        }

        public FractionFormatType FractionalType
        {
            get => 
                this.dimfrac;
            set => 
                (this.dimfrac = value);
        }

        public bool SuppressLinearLeadingZeros
        {
            get => 
                this.suppressLinearLeadingZeros;
            set => 
                (this.suppressLinearLeadingZeros = value);
        }

        public bool SuppressLinearTrailingZeros
        {
            get => 
                this.suppressLinearTrailingZeros;
            set => 
                (this.suppressLinearTrailingZeros = value);
        }

        public bool SuppressAngularLeadingZeros
        {
            get => 
                this.suppressAngularLeadingZeros;
            set => 
                (this.suppressAngularLeadingZeros = value);
        }

        public bool SuppressAngularTrailingZeros
        {
            get => 
                this.suppressAngularTrailingZeros;
            set => 
                (this.suppressAngularTrailingZeros = value);
        }

        public bool SuppressZeroFeet
        {
            get => 
                this.suppressZeroFeet;
            set => 
                (this.suppressZeroFeet = value);
        }

        public bool SuppressZeroInches
        {
            get => 
                this.suppressZeroInches;
            set => 
                (this.suppressZeroInches = value);
        }

        public double DimRoundoff
        {
            get => 
                this.dimrnd;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The nearest value to round all distances must be equals or greater than zero.");
                }
                this.dimrnd = value;
            }
        }

        public DimensionStyles Owner
        {
            get => 
                ((DimensionStyles) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public delegate void BlockChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Block> e);

        public delegate void LinetypeChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Linetype> e);

        public delegate void TextStyleChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<TextStyle> e);
    }
}

