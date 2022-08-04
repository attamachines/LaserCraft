namespace netDxf.IO
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Entities;
    using netDxf.Header;
    using netDxf.Objects;
    using netDxf.Tables;
    using netDxf.Units;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class DxfReader
    {
        private bool isBinary;
        private ICodeValueReader chunk;
        private DxfDocument doc;
        private Dictionary<string, string> decodedStrings;
        private Dictionary<string, BlockRecord> blockRecords;
        private Dictionary<EntityObject, string> entityList;
        private Dictionary<Viewport, string> viewports;
        private Dictionary<Hatch, List<HatchBoundaryPath>> hatchToPaths;
        private Dictionary<HatchBoundaryPath, List<string>> hatchContourns;
        private Dictionary<Insert, string> nestedInserts;
        private Dictionary<Dimension, string> nestedDimensions;
        private DictionaryObject namedDictionary;
        private Dictionary<string, DictionaryObject> dictionaries;
        private Dictionary<string, ImageDefinitionReactor> imageDefReactors;
        private Dictionary<Leader, string> leaderAnnotation;
        private Dictionary<Block, List<EntityObject>> blockEntities;
        private Dictionary<Group, List<string>> groupEntities;
        private Dictionary<DimensionStyle, string[]> dimStyleToHandles;
        private Dictionary<MLine, string> mLineToStyleNames;
        private Dictionary<Image, string> imgToImgDefHandles;
        private Dictionary<string, ImageDefinition> imgDefHandles;
        private Dictionary<Underlay, string> underlayToDefinitionHandles;
        private Dictionary<string, UnderlayDefinition> underlayDefHandles;
        private Dictionary<string, BlockRecord> blockRecordPointerToLayout;
        private List<Layout> orphanLayouts;

        private void CheckDimBlockName(string name)
        {
            if ((name.StartsWith("*D", StringComparison.OrdinalIgnoreCase) && int.TryParse(name.Remove(0, 2), out int num)) && (num > this.doc.DimensionBlocksGenerated))
            {
                this.doc.DimensionBlocksGenerated = num;
            }
        }

        private void CheckGroupName(string name)
        {
            if ((name.StartsWith("*A", StringComparison.OrdinalIgnoreCase) && int.TryParse(name.Remove(0, 2), out int num)) && (num > this.doc.GroupNamesGenerated))
            {
                this.doc.GroupNamesGenerated = num;
            }
        }

        public static string CheckHeaderVariable(Stream stream, string headerVariable, out bool isBinary)
        {
            ICodeValueReader reader;
            isBinary = IsBinary(stream);
            if (isBinary)
            {
                reader = new BinaryCodeValueReader(new BinaryReader(stream), Encoding.ASCII);
            }
            else
            {
                reader = new TextCodeValueReader(new StreamReader(stream));
            }
            reader.Next();
            while (reader.ReadString() != "EOF")
            {
                reader.Next();
                if (reader.ReadString() == "HEADER")
                {
                    reader.Next();
                    while (reader.ReadString() != "ENDSEC")
                    {
                        string str = reader.ReadString();
                        reader.Next();
                        if (str == headerVariable)
                        {
                            stream.Position = 0L;
                            return reader.ReadString();
                        }
                        while ((reader.Code != 0) && (reader.Code != 9))
                        {
                            reader.Next();
                        }
                    }
                    stream.Position = 0L;
                    return null;
                }
            }
            stream.Position = 0L;
            return null;
        }

        private void CreateObjectCollection(DictionaryObject namedDict)
        {
            string handle = null;
            string str2 = null;
            string str3 = null;
            string str4 = null;
            string str5 = null;
            string str6 = null;
            string str7 = null;
            foreach (KeyValuePair<string, string> pair in namedDict.Entries)
            {
                string s = pair.Value;
                switch (<PrivateImplementationDetails>.ComputeStringHash(s))
                {
                    case 0xb47db6a7:
                    {
                        if (s == "ACAD_LAYOUT")
                        {
                            goto Label_012F;
                        }
                        continue;
                    }
                    case 0xb49a8edf:
                    {
                        if (s == "ACAD_MLINESTYLE")
                        {
                            goto Label_0139;
                        }
                        continue;
                    }
                    case 0xc811d984:
                    {
                        if (s == "ACAD_DWFDEFINITIONS")
                        {
                            goto Label_0158;
                        }
                        continue;
                    }
                    case 0xe9085ebc:
                    {
                        if (s == "ACAD_DGNDEFINITIONS")
                        {
                            goto Label_014D;
                        }
                        continue;
                    }
                    case 0xffcae291:
                    {
                        if (s == "ACAD_PDFDEFINITIONS")
                        {
                            goto Label_0163;
                        }
                        continue;
                    }
                    case 0xc9b76704:
                    {
                        if (s == "ACAD_GROUP")
                        {
                            break;
                        }
                        continue;
                    }
                    case 0xd9a1e927:
                    {
                        if (s == "ACAD_IMAGE_DICT")
                        {
                            goto Label_0143;
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                handle = pair.Key;
                continue;
            Label_012F:
                str2 = pair.Key;
                continue;
            Label_0139:
                str3 = pair.Key;
                continue;
            Label_0143:
                str4 = pair.Key;
                continue;
            Label_014D:
                str5 = pair.Key;
                continue;
            Label_0158:
                str6 = pair.Key;
                continue;
            Label_0163:
                str7 = pair.Key;
            }
            this.doc.Groups = new Groups(this.doc, handle);
            this.doc.Layouts = new Layouts(this.doc, str2);
            this.doc.MlineStyles = new MLineStyles(this.doc, str3);
            this.doc.ImageDefinitions = new ImageDefinitions(this.doc, str4);
            this.doc.UnderlayDgnDefinitions = new UnderlayDgnDefinitions(this.doc, str5);
            this.doc.UnderlayDwfDefinitions = new UnderlayDwfDefinitions(this.doc, str6);
            this.doc.UnderlayPdfDefinitions = new UnderlayPdfDefinitions(this.doc, str7);
        }

        private void CreateTableCollection(string name, string handle)
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbSymbolTable");
            short capacity = 0;
            while (this.chunk.Code > 0)
            {
                if (this.chunk.Code == 70)
                {
                    capacity = this.chunk.ReadShort();
                }
                this.chunk.Next();
            }
            string s = name;
            switch (<PrivateImplementationDetails>.ComputeStringHash(s))
            {
                case 0x4087e5ee:
                    if (s == "BLOCK_RECORD")
                    {
                        this.doc.Blocks = new BlockRecords(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x5676b876:
                    if (s == "STYLE")
                    {
                        this.doc.TextStyles = new TextStyles(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x4ba33ea:
                    if (s == "DIMSTYLE")
                    {
                        this.doc.DimensionStyles = new DimensionStyles(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x19968db8:
                    if (s == "VIEW")
                    {
                        this.doc.Views = new Views(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x71ea82d6:
                    if (s == "LAYER")
                    {
                        this.doc.Layers = new Layers(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x7ef287ba:
                    if (s == "VPORT")
                    {
                        this.doc.VPorts = new VPorts(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x8894622e:
                    if (s == "UCS")
                    {
                        this.doc.UCSs = new UCSs(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0x971b0241:
                    if (s == "APPID")
                    {
                        this.doc.ApplicationRegistries = new ApplicationRegistries(this.doc, capacity, handle);
                        return;
                    }
                    break;

                case 0xac3c19f3:
                    if (s == "LTYPE")
                    {
                        this.doc.Linetypes = new Linetypes(this.doc, capacity, handle);
                        return;
                    }
                    break;
            }
            throw new Exception($"Unknown Table name {name} at position {this.chunk.CurrentPosition}");
        }

        private string DecodeEncodedNonAsciiCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            if (!this.decodedStrings.TryGetValue(text, out string str))
            {
                int length = text.Length;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    char ch = text[i];
                    if (((ch == '\\') && ((i + 6) < length)) && ((((text[i + 1] == 'U') || (text[i + 1] == 'u')) && (text[i + 2] == '+')) && int.TryParse(text.Substring(i + 3, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int num3)))
                    {
                        ch = (char) num3;
                        i += 6;
                    }
                    builder.Append(ch);
                }
                str = builder.ToString();
                this.decodedStrings.Add(text, str);
            }
            return str;
        }

        private ApplicationRegistry GetApplicationRegistry(string name)
        {
            if (this.doc.ApplicationRegistries.TryGetValue(name, out ApplicationRegistry registry))
            {
                return registry;
            }
            return this.doc.ApplicationRegistries.Add(new ApplicationRegistry(name));
        }

        private Block GetBlock(string name)
        {
            if (!this.doc.Blocks.TryGetValue(name, out Block block))
            {
                throw new ArgumentException("The block with name " + name + " does not exist.");
            }
            return block;
        }

        private DimensionStyle GetDimensionStyle(string name)
        {
            if (!TableObject.IsValidName(name))
            {
                name = "Standard";
            }
            if (this.doc.DimensionStyles.TryGetValue(name, out DimensionStyle style))
            {
                return style;
            }
            return this.doc.DimensionStyles.Add(new DimensionStyle(name));
        }

        private static string[] GetDimStylePrefixAndSuffix(string dimpost)
        {
            string str;
            string str2;
            int length = -1;
            for (int i = 0; i < dimpost.Length; i++)
            {
                if (((dimpost[i] == '<') && ((i + 1) < dimpost.Length)) && (dimpost[i + 1] == '>'))
                {
                    length = i;
                    break;
                }
            }
            if (length < 0)
            {
                str = dimpost;
                str2 = string.Empty;
            }
            else
            {
                str = dimpost.Substring(0, length);
                str2 = dimpost.Substring(length + 2, dimpost.Length - (length + 2));
            }
            return new string[] { str, str2 };
        }

        private Layer GetLayer(string name)
        {
            if (!TableObject.IsValidName(name))
            {
                name = "0";
            }
            if (this.doc.Layers.TryGetValue(name, out Layer layer))
            {
                return layer;
            }
            return this.doc.Layers.Add(new Layer(name));
        }

        private Linetype GetLinetype(string name)
        {
            if (!TableObject.IsValidName(name))
            {
                name = "Continuous";
            }
            if (this.doc.Linetypes.TryGetValue(name, out Linetype linetype))
            {
                return linetype;
            }
            return this.doc.Linetypes.Add(new Linetype(name));
        }

        private MLineStyle GetMLineStyle(string name)
        {
            if (!TableObject.IsValidName(name))
            {
                name = "Standard";
            }
            if (this.doc.MlineStyles.TryGetValue(name, out MLineStyle style))
            {
                return style;
            }
            return this.doc.MlineStyles.Add(new MLineStyle(name));
        }

        private TextStyle GetTextStyle(string name)
        {
            if (!TableObject.IsValidName(name))
            {
                name = "Standard";
            }
            if (this.doc.TextStyles.TryGetValue(name, out TextStyle style))
            {
                return style;
            }
            return this.doc.TextStyles.Add(new TextStyle(name));
        }

        public static bool IsBinary(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] buffer = reader.ReadBytes(0x16);
            StringBuilder builder = new StringBuilder(0x12);
            for (int i = 0; i < 0x12; i++)
            {
                builder.Append((char) buffer[i]);
            }
            reader.BaseStream.Position = 0L;
            return (builder.ToString() == "AutoCAD Binary DXF");
        }

        private static TextAlignment ObtainAlignment(short horizontal, short vertical)
        {
            TextAlignment baselineLeft = TextAlignment.BaselineLeft;
            if ((horizontal == 0) && (vertical == 3))
            {
                baselineLeft = TextAlignment.TopLeft;
            }
            else if ((horizontal == 1) && (vertical == 3))
            {
                baselineLeft = TextAlignment.TopCenter;
            }
            else if ((horizontal == 2) && (vertical == 3))
            {
                baselineLeft = TextAlignment.TopRight;
            }
            else if ((horizontal == 0) && (vertical == 2))
            {
                baselineLeft = TextAlignment.MiddleLeft;
            }
            else if ((horizontal == 1) && (vertical == 2))
            {
                baselineLeft = TextAlignment.MiddleCenter;
            }
            else if ((horizontal == 2) && (vertical == 2))
            {
                baselineLeft = TextAlignment.MiddleRight;
            }
            else if ((horizontal == 0) && (vertical == 1))
            {
                baselineLeft = TextAlignment.BottomLeft;
            }
            else if ((horizontal == 1) && (vertical == 1))
            {
                baselineLeft = TextAlignment.BottomCenter;
            }
            else if ((horizontal == 2) && (vertical == 1))
            {
                baselineLeft = TextAlignment.BottomRight;
            }
            else if ((horizontal == 0) && (vertical == 0))
            {
                baselineLeft = TextAlignment.BaselineLeft;
            }
            if ((horizontal == 1) && (vertical == 0))
            {
                return TextAlignment.BaselineCenter;
            }
            if ((horizontal == 2) && (vertical == 0))
            {
                return TextAlignment.BaselineRight;
            }
            if ((horizontal == 3) && (vertical == 0))
            {
                return TextAlignment.Aligned;
            }
            if ((horizontal == 4) && (vertical == 0))
            {
                return TextAlignment.Middle;
            }
            if ((horizontal == 5) && (vertical == 0))
            {
                baselineLeft = TextAlignment.Fit;
            }
            return baselineLeft;
        }

        private void PostProcesses()
        {
            foreach (KeyValuePair<DimensionStyle, string[]> pair in this.dimStyleToHandles)
            {
                DimensionStyle style = DimensionStyle.Default;
                BlockRecord objectByHandle = this.doc.GetObjectByHandle(pair.Value[0]) as BlockRecord;
                pair.Key.DimArrow1 = (objectByHandle == null) ? null : this.doc.Blocks[objectByHandle.Name];
                objectByHandle = this.doc.GetObjectByHandle(pair.Value[1]) as BlockRecord;
                pair.Key.DimArrow2 = (objectByHandle == null) ? null : this.doc.Blocks[objectByHandle.Name];
                objectByHandle = this.doc.GetObjectByHandle(pair.Value[2]) as BlockRecord;
                pair.Key.LeaderArrow = (objectByHandle == null) ? null : this.doc.Blocks[objectByHandle.Name];
                TextStyle style2 = this.doc.GetObjectByHandle(pair.Value[3]) as TextStyle;
                pair.Key.TextStyle = (style2 == null) ? this.doc.TextStyles[style.TextStyle.Name] : this.doc.TextStyles[style2.Name];
                Linetype linetype = this.doc.GetObjectByHandle(pair.Value[4]) as Linetype;
                pair.Key.DimLineLinetype = (linetype == null) ? this.doc.Linetypes[style.DimLineLinetype.Name] : this.doc.Linetypes[linetype.Name];
                linetype = this.doc.GetObjectByHandle(pair.Value[5]) as Linetype;
                pair.Key.ExtLine1Linetype = (linetype == null) ? this.doc.Linetypes[style.ExtLine1Linetype.Name] : this.doc.Linetypes[linetype.Name];
                linetype = this.doc.GetObjectByHandle(pair.Value[6]) as Linetype;
                pair.Key.ExtLine2Linetype = (linetype == null) ? this.doc.Linetypes[style.ExtLine2Linetype.Name] : this.doc.Linetypes[linetype.Name];
            }
            foreach (KeyValuePair<Image, string> pair2 in this.imgToImgDefHandles)
            {
                Image key = pair2.Key;
                key.Definition = this.imgDefHandles[pair2.Value];
                key.Definition.Reactors.Add(key.Handle, this.imageDefReactors[key.Handle]);
                double num = UnitHelper.ConversionFactor(this.doc.DrawingVariables.InsUnits, this.doc.RasterVariables.Units);
                key.Width *= num;
                key.Height *= num;
            }
            foreach (KeyValuePair<Underlay, string> pair3 in this.underlayToDefinitionHandles)
            {
                pair3.Key.Definition = this.underlayDefHandles[pair3.Value];
            }
            foreach (KeyValuePair<MLine, string> pair4 in this.mLineToStyleNames)
            {
                pair4.Key.Style = this.GetMLineStyle(pair4.Value);
            }
            foreach (KeyValuePair<Block, List<EntityObject>> pair5 in this.blockEntities)
            {
                Block key = pair5.Key;
                foreach (EntityObject obj2 in pair5.Value)
                {
                    key.Entities.Add(obj2);
                }
            }
            foreach (KeyValuePair<EntityObject, string> pair6 in this.entityList)
            {
                Layout layout;
                Block associatedBlock;
                if (pair6.Value == null)
                {
                    layout = this.doc.Layouts["Model"];
                    associatedBlock = layout.AssociatedBlock;
                }
                else
                {
                    associatedBlock = this.GetBlock(((BlockRecord) this.doc.GetObjectByHandle(pair6.Value)).Name);
                    layout = associatedBlock.Record.Layout;
                }
                Viewport key = pair6.Key as Viewport;
                if (key > null)
                {
                    if (key.Id == 1)
                    {
                        layout.Viewport = key;
                        layout.Viewport.Owner = associatedBlock;
                    }
                    else
                    {
                        this.doc.ActiveLayout = layout.Name;
                        this.doc.AddEntity(pair6.Key, false, false);
                    }
                }
                else
                {
                    Insert insert = pair6.Key as Insert;
                    if (insert > null)
                    {
                        double num2 = UnitHelper.ConversionFactor(this.doc.DrawingVariables.InsUnits, insert.Block.Record.Units);
                        insert.Scale *= num2;
                    }
                    this.doc.ActiveLayout = layout.Name;
                    this.doc.AddEntity(pair6.Key, false, false);
                }
            }
            foreach (Layout layout2 in this.doc.Layouts)
            {
                if ((layout2.Viewport != null) && string.IsNullOrEmpty(layout2.Viewport.Handle))
                {
                    this.doc.NumHandles = layout2.Viewport.AsignHandle(this.doc.NumHandles);
                }
            }
            foreach (KeyValuePair<Viewport, string> pair7 in this.viewports)
            {
                EntityObject objectByHandle = this.doc.GetObjectByHandle(pair7.Value) as EntityObject;
                if (objectByHandle > null)
                {
                    pair7.Key.ClippingBoundary = objectByHandle;
                }
            }
            foreach (KeyValuePair<Hatch, List<HatchBoundaryPath>> pair8 in this.hatchToPaths)
            {
                Hatch key = pair8.Key;
                foreach (HatchBoundaryPath path in pair8.Value)
                {
                    List<string> list = this.hatchContourns[path];
                    foreach (string str in list)
                    {
                        EntityObject objectByHandle = this.doc.GetObjectByHandle(str) as EntityObject;
                        if (objectByHandle > null)
                        {
                            path.AddContour(objectByHandle);
                        }
                    }
                    key.BoundaryPaths.Add(path);
                }
            }
            foreach (KeyValuePair<Leader, string> pair9 in this.leaderAnnotation)
            {
                EntityObject objectByHandle = this.doc.GetObjectByHandle(pair9.Value) as EntityObject;
                if (objectByHandle > null)
                {
                    pair9.Key.Annotation = objectByHandle;
                    pair9.Key.Update(false);
                }
            }
            foreach (KeyValuePair<Group, List<string>> pair10 in this.groupEntities)
            {
                foreach (string str2 in pair10.Value)
                {
                    EntityObject objectByHandle = this.doc.GetObjectByHandle(str2) as EntityObject;
                    if (objectByHandle > null)
                    {
                        pair10.Key.Entities.Add(objectByHandle);
                    }
                }
            }
            foreach (Dimension dimension in this.doc.Dimensions)
            {
                if (dimension.XData.TryGetValue("ACAD", out XData data))
                {
                    dimension.StyleOverrides.AddRange(this.ReadDimensionStyleOverrideXData(data, dimension));
                }
            }
            foreach (Leader leader in this.doc.Leaders)
            {
                if (leader.XData.TryGetValue("ACAD", out XData data2))
                {
                    leader.StyleOverrides.AddRange(this.ReadDimensionStyleOverrideXData(data2, leader));
                }
            }
        }

        public DxfDocument Read(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            string str = CheckHeaderVariable(stream, "$DWGCODEPAGE", out this.isBinary);
            try
            {
                if (this.isBinary)
                {
                    Encoding encoding;
                    if (DxfDocument.CheckDxfFileVersion(stream, out this.isBinary) >= DxfVersion.AutoCad2007)
                    {
                        encoding = Encoding.UTF8;
                    }
                    else if (string.IsNullOrEmpty(str))
                    {
                        encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage);
                    }
                    else
                    {
                        char[] separator = new char[] { '_' };
                        encoding = Encoding.GetEncoding(int.TryParse(str.Split(separator)[1], out int num) ? num : Encoding.Default.WindowsCodePage);
                    }
                    this.chunk = new BinaryCodeValueReader(new BinaryReader(stream), encoding);
                }
                else
                {
                    Encoding encoding;
                    Encoding type = EncodingType.GetType(stream);
                    if (((type.EncodingName == Encoding.UTF8.EncodingName) || (type.EncodingName == Encoding.BigEndianUnicode.EncodingName)) || (type.EncodingName == Encoding.Unicode.EncodingName))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else if (string.IsNullOrEmpty(str))
                    {
                        encoding = Encoding.GetEncoding(Encoding.Default.WindowsCodePage);
                    }
                    else
                    {
                        char[] separator = new char[] { '_' };
                        encoding = Encoding.GetEncoding(!int.TryParse(str.Split(separator)[1], out int num2) ? Encoding.Default.WindowsCodePage : num2);
                    }
                    this.chunk = new TextCodeValueReader(new StreamReader(stream, encoding, true));
                }
            }
            catch (Exception exception)
            {
                throw new IOException("Unknown error opening the reader.", exception);
            }
            this.doc = new DxfDocument(new HeaderVariables(), false);
            this.entityList = new Dictionary<EntityObject, string>();
            this.viewports = new Dictionary<Viewport, string>();
            this.hatchToPaths = new Dictionary<Hatch, List<HatchBoundaryPath>>();
            this.hatchContourns = new Dictionary<HatchBoundaryPath, List<string>>();
            this.decodedStrings = new Dictionary<string, string>();
            this.leaderAnnotation = new Dictionary<Leader, string>();
            this.nestedInserts = new Dictionary<Insert, string>();
            this.nestedDimensions = new Dictionary<Dimension, string>();
            this.blockRecords = new Dictionary<string, BlockRecord>(StringComparer.OrdinalIgnoreCase);
            this.blockEntities = new Dictionary<Block, List<EntityObject>>();
            this.dictionaries = new Dictionary<string, DictionaryObject>(StringComparer.OrdinalIgnoreCase);
            this.groupEntities = new Dictionary<Group, List<string>>();
            this.dimStyleToHandles = new Dictionary<DimensionStyle, string[]>();
            this.imageDefReactors = new Dictionary<string, ImageDefinitionReactor>(StringComparer.OrdinalIgnoreCase);
            this.imgDefHandles = new Dictionary<string, ImageDefinition>(StringComparer.OrdinalIgnoreCase);
            this.imgToImgDefHandles = new Dictionary<Image, string>();
            this.mLineToStyleNames = new Dictionary<MLine, string>();
            this.underlayToDefinitionHandles = new Dictionary<Underlay, string>();
            this.underlayDefHandles = new Dictionary<string, UnderlayDefinition>();
            this.blockRecordPointerToLayout = new Dictionary<string, BlockRecord>(StringComparer.OrdinalIgnoreCase);
            this.orphanLayouts = new List<Layout>();
            this.chunk.Next();
            this.doc.Comments.Clear();
            while (this.chunk.Code == 0x3e7)
            {
                this.doc.Comments.Add(this.chunk.ReadString());
                this.chunk.Next();
            }
            while (this.chunk.ReadString() != "EOF")
            {
                if (this.chunk.ReadString() == "SECTION")
                {
                    this.chunk.Next();
                    string s = this.chunk.ReadString();
                    switch (<PrivateImplementationDetails>.ComputeStringHash(s))
                    {
                        case 0x76300d04:
                            if (s == "TABLES")
                            {
                                goto Label_0480;
                            }
                            goto Label_04B6;

                        case 0x86efa8e0:
                            if (s == "HEADER")
                            {
                                break;
                            }
                            goto Label_04B6;

                        case 0x561da07:
                            if (s == "CLASSES")
                            {
                                goto Label_0477;
                            }
                            goto Label_04B6;

                        case 0x4f59141c:
                            if (s == "THUMBNAILIMAGE")
                            {
                                goto Label_04A4;
                            }
                            goto Label_04B6;

                        case 0xa6bcec43:
                            if (s == "BLOCKS")
                            {
                                goto Label_0489;
                            }
                            goto Label_04B6;

                        case 0xb21321c0:
                            if (s == "ACDSDATA")
                            {
                                goto Label_04AD;
                            }
                            goto Label_04B6;

                        case 0xf281dd8b:
                            if (s == "OBJECTS")
                            {
                                goto Label_049B;
                            }
                            goto Label_04B6;

                        case 0xfab190ea:
                            if (s == "ENTITIES")
                            {
                                goto Label_0492;
                            }
                            goto Label_04B6;

                        default:
                            goto Label_04B6;
                    }
                    this.ReadHeader();
                }
                goto Label_04D2;
            Label_0477:
                this.ReadClasses();
                goto Label_04D2;
            Label_0480:
                this.ReadTables();
                goto Label_04D2;
            Label_0489:
                this.ReadBlocks();
                goto Label_04D2;
            Label_0492:
                this.ReadEntities();
                goto Label_04D2;
            Label_049B:
                this.ReadObjects();
                goto Label_04D2;
            Label_04A4:
                this.ReadThumbnailImage();
                goto Label_04D2;
            Label_04AD:
                this.ReadAcdsData();
                goto Label_04D2;
            Label_04B6:
                throw new Exception($"Unknown section {this.chunk.ReadString()}.");
            Label_04D2:
                this.chunk.Next();
            }
            stream.Position = 0L;
            this.PostProcesses();
            this.doc.Layers.Add(Layer.Default);
            this.doc.Linetypes.Add(Linetype.ByLayer);
            this.doc.Linetypes.Add(Linetype.ByBlock);
            this.doc.Linetypes.Add(Linetype.Continuous);
            this.doc.TextStyles.Add(TextStyle.Default);
            this.doc.ApplicationRegistries.Add(ApplicationRegistry.Default);
            this.doc.DimensionStyles.Add(DimensionStyle.Default);
            this.doc.MlineStyles.Add(MLineStyle.Default);
            this.doc.ActiveLayout = "Model";
            return this.doc;
        }

        private Insert ReadAcadTable(bool isBlockEntity)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            Vector3 unitX = Vector3.UnitX;
            string name = null;
            Block block = null;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 11:
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!isBlockEntity)
                        {
                            block = this.GetBlock(name);
                        }
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(str2));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 vector4 = MathHelper.Transform(zero, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            Insert key = new Insert(new List<netDxf.Entities.Attribute>()) {
                Block = block,
                Position = vector4,
                Normal = unitZ
            };
            this.doc.NumHandles = key.EndSequence.AsignHandle(this.doc.NumHandles);
            key.XData.AddRange(items);
            if (isBlockEntity)
            {
                this.nestedInserts.Add(key, name);
            }
            return key;
        }

        private void ReadAcdsData()
        {
            Debug.Assert(this.chunk.ReadString() == "ACDSDATA");
            while (this.chunk.ReadString() != "ENDSEC")
            {
                do
                {
                    this.chunk.Next();
                }
                while (this.chunk.Code > 0);
            }
        }

        private AlignedDimension ReadAlignedDimension(Vector3 defPoint, Vector3 normal)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 14:
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x17:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x18:
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x21:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x22:
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            List<Vector3> points = new List<Vector3> {
                zero,
                vector2,
                defPoint
            };
            IList<Vector3> list2 = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            AlignedDimension dimension1 = new AlignedDimension();
            Vector3 vector3 = list2[0];
            vector3 = list2[0];
            dimension1.FirstReferencePoint = new Vector2(vector3.X, vector3.Y);
            vector3 = list2[1];
            vector3 = list2[1];
            dimension1.SecondReferencePoint = new Vector2(vector3.X, vector3.Y);
            vector3 = list2[2];
            dimension1.Elevation = vector3.Z;
            dimension1.Normal = normal;
            AlignedDimension dimension = dimension1;
            vector3 = list2[2];
            vector3 = list2[2];
            dimension.SetDimensionLinePosition(new Vector2(vector3.X, vector3.Y));
            dimension.XData.AddRange(items);
            return dimension;
        }

        private Angular2LineDimension ReadAngular2LineDimension(Vector3 defPoint, Vector3 normal)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector3 = Vector3.Zero;
            Vector3 vector4 = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 14:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 15:
                    {
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x10:
                    {
                        vector4.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x18:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x19:
                    {
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1a:
                    {
                        vector4.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x21:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x22:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x23:
                    {
                        vector3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x24:
                    {
                        vector4.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            Vector3[] points = new Vector3[] { zero, vector2, vector3, defPoint };
            IList<Vector3> list2 = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 vector11 = list2[0];
            vector11 = list2[0];
            Vector2 vector5 = new Vector2(vector11.X, vector11.Y);
            vector11 = list2[0];
            vector11 = list2[0];
            Vector2 vector6 = new Vector2(vector11.X, vector11.Y);
            vector11 = list2[0];
            vector11 = list2[0];
            Vector2 vector7 = new Vector2(vector11.X, vector11.Y);
            vector11 = list2[0];
            vector11 = list2[0];
            Vector2 vector8 = new Vector2(vector11.X, vector11.Y);
            Vector2 u = vector6 - vector5;
            Vector2 v = vector8 - vector7;
            if (Vector2.AreParallel(u, v))
            {
                return null;
            }
            Angular2LineDimension dimension = new Angular2LineDimension {
                StartFirstLine = vector5,
                EndFirstLine = vector6,
                StartSecondLine = vector7,
                EndSecondLine = vector8,
                Elevation = vector4.Z
            };
            dimension.SetDimensionLinePosition(new Vector2(vector4.X, vector4.Y));
            dimension.XData.AddRange(items);
            return dimension;
        }

        private Angular3PointDimension ReadAngular3PointDimension(Vector3 defPoint, Vector3 normal)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector3 = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 14:
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 15:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x17:
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x18:
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x19:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x21:
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x22:
                        vector3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x23:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3[] points = new Vector3[] { zero, vector2, vector3, defPoint };
            IList<Vector3> list2 = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Angular3PointDimension dimension1 = new Angular3PointDimension();
            Vector3 vector4 = list2[0];
            vector4 = list2[0];
            dimension1.CenterPoint = new Vector2(vector4.X, vector4.Y);
            vector4 = list2[1];
            vector4 = list2[1];
            dimension1.StartPoint = new Vector2(vector4.X, vector4.Y);
            vector4 = list2[2];
            vector4 = list2[2];
            dimension1.EndPoint = new Vector2(vector4.X, vector4.Y);
            Angular3PointDimension dimension = dimension1;
            vector4 = list2[3];
            vector4 = list2[3];
            dimension.SetDimensionLinePosition(new Vector2(vector4.X, vector4.Y));
            dimension.XData.AddRange(items);
            return dimension;
        }

        private ApplicationRegistry ReadApplicationId()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbRegAppTableRecord");
            string str = string.Empty;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                if (this.chunk.Code == 2)
                {
                    str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                    this.chunk.Next();
                }
                else
                {
                    this.chunk.Next();
                }
            }
            if (string.IsNullOrEmpty(str) || !TableObject.IsValidName(str))
            {
                return null;
            }
            return new ApplicationRegistry(str, false);
        }

        private netDxf.Entities.Arc ReadArc()
        {
            Vector3 zero = Vector3.Zero;
            double num = 1.0;
            double num2 = 0.0;
            double num3 = 180.0;
            double num4 = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x27:
                        num4 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                        num = this.chunk.ReadDouble();
                        if (num <= 0.0)
                        {
                            num = 1.0;
                        }
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 50:
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x33:
                        num3 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 vector3 = MathHelper.Transform(zero, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            netDxf.Entities.Arc arc = new netDxf.Entities.Arc {
                Center = vector3,
                Radius = num,
                StartAngle = num2,
                EndAngle = num3,
                Thickness = num4,
                Normal = unitZ
            };
            arc.XData.AddRange(items);
            return arc;
        }

        private netDxf.Entities.Attribute ReadAttribute(Block block, bool isBlockEntity = false)
        {
            string str = null;
            Layer layer = Layer.Default;
            AciColor byLayer = AciColor.ByLayer;
            Linetype linetype = Linetype.ByLayer;
            Lineweight lineweight = Lineweight.ByLayer;
            double num = 1.0;
            bool flag = true;
            Transparency transparency = Transparency.ByLayer;
            AttributeFlags visible = AttributeFlags.Visible;
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            TextStyle textStyle = TextStyle.Default;
            double num2 = 0.0;
            double number = 0.0;
            double num4 = 0.0;
            short horizontal = 0;
            short vertical = 0;
            double num7 = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new Exception($"Premature end of entity {"ATTRIB"} definition.");

                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 8:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(name);
                        this.chunk.Next();
                        break;
                    }
                    case 0x30:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0:
                        throw new Exception($"Premature end of entity {"ATTRIB"} definition.");

                    case 6:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        linetype = this.GetLinetype(name);
                        this.chunk.Next();
                        break;
                    }
                    case 60:
                        flag = this.chunk.ReadShort() == 0;
                        this.chunk.Next();
                        break;

                    case 0x3e:
                        if (!byLayer.UseTrueColor)
                        {
                            byLayer = AciColor.FromCadIndex(this.chunk.ReadShort());
                        }
                        this.chunk.Next();
                        break;

                    case 370:
                        lineweight = (Lineweight) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 420:
                        byLayer = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;

                    case 440:
                        transparency = Transparency.FromAlphaValue(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            string tag = null;
            AttributeDefinition definition = null;
            object obj2 = null;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                    {
                        obj2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        tag = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!isBlockEntity)
                        {
                            block.AttributeDefinitions.TryGetValue(tag, out definition);
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 7:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        textStyle = this.GetTextStyle(name);
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 50:
                    {
                        num7 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x33:
                    {
                        num4 = MathHelper.NormalizeAngle(this.chunk.ReadDouble());
                        if (num4 > 180.0)
                        {
                            num4 -= 360.0;
                        }
                        if ((num4 < -85.0) || (num4 > 85.0))
                        {
                            num4 = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        number = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        visible = (AttributeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        horizontal = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4a:
                    {
                        vertical = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            TextAlignment alignment = ObtainAlignment(horizontal, vertical);
            Vector3 point = (alignment == TextAlignment.BaselineLeft) ? zero : vector2;
            Vector3 vector5 = MathHelper.Transform(point, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            return new netDxf.Entities.Attribute { 
                Handle = str,
                Color = byLayer,
                Layer = layer,
                Linetype = linetype,
                Lineweight = lineweight,
                LinetypeScale = num,
                Transparency = transparency,
                IsVisible = flag,
                Definition = definition,
                Tag = tag,
                Position = vector5,
                Normal = unitZ,
                Alignment = alignment,
                Value = obj2,
                Flags = visible,
                Style = textStyle,
                Height = num2,
                WidthFactor = MathHelper.IsZero(number) ? textStyle.WidthFactor : number,
                ObliqueAngle = num4,
                Rotation = num7
            };
        }

        private AttributeDefinition ReadAttributeDefinition()
        {
            string tag = string.Empty;
            string str2 = string.Empty;
            object obj2 = null;
            AttributeFlags visible = AttributeFlags.Visible;
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            TextStyle textStyle = TextStyle.Default;
            double num = 0.0;
            double number = 0.0;
            short horizontal = 0;
            short vertical = 0;
            double num5 = 0.0;
            double num6 = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 1:
                    {
                        obj2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        tag = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 3:
                    {
                        str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 7:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        textStyle = this.GetTextStyle(name);
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        number = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 50:
                    {
                        num5 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x33:
                    {
                        num6 = MathHelper.NormalizeAngle(this.chunk.ReadDouble());
                        if (num6 > 180.0)
                        {
                            num6 -= 360.0;
                        }
                        if ((num6 < -85.0) || (num6 > 85.0))
                        {
                            num6 = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        visible = (AttributeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        horizontal = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4a:
                    {
                        vertical = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            TextAlignment alignment = ObtainAlignment(horizontal, vertical);
            Vector3 point = (alignment == TextAlignment.BaselineLeft) ? zero : vector2;
            Vector3 vector5 = MathHelper.Transform(point, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            return new AttributeDefinition(tag) { 
                Position = vector5,
                Normal = unitZ,
                Alignment = alignment,
                Prompt = str2,
                Value = obj2,
                Flags = visible,
                Style = textStyle,
                Height = num,
                WidthFactor = MathHelper.IsZero(number) ? textStyle.WidthFactor : number,
                ObliqueAngle = num6,
                Rotation = num5
            };
        }

        private Block ReadBlock()
        {
            Block block;
            Debug.Assert(this.chunk.ReadString() == "BLOCK");
            Layer layer = Layer.Default;
            string key = string.Empty;
            string str2 = string.Empty;
            string xrefFile = string.Empty;
            BlockTypeFlags none = BlockTypeFlags.None;
            Vector3 zero = Vector3.Zero;
            List<EntityObject> list = new List<EntityObject>();
            List<AttributeDefinition> list2 = new List<AttributeDefinition>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                short code = this.chunk.Code;
                if (code <= 20)
                {
                    switch (code)
                    {
                        case 1:
                        {
                            xrefFile = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                            this.chunk.Next();
                            continue;
                        }
                        case 2:
                        {
                            key = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                            this.chunk.Next();
                            continue;
                        }
                        case 3:
                        {
                            this.chunk.Next();
                            continue;
                        }
                        case 5:
                        {
                            str2 = this.chunk.ReadHex();
                            this.chunk.Next();
                            continue;
                        }
                        case 8:
                        {
                            string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                            layer = this.GetLayer(name);
                            this.chunk.Next();
                            continue;
                        }
                        case 10:
                        {
                            zero.X = this.chunk.ReadDouble();
                            this.chunk.Next();
                            continue;
                        }
                        case 20:
                            goto Label_0199;
                    }
                    goto Label_01E9;
                }
                if (code == 30)
                {
                    goto Label_01BA;
                }
                if (code != 70)
                {
                    goto Label_01E9;
                }
                none = (BlockTypeFlags) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0199:
                zero.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_01BA:
                zero.Z = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_01E9:
                this.chunk.Next();
            }
            while (this.chunk.ReadString() != "ENDBLK")
            {
                EntityObject item = this.ReadEntity(true);
                if (item > null)
                {
                    AttributeDefinition definition = item as AttributeDefinition;
                    if (definition > null)
                    {
                        list2.Add(definition);
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
            }
            this.chunk.Next();
            string str4 = string.Empty;
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        str4 = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 8:
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (!this.blockRecords.TryGetValue(key, out BlockRecord record))
            {
                throw new Exception($"The block record {key} is not defined.");
            }
            if (none.HasFlag(BlockTypeFlags.XRef))
            {
                block = new Block(key, xrefFile) {
                    Handle = str2,
                    Owner = record,
                    Origin = zero,
                    Layer = layer,
                    Flags = none
                };
            }
            else
            {
                block = new Block(key, false, null, null) {
                    Handle = str2,
                    Owner = record,
                    Origin = zero,
                    Layer = layer,
                    Flags = none
                };
            }
            block.End.Handle = str4;
            if (key.StartsWith("*Paper_Space", StringComparison.OrdinalIgnoreCase))
            {
                foreach (EntityObject obj3 in list)
                {
                    this.entityList.Add(obj3, record.Handle);
                }
                return block;
            }
            foreach (AttributeDefinition definition2 in list2)
            {
                if (!block.AttributeDefinitions.ContainsTag(definition2.Tag))
                {
                    block.AttributeDefinitions.Add(definition2);
                }
            }
            this.blockEntities.Add(block, list);
            return block;
        }

        private BlockRecord ReadBlockRecord()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbBlockTableRecord");
            string str = null;
            DrawingUnits unitless = DrawingUnits.Unitless;
            bool flag = true;
            bool flag2 = false;
            List<XData> items = new List<XData>();
            string str2 = null;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 70:
                        unitless = (DrawingUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 280:
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 0x119:
                        flag2 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 340:
                        str2 = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            this.CheckDimBlockName(str);
            BlockRecord record = new BlockRecord(str) {
                Units = unitless,
                AllowExploding = flag,
                ScaleUniformly = flag2
            };
            record.XData.AddRange(items);
            if (record.XData.TryGetValue("ACAD", out XData data))
            {
                IEnumerator<XDataRecord> enumerator = data.XDataRecord.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    XDataRecord current = enumerator.Current;
                    if ((current.Code == XDataCode.String) && string.Equals((string) current.Value, "DesignCenter Data", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                        current = enumerator.Current;
                        if ((current.Code != XDataCode.ControlString) || !enumerator.MoveNext())
                        {
                            break;
                        }
                        current = enumerator.Current;
                        while (current.Code != XDataCode.ControlString)
                        {
                            if (!enumerator.MoveNext())
                            {
                                break;
                            }
                            current = enumerator.Current;
                            if (current.Code == XDataCode.Int16)
                            {
                                record.Units = (DrawingUnits) ((short) current.Value);
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(str2) && (str2 != "0"))
            {
                this.blockRecordPointerToLayout.Add(str2, record);
            }
            return record;
        }

        private void ReadBlocks()
        {
            Debug.Assert(this.chunk.ReadString() == "BLOCKS");
            Dictionary<string, Block> dictionary = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);
            this.chunk.Next();
            while (this.chunk.ReadString() != "ENDSEC")
            {
                if (this.chunk.ReadString() == "BLOCK")
                {
                    Block block = this.ReadBlock();
                    dictionary.Add(block.Name, block);
                }
                else
                {
                    this.chunk.Next();
                }
            }
            foreach (KeyValuePair<Insert, string> pair in this.nestedInserts)
            {
                Insert key = pair.Key;
                key.Block = dictionary[pair.Value];
                foreach (netDxf.Entities.Attribute attribute in key.Attributes)
                {
                    if (key.Block.AttributeDefinitions.TryGetValue(attribute.Tag, out AttributeDefinition definition))
                    {
                        attribute.Definition = definition;
                    }
                    attribute.Owner = key;
                }
                if (key.Owner > null)
                {
                    double num = UnitHelper.ConversionFactor(key.Owner.Record.Units, key.Block.Record.Units);
                    key.Scale *= num;
                }
            }
            foreach (KeyValuePair<Dimension, string> pair2 in this.nestedDimensions)
            {
                pair2.Key.Block = dictionary[pair2.Value];
            }
            foreach (Block block2 in dictionary.Values)
            {
                this.doc.Blocks.Add(block2, false);
            }
        }

        private Circle ReadCircle()
        {
            Vector3 zero = Vector3.Zero;
            double num = 1.0;
            double num2 = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x27:
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                        num = this.chunk.ReadDouble();
                        if (num <= 0.0)
                        {
                            num = 1.0;
                        }
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 vector3 = MathHelper.Transform(zero, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            Circle circle = new Circle {
                Center = vector3,
                Radius = num,
                Thickness = num2,
                Normal = unitZ
            };
            circle.XData.AddRange(items);
            return circle;
        }

        private void ReadClasses()
        {
            Debug.Assert(this.chunk.ReadString() == "CLASSES");
            this.chunk.Next();
            while (this.chunk.ReadString() != "ENDSEC")
            {
                do
                {
                    this.chunk.Next();
                }
                while (this.chunk.Code > 0);
            }
        }

        private DiametricDimension ReadDiametricDimension(Vector3 defPoint, Vector3 midtxtPoint, Vector3 normal)
        {
            Vector3 zero = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 15:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x19:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x23:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            List<Vector3> points = new List<Vector3> {
                zero,
                defPoint
            };
            IList<Vector3> list2 = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 u = Vector3.MidPoint(list2[0], list2[1]);
            double num = Vector3.Distance(u, midtxtPoint);
            DiametricDimension dimension1 = new DiametricDimension {
                CenterPoint = new Vector2(u.X, u.Y)
            };
            Vector3 vector3 = list2[0];
            vector3 = list2[0];
            dimension1.ReferencePoint = new Vector2(vector3.X, vector3.Y);
            vector3 = list2[1];
            dimension1.Elevation = vector3.Z;
            dimension1.Normal = normal;
            dimension1.Offset = num;
            DiametricDimension dimension = dimension1;
            dimension.XData.AddRange(items);
            return dimension;
        }

        private DictionaryObject ReadDictionary()
        {
            string str = null;
            DictionaryCloningFlags keepExisting = DictionaryCloningFlags.KeepExisting;
            bool flag = false;
            int num = 0;
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 3:
                        num++;
                        list.Add(this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString()));
                        this.chunk.Next();
                        break;

                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 280:
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 0x119:
                        keepExisting = (DictionaryCloningFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 330:
                        this.chunk.Next();
                        break;

                    case 350:
                        list2.Add(this.chunk.ReadHex());
                        this.chunk.Next();
                        break;

                    case 360:
                        list2.Add(this.chunk.ReadHex());
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            DictionaryObject obj2 = new DictionaryObject(null) {
                Handle = str,
                IsHardOwner = flag,
                Cloning = keepExisting
            };
            for (int i = 0; i < num; i++)
            {
                string key = list2[i];
                if (key == null)
                {
                    throw new NullReferenceException("Null handle in dictionary.");
                }
                obj2.Entries.Add(key, list[i]);
            }
            return obj2;
        }

        private Dimension ReadDimension(bool isBlockEntity)
        {
            string name = null;
            Block block = null;
            Dimension dimension;
            Vector3 zero = Vector3.Zero;
            Vector3 midtxtPoint = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            DimensionTypeFlags linear = DimensionTypeFlags.Linear;
            MTextAttachmentPoint bottomCenter = MTextAttachmentPoint.BottomCenter;
            MTextLineSpacingStyle atLeast = MTextLineSpacingStyle.AtLeast;
            DimensionStyle dimensionStyle = DimensionStyle.Default;
            double rotation = 0.0;
            double num2 = 1.0;
            bool flag = false;
            string str2 = null;
            this.chunk.Next();
            while (!flag)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                    {
                        str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        char[] trimChars = new char[] { ' ', '\t' };
                        if (string.IsNullOrEmpty(str2.Trim(trimChars)))
                        {
                            str2 = " ";
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!isBlockEntity)
                        {
                            block = this.GetBlock(name);
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 3:
                    {
                        string dimStyle = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(dimStyle))
                        {
                            dimStyle = this.doc.DrawingVariables.DimStyle;
                        }
                        dimensionStyle = this.GetDimensionStyle(dimStyle);
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        midtxtPoint.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        midtxtPoint.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        midtxtPoint.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        num2 = this.chunk.ReadDouble();
                        if ((num2 < 0.25) || (num2 > 4.0))
                        {
                            num2 = 1.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x33:
                    {
                        rotation = 360.0 - this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        linear = (DimensionTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        bottomCenter = (MTextAttachmentPoint) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        atLeast = (MTextLineSpacingStyle) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 100:
                    {
                        switch (this.chunk.ReadString())
                        {
                            case "AcDbAlignedDimension":
                            case "AcDbRadialDimension":
                            case "AcDbDiametricDimension":
                            case "AcDb3PointAngularDimension":
                            case "AcDb2LineAngularDimension":
                            case "AcDbOrdinateDimension":
                                flag = true;
                                break;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            DimensionTypeFlags flags2 = linear;
            OrdinateDimensionAxis y = OrdinateDimensionAxis.Y;
            if (flags2.HasFlag(DimensionTypeFlags.BlockReference))
            {
                flags2 -= 0x20;
            }
            if (flags2.HasFlag(DimensionTypeFlags.OrdinteType))
            {
                y = OrdinateDimensionAxis.X;
                flags2 -= 0x40;
            }
            if (flags2.HasFlag(DimensionTypeFlags.UserTextPosition))
            {
                flags2 -= 0x80;
            }
            switch (flags2)
            {
                case DimensionTypeFlags.Linear:
                    dimension = this.ReadLinearDimension(zero, unitZ);
                    break;

                case DimensionTypeFlags.Aligned:
                    dimension = this.ReadAlignedDimension(zero, unitZ);
                    break;

                case DimensionTypeFlags.Angular:
                    dimension = this.ReadAngular2LineDimension(zero, unitZ);
                    break;

                case DimensionTypeFlags.Diameter:
                    dimension = this.ReadDiametricDimension(zero, midtxtPoint, unitZ);
                    break;

                case DimensionTypeFlags.Radius:
                    dimension = this.ReadRadialDimension(zero, midtxtPoint, unitZ);
                    break;

                case DimensionTypeFlags.Angular3Point:
                    dimension = this.ReadAngular3PointDimension(zero, unitZ);
                    break;

                case DimensionTypeFlags.Ordinate:
                    dimension = this.ReadOrdinateDimension(zero, y, unitZ, rotation);
                    break;

                default:
                    throw new ArgumentException("The dimension type: " + flags2 + " is not implemented or unknown.");
            }
            if (dimension == null)
            {
                return null;
            }
            if (name == null)
            {
                return null;
            }
            dimension.Style = dimensionStyle;
            dimension.Block = block;
            dimension.DefinitionPoint = zero;
            dimension.MidTextPoint = midtxtPoint;
            dimension.AttachmentPoint = bottomCenter;
            dimension.LineSpacingStyle = atLeast;
            dimension.LineSpacingFactor = num2;
            dimension.Normal = unitZ;
            dimension.UserText = str2;
            if (isBlockEntity)
            {
                this.nestedDimensions.Add(dimension, name);
            }
            return dimension;
        }

        private DimensionStyle ReadDimensionStyle()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbDimStyleTableRecord");
            DimensionStyle style = DimensionStyle.Default;
            string str = null;
            AciColor dimLineColor = style.DimLineColor;
            string str2 = string.Empty;
            Lineweight dimLineLineweight = style.DimLineLineweight;
            bool flag = false;
            bool flag2 = false;
            double dimLineExtend = style.DimLineExtend;
            double dimBaselineSpacing = style.DimBaselineSpacing;
            AciColor extLineColor = style.ExtLineColor;
            string str3 = string.Empty;
            string str4 = string.Empty;
            Lineweight extLineLineweight = style.ExtLineLineweight;
            bool flag3 = false;
            bool flag4 = false;
            double extLineOffset = style.ExtLineOffset;
            double extLineExtend = style.ExtLineExtend;
            double arrowSize = style.ArrowSize;
            double centerMarkSize = style.CenterMarkSize;
            bool flag5 = false;
            string str5 = string.Empty;
            string str6 = string.Empty;
            string str7 = string.Empty;
            string str8 = string.Empty;
            double dimScaleOverall = style.DimScaleOverall;
            short dIMTIH = style.DIMTIH;
            short dIMTOH = style.DIMTOH;
            string str9 = string.Empty;
            AciColor textColor = style.TextColor;
            double textHeight = style.TextHeight;
            short dIMJUST = style.DIMJUST;
            short dIMTAD = style.DIMTAD;
            double textOffset = style.TextOffset;
            double dimScaleLinear = style.DimScaleLinear;
            short angularPrecision = style.AngularPrecision;
            short lengthPrecision = style.LengthPrecision;
            string dimpost = string.Empty;
            char decimalSeparator = style.DecimalSeparator;
            AngleUnitType dimAngularUnits = style.DimAngularUnits;
            LinearUnitType dimLengthUnits = style.DimLengthUnits;
            FractionFormatType fractionalType = style.FractionalType;
            double num17 = 0.0;
            short num18 = 0;
            short num19 = 3;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                    {
                        str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 3:
                    {
                        dimpost = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        dimScaleOverall = this.chunk.ReadDouble();
                        if (dimScaleOverall <= 0.0)
                        {
                            dimScaleOverall = style.DimScaleOverall;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        arrowSize = this.chunk.ReadDouble();
                        if (arrowSize < 0.0)
                        {
                            arrowSize = style.ArrowSize;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2a:
                    {
                        extLineOffset = this.chunk.ReadDouble();
                        if (extLineOffset < 0.0)
                        {
                            extLineOffset = style.ExtLineOffset;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2b:
                    {
                        dimBaselineSpacing = this.chunk.ReadDouble();
                        if (dimBaselineSpacing < 0.0)
                        {
                            dimBaselineSpacing = style.DimBaselineSpacing;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2c:
                    {
                        extLineExtend = this.chunk.ReadDouble();
                        if (extLineExtend < 0.0)
                        {
                            extLineExtend = style.ExtLineExtend;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2d:
                    {
                        num17 = this.chunk.ReadDouble();
                        if (num17 < 0.0)
                        {
                            num17 = style.ExtLineExtend;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2e:
                    {
                        dimLineExtend = this.chunk.ReadDouble();
                        if (dimLineExtend < 0.0)
                        {
                            dimLineExtend = style.DimLineExtend;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x49:
                    {
                        dIMTIH = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4a:
                    {
                        dIMTOH = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4b:
                    {
                        flag3 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4c:
                    {
                        flag4 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4d:
                    {
                        dIMTAD = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4e:
                    {
                        num18 = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4f:
                    {
                        num19 = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 140:
                    {
                        textHeight = this.chunk.ReadDouble();
                        if (textHeight <= 0.0)
                        {
                            textHeight = style.TextHeight;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x8d:
                    {
                        centerMarkSize = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x90:
                    {
                        dimScaleLinear = this.chunk.ReadDouble();
                        if (MathHelper.IsZero(dimScaleLinear))
                        {
                            dimScaleLinear = 1.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0xad:
                    {
                        flag5 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 0xb0:
                    {
                        dimLineColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 0xb1:
                    {
                        extLineColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 0xb2:
                    {
                        textColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 0xb3:
                    {
                        angularPrecision = this.chunk.ReadShort();
                        if (angularPrecision < 0)
                        {
                            angularPrecision = style.AngularPrecision;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x93:
                    {
                        textOffset = this.chunk.ReadDouble();
                        if (textOffset < 0.0)
                        {
                            textOffset = style.TextOffset;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x10f:
                    {
                        lengthPrecision = this.chunk.ReadShort();
                        if (lengthPrecision < 0)
                        {
                            lengthPrecision = style.LengthPrecision;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x113:
                    {
                        dimAngularUnits = (AngleUnitType) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x114:
                    {
                        fractionalType = (FractionFormatType) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x115:
                    {
                        dimLengthUnits = (LinearUnitType) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x116:
                    {
                        decimalSeparator = (char) ((ushort) this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 280:
                    {
                        dIMJUST = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x119:
                    {
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 0x11a:
                    {
                        flag2 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 340:
                    {
                        str9 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x155:
                    {
                        str8 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x156:
                    {
                        str5 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x157:
                    {
                        str6 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x158:
                    {
                        str7 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x159:
                    {
                        str2 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15a:
                    {
                        str3 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15b:
                    {
                        str4 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x173:
                    {
                        dimLineLineweight = (Lineweight) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x174:
                    {
                        extLineLineweight = (Lineweight) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            if (string.IsNullOrEmpty(str) || !TableObject.IsValidName(str))
            {
                return null;
            }
            string[] dimStylePrefixAndSuffix = GetDimStylePrefixAndSuffix(dimpost);
            DimensionStyle key = new DimensionStyle(str, false) {
                DimLineColor = dimLineColor,
                DimLineLineweight = dimLineLineweight,
                DimLineOff = flag & flag2,
                DimBaselineSpacing = dimBaselineSpacing,
                DimLineExtend = dimLineExtend,
                ExtLineColor = extLineColor,
                ExtLineLineweight = extLineLineweight,
                ExtLine1Off = flag3,
                ExtLine2Off = flag4,
                ExtLineOffset = extLineOffset,
                ExtLineExtend = extLineExtend,
                ArrowSize = arrowSize,
                CenterMarkSize = centerMarkSize,
                DimScaleOverall = dimScaleOverall,
                DIMTIH = dIMTIH,
                DIMTOH = dIMTOH,
                TextHeight = textHeight,
                TextColor = textColor,
                DIMJUST = dIMJUST,
                DIMTAD = dIMTAD,
                TextOffset = textOffset,
                AngularPrecision = angularPrecision,
                LengthPrecision = lengthPrecision,
                DimScaleLinear = dimScaleLinear,
                DimPrefix = dimStylePrefixAndSuffix[0],
                DimSuffix = dimStylePrefixAndSuffix[1],
                DecimalSeparator = decimalSeparator,
                DimAngularUnits = dimAngularUnits,
                DimLengthUnits = dimLengthUnits,
                FractionalType = fractionalType,
                DimRoundoff = num17
            };
            if (!flag5)
            {
                str6 = str5;
                str7 = str5;
            }
            string[] strArray2 = new string[] { str6, str7, str8, str9, str2, str3, str4 };
            this.dimStyleToHandles.Add(key, strArray2);
            if ((12 - num18) <= 0)
            {
                key.SuppressLinearLeadingZeros = true;
                key.SuppressLinearTrailingZeros = true;
                num18 = (short) (num18 - 12);
            }
            else if ((8 - num18) <= 0)
            {
                key.SuppressLinearLeadingZeros = false;
                key.SuppressLinearTrailingZeros = true;
                num18 = (short) (num18 - 8);
            }
            else if ((4 - num18) <= 0)
            {
                key.SuppressLinearLeadingZeros = true;
                key.SuppressLinearTrailingZeros = false;
                num18 = (short) (num18 - 4);
            }
            else
            {
                key.SuppressLinearLeadingZeros = false;
                key.SuppressLinearTrailingZeros = false;
            }
            switch (num18)
            {
                case 0:
                    key.SuppressZeroFeet = true;
                    key.SuppressZeroInches = true;
                    break;

                case 1:
                    key.SuppressZeroFeet = false;
                    key.SuppressZeroInches = false;
                    break;

                case 2:
                    key.SuppressZeroFeet = false;
                    key.SuppressZeroInches = true;
                    break;

                case 3:
                    key.SuppressZeroFeet = true;
                    key.SuppressZeroInches = false;
                    break;

                default:
                    key.SuppressZeroFeet = true;
                    key.SuppressZeroInches = true;
                    break;
            }
            switch (dimLengthUnits)
            {
                case LinearUnitType.Architectural:
                case LinearUnitType.Engineering:
                    if (num18 == 1)
                    {
                        key.SuppressZeroFeet = false;
                        key.SuppressZeroInches = false;
                    }
                    else
                    {
                        if (num18 == 2)
                        {
                            key.SuppressZeroFeet = false;
                            key.SuppressZeroInches = true;
                            break;
                        }
                        if (num18 == 3)
                        {
                            key.SuppressZeroFeet = true;
                            key.SuppressZeroInches = false;
                        }
                        else
                        {
                            key.SuppressZeroFeet = true;
                            key.SuppressZeroInches = true;
                        }
                    }
                    break;
            }
            if ((12 - num18) <= 0)
            {
                key.SuppressLinearLeadingZeros = true;
                key.SuppressLinearTrailingZeros = true;
            }
            else if ((8 - num18) <= 0)
            {
                key.SuppressLinearLeadingZeros = false;
                key.SuppressLinearTrailingZeros = true;
            }
            else if ((4 - num18) <= 0)
            {
                key.SuppressLinearLeadingZeros = true;
                key.SuppressLinearTrailingZeros = false;
            }
            else
            {
                key.SuppressLinearLeadingZeros = false;
                key.SuppressLinearTrailingZeros = false;
            }
            if (num19 == 1)
            {
                key.SuppressAngularLeadingZeros = true;
                key.SuppressAngularTrailingZeros = false;
            }
            else
            {
                if (num19 == 2)
                {
                    key.SuppressAngularLeadingZeros = false;
                    key.SuppressAngularTrailingZeros = true;
                    return key;
                }
                if (num19 == 3)
                {
                    key.SuppressAngularLeadingZeros = true;
                    key.SuppressAngularTrailingZeros = true;
                }
                else
                {
                    key.SuppressAngularLeadingZeros = false;
                    key.SuppressAngularTrailingZeros = false;
                }
            }
            return key;
        }

        private List<DimensionStyleOverride> ReadDimensionStyleOverrideXData(XData xDataOverrides, EntityObject entity)
        {
            List<DimensionStyleOverride> list = new List<DimensionStyleOverride>();
            IEnumerator<XDataRecord> enumerator = xDataOverrides.XDataRecord.GetEnumerator();
            short num = -1;
            short num2 = -1;
            bool flag = false;
            bool flag2 = false;
            while (enumerator.MoveNext())
            {
                short num3;
                Linetype linetype;
                Linetype linetype2;
                Linetype linetype3;
                BlockRecord record2;
                BlockRecord record3;
                BlockRecord record4;
                TextStyle style;
                XDataRecord current = enumerator.Current;
                if (!((current.Code == XDataCode.String) && string.Equals((string) current.Value, "DSTYLE", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                if (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                }
                else
                {
                    return list;
                }
                if (((current.Code == XDataCode.ControlString) || (((string) current.Value) == "{")) && enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    goto Label_0D70;
                }
                return list;
            Label_00D8:
                if (current.Code == XDataCode.Int16)
                {
                    num3 = (short) current.Value;
                    if (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        goto Label_0124;
                    }
                }
                return list;
            Label_0124:
                switch (num3)
                {
                    case 40:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_0A04;
                        }
                        return list;

                    case 0x29:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_06E2;
                        }
                        return list;

                    case 0x2a:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_065C;
                        }
                        return list;

                    case 0x2c:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_069F;
                        }
                        return list;

                    case 0x2d:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0D05;
                        }
                        return list;

                    case 0x2e:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_0406;
                        }
                        return list;

                    case 3:
                    {
                        if (current.Code != XDataCode.String)
                        {
                            return list;
                        }
                        string[] dimStylePrefixAndSuffix = GetDimStylePrefixAndSuffix(this.DecodeEncodedNonAsciiCharacters((string) current.Value));
                        if (!string.IsNullOrEmpty(dimStylePrefixAndSuffix[0]))
                        {
                            list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimPrefix, dimStylePrefixAndSuffix[0]));
                        }
                        if (!string.IsNullOrEmpty(dimStylePrefixAndSuffix[1]))
                        {
                            list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimSuffix, dimStylePrefixAndSuffix[1]));
                        }
                        goto Label_0D51;
                    }
                    case 0x4b:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_05B2;
                        }
                        return list;

                    case 0x4c:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0607;
                        }
                        return list;

                    case 0x4e:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0CA6;
                        }
                        return list;

                    case 0x4f:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0CD7;
                        }
                        return list;

                    case 70:
                    {
                        Leader leader = entity as Leader;
                        if (leader != null)
                        {
                            leader.TextVerticalPosition = (LeaderTextVerticalPosition) ((short) current.Value);
                        }
                        goto Label_0D51;
                    }
                    case 140:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_097E;
                        }
                        return list;

                    case 0x8d:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_0725;
                        }
                        return list;

                    case 0x90:
                        if (current.Code == XDataCode.Real)
                        {
                            goto Label_0B9A;
                        }
                        return list;

                    case 0xb0:
                        if (current.Code == XDataCode.Int16)
                        {
                            break;
                        }
                        return list;

                    case 0xb1:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0448;
                        }
                        return list;

                    case 0xb2:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_093B;
                        }
                        return list;

                    case 0xb3:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0A47;
                        }
                        return list;

                    case 0x93:
                        if (current.Code != XDataCode.Real)
                        {
                            return list;
                        }
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.TextOffset, (double) current.Value));
                        goto Label_0D51;

                    case 0x10f:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0A8A;
                        }
                        return list;

                    case 0x113:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0C20;
                        }
                        return list;

                    case 0x114:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0C63;
                        }
                        return list;

                    case 0x115:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0BDD;
                        }
                        return list;

                    case 0x116:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_0B56;
                        }
                        return list;

                    case 0x119:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_039C;
                        }
                        return list;

                    case 0x11a:
                        if (current.Code == XDataCode.Int16)
                        {
                            goto Label_03D1;
                        }
                        return list;

                    case 340:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_08C7;
                        }
                        return list;

                    case 0x155:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_0768;
                        }
                        return list;

                    case 0x157:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_07DD;
                        }
                        return list;

                    case 0x158:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_0852;
                        }
                        return list;

                    case 0x159:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_02E7;
                        }
                        return list;

                    case 0x15a:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_048A;
                        }
                        return list;

                    case 0x15b:
                        if (current.Code == XDataCode.DatabaseHandle)
                        {
                            goto Label_04FD;
                        }
                        return list;

                    case 0x173:
                        if (current.Code != XDataCode.Int16)
                        {
                            return list;
                        }
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimLineLineweight, (Lineweight) ((short) current.Value)));
                        goto Label_0D51;

                    case 0x174:
                        if (current.Code != XDataCode.Int16)
                        {
                            return list;
                        }
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLineLineweight, (Lineweight) ((short) current.Value)));
                        goto Label_0D51;

                    default:
                        goto Label_0D51;
                }
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimLineColor, AciColor.FromCadIndex((short) current.Value)));
                goto Label_0D51;
            Label_02E7:
                linetype = this.doc.GetObjectByHandle((string) current.Value) as Linetype;
                if (linetype == null)
                {
                    linetype = this.doc.Linetypes["Continuous"];
                }
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimLineLinetype, linetype));
                goto Label_0D51;
            Label_039C:
                flag = ((short) current.Value) > 0;
                goto Label_0D51;
            Label_03D1:
                flag2 = ((short) current.Value) > 0;
                goto Label_0D51;
            Label_0406:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimLineExtend, (double) current.Value));
                goto Label_0D51;
            Label_0448:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLineColor, AciColor.FromCadIndex((short) current.Value)));
                goto Label_0D51;
            Label_048A:
                linetype2 = this.doc.GetObjectByHandle((string) current.Value) as Linetype;
                if (linetype2 == null)
                {
                    linetype2 = this.doc.Linetypes["Continuous"];
                }
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLine1Linetype, linetype2));
                goto Label_0D51;
            Label_04FD:
                linetype3 = this.doc.GetObjectByHandle((string) current.Value) as Linetype;
                if (linetype3 == null)
                {
                    linetype3 = this.doc.Linetypes["Continuous"];
                }
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLine2Linetype, linetype3));
                goto Label_0D51;
            Label_05B2:
                list.Add((((short) current.Value) == 0) ? new DimensionStyleOverride(DimensionStyleOverrideType.ExtLine1Off, false) : new DimensionStyleOverride(DimensionStyleOverrideType.ExtLine1Off, true));
                goto Label_0D51;
            Label_0607:
                list.Add((((short) current.Value) == 0) ? new DimensionStyleOverride(DimensionStyleOverrideType.ExtLine2Off, false) : new DimensionStyleOverride(DimensionStyleOverrideType.ExtLine2Off, true));
                goto Label_0D51;
            Label_065C:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLineOffset, (double) current.Value));
                goto Label_0D51;
            Label_069F:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ExtLineExtend, (double) current.Value));
                goto Label_0D51;
            Label_06E2:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.ArrowSize, (double) current.Value));
                goto Label_0D51;
            Label_0725:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.CenterMarkSize, (double) current.Value));
                goto Label_0D51;
            Label_0768:
                record2 = this.doc.GetObjectByHandle((string) current.Value) as BlockRecord;
                list.Add((record2 == null) ? new DimensionStyleOverride(DimensionStyleOverrideType.LeaderArrow, null) : new DimensionStyleOverride(DimensionStyleOverrideType.LeaderArrow, this.doc.Blocks[record2.Name]));
                goto Label_0D51;
            Label_07DD:
                record3 = this.doc.GetObjectByHandle((string) current.Value) as BlockRecord;
                list.Add((record3 == null) ? new DimensionStyleOverride(DimensionStyleOverrideType.DimArrow1, null) : new DimensionStyleOverride(DimensionStyleOverrideType.DimArrow1, this.doc.Blocks[record3.Name]));
                goto Label_0D51;
            Label_0852:
                record4 = this.doc.GetObjectByHandle((string) current.Value) as BlockRecord;
                list.Add((record4 == null) ? new DimensionStyleOverride(DimensionStyleOverrideType.DimArrow2, null) : new DimensionStyleOverride(DimensionStyleOverrideType.DimArrow2, this.doc.Blocks[record4.Name]));
                goto Label_0D51;
            Label_08C7:
                style = this.doc.GetObjectByHandle((string) current.Value) as TextStyle;
                if (style == null)
                {
                    style = this.doc.TextStyles["Standard"];
                }
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.TextStyle, style));
                goto Label_0D51;
            Label_093B:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.TextColor, AciColor.FromCadIndex((short) current.Value)));
                goto Label_0D51;
            Label_097E:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.TextHeight, (double) current.Value));
                goto Label_0D51;
            Label_0A04:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimScaleOverall, (double) current.Value));
                goto Label_0D51;
            Label_0A47:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.AngularPrecision, (short) current.Value));
                goto Label_0D51;
            Label_0A8A:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.LengthPrecision, (short) current.Value));
                goto Label_0D51;
            Label_0B56:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DecimalSeparator, (char) ((ushort) ((short) current.Value))));
                goto Label_0D51;
            Label_0B9A:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimScaleLinear, (double) current.Value));
                goto Label_0D51;
            Label_0BDD:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimLengthUnits, (LinearUnitType) ((short) current.Value)));
                goto Label_0D51;
            Label_0C20:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimAngularUnits, (AngleUnitType) ((short) current.Value)));
                goto Label_0D51;
            Label_0C63:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.FractionalType, (FractionFormatType) ((short) current.Value)));
                goto Label_0D51;
            Label_0CA6:
                num = (short) current.Value;
                goto Label_0D51;
            Label_0CD7:
                num2 = (short) current.Value;
                goto Label_0D51;
            Label_0D05:
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimRoundoff, (short) current.Value));
            Label_0D51:
                if (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                }
                else
                {
                    return list;
                }
            Label_0D70:
                if ((current.Code != XDataCode.ControlString) || (((string) current.Value) != "}"))
                {
                    goto Label_00D8;
                }
            }
            list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.DimLineOff, flag & flag2));
            if (num >= 0)
            {
                if ((12 - num) <= 0)
                {
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearLeadingZeros, true));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearTrailingZeros, true));
                    num = (short) (num - 12);
                }
                else if ((8 - num) <= 0)
                {
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearLeadingZeros, false));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearTrailingZeros, true));
                    num = (short) (num - 8);
                }
                else if ((4 - num) <= 0)
                {
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearLeadingZeros, true));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearTrailingZeros, false));
                    num = (short) (num - 4);
                }
                else
                {
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearLeadingZeros, false));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressLinearTrailingZeros, false));
                }
                switch (num)
                {
                    case 0:
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroFeet, true));
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroInches, true));
                        goto Label_0FC0;

                    case 1:
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroFeet, false));
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroInches, false));
                        goto Label_0FC0;

                    case 2:
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroFeet, false));
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroInches, true));
                        goto Label_0FC0;

                    case 3:
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroFeet, true));
                        list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroInches, false));
                        goto Label_0FC0;
                }
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroFeet, true));
                list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressZeroInches, true));
            }
        Label_0FC0:
            switch (num2)
            {
                case 0:
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularLeadingZeros, false));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularTrailingZeros, false));
                    return list;

                case 1:
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularLeadingZeros, true));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularTrailingZeros, false));
                    return list;

                case 2:
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularLeadingZeros, false));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularTrailingZeros, true));
                    return list;

                case 3:
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularLeadingZeros, true));
                    list.Add(new DimensionStyleOverride(DimensionStyleOverrideType.SuppressAngularTrailingZeros, true));
                    return list;
            }
            return list;
        }

        private HatchBoundaryPath ReadEdgeBoundaryPath(int numEdges)
        {
            List<HatchBoundaryPath.Edge> edges = new List<HatchBoundaryPath.Edge>();
            this.chunk.Next();
            while (edges.Count < numEdges)
            {
                short num18;
                bool flag3;
                bool flag4;
                int num19;
                double[] numArray;
                int num20;
                Vector3[] vectorArray;
                int num21;
                switch (this.chunk.ReadShort())
                {
                    case 1:
                    {
                        this.chunk.Next();
                        double x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num4 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num5 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        HatchBoundaryPath.Line line = new HatchBoundaryPath.Line {
                            Start = new Vector2(x, y),
                            End = new Vector2(num4, num5)
                        };
                        edges.Add(line);
                        continue;
                    }
                    case 2:
                    {
                        this.chunk.Next();
                        double x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num8 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num9 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num10 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        bool flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        HatchBoundaryPath.Arc arc = new HatchBoundaryPath.Arc {
                            Center = new Vector2(x, y),
                            Radius = num8,
                            StartAngle = num9,
                            EndAngle = num10,
                            IsCounterclockwise = flag
                        };
                        edges.Add(arc);
                        continue;
                    }
                    case 3:
                    {
                        this.chunk.Next();
                        double x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num13 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num14 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num15 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num16 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        double num17 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        bool flag2 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        HatchBoundaryPath.Ellipse ellipse = new HatchBoundaryPath.Ellipse {
                            Center = new Vector2(x, y),
                            EndMajorAxis = new Vector2(num13, num14),
                            MinorRatio = num15,
                            StartAngle = num16,
                            EndAngle = num17,
                            IsCounterclockwise = flag2
                        };
                        edges.Add(ellipse);
                        continue;
                    }
                    case 4:
                        this.chunk.Next();
                        num18 = (short) this.chunk.ReadInt();
                        this.chunk.Next();
                        flag3 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        flag4 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        num19 = this.chunk.ReadInt();
                        numArray = new double[num19];
                        this.chunk.Next();
                        num20 = this.chunk.ReadInt();
                        vectorArray = new Vector3[num20];
                        this.chunk.Next();
                        num21 = 0;
                        goto Label_03C7;

                    default:
                    {
                        continue;
                    }
                }
            Label_03A3:
                numArray[num21] = this.chunk.ReadDouble();
                this.chunk.Next();
                num21++;
            Label_03C7:
                if (num21 < num19)
                {
                    goto Label_03A3;
                }
                for (int j = 0; j < num20; j++)
                {
                    double x = this.chunk.ReadDouble();
                    this.chunk.Next();
                    double y = this.chunk.ReadDouble();
                    this.chunk.Next();
                    double z = 1.0;
                    if (this.chunk.Code == 0x2a)
                    {
                        z = this.chunk.ReadDouble();
                        this.chunk.Next();
                    }
                    vectorArray[j] = new Vector3(x, y, z);
                }
                if (this.doc.DrawingVariables.AcadVer >= DxfVersion.AutoCad2010)
                {
                    int num26 = this.chunk.ReadInt();
                    this.chunk.Next();
                    for (int k = 0; k < num26; k++)
                    {
                        this.chunk.Next();
                        this.chunk.Next();
                    }
                    if (this.chunk.Code == 12)
                    {
                        this.chunk.Next();
                        this.chunk.Next();
                    }
                    if (this.chunk.Code == 13)
                    {
                        this.chunk.Next();
                        this.chunk.Next();
                    }
                }
                HatchBoundaryPath.Spline item = new HatchBoundaryPath.Spline {
                    Degree = num18,
                    IsPeriodic = flag4,
                    IsRational = flag3,
                    ControlPoints = vectorArray,
                    Knots = numArray
                };
                edges.Add(item);
            }
            HatchBoundaryPath key = new HatchBoundaryPath(edges);
            Debug.Assert(this.chunk.Code == 0x61, "The reference count code 97 was expected.");
            int capacity = this.chunk.ReadInt();
            this.hatchContourns.Add(key, new List<string>(capacity));
            this.chunk.Next();
            for (int i = 0; i < capacity; i++)
            {
                Debug.Assert(this.chunk.Code == 330, "The reference handle code 330 was expected.");
                this.hatchContourns[key].Add(this.chunk.ReadString());
                this.chunk.Next();
            }
            return key;
        }

        private HatchBoundaryPath ReadEdgePolylineBoundaryPath()
        {
            HatchBoundaryPath.Polyline polyline = new HatchBoundaryPath.Polyline();
            this.chunk.Next();
            bool flag = this.chunk.ReadShort() > 0;
            this.chunk.Next();
            polyline.IsClosed = this.chunk.ReadShort() > 0;
            this.chunk.Next();
            int num = this.chunk.ReadInt();
            polyline.Vertexes = new Vector3[num];
            this.chunk.Next();
            for (int i = 0; i < num; i++)
            {
                double z = 0.0;
                double x = this.chunk.ReadDouble();
                this.chunk.Next();
                double y = this.chunk.ReadDouble();
                this.chunk.Next();
                if (flag)
                {
                    z = this.chunk.ReadDouble();
                    this.chunk.Next();
                }
                polyline.Vertexes[i] = new Vector3(x, y, z);
            }
            List<HatchBoundaryPath.Edge> edges = new List<HatchBoundaryPath.Edge> {
                polyline
            };
            HatchBoundaryPath key = new HatchBoundaryPath(edges);
            Debug.Assert(this.chunk.Code == 0x61, "The reference count code 97 was expected.");
            int capacity = this.chunk.ReadInt();
            this.hatchContourns.Add(key, new List<string>(capacity));
            this.chunk.Next();
            for (int j = 0; j < capacity; j++)
            {
                Debug.Assert(this.chunk.Code == 330, "The reference handle code 330 was expected.");
                this.hatchContourns[key].Add(this.chunk.ReadString());
                this.chunk.Next();
            }
            return key;
        }

        private netDxf.Entities.Ellipse ReadEllipse()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 point = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            double[] param = new double[2];
            double num = 0.0;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        point.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        point.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x29:
                        param[0] = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x2a:
                        param[1] = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        point.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 vector4 = MathHelper.Transform(point, unitZ, CoordinateSystem.World, CoordinateSystem.Object);
            double num2 = Vector2.Angle(new Vector2(vector4.X, vector4.Y));
            double num3 = 2.0 * point.Modulus();
            double num4 = num3 * num;
            netDxf.Entities.Ellipse ellipse = new netDxf.Entities.Ellipse {
                MajorAxis = num3,
                MinorAxis = num4,
                Rotation = num2 * 57.295779513082323,
                Center = zero,
                Normal = unitZ
            };
            ellipse.XData.AddRange(items);
            SetEllipseParameters(ellipse, param);
            return ellipse;
        }

        private void ReadEntities()
        {
            Debug.Assert(this.chunk.ReadString() == "ENTITIES");
            this.chunk.Next();
            while (this.chunk.ReadString() != "ENDSEC")
            {
                this.ReadEntity(false);
            }
        }

        private EntityObject ReadEntity(bool isBlockEntity)
        {
            string str = null;
            string str2 = null;
            EntityObject obj2;
            Layer layer = Layer.Default;
            AciColor byLayer = AciColor.ByLayer;
            Linetype linetype = Linetype.ByLayer;
            Lineweight lineweight = Lineweight.ByLayer;
            double num = 1.0;
            bool flag = true;
            Transparency transparency = Transparency.ByLayer;
            string str3 = this.chunk.ReadString();
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new Exception($"Premature end of entity {str3} definition.");

                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 0x66:
                        this.ReadExtensionDictionaryGroup();
                        this.chunk.Next();
                        break;

                    case 330:
                        str2 = this.chunk.ReadHex();
                        if (str2 == "0")
                        {
                            str2 = null;
                        }
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            Debug.Assert(this.chunk.ReadString() == "AcDbEntity");
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 8:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(name);
                        this.chunk.Next();
                        break;
                    }
                    case 0x30:
                        num = this.chunk.ReadDouble();
                        if (num <= 0.0)
                        {
                            num = 1.0;
                        }
                        this.chunk.Next();
                        break;

                    case 0:
                        throw new Exception($"Premature end of entity {str3} definition.");

                    case 6:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        linetype = this.GetLinetype(name);
                        this.chunk.Next();
                        break;
                    }
                    case 60:
                        flag = this.chunk.ReadShort() == 0;
                        this.chunk.Next();
                        break;

                    case 0x3e:
                        if (!byLayer.UseTrueColor)
                        {
                            byLayer = AciColor.FromCadIndex(this.chunk.ReadShort());
                        }
                        this.chunk.Next();
                        break;

                    case 370:
                        lineweight = (Lineweight) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 420:
                        byLayer = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;

                    case 440:
                        transparency = Transparency.FromAlphaValue(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            string s = str3;
            switch (<PrivateImplementationDetails>.ComputeStringHash(s))
            {
                case 0x18d880b:
                    if (s == "ATTDEF")
                    {
                        obj2 = this.ReadAttributeDefinition();
                        goto Label_09AC;
                    }
                    break;

                case 0x74e81e1:
                    if (s == "HATCH")
                    {
                        obj2 = this.ReadHatch();
                        goto Label_09AC;
                    }
                    break;

                case 0x152e7967:
                    if (s == "LINE")
                    {
                        obj2 = this.ReadLine();
                        goto Label_09AC;
                    }
                    break;

                case 0x276b5892:
                    if (s == "DGNUNDERLAY")
                    {
                        obj2 = this.ReadUnderlay();
                        goto Label_09AC;
                    }
                    break;

                case 0x2c7a184e:
                    if (s == "TRACE")
                    {
                        obj2 = this.ReadTrace();
                        goto Label_09AC;
                    }
                    break;

                case 0x190be6d4:
                    if (s == "SOLID")
                    {
                        obj2 = this.ReadSolid();
                        goto Label_09AC;
                    }
                    break;

                case 0x22d0888b:
                    if (s == "3DFACE")
                    {
                        obj2 = this.ReadFace3d();
                        goto Label_09AC;
                    }
                    break;

                case 0x505aa655:
                    if (s == "POLYLINE")
                    {
                        obj2 = this.ReadPolyline();
                        goto Label_09AC;
                    }
                    break;

                case 0x5bbad75a:
                    if (s == "IMAGE")
                    {
                        obj2 = this.ReadImage();
                        goto Label_09AC;
                    }
                    break;

                case 0x442ef250:
                    if (s == "LWPOLYLINE")
                    {
                        obj2 = this.ReadLwPolyline();
                        goto Label_09AC;
                    }
                    break;

                case 0x4aba6cfc:
                    if (s == "MESH")
                    {
                        obj2 = this.ReadMesh();
                        goto Label_09AC;
                    }
                    break;

                case 0x65987924:
                    if (s == "LEADER")
                    {
                        obj2 = this.ReadLeader();
                        goto Label_09AC;
                    }
                    break;

                case 0x7f87bbea:
                    if (s == "DWFUNDERLAY")
                    {
                        obj2 = this.ReadUnderlay();
                        goto Label_09AC;
                    }
                    break;

                case 0x80d9209f:
                    if (s == "RAY")
                    {
                        obj2 = this.ReadRay();
                        goto Label_09AC;
                    }
                    break;

                case 0x80ee447e:
                    if (s == "TEXT")
                    {
                        obj2 = this.ReadText();
                        goto Label_09AC;
                    }
                    break;

                case 0x86b48e3f:
                    if (s == "MTEXT")
                    {
                        obj2 = this.ReadMText();
                        goto Label_09AC;
                    }
                    break;

                case 0x8f5d7c03:
                    if (s == "DIMENSION")
                    {
                        obj2 = this.ReadDimension(isBlockEntity);
                        goto Label_09AC;
                    }
                    break;

                case 0xa4070548:
                    if (s == "TOLERANCE")
                    {
                        obj2 = this.ReadTolerance();
                        goto Label_09AC;
                    }
                    break;

                case 0xab48ce3c:
                    if (s == "SPLINE")
                    {
                        obj2 = this.ReadSpline();
                        goto Label_09AC;
                    }
                    break;

                case 0xaf8afbcd:
                    if (s == "PDFUNDERLAY")
                    {
                        obj2 = this.ReadUnderlay();
                        goto Label_09AC;
                    }
                    break;

                case 0xa4ed3768:
                    if (s == "INSERT")
                    {
                        obj2 = this.ReadInsert(isBlockEntity);
                        goto Label_09AC;
                    }
                    break;

                case 0xaa40a431:
                    if (s == "ELLIPSE")
                    {
                        obj2 = this.ReadEllipse();
                        goto Label_09AC;
                    }
                    break;

                case 0xcb2615c9:
                    if (s == "CIRCLE")
                    {
                        obj2 = this.ReadCircle();
                        goto Label_09AC;
                    }
                    break;

                case 0xcbea8075:
                    if (s == "XLINE")
                    {
                        obj2 = this.ReadXLine();
                        goto Label_09AC;
                    }
                    break;

                case 0xb76dbc42:
                    if (s == "MLINE")
                    {
                        obj2 = this.ReadMLine();
                        goto Label_09AC;
                    }
                    break;

                case 0xbcabd5fb:
                    if (s == "ARC")
                    {
                        obj2 = this.ReadArc();
                        goto Label_09AC;
                    }
                    break;

                case 0xd5d0cf83:
                    if (s == "VIEWPORT")
                    {
                        obj2 = this.ReadViewport();
                        goto Label_09AC;
                    }
                    break;

                case 0xe75a438a:
                    if (s == "WIPEOUT")
                    {
                        obj2 = this.ReadWipeout();
                        goto Label_09AC;
                    }
                    break;

                case 0xe7fbebd7:
                    if (s == "ACAD_TABLE")
                    {
                        obj2 = this.ReadAcadTable(isBlockEntity);
                        goto Label_09AC;
                    }
                    break;

                case 0xf02a2031:
                    if (s == "POINT")
                    {
                        obj2 = this.ReadPoint();
                        goto Label_09AC;
                    }
                    break;
            }
            this.ReadUnknowEntity();
            return null;
        Label_09AC:
            if ((obj2 == null) || string.IsNullOrEmpty(str))
            {
                return null;
            }
            obj2.Handle = str;
            obj2.Layer = layer;
            obj2.Color = byLayer;
            obj2.Linetype = linetype;
            obj2.Lineweight = lineweight;
            obj2.LinetypeScale = num;
            obj2.IsVisible = flag;
            obj2.Transparency = transparency;
            if (!isBlockEntity)
            {
                this.entityList.Add(obj2, str2);
            }
            return obj2;
        }

        private void ReadExtensionDictionaryGroup()
        {
            this.chunk.Next();
            while (this.chunk.Code != 0x66)
            {
                switch (this.chunk.Code)
                {
                }
                this.chunk.Next();
            }
        }

        private Face3d ReadFace3d()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector3 = Vector3.Zero;
            Vector3 vector4 = Vector3.Zero;
            Face3dEdgeFlags visibles = Face3dEdgeFlags.Visibles;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        vector4.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        vector4.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        vector3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x21:
                    {
                        vector4.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        visibles = (Face3dEdgeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            Face3d faced = new Face3d {
                FirstVertex = zero,
                SecondVertex = vector2,
                ThirdVertex = vector3,
                FourthVertex = vector4,
                EdgeFlags = visibles
            };
            faced.XData.AddRange(items);
            return faced;
        }

        private Group ReadGroup()
        {
            string str = null;
            string str2 = null;
            string name = null;
            bool flag = true;
            bool flag2 = true;
            List<string> list = new List<string>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 70:
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 0x47:
                        flag2 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 300:
                        str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 330:
                    {
                        string str4 = this.chunk.ReadHex();
                        DictionaryObject obj2 = this.dictionaries[str4];
                        if (str == null)
                        {
                            throw new NullReferenceException("Null handle in Group dictionary.");
                        }
                        name = obj2.Entries[str];
                        this.chunk.Next();
                        break;
                    }
                    case 340:
                    {
                        string item = this.chunk.ReadHex();
                        list.Add(item);
                        this.chunk.Next();
                        break;
                    }
                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (flag)
            {
                this.CheckGroupName(name);
            }
            Group key = new Group(name, false) {
                Handle = str,
                Description = str2,
                IsUnnamed = flag,
                IsSelectable = flag2
            };
            this.groupEntities.Add(key, list);
            return key;
        }

        private Hatch ReadHatch()
        {
            string name = string.Empty;
            HatchFillType solidFill = HatchFillType.SolidFill;
            double num = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            HatchPattern line = HatchPattern.Line;
            bool associative = false;
            List<HatchBoundaryPath> list = new List<HatchBoundaryPath>();
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 70:
                        solidFill = (HatchFillType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x47:
                        if (this.chunk.ReadShort() > 0)
                        {
                            associative = true;
                        }
                        this.chunk.Next();
                        break;

                    case 0x4b:
                        line = this.ReadHatchPattern(name);
                        line.Fill = solidFill;
                        break;

                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 30:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x5b:
                    {
                        int numPaths = this.chunk.ReadInt();
                        list = this.ReadHatchBoundaryPaths(numPaths);
                        break;
                    }
                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(str2));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            if (list.Count == 0)
            {
                return null;
            }
            Hatch key = new Hatch(line, new List<HatchBoundaryPath>(), associative) {
                Elevation = num,
                Normal = unitZ
            };
            this.hatchToPaths.Add(key, list);
            key.XData.AddRange(items);
            Vector2 zero = Vector2.Zero;
            if (key.XData.TryGetValue("ACAD", out XData data))
            {
                foreach (XDataRecord record in data.XDataRecord)
                {
                    if (record.Code == XDataCode.RealX)
                    {
                        zero.X = (double) record.Value;
                    }
                    else if (record.Code == XDataCode.RealY)
                    {
                        zero.Y = (double) record.Value;
                    }
                }
            }
            line.Origin = zero;
            return key;
        }

        private List<HatchBoundaryPath> ReadHatchBoundaryPaths(int numPaths)
        {
            List<HatchBoundaryPath> list = new List<HatchBoundaryPath>();
            HatchBoundaryPathTypeFlags flags = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            this.chunk.Next();
            while (list.Count < numPaths)
            {
                HatchBoundaryPath path;
                switch (this.chunk.Code)
                {
                    case 0x5c:
                        flags = (((HatchBoundaryPathTypeFlags) this.chunk.ReadInt()) | HatchBoundaryPathTypeFlags.External) | HatchBoundaryPathTypeFlags.Derived;
                        if (flags.HasFlag(HatchBoundaryPathTypeFlags.Polyline))
                        {
                            path = this.ReadEdgePolylineBoundaryPath();
                            path.PathType = flags;
                            list.Add(path);
                        }
                        else
                        {
                            this.chunk.Next();
                        }
                        break;

                    case 0x5d:
                    {
                        int numEdges = this.chunk.ReadInt();
                        path = this.ReadEdgeBoundaryPath(numEdges);
                        path.PathType = flags;
                        list.Add(path);
                        break;
                    }
                    default:
                        this.chunk.Next();
                        break;
                }
            }
            return list;
        }

        private HatchGradientPattern ReadHatchGradientPattern()
        {
            this.chunk.Next();
            this.chunk.Next();
            double num = this.chunk.ReadDouble();
            this.chunk.Next();
            bool flag = ((int) this.chunk.ReadDouble()) == 0;
            this.chunk.Next();
            bool flag2 = this.chunk.ReadInt() > 0;
            this.chunk.Next();
            double tint = this.chunk.ReadDouble();
            this.chunk.Next();
            this.chunk.Next();
            this.chunk.Next();
            this.chunk.Next();
            AciColor color = AciColor.FromTrueColor(this.chunk.ReadInt());
            this.chunk.Next();
            this.chunk.Next();
            this.chunk.Next();
            AciColor color2 = AciColor.FromTrueColor(this.chunk.ReadInt());
            this.chunk.Next();
            string str = this.chunk.ReadString();
            if (!StringEnum.IsStringDefined(typeof(HatchGradientPatternType), str))
            {
                throw new Exception($"Unknown hatch gradient type: {str}.");
            }
            HatchGradientPatternType type = (HatchGradientPatternType) StringEnum.Parse(typeof(HatchGradientPatternType), str);
            if (flag2)
            {
                return new HatchGradientPattern(color, tint, type) { 
                    Centered = flag,
                    Angle = num * 57.295779513082323
                };
            }
            return new HatchGradientPattern(color, color2, type) { 
                Centered = flag,
                Angle = num * 57.295779513082323
            };
        }

        private HatchPattern ReadHatchPattern(string name)
        {
            HatchPattern pattern = null;
            double patternAngle = 0.0;
            double patternScale = 1.0;
            bool flag = false;
            List<HatchPatternLineDefinition> collection = new List<HatchPatternLineDefinition>();
            HatchType userDefined = HatchType.UserDefined;
            HatchStyle normal = HatchStyle.Normal;
            while ((this.chunk.Code != 0) && (this.chunk.Code != 0x3e9))
            {
                switch (this.chunk.Code)
                {
                    case 0x29:
                        patternScale = this.chunk.ReadDouble();
                        if (patternScale <= 0.0)
                        {
                            patternScale = 1.0;
                        }
                        this.chunk.Next();
                        break;

                    case 0x2f:
                        this.chunk.Next();
                        break;

                    case 10:
                        this.chunk.Next();
                        break;

                    case 20:
                        this.chunk.Next();
                        break;

                    case 0x4b:
                        normal = (HatchStyle) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x4c:
                        userDefined = (HatchType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x4d:
                        this.chunk.Next();
                        break;

                    case 0x4e:
                    {
                        short numLines = this.chunk.ReadShort();
                        collection = this.ReadHatchPatternDefinitionLine(patternScale, patternAngle, numLines);
                        break;
                    }
                    case 0x34:
                        patternAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x62:
                        this.chunk.Next();
                        break;

                    case 450:
                        if (this.chunk.ReadInt() == 1)
                        {
                            flag = true;
                            pattern = this.ReadHatchGradientPattern();
                        }
                        else
                        {
                            this.chunk.Next();
                        }
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (!flag)
            {
                pattern = new HatchPattern(name);
            }
            pattern.Angle = patternAngle;
            pattern.Style = normal;
            pattern.Scale = patternScale;
            pattern.Type = userDefined;
            pattern.LineDefinitions.AddRange(collection);
            return pattern;
        }

        private List<HatchPatternLineDefinition> ReadHatchPatternDefinitionLine(double patternScale, double patternAngle, short numLines)
        {
            List<HatchPatternLineDefinition> list = new List<HatchPatternLineDefinition>();
            this.chunk.Next();
            for (int i = 0; i < numLines; i++)
            {
                Vector2 zero = Vector2.Zero;
                Vector2 vector2 = Vector2.Zero;
                double num2 = this.chunk.ReadDouble();
                this.chunk.Next();
                zero.X = this.chunk.ReadDouble();
                this.chunk.Next();
                zero.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                vector2.X = this.chunk.ReadDouble();
                this.chunk.Next();
                vector2.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                short num3 = this.chunk.ReadShort();
                this.chunk.Next();
                double num4 = Math.Sin(patternAngle * 0.017453292519943295);
                double num5 = Math.Cos(patternAngle * 0.017453292519943295);
                zero = new Vector2(((num5 * zero.X) / patternScale) + ((num4 * zero.Y) / patternScale), ((-num4 * zero.X) / patternScale) + ((num5 * zero.Y) / patternScale));
                double num6 = Math.Sin(num2 * 0.017453292519943295);
                double num7 = Math.Cos(num2 * 0.017453292519943295);
                vector2 = new Vector2(((num7 * vector2.X) / patternScale) + ((num6 * vector2.Y) / patternScale), ((-num6 * vector2.X) / patternScale) + ((num7 * vector2.Y) / patternScale));
                HatchPatternLineDefinition item = new HatchPatternLineDefinition {
                    Angle = num2 - patternAngle,
                    Origin = zero,
                    Delta = vector2
                };
                for (int j = 0; j < num3; j++)
                {
                    item.DashPattern.Add(this.chunk.ReadDouble() / patternScale);
                    this.chunk.Next();
                }
                list.Add(item);
            }
            return list;
        }

        private void ReadHeader()
        {
            Debug.Assert(this.chunk.ReadString() == "HEADER");
            this.chunk.Next();
            while (this.chunk.ReadString() != "ENDSEC")
            {
                double num;
                string str4;
                string str5;
                string str6;
                double num3;
                string str7;
                Vector3 vector;
                double num4;
                string str = this.chunk.ReadString();
                this.chunk.Next();
                string s = str;
                switch (<PrivateImplementationDetails>.ComputeStringHash(s))
                {
                    case 0x18b2ca03:
                        if (s == "$TDCREATE")
                        {
                            goto Label_0CD7;
                        }
                        goto Label_0F0C;

                    case 0x19830bb2:
                        if (s == "$TDINDWG")
                        {
                            goto Label_0E8B;
                        }
                        goto Label_0F0C;

                    case 0x139e695f:
                        if (s == "$PLINEGEN")
                        {
                            goto Label_0C7D;
                        }
                        goto Label_0F0C;

                    case 0x17e7e534:
                        if (s == "$INSBASE")
                        {
                            goto Label_0AB8;
                        }
                        goto Label_0F0C;

                    case 0x2287ee19:
                        if (s == "$TEXTSTYLE")
                        {
                            goto Label_098A;
                        }
                        goto Label_0F0C;

                    case 0x230694c0:
                        if (s == "$PDMODE")
                        {
                            goto Label_0C23;
                        }
                        goto Label_0F0C;

                    case 0x399750cb:
                        if (s == "$TDUUPDATE")
                        {
                            goto Label_0E1E;
                        }
                        goto Label_0F0C;

                    case 0x4508691c:
                        if (s == "$LWDISPLAY")
                        {
                            goto Label_0BF6;
                        }
                        goto Label_0F0C;

                    case 0x4cb5581a:
                        if (s == "$EXTNAMES")
                        {
                            goto Label_0A8B;
                        }
                        goto Label_0F0C;

                    case 0x67783fd4:
                        if (s == "$LUNITS")
                        {
                            goto Label_0A04;
                        }
                        goto Label_0F0C;

                    case 0x68822717:
                        if (s == "$LASTSAVEDBY")
                        {
                            goto Label_09D1;
                        }
                        goto Label_0F0C;

                    case 0x4dc8306c:
                        if (s == "$ANGDIR")
                        {
                            goto Label_06B8;
                        }
                        goto Label_0F0C;

                    case 0x64449477:
                        if (s == "$CELTSCALE")
                        {
                            goto Label_079E;
                        }
                        goto Label_0F0C;

                    case 0x6e822850:
                        if (s == "$ANGBASE")
                        {
                            goto Label_068B;
                        }
                        goto Label_0F0C;

                    case 0x7323e22d:
                        if (s == "$CELWEIGHT")
                        {
                            goto Label_07FE;
                        }
                        goto Label_0F0C;

                    case 0x7339f76b:
                        if (s == "$HANDSEED")
                        {
                            goto Label_063E;
                        }
                        goto Label_0F0C;

                    case 0x7d87d97e:
                        if (s == "$PDSIZE")
                        {
                            goto Label_0C50;
                        }
                        goto Label_0F0C;

                    case 0x81e80a53:
                        if (s == "$LTSCALE")
                        {
                            goto Label_0BC9;
                        }
                        goto Label_0F0C;

                    case 0x8f8c4beb:
                        if (s == "$CMLJUST")
                        {
                            goto Label_085E;
                        }
                        goto Label_0F0C;

                    case 0xa14edb5b:
                        if (s == "$AUPREC")
                        {
                            goto Label_073F;
                        }
                        goto Label_0F0C;

                    case 0x83a39e24:
                        if (s == "$PSLTSCALE")
                        {
                            goto Label_0CAA;
                        }
                        goto Label_0F0C;

                    case 0x8f10a26d:
                        if (s == "$TEXTSIZE")
                        {
                            goto Label_0946;
                        }
                        goto Label_0F0C;

                    case 0xb08bc155:
                        if (s == "$CLAYER")
                        {
                            goto Label_082B;
                        }
                        goto Label_0F0C;

                    case 0xbebec070:
                        if (s == "$INSUNITS")
                        {
                            goto Label_0B9C;
                        }
                        goto Label_0F0C;

                    case 0xc20767f2:
                        if (s == "$CMLSTYLE")
                        {
                            goto Label_08B8;
                        }
                        goto Label_0F0C;

                    case 0xc6f4a03e:
                        if (s == "$CECOLOR")
                        {
                            goto Label_076C;
                        }
                        goto Label_0F0C;

                    case 0xc7ea1c31:
                        if (s == "$DWGCODEPAGE")
                        {
                            goto Label_0A5E;
                        }
                        goto Label_0F0C;

                    case 0xd71d7b56:
                        if (s == "$LUPREC")
                        {
                            goto Label_0A31;
                        }
                        goto Label_0F0C;

                    case 0xdb4507c5:
                        if (s == "$CMLSCALE")
                        {
                            goto Label_088B;
                        }
                        goto Label_0F0C;

                    case 0xc9d72786:
                        if (s == "$TDUCREATE")
                        {
                            goto Label_0D44;
                        }
                        goto Label_0F0C;

                    case 0xd29d199d:
                        if (s == "$ATTMODE")
                        {
                            goto Label_06E5;
                        }
                        goto Label_0F0C;

                    case 0xdde43571:
                        if (s == "$CELTYPE")
                        {
                            goto Label_07CB;
                        }
                        goto Label_0F0C;

                    case 0xe4731d72:
                        if (s == "$TDUPDATE")
                        {
                            goto Label_0DB1;
                        }
                        goto Label_0F0C;

                    case 0xf3f552c9:
                        if (s == "$AUNITS")
                        {
                            goto Label_0712;
                        }
                        goto Label_0F0C;

                    case 0xf4c3fde3:
                        if (s == "$ACADVER")
                        {
                            break;
                        }
                        goto Label_0F0C;

                    case 0xfbaa84bc:
                        if (s == "$DIMSTYLE")
                        {
                            goto Label_08FF;
                        }
                        goto Label_0F0C;

                    default:
                        goto Label_0F0C;
                }
                DxfVersion unknown = DxfVersion.Unknown;
                string str3 = this.chunk.ReadString();
                if (StringEnum.IsStringDefined(typeof(DxfVersion), str3))
                {
                    unknown = (DxfVersion) StringEnum.Parse(typeof(DxfVersion), str3);
                }
                if (unknown < DxfVersion.AutoCad2000)
                {
                    throw new NotSupportedException("Only AutoCad2000 and higher dxf versions are supported.");
                }
                this.doc.DrawingVariables.AcadVer = unknown;
                this.chunk.Next();
                continue;
            Label_063E:
                str4 = this.chunk.ReadHex();
                this.doc.DrawingVariables.HandleSeed = str4;
                this.doc.NumHandles = long.Parse(str4, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.chunk.Next();
                continue;
            Label_068B:
                this.doc.DrawingVariables.Angbase = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_06B8:
                this.doc.DrawingVariables.Angdir = (AngleDirection) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_06E5:
                this.doc.DrawingVariables.AttMode = (AttMode) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0712:
                this.doc.DrawingVariables.AUnits = (AngleUnitType) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_073F:
                this.doc.DrawingVariables.AUprec = this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_076C:
                this.doc.DrawingVariables.CeColor = AciColor.FromCadIndex(this.chunk.ReadShort());
                this.chunk.Next();
                continue;
            Label_079E:
                this.doc.DrawingVariables.CeLtScale = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_07CB:
                this.doc.DrawingVariables.CeLtype = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                this.chunk.Next();
                continue;
            Label_07FE:
                this.doc.DrawingVariables.CeLweight = (Lineweight) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_082B:
                this.doc.DrawingVariables.CLayer = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                this.chunk.Next();
                continue;
            Label_085E:
                this.doc.DrawingVariables.CMLJust = (MLineJustification) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_088B:
                this.doc.DrawingVariables.CMLScale = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_08B8:
                str5 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                if (!string.IsNullOrEmpty(str5))
                {
                    this.doc.DrawingVariables.CMLStyle = str5;
                }
                this.chunk.Next();
                continue;
            Label_08FF:
                str6 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                if (!string.IsNullOrEmpty(str6))
                {
                    this.doc.DrawingVariables.DimStyle = str6;
                }
                this.chunk.Next();
                continue;
            Label_0946:
                num3 = this.chunk.ReadDouble();
                if (num3 > 0.0)
                {
                    this.doc.DrawingVariables.TextSize = num3;
                }
                this.chunk.Next();
                continue;
            Label_098A:
                str7 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                if (!string.IsNullOrEmpty(str7))
                {
                    this.doc.DrawingVariables.TextStyle = str7;
                }
                this.chunk.Next();
                continue;
            Label_09D1:
                this.doc.DrawingVariables.LastSavedBy = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                this.chunk.Next();
                continue;
            Label_0A04:
                this.doc.DrawingVariables.LUnits = (LinearUnitType) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0A31:
                this.doc.DrawingVariables.LUprec = this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0A5E:
                this.doc.DrawingVariables.DwgCodePage = this.chunk.ReadString();
                this.chunk.Next();
                continue;
            Label_0A8B:
                this.doc.DrawingVariables.Extnames = this.chunk.ReadBool();
                this.chunk.Next();
                continue;
            Label_0AB8:
                vector = Vector3.Zero;
                while ((this.chunk.Code != 0) && (this.chunk.Code != 9))
                {
                    switch (this.chunk.Code)
                    {
                        case 10:
                            vector.X = this.chunk.ReadDouble();
                            this.chunk.Next();
                            break;

                        case 20:
                            vector.Y = this.chunk.ReadDouble();
                            this.chunk.Next();
                            break;

                        case 30:
                            vector.Z = this.chunk.ReadDouble();
                            this.chunk.Next();
                            break;

                        default:
                            throw new Exception("Invalid code in InsBase header variable.");
                    }
                }
                this.doc.DrawingVariables.InsBase = vector;
                continue;
            Label_0B9C:
                this.doc.DrawingVariables.InsUnits = (DrawingUnits) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0BC9:
                this.doc.DrawingVariables.LtScale = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_0BF6:
                this.doc.DrawingVariables.LwDisplay = this.chunk.ReadBool();
                this.chunk.Next();
                continue;
            Label_0C23:
                this.doc.DrawingVariables.PdMode = (PointShape) this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0C50:
                this.doc.DrawingVariables.PdSize = this.chunk.ReadDouble();
                this.chunk.Next();
                continue;
            Label_0C7D:
                this.doc.DrawingVariables.PLineGen = this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0CAA:
                this.doc.DrawingVariables.PsLtScale = this.chunk.ReadShort();
                this.chunk.Next();
                continue;
            Label_0CD7:
                num = this.chunk.ReadDouble();
                if ((num < 1721426.0) || (num > 5373484.0))
                {
                    this.doc.DrawingVariables.TdCreate = DateTime.Now;
                }
                else
                {
                    this.doc.DrawingVariables.TdCreate = DrawingTime.FromJulianCalendar(num);
                }
                this.chunk.Next();
                continue;
            Label_0D44:
                num = this.chunk.ReadDouble();
                if ((num < 1721426.0) || (num > 5373484.0))
                {
                    this.doc.DrawingVariables.TduCreate = DateTime.Now;
                }
                else
                {
                    this.doc.DrawingVariables.TduCreate = DrawingTime.FromJulianCalendar(num);
                }
                this.chunk.Next();
                continue;
            Label_0DB1:
                num = this.chunk.ReadDouble();
                if ((num < 1721426.0) || (num > 5373484.0))
                {
                    this.doc.DrawingVariables.TdUpdate = DateTime.Now;
                }
                else
                {
                    this.doc.DrawingVariables.TdUpdate = DrawingTime.FromJulianCalendar(num);
                }
                this.chunk.Next();
                continue;
            Label_0E1E:
                num = this.chunk.ReadDouble();
                if ((num < 1721426.0) || (num > 5373484.0))
                {
                    this.doc.DrawingVariables.TduUpdate = DateTime.Now;
                }
                else
                {
                    this.doc.DrawingVariables.TduUpdate = DrawingTime.FromJulianCalendar(num);
                }
                this.chunk.Next();
                continue;
            Label_0E8B:
                num4 = this.chunk.ReadDouble();
                if ((num4 < 0.0) || (num4 > TimeSpan.MaxValue.TotalDays))
                {
                    this.doc.DrawingVariables.TdinDwg = TimeSpan.Zero;
                }
                else
                {
                    this.doc.DrawingVariables.TdinDwg = DrawingTime.EditingTime(num4);
                }
                this.chunk.Next();
                continue;
            Label_0F0C:
                while ((this.chunk.Code != 0) && (this.chunk.Code != 9))
                {
                    this.chunk.Next();
                }
            }
        }

        private Image ReadImage()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 u = Vector3.Zero;
            Vector3 v = Vector3.Zero;
            double num = 0.0;
            double num2 = 0.0;
            string str = null;
            ImageDisplayFlags flags = ImageDisplayFlags.UseClippingBoundary | ImageDisplayFlags.ShowImageWhenNotAlignedWithScreen | ImageDisplayFlags.ShowImage;
            bool flag = false;
            short num3 = 50;
            short num4 = 50;
            short num5 = 0;
            double x = 0.0;
            List<Vector2> vertexes = new List<Vector2>();
            ClippingBoundaryType rectangular = ClippingBoundaryType.Rectangular;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        u.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        v.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 14:
                    {
                        x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        u.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        v.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x18:
                    {
                        vertexes.Add(new Vector2(x, this.chunk.ReadDouble()));
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        u.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        v.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        flags = (ImageDisplayFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        rectangular = (ClippingBoundaryType) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 340:
                    {
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                    case 280:
                    {
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 0x119:
                    {
                        num3 = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x11a:
                    {
                        num4 = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x11b:
                    {
                        num5 = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x5b:
                    {
                        this.chunk.Next();
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            Vector3 zAxis = Vector3.CrossProduct(u, v);
            Vector3 vector5 = MathHelper.Transform(u, zAxis, CoordinateSystem.World, CoordinateSystem.Object);
            double num7 = Vector2.Angle(new Vector2(vector5.X, vector5.Y)) * 57.295779513082323;
            double num8 = u.Modulus();
            double num9 = v.Modulus();
            for (int i = 0; i < vertexes.Count; i++)
            {
                Vector2 vector6 = vertexes[i];
                double num12 = vector6.X * num8;
                vector6 = vertexes[i];
                double y = vector6.Y * num9;
                vertexes[i] = new Vector2(num12, y);
            }
            ClippingBoundary boundary = (rectangular == ClippingBoundaryType.Rectangular) ? new ClippingBoundary(vertexes[0], vertexes[1]) : new ClippingBoundary(vertexes);
            Image key = new Image {
                Width = num * num8,
                Height = num2 * num9,
                Position = zero,
                Normal = zAxis,
                Rotation = num7,
                DisplayOptions = flags,
                Clipping = flag,
                Brightness = num3,
                Contrast = num4,
                Fade = num5,
                ClippingBoundary = boundary
            };
            key.XData.AddRange(items);
            if (string.IsNullOrEmpty(str) || (str == "0"))
            {
                return null;
            }
            this.imgToImgDefHandles.Add(key, str);
            return key;
        }

        private ImageDefinition ReadImageDefinition()
        {
            string str = null;
            string str2 = null;
            string fileName = null;
            string name = null;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            ImageResolutionUnits unitless = ImageResolutionUnits.Unitless;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        num3 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 1:
                        fileName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 20:
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        num4 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x66:
                        if (this.chunk.ReadString().Equals("{ACAD_REACTORS", StringComparison.OrdinalIgnoreCase))
                        {
                            this.chunk.Next();
                            while (this.chunk.Code != 0x66)
                            {
                                if ((this.chunk.Code == 330) && string.IsNullOrEmpty(str2))
                                {
                                    str2 = this.chunk.ReadHex();
                                }
                                this.chunk.Next();
                            }
                        }
                        this.chunk.Next();
                        break;

                    case 0x119:
                        unitless = (ImageResolutionUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 330:
                        str2 = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (str2 > null)
            {
                DictionaryObject obj2 = this.dictionaries[str2];
                if (str == null)
                {
                    throw new NullReferenceException();
                }
                name = obj2.Entries[str];
            }
            double num5 = UnitHelper.ConversionFactor((ImageUnits) unitless, DrawingUnits.Millimeters);
            ImageDefinition definition = new ImageDefinition(fileName, name, (int) num, num5 / num3, (int) num2, num5 / num4, unitless) {
                Handle = str
            };
            this.imgDefHandles.Add(definition.Handle, definition);
            return definition;
        }

        private ImageDefinitionReactor ReadImageDefReactor()
        {
            string str = null;
            string imageHandle = null;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 330:
                        imageHandle = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            return new ImageDefinitionReactor(imageHandle) { Handle = str };
        }

        private Insert ReadInsert(bool isBlockEntity)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            Vector3 vector3 = new Vector3(1.0, 1.0, 1.0);
            double num = 0.0;
            string name = null;
            Block block = null;
            List<netDxf.Entities.Attribute> attributes = new List<netDxf.Entities.Attribute>();
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (!isBlockEntity)
                        {
                            block = this.GetBlock(name);
                        }
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x29:
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x2a:
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x2b:
                        vector3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 50:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string str3 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(str3));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            if (this.chunk.ReadString() == "ATTRIB")
            {
                while (this.chunk.ReadString() != "SEQEND")
                {
                    netDxf.Entities.Attribute item = this.ReadAttribute(block, isBlockEntity);
                    if (item > null)
                    {
                        attributes.Add(item);
                    }
                }
            }
            string str2 = string.Empty;
            if (this.chunk.ReadString() == "SEQEND")
            {
                this.chunk.Next();
                while (this.chunk.Code > 0)
                {
                    switch (this.chunk.Code)
                    {
                        case 5:
                            str2 = this.chunk.ReadHex();
                            this.chunk.Next();
                            break;

                        case 8:
                            this.chunk.Next();
                            break;

                        default:
                            this.chunk.Next();
                            break;
                    }
                }
            }
            Vector3 vector4 = MathHelper.Transform(zero, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            Insert key = new Insert(attributes) {
                Block = block,
                Position = vector4,
                Rotation = num,
                Scale = vector3,
                Normal = unitZ
            };
            key.EndSequence.Handle = str2;
            key.XData.AddRange(items);
            if (isBlockEntity)
            {
                this.nestedInserts.Add(key, name);
            }
            return key;
        }

        private Layer ReadLayer()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbLayerTableRecord");
            string str = null;
            bool flag = true;
            bool flag2 = true;
            AciColor color = AciColor.Default;
            Linetype byLayer = Linetype.ByLayer;
            Lineweight lineweight = Lineweight.Default;
            LayerFlags none = LayerFlags.None;
            Transparency transparency = new Transparency(0);
            XDataDictionary dictionary = new XDataDictionary();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 0x3e:
                    {
                        short num2 = this.chunk.ReadShort();
                        if (num2 < 0)
                        {
                            flag = false;
                            num2 = Math.Abs(num2);
                        }
                        if (!color.UseTrueColor)
                        {
                            color = AciColor.FromCadIndex(num2);
                        }
                        this.chunk.Next();
                        break;
                    }
                    case 70:
                        none = (LayerFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 2:
                        str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 6:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        byLayer = this.GetLinetype(name);
                        this.chunk.Next();
                        break;
                    }
                    case 290:
                        flag2 = this.chunk.ReadBool();
                        this.chunk.Next();
                        break;

                    case 370:
                        lineweight = (Lineweight) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 420:
                        color = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(new ApplicationRegistry(name));
                        dictionary.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            if (string.IsNullOrEmpty(str) || !TableObject.IsValidName(str))
            {
                return null;
            }
            if (dictionary.TryGetValue("AcCmTransparency", out XData data))
            {
                foreach (XDataRecord record in data.XDataRecord)
                {
                    if (record.Code == XDataCode.Int32)
                    {
                        transparency = Transparency.FromAlphaValue((int) record.Value);
                        break;
                    }
                }
            }
            return new Layer(str, false) { 
                Color = color,
                Linetype = byLayer,
                IsVisible = flag,
                IsFrozen = none.HasFlag(LayerFlags.Frozen),
                IsLocked = none.HasFlag(LayerFlags.Locked),
                Plot = flag2,
                Lineweight = lineweight,
                Transparency = transparency
            };
        }

        private Layout ReadLayout()
        {
            PlotSettings settings = new PlotSettings();
            string key = null;
            string name = null;
            short num = 1;
            Vector2 vector = new Vector2(-20.0, -7.5);
            Vector2 vector2 = new Vector2(277.0, 202.5);
            Vector3 zero = Vector3.Zero;
            Vector3 vector4 = new Vector3(25.7, 19.5, 0.0);
            Vector3 vector5 = new Vector3(231.3, 175.5, 0.0);
            double num2 = 0.0;
            Vector3 vector6 = Vector3.Zero;
            Vector3 unitX = Vector3.UnitX;
            Vector3 unitY = Vector3.UnitY;
            string objectHandle = null;
            string str4 = this.chunk.ReadString();
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 0:
                        throw new Exception($"Premature end of object {str4} definition.");

                    case 5:
                        key = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 0x66:
                        this.ReadExtensionDictionaryGroup();
                        this.chunk.Next();
                        break;

                    case 330:
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 100:
                    {
                        if (this.chunk.ReadString() == "AcDbPlotSettings")
                        {
                            settings = this.ReadPlotSettings();
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x92:
                    {
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 330:
                    {
                        objectHandle = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 1:
                    {
                        name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        vector.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        vector6.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 14:
                    {
                        vector4.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 15:
                    {
                        vector5.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x10:
                    {
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x11:
                    {
                        unitY.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        vector.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        vector6.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x18:
                    {
                        vector4.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x19:
                    {
                        vector5.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1a:
                    {
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1b:
                    {
                        unitY.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x21:
                    {
                        vector6.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x22:
                    {
                        vector4.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x23:
                    {
                        vector5.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x24:
                    {
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x25:
                    {
                        unitY.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        num = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            if (key > null)
            {
                if (this.blockRecordPointerToLayout.TryGetValue(key, out BlockRecord objectByHandle))
                {
                    this.blockRecordPointerToLayout.Remove(key);
                }
                else
                {
                    objectByHandle = this.doc.GetObjectByHandle(objectHandle) as BlockRecord;
                }
            }
            else
            {
                objectByHandle = this.doc.GetObjectByHandle(objectHandle) as BlockRecord;
            }
            if ((objectByHandle > null) && (this.doc.Blocks.GetReferences(objectByHandle.Name).Count > 0))
            {
                objectByHandle = null;
            }
            return new Layout(name) { 
                PlotSettings = settings,
                Handle = key,
                MinLimit = vector,
                MaxLimit = vector2,
                BasePoint = zero,
                MinExtents = vector4,
                MaxExtents = vector5,
                Elevation = num2,
                UcsOrigin = vector6,
                UcsXAxis = unitX,
                UcsYAxis = unitY,
                TabOrder = (num > 0) ? num : ((short) (this.doc.Layouts.Count + 1)),
                AssociatedBlock = (objectByHandle == null) ? null : this.doc.Blocks[objectByHandle.Name]
            };
        }

        private Leader ReadLeader()
        {
            DimensionStyle dimensionStyle = DimensionStyle.Default;
            bool flag = true;
            LeaderPathType straightLineSegements = LeaderPathType.StraightLineSegements;
            bool flag2 = false;
            List<Vector3> points = null;
            AciColor byLayer = AciColor.ByLayer;
            string str = string.Empty;
            Vector3 unitZ = Vector3.UnitZ;
            double z = 0.0;
            Vector3 zero = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 3:
                    {
                        string dimStyle = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(dimStyle))
                        {
                            dimStyle = this.doc.DrawingVariables.DimStyle;
                        }
                        dimensionStyle = this.GetDimensionStyle(dimStyle);
                        this.chunk.Next();
                        break;
                    }
                    case 10:
                        points = this.ReadLeaderVertexes();
                        break;

                    case 0x47:
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 0x48:
                        straightLineSegements = (LeaderPathType) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x49:
                        this.chunk.Next();
                        break;

                    case 0x4a:
                        this.chunk.Next();
                        break;

                    case 0x4b:
                        flag2 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 0x4c:
                        this.chunk.Next();
                        break;

                    case 0x4d:
                        byLayer = AciColor.FromCadIndex(this.chunk.ReadShort());
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0xd5:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0xdf:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0xe9:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 340:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            if (points == null)
            {
                return null;
            }
            if (flag2 && (points.Count >= 3))
            {
                points.RemoveAt(points.Count - 2);
            }
            IList<Vector3> list3 = MathHelper.Transform(points, unitZ, CoordinateSystem.World, CoordinateSystem.Object);
            List<Vector2> vertexes = new List<Vector2>();
            foreach (Vector3 vector4 in list3)
            {
                vertexes.Add(new Vector2(vector4.X, vector4.Y));
                z = vector4.Z;
            }
            Vector3 vector3 = MathHelper.Transform(zero, unitZ, CoordinateSystem.World, CoordinateSystem.Object);
            Leader key = new Leader(vertexes) {
                Style = dimensionStyle,
                ShowArrowhead = flag,
                PathType = straightLineSegements,
                LineColor = byLayer,
                Elevation = z,
                Normal = unitZ,
                Offset = new Vector2(vector3.X, vector3.Y),
                TextVerticalPosition = LeaderTextVerticalPosition.Above
            };
            key.XData.AddRange(items);
            this.leaderAnnotation.Add(key, str);
            return key;
        }

        private List<Vector3> ReadLeaderVertexes()
        {
            List<Vector3> list = new List<Vector3>();
            while (this.chunk.Code == 10)
            {
                Vector3 zero = Vector3.Zero;
                zero.X = this.chunk.ReadDouble();
                this.chunk.Next();
                zero.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                if (this.chunk.Code == 30)
                {
                    zero.Z = this.chunk.ReadDouble();
                    this.chunk.Next();
                }
                list.Add(zero);
            }
            return list;
        }

        private netDxf.Entities.Line ReadLine()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            double num = 0.0;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x27:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            netDxf.Entities.Line line = new netDxf.Entities.Line {
                StartPoint = zero,
                EndPoint = vector2,
                Normal = unitZ,
                Thickness = num
            };
            line.XData.AddRange(items);
            return line;
        }

        private LinearDimension ReadLinearDimension(Vector3 defPoint, Vector3 normal)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            double num = 0.0;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 0x17:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x18:
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 13:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 14:
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x21:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x22:
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 50:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x34:
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            List<Vector3> points = new List<Vector3> {
                zero,
                vector2,
                defPoint
            };
            IList<Vector3> list2 = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            LinearDimension dimension1 = new LinearDimension();
            Vector3 vector4 = list2[0];
            vector4 = list2[0];
            dimension1.FirstReferencePoint = new Vector2(vector4.X, vector4.Y);
            vector4 = list2[1];
            vector4 = list2[1];
            dimension1.SecondReferencePoint = new Vector2(vector4.X, vector4.Y);
            dimension1.Rotation = num;
            vector4 = list2[2];
            dimension1.Elevation = vector4.Z;
            dimension1.Normal = normal;
            LinearDimension dimension = dimension1;
            vector4 = list2[2];
            vector4 = list2[2];
            Vector2 point = new Vector2(vector4.X, vector4.Y);
            dimension.SetDimensionLinePosition(point);
            dimension.XData.AddRange(items);
            return dimension;
        }

        private Linetype ReadLinetype()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbLinetypeTableRecord");
            string a = null;
            string description = null;
            List<double> segments = new List<double>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 40:
                        this.chunk.Next();
                        break;

                    case 0x31:
                        segments.Add(this.chunk.ReadDouble());
                        this.chunk.Next();
                        break;

                    case 0x49:
                        this.chunk.Next();
                        break;

                    case 2:
                        a = this.chunk.ReadString();
                        if (string.Equals(a, "ByLayer", StringComparison.OrdinalIgnoreCase))
                        {
                            a = "ByLayer";
                        }
                        else if (string.Equals(a, "ByBlock", StringComparison.OrdinalIgnoreCase))
                        {
                            a = "ByBlock";
                        }
                        a = this.DecodeEncodedNonAsciiCharacters(a);
                        this.chunk.Next();
                        break;

                    case 3:
                        description = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (string.IsNullOrEmpty(a) || !TableObject.IsValidName(a))
            {
                return null;
            }
            return new Linetype(a, segments, description, false);
        }

        private LwPolyline ReadLwPolyline()
        {
            double num = 0.0;
            double num2 = 0.0;
            PolylinetypeFlags openPolyline = PolylinetypeFlags.OpenPolyline;
            double width = -1.0;
            List<LwPolylineVertex> collection = new List<LwPolylineVertex>();
            LwPolylineVertex item = null;
            double x = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 0x26:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x27:
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                    {
                        double num7 = this.chunk.ReadDouble();
                        if ((item > null) && (num7 >= 0.0))
                        {
                            item.StartWidth = num7;
                        }
                        this.chunk.Next();
                        break;
                    }
                    case 0x29:
                    {
                        double num8 = this.chunk.ReadDouble();
                        if ((item > null) && (num8 >= 0.0))
                        {
                            item.EndWidth = num8;
                        }
                        this.chunk.Next();
                        break;
                    }
                    case 0x2a:
                        if (item > null)
                        {
                            item.Bulge = this.chunk.ReadDouble();
                        }
                        this.chunk.Next();
                        break;

                    case 0x2b:
                        width = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 70:
                        openPolyline = (PolylinetypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 10:
                        x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                    {
                        double y = this.chunk.ReadDouble();
                        item = new LwPolylineVertex(x, y);
                        collection.Add(item);
                        this.chunk.Next();
                        break;
                    }
                    case 90:
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(data);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            LwPolyline polyline = new LwPolyline {
                Elevation = num,
                Thickness = num2,
                Flags = openPolyline,
                Normal = unitZ
            };
            polyline.Vertexes.AddRange(collection);
            if (width >= 0.0)
            {
                polyline.SetConstantWidth(width);
            }
            polyline.XData.AddRange(items);
            return polyline;
        }

        private Mesh ReadMesh()
        {
            int num = 0;
            List<Vector3> vertexes = null;
            List<int[]> faces = null;
            List<MeshEdge> edges = null;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                int num6;
                switch (this.chunk.Code)
                {
                    case 0x5b:
                    {
                        num = this.chunk.ReadInt();
                        if ((num < 0) || (num > 0xff))
                        {
                            num = 0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x5c:
                    {
                        int count = this.chunk.ReadInt();
                        this.chunk.Next();
                        vertexes = this.ReadMeshVertexes(count);
                        continue;
                    }
                    case 0x5d:
                    {
                        int size = this.chunk.ReadInt();
                        this.chunk.Next();
                        faces = this.ReadMeshFaces(size);
                        continue;
                    }
                    case 0x5e:
                    {
                        int count = this.chunk.ReadInt();
                        this.chunk.Next();
                        edges = this.ReadMeshEdges(count);
                        continue;
                    }
                    case 0x5f:
                        num6 = this.chunk.ReadInt();
                        this.chunk.Next();
                        if (edges == null)
                        {
                            throw new NullReferenceException("The edges list is not initialized.");
                        }
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                    default:
                        goto Label_0182;
                }
                if (num6 != edges.Count)
                {
                    throw new Exception("The number of edge creases must be the same as the number of edges.");
                }
                this.ReadMeshEdgeCreases(edges);
                continue;
            Label_0182:
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            Mesh mesh = new Mesh(vertexes, faces, edges) {
                SubdivisionLevel = (byte) num
            };
            mesh.XData.AddRange(items);
            return mesh;
        }

        private void ReadMeshEdgeCreases(IEnumerable<MeshEdge> edges)
        {
            foreach (MeshEdge edge in edges)
            {
                edge.Crease = this.chunk.ReadDouble();
                this.chunk.Next();
            }
        }

        private List<MeshEdge> ReadMeshEdges(int count)
        {
            List<MeshEdge> list = new List<MeshEdge>(count);
            for (int i = 0; i < count; i++)
            {
                int startVertexIndex = this.chunk.ReadInt();
                this.chunk.Next();
                int endVertexIndex = this.chunk.ReadInt();
                this.chunk.Next();
                list.Add(new MeshEdge(startVertexIndex, endVertexIndex));
            }
            return list;
        }

        private List<int[]> ReadMeshFaces(int size)
        {
            Debug.Assert(size > 0, "The size of face list must be greater than zero.");
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < size; i++)
            {
                int num2 = this.chunk.ReadInt();
                this.chunk.Next();
                int[] item = new int[num2];
                for (int j = 0; j < num2; j++)
                {
                    item[j] = this.chunk.ReadInt();
                    this.chunk.Next();
                }
                list.Add(item);
                i += num2;
            }
            return list;
        }

        private List<Vector3> ReadMeshVertexes(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "The number of vertexes must be greater than zero.");
            }
            List<Vector3> list = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
            {
                double x = this.chunk.ReadDouble();
                this.chunk.Next();
                double y = this.chunk.ReadDouble();
                this.chunk.Next();
                double z = this.chunk.ReadDouble();
                this.chunk.Next();
                list.Add(new Vector3(x, y, z));
            }
            return list;
        }

        private MLine ReadMLine()
        {
            string cMLStyle = null;
            double num = 1.0;
            MLineJustification zero = MLineJustification.Zero;
            MLineFlags has = MLineFlags.Has;
            int numVertexes = 0;
            int numStyleElements = 0;
            double elevation = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<MLineVertex> collection = new List<MLineVertex>();
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 11:
                        collection = this.ReadMLineSegments(numVertexes, numStyleElements, unitZ, out elevation);
                        break;

                    case 20:
                        this.chunk.Next();
                        break;

                    case 30:
                        this.chunk.Next();
                        break;

                    case 2:
                        cMLStyle = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(cMLStyle))
                        {
                            cMLStyle = this.doc.DrawingVariables.CMLStyle;
                        }
                        this.chunk.Next();
                        break;

                    case 10:
                        this.chunk.Next();
                        break;

                    case 70:
                        zero = (MLineJustification) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x47:
                        has = (MLineFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x48:
                        numVertexes = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x49:
                        numStyleElements = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            MLine key = new MLine {
                Elevation = elevation,
                Scale = num,
                Justification = zero,
                Normal = unitZ,
                Flags = has
            };
            key.Vertexes.AddRange(collection);
            key.XData.AddRange(items);
            this.mLineToStyleNames.Add(key, cMLStyle);
            return key;
        }

        private List<MLineVertex> ReadMLineSegments(int numVertexes, int numStyleElements, Vector3 normal, out double elevation)
        {
            elevation = 0.0;
            List<MLineVertex> list = new List<MLineVertex>();
            Matrix3 matrix = MathHelper.ArbitraryAxis(normal).Transpose();
            for (int i = 0; i < numVertexes; i++)
            {
                Vector3 vector = new Vector3 {
                    X = this.chunk.ReadDouble()
                };
                this.chunk.Next();
                vector.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                vector.Z = this.chunk.ReadDouble();
                this.chunk.Next();
                Vector3 vector2 = new Vector3 {
                    X = this.chunk.ReadDouble()
                };
                this.chunk.Next();
                vector2.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                vector2.Z = this.chunk.ReadDouble();
                this.chunk.Next();
                Vector3 vector3 = new Vector3 {
                    X = this.chunk.ReadDouble()
                };
                this.chunk.Next();
                vector3.Y = this.chunk.ReadDouble();
                this.chunk.Next();
                vector3.Z = this.chunk.ReadDouble();
                this.chunk.Next();
                List<double>[] distances = new List<double>[numStyleElements];
                for (int j = 0; j < numStyleElements; j++)
                {
                    distances[j] = new List<double>();
                    short num3 = this.chunk.ReadShort();
                    this.chunk.Next();
                    for (short k = 0; k < num3; k = (short) (k + 1))
                    {
                        distances[j].Add(this.chunk.ReadDouble());
                        this.chunk.Next();
                    }
                    short num4 = this.chunk.ReadShort();
                    this.chunk.Next();
                    for (short m = 0; m < num4; m = (short) (m + 1))
                    {
                        this.chunk.Next();
                    }
                }
                if (!normal.Equals(Vector3.UnitZ))
                {
                    vector = (Vector3) (matrix * vector);
                    vector2 = (Vector3) (matrix * vector2);
                    vector3 = (Vector3) (matrix * vector3);
                }
                MLineVertex item = new MLineVertex(new Vector2(vector.X, vector.Y), new Vector2(vector2.X, vector2.Y), new Vector2(vector3.X, vector3.Y), distances);
                elevation = vector.Z;
                list.Add(item);
            }
            return list;
        }

        private MLineStyle ReadMLineStyle()
        {
            string str = null;
            string str2 = null;
            string description = null;
            AciColor byLayer = AciColor.ByLayer;
            double num = 90.0;
            double num2 = 90.0;
            MLineStyleFlags none = MLineStyleFlags.None;
            List<MLineStyleElement> elements = new List<MLineStyleElement>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 2:
                    {
                        str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 3:
                    {
                        description = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 5:
                    {
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x33:
                    {
                        num = this.chunk.ReadDouble();
                        if ((num < 10.0) || (num > 170.0))
                        {
                            num = 90.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x34:
                    {
                        num2 = this.chunk.ReadDouble();
                        if ((num2 < 10.0) || (num2 > 170.0))
                        {
                            num2 = 90.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e:
                    {
                        if (!byLayer.UseTrueColor)
                        {
                            byLayer = AciColor.FromCadIndex(this.chunk.ReadShort());
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        none = (MLineStyleFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        short numElements = this.chunk.ReadShort();
                        elements = this.ReadMLineStyleElements(numElements);
                        continue;
                    }
                    case 420:
                    {
                        byLayer = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            if (string.IsNullOrEmpty(str2))
            {
                return null;
            }
            return new MLineStyle(str2, elements, description) { 
                Handle = str,
                FillColor = byLayer,
                Flags = none,
                StartAngle = num,
                EndAngle = num2
            };
        }

        private List<MLineStyleElement> ReadMLineStyleElements(short numElements)
        {
            List<MLineStyleElement> list = new List<MLineStyleElement>();
            this.chunk.Next();
            for (short i = 0; i < numElements; i = (short) (i + 1))
            {
                double offset = this.chunk.ReadDouble();
                this.chunk.Next();
                AciColor color = AciColor.FromCadIndex(this.chunk.ReadShort());
                this.chunk.Next();
                if (this.chunk.Code == 420)
                {
                    color = AciColor.FromTrueColor(this.chunk.ReadInt());
                    this.chunk.Next();
                }
                string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                Linetype linetype = this.GetLinetype(name);
                this.chunk.Next();
                MLineStyleElement item = new MLineStyleElement(offset) {
                    Color = color,
                    Linetype = linetype
                };
                list.Add(item);
            }
            return list;
        }

        private MText ReadMText()
        {
            Vector3 zero = Vector3.Zero;
            Vector2 unitX = Vector2.UnitX;
            Vector3 unitZ = Vector3.UnitZ;
            double textSize = 0.0;
            double num2 = 0.0;
            double num3 = 1.0;
            double num4 = 0.0;
            bool flag = false;
            MTextAttachmentPoint topLeft = MTextAttachmentPoint.TopLeft;
            TextStyle textStyle = TextStyle.Default;
            string str = string.Empty;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                    {
                        str = str + this.chunk.ReadString();
                        this.chunk.Next();
                        continue;
                    }
                    case 3:
                    {
                        str = str + this.chunk.ReadString();
                        this.chunk.Next();
                        continue;
                    }
                    case 7:
                    {
                        string textStyle = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(textStyle))
                        {
                            textStyle = this.doc.DrawingVariables.TextStyle;
                        }
                        textStyle = this.GetTextStyle(textStyle);
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        textSize = this.chunk.ReadDouble();
                        if (textSize <= 0.0)
                        {
                            textSize = this.doc.DrawingVariables.TextSize;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        num2 = this.chunk.ReadDouble();
                        if (num2 < 0.0)
                        {
                            num2 = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2c:
                    {
                        num3 = this.chunk.ReadDouble();
                        if ((num3 < 0.25) || (num3 > 4.0))
                        {
                            num3 = 1.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 50:
                    {
                        flag = true;
                        num4 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        topLeft = (MTextAttachmentPoint) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            str = this.DecodeEncodedNonAsciiCharacters(str);
            if (!this.isBinary)
            {
                str = str.Replace("^I", "\t");
            }
            MText text = new MText {
                Value = str,
                Position = zero,
                Height = textSize,
                RectangleWidth = num2,
                Style = textStyle,
                LineSpacingFactor = num3,
                AttachmentPoint = topLeft,
                Rotation = flag ? num4 : (Vector2.Angle(unitX) * 57.295779513082323),
                Normal = unitZ
            };
            text.XData.AddRange(items);
            return text;
        }

        private void ReadObjects()
        {
            Debug.Assert(this.chunk.ReadString() == "OBJECTS");
            this.chunk.Next();
            while (this.chunk.ReadString() != "ENDSEC")
            {
                ImageDefinition definition;
                ImageDefinitionReactor reactor;
                MLineStyle style;
                Group group;
                Layout layout;
                UnderlayDgnDefinition definition2;
                UnderlayDwfDefinition definition3;
                UnderlayPdfDefinition definition4;
                string s = this.chunk.ReadString();
                switch (<PrivateImplementationDetails>.ComputeStringHash(s))
                {
                    case 0x1673cd27:
                        if (s == "MLINESTYLE")
                        {
                            goto Label_0262;
                        }
                        goto Label_0351;

                    case 0x30b5cef5:
                        if (s == "DICTIONARY")
                        {
                            break;
                        }
                        goto Label_0351;

                    case 0x39f7a7af:
                        if (s == "DGNDEFINITION")
                        {
                            goto Label_02E5;
                        }
                        goto Label_0351;

                    case 0x822bfec:
                        if (s == "GROUP")
                        {
                            goto Label_0283;
                        }
                        goto Label_0351;

                    case 0x9c59631:
                        if (s == "IMAGEDEF")
                        {
                            goto Label_0206;
                        }
                        goto Label_0351;

                    case 0x54d3987f:
                        if (s == "LAYOUT")
                        {
                            goto Label_02A4;
                        }
                        goto Label_0351;

                    case 0x83b9e1d7:
                        if (s == "DWFDEFINITION")
                        {
                            goto Label_0309;
                        }
                        goto Label_0351;

                    case 0xb998c1e1:
                        if (s == "RASTERVARIABLES")
                        {
                            goto Label_01EF;
                        }
                        goto Label_0351;

                    case 0xbbf0e830:
                        if (s == "PDFDEFINITION")
                        {
                            goto Label_032D;
                        }
                        goto Label_0351;

                    case 0xd31c9964:
                        if (s == "IMAGEDEF_REACTOR")
                        {
                            goto Label_0225;
                        }
                        goto Label_0351;

                    default:
                        goto Label_0351;
                }
                DictionaryObject obj2 = this.ReadDictionary();
                this.dictionaries.Add(obj2.Handle, obj2);
                if (this.namedDictionary == null)
                {
                    this.CreateObjectCollection(obj2);
                    this.namedDictionary = obj2;
                }
                continue;
            Label_01EF:
                this.doc.RasterVariables = this.ReadRasterVariables();
                continue;
            Label_0206:
                definition = this.ReadImageDefinition();
                this.doc.ImageDefinitions.Add(definition, false);
                continue;
            Label_0225:
                reactor = this.ReadImageDefReactor();
                if (!this.imageDefReactors.ContainsKey(reactor.ImageHandle))
                {
                    this.imageDefReactors.Add(reactor.ImageHandle, reactor);
                }
                continue;
            Label_0262:
                style = this.ReadMLineStyle();
                this.doc.MlineStyles.Add(style, false);
                continue;
            Label_0283:
                group = this.ReadGroup();
                this.doc.Groups.Add(group, false);
                continue;
            Label_02A4:
                layout = this.ReadLayout();
                if (layout.AssociatedBlock == null)
                {
                    this.orphanLayouts.Add(layout);
                }
                else
                {
                    this.doc.Layouts.Add(layout, false);
                }
                continue;
            Label_02E5:
                definition2 = (UnderlayDgnDefinition) this.ReadUnderlayDefinition(UnderlayType.DGN);
                this.doc.UnderlayDgnDefinitions.Add(definition2, false);
                continue;
            Label_0309:
                definition3 = (UnderlayDwfDefinition) this.ReadUnderlayDefinition(UnderlayType.DWF);
                this.doc.UnderlayDwfDefinitions.Add(definition3, false);
                continue;
            Label_032D:
                definition4 = (UnderlayPdfDefinition) this.ReadUnderlayDefinition(UnderlayType.PDF);
                this.doc.UnderlayPdfDefinitions.Add(definition4, false);
                continue;
            Label_0351:
                this.chunk.Next();
                if (this.chunk.Code > 0)
                {
                    goto Label_0351;
                }
            }
            this.RelinkOrphanLayouts();
            if (this.doc.RasterVariables == null)
            {
                this.doc.RasterVariables = new RasterVariables();
            }
        }

        private OrdinateDimension ReadOrdinateDimension(Vector3 defPoint, OrdinateDimensionAxis axis, Vector3 normal, double rotation)
        {
            Vector3 zero = Vector3.Zero;
            Vector3 point = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 13:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 14:
                        point.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x17:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x18:
                        point.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x21:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x22:
                        point.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 vector3 = MathHelper.Transform(defPoint, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 vector4 = new Vector2(vector3.X, vector3.Y);
            vector3 = MathHelper.Transform(zero, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 vector5 = MathHelper.Transform(new Vector2(vector3.X, vector3.Y) - vector4, rotation * 0.017453292519943295, CoordinateSystem.World, CoordinateSystem.Object);
            vector3 = MathHelper.Transform(point, normal, CoordinateSystem.World, CoordinateSystem.Object);
            Vector2 vector6 = MathHelper.Transform(new Vector2(vector3.X, vector3.Y) - vector4, rotation * 0.017453292519943295, CoordinateSystem.World, CoordinateSystem.Object);
            double num = (axis == OrdinateDimensionAxis.X) ? (vector6.Y - vector5.Y) : (vector6.X - vector5.X);
            OrdinateDimension dimension = new OrdinateDimension {
                Origin = vector4,
                ReferencePoint = vector5,
                Length = num,
                Rotation = rotation,
                Axis = axis
            };
            dimension.XData.AddRange(items);
            return dimension;
        }

        private PlotSettings ReadPlotSettings()
        {
            PlotSettings settings = new PlotSettings();
            Vector2 paperSize = settings.PaperSize;
            Vector2 windowBottomLeft = settings.WindowBottomLeft;
            Vector2 windowUpRight = settings.WindowUpRight;
            Vector2 paperImageOrigin = settings.PaperImageOrigin;
            this.chunk.Next();
            while (this.chunk.Code != 100)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                    {
                        settings.PageSetupName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        settings.PlotterName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 4:
                    {
                        settings.PaperSizeName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 6:
                    {
                        settings.ViewName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 7:
                    {
                        settings.CurrentStyleSheet = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        settings.LeftMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        settings.BottomMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2a:
                    {
                        settings.RightMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2b:
                    {
                        settings.TopMargin = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2c:
                    {
                        paperSize.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2d:
                    {
                        paperSize.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x30:
                    {
                        windowBottomLeft.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x31:
                    {
                        windowUpRight.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        settings.Flags = (PlotFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        settings.PaperUnits = (PlotPaperUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x49:
                    {
                        settings.PaperRotation = (PlotRotation) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 140:
                    {
                        windowBottomLeft.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x8d:
                    {
                        windowUpRight.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x8e:
                    {
                        settings.PrintScaleNumerator = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x8f:
                    {
                        settings.PrintScaleDenominator = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x94:
                    {
                        paperImageOrigin.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x95:
                    {
                        paperImageOrigin.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            settings.PaperSize = paperSize;
            settings.WindowBottomLeft = windowBottomLeft;
            settings.WindowUpRight = windowUpRight;
            settings.PaperImageOrigin = paperImageOrigin;
            return settings;
        }

        private Point ReadPoint()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            double num = 0.0;
            double num2 = 0.0;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x27:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 50:
                        num2 = 360.0 - this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Point point = new Point {
                Position = zero,
                Thickness = num,
                Rotation = num2,
                Normal = unitZ
            };
            point.XData.AddRange(items);
            return point;
        }

        private EntityObject ReadPolyline()
        {
            EntityObject obj2;
            PolylinetypeFlags openPolyline = PolylinetypeFlags.OpenPolyline;
            PolylineSmoothType noSmooth = PolylineSmoothType.NoSmooth;
            double num = 0.0;
            double num2 = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<Vertex> list = new List<Vertex>();
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 30:
                    {
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x27:
                    {
                        num2 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        openPolyline = (PolylinetypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4b:
                    {
                        noSmooth = (PolylineSmoothType) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            while (this.chunk.ReadString() != "SEQEND")
            {
                if (this.chunk.ReadString() == "VERTEX")
                {
                    Vertex item = this.ReadVertex();
                    list.Add(item);
                }
            }
            this.chunk.Next();
            string str = null;
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 8:
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            bool isClosed = openPolyline.HasFlag(PolylinetypeFlags.ClosedPolylineOrClosedPolygonMeshInM);
            if (openPolyline.HasFlag(PolylinetypeFlags.Polyline3D))
            {
                List<PolylineVertex> vertexes = new List<PolylineVertex>();
                foreach (Vertex vertex2 in list)
                {
                    PolylineVertex item = new PolylineVertex {
                        Flags = vertex2.Flags,
                        Position = vertex2.Position,
                        Handle = vertex2.Handle
                    };
                    vertexes.Add(item);
                }
                netDxf.Entities.Polyline polyline1 = new netDxf.Entities.Polyline(vertexes, isClosed) {
                    Flags = openPolyline,
                    SmoothType = noSmooth,
                    Normal = unitZ
                };
                obj2 = polyline1;
                ((netDxf.Entities.Polyline) obj2).EndSequence.Handle = str;
            }
            else if (openPolyline.HasFlag(PolylinetypeFlags.PolyfaceMesh))
            {
                List<PolyfaceMeshVertex> vertexes = new List<PolyfaceMeshVertex>();
                List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>();
                foreach (Vertex vertex4 in list)
                {
                    if (vertex4.Flags.HasFlag(VertexTypeFlags.PolyfaceMeshVertex | VertexTypeFlags.Polygon3dMesh))
                    {
                        PolyfaceMeshVertex item = new PolyfaceMeshVertex {
                            Location = vertex4.Position,
                            Handle = vertex4.Handle
                        };
                        vertexes.Add(item);
                    }
                    else if (vertex4.Flags.HasFlag(VertexTypeFlags.PolyfaceMeshVertex))
                    {
                        PolyfaceMeshFace item = new PolyfaceMeshFace(vertex4.VertexIndexes) {
                            Handle = vertex4.Handle
                        };
                        faces.Add(item);
                    }
                }
                PolyfaceMesh mesh1 = new PolyfaceMesh(vertexes, faces) {
                    Normal = unitZ
                };
                obj2 = mesh1;
                ((PolyfaceMesh) obj2).EndSequence.Handle = str;
            }
            else
            {
                List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>();
                foreach (Vertex vertex6 in list)
                {
                    LwPolylineVertex item = new LwPolylineVertex {
                        Position = new Vector2(vertex6.Position.X, vertex6.Position.Y),
                        StartWidth = vertex6.StartWidth,
                        Bulge = vertex6.Bulge,
                        EndWidth = vertex6.EndWidth
                    };
                    vertexes.Add(item);
                }
                LwPolyline polyline2 = new LwPolyline(vertexes, isClosed) {
                    Flags = openPolyline,
                    Thickness = num2,
                    Elevation = num,
                    Normal = unitZ
                };
                obj2 = polyline2;
            }
            obj2.XData.AddRange(items);
            return obj2;
        }

        private RadialDimension ReadRadialDimension(Vector3 defPoint, Vector3 midtxtPoint, Vector3 normal)
        {
            Vector3 zero = Vector3.Zero;
            List<XData> items = new List<XData>();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 15:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x19:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x23:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 40:
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            List<Vector3> points = new List<Vector3> {
                zero,
                defPoint
            };
            IList<Vector3> list2 = MathHelper.Transform(points, normal, CoordinateSystem.World, CoordinateSystem.Object);
            double num = Vector3.Distance(defPoint, midtxtPoint);
            RadialDimension dimension1 = new RadialDimension();
            Vector3 vector2 = list2[1];
            vector2 = list2[1];
            dimension1.CenterPoint = new Vector2(vector2.X, vector2.Y);
            vector2 = list2[0];
            vector2 = list2[0];
            dimension1.ReferencePoint = new Vector2(vector2.X, vector2.Y);
            vector2 = list2[1];
            dimension1.Elevation = vector2.Z;
            dimension1.Normal = normal;
            dimension1.Offset = num;
            RadialDimension dimension = dimension1;
            dimension.XData.AddRange(items);
            return dimension;
        }

        private RasterVariables ReadRasterVariables()
        {
            RasterVariables variables = new RasterVariables();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 70:
                        variables.DisplayFrame = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        break;

                    case 0x47:
                        variables.DisplayQuality = (ImageDisplayQuality) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x48:
                        variables.Units = (ImageUnits) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 5:
                        variables.Handle = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            return variables;
        }

        private Ray ReadRay()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 unitX = Vector3.UnitX;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Ray ray = new Ray {
                Origin = zero,
                Direction = unitX
            };
            ray.XData.AddRange(items);
            return ray;
        }

        private Solid ReadSolid()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector3 = Vector3.Zero;
            Vector3 vector4 = Vector3.Zero;
            double num = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        vector4.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        vector4.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        vector3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x21:
                    {
                        vector4.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            Solid solid = new Solid {
                FirstVertex = new Vector2(zero.X, zero.Y),
                SecondVertex = new Vector2(vector2.X, vector2.Y),
                ThirdVertex = new Vector2(vector3.X, vector3.Y),
                FourthVertex = new Vector2(vector4.X, vector4.Y),
                Elevation = zero.Z,
                Thickness = num,
                Normal = unitZ
            };
            solid.XData.AddRange(items);
            return solid;
        }

        private netDxf.Entities.Spline ReadSpline()
        {
            SplinetypeFlags none = SplinetypeFlags.None;
            Vector3 unitZ = Vector3.UnitZ;
            short degree = 3;
            int num2 = -1;
            List<double> knots = new List<double>();
            List<SplineVertex> controlPoints = new List<SplineVertex>();
            double x = 0.0;
            double y = 0.0;
            double w = -1.0;
            double num7 = 1E-07;
            double num8 = 1E-07;
            double num9 = 1E-10;
            double num10 = 0.0;
            double num11 = 0.0;
            Vector3? nullable = null;
            double num13 = 0.0;
            double num14 = 0.0;
            Vector3? nullable2 = null;
            List<Vector3> fitPoints = new List<Vector3>();
            double num16 = 0.0;
            double num17 = 0.0;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                double num5;
                switch (this.chunk.Code)
                {
                    case 10:
                    {
                        x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        num16 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        num10 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        num13 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        num17 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        num11 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        num14 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                        num5 = this.chunk.ReadDouble();
                        if (w > 0.0)
                        {
                            break;
                        }
                        controlPoints.Add(new SplineVertex(x, y, num5));
                        num2 = controlPoints.Count - 1;
                        goto Label_04FD;

                    case 0x1f:
                    {
                        double z = this.chunk.ReadDouble();
                        fitPoints.Add(new Vector3(num16, num17, z));
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        double z = this.chunk.ReadDouble();
                        nullable = new Vector3(num10, num11, z);
                        this.chunk.Next();
                        continue;
                    }
                    case 0x21:
                    {
                        double z = this.chunk.ReadDouble();
                        nullable2 = new Vector3(num13, num14, z);
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        knots.Add(this.chunk.ReadDouble());
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        double num20 = this.chunk.ReadDouble();
                        if (num20 <= 0.0)
                        {
                            num20 = 1.0;
                        }
                        if (num2 == -1)
                        {
                            w = num20;
                        }
                        else
                        {
                            controlPoints[num2].Weigth = num20;
                            w = -1.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2a:
                    {
                        num7 = this.chunk.ReadDouble();
                        if (num7 <= 0.0)
                        {
                            num7 = 1E-07;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2b:
                    {
                        num8 = this.chunk.ReadDouble();
                        if (num8 <= 0.0)
                        {
                            num8 = 1E-07;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2c:
                    {
                        num9 = this.chunk.ReadDouble();
                        if (num9 <= 0.0)
                        {
                            num9 = 1E-10;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        none = (SplinetypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        degree = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 0x49:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4a:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    default:
                        goto Label_0615;
                }
                controlPoints.Add(new SplineVertex(x, y, num5, w));
                num2 = -1;
            Label_04FD:
                this.chunk.Next();
                continue;
            Label_0615:
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            SplineCreationMethod method = none.HasFlag(SplinetypeFlags.FitPointCreationMethod) ? SplineCreationMethod.FitPoints : SplineCreationMethod.ControlPoints;
            bool isPeriodic = none.HasFlag(SplinetypeFlags.ClosedPeriodicSpline) || none.HasFlag(SplinetypeFlags.Periodic);
            netDxf.Entities.Spline spline = new netDxf.Entities.Spline(controlPoints, knots, degree, fitPoints, method, isPeriodic) {
                KnotTolerance = num7,
                CtrlPointTolerance = num8,
                FitTolerance = num9,
                StartTangent = nullable,
                EndTangent = nullable2
            };
            if (none.HasFlag(SplinetypeFlags.FitChord))
            {
                spline.KnotParameterization = SplineKnotParameterization.FitChord;
            }
            else if (none.HasFlag(SplinetypeFlags.FitSqrtChord))
            {
                spline.KnotParameterization = SplineKnotParameterization.FitSqrtChord;
            }
            else if (none.HasFlag(SplinetypeFlags.FitUniform))
            {
                spline.KnotParameterization = SplineKnotParameterization.FitUniform;
            }
            else if (none.HasFlag(SplinetypeFlags.FitCustom))
            {
                spline.KnotParameterization = SplineKnotParameterization.FitCustom;
            }
            spline.XData.AddRange(items);
            return spline;
        }

        private void ReadTable()
        {
            Debug.Assert(this.chunk.ReadString() == "TABLE");
            string handle = null;
            this.chunk.Next();
            string name = this.chunk.ReadString();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                        handle = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 100:
                        Debug.Assert(this.chunk.ReadString() == "AcDbSymbolTable");
                        this.CreateTableCollection(name, handle);
                        break;

                    case 0x66:
                        this.ReadExtensionDictionaryGroup();
                        this.chunk.Next();
                        break;

                    case 330:
                        Debug.Assert(this.chunk.ReadHex() == "0");
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            while (this.chunk.ReadString() != "ENDTAB")
            {
                this.ReadTableEntry();
            }
            this.chunk.Next();
        }

        private void ReadTableEntry()
        {
            string str = this.chunk.ReadString();
            VPort viewport = null;
            string str2 = null;
            while (this.chunk.ReadString() != "ENDTAB")
            {
                ApplicationRegistry registry;
                BlockRecord record;
                DimensionStyle style;
                Layer layer;
                Linetype linetype;
                TextStyle style2;
                UCS ucs;
                VPort port2;
                while (this.chunk.Code != 100)
                {
                    switch (this.chunk.Code)
                    {
                        case 0x69:
                        {
                            str2 = this.chunk.ReadHex();
                            this.chunk.Next();
                            continue;
                        }
                        case 330:
                        {
                            this.chunk.Next();
                            continue;
                        }
                        case 5:
                        {
                            str2 = this.chunk.ReadHex();
                            this.chunk.Next();
                            continue;
                        }
                        case 0x66:
                        {
                            this.ReadExtensionDictionaryGroup();
                            this.chunk.Next();
                            continue;
                        }
                    }
                    this.chunk.Next();
                }
                this.chunk.Next();
                string s = str;
                switch (<PrivateImplementationDetails>.ComputeStringHash(s))
                {
                    case 0x4ba33ea:
                        if (s == "DIMSTYLE")
                        {
                            goto Label_02B3;
                        }
                        break;

                    case 0x19968db8:
                        if (s == "VIEW")
                        {
                            goto Label_03C6;
                        }
                        break;

                    case 0x4087e5ee:
                        if (s == "BLOCK_RECORD")
                        {
                            goto Label_027B;
                        }
                        break;

                    case 0x5676b876:
                        if (s == "STYLE")
                        {
                            goto Label_0358;
                        }
                        break;

                    case 0x71ea82d6:
                        if (s == "LAYER")
                        {
                            goto Label_02EA;
                        }
                        break;

                    case 0x7ef287ba:
                        if (s == "VPORT")
                        {
                            goto Label_03D2;
                        }
                        break;

                    case 0x8894622e:
                        if (s == "UCS")
                        {
                            goto Label_038F;
                        }
                        break;

                    case 0x971b0241:
                        if (s == "APPID")
                        {
                            goto Label_0244;
                        }
                        break;

                    case 0xac3c19f3:
                        if (s == "LTYPE")
                        {
                            goto Label_0321;
                        }
                        break;
                }
                goto Label_0492;
            Label_0244:
                registry = this.ReadApplicationId();
                if (registry > null)
                {
                    registry.Handle = str2;
                    this.doc.ApplicationRegistries.Add(registry, false);
                }
                continue;
            Label_027B:
                record = this.ReadBlockRecord();
                if (record > null)
                {
                    record.Handle = str2;
                    this.blockRecords.Add(record.Name, record);
                }
                continue;
            Label_02B3:
                style = this.ReadDimensionStyle();
                if (style > null)
                {
                    style.Handle = str2;
                    this.doc.DimensionStyles.Add(style, false);
                }
                continue;
            Label_02EA:
                layer = this.ReadLayer();
                if (layer > null)
                {
                    layer.Handle = str2;
                    this.doc.Layers.Add(layer, false);
                }
                continue;
            Label_0321:
                linetype = this.ReadLinetype();
                if (linetype > null)
                {
                    linetype.Handle = str2;
                    this.doc.Linetypes.Add(linetype, false);
                }
                continue;
            Label_0358:
                style2 = this.ReadTextStyle();
                if (style2 > null)
                {
                    style2.Handle = str2;
                    this.doc.TextStyles.Add(style2, false);
                }
                continue;
            Label_038F:
                ucs = this.ReadUCS();
                if (ucs > null)
                {
                    ucs.Handle = str2;
                    this.doc.UCSs.Add(ucs, false);
                }
                continue;
            Label_03C6:
                this.ReadView();
                continue;
            Label_03D2:
                port2 = this.ReadVPort();
                if ((port2 != null) && (viewport == null))
                {
                    viewport = this.doc.Viewport;
                    viewport.Handle = str2;
                    viewport.ViewCenter = port2.ViewCenter;
                    viewport.SnapBasePoint = port2.SnapBasePoint;
                    viewport.SnapSpacing = port2.SnapSpacing;
                    viewport.GridSpacing = port2.GridSpacing;
                    viewport.ViewDirection = port2.ViewDirection;
                    viewport.ViewTarget = port2.ViewTarget;
                    viewport.ViewHeight = port2.ViewHeight;
                    viewport.ViewAspectRatio = port2.ViewAspectRatio;
                    viewport.ShowGrid = port2.ShowGrid;
                    viewport.SnapMode = port2.SnapMode;
                }
                continue;
            Label_0492:
                this.ReadUnkownTableEntry();
                break;
            }
        }

        private void ReadTables()
        {
            Debug.Assert(this.chunk.ReadString() == "TABLES");
            this.chunk.Next();
            while (this.chunk.ReadString() != "ENDSEC")
            {
                this.ReadTable();
            }
            if (this.doc.ApplicationRegistries == null)
            {
                this.doc.ApplicationRegistries = new ApplicationRegistries(this.doc, null);
            }
            if (this.doc.Blocks == null)
            {
                this.doc.Blocks = new BlockRecords(this.doc, null);
            }
            if (this.doc.DimensionStyles == null)
            {
                this.doc.DimensionStyles = new DimensionStyles(this.doc, null);
            }
            if (this.doc.Layers == null)
            {
                this.doc.Layers = new Layers(this.doc, null);
            }
            if (this.doc.Linetypes == null)
            {
                this.doc.Linetypes = new Linetypes(this.doc, null);
            }
            if (this.doc.TextStyles == null)
            {
                this.doc.TextStyles = new TextStyles(this.doc, null);
            }
            if (this.doc.UCSs == null)
            {
                this.doc.UCSs = new UCSs(this.doc, null);
            }
            if (this.doc.Views == null)
            {
                this.doc.Views = new Views(this.doc, null);
            }
            if (this.doc.VPorts == null)
            {
                this.doc.VPorts = new VPorts(this.doc, null);
            }
        }

        private Text ReadText()
        {
            string str = string.Empty;
            double textSize = 0.0;
            double textSize = 1.0;
            double num3 = 0.0;
            double num4 = 0.0;
            TextStyle textStyle = TextStyle.Default;
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            short horizontal = 0;
            short vertical = 0;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 7:
                    {
                        string textStyle = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(textStyle))
                        {
                            textStyle = this.doc.DrawingVariables.TextStyle;
                        }
                        textStyle = this.GetTextStyle(textStyle);
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 1:
                    {
                        str = this.chunk.ReadString();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        textSize = this.chunk.ReadDouble();
                        if (textSize <= 0.0)
                        {
                            textSize = this.doc.DrawingVariables.TextSize;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        textSize = this.chunk.ReadDouble();
                        if ((textSize < 0.01) || (textSize > 100.0))
                        {
                            textSize = this.doc.DrawingVariables.TextSize;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        horizontal = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x49:
                    {
                        vertical = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 50:
                    {
                        num3 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x33:
                    {
                        num4 = this.chunk.ReadDouble();
                        if ((num4 < -85.0) || (num4 > 85.0))
                        {
                            num4 = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            TextAlignment alignment = ObtainAlignment(horizontal, vertical);
            Vector3 point = (alignment == TextAlignment.BaselineLeft) ? zero : vector2;
            str = this.DecodeEncodedNonAsciiCharacters(str);
            Text text = new Text {
                Value = str,
                Height = textSize,
                WidthFactor = textSize,
                Rotation = num3,
                ObliqueAngle = num4,
                Style = textStyle,
                Position = MathHelper.Transform(point, unitZ, CoordinateSystem.Object, CoordinateSystem.World),
                Normal = unitZ,
                Alignment = alignment
            };
            text.XData.AddRange(items);
            return text;
        }

        private TextStyle ReadTextStyle()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbTextStyleTableRecord");
            string str = null;
            string str2 = null;
            string str3 = null;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 50:
                    {
                        num3 = this.chunk.ReadDouble();
                        if ((num3 < -85.0) || (num3 > 85.0))
                        {
                            num3 = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        if (this.chunk.ReadShort() == 4)
                        {
                            flag = true;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        switch (this.chunk.ReadShort())
                        {
                            case 6:
                                flag2 = true;
                                flag3 = true;
                                break;

                            case 2:
                                flag2 = true;
                                break;

                            case 4:
                                flag3 = true;
                                break;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 3:
                    {
                        str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 4:
                    {
                        str3 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        num = this.chunk.ReadDouble();
                        if (num < 0.0)
                        {
                            num = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        num2 = this.chunk.ReadDouble();
                        if ((num2 < 0.01) || (num2 > 100.0))
                        {
                            num2 = 1.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2a:
                    {
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            if (string.IsNullOrEmpty(str) || !TableObject.IsValidName(str))
            {
                return null;
            }
            if (string.IsNullOrEmpty(str2))
            {
                return null;
            }
            TextStyle style = new TextStyle(str, str2, false) {
                Height = num,
                IsBackward = flag2,
                IsUpsideDown = flag3,
                IsVertical = flag,
                ObliqueAngle = num3,
                WidthFactor = num2
            };
            if (!string.IsNullOrEmpty(str3) && (Path.GetExtension(str2).Equals(".shx", StringComparison.OrdinalIgnoreCase) && Path.GetExtension(str3).Equals(".shx", StringComparison.OrdinalIgnoreCase)))
            {
                style.BigFont = str3;
            }
            return style;
        }

        private void ReadThumbnailImage()
        {
            Debug.Assert(this.chunk.ReadString() == "THUMBNAILIMAGE");
            while (this.chunk.ReadString() != "ENDSEC")
            {
                do
                {
                    this.chunk.Next();
                }
                while (this.chunk.Code > 0);
            }
        }

        private Tolerance ReadTolerance()
        {
            DimensionStyle dimensionStyle = DimensionStyle.Default;
            Vector3 zero = Vector3.Zero;
            string s = string.Empty;
            Vector3 unitZ = Vector3.UnitZ;
            Vector3 unitX = Vector3.UnitX;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 11:
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 1:
                        s = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 3:
                    {
                        string dimStyle = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        if (string.IsNullOrEmpty(dimStyle))
                        {
                            dimStyle = this.doc.DrawingVariables.DimStyle;
                        }
                        dimensionStyle = this.GetDimensionStyle(dimStyle);
                        this.chunk.Next();
                        break;
                    }
                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            unitX = MathHelper.Transform(unitX, unitZ, CoordinateSystem.World, CoordinateSystem.Object);
            double num = Vector2.Angle(new Vector2(unitX.X, unitX.Y));
            Tolerance tolerance = Tolerance.ParseRepresentation(s);
            tolerance.Style = dimensionStyle;
            tolerance.Position = zero;
            tolerance.Rotation = num * 57.295779513082323;
            tolerance.Normal = unitZ;
            tolerance.XData.AddRange(items);
            return tolerance;
        }

        private netDxf.Entities.Trace ReadTrace()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            Vector3 vector3 = Vector3.Zero;
            Vector3 vector4 = Vector3.Zero;
            double num = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        vector4.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        vector4.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        vector3.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x21:
                    {
                        vector4.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 230:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                    case 210:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 220:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            netDxf.Entities.Trace trace = new netDxf.Entities.Trace {
                FirstVertex = new Vector2(zero.X, zero.Y),
                SecondVertex = new Vector2(vector2.X, vector2.Y),
                ThirdVertex = new Vector2(vector3.X, vector3.Y),
                FourthVertex = new Vector2(vector4.X, vector4.Y),
                Elevation = zero.Z,
                Thickness = num,
                Normal = unitZ
            };
            trace.XData.AddRange(items);
            return trace;
        }

        private UCS ReadUCS()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbUCSTableRecord");
            string str = null;
            Vector3 zero = Vector3.Zero;
            Vector3 unitX = Vector3.UnitX;
            Vector3 unitY = Vector3.UnitY;
            double num = 0.0;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 12:
                        unitY.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 2:
                        str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x16:
                        unitY.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x20:
                        unitY.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x92:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    default:
                        this.chunk.Next();
                        break;
                }
            }
            if (string.IsNullOrEmpty(str) || !TableObject.IsValidName(str))
            {
                return null;
            }
            return new UCS(str, zero, unitX, unitY, false) { Elevation = num };
        }

        private Underlay ReadUnderlay()
        {
            string str = null;
            ClippingBoundary boundary;
            Vector3 zero = Vector3.Zero;
            Vector3 vector2 = new Vector3(1.0);
            double num = 0.0;
            Vector3 unitZ = Vector3.UnitZ;
            UnderlayDisplayFlags showUnderlay = UnderlayDisplayFlags.ShowUnderlay;
            short num2 = 100;
            short num3 = 0;
            Vector2 item = Vector2.Zero;
            List<Vector2> vertexes = new List<Vector2>();
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        item = new Vector2 {
                            X = this.chunk.ReadDouble()
                        };
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        item.Y = this.chunk.ReadDouble();
                        vertexes.Add(item);
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x29:
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x2a:
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x2b:
                        vector2.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 50:
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 210:
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 220:
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 340:
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData data = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(data);
                        break;
                    }
                    case 280:
                        showUnderlay = (UnderlayDisplayFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x119:
                        num2 = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 0x11a:
                        num3 = this.chunk.ReadShort();
                        this.chunk.Next();
                        break;

                    case 230:
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            Vector3 vector5 = MathHelper.Transform(zero, unitZ, CoordinateSystem.Object, CoordinateSystem.World);
            if (vertexes.Count < 2)
            {
                boundary = null;
            }
            else if (vertexes.Count == 2)
            {
                boundary = new ClippingBoundary(vertexes[0], vertexes[1]);
            }
            else
            {
                boundary = new ClippingBoundary(vertexes);
            }
            Underlay key = new Underlay {
                Position = vector5,
                Scale = vector2,
                Normal = unitZ,
                Rotation = num,
                DisplayOptions = showUnderlay,
                Contrast = num2,
                Fade = num3,
                ClippingBoundary = boundary
            };
            key.XData.AddRange(items);
            if (string.IsNullOrEmpty(str) || (str == "0"))
            {
                return null;
            }
            this.underlayToDefinitionHandles.Add(key, str);
            return key;
        }

        private UnderlayDefinition ReadUnderlayDefinition(UnderlayType type)
        {
            string str = null;
            string str2 = string.Empty;
            string str3 = null;
            string fileName = null;
            string name = null;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 1:
                    {
                        fileName = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        str2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                    case 5:
                    {
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 330:
                    {
                        str3 = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            if (str3 > null)
            {
                DictionaryObject obj2 = this.dictionaries[str3];
                if (str == null)
                {
                    throw new NullReferenceException("Null handle in underlay definition dictionary.");
                }
                name = obj2.Entries[str];
            }
            UnderlayDefinition definition = null;
            switch (type)
            {
                case UnderlayType.DGN:
                {
                    UnderlayDgnDefinition definition1 = new UnderlayDgnDefinition(fileName, name) {
                        Handle = str,
                        Layout = str2
                    };
                    definition = definition1;
                    break;
                }
                case UnderlayType.DWF:
                {
                    UnderlayDwfDefinition definition3 = new UnderlayDwfDefinition(fileName, name) {
                        Handle = str
                    };
                    definition = definition3;
                    break;
                }
                case UnderlayType.PDF:
                {
                    UnderlayPdfDefinition definition4 = new UnderlayPdfDefinition(fileName, name) {
                        Handle = str,
                        Page = str2
                    };
                    definition = definition4;
                    break;
                }
            }
            if (definition == null)
            {
                throw new NullReferenceException("Underlay reference definition.");
            }
            this.underlayDefHandles.Add(definition.Handle, definition);
            return definition;
        }

        private void ReadUnknowEntity()
        {
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                this.chunk.Next();
            }
        }

        private void ReadUnkownTableEntry()
        {
            do
            {
                this.chunk.Next();
            }
            while (this.chunk.Code > 0);
        }

        private Vertex ReadVertex()
        {
            string str = string.Empty;
            Layer layer = Layer.Default;
            AciColor byLayer = AciColor.ByLayer;
            Linetype linetype = Linetype.ByLayer;
            Vector3 vector = new Vector3();
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            List<short> list = new List<short>();
            VertexTypeFlags polylineVertex = VertexTypeFlags.PolylineVertex;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 5:
                    {
                        str = this.chunk.ReadHex();
                        this.chunk.Next();
                        continue;
                    }
                    case 6:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        linetype = this.GetLinetype(name);
                        this.chunk.Next();
                        continue;
                    }
                    case 8:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        layer = this.GetLayer(name);
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        vector.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        vector.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        vector.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 70:
                    {
                        polylineVertex = (VertexTypeFlags) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        list.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        list.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 0x49:
                    {
                        list.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4a:
                    {
                        list.Add(this.chunk.ReadShort());
                        this.chunk.Next();
                        continue;
                    }
                    case 420:
                    {
                        byLayer = AciColor.FromTrueColor(this.chunk.ReadInt());
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        num = this.chunk.ReadDouble();
                        if (num < 0.0)
                        {
                            num = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        num2 = this.chunk.ReadDouble();
                        if (num2 < 0.0)
                        {
                            num2 = 0.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2a:
                    {
                        num3 = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e:
                    {
                        if (!byLayer.UseTrueColor)
                        {
                            byLayer = AciColor.FromCadIndex(this.chunk.ReadShort());
                        }
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            return new Vertex { 
                Flags = polylineVertex,
                Position = vector,
                StartWidth = num,
                Bulge = num3,
                Color = byLayer,
                EndWidth = num2,
                Layer = layer,
                Linetype = linetype,
                VertexIndexes = list.ToArray(),
                Handle = str
            };
        }

        private View ReadView()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbViewTableRecord");
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                this.chunk.Next();
            }
            return null;
        }

        private Viewport ReadViewport()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbViewport");
            Viewport key = new Viewport();
            Vector3 center = key.Center;
            Vector2 viewCenter = key.ViewCenter;
            Vector2 snapBase = key.SnapBase;
            Vector2 snapSpacing = key.SnapSpacing;
            Vector2 gridSpacing = key.GridSpacing;
            Vector3 viewDirection = key.ViewDirection;
            Vector3 viewTarget = key.ViewTarget;
            Vector3 zero = Vector3.Zero;
            Vector3 unitX = Vector3.UnitX;
            Vector3 unitY = Vector3.UnitY;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 110:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x6f:
                    {
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x70:
                    {
                        unitY.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 90:
                    {
                        key.Status = (ViewportStatusFlags) this.chunk.ReadInt();
                        this.chunk.Next();
                        continue;
                    }
                    case 10:
                    {
                        center.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        viewCenter.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        snapBase.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 14:
                    {
                        snapSpacing.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 15:
                    {
                        gridSpacing.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x10:
                    {
                        viewDirection.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x11:
                    {
                        viewTarget.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        center.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        viewCenter.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        snapBase.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x18:
                    {
                        snapSpacing.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x19:
                    {
                        gridSpacing.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1a:
                    {
                        viewDirection.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1b:
                    {
                        viewTarget.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        center.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x24:
                    {
                        viewDirection.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x25:
                    {
                        viewTarget.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        key.Width = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        key.Height = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2a:
                    {
                        key.LensLength = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2b:
                    {
                        key.FrontClipPlane = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2c:
                    {
                        key.BackClipPlane = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x2d:
                    {
                        key.ViewHeight = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 50:
                    {
                        key.SnapAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x33:
                    {
                        key.TwistAngle = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x44:
                    {
                        key.Stacking = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x45:
                    {
                        key.Id = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x48:
                    {
                        key.CircleZoomPercent = this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 120:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x79:
                    {
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x7a:
                    {
                        unitY.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 130:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x83:
                    {
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x84:
                    {
                        unitY.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x14b:
                    {
                        Layer objectByHandle = (Layer) this.doc.GetObjectByHandle(this.chunk.ReadString());
                        key.FrozenLayers.Add(objectByHandle);
                        this.chunk.Next();
                        continue;
                    }
                    case 340:
                    {
                        this.viewports.Add(key, this.chunk.ReadHex());
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            key.Center = center;
            key.ViewCenter = viewCenter;
            key.SnapBase = snapBase;
            key.SnapSpacing = snapSpacing;
            key.GridSpacing = gridSpacing;
            key.ViewDirection = viewDirection;
            key.ViewTarget = viewTarget;
            key.UcsOrigin = zero;
            key.UcsXAxis = unitX;
            key.UcsYAxis = unitY;
            key.XData.AddRange(items);
            return key;
        }

        private VPort ReadVPort()
        {
            Debug.Assert(this.chunk.ReadString() == "AcDbViewportTableRecord");
            string str = null;
            Vector2 zero = Vector2.Zero;
            Vector2 vector2 = Vector2.Zero;
            Vector2 vector3 = new Vector2(0.5);
            Vector2 vector4 = new Vector2(10.0);
            Vector3 vector5 = Vector3.Zero;
            Vector3 unitZ = Vector3.UnitZ;
            double num = 10.0;
            double num2 = 1.0;
            bool flag = true;
            bool flag2 = false;
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 0x4b:
                    {
                        flag2 = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 0x4c:
                    {
                        flag = this.chunk.ReadShort() > 0;
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 13:
                    {
                        vector2.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 14:
                    {
                        vector3.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 15:
                    {
                        vector4.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x10:
                    {
                        unitZ.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x11:
                    {
                        vector5.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x17:
                    {
                        vector2.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x18:
                    {
                        vector3.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x19:
                    {
                        vector4.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1a:
                    {
                        unitZ.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1b:
                    {
                        vector5.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x24:
                    {
                        unitZ.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x25:
                    {
                        vector5.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 40:
                    {
                        num = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x29:
                    {
                        num2 = this.chunk.ReadDouble();
                        if (num2 <= 0.0)
                        {
                            num2 = 1.0;
                        }
                        this.chunk.Next();
                        continue;
                    }
                    case 2:
                    {
                        str = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        this.chunk.Next();
                        continue;
                    }
                }
                this.chunk.Next();
            }
            if (string.IsNullOrEmpty(str) || !TableObject.IsValidName(str))
            {
                return null;
            }
            if (!str.Equals("*Active", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            return new VPort(str, false) { 
                ViewCenter = zero,
                SnapBasePoint = vector2,
                SnapSpacing = vector3,
                GridSpacing = vector4,
                ViewTarget = vector5,
                ViewDirection = unitZ,
                ViewHeight = num,
                ViewAspectRatio = num2,
                ShowGrid = flag,
                SnapMode = flag2
            };
        }

        private Wipeout ReadWipeout()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 unitX = Vector3.UnitX;
            Vector3 unitY = Vector3.UnitY;
            ClippingBoundaryType rectangular = ClippingBoundaryType.Rectangular;
            double x = 0.0;
            List<Vector2> vertexes = new List<Vector2>();
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                    {
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 11:
                    {
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 12:
                    {
                        unitY.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 14:
                    {
                        x = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 20:
                    {
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x15:
                    {
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x16:
                    {
                        unitY.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x18:
                    {
                        vertexes.Add(new Vector2(x, this.chunk.ReadDouble()));
                        this.chunk.Next();
                        continue;
                    }
                    case 30:
                    {
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x1f:
                    {
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x20:
                    {
                        unitY.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x47:
                    {
                        rectangular = (ClippingBoundaryType) this.chunk.ReadShort();
                        this.chunk.Next();
                        continue;
                    }
                    case 0x5b:
                    {
                        this.chunk.Next();
                        continue;
                    }
                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        continue;
                    }
                }
                if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                {
                    throw new Exception("The extended data of an entity must start with the application registry code.");
                }
                this.chunk.Next();
            }
            if (rectangular == ClippingBoundaryType.Polygonal)
            {
                vertexes.RemoveAt(vertexes.Count - 1);
            }
            Vector3 zAxis = Vector3.Normalize(Vector3.CrossProduct(unitX, unitY));
            List<Vector3> points = new List<Vector3> {
                zero,
                unitX,
                unitY
            };
            IList<Vector3> list3 = MathHelper.Transform(points, zAxis, CoordinateSystem.World, CoordinateSystem.Object);
            Vector3 vector5 = list3[0];
            double num2 = vector5.X;
            vector5 = list3[0];
            double y = vector5.Y;
            vector5 = list3[0];
            double z = vector5.Z;
            vector5 = list3[1];
            double num5 = vector5.X;
            for (int i = 0; i < vertexes.Count; i++)
            {
                Vector2 vector6 = vertexes[i];
                double num8 = num2 + (num5 * (vector6.X + 0.5));
                vector6 = vertexes[i];
                double num9 = y + (num5 * (0.5 - vector6.Y));
                vertexes[i] = new Vector2(num8, num9);
            }
            ClippingBoundary clippingBoundary = (rectangular == ClippingBoundaryType.Rectangular) ? new ClippingBoundary(vertexes[0], vertexes[1]) : new ClippingBoundary(vertexes);
            Wipeout wipeout = new Wipeout(clippingBoundary) {
                Normal = zAxis,
                Elevation = z
            };
            wipeout.XData.AddRange(items);
            return wipeout;
        }

        private XData ReadXDataRecord(ApplicationRegistry appReg)
        {
            XData data = new XData(appReg);
            this.chunk.Next();
            while ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
            {
                if (this.chunk.Code == 0x3e9)
                {
                    return data;
                }
                XDataCode code = (XDataCode) this.chunk.Code;
                object obj2 = null;
                switch (code)
                {
                    case XDataCode.String:
                        obj2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;

                    case XDataCode.ControlString:
                        obj2 = this.chunk.ReadString();
                        break;

                    case XDataCode.LayerName:
                        obj2 = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        break;

                    case XDataCode.BinaryData:
                        obj2 = this.chunk.ReadBytes();
                        break;

                    case XDataCode.DatabaseHandle:
                        obj2 = this.chunk.ReadString();
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
                        obj2 = this.chunk.ReadDouble();
                        break;

                    case XDataCode.Int16:
                        obj2 = this.chunk.ReadShort();
                        break;

                    case XDataCode.Int32:
                        obj2 = this.chunk.ReadInt();
                        break;
                }
                XDataRecord item = new XDataRecord(code, obj2);
                data.XDataRecord.Add(item);
                this.chunk.Next();
            }
            return data;
        }

        private XLine ReadXLine()
        {
            Vector3 zero = Vector3.Zero;
            Vector3 unitX = Vector3.UnitX;
            List<XData> items = new List<XData>();
            this.chunk.Next();
            while (this.chunk.Code > 0)
            {
                switch (this.chunk.Code)
                {
                    case 10:
                        zero.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 11:
                        unitX.X = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 20:
                        zero.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x15:
                        unitX.Y = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 30:
                        zero.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x1f:
                        unitX.Z = this.chunk.ReadDouble();
                        this.chunk.Next();
                        break;

                    case 0x3e9:
                    {
                        string name = this.DecodeEncodedNonAsciiCharacters(this.chunk.ReadString());
                        XData item = this.ReadXDataRecord(this.GetApplicationRegistry(name));
                        items.Add(item);
                        break;
                    }
                    default:
                        if ((this.chunk.Code >= 0x3e8) && (this.chunk.Code <= 0x42f))
                        {
                            throw new Exception("The extended data of an entity must start with the application registry code.");
                        }
                        this.chunk.Next();
                        break;
                }
            }
            XLine line = new XLine {
                Origin = zero,
                Direction = unitX
            };
            line.XData.AddRange(items);
            return line;
        }

        private void RelinkOrphanLayouts()
        {
            foreach (BlockRecord record in this.blockRecordPointerToLayout.Values)
            {
                Layout item = null;
                if (string.Equals(record.Name, "*Model_Space", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Layout layout2 in this.orphanLayouts)
                    {
                        if (string.Equals(layout2.Name, "Model", StringComparison.OrdinalIgnoreCase))
                        {
                            item = layout2;
                            break;
                        }
                    }
                    if (item == null)
                    {
                        item = Layout.ModelSpace;
                        this.doc.Layouts.Add(item);
                    }
                    else
                    {
                        item.AssociatedBlock = this.doc.Blocks[record.Name];
                        this.orphanLayouts.Remove(item);
                        this.doc.Layouts.Add(item);
                    }
                    continue;
                }
                if (this.orphanLayouts.Count > 0)
                {
                    foreach (Layout layout3 in this.orphanLayouts)
                    {
                        if (!string.Equals(layout3.Name, "Model", StringComparison.OrdinalIgnoreCase))
                        {
                            item = layout3;
                            break;
                        }
                    }
                    if (item > null)
                    {
                        item = this.orphanLayouts[0];
                        item.AssociatedBlock = this.doc.Blocks[record.Name];
                        this.doc.Layouts.Add(item);
                        this.orphanLayouts.Remove(item);
                        continue;
                    }
                }
                short num = 1;
                string name = "Layout" + 1;
                while (this.doc.Layouts.Contains(name))
                {
                    num = (short) (num + 1);
                    name = "Layout" + num;
                }
                item = new Layout(name) {
                    TabOrder = (short) (this.doc.Layouts.Count + 1),
                    AssociatedBlock = this.doc.Blocks[record.Name]
                };
                this.doc.Layouts.Add(item);
            }
            foreach (Layout layout4 in this.orphanLayouts)
            {
                this.doc.Layouts.Add(layout4, false);
            }
            this.doc.Layouts.Add(Layout.ModelSpace);
        }

        private static void SetEllipseParameters(netDxf.Entities.Ellipse ellipse, double[] param)
        {
            if (MathHelper.IsZero(param[0]) && MathHelper.IsEqual(param[1], 6.2831853071795862))
            {
                ellipse.StartAngle = 0.0;
                ellipse.EndAngle = 0.0;
            }
            else
            {
                double num = ellipse.MajorAxis * 0.5;
                double num2 = ellipse.MinorAxis * 0.5;
                Vector2 objA = new Vector2(num * Math.Cos(param[0]), num2 * Math.Sin(param[0]));
                Vector2 objB = new Vector2(num * Math.Cos(param[1]), num2 * Math.Sin(param[1]));
                if (object.Equals(objA, objB))
                {
                    ellipse.StartAngle = 0.0;
                    ellipse.EndAngle = 0.0;
                }
                else
                {
                    ellipse.StartAngle = Vector2.Angle(objA) * 57.295779513082323;
                    ellipse.EndAngle = Vector2.Angle(objB) * 57.295779513082323;
                }
            }
        }
    }
}

