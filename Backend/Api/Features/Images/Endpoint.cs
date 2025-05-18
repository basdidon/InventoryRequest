using Api.Const;
using Api.Settings;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Api.Features.Images
{
    public class Endpoint(IOptions<StorageSettings> options) : Endpoint<Request>
    {
        public override void Configure()
        {
            Get("/images/{ImageUrl}");
            Roles(ApplicationRole.All);
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {            
            var path = Path.Combine(options.Value.ImagePath, req.ImageUrl);

            if (!System.IO.File.Exists(path))
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var stream = System.IO.File.OpenRead(path);
            await SendStreamAsync(stream, req.ImageUrl, stream.Length, GetContentType(req.ImageUrl), cancellation: ct);

            /*
            try
            {
                HttpContext.MarkResponseStart();
                HttpContext.Response.StatusCode = 200;
                HttpContext.Response.ContentType = "image/jpeg";
                await imageService.LoadImageAsync(req.ImageUrl.ToString(), HttpContext.Response.Body, ct);
            }
            catch (Exception)
            {
                await SendNotFoundAsync(ct);
            }*/
        }

        private static string GetContentType(string fileName)
        {
            return fileName.EndsWith(".png") ? "image/png" :
                   fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" :
                   "application/octet-stream";
        }
    }
}
