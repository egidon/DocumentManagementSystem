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
    public class ModulePermissionController : ControllerBase
    {
        private readonly string connnectionstring;
        public ModulePermissionController(IConfiguration configuration) 
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }
        [HttpPost("CreateModulePermission")]
        public IActionResult CreateModulePermission([FromBody] ModulePermission mp)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(mp.Token, mp.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertModulePermission", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ModuleID", mp.ModuleID);
                        cmd.Parameters.AddWithValue("@PermissionID", mp.PermissionID);
                        cmd.Parameters.AddWithValue("@AllowAccess", mp.AllowAccess);
                        cmd.Parameters.AddWithValue("@RoleID", mp.RoleID);
                        cmd.Parameters.AddWithValue("@UserID", mp.UserID);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", mp.CreatedByUserID);
                        cmd.Parameters.AddWithValue("@LastModifiedByUserID", mp.LastModifiedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"Module with ID {mp.ModuleID} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"Module with ID {mp.ModuleID} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating Module with ID {mp.ModuleID}" });
            }
        }

        [HttpDelete("DeleteModulePermission")]
        public IActionResult DeleteModulePermission([FromBody] ModulePermission mp)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(mp.Token, mp.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeleteModulePermission", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ModulePermissionID", mp.ModulePermissionID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"Module with ID  {mp.ModulePermissionID} deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"Module with ID  {mp.ModulePermissionID} deletion failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"failed to delete Module with ID  {mp.ModulePermissionID}" });
            }
        }

        [HttpPost("GetModulePermissions")]
        public IActionResult GetModulePermissions([FromBody] ModulePermission mp)
        {
            Utility u = new Utility();
            List<ModulePermission> GetallModulePermissions = new List<ModulePermission>();
            if (!u.VerifyToken(mp.Token, mp.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectModulePermissions", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModulePermission AllModulePermissions = new ModulePermission();
                                AllModulePermissions.ModulePermissionID = reader.GetInt32("ModulePermissionID");
                                AllModulePermissions.ModuleID = reader.GetInt32("ModuleID");
                                AllModulePermissions.PermissionID = reader.GetInt32("PermissionID");
                                AllModulePermissions.AllowAccess = reader.GetBoolean("AllowAccess");
                                AllModulePermissions.RoleID = reader.GetInt32("RoleID");
                                AllModulePermissions.UserID = reader.GetInt32("UserID");
                                AllModulePermissions.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                AllModulePermissions.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                AllModulePermissions.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                AllModulePermissions.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");

                                GetallModulePermissions.Add(AllModulePermissions);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ModulePermissions", "Error Retrieving Module Permissions");
                return BadRequest(ModelState);
            }
            return Ok(GetallModulePermissions);
        }

        [HttpPost("GetModulePermission")]
        public IActionResult GetModulePermission([FromBody] ModulePermission mp)
        {
            Utility u = new Utility();
            List<ModulePermission> GetallModulePermission = new List<ModulePermission>();
            if (!u.VerifyToken(mp.Token, mp.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectModulePermissions", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ModulePermissionID", mp.ModulePermissionID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModulePermission AllModulePermissions = new ModulePermission();
                                AllModulePermissions.ModulePermissionID = reader.GetInt32("ModulePermissionID");
                                AllModulePermissions.ModuleID = reader.GetInt32("ModuleID");
                                AllModulePermissions.PermissionID = reader.GetInt32("PermissionID");
                                AllModulePermissions.AllowAccess = reader.GetBoolean("AllowAccess");
                                AllModulePermissions.RoleID = reader.GetInt32("RoleID");
                                AllModulePermissions.UserID = reader.GetInt32("UserID");
                                AllModulePermissions.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                AllModulePermissions.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                AllModulePermissions.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                AllModulePermissions.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");

                                GetallModulePermission.Add(AllModulePermissions);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ModulePermission", "Error Retrieving Module Permission");
                return BadRequest(ModelState);
            }
            return Ok(GetallModulePermission);
        }

        [HttpPut("UpdateModulePermission")]
        public IActionResult UpdateModulePermission([FromBody] ModulePermission mp)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(mp.Token, mp.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("UpdateModuleDefinition", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ModulePermissionID", mp.ModulePermissionID);
                        cmd.Parameters.AddWithValue("@ModuleID", mp.ModuleID);
                        cmd.Parameters.AddWithValue("@PermissionID", mp.PermissionID);
                        cmd.Parameters.AddWithValue("@AllowAccess", mp.AllowAccess);
                        cmd.Parameters.AddWithValue("@RoleID", mp.RoleID);
                        cmd.Parameters.AddWithValue("@UserID", mp.UserID);
                        cmd.Parameters.AddWithValue("@LastModifiedByUserID", mp.LastModifiedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"Module with ID {mp.ModuleID} updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"Module with ID  {mp.ModuleID} update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating Module with ID {mp.ModuleID}" });
            }
        }

    }
}
