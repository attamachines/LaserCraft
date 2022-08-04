namespace netDxf.Objects
{
    using netDxf.Collections;
    using netDxf.Entities;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Group : TableObject
    {
        private string description;
        private bool isSelectable;
        private bool isUnnamed;
        private readonly EntityCollection entities;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EntityAddedEventHandler EntityAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EntityRemovedEventHandler EntityRemoved;

        public Group() : this(string.Empty)
        {
        }

        public Group(IEnumerable<EntityObject> entities) : this(string.Empty, entities)
        {
        }

        public Group(string name) : this(name, new EntityCollection())
        {
        }

        internal Group(string name, bool checkName) : base(name, "GROUP", checkName)
        {
            this.isUnnamed = string.IsNullOrEmpty(name) || name.StartsWith("*");
            this.description = string.Empty;
            this.isSelectable = true;
            this.entities = new EntityCollection();
            this.entities.BeforeAddItem += new EntityCollection.BeforeAddItemEventHandler(this.Entities_BeforeAddItem);
            this.entities.AddItem += new EntityCollection.AddItemEventHandler(this.Entities_AddItem);
            this.entities.BeforeRemoveItem += new EntityCollection.BeforeRemoveItemEventHandler(this.Entities_BeforeRemoveItem);
            this.entities.RemoveItem += new EntityCollection.RemoveItemEventHandler(this.Entities_RemoveItem);
        }

        public Group(string name, IEnumerable<EntityObject> entities) : base(name, "GROUP", !string.IsNullOrEmpty(name))
        {
            this.isUnnamed = string.IsNullOrEmpty(name);
            this.description = string.Empty;
            this.isSelectable = true;
            this.entities = new EntityCollection();
            this.entities.BeforeAddItem += new EntityCollection.BeforeAddItemEventHandler(this.Entities_BeforeAddItem);
            this.entities.AddItem += new EntityCollection.AddItemEventHandler(this.Entities_AddItem);
            this.entities.BeforeRemoveItem += new EntityCollection.BeforeRemoveItemEventHandler(this.Entities_BeforeRemoveItem);
            this.entities.RemoveItem += new EntityCollection.RemoveItemEventHandler(this.Entities_RemoveItem);
            this.entities.AddRange(entities);
        }

        public override object Clone() => 
            this.Clone(this.IsUnnamed ? string.Empty : this.Name);

        public override TableObject Clone(string newName)
        {
            EntityObject[] entities = new EntityObject[this.entities.Count];
            for (int i = 0; i < this.entities.Count; i++)
            {
                entities[i] = (EntityObject) this.entities[i].Clone();
            }
            return new Group(newName, entities) { 
                Description = this.description,
                IsSelectable = this.isSelectable
            };
        }

        private void Entities_AddItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            e.Item.AddReactor(this);
            this.OnEntityAddedEvent(e.Item);
        }

        private void Entities_BeforeAddItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            if (e.Item == null)
            {
                e.Cancel = true;
            }
            else if (this.entities.Contains(e.Item))
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Entities_BeforeRemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
        }

        private void Entities_RemoveItem(EntityCollection sender, EntityCollectionEventArgs e)
        {
            e.Item.RemoveReactor(this);
            this.OnEntityRemovedEvent(e.Item);
        }

        protected virtual void OnEntityAddedEvent(EntityObject item)
        {
            EntityAddedEventHandler entityAdded = this.EntityAdded;
            if (entityAdded > null)
            {
                entityAdded(this, new GroupEntityChangeEventArgs(item));
            }
        }

        protected virtual void OnEntityRemovedEvent(EntityObject item)
        {
            EntityRemovedEventHandler entityRemoved = this.EntityRemoved;
            if (entityRemoved > null)
            {
                entityRemoved(this, new GroupEntityChangeEventArgs(item));
            }
        }

        public string Name
        {
            get => 
                base.Name;
            set
            {
                base.Name = value;
                this.isUnnamed = false;
            }
        }

        public string Description
        {
            get => 
                this.description;
            set => 
                (this.description = value);
        }

        public bool IsUnnamed
        {
            get => 
                this.isUnnamed;
            internal set => 
                (this.isUnnamed = value);
        }

        public bool IsSelectable
        {
            get => 
                this.isSelectable;
            set => 
                (this.isSelectable = value);
        }

        public EntityCollection Entities =>
            this.entities;

        public Groups Owner
        {
            get => 
                ((Groups) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public delegate void EntityAddedEventHandler(Group sender, GroupEntityChangeEventArgs e);

        public delegate void EntityRemovedEventHandler(Group sender, GroupEntityChangeEventArgs e);
    }
}

