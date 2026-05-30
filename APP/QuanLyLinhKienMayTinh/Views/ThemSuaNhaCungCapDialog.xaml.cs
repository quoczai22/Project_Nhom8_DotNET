using QuanLyLinhKienMayTinh.ViewModels;
using System.Windows;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaNhaCungCapDialog : Window
    {
        private readonly ThemSuaNhaCungCapDialogViewModel _vm;
        public NhaCungCapDisplay KetQua => _vm.KetQua;

        public ThemSuaNhaCungCapDialog(string maNsxMoi)
        {
            InitializeComponent();
            _vm = new ThemSuaNhaCungCapDialogViewModel(maNsxMoi);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }

        public ThemSuaNhaCungCapDialog(NhaCungCapDisplay nsx)
        {
            InitializeComponent();
            _vm = new ThemSuaNhaCungCapDialogViewModel(nsx);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
