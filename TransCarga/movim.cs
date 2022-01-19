using System;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class movim : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public bool retorno;
        string para1;
        string[] para2;
        libreria lib = new libreria();
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        // conexion a la base de datos
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public movim(string parm1, string[] parm2)    // parm1 = modo = reserva o salida
        {
            InitializeComponent();                              // parm3 = codigo del mueble
            lb_titulo.Text = parm1.ToUpper(); // modo del movimiento
            para1 = parm1;
            para2 = parm2;
            dataGridView1.Columns.Add("id", "ID");
            dataGridView1.Columns.Add("guia", "GUIA");
            dataGridView1.Columns.Add("cant", "CANT");
            dataGridView1.Columns.Add("almac", "ALMACEN");
            dataGridView1.Columns.Add("repart", "REPARTIDOR");
            dataGridView1.Columns.Add("frepar", "F_REPART");
            dataGridView1.Columns.Add("unidad", "UNIDAD");
            dataGridView1.Columns.Add("unidad", "DESTINATARIO");
            dataGridView1.Columns[0].Width = 40;    // id
            dataGridView1.Columns[1].Width = 80;    // guia
            dataGridView1.Columns[2].Width = 30;    // cantid
            dataGridView1.Columns[3].Width = 60;    // almacen
            dataGridView1.Columns[4].Width = 60;    // 
            dataGridView1.Columns[5].Width = 80;    // fecha reparto
            dataGridView1.Columns[6].Width = 60;    // 
            dataGridView1.Columns[7].Width = 120;    // nombre del destinario
            //
            if (parm1 == "reserva")
            {
                panel4.Visible = false;
            }
            if (parm1 == "salida")
            {
                panel4.Visible = true;
                panel4.Left = 0;
                panel4.Top = 30;
                rb_mov.Checked = true;
                combos();
            }
        }
        private void movim_Load(object sender, EventArgs e)
        {
            combos();
            dataGridView1.Rows.Add(para2[0], para2[1], para2[2], para2[3], para2[4], para2[5], para2[6], para2[7]);
            tx_evento.MaxLength = 100;
            tx_ndr.MaxLength = 15;
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void bt_close_Click(object sender, EventArgs e)
        {
            retorno = false;
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var aa = MessageBox.Show("Confirma que desea grabar la operación?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (aa == DialogResult.Yes)
            {
                if (lb_titulo.Text.ToLower() == "reserva")
                {
                    //
                }
                if (para1 == "salida")
                {
                    if (salida() == true)
                    {
                        retorno = true; // true = se efectuo la operacion
                    }
                }
                this.Close();
            }
        }
        //
        private bool salida()
        {
            bool bien = false;
            try
            {
                using (MySqlConnection cn = new MySqlConnection(DB_CONN_STR))
                {
                    if (lib.procConn(cn) == true)
                    {
                        string llama = "borraseguro";
                        using (MySqlCommand micon = new MySqlCommand(llama, cn))
                        {
                            micon.CommandType = CommandType.StoredProcedure;
                            micon.Parameters.AddWithValue("@tabla", "cabalmac");
                            micon.Parameters.AddWithValue("@vidr", dataGridView1.Rows[0].Cells[0].Value.ToString());
                            micon.Parameters.AddWithValue("@vidc", 0);
                            micon.ExecuteNonQuery();
                        }
                        // luego por aca actualizamos el registro insertado por el trigger del cabalmac en cabsalalm
                        // con los datos del usuario, fecha, etc.
                        string trep = "0";
                        if (rb_mov.Checked == true) trep = "1";
                        if (rb_ajuste.Checked == true) trep = "2";
                        string actua = "UPDATE controlg a LEFT JOIN cabsalalm b ON CONCAT(a.serguitra,a.numguitra)=b.gremtra " +
                            "SET a.fecentr=@fece,a.tipoent=@tipe,b.nDocRecep=@ndrec,b.nomRecep=@nonrec," +
                            "b.estsalgr=a.estadoser,b.fecentclt=@fece,b.comentclt=@comen,b.tipentclt=@tipe," +
                            "b.verApp=@vera,userc=@asd,fechc=now(),diriplan4=@dipl,diripwan4=@dipw,netbname=@netn " +
                            "WHERE CONCAT(a.serguitra,a.numguitra)=@grte";
                        using (MySqlCommand micon = new MySqlCommand(actua, cn))
                        {
                            micon.Parameters.AddWithValue("@fece", dtp_fsal.Value.ToString("yyyy-MM-dd"));
                            micon.Parameters.AddWithValue("@tipe", trep);
                            micon.Parameters.AddWithValue("@comen", tx_comsal.Text);
                            micon.Parameters.AddWithValue("@grte", dataGridView1.Rows[0].Cells[1].Value.ToString());
                            micon.Parameters.AddWithValue("@ndrec", tx_ndr.Text);
                            micon.Parameters.AddWithValue("@nonrec", tx_evento.Text);
                            micon.Parameters.AddWithValue("@vera", verapp);
                            micon.Parameters.AddWithValue("@asd", Program.vg_user);
                            micon.Parameters.AddWithValue("@dipl", lib.iplan());
                            micon.Parameters.AddWithValue("@dipw", TransCarga.Program.vg_ipwan);
                            micon.Parameters.AddWithValue("@netn", Environment.MachineName);
                            micon.ExecuteNonQuery();
                        }
                        bien = true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error en conexión al servidor");
                Application.Exit();
            }
            return bien;
        }
        private void combos()
        {
            //
        }
        private void cmb_dest_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }
        private void rb_ajuste_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ajuste.Checked == true)
            {
                tx_dat_dest.Text = "";
                cmb_dest.SelectedIndex = -1;
                cmb_dest.Enabled = false;
                tx_evento.Text = "";
                tx_evento.Enabled = false;
            }
        }
        private void rb_mov_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_mov.Checked == true)
            {
                cmb_dest.Enabled = true;
                tx_evento.Enabled = true;
            }
        }
    }
}
