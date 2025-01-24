using commhub.Models;
using commhub.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class UserRolesController : ControllerBase
    {
        private readonly string connnectionstring;

        Utility u = new Utility();
        public UserRolesController(IConfiguration configuration) 
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        #region User Roles
        [HttpPost("CreateUserRole")]
        public IActionResult CreateUserRole([FromBody] UserRoles ur)
        {
            if (!u.VerifyToken(ur.Token, ur.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertUserRole", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleID", ur.RoleID);
                        cmd.Parameters.AddWithValue("@EffectiveDate", ur.EffectiveDate);
                        cmd.Parameters.AddWithValue("@ExpiryDate", ur.ExpiryDate);
                        cmd.Parameters.AddWithValue("@Status", ur.Status);
                        cmd.Parameters.AddWithValue("@IsOwner", ur.IsOwner); 
                        cmd.Parameters.AddWithValue("@UserID", ur.UserID);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", ur.CreatedByUserID);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = "UserRole created successfully"
                            }) ;
                        }
                        else
                        {
                            return NotFound(new { message = "UserRole creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Creating UserRole" });
            }
        }

        [HttpDelete("DeleteUserRole")]
        public IActionResult DeleteUserRole([FromBody] UserRoles ur)
        {
            
            if (!u.VerifyToken(ur.Token, ur.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeleteUserRole", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserRoleID", ur.UserRoleID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = "UserRole deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = "UserRole deletion failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "failed to delete UserRole" });
            }
        }

        [HttpPost("GetUserRoles")]
        public IActionResult GetUserRoles([FromBody] UserRoles ur)
        {
            List<UserRoles> GetallUserRoles = new List<UserRoles>();
            if (!u.VerifyToken(ur.Token, ur.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectUserRoles", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserRoles arg = new UserRoles();
                                arg.UserRoleID = reader.GetInt32("UserRoleID");
                                arg.UserID = reader.GetInt32("UserID");
                                arg.RoleID = reader.GetInt32("RoleID");
                                arg.ExpiryDate = reader.GetDateTime("ExpiryDate");
                                arg.IsTrialUsed = reader.GetBoolean("IsTrialUsed");
                                arg.EffectiveDate = reader.GetDateTime("EffectiveDate");
                                arg.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                arg.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                arg.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("Status");
                                arg.LastModifiedByUserID = reader.GetInt32("IsOwner");

                                GetallUserRoles.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserRoles", "Error Retrieving UserRoles");
                return BadRequest(ModelState);
            }
            return Ok(GetallUserRoles);
        }

        [HttpPost("GetUserRole")]
        public IActionResult GetUserRole([FromBody] UserRoles ur)
        {
            List<UserRoles> GetallUserRole = new List<UserRoles>();
            if (!u.VerifyToken(ur.Token, ur.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectUserRole", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", ur.UserRoleID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserRoles arg = new UserRoles();
                                arg.UserRoleID = reader.GetInt32("UserRoleID");
                                arg.UserID = reader.GetInt32("UserID");
                                arg.RoleID = reader.GetInt32("RoleID");
                                arg.ExpiryDate = reader.GetDateTime("ExpiryDate");
                                arg.IsTrialUsed = reader.GetBoolean("IsTrialUsed");
                                arg.EffectiveDate = reader.GetDateTime("EffectiveDate");
                                arg.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                arg.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                arg.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("Status");
                                arg.LastModifiedByUserID = reader.GetInt32("IsOwner");

                                GetallUserRole.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserRole", "Error Retrieving UserRole");
                return BadRequest(ModelState);
            }
            return Ok(GetallUserRole);
        }

        [HttpPut("UpdateUserRoles")]
        public IActionResult UpdateUserRoles([FromBody] UserRoles ur)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(ur.Token, ur.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("UpdateUserRole", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserRoleID", ur.UserRoleID);
                        cmd.Parameters.AddWithValue("@UserID", ur.UserID);
                        cmd.Parameters.AddWithValue("@RoleID", ur.RoleID);
                        cmd.Parameters.AddWithValue("@Status", ur.Status);
                        cmd.Parameters.AddWithValue("@IsOwner", ur.IsOwner);
                       
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = "UserRole updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = "UserRole update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating UserRole" });
            }
        }
        #endregion
    }
}
