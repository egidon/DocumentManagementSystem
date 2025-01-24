using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using commhub.Utilities;
using commhub.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Cors;

namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class RolesController : ControllerBase
    {
        private readonly string connnectionstring;
        public RolesController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        #region Roles
        /* 
        ************ CREATE ROLE **************
        This module is to create a new role for a user. Authorized users
        will be able to create a role to be assigned to a user.
        */
        [HttpPost("CreateRole")]
        public IActionResult CreateRole([FromBody] Roles roles)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(roles.Token, roles.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("NewRole", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", roles.CreatedByUserID);
                        cmd.Parameters.AddWithValue("@rolename", roles.RoleName);
                        cmd.Parameters.AddWithValue("@description", roles.Description);
                        cmd.Parameters.AddWithValue("@ispublic", roles.IsPublic);
                        cmd.Parameters.AddWithValue("@autoassign", roles.AutoAssignment);
                        cmd.Parameters.AddWithValue("@status", roles.Status);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{roles.RoleName} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{roles.RoleName} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Creating Role" });
            }
        }

        [HttpDelete("DeleteRole")]
        public IActionResult DeleteRole([FromBody] Roles roles)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(roles.Token, roles.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeleteRole", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@roleid", roles.RoleID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{roles.RoleName} deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{roles.RoleName} deletion failed." });
                        }
                    }
                }
                    }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"failed to delete {roles.RoleName}" });
            }
        }

        [HttpPost("GetRoles")]
        public IActionResult GetRoles([FromBody] Roles roles)
        {
            Utility u = new Utility();
            List<Roles>  GetallRoles = new List<Roles>();
            if (!u.VerifyToken(roles.Token, roles.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try 
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetRoles", conn))
                    { 
                    command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Roles AllRoles = new Roles();
                                AllRoles.RoleID = reader.GetInt32(reader.GetOrdinal("RoleID"));
                                AllRoles.RoleGroupID = reader.GetInt32(reader.GetOrdinal("RoleGroupID"));
                                AllRoles.RoleName = reader.GetString(reader.GetOrdinal("RoleName"));
                                AllRoles.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllRoles.IsPublic = reader.GetBoolean(reader.GetOrdinal("IsPublic"));
                                AllRoles.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));

                                AllRoles.LastModifiedOnDate = ResolveDate(reader["LastModifiedOnDate"]);

                                AllRoles.CreatedByUserID = reader.GetInt32(reader.GetOrdinal("CreatedByUserID"));
                                AllRoles.LastModifiedByUserID = ResolveNullableInt(reader["LastModifiedByUserID"]);
                                AllRoles.Status = ResolveNullableInt(reader["Status"]);
                                GetallRoles.Add(AllRoles);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Roles", "Error Retrieving Roles");
                return BadRequest(ModelState);
            }
            return Ok(GetallRoles);
        }

        private DateTime? ResolveDate(object dbValue)
        {
            return dbValue == DBNull.Value ? (DateTime?)null : (DateTime)dbValue;
        }

        private int? ResolveNullableInt(object dbValue)
        {
            return dbValue == DBNull.Value ? (int?)null : (int)dbValue;
        }

        [HttpPost("GetRole")]
        public IActionResult GetRole([FromBody] Roles roles)
        {
            Utility u = new Utility();
            List<Roles> GetallRole = new List<Roles>();
            if (!u.VerifyToken(roles.Token, roles.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetRole", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@roleid", roles.RoleID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Roles AllRoles = new Roles();
                                AllRoles.RoleGroupID = reader.GetInt32("RoleGroupId");
                                AllRoles.RoleName = reader.GetString("RoleName");
                                AllRoles.Description = reader.GetString("Description");
                                AllRoles.IsPublic = reader.GetBoolean("IsPublic");
                                AllRoles.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                AllRoles.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                AllRoles.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                AllRoles.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                AllRoles.Status = reader.GetInt32("Status");
                                GetallRole.Add(AllRoles);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Roles", "Error Retrieving Roles");
                return BadRequest(ModelState);
            }
            return Ok(GetallRole);
        }

        [HttpPut("UpdateRole")]
        public IActionResult UpdateRole([FromBody] Roles roles)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(roles.Token, roles.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("UpdateRole", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", roles.CreatedByUserID);
                        cmd.Parameters.AddWithValue("@rolename", roles.RoleName);
                        cmd.Parameters.AddWithValue("@description", roles.Description);
                        cmd.Parameters.AddWithValue("@ispublic", roles.IsPublic);
                        cmd.Parameters.AddWithValue("@autoassign", roles.AutoAssignment);
                        cmd.Parameters.AddWithValue("@status", roles.Status);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{roles.RoleName} updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{roles.RoleName} update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating {roles.RoleName}" });
            }
        }

        #endregion

    }
}
