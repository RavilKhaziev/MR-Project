using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{
	public class DefaultUserModelView
	{
		/// <summary>
		/// Имя пользователя
		/// </summary>
		[Required]
		[Display(Name = "Имя пользователя")]
		public string Name { get; set; } = null!;


		/// <summary>
		/// Почта для регистрации пользователя.
		/// </summary>
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; } = null!;


		/// <summary>
		/// Телефонный номер для регистрации пользователя
		/// </summary>
		[Required]
		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Телефонный номер пользователя для связи")]
		public string PhoneNumber { get; set; } = null!;

		/// <summary>
		/// Пароль пользователя
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; } = null!;

		[Required]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		[DataType(DataType.Password)]
		[Display(Name = "Подтвердить пароль")]
		public string PasswordConfirm { get; set; } = null!;
	}
}
