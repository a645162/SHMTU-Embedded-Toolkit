using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using WindowsSystemTheme;

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
            System.Diagnostics.Process.Start(url);
        }

        private void MenuItemGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl(@"https://github.com/a645162/SHMTU-Embedded-Toolkit");
        }

        private void MenuItemGithubHome_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl(@"https://github.com/a645162");
        }

        private void ButtonGithubRepo_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl(@"https://github.com/a645162/SHMTU-Embedded-Toolkit");
        }
    }
}