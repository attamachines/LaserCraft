namespace netDxf.Units
{
    using netDxf;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public static class LinearUnitFormat
    {
        private static string DecimalNumberFormat(UnitStyleFormat format)
        {
            char[] chArray = new char[format.LinearDecimalPlaces + 2];
            if (format.SupressLinearLeadingZeros)
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
                if (format.SupressLinearTrailingZeros)
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

        private static void GetFraction(double number, int precision, out int numerator, out int denominator)
        {
            numerator = Convert.ToInt32((double) ((number - ((int) number)) * precision));
            int gCD = GetGCD(numerator, precision);
            if (gCD <= 0)
            {
                gCD = 1;
            }
            numerator /= gCD;
            denominator = precision / gCD;
        }

        private static int GetGCD(int number1, int number2)
        {
            int num3;
            int num = number1;
            for (int i = number2; i > 0; i = num3)
            {
                num3 = num % i;
                num = i;
            }
            return num;
        }

        public static string ToArchitectural(double length, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            int num = (int) (length / 12.0);
            double number = length - (12 * num);
            int num3 = (int) number;
            if (MathHelper.IsZero(number))
            {
                if (num == 0)
                {
                    if (format.SupressZeroFeet)
                    {
                        return ("0" + format.InchesSymbol);
                    }
                    if (format.SupressZeroInches)
                    {
                        return ("0" + format.FeetSymbol);
                    }
                    string[] textArray1 = new string[] { "0", format.FeetSymbol, format.FeetInchesSeparator, "0", format.InchesSymbol };
                    return string.Concat(textArray1);
                }
                if (format.SupressZeroInches)
                {
                    return string.Format("{0}" + format.FeetSymbol, num);
                }
                string[] textArray2 = new string[] { "{0}", format.FeetSymbol, format.FeetInchesSeparator, "0", format.InchesSymbol };
                return string.Format(string.Concat(textArray2), num);
            }
            GetFraction(number, (short) Math.Pow(2.0, (double) format.LinearDecimalPlaces), out int num4, out int num5);
            if (num4 == 0)
            {
                if (num3 == 0)
                {
                    if (num == 0)
                    {
                        if (format.SupressZeroFeet)
                        {
                            return ("0" + format.InchesSymbol);
                        }
                        if (format.SupressZeroInches)
                        {
                            return ("0" + format.FeetSymbol);
                        }
                        string[] textArray3 = new string[] { "0", format.FeetSymbol, format.FeetInchesSeparator, "0", format.InchesSymbol };
                        return string.Concat(textArray3);
                    }
                    if (format.SupressZeroInches)
                    {
                        return string.Format("{0}" + format.FeetSymbol, num);
                    }
                    string[] textArray4 = new string[] { "{0}", format.FeetSymbol, format.FeetInchesSeparator, "0", format.InchesSymbol };
                    return string.Format(string.Concat(textArray4), num);
                }
                if (num == 0)
                {
                    if (format.SupressZeroFeet)
                    {
                        return string.Format("{0}" + format.InchesSymbol, num3);
                    }
                    string[] textArray5 = new string[] { "0", format.FeetSymbol, format.FeetInchesSeparator, "{0}", format.InchesSymbol };
                    return string.Format(string.Concat(textArray5), num3);
                }
                string[] textArray6 = new string[] { "{0}", format.FeetSymbol, format.FeetInchesSeparator, "{0}", format.InchesSymbol };
                return string.Format(string.Concat(textArray6), num, num3);
            }
            string str = string.Empty;
            string str2 = num + format.FeetSymbol + format.FeetInchesSeparator;
            if (format.SupressZeroFeet && (num == 0))
            {
                str2 = string.Empty;
            }
            switch (format.FractionType)
            {
                case FractionFormatType.Horizontal:
                {
                    object[] objArray2 = new object[] { @"\A1;", str2, num3, @"{\H1.0x;\S", num4, "/", num5, ";}", format.InchesSymbol };
                    return string.Concat(objArray2);
                }
                case FractionFormatType.Diagonal:
                {
                    object[] objArray1 = new object[] { @"\A1;", str2, num3, @"{\H1.0x;\S", num4, "#", num5, ";}", format.InchesSymbol };
                    return string.Concat(objArray1);
                }
                case FractionFormatType.NotStacked:
                {
                    object[] objArray3 = new object[] { str2, num3, " ", num4, "/", num5, format.InchesSymbol };
                    return string.Concat(objArray3);
                }
            }
            return str;
        }

        public static string ToDecimal(double length, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            return length.ToString(DecimalNumberFormat(format), provider);
        }

        public static string ToEngineering(double length, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            int num = (int) (length / 12.0);
            double number = length - (12 * num);
            if (MathHelper.IsZero(number))
            {
                if (num == 0)
                {
                    if (format.SupressZeroFeet)
                    {
                        return ("0" + format.InchesSymbol);
                    }
                    if (format.SupressZeroInches)
                    {
                        return ("0" + format.FeetSymbol);
                    }
                    string[] textArray1 = new string[] { "0", format.FeetSymbol, format.FeetInchesSeparator, "0", format.InchesSymbol };
                    return string.Concat(textArray1);
                }
                if (format.SupressZeroInches)
                {
                    return string.Format("{0}" + format.FeetSymbol, num);
                }
                string[] textArray2 = new string[] { "{0}", format.FeetSymbol, format.FeetInchesSeparator, "0", format.InchesSymbol };
                return string.Format(string.Concat(textArray2), num);
            }
            string str = num + format.FeetSymbol + format.FeetInchesSeparator;
            if (format.SupressZeroFeet && (num == 0))
            {
                str = string.Empty;
            }
            object[] args = new object[] { number.ToString(DecimalNumberFormat(format), provider) };
            return string.Format(provider, str + "{0}" + format.InchesSymbol, args);
        }

        public static string ToFractional(double length, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            int num = (int) length;
            GetFraction(length, (short) Math.Pow(2.0, (double) format.LinearDecimalPlaces), out int num2, out int num3);
            if (num2 == 0)
            {
                return $"{((int) length)}";
            }
            string str = string.Empty;
            switch (format.FractionType)
            {
                case FractionFormatType.Horizontal:
                {
                    object[] objArray2 = new object[] { @"\A1;", num, @"{\H1.0x;\S", num2, "/", num3, ";}" };
                    return string.Concat(objArray2);
                }
                case FractionFormatType.Diagonal:
                {
                    object[] objArray1 = new object[] { @"\A1;", num, @"{\H1.0x;\S", num2, "#", num3, ";}" };
                    return string.Concat(objArray1);
                }
                case FractionFormatType.NotStacked:
                {
                    object[] objArray3 = new object[] { num, " ", num2, "/", num3 };
                    return string.Concat(objArray3);
                }
            }
            return str;
        }

        public static string ToScientific(double length, UnitStyleFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            NumberFormatInfo provider = new NumberFormatInfo {
                NumberDecimalSeparator = format.DecimalSeparator
            };
            return length.ToString(DecimalNumberFormat(format) + "E+00", provider);
        }
    }
}

