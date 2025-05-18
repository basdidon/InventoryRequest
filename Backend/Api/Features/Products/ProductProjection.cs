using Marten.Events.Aggregation;

namespace Api.Features.Products
{
    public record ProductCreated(string Barcode, string Name, decimal Cost, decimal DefaultPrice, string? ThumbnailUrl);
    public record ProductModified(string Barcode, string Name, decimal Cost, decimal DefaultPrice);
    public record ProductThumbnailUpdated(string thumbnailUrl);
    public record ProductDeleted();

    public record ProductSalePriceAdded(Guid BranchId, decimal SalePrice);
    public record ProductSalePriceModified(Guid BranchId, decimal SalePrice);
    public record ProductSalePriceRemoved(Guid BranchId);
    public record ProductSalePriceCleared();

    public class ProductProjection : SingleStreamProjection<Product>
    {
        public static Product Create(ProductCreated @event)
        => new()
        {
            Barcode = @event.Barcode,
            Name = @event.Name,
            Cost = @event.Cost,
            DefaultPrice = @event.DefaultPrice,
            ThumbnailUrl = @event.ThumbnailUrl,
        };

        public static void Apply(ProductModified @event, Product product)
        {
            product.Barcode = @event.Barcode;
            product.Name = @event.Name;
            product.Cost = @event.Cost;
            product.DefaultPrice = @event.DefaultPrice;
        }

        public static void Apply(ProductThumbnailUpdated @event,Product product)
        {
            product.ThumbnailUrl = @event.thumbnailUrl;
        }

        public static void Apply(ProductSalePriceAdded @event, Product product)
        {
            product.SalePrices.Add(@event.BranchId, @event.SalePrice);
        }

        public static void Apply(ProductSalePriceModified @event, Product product)
        {
            product.SalePrices[@event.BranchId] = @event.SalePrice;
        }

        public static void Apply(ProductSalePriceRemoved @event, Product product)
        {
            product.SalePrices.Remove(@event.BranchId);
        }

        public static void Apply(ProductSalePriceCleared _, Product product)
        {
            product.SalePrices.Clear();
        }
    }
}
