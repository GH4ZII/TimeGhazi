using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // For List<Shift>

namespace TimeGhazi.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string Address { get; set; }

        // 🔹 Kobling til IdentityUser
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; } 

        // 🔹 Knytter Employee til Shift-tabellen (1 Employee har mange Shift)
        public List<Shift> Shifts { get; set; } = new List<Shift>(); // 🔥 Nytt
    }
}