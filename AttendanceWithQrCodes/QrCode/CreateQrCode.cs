using QRCoder;

namespace AttendanceWithQrCodes.QrCode
{
    public class CreateQrCode : ICreateQrCode
    {
        public CreateQrCode() { }

        public Models.QrCode GenerateQrCode(DateTime time, int lectureId)
        {
            Models.QrCode qrCode = new Models.QrCode();
            string data = "lecture id: " + lectureId;
            string img = "qr-" + Guid.NewGuid() + ".png";
            string domian = AppDomain.CurrentDomain.BaseDirectory;
            string[] domianParts = domian.Split("bin");
            string path = String.Concat(domianParts[0], "QrCode\\codes\\", img);
            qrCode.Data = data;
            qrCode.ImageName = img;
            qrCode.ExpiresAt = time.AddMinutes(5);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode pngByteQRCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = pngByteQRCode.GetGraphic(20);
            MemoryStream qrCodeImageStream = new MemoryStream(qrCodeAsPngByteArr);
            using (var image = Image.Load<Rgba32>(qrCodeImageStream))
            {
                image.Save(path);
            }

            return qrCode;
        }
    }
}
