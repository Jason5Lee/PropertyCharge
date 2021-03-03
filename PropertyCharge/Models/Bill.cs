using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Models
{
    public class Bill
    {
        [Required]
        public ulong PersonaleId { get; set; }
        [Required]
        public IReadOnlyList<Charge> Charges { get; set; }
        [Required]
        public decimal BillFee { get; set; }
    }
}
