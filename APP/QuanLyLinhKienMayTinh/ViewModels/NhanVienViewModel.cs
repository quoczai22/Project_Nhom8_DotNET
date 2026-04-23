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
        // ── Backing collection ──────────────────────────────────────────────
        private ObservableCollection<NhanVienDisplay> _all;

        // ── Bound to DataGrid ────────────────────────────────────────────────
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

        // ── ComboBox chức vụ ────────────────────────────────────────────────
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

        // ── Search box ───────────────────────────────────────────────────────
        private string _timKiem = string.Empty;
        public string TimKiem
        {
            get => _timKiem;
            set { _timKiem = value; OnPropertyChanged(); DanhSachNhanVien?.Refresh(); }
        }

        // ── Commands ─────────────────────────────────────────────────────────
        public ICommand ThemNhanVienCommand { get; private set; }
        public ICommand SuaNhanVienCommand { get; private set; }
        public ICommand XoaNhanVienCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        public NhanVienViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }

        // ── Tải dữ liệu ─────────────────────────────────────────────────────
        public void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.DB;

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

        // ── Filter ───────────────────────────────────────────────────────────
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

        // ── ISearchable ──────────────────────────────────────────────────────
        public void ApplySearch(string keyword)
        {
            TimKiem = keyword?.Trim() ?? string.Empty;
        }

        // ── Khởi tạo Commands ────────────────────────────────────────────────
        private void KhoiTaoCommands()
        {
            // ── THÊM ─────────────────────────────────────────────────────
            ThemNhanVienCommand = new RelayCommand<object>(_ => true, _ =>
            {
                try
                {
                    var lastID = DataProvider.Ins.DB.NhanViens
                        .OrderByDescending(x => x.MaNv)
                        .Select(x => x.MaNv).FirstOrDefault();
                    string newID = Services.AutoIDService.GetNextID("NV", lastID);

                    var dialog = new ThemSuaNhanVienDialog(newID);
                    dialog.Owner = Application.Current.MainWindow;
                    if (dialog.ShowDialog() == true)
                    {
                        var nvMoi = new NhanVien
                        {
                            MaNv = dialog.MaNv,
                            TenNv = dialog.HoTen,
                            ChucVu = dialog.ChucVu,
                            GioiTinh = dialog.GioiTinh,
                            Sdt = dialog.Sdt,
                            Email = dialog.Email,
                            NgaySinh = dialog.NgaySinh,
                            NgayVaoLam = dialog.NgayVaoLam,
                            DaNghiViec = false
                        };

                        // Tạo mặc định tài khoản cho nhân viên mới
                        var tkMoi = new TaiKhoan { TenDn = newID, MatKhau = "123", MaNv = newID };

                        DataProvider.Ins.DB.NhanViens.Add(nvMoi);
                        DataProvider.Ins.DB.TaiKhoans.Add(tkMoi);
                        DataProvider.Ins.DB.SaveChanges();

                        TaiDuLieu();
                        MessageBox.Show($"Thêm nhân viên thành công!\nTài khoản mặc định: {newID} / 123",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            // ── SỬA ──────────────────────────────────────────────────────
            SuaNhanVienCommand = new RelayCommand<NhanVienDisplay>(nv => nv != null, nv =>
            {
                try
                {
                    var dialog = new ThemSuaNhanVienDialog(nv);
                    dialog.Owner = Application.Current.MainWindow;
                    if (dialog.ShowDialog() == true)
                    {
                        var entity = DataProvider.Ins.DB.NhanViens.Find(dialog.MaNv);
                        if (entity != null)
                        {
                            entity.TenNv = dialog.HoTen;
                            entity.ChucVu = dialog.ChucVu;
                            entity.GioiTinh = dialog.GioiTinh;
                            entity.Sdt = dialog.Sdt;
                            entity.Email = dialog.Email;
                            entity.NgaySinh = dialog.NgaySinh;
                            entity.NgayVaoLam = dialog.NgayVaoLam;

                            DataProvider.Ins.DB.SaveChanges();
                            TaiDuLieu();

                            MessageBox.Show("Cập nhật nhân viên thành công!",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa nhân viên: " + ex.Message,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            // ── XÓA ──────────────────────────────────────────────────────
            XoaNhanVienCommand = new RelayCommand<NhanVienDisplay>(nv => nv != null, nv =>
            {
                var res = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa nhân viên [{nv.HoTen}] không?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                    ThucHienXoa(nv);
            });

            // ── LÀM MỚI ─────────────────────────────────────────────────
            LamMoiCommand = new RelayCommand<object>(
                _ => true,
                _ => { TimKiem = string.Empty; TaiDuLieu(); });
        }

        // ── Xóa nhân viên ────────────────────────────────────────────────────
        private void ThucHienXoa(NhanVienDisplay nv)
        {
            try
            {
                var db = DataProvider.Ins.DB;
                var entity = db.NhanViens.Find(nv.MaNv);
                if (entity == null) return;

                entity.DaNghiViec = true;
                db.SaveChanges();
                _all.Remove(nv);

                MessageBox.Show("Xóa nhân viên thành công!",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}