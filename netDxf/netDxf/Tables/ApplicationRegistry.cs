namespace netDxf.Tables
{
    using netDxf.Collections;
    using System;

    public class ApplicationRegistry : TableObject
    {
        public const string DefaultName = "ACAD";

        public ApplicationRegistry(string name) : this(name, true)
        {
        }

        internal ApplicationRegistry(string name, bool checkName) : base(name, "APPID", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The application registry name should be at least one character long.");
            }
            base.IsReserved = name.Equals("ACAD", StringComparison.OrdinalIgnoreCase);
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new ApplicationRegistry(newName);

        public static ApplicationRegistry Default =>
            new ApplicationRegistry("ACAD");

        public ApplicationRegistries Owner
        {
            get => 
                ((ApplicationRegistries) base.Owner);
            internal set => 
                (base.Owner = value);
        }
    }
}

