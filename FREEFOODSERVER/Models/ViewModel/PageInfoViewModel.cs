using System.Drawing;

namespace FREEFOODSERVER.Models.ViewModel
{
    public class PageInfoViewModel
    {
        public static readonly int PAGESIZE = 10;

        /// <summary>
        /// номер текущей страницы
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// кол-во объектов на странице
        /// </summary>
        public int PageSize { get; set; } = PAGESIZE;

        /// <summary>
        /// всего объектов
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// всего страниц
        /// </summary>
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}
