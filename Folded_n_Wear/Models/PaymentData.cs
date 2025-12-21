using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Folded_n_Wear.Models
{
    public class PaymentData
    {
        [Key]
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
        public string OrderID { get; set; }
        public int balance { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime PaymentDate { get; set; }
    }
}
