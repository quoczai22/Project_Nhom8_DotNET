using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QuanLyLinhKienMayTinh.Services;
using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.ViewModels; 

namespace QuanLyLinhKienMayTinh
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                DataProvider.Ins.GetContext().Database.EnsureCreated();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Chi tiết lỗi: " + ex.Message, "Lỗi Đăng Nhập",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}