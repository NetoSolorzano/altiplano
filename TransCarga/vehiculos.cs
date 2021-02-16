using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class vehiculos : Form
    {
        static string nomform = "vehiculos";               // nombre del formulario
        string asd = TransCarga.Program.vg_user;        // usuario conectado al sistema
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color fondo sin grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "vehiculos";
        public int totfilgrid, cta;      // variables para impresion
        string img_btN = "";
        string img_btE = "";
        string img_btA = "";
        string img_bti = "";
        string img_bts = "";
        string img_btr = "";
        string img_btf = "";
        string img_btq = "";
        string img_grab = "";
        string img_anul = "";
        string vEstAnu = "";            // estado de serie anulada
        string vtd_ruc = "";
        string v_tipcarr = "";          // tipo de placa CARRETA
        libreria lib = new libreria();
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        DataTable dtg = new DataTable();

        public vehiculos()
        {
            InitializeComponent();
        }
        private void vehiculos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void vehiculos_Load(object sender, EventArgs e)
        {
            /*
            ToolTip toolTipNombre = new ToolTip();           // Create the ToolTip and associate with the Form container.
            toolTipNombre.AutoPopDelay = 5000;
            toolTipNombre.InitialDelay = 1000;
            toolTipNombre.ReshowDelay = 500;
            toolTipNombre.ShowAlways = true;                 // Force the ToolTip text to be displayed whether or not the form is active.
            toolTipNombre.SetToolTip(toolStrip1, nomform);   // Set up the ToolTip text for the object
            */
            init();
            toolboton();
            limpiar(this);
            sololee(this);
            dataload();
            grilla();
            this.KeyPreview = true;
            tabControl1.SelectedTab = tabgrilla;
            advancedDataGridView1.Enabled = false;
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            advancedDataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //advancedDataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //advancedDataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //advancedDataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            tabreg.BackColor = Color.FromName(colpage);

            jalainfo();
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            Bt_close.Image = Image.FromFile(img_btq);
            Bt_ini.Image = Image.FromFile(img_bti);
            Bt_sig.Image = Image.FromFile(img_bts);
            Bt_ret.Image = Image.FromFile(img_btr);
            Bt_fin.Image = Image.FromFile(img_btf);
            // tamaños maximos de caracteres
            tx_ruc.MaxLength = 11;
            tx_placa.MaxLength = 7;
            tx_placa.CharacterCasing = CharacterCasing.Upper;
            tx_trackAsoc.MaxLength = 7;
            tx_trackAsoc.CharacterCasing = CharacterCasing.Upper;
            tx_marca.MaxLength = 45;
            tx_marca.CharacterCasing = CharacterCasing.Upper;
            tx_chasis.MaxLength = 45;
            tx_modelo.MaxLength = 45;
            tx_modelo.CharacterCasing = CharacterCasing.Upper;
            tx_motor.MaxLength = 45;
            tx_autor1.MaxLength = 45;
            tx_soat.MaxLength = 45;
            tx_confv.MaxLength = 10;
            tx_confv.CharacterCasing = CharacterCasing.Upper;
            tx_coment.MaxLength = 150;
        }
        private void grilla()                   // arma la grilla
        {
            // a.id,a.rucpro,c.razonsocial,a.coment,a.tipo,b.descrizionerid,a.status,a.placa,a.marca,
            // a.modelo,a.confve,a.chasis,a.motor,a.autor1,a.soat
            Font tiplg = new Font("Arial",7, FontStyle.Bold);
            advancedDataGridView1.Font = tiplg;
            advancedDataGridView1.DefaultCellStyle.Font = tiplg;
            advancedDataGridView1.RowTemplate.Height = 15;
            advancedDataGridView1.DataSource = dtg;
            // id
            advancedDataGridView1.Columns[0].Visible = false;
            // ruc propietario
            advancedDataGridView1.Columns[1].Visible = true;            // columna visible o no
            advancedDataGridView1.Columns[1].HeaderText = "RUC";        // titulo de la columna
            advancedDataGridView1.Columns[1].Width = 70;                // ancho
            advancedDataGridView1.Columns[1].ReadOnly = true;           // lectura o no
            advancedDataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // nombre propietario
            advancedDataGridView1.Columns[2].Visible = true;       
            advancedDataGridView1.Columns[2].HeaderText = "PROPIETARIO";
            advancedDataGridView1.Columns[2].Width = 150;
            advancedDataGridView1.Columns[2].ReadOnly = true;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[2].Tag = "validaNO";          // las celdas de esta columna se NO se validan
            advancedDataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // COMENT
            advancedDataGridView1.Columns[3].Visible = true;
            advancedDataGridView1.Columns[3].HeaderText = "COMENTARIOS";
            advancedDataGridView1.Columns[3].Width = 100;
            advancedDataGridView1.Columns[3].ReadOnly = false;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[3].Tag = "validaNO";          // las celdas de esta columna se validan
            advancedDataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // TIPO codigo
            advancedDataGridView1.Columns[4].Visible = false;
            advancedDataGridView1.Columns[4].HeaderText = "TIPO";
            advancedDataGridView1.Columns[4].Width = 30;
            advancedDataGridView1.Columns[4].ReadOnly = true;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[4].Tag = "validaNO";          // las celdas de esta columna se validan
            advancedDataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // TIPO NOMBRE
            advancedDataGridView1.Columns[5].Visible = true;
            advancedDataGridView1.Columns[5].HeaderText = "TIPOV";
            advancedDataGridView1.Columns[5].Width = 80;
            advancedDataGridView1.Columns[5].ReadOnly = true;
            advancedDataGridView1.Columns[5].Tag = "validaNO";          // las celdas de esta columna se NO se validan
            advancedDataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // status
            advancedDataGridView1.Columns[6].Visible = false;
            // placa
            advancedDataGridView1.Columns[7].Visible = true;       
            advancedDataGridView1.Columns[7].HeaderText = "PLACA";
            advancedDataGridView1.Columns[7].Width = 70;
            advancedDataGridView1.Columns[7].ReadOnly = true;
            advancedDataGridView1.Columns[7].Tag = "validaNO";          // las celdas de esta columna SI se validan
            advancedDataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // marca
            advancedDataGridView1.Columns[8].Visible = true;
            advancedDataGridView1.Columns[8].HeaderText = "MARCA";
            advancedDataGridView1.Columns[8].Width = 80;
            advancedDataGridView1.Columns[8].ReadOnly = true;
            advancedDataGridView1.Columns[8].Tag = "validaNO";          // las celdas de esta columna SI se validan
            advancedDataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // modelo
            advancedDataGridView1.Columns[9].Visible = true;    
            advancedDataGridView1.Columns[9].HeaderText = "MODELO";
            advancedDataGridView1.Columns[9].Width = 100;
            advancedDataGridView1.Columns[9].ReadOnly = true;
            advancedDataGridView1.Columns[9].Tag = "validaNO";
            advancedDataGridView1.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // config. vehicular
            advancedDataGridView1.Columns[10].Visible = true;
            advancedDataGridView1.Columns[10].HeaderText = "CONF.V";
            advancedDataGridView1.Columns[10].Width = 70;
            advancedDataGridView1.Columns[10].ReadOnly = true;
            advancedDataGridView1.Columns[10].Tag = "validaNO";
            advancedDataGridView1.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // CHASIS
            advancedDataGridView1.Columns[11].Visible = true;
            advancedDataGridView1.Columns[11].HeaderText = "CHASIS";
            advancedDataGridView1.Columns[11].Width = 100;
            advancedDataGridView1.Columns[11].ReadOnly = true;
            advancedDataGridView1.Columns[11].Tag = "validaNO";
            advancedDataGridView1.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // motor
            advancedDataGridView1.Columns[12].Visible = true;
            advancedDataGridView1.Columns[12].HeaderText = "MOTOR";
            advancedDataGridView1.Columns[12].Width = 100;
            advancedDataGridView1.Columns[12].ReadOnly = true;
            advancedDataGridView1.Columns[12].Tag = "validaNO";
            advancedDataGridView1.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // autorizacion circulacion
            advancedDataGridView1.Columns[13].Visible = true;
            advancedDataGridView1.Columns[13].HeaderText = "AUTORIZ.";
            advancedDataGridView1.Columns[13].Width = 100;
            advancedDataGridView1.Columns[13].ReadOnly = true;
            advancedDataGridView1.Columns[13].Tag = "validaNO";
            advancedDataGridView1.Columns[13].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // soat
            advancedDataGridView1.Columns[14].Visible = true;
            advancedDataGridView1.Columns[14].HeaderText = "SOAT";
            advancedDataGridView1.Columns[14].Width = 100;
            advancedDataGridView1.Columns[14].ReadOnly = true;
            advancedDataGridView1.Columns[14].Tag = "validaNO";
            advancedDataGridView1.Columns[14].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        private void jalainfo()                 // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nofa,@nofi)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nofa", nomform);
                micon.Parameters.AddWithValue("@nofi", "proveed");
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["formulario"].ToString() == "main" && row["campo"].ToString() == "imagenes")
                    {
                        if (row["param"].ToString() == "img_btN") img_btN = row["valor"].ToString().Trim();         // imagen del boton de accion NUEVO
                        if (row["param"].ToString() == "img_btE") img_btE = row["valor"].ToString().Trim();         // imagen del boton de accion EDITAR
                        if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                        if (row["param"].ToString() == "img_btQ") img_btq = row["valor"].ToString().Trim();         // imagen del boton de accion SALIR
                        if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                        if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                        if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                        if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                        if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                        if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular

                    }
                    if (row["formulario"].ToString() == "main" && row["campo"].ToString() == "estado" && row["param"].ToString() == "anulado") vEstAnu = row["valor"].ToString().Trim();
                    if (row["formulario"].ToString() == "proveed" && row["campo"].ToString() == "documento" && row["param"].ToString() == "ruc") vtd_ruc = row["valor"].ToString().Trim();
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "carreta") v_tipcarr = row["valor"].ToString().Trim();
                    }
                }
                da.Dispose();
                dt.Dispose();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        public void jalaoc(string campo)        // jala datos de grilla
        {
            // a.id,a.rucpro,c.razonsocial,a.coment,a.tipo,b.descrizionerid,a.status,a.placa,a.marca,
            // a.modelo,a.confve,a.chasis,a.motor,a.autor1,a.soat
            if (campo == "tx_rind")
            {
                tx_ruc.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[1].Value.ToString();  // ruc propiet
                tx_propiet.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[2].Value.ToString();    // nombre p
                tx_coment.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[3].Value.ToString();     // comentario
                tx_tipo.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[4].Value.ToString();       // tipo vehiculo
                cmb_tipo.SelectedValue = tx_tipo.Text;                                                          // tipo de vehiculo
                chk_habil.Checked = (advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[6].Value.ToString() != vEstAnu) ? true : false;
                tx_placa.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[7].Value.ToString();      // placa
                tx_marca.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[8].Value.ToString();      // marca
                tx_modelo.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[9].Value.ToString();     // modelo
                tx_motor.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[12].Value.ToString();     // motor
                tx_autor1.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[13].Value.ToString();    // autorizacion
                tx_soat.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[14].Value.ToString();      // motor
                tx_confv.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[10].Value.ToString();     // conf.vehicular
                tx_chasis.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[11].Value.ToString();     // chasis
                tx_trackAsoc.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[15].Value.ToString();     // placa asociada trackto-carreta
            }
            if (campo == "tx_idr")
            {
                // ... no lo soo
            }
        }
        public void dataload()                  // jala datos para los combos y la grilla
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            tabControl1.SelectedTab = tabreg;
            // DATOS DEL tipo de vehiculo
            cmb_tipo.Items.Clear();
            const string contpu = "select idcodice,descrizionerid from desc_tve " +
                "order by idcodice";
            MySqlCommand cmbtpu = new MySqlCommand(contpu, conn);
            DataTable dttpu = new DataTable();
            MySqlDataAdapter datpu = new MySqlDataAdapter(cmbtpu);
            datpu.Fill(dttpu);
            cmb_tipo.DataSource = dttpu;
            cmb_tipo.DisplayMember = "descrizionerid";
            cmb_tipo.ValueMember = "idcodice";
            // datos vehiculos
            string datgri = "select a.id,a.rucpro,c.razonsocial,a.coment,a.tipo,b.descrizionerid,a.status,a.placa,a.marca," +
                "a.modelo,a.confve,a.chasis,a.motor,a.autor1,a.soat,a.placAsoc " +
                "from vehiculos a " +
                "left join desc_tve b on b.idcodice=a.tipo " +
                "left join anag_for c on c.ruc=a.rucpro " +
                "order by a.placa,a.tipo";
            MySqlCommand cdg = new MySqlCommand(datgri, conn);
            MySqlDataAdapter dag = new MySqlDataAdapter(cdg);
            dtg.Clear();
            dag.Fill(dtg);
            //
            conn.Close();
        }
        string[] equivinter(string titulo)        // equivalencia entre titulo de columna y tabla 
        {
            string[] retorna = new string[2];
            switch (titulo)
            {
                case "NIVEL":
                    retorna[0] = "desc_niv";
                    retorna[1] = "codigo";
                    break;
                case "???":
                    retorna[0] = "";
                    retorna[1] = "";
                    break;
                case "????":
                    retorna[0] = "";
                    retorna[1] = "";
                    break;
                case "LOCAL":
                    retorna[0] = "desc_alm";
                    retorna[1] = "idcodice";
                    break;
                case "TIENDA":
                    retorna[0] = "desc_ven";
                    retorna[1] = "idcodice";
                    break;
                case "SEDE":
                    retorna[0] = "desc_loc";
                    retorna[1] = "idcodice";
                    break;
                case "RUC":
                    retorna[0] = "desc_raz";
                    retorna[1] = "idcodice";
                    break;
            }
            return retorna;
        }
        private void vali_placa()
        {
            // nuevo -> placa No debe existir en la grilla
            // editar -> placa Si debe existir y jalar datos de la grilla
            if (tx_placa.Text.Trim() != "")
            {
                DataRow[] rowb = dtg.Select("placa = '" + tx_placa.Text + "'");
                if (Tx_modo.Text == "NUEVO")
                {
                    if (rowb.Length > 0)
                    {
                        MessageBox.Show("Ya existe la placa ingresada", "Atención - rectifique", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_placa.Text = "";
                        tx_placa.Focus();
                        return;
                    }
                }
                if (Tx_modo.Text == "EDITAR")
                {
                    if (rowb.Length < 1)
                    {
                        MessageBox.Show("NO existe la placa ingresada", "Atención - rectifique", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_placa.Text = "";
                        tx_placa.Focus();
                        return;
                    }
                    else
                    {
                        tx_placa.ReadOnly = true;
                        DataRow row = rowb[0];
                        tx_idr.Text = row[0].ToString();
                        //jalaoc("tx_idr");
                        tx_ruc.Text = row[1].ToString();
                        tx_propiet.Text = row[2].ToString();
                        tx_coment.Text = row[3].ToString();
                        tx_tipo.Text = row[4].ToString();
                        cmb_tipo.SelectedValue = tx_tipo.Text;
                        chk_habil.Checked = (row[6].ToString() != vEstAnu) ? true : false;
                        tx_placa.Text = row[7].ToString();
                        tx_marca.Text = row[8].ToString();
                        tx_modelo.Text = row[9].ToString();
                        tx_motor.Text = row[12].ToString();
                        tx_autor1.Text = row[13].ToString();
                        tx_soat.Text = row[14].ToString();
                        tx_confv.Text = row[10].ToString();
                        tx_chasis.Text = row[11].ToString();
                    }
                }
            }
        }

        #region limpiadores_modos
        public void sololee(Form lfrm)
        {
            foreach (Control oControls in lfrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is ComboBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is RadioButton)
                {
                    oControls.Enabled = false;
                }
                if (oControls is DateTimePicker)
                {
                    oControls.Enabled = false;
                }
                if (oControls is MaskedTextBox)
                {
                    oControls.Enabled = false;
                }
                if (oControls is GroupBox)
                {
                    oControls.Enabled = false;
                }
            }
        }
        public void escribe(Form efrm)
        {
            foreach (Control oControls in efrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is ComboBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is RadioButton)
                {
                    oControls.Enabled = true;
                }
                if (oControls is DateTimePicker)
                {
                    oControls.Enabled = true;
                }
                if (oControls is MaskedTextBox)
                {
                    oControls.Enabled = true;
                }
            }
        }
        private void limpiar(Form ofrm)
        {
            foreach (Control oControls in ofrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
            }
        }
        private void limpiaPag(TabPage pag)
        {
            foreach (Control oControls in pag.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
            }
        }
        public void limpia_chk()    
        {
            chk_habil.Checked = false;
        }
        public void limpia_otros()
        {
            //chk_habil.Checked = false;
        }
        public void limpia_combos()
        {
            cmb_tipo.SelectedIndex = -1;
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            // validamos que los campos no esten vacíos
            if (tx_placa.Text == "")
            {
                MessageBox.Show("Ingrese la Placa", " Error! ");
                tx_placa.Focus();
                return;
            }
            if (tx_marca.Text == "")
            {
                MessageBox.Show("Ingrese la marca", " Error! ");
                tx_marca.Focus();
                return;
            }
            if (tx_ruc.Text == "")
            {
                MessageBox.Show("Seleccione el propietario", " Atención ");
                tx_ruc.Focus();
                return;
            }
            if(tx_tipo.Text == "")
            {
                MessageBox.Show("Seleccione el tipo de Veh.", " Atención ");
                cmb_tipo.Focus();
                return;
            }
            if(tx_modelo.Text == "")
            {
                MessageBox.Show("Seleccione el modelo", " Atención ");
                tx_modelo.Focus();
                return;
            }
            if (tx_confv.Text == "")
            {
                MessageBox.Show("Ingrese la configuración vehicular", " Atención ");
                tx_confv.Focus();
                return;
            }
            if (tx_autor1.Text == "")
            {
                MessageBox.Show("Ingrese la autorización de circulación", " Atención ");
                tx_autor1.Focus();
                return;
            }
            // grabamos, actualizamos, etc
            string modo = this.Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                var aa = MessageBox.Show("Confirma que desea agregar?", "Atención - Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    iserror = "no";
                    string consulta = "insert into vehiculos (rucpro,coment,tipo,status,placa,marca,modelo,confve,chasis,motor,autor1,soat,placAsoc," +
                        "verApp,userc,fechc,diriplan4,diripwan4,nbname)" +
                        " values (@ruc,@com,@tip,@est,@pla,@mar,@mod,@cov,@cha,@mot,@aut,@soa,@pas," +
                        "@vapp,@asd,now(),@dil4,@diw4,@nbna)";
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        MySqlCommand mycomand = new MySqlCommand(consulta, conn);
                        mycomand.Parameters.AddWithValue("@ruc", tx_ruc.Text);
                        mycomand.Parameters.AddWithValue("@com", tx_coment.Text);
                        mycomand.Parameters.AddWithValue("@tip", tx_tipo.Text);
                        mycomand.Parameters.AddWithValue("@est", (chk_habil.Checked == true) ? "" : vEstAnu);
                        mycomand.Parameters.AddWithValue("@pla", tx_placa.Text);
                        mycomand.Parameters.AddWithValue("@mar", tx_marca.Text);
                        mycomand.Parameters.AddWithValue("@mod", tx_modelo.Text);
                        mycomand.Parameters.AddWithValue("@cov", tx_confv.Text);
                        mycomand.Parameters.AddWithValue("@cha", tx_chasis.Text);
                        mycomand.Parameters.AddWithValue("@mot", tx_motor.Text);
                        mycomand.Parameters.AddWithValue("@aut", tx_autor1.Text);
                        mycomand.Parameters.AddWithValue("@soa", tx_soat.Text);
                        mycomand.Parameters.AddWithValue("@pas", tx_trackAsoc.Text);
                        //
                        mycomand.Parameters.AddWithValue("@asd", asd);
                        mycomand.Parameters.AddWithValue("@vapp", verapp);
                        mycomand.Parameters.AddWithValue("@dil4", lib.iplan());
                        mycomand.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                        mycomand.Parameters.AddWithValue("@nbna", lib.nbname());
                        try
                        {
                            mycomand.ExecuteNonQuery();
                            mycomand = new MySqlCommand("select last_insert_id()", conn);
                            MySqlDataReader dr = mycomand.ExecuteReader();
                            string idtu = "";
                            if (dr.Read()) idtu = dr.GetString(0);
                            dr.Close();
                            mycomand.Dispose();
                            // insertamos en el datatable
                            DataRow drs = dtg.NewRow();
                            // a.id,a.rucpro,c.razonsocial,a.coment,a.tipo,b.descrizionerid,a.status,a.placa,a.marca,
                            // a.modelo,a.confve,a.chasis,a.motor,a.autor1,a.soat
                            drs[0] = idtu;
                            drs[1] = tx_ruc.Text;
                            drs[2] = tx_propiet.Text;
                            drs[3] = tx_coment.Text;
                            drs[4] = tx_tipo.Text;
                            drs[5] = cmb_tipo.Text;
                            drs[6] = (chk_habil.Checked == true) ? "" : vEstAnu;
                            drs[7] = tx_placa.Text;
                            drs[8] = tx_marca.Text;
                            drs[9] = tx_modelo.Text;
                            drs[10] = tx_confv.Text;
                            drs[11] = tx_chasis.Text;
                            drs[12] = tx_motor.Text;
                            drs[13] = tx_autor1.Text;
                            drs[14] = tx_soat.Text;
                            drs[15] = tx_trackAsoc.Text;
                            dtg.Rows.Add(drs);
                            //
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")                                    // actualizamos la tabla usuarios
                            {
                                MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                                return;
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.Message, "Error en ingresar el vehículo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            iserror = "si";
                        }
                        if (tx_trackAsoc.Text.Trim() != "")
                        {
                            string actua = "update vehiculos set placAsoc=@pla where placa=@pas";
                            using (MySqlCommand micon = new MySqlCommand(actua, conn))
                            {
                                micon.Parameters.AddWithValue("@pla", tx_placa.Text);
                                micon.Parameters.AddWithValue("@pas", tx_trackAsoc.Text);
                                micon.ExecuteNonQuery();
                            }
                        }
                        conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se estableció conexión con el servidor", "Atención - no se puede continuar");
                        Application.Exit();
                        return;
                    }
                }
            }
            if (modo == "EDITAR")
            {
                var aa = MessageBox.Show("Confirma que desea modificar?", "Atención - Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(aa == DialogResult.Yes)
                {
                    iserror = "no";
                    string consulta = "update vehiculos set " +
                        "rucpro=@ruc,coment=@com,tipo=@tip,status=@est,marca=@mar,modelo=@mod,confve=@cov,chasis=@cha,motor=@mot,autor1=@aut,soat=@soa,placAsoc=@pas," +
                        "verApp=@vapp,userm=@asd,fechm=now(),diriplan4=@dil4,diripwan4=@diw4,nbname=@nbna " +
                        "where id=@idc";
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        MySqlCommand mycom = new MySqlCommand(consulta, conn);
                        mycom.Parameters.AddWithValue("@idc", tx_idr.Text);
                        mycom.Parameters.AddWithValue("@ruc", tx_ruc.Text);
                        mycom.Parameters.AddWithValue("@com", tx_coment.Text);
                        mycom.Parameters.AddWithValue("@tip", tx_tipo.Text);
                        mycom.Parameters.AddWithValue("@est", (chk_habil.Checked == true) ? "" : vEstAnu);
                        mycom.Parameters.AddWithValue("@mar", tx_marca.Text);
                        mycom.Parameters.AddWithValue("@mod", tx_modelo.Text);
                        mycom.Parameters.AddWithValue("@cov", tx_confv.Text);
                        mycom.Parameters.AddWithValue("@cha", tx_chasis.Text);
                        mycom.Parameters.AddWithValue("@mot", tx_motor.Text);
                        mycom.Parameters.AddWithValue("@aut", tx_autor1.Text);
                        mycom.Parameters.AddWithValue("@soa", tx_soat.Text);
                        mycom.Parameters.AddWithValue("@pas", tx_trackAsoc.Text);
                        //
                        mycom.Parameters.AddWithValue("@asd", asd);
                        mycom.Parameters.AddWithValue("@vapp", verapp);
                        mycom.Parameters.AddWithValue("@dil4", lib.iplan());
                        mycom.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                        mycom.Parameters.AddWithValue("@nbna", lib.nbname());
                        try
                        {
                            mycom.ExecuteNonQuery();
                            mycom = new MySqlCommand("select last_insert_id()", conn);
                            MySqlDataReader dr = mycom.ExecuteReader();
                            string idtu = "";
                            if (dr.Read()) idtu = dr.GetString(0);
                            dr.Close();
                            mycom.Dispose();
                            // actualizamos el datatable
                            for (int i = 0; i < dtg.Rows.Count; i++)
                            {
                                DataRow row = dtg.Rows[i];
                                if (row[0].ToString() == tx_idr.Text)
                                {
                                    dtg.Rows[i][1] = tx_ruc.Text;
                                    dtg.Rows[i][2] = tx_propiet.Text;
                                    dtg.Rows[i][3] = tx_coment.Text;
                                    dtg.Rows[i][4] = tx_tipo.Text;
                                    dtg.Rows[i][5] = cmb_tipo.Text;
                                    dtg.Rows[i][6] = (chk_habil.Checked == true) ? "" : vEstAnu;
                                    dtg.Rows[i][7] = tx_placa.Text;
                                    dtg.Rows[i][8] = tx_marca.Text;
                                    dtg.Rows[i][9] = tx_modelo.Text;
                                    dtg.Rows[i][10] = tx_confv.Text;
                                    dtg.Rows[i][11] = tx_chasis.Text;
                                    dtg.Rows[i][12] = tx_motor.Text;
                                    dtg.Rows[i][13] = tx_autor1.Text;
                                    dtg.Rows[i][14] = tx_soat.Text;
                                    dtg.Rows[i][15] = tx_trackAsoc.Text;
                                }
                            }
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")                                        // actualizamos la tabla usuarios
                            {
                                MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                                return;
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.Message, "Error de Editar vehículo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            iserror = "si";
                        }
                        if (tx_trackAsoc.Text.Trim() != "")
                        {
                            string actua = "update vehiculos set placAsoc=@pla where placa=@pas";
                            using (MySqlCommand micon = new MySqlCommand(actua, conn))
                            {
                                micon.Parameters.AddWithValue("@pla", tx_placa.Text);
                                micon.Parameters.AddWithValue("@pas", tx_trackAsoc.Text);
                                micon.ExecuteNonQuery();
                            }
                        }
                        conn.Close();
                        //permisos();
                    }
                    else
                    {
                        MessageBox.Show("No se estableció conexión con el servidor", "Atención - no se puede continuar");
                        Application.Exit();
                        return;
                    }
                }
            }
            if (modo == "ANULAR")       // opción para borrar
            { 
                // no se anulan, solo se habilitan o deshabilitan
            }
            if (iserror == "no")
            {
                // debe limpiar los campos y actualizar la grilla
                limpiar(this);
                limpiaPag(tabreg);
                limpia_otros();
                limpia_chk();
                limpia_combos();
            }
        }
        #endregion boton_form;

        #region leaves
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                //jalaoc("tx_idr");               // jalamos los datos del registro
            }
        }
        private void tx_ruc_Leave(object sender, EventArgs e)
        {
            if (tx_ruc.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                if (lib.valiruc(tx_ruc.Text,vtd_ruc) == false)
                {
                    MessageBox.Show("Ruc no válido!", "Atención, debe corregir", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx_ruc.Text = "";
                    tx_ruc.Focus();
                    return;
                }
                else
                {
                    tx_propiet.Text = lib.nomsn("FOR",vtd_ruc,tx_ruc.Text);
                }
            }
        }
        private void tx_placa_Leave(object sender, EventArgs e)
        {
            if(tx_placa.ReadOnly == false && tx_placa.Text.Trim() != "")
            {
                vali_placa();
            }
        }
        private void tx_trackAsoc_Leave(object sender, EventArgs e)
        {
            if (tx_trackAsoc.Text.Trim() != "")
            {
                DataRow[] rowb = dtg.Select("placa = '" + tx_trackAsoc.Text + "' and tipo <> '" + v_tipcarr + "'");
                if (rowb.Length < 1)
                {
                    MessageBox.Show("Placa incorrecta!","Corrija",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    tx_trackAsoc.Text = "";
                    tx_trackAsoc.Focus();
                }
            }
        }
        private void tabreg_Enter(object sender, EventArgs e)
        {
            if(Tx_modo.Text == "EDITAR" && tx_rind.Text.Trim() == "" && tx_placa.Text.Trim() == "")
            {
                tx_placa.ReadOnly = false;
            }
            else
            {
                tx_placa.ReadOnly = true;
            }
        }
        #endregion leaves;

        #region botones_de_comando_y_permisos  
        public void toolboton()
        {
            DataTable mdtb = new DataTable();
            const string consbot = "select * from permisos where formulario=@nomform and usuario=@use";
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    MySqlCommand consulb = new MySqlCommand(consbot, conn);
                    consulb.Parameters.AddWithValue("@nomform", nomform);
                    consulb.Parameters.AddWithValue("@use", asd);
                    MySqlDataAdapter mab = new MySqlDataAdapter(consulb);
                    mab.Fill(mdtb);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, " Error ");
                    return;
                }
                finally { conn.Close(); }
            }
            else
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            if (mdtb.Rows.Count > 0)
            {
                DataRow row = mdtb.Rows[0];
                if (Convert.ToString(row["btn1"]) == "S")
                {
                    this.Bt_add.Visible = true;
                }
                else { this.Bt_add.Visible = false; }
                if (Convert.ToString(row["btn2"]) == "S")
                {
                    this.Bt_edit.Visible = true;
                }
                else { this.Bt_edit.Visible = false; }
                //if (Convert.ToString(row["btn5"]) == "S")
                //{
                //    this.Bt_print.Visible = true;
                //}
                //else { this.Bt_print.Visible = false; }
                if (Convert.ToString(row["btn3"]) == "S")
                {
                    this.Bt_anul.Visible = true;
                }
                else { this.Bt_anul.Visible = false; }
                //if (Convert.ToString(row["btn4"]) == "S")
                //{
                //    this.Bt_ver.Visible = true;
                //}
                //else { this.Bt_ver.Visible = false; }
                if (Convert.ToString(row["btn6"]) == "S")
                {
                    this.Bt_close.Visible = true;
                }
                else { this.Bt_close.Visible = false; }
            }
        }
        #region botones
        private void Bt_add_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.Enabled = true;
            tabControl1.SelectedTab = tabreg;
            escribe(this);
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            limpiar(this);
            limpiaPag(tabreg);
            limpia_otros();
            limpia_combos();
            limpia_chk();
            tx_placa.ReadOnly = false;
            tx_placa.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.Enabled = true;
            if (advancedDataGridView1.CurrentRow.Index > -1)
            {
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
            }
            tabControl1.SelectedTab = tabgrilla;
            escribe(this);
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            limpiar(this);
            limpiaPag(tabreg);
            limpia_otros();
            limpia_combos();
            limpia_chk();
            tx_placa.ReadOnly = true;
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            sololee(this);
            this.Tx_modo.Text = "IMPRIMIR";
            this.button1.Image = Image.FromFile("print48");
            this.tx_placa.Focus();
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            //
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpiaPag(tabreg);
            limpia_otros();
            //--
            tx_idr.Text = lib.gofirts(nomtab);
            tx_idr_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            string aca = tx_idr.Text;
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpiaPag(tabreg);
            limpia_otros();
            //--
            tx_idr.Text = lib.goback(nomtab, aca);
            tx_idr_Leave(null, null);
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            string aca = tx_idr.Text;
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpiaPag(tabreg);
            limpia_otros();
            //--
            tx_idr.Text = lib.gonext(nomtab, aca);
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpiaPag(tabreg);
            limpia_otros();
            //--
            tx_idr.Text = lib.golast(nomtab);
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // permisos para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_tipo.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)cmb_tipo.DataSource).Rows[cmb_tipo.SelectedIndex];
                tx_tipo.Text = (string)row["idcodice"];
            }
            if (tx_tipo.Text == v_tipcarr)      // tipo de placa, CARRETA
            {
                lin_trackAsoc.Visible = true;
                lb_trackAsoc.Visible = true;
                tx_trackAsoc.Visible = true;
                tx_trackAsoc.Focus();
            }
            else
            {
                lin_trackAsoc.Visible = false;
                lb_trackAsoc.Visible = false;
                tx_trackAsoc.Visible = false;
            }
        }
        #endregion comboboxes

        #region advancedatagridview
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)                  // filtro de las columnas
        {
            dtg.DefaultView.RowFilter = advancedDataGridView1.FilterString;
        }
        private void advancedDataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)            // 
        {
            advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }
        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 1)
            {
                //string idr;
                //idr = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                //idr = advancedDataGridView1.CurrentRow.Index.ToString();
                tabControl1.SelectedTab = tabreg;
                limpiar(this);
                limpia_otros();
                limpia_combos();
                limpiaPag(tabreg);
                limpia_otros();
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
                tx_idr.Text = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                jalaoc("tx_rind");
                tx_coment.Focus();
            }
        }
        private void tabreg_Click(object sender, EventArgs e)
        {

        }
        private void advancedDataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) // valida cambios en valor de la celda
        {
            if (e.RowIndex > -1 && e.ColumnIndex > 0 
                && advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != e.FormattedValue.ToString())
            {
                string campo = advancedDataGridView1.Columns[e.ColumnIndex].Name.ToString();
                string[] noeta = equivinter(advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString());    // retorna la tabla segun el titulo de la columna

                var aaa = MessageBox.Show("Confirma que desea cambiar el valor?",
                    "Columna: " + advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString(),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aaa == DialogResult.Yes)
                {
                    if(advancedDataGridView1.Columns[e.ColumnIndex].Tag.ToString() == "validaSI")   // la columna se valida?
                    {
                        // valida si el dato ingresado es valido en la columna
                        if (lib.validac(noeta[0], noeta[1], e.FormattedValue.ToString()) == true)
                        {
                            // llama a libreria con los datos para el update - tabla,id,campo,nuevo valor
                            lib.actuac(nomtab, campo, e.FormattedValue.ToString(),advancedDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                        }
                        else
                        {
                            MessageBox.Show("El valor no es válido para la columna", "Atención - Corrija");
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        // llama a libreria con los datos para el update - tabla,id,campo,nuevo valor
                        lib.actuac(nomtab, campo, e.FormattedValue.ToString(), advancedDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

    }
}
