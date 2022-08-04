namespace netDxf.Blocks
{
    using netDxf;
    using netDxf.Collections;
    using netDxf.Entities;
    using netDxf.Header;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Block : TableObject
    {
        private readonly EntityCollection entities;
        private readonly AttributeDefinitionDictionary attributes;
        private string description;
        private readonly EndBlock end;
        private BlockTypeFlags flags;
        private netDxf.Tables.Layer layer;
        private Vector3 origin;
        private readonly bool readOnly;
        private readonly string xrefFile;
        public const string DefaultModelSpaceName = "*Model_Space";
        public const string DefaultPaperSpaceName = "*Paper_Space";

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AttributeDefinitionAddedEventHandler AttributeDefinitionAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event AttributeDefinitionRemovedEventHandler AttributeDefinitionRemoved;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EntityAddedEventHandler EntityAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EntityRemovedEventHandler EntityRemoved;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LayerChangedEventHandler LayerChanged;

        public Block(string name) : this(name, true, null, null)
        {
        }

        public Block(string name, IEnumerable<EntityObject> entities) : this(name, true, entities, null)
        {
        }

        public Block(string name, string xrefFile) : this(name, xrefFile, false)
        {
        }

        public Block(string name, IEnumerable<EntityObject> entities, IEnumerable<AttributeDefinition> attributes) : this(name, true, entities, attributes)
        {
        }

        public Block(string name, string xrefFile, bool overlay) : this(name, true, null, null)
        {
            if (string.IsNullOrEmpty(xrefFile))
            {
                throw new ArgumentNullException("xrefFile");
            }
            this.readOnly = true;
            this.xrefFile = xrefFile;
            this.flags = BlockTypeFlags.ResolvedExternalReference | BlockTypeFlags.XRef;
            if (overlay)
            {
                this.flags |= BlockTypeFlags.XRefOverlay;
            }
        }

        internal Block(string name, bool checkName, IEnumerable<EntityObject> entities, IEnumerable<AttributeDefinition> attributes) : base(name, "BLOCK", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            base.IsReserved = string.Equals(name, "*Model_Space", StringComparison.OrdinalIgnoreCase);
            this.readOnly = base.IsReserved || name.StartsWith("*Paper_Space", StringComparison.OrdinalIgnoreCase);
            this.description = string.Empty;
            this.origin = Vector3.Zero;
            this.layer = netDxf.Tables.Layer.Default;
            this.xrefFile = string.Empty;
            this.entities = new EntityCollection();
            this.entities.BeforeAddItem += new EntityCollection.BeforeAddItemEventHandler(this.Entities_BeforeAddItem);
            this.entities.AddItem += new EntityCollection.AddItemEventHandler(this.Entities_AddItem);
            this.entities.BeforeRemoveItem += new EntityCollection.BeforeRemoveItemEventHandler(this.Entities_BeforeRemoveItem);
            this.entities.RemoveItem += new EntityCollection.RemoveItemEventHandler(this.Entities_RemoveItem);
            this.attributes = new AttributeDefinitionDictionary();
            this.attributes.BeforeAddItem += new AttributeDefinitionDictionary.BeforeAddItemEventHandler(this.AttributeDefinitions_BeforeAddItem);
            this.attributes.AddItem += new AttributeDefinitionDictionary.AddItemEventHandler(this.AttributeDefinitions_ItemAdd);
            this.attributes.BeforeRemoveItem += new AttributeDefinitionDictionary.BeforeRemoveItemEventHandler(this.AttributeDefinitions_BeforeRemoveItem);
            this.attributes.RemoveItem += new AttributeDefinitionDictionary.RemoveItemEventHandler(this.AttributeDefinitions_RemoveItem);
            base.Owner = new BlockRecord(name);
            this.flags = BlockTypeFlags.None;
            this.end = new EndBlock(this);
            if (entities > null)
            {
                this.entities.AddRange(entities);
            }
            if (attributes > null)
            {
                this.attributes.AddRange(attributes);
            }
        }

        internal override long AsignHandle(long entityNumber)
        {
            entityNumber = base.Owner.AsignHandle(entityNumber);
            entityNumber = this.end.AsignHandle(entityNumber);
            foreach (AttributeDefinition definition in this.attributes.Values)
            {
                entityNumber = definition.AsignHandle(entityNumber);
            }
            return base.AsignHandle(entityNumber);
        }

        private void AttributeDefinitions_BeforeAddItem(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
        {
            if (this.readOnly)
            {
                e.Cancel = true;
            }
            else if (e.Item.Owner > null)
            {
                if (this.Record.Owner == null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void AttributeDefinitions_BeforeRemoveItem(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
        {
            e.Cancel = (e.Item.Owner != this) || this.readOnly;
        }

        private void AttributeDefinitions_ItemAdd(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
        {
            if (!this.readOnly)
            {
                this.OnAttributeDefinitionAddedEvent(e.Item);
                e.Item.Owner = this;
                this.flags |= BlockTypeFlags.NonConstantAttributeDefinitions;
            }
        }

        private void AttributeDefinitions_RemoveItem(AttributeDefinitionDictionary sender, AttributeDefinitionDictionaryEventArgs e)
        {
            if (!this.readOnly)
            {
                this.OnAttributeDefinitionRemovedEvent(e.Item);
                e.Item.Owner = null;
                if (this.attributes.Count == 0)
                {
                    this.flags &= ~BlockTypeFlags.NonConstantAttributeDefinitions;
                }
            }
        }

        public override object Clone() => 
            Clone(this, this.Name, !this.flags.HasFlag(BlockTypeFlags.AnonymousBlock));

        public override TableObject Clone(string newName) => 
            Clone(this, newName, true);

        private static TableObject Clone(Block block, string newName, bool checkName)
        {
            if (block.ReadOnly)
            {
                throw new ArgumentException("Read only blocks cannot be cloned.", "block");
            }
            Block block2 = new Block(newName, checkName, null, null) {
                Description = block.description,
                Flags = block.flags,
                Layer = (netDxf.Tables.Layer) block.Layer.Clone(),
                Origin = block.origin
            };
            if (checkName)
            {
                block2.Flags &= ~BlockTypeFlags.AnonymousBlock;
            }
            foreach (EntityObject obj2 in block.entities)
            {
                block2.entities.Add((EntityObject) obj2.Clone());
            }
            foreach (AttributeDefinition definition in block.attributes.Values)
            {
                block2.attributes.Add((AttributeDefinition) definition.Clone());
            }
            return block2;
        }

        public static Block Create(DxfDocument doc, string name)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            Block block = new Block(name) {
                Origin = doc.DrawingVariables.InsBase
            };
            block.Record.Units = doc.DrawingVariables.InsUnits;
            List<DxfObject> references = doc.Layouts.GetReferences("Model");
            foreach (DxfObject obj2 in references)
            {
                EntityObject obj3 = obj2 as EntityObject;
                if (obj3 != null)
                {
                    EntityObject item = (EntityObject) obj3.Clone();
                    AttributeDefinition definition = item as AttributeDefinition;
                    if (definition > null)
                    {
                        block.AttributeDefinitions.Add(definition);
                    }
                    else
                    {
                        block.Entities.Add(item);
                    }
                }
            }
            return block;
        }

        private void Entities_AddItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            if (!this.readOnly)
            {
                if (e.Item.Type == EntityType.Hatch)
                {
                    Hatch item = (Hatch) e.Item;
                    foreach (HatchBoundaryPath path in item.BoundaryPaths)
                    {
                        this.Hatch_BoundaryPathAdded(item, new ObservableCollectionEventArgs<HatchBoundaryPath>(path));
                    }
                    item.HatchBoundaryPathAdded += new Hatch.HatchBoundaryPathAddedEventHandler(this.Hatch_BoundaryPathAdded);
                    item.HatchBoundaryPathRemoved += new Hatch.HatchBoundaryPathRemovedEventHandler(this.Hatch_BoundaryPathRemoved);
                }
                this.OnEntityAddedEvent(e.Item);
                e.Item.Owner = this;
            }
        }

        private void Entities_BeforeAddItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            if (e.Item == null)
            {
                e.Cancel = true;
            }
            else if (this.entities.Contains(e.Item))
            {
                e.Cancel = true;
            }
            else if (this.readOnly)
            {
                e.Cancel = true;
            }
            else if (e.Item is AttributeDefinition)
            {
                e.Cancel = true;
            }
            else if (e.Item.Owner > null)
            {
                if (this.Record.Owner == null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Entities_BeforeRemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            e.Cancel = (e.Item.Owner != this) || this.readOnly;
        }

        private void Entities_RemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            if (!this.readOnly)
            {
                if (e.Item.Type == EntityType.Hatch)
                {
                    Hatch item = (Hatch) e.Item;
                    foreach (HatchBoundaryPath path in item.BoundaryPaths)
                    {
                        this.Hatch_BoundaryPathRemoved(item, new ObservableCollectionEventArgs<HatchBoundaryPath>(path));
                    }
                    item.HatchBoundaryPathAdded -= new Hatch.HatchBoundaryPathAddedEventHandler(this.Hatch_BoundaryPathAdded);
                    item.HatchBoundaryPathRemoved -= new Hatch.HatchBoundaryPathRemovedEventHandler(this.Hatch_BoundaryPathRemoved);
                }
                this.OnEntityRemovedEvent(e.Item);
                e.Item.Owner = null;
            }
        }

        private void Hatch_BoundaryPathAdded(Hatch sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
        {
            foreach (EntityObject obj2 in e.Item.Entities)
            {
                if (obj2.Owner > null)
                {
                    if (obj2.Owner != this)
                    {
                        throw new ArgumentException("The HatchBoundaryPath entity and the hatch must belong to the same block. Clone it instead.");
                    }
                }
                else
                {
                    this.Entities.Add(obj2);
                }
            }
        }

        private void Hatch_BoundaryPathRemoved(Hatch sender, ObservableCollectionEventArgs<HatchBoundaryPath> e)
        {
            this.Entities.Remove(e.Item.Entities);
        }

        public static Block Load(string file) => 
            Load(file, null);

        public static Block Load(string file, string name)
        {
            DxfDocument doc = DxfDocument.Load(file);
            string str = string.IsNullOrEmpty(name) ? doc.Name : name;
            return Create(doc, str);
        }

        protected virtual void OnAttributeDefinitionAddedEvent(EntityObject item)
        {
            AttributeDefinitionAddedEventHandler attributeDefinitionAdded = this.AttributeDefinitionAdded;
            if (attributeDefinitionAdded > null)
            {
                attributeDefinitionAdded(this, new BlockEntityChangeEventArgs(item));
            }
        }

        protected virtual void OnAttributeDefinitionRemovedEvent(EntityObject item)
        {
            AttributeDefinitionRemovedEventHandler attributeDefinitionRemoved = this.AttributeDefinitionRemoved;
            if (attributeDefinitionRemoved > null)
            {
                attributeDefinitionRemoved(this, new BlockEntityChangeEventArgs(item));
            }
        }

        protected virtual void OnEntityAddedEvent(EntityObject item)
        {
            EntityAddedEventHandler entityAdded = this.EntityAdded;
            if (entityAdded > null)
            {
                entityAdded(this, new BlockEntityChangeEventArgs(item));
            }
        }

        protected virtual void OnEntityRemovedEvent(EntityObject item)
        {
            EntityRemovedEventHandler entityRemoved = this.EntityRemoved;
            if (entityRemoved > null)
            {
                entityRemoved(this, new BlockEntityChangeEventArgs(item));
            }
        }

        protected virtual netDxf.Tables.Layer OnLayerChangedEvent(netDxf.Tables.Layer oldLayer, netDxf.Tables.Layer newLayer)
        {
            LayerChangedEventHandler layerChanged = this.LayerChanged;
            if (layerChanged > null)
            {
                TableObjectChangedEventArgs<netDxf.Tables.Layer> e = new TableObjectChangedEventArgs<netDxf.Tables.Layer>(oldLayer, newLayer);
                layerChanged(this, e);
                return e.NewValue;
            }
            return newLayer;
        }

        public bool Save(string file, DxfVersion version) => 
            this.Save(file, version, false);

        public bool Save(string file, DxfVersion version, bool isBinary)
        {
            DxfDocument document = new DxfDocument(version) {
                DrawingVariables = { 
                    InsBase = this.origin,
                    InsUnits = this.Record.Units
                }
            };
            foreach (EntityObject obj2 in this.entities)
            {
                document.AddEntity((EntityObject) obj2.Clone());
            }
            foreach (AttributeDefinition definition in this.attributes.Values)
            {
                document.AddEntity((EntityObject) definition.Clone());
            }
            return document.Save(file, isBinary);
        }

        public static Block ModelSpace =>
            new Block("*Model_Space", false, null, null);

        public static Block PaperSpace =>
            new Block("*Paper_Space", false, null, null);

        public string Name
        {
            get => 
                base.Name;
            set
            {
                if (base.Name.StartsWith("*"))
                {
                    throw new ArgumentException("Blocks for internal use cannot be renamed.", "value");
                }
                base.Name = value;
                this.Record.Name = value;
            }
        }

        public string Description
        {
            get => 
                this.description;
            set
            {
                if (!this.readOnly)
                {
                    this.description = string.IsNullOrEmpty(value) ? string.Empty : value;
                }
            }
        }

        public Vector3 Origin
        {
            get => 
                this.origin;
            set
            {
                if (!this.readOnly)
                {
                    this.origin = value;
                }
            }
        }

        public netDxf.Tables.Layer Layer
        {
            get => 
                this.layer;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.layer = this.OnLayerChangedEvent(this.layer, value);
            }
        }

        public EntityCollection Entities =>
            this.entities;

        public AttributeDefinitionDictionary AttributeDefinitions =>
            this.attributes;

        public BlockRecord Record =>
            ((BlockRecord) base.Owner);

        public BlockTypeFlags Flags
        {
            get => 
                this.flags;
            internal set => 
                (this.flags = value);
        }

        public bool ReadOnly =>
            this.readOnly;

        public string XrefFile =>
            this.xrefFile;

        public bool IsXRef =>
            this.flags.HasFlag(BlockTypeFlags.XRef);

        internal EndBlock End =>
            this.end;

        public delegate void AttributeDefinitionAddedEventHandler(Block sender, BlockEntityChangeEventArgs e);

        public delegate void AttributeDefinitionRemovedEventHandler(Block sender, BlockEntityChangeEventArgs e);

        public delegate void EntityAddedEventHandler(Block sender, BlockEntityChangeEventArgs e);

        public delegate void EntityRemovedEventHandler(Block sender, BlockEntityChangeEventArgs e);

        public delegate void LayerChangedEventHandler(Block sender, TableObjectChangedEventArgs<Layer> e);
    }
}

