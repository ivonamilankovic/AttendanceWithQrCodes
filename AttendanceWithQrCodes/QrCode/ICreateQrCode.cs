namespace AttendanceWithQrCodes.QrCode
{
    public interface ICreateQrCode
    {
        public Models.QrCode GenerateQrCode(DateTime time, int lectureId);
    }
}
