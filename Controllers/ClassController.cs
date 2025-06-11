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
        public List<LopHoc> GetLopHoc()
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("GETLopHoc", con);
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
                return liL;
            }
            else
            {
                return null;
            }

        }

        [HttpPost]
        public String CreateLopHoc(LopHoc lopHoc)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("AddLopHoc",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", lopHoc.Id);
            cmd.Parameters.AddWithValue("@ClassName", lopHoc.ClassName);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            string ms;
            if (i > 0) { ms = "Add success!"; }
            else { ms = "Error"; }
            return ms;
        }

        [HttpGet("{id}")]
        public LopHoc GetLopHocById(int id)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("GETLopHocbyId", con);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("@ID", id); 
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            LopHoc l = new LopHoc();
            if (dataTable.Rows.Count > 0)
            {
                l.Id = Convert.ToInt32(dataTable.Rows[0]["ID"].ToString());
                l.ClassName = dataTable.Rows[0]["ClassName"].ToString();
                return l;
            }
            else
            {
                return null;
            }
            
        }

        [HttpPut("{id}")]
        public string UpdateLopHoc(int id, LopHoc updatedLopHoc)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("UpdateLopHoc", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ClassName", updatedLopHoc.ClassName);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            string ms;
            if (i > 0) { ms = "Update success!"; }
            else { ms = "Error"; }
            return ms;
        }

        [HttpDelete("{id}")]
        public string DeleteLopHoc(int id)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("DeleteLopHoc", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            string ms;
            if (i > 0) { ms = "Delete success!"; }
            else { ms = "Error"; }
            return ms;
        }

    }
}
