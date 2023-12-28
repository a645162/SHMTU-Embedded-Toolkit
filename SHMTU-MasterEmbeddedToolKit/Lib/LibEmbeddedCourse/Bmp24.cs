﻿// Ignore Spelling: bmp

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;


namespace EmbeddedCourseLib
{
    public static class Bmp24
    {
        public static Bitmap ConvertBitmap(Image bmp32)
        {
            var bmp24 = new Bitmap(bmp32.Width, bmp32.Height, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(bmp24);
            g.DrawImage(bmp32, new Rectangle(0, 0, bmp32.Width, bmp32.Height));
            g.Dispose();
            bmp32.Dispose();
            return bmp24;
        }

        public static Bitmap ConvertBitmap(Bitmap bmp32)
        {
            return ConvertBitmap((Image)bmp32);
        }

        public static Bitmap ConvertBitmap(string fileName)
        {
            var bmp32 = Image.FromFile(fileName);
            return ConvertBitmap(bmp32);
        }

        public static void SaveBitmap(Bitmap bmp, string fileName)
        {
            bmp.Save(fileName, ImageFormat.Bmp);
        }

        public static string Convert2Hex(int value)
        {
            return $"0x{value:X2}";
        }

        public static string ConvertBitmap2CArray(
            Bitmap bmp,
            string varName,
            string headerUniqueIdentification,
            string imageSourceFileName = ""
        )
        {
            var sb = new StringBuilder();

            sb.Append("/* This is a Image Raw Data Header File. */\n");
            sb.Append("/* This file was generated by a tool created by Haomin Kong. */\n\n");

            if (imageSourceFileName.Length > 0)
            {
                sb.Append($"/* Image Source: {imageSourceFileName} */\n");
            }

            sb.Append($"/* Image Size: {bmp.Width} x {bmp.Height} */\n\n");

            sb.Append($"#ifndef {headerUniqueIdentification}\n");
            sb.Append($"#define {headerUniqueIdentification}\n\n");

            const int channel = 3;

            sb.Append($"#define {varName}_WIDTH {bmp.Width}\n");
            sb.Append($"#define {varName}_HEIGHT {bmp.Height}\n");
            sb.Append($"#define {varName}_CHANNEL {channel}\n");

            sb.Append("\n");

            sb.Append("// Int08U <=> unsigned char (8bit)\n");

            sb.Append("\n");

            sb.Append(
                $"const unsigned char {varName}[{bmp.Width} * {bmp.Height} * {channel}] = "
            );
            sb.Append("{\n\t\t");
            var count = 0;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    var r = c.R;
                    var g = c.G;
                    var b = c.B;
                    sb.Append($"{Convert2Hex(b)}, ");
                    sb.Append($"{Convert2Hex(g)}, ");
                    sb.Append($"{Convert2Hex(r)}");
                    count++;
                    sb.Append(count % 5 == 0 ? ",\n\t\t" : ", ");
                }
            }

            var str = sb.ToString();
            while (
                str.EndsWith(" ")
                || str.EndsWith(",")
                || str.EndsWith("\n")
                || str.EndsWith("\t")
            )
            {
                str = str.Remove(str.Length - 1);
            }

            str += "\n};\n";
            str += $"\n#endif //{headerUniqueIdentification}";
            return str;
        }

        public static string ConvertBitmap2CArray_GRAY_AVE(Bitmap bmp)
        {
            var sb = new StringBuilder();
            sb.Append("const uint8_t bmp[] = {");
            for (var y = 0; y < bmp.Height; y++)
            {
                sb.Append("\n");
                for (var x = 0; x < bmp.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    var r = c.R;
                    var g = c.G;
                    var b = c.B;
                    var gray = (r + g + b) / 3;
                    sb.Append(gray);
                    sb.Append(", ");
                }
            }

            sb.Append("\n};");
            return sb.ToString();
        }

        public static string ConvertBitmap2Base64(Bitmap bmp)
        {
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);
            var bytes = ms.ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
}