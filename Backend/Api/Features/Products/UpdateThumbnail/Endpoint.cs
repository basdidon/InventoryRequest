using Api.Services;
using FastEndpoints;
using Marten;

namespace Api.Features.Products.UpdateThumbnail
{
    public class Endpoint(IImageService imageService, IDocumentSession session) : Endpoint<Request,Response>
    {
        public override void Configure()
        {
            Post("/products/{ProductId}/update-thumbnail");
            AllowFileUploads();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            if (Files.Count > 0)
            {
                var stream = await session.Events.FetchForWriting<Product>(req.ProductId, ct);
                if(stream is null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }
                
                var file = Files[0];
                var imageName = await imageService.SaveImageAsync(file,ct);

                stream.AppendOne(new ProductThumbnailUpdated(imageName));
                await session.SaveChangesAsync(ct);
                
                await SendOkAsync(new Response() { ImageUrl = imageName },ct);
                return;
            }

            await SendErrorsAsync(400,ct);
        }
    }
}
