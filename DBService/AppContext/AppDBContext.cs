using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using computerwala.DBService.Models;
using DBService.Models;
using Microsoft.EntityFrameworkCore;

namespace DBService.AppContext
{

    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {

        }

        public DbSet<CWSubscriptions> CWSubscriptions { get; set; }
        public DbSet<CWVisiteres> CWVisiters { get; set; }
        public DbSet<CWCalender> CWCalenders { get; set; }
        public DbSet<CWYear> CWYears { get; set; }
        public DbSet<CWMonth> CWMonths { get; set; }
        public DbSet<CWDays> CWDays { get; set; }
        public DbSet<CWAttendance> CWAttendance { get; set; }
        public DbSet<CWTiffinsPreferences> CWTiffinPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Seed();
        }

    }

    public static class ModalBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CWTiffinsPreferences>().HasData(
                   new CWTiffinsPreferences
                   {
                       Id = Guid.NewGuid().ToString(),
                       Active = true,
                       CreatedOn = DateTime.Now,
                       FullMealAmount = 70,
                       HalfMealAmount=50,

                   });
        }
    }
}
