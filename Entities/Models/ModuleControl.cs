using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class ModuleControl
    {
        public ModuleControl()
        {
            PolicyModuleControl = new HashSet<PolicyModuleControl>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public int ControlId { get; set; }
        public int ModuleId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(ModuleId))]
        [InverseProperty("ModuleControl")]
        public virtual Module Module { get; set; }
        [InverseProperty("ModuleControl")]
        public virtual ICollection<PolicyModuleControl> PolicyModuleControl { get; set; }
    }
}
