using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
namespace QuanLyLinhKienMayTinh.Models

{

    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins => _ins ??= new DataProvider();

        private string _localConnStr;
        private string _supabaseConnStr;

        private bool _isUsingSupabase;
        private bool _hasTestedConnection;

        private DataProvider()
        {
            _localConnStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";

               _supabaseConnStr = "Server=db.pmkwulshpbpugvphzvwk.supabase.co;Port=5432;Database=postgres;User Id=postgres;Password=Kienquoc@1704;";

        }

        public QL_LinhKien_PC_Context GetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<QL_LinhKien_PC_Context>();

            optionsBuilder.UseNpgsql(_supabaseConnStr);

            return new QL_LinhKien_PC_Context(optionsBuilder.Options);
        }

        public void ChangeToQuanLyConnection()
        {
            _localConnStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public void ChangeToNhanVienConnection()
        {
            _localConnStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }
    }
}