using QuanLyLinhKienMayTinh.Models;
using QuanLyLinhKienMayTinh.ViewModels;
using QuanLyLinhKienMayTinh.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyLinhKienMayTinh
{

    public partial class MainWindow : Window
    {

        public MainWindow(string username)
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(username);
            MainFrame.Navigate(new TrangChuView());
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TrangChuView());
            txtSearch.Clear();
        }

        private void btnLinhKien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LinhKienView());
            txtSearch.Clear();
        }

        private void btnLoaiLK_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoaiLinhKienView());
            txtSearch.Clear();
        }

        private void btnKhachHang_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new KhachHang());
            txtSearch.Clear();
        }

        private void btnHoaDon_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HoaDon());
            txtSearch.Clear();
        }

        private void btnNhanVien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new NhanVien());
            txtSearch.Clear();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string word = txtSearch.Text.Trim();
            if (MainFrame.Content is Page currentPage && currentPage.DataContext is ISearchable searchable)
            {                 
                searchable.ApplySearch(word); 
            }
        }
    }
}
