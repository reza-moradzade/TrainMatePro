using System.ComponentModel.DataAnnotations;

namespace TrainMatePro.DTOs
{
    public class CreateStudentDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Department { get; set; }

        // StudentProfile fields
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string? FitnessLevel { get; set; }
        public string? Goals { get; set; }
        public string? Notes { get; set; }
    }
}