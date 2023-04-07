using AttendanceWithQrCodes.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendanceWithQrCodes.Data
{
    public class Context : DbContext
    {
        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options) { }
        
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StudyProfile> StudyProfiles { get; set; }
        public DbSet<StudyLanguage> StudyLanguages { get; set; }
        public DbSet<StudentInformation> StudentInformations { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseStudyProfile> CoursesStudyProfiles { get; set; }
        public DbSet<CourseLanguage> CoursesLanguages { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
    }
}
