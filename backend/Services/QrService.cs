using QRCoder;
using QRCoder.Generated;
using System.Drawing;
using System.Drawing.Imaging;

namespace HuellaApi.Services;

public class QrService
{
    public byte[] GeneratePng(string text, int pixelsPerModule = 8)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
        using var qr = new QRCoder.Generated.QRCode(data);
        using var bmp = qr.GetGraphic(pixelsPerModule);
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
