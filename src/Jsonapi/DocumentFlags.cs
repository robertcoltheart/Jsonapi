using System;

namespace Jsonapi
{
    [Flags]
    internal enum DocumentFlags
    {
        None,
        Data,
        Errors,
        Meta
    }
}
