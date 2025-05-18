using Api.Const;
using Api.Features.Users.Auth;
using Marten;

namespace Api.Extensions
{
    public static class SeedDataExtensions
    {
        public static async Task SeedDb(this IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
            var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
            var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

            await store.Advanced.ResetAllData();

            ApplicationUser user = new()
            {
                Username = "productManager",
                Firstname = "username",
                Lastname = "lastname",
                PhoneNumber = "xxx-xxxxxxx",
                Roles = [ApplicationRole.ProductManager]  // add role to array
            };
            user.HashedPassword = passwordService.HashPassword(user, "productManager");
            session.Insert(user);
            await session.SaveChangesAsync();
        }
    }
}
