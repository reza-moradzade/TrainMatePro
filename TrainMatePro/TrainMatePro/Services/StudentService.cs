using Microsoft.EntityFrameworkCore;
using TrainMatePro.Data;
using TrainMatePro.DTOs;
using TrainMatePro.Models;
using BCrypt.Net;

namespace TrainMatePro.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentResponseDto>> GetCoachStudentsAsync(int coachId)
        {
            var students = await _context.Users
                .Include(u => u.StudentProfile)
                .Where(u => u.CoachId == coachId && u.Role == "student")
                .OrderByDescending(u => u.Id)
                .ToListAsync();

            return students.Select(student => new StudentResponseDto
            {
                UserId = student.Id,
                StudentId = student.StudentProfile?.Id,
                Email = student.Email,
                FullName = student.FullName,
                Department = student.Department,
                CreatedAt = student.Id > 0 ? DateTime.Now.AddDays(-student.Id) : DateTime.Now, // تقریبی
                Age = student.StudentProfile?.Age,
                Gender = student.StudentProfile?.Gender,
                Height = student.StudentProfile?.Height,
                Weight = student.StudentProfile?.Weight,
                FitnessLevel = student.StudentProfile?.FitnessLevel ?? "beginner",
                Goals = student.StudentProfile?.Goals,
                Notes = student.StudentProfile?.Notes
            }).ToList();
        }

        public async Task<StudentResponseDto?> CreateStudentAsync(int coachId, CreateStudentDto dto)
        {
            // بررسی وجود ایمیل
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return null; // ایمیل تکراری
            }

            // ایجاد کاربر جدید
            var studentUser = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Role = "student",
                Department = dto.Department ?? "ورزشکار",
                CoachId = coachId
            };

            _context.Users.Add(studentUser);
            await _context.SaveChangesAsync();

            // ایجاد پروفایل شاگرد
            var studentProfile = new StudentProfile
            {
                UserId = studentUser.Id,
                Age = dto.Age,
                Gender = dto.Gender,
                Height = dto.Height,
                Weight = dto.Weight,
                FitnessLevel = dto.FitnessLevel ?? "beginner",
                Goals = dto.Goals,
                Notes = dto.Notes
            };

            _context.StudentProfiles.Add(studentProfile);
            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                UserId = studentUser.Id,
                StudentId = studentProfile.Id,
                Email = studentUser.Email,
                FullName = studentUser.FullName,
                Department = studentUser.Department,
                CreatedAt = DateTime.Now,
                Age = studentProfile.Age,
                Gender = studentProfile.Gender,
                Height = studentProfile.Height,
                Weight = studentProfile.Weight,
                FitnessLevel = studentProfile.FitnessLevel,
                Goals = studentProfile.Goals,
                Notes = studentProfile.Notes
            };
        }

        public async Task<bool> IsStudentBelongsToCoachAsync(int studentUserId, int coachId)
        {
            var student = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == studentUserId && u.CoachId == coachId && u.Role == "student");

            return student != null;
        }
    }
}