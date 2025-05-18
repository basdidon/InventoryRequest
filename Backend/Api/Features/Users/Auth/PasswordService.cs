using Microsoft.AspNetCore.Identity;

namespace Api.Features.Users.Auth
{
    public class PasswordService(IPasswordHasher<ApplicationUser> hasher)
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return hasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyPassword(ApplicationUser user, string providedPassword)
        {
            return hasher.VerifyHashedPassword(user, user.HashedPassword, providedPassword);
        }
    }
}
