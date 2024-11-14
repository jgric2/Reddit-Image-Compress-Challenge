using System.Drawing.Imaging;
using System.Numerics;


namespace RedditImageCompressChallange.Compression
{

    public enum PixelColor : byte
    {
        Transparent = 0,
        ColorRed = 1,
        ColorBlack = 2
    }

    public class ImageCompressor
    {
        private const int GridSize = 8;
        //private const int BitsPerPixel = 2;
      

        public PixelColor[,] Grid { get; private set; }

        public ImageCompressor(PixelColor[,] grid)
        {
            if (grid.GetLength(0) != GridSize || grid.GetLength(1) != GridSize)
                throw new ArgumentException($"Grid must be {GridSize}x{GridSize}.");

            Grid = grid;
        }

        public byte[] SerializeBase3()
        {
            // Convert grid to base-3 number
            BigInteger base3Number = 0;
            BigInteger multiplier = 1;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    base3Number += (BigInteger)(byte)Grid[row, col] * multiplier;
                    multiplier *= 3;
                }
            }

            // Convert base3Number to byte array (little-endian)
            byte[] bytes = base3Number.ToByteArray();

            // Ensure it fits into 13 bytes (104 bits)
            if (bytes.Length > 13)
                throw new ArgumentException("Grid data exceeds 13 bytes");

            if (bytes.Length < 13)
            {
                Array.Resize(ref bytes, 13);
            }

            return bytes;
        }


        public static ImageCompressor DeserializeBase3(byte[] data)
        {
            if (data.Length != 13)
                throw new ArgumentException("Data must be exactly 13 bytes for base-3 encoding.");

            //(little-endian)
            BigInteger base3Number = new BigInteger(data, isUnsigned: true, isBigEndian: false);

            PixelColor[,] grid = new PixelColor[GridSize, GridSize];

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    grid[row, col] = (PixelColor)(byte)(base3Number % 3);
                    base3Number /= 3;
                }
            }

            return new ImageCompressor(grid);
        }


        public static ImageCompressor FromBitmap(Bitmap bmp)
        {
            //ensure 8x8
            if (bmp.Width != GridSize || bmp.Height != GridSize)
                throw new ArgumentException($"Bitmap must be {GridSize}x{GridSize} pixels.");

            PixelColor[,] grid = new PixelColor[GridSize, GridSize];

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Color pixelColor = bmp.GetPixel(col, row);

                    if (pixelColor.A == 0)
                    {
                        grid[row, col] = PixelColor.Transparent;
                    }
                    else if (pixelColor.R == 185 && pixelColor.G == 106 && pixelColor.B == 106)
                    {
                        grid[row, col] = PixelColor.ColorRed;
                    }
                    else if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                    {
                        grid[row, col] = PixelColor.ColorBlack;
                    }
                    else
                    {
                        throw new ArgumentException($"Unsupported color at ({row},{col}): {pixelColor}");
                    }
                }
            }

            return new ImageCompressor(grid);
        }


        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(GridSize, GridSize, PixelFormat.Format32bppArgb);

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Color color;
                    switch (Grid[row, col])
                    {
                        case PixelColor.Transparent:
                            color = Color.FromArgb(0, 0, 0, 0);
                            break;
                        case PixelColor.ColorRed:
                            color = Color.FromArgb(255, 185, 106, 106);
                            break;
                        case PixelColor.ColorBlack:
                            color = Color.FromArgb(255, 0, 0, 0);
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown PixelColor at ({row},{col})");
                    }
                    bmp.SetPixel(col, row, color);
                }
            }

            return bmp;
        }
      

        public void SaveToFile(string filePath)
        {
            byte[] serializedData = SerializeBase3();

            File.WriteAllBytes(filePath, serializedData);
        }


    public static ImageCompressor LoadFromFile(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            return DeserializeBase3(data);
        }
    }
}
