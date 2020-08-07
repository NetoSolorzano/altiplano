using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransCarga
{
    public partial class facelec : Form
    {
        public facelec()
        {
            InitializeComponent();
        }

        private void facelec_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            MinimizeBox = false;
            Text = "Facturación";
            
        }

        private void facelec_Deactivate(object sender, EventArgs e)
        {

        }

        private void facelec_Activated(object sender, EventArgs e)
        {
            //main padre = new main();
            //padre.bt_nuevo.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void facelec_Enter(object sender, EventArgs e)
        {
        }

        private void facelec_Click(object sender, EventArgs e)
        {
            
        }
    }
}