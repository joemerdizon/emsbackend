using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class VoterViewModel : BaseViewModel
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
        public string district { get; set; } = string.Empty;
        public string zone { get; set; } = string.Empty;
        public string barangay { get; set; } = string.Empty;
        public string cluster { get; set; } = string.Empty;
        public string precinct { get; set; } = string.Empty;
        public string school { get; set; } = string.Empty;
        public string votersId { get; set; } = string.Empty;
        public string mobile { get; set; } = string.Empty;
    }

    public class VoterUpdateViewModel : VoterViewModel
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
        
        [Range(0, int.MaxValue, ErrorMessage = "District is Required")]
        public int districtId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Zone is Required")]
        public int zoneId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Barangay is Required")]
        public int brgyId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Cluster is Required")]
        public int clusterId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Precinct is Required")]
        public int precinctId { get; set; }
    }
}
