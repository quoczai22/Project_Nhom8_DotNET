using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class ThemSuaLinhKienDialogViewModel : BaseViewModel
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
        private string _maLk;
        public string MaLk
        {
            get => _maLk;
            set { _maLk = value; OnPropertyChanged(); }
        }
        private string _tenLk;
        public string TenLk
        {
            get => _tenLk;
            set { _tenLk = value; OnPropertyChanged(); }
        }
        private LoaiLk _selectedLoai;
        public LoaiLk SelectedLoai
        {
            get => _selectedLoai;
            set { _selectedLoai = value; OnPropertyChanged(); }
        }
        private NhaSanXuat _selectedNsx;
        public NhaSanXuat SelectedNsx
        {
            get => _selectedNsx;
            set { _selectedNsx = value; OnPropertyChanged(); }
        }
        private string _dvt;
        public string Dvt
        {
            get => _dvt;
            set { _dvt = value; OnPropertyChanged(); }
        }
        private string _tgbhText;
        public string TgbhText
        {
            get => _tgbhText;
            set { _tgbhText = value; OnPropertyChanged(); }
        }
        private string _donGiaText;
        public string DonGiaText
        {
            get => _donGiaText;
            set { _donGiaText = value; OnPropertyChanged(); }
        }
        private string _soLuongText;
        public string SoLuongText
        {
            get => _soLuongText;
            set { _soLuongText = value; OnPropertyChanged(); }
        }
        private DateTime? _ngayNhap;
        public DateTime? NgayNhap
        {
            get => _ngayNhap;
            set { _ngayNhap = value; OnPropertyChanged(); }
        }
        private bool _isMaLkReadOnly;
        public bool IsMaLkReadOnly
        {
            get => _isMaLkReadOnly;
            set { _isMaLkReadOnly = value; OnPropertyChanged(); }
        }
        private double _maLkOpacity = 1.0;
        public double MaLkOpacity
        {
            get => _maLkOpacity;
            set { _maLkOpacity = value; OnPropertyChanged(); }
        }
        private Visibility _maLkHintVisibility;
        public Visibility MaLkHintVisibility
        {
            get => _maLkHintVisibility;
            set { _maLkHintVisibility = value; OnPropertyChanged(); }
        }
        public List<LoaiLk> DanhSachLoai { get; set; }
        public List<NhaSanXuat> DanhSachNsx { get; set; }
        // Result properties
        public string ResultMaLk { get; private set; }
        public string ResultTenLk { get; private set; }
        public string ResultMaLoai { get; private set; }
        public string ResultMaNsx { get; private set; }
        public string ResultDvt { get; private set; }
        public byte? ResultTgbh { get; private set; }
        public int? ResultDonGiaBan { get; private set; }
        public int? ResultSoLuongTon { get; private set; }
        public DateOnly? ResultNgayNhap { get; private set; }
        private readonly bool _laMoiThem;
        // Lệnh xác nhận lưu thông tin linh kiện sau khi kiểm tra dữ liệu hợp lệ
        public ICommand LuuCommand { get; private set; }

        // Lệnh hủy bỏ thao tác và đóng cửa sổ mà không lưu gì
        public ICommand HuyCommand { get; private set; }

        // Hành động đóng cửa sổ dialog, được gán từ View để ViewModel có thể yêu cầu đóng
        public Action<bool?> CloseAction { get; set; }
        // Constructors
        public ThemSuaLinhKienDialogViewModel(string maLkGoiY)
        {
            _laMoiThem = true;
            TitleText = "Thêm Linh Kiện";
            ButtonContent = "Lưu";
            MaLk = maLkGoiY;
            MaLkHintVisibility = Visibility.Visible;
            NgayNhap = DateTime.Now;
            IsMaLkReadOnly = false;
            MaLkOpacity = 1.0;
            TaiDanhSachComboBox();
            KhoiTaoCommands();
        }
        public ThemSuaLinhKienDialogViewModel(LinhKienDisplay lk)
        {
            _laMoiThem = false;
            TitleText = "Sửa Linh Kiện";
            ButtonContent = "Cập nhật";
            MaLk = lk.MaLk;
            IsMaLkReadOnly = true;
            MaLkOpacity = 0.6;
            MaLkHintVisibility = Visibility.Collapsed;
            TenLk = lk.TenLk;
            Dvt = lk.Dvt;
            TgbhText = lk.Tgbh?.ToString();
            if (lk.NgayNhap.HasValue)
                NgayNhap = lk.NgayNhap.Value.ToDateTime(TimeOnly.MinValue);
            TaiDanhSachComboBox();
            // Load thêm thông tin từ DB cho Sửa
            var entity = DataProvider.Ins.GetContext().LinhKiens.AsNoTracking().FirstOrDefault(x => x.MaLk == lk.MaLk);
            if (entity != null)
            {
                DonGiaText = entity.DonGiaBan?.ToString();
                SoLuongText = entity.SoLuongTon?.ToString();
                // Select đúng item trong ComboBox
                SelectedLoai = DanhSachLoai.FirstOrDefault(l => l.MaLoai == entity.MaLoai);
                SelectedNsx = DanhSachNsx.FirstOrDefault(n => n.MaNsx == entity.MaNsx);
            }
            KhoiTaoCommands();
        }
        private void TaiDanhSachComboBox()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                DanhSachLoai = db.LoaiLks.AsNoTracking().OrderBy(l => l.TenLoai).ToList();
                DanhSachNsx = db.NhaSanXuats.AsNoTracking().OrderBy(n => n.TenNsx).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục loại/NSX: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DanhSachLoai = new List<LoaiLk>();
                DanhSachNsx = new List<NhaSanXuat>();
            }
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
            // Validate MaLk
            string maLk = MaLk?.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(maLk))
            {
                MessageBox.Show("Vui lòng nhập mã linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TenLk))
            {
                MessageBox.Show("Vui lòng nhập tên linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedLoai == null)
            {
                MessageBox.Show("Vui lòng chọn loại linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedNsx == null)
            {
                MessageBox.Show("Vui lòng chọn nhà sản xuất!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Validate số liệu
            if (!string.IsNullOrWhiteSpace(DonGiaText) && (!int.TryParse(DonGiaText, out int donGiaKiemTra) || donGiaKiemTra < 0))
            {
                MessageBox.Show("Đơn giá bán phải là số nguyên không âm!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(SoLuongText) && (!int.TryParse(SoLuongText, out int soLuongKiemTra) || soLuongKiemTra < 0))
            {
                MessageBox.Show("Số lượng tồn phải là số nguyên không âm!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(TgbhText) && !byte.TryParse(TgbhText, out _))
            {
                MessageBox.Show("Thời gian bảo hành phải là số nguyên (0-255)!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NgayNhap.HasValue && NgayNhap.Value.Date > DateTime.Today)
            {
                MessageBox.Show("Ngày nhập linh kiện không được lớn hơn ngày hiện tại!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Kiểm tra trùng mã khi thêm mới
            if (_laMoiThem)
            {
                bool trung = DataProvider.Ins.GetContext().LinhKiens
                    .AsNoTracking()
                    .Any(lk => lk.MaLk == maLk);
                if (trung)
                {
                    MessageBox.Show($"Mã linh kiện '{maLk}' đã tồn tại! Vui lòng nhập mã khác.",
                        "Trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            ResultMaLk = maLk;
            ResultTenLk = TenLk.Trim();
            ResultMaLoai = SelectedLoai.MaLoai;
            ResultMaNsx = SelectedNsx.MaNsx;
            ResultDvt = Dvt?.Trim();
            if (byte.TryParse(TgbhText, out byte tgbh))
                ResultTgbh = tgbh;
            else
                ResultTgbh = null;
            if (int.TryParse(DonGiaText, out int donGia))
                ResultDonGiaBan = donGia;
            else
                ResultDonGiaBan = null;
            if (int.TryParse(SoLuongText, out int soLuong))
                ResultSoLuongTon = soLuong;
            else
                ResultSoLuongTon = null;
            if (NgayNhap.HasValue)
                ResultNgayNhap = DateOnly.FromDateTime(NgayNhap.Value);
            else
                ResultNgayNhap = null;
            CloseAction?.Invoke(true);
        }
        // Đóng cửa sổ mà không lưu bất kỳ thay đổi nào
        private void ThucHienHuy(object parameter)
        {
            CloseAction?.Invoke(false);
        }
    }
}
