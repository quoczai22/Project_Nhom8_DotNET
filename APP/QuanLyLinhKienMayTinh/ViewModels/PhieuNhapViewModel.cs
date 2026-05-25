using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Views;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    // Display class cho danh sách phiếu nhập (DataGrid trái)
    public class PhieuNhapDisplay
    {
        public string MaPN { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime? NgayNhap { get; set; }
        public int? TongTien { get; set; }
        public int? TongSoLuong { get; set; }
        public string TenNhaCungCap { get; set; } 
        public string SoDienThoaiNCC { get; set; } 
    }

    // Display class cho chi tiết linh kiện trong phiếu nhập (DataGrid panel phải)
    public class ChiTietLinhKienNhapDisplay
    {
        public string TenLinhKien { get; set; }
        public int? SoLuongNhap { get; set; }
        public int? DonGiaNhap { get; set; }
    }

    public class PhieuNhapViewModel : BaseViewModel, ISearchable
    {
        // ── Backing data ──────────────────────────────────────────────────────
        private ObservableCollection<PhieuNhapDisplay> _all;

        // ── Danh sách phiếu nhập (DataGrid) ──────────────────────────────────
        private ObservableCollection<PhieuNhapDisplay> _danhSachPhieuNhap;
        public ObservableCollection<PhieuNhapDisplay> DanhSachPhieuNhap
        {
            get => _danhSachPhieuNhap;
            set { _danhSachPhieuNhap = value; OnPropertyChanged(); }
        }

        // ── Phiếu nhập đang chọn ──────────────────────────────────────────────
        private PhieuNhapDisplay _phieuNhapChon;
        public PhieuNhapDisplay PhieuNhapChon
        {
            get => _phieuNhapChon;
            set
            {
                _phieuNhapChon = value;
                OnPropertyChanged();
                ChiTietVisibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                ChuaChonPhieuVisibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                TaiChiTietLinhKien(value?.MaPN);
            }
        }

        // ── Chi tiết linh kiện trong phiếu nhập ──────────────────────────────
        private ObservableCollection<ChiTietLinhKienNhapDisplay> _chiTietLinhKienNhap;
        public ObservableCollection<ChiTietLinhKienNhapDisplay> ChiTietLinhKienNhap
        {
            get => _chiTietLinhKienNhap;
            set { _chiTietLinhKienNhap = value; OnPropertyChanged(); }
        }

        // ── Visibility panel chi tiết ─────────────────────────────────────────
        private Visibility _chiTietVisibility = Visibility.Collapsed;
        public Visibility ChiTietVisibility
        {
            get => _chiTietVisibility;
            set { _chiTietVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _chuaChonPhieuVisibility = Visibility.Visible;
        public Visibility ChuaChonPhieuVisibility
        {
            get => _chuaChonPhieuVisibility;
            set { _chuaChonPhieuVisibility = value; OnPropertyChanged(); }
        }

        // ── Toolbar: search, lọc ngày ─────────────────────────────────────────
        private string _tuKhoanTimKiem = string.Empty;
        public string TuKhoanTimKiem
        {
            get => _tuKhoanTimKiem;
            set { _tuKhoanTimKiem = value; OnPropertyChanged(); }
        }

        private DateTime? _tuNgay;
        public DateTime? TuNgay
        {
            get => _tuNgay;
            set { _tuNgay = value; OnPropertyChanged(); }
        }

        private DateTime? _denNgay;
        public DateTime? DenNgay
        {
            get => _denNgay;
            set { _denNgay = value; OnPropertyChanged(); }
        }

        // ── Footer thống kê ───────────────────────────────────────────────────
        private int _tongSoPhieuNhap;
        public int TongSoPhieuNhap
        {
            get => _tongSoPhieuNhap;
            set { _tongSoPhieuNhap = value; OnPropertyChanged(); }
        }

        private long _tongChiPhiNhap;
        public long TongChiPhiNhap
        {
            get => _tongChiPhiNhap;
            set { _tongChiPhiNhap = value; OnPropertyChanged(); }
        }

        private int _tongSoLuongLinhKienNhap;
        public int TongSoLuongLinhKienNhap
        {
            get => _tongSoLuongLinhKienNhap;
            set { _tongSoLuongLinhKienNhap = value; OnPropertyChanged(); }
        }

        private int _soPhieuNhapThangNay;
        public int SoPhieuNhapThangNay
        {
            get => _soPhieuNhapThangNay;
            set { _soPhieuNhapThangNay = value; OnPropertyChanged(); }
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand TaoPhieuNhapMoiCommand { get; private set; }
        public ICommand InPhieuNhapCommand { get; private set; }
        public ICommand XoaPhieuNhapCommand { get; private set; }
        public ICommand LocPhieuNhapCommand { get; private set; }

        // ── Constructor ───────────────────────────────────────────────────────
        public PhieuNhapViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }

        // ── Tải dữ liệu ──────────────────────────────────────────────────────
        public void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();

                var list = db.PhieuNhaps
                    .AsNoTracking()
                    .Include(pn => pn.MaNvNavigation)
                    .Include(pn => pn.MaNsxNavigation)
                    .Include(pn => pn.ChiTietPns)
                        .ThenInclude(ct => ct.MaLkNavigation)
                    .OrderByDescending(pn => pn.NgayNhap)
                    .ToList()
                    .Select(pn => MapToDisplay(pn))
                    .ToList();

                _all = new ObservableCollection<PhieuNhapDisplay>(list);
                DanhSachPhieuNhap = new ObservableCollection<PhieuNhapDisplay>(list);

                CapNhatThongKe(_all);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu phiếu nhập: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Tải chi tiết linh kiện ────────────────────────────────────────────
        private void TaiChiTietLinhKien(string maPn)
        {
            if (string.IsNullOrEmpty(maPn))
            {
                ChiTietLinhKienNhap = new ObservableCollection<ChiTietLinhKienNhapDisplay>();
                return;
            }

            try
            {
                var db = DataProvider.Ins.GetContext();

                var chiTiet = db.ChiTietPns
                    .AsNoTracking()
                    .Include(ct => ct.MaLkNavigation)
                    .Where(ct => ct.MaPn == maPn)
                    .ToList()
                    .Select(ct => new ChiTietLinhKienNhapDisplay
                    {
                        TenLinhKien = ct.MaLkNavigation?.TenLk ?? "Linh kiện ẩn",
                        SoLuongNhap = ct.SoLuongNhap,
                        DonGiaNhap = ct.DonGiaNhap
                    })
                    .ToList();

                ChiTietLinhKienNhap = new ObservableCollection<ChiTietLinhKienNhapDisplay>(chiTiet);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết phiếu nhập: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Map model → display ───────────────────────────────────────────────
        private static PhieuNhapDisplay MapToDisplay(PhieuNhap pn)
        {
            int tongTien = pn.ChiTietPns.Sum(ct => (ct.SoLuongNhap ?? 0) * (ct.DonGiaNhap ?? 0));
            int tongSoLuong = pn.ChiTietPns.Sum(ct => ct.SoLuongNhap ?? 0);

            DateTime? ngayNhap = pn.NgayNhap.HasValue
                ? pn.NgayNhap.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null;
            return new PhieuNhapDisplay
            {
                MaPN = pn.MaPn,
                TenNhanVien = pn.MaNvNavigation?.TenNv ?? pn.MaNv,
                NgayNhap = ngayNhap,
                TongTien = tongTien,
                TongSoLuong = tongSoLuong,
                TenNhaCungCap = pn.MaNsxNavigation?.TenNsx ?? pn.MaNsx,
                SoDienThoaiNCC = pn.MaNsxNavigation?.Sdt ?? string.Empty
            };
        }

        // ── Lọc dữ liệu ──────────────────────────────────────────────────────
        private void LocPhieuNhap()
        {
            var filtered = _all.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TuKhoanTimKiem))
            {
                string kw = TuKhoanTimKiem.ToLower();
                filtered = filtered.Where(pn =>
                    (pn.MaPN?.ToLower().Contains(kw) ?? false) ||
                    (pn.TenNhanVien?.ToLower().Contains(kw) ?? false));
            }

            if (TuNgay.HasValue)
                filtered = filtered.Where(pn => pn.NgayNhap >= TuNgay.Value);
            if (DenNgay.HasValue)
                filtered = filtered.Where(pn => pn.NgayNhap <= DenNgay.Value.AddDays(1));

            var result = filtered.ToList();
            DanhSachPhieuNhap = new ObservableCollection<PhieuNhapDisplay>(result);
            CapNhatThongKe(DanhSachPhieuNhap);
            PhieuNhapChon = null;
        }

        // ── Thống kê footer ───────────────────────────────────────────────────
        private void CapNhatThongKe(IEnumerable<PhieuNhapDisplay> ds)
        {
            var list = ds.ToList();
            var now = DateTime.Now;

            TongSoPhieuNhap = list.Count;
            TongChiPhiNhap = list.Sum(pn => (long)(pn.TongTien ?? 0));
            TongSoLuongLinhKienNhap = list.Sum(pn => pn.TongSoLuong ?? 0);
            SoPhieuNhapThangNay = list.Count(pn =>
                pn.NgayNhap.HasValue &&
                pn.NgayNhap.Value.Month == now.Month &&
                pn.NgayNhap.Value.Year == now.Year);
        }

        // ── ISearchable ───────────────────────────────────────────────────────
        public void ApplySearch(string keyword)
        {
            TuKhoanTimKiem = keyword?.Trim() ?? string.Empty;
            LocPhieuNhap();
        }

        // ── Khởi tạo commands ─────────────────────────────────────────────────
        private void KhoiTaoCommands()
        {
            TaoPhieuNhapMoiCommand = new RelayCommand<object>(_ => true, ThucHienTaoPhieuNhap);
            InPhieuNhapCommand = new RelayCommand<object>(_ => PhieuNhapChon != null, ThucHienInPhieuNhap);
            XoaPhieuNhapCommand = new RelayCommand<object>(_ => PhieuNhapChon != null, ThucHienXoaPhieuNhap);
            LocPhieuNhapCommand = new RelayCommand<object>(_ => true, _ => LocPhieuNhap());
        }

        // ── Tạo phiếu nhập mới ────────────────────────────────────────────────
        private void ThucHienTaoPhieuNhap(object parameter)
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                var lastID = db.PhieuNhaps
                    .OrderByDescending(x => x.MaPn)
                    .Select(x => x.MaPn)
                    .FirstOrDefault();
                string newID = AutoIDService.GetNextID("PN", lastID);

                var dialog = new ThemPhieuNhapDialog(newID);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    LuuPhieuNhapAnToan(dialog.PhieuNhapMoi, dialog.ChiTietPns);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo phiếu nhập: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── In phiếu nhập ─────────────────────────────────────────────────────
        private void ThucHienInPhieuNhap(object parameter)
        {
            var pn = PhieuNhapChon;
            var chiTiet = ChiTietLinhKienNhap;

            string content = $"PHIẾU NHẬP KHO\n" +
                                $"Mã PN: {pn.MaPN}\n" +
                                $"Ngày nhập: {pn.NgayNhap:dd/MM/yyyy}\n" +
                                $"Nhân viên: {pn.TenNhanVien}\n" +
                                "------------------------------------------\n" +
                                "Linh kiện\t\tSL\tĐơn giá nhập\n";

            foreach (var item in chiTiet)
                content += $"{item.TenLinhKien}\t{item.SoLuongNhap}\t{item.DonGiaNhap:N0}\n";

            content += "------------------------------------------\n" +
                        $"TỔNG CHI PHÍ: {pn.TongTien:N0} VNĐ";

            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"PhieuNhap_{pn.MaPN}.txt",
                Filter = "Text File (*.txt)|*.txt"
            };

            if (sfd.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(sfd.FileName, content);
                MessageBox.Show("Đã xuất phiếu nhập thành công!", "In phiếu nhập");
            }
        }

        // ── Xóa phiếu nhập ───────────────────────────────────────────────────
        private void ThucHienXoaPhieuNhap(object parameter)
        {
            if (PhieuNhapChon == null) return;

            var res = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa phiếu nhập [{PhieuNhapChon.MaPN}] không?\n" +
                "Số lượng linh kiện trong kho sẽ bị trừ lại.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
                ThucHienXoa(PhieuNhapChon);
        }

        private void ThucHienXoa(PhieuNhapDisplay pn)
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                var entity = db.PhieuNhaps
                    .Include(p => p.ChiTietPns)
                    .FirstOrDefault(p => p.MaPn == pn.MaPN);

                if (entity == null) return;

                using var giaoDich = db.Database.BeginTransaction();
                try
                {
                    foreach (var chiTiet in entity.ChiTietPns)
                    {
                        var linhKien = db.LinhKiens.Find(chiTiet.MaLk);
                        if (linhKien != null)
                            linhKien.SoLuongTon -= chiTiet.SoLuongNhap; // Trừ lại kho
                    }

                    db.ChiTietPns.RemoveRange(entity.ChiTietPns);
                    db.PhieuNhaps.Remove(entity);
                    db.SaveChanges();
                    giaoDich.Commit();

                    _all.Remove(pn);
                    DanhSachPhieuNhap.Remove(pn);
                    PhieuNhapChon = null;
                    CapNhatThongKe(DanhSachPhieuNhap);

                    MessageBox.Show("Xóa phiếu nhập thành công! Đã hoàn trả lại số lượng kho.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    giaoDich.Rollback();
                    throw new Exception("Quá trình xóa thất bại: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Lưu phiếu nhập an toàn (Transaction) ─────────────────────────────
        public async Task LuuPhieuNhapAnToan(PhieuNhap pnMoi, List<ChiTietPn> danhSachChiTiet)
        {
            var db = DataProvider.Ins.GetContext();

            using var giaoDich = await db.Database.BeginTransactionAsync();
            try
            {
                db.PhieuNhaps.Add(pnMoi);

                var danhSachMaLk = danhSachChiTiet.Select(m => m.MaLk).ToList();
                var danhSachKho = await db.LinhKiens
                    .Where(lk => danhSachMaLk.Contains(lk.MaLk))
                    .ToDictionaryAsync(lk => lk.MaLk);

                foreach (var ct in danhSachChiTiet)
                {
                    if (!danhSachKho.TryGetValue(ct.MaLk, out var kho))
                        throw new Exception($"Không tìm thấy linh kiện mã '{ct.MaLk}'!");

                    kho.SoLuongTon += ct.SoLuongNhap; // Cộng vào kho
                    db.ChiTietPns.Add(ct);
                }

                await db.SaveChangesAsync();
                await giaoDich.CommitAsync();

                TaiDuLieu();
                MessageBox.Show("Nhập kho thành công! Đã cập nhật số lượng linh kiện.",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await giaoDich.RollbackAsync();
                MessageBox.Show("Lỗi nhập kho: " + ex.Message, "Báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

