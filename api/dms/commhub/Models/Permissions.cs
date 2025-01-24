namespace commhub.Models
{
    public class Permissions
    {
        public int Id { get; set; }
        public int PermissionID { get; set; }
        public string PermissionCode { get; set; } = "";
        public int ModuleDefID { get; set; }
        public string PermissionKey { get; set; } = "";
        public string PermissionName { get; set; } = "";
        public int ViewOrder { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }
}
