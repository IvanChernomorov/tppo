using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Cassa
{
    public partial class Form2 : Form
    {
        private Cashier cashier;
        private Form1 form1;
        private DBConnection conn;
        private List<Product> products;
        private int curIndex = 0;
        private int chequeId;
        public Form2(Cashier cashier,  Form1 form, DBConnection conn)
        {
            this.form1 = form;
            this.cashier = cashier;
            this.conn = conn;
            InitializeComponent();
            InitProductsList();
            label1.Text = cashier.Fullname;
            chequeId = conn.NewCheque(cashier.ID);
        }

        private void InitProductsList()
        {
            products = conn.fetchProducts();
            foreach (var product in products)
            {
                listBox1.Items.Add(product.Name);
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.deleteCheque(chequeId);
            form1.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex == -1 || numericUpDown1.Value <= 0)
            {
                return;
            }
            foreach (var product in products)
            {
                if (product.Name == listBox1.SelectedItem.ToString())
                {
                    double quantity = (double)numericUpDown1.Value;
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1[1, i].Value.ToString() == product.Name)
                        {
                            dataGridView1[3, i].Value = Convert.ToDouble(dataGridView1[3, i].Value) + quantity;
                            return;
                        }
                    }
                    decimal subtotal = (decimal)quantity * product.Price;

                    dataGridView1.Rows.Add(++curIndex, product.Name, product.Price, quantity, subtotal);
                    label4.Text = (Decimal.Parse(label4.Text) + subtotal).ToString();
                    dataGridView1.CurrentCell = null;

                    
                    conn.newOrder(chequeId, product.ID, quantity, subtotal);
                    return;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var product in products)
            {
                if (product.Name.ToLower().Contains(textBox1.Text.ToLower()))
                {
                    listBox1.Items.Add(product.Name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
                return;
            Form3 form3 = new Form3(label4.Text, conn, chequeId, this);
            form3.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            conn.NewCheque(cashier.ID);
        }


        private void clear()
        {
            dataGridView1.Rows.Clear();
            label4.Text = "0,00";
            numericUpDown1.Value = 0;
            textBox1.Text = "";
            listBox1.SelectedItem = null;
            curIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
                return;
            conn.RejectCheque(chequeId, Decimal.Parse(label4.Text));
            clear();
            MessageBox.Show("Заказ отменён", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            chequeId = conn.NewCheque(cashier.ID);
        }

        public void form3Closed()
        {
            clear();
            MessageBox.Show("Заказ подтверждён", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            chequeId = conn.NewCheque(cashier.ID);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Rows.Count == 0 || e.ColumnIndex != 3)
                return;
            string name = dataGridView1[1, e.RowIndex].Value.ToString();
            double quantity = Convert.ToDouble(dataGridView1[e.ColumnIndex, e.RowIndex].Value);

            foreach (var product in products)
            {
                if (product.Name == name)
                {
                    decimal subtotal = (decimal)quantity * product.Price;
                    decimal prevSubtotal = Convert.ToDecimal(dataGridView1[4, e.RowIndex].Value);
                    dataGridView1[4, e.RowIndex].Value = subtotal;
                    label4.Text = (Decimal.Parse(label4.Text) - prevSubtotal + subtotal).ToString();
                    conn.changeOrder(quantity, subtotal, chequeId, product.ID);
                    dataGridView1.CurrentCell = null;
                    break;
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 1)
            {
                DataTable dt = conn.fetchCheques();
                dataGridView2.DataSource = dt;
            } 
            else if(tabControl1.SelectedIndex == 2 && dataGridView3.RowCount == 0)
            { 
                foreach(var product in products)
                {
                    dataGridView3.Rows.Add(product.Name, product.Price, product.Description, product.Restriction);
                }
            }
            dataGridView2.CurrentCell = null;
        }
    }
}
