using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("temp_precint")]
    public partial class TempPrecint
    {
        [StringLength(50)]
        public string Cluster { get; set; }
        [StringLength(50)]
        public string Precint { get; set; }
    }
}
