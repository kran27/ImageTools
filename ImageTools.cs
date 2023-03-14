using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImageTools
{
    public static class Tools
    {
        public static byte PlusTruncate(byte a, int b)
        {
            if ((a & byte.MaxValue) + b < 0)
                return 0;
            return (a & byte.MaxValue) + b > byte.MaxValue ? byte.MaxValue : (byte)(a + (uint)b);
        }

        public static DirectBitmap Crop(this DirectBitmap bmp, int x1, int y1, Size s)
        {
            var tmp = new DirectBitmap(s.Width, s.Height);
            for (var y = y1; y < s.Height + y1; y++)
            {
                for (var x = x1; x < s.Width + x1; x++)
                {
                    tmp.SetPixel(x - x1, y - y1, bmp.GetPixel(x, y));
                }
            }
            return tmp;
        }

        public static DirectBitmap ToDirectBitmap(this Bitmap bmp)
        {
            var directBitmap = new DirectBitmap(bmp.Width, bmp.Height);
            using (var graphics = Graphics.FromImage(directBitmap.Bitmap))
            {
                graphics.DrawImageUnscaledAndClipped(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            return directBitmap;
        }

        public static DirectBitmap FillImage(this DirectBitmap dbmp, Color col)
        {
            using (var gr = Graphics.FromImage(dbmp.Bitmap))
                gr.FillRectangle(new SolidBrush(col), 0, 0, dbmp.Width, dbmp.Height);
            return dbmp;
        }

        public static Color FindNearestColor(this Color color, Color[] palette)
        {
            var num1 = 195076;
            byte index1 = 0;
            for (byte index2 = 0; index2 < palette.Length; ++index2)
            {
                var num2 = (color.R & byte.MaxValue) - (palette[index2].R & byte.MaxValue);
                var num3 = (color.G & byte.MaxValue) - (palette[index2].G & byte.MaxValue);
                var num4 = (color.B & byte.MaxValue) - (palette[index2].B & byte.MaxValue);
                var num5 = num2 * num2 + num3 * num3 + num4 * num4;
                if (num5 < num1)
                {
                    num1 = num5;
                    index1 = index2;
                }
            }
            return palette[index1];
        }

        public static DirectBitmap SetPalette(this DirectBitmap dbmp, Color[] palette)
        {
            for (var y = 0; y < dbmp.Height; ++y)
            {
                for (var x = 0; x < dbmp.Width; ++x)
                {
                    Color nearestColor = dbmp.GetPixel(x, y).FindNearestColor(palette);
                    dbmp.SetPixel(x, y, nearestColor);
                }
            }
            return dbmp;
        }

        public static DirectBitmap Greyscale(this DirectBitmap dbmp)
        {
            for (var y = 0; y < dbmp.Height; ++y)
            {
                for (var x = 0; x < dbmp.Width; ++x)
                {
                    var pixel = dbmp.GetPixel(x, y);
                    var num = (pixel.R + pixel.G + pixel.B) / 3;
                    var colour = Color.FromArgb(num, num, num);
                    dbmp.SetPixel(x, y, colour);
                }
            }
            return dbmp;
        }

        public static DirectBitmap Dither(this DirectBitmap directBitmap1, Color[] palette)
        {
            var directBitmap2 = directBitmap1;
            for (var y1 = 0; y1 < directBitmap1.Height; ++y1)
            {
                for (var x1 = 0; x1 < directBitmap1.Width; ++x1)
                {
                    var pixel1 = directBitmap2.GetPixel(x1, y1);
                    Color nearestColor = pixel1.FindNearestColor(palette);
                    directBitmap1.SetPixel(x1, y1, nearestColor);
                    var num1 = (pixel1.R & byte.MaxValue) - (nearestColor.R & byte.MaxValue);
                    var num2 = (pixel1.G & byte.MaxValue) - (nearestColor.G & byte.MaxValue);
                    var num3 = (pixel1.B & byte.MaxValue) - (nearestColor.B & byte.MaxValue);
                    Color pixel2;
                    if (x1 + 1 < directBitmap1.Width)
                    {
                        var directBitmap3 = directBitmap2;
                        var x2 = x1 + 1;
                        var y2 = y1;
                        pixel2 = directBitmap2.GetPixel(x1 + 1, y1);
                        var red = (int)PlusTruncate(pixel2.R, num1 * 7 >> 4);
                        pixel2 = directBitmap2.GetPixel(x1 + 1, y1);
                        var green = (int)PlusTruncate(pixel2.G, num2 * 7 >> 4);
                        pixel2 = directBitmap2.GetPixel(x1 + 1, y1);
                        var blue = (int)PlusTruncate(pixel2.B, num3 * 7 >> 4);
                        var colour = Color.FromArgb(red, green, blue);
                        directBitmap3.SetPixel(x2, y2, colour);
                    }
                    if (y1 + 1 < directBitmap1.Height)
                    {
                        if (x1 - 1 > 0)
                        {
                            var directBitmap4 = directBitmap2;
                            var x3 = x1 - 1;
                            var y3 = y1 + 1;
                            pixel2 = directBitmap2.GetPixel(x1 - 1, y1 + 1);
                            var red = (int)PlusTruncate(pixel2.R, num1 * 3 >> 4);
                            pixel2 = directBitmap2.GetPixel(x1 - 1, y1 + 1);
                            var green = (int)PlusTruncate(pixel2.G, num2 * 3 >> 4);
                            pixel2 = directBitmap2.GetPixel(x1 - 1, y1 + 1);
                            var blue = (int)PlusTruncate(pixel2.B, num3 * 3 >> 4);
                            var colour = Color.FromArgb(red, green, blue);
                            directBitmap4.SetPixel(x3, y3, colour);
                        }
                        var directBitmap5 = directBitmap2;
                        var x4 = x1;
                        var y4 = y1 + 1;
                        pixel2 = directBitmap2.GetPixel(x1, y1 + 1);
                        var red1 = (int)PlusTruncate(pixel2.R, num1 * 5 >> 4);
                        pixel2 = directBitmap2.GetPixel(x1, y1 + 1);
                        var green1 = (int)PlusTruncate(pixel2.G, num2 * 5 >> 4);
                        pixel2 = directBitmap2.GetPixel(x1, y1 + 1);
                        var blue1 = (int)PlusTruncate(pixel2.B, num3 * 5 >> 4);
                        var colour1 = Color.FromArgb(red1, green1, blue1);
                        directBitmap5.SetPixel(x4, y4, colour1);
                        if (x1 + 1 < directBitmap1.Width)
                        {
                            var directBitmap6 = directBitmap2;
                            var x5 = x1 + 1;
                            var y5 = y1 + 1;
                            pixel2 = directBitmap2.GetPixel(x1 + 1, y1 + 1);
                            var red2 = (int)PlusTruncate(pixel2.R, num1 >> 4);
                            pixel2 = directBitmap2.GetPixel(x1 + 1, y1 + 1);
                            var green2 = (int)PlusTruncate(pixel2.G, num2 >> 4);
                            pixel2 = directBitmap2.GetPixel(x1 + 1, y1 + 1);
                            var blue2 = (int)PlusTruncate(pixel2.B, num3 >> 4);
                            var colour2 = Color.FromArgb(red2, green2, blue2);
                            directBitmap6.SetPixel(x5, y5, colour2);
                        }
                    }
                }
            }
            return directBitmap1;
        }

        public static DirectBitmap RestoreTransparency(this DirectBitmap proc, DirectBitmap og)
        {
            for (var y = 0; y < proc.Height; ++y)
            {
                for (var x = 0; x < proc.Width; ++x)
                {
                    var pixel1 = proc.GetPixel(x, y);
                    var pixel2 = og.GetPixel(x, y);
                    proc.SetPixel(x, y, Color.FromArgb(pixel2.A, pixel1.R, pixel1.G, pixel1.B));
                }
            }
            return proc;
        }

        public static DirectBitmap ResizeFit(this DirectBitmap dbmp, int maxWidth, int maxHeight, InterpolationMode interpolation = InterpolationMode.HighQualityBicubic)
        {
            var ratioX = (double)maxWidth / dbmp.Width;
            var ratioY = (double)maxHeight / dbmp.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(dbmp.Width * ratio);
            var newHeight = (int)(dbmp.Height * ratio);

            return dbmp.Resize(newWidth, newHeight, interpolation);
        }

        public static DirectBitmap Resize(this DirectBitmap dbmp, int width, int height, InterpolationMode interpolation = InterpolationMode.HighQualityBicubic)
        {
            var scaled = new DirectBitmap(width, height);
            using (var graphics = Graphics.FromImage(scaled.Bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = interpolation;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(dbmp.Bitmap, new Rectangle(0, 0, width, height));
            }
            return scaled;
        }

        public static Color[] GetPalette(this DirectBitmap dbmp)
        {
            var colorList = new List<Color>();
            for (var y = 0; y < dbmp.Height; ++y)
            {
                for (var x = 0; x < dbmp.Width; ++x)
                {
                    var pixel = dbmp.GetPixel(x, y);
                    if (!colorList.Contains(pixel))
                        colorList.Add(pixel);
                }
            }
            return colorList.ToArray();
        }

        public static double[,] GaussianBlur(int lenght, double weight)
        {
            var kernel = new double[lenght, lenght];
            double kernelSum = 0;
            var foff = (lenght - 1) / 2;
            var constant = 1d / (2 * Math.PI * weight * weight);
            for (var y = -foff; y <= foff; y++)
            {
                for (var x = -foff; x <= foff; x++)
                {
                    var distance = ((y * y) + (x * x)) / (2 * weight * weight);
                    kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                    kernelSum += kernel[y + foff, x + foff];
                }
            }
            for (var y = 0; y < lenght; y++)
            {
                for (var x = 0; x < lenght; x++)
                {
                    kernel[y, x] = kernel[y, x] * 1d / kernelSum;
                }
            }
            return kernel;
        }

        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, int mult = 1, double factor = 1, int bias = 0)
        {
            var sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                     sourceBitmap.Width, sourceBitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            var resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            var filterWidth = filterMatrix.GetLength(1);

            var filterOffset = (filterWidth - 1) / 2;

            for (var offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (var offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    double blue = 0;
                    double green = 0;
                    double red = 0;
                    double alpha = 0;

                    var byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (var filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (var filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            var calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceData.Stride);

                            blue += pixelBuffer[calcOffset] *
                                    filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            green += pixelBuffer[calcOffset + 1] *
                                     filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            red += pixelBuffer[calcOffset + 2] *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];

                            alpha += pixelBuffer[calcOffset + 3] *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];
                        }
                    }

                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;
                    alpha = factor * alpha + bias;

                    alpha *= mult;

                    blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));

                    green = (green > 255 ? 255 : (green < 0 ? 0 : green));

                    red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                    alpha = (alpha > 255 ? 255 : (alpha < 0 ? 0 : alpha));

                    resultBuffer[byteOffset] = (byte)(blue);
                    resultBuffer[byteOffset + 1] = (byte)(green);
                    resultBuffer[byteOffset + 2] = (byte)(red);
                    resultBuffer[byteOffset + 3] = (byte)(alpha);
                }
            }

            var resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            var resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly,
                                                 PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Bitmap AOConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, Color col, int mult = 1, double factor = 1, int bias = 0)
        {
            var sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            var resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            var filterWidth = filterMatrix.GetLength(1);

            var filterOffset = (filterWidth - 1) / 2;

            for (var offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (var offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    double alpha = 0;

                    var byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (var filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (var filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            var calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * sourceData.Stride);
                            alpha += pixelBuffer[calcOffset + 3] *
                                     filterMatrix[filterY + filterOffset,
                                         filterX + filterOffset];
                        }
                    }
                    alpha = factor * alpha + bias;
                    alpha *= mult;
                    alpha = (alpha > 255 ? 255 : (alpha < 0 ? 0 : alpha));

                    resultBuffer[byteOffset] = col.B;
                    resultBuffer[byteOffset + 1] = col.G;
                    resultBuffer[byteOffset + 2] = col.R;
                    resultBuffer[byteOffset + 3] = (byte)(alpha);
                }
            }

            var resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            var resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly,
                                                 PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static DirectBitmap ApplyBlur(this DirectBitmap src, int length, double weight, int mult = 1)
        {
            return ConvolutionFilter(src.Bitmap, GaussianBlur(length, weight), mult).ToDirectBitmap();
        }

        public static DirectBitmap ApplyBlur(this DirectBitmap src, int length, double weight, Color col, int mult = 1)
        {
            return AOConvolutionFilter(src.Bitmap, GaussianBlur(length, weight), col, mult).ToDirectBitmap();
        }

        public static Color GetMostUsedColour(this DirectBitmap dbmp, int tolerance = 0)
        {
            var colorIncidence = new Dictionary<int, int>();
            for (var x = 0; x < dbmp.Width; x++)
                for (var y = 0; y < dbmp.Height; y++)
                {
                    var pixelColor = dbmp.GetPixel(x, y).ToArgb();
                    var found = false;
                    foreach (var key in colorIncidence.Keys.Where(key => Math.Abs(key - pixelColor) <= tolerance))
                    {
                        colorIncidence[key]++;
                        found = true;
                        break;
                    }
                    if (found == false)
                        colorIncidence.Add(pixelColor, 1);
                }
            return Color.FromArgb(colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);
        }

        public static List<Color> GetColourPalette(this DirectBitmap dbmp, int count, int tolerance = 0)
        {
            var colorIncidence = new Dictionary<int, int>();
            for (var x = 0; x < dbmp.Width; x++)
                for (var y = 0; y < dbmp.Height; y++)
                {
                    var pixelColor = dbmp.GetPixel(x, y).ToArgb();

                    if (colorIncidence.Keys.Contains(pixelColor))
                        colorIncidence[pixelColor]++;
                    else
                        colorIncidence.Add(pixelColor, 1);
                }
            // remove duplicate colours within tolerance
            if (tolerance != 0)
            {
                var keys = colorIncidence.Keys.ToList();
                foreach (var key in keys)
                    foreach (var key2 in keys.Where(key2 => Math.Abs(key - key2) <= tolerance && key != key2))
                        colorIncidence[key] += colorIncidence[key2];
            }

            return colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Take(count).Select(x => Color.FromArgb(x.Key)).ToList();
        }

        public class DirectBitmap : IDisposable
        {
            public Bitmap Bitmap { get; }

            public int[] Bits { get; }

            public bool Disposed { get; private set; }

            public int Height { get; }

            public int Width { get; }

            protected GCHandle BitsHandle { get; }

            public DirectBitmap(int width, int height)
            {
                Width = width;
                Height = height;
                Bits = new int[width * height];
                BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
                Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
            }

            public void SetPixel(int x, int y, Color colour)
            {
                if (!new Rectangle(0, 0, Width, Height).Contains(new Point(x, y))) return;
                Bits[x + y * Width] = colour.ToArgb();
            }

            public Color GetPixel(int x, int y)
            {
                return !new Rectangle(0, 0, Width, Height).Contains(new Point(x, y)) ? Color.Black : Color.FromArgb(Bits[x + y * Width]);
            }

            public void Dispose()
            {
                if (Disposed)
                    return;
                Disposed = true;
                Bitmap.Dispose();
                BitsHandle.Free();
            }
        }
    }
}