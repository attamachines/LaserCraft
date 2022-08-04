namespace netDxf.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class BinaryCodeValueReader : ICodeValueReader
    {
        private readonly BinaryReader reader;
        private readonly Encoding encoding;
        private short code;
        private object value;

        public BinaryCodeValueReader(BinaryReader reader, Encoding encoding)
        {
            this.reader = reader;
            this.encoding = encoding;
            byte[] buffer = this.reader.ReadBytes(0x16);
            StringBuilder builder = new StringBuilder(0x12);
            for (int i = 0; i < 0x12; i++)
            {
                builder.Append((char) buffer[i]);
            }
            if (builder.ToString() != "AutoCAD Binary DXF")
            {
                throw new ArgumentException("Not a valid binary dxf.");
            }
            this.code = 0;
            this.value = null;
        }

        public void Next()
        {
            this.code = this.reader.ReadInt16();
            if ((this.code >= 0) && (this.code <= 9))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 10) && (this.code <= 0x27))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 40) && (this.code <= 0x3b))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 60) && (this.code <= 0x4f))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 90) && (this.code <= 0x63))
            {
                this.value = this.reader.ReadInt32();
            }
            else if (this.code == 100)
            {
                this.value = this.NullTerminatedString();
            }
            else if (this.code == 0x65)
            {
                this.value = this.NullTerminatedString();
            }
            else if (this.code == 0x66)
            {
                this.value = this.NullTerminatedString();
            }
            else if (this.code == 0x69)
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 110) && (this.code <= 0x77))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 120) && (this.code <= 0x81))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 130) && (this.code <= 0x8b))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 140) && (this.code <= 0x95))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 160) && (this.code <= 0xa9))
            {
                this.value = this.reader.ReadInt64();
            }
            else if ((this.code >= 170) && (this.code <= 0xb3))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 210) && (this.code <= 0xef))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 270) && (this.code <= 0x117))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 280) && (this.code <= 0x121))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 290) && (this.code <= 0x12b))
            {
                this.value = this.reader.ReadByte();
            }
            else if ((this.code >= 300) && (this.code <= 0x135))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 310) && (this.code <= 0x13f))
            {
                this.value = this.ReadBinaryData();
            }
            else if ((this.code >= 320) && (this.code <= 0x149))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 330) && (this.code <= 0x171))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 370) && (this.code <= 0x17b))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 380) && (this.code <= 0x185))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 390) && (this.code <= 0x18f))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 400) && (this.code <= 0x199))
            {
                this.value = this.reader.ReadInt16();
            }
            else if ((this.code >= 410) && (this.code <= 0x1a3))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 420) && (this.code <= 0x1ad))
            {
                this.value = this.reader.ReadInt32();
            }
            else if ((this.code >= 430) && (this.code <= 0x1b7))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 440) && (this.code <= 0x1c1))
            {
                this.value = this.reader.ReadInt32();
            }
            else if ((this.code >= 450) && (this.code <= 0x1cb))
            {
                this.value = this.reader.ReadInt32();
            }
            else if ((this.code >= 460) && (this.code <= 0x1d5))
            {
                this.value = this.reader.ReadDouble();
            }
            else if ((this.code >= 470) && (this.code <= 0x1df))
            {
                this.value = this.NullTerminatedString();
            }
            else if ((this.code >= 480) && (this.code <= 0x1e1))
            {
                this.value = this.NullTerminatedString();
            }
            else
            {
                if (this.code == 0x3e7)
                {
                    throw new Exception($"The comment group, 999, is not used in binary DXF files at byte address {this.reader.BaseStream.Position}");
                }
                if ((this.code >= 0x3f2) && (this.code <= 0x423))
                {
                    this.value = this.reader.ReadDouble();
                }
                else if ((this.code >= 0x3e8) && (this.code <= 0x3eb))
                {
                    this.value = this.NullTerminatedString();
                }
                else if (this.code == 0x3ec)
                {
                    this.value = this.ReadBinaryData();
                }
                else if ((this.code >= 0x3ed) && (this.code <= 0x3f1))
                {
                    this.value = this.NullTerminatedString();
                }
                else if ((this.code >= 0x424) && (this.code <= 0x42e))
                {
                    this.value = this.reader.ReadInt16();
                }
                else
                {
                    if (this.code != 0x42f)
                    {
                        throw new Exception($"Code {this.code} not valid at byte address {this.reader.BaseStream.Position}");
                    }
                    this.value = this.reader.ReadInt32();
                }
            }
        }

        private string NullTerminatedString()
        {
            byte item = this.reader.ReadByte();
            List<byte> list = new List<byte>();
            while (item > 0)
            {
                list.Add(item);
                item = this.reader.ReadByte();
            }
            return this.encoding.GetString(list.ToArray(), 0, list.Count);
        }

        private byte[] ReadBinaryData()
        {
            byte count = this.reader.ReadByte();
            return this.reader.ReadBytes(count);
        }

        public bool ReadBool() => 
            (this.ReadByte() > 0);

        public byte ReadByte() => 
            ((byte) this.value);

        public byte[] ReadBytes() => 
            ((byte[]) this.value);

        public double ReadDouble() => 
            ((double) this.value);

        public string ReadHex()
        {
            if (!long.TryParse((string) this.value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long num))
            {
                throw new Exception($"Value {this.value} not valid at line {this.CurrentPosition}");
            }
            return num.ToString("X");
        }

        public int ReadInt() => 
            ((int) this.value);

        public long ReadLong() => 
            ((long) this.value);

        public short ReadShort() => 
            ((short) this.value);

        public string ReadString() => 
            ((string) this.value);

        public override string ToString() => 
            $"{this.code}:{this.value}";

        public short Code =>
            this.code;

        public object Value =>
            this.value;

        public long CurrentPosition =>
            this.reader.BaseStream.Position;
    }
}

