using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Enums
{
    public enum Gender
    {
        [Description("Unspecified")]
        Unspecified = 0,

        [Description("Male")]
        Male = 1,

        [Description("Female")]
        Female = 2
    }
}
