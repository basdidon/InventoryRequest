namespace Api.Features.Orders
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;

        public bool IsReady { get; set; } = false;
        public bool IsCancled { get; set; } = false;
        public bool IsProcessed => IsReady || IsCancled;
    }

    public enum OrderState
    {
        Pending, 
        Preparing,
        Shipping,
        Cancled,
        Succesful,
    }

    public class Order
    {
        public Guid Id { get; set; }
        public OrderState OrderState { get; set; } = OrderState.Pending;

        public List<OrderItem> OrderItems { get; set; } = [];

        public decimal TotalPrice => OrderItems.Sum(x => x.SubTotal);

        public Guid OrderBy { get; set; }
        public Guid? CancleBy { get; set; }
    }

    public record OrderSubmitted();
    public record OrderAccepted();
    public record OrderItemWasReady();
    public record OrderItemCancled();
    public record OrderShipped(); // when all item was processed
    public record OrderSuccessful();
    public record OrderCancled();

}
