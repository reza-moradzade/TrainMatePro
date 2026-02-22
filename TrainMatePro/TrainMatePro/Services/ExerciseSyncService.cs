using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrainMatePro.Data;
using TrainMatePro.DTOs.ExerciseData;
using TrainMatePro.Models;
using TrainMatePro.Models.ExerciseData;

namespace TrainMatePro.Services
{
    public interface IExerciseSyncService
    {
        Task<int> SyncExercisesFromApiAsync();
        Task<ExerciseRef?> GetOrCreateExerciseRefAsync(ExerciseDto exerciseDto);
    }

    public class ExerciseSyncService : IExerciseSyncService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExerciseSyncService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExerciseSyncService(AppDbContext context, ILogger<ExerciseSyncService> logger, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<int> SyncExercisesFromApiAsync()
        {
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Exercises", "exercises.json");
                var json = await File.ReadAllTextAsync(jsonPath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var exercises = JsonSerializer.Deserialize<List<ExerciseItem>>(json, options);

                if (exercises == null) return 0;

                int newCount = 0;
                foreach (var ex in exercises)
                {
                    var existing = await _context.ExerciseRefs
                        .FirstOrDefaultAsync(e => e.ExerciseId == ex.ExerciseId);

                    if (existing == null)
                    {
                        var exerciseRef = new ExerciseRef
                        {
                            ExerciseId = ex.ExerciseId,
                            Name = ex.Name,
                            GifUrl = ex.GifUrl,
                            TargetMuscles = ex.TargetMuscles ?? Array.Empty<string>(),
                            BodyParts = ex.BodyParts ?? Array.Empty<string>(),
                            Equipments = ex.Equipments ?? Array.Empty<string>(),
                            SecondaryMuscles = ex.SecondaryMuscles ?? Array.Empty<string>(),
                            Instructions = ex.Instructions ?? Array.Empty<string>(),
                            LastSynced = DateTime.UtcNow
                        };

                        _context.ExerciseRefs.Add(exerciseRef);
                        newCount++;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ Synced {newCount} new exercises");
                return newCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error syncing exercises");
                return 0;
            }
        }

        public async Task<ExerciseRef?> GetOrCreateExerciseRefAsync(ExerciseDto exerciseDto)
        {
            // اینجا هم با ExerciseId مقایسه کن
            var existing = await _context.ExerciseRefs
                .FirstOrDefaultAsync(e => e.ExerciseId == exerciseDto.ExerciseId);

            if (existing != null)
                return existing;

            var exerciseRef = new ExerciseRef
            {
                ExerciseId = exerciseDto.ExerciseId,
                Name = exerciseDto.Name,
                GifUrl = exerciseDto.GifUrl,
                TargetMuscles = exerciseDto.TargetMuscles ?? Array.Empty<string>(),
                BodyParts = exerciseDto.BodyParts ?? Array.Empty<string>(),
                Equipments = exerciseDto.Equipments ?? Array.Empty<string>(),
                SecondaryMuscles = exerciseDto.SecondaryMuscles ?? Array.Empty<string>(),
                Instructions = exerciseDto.Instructions ?? Array.Empty<string>(),
                LastSynced = DateTime.UtcNow
            };

            _context.ExerciseRefs.Add(exerciseRef);
            await _context.SaveChangesAsync();

            return exerciseRef;
        }
    }
}