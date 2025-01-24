namespace commhub.Models
{
    public class CompanyManual
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool? IsDeleted { get; set; }
        public string Token { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedOnDate { get; set; }
        //public string[] FileForUpload { get; set; } = new string[0];
        public IFormFile FileForUpload { get; set; }
    }

    public class CompanyManualDocument
    {
        public int Id { get; set; }
        public int? myfeedid { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public DateTime? CreatedOnDate { get; set; }
        public int? UserId { get; set; }
    }

    public class ManualVerification
    {
        public string Token { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int CreatedByUserID { get; set; }
    }

}
