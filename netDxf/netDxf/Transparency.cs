namespace netDxf
{
    using System;
    using System.Globalization;

    public class Transparency : ICloneable, IEquatable<Transparency>
    {
        private short value;

        public Transparency()
        {
            this.value = -1;
        }

        public Transparency(short value)
        {
            if ((value < 0) || (value > 90))
            {
                throw new ArgumentOutOfRangeException("value", value, "Accepted transparency values range from 0 to 90.");
            }
            this.value = value;
        }

        public object Clone() => 
            FromCadIndex(this.value);

        public bool Equals(Transparency other)
        {
            if (other == null)
            {
                return false;
            }
            return (other.value == this.value);
        }

        public static Transparency FromAlphaValue(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            short alpha = (short) (100.0 - ((((double) bytes[0]) / 255.0) * 100.0));
            return FromCadIndex(alpha);
        }

        private static Transparency FromCadIndex(short alpha)
        {
            if (alpha == -1)
            {
                return ByLayer;
            }
            if (alpha == 100)
            {
                return ByBlock;
            }
            return new Transparency(alpha);
        }

        public static int ToAlphaValue(Transparency transparency)
        {
            if (transparency == null)
            {
                throw new ArgumentNullException("transparency");
            }
            byte num = (byte) (((double) (0xff * (100 - transparency.Value))) / 100.0);
            byte[] buffer1 = new byte[4];
            buffer1[0] = num;
            buffer1[3] = 2;
            byte[] buffer = buffer1;
            if (transparency.IsByBlock)
            {
                buffer[3] = 1;
            }
            return BitConverter.ToInt32(buffer, 0);
        }

        public override string ToString()
        {
            if (this.value == -1)
            {
                return "ByLayer";
            }
            if (this.value == 100)
            {
                return "ByBlock";
            }
            return this.value.ToString(CultureInfo.CurrentCulture);
        }

        public static Transparency ByLayer =>
            new Transparency { value=-1 };

        public static Transparency ByBlock =>
            new Transparency { value=100 };

        public bool IsByLayer =>
            (this.value == -1);

        public bool IsByBlock =>
            (this.value == 100);

        public short Value
        {
            get => 
                this.value;
            set
            {
                if ((value < 0) || (value > 90))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted transparency values range from 0 to 90.");
                }
                this.value = value;
            }
        }
    }
}

