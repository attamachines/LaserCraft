namespace netDxf.IO
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class TextCodeValueWriter : ICodeValueWriter
    {
        private readonly TextWriter writer;
        private short dxfCode;
        private object dxfValue;
        private long currentPosition;

        public TextCodeValueWriter(TextWriter writer)
        {
            this.writer = writer;
            this.currentPosition = 0L;
            this.dxfCode = 0;
            this.dxfValue = null;
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        public override string ToString() => 
            $"{this.dxfCode}:{this.dxfValue}";

        public void Write(short code, object value)
        {
            this.dxfCode = code;
            this.writer.WriteLine((int) code);
            this.currentPosition += 1L;
            if ((code >= 0) && (code <= 9))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 10) && (code <= 0x27))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 40) && (code <= 0x3b))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 60) && (code <= 0x4f))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 90) && (code <= 0x63))
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if (code == 100)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 0x65)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 0x66)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 0x69)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 110) && (code <= 0x77))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 120) && (code <= 0x81))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 130) && (code <= 0x8b))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 140) && (code <= 0x95))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 160) && (code <= 0xa9))
            {
                Debug.Assert(value is long, "Incorrect value type.");
                this.WriteLong((long) value);
            }
            else if ((code >= 170) && (code <= 0xb3))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 210) && (code <= 0xef))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 270) && (code <= 0x117))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 280) && (code <= 0x121))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 290) && (code <= 0x12b))
            {
                Debug.Assert(value is bool, "Incorrect value type.");
                this.WriteBool((bool) value);
            }
            else if ((code >= 300) && (code <= 0x135))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 310) && (code <= 0x13f))
            {
                Debug.Assert(value is byte[], "Incorrect value type.");
                this.WriteBytes((byte[]) value);
            }
            else if ((code >= 320) && (code <= 0x149))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 330) && (code <= 0x171))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 370) && (code <= 0x17b))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 380) && (code <= 0x185))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 390) && (code <= 0x18f))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 400) && (code <= 0x199))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else if ((code >= 410) && (code <= 0x1a3))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 420) && (code <= 0x1ad))
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if ((code >= 430) && (code <= 0x1b7))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 440) && (code <= 0x1c1))
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if ((code >= 450) && (code <= 0x1cb))
            {
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            else if ((code >= 460) && (code <= 0x1d5))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 470) && (code <= 0x1df))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 480) && (code <= 0x1e1))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 0x3e7)
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 0x3f2) && (code <= 0x423))
            {
                Debug.Assert(value is double, "Incorrect value type.");
                this.WriteDouble((double) value);
            }
            else if ((code >= 0x3e8) && (code <= 0x3eb))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if (code == 0x3ec)
            {
                Debug.Assert(value is byte[], "Incorrect value type.");
                this.WriteBytes((byte[]) value);
            }
            else if ((code >= 0x3ed) && (code <= 0x3f1))
            {
                Debug.Assert(value is string, "Incorrect value type.");
                this.WriteString((string) value);
            }
            else if ((code >= 0x424) && (code <= 0x42e))
            {
                Debug.Assert(value is short, "Incorrect value type.");
                this.WriteShort((short) value);
            }
            else
            {
                if (code != 0x42f)
                {
                    throw new Exception($"Code {this.dxfCode} not valid at line {this.currentPosition}");
                }
                Debug.Assert(value is int, "Incorrect value type.");
                this.WriteInt((int) value);
            }
            this.dxfValue = value;
            this.currentPosition += 1L;
        }

        public void WriteBool(bool value)
        {
            this.writer.WriteLine(value ? 1 : 0);
        }

        public void WriteByte(byte value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteBytes(byte[] value)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                builder.Append($"{value[i]:X2}");
            }
            this.writer.WriteLine(builder.ToString());
        }

        public void WriteDouble(double value)
        {
            this.writer.WriteLine(value.ToString("0.0#############", CultureInfo.InvariantCulture));
        }

        public void WriteInt(int value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteLong(long value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteShort(short value)
        {
            this.writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteString(string value)
        {
            this.writer.WriteLine(value);
        }

        public short Code =>
            this.dxfCode;

        public object Value =>
            this.dxfValue;

        public long CurrentPosition =>
            this.currentPosition;
    }
}

