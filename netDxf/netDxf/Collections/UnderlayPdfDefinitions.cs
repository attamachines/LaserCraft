namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class UnderlayPdfDefinitions : TableObjects<UnderlayPdfDefinition>
    {
        internal UnderlayPdfDefinitions(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal UnderlayPdfDefinitions(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, UnderlayPdfDefinition>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_PDFDEFINITIONS", handle)
        {
            base.MaxCapacity = 0x7fffffff;
        }

        internal override UnderlayPdfDefinition Add(UnderlayPdfDefinition underlayPdfDefinition, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (underlayPdfDefinition == null)
            {
                throw new ArgumentNullException("underlayPdfDefinition");
            }
            if (base.list.TryGetValue(underlayPdfDefinition.Name, out UnderlayPdfDefinition definition))
            {
                return definition;
            }
            if (assignHandle || string.IsNullOrEmpty(underlayPdfDefinition.Handle))
            {
                base.Owner.NumHandles = underlayPdfDefinition.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(underlayPdfDefinition.Handle, underlayPdfDefinition);
            base.list.Add(underlayPdfDefinition.Name, underlayPdfDefinition);
            base.references.Add(underlayPdfDefinition.Name, new List<DxfObject>());
            underlayPdfDefinition.Owner = this;
            underlayPdfDefinition.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return underlayPdfDefinition;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another pdf underlay definition with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (UnderlayPdfDefinition) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(UnderlayPdfDefinition item)
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

