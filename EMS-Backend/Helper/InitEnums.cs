using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Entities.Enums;
using Entities.ViewModel;
namespace EMS_Backend.Helper
{
    public static class InitEnums
    {
        public static List<ModuleControlViewModel> GetListOfModuleControlVM()
        {
            var list = new List<ModuleControlViewModel>();
            FieldInfo fi;
            DescriptionAttribute da;
            foreach (Control pageEnum in Enum.GetValues(typeof(Control)))
            {
                fi = typeof(Control).GetField((pageEnum.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,typeof(DescriptionAttribute));

                int id = ((int)Enum.Parse(typeof(Control), pageEnum.ToString()));
                string name = da.Description;

                var control = new ModuleControlViewModel() { id = 0, controlId = (Control)id, controlName = name, moduleId = 0, isChecked = false };
                list.Add(control);
            }
            return list;
        }

        public static List<int> GetListOfControls()
        {
            var list = new List<int>();

            // return (from Control pageEnum in Enum.GetValues(typeof (Control)) select ((int) Enum.Parse(typeof (Control), pageEnum.ToString()))).ToList();

            foreach (Control pageEnum in Enum.GetValues(typeof(Control)))
            {
                int id = ((int)Enum.Parse(typeof(Control), pageEnum.ToString()));
                list.Add(id);
            }
            return list;
        }

        public static IEnumerable<object> GetListOfParentModules()
        {
            var list = new List<object>();
            FieldInfo fi;
            DescriptionAttribute da;
            foreach (ParentModule parentEnum in Enum.GetValues(typeof(ParentModule)))
            {
                fi = typeof(ParentModule).GetField((parentEnum.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                int id = ((int)Enum.Parse(typeof(ParentModule), parentEnum.ToString()));
                list.Add(new { value = id, text = da.Description});
            }
            return list;
        }

        public static IEnumerable<object> GetListOfPages()
        {
            var list = new List<object>();
            FieldInfo fi;
            DescriptionAttribute da;
            foreach (Page pageEnum in Enum.GetValues(typeof(Page)))
            {
                fi = typeof(Page).GetField((pageEnum.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                int id = ((int)Enum.Parse(typeof(Page), pageEnum.ToString()));
                list.Add(new { value = id, text = da.Description });
            }
            return list;
        }

        public static IEnumerable<object> GetListOfEventTypes()
        {
            var list = new List<object>();
            FieldInfo fi;
            DescriptionAttribute da;
            foreach (EventType eventEnum in Enum.GetValues(typeof(EventType)))
            {
                fi = typeof(EventType).GetField((eventEnum.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                int id = ((int)Enum.Parse(typeof(EventType), eventEnum.ToString()));
                list.Add(new { value = id, text = da.Description });
            }
            return list;
        }

        public static IEnumerable<object> GetListOfGender()
        {
            var list = new List<object>();
            FieldInfo fi;
            DescriptionAttribute da;
            foreach (Gender genderEnum in Enum.GetValues(typeof(Gender)))
            {
                fi = typeof(Gender).GetField((genderEnum.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                int id = ((int)Enum.Parse(typeof(Gender), genderEnum.ToString()));
                list.Add(new { value = id, text = da.Description });
            }
            return list;
        }
    }
}
