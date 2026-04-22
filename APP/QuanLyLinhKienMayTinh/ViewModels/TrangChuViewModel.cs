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
                TongLK = db.LinhKiens.Sum(lk => lk.SoLuongTon ?? 0);
                TongLoaiLK = db.LoaiLks.Count();
                TongHD = db.HoaDons.Count();
                var year = Enumerable.Range(2023,4).ToList();

                // 1. Khởi tạo dữ liệu
                var giaTriDoanhThu = new ChartValues<double>();
                var danhSachThang = new List<string>();

                // 2. Lấy toàn bộ hóa đơn trong năm hiện tại về Memory (để tối ưu truy vấn)
                var hoaDonsTrongNam = db.HoaDons
                    .Where(hd => hd.NgayHd.HasValue && year.Contains(hd.NgayHd.Value.Year))
                    .Select(hd => new
                    {
                        Month = hd.NgayHd.Value.Month,
                        Year=hd.NgayHd.Value.Year,
                        TongTien = hd.TongTien ?? 0
                    })
                    .ToList();

                foreach (var y in year)
                {
                    for (int m = 1; m <= 12; m+=4)
                    {
                        var doanhThuThang = hoaDonsTrongNam
                            .Where(hd => hd.Month == m && hd.Year == y)
                            .Sum(hd => hd.TongTien);
                        giaTriDoanhThu.Add(doanhThuThang);
                        danhSachThang.Add($"Tháng {m}/{y}");
                    }
                }

                // 4. Cập nhật UI
                Labels = danhSachThang;
                Formatter = value => value.ToString("N0") + " đ";

                DoanhThu = new SeriesCollection
    {
                new LineSeries
                {
                    Title = $"Doanh thu từ năm 2023 đến 2026 ",
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
                        SoLuongBan = (int)(g.Sum(x => x.SoLuong) ?? 0),
                        DoanhThu = (double)(g.Sum(x => x.SoLuong * x.DonGia) ?? 0),
                        GiaTrungBinh = (double)(g.Average(x => x.DonGia) ?? 0),
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
