namespace netDxf.IO
{
    using System;

    internal interface ICodeValueReader
    {
        void Next();
        bool ReadBool();
        byte ReadByte();
        byte[] ReadBytes();
        double ReadDouble();
        string ReadHex();
        int ReadInt();
        long ReadLong();
        short ReadShort();
        string ReadString();

        short Code { get; }

        object Value { get; }

        long CurrentPosition { get; }
    }
}

