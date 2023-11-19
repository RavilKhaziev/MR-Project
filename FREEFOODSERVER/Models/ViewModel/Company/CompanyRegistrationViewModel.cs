using System.ComponentModel.DataAnnotations;

namespace FREEFOODSERVER.Models.ViewModel.Company
{
    public class CompanyRegistrationViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефонный номер пользователя для связи")]
        public string? PhoneNumber { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердить пароль")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = null!;
    }
}
