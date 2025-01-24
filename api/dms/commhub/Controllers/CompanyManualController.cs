using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using commhub.Models;
using commhub.Utilities;
using Microsoft.AspNetCore.Cors;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http.HttpResults;


namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class CompanyManualController : ControllerBase
    {
        private readonly string connnectionstring;
        Utility u = new Utility();
        private readonly string companymanualpdf = "Documents\\CompanyManual\\pdf";
        private readonly string companymanualmsexcel = "Documents\\CompanyManual\\msexcel";
        private readonly string companymanualmsword = "Documents\\CompanyManual\\msword";
        private readonly string companymanualppt = "Documents\\CompanyManual\\msppt";
        private readonly string companymanualothers = "Documents\\CompanyManual\\others";

        private readonly string[] _manualext = { ".pdf", ".xlsx", ".xls", ".docx", ".doc", ".pptx", "ppt" };
        private readonly string[] _extensions = { ".png", ".jpg", ".jpeg", ".webp", ".bmp" };
        private readonly string[] _videoextensions = { ".mov", ".mp4", ".mkv", ".avi", ".wmv", ".flv", ".webm", ".3pg" };
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<CompanyManualController> _logger;

        public CompanyManualController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<CompanyManualController> logger)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost("CreateCompanyManual")]
        public async Task<IActionResult> CreateCompanyManual([FromForm] CompanyManual cm)
        {
            var result = new CreateMediaResult();
            if (!u.VerifyToken(cm.Token, cm.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("CreateCompanyManual", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Title", cm.Title);
                        cmd.Parameters.AddWithValue("@Description", cm.Description);
                        cmd.Parameters.AddWithValue("@IsDeleted", 0);
                        cmd.Parameters.AddWithValue("@CreatedByUserId", cm.CreatedByUserId);

                        // Add the output parameter
                        SqlParameter outputIdParam = new SqlParameter("@id", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdParam);

                        // Execute the command
                        cmd.ExecuteNonQuery();

                        // Retrieve the output parameter value
                        int id = (int)outputIdParam.Value;


                        if (cm.FileForUpload.Length > 0)
                        {
                            result = await UploadFiles(cm, id);
                        }
                        if (result.StatusCode == 200)
                        {
                            return Ok(new
                            {
                                message = $"{cm.Title} created successfully"
                            });
                        }
                        else
                        {
                            return StatusCode(500, new { message = $"Error Creating {cm.Title}" });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error Creating {cm.Title}" });
            }
        }

        private async Task<CreateMediaResult> UploadFiles(CompanyManual cm, int id)
        {
            CreateMediaResult get_result = new CreateMediaResult();
            
            string filename = "";
            var fileExt = "";
            var physicalPath = string.Empty;

            filename = Path.GetFileName(cm.FileForUpload.FileName);
            fileExt = Path.GetExtension(cm.FileForUpload.FileName);

            switch (fileExt.ToLower())
                {
                    case ".docx":
                    case ".doc":
                        physicalPath = Path.Combine(Directory.GetCurrentDirectory(), companymanualmsword, filename);
                        break;
                    case ".xlsx":
                    case ".xls":
                        physicalPath = Path.Combine(Directory.GetCurrentDirectory(), companymanualmsexcel, filename);
                        break;
                    case ".pdf":
                        physicalPath = Path.Combine(Directory.GetCurrentDirectory(), companymanualpdf, filename);
                        break;
                    case ".pptx":
                    case ".ppt":
                        physicalPath = Path.Combine(Directory.GetCurrentDirectory(), companymanualppt, filename);
                        break;
                    default:
                    throw new InvalidOperationException("Unsupported file type.");
                }
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await cm.FileForUpload.CopyToAsync(stream);
            }
            
            var directory = Path.GetDirectoryName(physicalPath); // This gets the directory path without the file name
            var specificDirectory = Path.GetFileName(directory);
            
            Files f = new Files
                {
                    Id = id,
                    Url = $"Documents/CompanyManual/{specificDirectory}/" + filename,
                    Filename = filename,
                    UserId = cm.CreatedByUserId
                };

                get_result = CreateMedia(f);

            return get_result;
        }

        private CreateMediaResult CreateMedia(Files f)
        {
            int returnValue;
            var result = new CreateMediaResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertCompanyManualDocument", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Companymanualid", f.Id);
                        cmd.Parameters.AddWithValue("@URL", f.Url);
                        cmd.Parameters.AddWithValue("@FileName", f.Filename);
                        cmd.Parameters.AddWithValue("@userID", f.UserId);

                        // Add the return value parameter
                        SqlParameter returnParameter = new SqlParameter("@return_value", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(returnParameter);
                        // Execute the command
                        cmd.ExecuteNonQuery();

                        // Retrieve the output parameter value
                        returnValue = (int)cmd.Parameters["@return_value"].Value;
                        if (returnValue > 0)
                        {
                            result.Success = true;
                            result.Message = $"{f.Filename} created successfully";
                            result.StatusCode = 200;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Error";
                            result.StatusCode = 500;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = $"Error Creating {f.Filename}: " + e.Message;
                result.StatusCode = 500;
            }
            return result;
        }

        [HttpPost("ViewCompanyManuals")]
        public IActionResult ViewCompanyManuals([FromBody] ManualVerification cm)
        {
            List<CompanyManual> GetAllCompanyManuals = new List<CompanyManual>();
            if (!u.VerifyToken(cm.Token, cm.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetCompanyManual", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                CompanyManual arg = new CompanyManual();
                                arg.Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")); 
                                arg.Title = reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString(reader.GetOrdinal("title"));
                                arg.Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description"));
                                arg.IsDeleted = reader.IsDBNull(reader.GetOrdinal("isDeleted")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("isDeleted"));
                                arg.CreatedOnDate = reader.IsDBNull(reader.GetOrdinal("createdOnDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("createdOnDate"));
                                arg.CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("createdByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("createdByUserId"));
                                arg.Url = reader.IsDBNull(reader.GetOrdinal("url")) ? null : reader.GetString(reader.GetOrdinal("url"));

                                GetAllCompanyManuals.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("CompanyManual", "Error Retrieving Company Manual");
                return BadRequest(ModelState);
            }
            return Ok(GetAllCompanyManuals);
        }

    }
}
