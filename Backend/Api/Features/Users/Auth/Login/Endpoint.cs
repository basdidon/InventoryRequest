using FastEndpoints.Security;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Identity;
using Api.Features.Users.Auth.RefreshToken;
using System.Security.Claims;

namespace Api.Features.Users.Auth.Login
{
    public class Endpoint(IDocumentSession session, PasswordService passwordService) : Endpoint<Request, TokenResponse>
    {
        public override void Configure()
        {
            Post("/user/auth/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var user = await session.Query<ApplicationUser>().Where(x => x.Username == req.Username).SingleOrDefaultAsync(ct);


            if (user is not null)
            {
                var result = passwordService.VerifyPassword(user, req.Password);

                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    var heshed = passwordService.HashPassword(user, req.Password);
                    user.HashedPassword = heshed;
                    session.Update(user);
                    await session.SaveChangesAsync(ct);
                }

                if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    Response = await CreateTokenWith<FastEndpointsTokenService>(user.Id.ToString(), u =>
                    {
                        u.Roles.AddRange(user.Roles);
                        u.Claims.Add(new Claim("UserId", user.Id.ToString()));
                        u.Claims.Add(new Claim("Username", user.Username));
                    });

                    return;
                }
            }

            await SendUnauthorizedAsync(ct);
        }
    }
}
