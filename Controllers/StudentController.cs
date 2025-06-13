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
        // Stored Procedure Names
        private const string SP_GET_ALL = "GETSinhVien";
        private const string SP_GET_BY_ID = "GETSinhVienById";
        private const string SP_ADD = "AddSinhVien";
        private const string SP_UPDATE = "UpdateSinhVien";
        private const string SP_DELETE = "DeleteSinhVien";

        // Parameter Names
        private const string PARAM_STUDENT_ID = "@StudentID";
        private const string PARAM_STUDENT_NAME = "@StudentName";
        private const string PARAM_BIRTH_DATE = "@BirthDate";
        private const string PARAM_GENDER = "@Gender";
        private const string PARAM_CLASS_ID = "@ClassID";

        private readonly string _connectionString;
        private readonly ILogger<StudentController> _logger;
        public StudentController(IConfiguration configuration, ILogger<StudentController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSinhViens()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_GET_ALL, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                await con.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<SinhVien> liS = new List<SinhVien>();
                while (await reader.ReadAsync())
                {
                    SinhVien s = new SinhVien
                    {
                        StudentId = reader["StudentId"].ToString(),
                        StudentName = reader["StudentName"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                        Gender = reader["Gender"].ToString(),
                        ClassId = Convert.ToInt32(reader["ClassId"])
                    };
                    liS.Add(s);
                }
                return liS.Count > 0 ? Ok(liS) : NotFound("No students found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving students.");
                return StatusCode(500, "An unexpected error occurred while retrieving students. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSinhVien([FromBody] SinhVien sinhVien)
        {
            if (sinhVien == null)
                return BadRequest("Student data is missing.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_ADD, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, sinhVien.StudentId);
                cmd.Parameters.AddWithValue(PARAM_STUDENT_NAME, sinhVien.StudentName);
                cmd.Parameters.AddWithValue(PARAM_BIRTH_DATE, sinhVien.BirthDate);
                cmd.Parameters.AddWithValue(PARAM_GENDER, sinhVien.Gender);
                cmd.Parameters.AddWithValue(PARAM_CLASS_ID, sinhVien.ClassId);

                await con.OpenAsync();
                int i = await cmd.ExecuteNonQueryAsync();
                if (i > 0)
                {
                    // Fetch the created entity
                    using SqlCommand getCmd = new SqlCommand(SP_GET_BY_ID, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    getCmd.Parameters.AddWithValue(PARAM_STUDENT_ID, sinhVien.StudentId);
                    using SqlDataReader reader = await getCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        SinhVien created = new SinhVien
                        {
                            StudentId = reader["StudentId"].ToString(),
                            StudentName = reader["StudentName"].ToString(),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            Gender = reader["Gender"].ToString(),
                            ClassId = Convert.ToInt32(reader["ClassId"])
                        };
                        return CreatedAtAction(nameof(GetSinhVienById), new { studentId = created.StudentId }, created);
                    }
                }
                return StatusCode(500, "Failed to add student. The student may already exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a student.");
                return StatusCode(500, "An unexpected error occurred while adding the student. Please try again later.");
            }
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetSinhVienById(string studentId)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_GET_BY_ID, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
                await con.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    SinhVien s = new SinhVien
                    {
                        StudentId = reader["StudentId"].ToString(),
                        StudentName = reader["StudentName"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                        Gender = reader["Gender"].ToString(),
                        ClassId = Convert.ToInt32(reader["ClassId"])
                    };
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

        [HttpPut("{studentId}")]
        public async Task<IActionResult> UpdateSinhVien(string studentId, [FromBody] SinhVien updatedSinhVien)
        {
            if (updatedSinhVien == null)
                return BadRequest("Student data is missing.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_UPDATE, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
                cmd.Parameters.AddWithValue(PARAM_STUDENT_NAME, updatedSinhVien.StudentName);
                cmd.Parameters.AddWithValue(PARAM_BIRTH_DATE, updatedSinhVien.BirthDate);
                cmd.Parameters.AddWithValue(PARAM_GENDER, updatedSinhVien.Gender);
                cmd.Parameters.AddWithValue(PARAM_CLASS_ID, updatedSinhVien.ClassId);

                await con.OpenAsync();
                int i = await cmd.ExecuteNonQueryAsync();
                if (i > 0)
                {
                    // Fetch the updated entity
                    using SqlCommand getCmd = new SqlCommand(SP_GET_BY_ID, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    getCmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
                    using SqlDataReader reader = await getCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        SinhVien updated = new SinhVien
                        {
                            StudentId = reader["StudentId"].ToString(),
                            StudentName = reader["StudentName"].ToString(),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            Gender = reader["Gender"].ToString(),
                            ClassId = Convert.ToInt32(reader["ClassId"])
                        };
                        return Ok(updated);
                    }
                }
                return NotFound("No student found with the provided ID.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the student.");
                return StatusCode(500, "An unexpected error occurred while updating the student. Please try again later.");
            }
        }

        [HttpDelete("{studentId}")]
        public async Task<IActionResult> DeleteSinhVien(string studentId)
        {
            try
            {
                SinhVien deleted = null;
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    // Fetch the entity before deletion
                    using (SqlCommand getCmd = new SqlCommand(SP_GET_BY_ID, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        getCmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
                        await con.OpenAsync();
                        using (SqlDataReader reader = await getCmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                deleted = new SinhVien
                                {
                                    StudentId = reader["StudentId"].ToString(),
                                    StudentName = reader["StudentName"].ToString(),
                                    BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                                    Gender = reader["Gender"].ToString(),
                                    ClassId = Convert.ToInt32(reader["ClassId"])
                                };
                            }
                        }
                    }

                    if (deleted == null)
                        return NotFound("No student found with the provided ID.");

                    // Delete the entity
                    using (SqlCommand cmd = new SqlCommand(SP_DELETE, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        cmd.Parameters.AddWithValue(PARAM_STUDENT_ID, studentId);
                        await con.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                            return Ok(deleted); // Return the deleted entity
                        else
                            return NotFound("No student found with the provided ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the student.");
                return StatusCode(500, "An unexpected error occurred while deleting the student. Please try again later.");
            }
        }
    }
}