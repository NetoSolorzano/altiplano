using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class preguiat : Form
    {
        static string nomform = "preguiat";             // nombre del formulario
        string asd = TransCarga.Program.vg_user;        // usuario conectado al sistema
        string colback = TransCarga.Program.colbac;     // color de fondo
        string colpage = TransCarga.Program.colpag;     // color de los pageframes
        string colgrid = TransCarga.Program.colgri;     // color de las grillas
        string colstrp = TransCarga.Program.colstr;     // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabpregr";              // cabecera de pre guias
        public int totfilgrid, cta;      // variables para impresion
        string img_btN = "";
        string img_btE = "";
        string img_btA = "";            // anula = bloquea
        string img_btP = "";            // imprime
        string img_btV = "";            // visualiza
        string img_bti = "";            // imagen boton inicio
        string img_bts = "";            // imagen boton siguiente
        string img_btr = "";            // imagen boton regresa
        string img_btf = "";            // imagen boton final
        string img_btq = "";
        string img_grab = "";
        string img_anul = "";
        string vtc_dni = "";            // variable tipo cliente natural
        string vtc_ruc = "";            // variable tipo cliente empresa
        string vtc_ext = "";            // variable tipo cliente extranjero
        static libreria lib = new libreria();
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        //
        AutoCompleteStringCollection departamentos = new AutoCompleteStringCollection();// autocompletado departamentos
        AutoCompleteStringCollection provincias = new AutoCompleteStringCollection();   // autocompletado provincias
        AutoCompleteStringCollection distritos = new AutoCompleteStringCollection();    // autocompletado distritos

        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";
        DataTable dtu = new DataTable();

        public preguiat()
        {
            InitializeComponent();
        }
        private void preguiat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Alt && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Alt && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Alt && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Alt && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Alt && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Alt && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void preguiat_Load(object sender, EventArgs e)
        {
            /*
            ToolTip toolTipNombre = new ToolTip();           // Create the ToolTip and associate with the Form container.
            // Set up the delays for the ToolTip.
            toolTipNombre.AutoPopDelay = 5000;
            toolTipNombre.InitialDelay = 1000;
            toolTipNombre.ReshowDelay = 500;
            toolTipNombre.ShowAlways = true;                 // Force the ToolTip text to be displayed whether or not the form is active.
            toolTipNombre.SetToolTip(toolStrip1, nomform);   // Set up the ToolTip text for the object
            */
            jalainfo();
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            autodepa();                                     // autocompleta departamentos
            autoprov();                                     // autocompleta provincias
            autodist();                                     // autocompleta distritos
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            tx_user.Text += asd;
            tx_nomuser.Text = lib.nomuser(asd);
            tx_locuser.Text += lib.locuser(asd);
            tx_fechact.Text = DateTime.Today.ToString();
            //
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            Bt_ver.Image = Image.FromFile(img_btV);
            Bt_print.Image = Image.FromFile(img_btP);
            Bt_close.Image = Image.FromFile(img_btq);
            Bt_ini.Image = Image.FromFile(img_bti);
            Bt_sig.Image = Image.FromFile(img_bts);
            Bt_ret.Image = Image.FromFile(img_btr);
            Bt_fin.Image = Image.FromFile(img_btf);
            // autocompletados
            tx_dptoO.AutoCompleteMode = AutoCompleteMode.Suggest;           // departamentos
            tx_dptoO.AutoCompleteSource = AutoCompleteSource.CustomSource;  // departamentos
            tx_dptoO.AutoCompleteCustomSource = departamentos;              // departamentos
            tx_provO.AutoCompleteMode = AutoCompleteMode.Suggest;           // provincias
            tx_provO.AutoCompleteSource = AutoCompleteSource.CustomSource;  // provincias
            tx_provO.AutoCompleteCustomSource = provincias;                 // provincias
            tx_distO.AutoCompleteMode = AutoCompleteMode.Suggest;           // distritos
            tx_distO.AutoCompleteSource = AutoCompleteSource.CustomSource;  // distritos
            tx_distO.AutoCompleteCustomSource = distritos;                  // distritos
            // longitudes maximas de campos
            tx_nomRem.MaxLength = 100;           // nombre
            tx_dirRem.MaxLength = 100;           // direccion
            tx_ubigO.MaxLength = 6;            // ubigeo
            // .....
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nofa", nomform);
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
                        if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        if (row["param"].ToString() == "img_btV") img_btV = row["valor"].ToString().Trim();         // imagen del boton de accion visualizar
                        // boton de vista preliminar .... esta por verse su utlidad
                        if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                        if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                        if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                        if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                        if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                        if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                    }
                    if (row["formulario"].ToString() == nomform && row["campo"].ToString() == "documento")
                    {
                        if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
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
        public void jalaoc(string campo)        // 
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select .....";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        tx_dat_tdRem.Text = dr.GetString("tipdoc");
                        tx_numDocRem.Text = dr.GetString("RUC");
                        tx_nomRem.Text = dr.GetString("RazonSocial");
                        tx_dirRem.Text = dr.GetString("Direcc1").Trim() + " " + dr.GetString("Direcc2").Trim();
                        tx_dptoO.Text = dr.GetString("depart");
                        tx_provO.Text = dr.GetString("Provincia");
                        tx_distO.Text = dr.GetString("Localidad");
                        tx_ubigO.Text = dr.GetString("ubigeo");
                        // ..... falta arreglar esto
                        cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
                    }
                    //
                    dr.Dispose();
                    micon.Dispose();
                }
                conn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Fatal");
                Application.Exit();
                return;
            }
        }
        public void dataload()                  // jala datos para los combos 
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            //  datos para el combobox de tipo de documento
            cmb_docRem.Items.Clear();
            string datuse = "select idcodice,descrizionerid,codigo from desc_doc where numero=@bloq";
            MySqlCommand cdu = new MySqlCommand(datuse, conn);
            cdu.Parameters.AddWithValue("@bloq", 1);
            MySqlDataAdapter dacu = new MySqlDataAdapter(cdu);
            dtu.Clear();
            dacu.Fill(dtu);
            cmb_docRem.DataSource = dtu;
            cmb_docRem.DisplayMember = "descrizionerid";
            cmb_docRem.ValueMember = "idcodice";
            //
            dacu.Dispose();
            conn.Close();
        }

        #region autocompletados
        private void autodepa()                 // se jala en el load
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                string consulta = "select nombre from ubigeos where depart<>'00' and provin='00' and distri='00'";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                try
                {
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.HasRows == true)
                    {
                        while (dr.Read())
                        {
                            departamentos.Add(dr["nombre"].ToString());
                        }
                    }
                    dr.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en obtener relación de departamentos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
                conn.Close();
            }
            else
            {
                MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void autoprov()                 // se jala despues de ingresado el departamento
        {
            if (tx_ubigO.Text.Trim() != "")
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select nombre from ubigeos where depart=@dep and provin<>'00' and distri='00'";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@dep", tx_ubigO.Text.Substring(0, 2));
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                provincias.Add(dr["nombre"].ToString());
                            }
                        }
                        dr.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en obtener relación de provincias", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private void autodist()                 // se jala despues de ingresado la provincia
        {
            if (tx_ubigO.Text.Trim() != "" && tx_provO.Text.Trim() != "")
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select nombre from ubigeos where depart=@dep and provin=@prov and distri<>'00'";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@dep", tx_ubigO.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@prov", (tx_ubigO.Text.Length > 2)? tx_ubigO.Text.Substring(2, 2):"  ");
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                distritos.Add(dr["nombre"].ToString());
                            }
                        }
                        dr.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en obtener relación de distritos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        #endregion autocompletados

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
        public void limpia_chk()    
        {
            //
        }
        public void limpia_otros()
        {
            //this.checkBox1.Checked = false;
        }
        public void limpia_combos()
        {
            cmb_docRem.SelectedIndex = -1;
        }
        public void limpiapag(TabPage pag)
        {
            foreach (Control oControls in pag.Controls)
            {
                if (oControls is TextBox)
                {
                    oControls.Text = "";
                }
                if(oControls is CheckBox)
                {
                    //checkBox1.Checked = false;
                }
                if(oControls is ComboBox)
                {
                    cmb_docRem.SelectedIndex = -1;
                }
            }
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            if (tx_dat_tdRem.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de documento", " Error! ");
                tx_dat_tdRem.Focus();
                return;
            }
            if (tx_numDocRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de documento", " Error! ");
                tx_numDocRem.Focus();
                return;
            }
            if (tx_nomRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el nombre o razón social", " Error! ");
                tx_nomRem.Focus();
                return;
            }
            if (tx_dirRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la dirección", " Error! ");
                tx_dirRem.Focus();
                return;
            }
            if (tx_ubigO.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese ubigeo correcto", " Error! ");
                tx_ubigO.Focus();
                return;
            }
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear la guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            // 
                        }
                    }
                    else
                    {
                        tx_numDocRem.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Los datos no son nuevos", "Verifique duplicidad", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            if (modo == "EDITAR")
            {
                if(tx_dptoO.Text.Trim() == "")
                {
                    tx_dptoO.Focus();
                    return;
                }
                if (tx_provO.Text.Trim() == "")
                {
                    tx_provO.Focus();
                    return;
                }
                if (tx_distO.Text.Trim() == "")
                {
                    tx_distO.Focus();
                    return;
                }
                if (tx_ubigO.Text.Length < 6)
                {
                    MessageBox.Show("Falta información de ubigeo o es incorrecta", "Confirme dpto, prov. o distrito");
                    tx_provO.Focus();
                    return;
                }
                var aa = MessageBox.Show("Confirma que desea modificar la guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    edita();
                    //
                }
                else
                {
                    tx_dat_tdRem.Focus();
                    return;
                }
            }
            if (modo == "ANULAR")       // opción para borrar
            { 
                // 
            }
            if (iserror == "no")
            {
                // debe limpiar los campos y actualizar la grilla
                limpiar(this);
                limpia_chk();
                limpia_otros();
                limpia_combos();
                //textBox5.Focus();
            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                try
                {
                    string inserta = "insert into ";
                    MySqlCommand micon = new MySqlCommand(inserta, conn);
                    micon.Parameters.AddWithValue("@tidoc", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@nudoc", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@raso", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dir1", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@dir2", (tx_dirRem.Text.Trim().Length > 50) ? tx_dirRem.Text.Substring(50, (tx_dirRem.Text.Trim().Length - 50)) : "");
                    micon.Parameters.AddWithValue("@depa", tx_dptoO.Text);
                    micon.Parameters.AddWithValue("@prov", tx_provO.Text);
                    micon.Parameters.AddWithValue("@dist", tx_distO.Text);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", Program.vg_user);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.Parameters.AddWithValue("@cate", "FOR");
                    micon.ExecuteNonQuery();
                    //
                    string lectura = "select last_insert_id()";
                    micon = new MySqlCommand(lectura, conn);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        tx_idr.Text = dr.GetString(0);
                        retorna = true;
                    }
                    dr.Close();
                }
                catch(MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en insertar cliente");
                    Application.Exit();
                    return retorna;
                }
            }
            else
            {
                MessageBox.Show("No fue posible conectarse al servidor de datos");
                Application.Exit();
                return retorna;
            }
            conn.Close();
            return retorna;
        }
        private void edita()
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    string inserta = "update ";
                    MySqlCommand micon = new MySqlCommand(inserta, conn);
                    micon.Parameters.AddWithValue("@tidoc", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@nudoc", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@raso", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dir1", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@dir2", (tx_dirRem.Text.Trim().Length > 50) ? tx_dirRem.Text.Substring(50, (tx_dirRem.Text.Trim().Length - 50)) : "");
                    micon.Parameters.AddWithValue("@depa", tx_dptoO.Text);
                    micon.Parameters.AddWithValue("@prov", tx_provO.Text);
                    micon.Parameters.AddWithValue("@dist", tx_distO.Text);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", Program.vg_user);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);

                    micon.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar el proveedor");
                    Application.Exit();
                    return;
                }
            }
            else
            {
                MessageBox.Show("No fue posible conectarse al servidor de datos");
                Application.Exit();
                return;
            }
            conn.Close();
        }
        #endregion boton_form;

        #region leaves
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                jalaoc("tx_idr");               // no usamos grilla en este form
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento, jala provincia
        {
            if(tx_dptoO.Text != "" && TransCarga.Program.vg_conSol == false)
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select depart from ubigeos where trim(nombre)=@dep and depart<>'00' and provin='00' and distri='00'";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@dep", tx_dptoO.Text.Trim());
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                tx_ubigO.Text = dr.GetString(0).Trim();
                            }
                        }
                        dr.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en obtener codigo de departamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                autoprov();
            }
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia de un departamento, jala distrito
        {
            if(tx_provO.Text != "" && tx_dptoO.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select provin from ubigeos where trim(nombre)=@prov and depart=@dep and provin<>'00' and distri='00'";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@dep", tx_ubigO.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@prov", tx_provO.Text.Trim());
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                if (tx_ubigO.Text.Trim().Length == 6) tx_ubigO.Text = tx_ubigO.Text.Substring(0,2) + dr.GetString(0).Trim() + tx_ubigO.Text.Substring(4, 2);
                                if (tx_ubigO.Text.Trim().Length < 6) tx_ubigO.Text = tx_ubigO.Text.Substring(0, 2) + dr.GetString(0).Trim();
                            }
                        }
                        dr.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en obtener codigo de provincia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                autodist();
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)
        {
            if(tx_distO.Text.Trim() != "" && tx_provO.Text.Trim() != "" && tx_dptoO.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select distri from ubigeos where trim(nombre)=@dist and depart=@dep and provin=@prov and distri<>'00'";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@dep", tx_ubigO.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@prov", (tx_ubigO.Text.Length > 2)? tx_ubigO.Text.Substring(2, 2):"  ");
                    micon.Parameters.AddWithValue("@dist", tx_distO.Text.Trim());
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                if(tx_ubigO.Text.Trim().Length >= 4) tx_ubigO.Text = tx_ubigO.Text.Trim().Substring(0,4) + dr.GetString(0).Trim();
                            }
                        }
                        dr.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en obtener codigo de distrito", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private void textBox13_Leave(object sender, EventArgs e)        // ubigeo
        {
            if(tx_ubigO.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select d.nombre,b.nombre,c.nombre from ubigeos a " +
                        "left join ubigeos b on concat(b.depart, b.provin)= concat(a.depart, a.provin) and b.distri = '00' " +
                        "left join ubigeos c on concat(c.depart, c.provin, c.distri)= concat(a.depart, a.provin, a.distri) " +
                        "left join (select nombre, depart from ubigeos where depart<>'00' and provin = '00' and distri = '00')d " +
                        "on d.depart = a.depart " +
                        "where concat(a.depart, a.provin, a.distri)=@ubi";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@ubi", tx_ubigO.Text);
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                tx_dptoO.Text = dr.GetString(0).Trim();
                                tx_provO.Text = dr.GetString(1).Trim();
                                tx_distO.Text = dr.GetString(2).Trim();
                            }
                        }
                        dr.Close();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error en obtener codigo de distrito", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        private void textBox3_Leave(object sender, EventArgs e)         // número de documento
        {
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() != "")
            {
                if (tx_numDocRem.Text.Trim().Length != Int16.Parse(tx_mld.Text))
                {
                    MessageBox.Show("El número de caracteres para" + Environment.NewLine +
                        "su tipo de documento debe ser: " + tx_mld.Text, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_numDocRem.Focus();
                    return;
                }
                string encuentra = "no";
                if (Tx_modo.Text == "NUEVO")
                {
                    if (string.IsNullOrEmpty(lib.nomsn("FOR", tx_dat_tdRem.Text, tx_numDocRem.Text)))
                    {
                        if (tx_dat_tdRem.Text == vtc_ruc)
                        {
                            if (lib.valiruc(tx_numDocRem.Text, vtc_ruc) == false)
                            {
                                MessageBox.Show("Número de RUC inválido", "Atención - revise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                tx_numDocRem.Focus();
                                return;
                            }
                            if (encuentra == "no")
                            {
                                if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                                {
                                    string[] rl = lib.conectorSolorsoft("RUC", tx_numDocRem.Text);
                                    tx_nomRem.Text = rl[0];      // razon social
                                    tx_ubigO.Text = rl[1];     // ubigeo
                                    tx_dirRem.Text = rl[2];      // direccion
                                    tx_dptoO.Text = rl[3];      // departamento
                                    tx_provO.Text = rl[4];      // provincia
                                    tx_distO.Text = rl[5];      // distrito
                                }
                            }
                        }
                        if (tx_dat_tdRem.Text == vtc_dni)
                        {
                            if (encuentra == "no")
                            {
                                if (TransCarga.Program.vg_conSol == true) // conector solorsoft para dni
                                {
                                    string[] rl = lib.conectorSolorsoft("DNI", tx_numDocRem.Text);
                                    tx_nomRem.Text = rl[0];      // nombre
                                    tx_numDocRem.Text = rl[1];     // num dni
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ya existe el proveedor!", "Atención corrija", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        tx_numDocRem.Text = "";
                        //
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(lib.nomsn("FOR", tx_dat_tdRem.Text, tx_numDocRem.Text)))
                    {
                        MessageBox.Show("El cliente no existe!", "Atención corrija", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        tx_numDocRem.Text = "";
                        //
                    }
                    else
                    {
                        try
                        {
                            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                            conn.Open();
                            if (conn.State == ConnectionState.Open)
                            {
                                string consulta = "select tipdoc,RUC,RazonSocial,Direcc1,Direcc2,depart,Provincia,Localidad,NumeroTel1,NumeroTel2,EMail,pais,ubigeo," +
                                    "codigo,estado,idcategoria,id " +
                                    "from anag_for where tipdoc=@tdo and ruc=@ndo";
                                MySqlCommand micon = new MySqlCommand(consulta, conn);
                                micon.Parameters.AddWithValue("@tdo", tx_dat_tdRem.Text);
                                micon.Parameters.AddWithValue("@ndo", tx_numDocRem.Text);
                                MySqlDataReader dr = micon.ExecuteReader();
                                if (dr.Read())
                                {
                                    tx_idr.Text = dr.GetString("id");
                                    tx_nomRem.Text = dr.GetString("RazonSocial");
                                    tx_dirRem.Text = dr.GetString("Direcc1").Trim() + " " + dr.GetString("Direcc2").Trim();
                                    tx_dptoO.Text = dr.GetString("depart");
                                    tx_provO.Text = dr.GetString("Provincia");
                                    tx_distO.Text = dr.GetString("Localidad");
                                    tx_ubigO.Text = dr.GetString("ubigeo");
                                    //
                                    cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
                                }
                                //
                                dr.Dispose();
                                micon.Dispose();
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error Fatal");
                            Application.Exit();
                            return;
                        }
                    }
                }
            }
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() == "")
            {
                cmb_docRem.Focus();
            }
        }
        private void comboBox1_Leave(object sender, EventArgs e)
        {
            tx_numDocRem.Text = "";
            tx_numDocRem.Focus();
        }
        #endregion leaves;

        #region botones_de_comando_y_proveed  
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
            escribe(this);
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            limpiar(this);
            limpia_otros();
            limpia_combos();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            //string codu = "";
            //string idr = "";
            escribe(this);
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            limpiar(this);
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
            sololee(this);
            this.Tx_modo.Text = "IMPRIMIR";
            this.button1.Image = Image.FromFile("print48");
        }
        private void Bt_anul_Click(object sender, EventArgs e)          // pone todos los proveed en N
        {
            // no se anula, solo bloquea
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = lib.gofirts("anag_for");    // nomtab
            tx_idr_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            if(tx_idr.Text.Trim() != "")
            {
                int aca = int.Parse(tx_idr.Text) - 1;
                limpiar(this);
                limpia_chk();
                limpia_combos();
                limpia_otros();
                tx_idr.Text = aca.ToString();
                tx_idr_Leave(null, null);
            }
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            int aca = int.Parse(tx_idr.Text) + 1;
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = aca.ToString();
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = lib.golast("anag_for");     // nomtab
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // proveed para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        // selected index del combobox de usuarios
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_docRem.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)cmb_docRem.DataSource).Rows[cmb_docRem.SelectedIndex];
                tx_dat_tdRem.Text = (string)row["idcodice"];
                tx_mld.Text = (string)row["codigo"];
            }
            else
            {
                tx_dat_tdRem.Text = "";
            }
        }
        #endregion comboboxes
    }
}
