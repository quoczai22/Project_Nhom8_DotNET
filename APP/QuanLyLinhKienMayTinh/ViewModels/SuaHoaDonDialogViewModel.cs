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
    public class SuaHoaDonDialogViewModel : BaseViewModel
    {
        private string _maHdTitle;
        public string MaHdTitle
        {
            get => _maHdTitle;
            set { _maHdTitle = value; OnPropertyChanged(); }
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

        public List<KhachHang> DanhSachKhachHang { get; set; }
        public List<NhanVien> DanhSachNhanVien { get; set; }
        public List<LinhKien> DanhSachLinhKien { get; set; }
        public ObservableCollection<GioHangItem> GioHang { get; set; } = new();

        private readonly string _maHd;
        private readonly HoaDon _hoaDonGoc;
        private readonly List<ChiTietHd> _chiTietCu;

        // Lệnh thêm linh kiện đã chọn vào giỏ hàng của hóa đơn
        public ICommand ThemVaoGioCommand { get; private set; }

        // Lệnh xóa một linh kiện ra khỏi giỏ hàng hiện tại
        public ICommand XoaKhoiGioCommand { get; private set; }

        // Lệnh lưu toàn bộ thay đổi của hóa đơn xuống cơ sở dữ liệu
        public ICommand LuuCommand { get; private set; }

        // Lệnh hủy bỏ thao tác chỉnh sửa và đóng cửa sổ
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng cửa sổ dialog, được gán từ View để ViewModel có thể yêu cầu đóng
        public Action<bool?> CloseAction { get; set; }

        public SuaHoaDonDialogViewModel(string maHd)
        {
            _maHd = maHd;
            MaHdTitle = $"Mã hóa đơn: {maHd} | Trạng thái: Chưa thanh toán";

            var db = DataProvider.Ins.GetContext();
            _hoaDonGoc = db.HoaDons
                .Include(h => h.ChiTietHds)
                .FirstOrDefault(h => h.MaHd == maHd);

            _chiTietCu = _hoaDonGoc?.ChiTietHds.ToList() ?? new List<ChiTietHd>();

            TaiDuLieu();
            TaiChiTietHienTai();
            KhoiTaoCommands();
        }

        // Tải danh sách khách hàng, nhân viên và linh kiện từ cơ sở dữ liệu để hiển thị trên form
        private void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();

                DanhSachKhachHang = db.KhachHangs.AsNoTracking().OrderBy(kh => kh.TenKh).ToList();
                DanhSachNhanVien = db.NhanViens.AsNoTracking()
                    .Where(nv => nv.DaNghiViec == false)
                    .OrderBy(nv => nv.TenNv)
                    .ToList();
                DanhSachLinhKien = db.LinhKiens.AsNoTracking()
                    .Where(lk => lk.NgungKinhDoanh == false)
                    .OrderBy(lk => lk.TenLk)
                    .ToList();

                // Chọn sẵn khách hàng và nhân viên hiện tại của hóa đơn
                if (_hoaDonGoc != null)
                {
                    SelectedKhachHang = DanhSachKhachHang.FirstOrDefault(kh => kh.MaKh == _hoaDonGoc.MaKh);
                    SelectedNhanVien = DanhSachNhanVien.FirstOrDefault(nv => nv.MaNv == _hoaDonGoc.MaNv);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu hóa đơn: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachKhachHang = new List<KhachHang>();
                DanhSachNhanVien = new List<NhanVien>();
                DanhSachLinhKien = new List<LinhKien>();
            }
        }

        // Nạp chi tiết sản phẩm hiện có của hóa đơn vào giỏ hàng để người dùng có thể chỉnh sửa
        private void TaiChiTietHienTai()
        {
            if (_hoaDonGoc == null) return;
            try
            {
                var db = DataProvider.Ins.GetContext();
                foreach (var ct in _chiTietCu)
                {
                    var lk = db.LinhKiens.AsNoTracking().FirstOrDefault(l => l.MaLk == ct.MaLk);
                    if (lk != null)
                    {
                        GioHang.Add(new GioHangItem
                        {
                            MaLk = ct.MaLk,
                            TenLk = lk.TenLk,
                            SoLuong = ct.SoLuong ?? 1,
                            DonGia = ct.DonGia ?? lk.DonGiaBan ?? 0,
                            // Tồn kho thực tế = tồn hiện tại + số lượng đang có trong HD này
                            TonKho = (lk.SoLuongTon ?? 0) + (ct.SoLuong ?? 0)
                        });
                    }
                }
                CapNhatTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết hóa đơn hiện tại: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

        // Thêm linh kiện đang chọn vào giỏ hàng, kiểm tra số lượng tồn kho trước khi chấp nhận
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

            var existItem = GioHang.FirstOrDefault(g => g.MaLk == SelectedLinhKien.MaLk);

            // Tồn kho có hiệu = tồn hiện tại + số lượng trong HD cũ (nếu đã có)
            int chiTietCuSoLuong = _chiTietCu.FirstOrDefault(ct => ct.MaLk == SelectedLinhKien.MaLk)?.SoLuong ?? 0;
            int tonKhoHieuLuc = (SelectedLinhKien.SoLuongTon ?? 0) + chiTietCuSoLuong;

            if (soLuong > tonKhoHieuLuc)
            {
                MessageBox.Show($"Linh kiện '{SelectedLinhKien.TenLk}' chỉ có {tonKhoHieuLuc} cái khả dụng!",
                    "Không đủ hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (existItem != null)
            {
                existItem.SoLuong = soLuong; // Thay thế, không cộng thêm
            }
            else
            {
                GioHang.Add(new GioHangItem
                {
                    MaLk = SelectedLinhKien.MaLk,
                    TenLk = SelectedLinhKien.TenLk,
                    SoLuong = soLuong,
                    DonGia = SelectedLinhKien.DonGiaBan ?? 0,
                    TonKho = tonKhoHieuLuc
                });
            }

            CapNhatTongTien();
        }

        // Xóa một dòng sản phẩm khỏi giỏ hàng và tính lại tổng tiền
        private void ThucHienXoaKhoiGio(GioHangItem item)
        {
            if (item != null)
            {
                GioHang.Remove(item);
                CapNhatTongTien();
            }
        }

        // Tính lại tổng giá trị đơn hàng dựa trên tất cả sản phẩm trong giỏ
        private void CapNhatTongTien()
        {
            long tong = GioHang.Sum(g => (long)g.ThanhTien);
            TongTienText = $"{tong:N0} ₫";
        }

        // Lưu toàn bộ thay đổi vào cơ sở dữ liệu: hoàn kho cũ, cập nhật chi tiết mới và trừ kho tương ứng
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
                MessageBox.Show("Hóa đơn phải có ít nhất một sản phẩm!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var db = DataProvider.Ins.GetContext();
            using var giaoDich = db.Database.BeginTransaction();

            try
            {
                var entity = db.HoaDons.Include(h => h.ChiTietHds).FirstOrDefault(h => h.MaHd == _maHd);
                if (entity == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Hoàn kho của chi tiết cũ
                foreach (var ct in entity.ChiTietHds.ToList())
                {
                    var linhKien = db.LinhKiens.Find(ct.MaLk);
                    if (linhKien != null)
                        linhKien.SoLuongTon += ct.SoLuong;
                }

                // Xóa chi tiết cũ
                db.ChiTietHds.RemoveRange(entity.ChiTietHds);

                // Cập nhật thông tin hóa đơn
                entity.MaKh = SelectedKhachHang.MaKh;
                entity.MaNv = SelectedNhanVien.MaNv;
                entity.TongTien = GioHang.Sum(g => g.ThanhTien);

                // Thêm chi tiết mới + trừ kho
                foreach (var item in GioHang)
                {
                    var linhKien = db.LinhKiens.Find(item.MaLk);
                    if (linhKien == null || linhKien.SoLuongTon < item.SoLuong)
                        throw new Exception($"Linh kiện '{item.TenLk}' không đủ hàng trong kho!");

                    linhKien.SoLuongTon -= item.SoLuong;

                    db.ChiTietHds.Add(new ChiTietHd
                    {
                        MaHd = _maHd,
                        MaLk = item.MaLk,
                        SoLuong = (byte)item.SoLuong,
                        DonGia = item.DonGia
                    });
                }

                db.SaveChanges();
                giaoDich.Commit();

                MessageBox.Show("Cập nhật hóa đơn thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                giaoDich.Rollback();
                MessageBox.Show("Lỗi khi sửa hóa đơn: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Đóng cửa sổ chỉnh sửa mà không lưu bất kỳ thay đổi nào
        private void ThucHienHuy(object parameter)
        {
            CloseAction?.Invoke(false);
        }
    }
}