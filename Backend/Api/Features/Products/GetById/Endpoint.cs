using Api.Const;
using FastEndpoints;
using Marten;

namespace Api.Features.Products.GetById
{
    public class Endpoint(IQuerySession session) : Endpoint<Request,Product>
    {
        public override void Configure()
        {
            Get("/products/{ProductId}");
            Roles(ApplicationRole.All);
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var product = await session.LoadAsync<Product>(req.ProductId,ct);

            if(product is null)
            {
                AddError("Product notfound.");
                await SendNotFoundAsync(ct);
            }
            else
            {
                await SendOkAsync(product,ct);
            }

        }
    }
}
