namespace netDxf.Units
{
    using System;
    using System.Globalization;

    public static class AngleUnitFormat
    {
        private static string DecimalNumberFormat(UnitStyleFormat format)
        {
            char[] chArray = new char[format.AngularDecimalPlaces + 2];
            if (format.SupressAngularLeadingZeros)
            {
                chArray[0] = '#';
            }
            else
            {
                chArray[0] = '0';
            }
            chArray[1] = '.';
            for (int i = 2; i < chArray.Length; i++)
            {
                if (format.SupressAngularTrailingZeros)
                {
                    chArray[i] = '#';
                }
                else
                {
                    chArray[i] = '0';
                }
            }
            return new string(chArray);
        }

        public static string ToDecimal(double angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            return (angle.ToString(DecimalNumberFormat(format), provider) + format.DegreesSymbol);
        }

        public static string ToDegreesMinutesSeconds(double angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            double num = angle;
            double num2 = (num - ((int) num)) * 60.0;
            double num3 = (num2 - ((int) num2)) * 60.0;
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            if (format.AngularDecimalPlaces == 0)
            {
                object[] objArray1 = new object[] { (int) Math.Round(num, 0) };
                return string.Format(provider, "{0}" + format.DegreesSymbol, objArray1);
            }
            if ((format.AngularDecimalPlaces == 1) || (format.AngularDecimalPlaces == 2))
            {
                object[] objArray2 = new object[] { (int) num, (int) Math.Round(num2, 0) };
                return string.Format(provider, "{0}" + format.DegreesSymbol + "{1}" + format.MinutesSymbol, objArray2);
            }
            if ((format.AngularDecimalPlaces == 3) || (format.AngularDecimalPlaces == 4))
            {
                string[] textArray1 = new string[] { "{0}", format.DegreesSymbol, "{1}", format.MinutesSymbol, "{2}", format.SecondsSymbol };
                object[] objArray3 = new object[] { (int) num, (int) num2, (int) Math.Round(num3, 0) };
                return string.Format(provider, string.Concat(textArray1), objArray3);
            }
            string str = "0." + new string('0', format.AngularDecimalPlaces - 4);
            string[] textArray2 = new string[] { "{0}", format.DegreesSymbol, "{1}", format.MinutesSymbol, "{2}", format.SecondsSymbol };
            object[] args = new object[] { (int) num, (int) num2, num3.ToString(str, provider) };
            return string.Format(provider, string.Concat(textArray2), args);
        }

        public static string ToGradians(double angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            double num = angle * 1.1111111111111112;
            return (num.ToString(DecimalNumberFormat(format), provider) + format.GradiansSymbol);
        }

        public static string ToRadians(double angle, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            double num = angle * 0.017453292519943295;
            return (num.ToString(DecimalNumberFormat(format), provider) + format.RadiansSymbol);
        }
    }
}

