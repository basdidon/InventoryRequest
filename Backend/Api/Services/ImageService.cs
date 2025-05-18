using Api.Settings;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, CancellationToken ct = default);
        Task LoadImageAsync(string filename, Stream output, CancellationToken ct = default);
    }

    public class ImageService : IImageService
    {
        private readonly string _imageStoragePath;

        public ImageService(IOptions<StorageSettings> options)
        {
            var settings = options.Value;
            _imageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), settings.ImagePath);
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, CancellationToken ct = default)
        {
            if (!Directory.Exists(_imageStoragePath))
            {
                Directory.CreateDirectory(_imageStoragePath);
            }

            var fileType = GetImageFileType(imageFile) 
                ?? throw new AggregateException("filetype not supported");
            var filename = $"{Guid.NewGuid()}.{fileType}";
            var filePath = Path.Combine(_imageStoragePath, filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream, ct);
            }

            return filename;
        }

        public async Task LoadImageAsync(string filename, Stream output, CancellationToken ct = default)
        {
            var path = Path.Combine(_imageStoragePath, filename);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {filename}");

            await using var fileStream = File.OpenRead(path);
            await fileStream.CopyToAsync(output, ct);
        }

        private static string? GetImageFileType(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            Span<byte> header = stackalloc byte[8];
            int bytesRead = stream.Read(header);
            if (bytesRead < header.Length)
            {
                throw new InvalidDataException("Unexpected end of stream when reading file header.");
            }

            // PNG Signature: 89 50 4E 47 0D 0A 1A 0A
            if (header.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47 }))
                return "png";

            // JPEG Signature: FF D8 FF
            if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return "jpg";

            return null;
        }
    }
}
