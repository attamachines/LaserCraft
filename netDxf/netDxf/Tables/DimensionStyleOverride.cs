namespace netDxf.Tables
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Units;
    using System;

    public class DimensionStyleOverride
    {
        private readonly DimensionStyleOverrideType type;
        private readonly object value;

        public DimensionStyleOverride(DimensionStyleOverrideType type, object value)
        {
            switch (type)
            {
                case DimensionStyleOverrideType.DimLineColor:
                    if (!(value is AciColor))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(AciColor)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimLineLinetype:
                    if (!(value is Linetype))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Linetype)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimLineLineweight:
                    if (!(value is Lineweight))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Lineweight)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimLineOff:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimLineExtend:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The DimensionStyleOverrideType.{type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLineColor:
                    if (!(value is AciColor))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(AciColor)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLine1Linetype:
                    if (!(value is Linetype))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Linetype)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLine2Linetype:
                    if (!(value is Linetype))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Linetype)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLineLineweight:
                    if (!(value is Lineweight))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Lineweight)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLine1Off:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLine2Off:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLineOffset:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The DimensionStyleOverrideType.{type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.ExtLineExtend:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The DimensionStyleOverrideType.{type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.ArrowSize:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The DimensionStyleOverrideType.{type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.CenterMarkSize:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.LeaderArrow:
                    if (value != null)
                    {
                        if (!(value is Block))
                        {
                            throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Block)}", "value");
                        }
                        break;
                    }
                    break;

                case DimensionStyleOverrideType.DimArrow1:
                    if (value != null)
                    {
                        if (!(value is Block))
                        {
                            throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Block)}", "value");
                        }
                        break;
                    }
                    break;

                case DimensionStyleOverrideType.DimArrow2:
                    if (value != null)
                    {
                        if (!(value is Block))
                        {
                            throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(Block)}", "value");
                        }
                        break;
                    }
                    break;

                case DimensionStyleOverrideType.TextStyle:
                    if (!(value is TextStyle))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(TextStyle)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.TextColor:
                    if (!(value is AciColor))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(AciColor)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.TextHeight:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) <= 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The {type} dimension style override must be greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.TextOffset:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The {type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.DimScaleOverall:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The DimensionStyleOverrideType.{type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.AngularPrecision:
                    if (!(value is short))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(short)}", "value");
                    }
                    if (((short) value) < -1)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The {type} dimension style override must be greater than -1.");
                    }
                    break;

                case DimensionStyleOverrideType.LengthPrecision:
                    if (!(value is short))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(short)}", "value");
                    }
                    if (((short) value) < 0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The {type} dimension style override must be equals or greater than zero.");
                    }
                    break;

                case DimensionStyleOverrideType.DimPrefix:
                    if (!(value is string))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(string)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimSuffix:
                    if (!(value is string))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(string)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DecimalSeparator:
                    if (!(value is char))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(char)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimScaleLinear:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (MathHelper.IsZero((double) value))
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The DimensionStyleOverrideType.{type} dimension style override cannot be zero.");
                    }
                    break;

                case DimensionStyleOverrideType.DimLengthUnits:
                    if (!(value is LinearUnitType))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(LinearUnitType)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimAngularUnits:
                    if (!(value is AngleUnitType))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(AngleUnitType)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.FractionalType:
                    if (!(value is FractionFormatType))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(FractionFormatType)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.SuppressLinearLeadingZeros:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.SuppressLinearTrailingZeros:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.SuppressAngularLeadingZeros:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.SuppressAngularTrailingZeros:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.SuppressZeroFeet:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.SuppressZeroInches:
                    if (!(value is bool))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(bool)}", "value");
                    }
                    break;

                case DimensionStyleOverrideType.DimRoundoff:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The DimensionStyleOverrideType.{type} dimension style override must be a valid {typeof(double)}", "value");
                    }
                    if (((double) value) < 0.0)
                    {
                        throw new ArgumentOutOfRangeException("value", value, $"The {type} dimension style override must be equals or greater than zero.");
                    }
                    break;
            }
            this.type = type;
            this.value = value;
        }

        public override string ToString() => 
            $"{this.type} : {this.value}";

        public DimensionStyleOverrideType Type =>
            this.type;

        public object Value =>
            this.value;
    }
}

