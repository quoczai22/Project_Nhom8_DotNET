using QuanLyLinhKienMayTinh.ViewModels;
using System;
using System.Windows;
namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaLinhKienDialog : Window
    {
        private readonly ThemSuaLinhKienDialogViewModel _vm;
        public string MaLk => _vm.ResultMaLk;
        public string TenLk => _vm.ResultTenLk;
        public string MaLoai => _vm.ResultMaLoai;
        public string MaNsx => _vm.ResultMaNsx;
        public string Dvt => _vm.ResultDvt;
        public byte? Tgbh => _vm.ResultTgbh;
        public int? DonGiaBan => _vm.ResultDonGiaBan;
        public int? SoLuongTon => _vm.ResultSoLuongTon;
        public DateOnly? NgayNhap => _vm.ResultNgayNhap;
        /// Mở ở chế độ THÊM
        public ThemSuaLinhKienDialog(string maLkGoiY)
        {
            InitializeComponent();
            _vm = new ThemSuaLinhKienDialogViewModel(maLkGoiY);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
        ///Mở ở chế độ SỬA
        public ThemSuaLinhKienDialog(LinhKienDisplay lk)
        {
            InitializeComponent();
            _vm = new ThemSuaLinhKienDialogViewModel(lk);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}