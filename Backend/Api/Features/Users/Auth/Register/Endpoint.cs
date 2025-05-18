using FastEndpoints;
using Marten;

namespace Api.Features.Users.Auth.Register
{
    public class Endpoint(IDocumentSession session, PasswordService passwordService) : Endpoint<Request>
    {

        public override void Configure()
        {
            Post("user/auth/register");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var isUsernameTaken = await session.Query<ApplicationUser>().AnyAsync(x => x.Username == req.Username, token: ct);

            if (isUsernameTaken)
            {
                AddError(x => x.Username, "username was already taken.");
                await SendErrorsAsync(cancellation: ct);
                return;
            }



            ApplicationUser user = new()
            {
                Username = req.Username,
                Firstname = req.Firstname,
                Lastname = req.Lastname,
                PhoneNumber = req.PhoneNumber,
            };

            user.HashedPassword = passwordService.HashPassword(user, req.Password);
            session.Insert(user);
            await session.SaveChangesAsync(ct);
            await SendOkAsync(ct);
        }

    }
}
