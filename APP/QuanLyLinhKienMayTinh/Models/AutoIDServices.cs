using System.Linq;
using QuanLyLinhKienMayTinh.Models;

namespace QuanLyLinhKienMayTinh.Services
{
    public static class AutoIDService
    {
        public static string GetNextID(string prefix, string lastID)
        {
            if (string.IsNullOrEmpty(lastID)) return prefix + "001";
            int number = int.Parse(lastID.Substring(prefix.Length)) + 1;
            return prefix + number.ToString("D3");
        }
    }
}