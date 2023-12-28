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
using MessageBox = HandyControl.Controls.MessageBox;

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

        // 1:File 2:Clipboard
        private int _imageSource = 0;
        private string _imgSourceFilePath = "";

        private void BtnOpenBmp_Click(object sender, RoutedEventArgs e)
        {
            var filePath = TextBoxBmpPath.Text.Trim();
            if (!File.Exists(filePath))
            {
                LabelBmpStatus.Content = "File Not Exist";
                LabelBmpStatus.Style = (Style)FindResource("LabelDanger");
                MessageBox.Show(
                    $"File is not exist\n{filePath}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
                return;
            }

            if (!FileNameUtils.JudgeIsImage(filePath))
            {
                LabelBmpStatus.Content = "File Is Not Image";
                LabelBmpStatus.Style = (Style)FindResource("LabelDanger");
                MessageBox.Show(
                    $"File is not an image\n{filePath}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (_imageCurrent != null)
            {
                _imageCurrent.Dispose();
                _imageCurrent = null;
            }

            _imageCurrent = new Bitmap(filePath);
            _imageSource = 1;
            _imgSourceFilePath = filePath;

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
                // var fileName = Path.GetFileName(savePath).ToUpper();
                if (_imageSource == 1)
                {
                    bmpConstantName = GetUniqueIdentification(_imgSourceFilePath);
                }

                if (string.IsNullOrWhiteSpace(bmpConstantName))
                {
                    bmpConstantName = "PIC_IMAGE_1";
                }
            }

            var code =
                Bmp24.ConvertBitmap2CArray
                (
                    bmp,
                    bmpConstantName,
                    $"PIC_{bmpConstantName}_RGB_ARRAY",
                    _imageSource == 1 ? _imgSourceFilePath : ""
                );

            if (
                !(
                    savePath.EndsWith(".h", StringComparison.OrdinalIgnoreCase) ||
                    savePath.EndsWith(".hpp", StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                savePath += ".h";
            }

            File.WriteAllText(savePath, code);

            var messageBoxResult = MessageBox.Show(
                "Convert Image to C Array successfully!\n" +
                savePath + "\n" +
                "Click \"Yes\" to Open File\n" +
                "Click \"No\" to navigate to the file in Windows Explorer\n" +
                "Click \"Cancel\" to do nothing",
                "Message",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
            );
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    OpenFileWithDefaultProgram(savePath);
                    break;
                case MessageBoxResult.No:
                    OpenExplorerAndSelectFile(savePath);
                    break;
                case MessageBoxResult.None:
                case MessageBoxResult.Cancel:
                default:
                    break;
            }
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

            LabelBmpStatus.Content = "Image Opened";
            LabelBmpStatus.Style = (Style)FindResource("LabelSuccess");
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
                var clipboardImage = Clipboard.GetImage();

                // 判断图像数据是否为有效图像
                if (clipboardImage == null) return;

                _imageCurrent = clipboardImage.ToBitmap();
                _imageSource = 2;

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

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件路径数组
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // 处理拖拽的文件
            if (files?.Length == 1)
            {
                var filePath = files[0];
                var fileName = Path.GetFileName(filePath);
                if (FileNameUtils.JudgeIsImage(fileName))
                {
                    TextBoxBmpPath.Text = filePath;
                }
                else
                {
                    MessageBox.Show(
                        "File is not an image.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            else
            {
                if (files?.Length > 1)
                {
                    MessageBox.Show(
                        "Only one file can be dragged.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Drag Error!",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
        {
            // 判断拖拽的数据是否包含文件
            e.Effects =
                e.Data.GetDataPresent(
                    DataFormats.FileDrop
                )
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
        }
    }
}