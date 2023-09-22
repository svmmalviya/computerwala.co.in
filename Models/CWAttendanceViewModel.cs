using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace computerwala.Models
{
    
    public class CWAttendanceViewModel
    {
        public string Id { get; set; }
        public string AttendanceDate { get; set; }
        public bool HasAttended { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
}
