namespace FREEFOODSERVER.Models.ViewModel
{
    public class OutIndexPageViewModel<T> 
    {
        /// <summary>
        /// Информация по странице
        /// </summary>
        public PageInfoViewModel PageInfo { get; set; } = null!;

        /// <summary>
        /// Информация принадлежащая странице
        /// </summary>
        public List<T> Items { get; set; } = null!;
    }
}
