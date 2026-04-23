using QuanLyLinhKienMayTinh.ViewModels;
using System.Windows;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class ThemSuaLoaiLinhKienDialog : Window
    {
        public LoaiLkDisplay KetQua { get; private set; }

        /// <summary>Mở ở chế độ THÊM</summary>
        public ThemSuaLoaiLinhKienDialog(string maLoaiMoi)
        {
            InitializeComponent();
            TitleText.Text = "Thêm Loại Linh Kiện";
            TxtMaLoai.Text = maLoaiMoi;
        }

        /// <summary>Mở ở chế độ SỬA</summary>
        public ThemSuaLoaiLinhKienDialog(LoaiLkDisplay loai)
        {
            InitializeComponent();
            TitleText.Text = "Sửa Loại Linh Kiện";
            BtnLuu.Content = "Cập nhật";
            TxtMaLoai.Text = loai.MaLoai;
            TxtTenLoai.Text = loai.TenLoai;
            TxtMoTa.Text = loai.MoTa;
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTenLoai.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại linh kiện!", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTenLoai.Focus();
                return;
            }

            KetQua = new LoaiLkDisplay
            {
                MaLoai = TxtMaLoai.Text.Trim(),
                TenLoai = TxtTenLoai.Text.Trim(),
                MoTa = TxtMoTa.Text.Trim()
            };

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
