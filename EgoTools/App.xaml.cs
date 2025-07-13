using System;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json.Nodes;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EgoTools
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public static Window? MainWindowInstance { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindowInstance = _window;
            _window.Activate();
        }

        public async void CheckUtilsFiles(Window win)
        {
            string[] requiredFiles = new[]
            {
                "kbd-detach.exe",
                "KeyboardService.dll",
                "qdcm-loader.exe",
                "qdcmlib.dll"
            };
            string utilsPath = System.IO.Path.Combine(AppContext.BaseDirectory, "utils");
            var missing = requiredFiles.Where(f => !File.Exists(System.IO.Path.Combine(utilsPath, f))).ToList();
            if (missing.Count > 0)
            {
                string msg = "以下组件缺失：\n" + string.Join("\n", missing) + "\n请确保utils文件夹完整。";
                // 追加实际查找路径
                msg += $"\n\n实际查找路径：\n{utilsPath}";
                var dialog = new ContentDialog
                {
                    Title = "组件缺失",
                    Content = msg,
                    PrimaryButtonText = "退出"
                };
                if (win.Content is FrameworkElement fe)
                {
                    dialog.XamlRoot = fe.XamlRoot;
                }
                await dialog.ShowAsync();
                Application.Current.Exit();
                return;
            }
            // 检查AppData中的config.json
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configDir = Path.Combine(appData, "EgoTools");
            string configPath = Path.Combine(configDir, "config.json");
            if (!File.Exists(configPath))
            {
                // 不可取消的初始化进度弹窗
                var progressBar = new ProgressBar
                {
                    IsIndeterminate = true,
                    Width = 350,
                    Height = 20,
                    Margin = new Thickness(0, 16, 0, 0)
                };
                var stack = new StackPanel();
                stack.Children.Add(new TextBlock { Text = "首次使用，正在初始化设备...", FontSize = 16, Margin = new Thickness(0,0,0,8) });
                stack.Children.Add(progressBar);
                var dialog = new ContentDialog
                {
                    Title = "初始化中",
                    Content = stack
                };
                if (win.Content is FrameworkElement fe)
                    dialog.XamlRoot = fe.XamlRoot;
                var showTask = dialog.ShowAsync();
                bool success = true;
                string errorMsg = "";
                try
                {
                    Directory.CreateDirectory(configDir);
                    var defaultConfig = new AppConfig
                    {
                        KeyboardSettings = new KeyboardSettings { KeyboardDetachment = false },
                        ColorManagement = new ColorManagement { CurrentMode = "Factory", CurrentProfile = "sRGB", IgcFile = "", _3dlutFile = "" },
                        PowerThreshold = new PowerThreshold { ChargeLimit = 100 }
                    };
                    string json = JsonSerializer.Serialize(defaultConfig, AppJsonContext.Default.AppConfig);
                    File.WriteAllText(configPath, json);
                    await System.Threading.Tasks.Task.Delay(800); // 模拟耗时
                }
                catch (Exception ex)
                {
                    success = false;
                    errorMsg = ex.Message;
                }
                dialog.Hide();
                if (!success)
                {
                    // 错误进度条弹窗，只能退出
                    var errorBar = new ProgressBar
                    {
                        IsIndeterminate = false,
                        Value = 100,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                        Width = 250,
                        Height = 20,
                        Margin = new Thickness(0, 16, 0, 0)
                    };
                    var errorStack = new StackPanel();
                    errorStack.Children.Add(new TextBlock { Text = "初始化失败，程序将退出。", FontSize = 16, Margin = new Thickness(0,0,0,8), Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red) });
                    errorStack.Children.Add(new TextBlock { Text = errorMsg, FontSize = 12, Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray) });
                    errorStack.Children.Add(errorBar);
                    var errorDialog = new ContentDialog
                    {
                        Title = "错误",
                        Content = errorStack,
                        PrimaryButtonText = "退出",
                        IsPrimaryButtonEnabled = true
                    };
                    if (win.Content is FrameworkElement fe2)
                        errorDialog.XamlRoot = fe2.XamlRoot;
                    await errorDialog.ShowAsync();
                    Application.Current.Exit();
                    return;
                }
                else
                {
                    var okDialog = new ContentDialog
                    {
                        Title = "初始化完成",
                        Content = "初始化完毕，欢迎使用！",
                        CloseButtonText = "确定"
                    };
                    if (win.Content is FrameworkElement fe3)
                        okDialog.XamlRoot = fe3.XamlRoot;
                    await okDialog.ShowAsync();
                }
            }
        }

        public static AppConfig LoadConfig()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configPath = Path.Combine(appData, "EgoTools", "config.json");
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize(json, AppJsonContext.Default.AppConfig)!;
        }

        public static void SaveConfig(AppConfig config)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configPath = Path.Combine(appData, "EgoTools", "config.json");
            string json = JsonSerializer.Serialize(config, AppJsonContext.Default.AppConfig);
            File.WriteAllText(configPath, json);
        }
    }
}
