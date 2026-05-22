using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.ViewModels;
using System.Collections.Generic;
using System.Windows;
namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemHoaDonDialog : Window
    {
        private readonly ThemHoaDonDialogViewModel _vm;
        public HoaDon HoaDonMoi => _vm.HoaDonMoi;
        public List<ChiTietHd> ChiTietHds => _vm.ChiTietHds;
        public ThemHoaDonDialog(string maHdMoi)
        {
            InitializeComponent();
            _vm = new ThemHoaDonDialogViewModel(maHdMoi);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}