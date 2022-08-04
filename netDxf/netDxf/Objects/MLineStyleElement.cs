namespace netDxf.Objects
{
    using netDxf;
    using netDxf.Tables;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class MLineStyleElement : IComparable<MLineStyleElement>, ICloneable
    {
        private double offset;
        private AciColor color;
        private netDxf.Tables.Linetype linetype;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event LinetypeChangedEventHandler LinetypeChanged;

        public MLineStyleElement(double offset) : this(offset, AciColor.ByLayer, netDxf.Tables.Linetype.ByLayer)
        {
        }

        public MLineStyleElement(double offset, AciColor color, netDxf.Tables.Linetype linetype)
        {
            this.offset = offset;
            this.color = color;
            this.linetype = linetype;
        }

        public object Clone() => 
            new MLineStyleElement(this.offset) { 
                Color = (AciColor) this.Color.Clone(),
                Linetype = (netDxf.Tables.Linetype) this.linetype.Clone()
            };

        public int CompareTo(MLineStyleElement other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return this.offset.CompareTo(other.offset);
        }

        public bool Equals(MLineStyleElement other)
        {
            if (other == null)
            {
                return false;
            }
            return MathHelper.IsEqual(this.offset, other.offset);
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
            return this.Equals((MLineStyleElement) other);
        }

        public override int GetHashCode() => 
            this.Offset.GetHashCode();

        protected virtual netDxf.Tables.Linetype OnLinetypeChangedEvent(netDxf.Tables.Linetype oldLinetype, netDxf.Tables.Linetype newLinetype)
        {
            LinetypeChangedEventHandler linetypeChanged = this.LinetypeChanged;
            if (linetypeChanged > null)
            {
                TableObjectChangedEventArgs<netDxf.Tables.Linetype> e = new TableObjectChangedEventArgs<netDxf.Tables.Linetype>(oldLinetype, newLinetype);
                linetypeChanged(this, e);
                return e.NewValue;
            }
            return newLinetype;
        }

        public override string ToString() => 
            $"{this.offset}, color:{this.color}, line type:{this.linetype}";

        public double Offset
        {
            get => 
                this.offset;
            set => 
                (this.offset = value);
        }

        public AciColor Color
        {
            get => 
                this.color;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.color = value;
            }
        }

        public netDxf.Tables.Linetype Linetype
        {
            get => 
                this.linetype;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.linetype = this.OnLinetypeChangedEvent(this.linetype, value);
            }
        }

        public delegate void LinetypeChangedEventHandler(MLineStyleElement sender, TableObjectChangedEventArgs<Linetype> e);
    }
}

