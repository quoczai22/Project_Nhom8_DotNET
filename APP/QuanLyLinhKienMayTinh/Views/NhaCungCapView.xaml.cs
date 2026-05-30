using System.Windows.Controls;

namespace QuanLyLinhKienMayTinh.Views
{
    public partial class NhaCungCapView : Page
    {
        public NhaCungCapView()
        {
            InitializeComponent();
            DataContext = new ViewModels.NhaCungCapViewModel();
        }
    }
}
