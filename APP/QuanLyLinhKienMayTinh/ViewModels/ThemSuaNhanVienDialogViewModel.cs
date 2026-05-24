using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class ThemSuaNhanVienDialogViewModel : BaseViewModel
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

        private string _maNv;
        public string MaNv
        {
            get => _maNv;
            set { _maNv = value; OnPropertyChanged(); }
        }

        private string _hoTen;
        public string HoTen
        {
            get => _hoTen;
            set { _hoTen = value; OnPropertyChanged(); }
        }

        private ChucVuItem _selectedChucVu;
        public ChucVuItem SelectedChucVu
        {
            get => _selectedChucVu;
            set { _selectedChucVu = value; OnPropertyChanged(); }
        }

        private string _selectedGioiTinh;
        public string SelectedGioiTinh
        {
            get => _selectedGioiTinh;
            set { _selectedGioiTinh = value; OnPropertyChanged(); }
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

        private DateTime? _ngaySinh;
        public DateTime? NgaySinh
        {
            get => _ngaySinh;
            set { _ngaySinh = value; OnPropertyChanged(); }
        }

        private DateTime? _ngayVaoLam;
        public DateTime? NgayVaoLam
        {
            get => _ngayVaoLam;
            set { _ngayVaoLam = value; OnPropertyChanged(); }
        }

        private bool _isMaNvReadOnly;
        public bool IsMaNvReadOnly
        {
            get => _isMaNvReadOnly;
            set { _isMaNvReadOnly = value; OnPropertyChanged(); }
        }

        private double _maNvOpacity = 1.0;
        public double MaNvOpacity
        {
            get => _maNvOpacity;
            set { _maNvOpacity = value; OnPropertyChanged(); }
        }

        public List<ChucVuItem> DanhSachChucVu { get; set; }
        public List<string> DanhSachGioiTinh { get; set; } = new List<string> { "Nam", "Nữ" };

        // Lệnh xác nhận lưu thông tin nhân viên sau khi kiểm tra dữ liệu hợp lệ
        public ICommand LuuCommand { get; private set; }

        // Lệnh hủy bỏ thao tác và đóng cửa sổ mà không lưu gì
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng cửa sổ dialog, được gán từ View để ViewModel có thể yêu cầu đóng
        public Action<bool?> CloseAction { get; set; }

        public ThemSuaNhanVienDialogViewModel(string maNvMoi)
        {
            TitleText = "Thêm Nhân Viên";
            ButtonContent = "Lưu";
            MaNv = maNvMoi;
            NgayVaoLam = DateTime.Now;
            IsMaNvReadOnly = false;
            MaNvOpacity = 1.0;

            TaiDanhSachChucVu();
            KhoiTaoCommands();
        }

        public ThemSuaNhanVienDialogViewModel(NhanVienDisplay nv)
        {
            TitleText = "Sửa Nhân Viên";
            ButtonContent = "Cập nhật";
            MaNv = nv.MaNv;
            IsMaNvReadOnly = true;
            MaNvOpacity = 0.6;
            HoTen = nv.HoTen;
            Sdt = nv.Sdt;
            Email = nv.Email;

            if (nv.NgayVaoLam.HasValue)
                NgayVaoLam = nv.NgayVaoLam.Value.ToDateTime(TimeOnly.MinValue);

            TaiDanhSachChucVu();

            // Chọn sẵn chức vụ hiện tại của nhân viên
            if (!string.IsNullOrEmpty(nv.ChucVu))
            {
                SelectedChucVu = DanhSachChucVu.FirstOrDefault(cv => cv.TenChucVu == nv.ChucVu);
            }

            // Nạp thêm giới tính và ngày sinh từ cơ sở dữ liệu
            using (var db = DataProvider.Ins.GetContext())
            {
                var entity = db.NhanViens.Find(nv.MaNv);
                if (entity != null)
                {
                    if (!string.IsNullOrEmpty(entity.GioiTinh))
                    {
                        SelectedGioiTinh = DanhSachGioiTinh.FirstOrDefault(gt => gt == entity.GioiTinh);
                    }
                    if (entity.NgaySinh.HasValue)
                        NgaySinh = entity.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
                }
            }

            KhoiTaoCommands();
        }

        // Tải danh sách chức vụ từ cơ sở dữ liệu, kết hợp với các chức vụ mặc định của hệ thống
        private void TaiDanhSachChucVu()
        {
            try
            {
                // Lấy các chức vụ từ DB (distinct)
                var cacChucVuDb = DataProvider.Ins.GetContext().NhanViens
                    .AsNoTracking()
                    .Where(nv => nv.ChucVu != null && nv.ChucVu != "")
                    .Select(nv => nv.ChucVu)
                    .Distinct()
                    .OrderBy(cv => cv)
                    .ToList();

                // Đảm bảo luôn có các chức vụ cơ bản
                var chucVuMacDinh = new List<string>
                {
                    "Quản lý",
                    "Nhân viên thu ngân",
                    "Nhân viên chăm sóc khách hàng",
                    "Nhân viên kho"
                };

                DanhSachChucVu = cacChucVuDb.Union(chucVuMacDinh)
                    .OrderBy(cv => cv)
                    .Select(cv => new ChucVuItem { TenChucVu = cv })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách chức vụ: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachChucVu = new List<ChucVuItem>();
            }
        }

        // Gắn kết các nút Lưu và Hủy trên giao diện với hành động xử lý tương ứng
        private void KhoiTaoCommands()
        {
            LuuCommand = new RelayCommand<object>(CanLuu, ThucHienLuu);
            HuyCommand = new RelayCommand<object>(CanHuy, ThucHienHuy);
        }

        // Kiểm tra điều kiện: Luôn cho phép nhấn nút Lưu, việc kiểm tra chi tiết sẽ xử lý bên trong
        private bool CanLuu(object parameter)
        {
            return true;
        }

        // Kiểm tra điều kiện: Luôn cho phép hủy bỏ thao tác
        private bool CanHuy(object parameter)
        {
            return true;
        }

        // Kiểm tra thông tin bắt buộc rồi thông báo cho View đóng cửa sổ với kết quả thành công
        private void ThucHienLuu(object parameter)
        {
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedChucVu == null)
            {
                MessageBox.Show("Vui lòng chọn chức vụ!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CloseAction?.Invoke(true);
        }

        // Đóng cửa sổ mà không lưu bất kỳ thay đổi nào
        private void ThucHienHuy(object parameter)
        {
            CloseAction?.Invoke(false);
        }
    }
}