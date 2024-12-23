//using CraftOfWord.Auth.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Tournament
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MinLength(1)]
    public string Name { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? WinnerId { get; set; }

    [ForeignKey("WinnerId")]
    public User Winner { get; set; }

    [JsonIgnore]
    public ICollection<User> Participants { get; set; } = new List<User>();

    [JsonIgnore]
    public ICollection<Round> Rounds { get; set; } = new List<Round>();
}
