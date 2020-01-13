using System;

namespace JsonApi
{
    [Flags]
    internal enum ResourceFlags
    {
        None,
        Id,
        Type
    }
}
