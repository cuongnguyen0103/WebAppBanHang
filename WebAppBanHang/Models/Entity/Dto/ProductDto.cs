namespace WebAppBanHang.Models.Entity.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int DiscountId { get; set; }
        public decimal DiscountPercent { get; set; }
        public int Quantity { get; set; }
    }
}
