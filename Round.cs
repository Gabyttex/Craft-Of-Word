//using CraftOfWord.Auth.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Round
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int TournamentId { get; set; }
    [ForeignKey("TournamentId")]
    public Tournament Tournament { get; set; }

    public DateTime StartTime { get; set; }
    public TimeSpan Length { get; set; }

    [JsonIgnore]
    public ICollection<Word> Words { get; set; } = new List<Word>();
}
