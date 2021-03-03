using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Models
{
    public class Charge
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "numeric(18,0)")]
        [ForeignKey("Personale")]
        public ulong PersonaleId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool Paid { get; set; }
        [Required]
        [Column(TypeName = "numeric(18,2)")]
        public decimal Fee { get; set; }

        public virtual Personale Personale { get; set; }
    }
}
