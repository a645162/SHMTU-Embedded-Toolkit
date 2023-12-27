// Ignore Spelling: SHMTU

using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Com.FirstSolver.Splash;
using EmbeddedCourseLib;
using Microsoft.Win32;
using static EmbeddedChineseCharacter.EmbeddedChineseCharacter;

namespace SHMTU_MasterEmbeddedToolKit
{
    public partial class MainWindow
    {
        private void TextBoxImgWidth_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Check if the input is a number
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                // Cancel the input if it's not a number
                e.Handled = true;
            }
        }

        private void TextBoxImgHeight_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Check if the input is a number
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                // Cancel the input if it's not a number
                e.Handled = true;
            }
        }

        private void BtnSelectBmp_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.bmp;*.png)|*.jpg;*.jpeg;*.bmp;*.png",
            };

            var currentPath = TextBoxBmpPath.Text.Trim();
            if (File.Exists(currentPath))
            {
                openFileDialog.InitialDirectory =
                    Path.GetDirectoryName(currentPath) ?? string.Empty;
                openFileDialog.FileName = Path.GetFileName(currentPath);
            }

            if (!(openFileDialog.ShowDialog() ?? false)) return;

            var selectedFilePath = openFileDialog.FileName.Trim();

            // File is exist
            if (File.Exists(selectedFilePath))
            {
                TextBoxBmpPath.Text = selectedFilePath;
                LabelBmpStatus.Content = "File Selected";
                LabelBmpStatus.Style = (Style)FindResource("LabelSuccess");
            }
            else
            {
                LabelBmpStatus.Content = "File Not Exist";
                LabelBmpStatus.Style = (Style)FindResource("LabelDanger");
            }
        }

        private Bitmap _imageCurrent;

        private void BtnOpenBmp_Click(object sender, RoutedEventArgs e)
        {
            var filePath = TextBoxBmpPath.Text.Trim();
            if (!File.Exists(filePath))
            {
                LabelBmpStatus.Content = "File Not Exist";
                LabelBmpStatus.Style = (Style)FindResource("LabelDanger");
                return;
            }

            if (!FileNameUtil.JudgeIsImage(filePath))
            {
                LabelBmpStatus.Content = "File Is Not Image";
                LabelBmpStatus.Style = (Style)FindResource("LabelDanger");
                return;
            }

            if (_imageCurrent != null)
            {
                _imageCurrent.Dispose();
                _imageCurrent = null;
            }

            _imageCurrent = new Bitmap(filePath);

            UpdateImageSize();

            var fileName = Path.GetFileName(filePath).ToUpper();
            TextBoxImgConstantName.Text = GetUniqueIdentification(fileName);
        }

        private void BtnConvertBMP_Click(object sender, RoutedEventArgs e)
        {
            if (_imageCurrent == null)
            {
                MessageBox.Show(
                    "Please open a bmp file first.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            var savePath = TextBoxBmpHeaderPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(savePath))
            {
                MessageBox.Show(
                    "Please select a save path first.",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Create Directory if not exist
            var savePathDirectory = Path.GetDirectoryName(savePath);
            if (savePathDirectory != null && !Directory.Exists(savePathDirectory))
            {
                Directory.CreateDirectory(savePathDirectory);
            }

            if (!Directory.Exists(savePathDirectory))
            {
                MessageBox.Show(
                    "Save Path is invalid!",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            var bmp =
                Bmp24.ConvertBitmap(_imageCurrent);

            var bmpConstantName = TextBoxImgConstantName.Text.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(bmpConstantName))
            {
                var fileName = Path.GetFileName(savePath).ToUpper();
                bmpConstantName = GetUniqueIdentification(fileName);

                if (string.IsNullOrWhiteSpace(bmpConstantName))
                {
                    bmpConstantName = "PIC_IMAGE_1";
                }
            }

            var code =
                Bmp24.ConvertBitmap2CArray
                (
                    bmp,
                    "Int08U",
                    bmpConstantName,
                    $"PIC_{bmpConstantName}_RGB_ARRAY"
                );

            File.WriteAllText(savePath, code);

            MessageBox.Show("Convert BMP to C Array successfully!", "Message");
        }

        private void BtnSelectSaveBMPCPath_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "C Header File(*.h)|*.h|All Files (*.*)|*.*"
            };

            var currentPath = TextBoxBmpHeaderPath.Text.Trim();
            if (currentPath.Length > 0)
            {
                var currentPathDirectory = Path.GetDirectoryName(currentPath);
                if (Directory.Exists(currentPathDirectory))
                {
                    saveFileDialog.InitialDirectory = currentPathDirectory;
                    saveFileDialog.FileName = Path.GetFileName(currentPath);
                }
            }

            if (!(saveFileDialog.ShowDialog() ?? false)) return;

            var selectedFilePath = saveFileDialog.FileName.Trim();

            if (
                !(
                    selectedFilePath.EndsWith(".h", System.StringComparison.OrdinalIgnoreCase) ||
                    selectedFilePath.EndsWith(".hpp", System.StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                selectedFilePath += ".h";
            }

            TextBoxBmpHeaderPath.Text = selectedFilePath;
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            if (_imageCurrent == null)
            {
                MessageBox.Show(
                    "Please open a bmp file first.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            var widthStr = TextBoxImgWidth.Text.Trim();
            var heightStr = TextBoxImgHeight.Text.Trim();
            if (widthStr.Length == 0 || heightStr.Length == 0)
            {
                LabelBmpStatus.Content = "Width or Height is Empty";
                LabelBmpStatus.Style = (Style)FindResource("LabelDanger");
                return;
            }

            var newWidth = int.Parse(widthStr);
            var newHeight = int.Parse(heightStr);

            // Resize

            // 创建一个新的Bitmap对象，用于存储调整大小后的图像
            var resizedImage = new Bitmap(newWidth, newHeight);

            // 使用Graphics类进行图像绘制
            using (var g = Graphics.FromImage(resizedImage))
            {
                // 设置绘制质量
                g.InterpolationMode =
                    System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // 绘制调整大小后的图像
                g.DrawImage(_imageCurrent, 0, 0, newWidth, newHeight);
            }

            // Release Origin
            _imageCurrent.Dispose();
            _imageCurrent = resizedImage;

            UpdateImageSize();
        }

        private void UpdateImageSize()
        {
            if (_imageCurrent == null) return;

            var width = _imageCurrent.Width;
            var height = _imageCurrent.Height;

            TextBoxImgWidth.Text = width.ToString();
            TextBoxImgHeight.Text = height.ToString();

            LabelCurrentSize.Content = $"Image Size:{width} x {height}";

            var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                _imageCurrent.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            ImageThumbnail.Source = wpfBitmap;
        }

        private void ButtonGeneratePicConstantName_Click(object sender, RoutedEventArgs e)
        {
            var filePath = TextBoxBmpHeaderPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show(
                    "Please select a save path first.",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            var fileName = Path.GetFileName(filePath);
            var constantName = GetUniqueIdentification(fileName);
            if (!string.IsNullOrWhiteSpace(constantName))
            {
                TextBoxImgConstantName.Text = constantName;
            }
        }

        private void BtnReadClipboard_Click(object sender, RoutedEventArgs e)
        {
            // 读取剪贴板内容
            var dataObject = Clipboard.GetDataObject();

            // 判断剪贴板中是否包含图像数据
            if (dataObject?.GetDataPresent(DataFormats.Bitmap) == true)
            {
                BitmapSource clipboardImage = Clipboard.GetImage();

                // 判断图像数据是否为有效图像
                if (clipboardImage == null) return;

                _imageCurrent = clipboardImage.ToBitmap();

                UpdateImageSize();
            }
            else
            {
                MessageBox.Show(
                    "Clipboard does not contain image data.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}