namespace commhub.Models
{
    public class ModulePermission
    {
        public int ModulePermissionID { get; set; }
        public int ModuleDefID { get; set; }
        public int ModuleID { get; set; }
        public int PermissionID { get; set; }
        public bool AllowAccess { get; set; }
        public int? RoleID { get; set; }
        public int? UserID { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public int PortalID { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }
}
