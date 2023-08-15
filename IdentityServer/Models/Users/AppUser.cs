using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{
	public class AppUser : IdentityUser
	{
		public List<BasketModel> Baskets { get; set; } = new();

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

	}
}
