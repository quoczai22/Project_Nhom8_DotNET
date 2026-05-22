using QuanLyLinhKienMayTinh.ViewModels;
using System.Windows;
namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaKhachHangDialog : Window
    {
        private readonly ThemSuaKhachHangDialogViewModel _vm;
        public KhachHangDisplay KetQua => _vm.KetQua;
        /// <summary>
        /// Mở dialog ở chế độ THÊM
        /// </summary>
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
        /// <summary>
        /// Mở dialog ở chế độ SỬA
        /// </summary>
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