using System; // Gir tilgang til grunnleggende datatyper som DateTime
using System.ComponentModel.DataAnnotations; // Brukes for validering av data

namespace TimeGhazi.Models
{
    // **Definerer en Shift-modell som representerer et arbeidsskift**
    public class Shift
    {
        [Key] // **Markerer dette feltet som primærnøkkelen**
        public int Id { get; set; }

        [Required] // **Feltet er obligatorisk**
        public int EmployeeId { get; set; } // **Knytter skiftet til en ansatt**

        [Required] // **Feltet er obligatorisk**
        public DateTime StartTime { get; set; } // **Starttidspunkt for skiftet**

        [Required] // **Feltet er obligatorisk**
        public DateTime EndTime { get; set; } // **Sluttidspunkt for skiftet**

        public bool IsApproved { get; set; } = false; // **Angir om skiftet er godkjent (standard: false)**
    }
}