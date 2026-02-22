using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DayId { get; set; }

        [JsonIgnore]
        public WorkoutDay? Day { get; set; }

        public int? ExerciseRefId { get; set; }  // اشاره به ExerciseRef

        public ExerciseRef? ExerciseRef { get; set; }

        public string? ExerciseApiId { get; set; } // مستقیم از API

        public int Order { get; set; } = 1;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Sets { get; set; } = 3;

        public string Reps { get; set; } = "10-12";

        public string? RestTime { get; set; } = "60-90 ثانیه";

        public string? Notes { get; set; }

        public string? GifUrl { get; set; } // کش شده از API
    }
}