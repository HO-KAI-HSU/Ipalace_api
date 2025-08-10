using MySql.Data.MySqlClient;
using System;

namespace com.leslie.Core.ORM.MySql
{
    public class Database : IDisposable
    {
        public static string ConnectionString;

        public MySqlConnection Connection;

        public Database(bool open = true)
        {
            Connection = new MySqlConnection(ConnectionString);

            if (open)
            {
                Open();
            }
        }

        public Database(string connectionString, bool open = true)
        {
            Connection = new MySqlConnection(connectionString);

            if (open)
            {
                Open();
            }
        }

        public void Open()
        {
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
