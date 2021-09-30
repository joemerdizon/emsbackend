using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class PollOptionAnswer
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PollOptionID")]
        public int PollOptionId { get; set; }
        [Column("UserID")]
        public int UserId { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(PollOptionId))]
        [InverseProperty("PollOptionAnswer")]
        public virtual PollOption PollOption { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("PollOptionAnswer")]
        public virtual User User { get; set; }
    }
}
