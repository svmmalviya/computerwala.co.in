using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace computerwala.Models
{
	public class CWViewSubscriptions
	{
		public string? Id { get; set; }
		[Remote(action: "IsEmailInUser", controller: "Home")]
		public string Email { get; set; }
		public DateTime? CreatedOn { get; set; }
	}
}
