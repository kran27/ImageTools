using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace ImageTools
{
  public static class Tools
  {
    public static byte PlusTruncate(byte a, int b)
    {
      if (((int) a & (int) byte.MaxValue) + b < 0)
        return 0;
      return ((int) a & (int) byte.MaxValue) + b > (int) byte.MaxValue ? byte.MaxValue : (byte) ((uint) a + (uint) b);
    }
        public static DirectBitmap Crop(this DirectBitmap bmp, int x1, int y1, Size s)
        {
            DirectBitmap tmp = new DirectBitmap(s.Width, s.Height);
            for(int y = y1; y < s.Height + y1; y++)
            {
                for (int x = x1; x < s.Width + x1; x++)
                {
                    tmp.SetPixel(x - x1, y - y1, bmp.GetPixel(x, y));
                }
            }
            return tmp;
        }
    public static DirectBitmap ToDirectBitmap(this Bitmap bmp)
        {
            DirectBitmap directBitmap = new DirectBitmap(bmp.Width, bmp.Height);
            using (Graphics graphics = Graphics.FromImage(directBitmap.Bitmap))
                graphics.DrawImage(bmp, 0, 0);
            return directBitmap;
        }
    public static DirectBitmap FillImage(this DirectBitmap dbmp, Color col)
        {
            using (Graphics gr = Graphics.FromImage(dbmp.Bitmap))
                gr.FillRectangle(new SolidBrush(col), 0, 0, dbmp.Width, dbmp.Height);
            return dbmp;
        }
    public static Color FindNearestColor(this Color color, Color[] palette)
    {
      int num1 = 195076;
      byte index1 = 0;
      for (byte index2 = 0; (int) index2 < palette.Length; ++index2)
      {
        int num2 = ((int) color.R & (int) byte.MaxValue) - ((int) palette[(int) index2].R & (int) byte.MaxValue);
        int num3 = ((int) color.G & (int) byte.MaxValue) - ((int) palette[(int) index2].G & (int) byte.MaxValue);
        int num4 = ((int) color.B & (int) byte.MaxValue) - ((int) palette[(int) index2].B & (int) byte.MaxValue);
        int num5 = num2 * num2 + num3 * num3 + num4 * num4;
        if (num5 < num1)
        {
          num1 = num5;
          index1 = index2;
        }
      }
      return !(palette[(int) index1] == Color.White) ? palette[(int) index1] : Color.Empty;
    }
    public static DirectBitmap SetPalette(this DirectBitmap dbmp, Color[] palette)
    {
      for (int y = 0; y < dbmp.Height; ++y)
      {
        for (int x = 0; x < dbmp.Width; ++x)
        {
          Color nearestColor = dbmp.GetPixel(x, y).FindNearestColor(palette);
          dbmp.SetPixel(x, y, nearestColor);
        }
      }
      return dbmp;
    }
    public static DirectBitmap Greyscale(this DirectBitmap dbmp)
    {
      for (int y = 0; y < dbmp.Height; ++y)
      {
        for (int x = 0; x < dbmp.Width; ++x)
        {
          Color pixel = dbmp.GetPixel(x, y);
          int num = (pixel.R + pixel.G + pixel.B) / 3;
          Color colour = Color.FromArgb(num, num, num);
          dbmp.SetPixel(x, y, colour);
        }
      }
      return dbmp;
    }
    public static DirectBitmap Dither(this DirectBitmap directBitmap1, Color[] palette)
    {
            Tools.DirectBitmap directBitmap2 = directBitmap1;
      for (int y1 = 0; y1 < directBitmap1.Height; ++y1)
      {
        for (int x1 = 0; x1 < directBitmap1.Width; ++x1)
        {
          Color pixel1 = directBitmap2.GetPixel(x1, y1);
          Color nearestColor = pixel1.FindNearestColor(palette);
          directBitmap1.SetPixel(x1, y1, nearestColor);
          int num1 = ((int) pixel1.R & (int) byte.MaxValue) - ((int) nearestColor.R & (int) byte.MaxValue);
          int num2 = ((int) pixel1.G & (int) byte.MaxValue) - ((int) nearestColor.G & (int) byte.MaxValue);
          int num3 = ((int) pixel1.B & (int) byte.MaxValue) - ((int) nearestColor.B & (int) byte.MaxValue);
          Color pixel2;
          if (x1 + 1 < directBitmap1.Width)
          {
            Tools.DirectBitmap directBitmap3 = directBitmap2;
            int x2 = x1 + 1;
            int y2 = y1;
            pixel2 = directBitmap2.GetPixel(x1 + 1, y1);
            int red = (int) Tools.PlusTruncate(pixel2.R, num1 * 7 >> 4);
            pixel2 = directBitmap2.GetPixel(x1 + 1, y1);
            int green = (int) Tools.PlusTruncate(pixel2.G, num2 * 7 >> 4);
            pixel2 = directBitmap2.GetPixel(x1 + 1, y1);
            int blue = (int) Tools.PlusTruncate(pixel2.B, num3 * 7 >> 4);
            Color colour = Color.FromArgb(red, green, blue);
            directBitmap3.SetPixel(x2, y2, colour);
          }
          if (y1 + 1 < directBitmap1.Height)
          {
            if (x1 - 1 > 0)
            {
              Tools.DirectBitmap directBitmap4 = directBitmap2;
              int x3 = x1 - 1;
              int y3 = y1 + 1;
              pixel2 = directBitmap2.GetPixel(x1 - 1, y1 + 1);
              int red = (int) Tools.PlusTruncate(pixel2.R, num1 * 3 >> 4);
              pixel2 = directBitmap2.GetPixel(x1 - 1, y1 + 1);
              int green = (int) Tools.PlusTruncate(pixel2.G, num2 * 3 >> 4);
              pixel2 = directBitmap2.GetPixel(x1 - 1, y1 + 1);
              int blue = (int) Tools.PlusTruncate(pixel2.B, num3 * 3 >> 4);
              Color colour = Color.FromArgb(red, green, blue);
              directBitmap4.SetPixel(x3, y3, colour);
            }
            Tools.DirectBitmap directBitmap5 = directBitmap2;
            int x4 = x1;
            int y4 = y1 + 1;
            pixel2 = directBitmap2.GetPixel(x1, y1 + 1);
            int red1 = (int) Tools.PlusTruncate(pixel2.R, num1 * 5 >> 4);
            pixel2 = directBitmap2.GetPixel(x1, y1 + 1);
            int green1 = (int) Tools.PlusTruncate(pixel2.G, num2 * 5 >> 4);
            pixel2 = directBitmap2.GetPixel(x1, y1 + 1);
            int blue1 = (int) Tools.PlusTruncate(pixel2.B, num3 * 5 >> 4);
            Color colour1 = Color.FromArgb(red1, green1, blue1);
            directBitmap5.SetPixel(x4, y4, colour1);
            if (x1 + 1 < directBitmap1.Width)
            {
              Tools.DirectBitmap directBitmap6 = directBitmap2;
              int x5 = x1 + 1;
              int y5 = y1 + 1;
              pixel2 = directBitmap2.GetPixel(x1 + 1, y1 + 1);
              int red2 = (int) Tools.PlusTruncate(pixel2.R, num1 >> 4);
              pixel2 = directBitmap2.GetPixel(x1 + 1, y1 + 1);
              int green2 = (int) Tools.PlusTruncate(pixel2.G, num2 >> 4);
              pixel2 = directBitmap2.GetPixel(x1 + 1, y1 + 1);
              int blue2 = (int) Tools.PlusTruncate(pixel2.B, num3 >> 4);
              Color colour2 = Color.FromArgb(red2, green2, blue2);
              directBitmap6.SetPixel(x5, y5, colour2);
            }
          }
        }
      }
      return directBitmap1;
    }
    public static DirectBitmap RestoreTransparency(this DirectBitmap proc, DirectBitmap og)
        { 
      for (int y = 0; y < proc.Height; ++y)
      {
        for (int x = 0; x < proc.Width; ++x)
        {
          Color pixel1 = proc.GetPixel(x, y);
          Color pixel2 = og.GetPixel(x, y);
          proc.SetPixel(x, y, Color.FromArgb(pixel2.A, pixel1.R, pixel1.G, pixel1.B));
        }
      }
      return proc;
    }
    public static DirectBitmap Resize(this DirectBitmap dbmp, int width, int height, InterpolationMode interpolation = InterpolationMode.HighQualityBicubic)
    {
      DirectBitmap scaled = new DirectBitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(scaled.Bitmap))
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
            List<Color> colorList = (List<Color>)null;
      for (int y = 0; y < dbmp.Height; ++y)
      {
        for (int x = 0; x < dbmp.Width; ++x)
        {
          Color pixel = dbmp.GetPixel(x, y);
          if (!colorList.Contains(pixel))
            colorList.Add(pixel);
        }
      }
      return colorList.ToArray();
    }
        public static double[,] GaussianBlur(int lenght, double weight)
        {
            double[,] kernel = new double[lenght, lenght];
            double kernelSum = 0;
            int foff = (lenght - 1) / 2;
            double distance = 0;
            double constant = 1d / (2 * Math.PI * weight * weight);
            for (int y = -foff; y <= foff; y++)
            {
                for (int x = -foff; x <= foff; x++)
                {
                    distance = ((y * y) + (x * x)) / (2 * weight * weight);
                    kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                    kernelSum += kernel[y + foff, x + foff];
                }
            }
            for (int y = 0; y < lenght; y++)
            {
                for (int x = 0; x < lenght; x++)
                {
                    kernel[y, x] = kernel[y, x] * 1d / kernelSum;
                }
            }
            return kernel;
        }
        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, int mult = 1, double factor = 1, int bias = 0)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                     sourceBitmap.Width, sourceBitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;
            double alpha = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;
                    alpha = 0;

                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            blue += (double)(pixelBuffer[calcOffset]) *
                                    filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            green += (double)(pixelBuffer[calcOffset + 1]) *
                                     filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            red += (double)(pixelBuffer[calcOffset + 2]) *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];

                            alpha += (double)(pixelBuffer[calcOffset + 3]) *
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


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
        public static Bitmap AOConvolutionFilter(Bitmap sourceBitmap, double[,] filterMatrix, Color col, int mult = 1, double factor = 1, int bias = 0)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            double alpha = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    alpha = 0;

                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);
                            alpha += (double)(pixelBuffer[calcOffset + 3]) *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];
                        }
                    }
                    alpha = factor * alpha + bias;
                    alpha *= mult;
                    alpha = (alpha > 255 ? 255 : (alpha < 0 ? 0 : alpha));

                    resultBuffer[byteOffset] = (byte)(col.B);
                    resultBuffer[byteOffset + 1] = (byte)(col.G);
                    resultBuffer[byteOffset + 2] = (byte)(col.R);
                    resultBuffer[byteOffset + 3] = (byte)(alpha);
                }
            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
        public static Bitmap ApplyBlur(this Bitmap src, int length, double weight, Color col, bool alphaOnly = false, int mult = 1)
        {
            if (!alphaOnly) { return ConvolutionFilter(src, GaussianBlur(length, weight), mult); }
            return AOConvolutionFilter(src, GaussianBlur(length, weight), col, mult);

        }
        public class DirectBitmap : IDisposable
    {
      public Bitmap Bitmap { get; private set; }

      public int[] Bits { get; private set; }

      public bool Disposed { get; private set; }

      public int Height { get; private set; }

      public int Width { get; private set; }

      protected GCHandle BitsHandle { get; private set; }

      public DirectBitmap(int width, int height)
      {
        this.Width = width;
        this.Height = height;
        this.Bits = new int[width * height];
        this.BitsHandle = GCHandle.Alloc((object) this.Bits, GCHandleType.Pinned);
        this.Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
      }

            public void SetPixel(int x, int y, Color colour)
            {
                    this.Bits[x + y * this.Width] = colour.ToArgb();
            }

      public Color GetPixel(int x, int y) => Color.FromArgb(this.Bits[x + y * this.Width]);

      public void Dispose()
      {
        if (this.Disposed)
          return;
        this.Disposed = true;
        this.Bitmap.Dispose();
        this.BitsHandle.Free();
      }
    }
  }
}
