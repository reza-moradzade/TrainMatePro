using Microsoft.AspNetCore.Mvc;
using TrainMatePro.DTOs;
using TrainMatePro.Services;

namespace TrainMatePro.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IAuthService _authService;

        public StudentsController(IStudentService studentService, IAuthService authService)
        {
            _studentService = studentService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            if (user.Role != "coach")
                return Forbid();

            var students = await _studentService.GetCoachStudentsAsync(user.Id);

            return Ok(new { success = true, students });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            if (user.Role != "coach")
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _studentService.CreateStudentAsync(user.Id, dto);

            if (student == null)
                return BadRequest(new { message = "این ایمیل قبلاً ثبت شده است" });

            return Ok(new
            {
                success = true,
                message = "شاگرد با موفقیت ایجاد شد",
                student
            });
        }
    }
}