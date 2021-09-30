using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    /// <summary>
    /// This is user View Model that we need to use or map on response
    /// </summary>
    public class UserViewModel: BaseViewModel
    {

        [Required]
        [StringLength(255, ErrorMessage = "Must be between 3 and 255 characters", MinimumLength = 3)]
        public string username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; } = string.Empty;
        public string name { get; set; } 
        public string role { get; set; } = string.Empty;
        public string profilePicture { get; set; } = string.Empty;
    }

    public class UserUpdateViewModel : UserViewModel
    {
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("password")]
        public string confirmPassword { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 2 and 255 characters", MinimumLength = 2)]
        public string firstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 2 and 255 characters", MinimumLength = 2)]
        public string lastName { get; set; } = string.Empty;
        public string middleName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Role is Required")]
        public int roleId { get; set; }
        public int personId { get; set; }


    }

}
