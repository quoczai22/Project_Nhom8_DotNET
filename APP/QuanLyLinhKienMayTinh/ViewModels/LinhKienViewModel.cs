using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace QuanLyLinhKienMayTinh.ViewModels
{
    public class LinhKienViewModel : BaseViewModel, ISearchable
    {
        private ObservableCollection<LinhKien> _danhSachLinhKien;
        public ObservableCollection<LinhKien> DanhSachLinhKien
        {
            get => _danhSachLinhKien;
            set
            {
                _danhSachLinhKien = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _danhSachLinhKienView;
        public ICollectionView DanhSachLinhKienView
        {
            get => _danhSachLinhKienView;
            set
            {
                _danhSachLinhKienView = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                DanhSachLinhKienView?.Refresh();
            }
        }

        public LinhKienViewModel()
        {
            TaiDuLieu();
        }

        public void TaiDuLieu()
        {
            try
            {
                var data = DataProvider.Ins.DB.LinhKiens.ToList();
                DanhSachLinhKien = new ObservableCollection<LinhKien>(data);
                DanhSachLinhKienView = CollectionViewSource.GetDefaultView(DanhSachLinhKien);
                DanhSachLinhKienView.Filter = FilterLinhKien;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        public void ApplySearch(string keyword)
        {
            SearchKeyword = keyword?.Trim() ?? string.Empty;
        }

        private bool FilterLinhKien(object obj)
        {
            if (obj is not LinhKien item)
                return false;

            if (string.IsNullOrWhiteSpace(SearchKeyword))
                return true;

            string keyword = SearchKeyword.ToLower();

            return (item.MaLk?.ToLower().Contains(keyword) ?? false)
                || (item.TenLk?.ToLower().Contains(keyword) ?? false)
                || (item.MaLoai?.ToLower().Contains(keyword) ?? false)
                || (item.Nsx?.ToLower().Contains(keyword) ?? false)
                || (item.Dvt?.ToLower().Contains(keyword) ?? false)
                || (item.Tgbh?.ToString().Contains(keyword) ?? false)
                || (item.NgaySx?.ToString().ToLower().Contains(keyword) ?? false);
        }
    }
}