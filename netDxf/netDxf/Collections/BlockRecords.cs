namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Entities;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class BlockRecords : TableObjects<Block>
    {
        internal BlockRecords(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal BlockRecords(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, Block>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "BLOCK_RECORD", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override Block Add(Block block, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            if (base.list.TryGetValue(block.Name, out Block block2))
            {
                return block2;
            }
            if (assignHandle || string.IsNullOrEmpty(block.Handle))
            {
                base.Owner.NumHandles = block.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(block.Handle, block);
            base.Owner.AddedObjects.Add(block.Owner.Handle, block.Owner);
            base.list.Add(block.Name, block);
            base.references.Add(block.Name, new List<DxfObject>());
            block.Layer = base.Owner.Layers.Add(block.Layer);
            base.Owner.Layers.References[block.Layer.Name].Add(block);
            foreach (EntityObject obj2 in block.Entities)
            {
                base.Owner.AddEntity(obj2, true, assignHandle);
            }
            foreach (AttributeDefinition definition in block.AttributeDefinitions.Values)
            {
                base.Owner.AddEntity(definition, true, assignHandle);
            }
            block.Record.Owner = this;
            block.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            block.LayerChanged += new Block.LayerChangedEventHandler(this.Block_LayerChanged);
            block.EntityAdded += new Block.EntityAddedEventHandler(this.Block_EntityAdded);
            block.EntityRemoved += new Block.EntityRemovedEventHandler(this.Block_EntityRemoved);
            block.AttributeDefinitionAdded += new Block.AttributeDefinitionAddedEventHandler(this.Block_EntityAdded);
            block.AttributeDefinitionRemoved += new Block.AttributeDefinitionRemovedEventHandler(this.Block_EntityRemoved);
            return block;
        }

        private void Block_EntityAdded(TableObject sender, BlockEntityChangeEventArgs e)
        {
            if (e.Item.Owner > null)
            {
                if (e.Item.Owner.Record.Owner.Owner != base.Owner)
                {
                    throw new ArgumentException("The block and the entity must belong to the same document. Clone it instead.");
                }
                if (e.Item.Owner.Record.Layout == null)
                {
                    throw new ArgumentException("The entity cannot belong to another block. Clone it instead.");
                }
                base.Owner.RemoveEntity(e.Item);
            }
            base.Owner.AddEntity(e.Item, true, string.IsNullOrEmpty(e.Item.Handle));
        }

        private void Block_EntityRemoved(TableObject sender, BlockEntityChangeEventArgs e)
        {
            base.Owner.RemoveEntity(e.Item, true);
        }

        private void Block_LayerChanged(Block sender, TableObjectChangedEventArgs<Layer> e)
        {
            base.Owner.Layers.References[e.OldValue.Name].Remove(sender);
            e.NewValue = base.Owner.Layers.Add(e.NewValue);
            base.Owner.Layers.References[e.NewValue.Name].Add(sender);
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another block with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (Block) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(Block item)
        {
            if (item == null)
            {
                return false;
            }
            if (!base.Contains(item))
            {
                return false;
            }
            if (item.IsReserved)
            {
                return false;
            }
            if (base.references[item.Name].Count > 0)
            {
                return false;
            }
            base.Owner.Layers.References[item.Layer.Name].Remove(item);
            foreach (EntityObject obj2 in item.Entities)
            {
                base.Owner.RemoveEntity(obj2, true);
            }
            foreach (AttributeDefinition definition in item.AttributeDefinitions.Values)
            {
                base.Owner.RemoveEntity(definition, true);
            }
            base.Owner.AddedObjects.Remove(item.Handle);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Record.Handle = null;
            item.Record.Owner = null;
            item.Handle = null;
            item.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            item.LayerChanged -= new Block.LayerChangedEventHandler(this.Block_LayerChanged);
            item.EntityAdded -= new Block.EntityAddedEventHandler(this.Block_EntityAdded);
            item.EntityRemoved -= new Block.EntityRemovedEventHandler(this.Block_EntityRemoved);
            item.AttributeDefinitionAdded -= new Block.AttributeDefinitionAddedEventHandler(this.Block_EntityAdded);
            item.AttributeDefinitionRemoved -= new Block.AttributeDefinitionRemovedEventHandler(this.Block_EntityRemoved);
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

