using Microsoft.EntityFrameworkCore;
using QuanLyLinhKienMayTinh;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        string _loginUsername;
        public string LoginUsername
        {
            get { return _loginUsername; }
            set { _loginUsername = value; OnPropertyChanged(); }
        }

        string _loginPassword;
        public string LoginPassword
        {
            get { return _loginPassword; }
            set { _loginPassword = value; OnPropertyChanged(); }
        }

        string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        Visibility _loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {
            get { return _loginVisibility; }
            set { _loginVisibility = value; OnPropertyChanged(); }
        }

        string _message = "Vui lòng đăng nhập để tiếp tục";
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        bool _liPassVisible = false;
        public bool LiPassVisible
        {
            get { return _liPassVisible; }
            set { _liPassVisible = value; OnPropertyChanged(); }
        }

        bool _isDark = false; // cờ để theo dõi trạng thái theme hiện tại, mặc định là light

        public ICommand ToggleThemeCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        public ICommand ToggleLiPasswordCommand { get; set; }

        public LoginViewModel()
        {
            ToggleThemeCommand = new RelayCommand<object>(CanToggleTheme, ExecuteToggleTheme);
            LoginCommand = new RelayCommand<object>(CanLogin, ThucHienLogin);

            ToggleLiPasswordCommand = new RelayCommand<object>(CanExecuteAlways, ToggleLiPasswordExecute);
        }

        private bool CanExecuteAlways(object p)
        {
            return true;
        }

        private void ToggleLiPasswordExecute(object p)
        {
            object[] boxes = p as object[];// Nhận vào một mảng object chứa PasswordBox và TextBox tương ứng
            if (boxes != null && boxes.Length == 2)
            {
                PasswordBox pwdBox = boxes[0] as PasswordBox; // chuyển đổi thành PasswordBox
                TextBox txtBox = boxes[1] as TextBox; // chuyển đổi thành TextBox

                if (pwdBox != null && txtBox != null)
                {
                    if (!LiPassVisible) // Nếu đang ẩn (dấu chấm) chuẩn bị bật sang hiện chữ
                    {
                        LoginPassword = pwdBox.Password; // Đồng bộ text từ PasswordBox vào thuộc tính ViewModel
                    }
                    else // Nếu đang hiện chữ chuẩn bị quay về ẩn
                    {
                        pwdBox.Password = LoginPassword; // Gán ngược text lại cho PasswordBox trước khi đổi view
                    }
                }
            }
            LiPassVisible = !LiPassVisible;
        }

        private bool CanToggleTheme(object p)
        {
            return true;
        }

        private bool CanLogin(object p)
        {
            return true;
        }

        private void ThucHienLogin(object p)
        {
            // Đồng bộ dữ liệu lần cuối nếu người dùng đang ở chế độ ẩn mật khẩu
            if (!LiPassVisible)
            {
                PasswordBox pb = p as PasswordBox;
                if (pb != null)
                {
                    LoginPassword = pb.Password;
                }
            }

            ThucHienDangNhap(p);
        }
        // Xử lý xác thực tài khoản đăng nhập, cấp quyền tương ứng và mở giao diện chính
        public void ThucHienDangNhap(object p)
        {
            if (string.IsNullOrEmpty(LoginUsername) || string.IsNullOrEmpty(LoginPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
                return;
            }

            try
            {
                DataProvider.Ins.ResetToDefaultConnection();

                using var db = DataProvider.Ins.GetContext();

                var query = from t in db.TaiKhoans.Include("MaNvNavigation")
                            where t.TenDn == LoginUsername && t.MatKhau == LoginPassword
                            select t;

                var acc = query.FirstOrDefault(); // lấy thông tin tài khoản cùng với thông tin nhân viên

                if (acc != null)
                {
                    LuuTrangThai.MaNVDangNhap = acc.MaNv;
                    LuuTrangThai.QuyenDangNhap = acc.MaNvNavigation.Quyen;

                    DataProvider.Ins.ChangeConnectionByRole(LuuTrangThai.QuyenDangNhap);

                    MainWindow main = new MainWindow(LoginUsername);
                    main.Show();

                    foreach (Window item in Application.Current.Windows)
                    {
                        if (item is Views.LoginView)
                        {
                            item.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nội bộ, không hiển thị thông tin kỹ thuật ra cho người dùng
                System.Diagnostics.Debug.WriteLine(string.Format("[LoginError] {0}", ex));
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.");
            }
        }

        // Chuyển đổi qua lại giữa giao diện Sáng (Light mode) và Tối (Dark mode)
        void ExecuteToggleTheme(object obj)
        {
            _isDark = !_isDark;
            string themeFile = _isDark ? "../Themes/ThemeDark.xaml" : "../Themes/ThemeLight.xaml";

            try
            {
                var newThemeDict = new ResourceDictionary
                {
                    Source = new Uri(themeFile, UriKind.RelativeOrAbsolute) // tạo ResourceDictionary mới với đường dẫn đến file theme tương ứng
                };

                var mergedDicts = Application.Current.Resources.MergedDictionaries; // lấy danh sách ResourceDictionary đã được gộp vào tài nguyên ứng dụng

                ResourceDictionary oldThemeDict = null;
                foreach (ResourceDictionary d in mergedDicts)
                {
                    if (d.Source != null && d.Source.OriginalString.Contains("Theme"))
                    {
                        oldThemeDict = d; // tìm ResourceDictionary có chứa "Theme" 
                        break;
                    }
                }

                mergedDicts.Add(newThemeDict); // thêm ResourceDictionary mới vào danh sách gộp để áp dụng theme mới cho toàn bộ ứng dụng

                if (oldThemeDict != null)
                {
                    mergedDicts.Remove(oldThemeDict);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Lỗi chuyển theme: {0}", ex.Message));
            }
        }
    }
}