using System;

namespace JsonApi
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
