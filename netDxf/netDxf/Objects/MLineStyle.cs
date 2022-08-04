namespace netDxf.Objects
{
    using netDxf;
    using netDxf.Collections;
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class MLineStyle : TableObject
    {
        private MLineStyleFlags flags;
        private string description;
        private AciColor fillColor;
        private double startAngle;
        private double endAngle;
        private readonly ObservableCollection<MLineStyleElement> elements;
        public const string DefaultName = "Standard";

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event MLineStyleElementAddedEventHandler MLineStyleElementAdded;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event MLineStyleElementLinetypeChangedEventHandler MLineStyleElementLinetypeChanged;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event MLineStyleElementRemovedEventHandler MLineStyleElementRemoved;

        public MLineStyle(string name) : this(name, elementArray1, string.Empty)
        {
            MLineStyleElement[] elementArray1 = new MLineStyleElement[] { new MLineStyleElement(0.5), new MLineStyleElement(-0.5) };
        }

        public MLineStyle(string name, IEnumerable<MLineStyleElement> elements) : this(name, elements, string.Empty)
        {
        }

        public MLineStyle(string name, string description) : this(name, elementArray1, description)
        {
            MLineStyleElement[] elementArray1 = new MLineStyleElement[] { new MLineStyleElement(0.5), new MLineStyleElement(-0.5) };
        }

        public MLineStyle(string name, IEnumerable<MLineStyleElement> elements, string description) : base(name, "MLINESTYLE", true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The multiline style name should be at least one character long.");
            }
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }
            this.elements = new ObservableCollection<MLineStyleElement>();
            this.elements.BeforeAddItem += new ObservableCollection<MLineStyleElement>.BeforeAddItemEventHandler(this.Elements_BeforeAddItem);
            this.elements.AddItem += new ObservableCollection<MLineStyleElement>.AddItemEventHandler(this.Elements_AddItem);
            this.elements.BeforeRemoveItem += new ObservableCollection<MLineStyleElement>.BeforeRemoveItemEventHandler(this.Elements_BeforeRemoveItem);
            this.elements.RemoveItem += new ObservableCollection<MLineStyleElement>.RemoveItemEventHandler(this.Elements_RemoveItem);
            this.elements.AddRange(elements);
            if (this.elements.Count < 1)
            {
                throw new ArgumentOutOfRangeException("elements", this.elements.Count, "The elements list must have at least one element.");
            }
            this.elements.Sort();
            this.flags = MLineStyleFlags.None;
            this.description = string.IsNullOrEmpty(description) ? string.Empty : description;
            this.fillColor = AciColor.ByLayer;
            this.startAngle = 90.0;
            this.endAngle = 90.0;
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName)
        {
            List<MLineStyleElement> elements = new List<MLineStyleElement>();
            foreach (MLineStyleElement element in this.elements)
            {
                elements.Add((MLineStyleElement) element.Clone());
            }
            return new MLineStyle(newName, elements) { 
                Flags = this.flags,
                Description = this.description,
                FillColor = (AciColor) this.fillColor.Clone(),
                StartAngle = this.startAngle,
                EndAngle = this.endAngle
            };
        }

        private void Elements_AddItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
            this.OnMLineStyleElementAddedEvent(e.Item);
            e.Item.LinetypeChanged += new MLineStyleElement.LinetypeChangedEventHandler(this.MLineStyleElement_LinetypeChanged);
        }

        private void Elements_BeforeAddItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
            if (e.Item == null)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Elements_BeforeRemoveItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
        }

        private void Elements_RemoveItem(ObservableCollection<MLineStyleElement> sender, ObservableCollectionEventArgs<MLineStyleElement> e)
        {
            this.OnMLineStyleElementRemovedEvent(e.Item);
            e.Item.LinetypeChanged -= new MLineStyleElement.LinetypeChangedEventHandler(this.MLineStyleElement_LinetypeChanged);
        }

        private void MLineStyleElement_LinetypeChanged(MLineStyleElement sender, TableObjectChangedEventArgs<Linetype> e)
        {
            e.NewValue = this.OnMLineStyleElementLinetypeChangedEvent(e.OldValue, e.NewValue);
        }

        protected virtual void OnMLineStyleElementAddedEvent(MLineStyleElement item)
        {
            MLineStyleElementAddedEventHandler mLineStyleElementAdded = this.MLineStyleElementAdded;
            if (mLineStyleElementAdded > null)
            {
                mLineStyleElementAdded(this, new MLineStyleElementChangeEventArgs(item));
            }
        }

        protected virtual Linetype OnMLineStyleElementLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            MLineStyleElementLinetypeChangedEventHandler mLineStyleElementLinetypeChanged = this.MLineStyleElementLinetypeChanged;
            if (mLineStyleElementLinetypeChanged > null)
            {
                TableObjectChangedEventArgs<Linetype> e = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
                mLineStyleElementLinetypeChanged(this, e);
                return e.NewValue;
            }
            return newLinetype;
        }

        protected virtual void OnMLineStyleElementRemovedEvent(MLineStyleElement item)
        {
            MLineStyleElementRemovedEventHandler mLineStyleElementRemoved = this.MLineStyleElementRemoved;
            if (mLineStyleElementRemoved > null)
            {
                mLineStyleElementRemoved(this, new MLineStyleElementChangeEventArgs(item));
            }
        }

        public static MLineStyle Default =>
            new MLineStyle("Standard");

        public MLineStyleFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }

        public string Description
        {
            get => 
                this.description;
            set => 
                (this.description = string.IsNullOrEmpty(value) ? string.Empty : value);
        }

        public AciColor FillColor
        {
            get => 
                this.fillColor;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.fillColor = value;
            }
        }

        public double StartAngle
        {
            get => 
                this.startAngle;
            set
            {
                if ((value < 10.0) || (value > 170.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The MLine style start angle valid values range from 10 to 170 degrees.");
                }
                this.startAngle = value;
            }
        }

        public double EndAngle
        {
            get => 
                this.endAngle;
            set
            {
                if ((value < 10.0) || (value > 170.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The MLine style end angle valid values range from 10 to 170 degrees.");
                }
                this.endAngle = value;
            }
        }

        public ObservableCollection<MLineStyleElement> Elements =>
            this.elements;

        public MLineStyles Owner
        {
            get => 
                ((MLineStyles) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        public delegate void MLineStyleElementAddedEventHandler(MLineStyle sender, MLineStyleElementChangeEventArgs e);

        public delegate void MLineStyleElementLinetypeChangedEventHandler(MLineStyle sender, TableObjectChangedEventArgs<Linetype> e);

        public delegate void MLineStyleElementRemovedEventHandler(MLineStyle sender, MLineStyleElementChangeEventArgs e);
    }
}

