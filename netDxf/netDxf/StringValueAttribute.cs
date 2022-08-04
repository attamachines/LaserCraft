namespace netDxf
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StringValueAttribute : Attribute
    {
        private readonly string value;

        public StringValueAttribute(string value)
        {
            this.value = value;
        }

        public string Value =>
            this.value;
    }
}

