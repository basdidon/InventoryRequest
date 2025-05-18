namespace Api.Features.Products.Create
{
    public class Request
    {
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal DefaultPrice { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
