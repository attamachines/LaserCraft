namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Leader : EntityObject
    {
        private DimensionStyle style;
        private bool showArrowhead;
        private LeaderPathType pathType;
        private LeaderTextVerticalPosition textPosition;
        private readonly List<Vector2> vertexes;
        private EntityObject annotation;
        private bool hasHookLine;
        private AciColor lineColor;
        private double elevation;
        private Vector2 offset;
        private readonly DimensionStyleOverrideDictionary styleOverrides;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event DimensionStyleOverrideAddedEventHandler DimensionStyleOverrideAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event DimensionStyleOverrideRemovedEventHandler DimensionStyleOverrideRemoved;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LeaderStyleChangedEventHandler LeaderStyleChanged;

        public Leader(IEnumerable<Vector2> vertexes) : this(vertexes, DimensionStyle.Default)
        {
        }

        public Leader(Block block, IEnumerable<Vector2> vertexes) : this(block, vertexes, DimensionStyle.Default)
        {
        }

        public Leader(ToleranceEntry tolerance, IEnumerable<Vector2> vertexes) : this(tolerance, vertexes, DimensionStyle.Default)
        {
        }

        public Leader(IEnumerable<Vector2> vertexes, DimensionStyle style) : base(EntityType.Leader, "LEADER")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.vertexes.Count, "The leader vertexes list requires at least two points.");
            }
            this.style = style;
            this.hasHookLine = false;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.annotation = null;
            this.textPosition = LeaderTextVerticalPosition.Above;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += new DimensionStyleOverrideDictionary.BeforeAddItemEventHandler(this.StyleOverrides_BeforeAddItem);
            this.styleOverrides.AddItem += new DimensionStyleOverrideDictionary.AddItemEventHandler(this.StyleOverrides_AddItem);
            this.styleOverrides.BeforeRemoveItem += new DimensionStyleOverrideDictionary.BeforeRemoveItemEventHandler(this.StyleOverrides_BeforeRemoveItem);
            this.styleOverrides.RemoveItem += new DimensionStyleOverrideDictionary.RemoveItemEventHandler(this.StyleOverrides_RemoveItem);
        }

        public Leader(string text, IEnumerable<Vector2> vertexes) : this(text, vertexes, DimensionStyle.Default)
        {
        }

        public Leader(Block block, IEnumerable<Vector2> vertexes, DimensionStyle style) : base(EntityType.Leader, "LEADER")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.vertexes.Count, "The leader vertexes list requires at least two points.");
            }
            this.style = style;
            this.hasHookLine = false;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.textPosition = LeaderTextVerticalPosition.Above;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.annotation = this.BuildAnnotation(block);
            this.annotation.AddReactor(this);
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += new DimensionStyleOverrideDictionary.BeforeAddItemEventHandler(this.StyleOverrides_BeforeAddItem);
            this.styleOverrides.AddItem += new DimensionStyleOverrideDictionary.AddItemEventHandler(this.StyleOverrides_AddItem);
            this.styleOverrides.BeforeRemoveItem += new DimensionStyleOverrideDictionary.BeforeRemoveItemEventHandler(this.StyleOverrides_BeforeRemoveItem);
            this.styleOverrides.RemoveItem += new DimensionStyleOverrideDictionary.RemoveItemEventHandler(this.StyleOverrides_RemoveItem);
        }

        public Leader(ToleranceEntry tolerance, IEnumerable<Vector2> vertexes, DimensionStyle style) : base(EntityType.Leader, "LEADER")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.vertexes.Count, "The leader vertexes list requires at least two points.");
            }
            this.style = style;
            this.hasHookLine = false;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.textPosition = LeaderTextVerticalPosition.Above;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.annotation = this.BuildAnnotation(tolerance);
            this.annotation.AddReactor(this);
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += new DimensionStyleOverrideDictionary.BeforeAddItemEventHandler(this.StyleOverrides_BeforeAddItem);
            this.styleOverrides.AddItem += new DimensionStyleOverrideDictionary.AddItemEventHandler(this.StyleOverrides_AddItem);
            this.styleOverrides.BeforeRemoveItem += new DimensionStyleOverrideDictionary.BeforeRemoveItemEventHandler(this.StyleOverrides_BeforeRemoveItem);
            this.styleOverrides.RemoveItem += new DimensionStyleOverrideDictionary.RemoveItemEventHandler(this.StyleOverrides_RemoveItem);
        }

        public Leader(string text, IEnumerable<Vector2> vertexes, DimensionStyle style) : base(EntityType.Leader, "LEADER")
        {
            if (vertexes == null)
            {
                throw new ArgumentNullException("vertexes");
            }
            this.vertexes = new List<Vector2>(vertexes);
            if (this.vertexes.Count < 2)
            {
                throw new ArgumentOutOfRangeException("vertexes", this.vertexes.Count, "The leader vertexes list requires at least two points.");
            }
            this.style = style;
            this.hasHookLine = true;
            this.showArrowhead = true;
            this.pathType = LeaderPathType.StraightLineSegements;
            this.textPosition = LeaderTextVerticalPosition.Above;
            this.lineColor = AciColor.ByLayer;
            this.elevation = 0.0;
            this.offset = Vector2.Zero;
            this.annotation = this.BuildAnnotation(text);
            this.annotation.AddReactor(this);
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += new DimensionStyleOverrideDictionary.BeforeAddItemEventHandler(this.StyleOverrides_BeforeAddItem);
            this.styleOverrides.AddItem += new DimensionStyleOverrideDictionary.AddItemEventHandler(this.StyleOverrides_AddItem);
            this.styleOverrides.BeforeRemoveItem += new DimensionStyleOverrideDictionary.BeforeRemoveItemEventHandler(this.StyleOverrides_BeforeRemoveItem);
            this.styleOverrides.RemoveItem += new DimensionStyleOverrideDictionary.RemoveItemEventHandler(this.StyleOverrides_RemoveItem);
        }

        private Insert BuildAnnotation(Block block) => 
            new Insert(block, this.vertexes[this.vertexes.Count - 1]) { Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor };

        private Tolerance BuildAnnotation(ToleranceEntry tolerance) => 
            new Tolerance(tolerance, this.vertexes[this.vertexes.Count - 1]) { 
                Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor,
                Style = this.style
            };

        private MText BuildAnnotation(string text)
        {
            Vector2 vector = this.vertexes[this.vertexes.Count - 1] - this.vertexes[this.vertexes.Count - 2];
            int num = Math.Sign(vector.X);
            return new MText(text, this.vertexes[this.vertexes.Count - 1] + new Vector2((num * this.style.TextOffset) * this.style.DimScaleOverall, this.style.TextOffset * this.style.DimScaleOverall), this.style.TextHeight * this.style.DimScaleOverall, 0.0, this.style.TextStyle) { 
                Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor,
                AttachmentPoint = (num >= 0) ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight
            };
        }

        private void ChangeAnnotationCoordinateSystem(Vector3 newNormal, double newElevation)
        {
            if (this.annotation != null)
            {
                Vector3 vector2;
                Vector3 vector3;
                this.annotation.Normal = newNormal;
                switch (this.annotation.Type)
                {
                    case EntityType.Insert:
                        vector2 = MathHelper.Transform(((Insert) this.annotation).Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                        vector3 = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                        ((Insert) this.annotation).Position = vector3;
                        break;

                    case EntityType.MText:
                        vector2 = MathHelper.Transform(((MText) this.annotation).Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                        vector3 = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                        ((MText) this.annotation).Position = vector3;
                        break;

                    case EntityType.Text:
                        vector2 = MathHelper.Transform(((Text) this.annotation).Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                        vector3 = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                        ((Text) this.annotation).Position = vector3;
                        break;

                    case EntityType.Tolerance:
                        vector2 = MathHelper.Transform(((Tolerance) this.annotation).Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                        vector3 = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, newElevation), newNormal, CoordinateSystem.Object, CoordinateSystem.World);
                        ((Tolerance) this.annotation).Position = vector3;
                        break;
                }
            }
        }

        public override object Clone()
        {
            Leader leader = new Leader(this.vertexes) {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = this.Normal,
                IsVisible = base.IsVisible,
                Elevation = this.elevation,
                Style = (DimensionStyle) this.style.Clone(),
                ShowArrowhead = this.showArrowhead,
                PathType = this.pathType,
                Offset = this.offset,
                LineColor = this.lineColor,
                Annotation = (EntityObject) this.annotation.Clone(),
                hasHookLine = this.hasHookLine
            };
            foreach (XData data in base.XData.Values)
            {
                leader.XData.Add((XData) data.Clone());
            }
            return leader;
        }

        protected virtual DimensionStyle OnDimensionStyleChangedEvent(DimensionStyle oldStyle, DimensionStyle newStyle)
        {
            LeaderStyleChangedEventHandler leaderStyleChanged = this.LeaderStyleChanged;
            if (leaderStyleChanged > null)
            {
                TableObjectChangedEventArgs<DimensionStyle> e = new TableObjectChangedEventArgs<DimensionStyle>(oldStyle, newStyle);
                leaderStyleChanged(this, e);
                return e.NewValue;
            }
            return newStyle;
        }

        protected virtual void OnDimensionStyleOverrideAddedEvent(DimensionStyleOverride item)
        {
            DimensionStyleOverrideAddedEventHandler dimensionStyleOverrideAdded = this.DimensionStyleOverrideAdded;
            if (dimensionStyleOverrideAdded > null)
            {
                dimensionStyleOverrideAdded(this, new DimensionStyleOverrideChangeEventArgs(item));
            }
        }

        protected virtual void OnDimensionStyleOverrideRemovedEvent(DimensionStyleOverride item)
        {
            DimensionStyleOverrideRemovedEventHandler dimensionStyleOverrideRemoved = this.DimensionStyleOverrideRemoved;
            if (dimensionStyleOverrideRemoved > null)
            {
                dimensionStyleOverrideRemoved(this, new DimensionStyleOverrideChangeEventArgs(item));
            }
        }

        private void ResetAnnotationPosition()
        {
            Vector2 vector2;
            MText annotation;
            double num;
            if (this.vertexes.Count < 2)
            {
                throw new Exception("The leader vertexes list requires at least two points.");
            }
            if (this.annotation == null)
            {
                return;
            }
            Vector2 vector = this.vertexes[this.vertexes.Count - 1];
            switch (this.annotation.Type)
            {
                case EntityType.Text:
                {
                    Text annotation = (Text) this.annotation;
                    Vector2 vector4 = this.vertexes[this.vertexes.Count - 1] - this.vertexes[this.vertexes.Count - 2];
                    int num3 = Math.Sign(vector4.X);
                    vector2 = (vector + new Vector2((num3 * this.style.TextOffset) * this.style.DimScaleOverall, this.style.TextOffset * this.style.DimScaleOverall)) - this.offset;
                    annotation.Position = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    annotation.Alignment = (num3 >= 0) ? TextAlignment.BottomLeft : TextAlignment.BottomRight;
                    annotation.Height = this.style.TextHeight * this.style.DimScaleOverall;
                    annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
                    this.hasHookLine = true;
                    return;
                }
                case EntityType.Tolerance:
                {
                    Tolerance annotation = (Tolerance) this.annotation;
                    vector2 = vector - this.offset;
                    annotation.Position = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
                    this.hasHookLine = false;
                    return;
                }
                case EntityType.Insert:
                {
                    Insert annotation = (Insert) this.annotation;
                    vector2 = vector - this.offset;
                    annotation.Position = MathHelper.Transform(new Vector3(vector2.X, vector2.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
                    annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
                    this.hasHookLine = false;
                    return;
                }
                case EntityType.MText:
                {
                    annotation = (MText) this.annotation;
                    Vector2 vector3 = this.vertexes[this.vertexes.Count - 1] - this.vertexes[this.vertexes.Count - 2];
                    num = 0.0;
                    int num2 = Math.Sign(vector3.X);
                    if (this.TextVerticalPosition != LeaderTextVerticalPosition.Centered)
                    {
                        vector2 = vector + new Vector2((num2 * this.style.TextOffset) * this.style.DimScaleOverall, this.style.TextOffset * this.style.DimScaleOverall);
                        annotation.AttachmentPoint = (num2 >= 0) ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight;
                        goto Label_02A5;
                    }
                    if ((num2 < 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.TopLeft))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.TopRight;
                    }
                    else if ((num2 > 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.TopRight))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.TopLeft;
                    }
                    else if ((num2 < 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.MiddleLeft))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.MiddleRight;
                    }
                    else if ((num2 > 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.MiddleRight))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
                    }
                    else if ((num2 < 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.BottomLeft))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.BottomRight;
                    }
                    else if ((num2 > 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.BottomRight))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.BottomLeft;
                    }
                    switch (annotation.AttachmentPoint)
                    {
                        case MTextAttachmentPoint.TopLeft:
                        case MTextAttachmentPoint.MiddleLeft:
                        case MTextAttachmentPoint.BottomLeft:
                            num = -this.style.TextOffset * this.style.DimScaleOverall;
                            goto Label_024E;

                        case MTextAttachmentPoint.TopCenter:
                        case MTextAttachmentPoint.MiddleCenter:
                        case MTextAttachmentPoint.BottomCenter:
                            num = 0.0;
                            goto Label_024E;

                        case MTextAttachmentPoint.TopRight:
                        case MTextAttachmentPoint.MiddleRight:
                        case MTextAttachmentPoint.BottomRight:
                            num = this.style.TextOffset * this.style.DimScaleOverall;
                            goto Label_024E;
                    }
                    break;
                }
                default:
                    throw new Exception($"The entity type: {this.annotation.Type} not supported as a leader annotation.");
            }
        Label_024E:
            vector2 = vector;
        Label_02A5:
            vector2 -= this.offset;
            annotation.Position = MathHelper.Transform(new Vector3(vector2.X - num, vector2.Y, this.elevation), this.Normal, CoordinateSystem.Object, CoordinateSystem.World);
            annotation.Height = this.style.TextHeight * this.style.DimScaleOverall;
            annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
            this.hasHookLine = true;
        }

        private void ResetHookPosition()
        {
            Vector3 vector;
            MText annotation;
            Vector2 vector2;
            double num3;
            if (this.vertexes.Count < 2)
            {
                throw new Exception("The leader vertexes list requires at least two points.");
            }
            if (this.annotation == null)
            {
                return;
            }
            switch (this.annotation.Type)
            {
                case EntityType.Text:
                {
                    Text annotation = (Text) this.annotation;
                    vector = MathHelper.Transform(annotation.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    vector2 = this.vertexes[this.vertexes.Count - 2];
                    int num2 = Math.Sign((double) (vector.X - vector2.X));
                    vector -= new Vector3((num2 * this.style.TextOffset) * this.style.DimScaleOverall, this.style.TextOffset * this.style.DimScaleOverall, 0.0);
                    this.vertexes[this.vertexes.Count - 1] = new Vector2(vector.X, vector.Y) + this.offset;
                    annotation.Alignment = (num2 >= 0) ? TextAlignment.BottomLeft : TextAlignment.BottomRight;
                    annotation.Height = this.style.TextHeight * this.style.DimScaleOverall;
                    annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
                    this.hasHookLine = true;
                    return;
                }
                case EntityType.Tolerance:
                {
                    Tolerance annotation = (Tolerance) this.annotation;
                    vector = MathHelper.Transform(annotation.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    this.vertexes[this.vertexes.Count - 1] = new Vector2(vector.X, vector.Y) + this.offset;
                    annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
                    this.hasHookLine = false;
                    return;
                }
                case EntityType.Insert:
                {
                    Insert annotation = (Insert) this.annotation;
                    vector = MathHelper.Transform(annotation.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    this.vertexes[this.vertexes.Count - 1] = new Vector2(vector.X, vector.Y) + this.offset;
                    annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
                    this.hasHookLine = false;
                    return;
                }
                case EntityType.MText:
                {
                    annotation = (MText) this.annotation;
                    vector = MathHelper.Transform(annotation.Position, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);
                    vector2 = this.vertexes[this.vertexes.Count - 2];
                    int num = Math.Sign((double) (vector.X - vector2.X));
                    if (this.TextVerticalPosition != LeaderTextVerticalPosition.Centered)
                    {
                        vector -= new Vector3((num * this.style.TextOffset) * this.style.DimScaleOverall, this.style.TextOffset * this.style.DimScaleOverall, 0.0);
                        annotation.AttachmentPoint = (num >= 0) ? MTextAttachmentPoint.BottomLeft : MTextAttachmentPoint.BottomRight;
                        this.vertexes[this.vertexes.Count - 1] = new Vector2(vector.X, vector.Y) + this.offset;
                        goto Label_0301;
                    }
                    if ((num < 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.TopLeft))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.TopRight;
                    }
                    else if ((num > 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.TopRight))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.TopLeft;
                    }
                    else if ((num < 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.MiddleLeft))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.MiddleRight;
                    }
                    else if ((num > 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.MiddleRight))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.MiddleLeft;
                    }
                    else if ((num < 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.BottomLeft))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.BottomRight;
                    }
                    else if ((num > 0) && (annotation.AttachmentPoint == MTextAttachmentPoint.BottomRight))
                    {
                        annotation.AttachmentPoint = MTextAttachmentPoint.BottomLeft;
                    }
                    num3 = 0.0;
                    switch (annotation.AttachmentPoint)
                    {
                        case MTextAttachmentPoint.TopLeft:
                        case MTextAttachmentPoint.MiddleLeft:
                        case MTextAttachmentPoint.BottomLeft:
                            num3 = -this.style.TextOffset * this.style.DimScaleOverall;
                            goto Label_022F;

                        case MTextAttachmentPoint.TopCenter:
                        case MTextAttachmentPoint.MiddleCenter:
                        case MTextAttachmentPoint.BottomCenter:
                            num3 = 0.0;
                            goto Label_022F;

                        case MTextAttachmentPoint.TopRight:
                        case MTextAttachmentPoint.MiddleRight:
                        case MTextAttachmentPoint.BottomRight:
                            num3 = this.style.TextOffset * this.style.DimScaleOverall;
                            goto Label_022F;
                    }
                    break;
                }
                default:
                    throw new Exception($"The entity type: {this.annotation.Type} not supported as a leader annotation.");
            }
        Label_022F:
            this.vertexes[this.vertexes.Count - 1] = new Vector2(vector.X + num3, vector.Y) + this.offset;
        Label_0301:
            annotation.Height = this.style.TextHeight * this.style.DimScaleOverall;
            annotation.Color = this.style.TextColor.IsByBlock ? AciColor.ByLayer : this.style.TextColor;
            this.hasHookLine = true;
        }

        private void StyleOverrides_AddItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
            this.OnDimensionStyleOverrideAddedEvent(e.Item);
        }

        private void StyleOverrides_BeforeAddItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
            if (sender.TryGetValue(e.Item.Type, out DimensionStyleOverride @override) && (@override.Value == e.Item.Value))
            {
                e.Cancel = true;
            }
        }

        private void StyleOverrides_BeforeRemoveItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
        }

        private void StyleOverrides_RemoveItem(DimensionStyleOverrideDictionary sender, DimensionStyleOverrideDictionaryEventArgs e)
        {
            this.OnDimensionStyleOverrideRemovedEvent(e.Item);
        }

        public void Update(bool resetAnnotationPosition)
        {
            if (resetAnnotationPosition)
            {
                this.ResetAnnotationPosition();
            }
            else
            {
                this.ResetHookPosition();
            }
        }

        public Vector3 Normal
        {
            get => 
                base.Normal;
            set
            {
                this.ChangeAnnotationCoordinateSystem(value, this.elevation);
                base.Normal = value;
            }
        }

        public DimensionStyle Style
        {
            get => 
                this.style;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.style = this.OnDimensionStyleChangedEvent(this.style, value);
            }
        }

        public DimensionStyleOverrideDictionary StyleOverrides =>
            this.styleOverrides;

        public bool ShowArrowhead
        {
            get => 
                this.showArrowhead;
            set => 
                (this.showArrowhead = value);
        }

        public LeaderPathType PathType
        {
            get => 
                this.pathType;
            set => 
                (this.pathType = value);
        }

        public List<Vector2> Vertexes =>
            this.vertexes;

        public EntityObject Annotation
        {
            get => 
                this.annotation;
            set
            {
                if ((value > null) && ((((value.Type != EntityType.MText) && (value.Type != EntityType.Text)) && (value.Type != EntityType.Insert)) && (value.Type != EntityType.Tolerance)))
                {
                    throw new ArgumentException("Only MText, Text, Insert, and Tolerance entities are supported as a leader annotation.");
                }
                if (this.annotation != value)
                {
                    if (this.annotation > null)
                    {
                        this.annotation.RemoveReactor(this);
                    }
                    if (value > null)
                    {
                        value.AddReactor(this);
                    }
                    this.annotation = value;
                }
            }
        }

        public Vector2 Hook
        {
            get => 
                this.vertexes[this.vertexes.Count - 1];
            set => 
                (this.vertexes[this.vertexes.Count - 1] = value);
        }

        public bool HasHookline =>
            this.hasHookLine;

        public LeaderTextVerticalPosition TextVerticalPosition
        {
            get => 
                this.textPosition;
            set => 
                (this.textPosition = value);
        }

        public AciColor LineColor
        {
            get => 
                this.lineColor;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.lineColor = value;
            }
        }

        public double Elevation
        {
            get => 
                this.elevation;
            set
            {
                this.ChangeAnnotationCoordinateSystem(this.Normal, value);
                this.elevation = value;
            }
        }

        public Vector2 Offset
        {
            get => 
                this.offset;
            set => 
                (this.offset = value);
        }

        public delegate void DimensionStyleOverrideAddedEventHandler(Leader sender, DimensionStyleOverrideChangeEventArgs e);

        public delegate void DimensionStyleOverrideRemovedEventHandler(Leader sender, DimensionStyleOverrideChangeEventArgs e);

        public delegate void LeaderStyleChangedEventHandler(Leader sender, TableObjectChangedEventArgs<DimensionStyle> e);
    }
}

