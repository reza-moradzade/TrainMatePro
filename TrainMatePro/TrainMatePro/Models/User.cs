using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainMatePro.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "student"; // "coach" or "student"

        public string Department { get; set; } = "General";

        // اگر student باشه، مربیش کیه؟
        public int? CoachId { get; set; }

        [JsonIgnore]
        public User? Coach { get; set; }

        // لیست شاگردها (اگر مربی باشه)
        [JsonIgnore]
        public ICollection<User> Students { get; set; } = new List<User>();

        // پروفایل شاگرد (اگر student باشه)
        public StudentProfile? StudentProfile { get; set; }

        // برنامه‌هایی که این کاربر (به عنوان مربی) ساخته
        [JsonIgnore]
        public ICollection<WorkoutProgram> CoachPrograms { get; set; } = new List<WorkoutProgram>();
    }
}