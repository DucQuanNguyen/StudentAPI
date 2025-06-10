using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ClassController : ControllerBase
    {
        private readonly StudentDemoContext _context;

        public ClassController(StudentDemoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LopHoc>>> GetLopHocs()
        {
            return await _context.LopHocs.Include(l => l.SinhViens).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<LopHoc>> CreateLopHoc(LopHoc lopHoc)
        {
            _context.LopHocs.Add(lopHoc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLopHocs), new { id = lopHoc.Id }, lopHoc);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<LopHoc>> GetLopHoc(int id)
        {
            var LopHoc = await _context.LopHocs.FindAsync(id);
            if (LopHoc == null) return NotFound();
            return LopHoc;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLopHoc(int id, LopHoc updatedLopHoc)
        {
            if (id != updatedLopHoc.Id) return BadRequest();
            _context.Entry(updatedLopHoc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLopHoc(int id)
        {
            var LopHoc = await _context.LopHocs.FindAsync(id);
            if (LopHoc == null) return NotFound();
            _context.LopHocs.Remove(LopHoc);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
