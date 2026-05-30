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
    public class NhaCungCapDisplay
    {
        public string MaNsx { get; set; }
        public string TenNsx { get; set; }
        public string QuocGia { get; set; }
        public string Sdt { get; set; }
    }

    public class NhaCungCapViewModel : BaseViewModel, ISearchable
    {
        private ObservableCollection<NhaCungCapDisplay> _all;

        private ICollectionView _danhSachNhaCungCap;
        public ICollectionView DanhSachNhaCungCap
        {
            get => _danhSachNhaCungCap;
            set { _danhSachNhaCungCap = value; OnPropertyChanged(); }
        }

        private NhaCungCapDisplay _nhaCungCapChon;
        public NhaCungCapDisplay NhaCungCapChon
        {
            get => _nhaCungCapChon;
            set { _nhaCungCapChon = value; OnPropertyChanged(); }
        }

        private string _timKiem = string.Empty;
        public string TimKiem
        {
            get => _timKiem;
            set { _timKiem = value; OnPropertyChanged(); DanhSachNhaCungCap?.Refresh(); }
        }

        public ICommand ThemNhaCungCapCommand { get; private set; }
        public ICommand SuaNhaCungCapCommand { get; private set; }
        public ICommand XoaNhaCungCapCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        public NhaCungCapViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }

        public void TaiDuLieu()
        {
            try
            {
                var list = DataProvider.Ins.GetContext().NhaSanXuats
                    .AsNoTracking()
                    .Select(nsx => new NhaCungCapDisplay
                    {
                        MaNsx = nsx.MaNsx,
                        TenNsx = nsx.TenNsx,
                        QuocGia = nsx.QuocGia,
                        Sdt = nsx.Sdt
                    }).ToList();

                _all = new ObservableCollection<NhaCungCapDisplay>(list);
                DanhSachNhaCungCap = CollectionViewSource.GetDefaultView(_all);
                DanhSachNhaCungCap.Filter = Filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu nhà cung cấp: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Filter(object obj)
        {
            if (obj is not NhaCungCapDisplay item) return false;
            if (string.IsNullOrWhiteSpace(TimKiem)) return true;

            string kw = TimKiem.ToLower();
            return (item.MaNsx?.ToLower().Contains(kw) ?? false)
                || (item.TenNsx?.ToLower().Contains(kw) ?? false)
                || (item.QuocGia?.ToLower().Contains(kw) ?? false)
                || (item.Sdt?.ToLower().Contains(kw) ?? false);
        }

        public void ApplySearch(string keyword)
        {
            TimKiem = keyword?.Trim() ?? string.Empty;
        }

        private void KhoiTaoCommands()
        {
            ThemNhaCungCapCommand = new RelayCommand<object>(CanThemNhaCungCap, ThucHienThemNhaCungCap);
            SuaNhaCungCapCommand = new RelayCommand<NhaCungCapDisplay>(CanSuaNhaCungCap, ThucHienSuaNhaCungCap);
            XoaNhaCungCapCommand = new RelayCommand<NhaCungCapDisplay>(CanXoaNhaCungCap, ThucHienXoaNhaCungCap);
            LamMoiCommand = new RelayCommand<object>(CanLamMoi, ThucHienLamMoi);
        }

        private bool CanThemNhaCungCap(object parameter)
        {
            return true;
        }

        private void ThucHienThemNhaCungCap(object parameter)
        {
            try
            {
                var dbRead = DataProvider.Ins.GetContext();
                var lastID = dbRead.NhaSanXuats
                    .OrderByDescending(x => x.MaNsx)
                    .Select(x => x.MaNsx).FirstOrDefault();
                string newID = Services.AutoIDService.GetNextNhaSanXuatID(lastID);

                var dialog = new ThemSuaNhaCungCapDialog(newID);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    var kq = dialog.KetQua;
                    var nsxMoi = new NhaSanXuat
                    {
                        MaNsx = kq.MaNsx,
                        TenNsx = kq.TenNsx,
                        QuocGia = kq.QuocGia,
                        Sdt = kq.Sdt
                    };

                    var dbSave = DataProvider.Ins.GetContext();
                    dbSave.NhaSanXuats.Add(nsxMoi);
                    dbSave.SaveChanges();
                    TaiDuLieu();

                    MessageBox.Show("Thêm nhà cung cấp thành công!",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhà cung cấp: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSuaNhaCungCap(NhaCungCapDisplay nsx)
        {
            return nsx != null;
        }

        private void ThucHienSuaNhaCungCap(NhaCungCapDisplay nsx)
        {
            try
            {
                var dialog = new ThemSuaNhaCungCapDialog(nsx);
                var window = Application.Current.MainWindow;
                if (window != null && window.IsVisible && window.IsLoaded)
                {
                    dialog.Owner = window;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                if (dialog.ShowDialog() == true)
                {
                    var kq = dialog.KetQua;
                    var db = DataProvider.Ins.GetContext();
                    var entity = db.NhaSanXuats.Find(kq.MaNsx);
                    if (entity != null)
                    {
                        entity.TenNsx = kq.TenNsx;
                        entity.QuocGia = kq.QuocGia;
                        entity.Sdt = kq.Sdt;

                        db.SaveChanges();
                        TaiDuLieu();

                        MessageBox.Show("Cập nhật nhà cung cấp thành công!",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa nhà cung cấp: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanXoaNhaCungCap(NhaCungCapDisplay nsx)
        {
            return nsx != null;
        }

        private void ThucHienXoaNhaCungCap(NhaCungCapDisplay nsx)
        {
            var res = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhà cung cấp [{nsx.TenNsx}] không?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
                ThucHienXoa(nsx);
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

        private void ThucHienXoa(NhaCungCapDisplay nsx)
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                var entity = db.NhaSanXuats.Find(nsx.MaNsx);
                if (entity == null) return;

                db.NhaSanXuats.Remove(entity);
                db.SaveChanges();
                _all.Remove(nsx);

                MessageBox.Show("Xóa nhà cung cấp thành công!",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhà cung cấp: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
