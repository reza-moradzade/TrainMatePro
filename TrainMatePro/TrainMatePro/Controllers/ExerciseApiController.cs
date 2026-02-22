using Microsoft.AspNetCore.Mvc;
using TrainMatePro.DTOs.ExerciseData;
using TrainMatePro.Services;

namespace TrainMatePro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class exercisesController : ControllerBase  // اسم کنترلر باید با اسم Route یکی باشه
    {
        private readonly IExerciseApiService _exerciseService;
        private readonly ILogger<exercisesController> _logger;

        public exercisesController(IExerciseApiService exerciseService, ILogger<exercisesController> logger)
        {
            _exerciseService = exerciseService;
            _logger = logger;
        }

        [HttpGet("body-parts")]
        public async Task<IActionResult> GetBodyParts()
        {
            try
            {
                _logger.LogInformation("GetBodyParts called");
                var bodyParts = await _exerciseService.GetBodyPartsAsync();
                _logger.LogInformation($"Returning {bodyParts.Count} body parts");

                return Ok(new
                {
                    success = true,
                    bodyParts = bodyParts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting body parts");
                return StatusCode(500, new { success = false, message = "خطا در دریافت اعضای بدن" });
            }
        }

        [HttpGet("equipments")]
        public async Task<IActionResult> GetEquipments()
        {
            try
            {
                _logger.LogInformation("GetEquipments called");
                var equipments = await _exerciseService.GetEquipmentsAsync();
                _logger.LogInformation($"Returning {equipments.Count} equipments");

                return Ok(new
                {
                    success = true,
                    equipments = equipments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting equipments");
                return StatusCode(500, new { success = false, message = "خطا در دریافت تجهیزات" });
            }
        }

        [HttpGet("muscles")]
        public async Task<IActionResult> GetMuscles()
        {
            try
            {
                _logger.LogInformation("GetMuscles called");
                var muscles = await _exerciseService.GetMusclesAsync();
                _logger.LogInformation($"Returning {muscles.Count} muscles");

                return Ok(new
                {
                    success = true,
                    muscles = muscles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting muscles");
                return StatusCode(500, new { success = false, message = "خطا در دریافت عضلات" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExercises(
            [FromQuery] string? bodyPart = null,
            [FromQuery] string? equipment = null,
            [FromQuery] string? muscle = null)
        {
            try
            {
                _logger.LogInformation($"GetExercises called with filters: bodyPart={bodyPart}, equipment={equipment}, muscle={muscle}");

                var exercises = await _exerciseService.GetExercisesAsync(bodyPart, equipment, muscle);

                _logger.LogInformation($"Returning {exercises.Count} exercises");

                return Ok(new
                {
                    success = true,
                    count = exercises.Count,
                    exercises = exercises
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exercises");
                return StatusCode(500, new { success = false, message = "خطا در دریافت حرکات" });
            }
        }

        [HttpGet("{exerciseId}")]
        public async Task<IActionResult> GetExerciseById(string exerciseId)
        {
            try
            {
                _logger.LogInformation($"GetExerciseById called with id: {exerciseId}");

                var exercise = await _exerciseService.GetExerciseByIdAsync(exerciseId);

                if (exercise == null)
                {
                    return NotFound(new { success = false, message = "حرکت مورد نظر یافت نشد" });
                }

                return Ok(new { success = true, exercise });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exercise by id: {ExerciseId}", exerciseId);
                return StatusCode(500, new { success = false, message = "خطا در دریافت حرکت" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchExercises([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(new { success = false, message = "عبارت جستجو نمی‌تواند خالی باشد" });
                }

                _logger.LogInformation($"SearchExercises called with query: {q}");

                var exercises = await _exerciseService.GetExercisesAsync();
                var filtered = exercises.Where(e =>
                    e.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    e.TargetMuscles.Any(m => m.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    e.BodyParts.Any(b => b.Contains(q, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                _logger.LogInformation($"Search returned {filtered.Count} results");

                return Ok(new
                {
                    success = true,
                    count = filtered.Count,
                    exercises = filtered
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching exercises");
                return StatusCode(500, new { success = false, message = "خطا در جستجوی حرکات" });
            }
        }
    }
}