using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Enums
{
    public enum PageResult
    {
        [Description("Success")]
        Success = 1,
        [Description("Failed")]
        Failed = 2
    }
}
