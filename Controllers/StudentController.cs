using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentController : ControllerBase
    {
        private readonly StudentDemoContext _context;

        public StudentController(StudentDemoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SinhVien>>> GetSinhViens()
        {
            return await _context.SinhViens.Include(s => s.Class).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SinhVien>> CreateSinhVien(SinhVien sinhVien)
        {
            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSinhViens), new { id = sinhVien.StudentId }, sinhVien);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SinhVien>> GetSinhVien(string id)
        {
            var SinhVien = await _context.SinhViens.FindAsync(id);
            if (SinhVien == null) return NotFound();
            return SinhVien;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSinhVien(string id, SinhVien updatedSinhVien)
        {
            if (!id.Equals(updatedSinhVien.StudentId)) return BadRequest();
            _context.Entry(updatedSinhVien).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinhVien(string id)
        {
            var SinhVien = await _context.SinhViens.FindAsync(id);
            if (SinhVien == null) return NotFound();
            _context.SinhViens.Remove(SinhVien);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
