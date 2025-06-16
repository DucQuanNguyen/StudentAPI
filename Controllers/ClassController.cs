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
    public class ClassController : ControllerBase
    {
        private readonly ClassService _classService;
        private readonly ILogger<ClassController> _logger;

        public ClassController(ClassService classService, ILogger<ClassController> logger)
        {
            _classService = classService;
            _logger = logger;
        }
        //GET: api/lophocs
        [HttpGet]
        public async Task<IActionResult> GetLopHoc([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate page and pageSize parameters
                if (page <= 0 || pageSize <= 0)
                    return BadRequest(new { message = "Page and pageSize must be greater than zero." });
                // Ensure pageSize does not exceed a reasonable limit
                var (classes, totalCount) = await _classService.GetPagedAsync(page, pageSize);
                var dtos = classes.Select(Mapper.ToDto).ToList();
                // Check if any classes were found
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
                // Return the paginated response
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving classes.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving classes. Please try again later." });
            }
        }
        // POST: api/lophocs
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLopHoc([FromBody] ClassDto classDto)
        {
            // Validate the incoming classDto object
            if (classDto == null)
                return BadRequest(new { message = "Class data is missing." });
            // Validate the class name
            try
            {
                // Check if the class already exists
                var created = await _classService.CreateAsync(Mapper.ToEntity(classDto));
                if (created != null)
                    return CreatedAtAction(nameof(GetLopHocById), new { id = created.Id }, new { data = Mapper.ToDto(created) });
                // If the class already exists, return a conflict status
                return StatusCode(500, new { message = "Failed to add class. The class may already exist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a class.");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the class. Please try again later." });
            }
        }
        // Get: api/lophocs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLopHocById(int id)
        {
            // Validate the ID parameter
            try
            {
                // Check if the ID is valid
                var lopHoc = await _classService.GetByIdAsync(id);
                if (lopHoc != null)
                    return Ok(new { data = Mapper.ToDto(lopHoc) });
                // If no class found, return a not found status
                return NotFound(new { message = "No class found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving class by ID.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the class. Please try again later." });
            }
        }
        // PUT: api/lophocs/{id}
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLopHoc(int id, [FromBody] ClassDto updatedClassDto)
        {
            // Validate the incoming updatedClassDto object
            if (updatedClassDto == null)
                return BadRequest(new { message = "Class data is missing." });
            // Validate the ID parameter
            try
            {
                // Validate the ID parameter
                var updated = await _classService.UpdateAsync(id, Mapper.ToEntity(updatedClassDto));
                if (updated != null)
                    return Ok(new { data = Mapper.ToDto(updated) });
                // If no class found, return a not found status
                return NotFound(new { message = "No class found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the class.");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the class. Please try again later." });
            }
        }
        // DELETE: api/lophocs/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLopHoc(int id)
        {
            // Validate the ID parameter
            try
            {
                // Validate the ID parameter
                var deleted = await _classService.DeleteAsync(id);
                if (deleted != null)
                    return Ok(new { data = Mapper.ToDto(deleted) });
                // If no class found, return a not found status
                return NotFound(new { message = "No class found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the class.");
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the class. Please try again later." });
            }
        }
    }
}