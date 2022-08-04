namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Objects;
    using netDxf.Tables;
    using netDxf.Units;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal static class DimensionBlock
    {
        public static Block Build(AlignedDimension dim, string name)
        {
            bool flag = false;
            double measurement = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            Vector2 firstReferencePoint = dim.FirstReferencePoint;
            Vector2 secondReferencePoint = dim.SecondReferencePoint;
            double rotation = Vector2.Angle(firstReferencePoint, secondReferencePoint);
            if ((rotation > 1.5707963267948966) && (rotation <= 4.71238898038469))
            {
                Vector2 vector7 = firstReferencePoint;
                firstReferencePoint = secondReferencePoint;
                secondReferencePoint = vector7;
                rotation += 3.1415926535897931;
                flag = true;
            }
            Vector2 u = Vector2.Polar(firstReferencePoint, dim.Offset, rotation + 1.5707963267948966);
            Vector2 v = Vector2.Polar(secondReferencePoint, dim.Offset, rotation + 1.5707963267948966);
            Vector2 vector5 = Vector2.MidPoint(u, v);
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(firstReferencePoint) {
                Layer = layer
            };
            entities.Add(item);
            Point point2 = new Point(secondReferencePoint) {
                Layer = layer
            };
            entities.Add(point2);
            entities.Add(flag ? new Point(u) : new Point(v));
            if (!style.DimLineOff)
            {
                entities.Add(DimensionLine(u, v, rotation, style));
                if (flag)
                {
                    entities.Add(StartArrowHead(v, rotation, style));
                    entities.Add(EndArrowHead(u, rotation + 3.1415926535897931, style));
                }
                else
                {
                    entities.Add(StartArrowHead(u, rotation + 3.1415926535897931, style));
                    entities.Add(EndArrowHead(v, rotation, style));
                }
            }
            double distance = style.ExtLineOffset * style.DimScaleOverall;
            double num4 = style.ExtLineExtend * style.DimScaleOverall;
            if (dim.Offset < 0.0)
            {
                distance *= -1.0;
                num4 *= -1.0;
            }
            if (!style.ExtLine1Off)
            {
                entities.Add(ExtensionLine(Vector2.Polar(firstReferencePoint, distance, rotation + 1.5707963267948966), Vector2.Polar(u, num4, rotation + 1.5707963267948966), style, style.ExtLine1Linetype));
            }
            if (!style.ExtLine2Off)
            {
                entities.Add(ExtensionLine(Vector2.Polar(secondReferencePoint, distance, rotation + 1.5707963267948966), Vector2.Polar(v, num4, rotation + 1.5707963267948966), style, style.ExtLine2Linetype));
            }
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            MText text = DimensionText(Vector2.Polar(vector5, style.TextOffset * style.DimScaleOverall, rotation + 1.5707963267948966), MTextAttachmentPoint.BottomCenter, rotation, list2[0], style);
            if (text > null)
            {
                entities.Add(text);
            }
            if (list2.Count > 1)
            {
                string str = list2[1];
                MText text2 = DimensionText(Vector2.Polar(vector5, -style.TextOffset * style.DimScaleOverall, rotation + 1.5707963267948966), MTextAttachmentPoint.TopCenter, rotation, str, style);
                if (text2 > null)
                {
                    entities.Add(text2);
                }
            }
            Vector3 point = flag ? new Vector3(u.X, u.Y, dim.Elevation) : new Vector3(v.X, v.Y, dim.Elevation);
            dim.DefinitionPoint = MathHelper.Transform(point, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(vector5.X, vector5.Y, dim.Elevation);
            return new Block(name, false, entities, null) { Flags = BlockTypeFlags.AnonymousBlock };
        }

        public static Block Build(Angular2LineDimension dim, string name)
        {
            string str;
            Vector2 vector11;
            MTextAttachmentPoint middleCenter;
            double distance = Math.Abs(dim.Offset);
            double num2 = Math.Sign(dim.Offset);
            double measurement = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            Vector2 startFirstLine = dim.StartFirstLine;
            Vector2 endFirstLine = dim.EndFirstLine;
            Vector2 startSecondLine = dim.StartSecondLine;
            Vector2 endSecondLine = dim.EndSecondLine;
            if (num2 < 0.0)
            {
                Vector2 vector12 = startFirstLine;
                Vector2 vector13 = startSecondLine;
                startFirstLine = endFirstLine;
                endFirstLine = vector12;
                startSecondLine = endSecondLine;
                endSecondLine = vector13;
            }
            Vector2 vector5 = endFirstLine - startFirstLine;
            Vector2 vector6 = endSecondLine - startSecondLine;
            Vector2 u = MathHelper.FindIntersection(startFirstLine, vector5, startSecondLine, vector6);
            if (Vector2.IsNaN(u))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }
            double angle = Vector2.Angle(u, endFirstLine);
            double num5 = Vector2.Angle(u, endSecondLine);
            if (Vector2.CrossProduct(vector5, vector6) < 0.0)
            {
                Vector2 vector14 = startFirstLine;
                Vector2 vector15 = endFirstLine;
                startFirstLine = startSecondLine;
                endFirstLine = endSecondLine;
                startSecondLine = vector14;
                endSecondLine = vector15;
                double num13 = angle;
                angle = num5;
                num5 = num13;
            }
            double num7 = angle + ((measurement * 0.017453292519943295) * 0.5);
            Vector2 start = Vector2.Polar(u, distance, angle);
            Vector2 end = Vector2.Polar(u, distance, num5);
            Vector2 vector10 = Vector2.Polar(u, distance, num7);
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(startFirstLine) {
                Layer = layer
            };
            entities.Add(item);
            Point point2 = new Point(endFirstLine) {
                Layer = layer
            };
            entities.Add(point2);
            Point point3 = new Point(startSecondLine) {
                Layer = layer
            };
            entities.Add(point3);
            Point point4 = new Point(endSecondLine) {
                Layer = layer
            };
            entities.Add(point4);
            if (!style.DimLineOff)
            {
                entities.Add(DimensionArc(u, start, end, angle, num5, distance, style, out double num14, out double num15));
                double num16 = Math.Asin((num14 * 0.5) / distance);
                double num17 = Math.Asin((num15 * 0.5) / distance);
                entities.Add(StartArrowHead(start, (num16 + angle) - 1.5707963267948966, style));
                entities.Add(EndArrowHead(end, (num17 + num5) + 1.5707963267948966, style));
            }
            double num8 = style.ExtLineOffset * style.DimScaleOverall;
            double num9 = style.ExtLineExtend * style.DimScaleOverall;
            int num10 = MathHelper.PointInSegment(start, startFirstLine, endFirstLine);
            if (!style.ExtLine1Off && (num10 > 0))
            {
                Vector2 vector16 = Vector2.Polar((num10 < 0) ? startFirstLine : endFirstLine, num10 * num8, angle);
                entities.Add(ExtensionLine(vector16, Vector2.Polar(start, num10 * num9, angle), style, style.ExtLine1Linetype));
            }
            num10 = MathHelper.PointInSegment(end, startSecondLine, endSecondLine);
            if (!style.ExtLine2Off && (num10 > 0))
            {
                Vector2 vector17 = Vector2.Polar((num10 < 0) ? startSecondLine : endSecondLine, num10 * num8, num5);
                entities.Add(ExtensionLine(vector17, Vector2.Polar(end, num10 * num9, num5), style, style.ExtLine1Linetype));
            }
            double rotation = num7 - 1.5707963267948966;
            double num12 = style.TextOffset * style.DimScaleOverall;
            if ((rotation > 1.5707963267948966) && (rotation <= 4.71238898038469))
            {
                rotation += 3.1415926535897931;
                num12 *= -1.0;
            }
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            if (list2.Count > 1)
            {
                vector11 = vector10;
                str = list2[0] + @"\P" + list2[1];
                middleCenter = MTextAttachmentPoint.MiddleCenter;
            }
            else
            {
                vector11 = Vector2.Polar(vector10, num12, num7);
                str = list2[0];
                middleCenter = MTextAttachmentPoint.TopCenter;
            }
            MText text = DimensionText(vector11, middleCenter, rotation, str, style);
            if (text > null)
            {
                entities.Add(text);
            }
            dim.DefinitionPoint = MathHelper.Transform(new Vector3(dim.EndSecondLine.X, dim.EndSecondLine.Y, dim.Elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(vector10.X, vector10.Y, dim.Elevation);
            dim.ArcDefinitionPoint = dim.MidTextPoint;
            return new Block(name, false, entities, null) { Flags = BlockTypeFlags.AnonymousBlock };
        }

        public static Block Build(Angular3PointDimension dim, string name)
        {
            string str;
            Vector2 vector7;
            MTextAttachmentPoint middleCenter;
            double distance = Math.Abs(dim.Offset);
            double num2 = Math.Sign(dim.Offset);
            double measurement = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            Vector2 centerPoint = dim.CenterPoint;
            Vector2 startPoint = dim.StartPoint;
            Vector2 endPoint = dim.EndPoint;
            if (num2 < 0.0)
            {
                Vector2 vector8 = startPoint;
                startPoint = endPoint;
                endPoint = vector8;
            }
            double angle = Vector2.Angle(centerPoint, startPoint);
            double num5 = Vector2.Angle(centerPoint, endPoint);
            double num6 = angle + ((measurement * 0.017453292519943295) * 0.5);
            Vector2 start = Vector2.Polar(centerPoint, distance, angle);
            Vector2 end = Vector2.Polar(centerPoint, distance, num5);
            Vector2 u = Vector2.Polar(centerPoint, distance, num6);
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(startPoint) {
                Layer = layer
            };
            entities.Add(item);
            Point point2 = new Point(endPoint) {
                Layer = layer
            };
            entities.Add(point2);
            Point point3 = new Point(centerPoint) {
                Layer = layer
            };
            entities.Add(point3);
            if (!style.DimLineOff)
            {
                entities.Add(DimensionArc(centerPoint, start, end, angle, num5, distance, style, out double num11, out double num12));
                double num13 = Math.Asin((num11 * 0.5) / distance);
                double num14 = Math.Asin((num12 * 0.5) / distance);
                entities.Add(StartArrowHead(start, (num13 + angle) - 1.5707963267948966, style));
                entities.Add(EndArrowHead(end, (num14 + num5) + 1.5707963267948966, style));
            }
            double num7 = style.ExtLineOffset * style.DimScaleOverall;
            double num8 = style.ExtLineExtend * style.DimScaleOverall;
            if (!style.ExtLine1Off)
            {
                entities.Add(ExtensionLine(Vector2.Polar(startPoint, num7, angle), Vector2.Polar(start, num8, angle), style, style.ExtLine1Linetype));
            }
            if (!style.ExtLine2Off)
            {
                entities.Add(ExtensionLine(Vector2.Polar(endPoint, num7, num5), Vector2.Polar(end, num8, num5), style, style.ExtLine1Linetype));
            }
            double rotation = num6 - 1.5707963267948966;
            double num10 = style.TextOffset * style.DimScaleOverall;
            if ((rotation > 1.5707963267948966) && (rotation <= 4.71238898038469))
            {
                rotation += 3.1415926535897931;
                num10 *= -1.0;
            }
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            if (list2.Count > 1)
            {
                vector7 = u;
                str = list2[0] + @"\P" + list2[1];
                middleCenter = MTextAttachmentPoint.MiddleCenter;
            }
            else
            {
                vector7 = Vector2.Polar(u, num10, num6);
                str = list2[0];
                middleCenter = MTextAttachmentPoint.TopCenter;
            }
            MText text = DimensionText(vector7, middleCenter, rotation, str, style);
            if (text > null)
            {
                entities.Add(text);
            }
            dim.DefinitionPoint = MathHelper.Transform(new Vector3(u.X, u.Y, dim.Elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(u.X, u.Y, dim.Elevation);
            return new Block(name, false, entities, null);
        }

        public static Block Build(DiametricDimension dim, string name)
        {
            short num5;
            string str;
            double offset = dim.Offset;
            double measurement = dim.Measurement;
            double radius = measurement * 0.5;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            Vector2 centerPoint = dim.CenterPoint;
            Vector2 referencePoint = dim.ReferencePoint;
            double angle = Vector2.Angle(centerPoint, referencePoint);
            Vector2 vector3 = Vector2.Polar(referencePoint, -measurement, angle);
            double num6 = (2.0 * style.ArrowSize) + (style.TextOffset * style.DimScaleOverall);
            if ((offset >= radius) && (offset <= (radius + num6)))
            {
                offset = radius + num6;
                num5 = -1;
            }
            else if ((offset >= (radius - num6)) && (offset <= radius))
            {
                offset = radius - num6;
                num5 = 1;
            }
            else if (offset > radius)
            {
                num5 = -1;
            }
            else
            {
                num5 = 1;
            }
            Vector2 start = Vector2.Polar(centerPoint, offset, angle);
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(referencePoint) {
                Layer = layer
            };
            entities.Add(item);
            if (!style.DimLineOff)
            {
                entities.Add(DimensionRadialLine(start, referencePoint, angle, num5, style));
                entities.Add(EndArrowHead(referencePoint, ((1 - num5) * 1.5707963267948966) + angle, style));
            }
            entities.AddRange(CenterCross(centerPoint, radius, style));
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            if (list2.Count > 1)
            {
                str = list2[0] + @"\P" + list2[1];
            }
            else
            {
                str = list2[0];
            }
            double rotation = angle;
            short num8 = 1;
            if ((rotation > 1.5707963267948966) && (rotation <= 4.71238898038469))
            {
                rotation += 3.1415926535897931;
                num8 = -1;
            }
            MTextAttachmentPoint attachmentPoint = ((num8 * num5) < 0) ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
            MText text = DimensionText(Vector2.Polar(start, ((-num8 * num5) * style.TextOffset) * style.DimScaleOverall, rotation), attachmentPoint, rotation, str, style);
            if (text > null)
            {
                entities.Add(text);
            }
            dim.DefinitionPoint = MathHelper.Transform(new Vector3(vector3.X, vector3.Y, dim.Elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(start.X, start.Y, dim.Elevation);
            return new Block(name, false, entities, null);
        }

        public static Block Build(LinearDimension dim, string name)
        {
            double measurement = dim.Measurement;
            bool flag = false;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            Vector2 firstReferencePoint = dim.FirstReferencePoint;
            Vector2 secondReferencePoint = dim.SecondReferencePoint;
            Vector2 vector3 = Vector2.MidPoint(firstReferencePoint, secondReferencePoint);
            double rotation = dim.Rotation * 0.017453292519943295;
            Vector2 point = new Vector2(-measurement * 0.5, dim.Offset);
            Vector2 vector5 = new Vector2(measurement * 0.5, dim.Offset);
            Vector2 u = MathHelper.Transform(point, rotation, CoordinateSystem.Object, CoordinateSystem.World) + vector3;
            Vector2 v = MathHelper.Transform(vector5, rotation, CoordinateSystem.Object, CoordinateSystem.World) + vector3;
            Vector2 vector8 = Vector2.MidPoint(u, v);
            double num3 = Vector2.AngleBetween(secondReferencePoint - firstReferencePoint, v - u);
            if ((num3 > 1.5707963267948966) && (num3 <= 4.71238898038469))
            {
                Vector2 vector15 = firstReferencePoint;
                firstReferencePoint = secondReferencePoint;
                secondReferencePoint = vector15;
                flag = true;
            }
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(firstReferencePoint) {
                Layer = layer
            };
            entities.Add(item);
            Point point2 = new Point(secondReferencePoint) {
                Layer = layer
            };
            entities.Add(point2);
            entities.Add(flag ? new Point(u) : new Point(v));
            if (!style.DimLineOff)
            {
                entities.Add(DimensionLine(u, v, rotation, style));
                if (flag)
                {
                    entities.Add(StartArrowHead(v, rotation, style));
                    entities.Add(EndArrowHead(u, rotation + 3.1415926535897931, style));
                }
                else
                {
                    entities.Add(StartArrowHead(u, rotation + 3.1415926535897931, style));
                    entities.Add(EndArrowHead(v, rotation, style));
                }
            }
            Vector2 vector10 = Vector2.Perpendicular(Vector2.Normalize(v - u));
            Vector2 vector11 = u - firstReferencePoint;
            Vector2 vector12 = MathHelper.IsZero(Vector2.DotProduct(vector11, vector11)) ? vector10 : Vector2.Normalize(u - firstReferencePoint);
            vector11 = v - secondReferencePoint;
            Vector2 vector13 = MathHelper.IsZero(Vector2.DotProduct(vector11, vector11)) ? -vector10 : Vector2.Normalize(v - secondReferencePoint);
            double num4 = style.ExtLineOffset * style.DimScaleOverall;
            double num5 = style.ExtLineExtend * style.DimScaleOverall;
            if (!style.ExtLine1Off)
            {
                entities.Add(ExtensionLine(firstReferencePoint + (num4 * vector12), u + (num5 * vector12), style, style.ExtLine1Linetype));
            }
            if (!style.ExtLine2Off)
            {
                entities.Add(ExtensionLine(secondReferencePoint + (num4 * vector13), v + (num5 * vector13), style, style.ExtLine2Linetype));
            }
            double num6 = rotation;
            if ((num6 > 1.5707963267948966) && (num6 <= 4.71238898038469))
            {
                num6 += 3.1415926535897931;
            }
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            MText text = DimensionText(Vector2.Polar(vector8, style.TextOffset * style.DimScaleOverall, num6 + 1.5707963267948966), MTextAttachmentPoint.BottomCenter, num6, list2[0], style);
            if (text > null)
            {
                entities.Add(text);
            }
            if (list2.Count > 1)
            {
                string str = list2[1];
                MText text2 = DimensionText(Vector2.Polar(vector8, -style.TextOffset * style.DimScaleOverall, num6 + 1.5707963267948966), MTextAttachmentPoint.TopCenter, num6, str, style);
                if (text2 > null)
                {
                    entities.Add(text2);
                }
            }
            Vector3 vector14 = flag ? new Vector3(u.X, u.Y, dim.Elevation) : new Vector3(v.X, v.Y, dim.Elevation);
            dim.DefinitionPoint = MathHelper.Transform(vector14, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(vector8.X, vector8.Y, dim.Elevation);
            return new Block(name, false, entities, null) { Flags = BlockTypeFlags.AnonymousBlock };
        }

        public static Block Build(OrdinateDimension dim, string name)
        {
            string str;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            double measurement = dim.Measurement;
            double rotation = dim.Rotation * 0.017453292519943295;
            Vector2 origin = dim.Origin;
            Vector2 u = origin + MathHelper.Transform(dim.ReferencePoint, rotation, CoordinateSystem.Object, CoordinateSystem.World);
            if (dim.Axis == OrdinateDimensionAxis.X)
            {
                rotation += 1.5707963267948966;
            }
            Vector2 endPoint = Vector2.Polar(u, dim.Length, rotation);
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(origin) {
                Layer = layer
            };
            entities.Add(item);
            Point point2 = new Point(u) {
                Layer = layer
            };
            entities.Add(point2);
            short num3 = 1;
            if (dim.Length < 0.0)
            {
                num3 = -1;
            }
            entities.Add(new netDxf.Entities.Line(Vector2.Polar(u, (num3 * style.ExtLineOffset) * style.DimScaleOverall, rotation), endPoint));
            Vector2 position = Vector2.Polar(u, dim.Length + ((num3 * style.TextOffset) * style.DimScaleOverall), rotation);
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            if (list2.Count > 1)
            {
                str = list2[0] + @"\P" + list2[1];
            }
            else
            {
                str = list2[0];
            }
            MTextAttachmentPoint attachmentPoint = (num3 < 0) ? MTextAttachmentPoint.MiddleRight : MTextAttachmentPoint.MiddleLeft;
            MText text = DimensionText(position, attachmentPoint, rotation, str, style);
            if (text > null)
            {
                entities.Add(text);
            }
            Vector3[] points = new Vector3[] { new Vector3(u.X, u.Y, dim.Elevation), new Vector3(endPoint.X, endPoint.Y, dim.Elevation), new Vector3(dim.Origin.X, dim.Origin.Y, dim.Elevation) };
            IList<Vector3> list3 = MathHelper.Transform(points, dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.FirstPoint = list3[0];
            dim.SecondPoint = list3[1];
            dim.DefinitionPoint = list3[2];
            dim.MidTextPoint = new Vector3(position.X, position.Y, dim.Elevation);
            return new Block(name, false, entities, null);
        }

        public static Block Build(RadialDimension dim, string name)
        {
            short num4;
            string str;
            double offset = dim.Offset;
            double measurement = dim.Measurement;
            DimensionStyle style = BuildDimensionStyleOverride(dim);
            List<EntityObject> entities = new List<EntityObject>();
            Vector2 centerPoint = dim.CenterPoint;
            Vector2 referencePoint = dim.ReferencePoint;
            double angle = Vector2.Angle(centerPoint, referencePoint);
            double num5 = (2.0 * style.ArrowSize) + (style.TextOffset * style.DimScaleOverall);
            if ((offset >= measurement) && (offset <= (measurement + num5)))
            {
                offset = measurement + num5;
                num4 = -1;
            }
            else if ((offset >= (measurement - num5)) && (offset <= measurement))
            {
                offset = measurement - num5;
                num4 = 1;
            }
            else if (offset > measurement)
            {
                num4 = -1;
            }
            else
            {
                num4 = 1;
            }
            Vector2 start = Vector2.Polar(centerPoint, offset, angle);
            Layer layer = new Layer("Defpoints") {
                Plot = false
            };
            Point item = new Point(referencePoint) {
                Layer = layer
            };
            entities.Add(item);
            if (!style.DimLineOff)
            {
                entities.Add(DimensionRadialLine(start, referencePoint, angle, num4, style));
                entities.Add(EndArrowHead(referencePoint, ((1 - num4) * 1.5707963267948966) + angle, style));
            }
            entities.AddRange(CenterCross(centerPoint, measurement, style));
            List<string> list2 = FormatDimensionText(measurement, dim.DimensionType, dim.UserText, style, dim.Owner.Record.Layout);
            if (list2.Count > 1)
            {
                str = list2[0] + @"\P" + list2[1];
            }
            else
            {
                str = list2[0];
            }
            double rotation = angle;
            short num7 = 1;
            if ((angle > 1.5707963267948966) && (angle <= 4.71238898038469))
            {
                rotation += 3.1415926535897931;
                num7 = -1;
            }
            MTextAttachmentPoint attachmentPoint = ((num7 * num4) < 0) ? MTextAttachmentPoint.MiddleLeft : MTextAttachmentPoint.MiddleRight;
            MText text = DimensionText(Vector2.Polar(start, ((-num7 * num4) * style.TextOffset) * style.DimScaleOverall, rotation), attachmentPoint, rotation, str, style);
            if (text > null)
            {
                entities.Add(text);
            }
            dim.DefinitionPoint = MathHelper.Transform(new Vector3(centerPoint.X, centerPoint.Y, dim.Elevation), dim.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            dim.MidTextPoint = new Vector3(start.X, start.Y, dim.Elevation);
            return new Block(name, false, entities, null);
        }

        private static DimensionStyle BuildDimensionStyleOverride(Dimension dim)
        {
            if (dim.StyleOverrides.Count == 0)
            {
                return dim.Style;
            }
            DimensionStyle style = new DimensionStyle(dim.Style.Name) {
                DimLineColor = dim.Style.DimLineColor,
                DimLineLinetype = dim.Style.DimLineLinetype,
                DimLineLineweight = dim.Style.DimLineLineweight,
                DimLineOff = dim.Style.DimLineOff,
                DimBaselineSpacing = dim.Style.DimBaselineSpacing,
                DimLineExtend = dim.Style.DimLineExtend,
                ExtLineColor = dim.Style.ExtLineColor,
                ExtLine1Linetype = dim.Style.ExtLine1Linetype,
                ExtLine2Linetype = dim.Style.ExtLine2Linetype,
                ExtLineLineweight = dim.Style.ExtLineLineweight,
                ExtLine1Off = dim.Style.ExtLine1Off,
                ExtLine2Off = dim.Style.ExtLine2Off,
                ExtLineOffset = dim.Style.ExtLineOffset,
                ExtLineExtend = dim.Style.ExtLineExtend,
                ArrowSize = dim.Style.ArrowSize,
                CenterMarkSize = dim.Style.CenterMarkSize,
                DIMTIH = dim.Style.DIMTIH,
                DIMTOH = dim.Style.DIMTOH,
                TextStyle = dim.Style.TextStyle,
                TextColor = dim.Style.TextColor,
                TextHeight = dim.Style.TextHeight,
                DIMJUST = dim.Style.DIMJUST,
                DIMTAD = dim.Style.DIMTAD,
                TextOffset = dim.Style.TextOffset,
                AngularPrecision = dim.Style.AngularPrecision,
                LengthPrecision = dim.Style.LengthPrecision,
                DecimalSeparator = dim.Style.DecimalSeparator,
                DimAngularUnits = dim.Style.DimAngularUnits,
                LeaderArrow = dim.Style.LeaderArrow,
                DimArrow1 = dim.Style.DimArrow1,
                DimArrow2 = dim.Style.DimArrow2
            };
            foreach (DimensionStyleOverride @override in dim.StyleOverrides.Values)
            {
                switch (@override.Type)
                {
                    case DimensionStyleOverrideType.DimLineColor:
                        style.DimLineColor = (AciColor) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimLineLinetype:
                        style.DimLineLinetype = (Linetype) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimLineLineweight:
                        style.DimLineLineweight = (Lineweight) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimLineOff:
                        style.DimLineOff = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimLineExtend:
                        style.DimLineExtend = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLineColor:
                        style.ExtLineColor = (AciColor) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLine1Linetype:
                        style.ExtLine1Linetype = (Linetype) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLine2Linetype:
                        style.ExtLine2Linetype = (Linetype) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLineLineweight:
                        style.ExtLineLineweight = (Lineweight) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLine1Off:
                        style.ExtLine1Off = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLine2Off:
                        style.ExtLine2Off = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLineOffset:
                        style.ExtLineOffset = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ExtLineExtend:
                        style.ExtLineExtend = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.ArrowSize:
                        style.ArrowSize = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.CenterMarkSize:
                        style.CenterMarkSize = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.LeaderArrow:
                        style.LeaderArrow = (Block) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimArrow1:
                        style.DimArrow1 = (Block) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimArrow2:
                        style.DimArrow2 = (Block) @override.Value;
                        break;

                    case DimensionStyleOverrideType.TextStyle:
                        style.TextStyle = (TextStyle) @override.Value;
                        break;

                    case DimensionStyleOverrideType.TextColor:
                        style.TextColor = (AciColor) @override.Value;
                        break;

                    case DimensionStyleOverrideType.TextHeight:
                        style.TextHeight = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.TextOffset:
                        style.TextOffset = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimScaleOverall:
                        style.DimScaleOverall = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.AngularPrecision:
                        style.AngularPrecision = (short) @override.Value;
                        break;

                    case DimensionStyleOverrideType.LengthPrecision:
                        style.LengthPrecision = (short) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimPrefix:
                        style.DimPrefix = (string) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimSuffix:
                        style.DimSuffix = (string) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DecimalSeparator:
                        style.DecimalSeparator = (char) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimScaleLinear:
                        style.DimScaleLinear = (double) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimLengthUnits:
                        style.DimLengthUnits = (LinearUnitType) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimAngularUnits:
                        style.DimAngularUnits = (AngleUnitType) @override.Value;
                        break;

                    case DimensionStyleOverrideType.FractionalType:
                        style.FractionalType = (FractionFormatType) @override.Value;
                        break;

                    case DimensionStyleOverrideType.SuppressLinearLeadingZeros:
                        style.SuppressLinearLeadingZeros = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.SuppressLinearTrailingZeros:
                        style.SuppressLinearTrailingZeros = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.SuppressAngularLeadingZeros:
                        style.SuppressAngularLeadingZeros = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.SuppressAngularTrailingZeros:
                        style.SuppressAngularTrailingZeros = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.SuppressZeroFeet:
                        style.SuppressZeroFeet = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.SuppressZeroInches:
                        style.SuppressZeroInches = (bool) @override.Value;
                        break;

                    case DimensionStyleOverrideType.DimRoundoff:
                        style.DimRoundoff = (double) @override.Value;
                        break;
                }
            }
            return style;
        }

        private static List<EntityObject> CenterCross(Vector2 center, double radius, DimensionStyle style)
        {
            List<EntityObject> list = new List<EntityObject>();
            if (!MathHelper.IsZero(style.CenterMarkSize))
            {
                double y = Math.Abs((double) (style.CenterMarkSize * style.DimScaleOverall));
                Vector2 startPoint = new Vector2(0.0, -y) + center;
                Vector2 endPoint = new Vector2(0.0, y) + center;
                netDxf.Entities.Line item = new netDxf.Entities.Line(startPoint, endPoint) {
                    Color = style.ExtLineColor,
                    Lineweight = style.ExtLineLineweight
                };
                list.Add(item);
                startPoint = new Vector2(-y, 0.0) + center;
                endPoint = new Vector2(y, 0.0) + center;
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(startPoint, endPoint) {
                    Color = style.ExtLineColor,
                    Lineweight = style.ExtLineLineweight
                };
                list.Add(line2);
                if (style.CenterMarkSize < 0.0)
                {
                    startPoint = new Vector2(2.0 * y, 0.0) + center;
                    endPoint = new Vector2(radius + y, 0.0) + center;
                    netDxf.Entities.Line line3 = new netDxf.Entities.Line(startPoint, endPoint) {
                        Color = style.ExtLineColor,
                        Lineweight = style.ExtLineLineweight
                    };
                    list.Add(line3);
                    startPoint = new Vector2(-2.0 * y, 0.0) + center;
                    endPoint = new Vector2(-radius - y, 0.0) + center;
                    netDxf.Entities.Line line4 = new netDxf.Entities.Line(startPoint, endPoint) {
                        Color = style.ExtLineColor,
                        Lineweight = style.ExtLineLineweight
                    };
                    list.Add(line4);
                    startPoint = new Vector2(0.0, 2.0 * y) + center;
                    endPoint = new Vector2(0.0, radius + y) + center;
                    netDxf.Entities.Line line5 = new netDxf.Entities.Line(startPoint, endPoint) {
                        Color = style.ExtLineColor,
                        Lineweight = style.ExtLineLineweight
                    };
                    list.Add(line5);
                    startPoint = new Vector2(0.0, -2.0 * y) + center;
                    endPoint = new Vector2(0.0, -radius - y) + center;
                    netDxf.Entities.Line line6 = new netDxf.Entities.Line(startPoint, endPoint) {
                        Color = style.ExtLineColor,
                        Lineweight = style.ExtLineLineweight
                    };
                    list.Add(line6);
                }
            }
            return list;
        }

        private static netDxf.Entities.Arc DimensionArc(Vector2 center, Vector2 start, Vector2 end, double startAngle, double endAngle, double radius, DimensionStyle style, out double e1, out double e2)
        {
            double distance = style.ArrowSize * style.DimScaleOverall;
            double num2 = -style.ArrowSize * style.DimScaleOverall;
            e1 = distance;
            e2 = num2;
            Block block = style.DimArrow1;
            if ((block > null) && (((block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) || block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase)))
            {
                distance = 0.0;
                e1 = 0.0;
            }
            block = style.DimArrow2;
            if ((block > null) && (((block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) || block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase)))
            {
                num2 = 0.0;
                e2 = 0.0;
            }
            start = Vector2.Polar(start, distance, startAngle + 1.5707963267948966);
            end = Vector2.Polar(end, num2, endAngle + 1.5707963267948966);
            return new netDxf.Entities.Arc(center, radius, Vector2.Angle(center, start) * 57.295779513082323, Vector2.Angle(center, end) * 57.295779513082323) { 
                Color = style.DimLineColor,
                Linetype = style.DimLineLinetype,
                Lineweight = style.DimLineLineweight
            };
        }

        private static netDxf.Entities.Line DimensionLine(Vector2 start, Vector2 end, double rotation, DimensionStyle style)
        {
            double distance = style.ArrowSize * style.DimScaleOverall;
            double num2 = -style.ArrowSize * style.DimScaleOverall;
            Block block = style.DimArrow1;
            if ((block > null) && (((block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) || block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase)))
            {
                distance = -style.DimLineExtend * style.DimScaleOverall;
            }
            block = style.DimArrow2;
            if ((block > null) && (((block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) || block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase)))
            {
                num2 = style.DimLineExtend * style.DimScaleOverall;
            }
            start = Vector2.Polar(start, distance, rotation);
            end = Vector2.Polar(end, num2, rotation);
            return new netDxf.Entities.Line(start, end) { 
                Color = style.DimLineColor,
                Linetype = style.DimLineLinetype,
                Lineweight = style.DimLineLineweight
            };
        }

        private static netDxf.Entities.Line DimensionRadialLine(Vector2 start, Vector2 end, double rotation, short reversed, DimensionStyle style)
        {
            double num = -style.ArrowSize * style.DimScaleOverall;
            Block block = style.DimArrow2;
            if ((block > null) && (((block.Name.Equals("_OBLIQUE", StringComparison.OrdinalIgnoreCase) || block.Name.Equals("_ARCHTICK", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_INTEGRAL", StringComparison.OrdinalIgnoreCase)) || block.Name.Equals("_NONE", StringComparison.OrdinalIgnoreCase)))
            {
                num = style.DimLineExtend * style.DimScaleOverall;
            }
            end = Vector2.Polar(end, reversed * num, rotation);
            return new netDxf.Entities.Line(start, end) { 
                Color = style.DimLineColor,
                Linetype = style.DimLineLinetype,
                Lineweight = style.DimLineLineweight
            };
        }

        private static MText DimensionText(Vector2 position, MTextAttachmentPoint attachmentPoint, double rotation, string text, DimensionStyle style)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            return new MText(text, position, style.TextHeight * style.DimScaleOverall, 0.0, style.TextStyle) { 
                Color = style.TextColor,
                AttachmentPoint = attachmentPoint,
                Rotation = rotation * 57.295779513082323
            };
        }

        private static EntityObject EndArrowHead(Vector2 position, double rotation, DimensionStyle style)
        {
            Block block = style.DimArrow2;
            if (block == null)
            {
                Vector2 u = Vector2.Polar(position, -style.ArrowSize * style.DimScaleOverall, rotation);
                return new Solid(position, Vector2.Polar(u, -(style.ArrowSize / 6.0) * style.DimScaleOverall, rotation + 1.5707963267948966), Vector2.Polar(u, (style.ArrowSize / 6.0) * style.DimScaleOverall, rotation + 1.5707963267948966)) { Color = style.DimLineColor };
            }
            return new Insert(block, position) { 
                Color = style.DimLineColor,
                Scale = new Vector3(style.ArrowSize * style.DimScaleOverall),
                Rotation = rotation * 57.295779513082323,
                Lineweight = style.DimLineLineweight
            };
        }

        private static netDxf.Entities.Line ExtensionLine(Vector2 start, Vector2 end, DimensionStyle style, Linetype linetype) => 
            new netDxf.Entities.Line(start, end) { 
                Color = style.ExtLineColor,
                Linetype = linetype,
                Lineweight = style.ExtLineLineweight
            };

        private static List<string> FormatDimensionText(double measure, DimensionType dimType, string userText, DimensionStyle style, Layout layout)
        {
            List<string> list = new List<string>();
            if (userText != " ")
            {
                string str = string.Empty;
                UnitStyleFormat format = new UnitStyleFormat {
                    LinearDecimalPlaces = style.LengthPrecision,
                    AngularDecimalPlaces = (style.AngularPrecision == -1) ? style.LengthPrecision : style.AngularPrecision,
                    DecimalSeparator = style.DecimalSeparator.ToString(),
                    FractionType = style.FractionalType,
                    SupressLinearLeadingZeros = style.SuppressLinearLeadingZeros,
                    SupressLinearTrailingZeros = style.SuppressLinearTrailingZeros,
                    SupressAngularLeadingZeros = style.SuppressAngularLeadingZeros,
                    SupressAngularTrailingZeros = style.SuppressAngularTrailingZeros,
                    SupressZeroFeet = style.SuppressZeroFeet,
                    SupressZeroInches = style.SuppressZeroInches
                };
                if ((dimType == DimensionType.Angular) || (dimType == DimensionType.Angular3Point))
                {
                    switch (style.DimAngularUnits)
                    {
                        case AngleUnitType.DecimalDegrees:
                            str = AngleUnitFormat.ToDecimal(measure, format);
                            break;

                        case AngleUnitType.DegreesMinutesSeconds:
                            str = AngleUnitFormat.ToDegreesMinutesSeconds(measure, format);
                            break;

                        case AngleUnitType.Gradians:
                            str = AngleUnitFormat.ToGradians(measure, format);
                            break;

                        case AngleUnitType.Radians:
                            str = AngleUnitFormat.ToRadians(measure, format);
                            break;

                        case AngleUnitType.SurveyorUnits:
                            str = AngleUnitFormat.ToDecimal(measure, format);
                            break;
                    }
                }
                else
                {
                    double num = Math.Abs(style.DimScaleLinear);
                    if ((layout > null) && ((style.DimScaleLinear < 0.0) && !layout.IsPaperSpace))
                    {
                        num = 1.0;
                    }
                    if (style.DimRoundoff > 0.0)
                    {
                        measure = MathHelper.RoundToNearest(measure * num, style.DimRoundoff);
                    }
                    else
                    {
                        measure *= num;
                    }
                    switch (style.DimLengthUnits)
                    {
                        case LinearUnitType.Scientific:
                            str = LinearUnitFormat.ToScientific(measure, format);
                            break;

                        case LinearUnitType.Decimal:
                            str = LinearUnitFormat.ToDecimal(measure, format);
                            break;

                        case LinearUnitType.Engineering:
                            str = LinearUnitFormat.ToEngineering(measure, format);
                            break;

                        case LinearUnitType.Architectural:
                            str = LinearUnitFormat.ToArchitectural(measure, format);
                            break;

                        case LinearUnitType.Fractional:
                            str = LinearUnitFormat.ToFractional(measure, format);
                            break;

                        case LinearUnitType.WindowsDesktop:
                            format.LinearDecimalPlaces = (short) Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalDigits;
                            format.DecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                            str = LinearUnitFormat.ToDecimal(measure * style.DimScaleLinear, format);
                            break;
                    }
                }
                string str2 = string.Empty;
                if (dimType == DimensionType.Diameter)
                {
                    str2 = string.IsNullOrEmpty(style.DimPrefix) ? "\x00d8" : style.DimPrefix;
                }
                if (dimType == DimensionType.Radius)
                {
                    str2 = string.IsNullOrEmpty(style.DimPrefix) ? "R" : style.DimPrefix;
                }
                str = $"{str2}{str}{style.DimSuffix}";
                if (string.IsNullOrEmpty(userText))
                {
                    list.Add(str);
                    return list;
                }
                int length = 0;
                for (int i = 0; i < userText.Length; i++)
                {
                    char ch = userText[i];
                    if (ch.Equals('\\'))
                    {
                        int num4 = i + 1;
                        if (num4 >= userText.Length)
                        {
                            break;
                        }
                        ch = userText[num4];
                        if (ch.Equals('X'))
                        {
                            length = i;
                            break;
                        }
                    }
                }
                if (length > 0)
                {
                    list.Add(userText.Substring(0, length).Replace("<>", str));
                    list.Add(userText.Substring(length + 2, userText.Length - (length + 2)).Replace("<>", str));
                }
                else
                {
                    list.Add(userText.Replace("<>", str));
                }
            }
            return list;
        }

        private static EntityObject StartArrowHead(Vector2 position, double rotation, DimensionStyle style)
        {
            Block block = style.DimArrow1;
            if (block == null)
            {
                Vector2 u = Vector2.Polar(position, -style.ArrowSize * style.DimScaleOverall, rotation);
                return new Solid(position, Vector2.Polar(u, -(style.ArrowSize / 6.0) * style.DimScaleOverall, rotation + 1.5707963267948966), Vector2.Polar(u, (style.ArrowSize / 6.0) * style.DimScaleOverall, rotation + 1.5707963267948966)) { Color = style.DimLineColor };
            }
            return new Insert(block, position) { 
                Color = style.DimLineColor,
                Scale = new Vector3(style.ArrowSize * style.DimScaleOverall),
                Rotation = rotation * 57.295779513082323,
                Lineweight = style.DimLineLineweight
            };
        }
    }
}

