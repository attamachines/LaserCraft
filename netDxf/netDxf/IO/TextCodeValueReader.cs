namespace netDxf.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    internal class TextCodeValueReader : ICodeValueReader
    {
        private readonly TextReader reader;
        private short code;
        private string value;
        private long currentPosition;

        public TextCodeValueReader(TextReader reader)
        {
            this.reader = reader;
            this.code = 0;
            this.value = null;
            this.currentPosition = 0L;
        }

        public void Next()
        {
            string s = this.reader.ReadLine();
            this.currentPosition += 1L;
            if (!short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.code))
            {
                throw new Exception($"Code {this.code} not valid at line {this.currentPosition}");
            }
            this.value = this.reader.ReadLine();
            this.currentPosition += 1L;
        }

        public bool ReadBool() => 
            (this.ReadByte() > 0);

        public byte ReadByte()
        {
            if (!byte.TryParse(this.value, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture, out byte num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.currentPosition}");
            }
            return num;
        }

        public byte[] ReadBytes()
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < this.value.Length; i++)
            {
                string s = this.value[i] + this.value[++i];
                if (!byte.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte num2))
                {
                    throw new Exception($"Value {s} not valid at line {this.currentPosition}");
                }
                list.Add(num2);
            }
            return list.ToArray();
        }

        public double ReadDouble()
        {
            if (!double.TryParse(this.value, NumberStyles.Float, CultureInfo.InvariantCulture, out double num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.currentPosition}");
            }
            return num;
        }

        public string ReadHex()
        {
            if (!long.TryParse(this.value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.currentPosition}");
            }
            return num.ToString("X");
        }

        public int ReadInt()
        {
            if (!int.TryParse(this.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.currentPosition}");
            }
            return num;
        }

        public long ReadLong()
        {
            if (!long.TryParse(this.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.currentPosition}");
            }
            return num;
        }

        public short ReadShort()
        {
            if (!short.TryParse(this.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out short num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.currentPosition}");
            }
            return num;
        }

        public string ReadString() => 
            this.value;

        public override string ToString() => 
            $"{this.code}:{this.value}";

        public short Code =>
            this.code;

        public object Value =>
            this.value;

        public long CurrentPosition =>
            this.currentPosition;
    }
}

