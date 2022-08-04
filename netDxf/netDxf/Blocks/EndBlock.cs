namespace netDxf.Blocks
{
    using netDxf;
    using System;

    internal class EndBlock : DxfObject
    {
        public EndBlock(DxfObject owner) : base("ENDBLK")
        {
            base.Owner = owner;
        }
    }
}

