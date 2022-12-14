namespace netDxf.IO
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class BinaryCodeValueWriter : ICodeValueWriter
    {
        private readonly BinaryWriter writer;
        private short dxfCode;
        private object dxfValue;

        public BinaryCodeValueWriter(BinaryWriter writer)
        {
            this.writer = writer;
            byte[] buffer = new byte[] { 
                0x41, 0x75, 0x74, 0x6f, 0x43, 0x41, 0x44, 0x20, 0x42, 0x69, 110, 0x61, 0x72, 0x79, 0x20, 0x44,
                0x58, 70, 13, 10, 0x1a, 0
            };
            this.writer.Write(buffer);
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
            this.writer.Write(code);
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
            else
            {
                if (code == 0x3e7)
                {
                    throw new Exception($"The comment group, 999, is not used in binary DXF files at byte address {this.writer.BaseStream.Position}");
                }
                if ((code >= 0x3f2) && (code <= 0x423))
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
                        throw new Exception($"Code {this.dxfCode} not valid at byte address {this.writer.BaseStream.Position}");
                    }
                    Debug.Assert(value is int, "Incorrect value type.");
                    this.WriteInt((int) value);
                }
            }
            this.dxfValue = value;
        }

        public void WriteBool(bool value)
        {
            if (value)
            {
                this.writer.Write((byte) 1);
            }
            else
            {
                this.writer.Write((byte) 0);
            }
        }

        public void WriteByte(byte value)
        {
            this.writer.Write(value);
        }

        public void WriteBytes(byte[] value)
        {
            this.writer.Write((byte) value.Length);
            this.writer.Write(value);
        }

        public void WriteDouble(double value)
        {
            this.writer.Write(value);
        }

        public void WriteInt(int value)
        {
            this.writer.Write(value);
        }

        public void WriteLong(long value)
        {
            this.writer.Write(value);
        }

        public void WriteShort(short value)
        {
            this.writer.Write(value);
        }

        public void WriteString(string value)
        {
            this.writer.Write(value.ToCharArray());
            this.writer.Write('\0');
        }

        public short Code =>
            this.dxfCode;

        public object Value =>
            this.dxfValue;

        public long CurrentPosition =>
            this.writer.BaseStream.Position;
    }
}

