namespace netDxf.IO
{
    using System;

    internal interface ICodeValueWriter
    {
        void Flush();
        void Write(short code, object value);
        void WriteBool(bool value);
        void WriteByte(byte value);
        void WriteBytes(byte[] value);
        void WriteDouble(double value);
        void WriteInt(int value);
        void WriteLong(long value);
        void WriteShort(short value);
        void WriteString(string value);

        short Code { get; }

        object Value { get; }

        long CurrentPosition { get; }
    }
}

