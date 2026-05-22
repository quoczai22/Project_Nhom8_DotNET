using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class ThemHoaDonDialogViewModel : BaseViewModel
    {
        private string _maHd;
        public string MaHd
        {
            get => _maHd;
            set { _maHd = value; OnPropertyChanged(); }
        }
        private KhachHang _selectedKhachHang;
        public KhachHang SelectedKhachHang
        {
            get => _selectedKhachHang;
            set { _selectedKhachHang = value; OnPropertyChanged(); }
        }
        private NhanVien _selectedNhanVien;
        public NhanVien SelectedNhanVien
        {
            get => _selectedNhanVien;
            set { _selectedNhanVien = value; OnPropertyChanged(); }
        }
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
        private string _tongTienText = "0 ₫";
        public string TongTienText
        {
            get => _tongTienText;
            set { _tongTienText = value; OnPropertyChanged(); }
        }
        private string _selectedPhuongThuc = "Tiền mặt";
        public string SelectedPhuongThuc
        {
            get => _selectedPhuongThuc;
            set { _selectedPhuongThuc = value; OnPropertyChanged(); }
        }
        public List<KhachHang> DanhSachKhachHang { get; set; }
        public List<NhanVien> DanhSachNhanVien { get; set; }
        public List<LinhKien> DanhSachLinhKien { get; set; }
        public ObservableCollection<GioHangItem> GioHang { get; set; } = new();
        public List<string> DanhSachPhuongThuc { get; set; } = new() { "Tiền mặt", "Ví MoMo" };
        public HoaDon HoaDonMoi { get; private set; }
        public List<ChiTietHd> ChiTietHds { get; private set; }
        // Lệnh thêm linh kiện đã chọn vào giỏ hàng của hóa đơn mới
        public ICommand ThemVaoGioCommand { get; private set; }

        // Lệnh xóa một linh kiện ra khỏi giỏ hàng hiện tại
        public ICommand XoaKhoiGioCommand { get; private set; }

        // Lệnh lưu hóa đơn mới vào cơ sở dữ liệu
        public ICommand LuuCommand { get; private set; }

        // Lệnh hủy bỏ thao tác tạo hóa đơn và đóng cửa sổ
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng cửa sổ dialog, được gán từ View để ViewModel có thể yêu cầu đóng
        public Action<bool?> CloseAction { get; set; }
        public ThemHoaDonDialogViewModel(string maHdMoi)
        {
            MaHd = maHdMoi;
            TaiDuLieu();
            KhoiTaoCommands();
        }
        private void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                DanhSachKhachHang = db.KhachHangs.AsNoTracking().OrderBy(kh => kh.TenKh).ToList();
                DanhSachNhanVien = db.NhanViens.AsNoTracking().Where(nv => nv.DaNghiViec == false && (nv.ChucVu == "Nhân viên thu ngân" || nv.ChucVu == "Quản lý")).OrderBy(nv => nv.TenNv).ToList();
                DanhSachLinhKien = db.LinhKiens.AsNoTracking().Where(lk => lk.NgungKinhDoanh == false && lk.SoLuongTon > 0).OrderBy(lk => lk.TenLk).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachKhachHang = new List<KhachHang>();
                DanhSachNhanVien = new List<NhanVien>();
                DanhSachLinhKien = new List<LinhKien>();
            }
        }
        // Gắn kết các nút bấm trên giao diện với hành động tương ứng trong ViewModel
        private void KhoiTaoCommands()
        {
            ThemVaoGioCommand = new RelayCommand<object>(CanThemVaoGio, ThucHienThemVaoGio);
            XoaKhoiGioCommand = new RelayCommand<GioHangItem>(CanXoaKhoiGio, ThucHienXoaKhoiGio);
            LuuCommand = new RelayCommand<object>(CanLuu, ThucHienLuu);
            HuyCommand = new RelayCommand<object>(CanHuy, ThucHienHuy);
        }

        // Kiểm tra điều kiện: Luôn cho phép thêm linh kiện vào giỏ hàng
        private bool CanThemVaoGio(object parameter)
        {
            return true;
        }

        // Kiểm tra điều kiện: Chỉ cho phép xóa khi có mục hàng được chọn
        private bool CanXoaKhoiGio(GioHangItem item)
        {
            return item != null;
        }

        // Kiểm tra điều kiện: Luôn cho phép thực hiện lưu hóa đơn
        private bool CanLuu(object parameter)
        {
            return true;
        }

        // Kiểm tra điều kiện: Luôn cho phép hủy bỏ thao tác
        private bool CanHuy(object parameter)
        {
            return true;
        }
        // Thêm linh kiện vào giỏ hàng, kiểm tra tồn kho trước khi chấp nhận
        private void ThucHienThemVaoGio(object parameter)
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
            // Kiểm tra tồn kho
            int tonKho = SelectedLinhKien.SoLuongTon ?? 0;
            var existItem = GioHang.FirstOrDefault(g => g.MaLk == SelectedLinhKien.MaLk);
            int soLuongDaChon = existItem?.SoLuong ?? 0;
            if (soLuongDaChon + soLuong > tonKho)
            {
                MessageBox.Show($"Linh kiện '{SelectedLinhKien.TenLk}' chỉ còn {tonKho} cái trong kho (đã chọn {soLuongDaChon})!",
                    "Không đủ hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (existItem != null)
            {
                existItem.SoLuong += soLuong;
            }
            else
            {
                GioHang.Add(new GioHangItem
                {
                    MaLk = SelectedLinhKien.MaLk,
                    TenLk = SelectedLinhKien.TenLk,
                    SoLuong = soLuong,
                    DonGia = SelectedLinhKien.DonGiaBan ?? 0,
                    TonKho = tonKho
                });
            }
            CapNhatTongTien();
        }
        private void ThucHienXoaKhoiGio(GioHangItem item)
        {
            if (item != null)
            {
                GioHang.Remove(item);
                CapNhatTongTien();
            }
        }
        private void CapNhatTongTien()
        {
            long tong = GioHang.Sum(g => (long)g.ThanhTien);
            TongTienText = $"{tong:N0} ₫";
        }
        // Tạo đối tượng hóa đơn mới cùng danh sách chi tiết từ giỏ hàng và đóng cửa sổ
        private void ThucHienLuu(object parameter)
        {
            if (SelectedKhachHang == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedNhanVien == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (GioHang.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một sản phẩm vào hóa đơn!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int tongTien = GioHang.Sum(g => g.ThanhTien);
            HoaDonMoi = new HoaDon
            {
                MaHd = MaHd?.Trim(),
                NgayHd = DateOnly.FromDateTime(DateTime.Now),
                MaKh = SelectedKhachHang.MaKh,
                MaNv = SelectedNhanVien.MaNv,
                TongTien = tongTien,
                TrangThai = "Chưa thanh toán",
                PhuongThucThanhToan = SelectedPhuongThuc
            };
            ChiTietHds = GioHang.Select(g => new ChiTietHd
            {
                MaHd = MaHd?.Trim(),
                MaLk = g.MaLk,
                SoLuong = (byte)g.SoLuong,
                DonGia = g.DonGia
            }).ToList();
            CloseAction?.Invoke(true);
        }
        // Đóng cửa sổ mà không lưu bất kỳ thay đổi nào
        private void ThucHienHuy(object parameter)
        {
            CloseAction?.Invoke(false);
        }
    }
}