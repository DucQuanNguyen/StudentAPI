using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using StudentAPI.Model;
using System.Data;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<StudentController> _logger;
        public StudentController(IConfiguration configuration, ILogger<StudentController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetSinhViens()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlDataAdapter adapter = new SqlDataAdapter("GETSinhVien", con);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<SinhVien> liS = new List<SinhVien>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        SinhVien s = new SinhVien();
                        s.StudentId = row[0].ToString();
                        s.StudentName = row[1].ToString();
                        s.BirthDate = Convert.ToDateTime(row[2].ToString());
                        s.Gender = row[3].ToString();
                        s.ClassId = Convert.ToInt32(row[4].ToString());
                        liS.Add(s);
                    }
                }
                if (liS.Count > 0)
                {
                    return Ok(liS);
                }
                else
                {
                    return NotFound("No students found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving students.");
                return StatusCode(500, "An unexpected error occurred while retrieving students. Please try again later.");
            }
        }

        [HttpPost]
        public IActionResult CreateSinhVien(SinhVien sinhVien)
        {
            try
            {
                if (sinhVien == null)
                {
                    return BadRequest("Student data is missing.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("AddSinhVien", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentID", sinhVien.StudentId);
                cmd.Parameters.AddWithValue("@StudentName", sinhVien.StudentName);
                cmd.Parameters.AddWithValue("@BirthDate", sinhVien.BirthDate);
                cmd.Parameters.AddWithValue("@Gender", sinhVien.Gender);
                cmd.Parameters.AddWithValue("@ClassID", sinhVien.ClassId);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Student added successfully!") : StatusCode(500, "Failed to add student. The student may already exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a student.");
                return StatusCode(500, "An unexpected error occurred while adding the student. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult  GetSinhVienById(int id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlDataAdapter adapter = new SqlDataAdapter("GETSinhVienById", con);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                SinhVien s = new SinhVien();
                if (dataTable.Rows.Count > 0)
                {
                    s.StudentId = dataTable.Rows[0]["StudentId"].ToString();
                    s.StudentName = dataTable.Rows[0]["StudentName"].ToString();
                    s.BirthDate = Convert.ToDateTime(dataTable.Rows[0]["BirthDate"].ToString());
                    s.Gender = dataTable.Rows[0]["Gender"].ToString();
                    s.ClassId = Convert.ToInt32(dataTable.Rows[0]["ClassId"].ToString());
                    return Ok(s);
                }
                else
                {
                    return NotFound("No student found with the provided ID.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving student by ID.");
                return StatusCode(500, "An unexpected error occurred while retrieving the student. Please try again later.");
            }

        }

        [HttpPut("{id}")]
        public IActionResult UpdateSinhVien(string id, SinhVien updatedSinhVien)
        {
            try
            {
                if (updatedSinhVien == null)
                {
                    return BadRequest("Student data is missing.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("UpdateSinhVien", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentID", id);
                cmd.Parameters.AddWithValue("@StudentName", updatedSinhVien.StudentName);
                cmd.Parameters.AddWithValue("@BirthDate", updatedSinhVien.BirthDate);
                cmd.Parameters.AddWithValue("@Gender", updatedSinhVien.Gender);
                cmd.Parameters.AddWithValue("@ClassID", updatedSinhVien.ClassId);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Student updated successfully!") : NotFound("No student found with the provided ID.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the student.");
                return StatusCode(500, "An unexpected error occurred while updating the student. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSinhVien(string id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DeleteSinhVien", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentID", id);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Student deleted successfully!") : NotFound("No student found with the provided ID.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student.");
                return StatusCode(500, "An unexpected error occurred while deleting the student. Please try again later.");
            }
        }
    }
}
