using System.ComponentModel.DataAnnotations;

namespace computerwala.DBService.Models
{
    public class CWTiffinsConfigurations
    {
        [Key]
        public string Id { get; set; }
        public double HalfMealAmount { get; set; }
        public double FullMealAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool Active { get; set; }

    }
}
