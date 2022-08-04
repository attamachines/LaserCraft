namespace netDxf.Entities
{
    using netDxf;
    using System;

    public class HatchGradientPattern : HatchPattern
    {
        private HatchGradientPatternType gradientType;
        private AciColor color1;
        private AciColor color2;
        private bool singleColor;
        private double tint;
        private bool centered;

        public HatchGradientPattern() : this(string.Empty)
        {
        }

        public HatchGradientPattern(string description) : base("SOLID", description)
        {
            this.color1 = AciColor.Blue;
            this.color2 = AciColor.Yellow;
            this.singleColor = false;
            this.gradientType = HatchGradientPatternType.Linear;
            this.tint = 1.0;
            this.centered = true;
        }

        public HatchGradientPattern(AciColor color1, AciColor color2, HatchGradientPatternType type) : this(color1, color2, type, string.Empty)
        {
        }

        public HatchGradientPattern(AciColor color, double tint, HatchGradientPatternType type) : this(color, tint, type, string.Empty)
        {
        }

        public HatchGradientPattern(AciColor color1, AciColor color2, HatchGradientPatternType type, string description) : base("SOLID", description)
        {
            if (color1 == null)
            {
                throw new ArgumentNullException("color1");
            }
            this.color1 = color1;
            if (color2 == null)
            {
                throw new ArgumentNullException("color2");
            }
            this.color2 = color2;
            this.singleColor = false;
            this.gradientType = type;
            this.tint = 1.0;
            this.centered = true;
        }

        public HatchGradientPattern(AciColor color, double tint, HatchGradientPatternType type, string description) : base("SOLID", description)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            this.color1 = color;
            this.color2 = this.Color2FromTint(tint);
            this.singleColor = true;
            this.gradientType = type;
            this.tint = tint;
            this.centered = true;
        }

        public override object Clone() => 
            new HatchGradientPattern { 
                Fill = base.Fill,
                Type = base.Type,
                Origin = base.Origin,
                Angle = base.Angle,
                Scale = base.Scale,
                Style = base.Style,
                GradientType = this.gradientType,
                Color1 = (AciColor) this.color1.Clone(),
                Color2 = (AciColor) this.color2.Clone(),
                SingleColor = this.singleColor,
                Tint = this.tint,
                Centered = this.centered
            };

        private AciColor Color2FromTint(double value)
        {
            AciColor.ToHsl(this.color1, out double num, out double num2, out _);
            return AciColor.FromHsl(num, num2, value);
        }

        public HatchGradientPatternType GradientType
        {
            get => 
                this.gradientType;
            set => 
                (this.gradientType = value);
        }

        public AciColor Color1
        {
            get => 
                this.color1;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.color1 = value;
            }
        }

        public AciColor Color2
        {
            get => 
                this.color2;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.singleColor = false;
                this.color2 = value;
            }
        }

        public bool SingleColor
        {
            get => 
                this.singleColor;
            set
            {
                if (value)
                {
                    this.Color2 = this.Color2FromTint(this.tint);
                }
                this.singleColor = value;
            }
        }

        public double Tint
        {
            get => 
                this.tint;
            set
            {
                if (this.singleColor)
                {
                    this.Color2 = this.Color2FromTint(value);
                }
                this.tint = value;
            }
        }

        public bool Centered
        {
            get => 
                this.centered;
            set => 
                (this.centered = value);
        }
    }
}

