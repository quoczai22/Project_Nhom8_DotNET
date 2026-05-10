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

        public ICommand ThemKhachHangCommand { get; private set; }
        public ICommand SuaKhachHangCommand { get; private set; }
        public ICommand XoaKhachHangCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        public KhachHangViewModel()
        {
            TaiDuLieu();
            KhoiTaoCommands();
        }
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


        public void ApplySearch(string keyword)
        {
            TimKiem = keyword?.Trim() ?? string.Empty;
        }


        private void KhoiTaoCommands()
        {
            // ── THÊM ─────────────────────────────────────────────────────────
            ThemKhachHangCommand = new RelayCommand<object>(_ => true, _ =>
            {
                try
                {
                    var dbRead = DataProvider.Ins.GetContext();
                    var lastID = dbRead.KhachHangs
                        .OrderByDescending(x => x.MaKh)
                        .Select(x => x.MaKh).FirstOrDefault();
                    string newID = Services.AutoIDService.GetNextID("KH", lastID);

                    var dialog = new ThemSuaKhachHangDialog(newID);
                    dialog.Owner = Application.Current.MainWindow;
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
            });

            // ── SỬA ──────────────────────────────────────────────────────────
            SuaKhachHangCommand = new RelayCommand<KhachHangDisplay>(kh => kh != null, kh =>
            {
                try
                {
                    var dialog = new ThemSuaKhachHangDialog(kh);
                    dialog.Owner = Application.Current.MainWindow;
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
            });

            // ── XÓA ──────────────────────────────────────────────────────────
            XoaKhachHangCommand = new RelayCommand<KhachHangDisplay>(kh => kh != null, kh =>
            {
                var res = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa khách hàng [{kh.HoTen}] không?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                    ThucHienXoa(kh);
            });

            // ── LÀM MỚI ─────────────────────────────────────────────────────
            LamMoiCommand = new RelayCommand<object>(
                _ => true,
                _ => { TimKiem = string.Empty; TaiDuLieu(); });
        }

        // ── Xóa khách hàng ───────────────────────────────────────────────
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