namespace netDxf.Entities
{
    using netDxf;
    using netDxf.Objects;
    using netDxf.Tables;
    using System;

    public class Image : EntityObject
    {
        private Vector3 position;
        private double width;
        private double height;
        private double rotation;
        private ImageDefinition imageDefinition;
        private bool clipping;
        private short brightness;
        private short contrast;
        private short fade;
        private ImageDisplayFlags displayOptions;
        private netDxf.ClippingBoundary clippingBoundary;

        internal Image() : base(EntityType.Image, "IMAGE")
        {
        }

        public Image(ImageDefinition imageDefinition, Vector2 position, Vector2 size) : this(imageDefinition, new Vector3(position.X, position.Y, 0.0), size.X, size.Y)
        {
        }

        public Image(ImageDefinition imageDefinition, Vector3 position, Vector2 size) : this(imageDefinition, position, size.X, size.Y)
        {
        }

        public Image(ImageDefinition imageDefinition, Vector2 position, double width, double height) : this(imageDefinition, new Vector3(position.X, position.Y, 0.0), width, height)
        {
        }

        public Image(ImageDefinition imageDefinition, Vector3 position, double width, double height) : base(EntityType.Image, "IMAGE")
        {
            if (imageDefinition == null)
            {
                throw new ArgumentNullException("imageDefinition");
            }
            this.imageDefinition = imageDefinition;
            this.position = position;
            this.width = width;
            this.height = height;
            this.rotation = 0.0;
            this.clipping = false;
            this.brightness = 50;
            this.contrast = 50;
            this.fade = 0;
            this.displayOptions = ImageDisplayFlags.UseClippingBoundary | ImageDisplayFlags.ShowImageWhenNotAlignedWithScreen | ImageDisplayFlags.ShowImage;
            this.clippingBoundary = new netDxf.ClippingBoundary(-0.5, -0.5, (double) imageDefinition.Width, (double) imageDefinition.Height);
        }

        public override object Clone()
        {
            Image image = new Image {
                Layer = (Layer) base.Layer.Clone(),
                Linetype = (Linetype) base.Linetype.Clone(),
                Color = (AciColor) base.Color.Clone(),
                Lineweight = base.Lineweight,
                Transparency = (Transparency) base.Transparency.Clone(),
                LinetypeScale = base.LinetypeScale,
                Normal = base.Normal,
                IsVisible = base.IsVisible,
                Position = this.position,
                Height = this.height,
                Width = this.width,
                Rotation = this.rotation,
                Definition = (ImageDefinition) this.imageDefinition.Clone(),
                Clipping = this.clipping,
                Brightness = this.brightness,
                Contrast = this.contrast,
                Fade = this.fade,
                DisplayOptions = this.displayOptions,
                ClippingBoundary = (netDxf.ClippingBoundary) this.clippingBoundary.Clone()
            };
            foreach (XData data in base.XData.Values)
            {
                image.XData.Add((XData) data.Clone());
            }
            return image;
        }

        public Vector3 Position
        {
            get => 
                this.position;
            set => 
                (this.position = value);
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
                (this.rotation = MathHelper.NormalizeAngle(value));
        }

        public ImageDefinition Definition
        {
            get => 
                this.imageDefinition;
            internal set => 
                (this.imageDefinition = value);
        }

        public bool Clipping
        {
            get => 
                this.clipping;
            set => 
                (this.clipping = value);
        }

        public short Brightness
        {
            get => 
                this.brightness;
            set
            {
                if ((value < 0) && (value > 100))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted brightness values range from 0 to 100.");
                }
                this.brightness = value;
            }
        }

        public short Contrast
        {
            get => 
                this.contrast;
            set
            {
                if ((value < 0) && (value > 100))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted contrast values range from 0 to 100.");
                }
                this.contrast = value;
            }
        }

        public short Fade
        {
            get => 
                this.fade;
            set
            {
                if ((value < 0) && (value > 100))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Accepted fade values range from 0 to 100.");
                }
                this.fade = value;
            }
        }

        public ImageDisplayFlags DisplayOptions
        {
            get => 
                this.displayOptions;
            set => 
                (this.displayOptions = value);
        }

        public netDxf.ClippingBoundary ClippingBoundary
        {
            get => 
                this.clippingBoundary;
            set => 
                (this.clippingBoundary = value ?? new netDxf.ClippingBoundary(-0.5, -0.5, (double) this.imageDefinition.Width, (double) this.imageDefinition.Height));
        }
    }
}

