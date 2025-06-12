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
        public ClassController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult  GetLopHoc()
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
                    return NotFound("Không có lớp học.");
                }
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult CreateLopHoc(LopHoc lopHoc)
        {
            try
            {

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("AddLopHoc", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", lopHoc.Id);
            cmd.Parameters.AddWithValue("@ClassName", lopHoc.ClassName);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i > 0 ? Ok("Add success!") : StatusCode(500, "Error");
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
                    return NotFound("Không tìm thấy lớp học với ID được cung cấp.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLopHoc(int id, LopHoc updatedLopHoc)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("UpdateLopHoc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@ClassName", updatedLopHoc.ClassName);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Update success!") : StatusCode(500, "Error");
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                return i > 0 ? Ok("Delete success!") : StatusCode(500, "Error");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

    }
}
