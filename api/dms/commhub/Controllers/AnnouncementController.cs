using commhub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using commhub.Utilities;
using System.Data;

namespace commhub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly string connnectionstring;
        public AnnouncementController(IConfiguration configuration)
        {
            connnectionstring = configuration["ConnectionStrings:SqlServerdb"] ?? string.Empty;
        }

        [HttpPost("CreateAnnouncement")]
        public IActionResult CreateAnnouncement([FromBody] Announcement announcement)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(announcement.Token, announcement.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                    string querySelect = @"
                        INSERT INTO Announcement(title, content, isVisible, isDeleted, createdOnDate, modifiedOnDate, createdByUserId, modifiedByUserId)
                        VALUES(@title, @content, 1, 0, @createdOnDate, '', @userId, @userId)
                    ";

                    using (SqlCommand cmd = new SqlCommand(querySelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", announcement.Title);
                        cmd.Parameters.AddWithValue("@content", announcement.Content);
                        cmd.Parameters.AddWithValue("@createdOnDate", formattedDate);
                        cmd.Parameters.AddWithValue("@userId", announcement.UserId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Announcement created successfully" });
                        }
                        else
                        {
                            return NotFound(new { message = "Announcement creation failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Creating Announcement" });
            }
        }

        [HttpPost("ViewAnnouncements")]
        public IActionResult ViewAnnouncements([FromBody] Announcement announcement)
        {
            Utility u = new Utility();
            List<Announcement> ann = new List<Announcement>();
            if (!u.VerifyToken(announcement.Token, announcement.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string sqlGet = "SELECT id,title,content,isVisible,isDeleted,createdOnDate," +
                        " modifiedOnDate, createdByUserId, modifiedByUserId FROM" +
                        " Announcement WHERE isDELETED <> 1";
                    using (var command = new SqlCommand(sqlGet, conn))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Announcement announce = new Announcement();
                                announce.Id = reader.GetInt32(0);
                                announce.Title = reader.GetString(1);
                                announce.Content = reader.GetString(2);
                                announce.IsVisible = reader.GetBoolean(3);
                                announce.IsDeleted = reader.GetBoolean(4);
                                announce.CreatedOnDate = reader.GetDateTime(5);
                                announce.LastModifiedOnDate = reader.GetDateTime(6);
                                announce.CreatedByUserID = reader.GetInt32(7);
                                announce.LastModifiedByUserID = reader.GetInt32(8);
                                
                                ann.Add(announce);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Announcement", "No Announcement not found");
                return BadRequest(ModelState);
            }
            return Ok(ann);
        }

        [HttpPost("ViewAnnouncement")]
        public IActionResult ViewAnnouncement([FromBody] Announcement announcement)
        {
            Utility u = new Utility();
            List<Announcement> ann = new List<Announcement>();
            if (!u.VerifyToken(announcement.Token, announcement.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (var conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string sqlGet = "SELECT id,title,content,isVisible,isDeleted,createdOnDate," +
                        " modifiedOnDate, createdByUserId, modifiedByUserId FROM" +
                        " Announcement WHERE isDELETED <> 1 AND id = @id";
                    using (var command = new SqlCommand(sqlGet, conn))
                    {
                        command.Parameters.AddWithValue("@id", announcement.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Announcement announce = new Announcement();
                                announce.Id = reader.GetInt32(0);
                                announce.Title = reader.GetString(1);
                                announce.Content = reader.GetString(2);
                                announce.IsVisible = reader.GetBoolean(3);
                                announce.IsDeleted = reader.GetBoolean(4);
                                announce.CreatedOnDate = reader.GetDateTime(5);
                                announce.LastModifiedOnDate = reader.GetDateTime(6);
                                announce.CreatedByUserID = reader.GetInt32(7);
                                announce.LastModifiedByUserID = reader.GetInt32(8);

                                ann.Add(announce);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Announcement", "No Announcement not found");
                return BadRequest(ModelState);
            }
            return Ok(ann);
        }

        [HttpPut("UpdateAnnouncement")]
        public IActionResult UpdateAnnouncement([FromBody] Announcement announcement)
        {
            Utility u = new Utility();
            if (!u.VerifyToken(announcement.Token, announcement.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("UpdateAnnouncement", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", announcement.Id);
                        cmd.Parameters.AddWithValue("@title", announcement.Title);
                        cmd.Parameters.AddWithValue("@content", announcement.Content);
                        cmd.Parameters.AddWithValue("@isVisible", announcement.IsVisible);
                        cmd.Parameters.AddWithValue("@isDeleted", announcement.IsDeleted);
                        cmd.Parameters.AddWithValue("@modifiedByUserId", announcement.LastModifiedByUserID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{announcement.Title} updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{announcement.Title} update failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating {announcement.Title}" });
            }
        }

        [HttpPost("DeleteAnnouncement")]
        public IActionResult DeleteAnnouncement([FromBody] Announcement announcement) 
        {
            Utility u = new Utility();
            if (!u.VerifyToken(announcement.Token, announcement.SecretKey))
            {
                return Unauthorized(new { message = "User logged out. Please login again to continue." });
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    conn.Open();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    using (SqlCommand cmd = new SqlCommand("DeleteAnnouncement", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", announcement.Id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new
                            {
                                message = $"{announcement.Title} deleted successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new { message = $"{announcement.Title} deletion failed." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $" {announcement.Title}" });
            }

        }


    }
}
