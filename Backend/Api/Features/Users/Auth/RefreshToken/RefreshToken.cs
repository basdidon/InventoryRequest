namespace Api.Features.Users.Auth.RefreshToken
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public bool IsRevocked { get; set; } = false;

        public DateTime CreateAt { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
