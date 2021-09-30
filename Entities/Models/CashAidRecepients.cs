using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class CashAidRecepients
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int PaymentType { get; set; }
        [Column("PersonID")]
        public int PersonId { get; set; }
        [Column("EventID")]
        public int EventId { get; set; }
        public double? Amount { get; set; }
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

        [ForeignKey(nameof(EventId))]
        [InverseProperty("CashAidRecepients")]
        public virtual Event Event { get; set; }
        [ForeignKey(nameof(PersonId))]
        [InverseProperty("CashAidRecepients")]
        public virtual Person Person { get; set; }
    }
}
