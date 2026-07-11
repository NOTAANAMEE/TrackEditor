using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpecificControls.Graph;

public class CellInfo(string name): IInfo
{
    private static int Ids = 0;

    public required Point TopLeft;

    public required BitmapSource Bitmap;

    public string Name => name;

    public int Width => Bitmap.PixelWidth;

    public int Height => Bitmap.PixelHeight;

    public CellInfo(): this($"Map {Ids++}") { }

    public static WriteableBitmap ToBitmap(
        bool[,] cells, bool flipY, 
        byte R, byte G, byte B, byte A)
    {
        var width = cells.GetLength(0);
        var height = cells.GetLength(1);

        var bitmap = new WriteableBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Bgra32,
            null);

        var pixels = new byte[width * height * 4];

        for (var y = 0; y < height; y++)
        {
            var sourceY = flipY ? height - 1 - y : y;

            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 4;

                if (cells[x, sourceY])
                {
                    pixels[index + 0] = B;   // B
                    pixels[index + 1] = G;   // G
                    pixels[index + 2] = R;   // R
                    pixels[index + 3] = A; // A
                }
                else
                {
                    pixels[index + 3] = 0;   // transparent
                }
            }
        }

        bitmap.WritePixels(
            new Int32Rect(0, 0, width, height),
            pixels,
            width * 4,
            0);

        bitmap.Freeze();
        return bitmap;
    }
}
