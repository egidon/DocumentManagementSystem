using commhub.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using commhub.Utilities;



namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class FilesController : ControllerBase
    {
        private readonly string documents = "Documents";
        private readonly string teamdocuments = "Documents\\Teams";
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<FilesController> _logger;
        Utility u = new Utility();
        private readonly string connnectionstring;

        public FilesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<FilesController> logger)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }
        
        private DateTime? ResolveDate(object dbValue)
        {
            return dbValue == DBNull.Value ? (DateTime?)null : (DateTime)dbValue;
        }

        private int? ResolveNullableInt(object dbValue)
        {
            return dbValue == DBNull.Value ? (int?)null : (int)dbValue;
        }

        #region Personal Files
        [HttpPost("CreateFile")]
        public async Task<IActionResult> CreateFile([FromForm] commhub.Models.FolderFiles f)
        {
            var filename = "";
            var fileExt = "";
            var physicalPath = string.Empty;
            f.Size = f.FileForUpload.Length;

            var result = new CreateMediaResult();
            if (!u.VerifyToken(f.Token, f.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            //*********** Verify if file exists *********************
            if (f.FileForUpload.Length <= 0)
            {
                return StatusCode(500, new { message = $"File is Empty" });
            }
            try
            {
                //**************** Upload file *****************
                filename = Path.GetFileName(f.FileForUpload.FileName);
                fileExt = Path.GetExtension(f.FileForUpload.FileName);

                // Get the current directory
                string currentDirectory = Directory.GetCurrentDirectory();

                // Combine the current directory with the directory name
                var temp = Path.Combine(Environment.CurrentDirectory, documents, f.FolderName);
                
                physicalPath = Path.Combine(temp, filename);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    
                    await f.FileForUpload.CopyToAsync(stream);
                }

                //*************** Insert into Database *********
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("CreateFile", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FileName", filename);
                        cmd.Parameters.AddWithValue("@Extension", fileExt);
                        cmd.Parameters.AddWithValue("@Size", f.Size);
                        cmd.Parameters.AddWithValue("@Description", f.Description);
                        cmd.Parameters.AddWithValue("@Location", $"Documents/{f.FolderName}/" + filename);
                        cmd.Parameters.AddWithValue("@FolderId", f.FolderId);
                        cmd.Parameters.AddWithValue("@CreatedByUserId", f.CreatedByUserId);
                        cmd.Parameters.AddWithValue("@WhoHasAccess", 0);
                        cmd.Parameters.AddWithValue("@VersionGuid", f.VersionGuid);

                        // Execute the command
                        int rowsAffected =  cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{filename} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{filename} creation failed." });
                        }
                    }
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access to the path '{Path}' is denied.", physicalPath);
                return StatusCode(500, new { message = $"Access to the path '{physicalPath}' is denied: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating {f.FileName}" });
            }
        }

        [HttpPost("GetFiles")]
        public IActionResult GetFiles([FromBody] commhub.Models.DmsFiles files)
        {
            Utility u = new Utility();
            List<commhub.Models.DmsFiles> GetallFiles = new List<Models.DmsFiles>();
            if (!u.VerifyToken(files.Token, files.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetFilesByFolderId", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FolderId", files.FolderId);
                        command.Parameters.AddWithValue("@CreatedByUserId", files.CreatedByUserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Models.DmsFiles AllFiles = new Models.DmsFiles();
                                AllFiles.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                AllFiles.FolderId = ResolveNullableInt(reader["FolderId"]);
                                AllFiles.Size = reader.GetInt64(reader.GetOrdinal("Size"));
                                AllFiles.Extension = reader.GetString(reader.GetOrdinal("Extension"));
                                AllFiles.Location = reader.GetString(reader.GetOrdinal("Location"));
                                AllFiles.FileName = reader.GetString(reader.GetOrdinal("FileName"));
                                AllFiles.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllFiles.DisplayName = reader.IsDBNull(reader.GetOrdinal("CreatedByDisplayName")) ? null : reader.GetString(reader.GetOrdinal("CreatedByDisplayName"));
                                AllFiles.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFiles.ModifiedOnDate = ResolveDate(reader["ModifiedOnDate"]);
                                AllFiles.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId"));
                                AllFiles.ModifiedByUserId = ResolveNullableInt(reader["ModifiedByUserId"]);
                                AllFiles.WhoHasAccess = ResolveNullableInt(reader["WhoHasAccess"]);
                                AllFiles.VersionGuid = reader.GetGuid(reader.GetOrdinal("VersionGuid"));
                                GetallFiles.Add(AllFiles);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Folders", "Error Retrieving Folders");
                return BadRequest(ModelState);
            }
            return Ok(GetallFiles);
        }

        #endregion

        #region Team Files
        [HttpPost("CreateTeamFile")]
        public async Task<IActionResult> CreateTeamFile([FromForm] commhub.Models.FolderFiles f)
        {
            var filename = "";
            var fileExt = "";
            var physicalPath = string.Empty;
            f.Size = f.FileForUpload.Length;

            var result = new CreateMediaResult();
            if (!u.VerifyToken(f.Token, f.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            //*********** Verify if file exists *********************
            if (f.FileForUpload.Length <= 0)
            {
                return StatusCode(500, new { message = $"File is Empty" });
            }
            try
            {
                //**************** Upload file *****************
                filename = Path.GetFileName(f.FileForUpload.FileName);
                fileExt = Path.GetExtension(f.FileForUpload.FileName);

                // Get the current directory
                string currentDirectory = Directory.GetCurrentDirectory();

                // Combine the current directory with the directory name
                var temp = Path.Combine(Environment.CurrentDirectory, teamdocuments, f.FolderName);

                physicalPath = Path.Combine(temp, filename);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {

                    await f.FileForUpload.CopyToAsync(stream);
                }

                //*************** Insert into Database *********
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();

                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("CreateTeamFile", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FileName", filename);
                        cmd.Parameters.AddWithValue("@Extension", fileExt);
                        cmd.Parameters.AddWithValue("@Size", f.Size);
                        cmd.Parameters.AddWithValue("@Description", f.Description);
                        cmd.Parameters.AddWithValue("@Location", $"Documents/{f.FolderName}/" + filename);
                        cmd.Parameters.AddWithValue("@FolderId", f.FolderId);
                        cmd.Parameters.AddWithValue("@CreatedByUserId", f.CreatedByUserId);
                        cmd.Parameters.AddWithValue("@WhoHasAccess", 0);
                        cmd.Parameters.AddWithValue("@VersionGuid", f.VersionGuid);

                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{filename} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{filename} creation failed." });
                        }
                    }
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access to the path '{Path}' is denied.", physicalPath);
                return StatusCode(500, new { message = $"Access to the path '{physicalPath}' is denied: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating {f.FileName}" });
            }
        }

        /**
         Get Files from authorized folders for the team
         */
        [HttpPost("GetTeamFiles")]
        public IActionResult GetTeamFiles([FromBody] commhub.Models.DmsFiles files)
        {
            Utility u = new Utility();
            List<commhub.Models.DmsFiles> GetallFiles = new List<Models.DmsFiles>();
            if (!u.VerifyToken(files.Token, files.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            int roleId = GetRoleId(files.FolderId.ToString(), files.CreatedByUserId.ToString());
            if (roleId == 0) {
                return Unauthorized(new { message = $" You haven't been granted access to this folder."});
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("CheckUserAndGetFiles", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@folderid", files.FolderId);
                        command.Parameters.AddWithValue("@roleid", roleId);
                        command.Parameters.AddWithValue("@userid", files.CreatedByUserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Models.DmsFiles AllFiles = new Models.DmsFiles();
                                AllFiles.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                AllFiles.FolderId = ResolveNullableInt(reader["FolderId"]);
                                AllFiles.Size = reader.GetInt64(reader.GetOrdinal("Size"));
                                AllFiles.Extension = reader.GetString(reader.GetOrdinal("Extension"));
                                AllFiles.Location = reader.GetString(reader.GetOrdinal("Location"));
                                AllFiles.FileName = reader.GetString(reader.GetOrdinal("FileName"));
                                AllFiles.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllFiles.DisplayName = reader.IsDBNull(reader.GetOrdinal("CreatedByDisplayName")) ? null : reader.GetString(reader.GetOrdinal("CreatedByDisplayName"));
                                AllFiles.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFiles.ModifiedOnDate = ResolveDate(reader["ModifiedOnDate"]);
                                AllFiles.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId"));
                                AllFiles.ModifiedByUserId = ResolveNullableInt(reader["ModifiedByUserId"]);
                                AllFiles.WhoHasAccess = ResolveNullableInt(reader["WhoHasAccess"]);
                                AllFiles.VersionGuid = reader.GetGuid(reader.GetOrdinal("VersionGuid"));
                                GetallFiles.Add(AllFiles);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Folders", "Error Retrieving Folders");
                return BadRequest(ModelState);
            }
            return Ok(GetallFiles);
        }
        
        private int GetRoleId(string folderid, string userid)
        {
            int folder_id = Convert.ToInt32(folderid);
            int user_id = Convert.ToInt32(userid);
            int roleId = 0;

            using (var conn = new SqlConnection(connnectionstring))
            {
                conn.Open();
                using (var command = new SqlCommand("GetRoleIdByFolderIdAndUserId", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FolderId", folder_id);
                    command.Parameters.AddWithValue("@UserId", user_id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roleId = reader.GetInt32(reader.GetOrdinal("roleid"));
                        }
                    }
                }
            }
            return roleId;
        }
        #endregion
    }
}
