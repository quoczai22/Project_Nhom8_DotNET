using QuanLyLinhKienMayTinh.ViewModels;
using System.Windows;
namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaKhachHangDialog : Window
    {
        private readonly ThemSuaKhachHangDialogViewModel _vm;
        public KhachHangDisplay KetQua => _vm.KetQua;
        /// Mở dialog ở chế độ THÊM
        public ThemSuaKhachHangDialog(string maKhMoi)
        {
            InitializeComponent();
            _vm = new ThemSuaKhachHangDialogViewModel(maKhMoi);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
        /// Mở dialog ở chế độ SỬA
        public ThemSuaKhachHangDialog(KhachHangDisplay kh)
        {
            InitializeComponent();
            _vm = new ThemSuaKhachHangDialogViewModel(kh);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}