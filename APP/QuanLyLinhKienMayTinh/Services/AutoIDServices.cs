using System.Linq;
using System.Text.RegularExpressions;
using QuanLyLinhKienMayTinh.Models;

namespace QuanLyLinhKienMayTinh.Services
{
    public static class AutoIDService
    {
        /// Tạo ID tiếp theo với prefix cố định.
        /// Ví dụ: GetNextID("KH", "KH010") → "KH011"
        public static string GetNextID(string prefix, string lastID)
        {
            if (string.IsNullOrEmpty(lastID)) return prefix + "001";
            // Lấy phần số ở cuối chuỗi (bỏ qua prefix có thể có độ dài khác)
            var match = Regex.Match(lastID, @"\d+$");
            if (!match.Success) return prefix + "001";
            int number = int.Parse(match.Value) + 1;
            int padLen = System.Math.Max(3, match.Value.Length);
            return prefix + number.ToString("D" + padLen);
        }

        /// Tạo ID tiếp theo cho LoaiLK (dạng 3 chữ cái: MOU, RAM...).
        /// Dựa vào danh sách hiện có, không sinh ID tự động theo số.
        /// Trả về prefix dạng số thứ tự: L001, L002...
        public static string GetNextLoaiID(string lastID)
        {
            if (string.IsNullOrEmpty(lastID)) return "L001";
            var match = Regex.Match(lastID, @"\d+$");
            if (!match.Success) return "L001";
            int number = int.Parse(match.Value) + 1;
            return "L" + number.ToString("D3");
        }

        public static string GetNextNhaSanXuatID(string lastID)
        {
            if (string.IsNullOrEmpty(lastID)) return "NSX01";
            var match = Regex.Match(lastID, @"\d+$");
            if (!match.Success) return "NSX01";
            int number = int.Parse(match.Value) + 1;
            return "NSX" + number.ToString("D2");
        }
    }
}
