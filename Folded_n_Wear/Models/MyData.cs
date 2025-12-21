using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Folded_n_Wear.Models
{
    public class MyData
    {
        [Key]
        public int customerID { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MaxLength(255)]
        public string password { get; set; }

        [Required]
        [MaxLength(50)]
        public string custName { get; set; }

        [Required]
        [MaxLength(15)]
        public string contactNo { get; set; }


        [Required]
        public string address { get; set; }

        [Required]
        public DateTime dateJoined { get; set; }


        [Required]
        public string role { get; set; } = "Customer";

    }
}
