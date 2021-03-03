using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Models
{
    public class Personale
    {
        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public ulong Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Column(TypeName = "char(32)")]
        public string Password { get; set; }
    }
}
