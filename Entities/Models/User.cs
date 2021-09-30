using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public partial class User
    {
        public User()
        {
            PollOptionAnswer = new HashSet<PollOptionAnswer>();
            UserRefreshToken = new HashSet<UserRefreshToken>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int UserRoleId { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string ProfilePicture { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        [Column("PersonID")]
        public int? PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        [InverseProperty("User")]
        public virtual Person Person { get; set; }
        [ForeignKey(nameof(UserRoleId))]
        [InverseProperty("User")]
        public virtual UserRole UserRole { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<PollOptionAnswer> PollOptionAnswer { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserRefreshToken> UserRefreshToken { get; set; }
    }
}
