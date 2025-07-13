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
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EgoTools.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _deviceName = "设备名称";
        public string DeviceName
        {
            get => _deviceName;
            set { if (_deviceName != value) { _deviceName = value; OnPropertyChanged(); } }
        }
        private string _cpuInfo = "Qualcomm Snapdragon 8cx Gen3 @ 3.0 GHz 八核";
        public string CpuInfo
        {
            get => _cpuInfo;
            set { if (_cpuInfo != value) { _cpuInfo = value; OnPropertyChanged(); } }
        }
        private string _storageInfo = "-";
        public string StorageInfo
        {
            get => _storageInfo;
            set { if (_storageInfo != value) { _storageInfo = value; OnPropertyChanged(); } }
        }
        private string _ramInfo = "16GB LPDDR4X（低功耗版）4266MHz";
        public string RamInfo
        {
            get => _ramInfo;
            set { if (_ramInfo != value) { _ramInfo = value; OnPropertyChanged(); } }
        }
        private string _osInfo = "-";
        public string OsInfo
        {
            get => _osInfo;
            set { if (_osInfo != value) { _osInfo = value; OnPropertyChanged(); } }
        }
        private string _greeting = "";
        public string Greeting
        {
            get => _greeting;
            set { if (_greeting != value) { _greeting = value; OnPropertyChanged(); } }
        }
        public MainPage()
        {
            InitializeComponent();
            DeviceName = GetDeviceName();
            Greeting = GetGreeting();
            StorageInfo = GetStorageInfo();
            OsInfo = GetOsInfo();
            this.DataContext = this;
        }

        private string GetDeviceName()
        {
            // 读取设备名称，优先用 Windows API
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return "未知设备";
            }
        }
        private string GetStorageInfo()
        {
            try
            {
                var drives = System.IO.DriveInfo.GetDrives()
                    .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                    .ToArray();
                if (drives.Length == 0) return "未知";
                var infoList = drives.Select(d =>
                {
                    double totalGB = d.TotalSize / 1024.0 / 1024 / 1024;
                    double freeGB = d.TotalFreeSpace / 1024.0 / 1024 / 1024;
                    return $"{d.Name.TrimEnd('\\')} {freeGB:F0}GB / {totalGB:F0}GB";
                });
                return string.Join(", ", infoList);
            }
            catch { return "未知"; }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        private string GetOsInfo()
        {
            try
            {
                string name = "Windows";
                string edition = "";
                string version = "";

                // 获取Edition和DisplayVersion
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"))
                {
                    if (key != null)
                    {
                        edition = key.GetValue("EditionID") as string ?? "";
                        version = key.GetValue("DisplayVersion") as string ?? key.GetValue("ReleaseId") as string ?? "";
                    }
                }

                // 判断主版本
                var osVersion = Environment.OSVersion.Version;
                if (osVersion.Major == 10 && osVersion.Build >= 22000)
                    name = "Windows 11";
                else if (osVersion.Major == 10)
                    name = "Windows 10";
                    
                // Edition翻译
                string editionCn = edition switch
                {
                    "Core" => "家庭版",
                    "CoreCountrySpecific" => "家庭中文版",
                    "Professional" => "专业版",
                    "Enterprise" => "企业版",
                    "Education" => "教育版",
                    _ => edition
                };

                // 组合
                return $"{name} {editionCn} {version}";
            }
            catch { return "未知"; }
        }

        private string GetGreeting()
        {
            string user = Environment.UserName;
            int hour = DateTime.Now.Hour;
            if (hour >= 23 || hour < 5)
                return $"🌙 夜深了, {user}，早点休息哦！";
            if (hour >= 5 && hour < 9)
                return $"☀️ 早上好, {user}，新的一天加油！";
            if (hour >= 9 && hour < 12)
                return $"🌤 上午好, {user}，工作顺利！";
            if (hour >= 12 && hour < 14)
                return $"🍚 中午好, {user}，记得午休！";
            if (hour >= 14 && hour < 18)
                return $"🌞 下午好, {user}，继续加油！";
            if (hour >= 18 && hour < 23)
                return $"🌆 晚上好, {user}，注意休息！";
            return $"👋 你好, {user}！";
        }
    }
}
