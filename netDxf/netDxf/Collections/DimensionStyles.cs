namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class DimensionStyles : TableObjects<DimensionStyle>
    {
        internal DimensionStyles(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal DimensionStyles(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, DimensionStyle>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "DIMSTYLE", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override DimensionStyle Add(DimensionStyle style, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            if (base.list.TryGetValue(style.Name, out DimensionStyle style2))
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
            style.TextStyle = base.Owner.TextStyles.Add(style.TextStyle, assignHandle);
            base.Owner.TextStyles.References[style.TextStyle.Name].Add(style);
            if (style.LeaderArrow > null)
            {
                style.LeaderArrow = base.Owner.Blocks.Add(style.LeaderArrow, assignHandle);
                base.Owner.Blocks.References[style.LeaderArrow.Name].Add(style);
            }
            if (style.DimArrow1 > null)
            {
                style.DimArrow1 = base.Owner.Blocks.Add(style.DimArrow1, assignHandle);
                base.Owner.Blocks.References[style.DimArrow1.Name].Add(style);
            }
            if (style.DimArrow2 > null)
            {
                style.DimArrow2 = base.Owner.Blocks.Add(style.DimArrow2, assignHandle);
                base.Owner.Blocks.References[style.DimArrow2.Name].Add(style);
            }
            style.DimLineLinetype = base.Owner.Linetypes.Add(style.DimLineLinetype, assignHandle);
            base.Owner.Linetypes.References[style.DimLineLinetype.Name].Add(style);
            style.ExtLine1Linetype = base.Owner.Linetypes.Add(style.ExtLine1Linetype, assignHandle);
            base.Owner.Linetypes.References[style.ExtLine1Linetype.Name].Add(style);
            style.ExtLine2Linetype = base.Owner.Linetypes.Add(style.ExtLine2Linetype, assignHandle);
            base.Owner.Linetypes.References[style.ExtLine2Linetype.Name].Add(style);
            style.Owner = this;
            style.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            style.LinetypeChanged += new DimensionStyle.LinetypeChangedEventHandler(this.DimensionStyleLinetypeChanged);
            style.TextStyleChanged += new DimensionStyle.TextStyleChangedEventHandler(this.DimensionStyleTextStyleChanged);
            style.BlockChanged += new DimensionStyle.BlockChangedEventHandler(this.DimensionStyleBlockChanged);
            return style;
        }

        private void DimensionStyleBlockChanged(TableObject sender, TableObjectChangedEventArgs<Block> e)
        {
            if (e.OldValue > null)
            {
                base.Owner.Blocks.References[e.OldValue.Name].Remove(sender);
            }
            e.NewValue = base.Owner.Blocks.Add(e.NewValue);
            if (e.NewValue > null)
            {
                base.Owner.Blocks.References[e.NewValue.Name].Add(sender);
            }
        }

        private void DimensionStyleLinetypeChanged(TableObject sender, TableObjectChangedEventArgs<Linetype> e)
        {
            base.Owner.Linetypes.References[e.OldValue.Name].Remove(sender);
            e.NewValue = base.Owner.Linetypes.Add(e.NewValue);
            base.Owner.Linetypes.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleTextStyleChanged(TableObject sender, TableObjectChangedEventArgs<TextStyle> e)
        {
            base.Owner.TextStyles.References[e.OldValue.Name].Remove(sender);
            e.NewValue = base.Owner.TextStyles.Add(e.NewValue);
            base.Owner.TextStyles.References[e.NewValue.Name].Add(sender);
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another dimension style with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (DimensionStyle) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(DimensionStyle item)
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
            base.Owner.TextStyles.References[item.TextStyle.Name].Remove(item);
            if (item.DimArrow1 > null)
            {
                base.Owner.Blocks.References[item.DimArrow1.Name].Remove(item);
            }
            if (item.DimArrow2 > null)
            {
                base.Owner.Blocks.References[item.DimArrow2.Name].Remove(item);
            }
            base.Owner.Linetypes.References[item.DimLineLinetype.Name].Remove(item);
            base.Owner.Linetypes.References[item.ExtLine1Linetype.Name].Remove(item);
            base.Owner.Linetypes.References[item.ExtLine2Linetype.Name].Remove(item);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Handle = null;
            item.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            item.LinetypeChanged -= new DimensionStyle.LinetypeChangedEventHandler(this.DimensionStyleLinetypeChanged);
            item.TextStyleChanged -= new DimensionStyle.TextStyleChangedEventHandler(this.DimensionStyleTextStyleChanged);
            item.BlockChanged -= new DimensionStyle.BlockChangedEventHandler(this.DimensionStyleBlockChanged);
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

