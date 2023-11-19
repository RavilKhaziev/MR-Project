using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{
	public class LoginViewModel 
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;


	}
}
