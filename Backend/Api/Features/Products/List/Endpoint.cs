using Api.Const;
using FastEndpoints;
using Marten;
using Marten.Pagination;

namespace Api.Features.Products.List
{
    public class Endpoint(IQuerySession session) : Endpoint<Request>
    {
        public override void Configure()
        {
            Get("/products");
            Roles(ApplicationRole.All);
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var products = await session.Query<Product>()
                .ToPagedListAsync(req.Page, req.PageSize, ct);
            await SendOkAsync(products, ct);
        }
    }
}
