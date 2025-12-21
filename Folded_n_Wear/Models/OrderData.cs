using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Folded_n_Wear.Models
{
    public class OrderSummaryViewModel
    {
        [Key]
        public string OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DatePickup { get; set; }
        public string IsRushed { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Quantity { get; set; }

        public string OrderStatus { get; set; } = "Washing";

    }

    public class ItemViewModel
    {
        [Key]
        public int ItemID { get; set; }
        public string ItemTypeName { get; set; }
        public int Quantity { get; set; }
    }

}

