using commhub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using commhub.Utilities;
using Microsoft.AspNetCore.Cors;

namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class LoginUserController : ControllerBase
    {
        private readonly string connnectionstring;
        public LoginUserController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] Users users)
        {
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    
                    using (var command = new SqlCommand("AddUser", conn))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@username", users.Username);
                        command.Parameters.AddWithValue("@firstname", users.FirstName);
                        command.Parameters.AddWithValue("@lastname", users.LastName);
                        command.Parameters.AddWithValue("@issuperuser", 0);
                        command.Parameters.AddWithValue("@email", users.Email);
                        command.Parameters.AddWithValue("@password", users.Password);
                        command.Parameters.AddWithValue("@empid", users.EmpId);
                        command.Parameters.AddWithValue("@displayname", users.DisplayName);
                        command.Parameters.AddWithValue("@isdeleted", 0);
                        command.Parameters.AddWithValue("@createdbyuserid", users.CreatedByUserID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{users.DisplayName} created successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{users.DisplayName} creation failed." });
                        }
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("User", "User insertion failed");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("Getuser")]
        public IActionResult GetUser([FromBody] Users users)
        {
            List<Users> usr = new List<Users>();
            Utilities.Utility u = new Utilities.Utility();
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string sqlGet = "SELECT UserId, Username,FirstName," +
                        "LastName,IsSuperUser, Email, EmpId," +
                        "DisplayName, IsDeleted,CreatedOnDate, CreatedByUserID FROM Users WHERE Email = @email " +
                        "AND Password =@password";
                    using (var command = new SqlCommand(sqlGet, conn))
                    {
                        command.Parameters.AddWithValue("@email", users.Email);
                        command.Parameters.AddWithValue("@password", users.Password);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Users user = new Users();
                                user.UserId = reader.GetInt32("UserId");
                                user.Username = reader.GetString("Username");
                                user.FirstName = reader.GetString("FirstName");
                                user.LastName = reader.GetString("LastName");
                                user.IsSuperUser = reader.GetBoolean("IsSuperUser");
                                user.Email = reader.GetString("Email");
                                user.EmpId = reader.GetInt32("EmpId");
                                user.DisplayName = reader.GetString("DisplayName");
                                user.IsDeleted = reader.GetBoolean("IsDeleted");
                                user.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                user.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                (string token, string secretKey) = u.CreateToken(user.EmpId, connnectionstring).Result;
                                user.SecretKey = secretKey;
                                user.Token = token;
                                
                                usr.Add(user);
                            }
                        }
                    }
                }
                return Ok(usr);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("User", "User not found");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("Getusers")]
        public IActionResult GetUsers([FromBody] Users users)
        {
            List<Users> usr = new List<Users>();
            Utilities.Utility u = new Utilities.Utility();
            if (!u.VerifyToken(users.Token, users.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    using (var command = new SqlCommand("GetUsers", conn))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Users user = new Users();
                                user.Id = reader.GetInt32("UserId");
                                user.Username = reader.GetString("Username");
                                user.FirstName = reader.GetString("FirstName");
                                user.LastName = reader.GetString("LastName");
                                user.IsSuperUser = reader.GetBoolean("IsSuperUser");
                                user.Email = reader.GetString("Email");
                                user.EmpId = reader.GetInt32("EmpId");
                                user.DisplayName = reader.GetString("DisplayName");
                                user.IsDeleted = reader.GetBoolean("IsDeleted");
                                user.CreatedOnDate = reader.GetDateTime("CreatedOnDate");
                                user.CreatedByUserID = reader.GetInt32("CreatedByUserID");
                                (string token, string secretKey) = u.CreateToken(user.EmpId, connnectionstring).Result;
                                user.SecretKey = secretKey;
                                user.Token = token;

                                usr.Add(user);
                            }
                        }
                    }
                }
                return Ok(usr);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Users", "Users not found");
                return BadRequest(ModelState);
            }
        }
    }
}
