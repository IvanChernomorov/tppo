using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Cassa
{
    public class DBConnection
    {
        private string connString = "Host=127.0.0.1;Username=postgres;Password=123;Database=tppo";
        public NpgsqlConnection con;
        public DBConnection()
        {
            con = new NpgsqlConnection(connString);
        }
        public bool matchAccount(string login, string password )
        {
            con.Open();
            string sql = "SELECT (password = crypt(@pass, password)) AS password_match FROM account WHERE login=@login";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("login", login);
            command.Parameters.AddWithValue("pass", password);

            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                con.Close();
                return false;
            }
            bool isMatched = reader.GetBoolean(0);
            con.Close();
            return isMatched;
        }

        public Cashier GetCashier(string login)
        {
            con.Open();
            string sql = "SELECT * from cashiers WHERE id = (SELECT cashier_id FROM account WHERE login=@login)";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("login", login);
            var reader = command.ExecuteReader();
            reader.Read();
            int id = reader.GetInt32(0);
            string fullname = $"{reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}";
            string birthdate = reader.GetValue(4).ToString();
            var phone = reader.GetInt64(5);
            con.Close();
            return new Cashier(id, fullname, birthdate, phone);
        }

        public List<Product> fetchProducts()
        {
            con.Open();
            List<Product> list = new List<Product>();
            string sql = "SELECT * from products";
            var command = new NpgsqlCommand(sql, con);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                decimal price = reader.GetDecimal(2);
                double remains = reader.GetDouble(3);
                string desc = reader.GetString(4);
                string restr = reader.IsDBNull(5) ? "-" : reader.GetString(5);
                Product pr = new Product(id, name, price, remains, desc, restr);
                list.Add(pr);
            }
            con.Close();
            return list;
        }

        public int NewCheque(int cashier_id)
        {
            con.Open();
            string sql = "INSERT INTO cheque (cashier_id, status, handle_date) VALUES (@param1, @param2, @param3) RETURNING *";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("@param1", cashier_id);
            command.Parameters.AddWithValue("@param2", "created");
            command.Parameters.AddWithValue("@param3", DateTime.Now);
            var reader = command.ExecuteReader();
            reader.Read();
            int ID = reader.GetInt32(0);
            con.Close();
            return ID;
        }

        public void RejectCheque(int ID, decimal total)
        {
            con.Open();
            string sql = "UPDATE cheque SET status = 'rejected', total = @param1, handle_date = @param2 WHERE ID = @ID";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("@param1", total);
            command.Parameters.AddWithValue("@param2", DateTime.Now);
            command.Parameters.AddWithValue("@ID", ID);
            command.ExecuteNonQuery();
            con.Close();
        }

        public void confirmCheque(int ID, decimal total, bool inCash) 
        {
            con.Open();
            string sql = "UPDATE cheque SET status = 'succesfull', total = @param1, handle_date = @param2, in_cash = @param3 WHERE ID = @ID";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("@param1", total);
            command.Parameters.AddWithValue("@param2", DateTime.Now);
            command.Parameters.AddWithValue("@param3", inCash);
            command.Parameters.AddWithValue("@ID", ID);
            command.ExecuteNonQuery();
            con.Close();
        }

        public void newOrder(int cheque_id, int product_id, double quantity, decimal subtotal)
        {
            con.Open();
            string sql = "INSERT INTO orders VALUES(@param1, @param2, @param3, @param4)";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("@param1", cheque_id);
            command.Parameters.AddWithValue("@param2", product_id);
            command.Parameters.AddWithValue("@param3", quantity);
            command.Parameters.AddWithValue("@param4", subtotal);
            command.ExecuteNonQuery();
            con.Close();
        }

        public void changeOrder(double quantity, decimal subtotal, int cheque_id, int product_id)
        {
            con.Open();
            string sql = "UPDATE orders SET quantity = @param1, subtotal = @param2 WHERE cheque_id = @param3 and product_id = @param4";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("@param1", quantity);
            command.Parameters.AddWithValue("@param2", subtotal);
            command.Parameters.AddWithValue("@param3", cheque_id);
            command.Parameters.AddWithValue("@param4", product_id);
            command.ExecuteNonQuery();
            con.Close();
        }

        public void deleteCheque(int cheque_id)
        {
            con.Open();
            string sql = "DELETE FROM cheque WHERE id = @param1";
            var command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("@param1", cheque_id);
            command.ExecuteNonQuery();
            con.Close();
        }

        public DataTable fetchCheques()
        {
            string sql = "SELECT * FROM cheque";
            DataTable dt = new DataTable();
            NpgsqlDataAdapter adap = new NpgsqlDataAdapter(sql, con);
            adap.Fill(dt);
            return dt;
        }

    }
}
