using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class StudentProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public int? Age { get; set; }

        public string? Gender { get; set; } // "male" or "female"

        public float? Height { get; set; }

        public float? Weight { get; set; }

        public string FitnessLevel { get; set; } = "beginner"; // beginner, intermediate, advanced

        public string? Goals { get; set; }

        public string? Notes { get; set; }

        // برنامه‌های تمرینی این شاگرد
        [JsonIgnore]
        public ICollection<WorkoutProgram> WorkoutPrograms { get; set; } = new List<WorkoutProgram>();
    }
}