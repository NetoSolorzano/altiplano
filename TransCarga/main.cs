using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransCarga
{
    public partial class main : Form
    {
        // conexion a la base de datos
        static string serv = "solorsoft.com";
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        static string usua = "solorsof_rei";
        static string cont = "190969Sorol";
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        //static string ctl = ConfigurationManager.AppSettings["ConnectionLifeTime"].ToString();
        string DB_CONN_STR = "server=" + serv + ";uid=" + usua + ";pwd=" + cont + ";database=" + data + ";";
        libreria lib = new libreria();

        public main()
        {
            InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "- Versión " + System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                const string mensaje = "Desea salir del sistema?";
                const string titulo = "Confirme por favor";
                var result = MessageBox.Show(mensaje, titulo,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) Application.Exit(); // Environment.Exit(0);
                else e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
