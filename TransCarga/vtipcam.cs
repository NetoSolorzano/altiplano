using System;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

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
            // jalamos la tabla tipcamref
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
                string xnum = "";      // cnt de la fila
                string c = "";      // codigo internacional de moneda
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
                    micon.Parameters.AddWithValue("@fec", para3);
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
            // calculamos el valor cambiado
            tx_newVal.Text = (decimal.Parse(para1) * decimal.Parse(tx_tipcam.Text)).ToString();
            if (tx_newVal.Text.Trim() == "" || tx_newVal.Text.Trim().Substring(0,1) == "0")
            {
                MessageBox.Show("Falta información en tabla de tipos de cambio", "No se puede continuar");
                ReturnValue1 = "0";
                this.Close();
            }

        }
        private void vtipcam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }
        public string ReturnValue1 { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnValue1 = tx_newVal.Text;
            this.Close();
        }
    }
}
