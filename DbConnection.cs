using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Data.SqlClient;

namespace WebApplication1
{
    public class DbConnection
    {
        public const string connectionString = "Server=LEGION\\SQLEXPRESS;Database=UMS;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;";
        public static SqlConnection EstablishConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
