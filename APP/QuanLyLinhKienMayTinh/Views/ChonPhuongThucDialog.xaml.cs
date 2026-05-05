using System.Windows;
namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ChonPhuongThucDialog : Window
    {
        private readonly string _maHd;
        private readonly long _soTien;
        private readonly Action _onThanhToanThanhCong; // ← thêm

        public ChonPhuongThucDialog(string maHd, long soTien, Action onThanhToanThanhCong = null)
        {
            InitializeComponent();
            _maHd = maHd;
            _soTien = soTien;
            _onThanhToanThanhCong = onThanhToanThanhCong; // ← nhận callback
            txtMaHD.Text = $"#{maHd}";
            txtSoTien.Text = $"{soTien:N0} ₫";
        }

        private void BtnTiepTuc_Click(object sender, RoutedEventArgs e)
        {
            if (rdMomo.IsChecked == true)
            {
                // ← truyền callback vào MomoPaymentWindow
                var momoPage = new MomoPaymentWindow(_maHd, _soTien, _onThanhToanThanhCong);
                momoPage.Owner = this;
                momoPage.ShowDialog();
                Close();
            }
            else if (rdTienMat.IsChecked == true)
            {
                MessageBox.Show("Xác nhận thanh toán tiền mặt!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}