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
    public class ModuleDefinitionController : ControllerBase
    {
        private readonly string connnectionstring;
        public ModuleDefinitionController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }
        
        [HttpPost("CreateModuleDefinition")]
        public IActionResult CreateModuleDefinition([FromBody] ModuleDefinition md)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(md.Token, md.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertModuleDefinition", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FriendlyName", md.FriendlyName);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", md.CreatedByUserID);
                        cmd.Parameters.AddWithValue("@LastModifiedByUserID", md.LastModifiedByUserID);
                        cmd.Parameters.AddWithValue("@DefinitionName", md.DefinitionName);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{md.FriendlyName} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{md.FriendlyName} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating {md.FriendlyName}" });
            }
        }

        [HttpDelete("DeleteModuleDefinition")]
        public IActionResult DeleteModuleDefinition([FromBody] ModuleDefinition md)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(md.Token, md.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeleteModuleDefinition", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ModuleDefID", md.ModuleDefID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{md.FriendlyName} deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{md.FriendlyName} deletion failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"failed to delete {md.FriendlyName}" });
            }
        }

        [HttpPost("GetModuleDefinitions")]
        public IActionResult GetModuleDefinitions([FromBody] ModuleDefinition md)
        {
            Utility u = new Utility();
            List<ModuleDefinition> GetallModuleDefinitions = new List<ModuleDefinition>();
            if (!u.VerifyToken(md.Token, md.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectModuleDefinitions", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModuleDefinition AllModules = new ModuleDefinition();
                                AllModules.ModuleDefID = reader.GetInt32("ModuleDefID");
                                AllModules.FriendlyName = reader.GetString("FriendlyName");
                                AllModules.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                AllModules.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                AllModules.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                AllModules.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                AllModules.DefinitionName = reader.GetString("DefinitionName");

                                GetallModuleDefinitions.Add(AllModules);
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
            return Ok(GetallModuleDefinitions);
        }

        [HttpPost("GetModuleDefinition")]
        public IActionResult GetModuleDefinition([FromBody] ModuleDefinition md)
        {
            Utility u = new Utility();
            List<ModuleDefinition> GetallModuleDefinition = new List<ModuleDefinition>();
            if (!u.VerifyToken(md.Token, md.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("SelectModuleDefinition", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ModuleDefID", md.ModuleDefID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModuleDefinition AllModules = new ModuleDefinition();
                                AllModules.ModuleDefID = reader.GetInt32("ModuleDefID");
                                AllModules.FriendlyName = reader.GetString("FriendlyName");
                                AllModules.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                AllModules.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                AllModules.LastModifiedByUserID = reader.GetInt32("LastModifiedByUserID");
                                AllModules.LastModifiedOnDate = reader.GetDateTime("LastModifiedOnDate");
                                AllModules.DefinitionName = reader.GetString("DefinitionName");

                                GetallModuleDefinition.Add(AllModules);
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
            return Ok(GetallModuleDefinition);
        }

        [HttpPut("UpdateModuleDefinition")]
        public IActionResult UpdateModuleDefinition([FromBody] ModuleDefinition md)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(md.Token, md.SecretKey))
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
                        cmd.Parameters.AddWithValue("@ModuleDefID", md.ModuleDefID);
                        cmd.Parameters.AddWithValue("@FriendlyName", md.FriendlyName);
                        cmd.Parameters.AddWithValue("@LastModifiedByUserID", md.LastModifiedByUserID);
                        cmd.Parameters.AddWithValue("@DefinitionName", md.DefinitionName);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{md.FriendlyName} updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{md.FriendlyName} update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating {md.FriendlyName}" });
            }
        }

        [HttpGet("GetModuledefName")]
        public IActionResult GetModuledefName()
        {
            List<ModuleDefinition> GetallModuleDefinitions = new List<ModuleDefinition>();
           
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetModuledefName", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModuleDefinition AllModules = new ModuleDefinition();
                                AllModules.ModuleDefID = reader.GetInt32("ModuleDefID");
                                AllModules.DefinitionName = reader.GetString("DefinitionName");

                                GetallModuleDefinitions.Add(AllModules);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Roles", "Error Retrieving Modules");
                return BadRequest(ModelState);
            }
            return Ok(GetallModuleDefinitions);
        }

    }
}
