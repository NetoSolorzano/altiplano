using System;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;

namespace TransCarga
{
    public partial class vtipcam : Form
    {
        public string para1 = "";       // valor a calcular cambio
        public string para2 = "";       // codigo moneda a efectar el cambio
        public string para3 = "";       // fecha del cambio
        //
        DataTable dt = new DataTable();
        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";
        //
        libreria lib = new libreria();
        public vtipcam(string param1, string param2, string param3)
        {
            para1 = param1;             // valor original en moneda local se supone
            para2 = param2;             // codigo moneda deseada a cambiar
            para3 = param3;             // fecha del cambio
            InitializeComponent();
        }
        private void vtipcam_Load(object sender, EventArgs e)
        {
            Image salir = Image.FromFile("recursos/Close_32.png");
            button3.Image = salir;
            button3.ImageAlign = ContentAlignment.MiddleCenter;
            // jalamos la tabla tipcamref
            string xnum = "";      // cnt de la fila
            string c = "";      // codigo internacional de moneda
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                lib.procConn(conn);
                //
                string conmod = "select idcodice,codigo,cnt from desc_mon order by cnt";
                using (MySqlCommand micon = new MySqlCommand(conmod, conn))
                {
                    using (MySqlDataAdapter dr = new MySqlDataAdapter(micon))
                    {
                        dr.Fill(dt);
                    }
                }
                DataRow[] row = dt.Select("idcodice='" + para2 + "'");
                if (row != null)
                {
                    xnum = row[0].ItemArray[2].ToString();
                    c = row[0].ItemArray[1].ToString();
                }
                //
                string consulta = "select fechope,mext1,mext2,mext3,mext4 from tipcamref where fechope=@fec";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@fec", para3.Substring(6,4) + "-" + para3.Substring(3,2) + "-" + para3.Substring(0,2));
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            if (dr.Read())
                            {
                                tx_fecha.Text = para3.Substring(0,10);
                                tx_codmon.Text = c;                     //para2
                                if (xnum == "1") tx_tipcam.Text = dr.GetString(1);  // ALGO ANDA MAL POR ACA 04/11/2020 no funca
                                if (xnum == "2") tx_tipcam.Text = dr.GetString(2);
                                if (xnum == "3") tx_tipcam.Text = dr.GetString(3);
                                if (xnum == "4") tx_tipcam.Text = dr.GetString(4);
                            }
                        }
                    }
                }
            }
            if (tx_tipcam.Text.Trim() == "" || xnum == "" || c == "")
            {
                MessageBox.Show("Falta información en tabla de tipos de cambio" + Environment.NewLine + 
                    "o falta configurar tabla de monedas", "No se puede continuar",MessageBoxButtons.OK,MessageBoxIcon.Error);
                ReturnValue1 = "0";
                this.Close();
            }
            // calculamos el valor cambiado
            if (para1 == "" || para1 == "0")
            {
                tx_newVal.ReadOnly = false;
                tx_newVal.Focus();
            }
            else
            {
                tx_newVal.ReadOnly = true;
                tx_newVal.Text = Math.Round(decimal.Parse(para1) / decimal.Parse(tx_tipcam.Text), 3).ToString(); ;
            }
        }
        private void vtipcam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }   
        public string ReturnValue1 { get; set; }        // valor cambiado a la moneda deseada
        public string ReturnValue2 { get; set; }        // valor en moneda local
        public string ReturnValue3 { get; set; }        // tipo de cambio de la operacion

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnValue1 = tx_newVal.Text;                                                          // valor cambiado a la moneda deseada
            if (para1 == "" || para1 == "0")
            {
                ReturnValue2 = Math.Round(decimal.Parse(tx_newVal.Text) * decimal.Parse(tx_tipcam.Text),3).ToString();      // valor en moneda local
            }
            else ReturnValue2 = para1;
            ReturnValue3 = tx_tipcam.Text;                                                          // tipo de cambio de la operacion
            //
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ReturnValue1 = para1;
            ReturnValue2 = para1;
            ReturnValue3 = "0";
            this.Close();
        }

        private void tx_tipcam_Leave(object sender, EventArgs e)
        {
            tx_newVal.Text = Math.Round(decimal.Parse(para1) / decimal.Parse(tx_tipcam.Text), 3).ToString(); ;
        }

        private void tx_newVal_Leave(object sender, EventArgs e)
        {
            // nada que hacer
        }

    }
}
