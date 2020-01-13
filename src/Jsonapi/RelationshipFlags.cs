using System;

namespace JsonApi
{
    [Flags]
    internal enum RelationshipFlags
    {
        None,
        Links,
        Data,
        Meta
    }
}
