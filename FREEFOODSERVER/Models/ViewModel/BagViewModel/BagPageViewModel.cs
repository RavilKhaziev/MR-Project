using System.Drawing;

namespace FREEFOODSERVER.Models.ViewModel.BagViewModel
{
    public class BagPageViewModel
    {
        public static readonly int PAGESIZE = 10;

        /// <summary>
        /// номер текущей страницы
        /// </summary>
        public uint PageNumber { get; set; }
        /// <summary>
        /// кол-во объектов на странице
        /// </summary>
        public uint PageSize { get; set; }

        /// <summary>
        /// всего объектов
        /// </summary>
        public uint TotalItems { get; set; }
        /// <summary>
        /// всего страниц
        /// </summary>
        public uint TotalPages  
        {
            get { return (uint)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}
