using System;
using System.Data;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class movimas : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public bool retorno;
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        public string[,] para3 = new string[10, 7]
        {
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" },
                {"","","","","","","" }
        };
        AutoCompleteStringCollection repart = new AutoCompleteStringCollection();       // autocompletado repartidores cobradores
        string para1, para2;
        libreria lib = new libreria();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        public movimas(string parm1,string parm2,string[,] parm3)    //
        {
            InitializeComponent();
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
            dataGridView1.Columns[1].Width = 70;    // guia
            dataGridView1.Columns[2].Width = 30;    // cantid
            dataGridView1.Columns[3].Width = 60;    // almacen
            dataGridView1.Columns[4].Width = 60;    // 
            dataGridView1.Columns[5].Width = 80;    // fecha reparto
            dataGridView1.Columns[6].Width = 60;    // 
            dataGridView1.Columns[7].Width = 120;    // nombre del destinario
            tx_fecon.Text = DateTime.Now.ToString("dd/MM/yyyy");
            tx_contra.MaxLength = 6;

            if (parm1 == "reserva")
            {
                lb_titulo.Text = "SALIDA A REPARTO";
                panel3.Visible = true;
                panel3.Left = 2;    // 7
                panel3.Top = 25;     // 30
                panel4.Visible = false;
                dataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[7].Visible = true;
                for (int i = 0; i < 10; i++)
                {
                    dataGridView1.Rows.Add(parm3[i, 0], parm3[i, 1], parm3[i, 2], parm3[i, 3],"","","",parm3[i, 7]);
                }
            }
            if (parm1 == "salida")
            {
                lb_titulo.Text = "ENTREGA MASIVA";
                panel4.Visible = true;
                panel4.Left = 2;    // 7
                panel4.Top = 25;     // 30
                panel3.Visible = false;
                rb_mov.Checked = true;
                combos();
                dataGridView1.Columns[4].Visible = true;
                dataGridView1.Columns[5].Visible = true;
                dataGridView1.Columns[6].Visible = true;
                for (int i = 0; i < 10; i++)
                {
                    dataGridView1.Rows.Add(parm3[i, 0], parm3[i, 1], parm3[i, 2], parm3[i, 3], parm3[i, 4], parm3[i, 5], parm3[i, 6]);
                }
            }
            this.KeyPreview = true; // habilitando la posibilidad de pasar el tab con el enter
        }
        private void movimas_Load(object sender, EventArgs e)
        {
            combos();
            autorepar();                                     // autocompleta repartidores
            // autocompletados
            tx_contra.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_contra.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_contra.AutoCompleteCustomSource = repart;
        }
        private void movimas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
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
            retorno = false;    // false = no se hizo nada
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (tx_contra.Text.Trim() == "" && para1 == "reserva")
            {
                MessageBox.Show("Ingrese el reponsable del despacho","Complete la información",MessageBoxButtons.OK,MessageBoxIcon.Information);
                tx_contra.Focus();
                return;
            }
            if (tx_unidad.Text.Trim() == "" && para1 == "reserva")
            {
                MessageBox.Show("Ingrese la unidad de reparto", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_unidad.Focus();
                return;
            }
            var aa = MessageBox.Show("Confirma que desea grabar la operación?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (aa == DialogResult.Yes)
            {
                if (para1 == "reserva")
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                        {
                            dataGridView1.Rows[i].Cells[6].Value = tx_unidad.Text;
                            dataGridView1.Rows[i].Cells[4].Value = tx_contra.Text;
                            dataGridView1.Rows[i].Cells[5].Value = tx_fecon.Text;
                            //
                            para3[i, 0] = dataGridView1.Rows[i].Cells[0].Value.ToString();
                            para3[i, 1] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                            para3[i, 2] = dataGridView1.Rows[i].Cells[2].Value.ToString();
                            para3[i, 3] = dataGridView1.Rows[i].Cells[3].Value.ToString();
                            para3[i, 4] = dataGridView1.Rows[i].Cells[4].Value.ToString();
                            para3[i, 5] = dataGridView1.Rows[i].Cells[5].Value.ToString();
                            para3[i, 6] = dataGridView1.Rows[i].Cells[6].Value.ToString();
                        }
                    }
                    retorno = true; // true = se efectuo la operacion
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
        private bool salida()               // ACA BORRAMOS DEL STOCK Y AGREGAMOS A LA TABLA DE SALIDAS (trigger)
        {
            bool bien = false;
            using (MySqlConnection cn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(cn) == true)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                        {
                            string llama = "borraseguro";
                            using (MySqlCommand micon = new MySqlCommand(llama, cn))
                            {
                                micon.CommandType = CommandType.StoredProcedure;
                                micon.Parameters.AddWithValue("@tabla", "cabalmac");
                                micon.Parameters.AddWithValue("@vidr", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                micon.Parameters.AddWithValue("@vidc", 0);
                                micon.ExecuteNonQuery();
                            }
                            // luego por aca actualizamos el registro insertado por el trigger del cabalmac en cabsalalm
                            // con los datos del usuario, fecha, etc.
                            string trep = "0";
                            if (rb_mov.Checked == true) trep = "1";
                            if (rb_ajuste.Checked == true) trep = "2";
                            string actua = "UPDATE controlg a LEFT JOIN cabsalalm b ON CONCAT(a.serguitra,a.numguitra)=b.gremtra " +
                                "SET a.fecentr=@fece,a.tipoent=@tipe," +
                                "b.estsalgr=a.estadoser,b.fecentclt=@fece,b.comentclt=@comen,b.tipentclt=@tipe," +
                                "b.verApp=@vera,userc=@asd,fechc=now(),diriplan4=@dipl,diripwan4=@dipw,netbname=@netn " +
                                "WHERE CONCAT(a.serguitra,a.numguitra)=@grte";
                            using (MySqlCommand micon = new MySqlCommand(actua,cn))
                            {
                                micon.Parameters.AddWithValue("@fece", dtp_fsal.Value.ToString("yyyy-MM-dd"));
                                micon.Parameters.AddWithValue("@tipe", trep);
                                micon.Parameters.AddWithValue("@comen", tx_comsal.Text);
                                micon.Parameters.AddWithValue("@grte", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@vera", verapp);
                                micon.Parameters.AddWithValue("@asd", Program.vg_user);
                                micon.Parameters.AddWithValue("@dipl", lib.iplan());
                                micon.Parameters.AddWithValue("@dipw", TransCarga.Program.vg_ipwan);
                                micon.Parameters.AddWithValue("@netn", Environment.MachineName);
                                micon.ExecuteNonQuery();

                            }
                        }
                    }
                    bien = true;
                }
            }
            return bien;
        }
        private void tx_contra_Leave(object sender, EventArgs e)    // repartidores cobradores
        {
            if (tx_contra.Text == "")
            {
                //button1.Focus();
                tx_contra.Focus();
                return;
            }
            using (MySqlConnection cn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(cn) == true)
                {
                    string lee = "select count(id) from cabrrhh where sede=@sed and codigo=@cod";
                    using (MySqlCommand micon = new MySqlCommand(lee,cn))
                    {
                        micon.Parameters.AddWithValue("@sed", Program.vg_luse);
                        micon.Parameters.AddWithValue("@cod", tx_contra.Text);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr.GetInt16(0) == 0)
                                {
                                    tx_contra.Focus();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void combos()
        {
            this.panel4.Focus();
            MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
            cn.Open();
            try
            {
                // seleccion de los almacenes de destino
                this.cmb_dest.Items.Clear();
                tx_dat_dest.Text = "";
                ComboItem citem_dest = new ComboItem();
                const string condest = "select descrizionerid,idcodice from desc_alm " +
                    "where numero=1";
                MySqlCommand cmd2 = new MySqlCommand(condest, cn);
                DataTable dt2 = new DataTable();
                MySqlDataAdapter da2 = new MySqlDataAdapter(cmd2);
                da2.Fill(dt2);
                foreach (DataRow row in dt2.Rows)
                {
                    citem_dest.Text = row.ItemArray[0].ToString();
                    citem_dest.Value = row.ItemArray[1].ToString();
                    this.cmb_dest.Items.Add(citem_dest);
                    this.cmb_dest.ValueMember = citem_dest.Value.ToString();
                }
                cn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message,"No se puede conectar al servidor");
                Application.Exit();
                return;
            }
        }
        private void autorepar()
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    string consulta = "select codigo from cabrrhh where sede=@sed";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@sed", para2);
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                repart.Add(dr["codigo"].ToString());
                            }
                        }
                        dr.Close();
                    }
                }
            }
        }
        private void cmb_dest_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
            cn.Open();
            try
            {
                //int aq = Int16.Parse(this.cmb_dest.SelectedIndex.ToString());
                string consulta = "select idcodice from desc_alm where descrizionerid=@des and numero=1";
                MySqlCommand micon = new MySqlCommand(consulta, cn);
                micon.Parameters.AddWithValue("@des", cmb_dest.Text.ToString());
                MySqlDataReader midr = micon.ExecuteReader();
                if (midr.Read())
                {
                    this.tx_dat_dest.Text = midr["idcodice"].ToString();
                }
                midr.Close();
                cn.Close();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message,"No se pudo conectar con el servidor");
                Application.Exit();
                return;
            }
        }
        private void rb_mov_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_mov.Checked == true)
            {
                tx_dat_dest.Text = "";
                cmb_dest.Enabled = true;
                //tx_evento.Enabled = true;
            }
        }
        private void rb_ajuste_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ajuste.Checked == true)
            {
                tx_dat_dest.Text = "";
                cmb_dest.SelectedIndex = -1;
                cmb_dest.Enabled = false;
            }
        }
    }
}
