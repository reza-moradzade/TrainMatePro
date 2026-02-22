using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class WorkoutDay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WeekId { get; set; }

        [JsonIgnore]
        public WorkoutWeek? Week { get; set; }

        [Required]
        public int DayNumber { get; set; } // 1 to 7

        [Required]
        public string DayName { get; set; } = string.Empty; // saturday, sunday, ...

        public string Title { get; set; } = "روز تمرین";

        public string? Focus { get; set; }

        public int? Duration { get; set; } // دقیقه

        public string? Notes { get; set; }

        // حرکات این روز
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}