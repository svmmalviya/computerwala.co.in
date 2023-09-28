using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.AppContext
{
	public class DapperContext
	{
		private readonly IConfiguration _configuration;
		private readonly string _connectionString;

		public DapperContext(IConfiguration configuration)
		{
			_configuration = configuration;

			var db = _configuration["dbtype"];
			if (db == "mssql")
			{
				_connectionString = _configuration.GetConnectionString("SqlConnection");
			}
			else
				_connectionString = _configuration.GetConnectionString("MySqlConnection");
		}
		public IDbConnection CreateConnection()
		{
			var db = _configuration["dbtype"];
			if (db == "mssql")
				return new SqlConnection(_connectionString);
			else if (db == "mysql")
				return new MySqlConnection(_connectionString);
			else
				return new SqlConnection(_connectionString);

		}
	}
}
