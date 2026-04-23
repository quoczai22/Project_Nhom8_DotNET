using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.ViewModels;
using System;
using System.Linq;
using System.Windows;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaLinhKienDialog : Window
    {
        public string MaLk { get; private set; }
        public string TenLk { get; private set; }
        public string MaLoai { get; private set; }
        public string MaNsx { get; private set; }
        public string Dvt { get; private set; }
        public byte? Tgbh { get; private set; }
        public int? DonGiaBan { get; private set; }
        public int? SoLuongTon { get; private set; }
        public DateOnly? NgayNhap { get; private set; }

        /// <summary>Mở ở chế độ THÊM</summary>
        public ThemSuaLinhKienDialog(string maLkMoi)
        {
            InitializeComponent();
            TitleText.Text = "Thêm Linh Kiện";
            TxtMaLk.Text = maLkMoi;
            DpNgayNhap.SelectedDate = DateTime.Now;
            TaiDanhSachComboBox();
        }

        /// <summary>Mở ở chế độ SỬA</summary>
        public ThemSuaLinhKienDialog(LinhKienDisplay lk)
        {
            InitializeComponent();
            TitleText.Text = "Sửa Linh Kiện";
            BtnLuu.Content = "Cập nhật";
            TxtMaLk.Text = lk.MaLk;
            TxtTenLk.Text = lk.TenLk;
            TxtDvt.Text = lk.Dvt;
            TxtTgbh.Text = lk.Tgbh?.ToString();
            if (lk.NgayNhap.HasValue)
                DpNgayNhap.SelectedDate = lk.NgayNhap.Value.ToDateTime(TimeOnly.MinValue);

            TaiDanhSachComboBox();

            // Load thêm thông tin từ DB cho Sửa
            var entity = DataProvider.Ins.DB.LinhKiens.AsNoTracking().FirstOrDefault(x => x.MaLk == lk.MaLk);
            if (entity != null)
            {
                TxtDonGia.Text = entity.DonGiaBan?.ToString();
                TxtSoLuong.Text = entity.SoLuongTon?.ToString();

                // Select đúng item trong ComboBox
                for (int i = 0; i < CboLoai.Items.Count; i++)
                {
                    if (((LoaiLk)CboLoai.Items[i]).MaLoai == entity.MaLoai)
                    { CboLoai.SelectedIndex = i; break; }
                }
                for (int i = 0; i < CboNsx.Items.Count; i++)
                {
                    if (((NhaSanXuat)CboNsx.Items[i]).MaNsx == entity.MaNsx)
                    { CboNsx.SelectedIndex = i; break; }
                }
            }
        }

        private void TaiDanhSachComboBox()
        {
            var db = DataProvider.Ins.DB;
            CboLoai.ItemsSource = db.LoaiLks.AsNoTracking().OrderBy(l => l.TenLoai).ToList();
            CboNsx.ItemsSource = db.NhaSanXuats.AsNoTracking().OrderBy(n => n.TenNsx).ToList();
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTenLk.Text))
            {
                MessageBox.Show("Vui lòng nhập tên linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTenLk.Focus();
                return;
            }
            if (CboLoai.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CboNsx.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhà sản xuất!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MaLk = TxtMaLk.Text.Trim();
            TenLk = TxtTenLk.Text.Trim();
            MaLoai = ((LoaiLk)CboLoai.SelectedItem).MaLoai;
            MaNsx = ((NhaSanXuat)CboNsx.SelectedItem).MaNsx;
            Dvt = TxtDvt.Text.Trim();

            if (byte.TryParse(TxtTgbh.Text, out byte tgbh))
                Tgbh = tgbh;
            if (int.TryParse(TxtDonGia.Text, out int donGia))
                DonGiaBan = donGia;
            if (int.TryParse(TxtSoLuong.Text, out int soLuong))
                SoLuongTon = soLuong;
            if (DpNgayNhap.SelectedDate.HasValue)
                NgayNhap = DateOnly.FromDateTime(DpNgayNhap.SelectedDate.Value);

            DialogResult = true;
            Close();
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
