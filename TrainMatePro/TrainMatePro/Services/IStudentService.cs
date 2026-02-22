using TrainMatePro.DTOs;

namespace TrainMatePro.Services
{
    public interface IStudentService
    {
        Task<List<StudentResponseDto>> GetCoachStudentsAsync(int coachId);
        Task<StudentResponseDto?> CreateStudentAsync(int coachId, CreateStudentDto dto);
        Task<bool> IsStudentBelongsToCoachAsync(int studentUserId, int coachId);
    }
}