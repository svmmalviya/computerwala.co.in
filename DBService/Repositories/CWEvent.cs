using computerwala.DBService.APIModels;
using computerwala.DBService.Models;
using computerwala.Utility;
using Dapper;
using DBService.APIModels;
using DBService.AppContext;
using DBService.CWConstants;
using DBService.Interfaces;
using DBService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using CWAttendance = DBService.Models.CWAttendance;

namespace DBService.Repositories
{
    public class CWEvent : ICWEvent
    {
        private ApiResponse response;
        private readonly AppDBContext dbContext;
        private ISTDatetime _iSTDatetime;
        private ILogger<CWSubscription> _logger { get; set; }
        private DapperContext _dapperContext { get; set; }

        public CWEvent(ILogger<CWSubscription> logger, AppDBContext dBContext, DapperContext dapper,ISTDatetime iSTDatetime)
        {
            this.response = new ApiResponse();
            this._logger = logger;
            this.dbContext = dBContext;
            this._dapperContext = dapper;
            this._iSTDatetime = iSTDatetime;
        }
        public CWEvent()
        {

        }


        public async Task<ApiResponse> Subscribe(APICWSubscription subscription)
        {
            response = new ApiResponse();

            try
            {
                _logger.LogInformation("in subscribe method");

                var query = "select Count(Email) from CWSubscriptions where Email=@Email";
                using (var connection = _dapperContext.CreateConnection())
                {
                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { subscription.Email });

                    if (count == 0)
                    {

                        query = "insert into CWSubscriptions (Id,Email,CreatedOn) values(@Id,@Email,@CreatedOn)";

                        var parameters = new DynamicParameters();
                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("Email", subscription.Email, DbType.String);
                        parameters.Add("CreatedOn", _iSTDatetime.istDatetime, DbType.DateTime);

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

                var query = "select Count(IpAddress) from CWVisiters where IpAddress=@IpAddress";
                using (var connection = _dapperContext.CreateConnection())
                {
                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { ipaddress });

                    if (count == 0)
                    {

                        query = "insert into CWVisiters (Id,IpAddress,CreatedOn) values(@Id,@IpAddress,@CreatedOn)";

                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("IpAddress", ipaddress, DbType.String);
                        parameters.Add("CreatedOn", _iSTDatetime.istDatetime, DbType.DateTime);


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

        public async Task<ApiResponse> GetCalender()
        {
            response = new ApiResponse();
            try
            {
                List<String> weekdays = new List<String> { "Sunday", "Monday", "Tuesday", "Wednusday", "Thrusday", "Friday", "Saturday" };

                var cwcalender = new DBService.Models.CWCalender();
                var y1992 = new DateTime(1992, 1, 1);

                for (int i = 0; i < 2; i++)
                {

                    CWYear cWYear = new CWYear { Year = y1992.AddYears(i).Year };

                    var months = new List<CWMonth>();

                    for (int j = 0; j < 12; j++)
                    {
                        var month = new CWMonth { Month = (j + 1), Name = y1992.AddYears(i).AddMonths(j).Date.ToString("MMMM") };

                        var days = DateTime.DaysInMonth(y1992.AddYears(i).Year, y1992.AddYears(i).AddMonths(j).Month);
                        var allday = new List<CWDays>();

                        month.Weekdays = weekdays;

                        for (int k = 0; k < days; k++)
                        {
                            var date = y1992.AddYears(i).AddMonths(j).AddDays(k).Date.ToString("ddd");

                            month.Day.Add(new CWDays
                            {
                                Day = k + 1,
                                Name = date
                            });
                        }

                        months.Add(month);
                    }
                    cWYear.Month = months;
                    cwcalender.Years.Add(cWYear);
                }

                response.Success = true;
                response.Data = JsonConvert.SerializeObject(cwcalender);

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

        public Task<ApiResponse> GetCalenderYear(int year)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetCalenderByDate(int day, int month, int year)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> SaveCalenderDay(CWAttendance cWAttendance)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();


                using (var connection = _dapperContext.CreateConnection())
                {
                    var query = "select * from cwattendance where AttendanceDate=@Date and AttendanceTime=@Time";
                    parameters.Add("Date", cWAttendance.AttendanceDate, DbType.DateTime);
                    parameters.Add("Time", cWAttendance.AttendanceTime, DbType.String);

                    var count = await connection.QuerySingleOrDefaultAsync<CWAttendance>(query, parameters);

                    if (count == null)
                    {
                        query = "insert into cwattendance (Id,AttendanceDate,AttendanceTime,HasAttended,CreatedOn,Active) " +
                            " values(@Id,@AttendanceDate,@AttendanceTime,@HasAttended,@CreatedOn,@Active)";

                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("AttendanceDate", cWAttendance.AttendanceDate, DbType.DateTime);
                        parameters.Add("AttendanceTime", cWAttendance.AttendanceTime, DbType.String);
                        parameters.Add("HasAttended", cWAttendance.HasAttended, DbType.Boolean);
                        parameters.Add("CreatedOn", _iSTDatetime.istDatetime, DbType.DateTime);
                        parameters.Add("Active", cWAttendance.Active, DbType.Boolean);


                        var inserted = await connection.ExecuteAsync(query, parameters);

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

        public Task<ApiResponse> SaveCalender(List<Models.CWCalender> cWCalender)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> EventExists(CWAttendance cWAttendance)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();


                using (var connection = _dapperContext.CreateConnection())
                {
                    var query = "select * from cwattendance where AttendanceDate=@Date";
                    parameters.Add("Date", cWAttendance.AttendanceDate.ToString("yyyy-MM-dd"), DbType.Date);

                    var count = connection.Query<CWAttendance>(query, parameters);

                    if (count != null)
                    {
                        response.Success = true;
                        response.Data = JsonConvert.SerializeObject(count.ToList());
                        response.Message = Messages.SuccesfullySubscribed;
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


        public async Task<ApiResponse> SaveEvent(CWAttendance cWAttendance)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();
                using (var connection = _dapperContext.CreateConnection())
                {
                    var query = "select * from cwattendance where AttendanceDate=@Date and AttendanceTime=@Time";
                    parameters.Add("Date", cWAttendance.AttendanceDate, DbType.DateTime);
                    parameters.Add("Time", cWAttendance.AttendanceTime, DbType.String);

                    var count = await connection.QuerySingleOrDefaultAsync<CWAttendance>(query, parameters);

                    if (count == null)
                    {
                        query = "insert into cwattendance (Id,AttendanceDate,AttendanceTime,HasAttended,CreatedOn,Active,Type) " +
                            " values(@Id,@AttendanceDate,@AttendanceTime,@HasAttended,@CreatedOn,@Active,@TType)";

                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("AttendanceDate", cWAttendance.AttendanceDate, DbType.DateTime);
                        parameters.Add("AttendanceTime", cWAttendance.AttendanceTime, DbType.String);
                        parameters.Add("HasAttended", cWAttendance.HasAttended, DbType.Boolean);
                        parameters.Add("CreatedOn", _iSTDatetime.istDatetime, DbType.DateTime);
                        parameters.Add("Active", cWAttendance.Active, DbType.Boolean);
                        parameters.Add("TType", cWAttendance.Type, DbType.String);


                        var inserted = await connection.ExecuteAsync(query, parameters);

                        if (inserted == 1)
                        {
                            response.Success = true;
                            response.Data = JsonConvert.SerializeObject(true);
                            response.Message = Messages.AttendanceSaved;
                        }
                        else
                        {
                            response.Success = false;
                            response.Data = JsonConvert.SerializeObject(false);
                            response.Message = Messages.AttendanceSavingFailed;
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

        public async Task<ApiResponse> GetAttendanceDetails(int year, int month)
        {
            response = new ApiResponse();

            try
            {
                var parameters = new DynamicParameters();
                var currentDatte = _iSTDatetime.istDatetime;
                using (var connection = _dapperContext.CreateConnection())
                {
                    var query = "select * from cwattendance where AttendanceDate between @FromDate and @ToDate " +
                        "and (AttendanceTime=@morningTime or AttendanceTime=@eveningTime)";
                    parameters.Add("FromDate", new DateTime(year, month, 1).ToString("yyyy-MM-dd"), DbType.String);
                    parameters.Add("ToDate", new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd"), DbType.String);
                    parameters.Add("morningTime", "morning", DbType.String);
                    parameters.Add("eveningTime", "evening", DbType.String);

                    var attendances = await connection.QueryAsync<CWAttendance>(query, parameters);

                    query = "select * from cwtiffinpreferences limit 1";
                    var configuration = await connection.QuerySingleOrDefaultAsync<CWTiffinsPreferences>(query, parameters);

                    if (attendances.ToList().Count != 0)
                    {
                        CWTiffinAttendanceWithConfiguration wTiffinAttendanceWithConfiguration = new CWTiffinAttendanceWithConfiguration
                        {
                            Attendances = attendances.ToList(),
                            Configuration = configuration,
                        };

                        response.Success = true;
                        response.Data = JsonConvert.SerializeObject(wTiffinAttendanceWithConfiguration);
                        response.Message = Messages.SuccesfullySubscribed;
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

        public async Task<ApiResponse> GetTiffinPreferences()
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();
                var currentDatte = _iSTDatetime.istDatetime;
                using (var connection = _dapperContext.CreateConnection())
                {
                    var query = "select * from cwtiffinpreferences limit 1";
                    var configuration = await connection.QuerySingleOrDefaultAsync<CWTiffinsPreferences>(query, parameters);

                    if (configuration != null)
                    {
                        response.Success = true;
                        response.Data = JsonConvert.SerializeObject(configuration);
                        response.Message = Messages.SuccesfullySubscribed;
                    }else 
                    {
                        response.Success = true;
                        response.Data = JsonConvert.SerializeObject(new CWTiffinsPreferences());
                        response.Message = "";
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

        public async Task<ApiResponse> SaveTiffinPreferences(CWTiffinsPreferences cWTiffinsPreferences)
        {
            response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();
                var currentDatte = _iSTDatetime.istDatetime;
                using (var connection = _dapperContext.CreateConnection())
                {
                    var query = "select * from cwtiffinpreferences where id=@id";

                    if (cWTiffinsPreferences.Id != null && cWTiffinsPreferences.Id != string.Empty)
                    {
                        query = "update cwtiffinpreferences set HalfMealAmount=@HM, FullMealAmount=@FM,CreatedOn=@CreatedOn," +
                            "ModifiedOn=@ModifiedOn, Active=@Active where Id=@id";

                        parameters.Add("Id", cWTiffinsPreferences.Id, DbType.String);
                        parameters.Add("HM", cWTiffinsPreferences.HalfMealAmount, DbType.Double);
                        parameters.Add("FM", cWTiffinsPreferences.FullMealAmount, DbType.Double);
                        parameters.Add("CreatedOn", cWTiffinsPreferences.CreatedOn, DbType.DateTime);
                        parameters.Add("ModifiedOn", _iSTDatetime.istDatetime.ToString("yyyy-MM-dd"), DbType.DateTime);
                        parameters.Add("Active", cWTiffinsPreferences.Active, DbType.Boolean);

                        var inserted = await connection.ExecuteAsync(query, parameters);

                        if (inserted == 1)
                        {
                            response.Success = true;
                            response.Data = JsonConvert.SerializeObject(true);
                            response.Message = Messages.AttendanceSaved;
                        }
                        else
                        {
                            response.Success = false;
                            response.Data = JsonConvert.SerializeObject(false);
                            response.Message = Messages.AttendanceSavingFailed;
                        }

                    }
                    if (cWTiffinsPreferences.Id == null)
                    {

                        query = "insert into cwtiffinpreferences (Id,HalfMealAmount,FullMealAmount,CreatedOn,ModifiedOn,Active) " +
                            " values(@Id,@HM,@FM,@CreatedOn,@ModifiedOn,@Active)";

                        parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                        parameters.Add("HM", cWTiffinsPreferences.HalfMealAmount, DbType.Double);
                        parameters.Add("FM", cWTiffinsPreferences.FullMealAmount, DbType.Double);
                        parameters.Add("CreatedOn", _iSTDatetime.istDatetime.ToString("yyyy-MM-dd"), DbType.DateTime);
                        parameters.Add("ModifiedOn", null, DbType.DateTime);
                        parameters.Add("Active", cWTiffinsPreferences.Active, DbType.Boolean);


                        var inserted = await connection.ExecuteAsync(query, parameters);

                        if (inserted == 1)
                        {
                            response.Success = true;
                            response.Data = JsonConvert.SerializeObject(true);
                            response.Message = Messages.AttendanceSaved;
                        }
                        else
                        {
                            response.Success = false;
                            response.Data = JsonConvert.SerializeObject(false);
                            response.Message = Messages.AttendanceSavingFailed;
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
    }
}