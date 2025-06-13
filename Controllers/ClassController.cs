using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ClassController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<ClassController> _logger;
        public ClassController(IConfiguration configuration, ILogger<ClassController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetLopHoc()
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlDataAdapter adapter = new SqlDataAdapter("GETLopHoc", con);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<LopHoc> liL = new List<LopHoc>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        LopHoc l = new LopHoc();
                        l.Id = Convert.ToInt32(row[0].ToString());
                        l.ClassName = row[1].ToString();
                        liL.Add(l);
                    }
                }
                if (liL.Count > 0)
                {
                    return Ok(liL);
                }
                else
                {
                    return NotFound("No class found");
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while retrieving classes.");
                return StatusCode(500, "An unexpected error occurred while retrieving classes. Please try again later.");
            }
        }

        [HttpPost]
        public IActionResult CreateLopHoc(LopHoc lopHoc)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("AddLopHoc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", lopHoc.Id);
                cmd.Parameters.AddWithValue("@ClassName", lopHoc.ClassName);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Class added successfully!") : StatusCode(500, "Failed to add class. The class may already exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a class.");
                return StatusCode(500, "An unexpected error occurred while adding the class. Please try again later.");
            }
            
        }

        [HttpGet("{id}")]
        public IActionResult GetLopHocById(int id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlDataAdapter adapter = new SqlDataAdapter("GETLopHocbyId", con);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                LopHoc l = new LopHoc();
                if (dataTable.Rows.Count > 0)
                {
                    l.Id = Convert.ToInt32(dataTable.Rows[0]["ID"].ToString());
                    l.ClassName = dataTable.Rows[0]["ClassName"].ToString();
                    return Ok(l);
                }
                else
                {
                    return NotFound("No class found with the provided ID.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving class by ID.");
                return StatusCode(500, "An unexpected error occurred while retrieving the class. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLopHoc(int id, LopHoc updatedLopHoc)
        {
            try
            {
                if (updatedLopHoc == null)
                {
                    return BadRequest("Class data is missing.");
                }
                if (string.IsNullOrWhiteSpace(updatedLopHoc.ClassName))
                {
                    return BadRequest("Class name is required.");
                }
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("UpdateLopHoc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@ClassName", updatedLopHoc.ClassName);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Class updated successfully!") : StatusCode(500, "Failed to update class. The class may not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the class.");
                return StatusCode(500, "An unexpected error occurred while updating the class. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLopHoc(int id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("DeleteLopHoc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Class deleted successfully!") : NotFound("No class found with the provided ID.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the class.");
                return StatusCode(500, "An unexpected error occurred while deleting the class. Please try again later.");
            }
        }
    }
}
