using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuanLyLinhKienMayTinh.Models
{
    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new DataProvider();
                return _ins;
            }
            set
            {
                _ins = value;
            }
        }

        public QL_LinhKien_PC_Context DB { get; set; }

        private DataProvider()
        {
           var options= new DbContextOptionsBuilder<QL_LinhKien_PC_Context>()
                .UseSqlServer("Data Source=localhost;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True",
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(maxRetryCount:5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null))
                .Options;

            DB = new QL_LinhKien_PC_Context(options);
        }
        public void ChangeToQuanLyConnection()
        {
            string connStr = "Data Source=localhost;Initial Catalog=QL_LinhKien_PC_NET;User Id=QuanLyLogin;Password=123;TrustServerCertificate=True;";
            var options = new DbContextOptionsBuilder<QL_LinhKien_PC_Context>()
                 .UseSqlServer(connStr, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null))
                 .Options;

            DB = new QL_LinhKien_PC_Context(options);
        }

        public void ChangeToNhanVienConnection()
        {
            string connStr = "Data Source=localhost;Initial Catalog=QL_LinhKien_PC_NET;User Id=NhanVienBanHangLogin;Password=123;TrustServerCertificate=True;";
            var options = new DbContextOptionsBuilder<QL_LinhKien_PC_Context>()
                 .UseSqlServer(connStr, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null))
                 .Options;

            DB = new QL_LinhKien_PC_Context(options);
        }
    }
}