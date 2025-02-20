using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeGhazi.Models
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; } // 🔹 ID-en til den ansatte

        [ForeignKey("EmployeeId")] // ✅ Knytter Shift til Employee-tabellen
        public Employee? Employee { get; set; } // 🔹 Navigasjonsfelt til Employee

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}