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
    public class MyFeedController : ControllerBase
    {
        private readonly string connnectionstring;
        Utility u = new Utility();
        private readonly string mywallPhotos = "Media\\Photos\\mywall";
        private readonly string mywallVideos = "Media\\Videos\\mywall";
        private readonly string[] _extensions = { ".png", ".jpg", ".jpeg", ".webp", ".bmp" };
        private readonly string[] _videoextensions = { ".mov", ".mp4", ".mkv", ".avi", ".wmv", ".flv", ".webm", ".3pg" };
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<MyFeedController> _logger;

        public MyFeedController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, ILogger<MyFeedController> logger)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost("CreateMyFeed")]
        public async Task<IActionResult> CreateMyFeed([FromBody] MyFeed mf)
        {
            var result = new CreateMediaResult();
            if (!u.VerifyToken(mf.Token, mf.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("InsertMyFeed", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@say_your_mind", mf.SayYourMind);
                        cmd.Parameters.AddWithValue("@url", mf.Url);
                        cmd.Parameters.AddWithValue("@who_can_see", mf.WhoCanSee);
                        cmd.Parameters.AddWithValue("@tags", mf.Tags);
                        //cmd.Parameters.AddWithValue("@id", uf.Id);
                        cmd.Parameters.AddWithValue("@isDeleted", 0);
                        cmd.Parameters.AddWithValue("@createdByUserId", mf.CreatedByUserId);

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


                        if (mf.FileForUpload.Length > 0)
                        {
                            result = await UploadFiles(mf, id);
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
                        else {
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
                return StatusCode(500, new { message = "Error Creating Your Post" });
            }
        }

        private async Task<CreateMediaResult> UploadFiles(MyFeed mf, int id)
        {
            CreateMediaResult get_result = new CreateMediaResult();
            byte[] imageBytes = new byte[0];
            var uniqueFileName = "";
            var filePath = "";
            string filename = "";
            for (int i = 0; i < mf.FileForUpload.Length; i++)
            {
                string data = mf.FileForUpload[i];
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
                    var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), mywallPhotos);
                    //var physicalPath = Path.Combine(_hostingEnvironment.WebRootPath, companywallPhotos);
                    filePath = Path.Combine(physicalPath, uniqueFileName); // Combine the folder path and unique file name to create the full file path
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    //insert file record into database
                    Files f = new Files
                    {
                        Id = id,
                        Url = "Media/Photos/mywall/" + uniqueFileName,
                        Filename = uniqueFileName,
                        UserId = mf.CreatedByUserId
                    };
                    get_result = CreateMedia(f);

                }
                if (_videoextensions.Contains(ext.ToLower())) //Check if file passed is an image file
                {
                    uniqueFileName = Guid.NewGuid() + "_" + filename; //Create unique filename
                    var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), mywallVideos);
                    //var physicalPath = Path.Combine(_hostingEnvironment.WebRootPath, companywallVideos);
                    filePath = Path.Combine(physicalPath, uniqueFileName); // Combine the folder path and unique file name to create the full file path
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    //insert file record into database
                    Files f = new Files
                    {
                        Id = id,
                        Url = "Media/Videos/mywall/" + uniqueFileName,
                        Filename = uniqueFileName,
                        UserId = mf.CreatedByUserId
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
                    using (SqlCommand cmd = new SqlCommand("InsertMyMedia", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Myfeedid", f.Id);
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

        [HttpPost("ViewAllFeeds")]
        public IActionResult ViewAllFeeds([FromBody] MyFeed mf)
        {
            List<GetMyFeed> GetAllMyFeed = new List<GetMyFeed>();
            if (!u.VerifyToken(mf.Token, mf.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetMyWallFeed", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                GetMyFeed arg = new GetMyFeed();
                                arg.Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")); arg.SayYourMind = reader.IsDBNull(reader.GetOrdinal("say_your_mind")) ? null : reader.GetString(reader.GetOrdinal("say_your_mind"));
                                arg.WhoCanSee = reader.IsDBNull(reader.GetOrdinal("who_can_see")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("who_can_see"));
                                arg.IsDeleted = reader.IsDBNull(reader.GetOrdinal("isDeleted")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("isDeleted"));
                                arg.CreatedOnDate = reader.IsDBNull(reader.GetOrdinal("createdOnDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("createdOnDate"));
                                arg.DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? null : reader.GetString(reader.GetOrdinal("DisplayName"));
                                arg.CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("createdByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("createdByUserId"));
                                arg.Url = reader.IsDBNull(reader.GetOrdinal("url")) ? null : reader.GetString(reader.GetOrdinal("url"));

                                GetAllMyFeed.Add(arg);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("MyFeed", "Error Retrieving your Feed");
                return BadRequest(ModelState);
            }
            return Ok(GetAllMyFeed);
        }

    }

}
