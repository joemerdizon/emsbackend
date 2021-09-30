using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class EventViewModel : BaseViewModel
    {      
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Type of Event is Required")]
        public EventType typeOfEventId { get; set; }
        public string typeOfEvent
        {
            get
            {
                if (typeOfEventId == 0)
                {
                    return String.Empty;
                }
                var fi = typeof(EventType).GetField((Enum.GetName(typeof(EventType), typeOfEventId)));
                var da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                           typeof(DescriptionAttribute));
                return da.Description;
            }
        }

        [Required]
        [StringLength(100, ErrorMessage = "Must be between 3 and 100 characters", MinimumLength = 3)]
        public string name { get; set; } = string.Empty;
        [Required]
        [StringLength(250, ErrorMessage = "Must be between 3 and 250 characters", MinimumLength = 3)]
        public string description { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string additionalDetails { get; set; } = string.Empty;
        public double budget { get; set; }
        public DateTime? date { get; set; }
        public bool isClosed { get; set; }
        public double runningTotalAmount { get; set; }
        public double cashAidAmount { get; set; }
        public double additionalAmount { get; set; }
        public double runningCashAidAmount { get; set; }
        public double runningAdditionalAmount { get; set; }
        public int barangayId { get; set; }

    }

    public class EventUpdateViewModel : EventViewModel
    {
        private List<CashAidRecepientViewModel> _cashAidRecepients = new List<CashAidRecepientViewModel>();
        private List<CashAidNonVoterViewModel> _cashAidNonVoters = new List<CashAidNonVoterViewModel>();
        public List<CashAidRecepientViewModel> cashAidRecepients
        {
            get { return _cashAidRecepients; }
            set { _cashAidRecepients = value; }
        }

        public List<CashAidNonVoterViewModel> cashAidNonVoters
        {
            get { return _cashAidNonVoters; }
            set { _cashAidNonVoters = value; }
        }

        [Required]
        public string datestr { get { return date != null ? ((DateTime)date).ToString("MM/dd/yyyy") : ""; } }
    }
}
