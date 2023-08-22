using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.Feedback
{
    public class FeedbackCreateViewModel
    {
        [Required]
        public Guid BagId { get; set; }

        public DateTime Created { get; set; }

        [Required]
        public ushort Evaluation { get; set; }



    }
}
