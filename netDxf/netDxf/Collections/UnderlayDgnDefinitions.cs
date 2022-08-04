namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class UnderlayDgnDefinitions : TableObjects<UnderlayDgnDefinition>
    {
        internal UnderlayDgnDefinitions(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal UnderlayDgnDefinitions(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, UnderlayDgnDefinition>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_DGNDEFINITIONS", handle)
        {
            base.MaxCapacity = 0x7fffffff;
        }

        internal override UnderlayDgnDefinition Add(UnderlayDgnDefinition underlayDgnDefinition, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (underlayDgnDefinition == null)
            {
                throw new ArgumentNullException("underlayDgnDefinition");
            }
            if (base.list.TryGetValue(underlayDgnDefinition.Name, out UnderlayDgnDefinition definition))
            {
                return definition;
            }
            if (assignHandle || string.IsNullOrEmpty(underlayDgnDefinition.Handle))
            {
                base.Owner.NumHandles = underlayDgnDefinition.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(underlayDgnDefinition.Handle, underlayDgnDefinition);
            base.list.Add(underlayDgnDefinition.Name, underlayDgnDefinition);
            base.references.Add(underlayDgnDefinition.Name, new List<DxfObject>());
            underlayDgnDefinition.Owner = this;
            underlayDgnDefinition.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return underlayDgnDefinition;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another dgn underlay definition with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (UnderlayDgnDefinition) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(UnderlayDgnDefinition item)
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
            base.Owner.AddedObjects.Remove(item.Handle);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Handle = null;
            item.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

