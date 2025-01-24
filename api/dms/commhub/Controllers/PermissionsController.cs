using commhub.Models;
using commhub.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly string connnectionstring;
        public PermissionsController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        [HttpPost("CreatePermission")]
        public IActionResult CreatePermission([FromBody] Permissions p)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(p.Token, p.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertPermission", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PermissionCode", p.PermissionCode);
                        cmd.Parameters.AddWithValue("@ModuleDefID", p.ModuleDefID);
                        cmd.Parameters.AddWithValue("@PermissionKey", p.PermissionKey);
                        cmd.Parameters.AddWithValue("@PermissionName", p.PermissionName);
                        cmd.Parameters.AddWithValue("@ViewOrder", p.ViewOrder);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", p.CreatedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{p.PermissionCode} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{p.PermissionCode} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating {p.PermissionCode}" });
            }
        }

        [HttpDelete("DeletePermission")]
        public IActionResult DeletePermission([FromBody] Permissions p)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(p.Token, p.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeletePermission", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PermissionID", p.PermissionID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{p.PermissionCode} deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{p.PermissionCode} deletion failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"failed to delete {p.PermissionCode}" });
            }
        }

        [HttpPost("GetPermissions")]
        public IActionResult GetPermissions([FromBody] Permissions p)
        {
            Utility u = new Utility();
            List<Permissions> GetallPermissions = new List<Permissions>();
            if (!u.VerifyToken(p.Token, p.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectPermissions", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Permissions arg = new Permissions();
                                arg.Id = reader.GetInt32("PermissionID");
                                arg.PermissionCode = reader.GetString("PermissionCode");
                                arg.ModuleDefID = reader.GetInt32("ModuleDefID");
                                arg.PermissionKey = reader.GetString("PermissionKey");
                                arg.PermissionName = reader.GetString("PermissionName");
                                arg.ViewOrder = reader.GetInt32("ViewOrder");
                                //arg.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                arg.CreatedOnDate = u.ResolveDate(reader["CreatedOnDate"]);
                                arg.LastModifiedOnDate = u.ResolveDate(reader["LastModifiedOnDate"]);
                                //arg.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                arg.CreatedByUserID = u.ResolveNullableInt(reader["CreatedByUserID"]);
                                arg.LastModifiedByUserID = u.ResolveNullableInt(reader["LastModifiedByUserID"]);

                                GetallPermissions.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Permissions", "Error Retrieving Permissions");
                return BadRequest(ModelState);
            }
            return Ok(GetallPermissions);
        }

        [HttpPost("GetPermission")]
        public IActionResult GetPermission([FromBody] Permissions p)
        {
            Utility u = new Utility();
            List<Permissions> GetallPermission = new List<Permissions>();
            if (!u.VerifyToken(p.Token, p.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectPermission", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PermissionID", p.PermissionID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Permissions arg = new Permissions();
                                arg.PermissionID = reader.GetInt32("PermissionID");
                                arg.PermissionCode = reader.GetString("PermissionCode");
                                arg.ModuleDefID = reader.GetInt32("ModuleDefID");
                                arg.PermissionKey = reader.GetString("PermissionKey");
                                arg.PermissionName = reader.GetString("PermissionName");
                                arg.ViewOrder = reader.GetInt32("ViewOrder");
                                arg.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                arg.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                arg.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");

                                GetallPermission.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Permission", "Error Retrieving Permission");
                return BadRequest(ModelState);
            }
            return Ok(GetallPermission);
        }

        [HttpPut("UpdatePermission")]
        public IActionResult UpdatePermission([FromBody] Permissions p)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(p.Token, p.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("UpdatePermission", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PermissionID", p.PermissionID);
                        cmd.Parameters.AddWithValue("@PermissionCode", p.PermissionCode);
                        cmd.Parameters.AddWithValue("@ModuleDefID", p.ModuleDefID);
                        cmd.Parameters.AddWithValue("@PermissionKey", p.PermissionKey);
                        cmd.Parameters.AddWithValue("@PermissionName", p.PermissionName);
                        cmd.Parameters.AddWithValue("@ViewOrder", p.ViewOrder);
                        cmd.Parameters.AddWithValue("@LastModifiedByUserID", p.LastModifiedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{p.PermissionCode} updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{p.PermissionCode} update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating {p.PermissionCode}" });
            }
        }

        [HttpGet("GetPermissionKeys")]
        public IActionResult GetPermissionKeys()
        {
            try
            {
                string[] permissions = new string[]
                {
                    "VIEW",
                    "EDIT",
                    "CREATE",
                    "READ",
                    "WRITE",
                    "BROWSE",
                    "DELETE"
                };

                // Return the array wrapped in an Ok result
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetPermissionCodes")]
        public IActionResult GetPermissionCodes()
        {
            try
            {
                string[] permissionscode = new string[]
                {
                    "SYSTEM_MODULE_DEFINITION",
                    "SYSTEM_TAB",
                    "SYSTEM_FOLDER",
                    "GROUPS_MODULE",
                };

                // Return the array wrapped in an Ok result
                return Ok(permissionscode);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
