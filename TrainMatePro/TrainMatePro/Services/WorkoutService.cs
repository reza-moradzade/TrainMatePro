using Microsoft.EntityFrameworkCore;
using TrainMatePro.Data;
using TrainMatePro.DTOs;
using TrainMatePro.Models;

namespace TrainMatePro.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly AppDbContext _context;
        private readonly IStudentService _studentService;
        private readonly ILogger<WorkoutService> _logger;

        // ✅ اصلاح سازنده - اضافه کردن ILogger
        public WorkoutService(
            AppDbContext context,
            IStudentService studentService,
            ILogger<WorkoutService> logger)  // اینجا اضافه کن
        {
            _context = context;
            _studentService = studentService;
            _logger = logger;  // ✅ مقداردهی کن
        }

        public async Task<List<WorkoutResponseDto>> GetCoachWorkoutsAsync(int coachId, int? studentId = null)
        {
            try
            {
                // اول همه شاگردهای این مربی رو پیدا می‌کنیم
                var students = await _context.StudentProfiles
                    .Include(s => s.User)
                    .Where(s => s.User != null && s.User.CoachId == coachId)
                    .Select(s => s.Id)
                    .ToListAsync();

                _logger.LogDebug($"Found {students.Count} students for coach {coachId}");

                var query = _context.WorkoutPrograms
                    .Include(w => w.Student)
                        .ThenInclude(s => s != null ? s.User : null)
                    .Include(w => w.Coach)
                    .Include(w => w.WorkoutWeeks)
                        .ThenInclude(ww => ww.WorkoutDays)
                            .ThenInclude(wd => wd.Exercises)
                                .ThenInclude(e => e.ExerciseRef)
                    .Where(w => students.Contains(w.StudentId));

                // اگر studentId مشخص شده، فیلتر کن
                if (studentId.HasValue)
                {
                    _logger.LogDebug($"Filtering by studentId: {studentId}");

                    // چک کن این studentId متعلق به این مربی هست؟
                    if (!students.Contains(studentId.Value))
                    {
                        _logger.LogWarning($"Student {studentId} not found in coach's students");
                        return new List<WorkoutResponseDto>();
                    }
                    query = query.Where(w => w.StudentId == studentId.Value);
                }

                var programs = await query
                    .OrderByDescending(w => w.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {programs.Count} workouts for coach {coachId}");
                return programs.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coach workouts for coach {CoachId}", coachId);
                throw;
            }
        }

        public async Task<List<WorkoutResponseDto>> GetStudentWorkoutsAsync(int userId)
        {
            try
            {
                var student = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    _logger.LogWarning($"Student profile not found for user {userId}");
                    return new List<WorkoutResponseDto>();
                }

                var programs = await _context.WorkoutPrograms
                    .Include(w => w.Student)
                        .ThenInclude(s => s != null ? s.User : null)
                    .Include(w => w.Coach)
                    .Include(w => w.WorkoutWeeks)
                        .ThenInclude(ww => ww.WorkoutDays)
                            .ThenInclude(wd => wd.Exercises)
                                .ThenInclude(e => e.ExerciseRef)
                    .Where(w => w.StudentId == student.Id)
                    .OrderByDescending(w => w.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {programs.Count} workouts for student {userId}");
                return programs.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student workouts for user {UserId}", userId);
                throw;
            }
        }

        public async Task<WorkoutResponseDto?> GetWorkoutByIdAsync(int workoutId, int userId, string role)
        {
            try
            {
                var program = await _context.WorkoutPrograms
                    .Include(w => w.Student)
                        .ThenInclude(s => s != null ? s.User : null)
                    .Include(w => w.Coach)
                    .Include(w => w.WorkoutWeeks)
                        .ThenInclude(ww => ww.WorkoutDays)
                            .ThenInclude(wd => wd.Exercises)
                                .ThenInclude(e => e.ExerciseRef)
                    .FirstOrDefaultAsync(w => w.Id == workoutId);

                if (program == null)
                {
                    _logger.LogWarning($"Workout {workoutId} not found");
                    return null;
                }

                // بررسی دسترسی
                if (role == "coach" && program.CoachId != userId)
                {
                    _logger.LogWarning($"Coach {userId} attempted to access workout {workoutId} without permission");
                    return null;
                }

                if (role == "student")
                {
                    var student = await _context.StudentProfiles
                        .FirstOrDefaultAsync(s => s.UserId == userId);

                    if (student == null || program.StudentId != student.Id)
                    {
                        _logger.LogWarning($"Student {userId} attempted to access workout {workoutId} without permission");
                        return null;
                    }
                }

                _logger.LogInformation($"Retrieved workout {workoutId} for {role} {userId}");
                return MapToDto(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workout {WorkoutId}", workoutId);
                throw;
            }
        }

        public async Task<bool> DeleteWorkoutAsync(int workoutId)
        {
            try
            {
                var program = await _context.WorkoutPrograms
                    .Include(w => w.WorkoutWeeks)
                        .ThenInclude(ww => ww.WorkoutDays)
                            .ThenInclude(wd => wd.Exercises)
                    .FirstOrDefaultAsync(w => w.Id == workoutId);

                if (program == null)
                {
                    _logger.LogWarning($"Workout {workoutId} not found for deletion");
                    return false;
                }

                // حذف تمام Exercises
                foreach (var week in program.WorkoutWeeks)
                {
                    foreach (var day in week.WorkoutDays)
                    {
                        _context.Exercises.RemoveRange(day.Exercises);
                    }
                }

                // حذف تمام WorkoutDays
                foreach (var week in program.WorkoutWeeks)
                {
                    _context.WorkoutDays.RemoveRange(week.WorkoutDays);
                }

                // حذف تمام WorkoutWeeks
                _context.WorkoutWeeks.RemoveRange(program.WorkoutWeeks);

                // حذف برنامه اصلی
                _context.WorkoutPrograms.Remove(program);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Workout {workoutId} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workout {WorkoutId}", workoutId);
                return false;
            }
        }

        public async Task<int?> CreateWorkoutAsync(int coachId, CreateWorkoutDto dto)
        {
            try
            {
                // بررسی اینکه studentId متعلق به این مربی هست
                var student = await _context.StudentProfiles
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == dto.StudentId && s.User != null && s.User.CoachId == coachId);

                if (student == null)
                {
                    _logger.LogWarning($"Student {dto.StudentId} not found or not assigned to coach {coachId}");
                    return null;
                }

                // ✅ تبدیل تاریخ‌ها به UTC
                var startDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
                var endDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);

                // ایجاد برنامه اصلی
                var program = new WorkoutProgram
                {
                    StudentId = student.Id,
                    CoachId = coachId,
                    Title = dto.Title,
                    Description = dto.Description ?? "",
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationWeeks = dto.DurationWeeks,
                    Status = "active",
                    Notes = dto.Notes ?? ""
                };

                _context.WorkoutPrograms.Add(program);
                await _context.SaveChangesAsync();

                _logger.LogDebug($"Created workout program with ID: {program.Id}");

                // ایجاد هفته‌ها
                foreach (var weekDto in dto.Weeks)
                {
                    var week = new WorkoutWeek
                    {
                        ProgramId = program.Id,
                        WeekNumber = weekDto.WeekNumber,
                        Title = weekDto.Title ?? $"هفته {weekDto.WeekNumber}",
                        Focus = weekDto.Focus ?? "",
                        Notes = weekDto.Notes ?? ""
                    };

                    _context.WorkoutWeeks.Add(week);
                    await _context.SaveChangesAsync();

                    // ایجاد روزها
                    foreach (var dayDto in weekDto.Days)
                    {
                        var day = new WorkoutDay
                        {
                            WeekId = week.Id,
                            DayNumber = dayDto.DayNumber,
                            DayName = dayDto.DayName,
                            Title = dayDto.Title ?? $"روز {dayDto.DayNumber}",
                            Focus = dayDto.Focus ?? "",
                            Duration = dayDto.Duration,
                            Notes = dayDto.Notes ?? ""
                        };

                        _context.WorkoutDays.Add(day);
                        await _context.SaveChangesAsync();

                        // ایجاد حرکات
                        if (dayDto.Exercises != null && dayDto.Exercises.Any())
                        {
                            foreach (var exDto in dayDto.Exercises)
                            {
                                ExerciseRef? exerciseRef = null;

                                // اگر exerciseId داره، ببینیم تو ExerciseRef هست یا نه
                                if (!string.IsNullOrEmpty(exDto.ExerciseId))
                                {
                                    exerciseRef = await _context.ExerciseRefs
                                        .FirstOrDefaultAsync(r => r.ExerciseId == exDto.ExerciseId);

                                    if (exerciseRef == null && exDto.GifUrl != null)
                                    {
                                        // ایجاد ExerciseRef جدید
                                        exerciseRef = new ExerciseRef
                                        {
                                            ExerciseId = exDto.ExerciseId,
                                            Name = exDto.Name,
                                            GifUrl = exDto.GifUrl,
                                            TargetMuscles = exDto.TargetMuscles?.ToArray() ?? Array.Empty<string>(),
                                            BodyParts = exDto.BodyParts?.ToArray() ?? Array.Empty<string>(),
                                            Equipments = exDto.Equipments?.ToArray() ?? Array.Empty<string>(),
                                            Instructions = exDto.Instructions?.ToArray() ?? Array.Empty<string>(),
                                            LastSynced = DateTime.UtcNow
                                        };

                                        _context.ExerciseRefs.Add(exerciseRef);
                                        await _context.SaveChangesAsync();

                                        _logger.LogDebug($"Created ExerciseRef for {exDto.ExerciseId}");
                                    }
                                }

                                var exercise = new Exercise
                                {
                                    DayId = day.Id,
                                    ExerciseRefId = exerciseRef?.Id,
                                    ExerciseApiId = exDto.ExerciseId,
                                    Order = exDto.Order,
                                    Name = exDto.Name,
                                    Description = exDto.Description ?? "",
                                    Sets = exDto.Sets,
                                    Reps = exDto.Reps,
                                    RestTime = exDto.RestTime ?? "60-90 ثانیه",
                                    Notes = exDto.Notes ?? "",
                                    GifUrl = exDto.GifUrl
                                };

                                _context.Exercises.Add(exercise);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                _logger.LogInformation($"✅ Workout {program.Id} created successfully for student {student.Id}");
                return program.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workout for coach {CoachId}", coachId);
                return null;
            }
        }

        public async Task<int?> UpdateWorkoutAsync(int workoutId, int coachId, CreateWorkoutDto dto)
        {
            try
            {
                _logger.LogInformation($"Updating workout {workoutId} for coach {coachId}");

                // بررسی وجود برنامه و دسترسی مربی
                var existingProgram = await _context.WorkoutPrograms
                    .Include(w => w.WorkoutWeeks)
                        .ThenInclude(ww => ww.WorkoutDays)
                            .ThenInclude(wd => wd.Exercises)
                    .FirstOrDefaultAsync(w => w.Id == workoutId && w.CoachId == coachId);

                if (existingProgram == null)
                {
                    _logger.LogWarning($"Workout {workoutId} not found or not owned by coach {coachId}");
                    return null;
                }

                // بررسی اینکه studentId متعلق به این مربی هست
                var student = await _context.StudentProfiles
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == dto.StudentId && s.User != null && s.User.CoachId == coachId);

                if (student == null)
                {
                    _logger.LogWarning($"Student {dto.StudentId} not found or not assigned to coach {coachId}");
                    return null;
                }

                // به‌روزرسانی اطلاعات اصلی برنامه
                existingProgram.Title = dto.Title;
                existingProgram.Description = dto.Description ?? "";
                existingProgram.StartDate = dto.StartDate;
                existingProgram.EndDate = dto.EndDate;
                existingProgram.DurationWeeks = dto.DurationWeeks;
                existingProgram.Notes = dto.Notes ?? "";

                // حذف داده‌های قدیمی (هفته‌ها، روزها و حرکات)
                _logger.LogDebug($"Removing old weeks, days, and exercises for workout {workoutId}");

                foreach (var week in existingProgram.WorkoutWeeks)
                {
                    foreach (var day in week.WorkoutDays)
                    {
                        _context.Exercises.RemoveRange(day.Exercises);
                    }
                    _context.WorkoutDays.RemoveRange(week.WorkoutDays);
                }
                _context.WorkoutWeeks.RemoveRange(existingProgram.WorkoutWeeks);

                await _context.SaveChangesAsync();

                // ایجاد هفته‌های جدید
                foreach (var weekDto in dto.Weeks)
                {
                    var week = new WorkoutWeek
                    {
                        ProgramId = existingProgram.Id,
                        WeekNumber = weekDto.WeekNumber,
                        Title = weekDto.Title ?? $"هفته {weekDto.WeekNumber}",
                        Focus = weekDto.Focus ?? "",
                        Notes = weekDto.Notes ?? ""
                    };

                    _context.WorkoutWeeks.Add(week);
                    await _context.SaveChangesAsync();

                    // ایجاد روزها
                    foreach (var dayDto in weekDto.Days)
                    {
                        var day = new WorkoutDay
                        {
                            WeekId = week.Id,
                            DayNumber = dayDto.DayNumber,
                            DayName = dayDto.DayName,
                            Title = dayDto.Title ?? $"روز {dayDto.DayNumber}",
                            Focus = dayDto.Focus ?? "",
                            Duration = dayDto.Duration,
                            Notes = dayDto.Notes ?? ""
                        };

                        _context.WorkoutDays.Add(day);
                        await _context.SaveChangesAsync();

                        // ایجاد حرکات
                        if (dayDto.Exercises != null && dayDto.Exercises.Any())
                        {
                            foreach (var exDto in dayDto.Exercises)
                            {
                                ExerciseRef? exerciseRef = null;

                                if (!string.IsNullOrEmpty(exDto.ExerciseId))
                                {
                                    exerciseRef = await _context.ExerciseRefs
                                        .FirstOrDefaultAsync(r => r.ExerciseId == exDto.ExerciseId);

                                    if (exerciseRef == null && exDto.GifUrl != null)
                                    {
                                        exerciseRef = new ExerciseRef
                                        {
                                            ExerciseId = exDto.ExerciseId,
                                            Name = exDto.Name,
                                            GifUrl = exDto.GifUrl,
                                            TargetMuscles = exDto.TargetMuscles ?? Array.Empty<string>(),
                                            BodyParts = exDto.BodyParts ?? Array.Empty<string>(),
                                            Equipments = exDto.Equipments ?? Array.Empty<string>(),
                                            SecondaryMuscles = Array.Empty<string>(),
                                            Instructions = exDto.Instructions ?? Array.Empty<string>()
                                        };

                                        _context.ExerciseRefs.Add(exerciseRef);
                                        await _context.SaveChangesAsync();
                                    }
                                }

                                var exercise = new Exercise
                                {
                                    DayId = day.Id,
                                    ExerciseRefId = exerciseRef?.Id,
                                    ExerciseApiId = exDto.ExerciseId,
                                    Order = exDto.Order,
                                    Name = exDto.Name,
                                    Description = exDto.Description ?? "",
                                    Sets = exDto.Sets,
                                    Reps = exDto.Reps,
                                    RestTime = exDto.RestTime ?? "60-90 ثانیه",
                                    Notes = exDto.Notes ?? "",
                                    GifUrl = exDto.GifUrl
                                };

                                _context.Exercises.Add(exercise);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                _logger.LogInformation($"✅ Workout {workoutId} updated successfully");
                return existingProgram.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error updating workout {workoutId}");
                return null;
            }
        }

        // متد کمکی برای تبدیل به DTO
        private WorkoutResponseDto MapToDto(WorkoutProgram program)
        {
            return new WorkoutResponseDto
            {
                Id = program.Id,
                Title = program.Title,
                Description = program.Description,
                StartDate = program.StartDate,
                EndDate = program.EndDate,
                DurationWeeks = program.DurationWeeks,
                Status = program.Status,
                Notes = program.Notes,
                CreatedAt = program.CreatedAt,

                // ✅ اصلاح اینجا: از program.Student.Id استفاده کن نه program.Student.User.Id
                Student = program.Student != null ? new StudentInfoDto
                {
                    Id = program.Student.Id,  // این studentProfileId هست
                    FullName = program.Student.User?.FullName ?? "",
                    Email = program.Student.User?.Email ?? ""
                } : null,

                Coach = program.Coach != null ? new CoachInfoDto
                {
                    Id = program.Coach.Id,
                    FullName = program.Coach.FullName
                } : null,

                Weeks = program.WorkoutWeeks?.Select(week => new WorkoutWeekDto
                {
                    Id = week.Id,
                    WeekNumber = week.WeekNumber,
                    Title = week.Title,
                    Focus = week.Focus,
                    Notes = week.Notes,
                    Days = week.WorkoutDays?.Select(day => new WorkoutDayDto
                    {
                        Id = day.Id,
                        DayNumber = day.DayNumber,
                        DayName = day.DayName,
                        Title = day.Title,
                        Focus = day.Focus,
                        Duration = day.Duration,
                        Notes = day.Notes,
                        Exercises = day.Exercises?.Select(ex => new ExerciseDto
                        {
                            Id = ex.Id,
                            Order = ex.Order,
                            Name = ex.Name,
                            Description = ex.Description,
                            Sets = ex.Sets,
                            Reps = ex.Reps,
                            RestTime = ex.RestTime,
                            Notes = ex.Notes,
                            GifUrl = ex.GifUrl ?? ex.ExerciseRef?.GifUrl
                        }).ToList() ?? new List<ExerciseDto>()
                    }).ToList() ?? new List<WorkoutDayDto>()
                }).ToList() ?? new List<WorkoutWeekDto>()
            };
        }
    }
}