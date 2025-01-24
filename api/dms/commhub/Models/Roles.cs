namespace commhub.Models
{
    public class Roles
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal? ServiceFee { get; set; }
        public char? BillingFrequency { get; set; }
        public int? TrialPeriod { get; set; }
        public char? TrialFrequency { get; set; }
        public int? BillingPeriod { get; set; }
        public decimal? TrialFee { get; set; }
        public bool IsPublic { get; set; }
        public bool AutoAssignment { get; set; }
        public int? RoleGroupID { get; set; }
        public string RSVPCode { get; set; } = "";
        public string IconFile { get; set; } = "";
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public int? Status { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }
}
