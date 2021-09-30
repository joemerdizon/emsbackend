using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Enums
{
    public enum Page
    {
        #region Admin Pages
        [Description("Role")]
        Role = 11,

        [Description("Module")]
        Module = 12,

        [Description("Person")]
        Person = 13,

        [Description("User")]
        User = 14,

        [Description("Policy")]
        Policy = 15,

        [Description("Voter")]
        Voter = 16,
        #endregion

        #region Web Pages
        [Description("Member Registration")]
        WebMemberRegistration = 21,

        [Description("Event and Resource Management")]
        EventManagement = 22,

        [Description("Chat Support")]
        ChatSupport = 23,

        [Description("Polling Management")]
        PollingManagement = 24,

        [Description("Announcement Management")]
        AnnouncementManagement = 25,

        #endregion

        #region Mobile Pages

        [Description("Member Registration")]
        MobMemberRegistration = 31,

        [Description("Chat")]
        Chat = 32,

        #endregion


        #region Report Pages

        [Description("Reports")]
        Reports = 41

        #endregion

    }
}
