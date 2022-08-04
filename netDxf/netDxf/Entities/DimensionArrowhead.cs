namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public static class DimensionArrowhead
    {
        public static Block Dot
        {
            get
            {
                Block block = new Block("_DOT");
                List<LwPolylineVertex> vertexes = new List<LwPolylineVertex> {
                    new LwPolylineVertex(-0.25, 0.0, 1.0),
                    new LwPolylineVertex(0.25, 0.0, 1.0)
                };
                LwPolyline item = new LwPolyline(vertexes, true) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock
                };
                item.SetConstantWidth(0.5);
                block.Entities.Add(item);
                netDxf.Entities.Line line = new netDxf.Entities.Line(new Vector3(-0.5, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line);
                return block;
            }
        }

        public static Block DotSmall
        {
            get
            {
                Block block = new Block("_DOTSMALL");
                List<LwPolylineVertex> vertexes = new List<LwPolylineVertex> {
                    new LwPolylineVertex(-0.0625, 0.0, 1.0),
                    new LwPolylineVertex(0.0625, 0.0, 1.0)
                };
                LwPolyline item = new LwPolyline(vertexes, true) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock
                };
                item.SetConstantWidth(0.5);
                block.Entities.Add(item);
                return block;
            }
        }

        public static Block DotBlank
        {
            get
            {
                Block block = new Block("_DOTBLANK");
                Circle item = new Circle(new Vector3(0.0, 0.0, 0.0), 0.5) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line = new netDxf.Entities.Line(new Vector3(-0.5, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line);
                return block;
            }
        }

        public static Block OriginIndicator
        {
            get
            {
                Block block = new Block("_ORIGIN");
                Circle item = new Circle(new Vector3(0.0, 0.0, 0.0), 0.5) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line);
                return block;
            }
        }

        public static Block OriginIndicator2
        {
            get
            {
                Block block = new Block("_ORIGIN2");
                Circle item = new Circle(new Vector3(0.0, 0.0, 0.0), 0.5) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                Circle circle2 = new Circle(new Vector3(0.0, 0.0, 0.0), 0.25) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(circle2);
                netDxf.Entities.Line line = new netDxf.Entities.Line(new Vector3(-0.5, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line);
                return block;
            }
        }

        public static Block Open
        {
            get
            {
                Block block = new Block("_OPEN");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-1.0, 0.1666666666666666, 0.0), new Vector3(0.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, -0.1666666666666666, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                return block;
            }
        }

        public static Block Open90
        {
            get
            {
                Block block = new Block("_OPEN90");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-0.5, 0.5, 0.0), new Vector3(0.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-0.5, -0.5, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                return block;
            }
        }

        public static Block Open30
        {
            get
            {
                Block block = new Block("_OPEN30");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-1.0, 0.26794919, 0.0), new Vector3(0.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, -0.26794919, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                return block;
            }
        }

        public static Block Closed
        {
            get
            {
                Block block = new Block("_CLOSED");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-1.0, 0.1666666666666666, 0.0), new Vector3(0.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, -0.1666666666666666, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(-1.0, 0.1666666666666666, 0.0), new Vector3(-1.0, -0.1666666666666666, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                netDxf.Entities.Line line4 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line4);
                return block;
            }
        }

        public static Block DotSmallBlank
        {
            get
            {
                Block block = new Block("_SMALL");
                Circle item = new Circle(new Vector3(0.0, 0.0, 0.0), 0.25) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                return block;
            }
        }

        public static Block None =>
            new Block("_NONE");

        public static Block Oblique
        {
            get
            {
                Block block = new Block("_OBLIQUE");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-0.5, -0.5, 0.0), new Vector3(0.5, 0.5, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                return block;
            }
        }

        public static Block BoxFilled
        {
            get
            {
                Block block = new Block("_BOXFILLED");
                Solid item = new Solid(new Vector2(-0.5, 0.5), new Vector2(0.5, 0.5), new Vector2(-0.5, -0.5), new Vector2(0.5, -0.5)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line = new netDxf.Entities.Line(new Vector3(-0.5, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line);
                return block;
            }
        }

        public static Block Box
        {
            get
            {
                Block block = new Block("_BOXBLANK");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-0.5, -0.5, 0.0), new Vector3(0.5, -0.5, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(0.5, -0.5, 0.0), new Vector3(0.5, 0.5, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(0.5, 0.5, 0.0), new Vector3(-0.5, 0.5, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                netDxf.Entities.Line line4 = new netDxf.Entities.Line(new Vector3(-0.5, 0.5, 0.0), new Vector3(-0.5, -0.5, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line4);
                netDxf.Entities.Line line5 = new netDxf.Entities.Line(new Vector3(-0.5, 0.0, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line5);
                return block;
            }
        }

        public static Block ClosedBlank
        {
            get
            {
                Block block = new Block("_CLOSEDBLANK");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(-1.0, 0.1666666666666666, 0.0), new Vector3(0.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(0.0, 0.0, 0.0), new Vector3(-1.0, -0.1666666666666666, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(-1.0, 0.1666666666666666, 0.0), new Vector3(-1.0, -0.1666666666666666, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                return block;
            }
        }

        public static Block DatumTriangleFilled
        {
            get
            {
                Block block = new Block("_DATUMFILLED");
                Solid item = new Solid(new Vector2(0.0, 0.57735027), new Vector2(-1.0, 0.0), new Vector2(0.0, -0.57735027)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock
                };
                block.Entities.Add(item);
                return block;
            }
        }

        public static Block DatumTriangle
        {
            get
            {
                Block block = new Block("_DATUMBLANK");
                netDxf.Entities.Line item = new netDxf.Entities.Line(new Vector3(0.0, 0.57735027, 0.0), new Vector3(-1.0, 0.0, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Line line2 = new netDxf.Entities.Line(new Vector3(-1.0, 0.0, 0.0), new Vector3(0.0, -0.57735027, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line2);
                netDxf.Entities.Line line3 = new netDxf.Entities.Line(new Vector3(0.0, -0.57735027, 0.0), new Vector3(0.0, 0.57735027, 0.0)) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(line3);
                return block;
            }
        }

        public static Block Integral
        {
            get
            {
                Block block = new Block("_INTEGRAL");
                netDxf.Entities.Arc item = new netDxf.Entities.Arc(new Vector3(0.44488802, -0.09133463, 0.0), 0.45416667000000011, 101.9999999980395, 167.99999997991929) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(item);
                netDxf.Entities.Arc arc2 = new netDxf.Entities.Arc(new Vector3(-0.44488802, 0.09133463, 0.0), 0.45416667000000011, 282.00000002154269, 348.00000000342249) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock,
                    Lineweight = Lineweight.ByBlock
                };
                block.Entities.Add(arc2);
                return block;
            }
        }

        public static Block ArchitecturalTick
        {
            get
            {
                Block block = new Block("_ARCHTICK");
                List<LwPolylineVertex> vertexes = new List<LwPolylineVertex> {
                    new LwPolylineVertex(-0.5, -0.5),
                    new LwPolylineVertex(0.5, 0.5)
                };
                LwPolyline item = new LwPolyline(vertexes, false) {
                    Layer = Layer.Default,
                    Linetype = Linetype.ByBlock,
                    Color = AciColor.ByBlock
                };
                item.SetConstantWidth(0.15);
                block.Entities.Add(item);
                return block;
            }
        }
    }
}

