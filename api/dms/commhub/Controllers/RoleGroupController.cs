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
    public class RoleGroupController : ControllerBase
    {
        private readonly string connnectionstring;
        public RoleGroupController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        [HttpPost("CreateRoleGroup")]
        public IActionResult CreateRoleGroup([FromBody] RoleGroup rg)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(rg.Token, rg.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertRoleGroup", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleGroupName", rg.RoleGroupName);
                        cmd.Parameters.AddWithValue("@Description", rg.Description);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", rg.CreatedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{rg.RoleGroupName} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{rg.RoleGroupName} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating {rg.RoleGroupName}" });
            }
        }

        [HttpDelete("DeleteRoleGroup")]
        public IActionResult DeleteRoleGroup([FromBody] RoleGroup rg)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(rg.Token, rg.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeleteRoleGroup", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleGroupID", rg.RoleGroupID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{rg.RoleGroupName} deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{rg.RoleGroupName} deletion failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"failed to delete {rg.RoleGroupName}" });
            }
        }

        [HttpPost("GetRoleGroups")]
        public IActionResult GetRoleGroups([FromBody] RoleGroup rg)
        {
            Utility u = new Utility();
            List<RoleGroup> GetallRoleGroups = new List<RoleGroup>();
            if (!u.VerifyToken(rg.Token, rg.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectRoleGroups", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RoleGroup arg = new RoleGroup();
                                arg.RoleGroupID = reader.GetInt32("RoleGroupID");
                                arg.RoleGroupName = reader.GetString("RoleGroupName");
                                arg.Description = reader.GetString("Description");
                                arg.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                arg.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                arg.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");

                                GetallRoleGroups.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("RoleGroup", "Error Retrieving RoleGroups");
                return BadRequest(ModelState);
            }
            return Ok(GetallRoleGroups);
        }

        [HttpPost("GetRoleGroup")]
        public IActionResult GetRoleGroup([FromBody] RoleGroup rg)
        {
            Utility u = new Utility();
            List<RoleGroup> GetallRoleGroup = new List<RoleGroup>();
            if (!u.VerifyToken(rg.Token, rg.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectRoleGroup", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@RoleGroupID", rg.RoleGroupID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RoleGroup arg = new RoleGroup();
                                arg.RoleGroupID = reader.GetInt32("RoleGroupID");
                                arg.RoleGroupName = reader.GetString("RoleGroupName");
                                arg.Description = reader.GetString("Description");
                                arg.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                arg.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                arg.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                arg.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");

                                GetallRoleGroup.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("RoleGroup", "Error Retrieving RoleGroups");
                return BadRequest(ModelState);
            }
            return Ok(GetallRoleGroup);
        }

        [HttpPut("UpdateRoleGroup")]
        public IActionResult UpdateRoleGroup([FromBody] RoleGroup rg)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(rg.Token, rg.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("UpdateRoleGroup", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@RoleGroupID", rg.RoleGroupID);
                        cmd.Parameters.AddWithValue("@RoleGroupName", rg.RoleGroupName);
                        cmd.Parameters.AddWithValue("@Description", rg.Description);
                        cmd.Parameters.AddWithValue("@LastModifiedByUserID", rg.LastModifiedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{rg.RoleGroupName} updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{rg.RoleGroupName} update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating {rg.RoleGroupName}" });
            }
        }

    }
}
