namespace netDxf
{
    using System;
    using System.Globalization;

    public class XDataRecord
    {
        private readonly XDataCode code;
        private readonly object value;

        public XDataRecord(XDataCode code, object value)
        {
            switch (code)
            {
                case XDataCode.String:
                case XDataCode.LayerName:
                    if (!(value is string))
                    {
                        throw new ArgumentException($"The value of a XDataCode.{code} must be a {typeof(string)}.", "value");
                    }
                    break;

                case XDataCode.AppReg:
                    throw new ArgumentException("An application registry cannot be an extended data record.", "value");

                case XDataCode.ControlString:
                {
                    string str = value as string;
                    if (string.IsNullOrEmpty(str))
                    {
                        throw new ArgumentException("The value of a XDataCode.ControlString must be a string.", "value");
                    }
                    if (!string.Equals(str, "{") && !string.Equals(str, "}"))
                    {
                        throw new ArgumentException("The only valid values of a XDataCode.ControlString are { or }.", "value");
                    }
                    break;
                }
                case XDataCode.BinaryData:
                    if (!(value is byte[]))
                    {
                        throw new ArgumentException("The value of a XDataCode.BinaryData must be a byte array.", "value");
                    }
                    break;

                case XDataCode.DatabaseHandle:
                    if (!(value is string))
                    {
                        throw new ArgumentException("The value of a XDataCode.DatabaseHandle must be an hexadecimal number.", "value");
                    }
                    if (!long.TryParse((string) value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long num))
                    {
                        throw new ArgumentException("The value of a XDataCode.DatabaseHandle must be an hexadecimal number.", "value");
                    }
                    value = num.ToString("X");
                    break;

                case XDataCode.RealX:
                case XDataCode.WorldSpacePositionX:
                case XDataCode.WorldSpaceDisplacementX:
                case XDataCode.WorldDirectionX:
                case XDataCode.RealY:
                case XDataCode.WorldSpacePositionY:
                case XDataCode.WorldSpaceDisplacementY:
                case XDataCode.WorldDirectionY:
                case XDataCode.RealZ:
                case XDataCode.WorldSpacePositionZ:
                case XDataCode.WorldSpaceDisplacementZ:
                case XDataCode.WorldDirectionZ:
                case XDataCode.Real:
                case XDataCode.Distance:
                case XDataCode.ScaleFactor:
                    if (!(value is double))
                    {
                        throw new ArgumentException($"The value of a XDataCode.{code} must be a {typeof(double)}.", "value");
                    }
                    break;

                case XDataCode.Int16:
                    if (!(value is short))
                    {
                        throw new ArgumentException($"The value of a XDataCode.{code} must be a {typeof(short)}.", "value");
                    }
                    break;

                case XDataCode.Int32:
                    if (!(value is int))
                    {
                        throw new ArgumentException($"The value of a XDataCode.{code} must be an {typeof(int)}.", "value");
                    }
                    break;
            }
            this.code = code;
            this.value = value;
        }

        public override string ToString() => 
            $"{this.code} - {this.value}";

        public static XDataRecord OpenControlString =>
            new XDataRecord(XDataCode.ControlString, "{");

        public static XDataRecord CloseControlString =>
            new XDataRecord(XDataCode.ControlString, "}");

        public XDataCode Code =>
            this.code;

        public object Value =>
            this.value;
    }
}

