using Microsoft.EntityFrameworkCore;
namespace QuanLyLinhKienMayTinh.Models


{

    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins => _ins ??= new DataProvider();

        private string _supabaseConnStr;
        private string _localConnStr;

        private DataProvider()
        {

            _supabaseConnStr = "Server=db.pmkwulshpbpugvphzvwk.supabase.co;Port=5432;Database=postgres;User Id=postgres;Password=Kienquoc@1704;";

            _localConnStr = "Data Source=(localdbb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public QL_LinhKien_PC_Context GetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<QL_LinhKien_PC_Context>();

            optionsBuilder.UseNpgsql(_supabaseConnStr);

            return new QL_LinhKien_PC_Context(optionsBuilder.Options);
        }

        public void ChangeToQuanLyConnection()
        {
            _localConnStr = "Data Source=(localdbb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public void ChangeToNhanVienConnection()
        {
            _localConnStr = "Data Source=(localdbb)\\MSSQLLocalDB;Initial Catalog=QL_LinhKien_PC_NET;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }
    }
}