namespace netDxf.Units
{
    using System;

    public class UnitStyleFormat
    {
        private short linearDecimalPlaces = 2;
        private short angularDecimalPlaces = 0;
        private string decimalSeparator = ".";
        private string feetInchesSeparator = "-";
        private string degreesSymbol = "\x00b0";
        private string minutesSymbol = "'";
        private string secondsSymbol = "\"";
        private string radiansSymbol = "r";
        private string gradiansSymbol = "g";
        private string feetSymbol = "'";
        private string inchesSymbol = "\"";
        private FractionFormatType fractionType = FractionFormatType.Horizontal;
        private bool supressLinearLeadingZeros = false;
        private bool supressLinearTrailingZeros = false;
        private bool supressAngularLeadingZeros = false;
        private bool supressAngularTrailingZeros = false;
        private bool supressZeroFeet = true;
        private bool supressZeroInches = true;

        public short LinearDecimalPlaces
        {
            get => 
                this.linearDecimalPlaces;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The number of decimal places must be equals or greater than zero.");
                }
                this.linearDecimalPlaces = value;
            }
        }

        public short AngularDecimalPlaces
        {
            get => 
                this.angularDecimalPlaces;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The number of decimal places must be equals or greater than zero.");
                }
                this.angularDecimalPlaces = value;
            }
        }

        public string DecimalSeparator
        {
            get => 
                this.decimalSeparator;
            set => 
                (this.decimalSeparator = value);
        }

        public string FeetInchesSeparator
        {
            get => 
                this.feetInchesSeparator;
            set => 
                (this.feetInchesSeparator = value);
        }

        public string DegreesSymbol
        {
            get => 
                this.degreesSymbol;
            set => 
                (this.degreesSymbol = value);
        }

        public string MinutesSymbol
        {
            get => 
                this.minutesSymbol;
            set => 
                (this.minutesSymbol = value);
        }

        public string SecondsSymbol
        {
            get => 
                this.secondsSymbol;
            set => 
                (this.secondsSymbol = value);
        }

        public string RadiansSymbol
        {
            get => 
                this.radiansSymbol;
            set => 
                (this.radiansSymbol = value);
        }

        public string GradiansSymbol
        {
            get => 
                this.gradiansSymbol;
            set => 
                (this.gradiansSymbol = value);
        }

        public string FeetSymbol
        {
            get => 
                this.feetSymbol;
            set => 
                (this.feetSymbol = value);
        }

        public string InchesSymbol
        {
            get => 
                this.inchesSymbol;
            set => 
                (this.inchesSymbol = value);
        }

        public FractionFormatType FractionType
        {
            get => 
                this.fractionType;
            set => 
                (this.fractionType = value);
        }

        public bool SupressLinearLeadingZeros
        {
            get => 
                this.supressLinearLeadingZeros;
            set => 
                (this.supressLinearLeadingZeros = value);
        }

        public bool SupressLinearTrailingZeros
        {
            get => 
                this.supressLinearTrailingZeros;
            set => 
                (this.supressLinearTrailingZeros = value);
        }

        public bool SupressAngularLeadingZeros
        {
            get => 
                this.supressAngularLeadingZeros;
            set => 
                (this.supressAngularLeadingZeros = value);
        }

        public bool SupressAngularTrailingZeros
        {
            get => 
                this.supressAngularTrailingZeros;
            set => 
                (this.supressAngularTrailingZeros = value);
        }

        public bool SupressZeroFeet
        {
            get => 
                this.supressZeroFeet;
            set => 
                (this.supressZeroFeet = value);
        }

        public bool SupressZeroInches
        {
            get => 
                this.supressZeroInches;
            set => 
                (this.supressZeroInches = value);
        }
    }
}

