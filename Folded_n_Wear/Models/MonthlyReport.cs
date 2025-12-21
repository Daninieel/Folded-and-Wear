using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Folded_n_Wear.Models
{
    public class MonthlyReport
    {
        [Key]
        public int ReportID { get; set; }
        public DateTime DateOfReport { get; set; } 

        public decimal TotalEarnings { get; set; } 
        public decimal TotalKilosProcessed { get; set; }
    }
}