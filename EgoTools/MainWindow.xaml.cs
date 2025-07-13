using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EgoTools
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(typeof(Views.MainPage));
            // 设置窗口图标
            SetWindowIcon();
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).CheckUtilsFiles(this);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                NavigateToPage(selectedItem.Tag?.ToString());
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                NavigateToPage(item.Tag?.ToString());
            }
        }

        private void NavigateToPage(string? pageTag)
        {
            if (string.IsNullOrEmpty(pageTag)) return;
            switch (pageTag)
            {
                case "MainPage":
                    ContentFrame.Navigate(typeof(Views.MainPage));
                    break;
                case "KeyboardSettingsPage":
                    ContentFrame.Navigate(typeof(Views.KeyboardSettingsPage));
                    break;
                case "ColorManagementPage":
                    ContentFrame.Navigate(typeof(Views.ColorManagementPage));
                    break;
                case "PowerThresholdPage":
                    ContentFrame.Navigate(typeof(Views.PowerThresholdPage));
                    break;
                case "AboutPage":
                    ContentFrame.Navigate(typeof(Views.AboutPage));
                    break;
            }
        }

        private void SetWindowIcon()
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "logo.ico");
            IntPtr hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE);
            if (hIcon != IntPtr.Zero)
            {
                SendMessage(windowHandle, WM_SETICON, (IntPtr)ICON_BIG, hIcon);
                SendMessage(windowHandle, WM_SETICON, (IntPtr)ICON_SMALL, hIcon);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr LoadImage(IntPtr hInst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        private const int WM_SETICON = 0x80;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x00000010;
    }
}
