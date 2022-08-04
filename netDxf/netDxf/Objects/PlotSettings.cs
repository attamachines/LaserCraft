namespace netDxf.Objects
{
    using netDxf;
    using System;

    public class PlotSettings : ICloneable
    {
        private string pageSetupName = string.Empty;
        private string plotterName = "none_device";
        private string paperSizeName = "ISO_A4_(210.00_x_297.00_MM)";
        private string viewName = string.Empty;
        private string currentStyleSheet = string.Empty;
        private double left = 7.5;
        private double bottom = 20.0;
        private double right = 7.5;
        private double top = 20.0;
        private Vector2 paperSize = new Vector2(210.0, 297.0);
        private Vector2 origin = Vector2.Zero;
        private Vector2 windowUpRight = Vector2.Zero;
        private Vector2 windowBottomLeft = Vector2.Zero;
        private double numeratorScale = 1.0;
        private double denominatorScale = 1.0;
        private PlotFlags flags = (PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale);
        private PlotPaperUnits paperUnits = PlotPaperUnits.Milimeters;
        private PlotRotation rotation = PlotRotation.Degrees90;
        private Vector2 paperImageOrigin;

        public object Clone() => 
            new PlotSettings { 
                PageSetupName = this.pageSetupName,
                PlotterName = this.plotterName,
                PaperSizeName = this.paperSizeName,
                ViewName = this.viewName,
                CurrentStyleSheet = this.currentStyleSheet,
                LeftMargin = this.left,
                BottomMargin = this.bottom,
                RightMargin = this.right,
                TopMargin = this.top,
                PaperSize = this.paperSize,
                Origin = this.origin,
                WindowUpRight = this.windowUpRight,
                WindowBottomLeft = this.windowBottomLeft,
                PrintScaleNumerator = this.numeratorScale,
                PrintScaleDenominator = this.denominatorScale,
                Flags = this.flags,
                PaperUnits = this.paperUnits,
                PaperRotation = this.rotation
            };

        public string PageSetupName
        {
            get => 
                this.pageSetupName;
            set => 
                (this.pageSetupName = value);
        }

        public string PlotterName
        {
            get => 
                this.plotterName;
            set => 
                (this.plotterName = value);
        }

        public string PaperSizeName
        {
            get => 
                this.paperSizeName;
            set => 
                (this.paperSizeName = value);
        }

        public string ViewName
        {
            get => 
                this.viewName;
            set => 
                (this.viewName = value);
        }

        public string CurrentStyleSheet
        {
            get => 
                this.currentStyleSheet;
            set => 
                (this.currentStyleSheet = value);
        }

        public double LeftMargin
        {
            get => 
                this.left;
            set => 
                (this.left = value);
        }

        public double BottomMargin
        {
            get => 
                this.bottom;
            set => 
                (this.bottom = value);
        }

        public double RightMargin
        {
            get => 
                this.right;
            set => 
                (this.right = value);
        }

        public double TopMargin
        {
            get => 
                this.top;
            set => 
                (this.top = value);
        }

        public Vector2 PaperSize
        {
            get => 
                this.paperSize;
            set => 
                (this.paperSize = value);
        }

        public Vector2 Origin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public Vector2 WindowUpRight
        {
            get => 
                this.windowUpRight;
            set => 
                (this.windowUpRight = value);
        }

        public Vector2 WindowBottomLeft
        {
            get => 
                this.windowBottomLeft;
            set => 
                (this.windowBottomLeft = value);
        }

        public double PrintScaleNumerator
        {
            get => 
                this.numeratorScale;
            set => 
                (this.numeratorScale = value);
        }

        public double PrintScaleDenominator
        {
            get => 
                this.denominatorScale;
            set => 
                (this.denominatorScale = value);
        }

        public PlotFlags Flags
        {
            get => 
                this.flags;
            set => 
                (this.flags = value);
        }

        public PlotPaperUnits PaperUnits
        {
            get => 
                this.paperUnits;
            set => 
                (this.paperUnits = value);
        }

        public PlotRotation PaperRotation
        {
            get => 
                this.rotation;
            set => 
                (this.rotation = value);
        }

        public double PrintScale =>
            (this.numeratorScale / this.denominatorScale);

        public Vector2 PaperImageOrigin
        {
            get => 
                this.paperImageOrigin;
            set => 
                (this.paperImageOrigin = value);
        }
    }
}

