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
        public StudentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                    return NotFound("Không có sinh viên.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult CreateSinhVien(SinhVien sinhVien)
        {
            try
            {
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
                return i > 0 ? Ok("Add success!") : StatusCode(500, "Error!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                    return NotFound("Không tìm thấy sinh viên với ID được cung cấp.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSinhVien(string id, SinhVien updatedSinhVien)
        {
            try
            {
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
                return i > 0 ? Ok("Update success!") : StatusCode(500, "Error!");
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                return i > 0 ? Ok("Delete success!") : StatusCode(500, "Error!");
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

    }
}
