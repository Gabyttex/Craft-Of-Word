//using CraftOfWord.Auth.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Word
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Foreign key to Round
    [Required]
    public int RoundId { get; set; }

    [ForeignKey("RoundId")]
    public Round Round { get; set; }

    // Foreign key to User
    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Required, MinLength(1)]
    public string Content { get; set; }

    public int Points { get; set; }
}
