using System;

namespace Jsonapi
{
    [Flags]
    internal enum ResourceFlags
    {
        None,
        Id,
        Type
    }
}
