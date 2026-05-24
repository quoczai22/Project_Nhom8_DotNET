using Microsoft.EntityFrameworkCore;
namespace QuanLyLinhKienMayTinh.Models


{

    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins => _ins ??= new DataProvider();

        private string _localConnStr;
        // phải đổi chỗ data source thành tên server của máy, nếu máy dùng sql server express thì sẽ là .\SQLEXPRESS
        private DataProvider()
        {
            _localConnStr = "Data Source=localhost;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public QL_LinhKien_PC_Context GetContext()
        {
            var options = new DbContextOptionsBuilder<QL_LinhKien_PC_Context>()
              .UseSqlServer(_localConnStr)
              .Options;
            return new QL_LinhKien_PC_Context(options);
        }

        public void ChangeToQuanLyConnection()
        {
            _localConnStr = "Data Source=localhost;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public void ChangeToNhanVienConnection()
        {
            _localConnStr = "Data Source=localhost;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }
    }
}