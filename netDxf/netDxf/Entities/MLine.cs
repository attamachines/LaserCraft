namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class MLine : EntityObject
    {
        private double scale;
        private MLineStyle style;
        private MLineJustification justification;
        private double elevation;
        private MLineFlags flags;
        private readonly List<MLineVertex> vertexes;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event MLineStyleChangedEventHandler MLineStyleChanged;

        public MLine() : this(new List<Vector2>())
        {
        }

        public MLine(IEnumerable<Vector2> vertexes) : this(vertexes, MLineStyle.Default, 1.0, false)
        {
        }

        public MLine(IEnumerable<Vector2> vertexes, bool isClosed) : this(vertexes, MLineStyle.Default, 1.0, isClosed)
        {
        }

        public MLine(IEnumerable<Vector2> vertexes, double scale) : this(vertexes, MLineStyle.Default, scale, false)
        {
        }

        public MLine(IEnumerable<Vector2> vertexes, MLineStyle style, double scale) : this(vertexes, style, scale, false)
        {
        }

        public MLine(IEnumerable<Vector2> vertexes, double scale, bool isClosed) : this(vertexes, MLineStyle.Default, scale, isClosed)
        {
        }

        public MLine(IEnumerable<Vector2> vertexes, MLineStyle style, double scale, bool isClosed) : base(EntityType.MLine, "MLINE")
        {
            this.scale = scale;
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            if (isClosed)
            {
                this.flags = MLineFlags.Closed | MLineFlags.Has;
            }
            else
            {
                this.flags = MLineFlags.Has;
            }
            this.style = style;
            this.justification = MLineJustification.Zero;
            this.elevation = 0.0;
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<MLineVertex>();
            foreach (Vector2 vector in vertexes)
            {
                this.vertexes.Add(new MLineVertex(vector, Vector2.Zero, Vector2.Zero, null));
            }
            this.Update();
        }

        public override object Clone()
        {
            MLine line = new MLine {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Elevation = this.elevation,
                Scale = this.scale,
                Justification = this.justification,
                Style = this.style,
                Flags = this.Flags
            };
            foreach (MLineVertex vertex in this.vertexes)
            {
                line.vertexes.Add((MLineVertex) vertex.Clone());
            }
            foreach (XData data in base.XData.Values)
            {
                line.XData.Add((XData) data.Clone());
            }
            return line;
        }

        protected virtual MLineStyle OnMLineStyleChangedEvent(MLineStyle oldMLineStyle, MLineStyle newMLineStyle)
        {
            MLineStyleChangedEventHandler mLineStyleChanged = this.MLineStyleChanged;
            if (mLineStyleChanged > null)
            {
                TableObjectChangedEventArgs<MLineStyle> e = new TableObjectChangedEventArgs<MLineStyle>(oldMLineStyle, newMLineStyle);
                mLineStyleChanged(this, e);
                return e.NewValue;
            }
            return newMLineStyle;
        }

        public void Update()
        {
            if (this.vertexes.Count != 0)
            {
                Vector2 unitY;
                double offset = 0.0;
                switch (this.justification)
                {
                    case MLineJustification.Top:
                        offset = this.style.Elements[this.style.Elements.Count - 1].Offset;
                        break;

                    case MLineJustification.Zero:
                        offset = 0.0;
                        break;

                    case MLineJustification.Bottom:
                        offset = this.style.Elements[0].Offset;
                        break;
                }
                if (this.vertexes[0].Location.Equals(this.vertexes[this.vertexes.Count - 1].Location))
                {
                    unitY = Vector2.UnitY;
                }
                else
                {
                    unitY = this.vertexes[0].Location - this.vertexes[this.vertexes.Count - 1].Location;
                    unitY.Normalize();
                }
                for (int i = 0; i < this.vertexes.Count; i++)
                {
                    Vector2 vector4;
                    Vector2 unitY;
                    Vector2 location = this.vertexes[i].Location;
                    if (i == 0)
                    {
                        if (this.vertexes[i + 1].Location.Equals(location))
                        {
                            unitY = Vector2.UnitY;
                        }
                        else
                        {
                            unitY = this.vertexes[i + 1].Location - location;
                            unitY.Normalize();
                        }
                        if (this.IsClosed)
                        {
                            vector4 = unitY - unitY;
                            vector4.Normalize();
                        }
                        else
                        {
                            vector4 = MathHelper.Transform(unitY, this.style.StartAngle * 0.017453292519943295, CoordinateSystem.Object, CoordinateSystem.World);
                        }
                    }
                    else if ((i + 1) == this.vertexes.Count)
                    {
                        if (this.IsClosed)
                        {
                            if (this.vertexes[0].Location.Equals(location))
                            {
                                unitY = Vector2.UnitY;
                            }
                            else
                            {
                                unitY = this.vertexes[0].Location - location;
                                unitY.Normalize();
                            }
                            vector4 = unitY - unitY;
                            vector4.Normalize();
                        }
                        else
                        {
                            unitY = unitY;
                            vector4 = MathHelper.Transform(unitY, this.style.EndAngle * 0.017453292519943295, CoordinateSystem.Object, CoordinateSystem.World);
                        }
                    }
                    else
                    {
                        if (this.vertexes[i + 1].Location.Equals(location))
                        {
                            unitY = Vector2.UnitY;
                        }
                        else
                        {
                            unitY = this.vertexes[i + 1].Location - location;
                            unitY.Normalize();
                        }
                        vector4 = unitY - unitY;
                        vector4.Normalize();
                    }
                    unitY = unitY;
                    List<double>[] distances = new List<double>[this.style.Elements.Count];
                    double num3 = Vector2.Angle(vector4);
                    double num4 = Vector2.Angle(unitY);
                    double num5 = Math.Cos(num3 + (1.5707963267948966 - num4));
                    for (int j = 0; j < this.style.Elements.Count; j++)
                    {
                        double num7 = (this.style.Elements[j].Offset + offset) / num5;
                        distances[j] = new List<double> { 
                            num7 * this.scale,
                            0.0
                        };
                    }
                    this.vertexes[i] = new MLineVertex(location, unitY, vector4, distances);
                }
            }
        }

        public List<MLineVertex> Vertexes =>
            this.vertexes;

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public double Scale
        {
            get => 
                this.scale;
            set => 
                (this.scale = value);
        }

        public bool IsClosed
        {
            get => 
                this.flags.HasFlag(MLineFlags.Closed);
            set
            {
                if (value)
                {
                    this.flags |= MLineFlags.Closed;
                }
                else
                {
                    this.flags &= ~MLineFlags.Closed;
                }
            }
        }

        public bool NoStartCaps
        {
            get => 
                this.flags.HasFlag(MLineFlags.NoStartCaps);
            set
            {
                if (value)
                {
                    this.flags |= MLineFlags.NoStartCaps;
                }
                else
                {
                    this.flags &= ~MLineFlags.NoStartCaps;
                }
            }
        }

        public bool NoEndCaps
        {
            get => 
                this.flags.HasFlag(MLineFlags.NoEndCaps);
            set
            {
                if (value)
                {
                    this.flags |= MLineFlags.NoEndCaps;
                }
                else
                {
                    this.flags &= ~MLineFlags.NoEndCaps;
                }
            }
        }

        public MLineJustification Justification
        {
            get => 
                this.justification;
            set => 
                (this.justification = value);
        }

        public MLineStyle Style
        {
            get => 
                this.style;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.style = this.OnMLineStyleChangedEvent(this.style, value);
            }
        }

        internal MLineFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }

        public delegate void MLineStyleChangedEventHandler(MLine sender, TableObjectChangedEventArgs<MLineStyle> e);
    }
}

