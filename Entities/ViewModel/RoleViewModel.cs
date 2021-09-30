using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class RoleViewModel: BaseViewModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        public string role { get; set; } = string.Empty;

    }

    public class RoleUpdateViewModel : RoleViewModel
    {
       
    }

}
