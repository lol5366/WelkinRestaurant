using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace WelkinRestaurant.DataAccess
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=localhost;Database=welkin_restaurant;Uid=root;Pwd=Qvit916));";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Для SELECT запросов (возвращает таблицу)
        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Для INSERT/UPDATE/DELETE (возвращает количество затронутых строк)
        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Для запросов, возвращающих одно значение
        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}