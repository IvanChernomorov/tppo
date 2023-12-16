using Npgsql;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Cassa
{
    public partial class Form1 : Form
    {
        private DBConnection conn;
        public Form1()
        {
            conn = new DBConnection();
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Введите данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(!conn.matchAccount(textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form2 f2 = new Form2(conn.GetCashier(textBox1.Text), this, conn);
            this.Hide();
            f2.Show();

        }
    }
}
