namespace netDxf
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class AciColor : ICloneable, IEquatable<AciColor>
    {
        private static readonly IReadOnlyDictionary<byte, byte[]> indexRgb;
        private short index;
        private byte r;
        private byte g;
        private byte b;
        private bool useTrueColor;

        static AciColor()
        {
            Dictionary<byte, byte[]> dictionary = new Dictionary<byte, byte[]>();
            byte[] buffer1 = new byte[3];
            buffer1[0] = 0xff;
            dictionary.Add(1, buffer1);
            byte[] buffer2 = new byte[3];
            buffer2[0] = 0xff;
            buffer2[1] = 0xff;
            dictionary.Add(2, buffer2);
            byte[] buffer3 = new byte[3];
            buffer3[1] = 0xff;
            dictionary.Add(3, buffer3);
            byte[] buffer4 = new byte[3];
            buffer4[1] = 0xff;
            buffer4[2] = 0xff;
            dictionary.Add(4, buffer4);
            byte[] buffer5 = new byte[3];
            buffer5[2] = 0xff;
            dictionary.Add(5, buffer5);
            byte[] buffer6 = new byte[3];
            buffer6[0] = 0xff;
            buffer6[2] = 0xff;
            dictionary.Add(6, buffer6);
            dictionary.Add(7, new byte[] { 0xff, 0xff, 0xff });
            dictionary.Add(8, new byte[] { 0x80, 0x80, 0x80 });
            dictionary.Add(9, new byte[] { 0xc0, 0xc0, 0xc0 });
            byte[] buffer7 = new byte[3];
            buffer7[0] = 0xff;
            dictionary.Add(10, buffer7);
            dictionary.Add(11, new byte[] { 0xff, 0x7f, 0x7f });
            byte[] buffer8 = new byte[3];
            buffer8[0] = 0xa5;
            dictionary.Add(12, buffer8);
            dictionary.Add(13, new byte[] { 0xa5, 0x52, 0x52 });
            byte[] buffer9 = new byte[3];
            buffer9[0] = 0x7f;
            dictionary.Add(14, buffer9);
            dictionary.Add(15, new byte[] { 0x7f, 0x3f, 0x3f });
            byte[] buffer10 = new byte[3];
            buffer10[0] = 0x4c;
            dictionary.Add(0x10, buffer10);
            dictionary.Add(0x11, new byte[] { 0x4c, 0x26, 0x26 });
            byte[] buffer11 = new byte[3];
            buffer11[0] = 0x26;
            dictionary.Add(0x12, buffer11);
            dictionary.Add(0x13, new byte[] { 0x26, 0x13, 0x13 });
            byte[] buffer12 = new byte[3];
            buffer12[0] = 0xff;
            buffer12[1] = 0x3f;
            dictionary.Add(20, buffer12);
            dictionary.Add(0x15, new byte[] { 0xff, 0x9f, 0x7f });
            byte[] buffer13 = new byte[3];
            buffer13[0] = 0xa5;
            buffer13[1] = 0x29;
            dictionary.Add(0x16, buffer13);
            dictionary.Add(0x17, new byte[] { 0xa5, 0x67, 0x52 });
            byte[] buffer14 = new byte[3];
            buffer14[0] = 0x7f;
            buffer14[1] = 0x1f;
            dictionary.Add(0x18, buffer14);
            dictionary.Add(0x19, new byte[] { 0x7f, 0x4f, 0x3f });
            byte[] buffer15 = new byte[3];
            buffer15[0] = 0x4c;
            buffer15[1] = 0x13;
            dictionary.Add(0x1a, buffer15);
            dictionary.Add(0x1b, new byte[] { 0x4c, 0x2f, 0x26 });
            byte[] buffer16 = new byte[3];
            buffer16[0] = 0x26;
            buffer16[1] = 9;
            dictionary.Add(0x1c, buffer16);
            dictionary.Add(0x1d, new byte[] { 0x26, 0x17, 0x13 });
            byte[] buffer17 = new byte[3];
            buffer17[0] = 0xff;
            buffer17[1] = 0x7f;
            dictionary.Add(30, buffer17);
            dictionary.Add(0x1f, new byte[] { 0xff, 0xbf, 0x7f });
            byte[] buffer18 = new byte[3];
            buffer18[0] = 0xa5;
            buffer18[1] = 0x52;
            dictionary.Add(0x20, buffer18);
            dictionary.Add(0x21, new byte[] { 0xa5, 0x7c, 0x52 });
            byte[] buffer19 = new byte[3];
            buffer19[0] = 0x7f;
            buffer19[1] = 0x3f;
            dictionary.Add(0x22, buffer19);
            dictionary.Add(0x23, new byte[] { 0x7f, 0x5f, 0x3f });
            byte[] buffer20 = new byte[3];
            buffer20[0] = 0x4c;
            buffer20[1] = 0x26;
            dictionary.Add(0x24, buffer20);
            dictionary.Add(0x25, new byte[] { 0x4c, 0x39, 0x26 });
            byte[] buffer21 = new byte[3];
            buffer21[0] = 0x26;
            buffer21[1] = 0x13;
            dictionary.Add(0x26, buffer21);
            dictionary.Add(0x27, new byte[] { 0x26, 0x1c, 0x13 });
            byte[] buffer22 = new byte[3];
            buffer22[0] = 0xff;
            buffer22[1] = 0xbf;
            dictionary.Add(40, buffer22);
            dictionary.Add(0x29, new byte[] { 0xff, 0xdf, 0x7f });
            byte[] buffer23 = new byte[3];
            buffer23[0] = 0xa5;
            buffer23[1] = 0x7c;
            dictionary.Add(0x2a, buffer23);
            dictionary.Add(0x2b, new byte[] { 0xa5, 0x91, 0x52 });
            byte[] buffer24 = new byte[3];
            buffer24[0] = 0x7f;
            buffer24[1] = 0x5f;
            dictionary.Add(0x2c, buffer24);
            dictionary.Add(0x2d, new byte[] { 0x7f, 0x6f, 0x3f });
            byte[] buffer25 = new byte[3];
            buffer25[0] = 0x4c;
            buffer25[1] = 0x39;
            dictionary.Add(0x2e, buffer25);
            dictionary.Add(0x2f, new byte[] { 0x4c, 0x42, 0x26 });
            byte[] buffer26 = new byte[3];
            buffer26[0] = 0x26;
            buffer26[1] = 0x1c;
            dictionary.Add(0x30, buffer26);
            dictionary.Add(0x31, new byte[] { 0x26, 0x21, 0x13 });
            byte[] buffer27 = new byte[3];
            buffer27[0] = 0xff;
            buffer27[1] = 0xff;
            dictionary.Add(50, buffer27);
            dictionary.Add(0x33, new byte[] { 0xff, 0xff, 0x7f });
            byte[] buffer28 = new byte[3];
            buffer28[0] = 0xa5;
            buffer28[1] = 0xa5;
            dictionary.Add(0x34, buffer28);
            dictionary.Add(0x35, new byte[] { 0xa5, 0xa5, 0x52 });
            byte[] buffer29 = new byte[3];
            buffer29[0] = 0x7f;
            buffer29[1] = 0x7f;
            dictionary.Add(0x36, buffer29);
            dictionary.Add(0x37, new byte[] { 0x7f, 0x7f, 0x3f });
            byte[] buffer30 = new byte[3];
            buffer30[0] = 0x4c;
            buffer30[1] = 0x4c;
            dictionary.Add(0x38, buffer30);
            dictionary.Add(0x39, new byte[] { 0x4c, 0x4c, 0x26 });
            byte[] buffer31 = new byte[3];
            buffer31[0] = 0x26;
            buffer31[1] = 0x26;
            dictionary.Add(0x3a, buffer31);
            dictionary.Add(0x3b, new byte[] { 0x26, 0x26, 0x13 });
            byte[] buffer32 = new byte[3];
            buffer32[0] = 0xbf;
            buffer32[1] = 0xff;
            dictionary.Add(60, buffer32);
            dictionary.Add(0x3d, new byte[] { 0xdf, 0xff, 0x7f });
            byte[] buffer33 = new byte[3];
            buffer33[0] = 0x7c;
            buffer33[1] = 0xa5;
            dictionary.Add(0x3e, buffer33);
            dictionary.Add(0x3f, new byte[] { 0x91, 0xa5, 0x52 });
            byte[] buffer34 = new byte[3];
            buffer34[0] = 0x5f;
            buffer34[1] = 0x7f;
            dictionary.Add(0x40, buffer34);
            dictionary.Add(0x41, new byte[] { 0x6f, 0x7f, 0x3f });
            byte[] buffer35 = new byte[3];
            buffer35[0] = 0x39;
            buffer35[1] = 0x4c;
            dictionary.Add(0x42, buffer35);
            dictionary.Add(0x43, new byte[] { 0x42, 0x4c, 0x26 });
            byte[] buffer36 = new byte[3];
            buffer36[0] = 0x1c;
            buffer36[1] = 0x26;
            dictionary.Add(0x44, buffer36);
            dictionary.Add(0x45, new byte[] { 0x21, 0x26, 0x13 });
            byte[] buffer37 = new byte[3];
            buffer37[0] = 0x7f;
            buffer37[1] = 0xff;
            dictionary.Add(70, buffer37);
            dictionary.Add(0x47, new byte[] { 0xbf, 0xff, 0x7f });
            byte[] buffer38 = new byte[3];
            buffer38[0] = 0x52;
            buffer38[1] = 0xa5;
            dictionary.Add(0x48, buffer38);
            dictionary.Add(0x49, new byte[] { 0x7c, 0xa5, 0x52 });
            byte[] buffer39 = new byte[3];
            buffer39[0] = 0x3f;
            buffer39[1] = 0x7f;
            dictionary.Add(0x4a, buffer39);
            dictionary.Add(0x4b, new byte[] { 0x5f, 0x7f, 0x3f });
            byte[] buffer40 = new byte[3];
            buffer40[0] = 0x26;
            buffer40[1] = 0x4c;
            dictionary.Add(0x4c, buffer40);
            dictionary.Add(0x4d, new byte[] { 0x39, 0x4c, 0x26 });
            byte[] buffer41 = new byte[3];
            buffer41[0] = 0x13;
            buffer41[1] = 0x26;
            dictionary.Add(0x4e, buffer41);
            dictionary.Add(0x4f, new byte[] { 0x1c, 0x26, 0x13 });
            byte[] buffer42 = new byte[3];
            buffer42[0] = 0x3f;
            buffer42[1] = 0xff;
            dictionary.Add(80, buffer42);
            dictionary.Add(0x51, new byte[] { 0x9f, 0xff, 0x7f });
            byte[] buffer43 = new byte[3];
            buffer43[0] = 0x29;
            buffer43[1] = 0xa5;
            dictionary.Add(0x52, buffer43);
            dictionary.Add(0x53, new byte[] { 0x67, 0xa5, 0x52 });
            byte[] buffer44 = new byte[3];
            buffer44[0] = 0x1f;
            buffer44[1] = 0x7f;
            dictionary.Add(0x54, buffer44);
            dictionary.Add(0x55, new byte[] { 0x4f, 0x7f, 0x3f });
            byte[] buffer45 = new byte[3];
            buffer45[0] = 0x13;
            buffer45[1] = 0x4c;
            dictionary.Add(0x56, buffer45);
            dictionary.Add(0x57, new byte[] { 0x2f, 0x4c, 0x26 });
            byte[] buffer46 = new byte[3];
            buffer46[0] = 9;
            buffer46[1] = 0x26;
            dictionary.Add(0x58, buffer46);
            dictionary.Add(0x59, new byte[] { 0x17, 0x26, 0x13 });
            byte[] buffer47 = new byte[3];
            buffer47[1] = 0xff;
            dictionary.Add(90, buffer47);
            dictionary.Add(0x5b, new byte[] { 0x7f, 0xff, 0x7f });
            byte[] buffer48 = new byte[3];
            buffer48[1] = 0xa5;
            dictionary.Add(0x5c, buffer48);
            dictionary.Add(0x5d, new byte[] { 0x52, 0xa5, 0x52 });
            byte[] buffer49 = new byte[3];
            buffer49[1] = 0x7f;
            dictionary.Add(0x5e, buffer49);
            dictionary.Add(0x5f, new byte[] { 0x3f, 0x7f, 0x3f });
            byte[] buffer50 = new byte[3];
            buffer50[1] = 0x4c;
            dictionary.Add(0x60, buffer50);
            dictionary.Add(0x61, new byte[] { 0x26, 0x4c, 0x26 });
            byte[] buffer51 = new byte[3];
            buffer51[1] = 0x26;
            dictionary.Add(0x62, buffer51);
            dictionary.Add(0x63, new byte[] { 0x13, 0x26, 0x13 });
            byte[] buffer52 = new byte[3];
            buffer52[1] = 0xff;
            buffer52[2] = 0x3f;
            dictionary.Add(100, buffer52);
            dictionary.Add(0x65, new byte[] { 0x7f, 0xff, 0x9f });
            byte[] buffer53 = new byte[3];
            buffer53[1] = 0xa5;
            buffer53[2] = 0x29;
            dictionary.Add(0x66, buffer53);
            dictionary.Add(0x67, new byte[] { 0x52, 0xa5, 0x67 });
            byte[] buffer54 = new byte[3];
            buffer54[1] = 0x7f;
            buffer54[2] = 0x1f;
            dictionary.Add(0x68, buffer54);
            dictionary.Add(0x69, new byte[] { 0x3f, 0x7f, 0x4f });
            byte[] buffer55 = new byte[3];
            buffer55[1] = 0x4c;
            buffer55[2] = 0x13;
            dictionary.Add(0x6a, buffer55);
            dictionary.Add(0x6b, new byte[] { 0x26, 0x4c, 0x2f });
            byte[] buffer56 = new byte[3];
            buffer56[1] = 0x26;
            buffer56[2] = 9;
            dictionary.Add(0x6c, buffer56);
            dictionary.Add(0x6d, new byte[] { 0x13, 0x26, 0x17 });
            byte[] buffer57 = new byte[3];
            buffer57[1] = 0xff;
            buffer57[2] = 0x7f;
            dictionary.Add(110, buffer57);
            dictionary.Add(0x6f, new byte[] { 0x7f, 0xff, 0xbf });
            byte[] buffer58 = new byte[3];
            buffer58[1] = 0xa5;
            buffer58[2] = 0x52;
            dictionary.Add(0x70, buffer58);
            dictionary.Add(0x71, new byte[] { 0x52, 0xa5, 0x7c });
            byte[] buffer59 = new byte[3];
            buffer59[1] = 0x7f;
            buffer59[2] = 0x3f;
            dictionary.Add(0x72, buffer59);
            dictionary.Add(0x73, new byte[] { 0x3f, 0x7f, 0x5f });
            byte[] buffer60 = new byte[3];
            buffer60[1] = 0x4c;
            buffer60[2] = 0x26;
            dictionary.Add(0x74, buffer60);
            dictionary.Add(0x75, new byte[] { 0x26, 0x4c, 0x39 });
            byte[] buffer61 = new byte[3];
            buffer61[1] = 0x26;
            buffer61[2] = 0x13;
            dictionary.Add(0x76, buffer61);
            dictionary.Add(0x77, new byte[] { 0x13, 0x26, 0x1c });
            byte[] buffer62 = new byte[3];
            buffer62[1] = 0xff;
            buffer62[2] = 0xbf;
            dictionary.Add(120, buffer62);
            dictionary.Add(0x79, new byte[] { 0x7f, 0xff, 0xdf });
            byte[] buffer63 = new byte[3];
            buffer63[1] = 0xa5;
            buffer63[2] = 0x7c;
            dictionary.Add(0x7a, buffer63);
            dictionary.Add(0x7b, new byte[] { 0x52, 0xa5, 0x91 });
            byte[] buffer64 = new byte[3];
            buffer64[1] = 0x7f;
            buffer64[2] = 0x5f;
            dictionary.Add(0x7c, buffer64);
            dictionary.Add(0x7d, new byte[] { 0x3f, 0x7f, 0x6f });
            byte[] buffer65 = new byte[3];
            buffer65[1] = 0x4c;
            buffer65[2] = 0x39;
            dictionary.Add(0x7e, buffer65);
            dictionary.Add(0x7f, new byte[] { 0x26, 0x4c, 0x42 });
            byte[] buffer66 = new byte[3];
            buffer66[1] = 0x26;
            buffer66[2] = 0x1c;
            dictionary.Add(0x80, buffer66);
            dictionary.Add(0x81, new byte[] { 0x13, 0x26, 0x21 });
            byte[] buffer67 = new byte[3];
            buffer67[1] = 0xff;
            buffer67[2] = 0xff;
            dictionary.Add(130, buffer67);
            dictionary.Add(0x83, new byte[] { 0x7f, 0xff, 0xff });
            byte[] buffer68 = new byte[3];
            buffer68[1] = 0xa5;
            buffer68[2] = 0xa5;
            dictionary.Add(0x84, buffer68);
            dictionary.Add(0x85, new byte[] { 0x52, 0xa5, 0xa5 });
            byte[] buffer69 = new byte[3];
            buffer69[1] = 0x7f;
            buffer69[2] = 0x7f;
            dictionary.Add(0x86, buffer69);
            dictionary.Add(0x87, new byte[] { 0x3f, 0x7f, 0x7f });
            byte[] buffer70 = new byte[3];
            buffer70[1] = 0x4c;
            buffer70[2] = 0x4c;
            dictionary.Add(0x88, buffer70);
            dictionary.Add(0x89, new byte[] { 0x26, 0x4c, 0x4c });
            byte[] buffer71 = new byte[3];
            buffer71[1] = 0x26;
            buffer71[2] = 0x26;
            dictionary.Add(0x8a, buffer71);
            dictionary.Add(0x8b, new byte[] { 0x13, 0x26, 0x26 });
            byte[] buffer72 = new byte[3];
            buffer72[1] = 0xbf;
            buffer72[2] = 0xff;
            dictionary.Add(140, buffer72);
            dictionary.Add(0x8d, new byte[] { 0x7f, 0xdf, 0xff });
            byte[] buffer73 = new byte[3];
            buffer73[1] = 0x7c;
            buffer73[2] = 0xa5;
            dictionary.Add(0x8e, buffer73);
            dictionary.Add(0x8f, new byte[] { 0x52, 0x91, 0xa5 });
            byte[] buffer74 = new byte[3];
            buffer74[1] = 0x5f;
            buffer74[2] = 0x7f;
            dictionary.Add(0x90, buffer74);
            dictionary.Add(0x91, new byte[] { 0x3f, 0x6f, 0x7f });
            byte[] buffer75 = new byte[3];
            buffer75[1] = 0x39;
            buffer75[2] = 0x4c;
            dictionary.Add(0x92, buffer75);
            dictionary.Add(0x93, new byte[] { 0x26, 0x42, 0x4c });
            byte[] buffer76 = new byte[3];
            buffer76[1] = 0x1c;
            buffer76[2] = 0x26;
            dictionary.Add(0x94, buffer76);
            dictionary.Add(0x95, new byte[] { 0x13, 0x21, 0x26 });
            byte[] buffer77 = new byte[3];
            buffer77[1] = 0x7f;
            buffer77[2] = 0xff;
            dictionary.Add(150, buffer77);
            dictionary.Add(0x97, new byte[] { 0x7f, 0xbf, 0xff });
            byte[] buffer78 = new byte[3];
            buffer78[1] = 0x52;
            buffer78[2] = 0xa5;
            dictionary.Add(0x98, buffer78);
            dictionary.Add(0x99, new byte[] { 0x52, 0x7c, 0xa5 });
            byte[] buffer79 = new byte[3];
            buffer79[1] = 0x3f;
            buffer79[2] = 0x7f;
            dictionary.Add(0x9a, buffer79);
            dictionary.Add(0x9b, new byte[] { 0x3f, 0x5f, 0x7f });
            byte[] buffer80 = new byte[3];
            buffer80[1] = 0x26;
            buffer80[2] = 0x4c;
            dictionary.Add(0x9c, buffer80);
            dictionary.Add(0x9d, new byte[] { 0x26, 0x39, 0x4c });
            byte[] buffer81 = new byte[3];
            buffer81[1] = 0x13;
            buffer81[2] = 0x26;
            dictionary.Add(0x9e, buffer81);
            dictionary.Add(0x9f, new byte[] { 0x13, 0x1c, 0x26 });
            byte[] buffer82 = new byte[3];
            buffer82[1] = 0x3f;
            buffer82[2] = 0xff;
            dictionary.Add(160, buffer82);
            dictionary.Add(0xa1, new byte[] { 0x7f, 0x9f, 0xff });
            byte[] buffer83 = new byte[3];
            buffer83[1] = 0x29;
            buffer83[2] = 0xa5;
            dictionary.Add(0xa2, buffer83);
            dictionary.Add(0xa3, new byte[] { 0x52, 0x67, 0xa5 });
            byte[] buffer84 = new byte[3];
            buffer84[1] = 0x1f;
            buffer84[2] = 0x7f;
            dictionary.Add(0xa4, buffer84);
            dictionary.Add(0xa5, new byte[] { 0x3f, 0x4f, 0x7f });
            byte[] buffer85 = new byte[3];
            buffer85[1] = 0x13;
            buffer85[2] = 0x4c;
            dictionary.Add(0xa6, buffer85);
            dictionary.Add(0xa7, new byte[] { 0x26, 0x2f, 0x4c });
            byte[] buffer86 = new byte[3];
            buffer86[1] = 9;
            buffer86[2] = 0x26;
            dictionary.Add(0xa8, buffer86);
            dictionary.Add(0xa9, new byte[] { 0x13, 0x17, 0x26 });
            byte[] buffer87 = new byte[3];
            buffer87[2] = 0xff;
            dictionary.Add(170, buffer87);
            dictionary.Add(0xab, new byte[] { 0x7f, 0x7f, 0xff });
            byte[] buffer88 = new byte[3];
            buffer88[2] = 0xa5;
            dictionary.Add(0xac, buffer88);
            dictionary.Add(0xad, new byte[] { 0x52, 0x52, 0xa5 });
            byte[] buffer89 = new byte[3];
            buffer89[2] = 0x7f;
            dictionary.Add(0xae, buffer89);
            dictionary.Add(0xaf, new byte[] { 0x3f, 0x3f, 0x7f });
            byte[] buffer90 = new byte[3];
            buffer90[2] = 0x4c;
            dictionary.Add(0xb0, buffer90);
            dictionary.Add(0xb1, new byte[] { 0x26, 0x26, 0x4c });
            byte[] buffer91 = new byte[3];
            buffer91[2] = 0x26;
            dictionary.Add(0xb2, buffer91);
            dictionary.Add(0xb3, new byte[] { 0x13, 0x13, 0x26 });
            byte[] buffer92 = new byte[3];
            buffer92[0] = 0x3f;
            buffer92[2] = 0xff;
            dictionary.Add(180, buffer92);
            dictionary.Add(0xb5, new byte[] { 0x9f, 0x7f, 0xff });
            byte[] buffer93 = new byte[3];
            buffer93[0] = 0x29;
            buffer93[2] = 0xa5;
            dictionary.Add(0xb6, buffer93);
            dictionary.Add(0xb7, new byte[] { 0x67, 0x52, 0xa5 });
            byte[] buffer94 = new byte[3];
            buffer94[0] = 0x1f;
            buffer94[2] = 0x7f;
            dictionary.Add(0xb8, buffer94);
            dictionary.Add(0xb9, new byte[] { 0x4f, 0x3f, 0x7f });
            byte[] buffer95 = new byte[3];
            buffer95[0] = 0x13;
            buffer95[2] = 0x4c;
            dictionary.Add(0xba, buffer95);
            dictionary.Add(0xbb, new byte[] { 0x2f, 0x26, 0x4c });
            byte[] buffer96 = new byte[3];
            buffer96[0] = 9;
            buffer96[2] = 0x26;
            dictionary.Add(0xbc, buffer96);
            dictionary.Add(0xbd, new byte[] { 0x17, 0x13, 0x26 });
            byte[] buffer97 = new byte[3];
            buffer97[0] = 0x7f;
            buffer97[2] = 0xff;
            dictionary.Add(190, buffer97);
            dictionary.Add(0xbf, new byte[] { 0xbf, 0x7f, 0xff });
            byte[] buffer98 = new byte[3];
            buffer98[0] = 0x52;
            buffer98[2] = 0xa5;
            dictionary.Add(0xc0, buffer98);
            dictionary.Add(0xc1, new byte[] { 0x7c, 0x52, 0xa5 });
            byte[] buffer99 = new byte[3];
            buffer99[0] = 0x3f;
            buffer99[2] = 0x7f;
            dictionary.Add(0xc2, buffer99);
            dictionary.Add(0xc3, new byte[] { 0x5f, 0x3f, 0x7f });
            byte[] buffer100 = new byte[3];
            buffer100[0] = 0x26;
            buffer100[2] = 0x4c;
            dictionary.Add(0xc4, buffer100);
            dictionary.Add(0xc5, new byte[] { 0x39, 0x26, 0x4c });
            byte[] buffer101 = new byte[3];
            buffer101[0] = 0x13;
            buffer101[2] = 0x26;
            dictionary.Add(0xc6, buffer101);
            dictionary.Add(0xc7, new byte[] { 0x1c, 0x13, 0x26 });
            byte[] buffer102 = new byte[3];
            buffer102[0] = 0xbf;
            buffer102[2] = 0xff;
            dictionary.Add(200, buffer102);
            dictionary.Add(0xc9, new byte[] { 0xdf, 0x7f, 0xff });
            byte[] buffer103 = new byte[3];
            buffer103[0] = 0x7c;
            buffer103[2] = 0xa5;
            dictionary.Add(0xca, buffer103);
            dictionary.Add(0xcb, new byte[] { 0x91, 0x52, 0xa5 });
            byte[] buffer104 = new byte[3];
            buffer104[0] = 0x5f;
            buffer104[2] = 0x7f;
            dictionary.Add(0xcc, buffer104);
            dictionary.Add(0xcd, new byte[] { 0x6f, 0x3f, 0x7f });
            byte[] buffer105 = new byte[3];
            buffer105[0] = 0x39;
            buffer105[2] = 0x4c;
            dictionary.Add(0xce, buffer105);
            dictionary.Add(0xcf, new byte[] { 0x42, 0x26, 0x4c });
            byte[] buffer106 = new byte[3];
            buffer106[0] = 0x1c;
            buffer106[2] = 0x26;
            dictionary.Add(0xd0, buffer106);
            dictionary.Add(0xd1, new byte[] { 0x21, 0x13, 0x26 });
            byte[] buffer107 = new byte[3];
            buffer107[0] = 0xff;
            buffer107[2] = 0xff;
            dictionary.Add(210, buffer107);
            dictionary.Add(0xd3, new byte[] { 0xff, 0x7f, 0xff });
            byte[] buffer108 = new byte[3];
            buffer108[0] = 0xa5;
            buffer108[2] = 0xa5;
            dictionary.Add(0xd4, buffer108);
            dictionary.Add(0xd5, new byte[] { 0xa5, 0x52, 0xa5 });
            byte[] buffer109 = new byte[3];
            buffer109[0] = 0x7f;
            buffer109[2] = 0x7f;
            dictionary.Add(0xd6, buffer109);
            dictionary.Add(0xd7, new byte[] { 0x7f, 0x3f, 0x7f });
            byte[] buffer110 = new byte[3];
            buffer110[0] = 0x4c;
            buffer110[2] = 0x4c;
            dictionary.Add(0xd8, buffer110);
            dictionary.Add(0xd9, new byte[] { 0x4c, 0x26, 0x4c });
            byte[] buffer111 = new byte[3];
            buffer111[0] = 0x26;
            buffer111[2] = 0x26;
            dictionary.Add(0xda, buffer111);
            dictionary.Add(0xdb, new byte[] { 0x26, 0x13, 0x26 });
            byte[] buffer112 = new byte[3];
            buffer112[0] = 0xff;
            buffer112[2] = 0xbf;
            dictionary.Add(220, buffer112);
            dictionary.Add(0xdd, new byte[] { 0xff, 0x7f, 0xdf });
            byte[] buffer113 = new byte[3];
            buffer113[0] = 0xa5;
            buffer113[2] = 0x7c;
            dictionary.Add(0xde, buffer113);
            dictionary.Add(0xdf, new byte[] { 0xa5, 0x52, 0x91 });
            byte[] buffer114 = new byte[3];
            buffer114[0] = 0x7f;
            buffer114[2] = 0x5f;
            dictionary.Add(0xe0, buffer114);
            dictionary.Add(0xe1, new byte[] { 0x7f, 0x3f, 0x6f });
            byte[] buffer115 = new byte[3];
            buffer115[0] = 0x4c;
            buffer115[2] = 0x39;
            dictionary.Add(0xe2, buffer115);
            dictionary.Add(0xe3, new byte[] { 0x4c, 0x26, 0x42 });
            byte[] buffer116 = new byte[3];
            buffer116[0] = 0x26;
            buffer116[2] = 0x1c;
            dictionary.Add(0xe4, buffer116);
            dictionary.Add(0xe5, new byte[] { 0x26, 0x13, 0x21 });
            byte[] buffer117 = new byte[3];
            buffer117[0] = 0xff;
            buffer117[2] = 0x7f;
            dictionary.Add(230, buffer117);
            dictionary.Add(0xe7, new byte[] { 0xff, 0x7f, 0xbf });
            byte[] buffer118 = new byte[3];
            buffer118[0] = 0xa5;
            buffer118[2] = 0x52;
            dictionary.Add(0xe8, buffer118);
            dictionary.Add(0xe9, new byte[] { 0xa5, 0x52, 0x7c });
            byte[] buffer119 = new byte[3];
            buffer119[0] = 0x7f;
            buffer119[2] = 0x3f;
            dictionary.Add(0xea, buffer119);
            dictionary.Add(0xeb, new byte[] { 0x7f, 0x3f, 0x5f });
            byte[] buffer120 = new byte[3];
            buffer120[0] = 0x4c;
            buffer120[2] = 0x26;
            dictionary.Add(0xec, buffer120);
            dictionary.Add(0xed, new byte[] { 0x4c, 0x26, 0x39 });
            byte[] buffer121 = new byte[3];
            buffer121[0] = 0x26;
            buffer121[2] = 0x13;
            dictionary.Add(0xee, buffer121);
            dictionary.Add(0xef, new byte[] { 0x26, 0x13, 0x1c });
            byte[] buffer122 = new byte[3];
            buffer122[0] = 0xff;
            buffer122[2] = 0x3f;
            dictionary.Add(240, buffer122);
            dictionary.Add(0xf1, new byte[] { 0xff, 0x7f, 0x9f });
            byte[] buffer123 = new byte[3];
            buffer123[0] = 0xa5;
            buffer123[2] = 0x29;
            dictionary.Add(0xf2, buffer123);
            dictionary.Add(0xf3, new byte[] { 0xa5, 0x52, 0x67 });
            byte[] buffer124 = new byte[3];
            buffer124[0] = 0x7f;
            buffer124[2] = 0x1f;
            dictionary.Add(0xf4, buffer124);
            dictionary.Add(0xf5, new byte[] { 0x7f, 0x3f, 0x4f });
            byte[] buffer125 = new byte[3];
            buffer125[0] = 0x4c;
            buffer125[2] = 0x13;
            dictionary.Add(0xf6, buffer125);
            dictionary.Add(0xf7, new byte[] { 0x4c, 0x26, 0x2f });
            byte[] buffer126 = new byte[3];
            buffer126[0] = 0x26;
            buffer126[2] = 9;
            dictionary.Add(0xf8, buffer126);
            dictionary.Add(0xf9, new byte[] { 0x26, 0x13, 0x17 });
            dictionary.Add(250, new byte[3]);
            dictionary.Add(0xfb, new byte[] { 0x33, 0x33, 0x33 });
            dictionary.Add(0xfc, new byte[] { 0x66, 0x66, 0x66 });
            dictionary.Add(0xfd, new byte[] { 0x99, 0x99, 0x99 });
            dictionary.Add(0xfe, new byte[] { 0xcc, 0xcc, 0xcc });
            dictionary.Add(0xff, new byte[] { 0xff, 0xff, 0xff });
            indexRgb = dictionary;
        }

        public AciColor() : this((short) 7)
        {
        }

        public AciColor(Color color) : this(color.R, color.G, color.B)
        {
        }

        public AciColor(short index)
        {
            if ((index <= 0) || (index >= 0x100))
            {
                throw new ArgumentOutOfRangeException("index", index, "Accepted color index values range from 1 to 255.");
            }
            byte[] buffer = IndexRgb[(byte) index];
            this.r = buffer[0];
            this.g = buffer[1];
            this.b = buffer[2];
            this.useTrueColor = false;
            this.index = index;
        }

        public AciColor(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.useTrueColor = true;
            this.index = RgbToAci(this.r, this.g, this.b);
        }

        public AciColor(double r, double g, double b) : this((byte) (r * 255.0), (byte) (g * 255.0), (byte) (b * 255.0))
        {
        }

        public AciColor(float r, float g, float b) : this((byte) (r * 255f), (byte) (g * 255f), (byte) (b * 255f))
        {
        }

        public object Clone() => 
            new AciColor { 
                r = this.r,
                g = this.g,
                b = this.b,
                useTrueColor = this.useTrueColor,
                index = this.index
            };

        public bool Equals(AciColor other)
        {
            if (other == null)
            {
                return false;
            }
            return (((other.r == this.r) && (other.g == this.g)) && (other.b == this.b));
        }

        public static AciColor FromCadIndex(short index)
        {
            if ((index < 0) || (index > 0x100))
            {
                throw new ArgumentOutOfRangeException("index", index, "Accepted CAD indexed AciColor values range from 0 to 256.");
            }
            if (index == 0)
            {
                return ByBlock;
            }
            if (index == 0x100)
            {
                return ByLayer;
            }
            return new AciColor(index);
        }

        public void FromColor(Color color)
        {
            this.r = color.R;
            this.g = color.G;
            this.b = color.B;
            this.useTrueColor = true;
            this.index = RgbToAci(this.r, this.g, this.b);
        }

        public static AciColor FromHsl(Vector3 hsl) => 
            FromHsl(hsl.X, hsl.Y, hsl.Z);

        public static AciColor FromHsl(double hue, double saturation, double lightness)
        {
            double r = lightness;
            double g = lightness;
            double b = lightness;
            double num4 = (lightness <= 0.5) ? (lightness * (1.0 + saturation)) : ((lightness + saturation) - (lightness * saturation));
            if (num4 > 0.0)
            {
                double num5 = (lightness + lightness) - num4;
                double num6 = (num4 - num5) / num4;
                hue *= 6.0;
                int num7 = (int) hue;
                double num8 = hue - num7;
                double num9 = (num4 * num6) * num8;
                double num10 = num5 + num9;
                double num11 = num4 - num9;
                switch (num7)
                {
                    case 1:
                        r = num11;
                        g = num4;
                        b = num5;
                        break;

                    case 2:
                        r = num5;
                        g = num4;
                        b = num10;
                        break;

                    case 3:
                        r = num5;
                        g = num11;
                        b = num4;
                        break;

                    case 4:
                        r = num10;
                        g = num5;
                        b = num4;
                        break;

                    case 5:
                        r = num4;
                        g = num5;
                        b = num11;
                        break;

                    case 6:
                        r = num4;
                        g = num10;
                        b = num5;
                        break;
                }
            }
            return new AciColor(r, g, b);
        }

        public static AciColor FromTrueColor(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return new AciColor(bytes[2], bytes[1], bytes[0]);
        }

        private static byte RgbToAci(byte r, byte g, byte b)
        {
            double maxValue = double.MaxValue;
            byte num2 = 0;
            foreach (byte num3 in IndexRgb.Keys)
            {
                byte[] buffer = IndexRgb[num3];
                double num4 = Math.Abs((double) (((0.3 * (r - buffer[0])) + (0.59 * (g - buffer[1]))) + (0.11 * (b - buffer[2]))));
                if (num4 < maxValue)
                {
                    maxValue = num4;
                    num2 = num3;
                }
            }
            return num2;
        }

        public Color ToColor()
        {
            if ((this.index < 1) || (this.index > 0xff))
            {
                return Color.White;
            }
            return Color.FromArgb(this.r, this.g, this.b);
        }

        public static Vector3 ToHsl(AciColor color)
        {
            ToHsl(color, out double num, out double num2, out double num3);
            return new Vector3(num, num2, num3);
        }

        public static void ToHsl(AciColor color, out double hue, out double saturation, out double lightness)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            double num = ((double) color.R) / 255.0;
            double num2 = ((double) color.G) / 255.0;
            double num3 = ((double) color.B) / 255.0;
            hue = 0.0;
            saturation = 0.0;
            double b = Math.Max(Math.Max(num, num2), num3);
            double num5 = Math.Min(Math.Min(num, num2), num3);
            lightness = (num5 + b) / 2.0;
            if (lightness > 0.0)
            {
                double num6 = b - num5;
                saturation = num6;
                if (saturation > 0.0)
                {
                    saturation /= (lightness <= 0.5) ? (b + num5) : ((2.0 - b) - num5);
                    double num7 = (b - num) / num6;
                    double num8 = (b - num2) / num6;
                    double num9 = (b - num3) / num6;
                    if (MathHelper.IsEqual(num, b))
                    {
                        hue = MathHelper.IsEqual(num2, num5) ? (5.0 + num9) : (1.0 - num8);
                    }
                    else if (MathHelper.IsEqual(num2, b))
                    {
                        hue = MathHelper.IsEqual(num3, num5) ? (1.0 + num7) : (3.0 - num9);
                    }
                    else
                    {
                        hue = MathHelper.IsEqual(num, num5) ? (3.0 + num8) : (5.0 - num7);
                    }
                    hue /= 6.0;
                }
            }
        }

        public override string ToString()
        {
            if (this.index == 0)
            {
                return "ByBlock";
            }
            if (this.index == 0x100)
            {
                return "ByLayer";
            }
            if (this.useTrueColor)
            {
                return string.Format("{0}{3}{1}{3}{2}", new object[] { this.r, this.g, this.b, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator });
            }
            return this.index.ToString(CultureInfo.CurrentCulture);
        }

        public static int ToTrueColor(AciColor color)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            byte[] buffer1 = new byte[4];
            buffer1[0] = color.B;
            buffer1[1] = color.G;
            buffer1[2] = color.R;
            return BitConverter.ToInt32(buffer1, 0);
        }

        public static AciColor ByLayer =>
            new AciColor { index=0x100 };

        public static AciColor ByBlock =>
            new AciColor { index=0 };

        public static AciColor Red =>
            new AciColor(1);

        public static AciColor Yellow =>
            new AciColor(2);

        public static AciColor Green =>
            new AciColor(3);

        public static AciColor Cyan =>
            new AciColor(4);

        public static AciColor Blue =>
            new AciColor(5);

        public static AciColor Magenta =>
            new AciColor(6);

        public static AciColor Default =>
            new AciColor(7);

        public static AciColor DarkGray =>
            new AciColor(8);

        public static AciColor LightGray =>
            new AciColor(9);

        public static IReadOnlyDictionary<byte, byte[]> IndexRgb =>
            indexRgb;

        public bool IsByLayer =>
            (this.index == 0x100);

        public bool IsByBlock =>
            (this.index == 0);

        public byte R =>
            this.r;

        public byte G =>
            this.g;

        public byte B =>
            this.b;

        public bool UseTrueColor
        {
            get => 
                this.useTrueColor;
            set => 
                (this.useTrueColor = value);
        }

        public short Index
        {
            get => 
                this.index;
            set
            {
                if ((value <= 0) || (value >= 0x100))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted color index values range from 1 to 255.");
                }
                byte[] buffer = IndexRgb[(byte) this.index];
                this.r = buffer[0];
                this.g = buffer[1];
                this.b = buffer[2];
                this.useTrueColor = false;
                this.index = value;
            }
        }
    }
}

