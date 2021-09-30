using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Barangay
    {
        public Barangay()
        {
            CashAidNonVoter = new HashSet<CashAidNonVoter>();
            Cluster = new HashSet<Cluster>();
            Person = new HashSet<Person>();
            Voter = new HashSet<Voter>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public int ZoneId { get; set; }

        [ForeignKey(nameof(ZoneId))]
        [InverseProperty("Barangay")]
        public virtual Zone Zone { get; set; }
        [InverseProperty("Brgy")]
        public virtual ICollection<CashAidNonVoter> CashAidNonVoter { get; set; }
        [InverseProperty("Brgy")]
        public virtual ICollection<Cluster> Cluster { get; set; }
        [InverseProperty("Brgy")]
        public virtual ICollection<Person> Person { get; set; }
        [InverseProperty("Brgy")]
        public virtual ICollection<Voter> Voter { get; set; }
    }
}
