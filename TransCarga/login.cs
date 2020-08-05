using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text;
using System.Drawing;

namespace TransCarga
{
    public partial class login : Form
    {
        // conexion a la base de datos
        static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        static string usua = ConfigurationManager.AppSettings["user"].ToString();
        static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        //static string ctl = ConfigurationManager.AppSettings["ConnectionLifeTime"].ToString();
        string DB_CONN_STR = "server=" + serv + ";uid=" + usua + ";pwd=" + cont + ";database=" + data + ";";
        libreria lib = new libreria();

        public login()
        {
            InitializeComponent();
        }

        private void login_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "- Versión " + System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
            lb_titulo.Text = Program.tituloF;
            lb_titulo.BackColor = System.Drawing.Color.Transparent;
            lb_titulo.Parent = pictureBox1;
            Image logo = Image.FromFile("recursos/logo_artesanos_omg_peru.jpeg");
            Image salir = Image.FromFile("recursos/exit48.png");
            Image entrar = Image.FromFile("recursos/ok.png");
            pictureBox1.Image = logo;
            Button2.Image = salir;
            Button1.Image = entrar;
            init();
            Tx_user.Focus();
        }

        private void init()
        {
            checkBox1.Visible = false;
            tx_newcon.Visible = false;
            tx_newcon.MaxLength = 10;
        }

        private string desencrip(string entrada)
        {
            string retorno="";
            string xAcu="";
            for(int c=0;c<entrada.Trim().Length;c++)  
            {
                int ca = Encoding.ASCII.GetBytes(entrada.Substring(c, 1))[0] - 41;
                xAcu = xAcu.Trim() + (char)ca;
            }
            retorno = xAcu;
            MessageBox.Show(retorno);
            return retorno;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            // validamos los campos
            string usuari = this.Tx_user.Text;     // usuario
            //string contra = this.Tx_pwd.Text;      // passw
            string contra = lib.md5(Tx_pwd.Text);
            if (usuari == "")
            {
                MessageBox.Show("Por favor, ingrese el usuario", "Atención");
                return;
            }
            if (contra == "")
            {
                MessageBox.Show("Por favor, ingrese la contraseña", "Atención");
                return;
            }
            try
            {
                MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
                cn.Open();
                //validamos que el usuario y passw son los correctos
                string query = "select a.bloqueado,a.local,trim(a.mod1),trim(a.mod2),trim(a.mod3),a.nombre," +
                    "a.ruc,ifnull(b.descrizione,'- SIN ASIGNAR -') " +
                    "from usuarios a " +
                    "left join desc_raz b on b.idcodice=a.ruc " +
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
                            TransCarga.Program.vg_nuse = dr.GetString(5);
                            TransCarga.Program.almuser = dr.GetString(1);
                            TransCarga.Program.ruc = dr.GetString(6);
                            TransCarga.Program.cliente = dr.GetString(7);
                            if (dr.GetString(2) == "M0" || dr.GetString(3) == "M0" || dr.GetString(4) == "M0")
                            {
                                Program.m70 = "M0";
                            }
                            else
                            {
                                if (dr.GetString(2) == "M70" || dr.GetString(3) == "M70" || dr.GetString(4) == "M70")
                                {
                                    Program.m70 = "M70";
                                }
                            }
                            dr.Close();
                            // cambiamos la contraseña si fue hecha
                            cambiacont();
                            // jala datos de configuracion
                            jaladatos();
                            // nos vamos al form principal
                            Program.vg_user = this.Tx_user.Text;
                            main Main = new main();
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
                }
                else
                {
                    dr.Close();
                    MessageBox.Show("Usuario y/o Contraseña erronea", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                cn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "No se tiene conexión con el servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
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

        private void Tx_user_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Tx_pwd.Focus();
            }
        }

        private void Tx_pwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Button1.PerformClick();
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

        private void Tx_pwd_TextChanged(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                if (Tx_pwd.Text != "")
                {
                    checkBox1.Visible = true;
                    checkBox1.Checked = false;
                }
                else
                {
                    checkBox1.Visible = false;
                    checkBox1.Checked = false;
                }
            }
        }

        private void cambiacont()
        {
            if (checkBox1.Checked == true && tx_newcon.Text != "")
            {
                MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
                cn.Open();
                try
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
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en conexión");
                    Application.Exit();
                    return;
                }
                cn.Close();
            }
        }
        private void jaladatos()
        {
            MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
            cn.Open();
            try
            {
                string consulta = "select param,value,used from confmod";
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
                            if (dr.GetString(1) == "1") this.panel1.Visible = true;
                            else this.panel1.Visible = false;
                        }
                        // obtenemos la configuración de los colores
                        if (dr.GetString(0).StartsWith("color") == true)
                        {
                            if (dr.GetString(0).ToString() == "colorback") Program.colbac = dr.GetString(1).ToString();
                            if (dr.GetString(0).ToString() == "colorpgfr") Program.colpag = dr.GetString(1).ToString();
                            if (dr.GetString(0).ToString() == "colorgrid") Program.colgri = dr.GetString(1).ToString();
                            if (dr.GetString(0).ToString() == "colorstrip") Program.colstr = dr.GetString(1).ToString();
                        }
                    }
                }
                // jala datos de cliente y logo
                jalaclie();         
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error en conexión");
                Application.Exit();
                return;
            }
            cn.Close();
        }
        private void jalaclie()
        {
            MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
            cn.Open();
            try
            {
                string consulta = "select cliente,igv from baseconf limit 1";
                MySqlCommand micon = new MySqlCommand(consulta, cn);
                try
                {
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        Program.cliente = dr.GetString(0);
                    }
                    dr.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    Application.Exit();
                    return;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error en conexión");
                Application.Exit();
                return;
            }
            cn.Close();
        }

        private void titulo2_Click(object sender, EventArgs e)
        {

        }
    }
}
