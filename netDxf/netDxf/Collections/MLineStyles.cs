namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class MLineStyles : TableObjects<MLineStyle>
    {
        internal MLineStyles(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal MLineStyles(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, MLineStyle>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_MLINESTYLE", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override MLineStyle Add(MLineStyle style, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            if (base.list.TryGetValue(style.Name, out MLineStyle style2))
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
            foreach (MLineStyleElement element in style.Elements)
            {
                element.Linetype = base.Owner.Linetypes.Add(element.Linetype);
                base.Owner.Linetypes.References[element.Linetype.Name].Add(style);
            }
            style.Owner = this;
            style.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            style.MLineStyleElementAdded += new MLineStyle.MLineStyleElementAddedEventHandler(this.MLineStyle_ElementAdded);
            style.MLineStyleElementRemoved += new MLineStyle.MLineStyleElementRemovedEventHandler(this.MLineStyle_ElementRemoved);
            style.MLineStyleElementLinetypeChanged += new MLineStyle.MLineStyleElementLinetypeChangedEventHandler(this.MLineStyle_ElementLinetypeChanged);
            return style;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another multiline style with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (MLineStyle) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        private void MLineStyle_ElementAdded(MLineStyle sender, MLineStyleElementChangeEventArgs e)
        {
            base.Owner.Linetypes.References[e.Item.Linetype.Name].Add(sender);
        }

        private void MLineStyle_ElementLinetypeChanged(MLineStyle sender, TableObjectChangedEventArgs<Linetype> e)
        {
            base.Owner.Linetypes.References[e.OldValue.Name].Remove(sender);
            e.NewValue = base.Owner.Linetypes.Add(e.NewValue);
            base.Owner.Linetypes.References[e.NewValue.Name].Add(sender);
        }

        private void MLineStyle_ElementRemoved(MLineStyle sender, MLineStyleElementChangeEventArgs e)
        {
            base.Owner.Linetypes.References[e.Item.Linetype.Name].Remove(sender);
        }

        public override bool Remove(MLineStyle item)
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
            foreach (MLineStyleElement element in item.Elements)
            {
                base.Owner.Linetypes.References[element.Linetype.Name].Remove(item);
            }
            base.Owner.AddedObjects.Remove(item.Handle);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Handle = null;
            item.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            item.MLineStyleElementAdded -= new MLineStyle.MLineStyleElementAddedEventHandler(this.MLineStyle_ElementAdded);
            item.MLineStyleElementRemoved -= new MLineStyle.MLineStyleElementRemovedEventHandler(this.MLineStyle_ElementRemoved);
            item.MLineStyleElementLinetypeChanged -= new MLineStyle.MLineStyleElementLinetypeChangedEventHandler(this.MLineStyle_ElementLinetypeChanged);
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

