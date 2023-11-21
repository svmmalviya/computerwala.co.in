using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Models
{
	public class CWSubscriptions
	{
		[Key]
		public string Id { get; set; }
		public string Email { get; set; }
		[Column(TypeName = "TIMESTAMP")]

		public DateTime CreatedOn { get; set; }
	}
}
