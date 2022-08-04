namespace netDxf.Objects
{
    using System;

    [Flags]
    public enum PlotFlags
    {
        PlotViewportBorders = 1,
        ShowPlotStyles = 2,
        PlotCentered = 4,
        PlotHidden = 8,
        UseStandardScale = 0x10,
        PlotPlotStyles = 0x20,
        ScaleLineweights = 0x40,
        PrintLineweights = 0x80,
        DrawViewportsFirst = 0x200,
        ModelType = 0x400,
        UpdatePaper = 0x800,
        ZoomToPaperOnUpdate = 0x1000,
        Initializing = 0x2000,
        PrevPlotInit = 0x4000
    }
}

