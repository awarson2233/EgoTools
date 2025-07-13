using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EgoTools.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void OnDantmnfGithubClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://github.com/dantmnf");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
        private async void OnAngelaCooljxGithubClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://github.com/AngelaCooljx");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
        private async void OnEgoToolsGithubClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://github.com/SaKongA/EgoTools");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
        private async void OnGoddiesGithubClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://github.com/matebook-e-go/goodies");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
