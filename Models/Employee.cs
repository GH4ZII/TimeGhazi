using System.ComponentModel.DataAnnotations; // Gir tilgang til valideringsegenskaper
using System.ComponentModel.DataAnnotations.Schema; // Brukes for databasekonfigurasjon
using Microsoft.AspNetCore.Identity; // Gir tilgang til ASP.NET Identity

namespace TimeGhazi.Models
{
    // **Definerer Employee-tabellen som lagrer ansatte i databasen**
    public class Employee
    {
        [Key] // **Angir at dette er primærnøkkelen**
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // **Auto-increment ID**
        public int Id { get; set; }

        [Required] // **Feltet er obligatorisk**
        [StringLength(100)] // **Begrenser lengden på navnet til 100 tegn**
        public string Name { get; set; }

        [Required, EmailAddress] // **Påkrevd og må være en gyldig e-post**
        public string Email { get; set; }
        
        [Required] // **Påkrevd felt**
        [Phone] // **Må være et gyldig telefonnummer**
        public string PhoneNumber { get; set; }

        [Required] // **Påkrevd felt**
        [StringLength(50)] // **Maks 50 tegn for rollen**
        public string Role { get; set; } // **F.eks. "Admin" eller "Employee"**
        
        [Required] // **Påkrevd felt**
        public int Age { get; set; } // **Alder på den ansatte**
        
        [Required] // **Påkrevd felt**
        public string Address { get; set; } // **Adresse til den ansatte**

        // **Kobler Employee til IdentityUser (brukeren i ASP.NET Identity)**
        [Required] // **Påkrevd for å koble ansatt til en IdentityUser**
        [Column(TypeName = "TEXT")] // **Lagrer UserId som en tekststreng i databasen**
        public string UserId { get; set; }

        [ForeignKey("UserId")] // **Definerer fremmednøkkel-relasjon til IdentityUser**
        public IdentityUser? User { get; set; } // **Navigasjonsfelt for å hente tilknyttet bruker**
    }
}