using Api.Const;
using Api.Services;
using FastEndpoints;
using Marten;

namespace Api.Features.Products.Create
{
    public class Endpoint(IImageService imageService, IDocumentSession session) : Endpoint<Request>
    {
        public override void Configure()
        {
            Post("/products");
            AllowFormData();
            Roles(ApplicationRole.ProductManager);
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            string? thumbnailUrl = null;
            if (req.Thumbnail != null && req.Thumbnail.Length > 0)
            {
                thumbnailUrl = await imageService.SaveImageAsync(req.Thumbnail, ct);
            }

            var barcode = req.Barcode ?? "1234567890123";

            ProductCreated @event = new(barcode, req.Name, req.Cost, req.DefaultPrice, thumbnailUrl);
            var id = session.Events.StartStream<Product>(@event).Id;
            await session.SaveChangesAsync(ct);

            await SendCreatedAtAsync<GetById.Endpoint>(new {ProductId=id},cancellation:ct);
        }
    }
}
