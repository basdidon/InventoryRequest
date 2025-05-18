using System.ComponentModel;

namespace Api.Features.Products.List
{
    public class Request
    {
        [DefaultValue(1)]
        public int Page { get; set; } = 1;
        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;
    }
}
