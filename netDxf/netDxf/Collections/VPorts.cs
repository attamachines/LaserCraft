namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class VPorts : TableObjects<VPort>
    {
        internal VPorts(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal VPorts(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, VPort>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "VPORT", handle)
        {
            base.MaxCapacity = 0x7fff;
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            VPort active = VPort.Active;
            base.Owner.NumHandles = active.AsignHandle(base.Owner.NumHandles);
            base.Owner.AddedObjects.Add(active.Handle, active);
            base.list.Add(active.Name, active);
            base.references.Add(active.Name, new List<DxfObject>());
            active.Owner = this;
        }

        internal override VPort Add(VPort vport, bool assignHandle)
        {
            throw new ArgumentException("VPorts cannot be added to the collection. There is only one VPort in the list the \"*Active\".", "vport");
        }

        public override bool Remove(VPort item)
        {
            throw new ArgumentException("VPorts cannot be removed from the collection.", "item");
        }

        public override bool Remove(string name)
        {
            throw new ArgumentException("VPorts cannot be removed from the collection.", "name");
        }
    }
}

