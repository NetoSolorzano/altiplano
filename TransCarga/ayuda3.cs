using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class ayuda3 : Form
    {
        static string nomform = "ayuda3"; // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        public string para1 = "";
        public string para2 = "";
        public string para3 = "";
        // Se crea un DataTable que almacenará los datos desde donde se cargaran los datos al DataGridView
        DataTable dtDatos = new DataTable();
        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";
        libreria lib = new libreria();

        public ayuda3(string param1, string param2, string param3)
        {
            para1 = param1; // identificador de tabla a mostrar
            para2 = param2; // filtro 1
            para3 = param3; // filtro 2
            InitializeComponent();
        }
        private void ayuda3_Load(object sender, EventArgs e)
        {
            loadgrids();    // datos del grid
            this.Text = this.Text;
        }
        private void ayuda3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }
        public string ReturnValue0 { get; set; }
        public string ReturnValue1 { get; set; }
        public string ReturnValue2 { get; set; }

        public void loadgrids()
        {
            // DATOS DE LA GRILLA
            string consulta = "";
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            if (lib.procConn(conn) != true)
            {
                MessageBox.Show("Error de comunicación con el servidor", "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            dtDatos.Clear();
            dataGridView1.DataSource = null;
            if (para1 == "Proveedores")
            {
                consulta = "SELECT a.ID,a.RazonSocial as NOMBRE,a.RUC,a.Direcc1 as DIRECCION,a.depart as DPTO,a.Provincia as PROVINCIA,a.Localidad as DISTRITO,a.ubigeo as UBIGEO " +
                    "from anag_for a WHERE estado=0";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        da.Fill(dtDatos);
                        dataGridView1.DataSource = dtDatos;
                        dataGridView1.ReadOnly = true;
                    }
                }
            }
            // formateo de la grilla
            //dataGridView1.Font = tiplg;
            //dataGridView1.DefaultCellStyle.Font = tiplg;
            dataGridView1.RowTemplate.Height = 16;
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colback);
            dataGridView1.AllowUserToAddRows = false;
            //dataGridView1.Width = Parent.Width - 10;
            if (dataGridView1.DataSource == null) dataGridView1.ColumnCount = 5;
            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _ = decimal.TryParse(dataGridView1.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                    if (vd != 0) dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                int b = 0;
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    int a = dataGridView1.Columns[i].Width;
                    b += a;
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dataGridView1.Columns[i].Width = a;
                }
                if (b < dataGridView1.Width) dataGridView1.Width = b + 60;
                dataGridView1.ReadOnly = true;
            }
            //
            conn.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnValue0 = tx_id.Text;         // id
            ReturnValue1 = tx_codigo.Text;     // codigo/serie
            ReturnValue2 = tx_nombre.Text;     // nombre/numero
            Close();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            tx_id.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            tx_nombre.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            string cellva = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            tx_codigo.Text = cellva;
            //
            TransCarga.Program.retorna1 = cellva;
            tx_codigo.Focus();
            //this.Close();
        }

        private void tx_codigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                ReturnValue0 = tx_id.Text;
                ReturnValue1 = tx_codigo.Text;
                ReturnValue2 = tx_nombre.Text;
                Close();
            }
        }

        private void tx_buscar_Leave(object sender, EventArgs e)
        {
            if (tx_buscar.Text != "")
            {
                //
            }
        }
    }
}
