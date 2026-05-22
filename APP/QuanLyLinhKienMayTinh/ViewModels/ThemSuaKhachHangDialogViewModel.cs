using QuanLyLinhKienMayTinh.Models;
using System;
using System.Windows;
using System.Windows.Input;
namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class ThemSuaKhachHangDialogViewModel : BaseViewModel
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
        private string _maKh;
        public string MaKh
        {
            get => _maKh;
            set { _maKh = value; OnPropertyChanged(); }
        }
        private string _hoTen;
        public string HoTen
        {
            get => _hoTen;
            set { _hoTen = value; OnPropertyChanged(); }
        }
        private string _sdt;
        public string Sdt
        {
            get => _sdt;
            set { _sdt = value; OnPropertyChanged(); }
        }
        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        private string _diaChi;
        public string DiaChi
        {
            get => _diaChi;
            set { _diaChi = value; OnPropertyChanged(); }
        }
        private bool _isMaKhReadOnly;
        public bool IsMaKhReadOnly
        {
            get => _isMaKhReadOnly;
            set { _isMaKhReadOnly = value; OnPropertyChanged(); }
        }
        private double _maKhOpacity = 1.0;
        public double MaKhOpacity
        {
            get => _maKhOpacity;
            set { _maKhOpacity = value; OnPropertyChanged(); }
        }
        public KhachHangDisplay KetQua { get; private set; }
        // Lệnh xác nhận lưu thông tin khách hàng sau khi kiểm tra dữ liệu hợp lệ
        public ICommand LuuCommand { get; private set; }

        // Lệnh hủy bỏ thao tác và đóng cửa sổ mà không lưu gì
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng cửa sổ dialog, được gán từ View để ViewModel có thể yêu cầu đóng
        public Action<bool?> CloseAction { get; set; }
        // Constructors
        public ThemSuaKhachHangDialogViewModel(string maKhMoi)
        {
            TitleText = "Thêm Khách Hàng";
            ButtonContent = "Lưu";
            MaKh = maKhMoi;
            IsMaKhReadOnly = false;
            MaKhOpacity = 1.0;
            KhoiTaoCommands();
        }
        public ThemSuaKhachHangDialogViewModel(KhachHangDisplay kh)
        {
            TitleText = "Sửa Khách Hàng";
            ButtonContent = "Cập nhật";
            MaKh = kh.MaKh;
            IsMaKhReadOnly = true;
            MaKhOpacity = 0.6;
            HoTen = kh.HoTen;
            Sdt = kh.Sdt;
            Email = kh.Email;
            DiaChi = kh.DiaChi;
            KhoiTaoCommands();
        }
        // Gắn kết các nút Lưu và Hủy trên giao diện với hành động xử lý tương ứng
        private void KhoiTaoCommands()
        {
            LuuCommand = new RelayCommand<object>(CanLuu, ThucHienLuu);
            HuyCommand = new RelayCommand<object>(CanHuy, ThucHienHuy);
        }

        // Kiểm tra điều kiện: Luôn cho phép nhấn nút Lưu
        private bool CanLuu(object parameter)
        {
            return true;
        }

        // Kiểm tra điều kiện: Luôn cho phép hủy bỏ thao tác
        private bool CanHuy(object parameter)
        {
            return true;
        }
        // Kiểm tra họ tên bắt buộc rồi đóng gói kết quả và thông báo cho View đóng cửa sổ
        private void ThucHienLuu(object parameter)
        {
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            KetQua = new KhachHangDisplay
            {
                MaKh = MaKh?.Trim(),
                HoTen = HoTen.Trim(),
                Sdt = Sdt?.Trim(),
                Email = Email?.Trim(),
                DiaChi = DiaChi?.Trim()
            };
            CloseAction?.Invoke(true);
        }
        // Đóng cửa sổ mà không lưu bất kỳ thay đổi nào
        private void ThucHienHuy(object parameter)
        {
            CloseAction?.Invoke(false);
        }
    }
}