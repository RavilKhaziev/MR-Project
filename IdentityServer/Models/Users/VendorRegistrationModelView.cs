using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{

	public class VendorRegistrationModelView
	{

		[Required]
		[Display(Name = "Имя компании")]
		[DataType(DataType.Text)]
		public string CompanyName { get; set; } = null!;

		
		[Display(Name = "Имя компании")]
		[DataType(DataType.MultilineText)]
		public string? Discritption { get; set; }

		[Required]
		[Display(Name = "Местонаходение заведения")]
		[DataType(DataType.MultilineText)]
		public string Location { get; set; } = null!;

		[Required]
		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
		public string CompanyEmail { get; set; } = null!;

		[Required]
		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Телефонный номер компании для связи")]
		public string PhoneNumber { get; set; } = null!;

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
