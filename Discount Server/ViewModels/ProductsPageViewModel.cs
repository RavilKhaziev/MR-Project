namespace Discount_Server.ViewModels
{
    /// <summary>
    /// Для удобства представления и передачи, продукты разделены на страницы.
    /// </summary>
    public class ProductsPageViewModel
    {
        /// <summary>
        /// Продукты находящиеся на данной странице.
        /// </summary>
        public List<ProductInfoModel> Products { get; set; } = new List<ProductInfoModel>();
        /// <summary>
        /// Информация о запрашиваемой странице.
        /// </summary>
        public PageInfo PageInfo { get; set; } = new PageInfo();
    }
}
