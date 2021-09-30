using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class PollOption
    {
        public PollOption()
        {
            PollOptionAnswer = new HashSet<PollOptionAnswer>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PollID")]
        public int PollId { get; set; }
        [Required]
        public string OptionDescription { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(PollId))]
        [InverseProperty("PollOption")]
        public virtual Poll Poll { get; set; }
        [InverseProperty("PollOption")]
        public virtual ICollection<PollOptionAnswer> PollOptionAnswer { get; set; }
    }
}
