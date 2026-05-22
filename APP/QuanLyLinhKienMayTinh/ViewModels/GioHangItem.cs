using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class GioHangItem : BaseViewModel
    {
        public string MaLk { get; set; }
        public string TenLk { get; set; }
        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                _soLuong = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThanhTien));
            }
        }
        private int _donGia;
        public int DonGia
        {
            get => _donGia;
            set
            {
                _donGia = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThanhTien));
            }
        }
        public int ThanhTien => SoLuong * DonGia;
        public int TonKho { get; set; }
    }
}
