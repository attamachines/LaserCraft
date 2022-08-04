namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class Views : TableObjects<View>
    {
        internal Views(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal Views(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, View>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "VIEW", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override View Add(View view, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            if (base.list.TryGetValue(view.Name, out View view2))
            {
                return view2;
            }
            if (assignHandle || string.IsNullOrEmpty(view.Handle))
            {
                base.Owner.NumHandles = view.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(view.Handle, view);
            base.list.Add(view.Name, view);
            base.references.Add(view.Name, new List<DxfObject>());
            view.Owner = this;
            view.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return view;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another View with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (View) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(View item)
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

