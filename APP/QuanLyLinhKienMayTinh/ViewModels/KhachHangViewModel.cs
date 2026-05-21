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
    // Display class khớp với binding trong KhachHangView.xaml
    public class KhachHangDisplay
    {
        public string MaKh { get; set; }
        public string HoTen { get; set; }  // map TenKh
        public string Sdt { get; set; }  // map Dthoai
        public string Email { get; set; }  // không có trong DB, để trống
        public string DiaChi { get; set; }  // map Dchi
    }

    public class KhachHangViewModel : BaseViewModel, ISearchable
    {
        private ObservableCollection<KhachHangDisplay> _all;

        private ICollectionView _danhSachKhachHang;
        public ICollectionView DanhSachKhachHang
        {
            get => _danhSachKhachHang;
            set { _danhSachKhachHang = value; OnPropertyChanged(); }
        }

        private KhachHangDisplay _khachHangChon;
        public KhachHangDisplay KhachHangChon
        {
            get => _khachHangChon;
            set { _khachHangChon = value; OnPropertyChanged(); }
        }

        private string _timKiem = string.Empty;
        public string TimKiem
        {
            get => _timKiem;
            set { _timKiem = value; OnPropertyChanged(); DanhSachKhachHang?.Refresh(); }
        }

        // Commands
        public ICommand ThemKhachHangCommand { get; private set; }
        public ICommand SuaKhachHangCommand { get; private set; }
        public ICommand XoaKhachHangCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        public KhachHangViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }
        // Tải danh sách khách hàng từ cơ sở dữ liệu và thiết lập bộ lọc hiển thị
        public void TaiDuLieu()
        {
            try
            {
                var list = DataProvider.Ins.GetContext().KhachHangs
                    .AsNoTracking()
                    .Select(kh => new KhachHangDisplay
                    {
                        MaKh = kh.MaKh,
                        HoTen = kh.TenKh,
                        Sdt = kh.Sdt,
                        Email = kh.Email,
                        DiaChi = kh.Dchi
                    }).ToList();

                _all = new ObservableCollection<KhachHangDisplay>(list);
                DanhSachKhachHang = CollectionViewSource.GetDefaultView(_all);
                DanhSachKhachHang.Filter = Filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu khách hàng: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Kiểm tra xem khách hàng có khớp với từ khóa tìm kiếm hay không
        private bool Filter(object obj)
        {
            if (obj is not KhachHangDisplay item) return false;
            if (string.IsNullOrWhiteSpace(TimKiem)) return true;

            string kw = TimKiem.ToLower();
            return (item.MaKh?.ToLower().Contains(kw) ?? false)
                || (item.HoTen?.ToLower().Contains(kw) ?? false)
                || (item.Sdt?.ToLower().Contains(kw) ?? false)
                || (item.DiaChi?.ToLower().Contains(kw) ?? false);
        }


        // Cập nhật từ khóa tìm kiếm khi người dùng nhập liệu trên thanh tìm kiếm
        public void ApplySearch(string keyword)
        {
            TimKiem = keyword?.Trim() ?? string.Empty;
        }


        private void KhoiTaoCommands()
        {
            ThemKhachHangCommand = new RelayCommand<object>(CanThemKhachHang, ThucHienThemKhachHang);
            SuaKhachHangCommand = new RelayCommand<KhachHangDisplay>(CanSuaKhachHang, ThucHienSuaKhachHang);
            XoaKhachHangCommand = new RelayCommand<KhachHangDisplay>(CanXoaKhachHang, ThucHienXoaKhachHang);
            LamMoiCommand = new RelayCommand<object>(CanLamMoi, ThucHienLamMoi);
        }

        // Kiểm tra điều kiện: Luôn cho phép người dùng mở cửa sổ thêm khách hàng mới
        private bool CanThemKhachHang(object parameter)
        {
            return true;
        }

        // Thực hiện mở cửa sổ thêm khách hàng, tự động tạo mã mới và lưu dữ liệu nếu người dùng đồng ý
        private void ThucHienThemKhachHang(object parameter)
        {
            try
            {
                var dbRead = DataProvider.Ins.GetContext();
                var lastID = dbRead.KhachHangs
                    .OrderByDescending(x => x.MaKh)
                    .Select(x => x.MaKh).FirstOrDefault();
                string newID = Services.AutoIDService.GetNextID("KH", lastID);

                var dialog = new ThemSuaKhachHangDialog(newID);
                var window = Application.Current.MainWindow;
                if(window != null && window.IsVisible && window.IsLoaded) {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    var kq = dialog.KetQua;
                    var khMoi = new KhachHang
                    {
                        MaKh = kq.MaKh,
                        TenKh = kq.HoTen,
                        Sdt = kq.Sdt,
                        Email = kq.Email,
                        Dchi = kq.DiaChi
                    };

                    var dbSave = DataProvider.Ins.GetContext();
                    dbSave.KhachHangs.Add(khMoi);
                    dbSave.SaveChanges();
                    TaiDuLieu();

                    MessageBox.Show("Thêm khách hàng thành công!",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Kiểm tra điều kiện: Chỉ cho phép sửa khi đã chọn một khách hàng cụ thể trong danh sách
        private bool CanSuaKhachHang(KhachHangDisplay kh)
        {
            return kh != null;
        }

        // Thực hiện mở cửa sổ sửa thông tin khách hàng đang chọn và cập nhật thay đổi vào cơ sở dữ liệu
        private void ThucHienSuaKhachHang(KhachHangDisplay kh)
        {
            try
            {
                var dialog = new ThemSuaKhachHangDialog(kh);
                var window = Application.Current.MainWindow;
                if(window != null && window.IsVisible && window.IsLoaded) {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    var kq = dialog.KetQua;
                    var db = DataProvider.Ins.GetContext();
                    var entity = db.KhachHangs.Find(kq.MaKh);
                    if (entity != null)
                    {
                        entity.TenKh = kq.HoTen;
                        entity.Sdt = kq.Sdt;
                        entity.Email = kq.Email;
                        entity.Dchi = kq.DiaChi;

                        db.SaveChanges();
                        TaiDuLieu();

                        MessageBox.Show("Cập nhật khách hàng thành công!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa khách hàng: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Kiểm tra điều kiện: Chỉ cho phép thao tác xóa khi đã chọn một khách hàng
        private bool CanXoaKhachHang(KhachHangDisplay kh)
        {
            return kh != null;
        }

        // Hiển thị hộp thoại xác nhận trước khi xóa khách hàng để tránh xóa nhầm
        private void ThucHienXoaKhachHang(KhachHangDisplay kh)
        {
            var res = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa khách hàng [{kh.HoTen}] không?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
                ThucHienXoa(kh);
        }

        // Kiểm tra điều kiện: Luôn cho phép làm mới lại danh sách khách hàng
        private bool CanLamMoi(object parameter)
        {
            return true;
        }

        // Thực hiện xóa từ khóa tìm kiếm hiện tại và tải lại dữ liệu mới nhất từ cơ sở dữ liệu
        private void ThucHienLamMoi(object parameter)
        {
            TimKiem = string.Empty;
            TaiDuLieu();
        }

        // Tiến hành xóa khách hàng khỏi cơ sở dữ liệu và loại bỏ khỏi danh sách đang hiển thị
        private void ThucHienXoa(KhachHangDisplay kh)
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                var entity = db.KhachHangs.Find(kh.MaKh);
                if (entity == null) return;

                db.KhachHangs.Remove(entity);
                db.SaveChanges();
                _all.Remove(kh);

                MessageBox.Show("Xóa khách hàng thành công!",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khách hàng: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}