using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TrainMatePro.DTOs.ExerciseData;
using TrainMatePro.Models.ExerciseData;

namespace TrainMatePro.Services
{
    public class ExerciseApiService : IExerciseApiService
    {
        private List<ExerciseItem> _exercises = new();
        private List<string> _bodyParts = new();
        private List<string> _equipments = new();
        private List<string> _muscles = new();

        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExerciseApiService> _logger;
        private readonly string _mediaBaseUrl;

        public ExerciseApiService(IWebHostEnvironment env, ILogger<ExerciseApiService> logger, IConfiguration configuration)
        {
            _env = env;
            _logger = logger;
            _mediaBaseUrl = configuration["ExerciseApi:MediaBaseUrl"] ?? "/media/exercises";

            // بارگذاری synchronous در سازنده
            LoadDataSynchronously();
        }
        private void LoadDataSynchronously()
        {
            try
            {
                var dataPath = Path.Combine(_env.ContentRootPath, "Data", "Exercises");
                _logger.LogInformation($"🔍 Looking for data in: {dataPath}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // بارگذاری exercises
                var exercisesPath = Path.Combine(dataPath, "exercises.json");
                if (File.Exists(exercisesPath))
                {
                    var json = File.ReadAllText(exercisesPath);  // synchronous
                    _logger.LogInformation($"📄 exercises.json size: {json.Length} characters");

                    _exercises = JsonSerializer.Deserialize<List<ExerciseItem>>(json, options) ?? new();
                    _logger.LogInformation($"✅ Loaded {_exercises.Count} exercises");
                }

                // بارگذاری bodyparts
                var bodyPartsPath = Path.Combine(dataPath, "bodyparts.json");
                if (File.Exists(bodyPartsPath))
                {
                    var json = File.ReadAllText(bodyPartsPath);
                    _logger.LogInformation($"📄 bodyparts.json content length: {json.Length}");

                    var parts = JsonSerializer.Deserialize<List<BodyPart>>(json, options);
                    if (parts != null)
                    {
                        _bodyParts = parts.Select(p => p.Name).ToList() ?? new();
                        _logger.LogInformation($"✅ Loaded {_bodyParts.Count} body parts");
                    }
                }

                // بارگذاری equipments
                var equipmentsPath = Path.Combine(dataPath, "equipments.json");
                if (File.Exists(equipmentsPath))
                {
                    var json = File.ReadAllText(equipmentsPath);
                    _logger.LogInformation($"📄 equipments.json content length: {json.Length}");

                    var eqs = JsonSerializer.Deserialize<List<Equipment>>(json, options);
                    if (eqs != null)
                    {
                        _equipments = eqs.Select(e => e.Name).ToList() ?? new();
                        _logger.LogInformation($"✅ Loaded {_equipments.Count} equipments");
                    }
                }

                // بارگذاری muscles
                var musclesPath = Path.Combine(dataPath, "muscles.json");
                if (File.Exists(musclesPath))
                {
                    var json = File.ReadAllText(musclesPath);
                    _logger.LogInformation($"📄 muscles.json content length: {json.Length}");

                    var ms = JsonSerializer.Deserialize<List<Muscle>>(json, options);
                    if (ms != null)
                    {
                        _muscles = ms.Select(m => m.Name).ToList() ?? new();
                        _logger.LogInformation($"✅ Loaded {_muscles.Count} muscles");
                    }
                }

                _logger.LogInformation($"📊 Summary: {_exercises.Count} exercises, {_bodyParts.Count} body parts, {_equipments.Count} equipments, {_muscles.Count} muscles");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading exercise data");
            }
        }
        public async Task LoadDataFromJsonAsync()
        {
            try
            {
                var dataPath = Path.Combine(_env.ContentRootPath, "Data", "Exercises");
                _logger.LogInformation($"🔍 Looking for data in: {dataPath}");

                // چک کردن وجود پوشه
                if (!Directory.Exists(dataPath))
                {
                    _logger.LogError($"❌ Directory not found: {dataPath}");
                    return;
                }

                // تنظیمات JSON Deserializer
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // بارگذاری exercises
                var exercisesPath = Path.Combine(dataPath, "exercises.json");
                if (File.Exists(exercisesPath))
                {
                    var json = await File.ReadAllTextAsync(exercisesPath);
                    _logger.LogInformation($"📄 exercises.json size: {json.Length} characters");

                    _exercises = JsonSerializer.Deserialize<List<ExerciseItem>>(json, options) ?? new();
                    _logger.LogInformation($"✅ Loaded {_exercises.Count} exercises");
                }
                else
                {
                    _logger.LogError($"❌ exercises.json not found at: {exercisesPath}");
                }

                // بارگذاری bodyparts
                var bodyPartsPath = Path.Combine(dataPath, "bodyparts.json");
                if (File.Exists(bodyPartsPath))
                {
                    var json = await File.ReadAllTextAsync(bodyPartsPath);
                    _logger.LogInformation($"📄 bodyparts.json content length: {json.Length}");

                    var parts = JsonSerializer.Deserialize<List<BodyPart>>(json, options);
                    if (parts != null)
                    {
                        _bodyParts = parts.Select(p => p.Name).ToList() ?? new();
                        _logger.LogInformation($"✅ Loaded {_bodyParts.Count} body parts");
                    }
                    else
                    {
                        _logger.LogError("❌ Failed to deserialize bodyparts.json to List<BodyPart>");
                    }
                }
                else
                {
                    _logger.LogError($"❌ bodyparts.json not found at: {bodyPartsPath}");
                }

                // بارگذاری equipments
                var equipmentsPath = Path.Combine(dataPath, "equipments.json");
                if (File.Exists(equipmentsPath))
                {
                    var json = await File.ReadAllTextAsync(equipmentsPath);
                    _logger.LogInformation($"📄 equipments.json content length: {json.Length}");

                    var eqs = JsonSerializer.Deserialize<List<Equipment>>(json, options);
                    if (eqs != null)
                    {
                        _equipments = eqs.Select(e => e.Name).ToList() ?? new();
                        _logger.LogInformation($"✅ Loaded {_equipments.Count} equipments");
                    }
                }

                // بارگذاری muscles
                var musclesPath = Path.Combine(dataPath, "muscles.json");
                if (File.Exists(musclesPath))
                {
                    var json = await File.ReadAllTextAsync(musclesPath);
                    _logger.LogInformation($"📄 muscles.json content length: {json.Length}");

                    var ms = JsonSerializer.Deserialize<List<Muscle>>(json, options);
                    if (ms != null)
                    {
                        _muscles = ms.Select(m => m.Name).ToList() ?? new();
                        _logger.LogInformation($"✅ Loaded {_muscles.Count} muscles");
                    }
                }

                _logger.LogInformation($"📊 Summary: {_exercises.Count} exercises, {_bodyParts.Count} body parts, {_equipments.Count} equipments, {_muscles.Count} muscles");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading exercise data");
            }
        }

        public Task<List<ExerciseDto>> GetExercisesAsync(string? bodyPart = null, string? equipment = null, string? muscle = null)
        {
            var query = _exercises.AsEnumerable();

            if (!string.IsNullOrEmpty(bodyPart))
            {
                query = query.Where(e => e.BodyParts.Any(b => b.Contains(bodyPart, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(equipment))
            {
                query = query.Where(e => e.Equipments.Any(eq => eq.Contains(equipment, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(muscle))
            {
                query = query.Where(e =>
                    e.TargetMuscles.Any(m => m.Contains(muscle, StringComparison.OrdinalIgnoreCase)) ||
                    e.SecondaryMuscles.Any(m => m.Contains(muscle, StringComparison.OrdinalIgnoreCase))
                );
            }

            var result = query.Select(e => MapToDto(e)).ToList();
            return Task.FromResult(result);
        }

        public async Task<ExerciseDto?> GetExerciseByIdAsync(string id)
        {
            // اینجا باید با exerciseId مقایسه کنه نه Id
            var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == id);
            return exercise != null ? MapToDto(exercise) : null;
        }

        public Task<List<string>> GetBodyPartsAsync()
        {
            return Task.FromResult(_bodyParts);  // حالا دیگه داده هست
        }

        public Task<List<string>> GetEquipmentsAsync()
        {
            return Task.FromResult(_equipments);
        }

        public Task<List<string>> GetMusclesAsync()
        {
            return Task.FromResult(_muscles);
        }

        public string GetGifUrl(string gifFileName)
        {
            return $"{_mediaBaseUrl}/{gifFileName}";
        }

        private ExerciseDto MapToDto(ExerciseItem exercise)
        {
            // استخراج نام فایل GIF از URL
            string gifFileName = exercise.GifUrl;
            if (gifFileName.Contains("/"))
            {
                gifFileName = gifFileName.Substring(gifFileName.LastIndexOf('/') + 1);
            }

            return new ExerciseDto
            {
                ExerciseId = exercise.ExerciseId,
                Name = exercise.Name,
                GifUrl = GetGifUrl(gifFileName),
                TargetMuscles = exercise.TargetMuscles?.Length > 0 ? exercise.TargetMuscles : Array.Empty<string>(),
                BodyParts = exercise.BodyParts?.Length > 0 ? exercise.BodyParts : Array.Empty<string>(),
                SecondaryMuscles = exercise.SecondaryMuscles?.Length > 0 ? exercise.SecondaryMuscles : Array.Empty<string>(),
                Instructions = exercise.Instructions?.Length > 0 ? exercise.Instructions : Array.Empty<string>(),
                Equipments = exercise.Equipments?.Length > 0 ? exercise.Equipments : Array.Empty<string>()
            };
        }
    }
}