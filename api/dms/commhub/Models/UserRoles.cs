namespace commhub.Models
{
    public class UserRoles
    {
        public int UserRoleID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsTrialUsed { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public int Status { get; set; }
        public bool IsOwner { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }
}
