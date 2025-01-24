using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace commhub.Models
{
    public class Folders
    {
      
        
        public int Id { get; set; }

        public int? ParentFolderId { get; set; }
        public int RoleID { get; set; }
        public int EmpId { get; set; }
        public int FolderId { get; set; }
        public bool Enabled { get; set; }
        public string RoleName { get; set; } = string.Empty;
        // [ForeignKey("ParentFolderId")]
        //public virtual Folder ParentFolder { get; set; }

        //[Required]
        //[StringLength(300)]
        public string FolderName { get; set; } = string.Empty;

        //[StringLength(8000)]
        public string Description { get; set; } = string.Empty;

        //[StringLength(500)]
        public string Location { get; set; } = string.Empty;

        //[Required]
        public int CreatedByUserId { get; set; }

        //[Required]
        public DateTime CreatedOnDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedOnDate { get; set; }

        public int? WhoHasAccess { get; set; }

        //[Required]
        public Guid VersionGuid { get; set; } = Guid.NewGuid();

        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string DisplayName { get; set; } = string.Empty;

        public string Permission { get; set; } = string.Empty;  

    }

    public class TeamMembers
    {
        public int RoleID { get; set; }
        public int EmpId { get; set; }
        public int FolderId { get; set; }
        public int Enabled { get; set; }
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public int CreatedByUserId { get; set; }
    }
}
