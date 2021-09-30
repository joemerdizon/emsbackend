using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class Module
    {
        public Module()
        {
            ModuleControl = new HashSet<ModuleControl>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int ParentModuleId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        public int PageId { get; set; }
        [StringLength(50)]
        public string IconClass { get; set; }

        [InverseProperty("Module")]
        public virtual ICollection<ModuleControl> ModuleControl { get; set; }
    }
}
