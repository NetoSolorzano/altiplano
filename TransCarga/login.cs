using System;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace TransCarga
{
    public partial class login : Form
    {
        libreria lib = new libreria();
        // conexion a la base de datos
        public static string serv = Decrypt(ConfigurationManager.AppSettings["serv"].ToString(), true);     // "solorsoft.com";
        public static string port = ConfigurationManager.AppSettings["port"].ToString();
        public static string usua = ConfigurationManager.AppSettings["user"].ToString();                    // "solorsof_rei";
        public static string cont = Decrypt(ConfigurationManager.AppSettings["pass"].ToString(), true);     // "190969Sorol";
        public static string data = ConfigurationManager.AppSettings["data"].ToString();
        public static string ctl = ConfigurationManager.AppSettings["ConnectionLifeTime"].ToString();
        string DB_CONN_STR = "server=" + serv + ";uid=" + usua + ";pwd=" + cont + ";database=" + data + ";";
        public DataTable dt_enlaces = new DataTable();
        public static string CadenaConexion = "Data Source=TransCarga.db";

        public login()
        {
            InitializeComponent();
        }
        private void login_Load(object sender, EventArgs e)
        {
            lb_version.Text = "Versión " + System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
            lb_titulo.Text = "Solución para empresas de Transporte de Carga" + Environment.NewLine + Program.tituloF;
            lb_titulo.BackColor = System.Drawing.Color.White;
            //lb_titulo.Parent = pictureBox1;
            Image logo = Image.FromFile("recursos/logo_solorsoft_2p.png");
            Image salir = Image.FromFile("recursos/Close_32.png");
            //Image entrar = Image.FromFile("recursos/ok.png");
            //pictureBox1.Image = logo;
            Button2.Image = salir;
            Button2.ImageAlign = ContentAlignment.MiddleCenter;
            //Button1.Image = entrar;
            barra.Visible = false;
            init();
            // jala datos de configuracion
            jaladatos();
            //jaladatper();
            //Tx_user.Focus();
        }
        private void init()
        {
            checkBox1.Visible = false;
            tx_newcon.Visible = false;
            tx_newcon.MaxLength = 10;
            //
            this.BackColor = System.Drawing.ColorTranslator.FromHtml(Program.colbac);
            //
            Tx_user.ReadOnly = true;
            Tx_pwd.ReadOnly = true;
            barra.Visible = true;
            backgroundWorker1.RunWorkerAsync();     // 08/03/2023
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            // validamos los campos
            string usuari = Tx_user.Text.Trim();     // usuario
            if (usuari == "" || usuari == "USUARIO")
            {
                MessageBox.Show("Por favor, ingrese el nombre de usuario", "Atención");
                Tx_user.Focus();
                return;
            }
            if (Tx_pwd.Text.Trim() == "" || Tx_pwd.Text == "CLAVE")
            {
                MessageBox.Show("Por favor, ingrese la contraseña", "Atención");
                Tx_pwd.Focus();
                return;
            }
            if (Tx_user.Text != "USUARIO" && Tx_pwd.Text != "CLAVE")
            {
                string contra = lib.md5(Tx_pwd.Text);
                MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
                if (lib.procConn(cn) == true)
                {
                    //validamos que el usuario y passw son los correctos
                    string query = "select a.bloqueado,a.local,a.nombre,concat(trim(b.deta1),' - ',b.deta2,' - ',b.deta3,' - ',b.deta4) AS direcc,b.ubiDir," +
                        "b.descrizione,a.tipuser,a.nivel,b.codsunat,ifnull(c.fechope,'') " +
                        "from usuarios a " +
                        "LEFT JOIN desc_loc b ON b.idcodice=a.local " +
                        "left join cabccaja c on c.loccaja=a.local and c.fechcie is null " +
                        "where a.nom_user=@usuario and a.pwd_user=@contra";
                    MySqlCommand mycomand = new MySqlCommand(query, cn);
                    mycomand.Parameters.AddWithValue("@usuario", Tx_user.Text);
                    mycomand.Parameters.AddWithValue("@contra", contra);
                    MySqlDataReader dr = mycomand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            if (dr.GetString(0) == "0")
                            {
                                TransCarga.Program.vg_user = Tx_user.Text;
                                TransCarga.Program.vg_nuse = dr.GetString(2);
                                TransCarga.Program.almuser = dr.GetString(1);
                                TransCarga.Program.vg_uuse = dr.GetString(4);
                                TransCarga.Program.vg_duse = dr.GetString(3);
                                TransCarga.Program.vg_luse = dr.GetString(1);
                                TransCarga.Program.vg_nlus = dr.GetString(5);
                                TransCarga.Program.vg_tius = dr.GetString(6);       // codigo de tipo de usuario
                                TransCarga.Program.vg_nius = dr.GetString(7);       // codigo nivel de usuario
                                TransCarga.Program.codlocsunat = dr.GetString(8);   // codigo sunat pto. emision DV
                                TransCarga.Program.vg_fcaj = dr.GetString(9);       // fecha de la caja abierta del local
                                dr.Close();
                                // cambiamos la contraseña si fue hecha
                                cambiacont();
                                // jala la ip wan del cliente
                                try
                                {
                                    TransCarga.Program.vg_ipwan = lib.ipwan();
                                }
                                catch
                                {
                                    TransCarga.Program.vg_ipwan = "";
                                }
                                // nos vamos al form principal
                                Program.vg_user = this.Tx_user.Text;
                                //main Main = new main();
                                TransMain Main = new TransMain();
                                Main.Show();
                                this.Hide();
                            }
                            else
                            {
                                dr.Close();
                                MessageBox.Show("El usuario esta Bloqueado!");
                                return;
                            }
                        }
                        dr.Close();
                    }
                    else
                    {
                        dr.Close();
                        MessageBox.Show("Usuario y/o Contraseña erronea", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    mycomand.Dispose();
                }
                cn.Close();
            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            const string mensaje = "Deseas salir del sistema?";
            const string titulo = "Confirma por favor";
            var result = MessageBox.Show(mensaje, titulo,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            { Close(); }
        }
        private static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            //string key = (string)settingsReader.GetValue("pass",typeof(String));   // SecurityKey
            string key = "8312@Sorol";
            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider
                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        private void Tx_user_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Tx_pwd.Focus();
            }
        }
        private void Tx_user_Enter(object sender, EventArgs e)
        {
            if (Tx_user.Text == "USUARIO")
            {
                Tx_user.Text = "";
                Tx_user.ForeColor = Color.Black;
            }
        }
        private void Tx_user_Leave(object sender, EventArgs e)
        {
            if (Tx_user.Text.Trim() == "")
            {
                Tx_user.Text = "USUARIO";
                Tx_user.ForeColor = Color.Gray;
            }
        }

        private void Tx_pwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Button1.PerformClick();
            }
        }
        private void Tx_pwd_TextChanged(object sender, EventArgs e)
        {
            if (panel1.Visible == true)
            {
                if (Tx_pwd.Text.Trim() != "" && Tx_pwd.Text.Trim() != "CLAVE")
                {
                    checkBox1.Visible = true;
                    checkBox1.Checked = false;
                    tx_newcon.Visible = false;
                }
                else
                {
                    checkBox1.Visible = false;
                    checkBox1.Checked = false;
                    tx_newcon.Visible = false;
                }
            }
        }
        private void Tx_pwd_Enter(object sender, EventArgs e)
        {
            if (Tx_pwd.Text == "CLAVE")
            {
                Tx_pwd.Text = "";
                Tx_pwd.ForeColor = Color.Black;
                Tx_pwd.UseSystemPasswordChar = true;
            }
        }
        private void Tx_pwd_Leave(object sender, EventArgs e)
        {
            if (Tx_pwd.Text.Trim() == "")
            {
                Tx_pwd.Text = "CLAVE";
                Tx_pwd.ForeColor = Color.Gray;
                Tx_pwd.UseSystemPasswordChar = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                tx_newcon.Visible = true;
                tx_newcon.Focus();
            }
            else
            {
                tx_newcon.Text = "";
                tx_newcon.Visible = false;
            }
        }
        private void cambiacont()
        {
            if (checkBox1.Checked == true && tx_newcon.Text != "")
            {
                MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
                if (lib.procConn(cn) == true)
                {
                    string consulta = "update usuarios set pwd_user=@npa where nom_user=@nus";
                    MySqlCommand micon = new MySqlCommand(consulta, cn);
                    micon.Parameters.AddWithValue("@npa", lib.md5(tx_newcon.Text));
                    micon.Parameters.AddWithValue("@nus", Tx_user.Text);
                    try
                    {
                        micon.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en actualización del password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    micon.Dispose();
                }
                cn.Close();
            }
        }
        private void jaladatos()
        {
            MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
            if (lib.procConn(cn) == true)
            {
                string consulta = "SELECT a.param,a.value,a.used,b.cliente,b.ruc,b.igv,b.direcc,b.distrit,b.provin,b.depart,b.ubigeo," +
                    "b.ctadetra,b.valdetra,b.detra,b.coddetra,b.email,b.telef1,b.referen2 " +
                    "from confmod a INNER JOIN baseconf b";
                MySqlCommand micon = new MySqlCommand(consulta, cn);
                MySqlDataReader dr = micon.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        // usa conector solorsoft para ruc y dni?
                        if (dr.GetString(0) == "conSolorsoft")
                        {
                            if (dr.GetString(1) == "1") TransCarga.Program.vg_conSol = true;
                            else TransCarga.Program.vg_conSol = false;
                        }
                        // usuario puede cambiar su contraseña?
                        if (dr.GetString(0) == "chpwd")
                        {
                            if (dr.GetString(1) == "1") panel1.Visible = true;
                            else panel1.Visible = false;
                        }
                        // obtenemos la configuración de los colores
                        if (dr.GetString(0).StartsWith("color") == true)
                        {
                            if (dr.GetString(0).ToString() == "colorback") Program.colbac = dr.GetString(1).ToString();
                            if (dr.GetString(0).ToString() == "colorpgfr") Program.colpag = dr.GetString(1).ToString();
                            if (dr.GetString(0).ToString() == "colorgrid") Program.colgri = dr.GetString(1).ToString();
                            if (dr.GetString(0).ToString() == "colorstrip") Program.colstr = dr.GetString(1).ToString();
                        }
                        Program.cliente = dr.GetString(3);
                        TransCarga.Program.ruc = dr.GetString(4);
                        TransCarga.Program.cliente = dr.GetString(3);
                        TransCarga.Program.dirfisc = dr.GetString(6).Trim() + " - " + dr.GetString(7).Trim() + " - " + dr.GetString(8).Trim() + " - " + dr.GetString(9).Trim();      // direccion fiscal del cliente
                        TransCarga.Program.ubidirfis = dr.GetString(10);    // ubigeo dir fiscal
                        TransCarga.Program.distfis = dr.GetString(7).Trim();
                        TransCarga.Program.provfis = dr.GetString(8).Trim();
                        TransCarga.Program.depfisc = dr.GetString(9).Trim();
                        TransCarga.Program.ctadetra = dr.GetString(11);         // cuenta de detraccion
                        TransCarga.Program.valdetra = dr.GetString(12);         // valor flete desde donde origina la detraccion
                        TransCarga.Program.pordetra = dr.GetString(13);         // valor en % de la detraccion
                        TransCarga.Program.coddetra = dr.GetString(14);         // codigo detraccion sunat
                        TransCarga.Program.mailclte = dr.GetString(15);         // correo electronico emisor
                        TransCarga.Program.telclte1 = dr.GetString(16);         // telefono emisor
                        TransCarga.Program.regmtc = dr.GetString(17);           // Num. Registro del MTC
                    }
                    dr.Close();
                    micon.Dispose();
                }
            }
            cn.Close();
        }
        private void login_KeyDown(object sender, KeyEventArgs e)
        {
            //ReleaseCapture();
            //SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true) tx_newcon.Visible = true;
            else tx_newcon.Visible = false;
        }
        private void jaladatper()
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand mico = new MySqlCommand("select id,formulario,campo,descrip,valor,param from enlaces", conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(mico))
                    {
                        da.Fill(dt_enlaces);
                        using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
                        {
                            cnx.Open();
                            string sqlborra = "DROP TABLE IF EXISTS dt_enlaces; DROP TABLE IF EXISTS sunat_webservices";
                            using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                            {
                                cmdB.ExecuteNonQuery();
                            }
                            string sqlTabla = "create table dt_enlaces (id integer primary key autoincrement, formulario varchar(20), campo varchar(20), descrip varchar(150), valor varchar(100), param varchar(50))";
                            using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            foreach (DataRow row in dt_enlaces.Rows)
                            {
                                string metela = "insert into dt_enlaces (formulario, campo, descrip, valor, param) " +
                                    "values ('" + row.ItemArray[1].ToString() + "','" + row.ItemArray[2].ToString() + "','" +
                                    row.ItemArray[3].ToString() + "','" + row.ItemArray[4].ToString() + "','" + row.ItemArray[5].ToString() + "')";
                                using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                /*
                                configuracion.dt_enlacesRow nr = setC.dt_enlaces.Newdt_enlacesRow();
                                nr.id = int.Parse(row.ItemArray[0].ToString());
                                nr.formulario = row.ItemArray[1].ToString();
                                nr.campo = row.ItemArray[2].ToString();
                                nr.descrip = row.ItemArray[3].ToString();
                                nr.valor = row.ItemArray[4].ToString();
                                nr.param = row.ItemArray[5].ToString();
                                setC.dt_enlaces.Adddt_enlacesRow(nr);
                                */
                            }
                        }
                    }
                }
            }
        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int i = 10;
            backgroundWorker1.ReportProgress(i);
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand mico = new MySqlCommand("select id,formulario,campo,descrip,valor,param from enlaces", conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(mico))
                    {
                        da.Fill(dt_enlaces);
                        i = i + 40;
                        backgroundWorker1.ReportProgress(i);

                        using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
                        {
                            cnx.Open();
                            string sqlborra = "DROP TABLE IF EXISTS dt_enlaces"; // ; DROP TABLE IF EXISTS sunat_webservices
                            using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                            {
                                cmdB.ExecuteNonQuery();
                            }
                            string sqlTabla = "create table dt_enlaces (id integer primary key autoincrement, formulario varchar(20), campo varchar(20), descrip varchar(150), valor varchar(100), param varchar(50))";
                            using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            i = i + 49;
                            backgroundWorker1.ReportProgress(i);

                            foreach (DataRow row in dt_enlaces.Rows)
                            {
                                string metela = "insert into dt_enlaces (formulario, campo, descrip, valor, param) " +
                                    "values ('" + row.ItemArray[1].ToString() + "','" + row.ItemArray[2].ToString() + "','" +
                                    row.ItemArray[3].ToString() + "','" + row.ItemArray[4].ToString() + "','" + row.ItemArray[5].ToString() + "')";
                                using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                /*
                                configuracion.dt_enlacesRow nr = setC.dt_enlaces.Newdt_enlacesRow();
                                nr.id = int.Parse(row.ItemArray[0].ToString());
                                nr.formulario = row.ItemArray[1].ToString();
                                nr.campo = row.ItemArray[2].ToString();
                                nr.descrip = row.ItemArray[3].ToString();
                                nr.valor = row.ItemArray[4].ToString();
                                nr.param = row.ItemArray[5].ToString();
                                setC.dt_enlaces.Adddt_enlacesRow(nr);
                                */
                            }
                            sqlTabla = "create table IF NOT EXISTS sunat_webservices (id integer primary key autoincrement, sunat_plazoT integer default 0, sunat_horaT varchar(8) default '', sunat_TokenAct varchar(500) default '')";
                            using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            int k = 0;
                            using (SqliteCommand cmd = new SqliteCommand("select count(id) from sunat_webservices", cnx))
                            {
                                using (SqliteDataReader dr = cmd.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        k = dr.GetInt16(0);
                                    }
                                }
                            }
                            if (k == 0)
                            {
                                string insta = "insert into sunat_webservices (sunat_plazoT,sunat_horaT,sunat_TokenAct) values (0,'','') where ";
                                using (SqliteCommand cmd = new SqliteCommand(insta, cnx))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            i = i + 1;
                            backgroundWorker1.ReportProgress(i);
                        }
                    }
                }
            }
            
        }
        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //ProgressBar bar = new ProgressBar();
            //bar.Value = e.ProgressPercentage;
            progressBar1.Value = e.ProgressPercentage;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            barra.Visible = false;
            MessageBox.Show("Carga de datos terminada","Atención");
            Tx_user.ReadOnly = false;
            Tx_pwd.ReadOnly = false;
            Tx_user.Focus();
        }
    }
}
