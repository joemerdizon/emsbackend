using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.ViewModel
{
    public class BaseViewModel
    {
        private DateTime? _updatedDate;
        
        [Key]
        public int id { get; set; }
        public string updatedBy { get; set; } = string.Empty;

        public virtual DateTime? updatedDate
        {
            get { return _updatedDate; }
            set { _updatedDate = value; }
        }

        public virtual bool isActive { get; set; }

    }
}
