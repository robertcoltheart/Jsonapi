using System;

namespace Jsonapi
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
