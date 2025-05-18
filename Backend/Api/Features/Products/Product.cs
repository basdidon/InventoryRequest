namespace Api.Features.Products
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public decimal Cost { get; set; }
        public decimal DefaultPrice { get; set; }
        public Dictionary<Guid, decimal> SalePrices { get; set; } = [];

        public string? ThumbnailUrl { get; set; }
        //public List<string> ImageUrls { get; set; } = [];
    }
}
