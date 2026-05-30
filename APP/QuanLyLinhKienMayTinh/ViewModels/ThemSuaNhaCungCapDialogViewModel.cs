using System;
using System.Windows;
using System.Windows.Input;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class ThemSuaNhaCungCapDialogViewModel : BaseViewModel
    {
        private string _titleText;
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; OnPropertyChanged(); }
        }

        private string _buttonContent;
        public string ButtonContent
        {
            get => _buttonContent;
            set { _buttonContent = value; OnPropertyChanged(); }
        }

        private string _maNsx;
        public string MaNsx
        {
            get => _maNsx;
            set { _maNsx = value; OnPropertyChanged(); }
        }

        private string _tenNsx;
        public string TenNsx
        {
            get => _tenNsx;
            set { _tenNsx = value; OnPropertyChanged(); }
        }

        private string _quocGia;
        public string QuocGia
        {
            get => _quocGia;
            set { _quocGia = value; OnPropertyChanged(); }
        }

        private string _sdt;
        public string Sdt
        {
            get => _sdt;
            set { _sdt = value; OnPropertyChanged(); }
        }

        private bool _isMaNsxReadOnly;
        public bool IsMaNsxReadOnly
        {
            get => _isMaNsxReadOnly;
            set { _isMaNsxReadOnly = value; OnPropertyChanged(); }
        }

        private double _maNsxOpacity = 1.0;
        public double MaNsxOpacity
        {
            get => _maNsxOpacity;
            set { _maNsxOpacity = value; OnPropertyChanged(); }
        }

        public NhaCungCapDisplay KetQua { get; private set; }
        public ICommand LuuCommand { get; private set; }
        public ICommand HuyCommand { get; private set; }
        public Action<bool?> CloseAction { get; set; }

        public ThemSuaNhaCungCapDialogViewModel(string maNsxMoi)
        {
            TitleText = "Thêm Nhà Cung Cấp";
            ButtonContent = "Lưu";
            MaNsx = maNsxMoi;
            IsMaNsxReadOnly = false;
            MaNsxOpacity = 1.0;
            KhoiTaoCommands();
        }

        public ThemSuaNhaCungCapDialogViewModel(NhaCungCapDisplay nsx)
        {
            TitleText = "Sửa Nhà Cung Cấp";
            ButtonContent = "Cập nhật";
            MaNsx = nsx.MaNsx;
            IsMaNsxReadOnly = true;
            MaNsxOpacity = 0.6;
            TenNsx = nsx.TenNsx;
            QuocGia = nsx.QuocGia;
            Sdt = nsx.Sdt;
            KhoiTaoCommands();
        }

        private void KhoiTaoCommands()
        {
            LuuCommand = new RelayCommand<object>(CanLuu, ThucHienLuu);
            HuyCommand = new RelayCommand<object>(CanHuy, ThucHienHuy);
        }

        private bool CanLuu(object parameter)
        {
            return true;
        }

        private bool CanHuy(object parameter)
        {
            return true;
        }

        private void ThucHienLuu(object parameter)
        {
            if (string.IsNullOrWhiteSpace(TenNsx))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            KetQua = new NhaCungCapDisplay
            {
                MaNsx = MaNsx?.Trim(),
                TenNsx = TenNsx.Trim(),
                QuocGia = QuocGia?.Trim(),
                Sdt = Sdt?.Trim()
            };
            CloseAction?.Invoke(true);
        }

        private void ThucHienHuy(object parameter)
        {
            CloseAction?.Invoke(false);
        }
    }
}
