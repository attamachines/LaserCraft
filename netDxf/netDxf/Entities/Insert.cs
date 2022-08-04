namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Tables;
    using netDxf.Units;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Insert : EntityObject
    {
        private readonly netDxf.Entities.EndSequence endSequence;
        private netDxf.Blocks.Block block;
        private Vector3 position;
        private Vector3 scale;
        private double rotation;
        private AttributeCollection attributes;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AttributeAddedEventHandler AttributeAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AttributeRemovedEventHandler AttributeRemoved;

        public Insert(netDxf.Blocks.Block block) : this(block, Vector3.Zero)
        {
        }

        internal Insert(List<netDxf.Entities.Attribute> attributes) : base(EntityType.Insert, "INSERT")
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            this.attributes = new AttributeCollection(attributes);
            foreach (netDxf.Entities.Attribute attribute in this.attributes)
            {
                if (attribute.Owner > null)
                {
                    throw new ArgumentException("The attributes list contains at least an attribute that already has an owner.", "attributes");
                }
                attribute.Owner = this;
            }
            this.endSequence = new netDxf.Entities.EndSequence(this);
        }

        public Insert(netDxf.Blocks.Block block, Vector2 position) : this(block, new Vector3(position.X, position.Y, 0.0))
        {
        }

        public Insert(netDxf.Blocks.Block block, Vector3 position) : base(EntityType.Insert, "INSERT")
        {
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            this.block = block;
            this.position = position;
            this.scale = new Vector3(1.0);
            this.rotation = 0.0;
            this.endSequence = new netDxf.Entities.EndSequence(this);
            List<netDxf.Entities.Attribute> attributes = new List<netDxf.Entities.Attribute>(block.AttributeDefinitions.Count);
            foreach (AttributeDefinition definition in block.AttributeDefinitions.Values)
            {
                netDxf.Entities.Attribute item = new netDxf.Entities.Attribute(definition) {
                    Position = (definition.Position + this.position) - this.block.Origin,
                    Owner = this
                };
                attributes.Add(item);
            }
            this.attributes = new AttributeCollection(attributes);
        }

        internal override long AsignHandle(long entityNumber)
        {
            entityNumber = this.endSequence.AsignHandle(entityNumber);
            foreach (netDxf.Entities.Attribute attribute in this.attributes)
            {
                entityNumber = attribute.AsignHandle(entityNumber);
            }
            return base.AsignHandle(entityNumber);
        }

        public override object Clone()
        {
            List<netDxf.Entities.Attribute> attributes = new List<netDxf.Entities.Attribute>();
            foreach (netDxf.Entities.Attribute attribute in this.attributes)
            {
                attributes.Add((netDxf.Entities.Attribute) attribute.Clone());
            }
            Insert insert = new Insert(attributes) {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Position = this.position,
                Block = (netDxf.Blocks.Block) this.block.Clone(),
                Scale = this.scale,
                Rotation = this.rotation
            };
            foreach (XData data in base.XData.Values)
            {
                insert.XData.Add((XData) data.Clone());
            }
            return insert;
        }

        public Matrix3 GetTransformation(DrawingUnits insertionUnits)
        {
            double num = UnitHelper.ConversionFactor(this.Block.Record.Units, insertionUnits);
            Matrix3 matrix = MathHelper.ArbitraryAxis(base.Normal) * Matrix3.RotationZ(this.rotation * 0.017453292519943295);
            return (matrix * Matrix3.Scale(this.scale * num));
        }

        protected virtual void OnAttributeAddedEvent(netDxf.Entities.Attribute item)
        {
            AttributeAddedEventHandler attributeAdded = this.AttributeAdded;
            if (attributeAdded > null)
            {
                attributeAdded(this, new AttributeChangeEventArgs(item));
            }
        }

        protected virtual void OnAttributeRemovedEvent(netDxf.Entities.Attribute item)
        {
            AttributeRemovedEventHandler attributeRemoved = this.AttributeRemoved;
            if (attributeRemoved > null)
            {
                attributeRemoved(this, new AttributeChangeEventArgs(item));
            }
        }

        public void Sync()
        {
            List<netDxf.Entities.Attribute> attributes = new List<netDxf.Entities.Attribute>(this.block.AttributeDefinitions.Count);
            foreach (netDxf.Entities.Attribute attribute in this.attributes)
            {
                this.OnAttributeRemovedEvent(attribute);
                attribute.Handle = null;
                attribute.Owner = null;
            }
            foreach (AttributeDefinition definition in this.block.AttributeDefinitions.Values)
            {
                netDxf.Entities.Attribute item = new netDxf.Entities.Attribute(definition) {
                    Owner = this
                };
                attributes.Add(item);
                this.OnAttributeAddedEvent(item);
            }
            this.attributes = new AttributeCollection(attributes);
            this.TransformAttributes();
        }

        public void TransformAttributes()
        {
            if (this.attributes.Count != 0)
            {
                DrawingUnits unitless;
                if (base.Owner == null)
                {
                    unitless = DrawingUnits.Unitless;
                }
                else
                {
                    unitless = (base.Owner.Record.Layout == null) ? base.Owner.Record.Units : base.Owner.Record.Owner.Owner.DrawingVariables.InsUnits;
                }
                Vector3 vector = this.scale * UnitHelper.ConversionFactor(this.block.Record.Units, unitless);
                Matrix3 transformation = this.GetTransformation(unitless);
                foreach (netDxf.Entities.Attribute attribute in this.attributes)
                {
                    AttributeDefinition definition = attribute.Definition;
                    if (definition != null)
                    {
                        Vector3 vector2 = (Vector3) (transformation * (definition.Position - this.block.Origin));
                        attribute.Position = this.position + vector2;
                        Vector2 point = new Vector2(definition.WidthFactor, 0.0);
                        point = MathHelper.Transform(point, definition.Rotation * 0.017453292519943295, CoordinateSystem.Object, CoordinateSystem.World);
                        Vector3 vector4 = MathHelper.Transform(new Vector3(point.X, point.Y, 0.0), definition.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                        Vector3 u = (Vector3) (transformation * vector4);
                        Vector2 vector6 = new Vector2(0.0, definition.Height);
                        vector6 = MathHelper.Transform(vector6, definition.Rotation * 0.017453292519943295, CoordinateSystem.Object, CoordinateSystem.World);
                        Vector3 vector7 = MathHelper.Transform(new Vector3(vector6.X, vector6.Y, 0.0), definition.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                        Vector3 v = (Vector3) (transformation * vector7);
                        Vector3 vector9 = Vector3.CrossProduct(u, v);
                        attribute.Normal = vector9;
                        double num = MathHelper.PointLineDistance(v, Vector3.Zero, Vector3.Normalize(u));
                        attribute.Height = num;
                        double num2 = Vector2.Angle(new Vector2(point.X * vector.X, point.Y * vector.Y)) * 57.295779513082323;
                        if (Vector3.Equals(definition.Normal, Vector3.UnitZ))
                        {
                            attribute.Rotation = this.rotation + num2;
                            attribute.WidthFactor = definition.WidthFactor;
                            attribute.ObliqueAngle = definition.ObliqueAngle;
                        }
                        else
                        {
                            attribute.Rotation = num2;
                            attribute.WidthFactor = definition.WidthFactor;
                            attribute.ObliqueAngle = definition.ObliqueAngle;
                        }
                    }
                }
            }
        }

        public AttributeCollection Attributes =>
            this.attributes;

        public netDxf.Blocks.Block Block
        {
            get => 
                this.block;
            internal set => 
                (this.block = value);
        }

        public Vector3 Position
        {
            get => 
                this.position;
            set => 
                (this.position = value);
        }

        public Vector3 Scale
        {
            get => 
                this.scale;
            set => 
                (this.scale = value);
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = MathHelper.NormalizeAngle(value));
        }

        internal netDxf.Entities.EndSequence EndSequence =>
            this.endSequence;

        public delegate void AttributeAddedEventHandler(Insert sender, AttributeChangeEventArgs e);

        public delegate void AttributeRemovedEventHandler(Insert sender, AttributeChangeEventArgs e);
    }
}

