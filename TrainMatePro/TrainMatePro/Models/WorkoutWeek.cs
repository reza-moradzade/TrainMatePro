using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class WorkoutWeek
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProgramId { get; set; }

        [JsonIgnore]
        public WorkoutProgram? Program { get; set; }

        [Required]
        public int WeekNumber { get; set; } // 1 to 12

        public string Title { get; set; } = "هفته تمرینی";

        public string? Focus { get; set; }

        public string? Notes { get; set; }

        // روزهای این هفته
        public ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();
    }
}