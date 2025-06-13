using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentAPI.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication by default
    public class ClassController : ControllerBase
    {
        // Stored Procedure Names
        private const string SP_GET_ALL = "GETLopHoc";
        private const string SP_GET_BY_ID = "GETLopHocbyId";
        private const string SP_ADD = "AddLopHoc";
        private const string SP_UPDATE = "UpdateLopHoc";
        private const string SP_DELETE = "DeleteLopHoc";

        // Parameter Names
        private const string PARAM_ID = "@Id";
        private const string PARAM_CLASS_NAME = "@ClassName";

        private readonly string _connectionString;
        private readonly ILogger<ClassController> _logger;
        public ClassController(IConfiguration configuration, ILogger<ClassController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // All authenticated users can view classes
        [HttpGet]
        public async Task<IActionResult> GetLopHoc()
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
                List<LopHoc> liL = new List<LopHoc>();
                while (await reader.ReadAsync())
                {
                    LopHoc l = new LopHoc
                    {
                        Id = Convert.ToInt32(reader["ID"]),
                        ClassName = reader["ClassName"].ToString()
                    };
                    liL.Add(l);
                }
                return liL.Count > 0
                    ? Ok(liL)
                    : NotFound(new { message = "No classes found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving classes.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving classes. Please try again later." });
            }
        }

        // Only admins can create classes
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLopHoc([FromBody] LopHoc lopHoc)
        {
            if (lopHoc == null)
                return BadRequest(new { message = "Class data is missing." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new { message = "Validation failed.", errors });
            }

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_ADD, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(PARAM_CLASS_NAME, SqlDbType.NVarChar, 100).Value = lopHoc.ClassName;

                // Output parameter for new ID
                var idParam = cmd.Parameters.Add(PARAM_ID, SqlDbType.Int);
                idParam.Direction = ParameterDirection.Output;

                await con.OpenAsync();
                int i = await cmd.ExecuteNonQueryAsync();

                if (i > 0 && idParam.Value != DBNull.Value)
                {
                    int newId = (int)idParam.Value;
                    // Fetch the created entity
                    using SqlCommand getCmd = new SqlCommand(SP_GET_BY_ID, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    getCmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = newId;
                    using SqlDataReader reader = await getCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        LopHoc created = new LopHoc
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            ClassName = reader["ClassName"].ToString()
                        };
                        return CreatedAtAction(nameof(GetLopHocById), new { id = created.Id }, created);
                    }
                }
                return StatusCode(500, new { message = "Failed to add class. The class may already exist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a class.");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the class. Please try again later." });
            }
        }

        // All authenticated users can view a class by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLopHocById(int id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_GET_BY_ID, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
                await con.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    LopHoc l = new LopHoc
                    {
                        Id = Convert.ToInt32(reader["ID"]),
                        ClassName = reader["ClassName"].ToString()
                    };
                    return Ok(l);
                }
                else
                {
                    return NotFound(new { message = "No class found with the provided ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving class by ID.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the class. Please try again later." });
            }
        }

        // Only admins can update classes
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLopHoc(int id, [FromBody] LopHoc updatedLopHoc)
        {
            if (updatedLopHoc == null)
                return BadRequest(new { message = "Class data is missing." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new { message = "Validation failed.", errors });
            }

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(SP_UPDATE, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
                cmd.Parameters.Add(PARAM_CLASS_NAME, SqlDbType.NVarChar, 100).Value = updatedLopHoc.ClassName;

                await con.OpenAsync();
                int i = await cmd.ExecuteNonQueryAsync();
                if (i > 0)
                {
                    // Fetch the updated entity
                    using SqlCommand getCmd = new SqlCommand(SP_GET_BY_ID, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    getCmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
                    using SqlDataReader reader = await getCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        LopHoc updated = new LopHoc
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            ClassName = reader["ClassName"].ToString()
                        };
                        return Ok(updated);
                    }
                }
                return NotFound(new { message = "No class found with the provided ID." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the class.");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the class. Please try again later." });
            }
        }

        // Only admins can delete classes
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLopHoc(int id)
        {
            try
            {
                LopHoc deleted = null;
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();

                    // Fetch the entity before deletion
                    using (SqlCommand getCmd = new SqlCommand(SP_GET_BY_ID, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        getCmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
                        using (SqlDataReader reader = await getCmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                deleted = new LopHoc
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    ClassName = reader["ClassName"].ToString()
                                };
                            }
                        }
                    }

                    if (deleted == null)
                        return NotFound(new { message = "No class found with the provided ID." });

                    // Delete the entity
                    using (SqlCommand cmd = new SqlCommand(SP_DELETE, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        cmd.Parameters.Add(PARAM_ID, SqlDbType.Int).Value = id;
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                            return Ok(deleted); // Return the deleted entity
                        else
                            return NotFound(new { message = "No class found with the provided ID." });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the class.");
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the class. Please try again later." });
            }
        }
    }
}