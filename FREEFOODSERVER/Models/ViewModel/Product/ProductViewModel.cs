namespace FREEFOODSERVER.Models.ViewModel.Product
{
    public class ProductViewModel
    {
        public Guid ProductId { get; set; }

        public string? ProductName { get; set; }

        public List<string>? ProductCategories { get; set; }

        public static implicit operator ProductViewModel(Models.Product model)
        {
            return new()
            {
                ProductCategories = model.Categories,
                ProductName = model.Name,
                ProductId = model.Id
            };
        }
    }
}
