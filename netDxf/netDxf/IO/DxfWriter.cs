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
    using System.IO;
    using System.Text;
    using System.Windows;

    internal sealed class DxfWriter
    {
        private bool isBinary;
        private string activeSection = "";
        private string activeTable = "";
        private ICodeValueWriter chunk;
        private DxfDocument doc;
        private Dictionary<string, string> encodedStrings;

        private static void AddBlockRecordUnitsXData(BlockRecord record)
        {
            XData data;
            if (record.XData.ContainsAppId("ACAD"))
            {
                data = record.XData["ACAD"];
                data.XDataRecord.Clear();
            }
            else
            {
                data = new XData(new ApplicationRegistry("ACAD"));
                record.XData.Add(data);
            }
            data.XDataRecord.Add(new XDataRecord(XDataCode.String, "DesignCenter Data"));
            data.XDataRecord.Add(XDataRecord.OpenControlString);
            data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 1));
            data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) record.Units));
            data.XDataRecord.Add(XDataRecord.CloseControlString);
        }

        private void AddDimensionStyleOverridesXData(XDataDictionary xdata, DimensionStyleOverrideDictionary overrides, EntityObject entity)
        {
            XData data;
            if (xdata.ContainsAppId("ACAD"))
            {
                data = xdata["ACAD"];
                data.XDataRecord.Clear();
            }
            else
            {
                data = new XData(new ApplicationRegistry("ACAD"));
                xdata.Add(data);
            }
            data.XDataRecord.Add(new XDataRecord(XDataCode.String, "DSTYLE"));
            data.XDataRecord.Add(XDataRecord.OpenControlString);
            bool flag = false;
            string str = string.Empty;
            string str2 = string.Empty;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            bool flag9 = true;
            bool flag10 = true;
            foreach (DimensionStyleOverride @override in overrides.Values)
            {
                switch (@override.Type)
                {
                    case DimensionStyleOverrideType.DimLineColor:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0xb0));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, ((AciColor) @override.Value).Index));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimLineLinetype:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x159));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.DatabaseHandle, ((Linetype) @override.Value).Handle));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimLineLineweight:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x173));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) ((Lineweight) @override.Value)));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimLineOff:
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x119));
                        if (!((bool) @override.Value))
                        {
                            break;
                        }
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 1));
                        goto Label_02C1;

                    case DimensionStyleOverrideType.DimLineExtend:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x2e));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLineColor:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0xb1));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, ((AciColor) @override.Value).Index));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLine1Linetype:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x15a));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.DatabaseHandle, ((Linetype) @override.Value).Handle));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLine2Linetype:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x15b));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.DatabaseHandle, ((Linetype) @override.Value).Handle));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLineLineweight:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x174));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) ((Lineweight) @override.Value)));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLine1Off:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x4b));
                        if (!((bool) @override.Value))
                        {
                            goto Label_04FE;
                        }
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 1));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLine2Off:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x4c));
                        if (!((bool) @override.Value))
                        {
                            goto Label_056C;
                        }
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 1));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLineOffset:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x2a));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.ExtLineExtend:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x2c));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.ArrowSize:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x29));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.CenterMarkSize:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x8d));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.LeaderArrow:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x155));
                        data.XDataRecord.Add((@override.Value != null) ? new XDataRecord(XDataCode.DatabaseHandle, ((Block) @override.Value).Record.Handle) : new XDataRecord(XDataCode.DatabaseHandle, "0"));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimArrow1:
                    {
                        flag2 = true;
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x157));
                        data.XDataRecord.Add((@override.Value != null) ? new XDataRecord(XDataCode.DatabaseHandle, ((Block) @override.Value).Record.Handle) : new XDataRecord(XDataCode.DatabaseHandle, "0"));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimArrow2:
                    {
                        flag2 = true;
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x158));
                        data.XDataRecord.Add((@override.Value != null) ? new XDataRecord(XDataCode.DatabaseHandle, ((Block) @override.Value).Record.Handle) : new XDataRecord(XDataCode.DatabaseHandle, "0"));
                        continue;
                    }
                    case DimensionStyleOverrideType.TextStyle:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 340));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.DatabaseHandle, ((TextStyle) @override.Value).Handle));
                        continue;
                    }
                    case DimensionStyleOverrideType.TextColor:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0xb2));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, ((AciColor) @override.Value).Index));
                        continue;
                    }
                    case DimensionStyleOverrideType.TextHeight:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 140));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.TextOffset:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x93));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimScaleOverall:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 40));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.AngularPrecision:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0xb3));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.LengthPrecision:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x10f));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimPrefix:
                    {
                        flag = true;
                        str = (string) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.DimSuffix:
                    {
                        flag = true;
                        str2 = (string) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.DecimalSeparator:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x116));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) ((char) @override.Value)));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimScaleLinear:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x90));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Real, (double) @override.Value));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimLengthUnits:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x115));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) ((LinearUnitType) @override.Value)));
                        continue;
                    }
                    case DimensionStyleOverrideType.DimAngularUnits:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x113));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) ((AngleUnitType) @override.Value)));
                        continue;
                    }
                    case DimensionStyleOverrideType.FractionalType:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x114));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) ((FractionFormatType) @override.Value)));
                        continue;
                    }
                    case DimensionStyleOverrideType.SuppressLinearLeadingZeros:
                    {
                        flag3 = true;
                        flag5 = (bool) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.SuppressLinearTrailingZeros:
                    {
                        flag3 = true;
                        flag6 = (bool) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.SuppressAngularLeadingZeros:
                    {
                        flag4 = true;
                        flag7 = (bool) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.SuppressAngularTrailingZeros:
                    {
                        flag4 = true;
                        flag8 = (bool) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.SuppressZeroFeet:
                    {
                        flag3 = true;
                        flag9 = (bool) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.SuppressZeroInches:
                    {
                        flag3 = true;
                        flag10 = (bool) @override.Value;
                        continue;
                    }
                    case DimensionStyleOverrideType.DimRoundoff:
                    {
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x2d));
                        data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) @override.Value));
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0));
            Label_02C1:
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x11a));
                if ((bool) @override.Value)
                {
                    data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 1));
                }
                else
                {
                    data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0));
                }
                continue;
            Label_04FE:
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0));
                continue;
            Label_056C:
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0));
            }
            if (flag2)
            {
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0xad));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 1));
            }
            if (flag)
            {
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 3));
                data.XDataRecord.Add(new XDataRecord(XDataCode.String, this.EncodeNonAsciiCharacters($"{str}<>{str2}")));
            }
            if (flag3)
            {
                short num = 0;
                if (flag9 & flag10)
                {
                    num = 0;
                }
                else if (!flag9 && !flag10)
                {
                    num = (short) (num + 1);
                }
                else if (!flag9 & flag10)
                {
                    num = (short) (num + 2);
                }
                else if (flag9 && !flag10)
                {
                    num = (short) (num + 3);
                }
                if (!flag5 && !flag6)
                {
                    num = num;
                }
                else if (flag5 && !flag6)
                {
                    num = (short) (num + 4);
                }
                else if (!flag5 & flag6)
                {
                    num = (short) (num + 8);
                }
                else if (flag5 & flag6)
                {
                    num = (short) (num + 12);
                }
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x4e));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, num));
            }
            if (flag4)
            {
                short num2 = 3;
                if (flag7 & flag8)
                {
                    num2 = 3;
                }
                else if (!flag7 && !flag8)
                {
                    num2 = 0;
                }
                else if (!flag7 & flag8)
                {
                    num2 = 2;
                }
                else if (flag7 && !flag8)
                {
                    num2 = 1;
                }
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x4f));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, num2));
            }
            Leader leader = entity as Leader;
            if (leader > null)
            {
                MText annotation = leader.Annotation as MText;
                if (annotation > null)
                {
                    data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 70));
                    data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) annotation.AttachmentPoint));
                    data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x4d));
                    data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) leader.TextVerticalPosition));
                }
            }
            data.XDataRecord.Add(XDataRecord.CloseControlString);
        }

        private static void AddHatchPatternXData(Hatch hatch)
        {
            XData data;
            if (hatch.XData.ContainsAppId("ACAD"))
            {
                data = hatch.XData["ACAD"];
                data.XDataRecord.Clear();
            }
            else
            {
                data = new XData(new ApplicationRegistry("ACAD"));
                hatch.XData.Add(data);
            }
            data.XDataRecord.Add(new XDataRecord(XDataCode.RealX, hatch.Pattern.Origin.X));
            data.XDataRecord.Add(new XDataRecord(XDataCode.RealY, hatch.Pattern.Origin.Y));
            data.XDataRecord.Add(new XDataRecord(XDataCode.RealZ, 0.0));
            HatchGradientPattern pattern = hatch.Pattern as HatchGradientPattern;
            if (pattern != null)
            {
                if (hatch.XData.ContainsAppId("GradientColor1ACI"))
                {
                    data = hatch.XData["GradientColor1ACI"];
                    data.XDataRecord.Clear();
                }
                else
                {
                    data = new XData(new ApplicationRegistry("GradientColor1ACI"));
                    hatch.XData.Add(data);
                }
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, pattern.Color1.Index));
                if (hatch.XData.ContainsAppId("GradientColor2ACI"))
                {
                    data = hatch.XData["GradientColor2ACI"];
                    data.XDataRecord.Clear();
                }
                else
                {
                    data = new XData(new ApplicationRegistry("GradientColor2ACI"));
                    hatch.XData.Add(data);
                }
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, pattern.Color2.Index));
            }
        }

        private static void AddLeaderTextPositionXData(Leader leader)
        {
            MText annotation = leader.Annotation as MText;
            if (annotation > null)
            {
                XData data;
                if (leader.XData.ContainsAppId("ACAD"))
                {
                    data = leader.XData["ACAD"];
                    data.XDataRecord.Clear();
                }
                else
                {
                    data = new XData(new ApplicationRegistry("ACAD"));
                    leader.XData.Add(data);
                }
                data.XDataRecord.Add(new XDataRecord(XDataCode.String, "DSTYLE"));
                data.XDataRecord.Add(new XDataRecord(XDataCode.ControlString, "{"));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 70));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) annotation.AttachmentPoint));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) 0x4d));
                data.XDataRecord.Add(new XDataRecord(XDataCode.Int16, (short) leader.TextVerticalPosition));
                data.XDataRecord.Add(new XDataRecord(XDataCode.ControlString, "}"));
            }
        }

        private void BeginSection(string section)
        {
            Debug.Assert(this.activeSection == "");
            this.chunk.Write(0, "SECTION");
            this.chunk.Write(2, section);
            this.activeSection = section;
        }

        private void BeginTable(string table, short numEntries, string handle)
        {
            Debug.Assert(this.activeSection == "TABLES");
            this.chunk.Write(0, "TABLE");
            this.chunk.Write(2, table);
            this.chunk.Write(5, handle);
            this.chunk.Write(330, "0");
            this.chunk.Write(100, "AcDbSymbolTable");
            this.chunk.Write(70, numEntries);
            if (table == "DIMSTYLE")
            {
                this.chunk.Write(100, "AcDbDimStyleTable");
            }
            this.activeTable = table;
        }

        private void Close()
        {
            this.chunk.Write(0, "EOF");
            this.chunk.Flush();
        }

        private string EncodeNonAsciiCharacters(string text)
        {
            if (this.doc.DrawingVariables.AcadVer >= DxfVersion.AutoCad2007)
            {
                return text;
            }
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            if (!this.encodedStrings.TryGetValue(text, out string str))
            {
                StringBuilder builder = new StringBuilder();
                foreach (char ch in text)
                {
                    if (ch > '\x00ff')
                    {
                        builder.Append(@"\U+" + $"{Convert.ToInt32(ch):X4}");
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
                str = builder.ToString();
                this.encodedStrings.Add(text, str);
            }
            return str;
        }

        private void EndSection()
        {
            Debug.Assert(this.activeSection != "");
            this.chunk.Write(0, "ENDSEC");
            this.activeSection = "";
        }

        private void EndTable()
        {
            Debug.Assert(this.activeSection != "");
            this.chunk.Write(0, "ENDTAB");
            this.activeTable = "";
        }

        private static double[] GetEllipseParameters(netDxf.Entities.Ellipse ellipse)
        {
            double num;
            double num2;
            if (ellipse.IsFullEllipse)
            {
                num = 0.0;
                num2 = 6.2831853071795862;
            }
            else
            {
                Vector2 vector = new Vector2(ellipse.Center.X, ellipse.Center.Y) + ellipse.PolarCoordinateRelativeToCenter(ellipse.StartAngle);
                Vector2 vector2 = new Vector2(ellipse.Center.X, ellipse.Center.Y) + ellipse.PolarCoordinateRelativeToCenter(ellipse.EndAngle);
                double num3 = ellipse.MajorAxis * 0.5;
                double num4 = ellipse.MinorAxis * 0.5;
                double x = (vector.X - ellipse.Center.X) / num3;
                double y = (vector.Y - ellipse.Center.Y) / num4;
                double num7 = (vector2.X - ellipse.Center.X) / num3;
                double num8 = (vector2.Y - ellipse.Center.Y) / num4;
                num = Math.Atan2(y, x);
                num2 = Math.Atan2(num8, num7);
            }
            return new double[] { num, num2 };
        }

        private void Open(Stream stream, Encoding encoding)
        {
            if (this.isBinary)
            {
                this.chunk = new BinaryCodeValueWriter((encoding == null) ? new BinaryWriter(stream) : new BinaryWriter(stream, encoding));
            }
            else
            {
                this.chunk = new TextCodeValueWriter((encoding == null) ? new StreamWriter(stream) : new StreamWriter(stream, encoding));
            }
        }

        public void Write(Stream stream, DxfDocument document, bool binary)
        {
            this.doc = document;
            this.isBinary = binary;
            if (this.doc.DrawingVariables.AcadVer < DxfVersion.AutoCad2000)
            {
                throw new NotSupportedException("Only AutoCad2000 and newer dxf versions are supported.");
            }
            this.encodedStrings = new Dictionary<string, string>();
            if (this.doc.Layouts.Count == 1)
            {
                this.doc.Layouts.Add(new Layout("Layout1"));
            }
            this.doc.ApplicationRegistries.Add(new ApplicationRegistry("AcCmTransparency"));
            this.doc.ApplicationRegistries.Add(new ApplicationRegistry("GradientColor1ACI"));
            this.doc.ApplicationRegistries.Add(new ApplicationRegistry("GradientColor2ACI"));
            List<DictionaryObject> list = new List<DictionaryObject>();
            DictionaryObject item = new DictionaryObject(this.doc);
            this.doc.NumHandles = item.AsignHandle(this.doc.NumHandles);
            list.Add(item);
            DictionaryObject obj3 = new DictionaryObject(item);
            this.doc.NumHandles = obj3.AsignHandle(this.doc.NumHandles);
            foreach (Group group in this.doc.Groups.Items)
            {
                obj3.Entries.Add(group.Handle, group.Name);
            }
            list.Add(obj3);
            item.Entries.Add(obj3.Handle, "ACAD_GROUP");
            DictionaryObject obj4 = new DictionaryObject(item);
            this.doc.NumHandles = obj4.AsignHandle(this.doc.NumHandles);
            if (this.doc.Layouts.Count > 0)
            {
                foreach (Layout layout in this.doc.Layouts.Items)
                {
                    obj4.Entries.Add(layout.Handle, layout.Name);
                }
                list.Add(obj4);
                item.Entries.Add(obj4.Handle, "ACAD_LAYOUT");
            }
            DictionaryObject obj5 = new DictionaryObject(item);
            this.doc.NumHandles = obj5.AsignHandle(this.doc.NumHandles);
            if (this.doc.UnderlayDgnDefinitions.Count > 0)
            {
                foreach (UnderlayDgnDefinition definition in this.doc.UnderlayDgnDefinitions.Items)
                {
                    obj5.Entries.Add(definition.Handle, definition.Name);
                    list.Add(obj5);
                    item.Entries.Add(obj5.Handle, "ACAD_DGNDEFINITIONS");
                }
            }
            DictionaryObject obj6 = new DictionaryObject(item);
            this.doc.NumHandles = obj6.AsignHandle(this.doc.NumHandles);
            if (this.doc.UnderlayDwfDefinitions.Count > 0)
            {
                foreach (UnderlayDwfDefinition definition2 in this.doc.UnderlayDwfDefinitions.Items)
                {
                    obj6.Entries.Add(definition2.Handle, definition2.Name);
                    list.Add(obj6);
                    item.Entries.Add(obj6.Handle, "ACAD_DWFDEFINITIONS");
                }
            }
            DictionaryObject obj7 = new DictionaryObject(item);
            this.doc.NumHandles = obj7.AsignHandle(this.doc.NumHandles);
            if (this.doc.UnderlayPdfDefinitions.Count > 0)
            {
                foreach (UnderlayPdfDefinition definition3 in this.doc.UnderlayPdfDefinitions.Items)
                {
                    obj7.Entries.Add(definition3.Handle, definition3.Name);
                    list.Add(obj7);
                    item.Entries.Add(obj7.Handle, "ACAD_PDFDEFINITIONS");
                }
            }
            DictionaryObject obj8 = new DictionaryObject(item);
            this.doc.NumHandles = obj8.AsignHandle(this.doc.NumHandles);
            if (this.doc.MlineStyles.Count > 0)
            {
                foreach (MLineStyle style2 in this.doc.MlineStyles.Items)
                {
                    obj8.Entries.Add(style2.Handle, style2.Name);
                }
                list.Add(obj8);
                item.Entries.Add(obj8.Handle, "ACAD_MLINESTYLE");
            }
            DictionaryObject obj9 = new DictionaryObject(item);
            this.doc.NumHandles = obj9.AsignHandle(this.doc.NumHandles);
            if (this.doc.ImageDefinitions.Count > 0)
            {
                foreach (ImageDefinition definition4 in this.doc.ImageDefinitions.Items)
                {
                    obj9.Entries.Add(definition4.Handle, definition4.Name);
                }
                list.Add(obj9);
                item.Entries.Add(obj9.Handle, "ACAD_IMAGE_DICT");
                item.Entries.Add(this.doc.RasterVariables.Handle, "ACAD_IMAGE_VARS");
            }
            this.doc.DrawingVariables.HandleSeed = this.doc.NumHandles.ToString("X");
            this.Open(stream, (this.doc.DrawingVariables.AcadVer < DxfVersion.AutoCad2007) ? Encoding.Default : null);
            if (!this.isBinary)
            {
                foreach (string str in this.doc.Comments)
                {
                    this.WriteComment(str);
                }
            }
            this.BeginSection("HEADER");
            foreach (HeaderVariable variable in this.doc.DrawingVariables.Values)
            {
                this.WriteSystemVariable(variable);
            }
            if (this.doc.DimensionStyles.TryGetValue(this.doc.DrawingVariables.DimStyle, out DimensionStyle style))
            {
                this.WriteActiveDimensionStyleSystemVaribles(style);
            }
            this.EndSection();
            this.BeginSection("CLASSES");
            this.WriteRasterVariablesClass(1);
            if (this.doc.ImageDefinitions.Items.Count > 0)
            {
                this.WriteImageDefClass(this.doc.ImageDefinitions.Count);
                this.WriteImageDefRectorClass(this.doc.Images.Count);
                this.WriteImageClass(this.doc.Images.Count);
            }
            this.EndSection();
            this.BeginSection("TABLES");
            this.BeginTable(this.doc.ApplicationRegistries.CodeName, (short) this.doc.ApplicationRegistries.Count, this.doc.ApplicationRegistries.Handle);
            foreach (ApplicationRegistry registry in this.doc.ApplicationRegistries.Items)
            {
                this.WriteApplicationRegistry(registry);
            }
            this.EndTable();
            this.BeginTable(this.doc.VPorts.CodeName, (short) this.doc.VPorts.Count, this.doc.VPorts.Handle);
            foreach (VPort port in this.doc.VPorts)
            {
                this.WriteVPort(port);
            }
            this.EndTable();
            this.BeginTable(this.doc.Linetypes.CodeName, (short) this.doc.Linetypes.Count, this.doc.Linetypes.Handle);
            foreach (Linetype linetype in this.doc.Linetypes.Items)
            {
                this.WriteLinetype(linetype);
            }
            this.EndTable();
            this.BeginTable(this.doc.Layers.CodeName, (short) this.doc.Layers.Count, this.doc.Layers.Handle);
            foreach (Layer layer in this.doc.Layers.Items)
            {
                this.WriteLayer(layer);
            }
            this.EndTable();
            this.BeginTable(this.doc.TextStyles.CodeName, (short) this.doc.TextStyles.Count, this.doc.TextStyles.Handle);
            foreach (TextStyle style3 in this.doc.TextStyles.Items)
            {
                this.WriteTextStyle(style3);
            }
            this.EndTable();
            this.BeginTable(this.doc.DimensionStyles.CodeName, (short) this.doc.DimensionStyles.Count, this.doc.DimensionStyles.Handle);
            foreach (DimensionStyle style4 in this.doc.DimensionStyles.Items)
            {
                this.WriteDimensionStyle(style4);
            }
            this.EndTable();
            this.BeginTable(this.doc.Views.CodeName, (short) this.doc.Views.Count, this.doc.Views.Handle);
            this.EndTable();
            this.BeginTable(this.doc.UCSs.CodeName, (short) this.doc.UCSs.Count, this.doc.UCSs.Handle);
            foreach (UCS ucs in this.doc.UCSs.Items)
            {
                this.WriteUCS(ucs);
            }
            this.EndTable();
            this.BeginTable(this.doc.Blocks.CodeName, (short) this.doc.Blocks.Count, this.doc.Blocks.Handle);
            foreach (Block block in this.doc.Blocks.Items)
            {
                this.WriteBlockRecord(block.Record);
            }
            this.EndTable();
            this.EndSection();
            this.BeginSection("BLOCKS");
            foreach (Block block2 in this.doc.Blocks.Items)
            {
                Layout layout = null;
                if (block2.Name.StartsWith("*Paper_Space") && !string.IsNullOrEmpty(block2.Name.Remove(0, 12)))
                {
                    layout = block2.Record.Layout;
                }
                this.WriteBlock(block2, layout);
            }
            this.EndSection();
            this.BeginSection("ENTITIES");
            foreach (Layout layout3 in this.doc.Layouts)
            {
                if (!layout3.IsPaperSpace)
                {
                    List<DxfObject> references = this.doc.Layouts.GetReferences(layout3);
                    foreach (DxfObject obj10 in references)
                    {
                        this.WriteEntity(obj10 as EntityObject, layout3);
                    }
                }
                else if (layout3.AssociatedBlock.Name.StartsWith("*Paper_Space") && string.IsNullOrEmpty(layout3.AssociatedBlock.Name.Remove(0, 12)))
                {
                    this.WriteEntity(layout3.Viewport, layout3);
                    List<DxfObject> references = this.doc.Layouts.GetReferences(layout3);
                    foreach (DxfObject obj11 in references)
                    {
                        this.WriteEntity(obj11 as EntityObject, layout3);
                    }
                }
            }
            this.EndSection();
            this.BeginSection("OBJECTS");
            foreach (DictionaryObject obj12 in list)
            {
                this.WriteDictionary(obj12);
            }
            foreach (Group group2 in this.doc.Groups.Items)
            {
                this.WriteGroup(group2, obj3.Handle);
            }
            foreach (Layout layout4 in this.doc.Layouts)
            {
                this.WriteLayout(layout4, obj4.Handle);
            }
            foreach (MLineStyle style5 in this.doc.MlineStyles.Items)
            {
                this.WriteMLineStyle(style5, obj8.Handle);
            }
            foreach (UnderlayDgnDefinition definition5 in this.doc.UnderlayDgnDefinitions.Items)
            {
                this.WriteUnderlayDefinition(definition5, obj5.Handle);
            }
            foreach (UnderlayDwfDefinition definition6 in this.doc.UnderlayDwfDefinitions.Items)
            {
                this.WriteUnderlayDefinition(definition6, obj6.Handle);
            }
            foreach (UnderlayPdfDefinition definition7 in this.doc.UnderlayPdfDefinitions.Items)
            {
                this.WriteUnderlayDefinition(definition7, obj7.Handle);
            }
            if (this.doc.ImageDefinitions.Count > 0)
            {
                this.WriteRasterVariables(this.doc.RasterVariables, obj9.Handle);
                foreach (ImageDefinition definition8 in this.doc.ImageDefinitions.Items)
                {
                    foreach (ImageDefinitionReactor reactor in definition8.Reactors.Values)
                    {
                        this.WriteImageDefReactor(reactor);
                    }
                    this.WriteImageDef(definition8, obj9.Handle);
                }
            }
            this.EndSection();
            this.Close();
            stream.Position = 0L;
        }

        private void WriteActiveDimensionStyleSystemVaribles(DimensionStyle style)
        {
            this.chunk.Write(9, "$DIMADEC");
            this.chunk.Write(70, style.AngularPrecision);
            this.chunk.Write(9, "$DIMAUNIT");
            this.chunk.Write(70, (short) style.DimAngularUnits);
            this.chunk.Write(9, "$DIMASZ");
            this.chunk.Write(40, style.ArrowSize);
            short num = 3;
            if (style.SuppressAngularLeadingZeros && style.SuppressAngularTrailingZeros)
            {
                num = 3;
            }
            else if (!style.SuppressAngularLeadingZeros && !style.SuppressAngularTrailingZeros)
            {
                num = 0;
            }
            else if (!style.SuppressAngularLeadingZeros && style.SuppressAngularTrailingZeros)
            {
                num = 2;
            }
            else if (style.SuppressAngularLeadingZeros && !style.SuppressAngularTrailingZeros)
            {
                num = 1;
            }
            this.chunk.Write(9, "$DIMAZIN");
            this.chunk.Write(70, num);
            if ((style.DimArrow1 == null) && (style.DimArrow2 == null))
            {
                this.chunk.Write(9, "$DIMSAH");
                this.chunk.Write(70, (short) 0);
                this.chunk.Write(9, "$DIMBLK");
                this.chunk.Write(1, "");
            }
            else if (style.DimArrow1 == null)
            {
                this.chunk.Write(9, "$DIMSAH");
                this.chunk.Write(70, (short) 1);
                this.chunk.Write(9, "$DIMBLK1");
                this.chunk.Write(1, "");
                this.chunk.Write(9, "$DIMBLK2");
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(style.DimArrow2.Name));
            }
            else if (style.DimArrow2 == null)
            {
                this.chunk.Write(9, "$DIMSAH");
                this.chunk.Write(70, (short) 1);
                this.chunk.Write(9, "$DIMBLK1");
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(style.DimArrow1.Name));
                this.chunk.Write(9, "$DIMBLK2");
                this.chunk.Write(1, "");
            }
            else if (string.Equals(style.DimArrow1.Name, style.DimArrow2.Name, StringComparison.OrdinalIgnoreCase))
            {
                this.chunk.Write(9, "$DIMSAH");
                this.chunk.Write(70, (short) 0);
                this.chunk.Write(9, "$DIMBLK");
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(style.DimArrow1.Name));
            }
            else
            {
                this.chunk.Write(9, "$DIMSAH");
                this.chunk.Write(70, (short) 1);
                this.chunk.Write(9, "$DIMBLK1");
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(style.DimArrow1.Name));
                this.chunk.Write(9, "$DIMBLK2");
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(style.DimArrow2.Name));
            }
            this.chunk.Write(9, "$DIMLDRBLK");
            this.chunk.Write(1, (style.LeaderArrow == null) ? "" : this.EncodeNonAsciiCharacters(style.LeaderArrow.Name));
            this.chunk.Write(9, "$DIMCEN");
            this.chunk.Write(40, style.CenterMarkSize);
            this.chunk.Write(9, "$DIMCLRD");
            this.chunk.Write(70, style.DimLineColor.Index);
            this.chunk.Write(9, "$DIMCLRE");
            this.chunk.Write(70, style.ExtLineColor.Index);
            this.chunk.Write(9, "$DIMCLRT");
            this.chunk.Write(70, style.TextColor.Index);
            this.chunk.Write(9, "$DIMDEC");
            this.chunk.Write(70, style.LengthPrecision);
            this.chunk.Write(9, "$DIMDLE");
            this.chunk.Write(40, style.DimLineExtend);
            this.chunk.Write(9, "$DIMDLI");
            this.chunk.Write(40, style.DimBaselineSpacing);
            this.chunk.Write(9, "$DIMDSEP");
            this.chunk.Write(70, (short) style.DecimalSeparator);
            this.chunk.Write(9, "$DIMEXE");
            this.chunk.Write(40, style.ExtLineExtend);
            this.chunk.Write(9, "$DIMEXO");
            this.chunk.Write(40, style.ExtLineOffset);
            this.chunk.Write(9, "$DIMGAP");
            this.chunk.Write(40, style.TextOffset);
            this.chunk.Write(9, "$DIMJUST");
            this.chunk.Write(70, style.DIMJUST);
            this.chunk.Write(9, "$DIMLFAC");
            this.chunk.Write(40, style.DimScaleLinear);
            this.chunk.Write(9, "$DIMLUNIT");
            this.chunk.Write(70, (short) style.DimLengthUnits);
            this.chunk.Write(9, "$DIMLWD");
            this.chunk.Write(70, (short) style.DimLineLineweight);
            this.chunk.Write(9, "$DIMLWE");
            this.chunk.Write(70, (short) style.ExtLineLineweight);
            this.chunk.Write(9, "$DIMPOST");
            this.chunk.Write(1, this.EncodeNonAsciiCharacters($"{style.DimPrefix}<>{style.DimSuffix}"));
            this.chunk.Write(9, "$DIMRND");
            this.chunk.Write(40, style.DimRoundoff);
            this.chunk.Write(9, "$DIMSCALE");
            this.chunk.Write(40, style.DimScaleOverall);
            this.chunk.Write(9, "$DIMSD1");
            if (style.DimLineOff)
            {
                this.chunk.Write(70, (short) 1);
            }
            else
            {
                this.chunk.Write(70, (short) 0);
            }
            this.chunk.Write(9, "$DIMSD2");
            if (style.DimLineOff)
            {
                this.chunk.Write(70, (short) 1);
            }
            else
            {
                this.chunk.Write(70, (short) 0);
            }
            this.chunk.Write(9, "$DIMSE1");
            if (style.ExtLine1Off)
            {
                this.chunk.Write(70, (short) 1);
            }
            else
            {
                this.chunk.Write(70, (short) 0);
            }
            this.chunk.Write(9, "$DIMSE2");
            if (style.ExtLine2Off)
            {
                this.chunk.Write(70, (short) 1);
            }
            else
            {
                this.chunk.Write(70, (short) 0);
            }
            this.chunk.Write(9, "$DIMTAD");
            this.chunk.Write(70, style.DIMTAD);
            this.chunk.Write(9, "$DIMTIH");
            this.chunk.Write(70, style.DIMTIH);
            this.chunk.Write(9, "$DIMTOH");
            this.chunk.Write(70, style.DIMTOH);
            this.chunk.Write(9, "$DIMTXT");
            this.chunk.Write(40, style.TextHeight);
            short num2 = 0;
            if ((style.DimLengthUnits == LinearUnitType.Architectural) || (style.DimLengthUnits == LinearUnitType.Engineering))
            {
                if (style.SuppressZeroFeet && style.SuppressZeroFeet)
                {
                    num2 = 0;
                }
                else if (!style.SuppressZeroFeet && !style.SuppressZeroFeet)
                {
                    num2 = 1;
                }
                else if (!style.SuppressZeroFeet && style.SuppressZeroFeet)
                {
                    num2 = 2;
                }
                else if (style.SuppressZeroFeet && !style.SuppressZeroFeet)
                {
                    num2 = 3;
                }
            }
            if (!style.SuppressLinearLeadingZeros && !style.SuppressLinearTrailingZeros)
            {
                num2 = num2;
            }
            else if (style.SuppressLinearLeadingZeros && !style.SuppressLinearTrailingZeros)
            {
                num2 = (short) (num2 + 4);
            }
            else if (!style.SuppressLinearLeadingZeros && style.SuppressLinearTrailingZeros)
            {
                num2 = (short) (num2 + 8);
            }
            else if (style.SuppressLinearLeadingZeros && style.SuppressLinearTrailingZeros)
            {
                num2 = (short) (num2 + 12);
            }
            this.chunk.Write(9, "$DIMZIN");
            this.chunk.Write(70, num2);
            this.chunk.Write(9, "$DIMFRAC");
            this.chunk.Write(70, (short) style.FractionalType);
            this.chunk.Write(9, "$DIMLTYPE");
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(style.DimLineLinetype.Name));
            this.chunk.Write(9, "$DIMLTEX1");
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(style.ExtLine1Linetype.Name));
            this.chunk.Write(9, "$DIMLTEX2");
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(style.ExtLine2Linetype.Name));
        }

        private void WriteAlignedDimension(AlignedDimension dim)
        {
            this.chunk.Write(100, "AcDbAlignedDimension");
            Vector3[] points = new Vector3[] { new Vector3(dim.FirstReferencePoint.X, dim.FirstReferencePoint.Y, dim.Elevation), new Vector3(dim.SecondReferencePoint.X, dim.SecondReferencePoint.Y, dim.Elevation) };
            IList<Vector3> list = MathHelper.Transform(points, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3 vector2 = list[0];
            this.chunk.Write(13, vector2.X);
            vector2 = list[0];
            this.chunk.Write(0x17, vector2.Y);
            vector2 = list[0];
            this.chunk.Write(0x21, vector2.Z);
            vector2 = list[1];
            this.chunk.Write(14, vector2.X);
            vector2 = list[1];
            this.chunk.Write(0x18, vector2.Y);
            vector2 = list[1];
            this.chunk.Write(0x22, vector2.Z);
            this.WriteXData(dim.XData);
        }

        private void WriteAngular2LineDimension(Angular2LineDimension dim)
        {
            this.chunk.Write(100, "AcDb2LineAngularDimension");
            Vector3[] points = new Vector3[] { new Vector3(dim.StartFirstLine.X, dim.StartFirstLine.Y, dim.Elevation), new Vector3(dim.EndFirstLine.X, dim.EndFirstLine.Y, dim.Elevation), new Vector3(dim.StartSecondLine.X, dim.StartSecondLine.Y, dim.Elevation) };
            IList<Vector3> list = MathHelper.Transform(points, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3 vector2 = list[0];
            this.chunk.Write(13, vector2.X);
            vector2 = list[0];
            this.chunk.Write(0x17, vector2.Y);
            vector2 = list[0];
            this.chunk.Write(0x21, vector2.Z);
            vector2 = list[1];
            this.chunk.Write(14, vector2.X);
            vector2 = list[1];
            this.chunk.Write(0x18, vector2.Y);
            vector2 = list[1];
            this.chunk.Write(0x22, vector2.Z);
            vector2 = list[2];
            this.chunk.Write(15, vector2.X);
            vector2 = list[2];
            this.chunk.Write(0x19, vector2.Y);
            vector2 = list[2];
            this.chunk.Write(0x23, vector2.Z);
            this.chunk.Write(0x10, dim.ArcDefinitionPoint.X);
            this.chunk.Write(0x1a, dim.ArcDefinitionPoint.Y);
            this.chunk.Write(0x24, dim.ArcDefinitionPoint.Z);
            this.chunk.Write(40, 0.0);
            this.WriteXData(dim.XData);
        }

        private void WriteAngular3PointDimension(Angular3PointDimension dim)
        {
            this.chunk.Write(100, "AcDb3PointAngularDimension");
            Vector3[] points = new Vector3[] { new Vector3(dim.StartPoint.X, dim.StartPoint.Y, dim.Elevation), new Vector3(dim.EndPoint.X, dim.EndPoint.Y, dim.Elevation), new Vector3(dim.CenterPoint.X, dim.CenterPoint.Y, dim.Elevation) };
            IList<Vector3> list = MathHelper.Transform(points, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3 vector2 = list[0];
            this.chunk.Write(13, vector2.X);
            vector2 = list[0];
            this.chunk.Write(0x17, vector2.Y);
            vector2 = list[0];
            this.chunk.Write(0x21, vector2.Z);
            vector2 = list[1];
            this.chunk.Write(14, vector2.X);
            vector2 = list[1];
            this.chunk.Write(0x18, vector2.Y);
            vector2 = list[1];
            this.chunk.Write(0x22, vector2.Z);
            vector2 = list[2];
            this.chunk.Write(15, vector2.X);
            vector2 = list[2];
            this.chunk.Write(0x19, vector2.Y);
            vector2 = list[2];
            this.chunk.Write(0x23, vector2.Z);
            this.chunk.Write(40, 0.0);
            this.WriteXData(dim.XData);
        }

        private void WriteApplicationRegistry(ApplicationRegistry appReg)
        {
            Debug.Assert(this.activeTable == "APPID");
            this.chunk.Write(0, "APPID");
            this.chunk.Write(5, appReg.Handle);
            this.chunk.Write(330, appReg.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbRegAppTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(appReg.Name));
            this.chunk.Write(70, (short) 0);
        }

        private void WriteArc(netDxf.Entities.Arc arc)
        {
            this.chunk.Write(100, "AcDbCircle");
            this.chunk.Write(0x27, arc.Thickness);
            Vector3 vector = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            this.chunk.Write(40, arc.Radius);
            this.chunk.Write(210, arc.Normal.X);
            this.chunk.Write(220, arc.Normal.Y);
            this.chunk.Write(230, arc.Normal.Z);
            this.chunk.Write(100, "AcDbArc");
            this.chunk.Write(50, arc.StartAngle);
            this.chunk.Write(0x33, arc.EndAngle);
            this.WriteXData(arc.XData);
        }

        private void WriteAttribute(netDxf.Entities.Attribute attrib)
        {
            this.chunk.Write(0, attrib.CodeName);
            this.chunk.Write(5, attrib.Handle);
            this.chunk.Write(330, attrib.Owner.Handle);
            this.chunk.Write(100, "AcDbEntity");
            this.chunk.Write(8, this.EncodeNonAsciiCharacters(attrib.Layer.Name));
            this.chunk.Write(0x3e, attrib.Color.Index);
            if (attrib.Color.UseTrueColor)
            {
                this.chunk.Write(420, AciColor.ToTrueColor(attrib.Color));
            }
            if (attrib.Transparency.Value >= 0)
            {
                this.chunk.Write(440, Transparency.ToAlphaValue(attrib.Transparency));
            }
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(attrib.Linetype.Name));
            this.chunk.Write(370, (short) attrib.Lineweight);
            this.chunk.Write(0x30, attrib.LinetypeScale);
            this.chunk.Write(60, attrib.IsVisible ? ((short) 0) : ((short) 1));
            this.chunk.Write(100, "AcDbText");
            Vector3 vector = MathHelper.Transform(attrib.Position, attrib.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            this.chunk.Write(40, attrib.Height);
            this.chunk.Write(0x29, attrib.WidthFactor);
            this.chunk.Write(7, this.EncodeNonAsciiCharacters(attrib.Style.Name));
            object obj2 = attrib.Value;
            if (obj2 == null)
            {
                this.chunk.Write(1, string.Empty);
            }
            else if (obj2 is string)
            {
                this.chunk.Write(1, this.EncodeNonAsciiCharacters((string) obj2));
            }
            else
            {
                this.chunk.Write(1, obj2.ToString());
            }
            switch (attrib.Alignment)
            {
                case netDxf.Entities.TextAlignment.TopLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.TopCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.TopRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.MiddleCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.MiddleRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BottomLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BottomCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BaselineLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BaselineRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.Aligned:
                    this.chunk.Write(0x48, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.Middle:
                    this.chunk.Write(0x48, (short) 4);
                    break;

                case netDxf.Entities.TextAlignment.Fit:
                    this.chunk.Write(0x48, (short) 5);
                    break;
            }
            this.chunk.Write(11, vector.X);
            this.chunk.Write(0x15, vector.Y);
            this.chunk.Write(0x1f, vector.Z);
            this.chunk.Write(50, attrib.Rotation);
            this.chunk.Write(0x33, attrib.ObliqueAngle);
            this.chunk.Write(210, attrib.Normal.X);
            this.chunk.Write(220, attrib.Normal.Y);
            this.chunk.Write(230, attrib.Normal.Z);
            this.chunk.Write(100, "AcDbAttribute");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(attrib.Tag));
            this.chunk.Write(70, (short) attrib.Flags);
            switch (attrib.Alignment)
            {
                case netDxf.Entities.TextAlignment.TopLeft:
                    this.chunk.Write(0x4a, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.TopCenter:
                    this.chunk.Write(0x4a, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.TopRight:
                    this.chunk.Write(0x4a, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.MiddleLeft:
                    this.chunk.Write(0x4a, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleCenter:
                    this.chunk.Write(0x4a, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleRight:
                    this.chunk.Write(0x4a, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BottomLeft:
                    this.chunk.Write(0x4a, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomCenter:
                    this.chunk.Write(0x4a, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomRight:
                    this.chunk.Write(0x4a, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BaselineLeft:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineCenter:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineRight:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Aligned:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Middle:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Fit:
                    this.chunk.Write(0x4a, (short) 0);
                    break;
            }
        }

        private void WriteAttributeDefinition(AttributeDefinition def)
        {
            this.chunk.Write(100, "AcDbText");
            Vector3 vector = MathHelper.Transform(def.Position, def.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            this.chunk.Write(40, def.Height);
            object obj2 = def.Value;
            if (obj2 == null)
            {
                this.chunk.Write(1, string.Empty);
            }
            else if (obj2 is string)
            {
                this.chunk.Write(1, this.EncodeNonAsciiCharacters((string) obj2));
            }
            else
            {
                this.chunk.Write(1, obj2.ToString());
            }
            switch (def.Alignment)
            {
                case netDxf.Entities.TextAlignment.TopLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.TopCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.TopRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.MiddleCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.MiddleRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BottomLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BottomCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BaselineLeft:
                    this.chunk.Write(0x48, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineCenter:
                    this.chunk.Write(0x48, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BaselineRight:
                    this.chunk.Write(0x48, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.Aligned:
                    this.chunk.Write(0x48, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.Middle:
                    this.chunk.Write(0x48, (short) 4);
                    break;

                case netDxf.Entities.TextAlignment.Fit:
                    this.chunk.Write(0x48, (short) 5);
                    break;
            }
            this.chunk.Write(50, def.Rotation);
            this.chunk.Write(0x33, def.ObliqueAngle);
            this.chunk.Write(0x29, def.WidthFactor);
            this.chunk.Write(7, this.EncodeNonAsciiCharacters(def.Style.Name));
            this.chunk.Write(11, def.Position.X);
            this.chunk.Write(0x15, def.Position.Y);
            this.chunk.Write(0x1f, def.Position.Z);
            this.chunk.Write(210, def.Normal.X);
            this.chunk.Write(220, def.Normal.Y);
            this.chunk.Write(230, def.Normal.Z);
            this.chunk.Write(100, "AcDbAttributeDefinition");
            this.chunk.Write(3, this.EncodeNonAsciiCharacters(def.Prompt));
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(def.Tag));
            this.chunk.Write(70, (short) def.Flags);
            switch (def.Alignment)
            {
                case netDxf.Entities.TextAlignment.TopLeft:
                    this.chunk.Write(0x4a, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.TopCenter:
                    this.chunk.Write(0x4a, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.TopRight:
                    this.chunk.Write(0x4a, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.MiddleLeft:
                    this.chunk.Write(0x4a, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleCenter:
                    this.chunk.Write(0x4a, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleRight:
                    this.chunk.Write(0x4a, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BottomLeft:
                    this.chunk.Write(0x4a, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomCenter:
                    this.chunk.Write(0x4a, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomRight:
                    this.chunk.Write(0x4a, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BaselineLeft:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineCenter:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineRight:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Aligned:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Middle:
                    this.chunk.Write(0x4a, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Fit:
                    this.chunk.Write(0x4a, (short) 0);
                    break;
            }
        }

        private void WriteBlock(Block block, Layout layout)
        {
            Debug.Assert(this.activeSection == "BLOCKS");
            string str = this.EncodeNonAsciiCharacters(block.Name);
            string str2 = this.EncodeNonAsciiCharacters(block.Layer.Name);
            this.chunk.Write(0, block.CodeName);
            this.chunk.Write(5, block.Handle);
            this.chunk.Write(330, block.Owner.Handle);
            this.chunk.Write(100, "AcDbEntity");
            if (layout > null)
            {
                this.chunk.Write(0x43, layout.IsPaperSpace ? ((short) 1) : ((short) 0));
            }
            this.chunk.Write(8, str2);
            this.chunk.Write(100, "AcDbBlockBegin");
            if (block.IsXRef)
            {
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(block.XrefFile));
            }
            this.chunk.Write(2, str);
            this.chunk.Write(70, (short) block.Flags);
            this.chunk.Write(10, block.Origin.X);
            this.chunk.Write(20, block.Origin.Y);
            this.chunk.Write(30, block.Origin.Z);
            this.chunk.Write(3, str);
            foreach (AttributeDefinition definition in block.AttributeDefinitions.Values)
            {
                this.WriteEntityCommonCodes(definition, null);
                this.WriteAttributeDefinition(definition);
            }
            if (layout == null)
            {
                foreach (EntityObject obj2 in block.Entities)
                {
                    this.WriteEntity(obj2, null);
                }
            }
            else
            {
                this.WriteEntity(layout.Viewport, layout);
                List<DxfObject> references = this.doc.Layouts.GetReferences(layout);
                foreach (DxfObject obj3 in references)
                {
                    this.WriteEntity(obj3 as EntityObject, layout);
                }
            }
            this.chunk.Write(0, block.End.CodeName);
            this.chunk.Write(5, block.End.Handle);
            this.chunk.Write(330, block.Owner.Handle);
            this.chunk.Write(100, "AcDbEntity");
            this.chunk.Write(8, str2);
            this.chunk.Write(100, "AcDbBlockEnd");
        }

        private void WriteBlockRecord(BlockRecord blockRecord)
        {
            Debug.Assert(this.activeTable == "BLOCK_RECORD");
            this.chunk.Write(0, blockRecord.CodeName);
            this.chunk.Write(5, blockRecord.Handle);
            this.chunk.Write(330, blockRecord.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbBlockTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(blockRecord.Name));
            this.chunk.Write(340, (blockRecord.Layout == null) ? "0" : blockRecord.Layout.Handle);
            if (!blockRecord.IsForInternalUseOnly)
            {
                this.chunk.Write(70, (short) blockRecord.Units);
                this.chunk.Write(280, blockRecord.AllowExploding ? ((short) 1) : ((short) 0));
                this.chunk.Write(0x119, blockRecord.ScaleUniformly ? ((short) 1) : ((short) 0));
                AddBlockRecordUnitsXData(blockRecord);
                this.WriteXData(blockRecord.XData);
            }
        }

        private void WriteCircle(Circle circle)
        {
            this.chunk.Write(100, "AcDbCircle");
            Vector3 vector = MathHelper.Transform(circle.Center, circle.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            this.chunk.Write(40, circle.Radius);
            this.chunk.Write(0x27, circle.Thickness);
            this.chunk.Write(210, circle.Normal.X);
            this.chunk.Write(220, circle.Normal.Y);
            this.chunk.Write(230, circle.Normal.Z);
            this.WriteXData(circle.XData);
        }

        private void WriteComment(string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                this.chunk.Write(0x3e7, comment);
            }
        }

        private void WriteDiametricDimension(DiametricDimension dim)
        {
            this.chunk.Write(100, "AcDbDiametricDimension");
            Vector3 vector = MathHelper.Transform(new Vector3(dim.ReferencePoint.X, dim.ReferencePoint.Y, dim.Elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.chunk.Write(15, vector.X);
            this.chunk.Write(0x19, vector.Y);
            this.chunk.Write(0x23, vector.Z);
            this.chunk.Write(40, 0.0);
            this.WriteXData(dim.XData);
        }

        private void WriteDictionary(DictionaryObject dictionary)
        {
            this.chunk.Write(0, "DICTIONARY");
            this.chunk.Write(5, dictionary.Handle);
            this.chunk.Write(330, dictionary.Owner.Handle);
            this.chunk.Write(100, "AcDbDictionary");
            this.chunk.Write(280, dictionary.IsHardOwner ? ((short) 1) : ((short) 0));
            this.chunk.Write(0x119, (short) dictionary.Cloning);
            if (dictionary.Entries != null)
            {
                foreach (KeyValuePair<string, string> pair in dictionary.Entries)
                {
                    this.chunk.Write(3, this.EncodeNonAsciiCharacters(pair.Value));
                    this.chunk.Write(350, pair.Key);
                }
            }
        }

        private void WriteDimension(Dimension dim)
        {
            this.chunk.Write(100, "AcDbDimension");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(dim.Block.Name));
            this.chunk.Write(10, dim.DefinitionPoint.X);
            this.chunk.Write(20, dim.DefinitionPoint.Y);
            this.chunk.Write(30, dim.DefinitionPoint.Z);
            this.chunk.Write(11, dim.MidTextPoint.X);
            this.chunk.Write(0x15, dim.MidTextPoint.Y);
            this.chunk.Write(0x1f, dim.MidTextPoint.Z);
            short num = (short) (((short) dim.DimensionType) + 0x20);
            OrdinateDimension dimension = dim as OrdinateDimension;
            if (dimension > null)
            {
                this.chunk.Write(0x33, 360.0 - dimension.Rotation);
                if (dimension.Axis == OrdinateDimensionAxis.X)
                {
                    num = (short) (num + 0x40);
                }
            }
            this.chunk.Write(70, num);
            this.chunk.Write(0x47, (short) dim.AttachmentPoint);
            this.chunk.Write(0x48, (short) dim.LineSpacingStyle);
            this.chunk.Write(0x29, dim.LineSpacingFactor);
            if (dim.UserText > null)
            {
                this.chunk.Write(1, this.EncodeNonAsciiCharacters(dim.UserText));
            }
            this.chunk.Write(210, dim.Normal.X);
            this.chunk.Write(220, dim.Normal.Y);
            this.chunk.Write(230, dim.Normal.Z);
            this.chunk.Write(3, this.EncodeNonAsciiCharacters(dim.Style.Name));
            if (dim.StyleOverrides.Count > 0)
            {
                this.AddDimensionStyleOverridesXData(dim.XData, dim.StyleOverrides, dim);
            }
            switch (dim.DimensionType)
            {
                case DimensionType.Linear:
                    this.WriteLinearDimension((LinearDimension) dim);
                    break;

                case DimensionType.Aligned:
                    this.WriteAlignedDimension((AlignedDimension) dim);
                    break;

                case DimensionType.Angular:
                    this.WriteAngular2LineDimension((Angular2LineDimension) dim);
                    break;

                case DimensionType.Diameter:
                    this.WriteDiametricDimension((DiametricDimension) dim);
                    break;

                case DimensionType.Radius:
                    this.WriteRadialDimension((RadialDimension) dim);
                    break;

                case DimensionType.Angular3Point:
                    this.WriteAngular3PointDimension((Angular3PointDimension) dim);
                    break;

                case DimensionType.Ordinate:
                    this.WriteOrdinateDimension((OrdinateDimension) dim);
                    break;
            }
        }

        private void WriteDimensionStyle(DimensionStyle style)
        {
            Debug.Assert(this.activeTable == "DIMSTYLE");
            this.chunk.Write(0, style.CodeName);
            this.chunk.Write(0x69, style.Handle);
            this.chunk.Write(330, style.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbDimStyleTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(style.Name));
            this.chunk.Write(70, (short) 0);
            this.chunk.Write(3, this.EncodeNonAsciiCharacters($"{style.DimPrefix}<>{style.DimSuffix}"));
            this.chunk.Write(40, style.DimScaleOverall);
            this.chunk.Write(0x29, style.ArrowSize);
            this.chunk.Write(0x2a, style.ExtLineOffset);
            this.chunk.Write(0x2b, style.DimBaselineSpacing);
            this.chunk.Write(0x2c, style.ExtLineExtend);
            this.chunk.Write(0x2d, style.DimRoundoff);
            this.chunk.Write(0x2e, style.DimLineExtend);
            this.chunk.Write(0x49, style.DIMTIH);
            this.chunk.Write(0x4a, style.DIMTOH);
            if (style.ExtLine1Off)
            {
                this.chunk.Write(0x4b, (short) 1);
            }
            else
            {
                this.chunk.Write(0x4b, (short) 0);
            }
            if (style.ExtLine2Off)
            {
                this.chunk.Write(0x4c, (short) 1);
            }
            else
            {
                this.chunk.Write(0x4c, (short) 0);
            }
            this.chunk.Write(0x4d, style.DIMTAD);
            short num = 0;
            if (style.SuppressZeroFeet && style.SuppressZeroInches)
            {
                num = 0;
            }
            else if (!style.SuppressZeroFeet && !style.SuppressZeroInches)
            {
                num = (short) (num + 1);
            }
            else if (!style.SuppressZeroFeet && style.SuppressZeroInches)
            {
                num = (short) (num + 2);
            }
            else if (style.SuppressZeroFeet && !style.SuppressZeroInches)
            {
                num = (short) (num + 3);
            }
            if (!style.SuppressLinearLeadingZeros && !style.SuppressLinearTrailingZeros)
            {
                num = num;
            }
            else if (style.SuppressLinearLeadingZeros && !style.SuppressLinearTrailingZeros)
            {
                num = (short) (num + 4);
            }
            else if (!style.SuppressLinearLeadingZeros && style.SuppressLinearTrailingZeros)
            {
                num = (short) (num + 8);
            }
            else if (style.SuppressLinearLeadingZeros && style.SuppressLinearTrailingZeros)
            {
                num = (short) (num + 12);
            }
            this.chunk.Write(0x4e, num);
            short num2 = 3;
            if (style.SuppressAngularLeadingZeros && style.SuppressAngularTrailingZeros)
            {
                num2 = 3;
            }
            else if (!style.SuppressAngularLeadingZeros && !style.SuppressAngularTrailingZeros)
            {
                num2 = 0;
            }
            else if (!style.SuppressAngularLeadingZeros && style.SuppressAngularTrailingZeros)
            {
                num2 = 2;
            }
            else if (style.SuppressAngularLeadingZeros && !style.SuppressAngularTrailingZeros)
            {
                num2 = 1;
            }
            this.chunk.Write(0x4f, num2);
            this.chunk.Write(140, style.TextHeight);
            this.chunk.Write(0x8d, style.CenterMarkSize);
            this.chunk.Write(0x90, style.DimScaleLinear);
            this.chunk.Write(0x93, style.TextOffset);
            this.chunk.Write(0xb0, style.DimLineColor.Index);
            this.chunk.Write(0xb1, style.ExtLineColor.Index);
            this.chunk.Write(0xb2, style.TextColor.Index);
            this.chunk.Write(0xb3, style.AngularPrecision);
            this.chunk.Write(0x10f, style.LengthPrecision);
            this.chunk.Write(0x113, (short) style.DimAngularUnits);
            this.chunk.Write(0x114, (short) style.FractionalType);
            this.chunk.Write(0x115, (short) style.DimLengthUnits);
            this.chunk.Write(0x116, (short) style.DecimalSeparator);
            this.chunk.Write(280, style.DIMJUST);
            if (style.DimLineOff)
            {
                this.chunk.Write(0x119, (short) 1);
                this.chunk.Write(0x11a, (short) 1);
            }
            else
            {
                this.chunk.Write(0x119, (short) 0);
                this.chunk.Write(0x11a, (short) 0);
            }
            this.chunk.Write(340, style.TextStyle.Handle);
            if (style.LeaderArrow > null)
            {
                this.chunk.Write(0x155, style.LeaderArrow.Record.Handle);
            }
            if ((style.DimArrow1 == null) && (style.DimArrow2 == null))
            {
                this.chunk.Write(0xad, (short) 0);
            }
            else if (style.DimArrow1 == null)
            {
                this.chunk.Write(0xad, (short) 1);
                if (style.DimArrow2 > null)
                {
                    this.chunk.Write(0x158, style.DimArrow2.Record.Handle);
                }
            }
            else if (style.DimArrow2 == null)
            {
                this.chunk.Write(0xad, (short) 1);
                if (style.DimArrow1 > null)
                {
                    this.chunk.Write(0x158, style.DimArrow1.Record.Handle);
                }
            }
            else if (string.Equals(style.DimArrow1.Name, style.DimArrow2.Name, StringComparison.OrdinalIgnoreCase))
            {
                this.chunk.Write(0xad, (short) 0);
                this.chunk.Write(0x156, style.DimArrow1.Record.Handle);
            }
            else
            {
                this.chunk.Write(0xad, (short) 1);
                this.chunk.Write(0x157, style.DimArrow1.Record.Handle);
                this.chunk.Write(0x158, style.DimArrow2.Record.Handle);
            }
            this.chunk.Write(0x159, style.DimLineLinetype.Handle);
            this.chunk.Write(0x15a, style.ExtLine1Linetype.Handle);
            this.chunk.Write(0x15b, style.ExtLine2Linetype.Handle);
            this.chunk.Write(0x173, (short) style.DimLineLineweight);
            this.chunk.Write(0x174, (short) style.ExtLineLineweight);
        }

        private void WriteEllipse(netDxf.Entities.Ellipse ellipse)
        {
            this.chunk.Write(100, "AcDbEllipse");
            this.chunk.Write(10, ellipse.Center.X);
            this.chunk.Write(20, ellipse.Center.Y);
            this.chunk.Write(30, ellipse.Center.Z);
            double y = (0.5 * ellipse.MajorAxis) * Math.Sin(ellipse.Rotation * 0.017453292519943295);
            double x = (0.5 * ellipse.MajorAxis) * Math.Cos(ellipse.Rotation * 0.017453292519943295);
            Vector3 vector = MathHelper.Transform(new Vector3(x, y, 0.0), ellipse.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.chunk.Write(11, vector.X);
            this.chunk.Write(0x15, vector.Y);
            this.chunk.Write(0x1f, vector.Z);
            this.chunk.Write(210, ellipse.Normal.X);
            this.chunk.Write(220, ellipse.Normal.Y);
            this.chunk.Write(230, ellipse.Normal.Z);
            this.chunk.Write(40, ellipse.MinorAxis / ellipse.MajorAxis);
            double[] ellipseParameters = GetEllipseParameters(ellipse);
            this.chunk.Write(0x29, ellipseParameters[0]);
            this.chunk.Write(0x2a, ellipseParameters[1]);
            this.WriteXData(ellipse.XData);
        }

        private void WriteEntity(EntityObject entity, Layout layout)
        {
            Debug.Assert((this.activeSection == "ENTITIES") || (this.activeSection == "BLOCKS"));
            Debug.Assert(entity > null);
            if ((((entity.Type != EntityType.Hatch) || (((Hatch) entity).BoundaryPaths.Count != 0)) && ((entity.Type != EntityType.Leader) || (((Leader) entity).Vertexes.Count >= 2))) && (((entity.Type != EntityType.Polyline) || (((netDxf.Entities.Polyline) entity).Vertexes.Count >= 2)) && ((entity.Type != EntityType.LightWeightPolyline) || (((LwPolyline) entity).Vertexes.Count >= 2))))
            {
                this.WriteEntityCommonCodes(entity, layout);
                switch (entity.Type)
                {
                    case EntityType.Arc:
                        this.WriteArc((netDxf.Entities.Arc) entity);
                        return;

                    case EntityType.AttributeDefinition:
                        this.WriteAttributeDefinition((AttributeDefinition) entity);
                        return;

                    case EntityType.Circle:
                        this.WriteCircle((Circle) entity);
                        return;

                    case EntityType.Dimension:
                        this.WriteDimension((Dimension) entity);
                        return;

                    case EntityType.Ellipse:
                        this.WriteEllipse((netDxf.Entities.Ellipse) entity);
                        return;

                    case EntityType.Face3D:
                        this.WriteFace3D((Face3d) entity);
                        return;

                    case EntityType.Hatch:
                        this.WriteHatch((Hatch) entity);
                        return;

                    case EntityType.Image:
                        this.WriteImage((Image) entity);
                        return;

                    case EntityType.Insert:
                        this.WriteInsert((Insert) entity);
                        return;

                    case EntityType.Leader:
                        this.WriteLeader((Leader) entity);
                        return;

                    case EntityType.LightWeightPolyline:
                        this.WriteLightWeightPolyline((LwPolyline) entity);
                        return;

                    case EntityType.Line:
                        this.WriteLine((netDxf.Entities.Line) entity);
                        return;

                    case EntityType.Mesh:
                        this.WriteMesh((Mesh) entity);
                        return;

                    case EntityType.MLine:
                        this.WriteMLine((MLine) entity);
                        return;

                    case EntityType.MText:
                        this.WriteMText((MText) entity);
                        return;

                    case EntityType.Point:
                        this.WritePoint((Point) entity);
                        return;

                    case EntityType.PolyfaceMesh:
                        this.WritePolyfaceMesh((PolyfaceMesh) entity);
                        return;

                    case EntityType.Polyline:
                        this.WritePolyline((netDxf.Entities.Polyline) entity);
                        return;

                    case EntityType.Ray:
                        this.WriteRay((Ray) entity);
                        return;

                    case EntityType.Solid:
                        this.WriteSolid((Solid) entity);
                        return;

                    case EntityType.Spline:
                        this.WriteSpline((netDxf.Entities.Spline) entity);
                        return;

                    case EntityType.Text:
                        this.WriteText((Text) entity);
                        return;

                    case EntityType.Tolerance:
                        this.WriteTolerance((Tolerance) entity);
                        return;

                    case EntityType.Trace:
                        this.WriteTrace((netDxf.Entities.Trace) entity);
                        return;

                    case EntityType.Underlay:
                        this.WriteUnderlay((Underlay) entity);
                        return;

                    case EntityType.Viewport:
                        this.WriteViewport((Viewport) entity);
                        return;

                    case EntityType.Wipeout:
                        this.WriteWipeout((Wipeout) entity);
                        return;

                    case EntityType.XLine:
                        this.WriteXLine((XLine) entity);
                        return;
                }
                throw new ArgumentException("Entity unknown.", "entity");
            }
        }

        private void WriteEntityCommonCodes(EntityObject entity, Layout layout)
        {
            this.chunk.Write(0, entity.CodeName);
            this.chunk.Write(5, entity.Handle);
            if (entity.Reactors.Count > 0)
            {
                this.chunk.Write(0x66, "{ACAD_REACTORS");
                foreach (DxfObject obj2 in entity.Reactors)
                {
                    this.chunk.Write(330, obj2.Handle);
                }
                this.chunk.Write(0x66, "}");
            }
            this.chunk.Write(330, entity.Owner.Record.Handle);
            this.chunk.Write(100, "AcDbEntity");
            if (layout > null)
            {
                this.chunk.Write(0x43, layout.IsPaperSpace ? ((short) 1) : ((short) 0));
            }
            this.chunk.Write(8, this.EncodeNonAsciiCharacters(entity.Layer.Name));
            this.chunk.Write(0x3e, entity.Color.Index);
            if (entity.Color.UseTrueColor)
            {
                this.chunk.Write(420, AciColor.ToTrueColor(entity.Color));
            }
            if (entity.Transparency.Value >= 0)
            {
                this.chunk.Write(440, Transparency.ToAlphaValue(entity.Transparency));
            }
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(entity.Linetype.Name));
            this.chunk.Write(370, (short) entity.Lineweight);
            this.chunk.Write(0x30, entity.LinetypeScale);
            this.chunk.Write(60, entity.IsVisible ? ((short) 0) : ((short) 1));
        }

        private void WriteFace3D(Face3d face)
        {
            this.chunk.Write(100, "AcDbFace");
            this.chunk.Write(10, face.FirstVertex.X);
            this.chunk.Write(20, face.FirstVertex.Y);
            this.chunk.Write(30, face.FirstVertex.Z);
            this.chunk.Write(11, face.SecondVertex.X);
            this.chunk.Write(0x15, face.SecondVertex.Y);
            this.chunk.Write(0x1f, face.SecondVertex.Z);
            this.chunk.Write(12, face.ThirdVertex.X);
            this.chunk.Write(0x16, face.ThirdVertex.Y);
            this.chunk.Write(0x20, face.ThirdVertex.Z);
            this.chunk.Write(13, face.FourthVertex.X);
            this.chunk.Write(0x17, face.FourthVertex.Y);
            this.chunk.Write(0x21, face.FourthVertex.Z);
            this.chunk.Write(70, (short) face.EdgeFlags);
            this.WriteXData(face.XData);
        }

        private void WriteGradientHatchPattern(HatchGradientPattern pattern)
        {
            this.chunk.Write(450, 1);
            this.chunk.Write(0x1c3, 0);
            this.chunk.Write(460, pattern.Angle * 0.017453292519943295);
            this.chunk.Write(0x1cd, pattern.Centered ? 0.0 : 1.0);
            this.chunk.Write(0x1c4, pattern.SingleColor ? 1 : 0);
            this.chunk.Write(0x1ce, pattern.Tint);
            this.chunk.Write(0x1c5, 2);
            this.chunk.Write(0x1cf, 0.0);
            this.chunk.Write(0x3f, pattern.Color1.Index);
            this.chunk.Write(0x1a5, AciColor.ToTrueColor(pattern.Color1));
            this.chunk.Write(0x1cf, 1.0);
            this.chunk.Write(0x3f, pattern.Color2.Index);
            this.chunk.Write(0x1a5, AciColor.ToTrueColor(pattern.Color2));
            this.chunk.Write(470, StringEnum.GetStringValue(pattern.GradientType));
        }

        private void WriteGroup(Group group, string ownerHandle)
        {
            this.chunk.Write(0, group.CodeName);
            this.chunk.Write(5, group.Handle);
            this.chunk.Write(330, ownerHandle);
            this.chunk.Write(100, "AcDbGroup");
            this.chunk.Write(300, this.EncodeNonAsciiCharacters(group.Description));
            this.chunk.Write(70, group.IsUnnamed ? ((short) 1) : ((short) 0));
            this.chunk.Write(0x47, group.IsSelectable ? ((short) 1) : ((short) 0));
            foreach (EntityObject obj2 in group.Entities)
            {
                this.chunk.Write(340, obj2.Handle);
            }
        }

        private void WriteHatch(Hatch hatch)
        {
            this.chunk.Write(100, "AcDbHatch");
            this.chunk.Write(10, 0.0);
            this.chunk.Write(20, 0.0);
            this.chunk.Write(30, hatch.Elevation);
            this.chunk.Write(210, hatch.Normal.X);
            this.chunk.Write(220, hatch.Normal.Y);
            this.chunk.Write(230, hatch.Normal.Z);
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(hatch.Pattern.Name));
            this.chunk.Write(70, (short) hatch.Pattern.Fill);
            if (hatch.Associative)
            {
                this.chunk.Write(0x47, (short) 1);
            }
            else
            {
                this.chunk.Write(0x47, (short) 0);
            }
            this.WriteHatchBoundaryPaths(hatch.BoundaryPaths);
            this.WriteHatchPattern(hatch.Pattern);
            AddHatchPatternXData(hatch);
            this.WriteXData(hatch.XData);
        }

        private void WriteHatchBoundaryPathData(HatchBoundaryPath.Edge entity)
        {
            if (entity.Type == HatchBoundaryPath.EdgeType.Arc)
            {
                this.chunk.Write(0x48, (short) 2);
                HatchBoundaryPath.Arc arc = (HatchBoundaryPath.Arc) entity;
                this.chunk.Write(10, arc.Center.X);
                this.chunk.Write(20, arc.Center.Y);
                this.chunk.Write(40, arc.Radius);
                this.chunk.Write(50, arc.StartAngle);
                this.chunk.Write(0x33, arc.EndAngle);
                this.chunk.Write(0x49, arc.IsCounterclockwise ? ((short) 1) : ((short) 0));
            }
            else if (entity.Type == HatchBoundaryPath.EdgeType.Ellipse)
            {
                this.chunk.Write(0x48, (short) 3);
                HatchBoundaryPath.Ellipse ellipse = (HatchBoundaryPath.Ellipse) entity;
                this.chunk.Write(10, ellipse.Center.X);
                this.chunk.Write(20, ellipse.Center.Y);
                this.chunk.Write(11, ellipse.EndMajorAxis.X);
                this.chunk.Write(0x15, ellipse.EndMajorAxis.Y);
                this.chunk.Write(40, ellipse.MinorRatio);
                this.chunk.Write(50, ellipse.StartAngle);
                this.chunk.Write(0x33, ellipse.EndAngle);
                this.chunk.Write(0x49, ellipse.IsCounterclockwise ? ((short) 1) : ((short) 0));
            }
            else if (entity.Type == HatchBoundaryPath.EdgeType.Line)
            {
                this.chunk.Write(0x48, (short) 1);
                HatchBoundaryPath.Line line = (HatchBoundaryPath.Line) entity;
                this.chunk.Write(10, line.Start.X);
                this.chunk.Write(20, line.Start.Y);
                this.chunk.Write(11, line.End.X);
                this.chunk.Write(0x15, line.End.Y);
            }
            else if (entity.Type == HatchBoundaryPath.EdgeType.Polyline)
            {
                HatchBoundaryPath.Polyline polyline = (HatchBoundaryPath.Polyline) entity;
                this.chunk.Write(0x48, (short) 1);
                this.chunk.Write(0x49, polyline.IsClosed ? ((short) 1) : ((short) 0));
                this.chunk.Write(0x5d, polyline.Vertexes.Length);
                foreach (Vector3 vector in polyline.Vertexes)
                {
                    this.chunk.Write(10, vector.X);
                    this.chunk.Write(20, vector.Y);
                    this.chunk.Write(0x2a, vector.Z);
                }
            }
            else if (entity.Type == HatchBoundaryPath.EdgeType.Spline)
            {
                this.chunk.Write(0x48, (short) 4);
                HatchBoundaryPath.Spline spline = (HatchBoundaryPath.Spline) entity;
                this.chunk.Write(0x5e, (int) spline.Degree);
                this.chunk.Write(0x49, spline.IsRational ? ((short) 1) : ((short) 0));
                this.chunk.Write(0x4a, spline.IsPeriodic ? ((short) 1) : ((short) 0));
                this.chunk.Write(0x5f, spline.Knots.Length);
                this.chunk.Write(0x60, spline.ControlPoints.Length);
                foreach (double num3 in spline.Knots)
                {
                    this.chunk.Write(40, num3);
                }
                foreach (Vector3 vector2 in spline.ControlPoints)
                {
                    this.chunk.Write(10, vector2.X);
                    this.chunk.Write(20, vector2.Y);
                    if (spline.IsRational)
                    {
                        this.chunk.Write(0x2a, vector2.Z);
                    }
                }
                if (this.doc.DrawingVariables.AcadVer >= DxfVersion.AutoCad2010)
                {
                    this.chunk.Write(0x61, 0);
                }
            }
        }

        private void WriteHatchBoundaryPaths(ObservableCollection<HatchBoundaryPath> boundaryPaths)
        {
            this.chunk.Write(0x5b, boundaryPaths.Count);
            foreach (HatchBoundaryPath path in boundaryPaths)
            {
                this.chunk.Write(0x5c, (int) path.PathType);
                if (!path.PathType.HasFlag(HatchBoundaryPathTypeFlags.Polyline))
                {
                    this.chunk.Write(0x5d, path.Edges.Count);
                }
                foreach (HatchBoundaryPath.Edge edge in path.Edges)
                {
                    this.WriteHatchBoundaryPathData(edge);
                }
                this.chunk.Write(0x61, path.Entities.Count);
                foreach (EntityObject obj2 in path.Entities)
                {
                    this.chunk.Write(330, obj2.Handle);
                }
            }
        }

        private void WriteHatchPattern(HatchPattern pattern)
        {
            this.chunk.Write(0x4b, (short) pattern.Style);
            this.chunk.Write(0x4c, (short) pattern.Type);
            if (pattern.Fill == HatchFillType.PatternFill)
            {
                this.chunk.Write(0x34, pattern.Angle);
                this.chunk.Write(0x29, pattern.Scale);
                this.chunk.Write(0x4d, (short) 0);
                this.chunk.Write(0x4e, (short) pattern.LineDefinitions.Count);
                this.WriteHatchPatternDefinitonLines(pattern);
            }
            this.chunk.Write(0x2f, 0.0);
            this.chunk.Write(0x62, 1);
            this.chunk.Write(10, 0.0);
            this.chunk.Write(20, 0.0);
            if (this.doc.DrawingVariables.AcadVer > DxfVersion.AutoCad2000)
            {
                HatchGradientPattern pattern2 = pattern as HatchGradientPattern;
                if (pattern2 > null)
                {
                    this.WriteGradientHatchPattern(pattern2);
                }
            }
        }

        private void WriteHatchPatternDefinitonLines(HatchPattern pattern)
        {
            foreach (HatchPatternLineDefinition definition in pattern.LineDefinitions)
            {
                double scale = pattern.Scale;
                double num2 = definition.Angle + pattern.Angle;
                this.chunk.Write(0x35, num2);
                double num3 = Math.Sin(pattern.Angle * 0.017453292519943295);
                double num4 = Math.Cos(pattern.Angle * 0.017453292519943295);
                Vector2 vector = new Vector2(((num4 * definition.Origin.X) * scale) - ((num3 * definition.Origin.Y) * scale), ((num3 * definition.Origin.X) * scale) + ((num4 * definition.Origin.Y) * scale));
                this.chunk.Write(0x2b, vector.X);
                this.chunk.Write(0x2c, vector.Y);
                double num5 = Math.Sin(num2 * 0.017453292519943295);
                double num6 = Math.Cos(num2 * 0.017453292519943295);
                Vector2 vector2 = new Vector2(((num6 * definition.Delta.X) * scale) - ((num5 * definition.Delta.Y) * scale), ((num5 * definition.Delta.X) * scale) + ((num6 * definition.Delta.Y) * scale));
                this.chunk.Write(0x2d, vector2.X);
                this.chunk.Write(0x2e, vector2.Y);
                this.chunk.Write(0x4f, (short) definition.DashPattern.Count);
                foreach (double num7 in definition.DashPattern)
                {
                    this.chunk.Write(0x31, num7 * scale);
                }
            }
        }

        private void WriteImage(Image image)
        {
            this.chunk.Write(100, "AcDbRasterImage");
            this.chunk.Write(10, image.Position.X);
            this.chunk.Write(20, image.Position.Y);
            this.chunk.Write(30, image.Position.Z);
            double num = UnitHelper.ConversionFactor(this.doc.RasterVariables.Units, this.doc.DrawingVariables.InsUnits);
            Vector2 vector = new Vector2(image.Width / ((double) image.Definition.Width), 0.0);
            Vector2 vector2 = new Vector2(0.0, image.Height / ((double) image.Definition.Height));
            List<Vector2> points = new List<Vector2> {
                vector,
                vector2
            };
            IList<Vector2> list = MathHelper.Transform(points, image.Rotation * 0.017453292519943295, CoordinateSystem.Object, CoordinateSystem.World);
            Vector2 vector8 = list[0];
            vector8 = list[0];
            Vector3 vector3 = new Vector3(vector8.X, vector8.Y, 0.0);
            vector8 = list[1];
            vector8 = list[1];
            Vector3 vector4 = new Vector3(vector8.X, vector8.Y, 0.0);
            List<Vector3> list3 = new List<Vector3> {
                vector3,
                vector4
            };
            IList<Vector3> list2 = MathHelper.Transform(list3, image.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3 vector5 = list2[0] * num;
            this.chunk.Write(11, vector5.X);
            this.chunk.Write(0x15, vector5.Y);
            this.chunk.Write(0x1f, vector5.Z);
            Vector3 vector6 = list2[1] * num;
            this.chunk.Write(12, vector6.X);
            this.chunk.Write(0x16, vector6.Y);
            this.chunk.Write(0x20, vector6.Z);
            this.chunk.Write(13, (double) image.Definition.Width);
            this.chunk.Write(0x17, (double) image.Definition.Height);
            this.chunk.Write(340, image.Definition.Handle);
            this.chunk.Write(70, (short) image.DisplayOptions);
            this.chunk.Write(280, image.Clipping ? ((short) 1) : ((short) 0));
            this.chunk.Write(0x119, image.Brightness);
            this.chunk.Write(0x11a, image.Contrast);
            this.chunk.Write(0x11b, image.Fade);
            this.chunk.Write(360, image.Definition.Reactors[image.Handle].Handle);
            this.chunk.Write(0x47, (short) image.ClippingBoundary.Type);
            this.chunk.Write(0x5b, image.ClippingBoundary.Vertexes.Count);
            foreach (Vector2 vector9 in image.ClippingBoundary.Vertexes)
            {
                this.chunk.Write(14, vector9.X / vector.X);
                this.chunk.Write(0x18, vector9.Y / vector2.Y);
            }
            this.WriteXData(image.XData);
        }

        private void WriteImageClass(int count)
        {
            this.chunk.Write(0, "CLASS");
            this.chunk.Write(1, "IMAGE");
            this.chunk.Write(2, "AcDbRasterImage");
            this.chunk.Write(3, "ISM");
            this.chunk.Write(90, 0x7f);
            if (this.doc.DrawingVariables.AcadVer > DxfVersion.AutoCad2000)
            {
                this.chunk.Write(0x5b, count);
            }
            this.chunk.Write(280, (short) 0);
            this.chunk.Write(0x119, (short) 1);
        }

        private void WriteImageDef(ImageDefinition imageDefinition, string ownerHandle)
        {
            this.chunk.Write(0, imageDefinition.CodeName);
            this.chunk.Write(5, imageDefinition.Handle);
            this.chunk.Write(0x66, "{ACAD_REACTORS");
            this.chunk.Write(330, ownerHandle);
            foreach (ImageDefinitionReactor reactor in imageDefinition.Reactors.Values)
            {
                this.chunk.Write(330, reactor.Handle);
            }
            this.chunk.Write(0x66, "}");
            this.chunk.Write(330, ownerHandle);
            this.chunk.Write(100, "AcDbRasterImageDef");
            this.chunk.Write(1, imageDefinition.FileName);
            this.chunk.Write(10, (double) imageDefinition.Width);
            this.chunk.Write(20, (double) imageDefinition.Height);
            double num = UnitHelper.ConversionFactor((ImageUnits) imageDefinition.ResolutionUnits, DrawingUnits.Millimeters);
            this.chunk.Write(11, num / imageDefinition.HorizontalResolution);
            this.chunk.Write(0x15, num / imageDefinition.VerticalResolution);
            this.chunk.Write(280, (short) 1);
            this.chunk.Write(0x119, (short) imageDefinition.ResolutionUnits);
        }

        private void WriteImageDefClass(int count)
        {
            this.chunk.Write(0, "CLASS");
            this.chunk.Write(1, "IMAGEDEF");
            this.chunk.Write(2, "AcDbRasterImageDef");
            this.chunk.Write(3, "ISM");
            this.chunk.Write(90, 0);
            if (this.doc.DrawingVariables.AcadVer > DxfVersion.AutoCad2000)
            {
                this.chunk.Write(0x5b, count);
            }
            this.chunk.Write(280, (short) 0);
            this.chunk.Write(0x119, (short) 0);
        }

        private void WriteImageDefReactor(ImageDefinitionReactor reactor)
        {
            this.chunk.Write(0, reactor.CodeName);
            this.chunk.Write(5, reactor.Handle);
            this.chunk.Write(330, reactor.ImageHandle);
            this.chunk.Write(100, "AcDbRasterImageDefReactor");
            this.chunk.Write(90, 2);
            this.chunk.Write(330, reactor.ImageHandle);
        }

        private void WriteImageDefRectorClass(int count)
        {
            this.chunk.Write(0, "CLASS");
            this.chunk.Write(1, "IMAGEDEF_REACTOR");
            this.chunk.Write(2, "AcDbRasterImageDefReactor");
            this.chunk.Write(3, "ISM");
            this.chunk.Write(90, 1);
            if (this.doc.DrawingVariables.AcadVer > DxfVersion.AutoCad2000)
            {
                this.chunk.Write(0x5b, count);
            }
            this.chunk.Write(280, (short) 0);
            this.chunk.Write(0x119, (short) 0);
        }

        private void WriteInsert(Insert insert)
        {
            this.chunk.Write(100, "AcDbBlockReference");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(insert.Block.Name));
            Vector3 vector = MathHelper.Transform(insert.Position, insert.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            double num = UnitHelper.ConversionFactor(insert.Block.Record.Units, insert.Owner.Record.IsForInternalUseOnly ? this.doc.DrawingVariables.InsUnits : insert.Owner.Record.Units);
            this.chunk.Write(0x29, insert.Scale.X * num);
            this.chunk.Write(0x2a, insert.Scale.Y * num);
            this.chunk.Write(0x2b, insert.Scale.Z * num);
            this.chunk.Write(50, insert.Rotation);
            this.chunk.Write(210, insert.Normal.X);
            this.chunk.Write(220, insert.Normal.Y);
            this.chunk.Write(230, insert.Normal.Z);
            if (insert.Attributes.Count > 0)
            {
                this.chunk.Write(0x42, (short) 1);
                this.WriteXData(insert.XData);
                foreach (netDxf.Entities.Attribute attribute in insert.Attributes)
                {
                    this.WriteAttribute(attribute);
                }
                this.chunk.Write(0, insert.EndSequence.CodeName);
                this.chunk.Write(5, insert.EndSequence.Handle);
                this.chunk.Write(100, "AcDbEntity");
                this.chunk.Write(8, this.EncodeNonAsciiCharacters(insert.Layer.Name));
            }
            else
            {
                this.WriteXData(insert.XData);
            }
        }

        private void WriteLayer(Layer layer)
        {
            Debug.Assert(this.activeTable == "LAYER");
            this.chunk.Write(0, layer.CodeName);
            this.chunk.Write(5, layer.Handle);
            this.chunk.Write(330, layer.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbLayerTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(layer.Name));
            LayerFlags none = LayerFlags.None;
            if (layer.IsFrozen)
            {
                none |= LayerFlags.Frozen;
            }
            if (layer.IsLocked)
            {
                none |= LayerFlags.Locked;
            }
            this.chunk.Write(70, (short) none);
            if (layer.IsVisible)
            {
                this.chunk.Write(0x3e, layer.Color.Index);
            }
            else
            {
                this.chunk.Write(0x3e, -layer.Color.Index);
            }
            if (layer.Color.UseTrueColor)
            {
                this.chunk.Write(420, AciColor.ToTrueColor(layer.Color));
            }
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(layer.Linetype.Name));
            this.chunk.Write(290, layer.Plot);
            this.chunk.Write(370, (short) layer.Lineweight);
            this.chunk.Write(390, "0");
            if (layer.Transparency.Value > 0)
            {
                int num = Transparency.ToAlphaValue(layer.Transparency);
                this.chunk.Write(0x3e9, "AcCmTransparency");
                this.chunk.Write(0x42f, num);
            }
        }

        private void WriteLayout(Layout layout, string ownerHandle)
        {
            this.chunk.Write(0, layout.CodeName);
            this.chunk.Write(5, layout.Handle);
            this.chunk.Write(330, ownerHandle);
            PlotSettings plotSettings = layout.PlotSettings;
            this.chunk.Write(100, "AcDbPlotSettings");
            this.chunk.Write(1, this.EncodeNonAsciiCharacters(plotSettings.PageSetupName));
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(plotSettings.PlotterName));
            this.chunk.Write(4, this.EncodeNonAsciiCharacters(plotSettings.PaperSizeName));
            this.chunk.Write(6, this.EncodeNonAsciiCharacters(plotSettings.ViewName));
            this.chunk.Write(40, plotSettings.LeftMargin);
            this.chunk.Write(0x29, plotSettings.BottomMargin);
            this.chunk.Write(0x2a, plotSettings.RightMargin);
            this.chunk.Write(0x2b, plotSettings.TopMargin);
            this.chunk.Write(0x2c, plotSettings.PaperSize.X);
            this.chunk.Write(0x2d, plotSettings.PaperSize.Y);
            this.chunk.Write(0x2e, plotSettings.Origin.X);
            this.chunk.Write(0x2f, plotSettings.Origin.Y);
            this.chunk.Write(0x30, plotSettings.WindowBottomLeft.X);
            this.chunk.Write(0x31, plotSettings.WindowUpRight.X);
            this.chunk.Write(140, plotSettings.WindowBottomLeft.Y);
            this.chunk.Write(0x8d, plotSettings.WindowUpRight.Y);
            this.chunk.Write(0x8e, plotSettings.PrintScaleNumerator);
            this.chunk.Write(0x8f, plotSettings.PrintScaleDenominator);
            this.chunk.Write(70, (short) plotSettings.Flags);
            this.chunk.Write(0x48, (short) plotSettings.PaperUnits);
            this.chunk.Write(0x49, (short) plotSettings.PaperRotation);
            this.chunk.Write(0x4a, (short) 5);
            this.chunk.Write(7, this.EncodeNonAsciiCharacters(plotSettings.CurrentStyleSheet));
            this.chunk.Write(0x4b, (short) 0x10);
            this.chunk.Write(0x93, plotSettings.PrintScale);
            this.chunk.Write(0x94, plotSettings.PaperImageOrigin.X);
            this.chunk.Write(0x95, plotSettings.PaperImageOrigin.Y);
            this.chunk.Write(100, "AcDbLayout");
            this.chunk.Write(1, this.EncodeNonAsciiCharacters(layout.Name));
            this.chunk.Write(70, (short) 1);
            this.chunk.Write(0x47, layout.TabOrder);
            this.chunk.Write(10, layout.MinLimit.X);
            this.chunk.Write(20, layout.MinLimit.Y);
            this.chunk.Write(11, layout.MaxLimit.X);
            this.chunk.Write(0x15, layout.MaxLimit.Y);
            this.chunk.Write(12, layout.BasePoint.X);
            this.chunk.Write(0x16, layout.BasePoint.Y);
            this.chunk.Write(0x20, layout.BasePoint.Z);
            this.chunk.Write(14, layout.MinExtents.X);
            this.chunk.Write(0x18, layout.MinExtents.Y);
            this.chunk.Write(0x22, layout.MinExtents.Z);
            this.chunk.Write(15, layout.MaxExtents.X);
            this.chunk.Write(0x19, layout.MaxExtents.Y);
            this.chunk.Write(0x23, layout.MaxExtents.Z);
            this.chunk.Write(0x92, layout.Elevation);
            this.chunk.Write(13, layout.UcsOrigin.X);
            this.chunk.Write(0x17, layout.UcsOrigin.Y);
            this.chunk.Write(0x21, layout.UcsOrigin.Z);
            this.chunk.Write(0x10, layout.UcsXAxis.X);
            this.chunk.Write(0x1a, layout.UcsXAxis.Y);
            this.chunk.Write(0x24, layout.UcsXAxis.Z);
            this.chunk.Write(0x11, layout.UcsYAxis.X);
            this.chunk.Write(0x1b, layout.UcsYAxis.Y);
            this.chunk.Write(0x25, layout.UcsYAxis.Z);
            this.chunk.Write(0x4c, (short) 0);
            this.chunk.Write(330, layout.AssociatedBlock.Owner.Handle);
        }

        private void WriteLeader(Leader leader)
        {
            this.chunk.Write(100, "AcDbLeader");
            this.chunk.Write(3, leader.Style.Name);
            if (leader.ShowArrowhead)
            {
                this.chunk.Write(0x47, (short) 1);
            }
            else
            {
                this.chunk.Write(0x47, (short) 0);
            }
            this.chunk.Write(0x48, (short) leader.PathType);
            if (leader.Annotation <= null)
            {
                this.chunk.Write(0x49, (short) 3);
            }
            else
            {
                EntityType type = leader.Annotation.Type;
                if (type != EntityType.Insert)
                {
                    if (type != EntityType.MText)
                    {
                        this.chunk.Write(0x49, (short) 3);
                    }
                    else
                    {
                        this.chunk.Write(0x49, (short) 0);
                    }
                }
                else
                {
                    this.chunk.Write(0x49, (short) 2);
                }
            }
            Vector2 vector = leader.Vertexes[leader.Vertexes.Count - 1] - leader.Vertexes[leader.Vertexes.Count - 2];
            if (vector.Equals(Vector2.Zero))
            {
                throw new Exception($"The last and previous vertex of the leader with handle {leader.Handle} cannot be the same");
            }
            int num = (vector.X < 0.0) ? -1 : 1;
            if (num < 0)
            {
                this.chunk.Write(0x4a, (short) 1);
            }
            else
            {
                this.chunk.Write(0x4a, (short) 0);
            }
            if (leader.HasHookline)
            {
                this.chunk.Write(0x4b, (short) 1);
            }
            else
            {
                this.chunk.Write(0x4b, (short) 0);
            }
            Vector2 vector2 = (Vector2) (num * Vector2.UnitX);
            List<Vector3> points = new List<Vector3>();
            if (leader.HasHookline)
            {
                double textOffset = leader.Style.TextOffset;
                if (leader.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out DimensionStyleOverride @override))
                {
                    textOffset = (double) @override.Value;
                }
                double dimScaleOverall = leader.Style.DimScaleOverall;
                if (leader.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out @override))
                {
                    dimScaleOverall = (double) @override.Value;
                }
                double arrowSize = leader.Style.ArrowSize;
                if (leader.StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out @override))
                {
                    arrowSize = (double) @override.Value;
                }
                Vector2 vector5 = leader.Vertexes[leader.Vertexes.Count - 1];
                MText annotation = leader.Annotation as MText;
                if (annotation > null)
                {
                    double a = annotation.Rotation * 0.017453292519943295;
                    double y = Math.Sin(a);
                    double x = Math.Cos(a);
                    vector2 = new Vector2(x, y);
                    Vector2 vector7 = vector5 + new Vector2((num * textOffset) * dimScaleOverall, textOffset * dimScaleOverall);
                    Vector2 point = vector5 - vector7;
                    vector5 = MathHelper.Transform(point, a, CoordinateSystem.Object, CoordinateSystem.World) + vector7;
                }
                Vector2 vector6 = vector5 + ((vector2 * arrowSize) * dimScaleOverall);
                for (int i = 0; i < (leader.Vertexes.Count - 1); i++)
                {
                    Vector2 vector9 = leader.Vertexes[i];
                    vector9 = leader.Vertexes[i];
                    points.Add(new Vector3(vector9.X, vector9.Y, leader.Elevation));
                }
                points.Add(new Vector3(vector6.X, vector6.Y, leader.Elevation));
                points.Add(new Vector3(vector5.X, vector5.Y, leader.Elevation));
            }
            else
            {
                foreach (Vector2 vector10 in leader.Vertexes)
                {
                    points.Add(new Vector3(vector10.X, vector10.Y, leader.Elevation));
                }
            }
            IList<Vector3> list2 = MathHelper.Transform(points, leader.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.chunk.Write(0x4c, (short) list2.Count);
            foreach (Vector3 vector11 in list2)
            {
                this.chunk.Write(10, vector11.X);
                this.chunk.Write(20, vector11.Y);
                this.chunk.Write(30, vector11.Z);
            }
            this.chunk.Write(0x4d, leader.LineColor.Index);
            if (leader.Annotation > null)
            {
                this.chunk.Write(340, leader.Annotation.Handle);
            }
            this.chunk.Write(210, leader.Normal.X);
            this.chunk.Write(220, leader.Normal.Y);
            this.chunk.Write(230, leader.Normal.Z);
            Vector3 vector3 = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, 0.0), leader.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            vector3.Normalize();
            this.chunk.Write(0xd3, vector3.X);
            this.chunk.Write(0xdd, vector3.Y);
            this.chunk.Write(0xe7, vector3.Z);
            Vector3 vector4 = MathHelper.Transform(new Vector3(leader.Offset.X, leader.Offset.Y, leader.Elevation), leader.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.chunk.Write(0xd5, vector4.X);
            this.chunk.Write(0xdf, vector4.Y);
            this.chunk.Write(0xe9, vector4.Z);
            if (leader.StyleOverrides.Count > 0)
            {
                this.AddDimensionStyleOverridesXData(leader.XData, leader.StyleOverrides, leader);
            }
            else
            {
                AddLeaderTextPositionXData(leader);
            }
            this.WriteXData(leader.XData);
        }

        private void WriteLightWeightPolyline(LwPolyline polyline)
        {
            this.chunk.Write(100, "AcDbPolyline");
            this.chunk.Write(90, polyline.Vertexes.Count);
            this.chunk.Write(70, (short) polyline.Flags);
            this.chunk.Write(0x26, polyline.Elevation);
            this.chunk.Write(0x27, polyline.Thickness);
            foreach (LwPolylineVertex vertex in polyline.Vertexes)
            {
                this.chunk.Write(10, vertex.Position.X);
                this.chunk.Write(20, vertex.Position.Y);
                this.chunk.Write(40, vertex.StartWidth);
                this.chunk.Write(0x29, vertex.EndWidth);
                this.chunk.Write(0x2a, vertex.Bulge);
            }
            this.chunk.Write(210, polyline.Normal.X);
            this.chunk.Write(220, polyline.Normal.Y);
            this.chunk.Write(230, polyline.Normal.Z);
            this.WriteXData(polyline.XData);
        }

        private void WriteLine(netDxf.Entities.Line line)
        {
            this.chunk.Write(100, "AcDbLine");
            this.chunk.Write(10, line.StartPoint.X);
            this.chunk.Write(20, line.StartPoint.Y);
            this.chunk.Write(30, line.StartPoint.Z);
            this.chunk.Write(11, line.EndPoint.X);
            this.chunk.Write(0x15, line.EndPoint.Y);
            this.chunk.Write(0x1f, line.EndPoint.Z);
            this.chunk.Write(0x27, line.Thickness);
            this.chunk.Write(210, line.Normal.X);
            this.chunk.Write(220, line.Normal.Y);
            this.chunk.Write(230, line.Normal.Z);
            this.WriteXData(line.XData);
        }

        private void WriteLinearDimension(LinearDimension dim)
        {
            this.chunk.Write(100, "AcDbAlignedDimension");
            Vector3[] points = new Vector3[] { new Vector3(dim.FirstReferencePoint.X, dim.FirstReferencePoint.Y, dim.Elevation), new Vector3(dim.SecondReferencePoint.X, dim.SecondReferencePoint.Y, dim.Elevation) };
            IList<Vector3> list = MathHelper.Transform(points, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3 vector2 = list[0];
            this.chunk.Write(13, vector2.X);
            vector2 = list[0];
            this.chunk.Write(0x17, vector2.Y);
            vector2 = list[0];
            this.chunk.Write(0x21, vector2.Z);
            vector2 = list[1];
            this.chunk.Write(14, vector2.X);
            vector2 = list[1];
            this.chunk.Write(0x18, vector2.Y);
            vector2 = list[1];
            this.chunk.Write(0x22, vector2.Z);
            this.chunk.Write(50, dim.Rotation);
            this.chunk.Write(100, "AcDbRotatedDimension");
            this.WriteXData(dim.XData);
        }

        private void WriteLinetype(Linetype tl)
        {
            Debug.Assert(this.activeTable == "LTYPE");
            this.chunk.Write(0, tl.CodeName);
            this.chunk.Write(5, tl.Handle);
            this.chunk.Write(330, tl.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbLinetypeTableRecord");
            this.chunk.Write(70, (short) 0);
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(tl.Name));
            this.chunk.Write(3, this.EncodeNonAsciiCharacters(tl.Description));
            this.chunk.Write(0x48, (short) 0x41);
            this.chunk.Write(0x49, (short) tl.Segments.Count);
            this.chunk.Write(40, tl.Length());
            foreach (double num in tl.Segments)
            {
                this.chunk.Write(0x31, num);
                this.chunk.Write(0x4a, (short) 0);
            }
        }

        private void WriteMesh(Mesh mesh)
        {
            this.chunk.Write(100, "AcDbSubDMesh");
            this.chunk.Write(0x47, (short) 2);
            this.chunk.Write(0x48, (short) 0);
            this.chunk.Write(0x5b, (int) mesh.SubdivisionLevel);
            this.chunk.Write(0x5c, mesh.Vertexes.Count);
            foreach (Vector3 vector in mesh.Vertexes)
            {
                this.chunk.Write(10, vector.X);
                this.chunk.Write(20, vector.Y);
                this.chunk.Write(30, vector.Z);
            }
            int count = mesh.Faces.Count;
            foreach (int[] numArray in mesh.Faces)
            {
                count += numArray.Length;
            }
            this.chunk.Write(0x5d, count);
            foreach (int[] numArray2 in mesh.Faces)
            {
                this.chunk.Write(90, numArray2.Length);
                foreach (int num3 in numArray2)
                {
                    this.chunk.Write(90, num3);
                }
            }
            if (mesh.Edges > null)
            {
                this.chunk.Write(0x5e, mesh.Edges.Count);
                foreach (MeshEdge edge in mesh.Edges)
                {
                    this.chunk.Write(90, edge.StartVertexIndex);
                    this.chunk.Write(90, edge.EndVertexIndex);
                }
                this.chunk.Write(0x5f, mesh.Edges.Count);
                foreach (MeshEdge edge2 in mesh.Edges)
                {
                    this.chunk.Write(140, edge2.Crease);
                }
            }
            this.chunk.Write(90, 0);
            this.WriteXData(mesh.XData);
        }

        private void WriteMLine(MLine mLine)
        {
            this.chunk.Write(100, "AcDbMline");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(mLine.Style.Name));
            this.chunk.Write(340, mLine.Style.Handle);
            this.chunk.Write(40, mLine.Scale);
            this.chunk.Write(70, (short) mLine.Justification);
            this.chunk.Write(0x47, (short) mLine.Flags);
            this.chunk.Write(0x48, (short) mLine.Vertexes.Count);
            this.chunk.Write(0x49, (short) mLine.Style.Elements.Count);
            List<Vector3> points = new List<Vector3>();
            foreach (MLineVertex vertex in mLine.Vertexes)
            {
                points.Add(new Vector3(vertex.Location.X, vertex.Location.Y, mLine.Elevation));
            }
            IList<Vector3> list2 = MathHelper.Transform(points, mLine.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3[] array = new Vector3[list2.Count];
            list2.CopyTo(array, 0);
            if (array.Length == 0)
            {
                this.chunk.Write(10, 0.0);
                this.chunk.Write(20, 0.0);
                this.chunk.Write(30, 0.0);
            }
            else
            {
                this.chunk.Write(10, array[0].X);
                this.chunk.Write(20, array[0].Y);
                this.chunk.Write(30, array[0].Z);
            }
            this.chunk.Write(210, mLine.Normal.X);
            this.chunk.Write(220, mLine.Normal.Y);
            this.chunk.Write(230, mLine.Normal.Z);
            for (int i = 0; i < array.Length; i++)
            {
                this.chunk.Write(11, array[i].X);
                this.chunk.Write(0x15, array[i].Y);
                this.chunk.Write(0x1f, array[i].Z);
                Vector2 direction = mLine.Vertexes[i].Direction;
                Vector3 vector4 = MathHelper.Transform(new Vector3(direction.X, direction.Y, mLine.Elevation), mLine.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                this.chunk.Write(12, vector4.X);
                this.chunk.Write(0x16, vector4.Y);
                this.chunk.Write(0x20, vector4.Z);
                Vector2 miter = mLine.Vertexes[i].Miter;
                Vector3 vector6 = MathHelper.Transform(new Vector3(miter.X, miter.Y, mLine.Elevation), mLine.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                this.chunk.Write(13, vector6.X);
                this.chunk.Write(0x17, vector6.Y);
                this.chunk.Write(0x21, vector6.Z);
                foreach (List<double> list3 in mLine.Vertexes[i].Distances)
                {
                    this.chunk.Write(0x4a, (short) list3.Count);
                    foreach (double num3 in list3)
                    {
                        this.chunk.Write(0x29, num3);
                    }
                    this.chunk.Write(0x4b, (short) 0);
                }
            }
            this.WriteXData(mLine.XData);
        }

        private void WriteMLineStyle(MLineStyle style, string ownerHandle)
        {
            this.chunk.Write(0, style.CodeName);
            this.chunk.Write(5, style.Handle);
            this.chunk.Write(330, ownerHandle);
            this.chunk.Write(100, "AcDbMlineStyle");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(style.Name));
            this.chunk.Write(70, (short) style.Flags);
            this.chunk.Write(3, this.EncodeNonAsciiCharacters(style.Description));
            this.chunk.Write(0x3e, style.FillColor.Index);
            if (style.FillColor.UseTrueColor)
            {
                this.chunk.Write(420, AciColor.ToTrueColor(style.FillColor));
            }
            this.chunk.Write(0x33, style.StartAngle);
            this.chunk.Write(0x34, style.EndAngle);
            this.chunk.Write(0x47, (short) style.Elements.Count);
            foreach (MLineStyleElement element in style.Elements)
            {
                this.chunk.Write(0x31, element.Offset);
                this.chunk.Write(0x3e, element.Color.Index);
                if (element.Color.UseTrueColor)
                {
                    this.chunk.Write(420, AciColor.ToTrueColor(element.Color));
                }
                this.chunk.Write(6, this.EncodeNonAsciiCharacters(element.Linetype.Name));
            }
        }

        private void WriteMText(MText mText)
        {
            this.chunk.Write(100, "AcDbMText");
            this.chunk.Write(10, mText.Position.X);
            this.chunk.Write(20, mText.Position.Y);
            this.chunk.Write(30, mText.Position.Z);
            this.chunk.Write(210, mText.Normal.X);
            this.chunk.Write(220, mText.Normal.Y);
            this.chunk.Write(230, mText.Normal.Z);
            this.WriteMTextChunks(this.EncodeNonAsciiCharacters(mText.Value));
            this.chunk.Write(40, mText.Height);
            this.chunk.Write(0x29, mText.RectangleWidth);
            this.chunk.Write(0x2c, mText.LineSpacingFactor);
            this.chunk.Write(50, mText.Rotation);
            this.chunk.Write(0x47, (short) mText.AttachmentPoint);
            this.chunk.Write(0x48, (short) 5);
            this.chunk.Write(7, this.EncodeNonAsciiCharacters(mText.Style.Name));
            this.WriteXData(mText.XData);
        }

        private void WriteMTextChunks(string text)
        {
            while (text.Length > 250)
            {
                string str = text.Substring(0, 250);
                this.chunk.Write(3, str);
                text = text.Remove(0, 250);
            }
            this.chunk.Write(1, text);
        }

        private void WriteOrdinateDimension(OrdinateDimension dim)
        {
            this.chunk.Write(100, "AcDbOrdinateDimension");
            this.chunk.Write(13, dim.FirstPoint.X);
            this.chunk.Write(0x17, dim.FirstPoint.Y);
            this.chunk.Write(0x21, dim.FirstPoint.Z);
            this.chunk.Write(14, dim.SecondPoint.X);
            this.chunk.Write(0x18, dim.SecondPoint.Y);
            this.chunk.Write(0x22, dim.SecondPoint.Z);
            this.WriteXData(dim.XData);
        }

        private void WritePoint(Point point)
        {
            this.chunk.Write(100, "AcDbPoint");
            this.chunk.Write(10, point.Position.X);
            this.chunk.Write(20, point.Position.Y);
            this.chunk.Write(30, point.Position.Z);
            this.chunk.Write(0x27, point.Thickness);
            this.chunk.Write(210, point.Normal.X);
            this.chunk.Write(220, point.Normal.Y);
            this.chunk.Write(230, point.Normal.Z);
            this.chunk.Write(50, 360.0 - point.Rotation);
            this.WriteXData(point.XData);
        }

        private void WritePolyfaceMesh(PolyfaceMesh mesh)
        {
            this.chunk.Write(100, "AcDbPolyFaceMesh");
            this.chunk.Write(70, (short) mesh.Flags);
            this.chunk.Write(0x47, (short) mesh.Vertexes.Count);
            this.chunk.Write(0x48, (short) mesh.Faces.Count);
            this.chunk.Write(10, 0.0);
            this.chunk.Write(20, 0.0);
            this.chunk.Write(30, 0.0);
            this.chunk.Write(210, mesh.Normal.X);
            this.chunk.Write(220, mesh.Normal.Y);
            this.chunk.Write(230, mesh.Normal.Z);
            if (mesh.XData > null)
            {
                this.WriteXData(mesh.XData);
            }
            string str = this.EncodeNonAsciiCharacters(mesh.Layer.Name);
            foreach (PolyfaceMeshVertex vertex in mesh.Vertexes)
            {
                this.chunk.Write(0, vertex.CodeName);
                this.chunk.Write(5, vertex.Handle);
                this.chunk.Write(100, "AcDbEntity");
                this.chunk.Write(8, str);
                this.chunk.Write(0x3e, mesh.Color.Index);
                if (mesh.Color.UseTrueColor)
                {
                    this.chunk.Write(420, AciColor.ToTrueColor(mesh.Color));
                }
                this.chunk.Write(100, "AcDbVertex");
                this.chunk.Write(100, "AcDbPolyFaceMeshVertex");
                this.chunk.Write(70, (short) vertex.Flags);
                this.chunk.Write(10, vertex.Location.X);
                this.chunk.Write(20, vertex.Location.Y);
                this.chunk.Write(30, vertex.Location.Z);
            }
            foreach (PolyfaceMeshFace face in mesh.Faces)
            {
                this.chunk.Write(0, face.CodeName);
                this.chunk.Write(5, face.Handle);
                this.chunk.Write(100, "AcDbEntity");
                this.chunk.Write(8, str);
                this.chunk.Write(0x3e, mesh.Color.Index);
                if (mesh.Color.UseTrueColor)
                {
                    this.chunk.Write(420, AciColor.ToTrueColor(mesh.Color));
                }
                this.chunk.Write(100, "AcDbFaceRecord");
                this.chunk.Write(70, (short) 0x80);
                this.chunk.Write(10, 0.0);
                this.chunk.Write(20, 0.0);
                this.chunk.Write(30, 0.0);
                this.chunk.Write(0x47, face.VertexIndexes[0]);
                if (face.VertexIndexes.Count > 1)
                {
                    this.chunk.Write(0x48, face.VertexIndexes[1]);
                }
                if (face.VertexIndexes.Count > 2)
                {
                    this.chunk.Write(0x49, face.VertexIndexes[2]);
                }
                if (face.VertexIndexes.Count > 3)
                {
                    this.chunk.Write(0x4a, face.VertexIndexes[3]);
                }
            }
            this.chunk.Write(0, mesh.EndSequence.CodeName);
            this.chunk.Write(5, mesh.EndSequence.Handle);
            this.chunk.Write(100, "AcDbEntity");
            this.chunk.Write(8, str);
        }

        private void WritePolyline(netDxf.Entities.Polyline polyline)
        {
            this.chunk.Write(100, "AcDb3dPolyline");
            this.chunk.Write(10, 0.0);
            this.chunk.Write(20, 0.0);
            this.chunk.Write(30, 0.0);
            this.chunk.Write(70, (short) polyline.Flags);
            this.chunk.Write(0x4b, (short) polyline.SmoothType);
            this.chunk.Write(210, polyline.Normal.X);
            this.chunk.Write(220, polyline.Normal.Y);
            this.chunk.Write(230, polyline.Normal.Z);
            this.WriteXData(polyline.XData);
            string str = this.EncodeNonAsciiCharacters(polyline.Layer.Name);
            foreach (PolylineVertex vertex in polyline.Vertexes)
            {
                this.chunk.Write(0, vertex.CodeName);
                this.chunk.Write(5, vertex.Handle);
                this.chunk.Write(100, "AcDbEntity");
                this.chunk.Write(8, str);
                this.chunk.Write(0x3e, polyline.Color.Index);
                if (polyline.Color.UseTrueColor)
                {
                    this.chunk.Write(420, AciColor.ToTrueColor(polyline.Color));
                }
                this.chunk.Write(100, "AcDbVertex");
                this.chunk.Write(100, "AcDb3dPolylineVertex");
                this.chunk.Write(10, vertex.Position.X);
                this.chunk.Write(20, vertex.Position.Y);
                this.chunk.Write(30, vertex.Position.Z);
                this.chunk.Write(70, (short) vertex.Flags);
            }
            this.chunk.Write(0, polyline.EndSequence.CodeName);
            this.chunk.Write(5, polyline.EndSequence.Handle);
            this.chunk.Write(100, "AcDbEntity");
            this.chunk.Write(8, str);
        }

        private void WriteRadialDimension(RadialDimension dim)
        {
            this.chunk.Write(100, "AcDbRadialDimension");
            Vector3 vector = MathHelper.Transform(new Vector3(dim.ReferencePoint.X, dim.ReferencePoint.Y, dim.Elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.chunk.Write(15, vector.X);
            this.chunk.Write(0x19, vector.Y);
            this.chunk.Write(0x23, vector.Z);
            this.chunk.Write(40, 0.0);
            this.WriteXData(dim.XData);
        }

        private void WriteRasterVariables(RasterVariables variables, string ownerHandle)
        {
            this.chunk.Write(0, variables.CodeName);
            this.chunk.Write(5, variables.Handle);
            this.chunk.Write(330, ownerHandle);
            this.chunk.Write(100, "AcDbRasterVariables");
            this.chunk.Write(90, 0);
            this.chunk.Write(70, variables.DisplayFrame ? ((short) 1) : ((short) 0));
            this.chunk.Write(0x47, (short) variables.DisplayQuality);
            this.chunk.Write(0x48, (short) variables.Units);
        }

        private void WriteRasterVariablesClass(int count)
        {
            this.chunk.Write(0, "CLASS");
            this.chunk.Write(1, "RASTERVARIABLES");
            this.chunk.Write(2, "AcDbRasterVariables");
            this.chunk.Write(3, "ISM");
            this.chunk.Write(90, 0);
            if (this.doc.DrawingVariables.AcadVer > DxfVersion.AutoCad2000)
            {
                this.chunk.Write(0x5b, count);
            }
            this.chunk.Write(280, (short) 0);
            this.chunk.Write(0x119, (short) 0);
        }

        private void WriteRay(Ray ray)
        {
            this.chunk.Write(100, "AcDbRay");
            this.chunk.Write(10, ray.Origin.X);
            this.chunk.Write(20, ray.Origin.Y);
            this.chunk.Write(30, ray.Origin.Z);
            this.chunk.Write(11, ray.Direction.X);
            this.chunk.Write(0x15, ray.Direction.Y);
            this.chunk.Write(0x1f, ray.Direction.Z);
            this.WriteXData(ray.XData);
        }

        private void WriteSolid(Solid solid)
        {
            this.chunk.Write(100, "AcDbTrace");
            this.chunk.Write(10, solid.FirstVertex.X);
            this.chunk.Write(20, solid.FirstVertex.Y);
            this.chunk.Write(30, solid.Elevation);
            this.chunk.Write(11, solid.SecondVertex.X);
            this.chunk.Write(0x15, solid.SecondVertex.Y);
            this.chunk.Write(0x1f, solid.Elevation);
            this.chunk.Write(12, solid.ThirdVertex.X);
            this.chunk.Write(0x16, solid.ThirdVertex.Y);
            this.chunk.Write(0x20, solid.Elevation);
            this.chunk.Write(13, solid.FourthVertex.X);
            this.chunk.Write(0x17, solid.FourthVertex.Y);
            this.chunk.Write(0x21, solid.Elevation);
            this.chunk.Write(0x27, solid.Thickness);
            this.chunk.Write(210, solid.Normal.X);
            this.chunk.Write(220, solid.Normal.Y);
            this.chunk.Write(230, solid.Normal.Z);
            this.WriteXData(solid.XData);
        }

        private void WriteSpline(netDxf.Entities.Spline spline)
        {
            this.chunk.Write(100, "AcDbSpline");
            short flags = (short) spline.Flags;
            if (spline.CreationMethod == SplineCreationMethod.FitPoints)
            {
                flags = (short) (flags + 0x400);
                flags = (short) (flags + ((short) spline.KnotParameterization));
            }
            if (spline.IsPeriodic)
            {
                flags = (short) (flags + 0x800);
            }
            this.chunk.Write(70, flags);
            this.chunk.Write(0x47, spline.Degree);
            this.chunk.Write(0x2a, spline.KnotTolerance);
            this.chunk.Write(0x2b, spline.CtrlPointTolerance);
            this.chunk.Write(0x2c, spline.FitTolerance);
            if (spline.StartTangent.HasValue)
            {
                this.chunk.Write(12, spline.StartTangent.Value.X);
                this.chunk.Write(0x16, spline.StartTangent.Value.Y);
                this.chunk.Write(0x20, spline.StartTangent.Value.Z);
            }
            if (spline.EndTangent.HasValue)
            {
                this.chunk.Write(13, spline.EndTangent.Value.X);
                this.chunk.Write(0x17, spline.EndTangent.Value.Y);
                this.chunk.Write(0x21, spline.EndTangent.Value.Z);
            }
            foreach (double num2 in spline.Knots)
            {
                this.chunk.Write(40, num2);
            }
            foreach (SplineVertex vertex in spline.ControlPoints)
            {
                this.chunk.Write(0x29, vertex.Weigth);
                this.chunk.Write(10, vertex.Position.X);
                this.chunk.Write(20, vertex.Position.Y);
                this.chunk.Write(30, vertex.Position.Z);
            }
            foreach (Vector3 vector2 in spline.FitPoints)
            {
                this.chunk.Write(11, vector2.X);
                this.chunk.Write(0x15, vector2.Y);
                this.chunk.Write(0x1f, vector2.Z);
            }
            this.WriteXData(spline.XData);
        }

        private void WriteSystemVariable(HeaderVariable variable)
        {
            Debug.Assert(this.activeSection == "HEADER");
            string name = variable.Name;
            object obj2 = variable.Value;
            string s = name;
            switch (<PrivateImplementationDetails>.ComputeStringHash(s))
            {
                case 0x18b2ca03:
                    if (s == "$TDCREATE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, DrawingTime.ToJulianCalendar((DateTime) obj2));
                        break;
                    }
                    break;

                case 0x19830bb2:
                    if (s == "$TDINDWG")
                    {
                        this.chunk.Write(9, name);
                        TimeSpan span = (TimeSpan) obj2;
                        this.chunk.Write(40, span.TotalDays);
                        break;
                    }
                    break;

                case 0x139e695f:
                    if (s == "$PLINEGEN")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, obj2);
                        break;
                    }
                    break;

                case 0x17e7e534:
                    if (s == "$INSBASE")
                    {
                        this.chunk.Write(9, name);
                        Vector3 vector = (Vector3) obj2;
                        this.chunk.Write(10, vector.X);
                        this.chunk.Write(20, vector.Y);
                        this.chunk.Write(30, vector.Z);
                        break;
                    }
                    break;

                case 0x2287ee19:
                    if (s == "$TEXTSTYLE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(7, this.EncodeNonAsciiCharacters((string) obj2));
                        break;
                    }
                    break;

                case 0x230694c0:
                    if (s == "$PDMODE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((PointShape) obj2));
                        break;
                    }
                    break;

                case 0x399750cb:
                    if (s == "$TDUUPDATE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, DrawingTime.ToJulianCalendar((DateTime) obj2));
                        break;
                    }
                    break;

                case 0x4508691c:
                    if (s == "$LWDISPLAY")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(290, obj2);
                        break;
                    }
                    break;

                case 0x4cb5581a:
                    if (s == "$EXTNAMES")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(290, obj2);
                        break;
                    }
                    break;

                case 0x67783fd4:
                    if (s == "$LUNITS")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((LinearUnitType) obj2));
                        break;
                    }
                    break;

                case 0x68822717:
                    if (s == "$LASTSAVEDBY")
                    {
                        if (this.doc.DrawingVariables.AcadVer > DxfVersion.AutoCad2000)
                        {
                            this.chunk.Write(9, name);
                            this.chunk.Write(1, this.EncodeNonAsciiCharacters((string) obj2));
                        }
                        break;
                    }
                    break;

                case 0x4dc8306c:
                    if (s == "$ANGDIR")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((AngleDirection) obj2));
                        break;
                    }
                    break;

                case 0x64449477:
                    if (s == "$CELTSCALE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, obj2);
                        break;
                    }
                    break;

                case 0x6e822850:
                    if (s == "$ANGBASE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(50, obj2);
                        break;
                    }
                    break;

                case 0x7323e22d:
                    if (s == "$CELWEIGHT")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(370, (short) ((Lineweight) obj2));
                        break;
                    }
                    break;

                case 0x7339f76b:
                    if (s == "$HANDSEED")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(5, obj2);
                        break;
                    }
                    break;

                case 0x7d87d97e:
                    if (s == "$PDSIZE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, obj2);
                        break;
                    }
                    break;

                case 0x81e80a53:
                    if (s == "$LTSCALE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, obj2);
                        break;
                    }
                    break;

                case 0x8f8c4beb:
                    if (s == "$CMLJUST")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((MLineJustification) obj2));
                        break;
                    }
                    break;

                case 0xa14edb5b:
                    if (s == "$AUPREC")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, obj2);
                        break;
                    }
                    break;

                case 0x83a39e24:
                    if (s == "$PSLTSCALE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, obj2);
                        break;
                    }
                    break;

                case 0x8f10a26d:
                    if (s == "$TEXTSIZE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, obj2);
                        break;
                    }
                    break;

                case 0xb08bc155:
                    if (s == "$CLAYER")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(8, this.EncodeNonAsciiCharacters((string) obj2));
                        break;
                    }
                    break;

                case 0xbebec070:
                    if (s == "$INSUNITS")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((DrawingUnits) obj2));
                        break;
                    }
                    break;

                case 0xc20767f2:
                    if (s == "$CMLSTYLE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(2, this.EncodeNonAsciiCharacters((string) obj2));
                        break;
                    }
                    break;

                case 0xc6f4a03e:
                    if (s == "$CECOLOR")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(0x3e, ((AciColor) obj2).Index);
                        break;
                    }
                    break;

                case 0xc7ea1c31:
                    if (s == "$DWGCODEPAGE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(3, obj2);
                        break;
                    }
                    break;

                case 0xd71d7b56:
                    if (s == "$LUPREC")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, obj2);
                        break;
                    }
                    break;

                case 0xdb4507c5:
                    if (s == "$CMLSCALE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, obj2);
                        break;
                    }
                    break;

                case 0xc9d72786:
                    if (s == "$TDUCREATE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, DrawingTime.ToJulianCalendar((DateTime) obj2));
                        break;
                    }
                    break;

                case 0xd29d199d:
                    if (s == "$ATTMODE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((AttMode) obj2));
                        break;
                    }
                    break;

                case 0xdde43571:
                    if (s == "$CELTYPE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(6, this.EncodeNonAsciiCharacters((string) obj2));
                        break;
                    }
                    break;

                case 0xe4731d72:
                    if (s == "$TDUPDATE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(40, DrawingTime.ToJulianCalendar((DateTime) obj2));
                        break;
                    }
                    break;

                case 0xf3f552c9:
                    if (s == "$AUNITS")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(70, (short) ((AngleUnitType) obj2));
                        break;
                    }
                    break;

                case 0xf4c3fde3:
                    if (s == "$ACADVER")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(1, StringEnum.GetStringValue((DxfVersion) obj2));
                        break;
                    }
                    break;

                case 0xfbaa84bc:
                    if (s == "$DIMSTYLE")
                    {
                        this.chunk.Write(9, name);
                        this.chunk.Write(2, this.EncodeNonAsciiCharacters((string) obj2));
                        break;
                    }
                    break;
            }
        }

        private void WriteText(Text text)
        {
            this.chunk.Write(100, "AcDbText");
            this.chunk.Write(1, this.EncodeNonAsciiCharacters(text.Value));
            Vector3 vector = MathHelper.Transform(text.Position, text.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            this.chunk.Write(40, text.Height);
            this.chunk.Write(0x29, text.WidthFactor);
            this.chunk.Write(50, text.Rotation);
            this.chunk.Write(0x33, text.ObliqueAngle);
            this.chunk.Write(7, this.EncodeNonAsciiCharacters(text.Style.Name));
            this.chunk.Write(11, vector.X);
            this.chunk.Write(0x15, vector.Y);
            this.chunk.Write(0x1f, vector.Z);
            this.chunk.Write(210, text.Normal.X);
            this.chunk.Write(220, text.Normal.Y);
            this.chunk.Write(230, text.Normal.Z);
            switch (text.Alignment)
            {
                case netDxf.Entities.TextAlignment.TopLeft:
                    this.chunk.Write(0x48, (short) 0);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.TopCenter:
                    this.chunk.Write(0x48, (short) 1);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.TopRight:
                    this.chunk.Write(0x48, (short) 2);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 3);
                    break;

                case netDxf.Entities.TextAlignment.MiddleLeft:
                    this.chunk.Write(0x48, (short) 0);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleCenter:
                    this.chunk.Write(0x48, (short) 1);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.MiddleRight:
                    this.chunk.Write(0x48, (short) 2);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 2);
                    break;

                case netDxf.Entities.TextAlignment.BottomLeft:
                    this.chunk.Write(0x48, (short) 0);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomCenter:
                    this.chunk.Write(0x48, (short) 1);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BottomRight:
                    this.chunk.Write(0x48, (short) 2);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 1);
                    break;

                case netDxf.Entities.TextAlignment.BaselineLeft:
                    this.chunk.Write(0x48, (short) 0);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineCenter:
                    this.chunk.Write(0x48, (short) 1);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.BaselineRight:
                    this.chunk.Write(0x48, (short) 2);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Aligned:
                    this.chunk.Write(0x48, (short) 3);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Middle:
                    this.chunk.Write(0x48, (short) 4);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 0);
                    break;

                case netDxf.Entities.TextAlignment.Fit:
                    this.chunk.Write(0x48, (short) 5);
                    this.chunk.Write(100, "AcDbText");
                    this.chunk.Write(0x49, (short) 0);
                    break;
            }
            this.WriteXData(text.XData);
        }

        private void WriteTextStyle(TextStyle style)
        {
            Debug.Assert(this.activeTable == "STYLE");
            this.chunk.Write(0, style.CodeName);
            this.chunk.Write(5, style.Handle);
            this.chunk.Write(330, style.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbTextStyleTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(style.Name));
            this.chunk.Write(3, this.EncodeNonAsciiCharacters(style.FontFile));
            if (!string.IsNullOrEmpty(style.BigFont))
            {
                this.chunk.Write(4, this.EncodeNonAsciiCharacters(style.BigFont));
            }
            this.chunk.Write(70, style.IsVertical ? ((short) 4) : ((short) 0));
            if (style.IsBackward && style.IsUpsideDown)
            {
                this.chunk.Write(0x47, (short) 6);
            }
            else if (style.IsBackward)
            {
                this.chunk.Write(0x47, (short) 2);
            }
            else if (style.IsUpsideDown)
            {
                this.chunk.Write(0x47, (short) 4);
            }
            else
            {
                this.chunk.Write(0x47, (short) 0);
            }
            this.chunk.Write(40, style.Height);
            this.chunk.Write(0x29, style.WidthFactor);
            this.chunk.Write(0x2a, style.Height);
            this.chunk.Write(50, style.ObliqueAngle);
            if (style.GlyphTypeface != null)
            {
                this.chunk.Write(0x3e9, "ACAD");
                this.chunk.Write(0x3e8, this.EncodeNonAsciiCharacters(style.FontFamilyName));
                byte[] buffer = new byte[4];
                if ((style.GlyphTypeface.Style == FontStyles.Italic) || (style.GlyphTypeface.Style == FontStyles.Oblique))
                {
                    buffer[3] = (byte) (buffer[3] + 1);
                }
                if (style.GlyphTypeface.Weight == FontWeights.Bold)
                {
                    buffer[3] = (byte) (buffer[3] + 2);
                }
                this.chunk.Write(0x42f, BitConverter.ToInt32(buffer, 0));
            }
        }

        private void WriteTolerance(Tolerance tolerance)
        {
            this.chunk.Write(100, "AcDbFcf");
            this.chunk.Write(3, this.EncodeNonAsciiCharacters(tolerance.Style.Name));
            this.chunk.Write(10, tolerance.Position.X);
            this.chunk.Write(20, tolerance.Position.Y);
            this.chunk.Write(30, tolerance.Position.Z);
            string text = tolerance.ToStringRepresentation();
            this.chunk.Write(1, this.EncodeNonAsciiCharacters(text));
            this.chunk.Write(210, tolerance.Normal.X);
            this.chunk.Write(220, tolerance.Normal.Y);
            this.chunk.Write(230, tolerance.Normal.Z);
            double d = tolerance.Rotation * 0.017453292519943295;
            Vector3 point = new Vector3(Math.Cos(d), Math.Sin(d), 0.0);
            point = MathHelper.Transform(point, tolerance.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            this.chunk.Write(11, point.X);
            this.chunk.Write(0x15, point.Y);
            this.chunk.Write(0x1f, point.Z);
        }

        private void WriteTrace(netDxf.Entities.Trace trace)
        {
            this.chunk.Write(100, "AcDbTrace");
            this.chunk.Write(10, trace.FirstVertex.X);
            this.chunk.Write(20, trace.FirstVertex.Y);
            this.chunk.Write(30, trace.Elevation);
            this.chunk.Write(11, trace.SecondVertex.X);
            this.chunk.Write(0x15, trace.SecondVertex.Y);
            this.chunk.Write(0x1f, trace.Elevation);
            this.chunk.Write(12, trace.ThirdVertex.X);
            this.chunk.Write(0x16, trace.ThirdVertex.Y);
            this.chunk.Write(0x20, trace.Elevation);
            this.chunk.Write(13, trace.FourthVertex.X);
            this.chunk.Write(0x17, trace.FourthVertex.Y);
            this.chunk.Write(0x21, trace.Elevation);
            this.chunk.Write(0x27, trace.Thickness);
            this.chunk.Write(210, trace.Normal.X);
            this.chunk.Write(220, trace.Normal.Y);
            this.chunk.Write(230, trace.Normal.Z);
            this.WriteXData(trace.XData);
        }

        private void WriteUCS(UCS ucs)
        {
            Debug.Assert(this.activeTable == "UCS");
            this.chunk.Write(0, ucs.CodeName);
            this.chunk.Write(5, ucs.Handle);
            this.chunk.Write(330, ucs.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbUCSTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(ucs.Name));
            this.chunk.Write(70, (short) 0);
            this.chunk.Write(10, ucs.Origin.X);
            this.chunk.Write(20, ucs.Origin.Y);
            this.chunk.Write(30, ucs.Origin.Z);
            this.chunk.Write(11, ucs.XAxis.X);
            this.chunk.Write(0x15, ucs.XAxis.Y);
            this.chunk.Write(0x1f, ucs.XAxis.Z);
            this.chunk.Write(12, ucs.YAxis.X);
            this.chunk.Write(0x16, ucs.YAxis.Y);
            this.chunk.Write(0x20, ucs.YAxis.Z);
            this.chunk.Write(0x4f, (short) 0);
            this.chunk.Write(0x92, ucs.Elevation);
        }

        private void WriteUnderlay(Underlay underlay)
        {
            this.chunk.Write(100, "AcDbUnderlayReference");
            this.chunk.Write(340, underlay.Definition.Handle);
            Vector3 vector = MathHelper.Transform(underlay.Position, underlay.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.chunk.Write(10, vector.X);
            this.chunk.Write(20, vector.Y);
            this.chunk.Write(30, vector.Z);
            this.chunk.Write(0x29, underlay.Scale.X);
            this.chunk.Write(0x2a, underlay.Scale.Y);
            this.chunk.Write(0x2b, underlay.Scale.Z);
            this.chunk.Write(50, underlay.Rotation);
            this.chunk.Write(210, underlay.Normal.X);
            this.chunk.Write(220, underlay.Normal.Y);
            this.chunk.Write(230, underlay.Normal.Z);
            this.chunk.Write(280, (short) underlay.DisplayOptions);
            this.chunk.Write(0x119, underlay.Contrast);
            this.chunk.Write(0x11a, underlay.Fade);
            if (underlay.ClippingBoundary > null)
            {
                foreach (Vector2 vector3 in underlay.ClippingBoundary.Vertexes)
                {
                    this.chunk.Write(11, vector3.X);
                    this.chunk.Write(0x15, vector3.Y);
                }
            }
        }

        private void WriteUnderlayDefinition(UnderlayDefinition underlayDef, string ownerHandle)
        {
            this.chunk.Write(0, underlayDef.CodeName);
            this.chunk.Write(5, underlayDef.Handle);
            this.chunk.Write(0x66, "{ACAD_REACTORS");
            List<DxfObject> list = null;
            switch (underlayDef.Type)
            {
                case UnderlayType.DGN:
                    list = this.doc.UnderlayDgnDefinitions.References[underlayDef.Name];
                    break;

                case UnderlayType.DWF:
                    list = this.doc.UnderlayDwfDefinitions.References[underlayDef.Name];
                    break;

                case UnderlayType.PDF:
                    list = this.doc.UnderlayPdfDefinitions.References[underlayDef.Name];
                    break;
            }
            if (list == null)
            {
                throw new NullReferenceException("Underlay references list cannot be null");
            }
            foreach (DxfObject obj2 in list)
            {
                Underlay underlay = obj2 as Underlay;
                if (underlay > null)
                {
                    this.chunk.Write(330, underlay.Handle);
                }
            }
            this.chunk.Write(0x66, "}");
            this.chunk.Write(330, ownerHandle);
            this.chunk.Write(100, "AcDbUnderlayDefinition");
            this.chunk.Write(1, this.EncodeNonAsciiCharacters(underlayDef.FileName));
            switch (underlayDef.Type)
            {
                case UnderlayType.DGN:
                    this.chunk.Write(2, this.EncodeNonAsciiCharacters(((UnderlayDgnDefinition) underlayDef).Layout));
                    break;

                case UnderlayType.DWF:
                    this.chunk.Write(2, string.Empty);
                    break;

                case UnderlayType.PDF:
                    this.chunk.Write(2, this.EncodeNonAsciiCharacters(((UnderlayPdfDefinition) underlayDef).Page));
                    break;
            }
        }

        private void WriteViewport(Viewport vp)
        {
            this.chunk.Write(100, "AcDbViewport");
            this.chunk.Write(10, vp.Center.X);
            this.chunk.Write(20, vp.Center.Y);
            this.chunk.Write(30, vp.Center.Z);
            this.chunk.Write(40, vp.Width);
            this.chunk.Write(0x29, vp.Height);
            this.chunk.Write(0x44, vp.Stacking);
            this.chunk.Write(0x45, vp.Id);
            this.chunk.Write(12, vp.ViewCenter.X);
            this.chunk.Write(0x16, vp.ViewCenter.Y);
            this.chunk.Write(13, vp.SnapBase.X);
            this.chunk.Write(0x17, vp.SnapBase.Y);
            this.chunk.Write(14, vp.SnapSpacing.X);
            this.chunk.Write(0x18, vp.SnapSpacing.Y);
            this.chunk.Write(15, vp.GridSpacing.X);
            this.chunk.Write(0x19, vp.GridSpacing.Y);
            this.chunk.Write(0x10, vp.ViewDirection.X);
            this.chunk.Write(0x1a, vp.ViewDirection.Y);
            this.chunk.Write(0x24, vp.ViewDirection.Z);
            this.chunk.Write(0x11, vp.ViewTarget.X);
            this.chunk.Write(0x1b, vp.ViewTarget.Y);
            this.chunk.Write(0x25, vp.ViewTarget.Z);
            this.chunk.Write(0x2a, vp.LensLength);
            this.chunk.Write(0x2b, vp.FrontClipPlane);
            this.chunk.Write(0x2c, vp.BackClipPlane);
            this.chunk.Write(0x2d, vp.ViewHeight);
            this.chunk.Write(50, vp.SnapAngle);
            this.chunk.Write(0x33, vp.TwistAngle);
            this.chunk.Write(0x48, vp.CircleZoomPercent);
            foreach (Layer layer in vp.FrozenLayers)
            {
                this.chunk.Write(0x14b, layer.Handle);
            }
            this.chunk.Write(90, (int) vp.Status);
            if (vp.ClippingBoundary > null)
            {
                this.chunk.Write(340, vp.ClippingBoundary.Handle);
            }
            this.chunk.Write(110, vp.UcsOrigin.X);
            this.chunk.Write(120, vp.UcsOrigin.Y);
            this.chunk.Write(130, vp.UcsOrigin.Z);
            this.chunk.Write(0x6f, vp.UcsXAxis.X);
            this.chunk.Write(0x79, vp.UcsXAxis.Y);
            this.chunk.Write(0x83, vp.UcsXAxis.Z);
            this.chunk.Write(0x70, vp.UcsYAxis.X);
            this.chunk.Write(0x7a, vp.UcsYAxis.Y);
            this.chunk.Write(0x84, vp.UcsYAxis.Z);
            this.WriteXData(vp.XData);
        }

        private void WriteVPort(VPort vp)
        {
            Debug.Assert(this.activeTable == "VPORT");
            this.chunk.Write(0, vp.CodeName);
            this.chunk.Write(5, vp.Handle);
            this.chunk.Write(330, vp.Owner.Handle);
            this.chunk.Write(100, "AcDbSymbolTableRecord");
            this.chunk.Write(100, "AcDbViewportTableRecord");
            this.chunk.Write(2, this.EncodeNonAsciiCharacters(vp.Name));
            this.chunk.Write(70, (short) 0);
            this.chunk.Write(10, 0.0);
            this.chunk.Write(20, 0.0);
            this.chunk.Write(11, 1.0);
            this.chunk.Write(0x15, 1.0);
            this.chunk.Write(12, vp.ViewCenter.X);
            this.chunk.Write(0x16, vp.ViewCenter.Y);
            this.chunk.Write(13, vp.SnapBasePoint.X);
            this.chunk.Write(0x17, vp.SnapBasePoint.Y);
            this.chunk.Write(14, vp.SnapSpacing.X);
            this.chunk.Write(0x18, vp.SnapSpacing.Y);
            this.chunk.Write(15, vp.GridSpacing.X);
            this.chunk.Write(0x19, vp.GridSpacing.Y);
            this.chunk.Write(0x10, vp.ViewDirection.X);
            this.chunk.Write(0x1a, vp.ViewDirection.Y);
            this.chunk.Write(0x24, vp.ViewDirection.Z);
            this.chunk.Write(0x11, vp.ViewTarget.X);
            this.chunk.Write(0x1b, vp.ViewTarget.Y);
            this.chunk.Write(0x25, vp.ViewTarget.Z);
            this.chunk.Write(40, vp.ViewHeight);
            this.chunk.Write(0x29, vp.ViewAspectRatio);
            this.chunk.Write(0x4b, vp.SnapMode ? ((short) 1) : ((short) 0));
            this.chunk.Write(0x4c, vp.ShowGrid ? ((short) 1) : ((short) 0));
        }

        private void WriteWipeout(Wipeout wipeout)
        {
            this.chunk.Write(100, "AcDbWipeout");
            BoundingRectangle rectangle = new BoundingRectangle(wipeout.ClippingBoundary.Vertexes);
            Vector3 vector = new Vector3(rectangle.Min.X, rectangle.Min.Y, wipeout.Elevation);
            double width = rectangle.Width;
            double height = rectangle.Height;
            double x = (width >= height) ? width : height;
            Vector3 vector2 = new Vector3(x, 0.0, 0.0);
            Vector3 vector3 = new Vector3(0.0, x, 0.0);
            List<Vector3> points = new List<Vector3> {
                vector,
                vector2,
                vector3
            };
            IList<Vector3> list = MathHelper.Transform(points, wipeout.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            Vector3 vector5 = list[0];
            this.chunk.Write(10, vector5.X);
            vector5 = list[0];
            this.chunk.Write(20, vector5.Y);
            vector5 = list[0];
            this.chunk.Write(30, vector5.Z);
            vector5 = list[1];
            this.chunk.Write(11, vector5.X);
            vector5 = list[1];
            this.chunk.Write(0x15, vector5.Y);
            vector5 = list[1];
            this.chunk.Write(0x1f, vector5.Z);
            vector5 = list[2];
            this.chunk.Write(12, vector5.X);
            vector5 = list[2];
            this.chunk.Write(0x16, vector5.Y);
            vector5 = list[2];
            this.chunk.Write(0x20, vector5.Z);
            this.chunk.Write(13, 1.0);
            this.chunk.Write(0x17, 1.0);
            this.chunk.Write(280, (short) 1);
            this.chunk.Write(0x119, (short) 50);
            this.chunk.Write(0x11a, (short) 50);
            this.chunk.Write(0x11b, (short) 0);
            this.chunk.Write(0x47, (short) wipeout.ClippingBoundary.Type);
            if (wipeout.ClippingBoundary.Type == ClippingBoundaryType.Polygonal)
            {
                this.chunk.Write(0x5b, wipeout.ClippingBoundary.Vertexes.Count + 1);
            }
            else
            {
                this.chunk.Write(0x5b, wipeout.ClippingBoundary.Vertexes.Count);
            }
            foreach (Vector2 vector6 in wipeout.ClippingBoundary.Vertexes)
            {
                double num4 = ((vector6.X - vector.X) / x) - 0.5;
                double num5 = -(((vector6.Y - vector.Y) / x) - 0.5);
                this.chunk.Write(14, num4);
                this.chunk.Write(0x18, num5);
            }
            if (wipeout.ClippingBoundary.Type == ClippingBoundaryType.Polygonal)
            {
                Vector2 vector4 = wipeout.ClippingBoundary.Vertexes[0];
                this.chunk.Write(14, ((vector4.X - vector.X) / x) - 0.5);
                vector4 = wipeout.ClippingBoundary.Vertexes[0];
                this.chunk.Write(0x18, -(((vector4.Y - vector.Y) / x) - 0.5));
            }
            this.WriteXData(wipeout.XData);
        }

        private void WriteXData(XDataDictionary xData)
        {
            foreach (string str in xData.AppIds)
            {
                this.chunk.Write(0x3e9, this.EncodeNonAsciiCharacters(str));
                foreach (XDataRecord record in xData[str].XDataRecord)
                {
                    short code = (short) record.Code;
                    object obj2 = record.Value;
                    switch (code)
                    {
                        case 0x3e8:
                        case 0x3eb:
                            this.chunk.Write(code, this.EncodeNonAsciiCharacters((string) obj2));
                            break;

                        case 0x3ec:
                        {
                            byte[] buffer2;
                            byte[] sourceArray = (byte[]) obj2;
                            int length = sourceArray.Length;
                            int sourceIndex = 0;
                            while (length > 0x7f)
                            {
                                buffer2 = new byte[0x7f];
                                Array.Copy(sourceArray, sourceIndex, buffer2, 0, 0x7f);
                                this.chunk.Write(code, buffer2);
                                length -= 0x7f;
                                sourceIndex += 0x7f;
                            }
                            buffer2 = new byte[sourceArray.Length - sourceIndex];
                            Array.Copy(sourceArray, sourceIndex, buffer2, 0, sourceArray.Length - sourceIndex);
                            this.chunk.Write(code, buffer2);
                            break;
                        }
                        default:
                            this.chunk.Write(code, obj2);
                            break;
                    }
                }
            }
        }

        private void WriteXLine(XLine xline)
        {
            this.chunk.Write(100, "AcDbXline");
            this.chunk.Write(10, xline.Origin.X);
            this.chunk.Write(20, xline.Origin.Y);
            this.chunk.Write(30, xline.Origin.Z);
            this.chunk.Write(11, xline.Direction.X);
            this.chunk.Write(0x15, xline.Direction.Y);
            this.chunk.Write(0x1f, xline.Direction.Z);
            this.WriteXData(xline.XData);
        }
    }
}

