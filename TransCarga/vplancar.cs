using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;

namespace TransCarga
{
    public partial class vplancar : Form
    {
        public DataTable para1;        // valor a calcular cambio
        //public string para2 = "";       // codigo moneda a efectar el cambio
        //public string para3 = "";       // fecha del cambio
        //
        DataTable dt = new DataTable();
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        //
        libreria lib = new libreria();
        public vplancar(DataTable param1)
        {
            para1 = param1;             // datatable origen
            InitializeComponent();
        }
        private void vplancar_Load(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.DataSource = para1;
            // a.id,a.fechope,a.serplacar,a.numplacar,a.platracto,a.placarret,a.autorizac,a.confvehic,a.brevchofe,a.nomchofe,a.brevayuda," +
            // a.nomayuda,a.rucpropie,b.razonsocial,c.marca,c.modelo
            dataGridView1.Columns[0].Visible = false;   // a.id,
            dataGridView1.Columns[1].Visible = true;   // a.fechope,
            dataGridView1.Columns[1].Width = 70;
            dataGridView1.Columns[2].Visible = true;   // a.serplacar,
            dataGridView1.Columns[2].Width = 40;
            dataGridView1.Columns[3].Visible = true;   // a.numplacar,
            dataGridView1.Columns[3].Width = 60;
            dataGridView1.Columns[4].Visible = true;   // a.platracto,
            dataGridView1.Columns[4].Width = 60;
            dataGridView1.Columns[5].Visible = true;   // a.placarret,
            dataGridView1.Columns[5].Width = 60;
            dataGridView1.Columns[6].Visible = false;   // a.autorizac
            dataGridView1.Columns[7].Visible = false;   // a.confvehic,
            dataGridView1.Columns[8].Visible = false;   // a.brevchofe,
            dataGridView1.Columns[9].Visible = false;   // a.nomchofe,
            dataGridView1.Columns[10].Visible = false;   // a.brevayuda,
            dataGridView1.Columns[11].Visible = false;   // a.nomayuda,
            dataGridView1.Columns[12].Visible = false;   // a.rucpropie,
            dataGridView1.Columns[13].Visible = false;   // b.razonsocial,
            dataGridView1.Columns[14].Visible = false;   // c.marca,
            dataGridView1.Columns[15].Visible = false;   // modelo
        }
        private void vplancar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }   
        public int ReturnValue1 { get; set; }        // valor cambiado a la moneda deseada
        //public string ReturnValue2 { get; set; }        // valor en moneda local
        //public string ReturnValue3 { get; set; }        // tipo de cambio de la operacion

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnValue1 = dataGridView1.CurrentRow.Index;
            //
            this.Close();
        }
    }
}
