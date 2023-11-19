using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using IdentityServer.Models.Users;

namespace IdentityServer.Models
{
	public class BasketModel
	{
		[Key]
		public int? Id { get; set; }

		public AppUser? User { get; set; } 

		public string Name { get; set; } = null!;
	}
}
