namespace netDxf.Objects
{
    using netDxf.Collections;
    using netDxf.Tables;
    using netDxf.Units;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    public class ImageDefinition : TableObject
    {
        private readonly string fileName;
        private readonly int width;
        private readonly int height;
        private ImageResolutionUnits resolutionUnits;
        private double horizontalResolution;
        private double verticalResolution;
        private readonly Dictionary<string, ImageDefinitionReactor> reactors;

        public ImageDefinition(string fileName) : this(fileName, Path.GetFileNameWithoutExtension(fileName))
        {
        }

        public ImageDefinition(string fileName, string name) : base(name, "IMAGEDEF", false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "The image file name should be at least one character long.");
            }
            FileInfo info = new FileInfo(fileName);
            if (!info.Exists)
            {
                throw new FileNotFoundException("Image file not found", fileName);
            }
            this.fileName = fileName;
            try
            {
                using (Image image = Image.FromFile(fileName))
                {
                    this.width = image.Width;
                    this.height = image.Height;
                    this.horizontalResolution = image.HorizontalResolution;
                    this.verticalResolution = image.VerticalResolution;
                    this.resolutionUnits = ImageResolutionUnits.Inches;
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Image file not supported.", fileName);
            }
            this.reactors = new Dictionary<string, ImageDefinitionReactor>();
        }

        public ImageDefinition(string fileName, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units) : this(fileName, Path.GetFileNameWithoutExtension(fileName), width, horizontalResolution, height, verticalResolution, units)
        {
        }

        public ImageDefinition(string fileName, string name, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units) : base(name, "IMAGEDEF", false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "The image file name should be at least one character long.");
            }
            this.fileName = fileName;
            this.width = width;
            this.height = height;
            this.horizontalResolution = horizontalResolution;
            this.verticalResolution = verticalResolution;
            this.resolutionUnits = units;
            this.reactors = new Dictionary<string, ImageDefinitionReactor>();
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new ImageDefinition(this.fileName, newName, this.width, this.horizontalResolution, this.height, this.verticalResolution, this.resolutionUnits);

        public string FileName =>
            this.fileName;

        public int Width =>
            this.width;

        public int Height =>
            this.height;

        public ImageResolutionUnits ResolutionUnits
        {
            get => 
                this.resolutionUnits;
            set
            {
                if (this.resolutionUnits != value)
                {
                    switch (value)
                    {
                        case ImageResolutionUnits.Centimeters:
                            this.horizontalResolution /= 2.54;
                            this.verticalResolution /= 2.54;
                            break;

                        case ImageResolutionUnits.Inches:
                            this.horizontalResolution *= 2.54;
                            this.verticalResolution *= 2.54;
                            break;
                    }
                }
                this.resolutionUnits = value;
            }
        }

        public double HorizontalResolution =>
            this.horizontalResolution;

        public double VerticalResolution =>
            this.verticalResolution;

        public ImageDefinitions Owner
        {
            get => 
                ((ImageDefinitions) base.Owner);
            internal set => 
                (base.Owner = value);
        }

        internal Dictionary<string, ImageDefinitionReactor> Reactors =>
            this.reactors;
    }
}

