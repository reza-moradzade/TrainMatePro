using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class WorkoutProgram
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }  // اشاره به StudentProfile.Id

        [JsonIgnore]
        public StudentProfile? Student { get; set; }

        [Required]
        public int CoachId { get; set; }  // اشاره به User.Id

        [JsonIgnore]
        public User? Coach { get; set; }

        [Required]
        public string Title { get; set; } = "برنامه تمرینی جدید";

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int DurationWeeks { get; set; } = 4;

        public string Status { get; set; } = "active"; // active, completed, cancelled

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // هفته‌های برنامه
        public ICollection<WorkoutWeek> WorkoutWeeks { get; set; } = new List<WorkoutWeek>();
    }
}