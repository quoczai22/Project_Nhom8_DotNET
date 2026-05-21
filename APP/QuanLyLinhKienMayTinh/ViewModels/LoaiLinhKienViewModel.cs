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
    // Display class khớp với binding trong LoaiLinhKienView.xaml
    public class LoaiLkDisplay
    {
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
        public string MoTa { get; set; }  // không có trong DB, để trống
    }

    public class LoaiLinhKienViewModel : BaseViewModel, ISearchable
    {
        // Backing collection 
        private ObservableCollection<LoaiLkDisplay> _all;

        // Bound to DataGrid 
        private ICollectionView _danhSachLoaiLinhKien;
        public ICollectionView DanhSachLoaiLinhKien
        {
            get => _danhSachLoaiLinhKien;
            set { _danhSachLoaiLinhKien = value; OnPropertyChanged(); }
        }

        private LoaiLkDisplay _loaiChon;
        public LoaiLkDisplay LoaiChon
        {
            get => _loaiChon;
            set { _loaiChon = value; OnPropertyChanged(); }
        }

        // Search box 
        private string _timKiem = string.Empty;
        public string TimKiem
        {
            get => _timKiem;
            set { _timKiem = value; OnPropertyChanged(); DanhSachLoaiLinhKien?.Refresh(); }
        }

        // Commands 
        public ICommand ThemLoaiCommand { get; private set; }
        public ICommand SuaLoaiCommand { get; private set; }
        public ICommand XoaLoaiCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        public LoaiLinhKienViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }

        // Tải danh sách loại linh kiện từ cơ sở dữ liệu để hiển thị lên bảng
        public void TaiDuLieu()
        {
            try
            {
                var list = DataProvider.Ins.GetContext().LoaiLks
                    .AsNoTracking()
                    .Select(lk => new LoaiLkDisplay
                    {
                        MaLoai = lk.MaLoai,
                        TenLoai = lk.TenLoai,
                        MoTa = lk.MoTa,
                    }).ToList();

                _all = new ObservableCollection<LoaiLkDisplay>(list);
                DanhSachLoaiLinhKien = CollectionViewSource.GetDefaultView(_all);
                DanhSachLoaiLinhKien.Filter = Filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu loại linh kiện: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Kiểm tra xem loại linh kiện này có khớp với từ khóa người dùng đang tìm kiếm hay không
        private bool Filter(object obj)
        {
            if (obj is not LoaiLkDisplay item) return false;
            if (string.IsNullOrWhiteSpace(TimKiem)) return true;

            string kw = TimKiem.ToLower();
            return (item.MaLoai?.ToLower().Contains(kw) ?? false)
                || (item.TenLoai?.ToLower().Contains(kw) ?? false);
        }

        // Nhận từ khóa tìm kiếm từ giao diện và áp dụng bộ lọc hiển thị
        public void ApplySearch(string keyword)
        {
            TimKiem = keyword?.Trim() ?? string.Empty;
        }

        // Cấu hình các thao tác Thêm, Sửa, Xóa và Làm mới cho các nút bấm trên giao diện
        private void KhoiTaoCommands()
        {
            ThemLoaiCommand = new RelayCommand<object>(CanThemLoai, ThucHienThemLoai);
            SuaLoaiCommand = new RelayCommand<LoaiLkDisplay>(CanSuaLoai, ThucHienSuaLoai);
            XoaLoaiCommand = new RelayCommand<LoaiLkDisplay>(CanXoaLoai, ThucHienXoaLoai);
            LamMoiCommand = new RelayCommand<object>(CanLamMoi, ThucHienLamMoi);
        }

        private bool CanThemLoai(object parameter)
        {
            return true;
        }

        private void ThucHienThemLoai(object parameter)
        {
            try
            {
                var dbRead = DataProvider.Ins.GetContext();
                var lastID = dbRead.LoaiLks
                    .OrderByDescending(x => x.MaLoai)
                    .Select(x => x.MaLoai).FirstOrDefault();
                string newID = Services.AutoIDService.GetNextLoaiID(lastID);

                var dialog = new ThemSuaLoaiLinhKienDialog(newID);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                }

                if (dialog.ShowDialog() == true)
                {
                    var kq = dialog.KetQua;
                    var loaiMoi = new LoaiLk
                    {
                        MaLoai = kq.MaLoai,
                        TenLoai = kq.TenLoai,
                        MoTa = kq.MoTa
                    };

                    var dbSave = DataProvider.Ins.GetContext();
                    dbSave.LoaiLks.Add(loaiMoi);
                    dbSave.SaveChanges();
                    TaiDuLieu();

                    MessageBox.Show("Thêm loại linh kiện thành công!",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm loại linh kiện: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSuaLoai(LoaiLkDisplay loai)
        {
            return loai != null;
        }

        private void ThucHienSuaLoai(LoaiLkDisplay loai)
        {
            try
            {
                var dialog = new ThemSuaLoaiLinhKienDialog(loai);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                }

                if (dialog.ShowDialog() == true)
                {
                    var kq = dialog.KetQua;
                    var db = DataProvider.Ins.GetContext();
                    var entity = db.LoaiLks.Find(kq.MaLoai);
                    if (entity != null)
                    {
                        entity.TenLoai = kq.TenLoai;
                        entity.MoTa = kq.MoTa;

                        db.SaveChanges();
                        TaiDuLieu();

                        MessageBox.Show("Cập nhật loại linh kiện thành công!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa loại linh kiện: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanXoaLoai(LoaiLkDisplay loai)
        {
            return loai != null;
        }

        private void ThucHienXoaLoai(LoaiLkDisplay loai)
        {
            var res = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa loại [{loai.TenLoai}] không?\n" +
                "Lưu ý: Không thể xóa nếu còn linh kiện thuộc loại này.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
                ThucHienXoa(loai);
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

        // Tiến hành xóa loại linh kiện khỏi hệ thống và loại bỏ khỏi danh sách đang hiển thị
        private void ThucHienXoa(LoaiLkDisplay loai)
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                var entity = db.LoaiLks.Find(loai.MaLoai);
                if (entity == null) return;

                db.LoaiLks.Remove(entity);
                db.SaveChanges();
                _all.Remove(loai);

                MessageBox.Show("Xóa loại linh kiện thành công!",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa loại linh kiện: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}