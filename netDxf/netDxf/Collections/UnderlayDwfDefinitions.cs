namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class UnderlayDwfDefinitions : TableObjects<UnderlayDwfDefinition>
    {
        internal UnderlayDwfDefinitions(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal UnderlayDwfDefinitions(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, UnderlayDwfDefinition>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_DWFDEFINITIONS", handle)
        {
            base.MaxCapacity = 0x7fffffff;
        }

        internal override UnderlayDwfDefinition Add(UnderlayDwfDefinition underlayDwfDefinition, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (underlayDwfDefinition == null)
            {
                throw new ArgumentNullException("underlayDwfDefinition");
            }
            if (base.list.TryGetValue(underlayDwfDefinition.Name, out UnderlayDwfDefinition definition))
            {
                return definition;
            }
            if (assignHandle || string.IsNullOrEmpty(underlayDwfDefinition.Handle))
            {
                base.Owner.NumHandles = underlayDwfDefinition.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(underlayDwfDefinition.Handle, underlayDwfDefinition);
            base.list.Add(underlayDwfDefinition.Name, underlayDwfDefinition);
            base.references.Add(underlayDwfDefinition.Name, new List<DxfObject>());
            underlayDwfDefinition.Owner = this;
            underlayDwfDefinition.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return underlayDwfDefinition;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another dwf underlay definition with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (UnderlayDwfDefinition) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(UnderlayDwfDefinition item)
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

