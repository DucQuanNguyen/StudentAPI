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
        //private readonly StudentDemoContext _context;
        //public StudentController(StudentDemoContext context)
        //{
        //    _context = context;
        //}
        private readonly string _connectionString;
        SinhVien sv = new SinhVien();
        public StudentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public List<SinhVien> GetSinhViens()
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("GETSinhVien",con);
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
                return liS;
            } 
            else
            {
                return null;
            }
            
        }

        [HttpPost]
        public String CreateSinhVien(SinhVien sinhVien)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("AddSinhVien",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StudentID", sinhVien.StudentId);
            cmd.Parameters.AddWithValue("@StudentName", sinhVien.StudentName);
            cmd.Parameters.AddWithValue("@BirthDate", sinhVien.BirthDate);
            cmd.Parameters.AddWithValue("@Gender", sinhVien.Gender);
            cmd.Parameters.AddWithValue("@ClassID", sinhVien.ClassId);
            
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            string ms;
            if (i > 0) { ms = "Add success!"; }
            else { ms = "Error"; }
            return ms;
        }

        [HttpGet("{id}")]
        public SinhVien GetSinhVienById(int id)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("GETSinhVienById", con);
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
                return s;
            }
            else
            {
                return null;
            }

        }

        [HttpPut("{id}")]
        public string UpdateSinhVien(string id, SinhVien updatedSinhVien)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("UpdateSinhVien", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StudentID", id);
            cmd.Parameters.AddWithValue("@StudentName", updatedSinhVien.StudentName);
            cmd.Parameters.AddWithValue("@BirthDate", updatedSinhVien.BirthDate);
            cmd.Parameters.AddWithValue("@Gender", updatedSinhVien.Gender);
            cmd.Parameters.AddWithValue("@ClassID", updatedSinhVien.ClassId);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            string ms;
            if (i > 0) { ms = "Update success!"; }
            else { ms = "Error"; }
            return ms;
        }

        [HttpDelete("{id}")]
        public string DeleteSinhVien(string id)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("DeleteSinhVien", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StudentID", id);

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
