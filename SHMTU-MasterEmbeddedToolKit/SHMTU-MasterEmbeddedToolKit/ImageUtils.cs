// Ignore Spelling: Utils

// http://www.firstsolver.com/wordpress/?p=3462

/* ----------------------------------------------------------
 * 文件名称：ImageUtils.cs
 *
 * 作者：秦建辉
 *
 * 微信：splashcn
 *
 * 博客：http://www.firstsolver.com/wordpress/
 *
 * 开发环境：
 *      Visual Studio V2017
 *      .NET Framework 4 Client Profile
 *
 * 版本历史：
 *      V1.0    2018年01月03日
 *              实现Bitmap和BitmapSource之间的转换
 * ------------------------------------------------------------ */
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace Com.FirstSolver.Splash
{
    public static class ImageUtils
    {
        /// <summary>
        /// 将 InkCanvas 笔迹 转化为 BitmapSource 图像
        /// </summary>
        /// <param name="canvas"/>InkCanvas 控件
        /// <returns>存储笔画图像的 BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Windows.Controls.InkCanvas canvas)
        {
            // 获取笔画边界
            System.Windows.Rect rect = canvas.Strokes.GetBounds();

            // 获取笔画轮廓几何图形
            double width = canvas.DefaultDrawingAttributes.Width; // 笔画宽度
            double height = canvas.DefaultDrawingAttributes.Height; // 笔画高度
            System.Windows.Media.RectangleGeometry geometry = new System.Windows.Media.RectangleGeometry(new System.Windows.Rect(rect.X - width / 2, rect.Y - height / 2, rect.Width + width, rect.Height + height));
            canvas.Clip = geometry;
            canvas.UpdateLayout();

            // 将笔画转换为图像
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)geometry.Rect.Width, (int)geometry.Rect.Height, 96, 96, System.Windows.Media.PixelFormats.Default);
            System.Windows.Media.DrawingVisual visual = new System.Windows.Media.DrawingVisual();
            using (System.Windows.Media.DrawingContext context = visual.RenderOpen())
            {
                context.DrawRectangle(new System.Windows.Media.VisualBrush(canvas), null, new System.Windows.Rect(0, 0, geometry.Rect.Width, geometry.Rect.Height));
            }
            bitmap.Render(visual);
            canvas.Clip = null;
            return bitmap;
        }

        /// <summary>
        /// 将 Bitmap 转化为 BitmapSource
        /// </summary>
        /// <param name="bmp"/>要转换的 Bitmap
        /// <returns>转换后的 BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap bmp)
        {
            System.IntPtr hBitmap = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, System.IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        /// <summary>
        /// 将 Bitmap 转化为 BitmapSource
        /// </summary>
        /// <param name="bmp"/>要转换的 Bitmap
        /// <returns>转换后的 BitmapImage</returns>
        public static BitmapImage ToBitmapImage(this System.Drawing.Bitmap bmp)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        /// <summary>
        /// 将字节数组转换为 BitmapImage
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <returns>转换后的 BitmapImage</returns>
        public static BitmapImage ToBitmapImage(this byte[] bytes)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new System.IO.MemoryStream(bytes);
            image.EndInit();
            return image;
        }

        /// <summary>
        /// 将 Bitmap 转化为字节数组
        /// </summary>
        /// <param name="bmp">要转换的 Bitmap</param>
        /// <returns>转换后的字节数组</returns>
        public static byte[] ToByteArray(this System.Drawing.Bitmap bmp)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将 BitmapSource 转化为 Bitmap
        /// </summary>
        /// <param name="source"/>要转换的 BitmapSource
        /// <returns>转化后的 Bitmap</returns>
        public static System.Drawing.Bitmap ToBitmap(this BitmapSource source)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(ms);
                return new System.Drawing.Bitmap(ms);
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern bool DeleteObject(System.IntPtr hObject);
    }
}