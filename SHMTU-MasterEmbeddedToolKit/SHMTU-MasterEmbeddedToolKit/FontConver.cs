using System;
using System.IO;
using System.Windows;
using EmbeddedCourseLib;
using Microsoft.Win32;
using static EmbeddedChineseCharacter.EmbeddedChineseCharacter;

namespace SHMTU_MasterEmbeddedToolKit
{
    public partial class MainWindow
    {
        private void ButtonRefreshTTFList_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxFonts.Items.Clear();
            foreach (var fontFamily in System.Drawing.FontFamily.Families)
            {
                ComboBoxFonts.Items.Add(fontFamily.Name);
            }

            if (ComboBoxFonts.Items.Count <= 0) return;

            // Find Microsoft YaHei
            var index = ComboBoxFonts.Items.IndexOf("Microsoft YaHei UI");

            ComboBoxFonts.SelectedIndex = index > 0 ? index : 0;
        }

        private void BtnSelectSaveTTFCPath_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "C Header File(*.h)|*.h"
            };

            var currentPath = TextBoxSaveTTFCPath.Text.Trim();
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

            TextBoxSaveTTFCPath.Text = selectedFilePath;
        }

        private void BtnConvertTttSave_Click(object sender, RoutedEventArgs e)
        {
            // Check the font name
            var fontName = ComboBoxFonts.Text.Trim();
            if (string.IsNullOrWhiteSpace(fontName))
            {
                MessageBox.Show(
                    "Please select a font first.",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Check the save path
            var savePath = TextBoxSaveTTFCPath.Text.Trim();
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

            // Check the text of input
            var originText = TextBoxTTFOriginText.Text.Trim();
            if (string.IsNullOrWhiteSpace(originText))
            {
                MessageBox.Show(
                    "Please input a text first.",
                    "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Get TTF Constant Name
            var ttfConstantName = TextBoxTTFConstantName.Text.Trim();
            if (string.IsNullOrWhiteSpace(ttfConstantName))
            {
                ttfConstantName = "TTF_ARRAY_1";
            }

            // Do convert
            var cArray =
                FontUtil.GetChineseCharacterFontFromString(
                    originText,
                    FontUtil.GetFontObjectFromName(fontName),
                    ttfConstantName
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

            // Save to file
            File.WriteAllText(savePath, cArray);

            MessageBox.Show(
                "Convert TTF to C Array successfully!",
                "Message", MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void ButtonGenerateTextConstantName_Click(object sender, RoutedEventArgs e)
        {
            var filePath = TextBoxSaveTTFCPath.Text.Trim();
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
                TextBoxTTFConstantName.Text = constantName;
            }
        }
    }
}