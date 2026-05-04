using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _currentUsername;
        public string CurrentUsername
        {
            get => _currentUsername;
            set { _currentUsername = value; OnPropertyChanged(); }
        }

        private int soNhanVienGoc = 0;
        private int soLinhKienGoc = 0;
        private int soHoaDonGoc = 0;
        bool _isDark = false;

        public ICommand ThongBaoCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand ToggleThemeCommand { get; set; }

        public MainViewModel(string username)
        {
            CurrentUsername = username;
            LaySoLuongGoc();

            ThongBaoCommand = new RelayCommand<object>((p) => true, (p) => ThongBao());
            LogOutCommand = new RelayCommand<object>((p) => true, (p) => LogOut());
            ToggleThemeCommand = new RelayCommand<object>((p) => true, (p) => ExecuteToggleTheme(p));
        }

        private void LaySoLuongGoc()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                soNhanVienGoc = db.NhanViens.Count();
                soLinhKienGoc = db.LinhKiens.Count();
                soHoaDonGoc = db.HoaDons.Count();
            }
            catch { }
        }

        public void ThongBao()
        {
            try
            {
                var db = DataProvider.Ins.GetContext();
                int soNhanVienMoi = db.NhanViens.Count();
                int soLinhKienMoi = db.LinhKiens.Count();
                int soHoaDonMoi = db.HoaDons.Count();

                List<string> danhSachThongBao = new List<string>();

                if (soNhanVienMoi > soNhanVienGoc)
                    danhSachThongBao.Add($"- Có {soNhanVienMoi - soNhanVienGoc} nhân viên MỚI gia nhập.");
                else if (soNhanVienMoi < soNhanVienGoc)
                    danhSachThongBao.Add($"- Có {soNhanVienGoc - soNhanVienMoi} nhân viên ĐÃ NGHỈ VIỆC.");

                if (soLinhKienMoi > soLinhKienGoc)
                    danhSachThongBao.Add($"- Có {soLinhKienMoi - soLinhKienGoc} mã linh kiện MỚI.");
                else if (soLinhKienMoi < soLinhKienGoc)
                    danhSachThongBao.Add($"- Có {soLinhKienGoc - soLinhKienMoi} mã linh kiện bị xóa.");

                if (soHoaDonMoi > soHoaDonGoc)
                    danhSachThongBao.Add($"- Có {soHoaDonMoi - soHoaDonGoc} hóa đơn MỚI.");

                if (danhSachThongBao.Count > 0)
                {
                    MessageBox.Show("Hệ thống có thay đổi:\n\n" + string.Join("\n", danhSachThongBao),
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    soNhanVienGoc = soNhanVienMoi;
                    soLinhKienGoc = soLinhKienMoi;
                    soHoaDonGoc = soHoaDonMoi;
                }
                else
                {
                    MessageBox.Show("Chưa có thay đổi dữ liệu mới.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông báo: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LogOut()
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn đăng xuất không?", "Thông báo",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginView login = new LoginView();
                login.Show();
                Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
            }
        }
        private void ExecuteToggleTheme(object obj)
        {

            _isDark = !_isDark;
            string themeFile = _isDark ? "../Themes/ThemeDark.xaml" : "../Themes/ThemeLight.xaml";

            try
            {
                var newThemeDict = new ResourceDictionary
                {
                    Source = new Uri(themeFile, UriKind.RelativeOrAbsolute)
                };

                var mergedDicts = Application.Current.Resources.MergedDictionaries;
                var oldThemeDict = mergedDicts.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));
                mergedDicts.Add(newThemeDict);
                if (oldThemeDict != null)
                {
                    mergedDicts.Remove(oldThemeDict);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi chuyển theme: {ex.Message}");
            }
        }
    }
}
