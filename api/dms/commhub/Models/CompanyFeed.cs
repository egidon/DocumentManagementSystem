namespace commhub.Models
{
    public class CompanyFeed
    {
        public int Id { get; set; }
        public string SayYourMind { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int? WhoCanSee { get; set; }
        public string Tags { get; set; } = string.Empty;
        public bool? IsDeleted { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedOnDate { get; set; }
        public string Token { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string[] FileForUpload { get; set; } = new string[0];

    }

    public class CompanyMedia
    {
        public int Id { get; set; }
        public int? CompanyWallId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public DateTime? CreatedOnDate { get; set; }
        public int? UserId { get; set; }
    }
    public class GetCompanyFeed
    {
        public int Id { get; set; }
        public string SayYourMind { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int? WhoCanSee { get; set; }
        public string Tags { get; set; } = string.Empty;
        public bool? IsDeleted { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
