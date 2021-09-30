using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.ViewModel
{
    public class CashAidRecepientViewModel : BaseViewModel
    {       

        public int personId { get; set; }
        public string personName { get; set; } = string.Empty;
        public int eventId { get; set; }
        public string fullName { get; set; } = string.Empty;        
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
        public string address { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string zone { get; set; } = string.Empty;
        public string barangay { get; set; } = string.Empty;
        public string cluster { get; set; } = string.Empty;
        public string precinct { get; set; } = string.Empty;
        public string school { get; set; } = string.Empty;
        public string votersId { get; set; } = string.Empty;
        public string mobile { get; set; } = string.Empty;
        public string dobString { get { return dob != null ? ((DateTime)dob).ToString("MM/dd/yyyy") : ""; } }
        public int districtId { get; set; }
        public int zoneId { get; set; }
        public int brgyId { get; set; }
        public int clusterId { get; set; }
        public int precinctId { get; set; }
        public int age { get; set; }
        public double amountPerHead { get; set; }
    }

    public class CashAidNonVoterViewModel : BaseViewModel
    {
        
        public int eventId { get; set; }
        public string firstName { get; set; } = string.Empty;
        public string middleName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
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
        public string address { get; set; } = string.Empty;        
        public string barangay { get; set; } = string.Empty;
        public string cluster { get; set; } = string.Empty;
        public string mobile { get; set; } = string.Empty;
        public string dobString { get { return dob != null ? ((DateTime)dob).ToString("MM/dd/yyyy") : ""; } }        
        public int brgyId { get; set; }
        public int age { get; set; }
        public double amount { get; set; }
    }
}
