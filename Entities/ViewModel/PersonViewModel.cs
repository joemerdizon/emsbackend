using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class PersonViewModel : BaseViewModel
    {
        public string fullName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Gender is Required")]
        public Gender genderId { get; set; }
        public string genderName
        {
            get
            {
                if (genderId == 0)
                {
                    return String.Empty;
                }
                var fi = typeof(Gender).GetField((Enum.GetName(typeof(Gender), genderId)));
                var da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                           typeof(DescriptionAttribute));
                return da.Description;
            }
        }

        public DateTime? dob { get; set; }
        [Required]
        public string address { get; set; } = string.Empty;
        public string barangay { get; set; } = string.Empty;
        public string cluster { get; set; } = string.Empty;
        public string precinct { get; set; } = string.Empty;
        public string school { get; set; } = string.Empty;
        public string gCash { get; set; } = string.Empty;

        public string votersId { get; set; } = string.Empty;
        [Required]
        public string mobile { get; set; } = string.Empty;
        public bool IsMember { get; set; }
        public Guid ApprovalId { get; set; }
        public string emailaddress { get; set; }
        public int voterId { get; set; }

    }

    public class PersonUpdateViewModel : PersonViewModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 2 and 255 characters", MinimumLength = 2)]
        public string firstName { get; set; } = string.Empty;
        public string middleName { get; set; } = string.Empty;
        [Required]
        [StringLength(255, ErrorMessage = "Must be between 2 and 255 characters", MinimumLength = 2)]
        public string lastName { get; set; } = string.Empty;
        [Required]
        public string dobString { get { return dob != null ? ((DateTime)dob).ToString("MM/dd/yyyy") : ""; } }


        public int barangayId { get; set; }
        public int clusterId { get; set; }
        public int precinctId { get; set; }

        public int MemberStatus { get; set; }

        public string VoterFullname { get; set; }
        public string VoterNo { get; set; }

        public bool SMS { get; set; }
        public bool SMTP { get; set; }

        public int personId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string message { get; set; }



    }


    public class PersonActivationRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string message { get; set; }
        public int personId { get; set; }
        public string activationCode { get; set; }
        public PageResult PageResult { get; set; }
    }

    public class PersonRequest
    {
        public string firstName { get; set; } = string.Empty;
        public string middleName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public Gender genderId { get; set; }
        public DateTime? dob { get; set; }
        public string emailaddress { get; set; }
        public string address { get; set; } = string.Empty;
        public int barangayId { get; set; }
        public int clusterId { get; set; }
        public int precinctId { get; set; }
        public string school { get; set; } = string.Empty;
        public string gCash { get; set; } = string.Empty;
        public string votersId { get; set; } = string.Empty;
        public string mobile { get; set; } = string.Empty;
        public bool IsMember { get; set; }
    }
}
