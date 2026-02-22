using TrainMatePro.DTOs;
using TrainMatePro.DTOs.ExerciseData;
using ExerciseDto = TrainMatePro.DTOs.ExerciseData.ExerciseDto;

namespace TrainMatePro.Services
{
    public interface IExerciseApiService
    {
        Task<List<ExerciseDto>> GetExercisesAsync(string? bodyPart = null, string? equipment = null, string? muscle = null);
        Task<ExerciseDto?> GetExerciseByIdAsync(string id);
        Task<List<string>> GetBodyPartsAsync();
        Task<List<string>> GetEquipmentsAsync();
        Task<List<string>> GetMusclesAsync();
        Task LoadDataFromJsonAsync();
        string GetGifUrl(string gifFileName);
    }
}