namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Blocks;
    using netDxf.Entities;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class Layouts : TableObjects<Layout>
    {
        internal Layouts(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal Layouts(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, Layout>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_LAYOUT", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override Layout Add(Layout layout, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }
            if (base.list.TryGetValue(layout.Name, out Layout layout2))
            {
                return layout2;
            }
            layout.Owner = this;
            Block associatedBlock = layout.AssociatedBlock;
            if (layout.IsPaperSpace && (associatedBlock == null))
            {
                string name = (base.list.Count == 1) ? "*Paper_Space" : ("*Paper_Space" + (base.list.Count - 2));
                associatedBlock = new Block(name, false, null, null);
                if (layout.TabOrder == 0)
                {
                    layout.TabOrder = (short) base.list.Count;
                }
            }
            associatedBlock = base.Owner.Blocks.Add(associatedBlock);
            layout.AssociatedBlock = associatedBlock;
            associatedBlock.Record.Layout = layout;
            base.Owner.Blocks.References[associatedBlock.Name].Add(layout);
            if (layout.Viewport > null)
            {
                layout.Viewport.Owner = associatedBlock;
            }
            if (assignHandle || string.IsNullOrEmpty(layout.Handle))
            {
                base.Owner.NumHandles = layout.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(layout.Handle, layout);
            base.list.Add(layout.Name, layout);
            base.references.Add(layout.Name, new List<DxfObject>());
            layout.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return layout;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another layout with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (Layout) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(Layout item)
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
            List<DxfObject> list = base.references[item.Name];
            if (list.Count > 0)
            {
                DxfObject[] array = new DxfObject[list.Count];
                list.CopyTo(array);
                foreach (DxfObject obj2 in array)
                {
                    base.Owner.RemoveEntity(obj2 as EntityObject);
                }
            }
            foreach (Layout layout in base.list.Values)
            {
                if (layout.IsPaperSpace)
                {
                    base.Owner.Blocks.References[layout.AssociatedBlock.Name].Remove(layout);
                    base.Owner.Blocks.Remove(layout.AssociatedBlock);
                    layout.AssociatedBlock = null;
                }
            }
            base.Owner.AddedObjects.Remove(item.Handle);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Handle = null;
            item.Owner = null;
            item.Viewport.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            int num = 0;
            foreach (Layout layout2 in base.list.Values)
            {
                if (layout2.IsPaperSpace)
                {
                    string name = (num == 0) ? Block.PaperSpace.Name : (Block.PaperSpace.Name + (num - 1));
                    Block block = base.Owner.Blocks.Add(new Block(name, false, null, null));
                    base.Owner.Blocks.References[block.Name].Add(layout2);
                    layout2.AssociatedBlock = block;
                    block.Record.Layout = layout2;
                    num++;
                    layout2.Viewport.Owner = layout2.AssociatedBlock;
                    foreach (DxfObject obj3 in base.references[layout2.Name])
                    {
                        obj3.Owner = layout2.AssociatedBlock;
                    }
                }
            }
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

