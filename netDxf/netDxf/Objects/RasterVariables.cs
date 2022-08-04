namespace netDxf.Objects
{
    using netDxf;
    using netDxf.Units;
    using System;

    public class RasterVariables : DxfObject
    {
        private bool displayFrame;
        private ImageDisplayQuality quality;
        private ImageUnits units;

        public RasterVariables() : base("RASTERVARIABLES")
        {
            this.displayFrame = true;
            this.quality = ImageDisplayQuality.High;
            this.units = ImageUnits.Unitless;
        }

        public bool DisplayFrame
        {
            get => 
                this.displayFrame;
            set => 
                (this.displayFrame = value);
        }

        public ImageDisplayQuality DisplayQuality
        {
            get => 
                this.quality;
            set => 
                (this.quality = value);
        }

        public ImageUnits Units
        {
            get => 
                this.units;
            set => 
                (this.units = value);
        }
    }
}

