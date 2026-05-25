using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLyLinhKienMayTinh.Views
{
    /// <summary>
    /// Interaction logic for ThemPhieuNhapDialog.xaml
    /// </summary>
    public partial class ThemPhieuNhapDialog : Window
    {
        private readonly ThemPhieuNhapDialogViewModel _vm;
        public PhieuNhap PhieuNhapMoi => _vm.PhieuNhapMoi;
        public List<ChiTietPn> ChiTietPns => _vm.ChiTietPns;

        public ThemPhieuNhapDialog(string maPnMoi)
        {
            InitializeComponent();
            _vm = new ThemPhieuNhapDialogViewModel(maPnMoi);
            DataContext = _vm;
            _vm.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
