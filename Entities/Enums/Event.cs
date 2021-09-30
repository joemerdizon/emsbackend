using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Enums
{
    public enum EventType
    {
        [Description("Cash Aid")]
        CashAid = 1,

        [Description("Campaign Event")]
        CampaignEvent = 2,

        [Description("Project")]
        Project = 3
    }
}
