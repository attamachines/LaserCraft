namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class Linetypes : TableObjects<Linetype>
    {
        internal Linetypes(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal Linetypes(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, Linetype>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "LTYPE", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override Linetype Add(Linetype linetype, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (linetype == null)
            {
                throw new ArgumentNullException("linetype");
            }
            if (base.list.TryGetValue(linetype.Name, out Linetype linetype2))
            {
                return linetype2;
            }
            if (assignHandle || string.IsNullOrEmpty(linetype.Handle))
            {
                base.Owner.NumHandles = linetype.AsignHandle(base.Owner.NumHandles);
            }
            base.list.Add(linetype.Name, linetype);
            base.Owner.AddedObjects.Add(linetype.Handle, linetype);
            base.references.Add(linetype.Name, new List<DxfObject>());
            linetype.Owner = this;
            linetype.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return linetype;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another line type with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (Linetype) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(Linetype item)
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

