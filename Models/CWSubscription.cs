using System.ComponentModel.DataAnnotations;

namespace computerwala.Models
{
	public class CWSubscriptions
	{
		public string? Id { get; set; }
		public string Email { get; set; }
		public DateTime? CreatedOn { get; set; }
	}
}
