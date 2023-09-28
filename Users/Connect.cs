using MySqlConnector;

namespace Users
{
    public class Connect
    {
        public MySqlConnection Connection;

        public Connect(string dbName, string host, string password, string username)
        {
            Connection = new MySqlConnection(new MySqlConnectionStringBuilder
            {
                Server = host,
                UserID = username,
                Password = password,
                Database = dbName,
                SslMode = MySqlSslMode.None
            }.ConnectionString);
        }
    }
}
