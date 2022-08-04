namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class Layers : TableObjects<Layer>
    {
        internal Layers(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal Layers(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, Layer>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "LAYER", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override Layer Add(Layer layer, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }
            if (base.list.TryGetValue(layer.Name, out Layer layer2))
            {
                return layer2;
            }
            if (assignHandle || string.IsNullOrEmpty(layer.Handle))
            {
                base.Owner.NumHandles = layer.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(layer.Handle, layer);
            base.list.Add(layer.Name, layer);
            base.references.Add(layer.Name, new List<DxfObject>());
            layer.Linetype = base.Owner.Linetypes.Add(layer.Linetype);
            base.Owner.Linetypes.References[layer.Linetype.Name].Add(layer);
            layer.Owner = this;
            layer.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            layer.LinetypeChanged += new Layer.LinetypeChangedEventHandler(this.LayerLinetypeChanged);
            return layer;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another layer with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (Layer) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        private void LayerLinetypeChanged(TableObject sender, TableObjectChangedEventArgs<Linetype> e)
        {
            base.Owner.Linetypes.References[e.OldValue.Name].Remove(sender);
            e.NewValue = base.Owner.Linetypes.Add(e.NewValue);
            base.Owner.Linetypes.References[e.NewValue.Name].Add(sender);
        }

        public override bool Remove(Layer item)
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
            base.Owner.Linetypes.References[item.Linetype.Name].Remove(item);
            base.Owner.AddedObjects.Remove(item.Handle);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Handle = null;
            item.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            item.LinetypeChanged -= new Layer.LinetypeChangedEventHandler(this.LayerLinetypeChanged);
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

