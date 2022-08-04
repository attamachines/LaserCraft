namespace netDxf.Blocks
{
    using System;

    [Flags]
    public enum BlockTypeFlags
    {
        None = 0,
        AnonymousBlock = 1,
        NonConstantAttributeDefinitions = 2,
        XRef = 4,
        XRefOverlay = 8,
        ExternallyDependent = 0x10,
        ResolvedExternalReference = 0x20,
        DefinitionExternalReference = 0x40
    }
}

