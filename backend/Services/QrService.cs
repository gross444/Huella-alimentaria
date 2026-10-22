using QRCoder;

namespace HuellaApi.Services;

public class QrService
{
    // Genera un PNG sin depender de System.Drawing (funciona en Windows, Linux y Docker)
    public byte[] GeneratePng(string text, int pixelsPerModule = 8)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
        var qr = new PngByteQRCode(data);
        return qr.GetGraphic(pixelsPerModule);
    }
}
