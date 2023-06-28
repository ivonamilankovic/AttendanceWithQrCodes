namespace AttendanceWithQrCodes.HelperMethods
{
    public interface IDeletingHelperMethods
    {
        public Task DeleteLectures(int courseId, string authValue, string apiUrlPath);
        public Task DeleteCourseLanguagesAndProfiles(int courseId);
        public Task DeleteCourse(int courseId);
        public Task DeleteAttendances(int studentIndex, string authValue, string apiUrlPath);
        public Task DeleteStudentInfo(int studentIndex);
    }
}