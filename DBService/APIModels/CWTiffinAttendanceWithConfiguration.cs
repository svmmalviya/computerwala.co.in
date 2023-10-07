using computerwala.DBService.Models;
using DBService.Models;

namespace computerwala.DBService.APIModels
{
    public class CWTiffinAttendanceWithConfiguration
    {
        public List<CWAttendance> Attendances { get; set; }
        public CWTiffinsConfigurations Configuration { get; set; }
    }
}
