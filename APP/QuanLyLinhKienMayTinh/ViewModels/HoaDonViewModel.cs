using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.Services;
using QuanLyLinhKienMayTinh.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    // Display class cho danh sách hóa đơn (DataGrid trái)
    public class HoaDonDisplay
    {
        public string MaHoaDon { get; set; }
        public string TenKhachHang { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? TongTien { get; set; }
        public int? TamTinh { get; set; }
        public int? GiamGia { get; set; }
        public string TrangThai { get; set; }
        public Brush TrangThaiMauNen { get; set; }
        public Brush TrangThaiMauChu { get; set; }
        public string SoDienThoai { get; set; }
        public string PhuongThucThanhToan { get; set; }
    }

    // Display class cho chi tiết sản phẩm (DataGrid trong panel phải)
    public class ChiTietSanPhamDisplay
    {
        public string TenSanPham { get; set; }
        public byte? SoLuong { get; set; }
        public int? DonGia { get; set; }
        public string HanBaoHanhHienThi { get; set; }
    }
    public class HoaDonViewModel : BaseViewModel, ISearchable
    {
        private readonly BaoCaoViewModel _baoCaoViewModel = new BaoCaoViewModel();
        //Backing data
        private ObservableCollection<HoaDonDisplay> _all;

        //Danh sách hóa đơn (DataGrid)
        private ObservableCollection<HoaDonDisplay> _danhSachHoaDon;
        public ObservableCollection<HoaDonDisplay> DanhSachHoaDon
        {
            get => _danhSachHoaDon;
            set { _danhSachHoaDon = value; OnPropertyChanged(); }
        }

        // Dịch vụ MoMo
        IMomoService _momoService;

        //Hóa đơn đang chọn
        private HoaDonDisplay _hoaDonChon;
        public HoaDonDisplay HoaDonChon
        {
            get => _hoaDonChon;
            set
            {
                _hoaDonChon = value;
                OnPropertyChanged();
                // Cập nhật panel chi tiết
                ChiTietVisibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                ChuaChonHoaDonVisibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                TaiChiTietSanPham(value?.MaHoaDon);
                ThanhToanButtonVisibility = (value != null && value.TrangThai != "Đã thanh toán")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        //Chi tiết sản phẩm trong hóa đơn
        private ObservableCollection<ChiTietSanPhamDisplay> _chiTietSanPham;
        public ObservableCollection<ChiTietSanPhamDisplay> ChiTietSanPham
        {
            get => _chiTietSanPham;
            set { _chiTietSanPham = value; OnPropertyChanged(); }
        }

        //Visibility panel chi tiết
        private Visibility _chiTietVisibility = Visibility.Collapsed;
        public Visibility ChiTietVisibility
        {
            get => _chiTietVisibility;
            set { _chiTietVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _chuaChonHoaDonVisibility = Visibility.Visible;
        public Visibility ChuaChonHoaDonVisibility
        {
            get => _chuaChonHoaDonVisibility;
            set { _chuaChonHoaDonVisibility = value; OnPropertyChanged(); }
        }

        //Toolbar: search, filter trạng thái, lọc ngày
        private string _tuKhoanTimKiem = string.Empty;
        public string TuKhoanTimKiem
        {
            get => _tuKhoanTimKiem;
            set { _tuKhoanTimKiem = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _danhSachTrangThai;
        public ObservableCollection<string> DanhSachTrangThai
        {
            get => _danhSachTrangThai;
            set { _danhSachTrangThai = value; OnPropertyChanged(); }
        }

        private string _trangThaiChon;
        public string TrangThaiChon
        {
            get => _trangThaiChon;
            set { _trangThaiChon = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _danhSachPhuongThuc;
        public ObservableCollection<string> DanhSachPhuongThuc
        {
            get => _danhSachPhuongThuc;
            set { _danhSachPhuongThuc = value; OnPropertyChanged(); }
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

        // Footer thống kê 
        private int _tongSoHoaDon;
        public int TongSoHoaDon
        {
            get => _tongSoHoaDon;
            set { _tongSoHoaDon = value; OnPropertyChanged(); }
        }

        private long _tongDoanhThu;
        public long TongDoanhThu
        {
            get => _tongDoanhThu;
            set { _tongDoanhThu = value; OnPropertyChanged(); }
        }

        private int _soHoaDonDaThanhToan;
        public int SoHoaDonDaThanhToan
        {
            get => _soHoaDonDaThanhToan;
            set { _soHoaDonDaThanhToan = value; OnPropertyChanged(); }
        }

        private int _soHoaDonChoXuLy;
        public int SoHoaDonChoXuLy
        {
            get => _soHoaDonChoXuLy;
            set { _soHoaDonChoXuLy = value; OnPropertyChanged(); }
        }

        private Visibility _thanhToanButtonVisibility = Visibility.Collapsed;
        public Visibility ThanhToanButtonVisibility
        {
            get => _thanhToanButtonVisibility;
            set { _thanhToanButtonVisibility = value; OnPropertyChanged(); }
        }

        // Commands 
        public ICommand TaoHoaDonMoiCommand { get; private set; }
        public ICommand SuaHoaDonCommand { get; private set; }
        public ICommand InHoaDonCommand { get; private set; }
        public ICommand XoaHoaDonCommand { get; private set; }
        public ICommand LocHoaDonCommand { get; private set; }
        public ICommand ThanhToanHoaDonCommand { get; private set; }

        public HoaDonViewModel(IMomoService momoService)
        {
            DanhSachTrangThai = new ObservableCollection<string>
            {
                "-- Tất cả --", "Đã thanh toán", "Chưa thanh toán"
            };
            TrangThaiChon = "-- Tất cả --";
            DanhSachPhuongThuc = new ObservableCollection<string>
            {
                "Tiền mặt", "Chuyển khoản", "Thẻ"
            };
            TaiDuLieu();
            KhoiTaoCommands();
            _momoService = momoService; // Lưu service MoMo vào biến để sửa dụng sau này 

        }

        // Lấy toàn bộ danh sách hóa đơn từ cơ sở dữ liệu và tự động cập nhật số liệu thống kê
        public void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();

                var list = db.HoaDons
                    .AsNoTracking()
                    .Include(hd => hd.MaKhNavigation)
                    .Include(hd => hd.MaNvNavigation)
                    .OrderByDescending(hd => hd.NgayHd)
                    .ToList()
                    .Select(hd => MapToDisplay(hd))
                    .ToList();

                _all = new ObservableCollection<HoaDonDisplay>(list);
                DanhSachHoaDon = new ObservableCollection<HoaDonDisplay>(list);

                CapNhatThongKe(_all);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu hóa đơn: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Tải danh sách chi tiết các linh kiện (sản phẩm) thuộc về hóa đơn đang được chọn
        private void TaiChiTietSanPham(string maHoaDon)
        {
            if (string.IsNullOrEmpty(maHoaDon))
            {
                ChiTietSanPham = new ObservableCollection<ChiTietSanPhamDisplay>();
                return;
            }

            try
            {
                var db = DataProvider.Ins.GetContext();
                var chiTiet = db.ChiTietHds
                .AsNoTracking()
                .Include(ct => ct.MaLkNavigation)
                .Include(ct => ct.MaHdNavigation)
                .Where(ct => ct.MaHd == maHoaDon)
                .ToList()
                .Select(ct => {
                    DateTime? ngayMua = null;
                    int thoiGianBH = 0;
                    string chuoiHanBaoHanh = "Không có";
                    if (ct.MaHdNavigation != null && ct.MaHdNavigation.NgayHd != null)
                    {
                        ngayMua = ct.MaHdNavigation.NgayHd.Value.ToDateTime(TimeOnly.MinValue);
                    }
                    if (ct.MaLkNavigation != null && ct.MaLkNavigation.Tgbh != null)
                    {
                        thoiGianBH = ct.MaLkNavigation.Tgbh.Value;
                    }
                    if (ngayMua != null)
                    {
                        DateTime ngayHetHan = ngayMua.Value.AddMonths(thoiGianBH);
                        chuoiHanBaoHanh = ngayHetHan.ToString("dd/MM/yyyy");
                    }
                    return new ChiTietSanPhamDisplay
                    {
                        TenSanPham = (ct.MaLkNavigation != null) ? ct.MaLkNavigation.TenLk : "Linh kiện ẩn",
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,
                        HanBaoHanhHienThi = chuoiHanBaoHanh
                    };
                }).ToList();
                ChiTietSanPham = new ObservableCollection<ChiTietSanPhamDisplay>(chiTiet);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết hóa đơn: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Chuyển đổi dữ liệu hóa đơn từ cơ sở dữ liệu sang đối tượng hiển thị với màu sắc trạng thái tương ứng
        private static HoaDonDisplay MapToDisplay(HoaDon hd)
        {
            string trangThai = hd.TrangThai ?? "Chưa thanh toán";

            Brush mauNen, mauChu;

            if (trangThai == "Đã thanh toán")
            {
                mauNen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e8f5e9"));
                mauChu = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e7d32"));
            }
            else
            {
                // Dành cho "Chưa thanh toán" hoặc các trạng thái khác
                mauNen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff3e0"));
                mauChu = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e65100"));
            }

            DateTime? ngayTao = hd.NgayHd.HasValue
                ? hd.NgayHd.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null;

            return new HoaDonDisplay
            {
                MaHoaDon = hd.MaHd,
                TenKhachHang = hd.MaKhNavigation?.TenKh ?? hd.MaKh,
                TenNhanVien = hd.MaNvNavigation?.TenNv ?? hd.MaNv,
                NgayTao = ngayTao,
                TongTien = hd.TongTien,
                TamTinh = hd.TongTien,
                GiamGia = 0,
                TrangThai = trangThai,
                TrangThaiMauNen = mauNen,
                TrangThaiMauChu = mauChu,
                SoDienThoai = hd.MaKhNavigation?.Sdt ?? string.Empty,
                PhuongThucThanhToan = hd.PhuongThucThanhToan ?? "Tiền mặt"
            };
        }

        // Lọc danh sách hóa đơn dựa trên từ khóa tìm kiếm, trạng thái thanh toán và khoảng thời gian
        private void LocHoaDon()
        {
            var filtered = _all.AsEnumerable();

            // Lọc từ khóa
            if (!string.IsNullOrWhiteSpace(TuKhoanTimKiem))
            {
                string kw = TuKhoanTimKiem.ToLower();
                filtered = filtered.Where(hd =>
                    (hd.MaHoaDon?.ToLower().Contains(kw) ?? false) ||
                    (hd.TenKhachHang?.ToLower().Contains(kw) ?? false) ||
                    (hd.TenNhanVien?.ToLower().Contains(kw) ?? false));
            }

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(TrangThaiChon) && TrangThaiChon != "-- Tất cả --")
                filtered = filtered.Where(hd => hd.TrangThai == TrangThaiChon);

            // Lọc theo ngày
            if (TuNgay.HasValue)
                filtered = filtered.Where(hd => hd.NgayTao >= TuNgay.Value);
            if (DenNgay.HasValue)
                filtered = filtered.Where(hd => hd.NgayTao <= DenNgay.Value.AddDays(1));

            var result = filtered.ToList();
            DanhSachHoaDon = new ObservableCollection<HoaDonDisplay>(result);
            CapNhatThongKe(DanhSachHoaDon);
            HoaDonChon = null;
        }

        // Tính toán tổng số lượng hóa đơn, doanh thu và phân loại trạng thái để hiển thị ở thanh footer
        private void CapNhatThongKe(IEnumerable<HoaDonDisplay> ds)
        {
            var list = ds.ToList();
            TongSoHoaDon = list.Count;
            TongDoanhThu = list.Where(hd => hd.TrangThai == "Đã thanh toán").Sum(hd => (long)(hd.TongTien ?? 0));
            SoHoaDonDaThanhToan = list.Count(hd => hd.TrangThai == "Đã thanh toán");
            SoHoaDonChoXuLy = list.Count(hd => hd.TrangThai == "Chưa thanh toán");
        }

        // Cập nhật từ khóa tìm kiếm từ giao diện và kích hoạt lại bộ lọc danh sách hóa đơn
        public void ApplySearch(string keyword)
        {
            TuKhoanTimKiem = keyword?.Trim() ?? string.Empty;
            LocHoaDon();
        }

        // Cấu hình các thao tác Thêm, Sửa, In, Xóa và Thanh toán cho các nút bấm trên giao diện
        private void KhoiTaoCommands( )
        {
            TaoHoaDonMoiCommand = new RelayCommand<object>(CanTaoHoaDon, ThucHienTaoHoaDon);
            SuaHoaDonCommand = new RelayCommand<object>(CanSuaHoaDon, ThucHienSuaHoaDon);
            InHoaDonCommand = new RelayCommand<object>(CanInHoaDon, ThucHienInHoaDon);
            XoaHoaDonCommand = new RelayCommand<object>(CanXoaHoaDon, ThucHienXoaHoaDon);
            LocHoaDonCommand = new RelayCommand<object>(CanLocHoaDon, ThucHienLocHoaDon);
            ThanhToanHoaDonCommand = new RelayCommand<object>(CanThanhToanHoaDon, ThucHienThanhToan);
        }
        // Kiểm tra điều kiện: Luôn cho phép mở giao diện tạo hóa đơn mới
        private bool CanTaoHoaDon(object parameter)
        {
            return true; 
        }
        // Xử lý tạo mã hóa đơn mới, mở cửa sổ nhập thông tin và lưu hóa đơn nếu thành công
        private void ThucHienTaoHoaDon(object parameter)
        {
            try
            {
                var dbRead = DataProvider.Ins.GetContext();
                var lastID = dbRead.HoaDons
                    .OrderByDescending(x => x.MaHd)
                    .Select(x => x.MaHd).FirstOrDefault();
                string newID = Services.AutoIDService.GetNextID("HD", lastID);

                var dialog = new ThemHoaDonDialog(newID);
                var window = Application.Current.MainWindow;
                if(window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    LuuHoaDonAnToan(dialog.HoaDonMoi, dialog.ChiTietHds);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo hóa đơn: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Kiểm tra điều kiện: Chỉ cho phép chỉnh sửa khi người dùng đã chọn một hóa đơn cụ thể
        private bool CanSuaHoaDon(object parameter)
        {
            return HoaDonChon != null;
        }

        // Mở cửa sổ chỉnh sửa thông tin cho hóa đơn đang chọn (từ chối nếu hóa đơn đã thanh toán)
        private void ThucHienSuaHoaDon(object parameter)
        {
            if (HoaDonChon == null) return;
            if (HoaDonChon.TrangThai == "Đã thanh toán")
            {
                MessageBox.Show("Hóa đơn đã thanh toán, không thể chỉnh sửa nội dung!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var dialog = new SuaHoaDonDialog(HoaDonChon.MaHoaDon);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    TaiDuLieu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa hóa đơn: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Kiểm tra điều kiện: Chỉ cho phép in khi đã chọn một hóa đơn
        private bool CanInHoaDon(object parameter)
        {
            return HoaDonChon != null;
        }
        // Trích xuất thông tin chi tiết của hóa đơn và xuất ra file văn bản (TXT)
        private void ThucHienInHoaDon(object parameter)
        {
            var hd = HoaDonChon;
            var chiTiet = ChiTietSanPham;

            var document = _baoCaoViewModel.TaoBaoCaoHoaDon(hd, chiTiet); // Tạo FlowDocument chứa nội dung hóa đơn để hiển thị trong cửa sổ xem trước
            var window = new BaoCaoPreviewWindow(document);
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow.IsVisible && mainWindow.IsLoaded)
            {
                window.Owner = mainWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            window.ShowDialog();
        }

        // Kiểm tra điều kiện: Chỉ cho phép xóa khi người dùng đã chọn một hóa đơn
        private bool CanXoaHoaDon(object parameter)
        {
            return HoaDonChon != null;
        }
        // Hiển thị hộp thoại cảnh báo người dùng trước khi tiến hành xóa hóa đơn
        private void ThucHienXoaHoaDon(object parameter)
        {
            if (HoaDonChon == null) return;

            var res = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa hóa đơn [{HoaDonChon.MaHoaDon}] không?\n" +
                "Hàng tồn kho sẽ được hoàn trả lại.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
                ThucHienXoa(HoaDonChon);
        }
        // Kiểm tra điều kiện: Luôn cho phép chạy bộ lọc dữ liệu
        private bool CanLocHoaDon(object parameter)
        {
            return true;
        }
        // Kích hoạt chức năng lọc lại danh sách hóa đơn theo các tiêu chí hiện tại
        private void ThucHienLocHoaDon(object parameter)
        {
            LocHoaDon();
        }
        // Kiểm tra điều kiện: Chỉ cho phép thanh toán nếu hóa đơn đang ở trạng thái chưa thanh toán
        private bool CanThanhToanHoaDon(object parameter)
        {
            return HoaDonChon != null && HoaDonChon.TrangThai != "Đã thanh toán";
        }
        // Mở cửa sổ lựa chọn phương thức thanh toán (Tiền mặt, MoMo, v.v.) cho hóa đơn đang chọn
        private void ThucHienThanhToan(object parameter)
        {
            if (HoaDonChon == null) return;

            string maHoaDonDangThanhToan = HoaDonChon.MaHoaDon;
            var dialog = new ChonPhuongThucDialog(
                maHoaDonDangThanhToan,
                (long)(HoaDonChon.TongTien ?? 0),
                _momoService, 
                () => CapNhatTrangThaiThanhToan(maHoaDonDangThanhToan, "Đã thanh toán", "Ví MoMo"),
                () => CapNhatTrangThaiThanhToan(maHoaDonDangThanhToan, "Chưa thanh toán", "Ví MoMo")
            );

            var window = Application.Current.MainWindow;
            if (window != null && window.IsVisible && window.IsLoaded)
            {
                dialog.Owner = window;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            dialog.ShowDialog();

        }

        private void CapNhatTrangThaiThanhToan(string maHoaDon, string trangThai, string phuongThuc)
        {
            if (string.IsNullOrWhiteSpace(maHoaDon)) return;

            try
            {
                var db = DataProvider.Ins.GetContext();
                var entity = db.HoaDons.FirstOrDefault(hd => hd.MaHd == maHoaDon);
                if (entity == null) return;

                entity.TrangThai = trangThai;
                entity.PhuongThucThanhToan = phuongThuc;
                entity.NgayThanhToan = trangThai == "Đã thanh toán"
                    ? DateOnly.FromDateTime(DateTime.Now)
                    : null;

                db.SaveChanges();

                TaiDuLieu();
                HoaDonChon = DanhSachHoaDon?.FirstOrDefault(hd => hd.MaHoaDon == maHoaDon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật trạng thái thanh toán: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lưu hóa đơn vào cơ sở dữ liệu một cách an toàn (Sử dụng Transaction để trừ số lượng linh kiện trong kho)
        public async Task LuuHoaDonAnToan(HoaDon hdMoi, List<ChiTietHd> danhSachMonHang)
        {
            var db = DataProvider.Ins.GetContext();

            // Sử dụng Bất đồng bộ để UI WPF không bị đơ
            using (var giaoDich = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    db.HoaDons.Add(hdMoi);

                    // 1. TỐI ƯU HIỆU NĂNG (GOM LÔ): Lấy toàn bộ mã linh kiện khách mua
                    var danhSachMaLk = danhSachMonHang.Select(m => m.MaLk).ToList();

                    // Bắn 1 câu lệnh SQL DUY NHẤT để lấy tất cả linh kiện cần thiết lên RAM
                    var danhSachKho = await db.LinhKiens
                                              .Where(lk => danhSachMaLk.Contains(lk.MaLk))
                                              .ToDictionaryAsync(lk => lk.MaLk);

                    // 2. XỬ LÝ LOGIC TRÊN RAM
                    foreach (var mon in danhSachMonHang)
                    {
                        if (!danhSachKho.TryGetValue(mon.MaLk, out var kho))
                        {
                            throw new Exception($"Không tìm thấy dữ liệu cho mã '{mon.MaLk}'!");
                        }

                        if (kho.SoLuongTon < mon.SoLuong)
                        {
                            throw new Exception($"Linh kiện '{kho.TenLk}' không đủ hàng!");
                        }

                        kho.SoLuongTon -= mon.SoLuong;
                        db.ChiTietHds.Add(mon);
                    }

                    // 3. GHI XUỐNG DB & HOÀN TẤT GIAO DỊCH
                    await db.SaveChangesAsync(); // Ghi 1 cục duy nhất xuống Transaction Log (Chống chai máy)
                    await giaoDich.CommitAsync();

                    TaiDuLieu();
                    MessageBox.Show("Thanh toán thành công! Đã cập nhật kho.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    await giaoDich.RollbackAsync();
                    MessageBox.Show("Lỗi thanh toán: " + ex.Message, "Báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Xử lý xóa hóa đơn khỏi cơ sở dữ liệu và tự động hoàn trả lại số lượng linh kiện tương ứng vào kho
        private void ThucHienXoa(HoaDonDisplay hd)
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                var entity = db.HoaDons
                    .Include(h => h.ChiTietHds)
                    .FirstOrDefault(h => h.MaHd == hd.MaHoaDon);

                if (entity == null) return;

                using (var giaoDich = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var chiTiet in entity.ChiTietHds)
                        {
                            var linhKien = db.LinhKiens.Find(chiTiet.MaLk);
                            if (linhKien != null)
                            {
                                linhKien.SoLuongTon += chiTiet.SoLuong; // Cộng lại kho
                            }
                        }

                        db.ChiTietHds.RemoveRange(entity.ChiTietHds);
                        db.HoaDons.Remove(entity);

                        db.SaveChanges();
                        giaoDich.Commit();

                        _all.Remove(hd);
                        DanhSachHoaDon.Remove(hd);
                        HoaDonChon = null;
                        CapNhatThongKe(DanhSachHoaDon);

                        MessageBox.Show("Xóa hóa đơn thành công! Đã hoàn trả hàng vào kho.",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        giaoDich.Rollback();
                        throw new Exception("Quá trình xóa thất bại: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
