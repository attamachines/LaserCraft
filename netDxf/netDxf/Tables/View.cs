namespace netDxf.Tables
{
    using netDxf;
    using netDxf.Collections;
    using System;

    public class View : TableObject
    {
        private Vector3 target;
        private Vector3 camera;
        private double height;
        private double width;
        private double rotation;
        private ViewModeFlags viewmode;
        private double fov;
        private double frontClippingPlane;
        private double backClippingPlane;

        public View(string name) : this(name, true)
        {
        }

        internal View(string name, bool checkName) : base(name, "VIEW", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The view name should be at least one character long.");
            }
            base.IsReserved = false;
            this.target = Vector3.Zero;
            this.camera = Vector3.UnitZ;
            this.height = 1.0;
            this.width = 1.0;
            this.rotation = 0.0;
            this.viewmode = ViewModeFlags.Off;
            this.fov = 40.0;
            this.frontClippingPlane = 0.0;
            this.backClippingPlane = 0.0;
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new View(newName) { 
                Target = this.target,
                Camera = this.camera,
                Height = this.height,
                Width = this.width,
                Rotation = this.rotation,
                Viewmode = this.viewmode,
                Fov = this.fov,
                FrontClippingPlane = this.frontClippingPlane,
                BackClippingPlane = this.backClippingPlane
            };

        public Vector3 Target
        {
            get => 
                this.target;
            set => 
                (this.target = value);
        }

        public Vector3 Camera
        {
            get => 
                this.camera;
            set => 
                (this.camera = value);
        }

        public double Height
        {
            get => 
                this.height;
            set => 
                (this.height = value);
        }

        public double Width
        {
            get => 
                this.width;
            set => 
                (this.width = value);
        }

        public double Rotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = value);
        }

        public ViewModeFlags Viewmode
        {
            get => 
                this.viewmode;
            set => 
                (this.viewmode = value);
        }

        public double Fov
        {
            get => 
                this.fov;
            set => 
                (this.fov = value);
        }

        public double FrontClippingPlane
        {
            get => 
                this.frontClippingPlane;
            set => 
                (this.frontClippingPlane = value);
        }

        public double BackClippingPlane
        {
            get => 
                this.backClippingPlane;
            set => 
                (this.backClippingPlane = value);
        }

        public Views Owner
        {
            get => 
                ((Views) base.Owner);
            internal set => 
                (base.Owner = value);
        }
    }
}

