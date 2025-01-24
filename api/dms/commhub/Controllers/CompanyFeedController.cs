using commhub.Models;
using commhub.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class CompanyFeedController : ControllerBase
    {
        private readonly string connnectionstring;
        Utility u = new Utility();
        private readonly string companywallPhotos = "Media\\Photos\\companywall";
        private readonly string companywallVideos = "Media\\Videos\\companywall";
        private readonly string[] _extensions = { ".png", ".jpg", ".jpeg", ".webp", ".bmp" };
        private readonly string[] _videoextensions = { ".mov", ".mp4", ".mkv", ".avi", ".wmv", ".flv", ".webm", ".3pg" };
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger <CompanyFeedController> _logger;

        public CompanyFeedController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<CompanyFeedController> logger)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost("CreateCompanyFeed")]
        public async Task<IActionResult> CreateCompanyFeed([FromBody] CompanyFeed uf)
        {
            var result = new CreateMediaResult();
            if (!u.VerifyToken(uf.Token, uf.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertCompanyWallFeed", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@say_your_mind", uf.SayYourMind);
                        cmd.Parameters.AddWithValue("@url", uf.Url);
                        cmd.Parameters.AddWithValue("@who_can_see", uf.WhoCanSee);
                        cmd.Parameters.AddWithValue("@tags", uf.Tags);
                        //cmd.Parameters.AddWithValue("@id", uf.Id);
                        cmd.Parameters.AddWithValue("@isDeleted", 0);
                        cmd.Parameters.AddWithValue("@createdByUserId", uf.CreatedByUserId);

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

                        if (uf.FileForUpload.Length > 0)
                        {
                            result = await UploadFiles(uf, id);
                            if (result.StatusCode == 200)
                            {
                                return Ok(new
                                {
                                    message = $"post created successfully"
                                });
                            }
                            else
                            {
                                return StatusCode(500, new { message = "Error Creating Your Post" });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                message = $"post created successfully"
                            });
                        }

                    }
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Creating Company Post" });
            }
        }

        private async Task<CreateMediaResult> UploadFiles(CompanyFeed uf, int id)
        {
            CreateMediaResult get_result = new CreateMediaResult();
            byte[] imageBytes = new byte[0];
            var uniqueFileName = "";
            var filePath = "";
            string filename = "";
            for (int i = 0; i < uf.FileForUpload.Length; i++)
            {
                string data = uf.FileForUpload[i];
                if (data.StartsWith("data:image"))
                {
                    var commaIndex = data.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        data = data.Substring(commaIndex + 1);
                    }
                    filename = GenerateRandomString(15) + ".jpeg";
                }
                if (data.StartsWith("data:video"))
                {
                    var commaIndex = data.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        data = data.Substring(commaIndex + 1);
                    }
                    filename = GenerateRandomString(15) + ".mp4";
                }

                imageBytes = Convert.FromBase64String(data);
                var ext = Path.GetExtension(filename);

                //Write file to video or phots folder after verifying
                if (_extensions.Contains(ext.ToLower())) //Check if file passed is an image file
                {
                    uniqueFileName = Guid.NewGuid() + "_" + filename; //Create unique filename
                    //var k = Path.Combine(Directory.GetCurrentDirectory() + "\\Media\\Photos\\companywall", uniqueFileName);
                    var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), companywallPhotos);
                    //var physicalPath = Path.Combine(_hostingEnvironment.WebRootPath, companywallPhotos);
                    filePath = Path.Combine(physicalPath, uniqueFileName); // Combine the folder path and unique file name to create the full file path
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    //insert file record into database
                    Files f = new Files
                    {
                        Id = id,
                        Url = "Media/Photos/companywall/"+ uniqueFileName,
                        Filename = uniqueFileName,
                        UserId = uf.CreatedByUserId
                    };
                    get_result = CreateMedia(f);

                }
                if (_videoextensions.Contains(ext.ToLower())) //Check if file passed is an image file
                {
                    uniqueFileName = Guid.NewGuid() + "_" + filename; //Create unique filename
                    var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), companywallVideos);
                    //var physicalPath = Path.Combine(_hostingEnvironment.WebRootPath, companywallVideos);
                    filePath = Path.Combine(physicalPath, uniqueFileName); // Combine the folder path and unique file name to create the full file path
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    //insert file record into database
                    Files f = new Files
                    {
                        Id = id,
                        Url = "Media/Videos/companywall/" + uniqueFileName,
                        Filename = uniqueFileName,
                        UserId = uf.CreatedByUserId
                    };
                    get_result = CreateMedia(f);
                }
            }
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
                    using (SqlCommand cmd = new SqlCommand("InsertCompanyMedia", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyWallID", f.Id);
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

        private string GenerateRandomString(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder randomString = new StringBuilder();

            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(0, characters.Length);
                randomString.Append(characters[index]);
            }
            //if (verifyGenerateRandomString(randomString.ToString()) == "failed")
            //{
            //    GenerateRandomString(length);
            //}
            return randomString.ToString();
        }

        [HttpPost("ViewCompanyFeeds")]
        public IActionResult ViewCompanyFeeds([FromBody] CompanyFeed uf)
        {
            List<GetCompanyFeed> GetAllCompanyFeed = new List<GetCompanyFeed>();
            if (!u.VerifyToken(uf.Token, uf.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try 
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetCompanyWallFeed", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                GetCompanyFeed arg = new GetCompanyFeed();
                                arg.Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")); arg.SayYourMind = reader.IsDBNull(reader.GetOrdinal("say_your_mind")) ? null : reader.GetString(reader.GetOrdinal("say_your_mind"));
                                arg.WhoCanSee = reader.IsDBNull(reader.GetOrdinal("who_can_see")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("who_can_see"));
                                arg.IsDeleted = reader.IsDBNull(reader.GetOrdinal("isDeleted")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("isDeleted"));
                                arg.CreatedOnDate = reader.IsDBNull(reader.GetOrdinal("createdOnDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("createdOnDate"));
                                arg.DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? null : reader.GetString(reader.GetOrdinal("DisplayName"));
                                arg.CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("createdByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("createdByUserId"));
                                arg.Url = reader.IsDBNull(reader.GetOrdinal("url")) ? null : reader.GetString(reader.GetOrdinal("url"));

                                GetAllCompanyFeed.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("CompanyFeed", "Error Retrieving Company Feed");
                return BadRequest(ModelState);
            }
            return Ok(GetAllCompanyFeed);
        }
    }

    public class Files
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }

    public class CreateMediaResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}


