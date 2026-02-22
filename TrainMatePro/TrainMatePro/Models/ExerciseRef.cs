using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class ExerciseRef
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ExerciseId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string GifUrl { get; set; } = string.Empty;

        // این فیلدها باید به صورت آرایه JSON ذخیره بشن
        public string[] TargetMuscles { get; set; } = Array.Empty<string>();
        public string[] BodyParts { get; set; } = Array.Empty<string>();
        public string[] Equipments { get; set; } = Array.Empty<string>();
        public string[] SecondaryMuscles { get; set; } = Array.Empty<string>();
        public string[] Instructions { get; set; } = Array.Empty<string>();

        public DateTime LastSynced { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}