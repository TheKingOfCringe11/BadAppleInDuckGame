using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace FrameDataWriter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            WriteFrameData("filename", "directory", 0.1f, 6588);
        }

        private static void WriteFrameData(string fileName, string directory, float sizeMultiplier, int framesCount)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)) || !Directory.Exists(directory))
                return;

            int pixelsPerTile = (int)(1f / sizeMultiplier);

            using (FileStream stream = File.OpenWrite(fileName))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    for (int i = 1; i <= framesCount; i++)
                    {
                        string path = Path.Combine(directory, GetFileName(i));

                        if (!File.Exists(path))
                            break;

                        using (Bitmap bitmap = new Bitmap(path))
                        {
                            BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            var bytes = new List<byte>();

                            int frameWidth = (int)Math.Floor(bitmap.Width * sizeMultiplier);
                            int frameHeight = (int)Math.Floor(bitmap.Height * sizeMultiplier);

                            for (int x = 0; x < frameWidth; x++)
                            {
                                for (int y = 0; y < frameHeight; y++)
                                {
                                    if (IsBlackTile(data, x, y, pixelsPerTile))
                                    {
                                        for (int offsetX = -1; offsetX <= 1; offsetX++)
                                        {
                                            for (int offsetY = -1; offsetY <= 1; offsetY++)
                                            {
                                                if (offsetX == 0 && offsetY == 0)
                                                    continue;

                                                int neighbourX = x + offsetX;
                                                int neighbourY = y + offsetY;

                                                if (neighbourX >= frameWidth || neighbourX < 0 || neighbourY >= frameHeight || neighbourY < 0)
                                                    continue;

                                                if (!IsBlackTile(data, neighbourX, neighbourY, pixelsPerTile))
                                                {
                                                    bytes.AddRange(BitConverter.GetBytes(x));
                                                    bytes.AddRange(BitConverter.GetBytes(y));

                                                    goto exit;
                                                }
                                            }
                                        }
                                    }
                                exit:;
                                }
                            }

                            bitmap.UnlockBits(data);

                            if (bytes.Any())
                            {
                                writer.Write(true);
                                writer.Write(bytes.Count);
                                writer.Write(bytes.ToArray());
                            }
                            else
                            {
                                writer.Write(false);
                            }
                        }

                        Console.WriteLine(i.ToString());
                    }
                }
            }
        }

        private static bool IsBlackTile(BitmapData data, int tileX, int tileY, int pixelsPerTile)
        {
            int pixelX = tileX * pixelsPerTile;
            int pixelY = tileY * pixelsPerTile;

            for (int x = pixelX; x < pixelX + pixelsPerTile; x++)
                for (int y = pixelY; y < pixelY + pixelsPerTile; y++)
                    if (!IsBlackPixel(data, x, y))
                        return false;

            return true;
        }

        private static bool IsBlackPixel(BitmapData data, int x, int y)
        {
            unsafe
            {
                byte* row = (byte*)(data.Scan0 + y * data.Stride);
                int pixel = x * 4;

                byte r = row[pixel + 2];
                byte g = row[pixel + 1];
                byte b = row[pixel];

                return r != 255 && g != 255 && b != 255;
            }
        }

        private static string GetFileName(int frameNumber)
        {
            string rawNumber = frameNumber.ToString();
            var builder = new StringBuilder();

            for (int i = 0; i < 5 - rawNumber.Length; i++)
                builder.Append("0");

            builder.Append(rawNumber);
            builder.Append(".png");

            return builder.ToString();
        }
    }
}
