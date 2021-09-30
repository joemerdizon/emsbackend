using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class ActivationToken
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PersonID")]
        public int PersonId { get; set; }
        [Required]
        [StringLength(100)]
        public string Token { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ExpireDate { get; set; }
        public bool? IsActive { get; set; }

        [ForeignKey(nameof(PersonId))]
        [InverseProperty("ActivationToken")]
        public virtual Person Person { get; set; }
    }
}
