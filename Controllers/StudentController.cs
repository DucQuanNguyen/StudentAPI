using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentAPI.DTO;
using StudentAPI.Model;
using StudentAPI.Services;
using StudentAPI.Controllers; // For Mapper

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(StudentService studentService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSinhViens([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                    return BadRequest(new { message = "Page and pageSize must be greater than zero." });

                var (students, totalCount) = await _studentService.GetPagedAsync(page, pageSize);
                var dtos = students.Select(Mapper.ToDto).ToList();

                var response = new
                {
                    data = dtos,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving students.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving students. Please try again later." });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSinhVien([FromBody] StudentDto sinhVienDto)
        {
            if (sinhVienDto == null)
                return BadRequest(new { message = "Student data is missing." });

            try
            {
                var created = await _studentService.CreateAsync(Mapper.ToEntity(sinhVienDto));
                if (created != null)
                    return CreatedAtAction(nameof(GetSinhVienById), new { studentId = created.StudentId }, new { data = Mapper.ToDto(created) });

                return StatusCode(500, new { message = "Failed to add student. The student may already exist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a student.");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the student. Please try again later." });
            }
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetSinhVienById(string studentId)
        {
            try
            {
                var student = await _studentService.GetByIdAsync(studentId);
                if (student != null)
                    return Ok(new { data = Mapper.ToDto(student) });
                return NotFound(new { message = "No student found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving student by ID.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the student. Please try again later." });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{studentId}")]
        public async Task<IActionResult> UpdateSinhVien(string studentId, [FromBody] StudentDto updatedSinhVienDto)
        {
            if (updatedSinhVienDto == null)
                return BadRequest(new { message = "Student data is missing." });

            try
            {
                var updated = await _studentService.UpdateAsync(studentId, Mapper.ToEntity(updatedSinhVienDto));
                if (updated != null)
                    return Ok(new { data = Mapper.ToDto(updated) });
                return NotFound(new { message = "No student found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the student.");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the student. Please try again later." });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DeleteSinhVien(string studentId)
        {
            try
            {
                var deleted = await _studentService.DeleteAsync(studentId);
                if (deleted != null)
                    return Ok(new { data = Mapper.ToDto(deleted) });
                return NotFound(new { message = "No student found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student.");
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the student. Please try again later." });
            }
        }
    }
}