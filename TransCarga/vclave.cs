using System;
using System.Windows.Forms;

namespace TransCarga
{
    public partial class vclave : Form
    {
        public string para1 = "";       // clave ingresada

        public vclave(string param1)
        {
            para1 = param1;             // clave
            InitializeComponent();
        }
        private void vclave_Load(object sender, EventArgs e)
        {
            tx_clave.Text = para1;
            tx_clave.MaxLength = 4;
        }
        private void vclave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }
        public string ReturnValue1 { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tx_clave.Text.Trim().Length > 0 && tx_clave.Text.Trim().Length < 4)
            {
                tx_clave.Focus();
                return;
            }

            ReturnValue1 = tx_clave.Text;
            this.Close();
        }
    }
}
