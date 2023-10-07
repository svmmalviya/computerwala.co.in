namespace computerwala.Utility
{
    public  class ISTDatetime
    {
        private TimeZoneInfo timeZoneInfo;
        private string IST = "India Standard Time";
        public DateTime istDatetime;


        public ISTDatetime()
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(IST);
            istDatetime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
        }

        public DateTime GetISTDateTime()
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(IST);
            istDatetime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);

            return istDatetime;
        }
    }
}
