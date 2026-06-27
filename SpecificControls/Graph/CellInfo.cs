using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpecificControls.Graph;

public class CellInfo: IInfo
{
    private static int Ids = 0;

    public required bool[,] Cells;

    public required Point TopLeft;

    public required bool FlipY;

    private readonly int id = Ids++;

    public int Id => id;

    public string Name => $"Map {id}";

    public byte R;

    public byte G;

    public byte B;

    public byte A;

    public int Width => Cells.GetLength(0);

    public int Height => Cells.GetLength(1);

    public WriteableBitmap ToBitmap()
    {
        var cells = Cells;
        var flipY = FlipY;
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
