using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class PolicyModuleControl
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public bool IsRestricted { get; set; }
        public int PolicyId { get; set; }
        public int ModuleControlId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(ModuleControlId))]
        [InverseProperty("PolicyModuleControl")]
        public virtual ModuleControl ModuleControl { get; set; }
        [ForeignKey(nameof(PolicyId))]
        [InverseProperty("PolicyModuleControl")]
        public virtual Policy Policy { get; set; }
    }
}
