namespace commhub.Models
{
    public class RoleGroup
    {
        public int RoleGroupID { get; set; }
        public string RoleGroupName { get; set; } = "";
        public string Description { get; set; } = "";
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }
}
