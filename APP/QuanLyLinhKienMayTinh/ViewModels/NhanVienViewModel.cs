using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    // Display class khớp với binding trong NhanVienView.xaml
    public class NhanVienDisplay
    {
        public string MaNv { get; set; }
        public string HoTen { get; set; }   // map TenNv
        public string ChucVu { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }   // map TenDn (login name)
        public DateOnly? NgayVaoLam { get; set; } // map NgaySinh
    }

    // Item cho ComboBox lọc chức vụ
    public class ChucVuItem
    {
        public string TenChucVu { get; set; }
        public override string ToString() => TenChucVu;
    }

    public class NhanVienViewModel : BaseViewModel, ISearchable
    {
        // Backing collection 
        private ObservableCollection<NhanVienDisplay> _all;

        // Bound to DataGrid 
        private ICollectionView _danhSachNhanVien;
        public ICollectionView DanhSachNhanVien
        {
            get => _danhSachNhanVien;
            set { _danhSachNhanVien = value; OnPropertyChanged(); }
        }

        private NhanVienDisplay _nhanVienChon;
        public NhanVienDisplay NhanVienChon
        {
            get => _nhanVienChon;
            set { _nhanVienChon = value; OnPropertyChanged(); }
        }

        // ComboBox chức vụ
        private ObservableCollection<ChucVuItem> _danhSachChucVu;
        public ObservableCollection<ChucVuItem> DanhSachChucVu
        {
            get => _danhSachChucVu;
            set { _danhSachChucVu = value; OnPropertyChanged(); }
        }

        private ChucVuItem _chucVuChon;
        public ChucVuItem ChucVuChon
        {
            get => _chucVuChon;
            set { _chucVuChon = value; OnPropertyChanged(); DanhSachNhanVien?.Refresh(); }
        }

        // Search box 
        private string _timKiem = string.Empty;
        public string TimKiem
        {
            get => _timKiem;
            set { _timKiem = value; OnPropertyChanged(); DanhSachNhanVien?.Refresh(); }
        }

        // Commands 
        public ICommand ThemNhanVienCommand { get; private set; }
        public ICommand SuaNhanVienCommand { get; private set; }
        public ICommand XoaNhanVienCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        public NhanVienViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }

        // Lấy danh sách nhân viên đang làm việc từ cơ sở dữ liệu và tải danh mục chức vụ
        public void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();

                var list = db.NhanViens
                .AsNoTracking()
                .Where(x => x.DaNghiViec == false)
                .Select(nv => new NhanVienDisplay
                {
                    MaNv = nv.MaNv,
                    HoTen = nv.TenNv,
                    ChucVu = nv.ChucVu,
                    Sdt = nv.Sdt,
                    Email = nv.Email,
                    NgayVaoLam = nv.NgayVaoLam
                }).ToList();

                _all = new ObservableCollection<NhanVienDisplay>(list);
                DanhSachNhanVien = CollectionViewSource.GetDefaultView(_all);
                DanhSachNhanVien.Filter = Filter;

                // Danh sách chức vụ cho ComboBox
                var cacChucVu = db.NhanViens
                    .AsNoTracking()
                    .Where(nv => nv.ChucVu != null)
                    .Select(nv => nv.ChucVu)
                    .Distinct()
                    .OrderBy(cv => cv)
                    .Select(cv => new ChucVuItem { TenChucVu = cv })
                    .ToList();

                cacChucVu.Insert(0, new ChucVuItem { TenChucVu = "-- Tất cả --" });
                DanhSachChucVu = new ObservableCollection<ChucVuItem>(cacChucVu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu nhân viên: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lọc danh sách nhân viên theo từ khóa tìm kiếm và chức vụ được chọn
        private bool Filter(object obj)
        {
            if (obj is not NhanVienDisplay item) return false;

            // Lọc theo từ khóa tìm kiếm
            bool matchSearch = string.IsNullOrWhiteSpace(TimKiem)
                || (item.MaNv?.ToLower().Contains(TimKiem.ToLower()) ?? false)
                || (item.HoTen?.ToLower().Contains(TimKiem.ToLower()) ?? false)
                || (item.Sdt?.ToLower().Contains(TimKiem.ToLower()) ?? false)
                || (item.Email?.ToLower().Contains(TimKiem.ToLower()) ?? false);

            // Lọc theo chức vụ
            bool matchChucVu = ChucVuChon == null
                || ChucVuChon.TenChucVu == "-- Tất cả --"
                || item.ChucVu == ChucVuChon.TenChucVu;

            return matchSearch && matchChucVu;
        }

        // Cập nhật từ khóa tìm kiếm từ giao diện và áp dụng lại bộ lọc hiển thị
        public void ApplySearch(string keyword)
        {
            TimKiem = keyword?.Trim() ?? string.Empty;
        }

        // Cấu hình các thao tác Thêm, Sửa, Xóa và Làm mới cho các nút bấm trên giao diện
        private void KhoiTaoCommands()
        {
            ThemNhanVienCommand = new RelayCommand<object>(CanThemNhanVien, ThucHienThemNhanVien);
            SuaNhanVienCommand = new RelayCommand<NhanVienDisplay>(CanSuaNhanVien, ThucHienSuaNhanVien);
            XoaNhanVienCommand = new RelayCommand<NhanVienDisplay>(CanXoaNhanVien, ThucHienXoaNhanVien);
            LamMoiCommand = new RelayCommand<object>(CanLamMoi, ThucHienLamMoi);
        }

        private bool CanThemNhanVien(object parameter)
        {
            return true;
        }

        private bool CanSuaNhanVien(NhanVienDisplay nv)
        {
            return nv != null;
        }

        private bool CanXoaNhanVien(NhanVienDisplay nv)
        {
            return nv != null;
        }

        private bool CanLamMoi(object parameter)
        {
            return true;
        }

        private void ThucHienLamMoi(object parameter)
        {
            TimKiem = string.Empty;
            TaiDuLieu();
        }

        // Xử lý thêm nhân viên mới: Tự động tính toán mã, phân quyền theo chức vụ và cấp tài khoản mặc định
        private void ThucHienThemNhanVien(object parameter)
        {
            if (LuuTrangThai.QuyenDangNhap != "Quản lý toàn bộ")
            {
                MessageBox.Show("Chỉ tài khoản quản lý (machpv) mới được phép thêm nhân viên!", "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = DataProvider.Ins.GetContext(); // Sử dụng 1 DbContext duy nhất cho toàn bộ giao dịch

                // Tạo mã gợi ý tự động
                var lastID = db.NhanViens
                    .OrderByDescending(x => x.MaNv)
                    .Select(x => x.MaNv).FirstOrDefault();
                string newID = Services.AutoIDService.GetNextID("NV", lastID);

                var dialog = new ThemSuaNhanVienDialog(newID);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    string quyen = LayQuyenTuChucVu(dialog.ChucVu);

                    var nvMoi = new NhanVien
                    {
                        MaNv = dialog.MaNv,
                        TenNv = dialog.HoTen,
                        ChucVu = dialog.ChucVu,
                        Quyen = quyen,
                        GioiTinh = dialog.GioiTinh,
                        Sdt = dialog.Sdt,
                        Email = dialog.Email,
                        NgaySinh = dialog.NgaySinh,
                        NgayVaoLam = dialog.NgayVaoLam,
                        DaNghiViec = false
                    };

                    // Tạo mặc định tài khoản đăng nhập cho nhân viên mới
                    var tkMoi = new TaiKhoan { TenDn = dialog.MaNv, MatKhau = "123", MaNv = dialog.MaNv };

                    db.NhanViens.Add(nvMoi);
                    db.TaiKhoans.Add(tkMoi);
                    db.SaveChanges();

                    TaiDuLieu();
                    MessageBox.Show($"Thêm nhân viên thành công!\nTài khoản mặc định: {dialog.MaNv} / 123",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý cập nhật thông tin nhân viên đã chọn và phân quyền lại dựa trên chức vụ mới
        private void ThucHienSuaNhanVien(NhanVienDisplay nv)
        {
            if (LuuTrangThai.QuyenDangNhap != "Quản lý toàn bộ")
            {
                MessageBox.Show("Chỉ tài khoản quản lý (machpv) mới được phép sửa nhân viên!", "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dialog = new ThemSuaNhanVienDialog(nv);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    using var db = DataProvider.Ins.GetContext();
                    var entity = db.NhanViens.Find(dialog.MaNv);
                    if (entity != null)
                    {
                        entity.TenNv = dialog.HoTen;
                        entity.ChucVu = dialog.ChucVu;
                        entity.Quyen = LayQuyenTuChucVu(dialog.ChucVu);
                        entity.GioiTinh = dialog.GioiTinh;
                        entity.Sdt = dialog.Sdt;
                        entity.Email = dialog.Email;
                        entity.NgaySinh = dialog.NgaySinh;
                        entity.NgayVaoLam = dialog.NgayVaoLam;

                        db.SaveChanges();
                        TaiDuLieu();

                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý đánh dấu nhân viên đã nghỉ việc (Soft delete) thay vì xóa hoàn toàn dữ liệu khỏi hệ thống
        private void ThucHienXoaNhanVien(NhanVienDisplay nv)
        {
            if (LuuTrangThai.QuyenDangNhap != "Quản lý toàn bộ")
            {
                MessageBox.Show("Chỉ tài khoản quản lý (machpv) mới được phép xóa nhân viên!", "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên [{nv.HoTen}] không?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;

            try
            {
                using var db = DataProvider.Ins.GetContext();
                var entity = db.NhanViens.Find(nv.MaNv);
                if (entity == null) return;

                entity.DaNghiViec = true; // Soft-delete (nghỉ việc)
                db.SaveChanges();

                _all.Remove(nv);

                MessageBox.Show("Xóa nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Xác định quyền dựa trên chức vụ</summary>
        private string LayQuyenTuChucVu(string chucVu)
        {
            return chucVu switch
            {
                "Quản lý" => "Quản lý toàn bộ",
                "Nhân viên thu ngân" => "Thu ngân",
                "Nhân viên bán hàng" => "Bán hàng",
                "Nhân viên kỹ thuật" => "Kỹ thuật",
                "Nhân viên kho" => "Kho",
                _ => "Bán hàng"
            };
        }
    }
}