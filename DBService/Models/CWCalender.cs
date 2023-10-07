using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Models
{
    public class CWCalender
    {
        public CWCalender()
        {
            this.Years = new List<CWYear>();
        }
        [Key]
        public string Id { get; set; }
        public DateTime EventDate { get; set; }
        [NotMapped]
        public List<CWYear> Years { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
        public string AttachmentId { get; set; }

    }

    public class CWCurrentMonth
    {
        public CWCurrentMonth()
        {
            this.AttendanceDetails=new CurrentMonthAttendanceDetails();
        }
        public DateTime EventDate { get; set; }
        public CWMonth Month { get; set; }
        public int Year { get; set; }
        public int CurrentMonth { get; set; }
        public DateTime CurrentDate { get; set; }
        public string AttachmentId { get; set; }
        public CurrentMonthAttendanceDetails AttendanceDetails { get; set; }
    }

    public class CurrentMonthAttendanceDetails
    {
        public CurrentMonthAttendanceDetails()
        {
            this.CurrentMonthMorningHalf = 0;
            this.CurrentMonthEveningHalf= 0;
            this.CurrentMonthEveningFull= 0;
            this.CurrentMonthMorningFull= 0;
            this.CurrentMonthAmt = 0;
            this.CurrentMonthAdvanceAmt= 0;
        }
        public int CurrentMonthMorningHalf { get; set; }
        public int CurrentMonthMorningFull { get; set; }
        public int CurrentMonthEveningHalf { get; set; }
        public int CurrentMonthEveningFull { get; set; }
        public int CurrentMonthAdvanceAmt { get; set; }
        public double CurrentMonthAmt { get; set; }
    }


    public class CWYear
    {
        [Key]
        public string Id { get; set; }
        public int Year { get; set; }
        [NotMapped]
        public List<CWMonth>? Month { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
        public string AttachmentId { get; set; }
        public ICollection<CWMonth> CWMonths { get; set; }
    }

    public class CWDays
    {
        [Key]
        public string Id { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
        public string AttachmentId { get; set; }

        public string MonthId { get; set; }
        //[ForeignKey("MonthId")]
        //public CWMonth Month { get; set; }


    }

    public class CWMonth
    {
        public CWMonth()
        {
            this.Day = new List<CWDays>();
        }
        [Key]
        public string Id { get; set; }
        public int Month { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public List<String>? Weekdays { get; set; }
        [NotMapped]
        public List<CWDays> Day { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
        public string AttachmentId { get; set; }
        public ICollection<CWDays> cWDays { get; set; }

        public string YearId { get; set; }
        [ForeignKey("YearId")]
        public CWYear Year { get; set; }



    }



}
