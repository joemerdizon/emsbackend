using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Precinct
    {
        public Precinct()
        {
            Person = new HashSet<Person>();
            Voter = new HashSet<Voter>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public int? ClusterId { get; set; }

        [ForeignKey(nameof(ClusterId))]
        [InverseProperty("Precinct")]
        public virtual Cluster Cluster { get; set; }
        [InverseProperty("Precinct")]
        public virtual ICollection<Person> Person { get; set; }
        [InverseProperty("Precinct")]
        public virtual ICollection<Voter> Voter { get; set; }
    }
}
