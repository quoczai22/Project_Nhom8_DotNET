using QuanLyLinhKienMayTinh.Services;
using System.Windows;
namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ChonPhuongThucDialog : Window
    {
        string _maHd;
        long _soTien;
        Action _onThanhToanThanhCong; // khai báo biến để lưu callback khi thanh toán thành công
        IMomoService _momoService; // khai báo biến để lưu service MoMo, sẽ được truyền vào từ bên ngoài

        public ChonPhuongThucDialog(string maHd, long soTien, IMomoService momoService, Action onThanhToanThanhCong = null )
        {
            InitializeComponent(); // truyền callcak vào hàm khởi tạo 
            _maHd = maHd;
            _soTien = soTien;
            _momoService = momoService;
            _onThanhToanThanhCong = onThanhToanThanhCong; 
            txtSoTien.Text = $"{soTien:N0} ₫";
        }

        private void BtnTiepTuc_Click(object sender, RoutedEventArgs e)
        {
            if (rdMomo.IsChecked == true)
            {
                var momoPage = new MomoPaymentWindow(_maHd, _soTien, _momoService, _onThanhToanThanhCong);// truyền thông tin đơn hàng

                var window = System.Windows.Application.Current.MainWindow;
                if(window != null && window.IsVisible && window.IsLoaded) {
                    momoPage.Owner = window; // đặt cửa sổ cha để có thể gọi callback khi thanh toán thành công
                    momoPage.WindowStartupLocation = WindowStartupLocation.CenterScreen; // căn giữa so với cửa sổ cha
                }
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