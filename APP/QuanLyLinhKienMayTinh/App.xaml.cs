using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace QuanLyLinhKienMayTinh
{
    public partial class App : Application
    {
        private Process _apiProcess;
        private Process _ngrokProcess;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            StartApi();
            StartNgrok();

            await ChoDenKhiApiSanSang();

        }

        private void StartApi()
        {
            string apiExe = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                @"..\..\..\..\QuanLyLinhKienMayTinh.API\bin\Debug\net8.0\QuanLyLinhKienMayTinh.API.exe"));

            if (!File.Exists(apiExe))
            {
                MessageBox.Show($"Không tìm thấy API:\n{apiExe}", "Lỗi khởi động",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _apiProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = apiExe,
                    WorkingDirectory = Path.GetDirectoryName(apiExe),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    EnvironmentVariables =
                    {
                        ["ASPNETCORE_ENVIRONMENT"] = "Development",
                        ["ASPNETCORE_URLS"] = "http://localhost:5048"  // ← ép đúng port
                    }
                }
            };
            _apiProcess.Start();
        }
        private async System.Threading.Tasks.Task ChoDenKhiApiSanSang()
        {
            using var client = new System.Net.Http.HttpClient();
            // Thử tối đa 10 lần, mỗi lần cách nhau 1 giây
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var response = await client.GetAsync("http://localhost:5048/api/payment/check-status/test");
                    // Nhận được response (dù 404) là API đã chạy
                    if (response != null) return;
                }
                catch { }

                await System.Threading.Tasks.Task.Delay(1000);
            }
        }

        private void StartNgrok()
        {
            _ngrokProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ngrok",  // Phải có ngrok trong PATH
                    Arguments = "http 5048 --domain=ignition-good-urethane.ngrok-free.dev",
                    UseShellExecute = false,
                    CreateNoWindow = true  // Ẩn console ngrok
                }
            };
            _ngrokProcess.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Tắt API
            KillProcess(_apiProcess);
            // Tắt ngrok
            KillProcess(_ngrokProcess);

            base.OnExit(e);
        }

        private void KillProcess(Process process)
        {
            try
            {
                if (process != null && !process.HasExited)
                {
                    process.Kill();
                    process.Dispose();
                }
            }
            catch { /* Bỏ qua nếu process đã tắt */ }
        }
    }
}