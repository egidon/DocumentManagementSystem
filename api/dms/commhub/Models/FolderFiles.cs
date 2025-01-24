using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace commhub.Models
{
    public class FolderFiles
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public long Size { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int? FolderId { get; set; }

        public string FolderName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedOnDate { get; set; }

        public int? WhoHasAccess { get; set; }

        public Guid VersionGuid { get; set; } = Guid.NewGuid();

        public string Token { get; set; } = "";

        public string SecretKey { get; set; } = "";

        public string DisplayName { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public IFormFile FileForUpload { get; set; }
    }

    public class DmsFiles
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public long Size { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int? FolderId { get; set; }

        public string FolderName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedOnDate { get; set; }

        public int? WhoHasAccess { get; set; }

        public Guid VersionGuid { get; set; } = Guid.NewGuid();

        public string Token { get; set; } = "";

        public string SecretKey { get; set; } = "";

        public string DisplayName { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
        
    }
}
