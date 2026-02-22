using System.ComponentModel.DataAnnotations;

namespace TrainMatePro.DTOs
{
    public class CreateWorkoutDto
    {
        [Required]
        public int StudentId { get; set; }  // StudentProfile.Id

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int DurationWeeks { get; set; }

        public string? Notes { get; set; }

        [Required]
        public List<CreateWorkoutWeekDto> Weeks { get; set; } = new();
    }

    public class CreateWorkoutWeekDto
    {
        [Required]
        public int WeekNumber { get; set; }

        public string? Title { get; set; }
        public string? Focus { get; set; }
        public string? Notes { get; set; }

        [Required]
        public List<CreateWorkoutDayDto> Days { get; set; } = new();
    }

    public class CreateWorkoutDayDto
    {
        [Required]
        public int DayNumber { get; set; }

        [Required]
        public string DayName { get; set; } = string.Empty;

        public string? Title { get; set; }
        public string? Focus { get; set; }
        public int? Duration { get; set; }
        public string? Notes { get; set; }

        public List<CreateExerciseDto> Exercises { get; set; } = new();
    }

    public class CreateExerciseDto
    {
        public string? ExerciseId { get; set; } // از API خارجی
        public int Order { get; set; } = 1;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Sets { get; set; } = 3;
        public string Reps { get; set; } = "10-12";
        public string? RestTime { get; set; }
        public string? Notes { get; set; }
        public string? GifUrl { get; set; }
        public string[]? TargetMuscles { get; set; }
        public string[]? BodyParts { get; set; }
        public string[]? Equipments { get; set; }
        public string[]? Instructions { get; set; }
    }
}