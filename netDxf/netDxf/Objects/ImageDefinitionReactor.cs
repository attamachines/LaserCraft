namespace netDxf.Objects
{
    using netDxf;
    using System;

    internal class ImageDefinitionReactor : DxfObject
    {
        private string imageHandle;

        public ImageDefinitionReactor(string imageHandle) : base("IMAGEDEF_REACTOR")
        {
            this.imageHandle = imageHandle;
        }

        public string ImageHandle =>
            this.imageHandle;
    }
}

