namespace netDxf.Tables
{
    using netDxf;
    using netDxf.Collections;
    using System;

    public class VPort : TableObject
    {
        private Vector2 center;
        private Vector2 snapBasePoint;
        private Vector2 snapSpacing;
        private Vector2 gridSpacing;
        private Vector3 direction;
        private Vector3 target;
        private double height;
        private double aspectRatio;
        private bool showGrid;
        private bool snapMode;
        public const string DefaultName = "*Active";

        public VPort(string name) : this(name, true)
        {
        }

        internal VPort(string name, bool checkName) : base(name, "VPORT", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The viewport name should be at least one character long.");
            }
            base.IsReserved = name.Equals("*Active", StringComparison.OrdinalIgnoreCase);
            this.center = Vector2.Zero;
            this.snapBasePoint = Vector2.Zero;
            this.snapSpacing = new Vector2(0.5);
            this.gridSpacing = new Vector2(10.0);
            this.target = Vector3.Zero;
            this.direction = Vector3.UnitZ;
            this.height = 10.0;
            this.aspectRatio = 1.0;
            this.showGrid = true;
            this.snapMode = false;
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new VPort(newName) { 
                ViewCenter = this.center,
                SnapBasePoint = this.snapBasePoint,
                SnapSpacing = this.snapSpacing,
                GridSpacing = this.gridSpacing,
                ViewTarget = this.target,
                ViewDirection = this.direction,
                ViewHeight = this.height,
                ViewAspectRatio = this.aspectRatio,
                ShowGrid = this.showGrid
            };

        public static VPort Active =>
            new VPort("*Active", false);

        public Vector2 ViewCenter
        {
            get => 
                this.center;
            set => 
                (this.center = value);
        }

        public Vector2 SnapBasePoint
        {
            get => 
                this.snapBasePoint;
            set => 
                (this.snapBasePoint = value);
        }

        public Vector2 SnapSpacing
        {
            get => 
                this.snapSpacing;
            set => 
                (this.snapSpacing = value);
        }

        public Vector2 GridSpacing
        {
            get => 
                this.gridSpacing;
            set => 
                (this.gridSpacing = value);
        }

        public Vector3 ViewDirection
        {
            get => 
                this.direction;
            set
            {
                this.direction = Vector3.Normalize(value);
                if (Vector3.IsNaN(this.direction))
                {
                    throw new ArgumentException("The direction can not be the zero vector.", "value");
                }
            }
        }

        public Vector3 ViewTarget
        {
            get => 
                this.target;
            set => 
                (this.target = value);
        }

        public double ViewHeight
        {
            get => 
                this.height;
            set => 
                (this.height = value);
        }

        public double ViewAspectRatio
        {
            get => 
                this.aspectRatio;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The VPort aspect ratio must be greater than zero.");
                }
                this.aspectRatio = value;
            }
        }

        public bool ShowGrid
        {
            get => 
                this.showGrid;
            set => 
                (this.showGrid = value);
        }

        public bool SnapMode
        {
            get => 
                this.snapMode;
            set => 
                (this.snapMode = value);
        }

        public VPorts Owner
        {
            get => 
                ((VPorts) base.Owner);
            internal set => 
                (base.Owner = value);
        }
    }
}

