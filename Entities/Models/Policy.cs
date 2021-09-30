using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Policy
    {
        public Policy()
        {
            PolicyModuleControl = new HashSet<PolicyModuleControl>();
            PolicyRoles = new HashSet<PolicyRoles>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [InverseProperty("Policy")]
        public virtual ICollection<PolicyModuleControl> PolicyModuleControl { get; set; }
        [InverseProperty("Policy")]
        public virtual ICollection<PolicyRoles> PolicyRoles { get; set; }
    }
}
