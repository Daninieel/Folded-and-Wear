using System.ComponentModel.DataAnnotations;

namespace Folded_n_Wear.Models
{
    public class AdminData
    {
        public int customerID { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string custName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string contactNo { get; set; } = string.Empty;

        [Required]
        public string address { get; set; } = string.Empty;


        [Required]
        public string role { get; set; } = string.Empty;
    }
}
