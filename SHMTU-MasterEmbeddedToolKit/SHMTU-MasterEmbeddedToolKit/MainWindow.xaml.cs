﻿// Ignore Spelling: SHMTU

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using WindowsSystemTheme;
using MessageBox = HandyControl.Controls.MessageBox;

namespace SHMTU_MasterEmbeddedToolKit
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Auto Update Theme
            if (Windows10Theme.IsSupport())
            {
                ((App)Application.Current).UpdateTheme(
                    Windows10Theme.IsSystemThemeDark
                        ? ApplicationTheme.Dark
                        : ApplicationTheme.Light
                );
            }
        }

        #region Change Theme

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
            => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button)
            {
                PopupConfig.IsOpen = false;
                if (button.Tag is ApplicationTheme tag)
                {
                    ((App)Application.Current).UpdateTheme(tag);
                }
                else if (button.Tag is System.Windows.Media.Brush accentTag)
                {
                    ((App)Application.Current).UpdateAccent(accentTag);
                }
                else if (button.Tag is "Picker")
                {
                    var picker = SingleOpenHelper.CreateControl<ColorPicker>();
                    var window = new PopupWindow
                    {
                        PopupElement = picker,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        AllowsTransparency = true,
                        WindowStyle = WindowStyle.None,
                        MinWidth = 0,
                        MinHeight = 0,
                        Title = "Select Accent Color"
                    };

                    picker.SelectedColorChanged += delegate
                    {
                        ((App)Application.Current).UpdateAccent(picker.SelectedBrush);
                        window.Close();
                    };
                    picker.Canceled += delegate { window.Close(); };
                    window.Show();
                }
            }
        }

        #endregion

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            // Exit
            Application.Current.Shutdown();
        }

        // Open URL on Default Web Browser
        private static void OpenUrl(string url)
        {
            Process.Start(url);
        }

        private void MenuItemGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://github.com/a645162/SHMTU-Embedded-Toolkit");
        }

        private void MenuItemGithubHome_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://github.com/a645162");
        }

        private void ButtonGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://github.com/a645162/SHMTU-Embedded-Toolkit");
        }

        private static void OpenExplorerAndSelectFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show(
                    $"File is not exist：{filePath}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
                return;
            }

            try
            {
                var argument = $"/select, \"{filePath}\"";

                Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Cannot open Windows Explorer：{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
            }
        }

        private static void OpenFileWithDefaultProgram(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show(
                    $"File is not exist：{filePath}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
                return;
            }

            try
            {
                // 启动默认关联程序打开文件
                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Cannot open File：{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
            }
        }

        private static string GetFormatTimeString()
        {
            var currentTime = DateTime.Now;

            var formattedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");

            return formattedTime;
        }

        private static string GetGenerateTimeString()
        {
            return $"/* The file was created in {GetFormatTimeString()} */";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // 获取当前程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 获取程序集的版本信息
            var version = assembly.GetName().Version.ToString();

            WindowMain.Title += " " + version;
        }
    }
}