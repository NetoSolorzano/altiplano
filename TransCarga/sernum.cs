using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class sernum : Form
    {
        static string nomform = "sernum";               // nombre del formulario
        string asd = TransCarga.Program.vg_user;        // usuario conectado al sistema
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color fondo sin grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "series";
        public int totfilgrid, cta;      // variables para impresion
        public string perAg = "";
        public string perMo = "";
        public string perAn = "";
        public string perIm = "";
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
        AutoCompleteStringCollection forimp = new AutoCompleteStringCollection();
        libreria lib = new libreria();
        publico lp = new publico(); 
        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";
        DataTable dtg = new DataTable();

        public sernum()
        {
            InitializeComponent();
        }
        private void sernum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void sernum_Load(object sender, EventArgs e)
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
            limpiar();
            sololee();
            dataload();
            grilla();
            this.KeyPreview = true;
            //Bt_add_Click(null, null);
            tabControl1.SelectedTab = tabgrilla;
            advancedDataGridView1.Enabled = false;
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            advancedDataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            advancedDataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            advancedDataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            advancedDataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
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
            textBox1.MaxLength = 4;
            textBox2.MaxLength = 8;
            textBox10.MaxLength = 8;
            textBox3.MaxLength = 99;
            textBox6.MaxLength = 2;
            textBox7.MaxLength = 90;
            textBox8.MaxLength = 90;
            textBox9.MaxLength = 6;         // ubigeo
            // autocompletado
            textBox6.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox6.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox6.AutoCompleteCustomSource = forimp;
        }
        private void grilla()                   // arma la grilla
        {
            //id,rsocial,tipdoc,serie,inicial,actual,final,coment,status,userc,fechc,userm,fechm,usera,fecha,
            //sede,destino,format,zona,glosaser,imp_ini,imp_fec,imp_det,imp_dtr,imp_pie,dir_pe,ubigeo
            Font tiplg = new Font("Arial",7, FontStyle.Bold);
            advancedDataGridView1.Font = tiplg;
            advancedDataGridView1.DefaultCellStyle.Font = tiplg;
            advancedDataGridView1.RowTemplate.Height = 15;
            //advancedDataGridView1.DefaultCellStyle.BackColor = Color.MediumAquamarine;
            advancedDataGridView1.DataSource = dtg;
            // id 
            advancedDataGridView1.Columns[0].Visible = false;
            // rsocial
            advancedDataGridView1.Columns[1].Visible = true;            // columna visible o no
            advancedDataGridView1.Columns[1].HeaderText = "Organización";    // titulo de la columna
            advancedDataGridView1.Columns[1].Width = 70;                // ancho
            advancedDataGridView1.Columns[1].ReadOnly = true;           // lectura o no
            advancedDataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // tipdoc
            advancedDataGridView1.Columns[2].Visible = true;       
            advancedDataGridView1.Columns[2].HeaderText = "Tip.Doc.";
            advancedDataGridView1.Columns[2].Width = 70;
            advancedDataGridView1.Columns[2].ReadOnly = true;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[2].Tag = "validaSI";          // las celdas de esta columna se NO se validan
            advancedDataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // serie
            advancedDataGridView1.Columns[3].Visible = true;
            advancedDataGridView1.Columns[3].HeaderText = "Serie";
            advancedDataGridView1.Columns[3].Width = 50;
            advancedDataGridView1.Columns[3].ReadOnly = false;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[3].Tag = "validaNO";          // las celdas de esta columna se validan
            advancedDataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // inicial
            advancedDataGridView1.Columns[4].Visible = false;
            // actual
            advancedDataGridView1.Columns[5].Visible = true;
            advancedDataGridView1.Columns[5].HeaderText = "#Actual";
            advancedDataGridView1.Columns[5].Width = 70;
            advancedDataGridView1.Columns[5].ReadOnly = false;
            advancedDataGridView1.Columns[5].Tag = "validaNO";          // las celdas de esta columna se NO se validan
            advancedDataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // final
            advancedDataGridView1.Columns[6].Visible = false;
            // Comentario
            advancedDataGridView1.Columns[7].Visible = true;       
            advancedDataGridView1.Columns[7].HeaderText = "Comentario";
            advancedDataGridView1.Columns[7].Width = 100;
            advancedDataGridView1.Columns[7].ReadOnly = false;
            advancedDataGridView1.Columns[7].Tag = "validaNO";          // las celdas de esta columna SI se validan
            advancedDataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // invisibles
            advancedDataGridView1.Columns[8].Visible = false;           // status
            advancedDataGridView1.Columns[9].Visible = false;           // userc
            advancedDataGridView1.Columns[10].Visible = false;          // fechc
            advancedDataGridView1.Columns[11].Visible = false;          // userm
            advancedDataGridView1.Columns[12].Visible = false;          // fechm
            advancedDataGridView1.Columns[13].Visible = false;          // usera
            advancedDataGridView1.Columns[14].Visible = false;          // fecha
            // sede
            advancedDataGridView1.Columns[15].Visible = true;    
            advancedDataGridView1.Columns[15].HeaderText = "sede";
            advancedDataGridView1.Columns[15].Width = 50;
            advancedDataGridView1.Columns[15].ReadOnly = true;
            advancedDataGridView1.Columns[15].Tag = "validaSI";
            advancedDataGridView1.Columns[15].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // destino
            advancedDataGridView1.Columns[16].Visible = false;
            // format
            advancedDataGridView1.Columns[17].Visible = true;
            advancedDataGridView1.Columns[17].HeaderText = "Formato";
            advancedDataGridView1.Columns[17].Width = 50;
            advancedDataGridView1.Columns[17].ReadOnly = true;
            advancedDataGridView1.Columns[17].Tag = "validaNO";
            advancedDataGridView1.Columns[17].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // codigo de zona
            advancedDataGridView1.Columns[18].Visible = true;
            advancedDataGridView1.Columns[18].HeaderText = "Zona";
            advancedDataGridView1.Columns[18].Width = 60;
            advancedDataGridView1.Columns[18].ReadOnly = false;
            advancedDataGridView1.Columns[18].Tag = "validaSI";
            advancedDataGridView1.Columns[18].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // glosaser
            advancedDataGridView1.Columns[19].Visible = true;
            advancedDataGridView1.Columns[19].HeaderText = "Glosa";
            advancedDataGridView1.Columns[19].Width = 50;
            advancedDataGridView1.Columns[19].ReadOnly = true;
            advancedDataGridView1.Columns[19].Tag = "validaNO";
            advancedDataGridView1.Columns[19].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // invisibles
            advancedDataGridView1.Columns[20].Visible = false;           // imp_ini
            advancedDataGridView1.Columns[21].Visible = false;           // imp_fec
            advancedDataGridView1.Columns[22].Visible = false;          // imp_det
            advancedDataGridView1.Columns[23].Visible = false;          // imp_dtr
            advancedDataGridView1.Columns[24].Visible = false;          // imp_pie
            // dir_pe
            advancedDataGridView1.Columns[25].Visible = true;
            advancedDataGridView1.Columns[25].HeaderText = "Direc.Pto.Emisión";
            advancedDataGridView1.Columns[25].Width = 100;
            advancedDataGridView1.Columns[25].ReadOnly = true;
            advancedDataGridView1.Columns[25].Tag = "validaNO";
            advancedDataGridView1.Columns[25].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // ubigeo
            advancedDataGridView1.Columns[26].Visible = false;  
            advancedDataGridView1.Columns[26].HeaderText = "Ubigeo";
            advancedDataGridView1.Columns[26].Width = 60;
            advancedDataGridView1.Columns[26].ReadOnly = false;
            advancedDataGridView1.Columns[26].Tag = "validaSI";
            advancedDataGridView1.Columns[26].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // codigos de tipo de documento y sede
            advancedDataGridView1.Columns[27].Visible = false;           // a.tipdoc
            advancedDataGridView1.Columns[28].Visible = false;           // a.sede
        }
        private void jalainfo()                 // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select campo,param,valor from enlaces where formulario=@nofo";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");   // nomform
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["campo"].ToString() == "imagenes")
                    {
                        if (row["param"].ToString() == "img_btN") img_btN = row["valor"].ToString().Trim();         // imagen del boton de accion NUEVO
                        if (row["param"].ToString() == "img_btE") img_btE = row["valor"].ToString().Trim();         // imagen del boton de accion EDITAR
                        if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                        if (row["param"].ToString() == "img_btQ") img_btq = row["valor"].ToString().Trim();         // imagen del boton de accion SALIR
                        //if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        // boton de vista preliminar .... esta por verse su utlidad
                        if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                        if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                        if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                        if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                        if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                        if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                    }
                    if (row["campo"].ToString() == "estado" && row["param"].ToString() == "anulado") vEstAnu = row["valor"].ToString().Trim();
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
        public void jalaoc(string campo)        // jala datos de definiciones
        {
            textBox4.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[1].Value.ToString();  // rsocial
            comboBox1.SelectedValue = textBox4.Text;
            textCmb2.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[27].Value.ToString();  // tipdoc
            comboBox2.SelectedValue = textCmb2.Text;
            textBox1.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[3].Value.ToString();  // serie
            textBox2.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[5].Value.ToString();  // actual
            textBox3.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[7].Value.ToString();   // coment
            textBox5.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[28].Value.ToString();   // sede
            comboBox3.SelectedValue = textBox5.Text;
            textBox6.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[17].Value.ToString();   // format
            textBox7.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[19].Value.ToString();   // glosaser
            textBox8.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[25].Value.ToString();   // dir_pe
            textBox9.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[26].Value.ToString();   // ubigeo
            textBox10.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[6].Value.ToString();   // final
            tx_dat_zdes.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[18].Value.ToString();   // codigo zona
            cmb_zdes.SelectedValue = tx_dat_zdes.Text;
            checkBox1.Checked = (advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[8].Value.ToString() != vEstAnu) ? true : false;
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
            // DATOS DEL COMBOBOX1  Razón social
            comboBox1.Items.Clear();
            const string contpu = "select idcodice,descrizione from descrittive " +
                "where idtabella='RAZ' order by idcodice";
            MySqlCommand micom = new MySqlCommand(contpu, conn);
            DataTable dttpu = new DataTable();
            using (MySqlDataAdapter datpu = new MySqlDataAdapter(micom))
            {
                datpu.Fill(dttpu);
                comboBox1.DataSource = dttpu;
                comboBox1.DisplayMember = "descrizione";
                comboBox1.ValueMember = "idcodice";
            }
            // DATOS DEL COMBOBOX2  tipo documento
            comboBox2.Items.Clear();
            const string selcmb2 = "select idcodice,descrizione from descrittive " +
                "where idtabella='TDV' order by idcodice";
            micom = new MySqlCommand(selcmb2, conn);
            DataTable dtcmb2 = new DataTable();
            using (MySqlDataAdapter dacmb2 = new MySqlDataAdapter(micom))
            {
                dacmb2.Fill(dtcmb2);
                comboBox2.DataSource = dtcmb2;
                comboBox2.DisplayMember = "descrizione";
                comboBox2.ValueMember = "idcodice";
            }
            // DATOS DEL COMBOBOX3   
            comboBox3.Items.Clear();
            const string selcmb3 = "select idcodice,descrizione from descrittive " +
                "where idtabella='LOC' order by idcodice";
            micom = new MySqlCommand(selcmb3, conn);
            DataTable dtcmb3 = new DataTable();
            using (MySqlDataAdapter dacmb3 = new MySqlDataAdapter(micom))
            {
                dacmb3.Fill(dtcmb3);
                comboBox3.DataSource = dtcmb3;
                comboBox3.DisplayMember = "descrizione";
                comboBox3.ValueMember = "idcodice";
            }
            // datos combo zona
            cmb_zdes.Items.Clear();
            string conzona = "select idcodice,descrizione from descrittive " +
                "where idtabella='ZON' order by idcodice";
            micom = new MySqlCommand(conzona, conn);
            DataTable dtzona = new DataTable();
            using (MySqlDataAdapter dazona = new MySqlDataAdapter(micom))
            {
                dazona.Fill(dtzona);
                cmb_zdes.DataSource = dtzona;
                cmb_zdes.DisplayMember = "descrizione";
                cmb_zdes.ValueMember = "idcodice";
            }
            // datos de los formatos de impresion
            autoforimp();
            // datos de las series
            string datgri = "select a.id,a.rsocial,c.descrizionerid,a.serie,a.inicial,a.actual,a.final,a.coment,a.status," +
                "a.userc,a.fechc,a.userm,a.fechm,a.usera,a.fecha,b.descrizionerid,a.destino,a.format,a.zona,a.glosaser," +
                "a.imp_ini,a.imp_fec,a.imp_det,a.imp_dtr,a.imp_pie,a.dir_pe,a.ubigeo,a.tipdoc,a.sede " +
                "from series a " +
                "left join desc_loc b on b.idcodice=a.sede " +
                "left join desc_tdv c on c.idcodice=a.tipdoc " +
                "order by a.sede,a.tipdoc,a.serie";
            MySqlCommand cdg = new MySqlCommand(datgri, conn);
            using (MySqlDataAdapter dag = new MySqlDataAdapter(cdg))
            {
                dtg.Clear();
                dag.Fill(dtg);
            }
            micom.Dispose();
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
                case "Zona":
                    retorna[0] = "desc_zon";
                    retorna[1] = "idcodice";
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
        public void sololee()
        {
            lp.sololee(this);
        }
        public void escribe()
        {
            lp.escribe(this);
        }
        private void limpiar()
        {
            lp.limpiar(this);
        }
        private void limpiaPag(TabPage pag)
        {
            lp.limpiapag(pag);
        }
        public void limpia_chk()    
        {
            lp.limpia_chk(this);
        }
        public void limpia_otros()
        {
            //checkBox1.Checked = false;
        }
        public void limpia_combos()
        {
            lp.limpia_cmb(this);
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            // validamos que los campos no esten vacíos
            if (textBox1.Text == "")
            {
                MessageBox.Show("Ingrese la Serie", " Error! ");
                textBox1.Focus();
                return;
            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Ingrese la numeración actual", " Error! ");
                textBox2.Focus();
                return;
            }
            if (textBox4.Text == "")
            {
                MessageBox.Show("Seleccione la organiación", " Atención ");
                comboBox1.Focus();
                return;
            }
            if(textCmb2.Text == "")
            {
                MessageBox.Show("Seleccione el tipo de Doc.", " Atención ");
                comboBox2.Focus();
                return;
            }
            if(textBox5.Text == "")
            {
                MessageBox.Show("Seleccione el Local o sede", " Atención ");
                comboBox3.Focus();
                return;
            }
            if (textBox9.Text == "")
            {
                MessageBox.Show("Ingrese el Ubigeo de la dirección", " Atención ");
                textBox9.Focus();
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
                    string consulta = "insert into series (rsocial,tipdoc,serie,actual,coment,sede,format,glosaser,dir_pe,ubigeo," +
                        "inicial,final,userc,fechc,verApp,diriplan4,diripwan4,status,zona)" +
                        " values (@raz,@tip,@ser,@act,@com,@sed,@for,@glo,@dir,@ubi," +
                        "@nini,@nfin,@asd,now(),@vapp,@dil4,@diw4,@est,@zona)";
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        MySqlCommand mycomand = new MySqlCommand(consulta, conn);
                        mycomand.Parameters.AddWithValue("@raz", textBox4.Text);
                        mycomand.Parameters.AddWithValue("@tip", textCmb2.Text);
                        mycomand.Parameters.AddWithValue("@ser", textBox1.Text);
                        mycomand.Parameters.AddWithValue("@act", textBox2.Text);
                        mycomand.Parameters.AddWithValue("@com", textBox3.Text);
                        mycomand.Parameters.AddWithValue("@sed", textBox5.Text);
                        mycomand.Parameters.AddWithValue("@for", textBox6.Text);
                        mycomand.Parameters.AddWithValue("@glo", textBox7.Text);
                        mycomand.Parameters.AddWithValue("@est", (checkBox1.Checked == true) ? "" : vEstAnu);
                        mycomand.Parameters.AddWithValue("@dir", textBox8.Text);
                        mycomand.Parameters.AddWithValue("@ubi", textBox9.Text);
                        mycomand.Parameters.AddWithValue("@nini", textBox2.Text);
                        mycomand.Parameters.AddWithValue("@nfin", textBox10.Text);
                        mycomand.Parameters.AddWithValue("@zona", tx_dat_zdes.Text);
                        mycomand.Parameters.AddWithValue("@asd", asd);
                        mycomand.Parameters.AddWithValue("@vapp", verapp);
                        mycomand.Parameters.AddWithValue("@dil4", lib.iplan());
                        mycomand.Parameters.AddWithValue("@diw4", lib.ipwan());
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
                            //id,rsocial,tipdoc,serie,inicial,actual,final,coment,status,userc,fechc,userm,fechm,usera,fecha,
                            //sede,destino,format,zona,glosaser,imp_ini,imp_fec,imp_det,imp_dtr,imp_pie,dir_pe,ubigeo
                            drs[0] = idtu;
                            drs[1] = textBox4.Text;
                            drs[2] = comboBox2.Text;
                            drs[3] = textBox1.Text;
                            drs[4] = textBox2.Text;
                            drs[5] = textBox2.Text;
                            drs[6] = textBox10.Text;
                            drs[7] = textBox3.Text;
                            drs[8] = (checkBox1.Checked == true) ? "" : vEstAnu;
                            drs[15] = comboBox3.Text;
                            drs[17] = textBox6.Text;
                            drs[18] = tx_dat_zdes.Text;
                            drs[19] = textBox7.Text;
                            drs[25] = textBox8.Text;
                            drs[26] = textBox9.Text;
                            drs[27] = textCmb2.Text;
                            drs[28] = textBox5.Text;
                            dtg.Rows.Add(drs);
                            //
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")                                    // actualizamos la tabla usuarios
                            {
                                MessageBox.Show(resulta, "Error en actualización de tabla definiciones", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                                return;
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.Message, "Error en ingresar definición", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            iserror = "si";
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
                    string consulta = "update series set " +
                            "rsocial=@raz,tipdoc=@tip,serie=@ser,actual=@act,coment=@com,sede=@sed,format=@for,glosaser=@glo,status=@est," +
                            "dir_pe=@dir,ubigeo=@ubi,final=@nfin,userm=@asd,fechm=now(),verApp=@vapp,diriplan4=@dil4,diripwan4=@diw4,zona=@zona " +
                            "where id=@idc";
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        MySqlCommand mycom = new MySqlCommand(consulta, conn);
                        mycom.Parameters.AddWithValue("@idc", tx_idr.Text);
                        mycom.Parameters.AddWithValue("@raz", textBox4.Text);
                        mycom.Parameters.AddWithValue("@tip", textCmb2.Text);
                        mycom.Parameters.AddWithValue("@ser", textBox1.Text);
                        mycom.Parameters.AddWithValue("@act", textBox2.Text);
                        mycom.Parameters.AddWithValue("@com", textBox3.Text);
                        mycom.Parameters.AddWithValue("@sed", textBox5.Text);
                        mycom.Parameters.AddWithValue("@for", textBox6.Text);
                        mycom.Parameters.AddWithValue("@glo", textBox7.Text);
                        mycom.Parameters.AddWithValue("@est", (checkBox1.Checked == true) ? "": vEstAnu);
                        mycom.Parameters.AddWithValue("@dir", textBox8.Text);
                        mycom.Parameters.AddWithValue("@ubi", textBox9.Text);
                        mycom.Parameters.AddWithValue("@nfin", textBox10.Text);
                        mycom.Parameters.AddWithValue("@zona", tx_dat_zdes.Text);
                        mycom.Parameters.AddWithValue("@asd", asd);
                        mycom.Parameters.AddWithValue("@vapp", verapp);
                        mycom.Parameters.AddWithValue("@dil4", lib.iplan());
                        mycom.Parameters.AddWithValue("@diw4", lib.ipwan());
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
                                    //id,rsocial,tipdoc,serie,inicial,actual,final,coment,status,userc,fechc,userm,fechm,usera,fecha,
                                    //sede,destino,format,zona,glosaser,imp_ini,imp_fec,imp_det,imp_dtr,imp_pie,dir_pe,ubigeo
                                    dtg.Rows[i][1] = textBox4.Text;
                                    dtg.Rows[i][2] = comboBox2.Text;
                                    dtg.Rows[i][3] = textBox1.Text;
                                    dtg.Rows[i][4] = textBox2.Text;
                                    dtg.Rows[i][5] = textBox2.Text;
                                    dtg.Rows[i][6] = textBox10.Text;
                                    dtg.Rows[i][7] = textBox3.Text;
                                    dtg.Rows[i][8] = (checkBox1.Checked == true) ? "" : vEstAnu;
                                    dtg.Rows[i][15] = comboBox3.Text;
                                    dtg.Rows[i][17] = textBox6.Text;
                                    dtg.Rows[i][18] = tx_dat_zdes.Text;
                                    dtg.Rows[i][19] = textBox7.Text;
                                    dtg.Rows[i][25] = textBox8.Text;
                                    dtg.Rows[i][26] = textBox9.Text;
                                    dtg.Rows[i][27] = textCmb2.Text;
                                    dtg.Rows[i][28] = textBox5.Text;
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
                            MessageBox.Show(ex.Message, "Error de Editar definición", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            iserror = "si";
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
                limpiar();
                limpiaPag(tabreg);
                limpia_otros();
                limpia_chk();
                limpia_combos();
                //dataload();
            }
        }
        #endregion boton_form;

        #region leaves
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                jalaoc("tx_idr");               // jalamos los datos del registro
            }
        }
        private void ubigeo_Leave(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    String consulta = "select count(id) from ubigeos where concat(depart,provin,distri) = @cod";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@cod", textBox9.Text);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        if(dr.GetInt16(0) < 1)
                        {
                            MessageBox.Show("Código de ubigeo NO existe!", "Error en ingreso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            textBox9.Text = "";
                            textBox9.Focus();
                        }
                    }
                    dr.Close();
                    micon.Dispose();
                    conn.Close();
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error fatal");
                Application.Exit();
                return;
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
            escribe();
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            textBox1.Focus();
            limpiar();
            limpia_otros();
            limpia_combos();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.Enabled = true;
            string codu = "";
            string idr = "";
            if (advancedDataGridView1.CurrentRow.Index > -1)
            {
                codu = advancedDataGridView1.CurrentRow.Cells[1].Value.ToString();
                idr = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
            }
            tabControl1.SelectedTab = tabgrilla;
            escribe();
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            limpiar();
            limpia_otros();
            limpia_combos();
            jalaoc("tx_idr");
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            sololee();
            this.Tx_modo.Text = "IMPRIMIR";
            this.button1.Image = Image.FromFile("print48");
            this.textBox1.Focus();
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.Enabled = true;
            string codu = "";
            string idr = "";
            if (advancedDataGridView1.CurrentRow.Index > -1)
            {
                codu = advancedDataGridView1.CurrentRow.Cells[1].Value.ToString();
                idr = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
            }
            tabControl1.SelectedTab = tabreg;
            escribe();
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            limpiar();
            limpia_otros();
            limpia_combos();
            jalaoc("tx_idr");
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            //--
            tx_idr.Text = lib.gofirts(nomtab);
            tx_idr_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            string aca = tx_idr.Text;
            limpia_chk();
            limpia_combos();
            limpiar();
            //--
            tx_idr.Text = lib.goback(nomtab, aca);
            tx_idr_Leave(null, null);
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            string aca = tx_idr.Text;
            limpia_chk();
            limpia_combos();
            limpiar();
            //--
            tx_idr.Text = lib.gonext(nomtab, aca);
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            //--
            tx_idr.Text = lib.golast(nomtab);
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // permisos para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)     // razon social
        {
            if(comboBox1.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)comboBox1.DataSource).Rows[comboBox1.SelectedIndex];
                textBox4.Text = (string)row["idcodice"];
                //int Id = (int)row["idcodice"];
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)comboBox2.DataSource).Rows[comboBox2.SelectedIndex];
                textCmb2.Text = (string)row["idcodice"];
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)comboBox3.DataSource).Rows[comboBox3.SelectedIndex];
                textBox5.Text = (string)row["idcodice"];
            }
        }
        private void cmb_zdes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_zdes.SelectedIndex > -1)
            {
                tx_dat_zdes.Text = cmb_zdes.SelectedValue.ToString();
            }
        }
        #endregion comboboxes

        #region advancedatagridview
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)                  // filtro de las columnas
        {
            dtg.DefaultView.RowFilter = advancedDataGridView1.FilterString;
        }
        private void advancedDataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)            // almacena valor previo al ingresar a la celda
        {
            advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }
        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 1)
            {
                //string codu = "";
                string idr = "";
                idr = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
                tabControl1.SelectedTab = tabreg;
                limpiar();
                limpia_otros();
                limpia_combos();
                tx_idr.Text = idr;
                jalaoc("tx_idr");
            }
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

        #region autocompletados
        private void autoforimp()
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                string consulta = "select distinct descrizionerid from desc_fim order by descrizionerid asc";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                try
                {
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.HasRows == true)
                    {
                        while (dr.Read())
                        {
                            forimp.Add(dr["descrizionerid"].ToString());
                        }
                    }
                    dr.Close();
                    micon.Dispose();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en obtener unidades", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
                finally { conn.Close(); }
            }
        }
        #endregion autocompletados
    }
}
