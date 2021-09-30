using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("temp_voterlist")]
    public partial class TempVoterlist
    {
        [StringLength(50)]
        public string No { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string MiddleName { get; set; }
        public string Address { get; set; }
        [StringLength(50)]
        public string Sex { get; set; }
        [StringLength(50)]
        public string Precinct { get; set; }
        [StringLength(50)]
        public string Cluster { get; set; }
        public string Brgy { get; set; }
        [Column("DOB", TypeName = "datetime")]
        public DateTime? Dob { get; set; }
        public string School { get; set; }
        [StringLength(50)]
        public string Suffix { get; set; }
    }
}
