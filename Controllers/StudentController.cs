using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;
using StudentAPI.Repository;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentController : ControllerBase
    {
        private readonly DataContext _context;

        public StudentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SinhVien>>> GetSinhViens()
        {
            return await _context.students.Include(s => s.LopHoc).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SinhVien>> CreateSinhVien(SinhVien sinhVien)
        {
            _context.students.Add(sinhVien);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSinhViens), new { id = sinhVien.Id }, sinhVien);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SinhVien>> GetSinhVien(int id)
        {
            var SinhVien = await _context.students.FindAsync(id);
            if (SinhVien == null) return NotFound();
            return SinhVien;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSinhVien(int id, SinhVien updatedSinhVien)
        {
            if (id != updatedSinhVien.Id) return BadRequest();
            _context.Entry(updatedSinhVien).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinhVien(int id)
        {
            var SinhVien = await _context.students.FindAsync(id);
            if (SinhVien == null) return NotFound();
            _context.students.Remove(SinhVien);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
