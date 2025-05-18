namespace Api.Features.Users.Auth
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;

        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string[] Roles { get; set; } = [];
    }
}
