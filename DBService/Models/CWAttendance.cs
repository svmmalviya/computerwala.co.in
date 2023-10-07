using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Models
{
    public class CWAttendance
    {
        [Key]
        public string Id { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string AttendanceTime { get; set; }
        public string Type { get; set; }
        public bool HasAttended { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
}
