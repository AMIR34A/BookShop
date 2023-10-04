using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace BookShop.Services;


public class Captcha
{
    const string Letters = "ABCDEFGHIGKLMNOPQRSTUVWXYZ123456789";

    public static string GenerateCaptchaCode()
    {
        StringBuilder stringBuilder = new StringBuilder();
        Random random = new Random();
        for (int i = 0; i < 4; i++)
        {
            int index = random.Next(Letters.Length - 1);
            stringBuilder.Append(Letters[index]);
        }
        return stringBuilder.ToString();
    }

    public static bool ValidateCaptchaCode(string captchaCode, HttpContext httpContext)
    {
        var isValid = captchaCode == httpContext.Session.GetString("CaptchaCode");
        httpContext.Session.Remove("CaptchaCode");
        return isValid;
    }

    public static CaptchaResult GenerateCaptchImage(int width, int height, string captchaCode)
    {
        using Bitmap bitmap = new Bitmap(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);

        Random rand = new Random();
        graphics.Clear(GetRandomDeepColor());

        DrawCaptchaCode();
        DrawDisorderLine();
        AdjustRippleEffect();

        MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);

        return new CaptchaResult
        {
            CaptchaCode = captchaCode,
            CaptchaByteData = memoryStream.ToArray(),
            Timestamp = DateTime.Now
        };
        int GetFontSize(int imageWidth, int captchCodeCount)
        {
            var averageSize = imageWidth / captchCodeCount;

            return Convert.ToInt32(averageSize);
        }

        Color GetRandomDeepColor()
        {
            int redlow = 160, greenLow = 100, blueLow = 160;
            return Color.FromArgb(rand.Next(redlow), rand.Next(greenLow), rand.Next(blueLow));
        }

        Color GetRandomLightColor()
        {
            int low = 180, high = 255;
            int nRend = rand.Next(high) % (high - low) + low;
            int nGreen = rand.Next(high) % (high - low) + low;
            int nBlue = rand.Next(high) % (high - low) + low;

            //return Color.FromArgb(215, 234, 248);
            return Color.FromArgb(nRend, nGreen, nBlue);
        }

        void DrawCaptchaCode()
        {
            SolidBrush fontBrush = new SolidBrush(Color.Black);
            int fontSize = GetFontSize(width, captchaCode.Length);
            Font font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            for (int i = 0; i < captchaCode.Length; i++)
            {
                fontBrush.Color = GetRandomDeepColor();
                int shiftPx = fontSize / 6;
                float x = i * fontSize + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);
                int maxY = height - fontSize;
                if (maxY < 0) maxY = 0;
                float y = rand.Next(0, maxY);
                graphics.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
            }
        }

        void DrawDisorderLine()
        {
            Pen linePen = new Pen(new SolidBrush(Color.Black), 3);
            for (int i = 0; i < rand.Next(3, 5); i++)
            {
                linePen.Color = GetRandomDeepColor();
                Point startPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                Point endPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                graphics.DrawLine(linePen, startPoint, endPoint);
            }
        }

        void AdjustRippleEffect()
        {
            short nWave = 6;
            int nWidth = bitmap.Width;
            int nHeight = bitmap.Height;

            Point[,] pt = new Point[nWidth, nHeight];

            for (int x = 0; x < nWidth; ++x)
            {
                for (int y = 0; y < nHeight; ++y)
                {
                    var xo = nWave * Math.Sin(2.0 * 3.1415 * y / 128.0);
                    var yo = nWave * Math.Cos(2.0 * 3.1415 * x / 128.0);

                    var newX = x + xo;
                    var newY = y + yo;

                    if (newX > 0 && newX < nWidth)
                    {
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        pt[x, y].X = 0;
                    }


                    if (newY > 0 && newY < nHeight)
                    {
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        pt[x, y].Y = 0;
                    }
                }
            }

            Bitmap bSrc = (Bitmap)bitmap.Clone();

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int scanline = bitmapData.Stride;

            IntPtr scan0 = bitmapData.Scan0;
            IntPtr srcScan0 = bmSrc.Scan0;

            bitmap.UnlockBits(bitmapData);
            bSrc.UnlockBits(bmSrc);
            bSrc.Dispose();
        }
    }
}