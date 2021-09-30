using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Enums
{
    public enum ParentModule
    {
        [Description("Admin")]
        SystemSetup = 11,

        [Description("Transactions")]
        Transactions = 12,

        [Description("Reports")]
        Reports = 13
    }
}
