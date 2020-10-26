using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class facelect : Form
    {
        static string nomform = "facelect";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabfactu";              // cabecera de guias INDIVIDUALES

        #region variables
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
        string img_ver = "";
        string vtc_dni = "";            // variable tipo cliente natural
        string vtc_ruc = "";            // variable tipo cliente empresa
        string vtc_ext = "";            // variable tipo cliente extranjero
        string codAnul = "";            // codigo de documento anulado
        string codGene = "";            // codigo documento nuevo generado
        string MonDeft = "";            // moneda por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        string v_cid = "";              // codigo interno de tipo de documento
        string v_fra2 = "";             // frase libre 
        string v_sanu = "";             // serie anulacion interna ANU
        string v_CR_gr_ind = "";        // nombre del formato FT/BV en CR
        string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_codidv = "";           // variable codifo interno de documento de venta en vista TDV
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string claveSeg = "";                       // clave de seguridad del envío
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        #endregion

        AutoCompleteStringCollection departamentos = new AutoCompleteStringCollection();// autocompletado departamentos
        AutoCompleteStringCollection provincias = new AutoCompleteStringCollection();   // autocompletado provincias
        AutoCompleteStringCollection distritos = new AutoCompleteStringCollection();    // autocompletado distritos
        DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");

        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";

        DataTable dtu = new DataTable();
        DataTable dttd0 = new DataTable();
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();
        string[] datcltsR = { "", "", "", "", "", "", "", "" };
        string[] datcltsD = { "", "", "", "", "", "", "", "" };
        string[] datguias = { "", "", "", "", "" };

        public facelect()
        {
            InitializeComponent();
        }
        private void facelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void facelect_Load(object sender, EventArgs e)
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
            this.Focus();
            jalainfo();
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            autodepa();                                     // autocompleta departamentos
            autoprov();                                     // autocompleta provincias
            autodist();                                     // autocompleta distritos
            if (valiVars() == false)
            {
                //Application.Exit();
                //return;
            }
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //
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
            tx_dptoRtt.AutoCompleteMode = AutoCompleteMode.Suggest;           // departamentos
            tx_dptoRtt.AutoCompleteSource = AutoCompleteSource.CustomSource;  // departamentos
            tx_dptoRtt.AutoCompleteCustomSource = departamentos;              // departamentos
            tx_provRtt.AutoCompleteMode = AutoCompleteMode.Suggest;           // provincias
            tx_provRtt.AutoCompleteSource = AutoCompleteSource.CustomSource;  // provincias
            tx_provRtt.AutoCompleteCustomSource = provincias;                 // provincias
            tx_distRtt.AutoCompleteMode = AutoCompleteMode.Suggest;           // distritos
            tx_distRtt.AutoCompleteSource = AutoCompleteSource.CustomSource;  // distritos
            tx_distRtt.AutoCompleteCustomSource = distritos;                  // distritos
            // longitudes maximas de campos
            tx_serie.MaxLength = 4;         // serie doc vta
            tx_numero.MaxLength = 8;        // numero doc vta
            tx_serGR.MaxLength = 4;         // serie guia
            tx_numGR.MaxLength = 8;         // numero guia
            tx_numDocRem.MaxLength = 11;    // ruc o dni cliente
            tx_dirRem.MaxLength = 100;
            tx_nomRem.MaxLength = 100;           // nombre remitente
            tx_distRtt.MaxLength = 25;
            tx_provRtt.MaxLength = 25;
            tx_dptoRtt.MaxLength = 25;
            tx_obser1.MaxLength = 150;
            tx_telc1.MaxLength = 12;
            tx_telc2.MaxLength = 12;
            // grilla
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // todo desabilidado
            sololee();
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            dataGridView1.Rows.Clear();
            dataGridView1.ReadOnly = false;
            tx_flete.Text = "";
            tx_pagado.Text = "";
            tx_salxcob.Text = "";
            tx_numero.Text = "";
            tx_serie.Text = v_slu;
            tx_numero.ReadOnly = true;
            tx_dat_mone.Text = MonDeft;
            cmb_mon.SelectedValue = tx_dat_mone.Text;
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_digit.Text = v_nbu;
            tx_dat_estad.Text = codGene;
            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofa,@nofi)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                micon.Parameters.AddWithValue("@nofi", "clients");
                micon.Parameters.AddWithValue("@nofa", nomform);
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
                            if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                            if (row["param"].ToString() == "img_btV") img_btV = row["valor"].ToString().Trim();         // imagen del boton de accion visualizar
                            if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                            if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                            if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                            if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                            if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                            if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                            if (row["param"].ToString() == "img_preview") img_ver = row["valor"].ToString().Trim();         // imagen del boton grabar visualizar
                        }
                        if (row["campo"].ToString() == "estado")
                        {
                            if (row["param"].ToString() == "anulado") codAnul = row["valor"].ToString().Trim();         // codigo doc anulado
                            if (row["param"].ToString() == "generado") codGene = row["valor"].ToString().Trim();        // codigo doc generado
                        }
                    }
                    if (row["formulario"].ToString() == "clients" && row["campo"].ToString() == "documento")
                    {
                        if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "c_int") v_cid = row["valor"].ToString().Trim();               // codigo interno pre guias
                            if (row["param"].ToString() == "frase2") v_fra2 = row["valor"].ToString().Trim();               // frase otro dato
                            if (row["param"].ToString() == "serieAnu") v_sanu = row["valor"].ToString().Trim();               // serie anulacion interna
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "filasDet") v_mfildet = row["valor"].ToString().Trim();       // maxima cant de filas de detalle
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impMatris") v_impA5 = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "nomGRi_cr") v_CR_gr_ind = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "codinDV" && row["param"].ToString() == "DV") v_codidv = row["valor"].ToString().Trim();           // codigo de dov.vta en tabla TDV
                    }
                }
                da.Dispose();
                dt.Dispose();
                // jalamos datos del usuario y local
                v_clu = lib.codloc(asd);                // codigo local usuario
                v_slu = lib.serlocs(v_clu);             // serie local usuario
                v_nbu = lib.nomuser(asd);               // nombre del usuario
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void jalaoc(string campo)        // jala doc venta
        {
            //try
            {
                string parte = "";
                if (campo == "tx_idr")
                {
                    parte = "where a.id=@ida";
                }
                if (campo == "sernum")
                {
                    parte = "where a.sergui=@ser and a.numgui=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = " " + parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@tdep", vtc_ruc);
                    if (campo == "tx_idr") micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    if (campo == "sernum")
                    {
                        micon.Parameters.AddWithValue("@ser", tx_serie.Text);
                        micon.Parameters.AddWithValue("@num", tx_numero.Text);
                    }
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr != null)
                    {
                        if (dr.Read())
                        {
                            tx_idr.Text = dr.GetString("id");
                            tx_fechope.Text = dr.GetString("").Substring(0,10);
                            tx_digit.Text = dr.GetString("userc") + " " + dr.GetString("userm") + " " + dr.GetString("usera");
                            tx_dat_estad.Text = dr.GetString("estadoser");
                            tx_serie.Text = dr.GetString("");
                            tx_numero.Text = dr.GetString("");
                            tx_dat_tdRem.Text = dr.GetString("");
                            tx_numDocRem.Text = dr.GetString("");
                            tx_nomRem.Text = dr.GetString("");
                            tx_dirRem.Text = dr.GetString("");
                            tx_ubigRtt.Text = dr.GetString("");
                            tx_obser1.Text = dr.GetString("");
                            tx_dat_mone.Text = dr.GetString("");
                            tx_flete.Text = dr.GetDecimal("totgri").ToString("#.##");
                            tx_pagado.Text = dr.GetDecimal("totpag").ToString("#.##");
                            tx_salxcob.Text = dr.GetDecimal("salgri").ToString("#.##");
                            tx_totcant.Text = dr.GetString("cantotgri");
                            tx_impreso.Text = dr.GetString("impreso");
                            //
                            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                            cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
                            string[] du_remit = lib.retDPDubigeo(tx_ubigRtt.Text);
                            tx_dptoRtt.Text = du_remit[0];
                            tx_provRtt.Text = du_remit[1];
                            tx_distRtt.Text = du_remit[2];
                            cmb_mon.SelectedValue = tx_dat_mone.Text;
                        }
                        else
                        {
                            MessageBox.Show("No existe el número del documento de venta!", "Atención - dato incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            tx_numero.Text = "";
                            tx_numero.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No existe el número buscado!", "Atención - dato incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    dr.Dispose();
                    micon.Dispose();
                    //
                    if (Tx_modo.Text == "EDITAR")
                    {
                        sololee();
                        dataGridView1.ReadOnly = true;
                    }
                }
                conn.Close();
            }
        }
        private void jaladet(string idr)         // jala el detalle
        {
            string jalad = "select id,.... " +
                "from detfactu where idc=@idr";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand micon = new MySqlCommand(jalad, conn))
                {
                    micon.Parameters.AddWithValue("@idr", idr);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridView1.Rows.Add(
                                row[3].ToString(),
                                row[4].ToString(),
                                row[6].ToString(),
                                row[7].ToString());
                        }
                        dt.Dispose();
                    }
                }
            }
        }
        public void dataload()                  // jala datos para los combos 
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                while (true)
                {
                    try
                    {
                        conn.Open();
                        break;
                    }
                    catch (MySqlException ex)
                    {
                        var aa = MessageBox.Show("No se pudo conectar con el servidor" + Environment.NewLine +
                            "Desea volver a intentarlo?", "Error de conexión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.No)
                        {
                            Application.Exit();
                            return;
                        }
                    }
                }
                // datos para el combobox documento de venta
                cmb_tdv.Items.Clear();
                using (MySqlCommand cdv = new MySqlCommand("select idcodice,descrizionerid,enlace1 from desc_tdv where numero=@bloq and codigo=@codv", conn))
                {
                    cdv.Parameters.AddWithValue("@bloq", 1);
                    cdv.Parameters.AddWithValue("@codv", v_codidv);
                    using (MySqlDataAdapter datv = new MySqlDataAdapter(cdv))
                    {
                        dttd1.Clear();
                        datv.Fill(dttd1);
                        cmb_tdv.DataSource = dttd1;
                        cmb_tdv.DisplayMember = "descrizionerid";
                        cmb_tdv.ValueMember = "idcodice";
                    }
                }
                //  datos para los combobox de tipo de documento
                cmb_docRem.Items.Clear();
                using (MySqlCommand cdu = new MySqlCommand("select idcodice,descrizionerid,codigo from desc_doc where numero=@bloq", conn))
                {
                    cdu.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter datd = new MySqlDataAdapter(cdu))
                    {
                        dttd0.Clear();
                        datd.Fill(dttd0);
                        cmb_docRem.DataSource = dttd0;
                        cmb_docRem.DisplayMember = "descrizionerid";
                        cmb_docRem.ValueMember = "idcodice";
                    }
                }
                // datos para el combo de moneda
                cmb_mon.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mon where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtm.Clear();
                        dacu.Fill(dtm);
                        cmb_mon.DataSource = dtm;
                        cmb_mon.DisplayMember = "descrizionerid";
                        cmb_mon.ValueMember = "idcodice";
                    }
                }
            }
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (vtc_dni == "")           // variable tipo cliente natural
            {
                lib.messagebox("Tipo de cliente Natural");
                retorna = false;
            }
            if (vtc_ruc == "")          // variable tipo cliente empresa
            {
                lib.messagebox("Tipo de cliente Empresa");
                retorna = false;
            }
            if (vtc_ext == "")          // variable tipo cliente extranjero
            {
                lib.messagebox("Tipo de cliente Extranjero");
                retorna = false;
            }
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de GR indivual ANULADA");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de GR indivual GENERADA/NUEVA");
                retorna = false;
            }
            if (MonDeft == "")          // moneda por defecto
            {
                lib.messagebox("Moneda por defecto");
                retorna = false;
            }
            if (v_clu == "")            // codigo del local del usuario
            {
                lib.messagebox("Código local del usuario");
                retorna = false;
            }
            if (v_slu == "")            // serie del local del usuario
            {
                lib.messagebox("Serie general local del usuario");
                retorna = false;
            }
            if (v_nbu == "")            // nombre del usuario
            {
                lib.messagebox("Nombre del usuario");
                retorna = false;
            }
            if (vi_formato == "")       // formato de impresion del documento
            {
                lib.messagebox("formato de impresion de la GR interna");
                retorna = false;
            }
            if (vi_copias == "")        // cant copias impresion
            {
                lib.messagebox("# copias impresas de la GR interna");
                retorna = false;
            }
            if (v_impA5 == "")          // nombre de la impresora matricial
            {
                lib.messagebox("Nombre de impresora matricial");
                retorna = false;
            }
            if (v_impTK == "")           // nombre de la ticketera
            {
                lib.messagebox("Nombre de impresora de Tickets");
                retorna = false;
            }
            if (v_cid == "")             // codigo interno de tipo de documento
            {
                lib.messagebox("Código interno tipo de documento");
                retorna = false;
            }
            if (v_sanu == "")           // serie de anulacion del documento
            {
                lib.messagebox("Serie de Anulación interna");
                retorna = false;
            }
            if (v_CR_gr_ind == "")
            {
                lib.messagebox("Nombre formato GR en CR");
                retorna = false;
            }
            if (v_mfildet == "")
            {
                lib.messagebox("Max. filas de detalle");
                retorna = false;
            }
            if (vint_A0 == "")
            {
                lib.messagebox("Código interno enlace anulación BD - A0");
                retorna = false;
            }
            // aca falta agregar resto  ...........
            return retorna;
        }
        private bool validGR(string serie, string corre)    // validamos y devolvemos datos
        {
            bool retorna = false;
            if (serie != "" && corre != "")
            {
                datcltsR[0] = "";
                datcltsR[1] = "";
                datcltsR[2] = "";
                datcltsR[3] = "";
                datcltsR[4] = "";
                datcltsR[5] = "";
                datcltsR[6] = "";
                datcltsR[7] = "";
                //
                datcltsD[0] = "";
                datcltsD[1] = "";
                datcltsD[2] = "";
                datcltsD[3] = "";
                datcltsD[4] = "";
                datcltsD[5] = "";
                datcltsD[6] = "";
                datcltsD[7] = "";
                //
                datguias[0] = "";
                datguias[1] = "";
                datguias[2] = "";
                datguias[3] = "";
                datguias[4] = "";
                // validamos que la GR: 1.exista, 2.No este facturada, 3.No este anulada
                // y devolvemos una fila con los datos del remitente y otra fila los datos del destinatario
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    lib.procConn(conn);
                    string consulta = "SELECT a.tidoregri,a.nudoregri,a.nombregri,a.direregri,a.ubigregri,ifnull(b1.email,'') as emailR,ifnull(b1.numerotel1,'') as numtel1R,ifnull(b1.numerotel2,'') as numtel2R," +
                        "a.tidodegri,a.nudodegri,a.nombdegri,a.diredegri,a.ubigdegri,ifnull(b2.email,'') as emailD,ifnull(b2.numerotel1,'') as numtel1D,ifnull(b2.numerotel2,'') as numtel2D," +
                        "a.tipmongri,a.totgri,a.salgri,SUM(d.cantprodi) AS bultos,max(d.descprodi) AS descrip,m.descrizionerid as mon " +
                        "from cabguiai a left join detguiai d on d.idc=a.id " +
                        "LEFT JOIN controlg c ON c.serguitra = a.sergui AND c.numguitra = a.numgui " +
                        "left join anag_cli b1 on b1.tipdoc=a.tidoregri and b1.ruc=a.nudoregri " +
                        "left join anag_cli b2 on b2.tipdoc=a.tidodegri and b2.ruc=a.nudodegri " +
                        "left join desc_mon m on m.idcodice=a.tipmongri " +
                        "WHERE a.sergui = @ser AND a.numgui = @num AND a.estadoser IN(@est) AND c.fecdocvta IS NULL";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@ser", serie);
                        micon.Parameters.AddWithValue("@num", corre);
                        micon.Parameters.AddWithValue("@est", codGene);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr.IsDBNull(0) == false)
                                {
                                    datcltsR[0] = dr.GetString("tidoregri");        // datos del remitente de la GR
                                    datcltsR[1] = dr.GetString("nudoregri");
                                    datcltsR[2] = dr.GetString("nombregri");
                                    datcltsR[3] = dr.GetString("direregri");
                                    datcltsR[4] = dr.GetString("ubigregri");
                                    datcltsR[5] = dr.GetString("emailR");
                                    datcltsR[6] = dr.GetString("numtel1R");
                                    datcltsR[7] = dr.GetString("numtel2R");
                                    //
                                    datcltsD[0] = dr.GetString("tidodegri");        // datos del destinatario de la GR
                                    datcltsD[1] = dr.GetString("nudodegri");
                                    datcltsD[2] = dr.GetString("nombdegri");
                                    datcltsD[3] = dr.GetString("diredegri");
                                    datcltsD[4] = dr.GetString("ubigdegri");
                                    datcltsD[5] = dr.GetString("emailD");
                                    datcltsD[6] = dr.GetString("numtel1D");
                                    datcltsD[7] = dr.GetString("numtel2D");
                                    //
                                    datguias[0] = serie + "-" + corre;                 // GR
                                    datguias[1] = dr.GetString("descrip");         // descrip
                                    datguias[2] = dr.GetString("bultos");          // cant bultos
                                    datguias[3] = dr.GetString("mon");             // moneda
                                    datguias[4] = dr.GetString("totgri");          // valor GR
                                                                                   //
                                    retorna = true;
                                }
                            }
                        }
                    }
                }
            }
            return retorna;
        }

        #region autocompletados
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
            if (tx_dptoRtt.Text.Trim() != "")
            {
                DataRow[] provi = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                provincias.Clear();
                foreach (DataRow row in provi)
                {
                    provincias.Add(row["nombre"].ToString());
                }
            }
        }
        private void autodist()                 // se jala despues de ingresado la provincia
        {
            if (tx_ubigRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "")
            {
                DataRow[] distr = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and distri<>'00'");
                distritos.Clear();
                foreach (DataRow row in distr)
                {
                    distritos.Add(row["nombre"].ToString());
                }
            }
        }
        #endregion autocompletados

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
        }
        private void escribe()
        {
            lp.escribe(this);
            tx_nomRem.ReadOnly = true;
            //tx_dirRem.ReadOnly = true;
            //tx_dptoRtt.ReadOnly = true;
            //tx_provRtt.ReadOnly = true;
            //tx_distRtt.ReadOnly = true;
        }
        private void limpiar()
        {
            lp.limpiar(this);
        }
        private void limpia_chk()    
        {
            lp.limpia_chk(this);
        }
        private void limpia_otros()
        {
            //
        }
        private void limpia_combos()
        {
            lp.limpia_cmb(this);
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void bt_agr_Click(object sender, EventArgs e)
        {
            if (tx_serGR.Text.Trim() != "" && tx_numGR.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                // validamos que la GR: 1.exista, 2.No este facturada, 3.No este anulada
                if (validGR(tx_serGR.Text, tx_numGR.Text) == false)
                {
                    MessageBox.Show("La GR no existe, esta anulada o ya esta facturada", "Error en Guía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_numGR.Text = "";
                    tx_numGR.Focus();
                    return;
                }
                else
                {
                    rb_desGR.PerformClick();
                }
                //
                dataGridView1.Rows.Add(datguias[0], datguias[1], datguias[2], datguias[3], datguias[4]);     // insertamos en la grilla los datos de la GR
                MessageBox.Show(datguias[0], datguias[0]);
                int totfil = 0;
                int totcant = 0;
                decimal totflet = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                    {
                        totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                        totfil += 1;
                        if (dataGridView1.Rows[i].Cells[3].Value.ToString() != cmb_mon.Text)
                        {
                            MessageBox.Show("La GR " + dataGridView1.Rows[i].Cells[0].Value.ToString() + "tiene moneda distinta" + Environment.NewLine +
                                "Se calcula al tipo de cambio del día", "Atención");
                            // aca falta ver como le hacemos para los calculos
                        }
                        else
                        {
                            totflet = totflet + decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                        }
                    }
                }
                tx_totcant.Text = totcant.ToString();
                tx_tfil.Text = totfil.ToString();
                tx_flete.Text = totflet.ToString("#0.00");
                if (int.Parse(tx_tfil.Text) == int.Parse(v_mfildet))
                {
                    MessageBox.Show("Número máximo de filas de detalle", "El formato no permite mas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.AllowUserToAddRows = false;
                }
                else
                {
                    dataGridView1.AllowUserToAddRows = true;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            #region validaciones
            if (tx_serie.Text.Trim() == "")
            {
                tx_serie.Focus();
                return;
            }
            if (tx_dat_mone.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de moneda", " Atención ");
                cmb_mon.Focus();
                return;
            }
            if (tx_flete.Text.Trim() == "" || tx_flete.Text.Trim() == "0")
            {
                MessageBox.Show("No existe valor del documento", " Atención ");
                tx_flete.Focus();
                return;
            }
            if (tx_tfil.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del documento de venta", "Faltan ingresar guías");
                tx_serGR.Focus();
                return;
            }
            if (tx_dat_tdRem.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el documento de cliente", " Error en Cliente ");
                tx_dat_tdRem.Focus();
                return;
            }
            if (tx_numDocRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de documento", " Error en Cliente ");
                tx_numDocRem.Focus();
                return;
            }
            if (tx_nomRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el nombre o razón social", " Error en Cliente ");
                tx_nomRem.Focus();
                return;
            }
            if (tx_dirRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la dirección", " Error en Remitente ");
                tx_dirRem.Focus();
                return;
            }
            if (tx_dptoRtt.Text.Trim() == "" || tx_provRtt.Text.Trim() == "" || tx_distRtt.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese departamento, provincia y distrito", "Dirección incompleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_dptoRtt.Focus();
                return;
            }
            if (tx_email.Text.Trim() == "")
            {
                MessageBox.Show("Debe ingresar un correo electrónico", " Error en Cliente ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_email.Focus();
                return;
            }
            if (tx_dat_tdec.Text != tx_dat_tdRem.Text)
            {
                MessageBox.Show("Asegurese que el tipo de documento de venta" + Environment.NewLine +
                    "sean coincidente con el tipo de cliente", "Error de tipos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_docRem.Focus();
                return;
            }
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // valida pago y calcula
                if (rb_si.Checked == false && rb_no.Checked == false)
                {
                    MessageBox.Show("Seleccione si se cancela la factura o no","Atención - Confirme",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    rb_si.Focus();
                    return;
                }
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            var bb = MessageBox.Show("Desea imprimir el documento?" + Environment.NewLine +
                                "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (bb == DialogResult.Yes)
                            {
                                Bt_print.PerformClick();
                            }
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
                    MessageBox.Show("Los datos no son nuevos en doc.venta", "Verifique duplicidad", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            if (modo == "EDITAR")
            {
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número del documento", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("El documento esta ANULADO", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Focus();
                    return;
                }
                if (tx_numDocRem.Enabled == false)
                {
                    MessageBox.Show("El documento no se puede modificar", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (tx_impreso.Text == "N")
                {
                    // SI ESTA IMPRESO NO SE PUEDE MODIFICAR, SOLO ANULAR
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            edita();    // modificacion total
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            tx_dat_tdRem.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("El documento ya debe existir para editar", "Debe ser edición", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
            }
            if (modo == "ANULAR")
            {
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número del documento", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                // LA ANULACION LO VEMOS DESPUES, SE TIENE QUE VER COMO EL ASUNTO DEL PROVEEDOR DE LA FACT. ELECTRONICA
            }
            if (iserror == "no")
            {
                string resulta = lib.ult_mov(nomform, nomtab, asd);
                if (resulta != "OK")                                        // actualizamos la tabla usuarios
                {
                    MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // debe limpiar los campos y actualizar la grilla
                initIngreso();          // limpiamos todo para volver a empesar
            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                string todo = "corre_serie";
                using (MySqlCommand micon = new MySqlCommand(todo, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("td", v_cid);
                    micon.Parameters.AddWithValue("ser", tx_serie.Text);
                    using (MySqlDataReader dr0 = micon.ExecuteReader())
                    {
                        if (dr0.Read())
                        {
                            if (dr0[0] != null && dr0.GetString(0) != "")
                            {
                                tx_numero.Text = lib.Right("00000000" + dr0.GetString(0), 8);
                            }
                        }
                    }
                }
                string inserta = "insert into cabfactu (" +
                    " .... " +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fechop,@sergr,@numgr,@npregr,@tdcdes,@ndcdes,@nomdes,@dircde,@ubicde," +
                    "@tdcrem,@ndcrem,@nomrem,@dircre,@ubicre,@locpgr,@dirpgr,@ubopgr," +
                    "@ldcpgr,@didegr,@ubdegr,@dooprg,@obsprg,@conprg,@totcpr,@totppr," +
                    "@monppr,@tcprgr,@subpgr,@igvpgr,@totpgr,@pagpgr,@totpgr,@estpgr,@canfil," +
                    "@frase1,@frase2,@fleimp,@ticlre,@ticlde,@tipacc,@clavse," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@sergr", tx_serie.Text);
                    micon.Parameters.AddWithValue("@numgr", tx_numero.Text);
                    micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@ubicre", tx_ubigRtt.Text);
                    micon.Parameters.AddWithValue("@totcpr", tx_totcant.Text);
                    micon.Parameters.AddWithValue("@canfil", tx_tfil.Text);     // cantidad de filas de detalle
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@tcprgr", "0.00");           // tipo de cambio ... falta leer de la tabla de cambios
                    micon.Parameters.AddWithValue("@subpgr", "0");              // sub total de la pre guía
                    micon.Parameters.AddWithValue("@igvpgr", "0");              // igv
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);    // total inc. igv
                    micon.Parameters.AddWithValue("@pagpgr", "0");
                    micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                    micon.Parameters.AddWithValue("@frase2", v_fra2);
                    micon.Parameters.AddWithValue("@ticlre", tx_dat_tcr.Text);   // tipo de cliente remitente, credito o contado
                    micon.Parameters.AddWithValue("@tipacc", "");       // guía a credito o contra entrega
                    micon.Parameters.AddWithValue("@clavse", claveSeg);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.ExecuteNonQuery();
                }
                using (MySqlCommand micon = new MySqlCommand("select last_insert_id()", conn))
                {
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            tx_idr.Text = dr.GetString(0);
                        }
                    }
                }
                // detalle
                if (dataGridView1.Rows.Count > 0)
                {
                    int fila = 1;
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                        {

                            string inserd2 = "update detfactu set " +
                                "cantprodi=@can,unimedpro=@uni,codiprodi=@cod,descprodi=@des,pesoprodi=@pes,precprodi=@preu,totaprodi=@pret " +
                                "where idc=@idr and fila=@fila";
                            using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                            {
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@fila", fila);
                                //micon.Parameters.AddWithValue("@serpgr", tx_serie.Text);
                                //micon.Parameters.AddWithValue("@corpgr", tx_numero.Text);
                                micon.Parameters.AddWithValue("@can", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                micon.Parameters.AddWithValue("@uni", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@cod", "");
                                micon.Parameters.AddWithValue("@pes", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                micon.Parameters.AddWithValue("@preu", "0");
                                micon.Parameters.AddWithValue("@pret", "0");
                                micon.ExecuteNonQuery();
                                fila += 1;
                                //
                                retorna = true;         // no hubo errores!
                            }
                        }
                    }
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
                    if (tx_impreso.Text == "N")     // EDICION DE CABECERA
                    {
                        string actua = "update cabfactu a set " +
                            "a.fechopegr=@fechop,a.tidodegri=@tdcdes,a.nudodegri=@ndcdes," +
                            "a.nombdegri=@nomdes,a.diredegri=@dircde,a.ubigdegri=@ubicde,a.tidoregri=@tdcrem,a.nudoregri=@ndcrem," + 
                            "a.nombregri=@nomrem,a.direregri=@dircre,a.ubigregri=@ubicre,a.locorigen=@locpgr,a.dirorigen=@dirpgr," +
                            "a.ubiorigen=@ubopgr,a.locdestin=@ldcpgr,a.dirdestin=@didegr,a.ubidestin=@ubdegr,a.docsremit=@dooprg," +
                            "a.obspregri=@obsprg,a.clifingri=@conprg,a.cantotgri=@totcpr,a.pestotgri=@totppr,a.tipmongri=@monppr," +
                            "a.tipcamgri=@tcprgr,a.subtotgri=@subpgr,a.igvgri=@igvpgr,a.totgri=@totpgr,a.totpag=@pagpgr," +
                            "a.salgri=@totpgr,a.estadoser=@estpgr,a.seguroE=@clavse,a.cantfilas=@canfil," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                        micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                        micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                        micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                        micon.Parameters.AddWithValue("@ubicre", tx_ubigRtt.Text);
                        micon.Parameters.AddWithValue("@totcpr", tx_totcant.Text);
                        micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                        micon.Parameters.AddWithValue("@tcprgr", "0.00");  // tipo de cambio
                        micon.Parameters.AddWithValue("@subpgr", "0"); // sub total de la pre guía
                        micon.Parameters.AddWithValue("@igvpgr", "0"); // igv
                        micon.Parameters.AddWithValue("@pagpgr", "0");
                        micon.Parameters.AddWithValue("@totpgr", tx_flete.Text); // saldo de la pre guia = total pre guia
                        micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                        micon.Parameters.AddWithValue("@clavse", claveSeg);
                        micon.Parameters.AddWithValue("@canfil", tx_tfil.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE .... ANALIZAR SI VAMOS O NO?
                        //
                        micon = new MySqlCommand("borraseguro", conn);
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@tabla", "detguiai");
                        micon.Parameters.AddWithValue("@vidr", "0");
                        micon.Parameters.AddWithValue("@vidc", tx_idr.Text);
                        micon.ExecuteNonQuery();
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                            {
                                string inserd2 = "insert into detfactu (idc,fila,sergui,numgui," +
                                    "cantprodi,unimedpro,codiprodi,descprodi,pesoprodi,precprodi,totaprodi," +
                                    "estadoser,verApp,userm,fechm,diriplan4,diripwan4,netbname" +
                                    ") values (@idr,@fila,@serpgr,@corpgr," +
                                    "@can,@uni,@cod,@des,@pes,@preu,@pret," +
                                    "@estpgr,@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                                micon = new MySqlCommand(inserd2, conn);
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@fila", i+1);
                                micon.Parameters.AddWithValue("@serpgr", tx_serie.Text);
                                micon.Parameters.AddWithValue("@corpgr", tx_numero.Text);
                                micon.Parameters.AddWithValue("@can", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                micon.Parameters.AddWithValue("@uni", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@cod", "");
                                micon.Parameters.AddWithValue("@des", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                micon.Parameters.AddWithValue("@pes", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                micon.Parameters.AddWithValue("@preu", "0");
                                micon.Parameters.AddWithValue("@pret", "0");
                                micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                                micon.Parameters.AddWithValue("@verApp", verapp);
                                micon.Parameters.AddWithValue("@asd", asd);
                                micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                                micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                micon.ExecuteNonQuery();
                            }
                        }
                        //
                        micon.Dispose();
                    }
                    if (tx_impreso.Text == "S")
                    {
                        // de momento no cambiamos nada 16/08/2020
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar el documento");
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
        }
        private void anula()
        {
            // en el caso guias y otros documentos HAY 2: ANULACION FISICA y ANULACION INTERNA (serie ANU)
            // Anulacion fisica se "anula" el numero del documento en sistema y en fisico se tacha, marca anulado 
            // Anulación interna (ANU) el numero se recupera tanto en fisico como en sistema, el anulado internamente pasa a ser serie ANU
            // se borran todos los enlaces en cualquier tipo de anulacion
            var aa = MessageBox.Show("Anulación interna para recuperar el número?" + Environment.NewLine +
                "Se cambia la serie a ANU", "Atención, confirme por favor",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            string parte = " ";
            if (aa == DialogResult.Yes) parte = ",sergui=@coad ";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabfactu set estadoser=@estser,usera=@asd,fecha=now()," +
                        "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp,estintreg=@eiar" + parte +
                        "where id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
                        micon.Parameters.AddWithValue("@eiar", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
                        if (aa == DialogResult.Yes) micon.Parameters.AddWithValue("@coad", v_sanu);
                        micon.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion boton_form;

        #region leaves y checks
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                dataGridView1.Rows.Clear();
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento del remitente, jala provincia
        {
            if(tx_dptoRtt.Text.Trim() != "")    //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("nombre='" + tx_dptoRtt.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = row[0].ItemArray[1].ToString(); // lib.retCodubigeo(tx_dptoRtt.Text.Trim(),"","");
                    autoprov();
                }
                else tx_dptoRtt.Text = "";
            }
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia del remitente
        {
            if(tx_provRtt.Text != "" && tx_dptoRtt.Text.Trim() != "")   // && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and nombre='" + tx_provRtt.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = tx_ubigRtt.Text.Trim().Substring(0, 2) + row[0].ItemArray[2].ToString();
                    autodist();
                }
                else tx_provRtt.Text = "";
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)         // distrito del remitente
        {
            if(tx_distRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "" && tx_dptoRtt.Text.Trim() != "")
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and nombre='" + tx_distRtt.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = tx_ubigRtt.Text.Trim().Substring(0, 4) + row[0].ItemArray[3].ToString(); // lib.retCodubigeo(tx_distRtt.Text.Trim(),"",tx_ubigRtt.Text.Trim());
                }
                else tx_distRtt.Text = "";
            }
        }
        private void textBox13_Leave(object sender, EventArgs e)        // ubigeo del remitente
        {
            if(tx_ubigRtt.Text.Trim() != "" && tx_ubigRtt.Text.Length == 6)
            {
                string[] du_remit = lib.retDPDubigeo(tx_ubigRtt.Text);
                tx_dptoRtt.Text = du_remit[0];
                tx_provRtt.Text = du_remit[1];
                tx_distRtt.Text = du_remit[2];
            }
        }
        private void textBox3_Leave(object sender, EventArgs e)         // número de documento remitente
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
                if (tx_dat_tdRem.Text == vtc_ruc && lib.valiruc(tx_numDocRem.Text, vtc_ruc) == false)
                {
                    MessageBox.Show("Número de RUC inválido", "Atención - revise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx_numDocRem.Focus();
                    return;
                }
                string encuentra = "no";
                if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
                {
                    string[] datos = lib.datossn("CLI", tx_dat_tdRem.Text.Trim(), tx_numDocRem.Text.Trim());
                    if (datos.Length > 0)
                    {
                        tx_nomRem.Text = datos[0];
                        tx_nomRem.Select(0, 0);
                        tx_dirRem.Text = datos[1];
                        tx_dirRem.Select(0, 0);
                        tx_dptoRtt.Text = datos[2];
                        tx_dptoRtt.Select(0, 0);
                        tx_provRtt.Text = datos[3];
                        tx_provRtt.Select(0, 0);
                        tx_distRtt.Text = datos[4];
                        tx_distRtt.Select(0, 0);
                        tx_ubigRtt.Text = datos[5];
                        tx_ubigRtt.Select(0, 0);
                        encuentra = "si";
                    }
                    if (tx_dat_tdRem.Text == vtc_ruc)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                            {
                                string[] rl = lib.conectorSolorsoft("RUC", tx_numDocRem.Text);
                                tx_nomRem.Text = rl[0];      // razon social
                                tx_ubigRtt.Text = rl[1];     // ubigeo
                                tx_dirRem.Text = rl[2];      // direccion
                                tx_dptoRtt.Text = rl[3];      // departamento
                                tx_provRtt.Text = rl[4];      // provincia
                                tx_distRtt.Text = rl[5];      // distrito
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
                                //tx_numDocRem.Text = rl[1];     // num dni
                            }
                        }
                    }
                }
            }
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() == "")
            {
                cmb_docRem.Focus();
            }
        }
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO")
            {
                // en el caso de las pre guias el numero es el mismo que el ID del registro
                tx_numero.Text = lib.Right("00000000" + tx_numero.Text, 8);
                //tx_idr.Text = tx_numero.Text;
                jalaoc("sernum");
                dataGridView1.Rows.Clear();
                jaladet(tx_idr.Text);
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
            if (Tx_modo.Text == "NUEVO") tx_serGR.Focus();
        }
        private void tx_flete_Leave(object sender, EventArgs e)
        {
            button1.Focus();
        }
        private void tx_serGR_Leave(object sender, EventArgs e)
        {
            tx_serGR.Text = lib.Right("0000" + tx_serGR.Text, 4);
        }
        private void tx_numGR_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_serGR.Text.Trim() != "" && tx_numGR.Text.Trim() != "")
            {
                tx_numGR.Text = lib.Right("00000000" + tx_numGR.Text, 8);
            }
        }
        private void rb_remGR_Click(object sender, EventArgs e)         // datos del remitente de la GR
        {
            tx_dat_tdRem.Text = datcltsR[0];
            cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
            tx_numDocRem.Text = datcltsR[1];
            tx_nomRem.Text = datcltsR[2];
            tx_dirRem.Text = datcltsR[3];
            //
            DataRow[] row = dataUbig.Select("depart='" + datcltsR[4].Substring(0, 2) + "' and provin='00' and distri='00'");
            tx_dptoRtt.Text = row[0].ItemArray[4].ToString();
            row = dataUbig.Select("depart='" + datcltsR[4].Substring(0, 2) + "' and provin ='"+ datcltsR[4].Substring(2, 2) + "' and distri='00'");
            tx_provRtt.Text = row[0].ItemArray[4].ToString();
            row = dataUbig.Select("depart='" + datcltsR[4].Substring(0, 2) + "' and provin ='" + datcltsR[4].Substring(2, 2) + "' and distri='" + datcltsR[4].Substring(4, 2) + "'");
            tx_distRtt.Text = row[0].ItemArray[4].ToString();
            //
            tx_email.Text = datcltsR[5];
            tx_telc1.Text = datcltsR[6];
            tx_telc2.Text = datcltsR[7];
            //
            cmb_docRem.Enabled = false;
            tx_numDocRem.ReadOnly = true;
            tx_nomRem.ReadOnly = true;
        }
        private void rb_desGR_Click(object sender, EventArgs e)         // datos del destinatario de la GR
        {
            tx_dat_tdRem.Text = datcltsD[0];
            cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
            tx_numDocRem.Text = datcltsD[1];
            tx_nomRem.Text = datcltsD[2];
            tx_dirRem.Text = datcltsD[3];
            //
            DataRow[] row = dataUbig.Select("depart='" + datcltsD[4].Substring(0, 2) + "' and provin='00' and distri='00'");
            tx_dptoRtt.Text = row[0].ItemArray[4].ToString();
            row = dataUbig.Select("depart='" + datcltsD[4].Substring(0, 2) + "' and provin ='" + datcltsD[4].Substring(2, 2) + "' and distri='00'");
            tx_provRtt.Text = row[0].ItemArray[4].ToString();
            row = dataUbig.Select("depart='" + datcltsD[4].Substring(0, 2) + "' and provin ='" + datcltsD[4].Substring(2, 2) + "' and distri='" + datcltsD[4].Substring(4, 2) + "'");
            tx_distRtt.Text = row[0].ItemArray[4].ToString();
            //
            tx_email.Text = datcltsD[5];
            tx_telc1.Text = datcltsD[6];
            tx_telc2.Text = datcltsD[7];
            //
            cmb_docRem.Enabled = false;
            tx_numDocRem.ReadOnly = true;
            tx_nomRem.ReadOnly = true;
        }
        private void rb_otro_Click(object sender, EventArgs e)
        {
            cmb_docRem.Enabled = true;
            tx_numDocRem.ReadOnly = false;
            tx_nomRem.ReadOnly = false;
            //
            tx_numDocRem.Text = "";
            tx_nomRem.Text = "";
            tx_dirRem.Text = "";
            tx_dptoRtt.Text = "";
            tx_provRtt.Text = "";
            tx_distRtt.Text = "";
            tx_email.Text = "";
            tx_telc1.Text = "";
            tx_telc2.Text = "";
            cmb_docRem.SelectedIndex = 0;
            tx_dat_tdRem.Text = cmb_docRem.SelectedValue.ToString();
            DataRow[] fila = dttd0.Select("idcodice='" + tx_dat_tdRem.Text + "'");
            foreach (DataRow row in fila)
            {
                tx_mld.Text = row[2].ToString();
            }
            cmb_docRem.Focus();
        }
        private void tx_email_Leave(object sender, EventArgs e)
        {
            if (tx_email.Text.Trim() != "")
            {
                if (lib.email_bien_escrito(tx_email.Text.Trim()) == false)
                {
                    MessageBox.Show("El correo electrónico esta mal", "Por favor corrija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx_email.Focus();
                    return;
                }
            }
        }
        private void rb_si_Click(object sender, EventArgs e)
        {
            tx_pagado.Text = tx_flete.Text;
            tx_salxcob.Text = "0.00";
            tx_salxcob.BackColor = Color.Green;
        }
        private void rb_no_Click(object sender, EventArgs e)
        {
            tx_pagado.Text = "0.00";
            tx_salxcob.Text = tx_flete.Text;
            tx_salxcob.BackColor = Color.Red;
        }
        #endregion

        #region botones_de_comando
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
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            escribe();
            // 
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            tx_salxcob.BackColor = Color.White;
            //
            tx_numero.ReadOnly = true;
            cmb_tdv_SelectedIndexChanged(null, null);
            cmb_tdv.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            escribe();
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            tx_obser1.Enabled = true;
            tx_numero.Text = "";
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            tx_salxcob.BackColor = Color.White;
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            // Impresion ó Re-impresion ??
            if (tx_impreso.Text == "S")
            {
                var aa = MessageBox.Show("Desea re imprimir el documento?", "Confirme por favor", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    if (vi_formato == "A4")            // Seleccion de formato ... A4
                    {
                        if (imprimeA4() == true) updateprint("S");
                    }
                    if (vi_formato == "A5")            // Seleccion de formato ... A5
                    {
                        if (imprimeA5() == true) updateprint("S");
                    }
                    if (vi_formato == "TK")            // Seleccion de formato ... Ticket
                    {
                        if (imprimeTK() == true) updateprint("S");
                    }
                }
            }
            else
            {
                if (vi_formato == "A4")            // Seleccion de formato ... A4
                {
                    if (imprimeA4() == true) updateprint("S");
                }
                if (vi_formato == "A5")
                {
                    if (imprimeA5() == true) updateprint("S");
                }
                if (vi_formato == "TK")
                {
                    if (imprimeTK() == true) updateprint("S");
                }
            }
            // Cantidad de copias
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            initIngreso();
            gbox_serie.Enabled = true;
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
        }
        private void Bt_ver_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "VISUALIZAR";
            button1.Image = Image.FromFile(img_ver);
            initIngreso();
            gbox_serie.Enabled = true;
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            limpia_chk();
            tx_idr.Text = lib.gofirts(nomtab);
            tx_idr_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            if(tx_idr.Text.Trim() != "")
            {
                int aca = int.Parse(tx_idr.Text) - 1;
                limpiar();
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
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = aca.ToString();
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = lib.golast(nomtab);
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // proveed para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        private void cmb_docRem_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docRem.SelectedIndex > -1)
            {
                tx_dat_tdRem.Text = cmb_docRem.SelectedValue.ToString();
                DataRow[] fila = dttd0.Select("idcodice='" + tx_dat_tdRem.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mld.Text = row[2].ToString();
                }
            }
        }
        private void cmb_mon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_mon.SelectedIndex > -1)
            {
                tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                MessageBox.Show("Proceso de cálculo al tipo de cambio");
                tx_flete.Text = "0.00";
            }
        }
        private void cmb_tdv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_tdv.SelectedIndex > -1)
            {
                //tx_dat_tdv.Text = cmb_tdv. // cmb_tdv.SelectedValue.ToString();
                DataRow[] row = dttd1.Select("idcodice='" + cmb_tdv.SelectedValue.ToString() + "'");
                if (row.Length > 0)
                {
                    tx_dat_tdv.Text = row[0].ItemArray[0].ToString();
                    tx_dat_tdec.Text = row[0].ItemArray[2].ToString();
                }
            }
        }
        #endregion comboboxes

        #region impresion
        private bool imprimeA4()
        {
            bool retorna = false;

            return retorna;
        }
        private bool imprimeA5()
        {
            bool retorna = false;
            llenaDataSet();                         // metemos los datos al dataset de la impresion
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            try
            {
                printDocument1.PrinterSettings.PrinterName = v_impTK;
                printDocument1.Print();
                retorna = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error en imprimir TK");
                retorna = false;
            }
            return retorna;
        }
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (vi_formato == "A4")
            {
                imprime_A4(sender, e);
            }
            if (vi_formato == "A5")
            {
                imprime_A5(sender, e);
            }
            if (vi_formato == "TK")
            {
                imprime_TK(sender, e);
            }
        }
        private void imprime_A4(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }
        private void imprime_A5(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float alfi = 20.0F;     // alto de cada fila
            float alin = 50.0F;     // alto inicial
            float posi = 80.0F;     // posición de impresión
            float coli = 20.0F;     // columna mas a la izquierda
            float cold = 80.0F;
            Font lt_tit = new Font("Arial", 11);
            Font lt_titB = new Font("Arial", 11, FontStyle.Bold);
            PointF puntoF = new PointF(coli, alin);
            e.Graphics.DrawString(nomclie, lt_titB, Brushes.Black, puntoF, StringFormat.GenericTypographic);                      // titulo del reporte
            posi = posi + alfi;
            string numguia = "GR " + tx_serie.Text + "-" + tx_numero.Text;
            float lt = (lp.CentimeterToPixel(this,21F) - e.Graphics.MeasureString(numguia, lt_titB).Width) / 2;
            puntoF = new PointF(lt, posi);
            e.Graphics.DrawString(numguia, lt_titB, Brushes.Black, puntoF, StringFormat.GenericTypographic);                      // titulo del reporte
            posi = posi + alfi*2;
            PointF ptoimp = new PointF(coli, posi);                     // fecha de emision
            e.Graphics.DrawString("EMITIDO: " + tx_fechope.Text.Substring(0,10), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi + 30.0F;                                         // avance de fila
            ptoimp = new PointF(coli, posi);                               // direccion partida
            e.Graphics.DrawString("REMITENTE: " + tx_nomRem.Text.Trim(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi;
            ptoimp = new PointF(coli, posi);                       // destinatario
            posi = posi + alfi * 2;
            /*
            // seleccion de impresion en ruc u otro tipo
            ptoimp = new PointF(coli + 50.0F, posi);
            e.Graphics.DrawString(tx_numDocRem.Text, lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            ptoimp = new PointF(colm + 185.0F, posi);
            e.Graphics.DrawString(tx_numDocDes.Text, lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = 330.0F;             // avance de fila
            */
            // detalle de la pre guia
            for (int fila = 0; fila < dataGridView1.Rows.Count - 1; fila++)
            {
                ptoimp = new PointF(coli + 20.0F, posi);
                e.Graphics.DrawString(dataGridView1.Rows[fila].Cells[0].Value.ToString(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                ptoimp = new PointF(cold, posi);
                e.Graphics.DrawString(dataGridView1.Rows[fila].Cells[1].Value.ToString(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                ptoimp = new PointF(cold + 80.0F, posi);
                e.Graphics.DrawString(dataGridView1.Rows[fila].Cells[2].Value.ToString(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                ptoimp = new PointF(cold + 400.0F, posi);
                e.Graphics.DrawString("KGs." + dataGridView1.Rows[fila].Cells[3].Value.ToString(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                posi = posi + alfi;             // avance de fila
            }
            // guias del cliente
            posi = posi + alfi;
            ptoimp = new PointF(coli, posi);
            e.Graphics.DrawString("Docs. de remisión: ", lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            // imprime el flete
            posi = posi + alfi * 2;
            string gtotal = "FLETE " + cmb_mon.Text + " " + tx_flete.Text;
            lt = (lp.CentimeterToPixel(this,21F) - e.Graphics.MeasureString(gtotal, lt_titB).Width) / 2;
            ptoimp = new PointF(lt, posi);
            e.Graphics.DrawString(gtotal, lt_titB, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi;

        }
        private void imprime_TK(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // no hay guias en TK
        }
        private void updateprint(string sn)  // actualiza el campo impreso de la GR = S
        {   // S=si impreso || N=no impreso
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "update cabguiai set impreso=@sn where id=@idr";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@sn", sn);
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region crystal
        private void llenaDataSet()
        {
            conClie data = generaReporte();
            //gr_ind_transp repo = new gr_ind_transp();
            //repo.SetDataSource(data);
            //repo.PrintOptions.PrinterName = v_impA5;
            //repo.PrintToPrinter(int.Parse(vi_copias),false,1,1);
            ReportDocument repo = new ReportDocument();
            repo.Load(v_CR_gr_ind);
            repo.SetDataSource(data);
            repo.PrintOptions.PrinterName = v_impA5;
            repo.PrintToPrinter(int.Parse(vi_copias),false,1,1);
        }
        private conClie generaReporte()
        {
            conClie guiaT = new conClie();
            conClie.gr_ind_cabRow rowcabeza = guiaT.gr_ind_cab.Newgr_ind_cabRow();
            //
            // CABECERA
            rowcabeza.id = tx_idr.Text;
            rowcabeza.estadoser = tx_estado.Text;
            rowcabeza.sergui = tx_serie.Text;
            rowcabeza.numgui = tx_numero.Text;
            rowcabeza.fechope = tx_fechope.Text;
            rowcabeza.frase1 = "";  // no hay campo
            rowcabeza.frase2 = "";  // no hay campo
            // origen - destino
            rowcabeza.dptoDestino = ""; // no hay campo
            rowcabeza.provDestino = "";
            rowcabeza.distDestino = ""; // no hay campo
            rowcabeza.dptoOrigen = "";  // no hay campo
            rowcabeza.provOrigen = "";
            rowcabeza.distOrigen = "";  // no hay campo
            // remitente
            rowcabeza.docRemit = cmb_docRem.Text;
            rowcabeza.numRemit = tx_numDocRem.Text;
            rowcabeza.nomRemit = tx_nomRem.Text;
            rowcabeza.direRemit = tx_dirRem.Text;
            rowcabeza.dptoRemit = tx_dptoRtt.Text;
            rowcabeza.provRemit = tx_provRtt.Text;
            rowcabeza.distRemit = tx_distRtt.Text;
            // importes
            rowcabeza.nomMoneda = cmb_mon.Text;
            rowcabeza.igv = "";         // no hay campo
            rowcabeza.subtotal = "";    // no hay campo
            rowcabeza.total = tx_flete.Text;
            // pie

            //
            guiaT.gr_ind_cab.Addgr_ind_cabRow(rowcabeza);
            //
            // DETALLE  
            for (int i=0; i<dataGridView1.Rows.Count -1; i++)   // foreach (DataGridViewRow row in dataGridView1.Rows)
            {   
                conClie.gr_ind_detRow rowdetalle = guiaT.gr_ind_det.Newgr_ind_detRow();

                rowdetalle.fila = "";       // no estamos usando
                rowdetalle.cant = dataGridView1.Rows[i].Cells[0].Value.ToString();
                rowdetalle.codigo = "";     // no estamos usando
                rowdetalle.umed = dataGridView1.Rows[i].Cells[1].Value.ToString();
                rowdetalle.descrip = dataGridView1.Rows[i].Cells[2].Value.ToString();
                rowdetalle.precio = "";     // no estamos usando
                rowdetalle.total = "";      // no estamos usando
                rowdetalle.peso = dataGridView1.Rows[i].Cells[3].Value.ToString();
                guiaT.gr_ind_det.Addgr_ind_detRow(rowdetalle);
            }
            //
            return guiaT;
        }
        #endregion

    }
}
