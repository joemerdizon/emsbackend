using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Zone
    {
        public Zone()
        {
            Barangay = new HashSet<Barangay>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public int DistrictId { get; set; }

        [ForeignKey(nameof(DistrictId))]
        [InverseProperty("Zone")]
        public virtual District District { get; set; }
        [InverseProperty("Zone")]
        public virtual ICollection<Barangay> Barangay { get; set; }
    }
}
