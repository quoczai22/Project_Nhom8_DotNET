using QRCoder;
using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class MomoPaymentWindow : Window
    {
        private readonly string _maHd;
        private readonly long _soTien;
        private readonly MomoService _momoService;
        private readonly Action _onThanhToanThanhCong;
        private System.Threading.CancellationTokenSource _qrCancellationTokenSource;
        private bool _daThanhToan = false;

        public MomoPaymentWindow(string maHd, long soTien, Action onThanhToanThanhCong = null)
        {
            InitializeComponent();
            _maHd = maHd;
            _soTien = soTien;
            _momoService = new MomoService();
            _onThanhToanThanhCong = onThanhToanThanhCong;

            txtMaHD.Text = $"#{maHd}";
            txtSoTien.Text = $"{soTien:N0} ₫";

            Loaded += async (s, e) => await TaoMaQR();
        }

        private async System.Threading.Tasks.Task TaoMaQR()
        {
            if (_daThanhToan)
            {
                HienThiThanhToanThanhCong();
                return;
            }

            try
            {
                loadingOverlay.Visibility = Visibility.Visible;
                imgQRCode.Source = null;
                string qrCodeString = await _momoService.GetMomoPaymentUrl(_maHd, _soTien);
                GenerateQR(qrCodeString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tạo mã QR: {ex.Message}", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateQR(string content)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);

                using (QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData))
                {
                    Bitmap logo = null;
                    try
                    {
                        Uri resourceUri = new Uri("pack://application:,,,/Images/logo_momo.png", UriKind.Absolute);
                        var streamInfo = Application.GetResourceStream(resourceUri);
                        if (streamInfo != null)
                            logo = new Bitmap(streamInfo.Stream);
                    }
                    catch { }

                    System.Drawing.Color momoPink = System.Drawing.Color.FromArgb(165, 0, 100);

                    using (System.Drawing.Bitmap qrBitmap = qrCode.GetGraphic(20, momoPink, System.Drawing.Color.White, logo, 25, 2, true))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;

                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();

                            imgQRCode.Source = bitmapImage;
                            imgQRCode.Visibility = Visibility.Visible;
                            successPanel.Visibility = Visibility.Collapsed;
                            loadingOverlay.Visibility = Visibility.Collapsed;
                            txtCountdown.Visibility = Visibility.Visible;

                            StartQRCountdown(60);
                            StartPollingTrangThai();
                        }
                    }
                }
            }
        }

        private void HienThiThanhToanThanhCong()
        {
            imgQRCode.Visibility = Visibility.Collapsed;
            txtCountdown.Visibility = Visibility.Collapsed;
            loadingOverlay.Visibility = Visibility.Collapsed;
            successPanel.Visibility = Visibility.Visible;
            txtSuccessAmount.Text = $"Hóa đơn #{_maHd} — {_soTien:N0} ₫";
            CapNhatTrangThai("Đã thanh toán");
        }

        public void CapNhatTrangThai(string trangThai)
        {
            Dispatcher.Invoke(() =>
            {
                txtStatus.Text = trangThai;
                statusDot.Fill = trangThai == "Đã thanh toán"
                    ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 200, 83))
                    : new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 71, 68));
            });
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await TaoMaQR();
        }

        private void BtnDong_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StartQRCountdown(int seconds)
        {
            _qrCancellationTokenSource?.Cancel();
            _qrCancellationTokenSource = new System.Threading.CancellationTokenSource();
            var token = _qrCancellationTokenSource.Token;
            int remaining = seconds;

            System.Threading.Tasks.Task.Run(async () =>
            {
                while (remaining > 0 && !token.IsCancellationRequested)
                {
                    int secs = remaining;
                    Dispatcher.Invoke(() =>
                        txtCountdown.Text = $"Mã QR hết hạn sau: {secs}s");

                    await System.Threading.Tasks.Task.Delay(1000, token);
                    remaining--;
                }

                if (!token.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() =>
                    {
                        imgQRCode.Source = null;
                        txtCountdown.Text = "Mã QR đã hết hạn. Vui lòng tạo lại.";
                    });
                }
            }, token);
        }

        private async void StartPollingTrangThai()
        {
            _qrCancellationTokenSource ??= new System.Threading.CancellationTokenSource();
            var token = _qrCancellationTokenSource.Token;

            var handler = new System.Net.Http.HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
            using var client = new System.Net.Http.HttpClient(handler);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await System.Threading.Tasks.Task.Delay(3000, token);

                    var response = await client.GetAsync(
                        $"http://localhost:5048/api/payment/check-status/{_maHd}", token);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = System.Text.Json.JsonDocument.Parse(json).RootElement;
                        string trangThai = data.GetProperty("trangThai").GetString();

                        if (trangThai == "Đã thanh toán")
                        {
                            _daThanhToan = true;

                            Dispatcher.Invoke(() =>
                            {
                                HienThiThanhToanThanhCong();
                                _onThanhToanThanhCong?.Invoke();
                                MessageBox.Show("Thanh toán MoMo thành công! 🎉",
                                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            });

                            _qrCancellationTokenSource.Cancel();
                            return;
                        }
                        else if (trangThai == "Thanh toán thất bại") // ← thêm case này
                        {
                            Dispatcher.Invoke(() => HienThiThanhToanThatBai("Giao dịch bị hủy."));
                            _qrCancellationTokenSource.Cancel();
                            return;
                        }
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException) { break; }
                catch { }
            }
        }

        private void HienThiThanhToanThatBai(string lyDo = "Giao dịch bị hủy.")
        {
            imgQRCode.Visibility = Visibility.Collapsed;
            txtCountdown.Visibility = Visibility.Collapsed;
            loadingOverlay.Visibility = Visibility.Collapsed;
            successPanel.Visibility = Visibility.Collapsed;
            failPanel.Visibility = Visibility.Visible;
            txtFailReason.Text = lyDo;

            txtStatus.Text = "Thanh toán thất bại";
            statusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 71, 68));
        }
    }
}