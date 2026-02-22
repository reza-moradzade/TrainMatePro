namespace TrainMatePro.DTOs
{
    public class WorkoutResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationWeeks { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public StudentInfoDto? Student { get; set; }
        public CoachInfoDto? Coach { get; set; }

        public List<WorkoutWeekDto> Weeks { get; set; } = new();
    }

    public class StudentInfoDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CoachInfoDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class WorkoutWeekDto
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Focus { get; set; }
        public string? Notes { get; set; }
        public List<WorkoutDayDto> Days { get; set; } = new();
    }

    public class WorkoutDayDto
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public string DayName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Focus { get; set; }
        public int? Duration { get; set; }
        public string? Notes { get; set; }
        public List<ExerciseDto> Exercises { get; set; } = new();
    }

    public class ExerciseDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Sets { get; set; }
        public string Reps { get; set; } = string.Empty;
        public string? RestTime { get; set; }
        public string? Notes { get; set; }
        public string? GifUrl { get; set; }
    }
}