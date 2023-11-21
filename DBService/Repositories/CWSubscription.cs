using Dapper;
using DBService.APIModels;
using DBService.AppContext;
using DBService.CWConstants;
using DBService.Interfaces;
using DBService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Net;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DBService.Repositories
{
    public class CWSubscription : ICWSubscription
    {
        private ApiResponse response;
        private readonly AppDBContext dbContext;
        private ILogger<CWSubscription> _logger { get; set; }
        private DapperContext _dapperContext { get; set; }

        public CWSubscription(ILogger<CWSubscription> logger, AppDBContext dBContext, DapperContext dapper)
        {
            this.response = new ApiResponse();
            this._logger = logger;
            this.dbContext = dBContext;
            this._dapperContext = dapper;
        }

        public async Task<ApiResponse> Subscribe(APICWSubscription subscription)
        {
            response = new ApiResponse();

            try
            {
                _logger.LogInformation("in subscribe method");

                var query = "select Count(Email) from cwsubscriptions where Email=@Email";
                using (var connection = _dapperContext.CreateConnection())
                {
                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { subscription.Email });

                    if (count == 0)
                    {

                        query = "insert into cwsubscriptions (Id,Email,CreatedOn) values(@Id,@Email,@CreatedOn)";

                        var parameters = new DynamicParameters();
                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("Email", subscription.Email, DbType.String);
                        parameters.Add("CreatedOn", DateTime.Now, DbType.DateTime);

                        var companies = await connection.ExecuteAsync(query, parameters);

                        if (companies > 0)
                        {
                            response.Success = true;
                            response.Data = JsonConvert.SerializeObject(true);
                            response.Message = Messages.SuccesfullySubscribed;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                response.Success = false;
                response.Message = e.Message;
                response.Data = null;
            }

            return response;
        }

        public async Task<ApiResponse> Visiter(string ipaddress)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();

                var query = "select Count(IpAddress) from cwvisiters where IpAddress=@IpAddress";
                using (var connection = _dapperContext.CreateConnection())
                {
                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { ipaddress });

                    if (count == 0)
                    {

                        query = "insert into cwvisiters (Id,IpAddress,CreatedOn) values(@Id,@IpAddress,@CreatedOn)";

                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("IpAddress", ipaddress, DbType.String);
                        parameters.Add("CreatedOn", DateTime.Now, DbType.DateTime);


                        var inserted = connection.QuerySingleOrDefault<int>(query, parameters);
                        if (inserted == 1)
                        {
                            response.Success = true;
                            response.Data = JsonConvert.SerializeObject(true);
                            response.Message = Messages.SuccesfullySubscribed;
                        }
                    }
                }


            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                response.Success = false;
                response.Message = e.Message;
                response.Data = null;
            }

            return response;
        }

        public async Task<ApiResponse> EmailExists(string email)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();

                var query = "select Count(email) from cwsubscriptions where email=@email";
                using (var connection = _dapperContext.CreateConnection())
                {
                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { email });

                    if (count != 0 && count > 0)
                    {
                        response.Success = true;
                        response.Data = JsonConvert.SerializeObject(false);
                        response.Message = Messages.AlreadySubscribed;
                    }
                    else
                    {
                        response.Success = true;
                        response.Data = JsonConvert.SerializeObject(true);
                    }
                }


            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                response.Success = false;
                response.Message = e.Message;
                response.Data = null;
            }

            return response;
        }

        public async Task<ApiResponse> SaveVisiter(string ipaddress)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();

                var query = "select count(*) from cwvisiters where IpAddress=@Ip";
                parameters.Add("Ip", ipaddress, DbType.String);

                using (var connection = _dapperContext.CreateConnection())
                {

                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

                    if (count == 0)
                    {
                        query = "insert into cwvisiters(Id,IpAddress,CreatedOn) values(@Id,@Ip,@Created)";
                        parameters.Add("Id", Guid.NewGuid(), DbType.String);
                        parameters.Add("Ip", ipaddress, DbType.String);
                        parameters.Add("Created", DateTime.Now.ToString("yyyy-MM-dd"), DbType.DateTime);

                        var inserted = await connection.ExecuteAsync(query, parameters);

                        if (inserted > 0)
                        {
                            response.Success = true;
                            response.Data = JsonConvert.SerializeObject(false);
                            response.Message = Messages.AlreadySubscribed;
                        }
                        else
                        {
                            response.Success = false;
                            response.Data = JsonConvert.SerializeObject(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                response.Success = false;
                response.Message = e.Message;
                response.Data = null;
            }

            return response;
        }

        public async Task<ApiResponse> GetVisiterCount()
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();

                var query = "select count(*) from cwvisiters";

                using (var connection = _dapperContext.CreateConnection())
                {

                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);

                    response.Success = true;
                    response.Data = JsonConvert.SerializeObject(count);
                    response.Message = "";
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                response.Success = false;
                response.Message = e.Message;
                response.Data = null;
            }

            return response;
        }
    }
}