namespace FREEFOODSERVER.Models.ViewModel.Product
{
    public class ProductEditViewModel
    {
        public Guid BagId { get; set;}

        public Guid ProductId { get; set;}

        public string? ProductName { get; set; }

        public List<string>? ProductCategories { get; set; }
    }
}
