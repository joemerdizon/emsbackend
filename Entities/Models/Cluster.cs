using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Cluster
    {
        public Cluster()
        {
            Person = new HashSet<Person>();
            Precinct = new HashSet<Precinct>();
            Voter = new HashSet<Voter>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public int? BrgyId { get; set; }

        [ForeignKey(nameof(BrgyId))]
        [InverseProperty(nameof(Barangay.Cluster))]
        public virtual Barangay Brgy { get; set; }
        [InverseProperty("Cluster")]
        public virtual ICollection<Person> Person { get; set; }
        [InverseProperty("Cluster")]
        public virtual ICollection<Precinct> Precinct { get; set; }
        [InverseProperty("Cluster")]
        public virtual ICollection<Voter> Voter { get; set; }
    }
}
