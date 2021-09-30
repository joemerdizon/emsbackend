using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Voter
    {
        public Voter()
        {
            Person = new HashSet<Person>();
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
        [Column("VotersIDNo")]
        [StringLength(100)]
        public string VotersIdno { get; set; }
        [StringLength(20)]
        public string MobileNo { get; set; }
        public string School { get; set; }
        public int? Gender { get; set; }
        public int? BrgyId { get; set; }
        public int? ClusterId { get; set; }
        public int? PrecinctId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(BrgyId))]
        [InverseProperty(nameof(Barangay.Voter))]
        public virtual Barangay Brgy { get; set; }
        [ForeignKey(nameof(ClusterId))]
        [InverseProperty("Voter")]
        public virtual Cluster Cluster { get; set; }
        [ForeignKey(nameof(PrecinctId))]
        [InverseProperty("Voter")]
        public virtual Precinct Precinct { get; set; }
        [InverseProperty("Voter")]
        public virtual ICollection<Person> Person { get; set; }
    }
}
