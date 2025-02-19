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

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } // 🔹 Knytter skiftet til en ansatt

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}