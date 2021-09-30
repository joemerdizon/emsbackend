using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            PolicyRoles = new HashSet<PolicyRoles>();
            User = new HashSet<User>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        public string Role { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        [Column("OldID")]
        public int? OldId { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<PolicyRoles> PolicyRoles { get; set; }
        [InverseProperty("UserRole")]
        public virtual ICollection<User> User { get; set; }
    }
}
