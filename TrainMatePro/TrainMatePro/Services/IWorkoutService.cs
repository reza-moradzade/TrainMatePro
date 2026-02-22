using TrainMatePro.DTOs;

namespace TrainMatePro.Services
{
    public interface IWorkoutService
    {
        Task<List<WorkoutResponseDto>> GetCoachWorkoutsAsync(int coachId, int? studentId = null);
        Task<List<WorkoutResponseDto>> GetStudentWorkoutsAsync(int userId);
        Task<WorkoutResponseDto?> GetWorkoutByIdAsync(int workoutId, int userId, string role);
        Task<int?> CreateWorkoutAsync(int coachId, CreateWorkoutDto dto);
        Task<int?> UpdateWorkoutAsync(int workoutId, int coachId, CreateWorkoutDto dto); // متد جدید
        Task<bool> DeleteWorkoutAsync(int workoutId); // متد جدید
    }
}