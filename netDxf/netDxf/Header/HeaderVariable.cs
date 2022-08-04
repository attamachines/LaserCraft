namespace netDxf.Header
{
    using System;

    internal class HeaderVariable
    {
        private readonly string name;
        private object value;

        public HeaderVariable(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString() => 
            $"{this.name}:{this.value}";

        public string Name =>
            this.name;

        public object Value
        {
            get => 
                this.value;
            set => 
                (this.value = value);
        }
    }
}

