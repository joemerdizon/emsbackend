using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class PolicyViewModel : BaseViewModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 3 and 255 characters", MinimumLength = 3)]
        public string name { get; set; } = string.Empty;

        [Required]
        [StringLength(255, ErrorMessage = "Must be between 3 and 255 characters", MinimumLength = 3)]
        public string description { get; set; } = string.Empty;

        public string assignedRoles { get; set; } = string.Empty;

    }

    public class PolicyUpdateViewModel : PolicyViewModel
    {
        [Required]
        public List<int> policyRoles { get; set; } = new List<int>();

        public List<PolicyModuleViewModel> policyModule { get; set; } = new List<PolicyModuleViewModel>();
    }

    public class PolicyModuleViewModel 
    {
        public int moduleId { get; set; }
        public string module { get; set; }
        public List<PolicyModuleControlViewModel> policyModuleControl { get; set; } = new List<PolicyModuleControlViewModel>();
    }
    public class PolicyModuleControlViewModel {

        public int id { get; set; }

        public int policyId { get; set; }
        public ModuleControlViewModel moduleControl { get; set; } = new ModuleControlViewModel();

        public bool isRestricted { get; set; }

        public bool isEnabled { get; set; }

        public bool isActive { get; set; }
    }

}
