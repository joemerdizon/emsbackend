using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class PollViewModel : BaseViewModel
    {      
        
        [Required]
        [StringLength(250, ErrorMessage = "Must be between 3 and 250 characters", MinimumLength = 3)]
        public string pollDescription { get; set; } = string.Empty;
        public DateTime? expiryDate { get; set; }
        public string status { get; set; }

    }

    public class PollUpdateViewModel : PollViewModel
    {

        private IList<PollOptionsViewModel> _pollOptions = new List<PollOptionsViewModel>();

        [Required]
        public string datestr { get { return expiryDate != null ? ((DateTime)expiryDate).ToString("MM/dd/yyyy") : ""; } }

        public IList<PollOptionsViewModel> pollOptions
        {
            get { return _pollOptions; }
            set { _pollOptions = value; }
        }
    }

    public class PollOptionsViewModel : BaseViewModel
    {
        public int pollId { get; set; }

        public string optionDescription { get; set; } = string.Empty;
    }
}
