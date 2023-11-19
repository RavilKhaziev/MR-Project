namespace Discount_Server
{
    /// <summary>
    /// Класс для представления текущей информации о странице. 
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// Номер текущей страницы.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Количество объектов на странице. 
        /// </summary>
        public int PageSize { get; set; } 
        
        /// <summary>
        /// Всего объектов.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Всего страниц.
        /// </summary>
        public int TotalPages 
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}
