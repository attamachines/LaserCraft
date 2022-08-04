namespace netDxf.Tables
{
    using netDxf;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class TableObject : DxfObject, ICloneable, IComparable, IComparable<TableObject>, IEquatable<TableObject>
    {
        private static readonly IReadOnlyList<string> invalidCharacters;
        private bool reserved;
        private string name;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event NameChangedEventHandler NameChanged;

        static TableObject()
        {
            string[] textArray1 = new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|", ";", ",", "=", "`" };
            invalidCharacters = textArray1;
        }

        protected TableObject(string name, string codeName, bool checkName) : base(codeName)
        {
            if (checkName && !IsValidName(name))
            {
                throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", "name");
            }
            this.name = name;
            this.reserved = false;
        }

        public abstract object Clone();
        public abstract TableObject Clone(string newName);
        public int CompareTo(TableObject other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return ((base.GetType() == other.GetType()) ? string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) : 0);
        }

        public int CompareTo(object other) => 
            this.CompareTo((TableObject) other);

        public bool Equals(TableObject other)
        {
            if (other == null)
            {
                return false;
            }
            return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            if (base.GetType() != other.GetType())
            {
                return false;
            }
            return this.Equals((TableObject) other);
        }

        public override int GetHashCode() => 
            this.Name.GetHashCode();

        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            foreach (string str in InvalidCharacters)
            {
                if (name.Contains(str))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void OnNameChangedEvent(string oldName, string newName)
        {
            NameChangedEventHandler nameChanged = this.NameChanged;
            if (nameChanged > null)
            {
                TableObjectChangedEventArgs<string> e = new TableObjectChangedEventArgs<string>(oldName, newName);
                nameChanged(this, e);
            }
        }

        internal void SetName(string newName, bool checkName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException("newName");
            }
            if (this.IsReserved)
            {
                throw new ArgumentException("Reserved table objects cannot be renamed.", "newName");
            }
            if (!string.Equals(this.name, newName, StringComparison.OrdinalIgnoreCase))
            {
                if (checkName && !IsValidName(newName))
                {
                    throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", "newName");
                }
                this.OnNameChangedEvent(this.name, newName);
                this.name = newName;
            }
        }

        public override string ToString() => 
            this.Name;

        public string Name
        {
            get => 
                this.name;
            set => 
                this.SetName(value, true);
        }

        public bool IsReserved
        {
            get => 
                this.reserved;
            internal set => 
                (this.reserved = value);
        }

        public static IReadOnlyList<string> InvalidCharacters =>
            invalidCharacters;

        public delegate void NameChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<string> e);
    }
}

