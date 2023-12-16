using System;

using System.Windows.Forms;

namespace Cassa
{
    public partial class Form3 : Form
    {
        private DBConnection conn;
        private int chequeId;
        private decimal sum;
        private Form2 form;

        public Form3(string sum, DBConnection conn, int chequeId, Form2 form)
        {
            this.conn = conn;
            this.chequeId = chequeId;
            this.sum = Decimal.Parse(sum);
            this.form = form;
            InitializeComponent();
            label1.Text = sum;   
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = false;
            textBox1.Visible = false;
            label4.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = true;
            textBox1.Visible = true;
            label4.Visible = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(Decimal.TryParse(textBox1.Text, out decimal insertSum))
            {
                if (insertSum > sum)
                {
                    label4.Text = "Сдача: " + (insertSum - sum);
                }
                else
                {
                    label4.Text = "";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(radioButton2.Checked || 
                radioButton1.Checked && Decimal.TryParse(textBox1.Text, out decimal insertSum) 
                && insertSum > sum)
            {
                conn.confirmCheque(chequeId, sum, radioButton1.Checked);
                form.form3Closed();
                this.Close();
            }
        }
    }
}
