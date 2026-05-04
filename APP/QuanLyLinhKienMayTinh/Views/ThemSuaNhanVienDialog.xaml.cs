using QuanLyLinhKienMayTinh.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaNhanVienDialog : Window
    {
        public string MaNv { get; private set; }
        public string HoTen { get; private set; }
        public string ChucVu { get; private set; }
        public string GioiTinh { get; private set; }
        public string Sdt { get; private set; }
        public string Email { get; private set; }
        public DateOnly? NgaySinh { get; private set; }
        public DateOnly? NgayVaoLam { get; private set; }

        /// <summary>Mở ở chế độ THÊM</summary>
        public ThemSuaNhanVienDialog(string maNvMoi)
        {
            InitializeComponent();
            TitleText.Text = "Thêm Nhân Viên";
            TxtMaNv.Text = maNvMoi;
            DpNgayVaoLam.SelectedDate = DateTime.Now;
        }

        /// <summary>Mở ở chế độ SỬA</summary>
        public ThemSuaNhanVienDialog(NhanVienDisplay nv)
        {
            InitializeComponent();
            TitleText.Text = "Sửa Nhân Viên";
            BtnLuu.Content = "Cập nhật";
            TxtMaNv.Text = nv.MaNv;
            TxtHoTen.Text = nv.HoTen;
            TxtSdt.Text = nv.Sdt;
            TxtEmail.Text = nv.Email;

            // Set chức vụ ComboBox
            if (!string.IsNullOrEmpty(nv.ChucVu))
                CboChucVu.Text = nv.ChucVu;

            if (nv.NgayVaoLam.HasValue)
                DpNgayVaoLam.SelectedDate = nv.NgayVaoLam.Value.ToDateTime(TimeOnly.MinValue);

            // Load thêm giới tính và ngày sinh từ DB
            using (var db = Models.DataProvider.Ins.GetContext())
            {
                var entity = db.NhanViens.Find(nv.MaNv);

                if (entity != null)
                {
                    if (!string.IsNullOrEmpty(entity.GioiTinh))
                    {
                        foreach (ComboBoxItem item in CboGioiTinh.Items)
                        {
                            if (item.Content.ToString() == entity.GioiTinh)
                            {
                                CboGioiTinh.SelectedItem = item;
                                break;
                            }
                        }
                    }
                    if (entity.NgaySinh.HasValue)
                        DpNgaySinh.SelectedDate = entity.NgaySinh.Value.ToDateTime(TimeOnly.MinValue);
                }
            }
        }
        

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtHoTen.Focus();
                return;
            }

            MaNv = TxtMaNv.Text.Trim();
            HoTen = TxtHoTen.Text.Trim();
            ChucVu = CboChucVu.Text?.Trim();
            Sdt = TxtSdt.Text.Trim();
            Email = TxtEmail.Text.Trim();

            if (CboGioiTinh.SelectedItem is ComboBoxItem gioiTinhItem)
                GioiTinh = gioiTinhItem.Content.ToString();

            if (DpNgaySinh.SelectedDate.HasValue)
                NgaySinh = DateOnly.FromDateTime(DpNgaySinh.SelectedDate.Value);
            if (DpNgayVaoLam.SelectedDate.HasValue)
                NgayVaoLam = DateOnly.FromDateTime(DpNgayVaoLam.SelectedDate.Value);

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
