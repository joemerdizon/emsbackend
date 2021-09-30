using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Event
    {
        public Event()
        {
            CashAidNonVoter = new HashSet<CashAidNonVoter>();
            CashAidRecepients = new HashSet<CashAidRecepients>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int TypeOfEvent { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [StringLength(250)]
        public string Location { get; set; }
        public string AdditionalDetails { get; set; }
        public double? Budget { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        public int? DeletedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate { get; set; }
        public bool? IsClosed { get; set; }
        public double? RunningAmount { get; set; }
        public double? TotalCashAid { get; set; }
        public double? AdditionalBudget { get; set; }
        public double? RunningCashAid { get; set; }
        public double? RunningAdditionalBudget { get; set; }

        [InverseProperty("Event")]
        public virtual ICollection<CashAidNonVoter> CashAidNonVoter { get; set; }
        [InverseProperty("Event")]
        public virtual ICollection<CashAidRecepients> CashAidRecepients { get; set; }
    }
}
