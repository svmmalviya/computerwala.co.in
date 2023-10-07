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
using Calender = DBService.Models.CWCalender;

namespace DBService.Repositories
{
    public class CWCalender : ICWCalender
    {
        private ApiResponse response;
        private readonly AppDBContext dbContext;
        private ILogger<CWSubscription> _logger { get; set; }
        private DapperContext _dapperContext { get; set; }
        List<String> weekdays = new List<String>();
        private ISTDatetime _mISTDatetime;

        public CWCalender(ILogger<CWSubscription> logger, AppDBContext dBContext, DapperContext dapper, ISTDatetime iSTDatetime)
        {
            this.response = new ApiResponse();
            this._logger = logger;
            this.dbContext = dBContext;
            this._dapperContext = dapper;
            this._mISTDatetime = iSTDatetime;

            CultureInfo Culture = CultureInfo.CurrentCulture;

            // Get the current date
            DateTime currentDate = _mISTDatetime.istDatetime;


            var startInserting = false;
            // Loop through the days of the week and print them in Hindi
            for (int i = 0; i < 15; i++)
            {
                var dayName = new DateTime(currentDate.Year, currentDate.Month, i + 1).Date.ToString("ddd");

                // Use the DateTimeFormatInfo for the Hindi culture to get the day name
                //string dayName = Culture.DateTimeFormat.GetDayName(currentDate.DayOfWeek).Substring(0,3);
                if (dayName == "रवि" || dayName.ToLower() == "sun")
                    startInserting = true;

                if (!weekdays.Contains(dayName) && startInserting)
                    weekdays.Add(dayName);
                // Move to the next day
            }

        }
        public CWCalender()
        {

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
                        parameters.Add("CreatedOn", _mISTDatetime.istDatetime, DbType.DateTime);

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
                        parameters.Add("CreatedOn", _mISTDatetime.istDatetime, DbType.DateTime);


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

        private async void SaveCalender(DBService.Models.CWCalender cWCalender)
        {
            try
            {
                var parameters = new DynamicParameters();
                var yearid = "";

                foreach (var item in cWCalender.Years)
                {
                    var query = "select Count(Year) from cwyears where Year=@Year";
                    using (var connection = _dapperContext.CreateConnection())
                    {
                        var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { item.Year });

                        if (count == 0)
                        {
                            query = "insert into cwyears (Id,Year,CreatedOn,Active,AttachmentId) values(@Id,@Year,@CreatedOn,@Active,@AttachmentId)";

                            parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                            parameters.Add("Year", item.Year, DbType.Int32);
                            parameters.Add("CreatedOn", _mISTDatetime.istDatetime, DbType.DateTime);
                            parameters.Add("Active", item.Active, DbType.Boolean);
                            parameters.Add("AttachmentId", Guid.NewGuid(), DbType.String);


                            var inserted = await connection.ExecuteAsync(query, parameters);
                            if (inserted > 0)
                            {
                                response.Success = true;
                                response.Data = JsonConvert.SerializeObject(true);
                                response.Message = Messages.SuccesfullySubscribed;
                            }


                            foreach (var month in item.Month)
                            {
                                query = "select Id from cwyears where Year=@Year";
                                parameters.Add("Year", item.Year, DbType.Int32);
                                var id = await connection.QueryAsync<string>(query, parameters);
                                yearid = id.ToList()[0];

                                query = "select Count(Month) from cwmonths where Month=@Month and YearId=@YearId";
                                parameters.Add("@YearId", yearid, DbType.String);
                                parameters.Add("@Month", month.Month, DbType.Int32);
                                count = await connection.QuerySingleOrDefaultAsync<int>(query, new { month.Month, yearid });

                                if (count == 0 && id != null)
                                {

                                    query = "insert into cwmonths (Id,Month,Name,CreatedOn,Active,AttachmentId,YearId) values(@Id,@Month,@Name,@CreatedOn,@Active,@AttachmentId,@YearId)";

                                    parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                                    parameters.Add("Month", month.Month, DbType.Int32);
                                    parameters.Add("CreatedOn", _mISTDatetime.istDatetime, DbType.DateTime);
                                    parameters.Add("Active", item.Active, DbType.Boolean);
                                    parameters.Add("Name", month.Name, DbType.String);
                                    parameters.Add("AttachmentId", Guid.NewGuid(), DbType.String);
                                    parameters.Add("YearId", id.ToList()[0], DbType.String);


                                    inserted = await connection.ExecuteAsync(query, parameters);
                                    if (inserted > 0)
                                    {
                                        response.Success = true;
                                        response.Data = JsonConvert.SerializeObject(true);
                                        response.Message = Messages.SuccesfullySubscribed;
                                    }
                                }
                                foreach (var day in month.Day)
                                {
                                    query = "select Id from cwyears where Year=@Year";
                                    parameters.Add("Year", item.Year, DbType.Int32);
                                    var yearId = await connection.QueryAsync<string>(query, parameters);

                                    if (yearId != null)
                                    {
                                        query = "select Count(Day) from cwdays where Day=@Day and MonthId=@MonthId";
                                        parameters.Add("Day", day.Day, DbType.Int64);
                                        parameters.Add("MonthId", yearId.ToList()[0], DbType.String);
                                        count = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);


                                        if (count == 0 && yearId != null)
                                        {
                                            query = "select Id from cwmonths where Month=@Month and YearId=@YearId";
                                            parameters.Add("Month", month.Month, DbType.Int32);
                                            parameters.Add("YearId", yearId.ToList()[0], DbType.String);
                                            var monthId = await connection.QuerySingleOrDefaultAsync<string>(query, parameters);

                                            if (monthId != null)
                                            {

                                                query = "insert into cwdays (Id,Day,CreatedOn,Active,AttachmentId,MonthId,Name) values(@Id,@Day,@CreatedOn,@Active,@AttachmentId,@MonthId,@Name)";

                                                parameters.Add("Id", Guid.NewGuid().ToString(), DbType.String);
                                                parameters.Add("Day", day.Day, DbType.Int32);
                                                parameters.Add("CreatedOn", _mISTDatetime.istDatetime, DbType.DateTime);
                                                parameters.Add("Active", item.Active, DbType.Boolean);
                                                parameters.Add("AttachmentId", Guid.NewGuid(), DbType.String);
                                                parameters.Add("MonthId", monthId.ToList()[0], DbType.String);
                                                parameters.Add("Name", day.Name, DbType.String);


                                                inserted = await connection.ExecuteAsync(query, parameters);
                                                if (inserted > 0)
                                                {
                                                    response.Success = true;
                                                    response.Data = JsonConvert.SerializeObject(true);
                                                    response.Message = Messages.SuccesfullySubscribed;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }
        }
        public async Task<ApiResponse> GetCalender()
        {
            response = new ApiResponse();
            try
            {
                List<String> weekdays = new List<String> { "Sunday", "Monday", "Tuesday", "Wednusday", "Thusday", "Friday", "Saturday" };

                var cwcalender = new DBService.Models.CWCalender();
                var y1992 = new DateTime(1992, 1, 1);

                for (int i = 0; i < 100; i++)
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

                SaveCalender(cwcalender);

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

        public async Task<ApiResponse> GetCalenderByMonthYear(int month, int year)
        {
            response = new ApiResponse();
            try
            {
                var cwCurrentMonth = new DBService.Models.CWCurrentMonth();
                var currentDate = new DateTime(year,month,1);

                var currentmonth = new CWMonth { Month = currentDate.Month, Name = currentDate.Date.ToString("MMMM") };

                var days = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                currentmonth.Weekdays = weekdays;

                for (int k = 1; k <= days; k++)
                {
                    var day = new DateTime(currentDate.Year, currentDate.Month, k).Date.ToString("ddd");

                    currentmonth.Day.Add(new CWDays
                    {
                        Day = k,
                        Name = day
                    });
                }

                cwCurrentMonth.Month = currentmonth;
                cwCurrentMonth.Year = currentDate.Year;
                cwCurrentMonth.CurrentDate = currentDate;

                response.Success = true;
                response.Data = JsonConvert.SerializeObject(cwCurrentMonth);

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

        public Task<ApiResponse> SaveCalenderDay()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> SaveCalender(List<Models.CWCalender> cWCalender)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> GetCurrentCalender()
        {
            response = new ApiResponse();
            try
            {
                var cwCurrentMonth = new DBService.Models.CWCurrentMonth();
                var currentDate = _mISTDatetime.istDatetime;

                var month = new CWMonth { Month = currentDate.Month, Name = currentDate.Date.ToString("MMMM") };

                var days = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                month.Weekdays = weekdays;

                for (int k = 1; k <= days; k++)
                {
                    var day = new DateTime(currentDate.Year, currentDate.Month, k).Date.ToString("ddd");

                    month.Day.Add(new CWDays
                    {
                        Day = k,
                        Name = day
                    });
                }

                cwCurrentMonth.Month = month;
                cwCurrentMonth.Year = currentDate.Year;
                cwCurrentMonth.CurrentDate = currentDate;

                response.Success = true;
                response.Data = JsonConvert.SerializeObject(cwCurrentMonth);

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