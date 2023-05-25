using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class recshum : Form
    {
        static string nomform = "recshum";               // nombre del formulario
        string asd = TransCarga.Program.vg_user;        // usuario conectado al sistema
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color fondo sin grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "cabrrhh";
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
        string vEstAnu = "";            // 
        string vtd_ruc = "";
        string v_tipcarr = "";          // tipo de placa CARRETA
        libreria lib = new libreria();
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        DataTable dtg = new DataTable();

        public recshum()
        {
            InitializeComponent();
        }
        private void recshum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void recshum_Load(object sender, EventArgs e)
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
            tx_dni.MaxLength = 11;
            tx_codigo.MaxLength = 10;
            tx_codigo.CharacterCasing = CharacterCasing.Upper;
            tx_nombre.MaxLength = 100;
            tx_brevete.MaxLength = 15;
            tx_brevete.CharacterCasing = CharacterCasing.Upper;
            tx_usersis.MaxLength = 10;
            //tx_usersis.CharacterCasing = CharacterCasing.Upper;
            tx_coment.MaxLength = 150;
        }
        private void grilla()                   // arma la grilla
        {
            Font tiplg = new Font("Arial",7, FontStyle.Bold);
            advancedDataGridView1.Font = tiplg;
            advancedDataGridView1.DefaultCellStyle.Font = tiplg;
            advancedDataGridView1.RowTemplate.Height = 15;
            advancedDataGridView1.DataSource = dtg;
            // id
            advancedDataGridView1.Columns[0].Visible = false;
            // sede
            advancedDataGridView1.Columns[1].Visible = true;            // columna visible o no
            advancedDataGridView1.Columns[1].HeaderText = "SEDE";        // NOMBRE DE LA SEDE
            advancedDataGridView1.Columns[1].Width = 70;                // ancho
            advancedDataGridView1.Columns[1].ReadOnly = true;           // lectura o no
            advancedDataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // codigo
            advancedDataGridView1.Columns[2].Visible = true;       
            advancedDataGridView1.Columns[2].HeaderText = "CODIGO";
            advancedDataGridView1.Columns[2].Width = 70;
            advancedDataGridView1.Columns[2].ReadOnly = true;
            advancedDataGridView1.Columns[2].Tag = "validaNO";          // las celdas de esta columna se NO se validan
            advancedDataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // genero
            advancedDataGridView1.Columns[3].Visible = true;
            advancedDataGridView1.Columns[3].HeaderText = "GENERO";
            advancedDataGridView1.Columns[3].Width = 30;
            advancedDataGridView1.Columns[3].ReadOnly = true;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[3].Tag = "validaNO";
            advancedDataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // DNI
            advancedDataGridView1.Columns[4].Visible = false;
            advancedDataGridView1.Columns[4].HeaderText = "DNI";
            advancedDataGridView1.Columns[4].Width = 60;
            advancedDataGridView1.Columns[4].ReadOnly = true;
            advancedDataGridView1.Columns[4].Tag = "validaNO";
            advancedDataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // NOMBRE
            advancedDataGridView1.Columns[5].Visible = true;
            advancedDataGridView1.Columns[5].HeaderText = "NOMBRE";
            advancedDataGridView1.Columns[5].Width = 180;
            advancedDataGridView1.Columns[5].ReadOnly = true;
            advancedDataGridView1.Columns[5].Tag = "validaNO";
            advancedDataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // NOMBRE DEL TIPO DE EMP
            advancedDataGridView1.Columns[6].Visible = true;
            advancedDataGridView1.Columns[6].HeaderText = "TIPO";
            advancedDataGridView1.Columns[6].Width = 80;
            advancedDataGridView1.Columns[6].ReadOnly = true;
            advancedDataGridView1.Columns[6].Tag = "validaNO";
            advancedDataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // telefono
            advancedDataGridView1.Columns[7].Visible = true;       
            advancedDataGridView1.Columns[7].HeaderText = "TELEFONO";
            advancedDataGridView1.Columns[7].Width = 80;
            advancedDataGridView1.Columns[7].ReadOnly = true;
            advancedDataGridView1.Columns[7].Tag = "validaNO";
            advancedDataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // USUARIO SISTEMA
            advancedDataGridView1.Columns[8].Visible = true;
            advancedDataGridView1.Columns[8].HeaderText = "USUARIO";
            advancedDataGridView1.Columns[8].Width = 80;
            advancedDataGridView1.Columns[8].ReadOnly = true;
            advancedDataGridView1.Columns[8].Tag = "validaNO";
            advancedDataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // BREVETE
            advancedDataGridView1.Columns[9].Visible = true;    
            advancedDataGridView1.Columns[9].HeaderText = "BREVETE";
            advancedDataGridView1.Columns[9].Width = 100;
            advancedDataGridView1.Columns[9].ReadOnly = true;
            advancedDataGridView1.Columns[9].Tag = "validaNO";
            advancedDataGridView1.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // codigo tipo doc usuario
            advancedDataGridView1.Columns[10].Visible = false;
            advancedDataGridView1.Columns[10].HeaderText = "CODTIPU";
            advancedDataGridView1.Columns[10].Width = 70;
            advancedDataGridView1.Columns[10].ReadOnly = true;
            advancedDataGridView1.Columns[10].Tag = "validaNO";
            advancedDataGridView1.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // codigo sede
            advancedDataGridView1.Columns[11].Visible = false;
            advancedDataGridView1.Columns[11].HeaderText = "CODSEDE";
            advancedDataGridView1.Columns[11].Width = 100;
            advancedDataGridView1.Columns[11].ReadOnly = true;
            advancedDataGridView1.Columns[11].Tag = "validaNO";
            advancedDataGridView1.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // habilitado
            advancedDataGridView1.Columns[12].Visible = false;
            advancedDataGridView1.Columns[12].HeaderText = "HABILIT";
            advancedDataGridView1.Columns[12].Width = 100;
            advancedDataGridView1.Columns[12].ReadOnly = true;
            advancedDataGridView1.Columns[12].Tag = "validaNO";
            advancedDataGridView1.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // resto 
            advancedDataGridView1.Columns[13].Visible = false;      // a.codtipo
            advancedDataGridView1.Columns[14].Visible = false;      // dc.DescrizioneRid AS nomdoc
            advancedDataGridView1.Columns[15].Visible = false;      // a.coment
            advancedDataGridView1.Columns[16].Visible = false;      // a.fnacim
            advancedDataGridView1.Columns[17].Visible = false;      // a.fingres
            advancedDataGridView1.Columns[18].Visible = false;      // a.fcese
            advancedDataGridView1.Columns[19].Visible = false;      // a.telefono2
            advancedDataGridView1.Columns[20].Visible = false;      // a.direccion
            advancedDataGridView1.Columns[21].Visible = false;      // a.correo
        }
        private void jalainfo()                 // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nofa", nomform);
                //micon.Parameters.AddWithValue("@nofi", "proveed");
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["formulario"].ToString() == "main")
                    {
                        if (row["campo"].ToString() == "imagenes")
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
                        if (row["campo"].ToString() == "estado" && row["param"].ToString() == "anulado") vEstAnu = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        //if (row["campo"].ToString() == "documento" && row["param"].ToString() == "carreta") v_tipcarr = row["valor"].ToString().Trim();
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
            if (campo == "tx_rind")
            {
                chk_habil.Checked = (advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["bloqueado"].Value.ToString() == "1") ? true : false;
                tx_codigo.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["codigo"].Value.ToString();     // codigo empleado
                tx_dat_doc.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["tipdoc"].Value.ToString();
                cmb_doc.SelectedValue = tx_dat_doc.Text;
                tx_dni.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["numdoc"].Value.ToString();        // DNI DEL EMPLEADO
                tx_nombre.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["nombre"].Value.ToString();     // nombre p
                tx_dat_tipo.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["codtipo"].Value.ToString();   // tipo empleado
                cmb_tipo.SelectedValue = tx_dat_tipo.Text;                                                                  // 
                tx_dat_loca.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["sede"].Value.ToString();      // codigo sede
                cmb_local.SelectedValue = tx_dat_loca.Text;
                tx_brevete.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["brevete"].Value.ToString();    // brevete
                tx_telef1.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["telefono1"].Value.ToString();    // telefono 1
                tx_usersis.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["usersist"].Value.ToString();   // usuario
                tx_coment.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["coment"].Value.ToString();      // comentario
                rb_fem.Checked = (advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["genero"].Value.ToString() == "False") ? true : false;
                rb_mas.Checked = (advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells["genero"].Value.ToString() == "True") ? true : false;
            }
            if (campo == "tx_idr")
            {
                // ... no lo soo, no lo sooo
            }
        }
        public void dataload()                  // jala datos para los combos y la grilla
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    tabControl1.SelectedTab = tabreg;
                    // DATOS DEL tipo de empleado
                    cmb_tipo.Items.Clear();
                    const string contpu = "select idcodice,descrizionerid from desc_tem " +
                        "order by idcodice";
                    MySqlCommand cmbtpu = new MySqlCommand(contpu, conn);
                    DataTable dttpu = new DataTable();
                    MySqlDataAdapter datpu = new MySqlDataAdapter(cmbtpu);
                    datpu.Fill(dttpu);
                    cmb_tipo.DataSource = dttpu;
                    cmb_tipo.DisplayMember = "descrizionerid";
                    cmb_tipo.ValueMember = "idcodice";
                    // DATOS DE LA SEDE
                    cmb_local.Items.Clear();
                    const string conloc = "select idcodice,descrizionerid from desc_loc " +
                        "order by idcodice";
                    cmbtpu = new MySqlCommand(conloc, conn);
                    DataTable dtloc = new DataTable();
                    datpu = new MySqlDataAdapter(cmbtpu);
                    datpu.Fill(dtloc);
                    cmb_local.DataSource = dtloc;
                    cmb_local.DisplayMember = "descrizionerid";
                    cmb_local.ValueMember = "idcodice";
                    // DATOS DEL TIPO DE DOCUMENTO DEL TRABAJADOR
                    cmb_doc.Items.Clear();
                    const string condoc = "select idcodice,descrizionerid,descrizione,codigo,codsunat from desc_doc " +
                        "order by idcodice";
                    cmbtpu = new MySqlCommand(condoc, conn);
                    DataTable dtdoc = new DataTable();
                    datpu = new MySqlDataAdapter(cmbtpu);
                    datpu.Fill(dtdoc);
                    cmb_doc.DataSource = dtdoc;
                    cmb_doc.DisplayMember = "descrizionerid";
                    cmb_doc.ValueMember = "idcodice";
                    // datos recshum
                    string datgri = "SELECT a.id,lo.DescrizioneRid AS nomloc,a.codigo,a.genero,a.numdoc,a.nombre,te.DescrizioneRid as nomte,a.telefono1,a.usersist,a.brevete," +
                        "a.tipdoc,a.sede,a.bloqueado," +
                        "a.codtipo,dc.DescrizioneRid AS nomdoc,a.coment,a.fnacim,a.fingres,a.fcese,a.telefono2,a.direccion,a.correo " +
                        "FROM cabrrhh a " +
                        "LEFT JOIN desc_loc lo ON lo.IDCodice = a.sede " +
                        "LEFT JOIN desc_doc dc ON dc.IDCodice = a.tipdoc " +
                        "LEFT JOIN desc_tem te ON te.IDCodice = a.codtipo ";    // OJO, no debe haber where acá, esta bien así
                    MySqlCommand cdg = new MySqlCommand(datgri, conn);
                    MySqlDataAdapter dag = new MySqlDataAdapter(cdg);
                    dtg.Clear();
                    dag.Fill(dtg);
                }
            }
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
            cmb_local.SelectedIndex = -1;
            cmb_doc.SelectedIndex = -1;
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            // validamos que los campos no esten vacíos
            if (tx_codigo.Text == "")
            {
                MessageBox.Show("Ingrese el código", " Error! ");
                tx_codigo.Focus();
                return;
            }
            if (tx_dat_doc.Text == "")
            {
                MessageBox.Show("Seleccione el tipo de documento", " Error! ");
                cmb_doc.Focus();
                return;
            }
            if (tx_dni.Text == "")
            {
                MessageBox.Show("Ingrese el número de dni", " Atención ");
                tx_dni.Focus();
                return;
            }
            if (tx_nombre.Text.Trim() == "")
            {
                MessageBox.Show("Falta el nombre del empleado", " Atención ");
                tx_nombre.Focus();
                return;
            }
            if(tx_dat_tipo.Text == "")
            {
                MessageBox.Show("Seleccione el tipo de empleado", " Atención ");
                cmb_tipo.Focus();
                return;
            }
            if (tx_dat_loca.Text == "")
            {
                MessageBox.Show("Seleccione el local", " Atención ");
                cmb_local.Focus();
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
                    string consulta = "insert into cabrrhh (" +
                        "codigo,numdoc,nombre,usersist,coment,fnacim,fingres,telefono1,telefono2,direccion,correo,brevete,sede,tipdoc,codtipo,genero," +
                        "verApp,userc,fechc,diriplan4,diripwan4,nbname) " +
                        "values (@cod,@dni,@nom,@pas,@com,@fna,@fin,@te1,@te2,@dir,@cor,@bre,@sed,@tde,@tip,@gen," +
                        "@vapp,@asd,now(),@dil4,@diw4,@nbna)";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        if (lib.procConn(conn) == true)
                        {
                            using (MySqlCommand mycomand = new MySqlCommand(consulta, conn))
                            {
                                mycomand.Parameters.AddWithValue("@cod", tx_codigo.Text);
                                mycomand.Parameters.AddWithValue("@dni", tx_dni.Text);
                                mycomand.Parameters.AddWithValue("@nom", tx_nombre.Text);
                                mycomand.Parameters.AddWithValue("@pas", tx_usersis.Text);
                                mycomand.Parameters.AddWithValue("@com", tx_coment.Text);
                                mycomand.Parameters.AddWithValue("@fna", DBNull.Value);
                                mycomand.Parameters.AddWithValue("@fin", DBNull.Value);
                                mycomand.Parameters.AddWithValue("@te1", "");
                                mycomand.Parameters.AddWithValue("@te2", "");
                                mycomand.Parameters.AddWithValue("@dir", "");
                                mycomand.Parameters.AddWithValue("@cor", "");
                                mycomand.Parameters.AddWithValue("@bre", tx_brevete.Text);
                                mycomand.Parameters.AddWithValue("@sed", tx_dat_loca.Text);
                                mycomand.Parameters.AddWithValue("@tde", tx_dat_doc.Text);
                                mycomand.Parameters.AddWithValue("@tip", tx_dat_tipo.Text);
                                mycomand.Parameters.AddWithValue("@gen", (rb_fem.Checked == true) ? 0 : 1);
                                mycomand.Parameters.AddWithValue("@asd", asd);
                                mycomand.Parameters.AddWithValue("@vapp", verapp);
                                mycomand.Parameters.AddWithValue("@dil4", lib.iplan());
                                mycomand.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                                mycomand.Parameters.AddWithValue("@nbna", lib.nbname());
                                try
                                {
                                    mycomand.ExecuteNonQuery();
                                    using (MySqlCommand mycom = new MySqlCommand("select last_insert_id()", conn))
                                    {
                                        MySqlDataReader dr = mycom.ExecuteReader();
                                        string idtu = "";
                                        if (dr.Read()) idtu = dr.GetString(0);
                                        dr.Close();
                                        // insertamos en el datatable
                                        DataRow drs = dtg.NewRow();
                                        drs[0] = idtu;
                                        drs[1] = cmb_local.Text;
                                        drs[2] = tx_codigo.Text;
                                        drs[3] = (rb_fem.Checked == true) ? 0 : 1;
                                        drs[4] = tx_dni.Text;
                                        drs[5] = tx_nombre.Text;
                                        drs[6] = cmb_tipo.Text;
                                        drs[7] = tx_telef1.Text;
                                        drs[8] = tx_usersis.Text;
                                        drs[9] = tx_brevete.Text;
                                        drs[10] = tx_dat_doc.Text;
                                        drs[11] = tx_dat_loca.Text;
                                        drs[12] = (chk_habil.Checked == true) ? "1" : "0";
                                        drs[13] = tx_dat_tipo.Text;
                                        drs[14] = "";
                                        drs[15] = tx_coment.Text;
                                        dtg.Rows.Add(drs);
                                        //
                                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                                        if (resulta != "OK")                                    // actualizamos la tabla usuarios
                                        {
                                            MessageBox.Show(resulta, "Error en actualización de tabla de empleados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            Application.Exit();
                                            return;
                                        }
                                    }
                                }
                                catch (MySqlException ex)
                                {
                                    MessageBox.Show(ex.Message, "Error en ingresar empleado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    iserror = "si";
                                }
                            }
                        }
                    }
                }
            }
            if (modo == "EDITAR")
            {
                var aa = MessageBox.Show("Confirma que desea modificar?", "Atención - Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(aa == DialogResult.Yes)
                {
                    iserror = "no";
                    string consulta = "update cabrrhh set " +
                        "codigo=@cod,numdoc=@dni,nombre=@nom,usersist=@pas,coment=@com,fnacim=@fna,fingres=@fin,telefono1=@te1,telefono2=@te2,direccion=@dir," +
                        "correo=@cor,brevete=@bre,sede=@sed,tipdoc=@tde,codtipo=@tip,genero=@gen,bloqueado=@bloq," +
                        "verApp=@vapp,userm=@asd,fechm=now(),diriplan4=@dil4,diripwan4=@diw4,nbname=@nbna " +
                        "where id=@idc";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        if (lib.procConn(conn) == true)
                        {
                            MySqlCommand mycom = new MySqlCommand(consulta, conn);
                            mycom.Parameters.AddWithValue("@idc", tx_idr.Text);
                            mycom.Parameters.AddWithValue("@cod", tx_codigo.Text);
                            mycom.Parameters.AddWithValue("@dni", tx_dni.Text);
                            mycom.Parameters.AddWithValue("@nom", tx_nombre.Text);
                            mycom.Parameters.AddWithValue("@pas", tx_usersis.Text);
                            mycom.Parameters.AddWithValue("@com", tx_coment.Text);
                            mycom.Parameters.AddWithValue("@fna", DBNull.Value);
                            mycom.Parameters.AddWithValue("@fin", DBNull.Value);
                            mycom.Parameters.AddWithValue("@te1", "");
                            mycom.Parameters.AddWithValue("@te2", "");
                            mycom.Parameters.AddWithValue("@dir", "");
                            mycom.Parameters.AddWithValue("@cor", "");
                            mycom.Parameters.AddWithValue("@bre", tx_brevete.Text);
                            mycom.Parameters.AddWithValue("@sed", tx_dat_loca.Text);
                            mycom.Parameters.AddWithValue("@tde", tx_dat_doc.Text);
                            mycom.Parameters.AddWithValue("@tip", tx_dat_tipo.Text);
                            mycom.Parameters.AddWithValue("@gen", (rb_fem.Checked == true)? 0 : 1);
                            mycom.Parameters.AddWithValue("@bloq", (chk_habil.Checked == true)? 1 : 0);
                            //
                            mycom.Parameters.AddWithValue("@asd", asd);
                            mycom.Parameters.AddWithValue("@vapp", verapp);
                            mycom.Parameters.AddWithValue("@dil4", lib.iplan());
                            mycom.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                            mycom.Parameters.AddWithValue("@nbna", lib.nbname());
                            mycom.ExecuteNonQuery();
                            try
                            {
                                // actualizamos el datatable
                                for (int i = 0; i < dtg.Rows.Count; i++)
                                {
                                    DataRow row = dtg.Rows[i];
                                    if (row[0].ToString() == tx_idr.Text)
                                    {
                                        dtg.Rows[i][1] = cmb_local.Text;
                                        dtg.Rows[i][2] = tx_codigo.Text;
                                        dtg.Rows[i][3] = (rb_fem.Checked == true) ? 0 : 1;
                                        dtg.Rows[i][4] = tx_dni.Text;
                                        dtg.Rows[i][5] = tx_nombre.Text;
                                        dtg.Rows[i][6] = cmb_tipo.Text;
                                        dtg.Rows[i][7] = tx_telef1.Text;
                                        dtg.Rows[i][8] = tx_usersis.Text;
                                        dtg.Rows[i][9] = tx_brevete.Text;
                                        dtg.Rows[i][10] = tx_dat_doc.Text;
                                        dtg.Rows[i][11] = tx_dat_loca.Text;
                                        dtg.Rows[i][12] = (chk_habil.Checked == true) ? "1" : "0";
                                        dtg.Rows[i][13] = tx_dat_tipo.Text;
                                        dtg.Rows[i][14] = "";
                                        dtg.Rows[i][15] = tx_coment.Text;
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
                                MessageBox.Show(ex.Message, "Error de Editar empleado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                iserror = "si";
                            }
                        }
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
        private void tx_ruc_Leave(object sender, EventArgs e)       // valida dni
        {
            // validamos tipo de documento y número que no se repita
            if (Tx_modo.Text == "NUEVO")
            {
                if (tx_dat_doc.Text == "" || tx_dni.Text.Trim() == "")
                {
                    MessageBox.Show("Seleccione correctamente el tipo de documento y su número", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmb_doc.Focus();
                    return;
                }
                DataRow[] row = dtg.Select("tipdoc='" + tx_dat_doc.Text + "' and numdoc='" + tx_dni.Text + "'");
                if (row.Length > 0)
                {
                    MessageBox.Show("Esta repitiendo el documento");
                    tx_dni.Text = "";
                    cmb_doc.Focus();
                    return;
                }
            }
            if (Tx_modo.Text != "NUEVO" && tx_dni.Text.Trim() != "")
            {
                DataRow[] row = dtg.Select("tipdoc='" + tx_dat_doc.Text + "' and numdoc='" + tx_dni.Text + "'");
                if (row.Length > 0)
                {
                    int i = int.Parse(row[0].ItemArray[0].ToString()) - 1;
                    advancedDataGridView1.Rows[i].Selected = true;
                    DataGridViewCell cell = advancedDataGridView1.Rows[i].Cells[3];
                    //
                    advancedDataGridView1.CurrentCell = cell;
                    tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
                    tx_idr.Text = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                    jalaoc("tx_rind");
                }
            }
        }
        private void tx_placa_Leave(object sender, EventArgs e)     // valida codigo
        {
            if (Tx_modo.Text == "NUEVO")
            {
                if (tx_codigo.Text.Trim() != "")
                {
                    DataRow[] row = dtg.Select("codigo='" + tx_codigo.Text + "'");
                    if (row.Length > 0)
                    {
                        MessageBox.Show("Esta repitiendo el código");
                        tx_codigo.Text = "";
                        tx_codigo.Focus();
                        return;
                    }
                }
            }
            if (Tx_modo.Text != "NUEVO" && tx_codigo.Text.Trim() != "")
            {
                DataRow[] row = dtg.Select("codigo='" + tx_codigo.Text + "'");
                if (row.Length > 0)
                {
                    int i = int.Parse(row[0].ItemArray[0].ToString()) - 1;
                    advancedDataGridView1.Rows[i].Selected = true;
                    DataGridViewCell cell = advancedDataGridView1.Rows[i].Cells[3];
                    //
                    advancedDataGridView1.CurrentCell = cell;
                    tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
                    tx_idr.Text = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                    jalaoc("tx_rind");
                }
            }
        }
        private void tx_trackAsoc_Leave(object sender, EventArgs e)
        {

        }
        private void tabreg_Enter(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO")
            {
                if (tx_rind.Text.Trim() == "" && tx_codigo.Text.Trim() == "")
                {
                    tx_codigo.ReadOnly = false;
                }
                else
                {
                    tx_codigo.ReadOnly = true;
                }
            }
        }
        #endregion leaves;

        #region botones_de_comando_y_permisos  
        public void toolboton()
        {
            Bt_add.Visible = false;
            Bt_edit.Visible = false;
            Bt_anul.Visible = false;
            //
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
            chk_habil.Enabled = true;
            tx_codigo.ReadOnly = false;
            tx_codigo.Focus();
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
            chk_habil.Enabled = true;
            tx_codigo.ReadOnly = true;
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
            this.tx_codigo.Focus();
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
                tx_dat_tipo.Text = (string)row["idcodice"];
            }
        }
        private void cmb_local_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_local.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)cmb_local.DataSource).Rows[cmb_local.SelectedIndex];
                tx_dat_loca.Text = (string)row["idcodice"];
            }
        }
        private void cmb_doc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_doc.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)cmb_doc.DataSource).Rows[cmb_doc.SelectedIndex];
                tx_dat_doc.Text = row["idcodice"].ToString();
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
            /*
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
            */
        }
        #endregion

    }
}
