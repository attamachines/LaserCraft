namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class TextStyles : TableObjects<TextStyle>
    {
        internal TextStyles(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal TextStyles(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, TextStyle>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "STYLE", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override TextStyle Add(TextStyle style, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            if (base.list.TryGetValue(style.Name, out TextStyle style2))
            {
                return style2;
            }
            if (assignHandle || string.IsNullOrEmpty(style.Handle))
            {
                base.Owner.NumHandles = style.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(style.Handle, style);
            base.list.Add(style.Name, style);
            base.references.Add(style.Name, new List<DxfObject>());
            style.Owner = this;
            style.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return style;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another text style with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (TextStyle) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(TextStyle item)
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

