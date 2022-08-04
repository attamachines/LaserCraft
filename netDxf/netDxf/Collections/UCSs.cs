namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class UCSs : TableObjects<UCS>
    {
        internal UCSs(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal UCSs(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, UCS>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "UCS", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override UCS Add(UCS ucs, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (ucs == null)
            {
                throw new ArgumentNullException("ucs");
            }
            if (base.list.TryGetValue(ucs.Name, out UCS ucs2))
            {
                return ucs2;
            }
            if (assignHandle || string.IsNullOrEmpty(ucs.Handle))
            {
                base.Owner.NumHandles = ucs.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(ucs.Handle, ucs);
            base.list.Add(ucs.Name, ucs);
            base.references.Add(ucs.Name, new List<DxfObject>());
            ucs.Owner = this;
            ucs.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return ucs;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another UCS with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (UCS) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(UCS item)
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

