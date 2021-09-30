using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class District
    {
        public District()
        {
            Zone = new HashSet<Zone>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }

        [InverseProperty("District")]
        public virtual ICollection<Zone> Zone { get; set; }
    }
}
