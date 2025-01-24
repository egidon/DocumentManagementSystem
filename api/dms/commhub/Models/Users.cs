namespace commhub.Models
{
    public class Users
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsSuperUser { get; set; }
        public string Email { get; set; } = "";
        public string Token { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string Password { get; set; } = "";
        public int EmpId { get; set; }
        public string DisplayName { get; set; } = "";
        public string LastIPAddress { get; set; } = "";
        public bool IsDeleted { get; set; }
        public int? CreatedByUserID { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserID { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public Guid? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiration { get; set; }
    }
}
