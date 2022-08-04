namespace netDxf.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal static class EncodingType
    {
        public static Encoding GetType(Stream fs)
        {
            byte[] buffer = new byte[] { 0xff, 0xfe, 0x41 };
            byte[] buffer1 = new byte[3];
            buffer1[0] = 0xfe;
            buffer1[1] = 0xff;
            byte[] buffer2 = buffer1;
            byte[] buffer3 = new byte[] { 0xef, 0xbb, 0xbf };
            Encoding aSCII = Encoding.ASCII;
            BinaryReader reader = new BinaryReader(fs, Encoding.Default);
            if (!int.TryParse(fs.Length.ToString(CultureInfo.InvariantCulture), out int num))
            {
                return null;
            }
            byte[] data = reader.ReadBytes(num);
            if (IsUTF8Bytes(data) || (((data[0] == buffer3[0]) && (data[1] == buffer3[1])) && (data[2] == buffer3[2])))
            {
                aSCII = Encoding.UTF8;
            }
            else if (((data[0] == buffer2[0]) && (data[1] == buffer2[1])) && (data[2] == buffer2[2]))
            {
                aSCII = Encoding.BigEndianUnicode;
            }
            else if (((data[0] == buffer[0]) && (data[1] == buffer[1])) && (data[2] == buffer[2]))
            {
                aSCII = Encoding.Unicode;
            }
            fs.Position = 0L;
            return aSCII;
        }

        private static bool IsUTF8Bytes(byte[] data)
        {
            int num = 1;
            for (int i = 0; i < data.Length; i++)
            {
                byte num3 = data[i];
                if (num == 1)
                {
                    if (num3 >= 0x80)
                    {
                        while (((num3 = (byte) (num3 << 1)) & 0x80) > 0)
                        {
                            num++;
                        }
                        if ((num == 1) || (num > 6))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((num3 & 0xc0) != 0x80)
                    {
                        return false;
                    }
                    num--;
                }
            }
            if (num > 1)
            {
                throw new Exception("Error byte format.");
            }
            return true;
        }
    }
}

