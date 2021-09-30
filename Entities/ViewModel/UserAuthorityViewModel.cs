using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.ViewModel
{
    public class UserAuthorityViewModel
    {
        public int moduleId { get; set; }
        public string moduleName { get; set; } = string.Empty;

        public int controlId { get; set; }
        public string controlName
        {
            get
            {
                if (controlId == 0)
                {
                    return String.Empty;
                }
                var fi = typeof(Control).GetField((Enum.GetName(typeof(Control), controlId)));
                var da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                           typeof(DescriptionAttribute));
                return da.Description;
            }

            set { }
        }

        public int pageId { get; set; }
        public string pageName
        {
            get
            {
                if (pageId == 0)
                {
                    return String.Empty;
                }
                var fi = typeof(Page).GetField((Enum.GetName(typeof(Page), pageId)));
                var da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                           typeof(DescriptionAttribute));
                return da.Description;
            }
        }
    }
}
