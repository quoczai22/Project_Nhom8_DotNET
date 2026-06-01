using Microsoft.EntityFrameworkCore;

namespace QuanLyLinhKienMayTinh.Models
{
    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins => _ins ??= new DataProvider();

        private string _localConnStr;

        private const string ServerName = "localhost"; // đổi thành tên server của máy trước khi chạy
        private const string DatabaseName = "QL_LinhKien_PC_NET";

        private DataProvider()
        {
            ResetToDefaultConnection();
        }

        public void ResetToDefaultConnection()
        {
            _localConnStr = $"Data Source={ServerName};Initial Catalog={DatabaseName};Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public QL_LinhKien_PC_Context GetContext()
        {
            var options = new DbContextOptionsBuilder<QL_LinhKien_PC_Context>()
                .UseSqlServer(_localConnStr)
                .Options;

            return new QL_LinhKien_PC_Context(options);
        }

        public void ChangeConnectionByRole(string quyen)
        {
            string dbUser;
            string dbPass = "123";

            switch (quyen)
            {
                case "Quản lý toàn bộ":
                    dbUser = "quanLyLogin";
                    break;

                case "Thu ngân":
                    dbUser = "nhanVienThuNganLogin";
                    break;

                case "Chăm sóc khách hàng":
                    dbUser = "nhanVienCskhLogin";
                    break;

                case "Kho":
                    dbUser = "nhanVienKhoLogin";
                    break;

                default:
                    dbUser = "nhanVienCskhLogin";
                    break;
            }

            _localConnStr = $"Data Source={ServerName};Initial Catalog={DatabaseName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;Encrypt=False";
        }
    }
}