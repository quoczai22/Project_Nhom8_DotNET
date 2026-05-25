using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class PhieuNhapItem
    {
        public string MaLk { get; set; }
        public string TenLk { get; set; }
        public int SoLuong { get; set; }
        public int DonGiaNhap { get; set; }
        public int ThanhTien => SoLuong * DonGiaNhap;
    }

    public class ThemPhieuNhapDialogViewModel : BaseViewModel
    {
        // ── Thông tin phiếu ───────────────────────────────────────────────────
        private string _maPn;
        public string MaPn
        {
            get => _maPn;
            set { _maPn = value; OnPropertyChanged(); }
        }

        private NhanVien _selectedNhanVien;
        public NhanVien SelectedNhanVien
        {
            get => _selectedNhanVien;
            set { _selectedNhanVien = value; OnPropertyChanged(); }
        }

        private DateTime _ngayNhap = DateTime.Today;
        public DateTime NgayNhap
        {
            get => _ngayNhap;
            set { _ngayNhap = value; OnPropertyChanged(); }
        }

        // ── Thêm linh kiện ────────────────────────────────────────────────────
        private LinhKien _selectedLinhKien;
        public LinhKien SelectedLinhKien
        {
            get => _selectedLinhKien;
            set { _selectedLinhKien = value; OnPropertyChanged(); }
        }

        private string _soLuongText = "1";
        public string SoLuongText
        {
            get => _soLuongText;
            set { _soLuongText = value; OnPropertyChanged(); }
        }

        private string _donGiaNhapText = "0";
        public string DonGiaNhapText
        {
            get => _donGiaNhapText;
            set { _donGiaNhapText = value; OnPropertyChanged(); }
        }

        private string _tongChiPhiText = "0 ₫";
        public string TongChiPhiText
        {
            get => _tongChiPhiText;
            set { _tongChiPhiText = value; OnPropertyChanged(); }
        }

        // ── Dữ liệu nguồn ────────────────────────────────────────────────────
        public List<NhanVien> DanhSachNhanVien { get; set; }
        public List<LinhKien> DanhSachLinhKien { get; set; }
        public ObservableCollection<PhieuNhapItem> DanhSachNhap { get; set; } = new();

        // ── Kết quả trả về cho ViewModel cha ─────────────────────────────────
        public PhieuNhap PhieuNhapMoi { get; private set; }
        public List<ChiTietPn> ChiTietPns { get; private set; }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand ThemVaoPhieuCommand { get; private set; }
        public ICommand XoaKhoiPhieuCommand { get; private set; }
        public ICommand LuuCommand { get; private set; }
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng dialog, được gán từ code-behind
        public Action<bool?> CloseAction { get; set; }

        // ── Constructor ───────────────────────────────────────────────────────
        public ThemPhieuNhapDialogViewModel(string maPnMoi)
        {
            MaPn = maPnMoi;
            TaiDuLieu();
            KhoiTaoCommands();
        }

        // ── Tải dữ liệu ──────────────────────────────────────────────────────
        private void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();

                DanhSachNhanVien = db.NhanViens
                    .AsNoTracking()
                    .Where(nv => nv.DaNghiViec == false)
                    .OrderBy(nv => nv.TenNv)
                    .ToList();

                // Lấy tất cả linh kiện (kể cả hết hàng vì đây là NHẬP kho)
                DanhSachLinhKien = db.LinhKiens
                    .AsNoTracking()
                    .Where(lk => lk.NgungKinhDoanh == false)
                    .OrderBy(lk => lk.TenLk)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu phiếu nhập: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachNhanVien = new List<NhanVien>();
                DanhSachLinhKien = new List<LinhKien>();
            }
        }

        // ── Khởi tạo commands ─────────────────────────────────────────────────
        private void KhoiTaoCommands()
        {
            ThemVaoPhieuCommand = new RelayCommand<object>(_ => true, ThucHienThemVaoPhieu);
            XoaKhoiPhieuCommand = new RelayCommand<PhieuNhapItem>(item => item != null, ThucHienXoaKhoiPhieu);
            LuuCommand = new RelayCommand<object>(_ => true, ThucHienLuu);
            HuyCommand = new RelayCommand<object>(_ => true, _ => CloseAction?.Invoke(false));
        }

        // ── Thêm linh kiện vào phiếu ─────────────────────────────────────────
        private void ThucHienThemVaoPhieu(object parameter)
        {
            if (SelectedLinhKien == null)
            {
                MessageBox.Show("Vui lòng chọn linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(SoLuongText?.Trim(), out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(DonGiaNhapText?.Trim().Replace(",", "").Replace(".", ""), out int donGia) || donGia < 0)
            {
                MessageBox.Show("Đơn giá nhập không hợp lệ!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Nếu linh kiện đã có trong phiếu thì cộng dồn số lượng
            var existing = DanhSachNhap.FirstOrDefault(x => x.MaLk == SelectedLinhKien.MaLk);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                existing.DonGiaNhap = donGia; // Cập nhật giá mới nhất
            }
            else
            {
                DanhSachNhap.Add(new PhieuNhapItem
                {
                    MaLk = SelectedLinhKien.MaLk,
                    TenLk = SelectedLinhKien.TenLk,
                    SoLuong = soLuong,
                    DonGiaNhap = donGia
                });
            }

            // Reset input
            SoLuongText = "1";
            DonGiaNhapText = "0";
            SelectedLinhKien = null;

            CapNhatTongChiPhi();
        }

        // ── Xóa linh kiện khỏi phiếu ─────────────────────────────────────────
        private void ThucHienXoaKhoiPhieu(PhieuNhapItem item)
        {
            if (item != null)
            {
                DanhSachNhap.Remove(item);
                CapNhatTongChiPhi();
            }
        }

        // ── Tính tổng chi phí ─────────────────────────────────────────────────
        private void CapNhatTongChiPhi()
        {
            long tong = DanhSachNhap.Sum(x => (long)x.ThanhTien);
            TongChiPhiText = $"{tong:N0} ₫";
        }

        // ── Lưu phiếu nhập ───────────────────────────────────────────────────
        private void ThucHienLuu(object parameter)
        {
            if (SelectedNhanVien == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DanhSachNhap.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một linh kiện vào phiếu nhập!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PhieuNhapMoi = new PhieuNhap
            {
                MaPn = MaPn?.Trim(),
                NgayNhap = DateOnly.FromDateTime(NgayNhap),
                MaNv = SelectedNhanVien.MaNv
            };

            ChiTietPns = DanhSachNhap.Select(x => new ChiTietPn
            {
                MaPn = MaPn?.Trim(),
                MaLk = x.MaLk,
                SoLuongNhap = x.SoLuong,
                DonGiaNhap = x.DonGiaNhap
            }).ToList();

            CloseAction?.Invoke(true);
        }
    }
}
