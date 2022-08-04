namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class ApplicationRegistries : TableObjects<ApplicationRegistry>
    {
        internal ApplicationRegistries(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal ApplicationRegistries(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, ApplicationRegistry>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "APPID", handle)
        {
            base.MaxCapacity = 0x7fff;
        }

        internal override ApplicationRegistry Add(ApplicationRegistry appReg, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (appReg == null)
            {
                throw new ArgumentNullException("appReg");
            }
            if (base.list.TryGetValue(appReg.Name, out ApplicationRegistry registry))
            {
                return registry;
            }
            if (assignHandle || string.IsNullOrEmpty(appReg.Handle))
            {
                base.Owner.NumHandles = appReg.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(appReg.Handle, appReg);
            base.list.Add(appReg.Name, appReg);
            base.references.Add(appReg.Name, new List<DxfObject>());
            appReg.Owner = this;
            appReg.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            return appReg;
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another application registry with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (ApplicationRegistry) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(ApplicationRegistry item)
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

