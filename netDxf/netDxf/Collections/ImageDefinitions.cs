namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class ImageDefinitions : TableObjects<ImageDefinition>
    {
        internal ImageDefinitions(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal ImageDefinitions(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, ImageDefinition>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_IMAGE_DICT", handle)
        {
            base.MaxCapacity = 0x7fffffff;
        }

        internal override ImageDefinition Add(ImageDefinition imageDefinition, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (imageDefinition == null)
            {
                throw new ArgumentNullException("imageDefinition");
            }
            if (base.list.TryGetValue(imageDefinition.Name, out ImageDefinition definition))
            {
                return definition;
            }
            if (assignHandle || string.IsNullOrEmpty(imageDefinition.Handle))
            {
                base.Owner.NumHandles = imageDefinition.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(imageDefinition.Handle, imageDefinition);
            base.list.Add(imageDefinition.Name, imageDefinition);
            base.references.Add(imageDefinition.Name, new List<DxfObject>());
            imageDefinition.Owner = this;
            imageDefinition.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return imageDefinition;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another image definition with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (ImageDefinition) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(ImageDefinition item)
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

