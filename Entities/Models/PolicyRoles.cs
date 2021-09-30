using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class PolicyRoles
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(PolicyId))]
        [InverseProperty("PolicyRoles")]
        public virtual Policy Policy { get; set; }
        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(UserRole.PolicyRoles))]
        public virtual UserRole Role { get; set; }
    }
}
