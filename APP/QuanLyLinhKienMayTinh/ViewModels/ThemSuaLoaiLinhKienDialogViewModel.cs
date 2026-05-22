using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class ThemSuaLoaiLinhKienDialogViewModel : BaseViewModel
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
        private string _maLoai;
        public string MaLoai
        {
            get => _maLoai;
            set { _maLoai = value; OnPropertyChanged(); }
        }
        private string _tenLoai;
        public string TenLoai
        {
            get => _tenLoai;
            set { _tenLoai = value; OnPropertyChanged(); }
        }
        private string _moTa;
        public string MoTa
        {
            get => _moTa;
            set { _moTa = value; OnPropertyChanged(); }
        }
        private bool _isMaLoaiReadOnly;
        public bool IsMaLoaiReadOnly
        {
            get => _isMaLoaiReadOnly;
            set { _isMaLoaiReadOnly = value; OnPropertyChanged(); }
        }
        private double _maLoaiOpacity = 1.0;
        public double MaLoaiOpacity
        {
            get => _maLoaiOpacity;
            set { _maLoaiOpacity = value; OnPropertyChanged(); }
        }
        private Visibility _maLoaiHintVisibility;
        public Visibility MaLoaiHintVisibility
        {
            get => _maLoaiHintVisibility;
            set { _maLoaiHintVisibility = value; OnPropertyChanged(); }
        }
        public LoaiLkDisplay KetQua { get; private set; }
        private readonly bool _laMoiThem;
        // Lệnh xác nhận lưu thông tin loại linh kiện sau khi kiểm tra dữ liệu hợp lệ
        public ICommand LuuCommand { get; private set; }

        // Lệnh hủy bỏ thao tác và đóng cửa sổ mà không lưu gì
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng cửa sổ dialog, được gán từ View để ViewModel có thể yêu cầu đóng
        public Action<bool?> CloseAction { get; set; }
        // Constructors
        public ThemSuaLoaiLinhKienDialogViewModel(string maLoaiGoi)
        {
            _laMoiThem = true;
            TitleText = "Thêm Loại Linh Kiện";
            ButtonContent = "Lưu";
            MaLoai = string.Empty;
            IsMaLoaiReadOnly = false;
            MaLoaiOpacity = 1.0;
            MaLoaiHintVisibility = Visibility.Visible;
            KhoiTaoCommands();
        }
        public ThemSuaLoaiLinhKienDialogViewModel(LoaiLkDisplay loai)
        {
            _laMoiThem = false;
            TitleText = "Sửa Loại Linh Kiện";
            ButtonContent = "Cập nhật";
            MaLoai = loai.MaLoai;
            IsMaLoaiReadOnly = true;
            MaLoaiOpacity = 0.6;
            MaLoaiHintVisibility = Visibility.Collapsed;
            TenLoai = loai.TenLoai;
            MoTa = loai.MoTa;
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
        // Kiểm tra dữ liệu nhập vào, đóng gói kết quả và thông báo cho View đóng cửa sổ
        private void ThucHienLuu(object parameter)
        {
            string maLoai = MaLoai?.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(maLoai))
            {
                MessageBox.Show("Vui lòng nhập mã loại linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TenLoai))
            {
                MessageBox.Show("Vui lòng nhập tên loại linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Kiểm tra trùng mã khi thêm mới
            if (_laMoiThem)
            {
                bool trung = DataProvider.Ins.GetContext().LoaiLks
                    .AsNoTracking()
                    .Any(l => l.MaLoai == maLoai);
                if (trung)
                {
                    MessageBox.Show($"Mã loại '{maLoai}' đã tồn tại! Vui lòng nhập mã khác.",
                        "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            KetQua = new LoaiLkDisplay
            {
                MaLoai = maLoai,
                TenLoai = TenLoai.Trim(),
                MoTa = MoTa?.Trim()
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