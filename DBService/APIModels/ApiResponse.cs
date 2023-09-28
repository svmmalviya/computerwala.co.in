using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.APIModels
{
	public class ApiResponse
	{
		public string Message { get; set; }
		public bool Success { get; set; }
		public string Data { get; set; }
	}
}
