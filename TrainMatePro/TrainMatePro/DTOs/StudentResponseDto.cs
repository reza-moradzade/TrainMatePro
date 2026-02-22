namespace TrainMatePro.DTOs
{
    public class StudentResponseDto
    {
        public int UserId { get; set; }
        public int? StudentId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // StudentProfile fields
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string FitnessLevel { get; set; } = "beginner";
        public string? Goals { get; set; }
        public string? Notes { get; set; }
    }
}