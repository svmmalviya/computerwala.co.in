using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Models
{
	public class CWVisiteres
	{
		[Key]
		public Guid Id { get; set; }
		public string IpAddress { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
