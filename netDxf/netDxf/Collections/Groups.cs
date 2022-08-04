namespace netDxf.Collections
{
    using netDxf;
    using netDxf.Entities;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public sealed class Groups : TableObjects<Group>
    {
        internal Groups(DxfDocument document, string handle = null) : this(document, 0, handle)
        {
        }

        internal Groups(DxfDocument document, int capacity, string handle = null) : base(document, new Dictionary<string, Group>(capacity, StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<DxfObject>>(capacity, StringComparer.OrdinalIgnoreCase), "ACAD_GROUP", handle)
        {
            base.MaxCapacity = 0x7fffffff;
        }

        internal override Group Add(Group group, bool assignHandle)
        {
            if (base.list.Count >= base.MaxCapacity)
            {
                throw new OverflowException($"Table overflow. The maximum number of elements the table {base.CodeName} can have is {base.MaxCapacity}");
            }
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            if (group.IsUnnamed && string.IsNullOrEmpty(group.Name))
            {
                DxfDocument owner = base.Owner;
                int num = owner.GroupNamesGenerated + 1;
                owner.GroupNamesGenerated = num;
                group.SetName("*A" + num, false);
            }
            if (base.list.TryGetValue(group.Name, out Group group2))
            {
                return group2;
            }
            if (assignHandle || string.IsNullOrEmpty(group.Handle))
            {
                base.Owner.NumHandles = group.AsignHandle(base.Owner.NumHandles);
            }
            base.Owner.AddedObjects.Add(group.Handle, group);
            base.list.Add(group.Name, group);
            base.references.Add(group.Name, new List<DxfObject>());
            foreach (EntityObject obj2 in group.Entities)
            {
                if (obj2.Owner > null)
                {
                    if (obj2.Owner.Owner.Owner.Owner != base.Owner)
                    {
                        throw new ArgumentException("The group and their entities must belong to the same document. Clone them instead.");
                    }
                }
                else
                {
                    base.Owner.AddEntity(obj2);
                }
                base.references[group.Name].Add(obj2);
            }
            group.Owner = this;
            group.NameChanged += new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            group.EntityAdded += new Group.EntityAddedEventHandler(this.Group_EntityAdded);
            group.EntityRemoved += new Group.EntityRemovedEventHandler(this.Group_EntityRemoved);
            return group;
        }

        private void Group_EntityAdded(Group sender, GroupEntityChangeEventArgs e)
        {
            if (e.Item.Owner > null)
            {
                if (e.Item.Owner.Owner.Owner.Owner != base.Owner)
                {
                    throw new ArgumentException("The group and the entity must belong to the same document. Clone it instead.");
                }
            }
            else
            {
                base.Owner.AddEntity(e.Item);
            }
            base.references[sender.Name].Add(e.Item);
        }

        private void Group_EntityRemoved(Group sender, GroupEntityChangeEventArgs e)
        {
            base.references[sender.Name].Remove(e.Item);
        }

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (base.Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another dimension style with the same name.");
            }
            base.list.Remove(sender.Name);
            base.list.Add(e.NewValue, (Group) sender);
            List<DxfObject> list = base.references[sender.Name];
            base.references.Remove(sender.Name);
            base.references.Add(e.NewValue, list);
        }

        public override bool Remove(Group item)
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
            foreach (EntityObject obj2 in item.Entities)
            {
                obj2.RemoveReactor(item);
            }
            base.Owner.AddedObjects.Remove(item.Handle);
            base.references.Remove(item.Name);
            base.list.Remove(item.Name);
            item.Handle = null;
            item.Owner = null;
            item.NameChanged -= new TableObject.NameChangedEventHandler(this.Item_NameChanged);
            item.EntityAdded -= new Group.EntityAddedEventHandler(this.Group_EntityAdded);
            item.EntityRemoved -= new Group.EntityRemovedEventHandler(this.Group_EntityRemoved);
            return true;
        }

        public override bool Remove(string name) => 
            this.Remove(base[name]);
    }
}

