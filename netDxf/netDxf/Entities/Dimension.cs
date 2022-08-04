namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Collections;
    using netDxf.Tables;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class Dimension : EntityObject
    {
        private Vector3 definitionPoint;
        private Vector3 midTextPoint;
        private DimensionStyle style;
        private readonly netDxf.Entities.DimensionType dimensionType;
        private MTextAttachmentPoint attachmentPoint;
        private MTextLineSpacingStyle lineSpacingStyle;
        private netDxf.Blocks.Block block;
        private string userText;
        private double lineSpacing;
        private double elevation;
        private readonly DimensionStyleOverrideDictionary styleOverrides;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event DimensionBlockChangedEventHandler DimensionBlockChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event DimensionStyleChangedEventHandler DimensionStyleChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event DimensionStyleOverrideAddedEventHandler DimensionStyleOverrideAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event DimensionStyleOverrideRemovedEventHandler DimensionStyleOverrideRemoved;

        protected Dimension(netDxf.Entities.DimensionType type) : base(EntityType.Dimension, "DIMENSION")
        {
            this.definitionPoint = Vector3.Zero;
            this.midTextPoint = Vector3.Zero;
            this.dimensionType = type;
            this.attachmentPoint = MTextAttachmentPoint.MiddleCenter;
            this.lineSpacingStyle = MTextLineSpacingStyle.AtLeast;
            this.lineSpacing = 1.0;
            this.block = null;
            this.style = DimensionStyle.Default;
            this.userText = null;
            this.elevation = 0.0;
            this.styleOverrides = new DimensionStyleOverrideDictionary();
            this.styleOverrides.BeforeAddItem += new DimensionStyleOverrideDictionary.BeforeAddItemEventHandler(this.StyleOverrides_BeforeAddItem);
            this.styleOverrides.AddItem += new DimensionStyleOverrideDictionary.AddItemEventHandler(this.StyleOverrides_AddItem);
            this.styleOverrides.BeforeRemoveItem += new DimensionStyleOverrideDictionary.BeforeRemoveItemEventHandler(this.StyleOverrides_BeforeRemoveItem);
            this.styleOverrides.RemoveItem += new DimensionStyleOverrideDictionary.RemoveItemEventHandler(this.StyleOverrides_RemoveItem);
        }

        internal abstract netDxf.Blocks.Block BuildBlock(string name);
        protected virtual netDxf.Blocks.Block OnDimensionBlockChangedEvent(netDxf.Blocks.Block oldBlock, netDxf.Blocks.Block newBlock)
        {
            DimensionBlockChangedEventHandler dimensionBlockChanged = this.DimensionBlockChanged;
            if (dimensionBlockChanged > null)
            {
                TableObjectChangedEventArgs<netDxf.Blocks.Block> e = new TableObjectChangedEventArgs<netDxf.Blocks.Block>(oldBlock, newBlock);
                dimensionBlockChanged(this, e);
                return e.NewValue;
            }
            return newBlock;
        }

        protected virtual DimensionStyle OnDimensionStyleChangedEvent(DimensionStyle oldStyle, DimensionStyle newStyle)
        {
            DimensionStyleChangedEventHandler dimensionStyleChanged = this.DimensionStyleChanged;
            if (dimensionStyleChanged > null)
            {
                TableObjectChangedEventArgs<DimensionStyle> e = new TableObjectChangedEventArgs<DimensionStyle>(oldStyle, newStyle);
                dimensionStyleChanged(this, e);
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

        public void Update()
        {
            if (this.block != null)
            {
                netDxf.Blocks.Block newBlock = this.BuildBlock(this.block.Name);
                this.block = this.OnDimensionBlockChangedEvent(this.block, newBlock);
            }
        }

        internal Vector3 DefinitionPoint
        {
            get => 
                this.definitionPoint;
            set => 
                (this.definitionPoint = value);
        }

        internal Vector3 MidTextPoint
        {
            get => 
                this.midTextPoint;
            set => 
                (this.midTextPoint = value);
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

        public netDxf.Entities.DimensionType DimensionType =>
            this.dimensionType;

        public abstract double Measurement { get; }

        public MTextAttachmentPoint AttachmentPoint
        {
            get => 
                this.attachmentPoint;
            set => 
                (this.attachmentPoint = value);
        }

        public MTextLineSpacingStyle LineSpacingStyle
        {
            get => 
                this.lineSpacingStyle;
            set => 
                (this.lineSpacingStyle = value);
        }

        public double LineSpacingFactor
        {
            get => 
                this.lineSpacing;
            set
            {
                if ((value < 0.25) || (value > 4.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The line spacing factor valid values range from 0.25 to 4.00");
                }
                this.lineSpacing = value;
            }
        }

        public netDxf.Blocks.Block Block
        {
            get => 
                this.block;
            internal set => 
                (this.block = value);
        }

        public string UserText
        {
            get => 
                this.userText;
            set => 
                (this.userText = value);
        }

        public double Elevation
        {
            get => 
                this.elevation;
            set => 
                (this.elevation = value);
        }

        public delegate void DimensionBlockChangedEventHandler(Dimension sender, TableObjectChangedEventArgs<Block> e);

        public delegate void DimensionStyleChangedEventHandler(Dimension sender, TableObjectChangedEventArgs<DimensionStyle> e);

        public delegate void DimensionStyleOverrideAddedEventHandler(Dimension sender, DimensionStyleOverrideChangeEventArgs e);

        public delegate void DimensionStyleOverrideRemovedEventHandler(Dimension sender, DimensionStyleOverrideChangeEventArgs e);
    }
}

