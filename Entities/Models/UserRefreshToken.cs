using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class UserRefreshToken
    {
        [Key]
        public string UserName { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public string Token { get; set; }
        public int? UserId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserRefreshToken")]
        public virtual User User { get; set; }
    }
}
