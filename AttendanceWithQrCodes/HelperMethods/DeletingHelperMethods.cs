using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace AttendanceWithQrCodes.HelperMethods
{
    public class DeletingHelperMethods : IDeletingHelperMethods
    {
        private readonly Context _context;
        private readonly HttpClient _httpClient;

        public DeletingHelperMethods(Context context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task DeleteLectures(int courseId, string authValue, string apiUrlPath)
        {
            IList<Lecture> lectures = await _context.Lectures
                                    .Include(l => l.Course)
                                    .Where(l => l.CourseId == courseId)
                                    .ToListAsync();
            if (lectures.Any())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authValue);
            
                foreach (Lecture l in lectures)
                {
                    HttpResponseMessage response = await _httpClient.DeleteAsync(apiUrlPath + l.Id);
                    response.EnsureSuccessStatusCode();
                }
            }
           
        }

        public async Task DeleteCourseLanguagesAndProfiles(int courseId)
        {
            Course? course = await _context.Courses
                                .Where(c => c.Id == courseId)
                                .SingleOrDefaultAsync();
            if (course != null)
            {
                course.CourseLanguages = await _context.CoursesLanguages
                                      .Where(cl => cl.CourseId == courseId)
                                      .ToArrayAsync();
                course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                    .Where(cp => cp.CourseId == courseId)
                                    .ToArrayAsync();
            }

            if (course.CourseLanguages.Any())
            {
                foreach (CourseLanguage language in course.CourseLanguages)
                {
                    _context.CoursesLanguages.Remove(language);
                }
            }

            if (course.CourseStudyProfiles.Any())
            {
                foreach (CourseStudyProfile profile in course.CourseStudyProfiles)
                {
                    _context.CoursesStudyProfiles.Remove(profile);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourse(int courseId)
        {
            Course? course = await _context.Courses
                                .Where(c => c.Id == courseId)
                                .SingleOrDefaultAsync();
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAttendances(int studentIndex, string authValue, string apiUrlPath)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authValue);

            IList<StudentAttendance> attendances = await _context.StudentAttendances
                                                    .Include(a => a.Student)
                                                    .Where(a => a.StudentIndex == studentIndex)
                                                    .ToListAsync();
            if (attendances.Any())
            {
                foreach (StudentAttendance a in attendances)
                {
                    HttpResponseMessage response = await _httpClient.DeleteAsync(apiUrlPath + a.Id);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        public async Task DeleteStudentInfo(int studentIndex)
        {
            StudentInformation? student = await _context.StudentInformations.SingleOrDefaultAsync(s => s.Index == studentIndex);
            
            if (student != null)
            {
                _context.StudentInformations.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

    }
}
