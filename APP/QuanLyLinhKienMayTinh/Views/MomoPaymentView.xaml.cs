using QRCoder;
using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.Services;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace QuanLyLinhKienMayTinh.Views
{
    public partial class MomoPaymentWindow : Window
    {
        string _maHd;
        long _soTien;
        IMomoService _momoService;
        Action _onThanhToanThanhCong;
        Action _onThanhToanThatBai;
        CancellationTokenSource _qrCancellationTokenSource; // dùng để hủy việc kiểm tra trạng thái khi cửa sổ đóng
        bool _daThanhToan = false;// đặt cờ để biết thanh toán thành công hay chưa 
        string _momoOrderId;
        System.Windows.Threading.DispatcherTimer _countdownTimer;

        public MomoPaymentWindow(string maHd, long soTien, IMomoService momoService, Action onThanhToanThanhCong = null, Action onThanhToanThatBai = null)
        {
            InitializeComponent();
            _maHd = maHd;
            _soTien = soTien;
            _momoService = momoService;
            _onThanhToanThanhCong = onThanhToanThanhCong;
            _onThanhToanThatBai = onThanhToanThatBai;

            txtMaHD.Text = $"#{_maHd}";
            txtSoTien.Text = $"{_soTien:N0} ₫";

            Loaded += async (s, e) => await TaoMaQR(); // tự động gọi lấy mã QR khi cửa sổ được tải lên
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
                _qrCancellationTokenSource?.Cancel();
                _countdownTimer?.Stop();

                loadingOverlay.Visibility = Visibility.Visible;
                successPanel.Visibility = Visibility.Collapsed;
                failPanel.Visibility = Visibility.Collapsed;
                imgQRCode.Source = null;
                txtStatus.Text = "Đang chờ thanh toán...";
                statusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 165, 0));

                var hd = new HoaDon { MaHd = _maHd, TongTien = (int)_soTien };
                var response = await _momoService.CreatePaymentAsync(hd);

                if (response == null)
                {
                    MessageBox.Show("MoMo không trả về dữ liệu thanh toán.", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (response.resultCode != 0)
                {
                    MessageBox.Show("Lỗi từ MoMo: " + response.message, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _momoOrderId = response.orderId;

                string qrThanhToan = !string.IsNullOrWhiteSpace(response.qrCodeUrl)
                    ? response.qrCodeUrl
                    : response.payUrl;

                if (string.IsNullOrWhiteSpace(qrThanhToan))
                {
                    MessageBox.Show("MoMo không trả về mã QR hoặc URL thanh toán.", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!await HienThiAnhQrTuMomo(qrThanhToan))
                {
                    GenerateQR(qrThanhToan);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tạo mã QR: {ex.Message}", "Lỗi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                loadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        async Task<bool> HienThiAnhQrTuMomo(string qrCodeUrl)
        {
            if (!Uri.TryCreate(qrCodeUrl, UriKind.Absolute, out var uri))
                return false;

            try
            {
                using var client = new System.Net.Http.HttpClient();
                var response = await client.GetAsync(uri);

                if (!response.IsSuccessStatusCode)
                    return false;

                string contentType = response.Content.Headers.ContentType?.MediaType ?? "";
                if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return false;

                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                using var ms = new MemoryStream(bytes);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                imgQRCode.Source = bitmapImage;
                imgQRCode.Visibility = Visibility.Visible;
                txtCountdown.Visibility = Visibility.Visible;

                StartQRCountdown(60);
                StartPollingTrangThai(_momoOrderId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void GenerateQR(string content)
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
                            txtCountdown.Visibility = Visibility.Visible;

                            StartQRCountdown(60);
                            StartPollingTrangThai(_momoOrderId);
                        }
                    }
                }
            }
        }
        async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await TaoMaQR();
        }

        void BtnDong_Click(object sender, RoutedEventArgs e)
        {
            _qrCancellationTokenSource?.Cancel();
            _countdownTimer?.Stop();
            this.Close();
        }

        void HienThiThanhToanThanhCong()
        {
            _qrCancellationTokenSource?.Cancel();
            _countdownTimer?.Stop();
            imgQRCode.Visibility = Visibility.Collapsed;
            txtCountdown.Visibility = Visibility.Collapsed;
            loadingOverlay.Visibility = Visibility.Collapsed;
            failPanel.Visibility = Visibility.Collapsed;
            successPanel.Visibility = Visibility.Visible;
            txtSuccessAmount.Text = $"Hóa đơn #{_maHd} — {_soTien:N0} ₫";
            txtStatus.Text = "Đã thanh toán";
            statusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80));
        }

        void HienThiThanhToanThatBai(string lyDo = "Giao dịch bị hủy.")
        {
            _qrCancellationTokenSource?.Cancel();
            _countdownTimer?.Stop();
            imgQRCode.Visibility = Visibility.Collapsed;
            txtCountdown.Visibility = Visibility.Collapsed;
            loadingOverlay.Visibility = Visibility.Collapsed;
            successPanel.Visibility = Visibility.Collapsed;
            failPanel.Visibility = Visibility.Visible;
            txtFailReason.Text = lyDo;
            txtStatus.Text = "Thanh toán thất bại";
            statusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 71, 68));
            _onThanhToanThatBai?.Invoke();
        }

        void HienThiQrHetHan()
        {
            _qrCancellationTokenSource?.Cancel();
            imgQRCode.Source = null;
            imgQRCode.Visibility = Visibility.Collapsed;
            txtCountdown.Text = "Mã QR đã hết hạn. Vui lòng bấm Làm mới.";
            txtStatus.Text = "Mã QR hết hạn";
            statusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 165, 0));
        }

        void StartQRCountdown(int seconds)
        {
            _countdownTimer?.Stop();
            int remaining = seconds; // biến đếm thời gian còn lại bắt đầu từ số giây được truyền vào
            _countdownTimer = new System.Windows.Threading.DispatcherTimer(); // tạo 1 bộ đếm thời gian 
            _countdownTimer.Interval = TimeSpan.FromSeconds(1); // cứ 1 giây chạy 1 lần
            txtCountdown.Text = $"Mã QR hết hạn sau: {remaining}s";

            _countdownTimer.Tick += (s, e) => 
            {
                remaining--;
                txtCountdown.Text = $"Mã QR hết hạn sau: {remaining}s";

                if (remaining <= 0)
                {
                    _countdownTimer.Stop();
                    HienThiQrHetHan();
                }
            };

            _countdownTimer.Start();
        }

        private async void StartPollingTrangThai(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return;

            _qrCancellationTokenSource?.Cancel();
            _qrCancellationTokenSource = new System.Threading.CancellationTokenSource();
            var token = _qrCancellationTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await System.Threading.Tasks.Task.Delay(3000, token);

                    var response = await _momoService.QueryPaymentStatusAsync(orderId);
                    if (response == null)
                        continue;

                    if (response.resultCode == 0)
                    {
                        _daThanhToan = true;
                        Dispatcher.Invoke(() =>
                        {
                            HienThiThanhToanThanhCong();
                            _onThanhToanThanhCong?.Invoke();
                            MessageBox.Show("Thanh toán MoMo thành công!",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                        return;
                    }

                    if (!LaTrangThaiDangCho(response.resultCode))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            string lyDo = string.IsNullOrWhiteSpace(response.message)
                                ? "Giao dịch MoMo thất bại hoặc đã bị hủy."
                                : response.message;
                            HienThiThanhToanThatBai(lyDo);
                        });
                        return;
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException) { break; }
                catch { }
            }
        }

        bool LaTrangThaiDangCho(int resultCode)
        {
            return resultCode == 1000 || resultCode == 7000 || resultCode == 7002;
        }
    }
}
