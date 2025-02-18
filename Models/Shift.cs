using System;
using System.ComponentModel.DataAnnotations;

namespace TimeGhazi.Models
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}