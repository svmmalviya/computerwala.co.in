using System.ComponentModel.DataAnnotations;

namespace computerwala.Models
{
    public class CWUserData
    {
        [Key]
        public string Id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string createdon { get; set; }
        public string updatedon { get; set; }
        public bool isactive { get; set; }
        public int usertype { get; set; }
        public string userid { get; set; }
    }

}
