using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class ModuleViewModel : BaseViewModel
    {
        
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 3 and 255 characters", MinimumLength = 3)]
        public string name { get; set; } = string.Empty;

        [Required]
        [StringLength(255, ErrorMessage = "Must be between 3 and 255 characters", MinimumLength = 3)]
        public string description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Parent Module is Required")]
        public int parentModuleId { get; set; }
        public string parentModuleName
        {
            get
            {
                if (parentModuleId == 0) {
                    return String.Empty;
                }
                var fi = typeof(ParentModule).GetField((Enum.GetName(typeof(ParentModule), parentModuleId)));
                var da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                           typeof(DescriptionAttribute));
                return da.Description;
            }
        }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Page is Required")]
        public Page pageId { get; set; }
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

        public string controllerName
        {
            get
            {
                if (pageId == 0)
                {
                    return String.Empty;
                }
                
                return Enum.GetName(typeof(Page), pageId);
            }
        }

        [StringLength(255, ErrorMessage = "Must be between 3 and 255 characters", MinimumLength = 3)]
        public string iconClass { get; set; } = string.Empty;

    }

    public class ModuleUpdateViewModel : ModuleViewModel
    {
        private List<ModuleControlViewModel> _controlList = new List<ModuleControlViewModel>();
        public List<ModuleControlViewModel> controlList
        {
            get { return _controlList; }
            set { _controlList = value; }
        }
    }

}
