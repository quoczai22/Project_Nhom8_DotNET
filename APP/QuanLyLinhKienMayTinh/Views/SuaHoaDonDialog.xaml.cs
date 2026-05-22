using QuanLyLinhKienMayTinh.ViewModels;
using System.Windows;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class SuaHoaDonDialog : Window
    {
        public SuaHoaDonDialog(string maHd)
        {
            InitializeComponent();

            var vm = new SuaHoaDonDialogViewModel(maHd);

            // Gắn hành động đóng cửa sổ để ViewModel có thể điều khiển việc đóng dialog
            vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };

            DataContext = vm;
        }
    }
}
