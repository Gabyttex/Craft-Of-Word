using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key] // Explicitly define primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Use identity for ID generation
    public int Id { get; set; }

    [Required, MinLength(1)]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(8)]
    public string Password { get; set; }

    public int Points { get; set; }

    [Required]
    public string Role { get; set; } = "User"; // Default role is "User"

    public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    public ICollection<Word> Words { get; set; } = new List<Word>();
}
