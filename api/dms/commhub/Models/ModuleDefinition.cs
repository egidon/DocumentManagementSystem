namespace commhub.Models
{
    public class ModuleDefinition
    {
        public int ModuleDefID { get; set; }
        public string FriendlyName { get; set; } = string.Empty;
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public string DefinitionName { get; set; } = string.Empty;
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }

}
