namespace netDxf.Entities
{
    using netDxf;
    using System;

    internal class EndSequence : DxfObject
    {
        public EndSequence(DxfObject owner) : base("SEQEND")
        {
            base.Owner = owner;
        }
    }
}

