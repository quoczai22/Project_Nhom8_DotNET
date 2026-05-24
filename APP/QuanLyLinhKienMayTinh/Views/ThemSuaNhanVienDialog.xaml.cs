using QuanLyLinhKienMayTinh.ViewModels;
using QuanLyLinhKienMayTinh.Models;
using System.Windows;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaNhanVienDialog : Window
    {
        // Cung cấp ViewModel ra ngoài để NhanVienViewModel có thể đọc dữ liệu sau khi dialog đóng
        public ThemSuaNhanVienDialogViewModel ViewModel { get; private set; }

        /// Mở ở chế độ THÊM nhân viên mới
        public ThemSuaNhanVienDialog(string maNvMoi)
        {
            InitializeComponent();

            ViewModel = new ThemSuaNhanVienDialogViewModel(maNvMoi);

            // Gắn hành động đóng cửa sổ để ViewModel có thể điều khiển việc đóng dialog
            ViewModel.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };

            DataContext = ViewModel;
        }

        /// Mở ở chế độ SỬA thông tin nhân viên
        public ThemSuaNhanVienDialog(NhanVienDisplay nv)
        {
            InitializeComponent();

            ViewModel = new ThemSuaNhanVienDialogViewModel(nv);

            // Gắn hành động đóng cửa sổ để ViewModel có thể điều khiển việc đóng dialog
            ViewModel.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };

            DataContext = ViewModel;
        }
    }
}
