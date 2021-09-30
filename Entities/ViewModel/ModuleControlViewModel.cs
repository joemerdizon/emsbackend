using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.ViewModel
{
    public class ModuleControlViewModel
    {
        public int id { get; set; }

        public Control controlId { get; set; }

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

        public int moduleId { get; set; }
        public bool isChecked { get; set; }

    }
}
