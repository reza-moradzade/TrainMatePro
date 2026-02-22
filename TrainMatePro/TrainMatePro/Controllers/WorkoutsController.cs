using Microsoft.AspNetCore.Mvc;
using TrainMatePro.DTOs;
using TrainMatePro.Services;

namespace TrainMatePro.Controllers
{
    [Route("api/workouts")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;
        private readonly IStudentService _studentService;

        public WorkoutsController(IWorkoutService workoutService, IStudentService studentService)
        {
            _workoutService = workoutService;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkouts([FromQuery] int? studentId)
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            List<WorkoutResponseDto> programs;

            if (user.Role == "coach")
            {
                programs = await _workoutService.GetCoachWorkoutsAsync(user.Id, studentId);
            }
            else if (user.Role == "student")
            {
                programs = await _workoutService.GetStudentWorkoutsAsync(user.Id);
            }
            else
            {
                return Forbid();
            }

            return Ok(new { success = true, programs });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkoutById(int id)
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            var program = await _workoutService.GetWorkoutByIdAsync(id, user.Id, user.Role);

            if (program == null)
                return NotFound(new { message = "برنامه مورد نظر یافت نشد" });

            return Ok(new { success = true, program });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            if (user.Role != "coach")
                return Forbid();

            var workout = await _workoutService.GetWorkoutByIdAsync(id, user.Id, user.Role);

            if (workout == null)
                return NotFound(new { message = "برنامه مورد نظر یافت نشد" });

            var result = await _workoutService.DeleteWorkoutAsync(id);

            if (result)
            {
                return Ok(new { success = true, message = "برنامه با موفقیت حذف شد" });
            }

            return BadRequest(new { message = "خطا در حذف برنامه" });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkout(int id, [FromBody] CreateWorkoutDto dto)
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            if (user.Role != "coach")
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _workoutService.UpdateWorkoutAsync(id, user.Id, dto);

            if (result == null)
                return NotFound(new { message = "برنامه مورد نظر یافت نشد یا به شما تعلق ندارد" });

            return Ok(new
            {
                success = true,
                message = "برنامه تمرینی با موفقیت ویرایش شد",
                programId = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutDto dto)
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            if (user.Role != "coach")
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var programId = await _workoutService.CreateWorkoutAsync(user.Id, dto);

            if (programId == null)
                return BadRequest(new { message = "شاگرد مورد نظر یافت نشد یا به شما تعلق ندارد" });

            return Ok(new
            {
                success = true,
                message = "برنامه تمرینی با موفقیت ایجاد شد",
                programId
            });
        }
    }
}