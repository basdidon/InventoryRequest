using FastEndpoints.Security;
using FastEndpoints;
using Marten;
using FastEndpoints.Swagger;

namespace Api.Features.Users.Auth.RefreshToken
{
    public class FastEndpointsTokenService : RefreshTokenService<TokenRequest, TokenResponse>
    {
        IDocumentSession Session { get; }

        public FastEndpointsTokenService(IDocumentSession session)
        {
            Session = session;

            Setup(x =>
            {
                x.AccessTokenValidity = TimeSpan.FromMinutes(30);
                x.RefreshTokenValidity = TimeSpan.FromDays(7);
                x.Endpoint("/user/auth/refresh-token", ep =>
                {
                    ep.Summary(s => s.Description = "this is the refresh token endpoint");
                    ep.Description(d => d.AutoTagOverride("User"));
                });

            });
        }

        public override async Task PersistTokenAsync(TokenResponse response)
        {
            if (!Guid.TryParse(response.UserId, out Guid userIdAsGuid))
                throw new ArgumentException("user id is not GUID");

            Session.Store(new RefreshToken()
            {
                Token = response.RefreshToken,
                UserId = userIdAsGuid,
                CreateAt = DateTime.UtcNow,
                ExpiryTime = response.RefreshExpiry,
            });

            await Session.SaveChangesAsync();
        }

        public override async Task RefreshRequestValidationAsync(TokenRequest req)
        {
            var refreshToken = await Session.Query<RefreshToken>()
                .Where(x => x.Token == req.RefreshToken)
                .OrderBy(x => x.CreateAt)
                .FirstOrDefaultAsync();

            if (refreshToken is null)
                AddError(r => r.RefreshToken, "Refresh token is invalid!");
        }

        public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
        {
            var user = await Session.Query<ApplicationUser>()
                .Where(x => x.Username == request.UserId)
                .FirstOrDefaultAsync() ?? throw new Exception($"user with id {request.UserId} is notfound");

            var roles = user.Roles;
            privileges.Claims.Add(new("UserId", user.Id.ToString()));
            privileges.Roles.AddRange(roles);
        }
    }
}
