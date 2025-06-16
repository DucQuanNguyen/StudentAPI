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
        // GET: api/sinhviens
        [HttpGet]
        public async Task<IActionResult> GetSinhViens([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Validate page and pageSize parameters
            try
            {
                // Ensure page and pageSize are greater than zero
                if (page <= 0 || pageSize <= 0)
                    return BadRequest(new { message = "Page and pageSize must be greater than zero." });
                // Ensure pageSize does not exceed a reasonable limit
                var (students, totalCount) = await _studentService.GetPagedAsync(page, pageSize);
                var dtos = students.Select(Mapper.ToDto).ToList();
                // If no students found, return an empty list with pagination info
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
                // Return the list of students with pagination info
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving students.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving students. Please try again later." });
            }
        }
        // POST: api/sinhviens
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSinhVien([FromBody] StudentDto sinhVienDto)
        {
            // Validate the input data
            if (sinhVienDto == null)
                return BadRequest(new { message = "Student data is missing." });
            // Ensure required fields are present
            try
            {
                // Check if the student already exists
                var created = await _studentService.CreateAsync(Mapper.ToEntity(sinhVienDto));
                if (created != null)
                    return CreatedAtAction(nameof(GetSinhVienById), new { studentId = created.StudentId }, new { data = Mapper.ToDto(created) });
                // If the student already exists, return a conflict status
                return StatusCode(500, new { message = "Failed to add student. The student may already exist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a student.");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the student. Please try again later." });
            }
        }
        // GET: api/sinhviens/{studentId}
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetSinhVienById(string studentId)
        {
            try
            {
                // Validate the studentId parameter
                var student = await _studentService.GetByIdAsync(studentId);
                if (student != null)
                    return Ok(new { data = Mapper.ToDto(student) });
                // If no student found, return a not found status
                return NotFound(new { message = "No student found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving student by ID.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the student. Please try again later." });
            }
        }
        // PUT: api/sinhviens/{studentId}
        [Authorize(Roles = "admin")]
        [HttpPut("{studentId}")]
        public async Task<IActionResult> UpdateSinhVien(string studentId, [FromBody] StudentDto updatedSinhVienDto)
        {
            // Validate the input data
            if (updatedSinhVienDto == null)
                return BadRequest(new { message = "Student data is missing." });
            // Ensure required fields are present
            try
            {
                // Check if the student exists before updating
                var updated = await _studentService.UpdateAsync(studentId, Mapper.ToEntity(updatedSinhVienDto));
                if (updated != null)
                    return Ok(new { data = Mapper.ToDto(updated) });
                // If no student found, return a not found status
                return NotFound(new { message = "No student found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the student.");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the student. Please try again later." });
            }
        }
        // DELETE: api/sinhviens/{studentId}
        [Authorize(Roles = "admin")]
        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DeleteSinhVien(string studentId)
        {
            try
            {
                // Validate the studentId parameter
                var deleted = await _studentService.DeleteAsync(studentId);
                if (deleted != null)
                    return Ok(new { data = Mapper.ToDto(deleted) });
                // If no student found, return a not found status
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