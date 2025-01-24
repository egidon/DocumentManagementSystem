namespace commhub.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public bool IsDeleted {  get; set; }
        public bool IsVisible { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }
}
