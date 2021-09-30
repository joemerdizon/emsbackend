using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class CashAidNonVoter
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("EventID")]
        public int EventId { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        public string Address { get; set; }
        [Column("DOB", TypeName = "datetime")]
        public DateTime? Dob { get; set; }
        [StringLength(20)]
        public string MobileNo { get; set; }
        public int? Gender { get; set; }
        public int? BrgyId { get; set; }
        public double? Amount { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(BrgyId))]
        [InverseProperty(nameof(Barangay.CashAidNonVoter))]
        public virtual Barangay Brgy { get; set; }
        [ForeignKey(nameof(EventId))]
        [InverseProperty("CashAidNonVoter")]
        public virtual Event Event { get; set; }
    }
}
