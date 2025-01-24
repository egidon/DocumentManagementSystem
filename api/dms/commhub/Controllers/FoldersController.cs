using commhub.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using commhub.Utilities;
using System.Data;
using System.Security.AccessControl;
using System.Security.Principal;

namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class FoldersController : ControllerBase
    {
        private readonly string connnectionstring;
        private readonly string documents = "Documents";
        private readonly string teamdocuments = "Documents\\Teams";

        public FoldersController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        private DateTime? ResolveDate(object dbValue)
        {
            return dbValue == DBNull.Value ? (DateTime?)null : (DateTime)dbValue;
        }

        private int? ResolveNullableInt(object dbValue)
        {
            return dbValue == DBNull.Value ? (int?)null : (int)dbValue;
        }

        #region My Folders
        /* 
        ************ CREATE FOLDERS **************
        This module is to create a new folder for a user. Authorized users
        will be able to create a folders to be assigned to a user.
        */
        [HttpPost("CreateFolder")]
        public IActionResult CreateFolder([FromBody] Folders folder)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(folder.Token, folder.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }

            var physicalPath = string.Empty;
            // Get the current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Combine the current directory with the directory name
            var temp = Path.Combine(currentDirectory, documents);
            physicalPath = Path.Combine(temp, folder.FolderName);

            if (!Directory.Exists(physicalPath))
            {
                // Create the directory if it does not exist
                Directory.CreateDirectory(physicalPath);
                // Set the directory security
                DirectoryInfo dirInfo = new DirectoryInfo(physicalPath);

            }
            else
            {
                return Ok(new
                {
                    message = $"{folder.FolderName} already exists"
                });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("CreateFolder", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ParentFolderId", null);
                        cmd.Parameters.AddWithValue("@Foldername", folder.FolderName);
                        cmd.Parameters.AddWithValue("@Description", "");
                        cmd.Parameters.AddWithValue("@Location", $"Documents/{folder.FolderName}/");
                        cmd.Parameters.AddWithValue("@CreatedByUserId", folder.CreatedByUserId);
                        cmd.Parameters.AddWithValue("@WhoHasAccess", 0);
                        cmd.Parameters.AddWithValue("@VersionGuid", folder.VersionGuid);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{folder.FolderName} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{folder.FolderName} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Creating Folder" });
            }
        }

        [HttpPost("GetFolders")]
        public IActionResult GetFolders([FromBody] Folders folders)
        {
            Utility u = new Utility();
            List<Folders> GetallFolders = new List<Folders>();
            if (!u.VerifyToken(folders.Token, folders.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetFolders", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CreatedByUserId", folders.CreatedByUserId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Folders AllFolders = new Folders();
                                AllFolders.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                AllFolders.ParentFolderId = ResolveNullableInt(reader["ParentFolderId"]);
                                AllFolders.FolderName = reader.GetString(reader.GetOrdinal("FolderName"));
                                AllFolders.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllFolders.DisplayName = reader.IsDBNull(reader.GetOrdinal("CreatedByDisplayName")) ? null : reader.GetString(reader.GetOrdinal("CreatedByDisplayName"));
                                AllFolders.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFolders.ModifiedOnDate = ResolveDate(reader["ModifiedOnDate"]);
                                AllFolders.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId"));
                                AllFolders.ModifiedByUserId = ResolveNullableInt(reader["ModifiedByUserId"]);
                                AllFolders.WhoHasAccess = ResolveNullableInt(reader["WhoHasAccess"]);
                                AllFolders.VersionGuid = reader.GetGuid(reader.GetOrdinal("VersionGuid"));
                                GetallFolders.Add(AllFolders);
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
            return Ok(GetallFolders);
        }

        [HttpPost("ShowAllFolders")]
        public IActionResult ShowAllFolders([FromBody] Folders folders)
        {
            Utility u = new Utility();
            List<Folders> GetallFolders = new List<Folders>();
            if (!u.VerifyToken(folders.Token, folders.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("ShowAllFolders", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Folders AllFolders = new Folders();
                                AllFolders.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                AllFolders.ParentFolderId = ResolveNullableInt(reader["ParentFolderId"]);
                                AllFolders.FolderName = reader.GetString(reader.GetOrdinal("FolderName"));
                                AllFolders.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllFolders.DisplayName = reader.IsDBNull(reader.GetOrdinal("CreatedByDisplayName")) ? null : reader.GetString(reader.GetOrdinal("CreatedByDisplayName"));
                                AllFolders.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFolders.ModifiedOnDate = ResolveDate(reader["ModifiedOnDate"]);
                                AllFolders.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId"));
                                AllFolders.ModifiedByUserId = ResolveNullableInt(reader["ModifiedByUserId"]);
                                AllFolders.WhoHasAccess = ResolveNullableInt(reader["WhoHasAccess"]);
                                AllFolders.VersionGuid = reader.GetGuid(reader.GetOrdinal("VersionGuid"));
                                GetallFolders.Add(AllFolders);
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
            return Ok(GetallFolders);
        }
        #endregion

        #region Team Folders
        /* 
        ************ CREATE TEAM FOLDERS **************
        This module is to create a new team folder for a user. Authorized users
        will be able to create team folders to be assigned to a user.
        */
        [HttpPost("CreateTeamFolder")]
        public IActionResult CreateTeamFolder([FromBody] Folders folder)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(folder.Token, folder.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }

            var physicalPath = string.Empty;
            // Get the current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Combine the current directory with the directory name
            var temp = Path.Combine(currentDirectory, teamdocuments);
            physicalPath = Path.Combine(temp, folder.FolderName);

            if (!Directory.Exists(physicalPath))
            {
                // Create the directory if it does not exist
                Directory.CreateDirectory(physicalPath);
                // Set the directory security
                DirectoryInfo dirInfo = new DirectoryInfo(physicalPath);

            }
            else
            {
                return Ok(new
                {
                    message = $"{folder.FolderName} already exists"
                });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("CreateTeamFolder", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Foldername", folder.FolderName);
                        cmd.Parameters.AddWithValue("@Description", folder.Description);
                        cmd.Parameters.AddWithValue("@Permission", folder.Permission);
                        cmd.Parameters.AddWithValue("@Location", $"Documents/Teams/{folder.FolderName}/");
                        cmd.Parameters.AddWithValue("@CreatedByUserId", folder.CreatedByUserId);
                        cmd.Parameters.AddWithValue("@VersionGuid", folder.VersionGuid);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{folder.FolderName} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{folder.FolderName} creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Creating Team Folder" });
            }
        }

        /* 
       ************ Get TEAM FOLDERS **************
       This module is to get all team folders for users. Authorized users
       will be able to view team folders.
       */
        [HttpPost("GetTeamFolders")]
        public IActionResult GetTeamFolders([FromBody] Folders folders)
        {
            Utility u = new Utility();
            List<Folders> showTeamFolders = new List<Folders>();
            if (!u.VerifyToken(folders.Token, folders.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("ShowTeamFolders", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Folders AllFolders = new Folders();
                                AllFolders.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                AllFolders.Permission = reader.GetString(reader.GetOrdinal("Permission"));
                                AllFolders.FolderName = reader.GetString(reader.GetOrdinal("FolderName"));
                                AllFolders.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllFolders.DisplayName = reader.IsDBNull(reader.GetOrdinal("CreatedByDisplayName")) ? null : reader.GetString(reader.GetOrdinal("CreatedByDisplayName"));
                                AllFolders.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFolders.ModifiedOnDate = ResolveDate(reader["ModifiedOnDate"]);
                                AllFolders.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId"));
                                AllFolders.ModifiedByUserId = ResolveNullableInt(reader["ModifiedByUserId"]);
                                AllFolders.VersionGuid = reader.GetGuid(reader.GetOrdinal("VersionGuid"));
                                showTeamFolders.Add(AllFolders);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Folders", "Error Retrieving Team Folders");
                return BadRequest(ModelState);
            }
            return Ok(showTeamFolders);
        }

        /*
         This module is to get all the Details of a particular Team Folder to display.
        we need to show the details of a folder to the admin if he/she needs to make
        changes
         */
        [HttpPost("GetTeamFolderDetails")]
        public IActionResult GetTeamFolderDetails([FromBody] Folders folders)
        {
            Utility u = new Utility();
            List<Folders> showTeamFolders = new List<Folders>();
            if (!u.VerifyToken(folders.Token, folders.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetTeamFolderDetailById", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", folders.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Folders AllFolders = new Folders();
                                AllFolders.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                AllFolders.Permission = reader.GetString(reader.GetOrdinal("Permission"));
                                AllFolders.FolderName = reader.GetString(reader.GetOrdinal("FolderName"));
                                AllFolders.Description = reader.GetString(reader.GetOrdinal("Description"));
                                AllFolders.Location = reader.GetString(reader.GetOrdinal("Location"));
                                AllFolders.DisplayName = reader.IsDBNull(reader.GetOrdinal("CreatedByDisplayName")) ? null : reader.GetString(reader.GetOrdinal("CreatedByDisplayName"));
                                AllFolders.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFolders.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId"));

                                showTeamFolders.Add(AllFolders);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Folders", "Error Retrieving Team Folders Details");
                return BadRequest(ModelState);
            }
            return Ok(showTeamFolders);
        }

        /*
         This module Gets all the members that have been assigned to a particular folder
        in order to add some level of security and integrity to the folder
         */
        [HttpPost("GetTeamFolderMembers")]
        public IActionResult GetTeamFolderMembers([FromBody] Folders folders)
        {
            Utility u = new Utility();
            List<Folders> showTeamFolders = new List<Folders>();
            if (!u.VerifyToken(folders.Token, folders.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetTeamFolderMembers", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", folders.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Folders AllFolders = new Folders();
                                AllFolders.Id = reader.GetInt32(reader.GetOrdinal("id"));
                                AllFolders.RoleID = reader.GetInt32(reader.GetOrdinal("roleid"));
                                AllFolders.FolderId = reader.GetInt32(reader.GetOrdinal("folderid"));
                                AllFolders.Enabled = reader.GetBoolean(reader.GetOrdinal("enabled"));
                                AllFolders.CreatedByUserId = reader.GetInt32(reader.GetOrdinal("createdByUserId"));
                                AllFolders.DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? null : reader.GetString(reader.GetOrdinal("DisplayName"));
                                AllFolders.RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"));
                                AllFolders.CreatedOnDate = reader.GetDateTime(reader.GetOrdinal("CreatedOnDate"));
                                AllFolders.EmpId = reader.GetInt32(reader.GetOrdinal("EmpId"));
                                showTeamFolders.Add(AllFolders);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Folders", "Error Retrieving Team Folder Members");
                return BadRequest(ModelState);
            }
            return Ok(showTeamFolders);
        }

        /*
         This module adds a member to a folder and gives them permission to access the 
        files in that folder.
         */
        [HttpPost("AddTeamMembersToFolder")]
        public IActionResult AddTeamMembersToFolder([FromBody] Folders[] folders)
        {
            Utility u = new Utility();

            // Verify token for each folder entry
            foreach (var folder in folders)
            {
                if (!u.VerifyToken(folder.Token, folder.SecretKey))
                {
                    return Unauthorized(new { message = "User logged out. Please login again to continue." });
                }
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    foreach (var folder in folders)
                    {
                        using (SqlCommand cmd = new SqlCommand("InsertUserFolder", conn))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@roleid", folder.RoleID);
                            cmd.Parameters.AddWithValue("@userid", folder.EmpId);
                            cmd.Parameters.AddWithValue("@folderid", folder.FolderId);
                            cmd.Parameters.AddWithValue("@CreatedByUserId", folder.CreatedByUserId);
                            cmd.Parameters.AddWithValue("@enabled", 1);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                            {
                                return NotFound(new { message = "Adding user to Team folder failed." });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    message = "User(s) added to Team folder successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding user(s) to Team Folder", error = ex.Message });
            }
        }
        #endregion



    }
}
