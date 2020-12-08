using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class clients : Form
    {
        static string nomform = "clients"; // nombre del formulario
        string asd = TransCarga.Program.vg_user;   // usuario conectado al sistema
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;  // usa conector solorsoft? true=si; false=no
        static string nomtab = "anagrafiche";   // idcategoria='CLI' -> vista anag_cli
        public int totfilgrid, cta;      // variables para impresion
        public string perAg = "";
        public string perMo = "";
        public string perAn = "";
        public string perIm = "";
        string img_btN = "";
        string img_btE = "";
        string img_btA = "";            // anula = bloquea
        string img_bti = "";            // imagen boton inicio
        string img_bts = "";            // imagen boton siguiente
        string img_btr = "";            // imagen boton regresa
        string img_btf = "";            // imagen boton final
        string img_btq = "";
        string img_grab = "";
        string img_anul = "";
        string vapadef = "";            // variable pais por defecto para los clientes
        string vtc_dni = "";
        string vtc_ruc = "";
        static libreria lib = new libreria();
        DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string iplan = lib.iplan();
        string ipwan = TransCarga.Program.vg_ipwan;
        //
        AutoCompleteStringCollection paises = new AutoCompleteStringCollection();       // autocompletado paises
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
        DataTable dtg = new DataTable();
        DataTable dtu = new DataTable();

        public clients()
        {
            InitializeComponent();
        }
        private void clients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            //if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void clients_Load(object sender, EventArgs e)
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
            limpiar(this);
            sololee(this);
            this.KeyPreview = true;
            Bt_add.Enabled = true;
            Bt_anul.Enabled = true;
            comboBox1.SelectedIndex = -1;
            autopais();                                     // autocompleta paises
            autodepa();                                     // autocompleta departamentos
            autoprov();                                     // autocompleta provincias
            autodist();                                     // autocompleta distritos
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            this.toolStrip1.BackColor = Color.FromName(colstrp);
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            Bt_close.Image = Image.FromFile(img_btq);
            Bt_ini.Image = Image.FromFile(img_bti);
            Bt_sig.Image = Image.FromFile(img_bts);
            Bt_ret.Image = Image.FromFile(img_btr);
            Bt_fin.Image = Image.FromFile(img_btf);
            // autocompletados
            textBox5.AutoCompleteMode = AutoCompleteMode.Suggest;           // paises
            textBox5.AutoCompleteSource = AutoCompleteSource.CustomSource;  // paises
            textBox5.AutoCompleteCustomSource = paises;                     // paises
            textBox7.AutoCompleteMode = AutoCompleteMode.Suggest;           // departamentos
            textBox7.AutoCompleteSource = AutoCompleteSource.CustomSource;  // departamentos
            textBox7.AutoCompleteCustomSource = departamentos;              // departamentos
            textBox8.AutoCompleteMode = AutoCompleteMode.Suggest;           // provincias
            textBox8.AutoCompleteSource = AutoCompleteSource.CustomSource;  // provincias
            textBox8.AutoCompleteCustomSource = provincias;                 // provincias
            textBox9.AutoCompleteMode = AutoCompleteMode.Suggest;           // distritos
            textBox9.AutoCompleteSource = AutoCompleteSource.CustomSource;  // distritos
            textBox9.AutoCompleteCustomSource = distritos;                  // distritos
            // longitudes maximas de campos
            textBox5.MaxLength = 3;           // pais
            textBox5.CharacterCasing = CharacterCasing.Upper;
            textBox4.MaxLength = 100;           // nombre
            textBox6.MaxLength = 100;           // direccion
            textBox13.MaxLength = 6;            // ubigeo
            textBox10.MaxLength = 15;           // telef. 1
            textBox11.MaxLength = 15;           // telef. 2
            textBox12.MaxLength = 50;          // correo electr.
            // 
            textBox13.ReadOnly = true;          // ubigeos, no se escribe
        }
        private void jalainfo()                 // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");   // nomform
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
                        //if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        // boton de vista preliminar .... esta por verse su utlidad
                        if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                        if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                        if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                        if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                        if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                        if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                    }
                    if (row["formulario"].ToString() == "main" && row["campo"].ToString() == "pais" && row["param"].ToString() == "default")
                    {
                        vapadef = row["valor"].ToString().Trim();            // pais por defecto
                    }
                    if (row["formulario"].ToString() == nomform && row["campo"].ToString() == "documento")
                    {
                        if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
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
        public void jalaoc(string campo)        // en este form no hay
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select tipdoc,RUC,RazonSocial,Direcc1,Direcc2,depart,Provincia,Localidad,NumeroTel1,NumeroTel2,EMail,pais,ubigeo," +
                        "codigo,estado,idcategoria,id " +
                        "from anag_cli where id=@ida";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        checkBox1.Checked = (dr.GetInt16("estado") == 0) ? false : true;
                        textBox5.Text = dr.GetString("pais");
                        textBox1.Text = dr.GetString("id");
                        textBox2.Text = dr.GetString("tipdoc");
                        textBox3.Text = dr.GetString("RUC");
                        textBox4.Text = dr.GetString("RazonSocial");
                        textBox6.Text = dr.GetString("Direcc1").Trim() + " " + dr.GetString("Direcc2").Trim();
                        textBox7.Text = dr.GetString("depart");
                        textBox8.Text = dr.GetString("Provincia");
                        textBox9.Text = dr.GetString("Localidad");
                        textBox13.Text = dr.GetString("ubigeo");
                        textBox10.Text = dr.GetString("NumeroTel1");
                        textBox11.Text = dr.GetString("NumeroTel2");
                        textBox12.Text = dr.GetString("EMail");
                        //
                        comboBox1.SelectedValue = textBox2.Text;
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
            //  datos para el combobox de tipo de documento
            comboBox1.Items.Clear();
            string datuse = "select idcodice,descrizionerid,codigo from desc_doc where numero=@bloq";
            MySqlCommand cdu = new MySqlCommand(datuse, conn);
            cdu.Parameters.AddWithValue("@bloq", 1);
            MySqlDataAdapter dacu = new MySqlDataAdapter(cdu);
            dtu.Clear();
            dacu.Fill(dtu);
            comboBox1.DataSource = dtu;
            comboBox1.DisplayMember = "descrizionerid";
            comboBox1.ValueMember = "idcodice";
            //
            dacu.Dispose();
            conn.Close();
        }

        #region autocompletados
        private void autopais()
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                string consulta = "select distinct descrizionerid from desc_pai order by descrizionerid asc";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                try
                {
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.HasRows == true)
                    {
                        while (dr.Read())
                        {
                            paises.Add(dr["descrizionerid"].ToString());
                        }
                    }
                    dr.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en obtener relación de paises", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
                conn.Close();
            }
            else
            {
                MessageBox.Show("No se puede conectar al servidor!", "Error de conectividad",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }
        private void autodepa()                 // se jala en el load
        {
            DataRow[] depar = dataUbig.Select("depart<>'00' and provin='00' and distri='00'");
            departamentos.Clear();
            foreach (DataRow row in depar)
            {
                departamentos.Add(row["nombre"].ToString());
            }
        }
        private void autoprov()                 // se jala despues de ingresado el departamento
        {
            if (textBox13.Text.Length > 1)
            {
                DataRow[] provi = null;
                provi = dataUbig.Select("depart='" + textBox13.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                provincias.Clear();
                foreach (DataRow row in provi)
                {
                    provincias.Add(row["nombre"].ToString());
                }
            }
        }
        private void autodist()                 // se jala despues de ingresado la provincia
        {
            if (textBox13.Text.Trim() != "" && textBox8.Text.Trim() != "")
            {
                DataRow[] distr = null;
                distr = dataUbig.Select("depart='" + textBox13.Text.Substring(0, 2) + "' and provin='" + textBox13.Text.Substring(2, 2) + "' and distri<>'00'");
                distritos.Clear();
                foreach (DataRow row in distr)
                {
                    distritos.Add(row["nombre"].ToString());
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
            textBox13.ReadOnly = true;
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
            checkBox1.Checked = false;
        }
        public void limpia_otros()
        {
            //this.checkBox1.Checked = false;
        }
        public void limpia_combos()
        {
            comboBox1.SelectedIndex = -1;
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
                    checkBox1.Checked = false;
                }
                if(oControls is ComboBox)
                {
                    comboBox1.SelectedIndex = -1;
                }
            }
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de documento", " Error! ");
                textBox2.Focus();
                return;
            }
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de documento", " Error! ");
                textBox3.Focus();
                return;
            }
            if (textBox4.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el nombre o razón social", " Error! ");
                textBox4.Focus();
                return;
            }
            if (textBox5.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el nombre el país de origen", " Error! ");
                textBox5.Focus();
                return;
            }
            if (textBox6.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la dirección", " Error! ");
                textBox6.Focus();
                return;
            }
            if (textBox13.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese ubigeo correcto", " Error! ");
                textBox13.Focus();
                return;
            }
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear al cliente?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            // 
                        }
                    }
                    else
                    {
                        textBox1.Focus();
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
                if(textBox7.Text.Trim() == "")
                {
                    textBox7.Focus();
                    return;
                }
                if (textBox8.Text.Trim() == "")
                {
                    textBox8.Focus();
                    return;
                }
                if (textBox9.Text.Trim() == "")
                {
                    textBox9.Focus();
                    return;
                }
                if (textBox13.Text.Length < 6)
                {
                    MessageBox.Show("Falta información de ubigeo o es incorrecta", "Confirme dpto, prov. o distrito");
                    textBox8.Focus();
                    return;
                }
                var aa = MessageBox.Show("Confirma que desea modificar el cliente?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    if (textBox5.Text.Trim() == "") textBox5.Text = vapadef;
                    edita();
                    //
                }
                else
                {
                    textBox1.Focus();
                    return;
                }
            }
            if (modo == "ANULAR")       // opción para borrar
            { 
                // 
            }
            if (iserror == "no")
            {
                string resulta = lib.ult_mov(nomform, nomtab, asd);
                if (resulta != "OK")                                        // actualizamos la tabla usuarios
                {
                    MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // debe limpiar los campos y actualizar la grilla
                limpiar(this);
                limpia_chk();
                limpia_otros();
                limpia_combos();
                textBox5.Focus();
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
                    string inserta = "insert into anagrafiche (" +
                        "tipdoc,RUC,RazonSocial,Direcc1,Direcc2,depart,Provincia,Localidad,NumeroTel1,NumeroTel2,EMail,pais,ubigeo,codigo,estado,idcategoria," +
                        "verApp,userc,fechc,diriplan4,diripwan4,nbname) " +
                        "values (@tidoc,@nudoc,@raso,@dir1,@dir2,@depa,@prov,@dist,@tel1,@tel2,@mail,@pais,@ubig,@codi,@bloq,@cate," +
                        "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                    MySqlCommand micon = new MySqlCommand(inserta, conn);
                    micon.Parameters.AddWithValue("@tidoc", textBox2.Text);
                    micon.Parameters.AddWithValue("@nudoc", textBox3.Text);
                    micon.Parameters.AddWithValue("@raso", textBox4.Text);
                    micon.Parameters.AddWithValue("@dir1", textBox6.Text);
                    micon.Parameters.AddWithValue("@dir2", (textBox6.Text.Trim().Length > 50) ? textBox6.Text.Substring(50, (textBox6.Text.Trim().Length - 50)) : "");
                    micon.Parameters.AddWithValue("@depa", textBox7.Text);
                    micon.Parameters.AddWithValue("@prov", textBox8.Text);
                    micon.Parameters.AddWithValue("@dist", textBox9.Text);
                    micon.Parameters.AddWithValue("@tel1", textBox10.Text);
                    micon.Parameters.AddWithValue("@tel2", textBox11.Text);
                    micon.Parameters.AddWithValue("@mail", textBox12.Text);
                    micon.Parameters.AddWithValue("@pais", textBox5.Text.Substring(0,3));
                    micon.Parameters.AddWithValue("@ubig", textBox13.Text);
                    micon.Parameters.AddWithValue("@codi", textBox1.Text);
                    micon.Parameters.AddWithValue("@bloq", (checkBox1.Checked == true) ? "1" : "0");
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", Program.vg_user);
                    micon.Parameters.AddWithValue("@iplan", iplan);
                    micon.Parameters.AddWithValue("@ipwan", ipwan);
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.Parameters.AddWithValue("@cate", "CLI");                  // en la base de datos hay un trigger que actualiza el campo "codigo" con
                    micon.ExecuteNonQuery();                                        // la letra "C" + id del registro, C=cliente
                    //
                    string lectura = "select last_insert_id()";
                    micon = new MySqlCommand(lectura, conn);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetString(0);
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
                    string inserta = "update anagrafiche set tipdoc=@tidoc,RUC=@nudoc,RazonSocial=@raso,Direcc1=@dir1," +
                        "Direcc2=@dir2,depart=@depa,Provincia=@prov,Localidad=@dist,NumeroTel1=@tel1,NumeroTel2=@tel2," +
                        "EMail=@mail,pais=@pais,ubigeo=@ubig,estado=@bloq," +
                        "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,nbname=@nbnam " +
                        "where id=@idan";
                    MySqlCommand micon = new MySqlCommand(inserta, conn);
                    micon.Parameters.AddWithValue("@tidoc", textBox2.Text);
                    micon.Parameters.AddWithValue("@nudoc", textBox3.Text);
                    micon.Parameters.AddWithValue("@raso", textBox4.Text);
                    micon.Parameters.AddWithValue("@dir1", textBox6.Text);
                    micon.Parameters.AddWithValue("@dir2", (textBox6.Text.Trim().Length > 50) ? textBox6.Text.Substring(50, (textBox6.Text.Trim().Length - 50)) : "");
                    micon.Parameters.AddWithValue("@depa", textBox7.Text);
                    micon.Parameters.AddWithValue("@prov", textBox8.Text);
                    micon.Parameters.AddWithValue("@dist", textBox9.Text);
                    micon.Parameters.AddWithValue("@tel1", textBox10.Text);
                    micon.Parameters.AddWithValue("@tel2", textBox11.Text);
                    micon.Parameters.AddWithValue("@mail", textBox12.Text);
                    micon.Parameters.AddWithValue("@pais", textBox5.Text);
                    micon.Parameters.AddWithValue("@ubig", textBox13.Text);
                    micon.Parameters.AddWithValue("@bloq", (checkBox1.Checked == true)? "1":"0");
                    micon.Parameters.AddWithValue("@idan", tx_idr.Text);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", Program.vg_user);
                    micon.Parameters.AddWithValue("@iplan", iplan);
                    micon.Parameters.AddWithValue("@ipwan", ipwan);
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);

                    micon.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar el cliente");
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
            if(textBox7.Text != "") //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("nombre='" + textBox7.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    textBox13.Text = row[0].ItemArray[1].ToString();
                    autoprov();
                }
                else textBox7.Text = "";
            }
            textBox8.Focus();
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia de un departamento, jala distrito
        {
            if(textBox8.Text != "" && textBox7.Text.Trim() != "")   //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + textBox13.Text.Substring(0, 2) + "' and nombre='" + textBox8.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    textBox13.Text = textBox13.Text.Trim().Substring(0,2) + row[0].ItemArray[2].ToString();
                    autodist();
                }
                else textBox8.Text = "";
                textBox9.Focus();
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)
        {
            if(textBox9.Text.Trim() != "" && textBox8.Text.Trim() != "" && textBox7.Text.Trim() != "")  //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + textBox13.Text.Substring(0, 2) + "' and provin='" + textBox13.Text.Substring(2, 2) + "' and nombre='" + textBox9.Text.Trim() + "' and distri<>'00'");
                if (row.Length > 0)
                {
                    textBox13.Text = textBox13.Text.Trim().Substring(0,4) + row[0].ItemArray[3].ToString();
                }
                else textBox9.Text = "";
            }
        }
        private void textBox12_Leave(object sender, EventArgs e)        // correo electrónico
        {
            if(textBox12.Text != "")
            {
                if(lib.email_bien_escrito(textBox12.Text.Trim()) == false)
                {
                    MessageBox.Show("El formato del correo electrónico esta mal", "Atención - Corrija", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    textBox12.Focus();
                    return;
                }
            }
        }
        private void textBox13_Leave(object sender, EventArgs e)        // ubigeo
        {
            if(textBox13.Text.Trim() != "" && textBox13.Text.Trim().Length > 5) // && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + textBox13.Text.Substring(0,2) + "' and provin='00' and distri='00'");
                if (row.Length > 0) textBox7.Text = row[0].ItemArray[4].ToString();
                row = dataUbig.Select("depart='" + textBox13.Text.Substring(0, 2) + "' and provin='" + textBox13.Text.Substring(2, 2) + "' and distri='00'");
                if (row.Length > 0) textBox8.Text = row[0].ItemArray[4].ToString();
                row = dataUbig.Select("depart='" + textBox13.Text.Substring(0, 2) + "' and provin='" + textBox13.Text.Substring(2, 2) + "' and distri='" + textBox13.Text.Substring(4, 2) + "'");
                if (row.Length > 0) textBox9.Text = row[0].ItemArray[4].ToString();
                /*
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
                    micon.Parameters.AddWithValue("@ubi", textBox13.Text);
                    try
                    {
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            while (dr.Read())
                            {
                                textBox7.Text = dr.GetString(0).Trim();
                                textBox8.Text = dr.GetString(1).Trim();
                                textBox9.Text = dr.GetString(2).Trim();
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
                */
            }
        }
        private void textBox3_Leave(object sender, EventArgs e)         // número de documento
        {
            if (textBox3.Text.Trim() != "" && tx_mld.Text.Trim() != "")
            {
                if (textBox3.Text.Trim().Length != Int16.Parse(tx_mld.Text))
                {
                    MessageBox.Show("El número de caracteres para" + Environment.NewLine +
                        "su tipo de documento debe ser: " + tx_mld.Text, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    textBox3.Focus();
                    return;
                }
                string encuentra = "no";
                if (Tx_modo.Text == "NUEVO")
                {
                    if (string.IsNullOrEmpty(lib.nomsn("CLI", textBox2.Text, textBox3.Text)))
                    {
                        if (textBox2.Text == vtc_ruc)
                        {
                            if (lib.valiruc(textBox3.Text, vtc_ruc) == false)
                            {
                                MessageBox.Show("Número de RUC inválido", "Atención - revise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                textBox3.Focus();
                                return;
                            }
                            if (encuentra == "no")
                            {
                                if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                                {
                                    string[] rl = lib.conectorSolorsoft("RUC", textBox3.Text);
                                    textBox4.Text = rl[0];      // razon social
                                    textBox13.Text = rl[1];     // ubigeo
                                    textBox6.Text = rl[2];      // direccion
                                    textBox7.Text = rl[3];      // departamento
                                    textBox8.Text = rl[4];      // provincia
                                    textBox9.Text = rl[5];      // distrito
                                }
                            }
                        }
                        if (textBox2.Text == vtc_dni)
                        {
                            if (encuentra == "no")
                            {
                                if (TransCarga.Program.vg_conSol == true) // conector solorsoft para dni
                                {
                                    string[] rl = lib.conectorSolorsoft("DNI", textBox3.Text);
                                    textBox4.Text = rl[0];      // nombre
                                    textBox3.Text = rl[1];     // num dni
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ya existe el cliente!", "Atención corrija", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textBox3.Text = "";
                        //
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(lib.nomsn("CLI", textBox2.Text, textBox3.Text)))
                    {
                        MessageBox.Show("El cliente no existe!", "Atención corrija", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textBox3.Text = "";
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
                                    "from anag_cli where tipdoc=@tdo and ruc=@ndo";
                                MySqlCommand micon = new MySqlCommand(consulta, conn);
                                micon.Parameters.AddWithValue("@tdo", textBox2.Text);
                                micon.Parameters.AddWithValue("@ndo", textBox3.Text);
                                MySqlDataReader dr = micon.ExecuteReader();
                                if (dr.Read())
                                {
                                    checkBox1.Checked = (dr.GetInt16("estado") == 0) ? false : true;
                                    textBox5.Text = dr.GetString("pais");
                                    textBox1.Text = dr.GetString("id");
                                    tx_idr.Text = dr.GetString("id");
                                    textBox4.Text = dr.GetString("RazonSocial");
                                    textBox6.Text = dr.GetString("Direcc1").Trim() + " " + dr.GetString("Direcc2").Trim();
                                    textBox7.Text = dr.GetString("depart");
                                    textBox8.Text = dr.GetString("Provincia");
                                    textBox9.Text = dr.GetString("Localidad");
                                    textBox13.Text = dr.GetString("ubigeo");
                                    textBox10.Text = dr.GetString("NumeroTel1");
                                    textBox11.Text = dr.GetString("NumeroTel2");
                                    textBox12.Text = dr.GetString("EMail");
                                    //
                                    comboBox1.SelectedValue = textBox2.Text;
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
            if (textBox3.Text.Trim() != "" && tx_mld.Text.Trim() == "")
            {
                comboBox1.Focus();
            }
        }
        private void comboBox1_Leave(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox3.Focus();
        }
        #endregion leaves;

        #region botones_de_comando_y_clients  
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
            textBox1.Focus();
            limpiar(this);
            limpia_otros();
            limpia_combos();
            textBox1.ReadOnly = true;
            textBox5.Text = vapadef;
            textBox5.Focus();
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
            //jalaoc("tx_idr");
            textBox1.Focus();
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
            this.textBox1.Focus();
        }
        private void Bt_anul_Click(object sender, EventArgs e)          // pone todos los clients en N
        {
            // no se anula, solo bloquea
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar(this);
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = lib.gofirts("anag_cli");    // nomtab
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
            tx_idr.Text = lib.golast("anag_cli");     // nomtab
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // clients para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        // selected index del combobox de usuarios
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                DataRow row = ((DataTable)comboBox1.DataSource).Rows[comboBox1.SelectedIndex];
                textBox2.Text = (string)row["idcodice"];
                tx_mld.Text = (string)row["codigo"];
            }
            else
            {
                textBox2.Text = "";
            }
        }
        #endregion comboboxes
    }
}
