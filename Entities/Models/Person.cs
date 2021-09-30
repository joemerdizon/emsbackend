using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Person
    {
        public Person()
        {
            ActivationToken = new HashSet<ActivationToken>();
            CashAidRecepients = new HashSet<CashAidRecepients>();
            User = new HashSet<User>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
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
        [Column("GCashNumber")]
        [StringLength(50)]
        public string GcashNumber { get; set; }
        [Column("VotersIDNo")]
        [StringLength(100)]
        public string VotersIdno { get; set; }
        [StringLength(20)]
        public string MobileNo { get; set; }
        public string School { get; set; }
        public bool? IsMember { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        public int? Gender { get; set; }
        public int? BrgyId { get; set; }
        public int? ClusterId { get; set; }
        public int? PrecinctId { get; set; }
        public int MembershipStatus { get; set; }
        public int? VoterId { get; set; }
        public Guid? ApprovalId { get; set; }
        [StringLength(255)]
        public string EmailAddress { get; set; }

        [ForeignKey(nameof(BrgyId))]
        [InverseProperty(nameof(Barangay.Person))]
        public virtual Barangay Brgy { get; set; }
        [ForeignKey(nameof(ClusterId))]
        [InverseProperty("Person")]
        public virtual Cluster Cluster { get; set; }
        [ForeignKey(nameof(PrecinctId))]
        [InverseProperty("Person")]
        public virtual Precinct Precinct { get; set; }
        [ForeignKey(nameof(VoterId))]
        [InverseProperty("Person")]
        public virtual Voter Voter { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<ActivationToken> ActivationToken { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<CashAidRecepients> CashAidRecepients { get; set; }
        [InverseProperty("Person")]
        public virtual ICollection<User> User { get; set; }
    }
}
