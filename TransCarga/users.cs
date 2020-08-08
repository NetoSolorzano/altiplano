using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class users : Form
    {
        static string nomform = "users"; // nombre del formulario
        string asd = TransCarga.Program.vg_user;   // usuario conectado al sistema
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "usuarios";
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
        string cn_adm = "";     // codigo nivel usuario admin
        string cn_sup = "";     // codigo nivel usuario superusuario
        string cn_est = "";     // codigo nivel usuario estandar
        string cn_mir = "";     // codigo nivel usuario solo mira
        libreria lib = new libreria();
        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";
        DataTable dtg = new DataTable();

        public users()
        {
            InitializeComponent();
        }
        private void users_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void users_Load(object sender, EventArgs e)
        {
            ToolTip toolTipNombre = new ToolTip();           // Create the ToolTip and associate with the Form container.
            // Set up the delays for the ToolTip.
            toolTipNombre.AutoPopDelay = 5000;
            toolTipNombre.InitialDelay = 1000;
            toolTipNombre.ReshowDelay = 500;
            toolTipNombre.ShowAlways = true;                 // Force the ToolTip text to be displayed whether or not the form is active.
            toolTipNombre.SetToolTip(toolStrip1, nomform);   // Set up the ToolTip text for the object
            init();
            toolboton();
            limpiar(this);
            sololee(this);
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
            this.toolStrip1.BackColor = Color.FromName(colstrp);
            this.advancedDataGridView1.BackgroundColor = Color.FromName(TransCarga.Program.colgri);
            this.tabuser.BackColor = Color.FromName(TransCarga.Program.colgri);

            jalainfo();
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            Bt_close.Image = Image.FromFile(img_btq);
            Bt_ini.Image = Image.FromFile(img_bti);
            Bt_sig.Image = Image.FromFile(img_bts);
            Bt_ret.Image = Image.FromFile(img_btr);
            Bt_fin.Image = Image.FromFile(img_btf);
        }
        private void grilla()                   // arma la grilla
        {
            Font tiplg = new Font("Arial",7, FontStyle.Bold);
            advancedDataGridView1.Font = tiplg;
            advancedDataGridView1.DefaultCellStyle.Font = tiplg;
            advancedDataGridView1.RowTemplate.Height = 15;
            advancedDataGridView1.DefaultCellStyle.BackColor = Color.MediumAquamarine;
            advancedDataGridView1.DataSource = dtg;
            /*
            // id del usuario
            advancedDataGridView1.Columns[0].Visible = false;
            // nom_user
            advancedDataGridView1.Columns[1].Visible = true;            // columna visible o no
            advancedDataGridView1.Columns[1].HeaderText = "USUARIO";    // titulo de la columna
            advancedDataGridView1.Columns[1].Width = 70;                // ancho
            advancedDataGridView1.Columns[1].ReadOnly = true;           // lectura o no
            advancedDataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // nombre del usuario
            advancedDataGridView1.Columns[2].Visible = true;       
            advancedDataGridView1.Columns[2].HeaderText = "MOMBRE";
            advancedDataGridView1.Columns[2].Width = 150;
            advancedDataGridView1.Columns[2].ReadOnly = false;          // las celdas de esta columna pueden cambiarse
            advancedDataGridView1.Columns[2].Tag = "validaNO";          // las celdas de esta columna se NO se validan
            advancedDataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // passw
            advancedDataGridView1.Columns[3].Visible = false;
            // bloqueado
            advancedDataGridView1.Columns[4].Visible = true;       
            advancedDataGridView1.Columns[4].HeaderText = "BLOQ";
            advancedDataGridView1.Columns[4].Width = 30;
            advancedDataGridView1.Columns[4].ReadOnly = true;       // no dejo cambiar aca porque no lo puedo validar
            advancedDataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // nivel
            advancedDataGridView1.Columns[5].Visible = true;       
            advancedDataGridView1.Columns[5].HeaderText = "NIVEL";
            advancedDataGridView1.Columns[5].Width = 30;
            advancedDataGridView1.Columns[5].ReadOnly = false;
            advancedDataGridView1.Columns[5].Tag = "validaSI";          // las celdas de esta columna SI se validan
            advancedDataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // tipo de usuario
            advancedDataGridView1.Columns[6].Visible = false;    
            advancedDataGridView1.Columns[6].HeaderText = "TIPO";
            advancedDataGridView1.Columns[6].Width = 60;
            advancedDataGridView1.Columns[6].ReadOnly = false;
            advancedDataGridView1.Columns[6].Tag = "validaSI";
            advancedDataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // acceso
            advancedDataGridView1.Columns[7].Visible = false;       
            advancedDataGridView1.Columns[7].HeaderText = "ACCESO";
            advancedDataGridView1.Columns[7].Width = 60;
            advancedDataGridView1.Columns[7].ReadOnly = false;
            advancedDataGridView1.Columns[7].Tag = "validaSI";
            advancedDataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // local
            advancedDataGridView1.Columns[8].Visible = true;    
            advancedDataGridView1.Columns[8].HeaderText = "LOCAL";
            advancedDataGridView1.Columns[8].Width = 60;
            advancedDataGridView1.Columns[8].ReadOnly = false;
            advancedDataGridView1.Columns[8].Tag = "validaSI";
            advancedDataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // tienda
            advancedDataGridView1.Columns[9].Visible = false;    
            advancedDataGridView1.Columns[9].HeaderText = "TIENDA";
            advancedDataGridView1.Columns[9].Width = 60;
            advancedDataGridView1.Columns[9].ReadOnly = false;
            advancedDataGridView1.Columns[9].Tag = "validaSI";
            advancedDataGridView1.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // sede
            advancedDataGridView1.Columns[10].Visible = false;    
            advancedDataGridView1.Columns[10].HeaderText = "SEDE";
            advancedDataGridView1.Columns[10].Width = 60;
            advancedDataGridView1.Columns[10].ReadOnly = false;
            advancedDataGridView1.Columns[10].Tag = "validaSI";
            advancedDataGridView1.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // ruc de la organización (una de las tres en OMG)
            advancedDataGridView1.Columns[11].Visible = true;    
            advancedDataGridView1.Columns[11].HeaderText = "RUC";
            advancedDataGridView1.Columns[11].Width = 60;
            advancedDataGridView1.Columns[11].ReadOnly = false;
            advancedDataGridView1.Columns[11].Tag = "validaSI";
            advancedDataGridView1.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // resto de columnas no visibles
            advancedDataGridView1.Columns[12].Visible = false;
            advancedDataGridView1.Columns[13].Visible = false;
            advancedDataGridView1.Columns[14].Visible = false;
            advancedDataGridView1.Columns[15].Visible = false;
            advancedDataGridView1.Columns[16].Visible = false;
            advancedDataGridView1.Columns[17].Visible = false;
            advancedDataGridView1.Columns[18].Visible = false;
            advancedDataGridView1.Columns[19].Visible = false;
            advancedDataGridView1.Columns[20].Visible = false;
            */
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
                    if(row["campo"].ToString() == "niveles")
                    {
                        if (row["param"].ToString() == "admin") cn_adm = row["valor"].ToString().Trim();            // codigo admin
                        if (row["param"].ToString() == "super") cn_sup = row["valor"].ToString().Trim();            // codigo superusuario
                        if (row["param"].ToString() == "estan") cn_est = row["valor"].ToString().Trim();            // codigo estandar
                        if (row["param"].ToString() == "miron") cn_mir = row["valor"].ToString().Trim();            // codigo solo mira
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
        public void jalaoc(string campo)        // jala datos de usuarios por id o nom_user
        {
            if (campo == "tx_idr")
            {

            }
            if (campo == "tx_corre")
            {

            }
            if(tx_rind.Text.Trim() != "")
            {
                textBox1.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[1].Value.ToString();  // usurio
                textBox2.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[3].Value.ToString();  // contraseña
                textBox3.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[2].Value.ToString();  // nombre
                textBox4.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[11].Value.ToString();  // ruc
                textBox5.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[5].Value.ToString();  // nivel
                textBox6.Text = advancedDataGridView1.Rows[int.Parse(tx_rind.Text)].Cells[8].Value.ToString();  // local
                comboBox1.SelectedValue = textBox4.Text;
                comboBox2.SelectedValue = textBox5.Text;
                comboBox3.SelectedValue = textBox6.Text;
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
            tabControl1.SelectedTab = tabuser;
            // DATOS DEL COMBOBOX1  RAZON SOCIAL
            this.comboBox1.Items.Clear();
            ComboItem citem_tpu = new ComboItem();
            const string contpu = "select descrizione,idcodice from desc_raz " +
                "where numero=1";
            MySqlCommand cmbtpu = new MySqlCommand(contpu, conn);
            DataTable dttpu = new DataTable();
            MySqlDataAdapter datpu = new MySqlDataAdapter(cmbtpu);
            datpu.Fill(dttpu);
            comboBox1.DataSource = dttpu;
            comboBox1.DisplayMember = "descrizione";
            comboBox1.ValueMember = "idcodice";
            // DATOS DEL COMBOBOX2  NIVEL DE ACCESO
            this.comboBox2.Items.Clear();
            ComboItem citem_nvu = new ComboItem();
            const string consnvu = "select descrizione,codigo from desc_niv " +
                "where numero=1";
            MySqlCommand cmd2 = new MySqlCommand(consnvu, conn);
            DataTable dt2 = new DataTable();
            MySqlDataAdapter da2 = new MySqlDataAdapter(cmd2);
            da2.Fill(dt2);
            comboBox2.DataSource = dt2;
            comboBox2.DisplayMember = "descrizione";
            comboBox2.ValueMember = "codigo";
            // DATOS DEL COMBOBOX3  LOCAL
            this.comboBox3.Items.Clear();
            ComboItem citem_sds = new ComboItem();
            const string conssed = "select descrizionerid,idcodice from desc_alm " +
                "where numero=1";
            MySqlCommand cmd3 = new MySqlCommand(conssed, conn);
            DataTable dt3 = new DataTable();
            MySqlDataAdapter da3 = new MySqlDataAdapter(cmd3);
            da3.Fill(dt3);
            comboBox3.DataSource = dt3;
            comboBox3.DisplayMember = "descrizionerid";
            comboBox3.ValueMember = "idcodice";
            // datos de usuarios
            string datgri = "select id,nom_user,nombre,pwd_user,bloqueado,nivel,tipuser,local " +
                "from usuarios";
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
        public static void limpiar(Form ofrm)
        {
            foreach (Control oControls in ofrm.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
            }
        }
        public void limpiapag(TabPage pag)
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
            checkBox1.Checked = false;
        }
        public void limpia_otros(TabPage pag)
        {
            tabControl1.SelectedTab = pag;
            this.checkBox1.Checked = false;
        }
        public void limpia_combos(TabPage pag)
        {
            tabControl1.SelectedTab = pag;
            this.comboBox1.SelectedIndex = -1;
            this.comboBox2.SelectedIndex = -1;
            this.comboBox3.SelectedIndex = -1;
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            // validamos que los campos no esten vacíos
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("El usuario no puede estar vacío", " Error! ");
                return;
            }
            if (this.textBox2.Text == "")
            {
                MessageBox.Show("La contraseña no puede estar vacía", " Error! ");
                return;
            }
            if (this.comboBox1.Text == "")
            {
                MessageBox.Show("Seleccione la organización", " Atención ");
                return;
            }
            if (this.comboBox2.Text == "")
            {
                MessageBox.Show("Seleccione el nivel de acceso", " Atención ");
                return;
            }
            if (this.comboBox3.Text == "")
            {
                MessageBox.Show("La sede del usuario no puede estar vacío", " Error! ");
                return;
            }
            // grabamos, actualizamos, etc
            string modo = this.Tx_modo.Text;
            string iserror = "no";
            string asd = TransCarga.Program.vg_user;
            string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
            if (modo == "NUEVO")
            {
                string consulta = "insert into usuarios (" +
                    "nom_user,pwd_user,nombre,nivel,bloqueado,fecha,local,ruc,verapp,userc,fechc)" +
                    " values (" +
                    "@usuario,@contra,@nombre,@niv,@bloq,date(now()),@loca,@ruc,@ver,@vguser,now())";
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlCommand mycomand = new MySqlCommand(consulta, conn);
                    mycomand.Parameters.AddWithValue("@usuario", this.textBox1.Text);
                    mycomand.Parameters.AddWithValue("@contra", lib.md5(this.textBox2.Text));
                    mycomand.Parameters.AddWithValue("@nombre", this.textBox3.Text);
                    mycomand.Parameters.AddWithValue("@niv", this.textBox5.Text);
                    mycomand.Parameters.AddWithValue("@bloq", this.checkBox1.Checked);
                    mycomand.Parameters.AddWithValue("@loca", this.textBox6.Text);
                    mycomand.Parameters.AddWithValue("@ruc", this.textBox4.Text);
                    mycomand.Parameters.AddWithValue("@ver", verapp);
                    mycomand.Parameters.AddWithValue("@vguser", asd);
                    try
                    {
                        mycomand.ExecuteNonQuery();
                        string resulta = lib.ult_mov(nomform, nomtab, asd); 
                        if (resulta != "OK")                                    // actualizamos la tabla usuarios
                        {
                            MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            return;
                        }
                        if(confper("nuevo",textBox1.Text) == false)
                        {
                            MessageBox.Show("No fue posible crear los permisos nuevos" + Environment.NewLine +
                                "deberá borrar y volver a crear este usuario"
                                , "Error en tabla de permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        // agregar al datatable y eso hará que automaticamente se agregue al datagridview1
                        // acceso,tienda,sede,mod1,mod2,mod3,priv1,priv2,derecho,aoper,foto
                        DataRow nrow = dtg.NewRow();
                        nrow["nom_user"] = textBox1.Text;
                        nrow["pwd_user"] = textBox2.Text;
                        nrow["nombre"] = textBox3.Text;
                        nrow["nivel"] = textBox5.Text;
                        nrow["bloqueado"] = checkBox1.Checked;
                        nrow["local"] = textBox6.Text;
                        nrow["ruc"] = textBox4.Text;
                        nrow["fecha"] = DateTime.Now;
                        dtg.Rows.Add(nrow);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en ingresar usuario",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
            if (modo == "EDITAR")
            {
                string parte = "";
                if (chk_res.Checked == true) parte = "pwd_user=@contra,";
                string consulta = "update usuarios set " + parte +
                        "nombre=@nombre,nivel=@niv,bloqueado=@bloq,fecha=date(now()),local=@loca,ruc=@ruc,verapp=@ver " +
                        "where nom_user=@usuario";  // falta usuario actual que se logueo
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlCommand mycom = new MySqlCommand(consulta, conn);
                    if (chk_res.Checked == true) mycom.Parameters.AddWithValue("@contra", lib.md5("123456"));
                    mycom.Parameters.AddWithValue("@nombre", textBox3.Text);
                    mycom.Parameters.AddWithValue("@niv", textBox5.Text);
                    mycom.Parameters.AddWithValue("@bloq", checkBox1.Checked);
                    mycom.Parameters.AddWithValue("@loca", textBox6.Text);
                    mycom.Parameters.AddWithValue("@ruc", textBox4.Text);
                    mycom.Parameters.AddWithValue("@ver", verapp);
                    mycom.Parameters.AddWithValue("@usuario", textBox1.Text);
                    try
                    {
                        mycom.ExecuteNonQuery();
                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                        if (resulta != "OK")                                        // actualizamos la tabla usuarios
                        {
                            MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            return;
                        }
                        if(chk_permisos.Checked == true)
                        {
                            if (confper("reini", textBox1.Text) == false)
                            {
                                MessageBox.Show("No fue posible re-inicializar los permisos del" + Environment.NewLine +
                                    "usuario, deberá hacerlo manualmente"
                                    , "Error en tabla de permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            if (confper("edita", textBox1.Text) == false)
                            {
                                MessageBox.Show("No fue posible actualizar los permisos del" + Environment.NewLine +
                                    "usuario, deberá hacerlo manualmente"
                                    , "Error en tabla de permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        // actualizamos el tdg
                        if (tx_rind.Text.Trim() != "")
                        {
                            dtg.Rows[int.Parse(tx_rind.Text)]["nombre"] = textBox3.Text;
                            dtg.Rows[int.Parse(tx_rind.Text)]["nivel"] = textBox5.Text;
                            dtg.Rows[int.Parse(tx_rind.Text)]["bloqueado"] = checkBox1.Checked;
                            dtg.Rows[int.Parse(tx_rind.Text)]["local"] = textBox6.Text;
                            dtg.Rows[int.Parse(tx_rind.Text)]["ruc"] = textBox4.Text;
                        }
                        else
                        {
                            for (int i = dtg.Rows.Count - 1; i >= 0; i--)
                            {
                                DataRow drX = dtg.Rows[i];
                                if (drX["nom_user"].ToString() == textBox1.Text.ToString())
                                {
                                    dtg.Rows[i]["nombre"] = textBox3.Text;
                                    dtg.Rows[i]["nivel"] = textBox5.Text;
                                    dtg.Rows[i]["bloqueado"] = checkBox1.Checked;
                                    dtg.Rows[i]["local"] = textBox6.Text;
                                    dtg.Rows[i]["ruc"] = textBox4.Text;
                                }
                            }
                        }
                        dtg.AcceptChanges();    //
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error de Editar usuario",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
            if (modo == "ANULAR")       // opción para borrar
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                    Application.Exit();
                    return;
                }
                string consulta = "select ul_opera from usuarios where nom_user=@cam0 and ul_opera is not null";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@cam0", textBox1.Text);
                try
                {
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Close();
                        MessageBox.Show("El usuario seleccionado no se puede borrar." + "Tiene operaciones efectuadas", " Atención ");
                        return;
                    }
                    else
                    {
                        dr.Close();
                        DialogResult drb =
                        MessageBox.Show("Confirma que desea BORRAR el usuario?", " Atención ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (drb == DialogResult.Yes)
                        {
                            consulta = "delete from usuarios where nom_user=@cam0";
                            micon = new MySqlCommand(consulta, conn);
                            micon.Parameters.AddWithValue("@cam0", textBox1.Text);
                            try
                            {
                                micon.ExecuteNonQuery();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message, "Error al ejecutar el borrado");
                                iserror = "si";
                            }
                            // eliminamos del datatable y la grilla
                            if(tx_rind.Text.Trim() != "") dtg.Rows[int.Parse(tx_rind.Text)].Delete();
                            else
                            {
                                for (int i = dtg.Rows.Count - 1; i >= 0; i--)
                                {
                                    DataRow drX = dtg.Rows[i];
                                    if (drX["nom_user"].ToString() == textBox1.Text.ToString()) drX.Delete();
                                }
                            }
                            dtg.AcceptChanges();    // al borrar el dtg automaticamente se borra en la grilla porque es su datasource
                            // ahora borramos sus permisos
                            consulta = "delete from permisos where usuario=@cam0";
                            micon = new MySqlCommand(consulta, conn);
                            micon.Parameters.AddWithValue("@cam0", textBox1.Text);
                            try
                            {
                                micon.ExecuteNonQuery();
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(ex.Message, "Error al ejecutar el borrado de permisos");
                                iserror = "si";
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error de Acceso al borrar");
                    iserror = "si";
                }
                conn.Close();
            }
            if (iserror == "no")
            {
                // debe limpiar los campos y actualizar la grilla
                tabControl1.SelectedTab = tabuser;
                limpia_combos(tabuser);
                limpiapag(tabuser);
                limpia_otros(tabuser);
                textBox1.Focus();
                //dataload();
            }
        }
        #endregion boton_form;

        #region leaves
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                //string aca = tx_idr.Text;
                //limpia_chk();
                //limpia_combos();
                //limpiar(this);
                //tx_idr.Text = aca;
                jalaoc("tx_idr");               // jalamos los datos del registro
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            /*  validamos segun el modo
            if (textBox1.Text != "" && Tx_modo.Text=="NUEVO")
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                    Application.Exit();
                    return;
                }
                string consulta = "select count(nom_user) as cant from usuarios where nom_user=@usuario";
                MySqlCommand mycomand = new MySqlCommand(consulta, conn);
                mycomand.Parameters.AddWithValue("@usuario", this.textBox1.Text);
                int cant = System.Convert.ToInt16(mycomand.ExecuteScalar());
                if (cant > 0)
                {
                    MessageBox.Show("Usuario YA existe!", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    this.textBox1.Text = "";
                    return;        
                }
                conn.Close();
            }
            if (textBox1.Text != "" && Tx_modo.Text != "NUEVO")
            {
                DataRow[] linea = dtg.Select("nom_user like '%" + textBox1.Text + "%'");
                foreach(DataRow row in linea)
                {
                    textBox2.Text = row[1].ToString();
                    textBox3.Text = row[2].ToString();
                }
                
            }
            */
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
            tabControl1.SelectedTab = tabuser;
            escribe(this);
            this.Tx_modo.Text = "NUEVO";
            this.button1.Image = Image.FromFile(img_grab);
            this.textBox1.Focus();
            limpiar(this);
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            chk_res.Enabled = false;
            chk_permisos.Enabled = false;
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
            tabControl1.SelectedTab = tabuser;
            escribe(this);
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            //textBox1.Focus();
            limpiar(this);
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            chk_res.Enabled = true;
            chk_permisos.Enabled = true;
            //textBox1.Text = codu;
            //tx_idr.Text = idr;
            jalaoc("tx_idr");
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
            chk_res.Enabled = false;
            chk_permisos.Enabled = false;
            this.textBox1.Focus();
            //limpiar(this);
            //totfilgrid = dataGridView1.Rows.Count - 1;
            //printPreviewDialog1.Document = printDocument1;
            //printPreviewDialog1.ShowDialog();
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
            tabControl1.SelectedTab = tabuser;
            escribe(this);
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            //textBox1.Focus();
            limpiar(this);
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            chk_res.Enabled = true;
            chk_permisos.Enabled = false;
            //textBox1.Text = codu;
            //tx_idr.Text = idr;
            jalaoc("tx_idr");
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            //--
            tx_idr.Text = lib.gofirts(nomtab);
            tx_idr_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            string aca = tx_idr.Text;
            limpia_chk();
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            limpiar(this);
            //--
            tx_idr.Text = lib.goback(nomtab, aca);
            tx_idr_Leave(null, null);
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            string aca = tx_idr.Text;
            limpia_chk();
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            limpiar(this);
            //--
            tx_idr.Text = lib.gonext(nomtab, aca);
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpiapag(tabuser);
            limpia_otros(tabuser);
            limpia_combos(tabuser);
            //--
            tx_idr.Text = lib.golast(nomtab);
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // permisos para habilitar los botones de comando
        private void permisos()
        {
            string consulta = "select formulario,nivel,coment,btn1,btn2,btn3,btn4,btn5,btn6 from setupform";
            DataTable dt = new DataTable();
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                    da.Fill(dt);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error de conexión a setupform", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
            }
            string bot1 = "N";
            string bot2 = "N";
            string bot3 = "N";
            string bot4 = "N";
            string bot5 = "N";
            string bot6 = "S";
            string com = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow fil = dt.Rows[i];
                if (fil[1].ToString() == "0")
                { // usuarios de sistemas, acceso total a todo
                    if (tx_tpu_flag.Text == "0")
                    {
                        bot1 = "S";
                        bot2 = "S";
                        bot3 = "S";
                        bot4 = "S";
                        bot5 = "S";
                    }
                }
                if (fil[1].ToString() == "1")
                {   // usuario directivo, acceso de usuario avanzado
                    if (tx_tpu_flag.Text == "0")
                    {
                        bot1 = "S";
                        bot2 = "S";
                        bot3 = "S";
                        bot4 = "S";
                        bot5 = "S";
                    }
                    if (tx_tpu_flag.Text == "1" && tx_nvu_flag.Text == "1")
                    {
                        bot1 = "S";
                        bot2 = "S";
                        bot3 = "S";
                        bot4 = "S";
                        bot5 = "S";
                    }
                    if (tx_tpu_flag.Text == "1" && tx_nvu_flag.Text == "2")
                    {
                        bot1 = "S";
                        bot2 = "S";
                        bot3 = "N"; // ANULA
                        bot4 = "S";
                        bot5 = "S";
                    }
                }
                if (fil[1].ToString() == "2")
                {   // usuario secretarias, usuario normal
                    if (tx_tpu_flag.Text == "2" && tx_nvu_flag.Text == "2")
                    {
                        bot1 = "S";
                        bot2 = "S";
                        bot3 = "N";
                        bot4 = "S";
                        bot5 = "S";
                    }
                    if (tx_tpu_flag.Text == "2" && tx_nvu_flag.Text == "3")
                    {
                        bot1 = "N";
                        bot2 = "N";
                        bot3 = "N";
                        bot4 = "S";
                        bot5 = "S";
                    }
                }
                com = fil[2].ToString();    // comentario - descripcion del form

                if (Tx_modo.Text == "NUEVO")
                {
                    consulta = "insert into permisos (" +
                        "formulario,btn1,btn2,btn3,btn4,btn5,btn6,usuario,coment) values (" +
                        "@for,@bt1,@bt2,@bt3,@bt4,@bt5,@bt6,@use,@com)";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@for", fil[0].ToString());
                    micon.Parameters.AddWithValue("@bt1", bot1);
                    micon.Parameters.AddWithValue("@bt2", bot2);
                    micon.Parameters.AddWithValue("@bt3", bot3);
                    micon.Parameters.AddWithValue("@bt4", bot4);
                    micon.Parameters.AddWithValue("@bt5", bot5);
                    micon.Parameters.AddWithValue("@bt6", bot6);
                    micon.Parameters.AddWithValue("@use", textBox1.Text);
                    micon.Parameters.AddWithValue("@com", com);
                    try
                    {
                        micon.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en insertar permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                }
                //
                if (Tx_modo.Text == "EDITAR")
                {
                    consulta = "update permisos set btn1=@bt1,btn2=@bt2,btn3=@bt3,btn4=@bt4,btn5=@bt5,btn6=@bt6 " +
                        "where usuario=@use and formulario=@for";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@bt1", bot1);
                    micon.Parameters.AddWithValue("@bt2", bot2);
                    micon.Parameters.AddWithValue("@bt3", bot3);
                    micon.Parameters.AddWithValue("@bt4", bot4);
                    micon.Parameters.AddWithValue("@bt5", bot5);
                    micon.Parameters.AddWithValue("@bt6", bot6);
                    micon.Parameters.AddWithValue("@use", textBox1.Text);
                    micon.Parameters.AddWithValue("@for", fil[0].ToString());
                    try
                    {
                        micon.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en actualizar permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                }
            }
            conn.Close();
        }
        // configurador de permisos
        private bool confper(string tarea, string user)
        {
            bool retorna = false;

            string consulta = "select formulario,nivel,coment,btn1,btn2,btn3,btn4,btn5,btn6,rutaf from setupform";
            DataTable dt = new DataTable();
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(consulta, conn);
                    da.Fill(dt);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error de conexión a setupform", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return retorna;
                }
            }
            switch (tarea)
            {
                case "nuevo":
                    string pedaso = "";
                    string tuadm = ",'S','S','S','S','S','S',";   // administrador, todo de todo
                    string tusup = ",'S','S','S','S','S','S',";   // superusuario, todo menos config del sist.
                    string tuest = ",'S','S','N','S','S','S',";   // estandar, todo menos anular y panel de control
                    string tusmi = ",'N','N','N','S','S','S',";   // solo mira
                    if (textBox5.Text == cn_adm) pedaso = tuadm;       // administrador del sistema 
                    if (textBox5.Text == cn_sup) pedaso = tusup;       // super usuario 
                    if (textBox5.Text == cn_est) pedaso = tuest;       // usuario estandar
                    if (textBox5.Text == cn_mir) pedaso = tusmi;       // solo mira
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow fil = dt.Rows[i];
                        {
                            string inserta = "insert into permisos (" +
                                "formulario,btn1,btn2,btn3,btn4,btn5,btn6,usuario,coment,rutaf) values ('" +
                                fil[0].ToString().Trim() + "'" + pedaso + "'"+ textBox1.Text.Trim() + "','" + fil[2].ToString() + "','"+ fil[9].ToString() + "')";
                            MySqlCommand minser = new MySqlCommand(inserta, conn);
                            minser.ExecuteNonQuery();
                        }
                    }
                    retorna = true;
                    break;
                case "edita":
                    string parte = "";
                    tuadm = "btn1='S',btn2='S',btn3='S',btn4='S',btn5='S' ";   // administrador, todo de todo
                    tusup = "btn1='S',btn2='S',btn3='S',btn4='S',btn5='S' ";   // superusuario, todo menos config del sist.
                    tuest = "btn1='S',btn2='S',btn3='N',btn4='S',btn5='S' ";   // estandar, todo menos anular y panel de control
                    tusmi = "btn1='N',btn2='N',btn3='N',btn4='S',btn5='S' ";   // solo mira
                    if (textBox5.Text == cn_adm) parte = tuadm;       // administrador del sistema 
                    if (textBox5.Text == cn_sup) parte = tusup;       // superusuario
                    if (textBox5.Text == cn_est) parte = tuest;       // estandar
                    if (textBox5.Text == cn_mir) parte = tusmi;       // solo mira
                    consulta = "update permisos set " + parte +
                        "where usuario='" + textBox1.Text.Trim() + "'";   //  and formulario=@for
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.ExecuteNonQuery();
                    retorna = true;
                    break;
                case "reini":
                    string parter = "";
                    string rtuadm = ",'S','S','S','S','S','S',";   // administrador, todo de todo
                    string rtusup = ",'S','S','S','S','S','S',";   // superusuario, todo menos config del sist.
                    string rtuest = ",'S','S','N','S','S','S',";   // estandar, todo menos anular y panel de control
                    string rtusmi = ",'N','N','N','S','S','S',";   // solo mira
                    if (textBox5.Text == cn_adm) parter = rtuadm;       // administrador del sistema 
                    if (textBox5.Text == cn_sup) parter = rtusup;       // superusuario
                    if (textBox5.Text == cn_est) parter = rtuest;       // estandar
                    if (textBox5.Text == cn_mir) parter = rtusmi;       // solo mira
                    consulta = "delete from permisos where usuario='" + textBox1.Text.Trim() + "'";
                    micon = new MySqlCommand(consulta, conn);
                    micon.ExecuteNonQuery();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow fil = dt.Rows[i];
                        {
                            string inserta = "insert into permisos (" +
                                "formulario,btn1,btn2,btn3,btn4,btn5,btn6,usuario,coment,rutaf) values ('" +
                                fil[0].ToString().Trim() + "'" + parter + "'" + textBox1.Text.Trim() + "','" + fil[2].ToString() + "','" + fil[9].ToString() + "')";
                            MySqlCommand minser = new MySqlCommand(inserta, conn);
                            minser.ExecuteNonQuery();
                        }
                    }
                    retorna = true;
                    break;
            }
            conn.Close();
            return retorna;
        }
        #endregion botones_de_comando_y_permisos  ;

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
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)     // nivel de acceso
        {
            if(comboBox2.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)comboBox2.DataSource).Rows[comboBox2.SelectedIndex];
                textBox5.Text = (string)row["codigo"];
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)     // local del usuario
        {
            if(comboBox3.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)comboBox3.DataSource).Rows[comboBox3.SelectedIndex];
                textBox6.Text = (string)row["idcodice"];
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
                tabControl1.SelectedTab = tabuser;
                limpiar(this);
                limpiapag(tabuser);
                limpia_otros(tabuser);
                limpia_combos(tabuser);
                idr = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
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
    }
}
