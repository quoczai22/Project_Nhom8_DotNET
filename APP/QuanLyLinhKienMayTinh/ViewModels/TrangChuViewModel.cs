using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class TrangChuViewModel : BaseViewModel
    {
        private SeriesCollection _doanhThu;
        public SeriesCollection DoanhThu
        {
            get { return _doanhThu;  }
            set
            {
                _doanhThu = value;
                OnPropertyChanged();
            }
        }

        private List<string> _lables;
        public List<string> Labels
        {
            get { return _lables; }
            set
            {
                _lables = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> Formatter { get; set; }

        private SeriesCollection _chuVu;
        public SeriesCollection ChucVu
        {
            get { return _chuVu; }
            set
            {
                _chuVu = value;
                OnPropertyChanged();
            }
        }
        private List<ThongKeBanHang> _danhSachThongKeHang;
        public List<ThongKeBanHang> DanhSachThongKeBanHang 
        {
            get { return _danhSachThongKeHang; }
            set
            {
                _danhSachThongKeHang = value;
                OnPropertyChanged();
            }
        }
        private int _tongNV;

        public int TongNV
        {
            get { return _tongNV; }
            set
            {
                _tongNV = value;
                OnPropertyChanged();
            }
        }

        private int _tongKH;

        public int TongKH
        {
            get { return _tongKH; }
            set
            {
                _tongKH = value;
                OnPropertyChanged();
            }
        }

        private int _tongLK;
        public int TongLK
        {
            get { return _tongLK; }
            set
            {
                _tongLK = value;
                OnPropertyChanged();
            }
        }

        private int _tongLoaiLK;
        public int TongLoaiLK
        {
            get { return _tongLoaiLK; }
            set
            {
                _tongLoaiLK = value;
                OnPropertyChanged();
            }
        }

        private int _tongHD;
        public int TongHD
        {
            get { return _tongHD; }
            set
            {
                _tongHD = value;
                OnPropertyChanged();
            }
        }

        public TrangChuViewModel()
        {
            DoanhThu = new SeriesCollection();
            Labels = new List<string>();
            ChucVu = new SeriesCollection();
            DanhSachThongKeBanHang = new List<ThongKeBanHang>();

            TaiDuLieu();
            LoadPieChart();
        }
        public void TaiDuLieu()
        {
            try
            {
                var db = DataProvider.Ins.DB;
                TongNV = db.NhanViens.Count();
                TongKH = db.KhachHangs.Count();
                TongLK = db.LinhKiens.Count();
                TongLoaiLK = db.LoaiLks.Count();
                TongHD = db.HoaDons.Count();
                int namHienTai = DateTime.Now.Year;

                // 1. Khởi tạo dữ liệu
                var giaTriDoanhThu = new ChartValues<double>();
                var danhSachThang = new List<string>();

                // 2. Lấy toàn bộ hóa đơn trong năm hiện tại về Memory (để tối ưu truy vấn)
                var hoaDonsTrongNam = db.HoaDons
                    .Where(hd => hd.NgayHd.HasValue && hd.NgayHd.Value.Year == namHienTai)
                    .Select(hd => new { hd.NgayHd.Value.Month, hd.TongTien })
                    .ToList();

                // 3. Xử lý dữ liệu cho 12 tháng bằng LINQ to Object
                for (int i = 1; i <= 12; i++)
                {
                    // Tính tổng tiền của tháng i (ORM xử lý lọc từ danh sách đã lấy)
                    var tongDoanhThuThang = hoaDonsTrongNam
                        .Where(hd => hd.Month == i)
                        .Sum(hd => (double?)hd.TongTien) ?? 0;

                    giaTriDoanhThu.Add(tongDoanhThuThang);
                    danhSachThang.Add($"T{i}");
                }

                // 4. Cập nhật UI
                Labels = danhSachThang;
                Formatter = value => value.ToString("N0") + " đ";

                DoanhThu = new SeriesCollection
    {
                new LineSeries
                {
                    Title = $"Doanh thu {namHienTai}",
                    Values = giaTriDoanhThu,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3f51b5")),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#153f51b5"))
                }
            };

                var hangSX = db.ChiTietHds
                    .Include(ct => ct.MaLkNavigation)
                    .Include(ct => ct.MaHdNavigation)
                    .Include(ct => ct.MaLkNavigation.MaNsxNavigation)
                    .GroupBy(ct => ct.MaLkNavigation.MaNsxNavigation.TenNsx)
                    .Select(g => new ThongKeBanHang
                    {
                        HangSX = g.Key,
                        SoLuongBan = (int)g.Sum(x => x.SoLuong),
                        DoanhThu = (double)g.Sum(x => x.SoLuong * x.DonGia),
                        GiaTrungBinh = (double)g.Average(x => x.DonGia),
                        SoDonHang = g.Select(x => x.MaHd).Distinct().Count()
                    })
                    .ToList();

                DanhSachThongKeBanHang = hangSX;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void LoadPieChart()
        {
            try
            {
                ChucVu.Clear();
            var db = DataProvider.Ins.DB;
            var chucVuNV = db.NhanViens
                .GroupBy(nv => nv.ChucVu)
                .Select(g => new { ChucVu = g.Key, SoLuong = g.Count() })
                .ToList();

            var danhSachMau = new List<string> { "#ff9800", "#e91e63", "#2196f3", "#4caf50", "#9c27b0" };

            for (int i = 0; i < chucVuNV.Count; i++)
            {
                ChucVu.Add(new PieSeries
                {
                    Title = chucVuNV[i].ChucVu,
                    Values = new ChartValues<double> { chucVuNV[i].SoLuong },
                    DataLabels = true,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(danhSachMau[i % danhSachMau.Count]))
                });
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
