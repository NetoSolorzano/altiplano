using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class preguiat : Form
    {
        static string nomform = "preguiat";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabpregr";              // cabecera de pre guias

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
        string codDInt = "";            // codigo tipo de documento interno para las pre guías
        string codGene = "";            // codigo documento nuevo generado
        string MonDeft = "";            // moneda por defecto
        string gloDeta = "";            // glosa x defecto en el detalle
        static libreria lib = new libreria();
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string claveSeg = "";           // clave de seguridad del envío
        string nomclie = Program.cliente;
        string rucclie = Program.ruc;
        string asd = TransCarga.Program.vg_user;        // usuario conectado al sistema
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        string v_clte_rem = "";         // variable para marcar si el remitente es cliente nuevo "N" o para actualizar sus datos "E"
        string v_clte_des = "";         // variable para marcar si el destinatario es cliente nuevo "N" o para actualizar sus datos "E"
        string usoPGm = "";             // variable para indicar si el numerador es "automatico" o "manual"
        string caractNo = "";           // caracter prohibido en campos texto, caracter delimitador para los TXT de fact. electronica
        string det3dtm = "";            // palabra nombre descriptivo de las guias de remision electronicas de transportista
        string tccmr = "";              // tipos de comprobante sunat con el mismo ruc que del remitente de la guia
        #endregion

        AutoCompleteStringCollection departamentos = new AutoCompleteStringCollection();// autocompletado departamentos
        AutoCompleteStringCollection provincias = new AutoCompleteStringCollection();   // autocompletado provincias
        AutoCompleteStringCollection distritos = new AutoCompleteStringCollection();    // autocompletado distritos
        string[] datosR = { "" };                   // datos del remitente si existe en la B.D.
        string[] datosD = { "" };                   // datos del destinatario si existe en la B.D.
        string[] rl = { "" };                       // datos del NUEVO remitente

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public preguiat()
        {
            InitializeComponent();
        }
        private void preguiat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
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
            this.Focus();
            jalainfo();
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            autodepa();                                     // autocompleta departamentos
        }
        private void init()
        {
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
            tx_dptoDrio.AutoCompleteMode = AutoCompleteMode.Suggest;            // departamentos
            tx_dptoDrio.AutoCompleteSource = AutoCompleteSource.CustomSource;   // departamentos
            tx_dptoDrio.AutoCompleteCustomSource = departamentos;               // departamentos
            tx_proDrio.AutoCompleteMode = AutoCompleteMode.Suggest;             // provincias
            tx_proDrio.AutoCompleteSource = AutoCompleteSource.CustomSource;    // provincias
            tx_proDrio.AutoCompleteCustomSource = provincias;                   // provincias
            tx_disDrio.AutoCompleteMode = AutoCompleteMode.Suggest;             // distritos
            tx_disDrio.AutoCompleteSource = AutoCompleteSource.CustomSource;    // distritos
            tx_disDrio.AutoCompleteCustomSource = distritos;                    // distritos
            // longitudes maximas de campos
            tx_serie.MaxLength = 4;         // serie pre guia
            tx_numero.MaxLength = 8;        // numero pre guia
            tx_numDocRem.MaxLength = 11;
            tx_nomRem.MaxLength = 100;           // nombre remitente
            tx_dirRem.MaxLength = 100;
            tx_distRtt.MaxLength = 25;
            tx_provRtt.MaxLength = 25;
            tx_dptoRtt.MaxLength = 25;
            tx_numDocDes.MaxLength = 11;
            tx_nomDrio.MaxLength = 100;           // nombre destinatario
            tx_dirDrio.MaxLength = 100;
            tx_disDrio.MaxLength = 25;
            tx_proDrio.MaxLength = 25;
            tx_dptoDrio.MaxLength = 25;
            tx_docsOr.MaxLength = 100;          // documentos origen del traslado
            tx_obser1.MaxLength = 100;
            tx_consig.MaxLength = 100;
            // detalle
            tx_det_umed.MaxLength = 15;
            tx_det_desc.MaxLength = 50;
            // todo desabilidado
            sololee(this);
        }
        private void initIngreso()
        {
            limpiar(this);
            limpia_chk();
            limpia_otros();
            limpia_combos();
            rb_kg.Checked = true;
            claveSeg = "";
            tx_flete.Text = "";
            tx_numero.Text = "";
            tx_totcant.Text = "";
            tx_totpes.Text = "";
            tx_serie.Text = v_slu;
            tx_dat_tdi.Text = codDInt;
            lb_glodeta.Text = gloDeta;
            if (usoPGm == "manual")
            {
                tx_numero.Enabled = true;
                tx_numero.ReadOnly = ("NUEVO,EDITAR".Contains(Tx_modo.Text)) ? false : true;
                tx_numero.Text = "";
            }
            else
            {
                tx_numero.Enabled = true;
                tx_numero.ReadOnly = ("NUEVO".Contains(Tx_modo.Text)) ? true : false;
                tx_numero.Text = "";
            }
            tx_dat_locori.Text = v_clu;
            cmb_origen.SelectedValue = tx_dat_locori.Text;
            cmb_origen_SelectionChangeCommitted(null, null);
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
                for (int t = 0; t < Program.dt_enlaces.Rows.Count; t++)
                {
                    DataRow row = Program.dt_enlaces.Rows[t];
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
                            if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "c_int") codDInt = row["valor"].ToString().Trim();           // codigo interno pre guias
                            if (row["param"].ToString() == "usoPGm") usoPGm = row["valor"].ToString().Trim();           // numeración "automatico" ó "manual"
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impMatris") v_impA5 = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                        if (row["campo"].ToString() == "detalle" && row["param"].ToString() == "glosa") gloDeta = row["valor"].ToString().Trim();             // glosa del detalle
                    }
                    if (row["formulario"].ToString() == "facelect")
                    {
                        if (row["campo"].ToString() == "factelect")
                        {
                            if (row["param"].ToString() == "caracterNo") caractNo = row["valor"].ToString().Trim();
                        }
                    }
                    if (row["formulario"].ToString() == "guiati_e")
                    {
                        if (row["campo"].ToString() == "glosas" && row["param"].ToString() == "nomGRET") det3dtm = row["valor"].ToString().Trim();         // nombre detalle DTM de las GRE-Transportista 
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "tccmr") tccmr = row["valor"].ToString().Trim();         // tipos de doc sunat donde el ruc del doc relacionado = remitente de la GR
                    }
                }
                // jalamos datos del usuario y local
                v_clu = lib.codloc(asd);                // codigo local usuario
                v_slu = lib.serlocs(v_clu);             // serie local usuario
                v_nbu = lib.nomuser(asd);               // nombre del usuario
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void jalaoc(string campo)        // jala pre guia desde el campo tx_idr
        {
            try
            {
                string parte = "";
                if (campo == "tx_idr")
                {
                    parte = "where id=@ida";
                }
                if (campo == "sernum")
                {
                    parte = "where serpregui=@ser and numpregui=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select id,fechpregr,serpregui,numpregui,tidodepre,nudodepre,nombdepre,diredepre,ubigdepre," +
                        "tidorepre,nudorepre,nombrepre,direrepre,ubigrepre,locorigen,dirorigen,ubiorigen,locdestin," +
                        "dirdestin,ubidestin,obspregui,clifinpre,cantotpre,pestotpre,tipmonpre,tipcampre,seguroE," +
                        "subtotpre,igvpregui,totpregui,totpagpre,salpregui,estadoser,impreso,serguitra,numguitra," +
                        "tidocor,rucDorig,docsremit,tidocor2,rucDorig2,docsremit2," +
                        "userc,userm,usera " +
                        "from cabpregr " + parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
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
                            tx_fechope.Text = dr.GetString("fechpregr");
                            tx_digit.Text = dr.GetString("userc") + " " + dr.GetString("userm") + " " + dr.GetString("usera");
                            tx_dat_estad.Text = dr.GetString("estadoser");
                            tx_serie.Text = dr.GetString("serpregui");
                            tx_numero.Text = dr.GetString("numpregui");
                            tx_dat_locori.Text = dr.GetString("locorigen");
                            tx_dat_locdes.Text = dr.GetString("locdestin");
                            tx_ubigO.Text = dr.GetString("ubiorigen");
                            tx_ubigD.Text = dr.GetString("ubidestin");
                            tx_dat_tdRem.Text = dr.GetString("tidorepre");
                            tx_numDocRem.Text = dr.GetString("nudorepre");
                            tx_nomRem.Text = dr.GetString("nombrepre");
                            tx_dirRem.Text = dr.GetString("direrepre");
                            tx_ubigRtt.Text = dr.GetString("ubigrepre");
                            tx_dat_tDdest.Text = dr.GetString("tidodepre");
                            tx_numDocDes.Text = dr.GetString("nudodepre");
                            tx_nomDrio.Text = dr.GetString("nombdepre");
                            tx_dirDrio.Text = dr.GetString("diredepre");
                            tx_ubigDtt.Text = dr.GetString("ubigdepre");
                            tx_consig.Text = dr.GetString("clifinpre");
                            tx_dat_mone.Text = dr.GetString("tipmonpre");
                            tx_flete.Text = dr.GetDecimal("totpregui").ToString("#.##");
                            tx_totcant.Text = dr.GetString("cantotpre");
                            tx_totpes.Text = dr.GetDecimal("pestotpre").ToString("#.#");
                            tx_impreso.Text = dr.GetString("impreso");
                            tx_sergr.Text = dr.GetString("serguitra");
                            tx_numgr.Text = dr.GetString("numguitra");
                            claveSeg = dr.GetString("seguroE");
                            //
                            tx_docsOr.Text = dr.GetString("docsremit");
                            tx_docsOr2.Text = dr.GetString("docsremit2");
                            tx_rucEorig.Text = dr.GetString("rucDorig");
                            tx_rucEorig2.Text = dr.GetString("rucDorig2");
                            tx_dat_docOr.Text = dr.GetString("tidocor");
                            tx_dat_docOr2.Text = dr.GetString("tidocor2");
                        }
                        tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                        cmb_origen.SelectedValue = tx_dat_locori.Text;
                        cmb_origen_SelectionChangeCommitted(null, null);
                        cmb_destino.SelectedValue = tx_dat_locdes.Text;
                        cmb_destino_SelectionChangeCommitted(null, null);
                        cmb_docRem.SelectedValue = tx_dat_tdRem.Text;

                        cmb_docorig.SelectedValue = tx_dat_docOr.Text;
                        cmb_docorig_SelectionChangeCommitted(null, null);
                        cmb_docorig2.SelectedValue = tx_dat_docOr2.Text;
                        cmb_docorig2_SelectionChangeCommitted(null, null);

                        string[] du_remit = lib.retDPDubigeo(tx_ubigRtt.Text);
                        tx_dptoRtt.Text = du_remit[0];
                        tx_provRtt.Text = du_remit[1];
                        tx_distRtt.Text = du_remit[2];
                        cmb_docDes.SelectedValue = tx_dat_tDdest.Text;
                        string[] du_desti = lib.retDPDubigeo(tx_ubigDtt.Text);
                        tx_dptoDrio.Text = du_desti[0];
                        tx_proDrio.Text = du_desti[1];
                        tx_disDrio.Text = du_desti[2];
                        cmb_mon.SelectedValue = tx_dat_mone.Text;
                    }
                    else
                    {
                        MessageBox.Show("No existe el número buscado!", "Atención - data incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    //
                    dr.Dispose();
                    micon.Dispose();
                }
                conn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Fatal en codigo");
                Application.Exit();
                return;
            }
        }
        private void jaladet(string idr)         // jala el detalle
        {
            string jalad = "select id,serpregui,numpregui,cantprodi,unimedpro,codiprodi,descprodi,pesoprodi,precprodi,totaprodi " +
                "from detpregr where idc=@idr";
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
                            tx_det_cant.Text = row.ItemArray[3].ToString();
                            tx_det_umed.Text = row.ItemArray[4].ToString();
                            tx_det_desc.Text = row.ItemArray[6].ToString();
                            rb_kg.Checked = true;   // en carrion son kilos, si o si
                            tx_det_peso.Text = row.ItemArray[7].ToString();
                        }
                        dt.Dispose();
                    }
                }
            }
        }
        public void dataload()                  // jala datos para los combos 
        {
            //  datos para los combos de locales origen y destino
            cmb_origen.Items.Clear();
            cmb_origen.DataSource = Program.dt_definic.Select("idtabella='LOC'").CopyToDataTable() ;    // dtu;
            cmb_origen.DisplayMember = "descrizionerid";
            cmb_origen.ValueMember = "idcodice";
            //
            cmb_destino.Items.Clear();
            cmb_destino.DataSource = Program.dt_definic.Select("idtabella='LOC'").CopyToDataTable();    // dtd;
            cmb_destino.DisplayMember = "descrizionerid";
            cmb_destino.ValueMember = "idcodice";
            //  datos para los combobox de tipo de documento
            cmb_docRem.Items.Clear();
            cmb_docRem.DataSource = Program.dt_definic.Select("idtabella='DOC'").CopyToDataTable(); // dttd0;
            cmb_docRem.DisplayMember = "descrizionerid";
            cmb_docRem.ValueMember = "idcodice";
            //
            cmb_docDes.Items.Clear();
            cmb_docDes.DataSource = Program.dt_definic.Select("idtabella='DOC'").CopyToDataTable(); // dttd1;
            cmb_docDes.DisplayMember = "descrizionerid";
            cmb_docDes.ValueMember = "idcodice";
            // datos para el combo de moneda
            cmb_mon.Items.Clear();
            cmb_mon.DataSource = Program.dt_definic.Select("idtabella='MON'").CopyToDataTable(); // dtm;
            cmb_mon.DisplayMember = "descrizionerid";
            cmb_mon.ValueMember = "idcodice";
            // datos del documento origen 1
            cmb_docorig.DataSource = Program.dt_definic.Select("idtabella='DTM' and numero=1 and deta3='" + det3dtm + "' or deta4='" + det3dtm + "'").CopyToDataTable(); // dtdor;
            cmb_docorig.DisplayMember = "descrizione";
            cmb_docorig.ValueMember = "idcodice";
            // datos del documento origen 2
            cmb_docorig2.DataSource = Program.dt_definic.Select("idtabella='DTM' and numero=1 and deta3='" + det3dtm + "' or deta4='" + det3dtm + "'").CopyToDataTable(); // dtdor2;
            cmb_docorig2.DisplayMember = "descrizione";
            cmb_docorig2.ValueMember = "idcodice";
        }
        private void valiruc(object sender)     // valida los ruc del documento origen
        {
            TextBox campo = (TextBox)sender;

            if (campo.Text.Trim() != "" && (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR"))
            {
                if (lib.valiruc(campo.Text, vtc_ruc) == false)
                {
                    MessageBox.Show("Número de RUC inválido", "Atención - revise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    campo.Text = "";
                    campo.Focus();
                    return;
                }
                else
                {
                    datosR = lib.datossn("CLI", vtc_ruc, campo.Text.Trim());
                    if (datosR[0] != "")
                    {
                        MessageBox.Show("Razón Social: " + datosR[0], "Ruc encontrado en B.D.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        rl = lib.conectorSolorsoft("RUC", campo.Text);
                        MessageBox.Show("Razón Social: " + rl[0], "Ruc encontrado en conector", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        #region autocompletados
        private void autodepa()                             // se jala en el load
        {
            DataRow[] depar = Program.dt_ubigeos.Select("depart<>'00' and provin='00' and distri='00'");
            departamentos.Clear();
            foreach (DataRow row in depar)
            {
                departamentos.Add(row["nombre"].ToString());
            }
        }
        private void autoprov(string marca)                 // se jala despues de ingresado el departamento
        {
            DataRow[] provi = null;
            if (marca == "tx_ubigRtt")
            {
                provi = Program.dt_ubigeos.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
            }
            if (marca == "tx_ubigDtt")
            {
                provi = Program.dt_ubigeos.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
            }
            provincias.Clear();
            foreach (DataRow row in provi)
            {
                provincias.Add(row["nombre"].ToString());
            }
        }
        private void autodist(string marca)                 // se jala despues de ingresado la provincia
        {
            DataRow[] distr = null;
            if (marca == "tx_ubigRtt")
            {
                distr = Program.dt_ubigeos.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and distri<>'00'");
            }
            if (marca == "tx_ubigDtt")
            {
                distr = Program.dt_ubigeos.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigDtt.Text.Substring(2, 2) + "' and distri<>'00'");
            }
            distritos.Clear();
            foreach (DataRow row in distr)
            {
                distritos.Add(row["nombre"].ToString());
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
                if (oControls is CheckBox)
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
                if (oControls is GroupBox)
                {
                    oControls.Enabled = true;
                }
                if (oControls is CheckBox)
                {
                    oControls.Enabled = true;
                }
            }
            tx_dirOrigen.ReadOnly = true;
            tx_dirDestino.ReadOnly = true;
            tx_nomRem.ReadOnly = true;
            tx_dirRem.ReadOnly = true;
            tx_dptoRtt.ReadOnly = true;
            tx_provRtt.ReadOnly = true;
            tx_distRtt.ReadOnly = true;
            tx_nomDrio.ReadOnly = true;
            tx_dirDrio.ReadOnly = true;
            tx_dptoDrio.ReadOnly = true;
            tx_proDrio.ReadOnly = true;
            tx_disDrio.ReadOnly = true;
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
            chk_seguridad.Checked = false;
        }
        public void limpia_otros()
        {
            //GROUPBOX
            tx_det_cant.Text = "";
            tx_det_desc.Text = "";
            tx_det_peso.Text = "";
            tx_det_umed.Text = "";
        }
        public void limpia_combos()
        {
            cmb_origen.SelectedIndex = -1;
            cmb_destino.SelectedIndex = -1;
            cmb_docRem.SelectedIndex = -1;
            cmb_docDes.SelectedIndex = -1;
            cmb_mon.SelectedIndex = -1;
            cmb_docorig.SelectedIndex = -1;
            cmb_docorig2.SelectedIndex = -1;
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
            if (tx_serie.Text.Trim() == "")
            {
                tx_serie.Focus();
                return;
            }
            if (tx_dat_locori.Text.Trim() == "")
            {
                cmb_origen.Focus();
                return;
            }
            if (tx_dat_locdes.Text.Trim() == "")
            {
                cmb_destino.Focus();
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
                MessageBox.Show("Ingrese el valor del flete", " Atención ");
                tx_flete.Focus();
                return;
            }
            if (tx_totcant.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta cantidad ");
                //dataGridView1.Focus();
                return;
            }
            if (tx_totpes.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta peso ");
                //dataGridView1.Focus();
                return;
            }
            if (tx_dirRem.Text.Trim() != "" && (tx_dptoRtt.Text.Trim() == "" || tx_provRtt.Text.Trim() == "" || tx_distRtt.Text.Trim() == ""))
            {
                MessageBox.Show("Por favor, complete la dirección", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_dirRem.Focus();
                return;
            }
            if (tx_dirDrio.Text.Trim() != "" && (tx_dptoDrio.Text.Trim() == "" || tx_proDrio.Text.Trim() == "" || tx_disDrio.Text.Trim() == ""))
            {
                MessageBox.Show("Por favor, complete la dirección", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_dirDrio.Focus();
                return;
            }

            // NO HACEMOS VARIAS VALIDACIONES PORQUE VARIOS DATOS EN ESTE FORMULARIO SON OPCIONALES

            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // validaciones si es nuevo
                if (usoPGm == "manual" && tx_numero.Text.Trim() == "")
                {
                    MessageBox.Show("Debe ingresar el número del documento","Numeración MANUAL",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx_numero.Focus();
                    return;
                }

                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear la Pre Guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            /* var bb = MessageBox.Show("Desea imprimir la Pre Guía?" + Environment.NewLine +
                                "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (bb == DialogResult.Yes)
                            {
                                Bt_print.PerformClick();
                            } */
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
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número de la pre guía", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("La pre guía esta ANULADA", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Focus();
                    return;
                }
                if ((tx_sergr.Text.Trim() == "" && tx_numgr.Text.Trim() == "") && tx_impreso.Text == "N")
                {
                    // no tiene guía y no esta impreso => se puede modificar todo y SI anular
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar la Pre-guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            edita();
                        }
                        else
                        {
                            tx_dat_tdRem.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("La Pre-guía ya debe existir para editar", "Debe ser edición", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
                if ((tx_sergr.Text.Trim() == "" && tx_numgr.Text.Trim() == "") && tx_impreso.Text == "S")
                {
                    // no tiene guía y SI esta impreso => NO se puede modificar y SI anular
                    sololee(this);
                    MessageBox.Show("No se puede Modificar", "La Pre Guía esta impresa", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
                if ((tx_sergr.Text.Trim() != "" || tx_numgr.Text.Trim() != "") && tx_impreso.Text == "N")
                {
                    // si tiene guía y no esta impreso => NO se puede modificar NO anular
                    sololee(this);
                    MessageBox.Show("No se puede Modificar", "Tiene guía enlazada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
                if ((tx_sergr.Text.Trim() != "" || tx_numgr.Text.Trim() != "") && tx_impreso.Text == "S")
                {
                    // si tiene guía y si esta impreso => NO se puede modificar NO anular
                    sololee(this);
                    MessageBox.Show("No se puede Modificar", "Tiene guía enlazada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
            }
            if (modo == "ANULAR")
            {
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número de la pre gruía", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if ((tx_sergr.Text.Trim() == "" && tx_numgr.Text.Trim() == "") && tx_impreso.Text == "N")
                {
                    // no tiene guía y no esta impreso => se puede modificar todo y SI anular
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea ANULAR la Pre-guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            anula();
                            // veremos que mas hacemos aca
                        }
                        else
                        {
                            tx_dat_tdRem.Focus();
                            return;
                        }
                    }
                }
                if ((tx_sergr.Text.Trim() == "" && tx_numgr.Text.Trim() == "") && tx_impreso.Text == "S")
                {
                    // no tiene guía y SI esta impreso => NO se puede modificar y SI anular
                    sololee(this);
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea ANULAR la Pre-guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            anula();
                            // veremos que mas hacemos aca
                        }
                        else
                        {
                            tx_dat_tdRem.Focus();
                            return;
                        }
                    }
                }
                if ((tx_sergr.Text.Trim() != "" || tx_numgr.Text.Trim() != "") && tx_impreso.Text == "N")
                {
                    // si tiene guía y no esta impreso => NO se puede modificar NO anular
                    sololee(this);
                    MessageBox.Show("No se puede Anular", "Tiene guía enlazada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
                if ((tx_sergr.Text.Trim() != "" || tx_numgr.Text.Trim() != "") && tx_impreso.Text == "S")
                {
                    // si tiene guía y si esta impreso => NO se puede modificar NO anular
                    sololee(this);
                    MessageBox.Show("No se puede Anular", "Tiene guía enlazada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
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
                cmb_destino.Focus();
            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                // si estamos en GRABA es porque el modo es NUEVO entonces si el campo tx_numero.Text esta:
                // - en blanco, es porque la numeración es AUTOMATICA
                // - con valor, es porque la numeración en MANUAL 
                string yuca0 = "";
                string yuca1 = "";
                if (usoPGm == "manual") { yuca0 = "numpregui,"; yuca1 = "@npg,"; }
                try
                {
                    string inserta = "insert into cabpregr (" +
                        "fechpregr,serpregui," + yuca0 + "tidodepre,nudodepre,nombdepre,diredepre,ubigdepre," +
                        "tidorepre,nudorepre,nombrepre,direrepre,ubigrepre,locorigen,dirorigen,ubiorigen,locdestin," +
                        "dirdestin,ubidestin,obspregui,clifinpre,cantotpre,pestotpre,tipmonpre,tipcampre," +
                        "subtotpre,igvpregui,totpregui,totpagpre,salpregui,estadoser,seguroE,m1cliente,m2cliente," +
                        "tidocor,rucDorig,docsremit,tidocor2,rucDorig2,docsremit2," +
                        "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                        "values (@fechop,@serpgr," + yuca1 + "@tdcdes,@ndcdes,@nomdes,@dircde,@ubicde," +
                        "@tdcrem,@ndcrem,@nomrem,@dircre,@ubicre,@locpgr,@dirpgr,@ubopgr,@ldcpgr," +
                        "@didegr,@ubdegr,@obsprg,@conprg,@totcpr,@totppr,@monppr,@tcprgr," +
                        "@subpgr,@igvpgr,@totpgr,@pagpgr,@totpgr,@estpgr,@clavse,@m1clte,@m2clte," +
                        "@tdocor,@rucDor,@dooprg,@tidoc2,@rucDo2,@docsr2," +
                        "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                    MySqlCommand micon = new MySqlCommand(inserta, conn);
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6,4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@serpgr", tx_serie.Text);
                    if (usoPGm == "manual") micon.Parameters.AddWithValue("@npg", tx_numero.Text);
                    micon.Parameters.AddWithValue("@tdcdes", tx_dat_tDdest.Text);
                    micon.Parameters.AddWithValue("@ndcdes", tx_numDocDes.Text);
                    micon.Parameters.AddWithValue("@nomdes", tx_nomDrio.Text);
                    micon.Parameters.AddWithValue("@dircde", tx_dirDrio.Text);
                    micon.Parameters.AddWithValue("@ubicde", tx_ubigDtt.Text);
                    micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text); 
                    micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@ubicre", tx_ubigRtt.Text);
                    micon.Parameters.AddWithValue("@locpgr", tx_dat_locori.Text);
                    micon.Parameters.AddWithValue("@dirpgr", tx_dirOrigen.Text);
                    micon.Parameters.AddWithValue("@ubopgr", tx_ubigO.Text);
                    micon.Parameters.AddWithValue("@ldcpgr", tx_dat_locdes.Text);
                    micon.Parameters.AddWithValue("@didegr", tx_dirDestino.Text);
                    micon.Parameters.AddWithValue("@ubdegr", tx_ubigD.Text);
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);  // observaciones de la pre guia ... no hay
                    micon.Parameters.AddWithValue("@conprg", tx_consig.Text);
                    micon.Parameters.AddWithValue("@totcpr", tx_totcant.Text);
                    micon.Parameters.AddWithValue("@totppr", tx_totpes.Text);
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@tcprgr", "0.00");  // tipo de cambio ... falta leer de la tabla de cambios
                    micon.Parameters.AddWithValue("@subpgr", "0"); // sub total de la pre guía
                    micon.Parameters.AddWithValue("@igvpgr", "0"); // igv
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text); // total inc. igv
                    micon.Parameters.AddWithValue("@pagpgr", "0");
                    micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                    //micon.Parameters.AddWithValue("@ubiori", tx_ubigO.Text);
                    //micon.Parameters.AddWithValue("@ubides", tx_ubigD.Text);
                    micon.Parameters.AddWithValue("@clavse", claveSeg);
                    micon.Parameters.AddWithValue("@m1clte", v_clte_rem);
                    micon.Parameters.AddWithValue("@m2clte", v_clte_des);
                    micon.Parameters.AddWithValue("@tdocor", tx_dat_docOr.Text);                            // tipo del documento origen
                    micon.Parameters.AddWithValue("@rucDor", tx_rucEorig.Text);                             // ruc del emisor del doc origen
                    micon.Parameters.AddWithValue("@dooprg", tx_docsOr.Text);
                    micon.Parameters.AddWithValue("@tidoc2", tx_dat_docOr2.Text);
                    micon.Parameters.AddWithValue("@rucDo2", tx_rucEorig2.Text);
                    micon.Parameters.AddWithValue("@docsr2", tx_docsOr2.Text);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.ExecuteNonQuery();
                    //
                    string lectura = "select last_insert_id()";
                    micon = new MySqlCommand(lectura, conn);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        tx_idr.Text = dr.GetString(0);
                        if (usoPGm != "manual") tx_numero.Text = lib.Right("00000000" + dr.GetString(0),8);
                        dr.Close();
                        dr.Dispose();
                        // actualiza la tabla detalle,
                        string actua = "update detpregr set cantprodi=@can,unimedpro=@uni,codiprodi=@cod,descprodi=@des," +
                            "pesoprodi=@pes,precprodi=@preu,totaprodi=@pret " +
                            "where idc=@idr";
                        micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@can", tx_det_cant.Text);      // dataGridView1.Rows[0].Cells[0].Value.ToString()
                        micon.Parameters.AddWithValue("@uni", tx_det_umed.Text);      // dataGridView1.Rows[0].Cells[1].Value.ToString()
                        micon.Parameters.AddWithValue("@cod", "");
                        micon.Parameters.AddWithValue("@des", tx_det_desc.Text);      // dataGridView1.Rows[0].Cells[2].Value.ToString()
                        micon.Parameters.AddWithValue("@pes", tx_det_peso.Text);      // dataGridView1.Rows[0].Cells[3].Value.ToString()
                        micon.Parameters.AddWithValue("@preu", "0");
                        micon.Parameters.AddWithValue("@pret", "0");
                        micon.ExecuteNonQuery();
                        //
                        retorna = true;
                    }
                    dr.Close();
                    conn.Close();
                }
                catch(MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en insertar pre guía");
                    conn.Close();
                    //Application.Exit();
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
                    if (tx_impreso.Text == "N")     // EDICION DE CABECERA Y CONTROL 
                    {
                        string actua = "update cabpregr a, controlg b set " +
                            "a.fechpregr=@fechop,a.tidodepre=@tdcdes,a.nudodepre=@ndcdes," +
                            "a.nombdepre=@nomdes,a.diredepre=@dircde,a.ubigdepre=@ubicde,a.tidorepre=@tdcrem,a.nudorepre=@ndcrem," +
                            "a.nombrepre=@nomrem,a.direrepre=@dircre,a.ubigrepre=@ubicre,a.locorigen=@locpgr,a.dirorigen=@dirpgr," +
                            "a.ubiorigen=@ubopgr,a.locdestin=@ldcpgr,a.dirdestin=@didegr,a.ubidestin=@ubdegr,a.docsremit=@dooprg," +
                            "a.obspregui=@obsprg,a.clifinpre=@conprg,a.cantotpre=@totcpr,a.pestotpre=@totppr,a.tipmonpre=@monppr," +
                            "a.tipcampre=@tcprgr,a.subtotpre=@subpgr,a.igvpregui=@igvpgr,a.totpregui=@totpgr,a.totpagpre=@pagpgr," +
                            "a.salpregui=@totpgr,a.estadoser=@estpgr,a.seguroE=@clavse,m1cliente=@m1clte,m2cliente=@m2clte," +
                            "a.tidocor=@tdocor,a.rucDorig=@rucDor,a.docsremit=@dooprg,a.tidocor2=@tidoc2,a.rucDorig2=@rucDo2,a.docsremit2=@docsr2," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam," +
                            "b.tidodepre=@tdcdes,b.nudodepre=@ndcdes,b.tidorepre=@tdcrem,b.nudorepre=@ndcrem," +
                            "b.codmonpre=@monppr,b.totpregui=@totpgr,b.saldofina=@totpgr-b.totpagado " +
                            "where a.id=@idr and b.serpregui=a.serpregui and b.numpregui=a.numpregui";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@tdcdes", tx_dat_tDdest.Text);
                        micon.Parameters.AddWithValue("@ndcdes", tx_numDocDes.Text);
                        micon.Parameters.AddWithValue("@nomdes", tx_nomDrio.Text);
                        micon.Parameters.AddWithValue("@dircde", tx_dirDrio.Text);
                        micon.Parameters.AddWithValue("@ubicde", tx_ubigDtt.Text);
                        micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                        micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                        micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                        micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                        micon.Parameters.AddWithValue("@ubicre", tx_ubigRtt.Text);
                        micon.Parameters.AddWithValue("@locpgr", tx_dat_locori.Text);
                        micon.Parameters.AddWithValue("@dirpgr", tx_dirOrigen.Text);
                        micon.Parameters.AddWithValue("@ubopgr", tx_ubigO.Text);
                        micon.Parameters.AddWithValue("@ldcpgr", tx_dat_locdes.Text);
                        micon.Parameters.AddWithValue("@didegr", tx_dirDestino.Text);
                        micon.Parameters.AddWithValue("@ubdegr", tx_ubigD.Text);
                        micon.Parameters.AddWithValue("@obsprg", "");  // observaciones de la pre guia ... no hay
                        micon.Parameters.AddWithValue("@conprg", tx_consig.Text);
                        micon.Parameters.AddWithValue("@totcpr", tx_totcant.Text);
                        micon.Parameters.AddWithValue("@totppr", tx_totpes.Text);
                        micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                        micon.Parameters.AddWithValue("@tcprgr", "0.00");  // tipo de cambio
                        micon.Parameters.AddWithValue("@subpgr", "0"); // sub total de la pre guía
                        micon.Parameters.AddWithValue("@igvpgr", "0"); // igv
                        micon.Parameters.AddWithValue("@pagpgr", "0");
                        micon.Parameters.AddWithValue("@totpgr", tx_flete.Text); // saldo de la pre guia = total pre guia
                        micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                        micon.Parameters.AddWithValue("@clavse", claveSeg);
                        micon.Parameters.AddWithValue("@m1clte", v_clte_rem);
                        micon.Parameters.AddWithValue("@m2clte", v_clte_des);
                        micon.Parameters.AddWithValue("@tdocor", tx_dat_docOr.Text);                            // tipo del documento origen
                        micon.Parameters.AddWithValue("@rucDor", tx_rucEorig.Text);                             // ruc del emisor del doc origen
                        micon.Parameters.AddWithValue("@dooprg", tx_docsOr.Text);
                        micon.Parameters.AddWithValue("@tidoc2", tx_dat_docOr2.Text);
                        micon.Parameters.AddWithValue("@rucDo2", tx_rucEorig2.Text);
                        micon.Parameters.AddWithValue("@docsr2", tx_docsOr2.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE 
                        //
                        micon = new MySqlCommand("delete from detpregr where idc=@idr", conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.ExecuteNonQuery();
                        {   // dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != ""
                            if (true)
                            {
                                string inserd2 = "insert into detpregr (idc,serpregui,numpregui," +
                                    "cantprodi,unimedpro,codiprodi,descprodi,pesoprodi,precprodi,totaprodi," +
                                    "estadoser,verApp,userc,fechc,diriplan4,diripwan4,netbname " +
                                    ") values (@idr,@serpgr,@corpgr," +
                                    "@can,@uni,@cod,@des,@pes,@preu,@pret," +
                                    "@estpgr,@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                                micon = new MySqlCommand(inserd2, conn);
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@serpgr", tx_serie.Text);
                                micon.Parameters.AddWithValue("@corpgr", tx_numero.Text);
                                micon.Parameters.AddWithValue("@can", tx_det_cant.Text);  // dataGridView1.Rows[i].Cells[0].Value.ToString()
                                micon.Parameters.AddWithValue("@uni", tx_det_umed.Text);  // dataGridView1.Rows[i].Cells[1].Value.ToString()
                                micon.Parameters.AddWithValue("@cod", "");
                                micon.Parameters.AddWithValue("@des", tx_det_desc.Text);  // dataGridView1.Rows[i].Cells[2].Value.ToString()
                                micon.Parameters.AddWithValue("@pes", tx_det_peso.Text);  // dataGridView1.Rows[i].Cells[3].Value.ToString()
                                micon.Parameters.AddWithValue("@preu", "0");
                                micon.Parameters.AddWithValue("@pret", "0");
                                micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                                micon.Parameters.AddWithValue("@verApp", verapp);
                                micon.Parameters.AddWithValue("@asd", asd);
                                micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
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
                    MessageBox.Show(ex.Message, "Error en modificar la pre guía");
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
            // en el caso de pre guias SOLO HAY ANULACION FISICA, anulacion interna (serie ANU) en otros documentos
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabpregr, detpregr " +
                        "set cabpregr.estadoser=@estser,cabpregr.usera=@asd,cabpregr.fecha=now(),cabpregr.diriplan4=@dil4," +
                        "cabpregr.diripwan4=@diw4,cabpregr.netbname=@nbnp,cabpregr.verApp=@veap," +
                        "detpregr.estadoser=@estser,detpregr.usera=@asd,detpregr.fecha=now(),detpregr.verApp=@veap," +
                        "detpregr.diriplan4=@dil4,detpregr.diripwan4=@diw4,detpregr.netbname=@nbnp," +
                        "cabpregr.estintreg='A0',detpregr.estintreg='A0' " +
                        "where cabpregr.id=detpregr.idc and cabpregr.id=@idr";
                    // , controlg
                    // "controlg.estadoser=@estser " +
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
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
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
                chk_seguridad_CheckStateChanged(null,null);
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento del remitente, jala provincia
        {
            if(tx_dptoRtt.Text.Trim() != "")    //  && TransCarga.Program.vg_conSol == false
            {
                //DataRow[] row = dataUbig.Select("nombre='" + tx_dptoRtt.Text.Trim() + "' and provin='00' and distri='00'");
                DataRow[] row = Program.dt_ubigeos.Select("nombre='" + tx_dptoRtt.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = row[0].ItemArray[1].ToString(); // lib.retCodubigeo(tx_dptoRtt.Text.Trim(),"","");
                    autoprov("tx_ubigRtt");
                }
                else tx_dptoRtt.Text = "";
            }
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia del remitente
        {
            if(tx_provRtt.Text != "" && tx_dptoRtt.Text.Trim() != "")   //  && TransCarga.Program.vg_conSol == false
            {
                //DataRow[] row = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0,2) + "' and nombre='" + tx_provRtt.Text.Trim() + "' and provin<>'00' and distri='00'");
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and nombre='" + tx_provRtt.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = tx_ubigRtt.Text.Trim() + row[0].ItemArray[2].ToString();
                    autodist("tx_ubigRtt");
                }
                else tx_provRtt.Text = "";
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)         // distrito del remitente
        {
            if(tx_distRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "" && tx_dptoRtt.Text.Trim() != "")
            {   //  && TransCarga.Program.vg_conSol == false
                //DataRow[] row = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0,2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and nombre='" + tx_distRtt.Text.Trim() + "'");
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and nombre='" + tx_distRtt.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = tx_ubigRtt.Text.Trim() + row[0].ItemArray[3].ToString(); // lib.retCodubigeo(tx_distRtt.Text.Trim(),"",tx_ubigRtt.Text.Trim());
                }
                else tx_distRtt.Text = "";
            }
        }
        private void textBox13_Leave(object sender, EventArgs e)        // ubigeo del remitente
        {
            if(tx_ubigRtt.Text.Trim() != "" && tx_ubigRtt.Text.Length == 6 && TransCarga.Program.vg_conSol == false)
            {
                string[] du_remit = lib.retDPDubigeo(tx_ubigRtt.Text);
                tx_dptoRtt.Text = du_remit[0];
                tx_provRtt.Text = du_remit[1];
                tx_distRtt.Text = du_remit[2];
            }
        }
        private void tx_dptoDrio_Leave(object sender, EventArgs e)      // departamento del destinatario
        {
            if (tx_dptoDrio.Text.Trim() != "")  //  && TransCarga.Program.vg_conSol == false
            {
                //DataRow[] row = dataUbig.Select("nombre='" + tx_dptoDrio.Text.Trim() + "' and provin='00' and distri='00'");
                DataRow[] row = Program.dt_ubigeos.Select("nombre='" + tx_dptoDrio.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigDtt.Text = row[0].ItemArray[1].ToString(); // lib.retCodubigeo(tx_dptoDrio.Text.Trim(),"","");
                    autoprov("tx_ubigDtt");
                }
                else tx_dptoDrio.Text = "";
            }
        }
        private void tx_proDio_Leave(object sender, EventArgs e)      // provincia del destinatario
        {
            if (tx_proDrio.Text.Trim() != "" && tx_dptoDrio.Text.Trim() != "")  //  && TransCarga.Program.vg_conSol == false
            {
                //DataRow[] row = dataUbig.Select("depart='" + tx_ubigDtt.Text.Substring(0,2) + "' and nombre='" + tx_proDrio.Text.Trim() + "' and provin<>'00' and distri='00'");
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and nombre='" + tx_proDrio.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigDtt.Text = tx_ubigDtt.Text.Trim() + row[0].ItemArray[2].ToString(); // lib.retCodubigeo("", tx_proDrio.Text.Trim(), tx_ubigDtt.Text.Trim());
                    autodist("tx_ubigDtt");
                }
                else tx_proDrio.Text = "";
            }
        }
        private void tx_disDrio_Leave(object sender, EventArgs e)      // distrito del destinatario
        {
            if (tx_proDrio.Text.Trim() != "" && tx_dptoDrio.Text.Trim() != "" && tx_disDrio.Text.Trim() != "")
            {   //  && TransCarga.Program.vg_conSol == false
                //DataRow[] row = dataUbig.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigDtt.Text.Substring(2, 2) + "' and nombre='" + tx_disDrio.Text.Trim() + "'");
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigDtt.Text.Substring(2, 2) + "' and nombre='" + tx_disDrio.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_ubigDtt.Text = tx_ubigDtt.Text.Trim() + row[0].ItemArray[3].ToString(); // lib.retCodubigeo(tx_disDrio.Text.Trim(), "", tx_ubigDtt.Text.Trim());
                }
                else tx_disDrio.Text = "";
            }
        }
        private void tx_ubigDtt_Leave(object sender, EventArgs e)      // ubigeo destinatario
        {
            if (tx_ubigDtt.Text.Trim() != "" && tx_ubigDtt.Text.Length == 6 && TransCarga.Program.vg_conSol == false)
            {
                string[] du_desti = lib.retDPDubigeo(tx_ubigDtt.Text);
                tx_dptoDrio.Text = du_desti[0];
                tx_proDrio.Text = du_desti[1];
                tx_disDrio.Text = du_desti[2];
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
                    tx_nomRem.Text = "";
                    tx_dirRem.Text = "";
                    tx_dptoRtt.Text = "";
                    tx_provRtt.Text = "";
                    tx_distRtt.Text = "";
                    tx_ubigRtt.Text = "";
                    v_clte_rem = "";         // variable para marcar si el remitente es cliente nuevo "N" o para actualizar sus datos "E"
                    string[] datos = lib.datossn("CLI", tx_dat_tdRem.Text.Trim(), tx_numDocRem.Text.Trim());
                    if (datos[0] != "")
                    {
                        tx_nomRem.Text = datos[0];
                        tx_dirRem.Text = datos[1];
                        tx_dptoRtt.Text = datos[2];
                        tx_provRtt.Text = datos[3];
                        tx_distRtt.Text = datos[4];
                        tx_ubigRtt.Text = datos[5];
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
                                v_clte_rem = "N";
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
                                v_clte_rem = "N";
                            }
                        }
                    }
                    if (tx_dirRem.Text.Trim() == "")
                    {
                        tx_dirRem.ReadOnly = false;
                        tx_dptoRtt.ReadOnly = false;
                        tx_provRtt.ReadOnly = false;
                        tx_distRtt.ReadOnly = false;
                    }
                    cmb_docDes.Focus();
                }
            }
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() == "")
            {
                cmb_docRem.Focus();
            }
        }
        private void tx_numDocDes_Leave(object sender, EventArgs e)     // numero documento destinatario
        {
            if (tx_numDocDes.Text.Trim() != "" && tx_mldD.Text.Trim() != "")
            {
                if (tx_numDocDes.Text.Trim().Length != Int16.Parse(tx_mldD.Text))
                {
                    MessageBox.Show("El número de caracteres para" + Environment.NewLine +
                        "su tipo de documento debe ser: " + tx_mldD.Text, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_numDocDes.Focus();
                    return;
                }
                if (tx_dat_tDdest.Text == vtc_ruc && lib.valiruc(tx_numDocDes.Text, vtc_ruc) == false)
                {
                    MessageBox.Show("Número de RUC inválido", "Atención - revise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx_numDocDes.Focus();
                    return;
                }
                string encuentra = "no";
                if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
                {
                    tx_nomDrio.Text = "";
                    tx_dirDrio.Text = "";
                    tx_dptoDrio.Text = "";
                    tx_proDrio.Text = "";
                    tx_disDrio.Text = "";
                    tx_ubigDtt.Text = "";
                    v_clte_des = "";
                    string[] datos = lib.datossn("CLI", tx_dat_tDdest.Text.Trim(), tx_numDocDes.Text.Trim());
                    if (datos[0] != "")   // datos.Length > 0
                    {
                        tx_nomDrio.Text = datos[0];
                        tx_dirDrio.Text = datos[1];
                        tx_dptoDrio.Text = datos[2];
                        tx_proDrio.Text = datos[3];
                        tx_disDrio.Text = datos[4];
                        tx_ubigDtt.Text = datos[5];
                        encuentra = "si";
                    }
                    if (tx_dat_tDdest.Text == vtc_ruc)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                            {
                                string[] rl = lib.conectorSolorsoft("RUC", tx_numDocDes.Text);
                                tx_nomDrio.Text = rl[0];      // razon social
                                tx_ubigDtt.Text = rl[1];     // ubigeo
                                tx_dirDrio.Text = rl[2];      // direccion
                                tx_dptoDrio.Text = rl[3];      // departamento
                                tx_proDrio.Text = rl[4];      // provincia
                                tx_disDrio.Text = rl[5];      // distrito
                                v_clte_des = "N";
                            }
                        }
                    }
                    if (tx_dat_tDdest.Text == vtc_dni)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para dni
                            {
                                string[] rl = lib.conectorSolorsoft("DNI", tx_numDocDes.Text);
                                tx_nomDrio.Text = rl[0];      // nombre
                                //tx_numDocDes.Text = rl[1];     // num dni
                                v_clte_des = "N";
                            }
                        }
                    }
                    if (tx_dirDrio.Text.Trim() == "")
                    {
                        tx_dirDrio.ReadOnly = false;
                        tx_dptoDrio.ReadOnly = false;
                        tx_proDrio.ReadOnly = false;
                        tx_disDrio.ReadOnly = false;
                    }
                }
                cmb_docorig.Focus();     // tx_docsOr.Focus();
            }
            if (tx_numDocDes.Text.Trim() != "" && tx_mldD.Text.Trim() == "")
            {
                cmb_docDes.Focus();
            }
        }
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO")
            {
                if (usoPGm != "manual")
                {
                    // en el caso de las pre guias el numero es el mismo que el ID del registro
                    tx_numero.Text = lib.Right("00000000" + tx_numero.Text, 8);
                    tx_idr.Text = tx_numero.Text;
                    jalaoc("tx_idr");
                }
                else
                {
                    jalaoc("sernum");
                }
                jaladet(tx_idr.Text);

                chk_seguridad_CheckStateChanged(null, null);
                if ((tx_sergr.Text.Trim() == "" && tx_numgr.Text.Trim() == "") && tx_impreso.Text == "N")
                {
                    // no tiene guía y no esta impreso => se puede modificar todo y SI anular
                }
                if ((tx_sergr.Text.Trim() == "" && tx_numgr.Text.Trim() == "") && tx_impreso.Text == "S")
                {
                    // no tiene guía y SI esta impreso => NO se puede modificar y SI anular
                    sololee(this);
                }
                if ((tx_sergr.Text.Trim() != "" || tx_numgr.Text.Trim() != "") && tx_impreso.Text == "N")
                {
                    // si tiene guía y no esta impreso => NO se puede modificar NO anular
                    sololee(this);
                }
                if ((tx_sergr.Text.Trim() != "" || tx_numgr.Text.Trim() != "") && tx_impreso.Text == "S")
                {
                    // si tiene guía y si esta impreso => NO se puede modificar NO anular
                    sololee(this);
                }
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
        }
        private void tx_det_peso_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_det_peso.Text != "") 
            {
                tx_totpes.Text = tx_det_peso.Text;
                tx_flete.Focus();
            }
        }
        private void tx_det_cant_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_det_cant.Text != "")
            {
                tx_totcant.Text = tx_det_cant.Text;
                tx_det_umed.Focus();
            }
        }
        private void tx_flete_Leave(object sender, EventArgs e)
        {
            button1.Focus();
        }
        private void chk_seguridad_CheckStateChanged(object sender, EventArgs e)
        {
            if (chk_seguridad.Checked == false)
            {
                if (claveSeg != "") chk_seguridad.Checked = true;
            }
        }
        private void chk_seguridad_Click(object sender, EventArgs e)
        {
            if (chk_seguridad.Checked == true)
            {
                string para1 = claveSeg;
                vclave ayu1 = new vclave(para1);
                var result = ayu1.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    claveSeg = ayu1.ReturnValue1;
                    if (claveSeg == "") chk_seguridad.Checked = false;
                }
            }
        }
        private void tx_docsOr_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_docsOr);
        }
        private void tx_docsOr2_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_docsOr2);
        }
        private void val_NoCaracteres(TextBox textBox)
        {
            if (caractNo != "")
            {
                int index = textBox.Text.IndexOf(caractNo);
                if (index > -1)
                {
                    char cno = caractNo.ToCharArray()[0];
                    textBox.Text = textBox.Text.Replace(cno, ' ');
                }
            }
        }
        private void tx_rucEorig_Leave(object sender, EventArgs e)              // validamos el ruc del emisor documento origen 1
        {
            valiruc(tx_rucEorig);
        }
        private void tx_rucEorig2_Leave(object sender, EventArgs e)              // validamos el ruc del emisor documento origen 2
        {
            valiruc(tx_rucEorig2);
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
            escribe(this);
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            tx_serie.Text = "";
            initIngreso();  // limpiamos/preparamos todo para el ingreso
            gbox_flete.Enabled = true;
            tx_numero.Text = "";
            tx_numero.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            escribe(this);
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            tx_numero.Text = "";
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
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
            sololee(this);
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            initIngreso();
            gbox_serie.Enabled = true;
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
        }
        private void Bt_ver_Click(object sender, EventArgs e)
        {
            sololee(this);
            Tx_modo.Text = "VISUALIZAR";
            button1.Image = Image.FromFile(img_ver);
            initIngreso();
            gbox_serie.Enabled = true;
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar(this);
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
            tx_idr.Text = lib.golast(nomtab);
            tx_idr_Leave(null, null);
        }
        #endregion botones;
        // proveed para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        // IDTabella,IDCodice,Descrizione,DescrizioneRid,Numero,cnt,codigo,codsunat,deta1,deta2,deta3,deta4,ubiDir,marca1,marca2,marca3,enlace1
        private void cmb_docRem_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docRem.SelectedIndex > -1)
            {
                tx_dat_tdRem.Text = cmb_docRem.SelectedValue.ToString();
                //DataRow[] fila = dttd0.Select("idcodice='" + tx_dat_tdRem.Text + "'");
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_tdRem.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mld.Text = row["codigo"].ToString();
                }
            }
        }
        private void cmb_docDes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docDes.SelectedIndex > -1)
            {
                tx_dat_tDdest.Text = cmb_docDes.SelectedValue.ToString();
                //DataRow[] fila = dttd1.Select("idcodice='" + tx_dat_tDdest.Text + "'");
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_tDdest.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mldD.Text = row["codigo"].ToString();
                }
            }
        }
        private void cmb_mon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_mon.SelectedIndex > -1)
            {
                tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
            }
        }
        private void cmb_origen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_origen.SelectedIndex > -1)
            {
                tx_dat_locori.Text = cmb_origen.SelectedValue.ToString();
                tx_dirOrigen.Text = lib.dirloca(lib.codloc(asd));
            }
            // lo de arriba viene del selectedindexhcnaged
            if (tx_dat_locori.Text.Trim() != "")
            {
                //DataRow[] fila = dtu.Select("idcodice='" + tx_dat_locori.Text + "'");
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locori.Text + "'");
                tx_ubigO.Text = fila[0]["ubiDir"].ToString();     // fila[0][2].ToString();
            }
        }
        private void cmb_destino_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_destino.SelectedIndex > -1)
            {
                tx_dat_locdes.Text = cmb_destino.SelectedValue.ToString();
                tx_dirDestino.Text = lib.dirloca(tx_dat_locdes.Text);
                // direccion del pto de emision [tipdoc=preguia][est_anulado][origen][destino]
                //string newSer = lib.serOrgDes(tx_dat_tdi.Text, codAnul, tx_dat_locori.Text, tx_dat_locdes.Text);
                //MessageBox.Show(newSer, "Nueva serie");
            }
            // lo de arriba viene del selectedindexhcnaged
            if (tx_dat_locdes.Text.Trim() != "")
            {
                //DataRow[] fila = dtd.Select("idcodice='" + tx_dat_locdes.Text + "'");
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locdes.Text + "'");
                tx_ubigD.Text = fila[0]["ubiDir"].ToString();     // fila[0][2].ToString();
            }
        }
        private void cmb_docorig_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docorig.SelectedIndex > -1)
            {
                tx_dat_docOr.Text = cmb_docorig.SelectedValue.ToString();
                if (tx_dat_docOr.Text.Trim() != "")
                {
                    DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_docOr.Text + "'");
                    if (fila[0]["marca1"].ToString() == "1")              // sunat permite 2 documntos relacionados 
                    {
                        //cmb_docorig2.Enabled = true;
                        if (tccmr.Contains(fila[0]["codsunat"].ToString()) && tx_dat_tdRem.Text == vtc_ruc) tx_rucEorig.Text = tx_numDocRem.Text;
                        else tx_rucEorig.Text = "";
                        //tx_rucEorig2.Enabled = true;
                        //tx_rucEorig2.ReadOnly = false;
                    }
                    else
                    {
                        cmb_docorig2.SelectedIndex = -1;
                        cmb_docorig2.Enabled = false;
                        tx_docsOr2.Text = "";
                        tx_dat_docOr2.Text = "";
                        tx_rucEorig2.Text = "";
                    }
                }
                if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
                {
                    tx_docsOr.ReadOnly = false;
                    tx_rucEorig.ReadOnly = false;
                }
            }
            else
            {
                tx_docsOr.Text = "";
                tx_docsOr.ReadOnly = true;
                tx_rucEorig.Text = "";
                tx_rucEorig.ReadOnly = true;
                // debe ir en orden, no puede haber un segundo documento si el primero esta vacío
                cmb_docorig2.SelectedIndex = -1;
                tx_dat_docOr2.Text = "";
                tx_docsOr2.Text = "";
                tx_docsOr.ReadOnly = true;
                tx_rucEorig2.Text = "";
                tx_rucEorig2.ReadOnly = true;
            }
        }
        private void cmb_docorig2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docorig2.SelectedIndex > -1)
            {
                tx_dat_docOr2.Text = cmb_docorig2.SelectedValue.ToString();
                if (tx_dat_docOr2.Text.Trim() != "")
                {
                    DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_docOr2.Text + "'");
                    if (fila[0]["marca1"].ToString() == "1")              // sunat permite 2 documntos relacionados 
                    {
                        cmb_docorig2.Enabled = true;
                        if (tccmr.Contains(fila[0]["codsunat"].ToString()) && tx_dat_tdRem.Text == vtc_ruc) tx_rucEorig2.Text = tx_numDocRem.Text;
                        else tx_rucEorig2.Text = "";
                    }
                }
            }
            else
            {
                //
            }
        }
        private void cmb_docorig2_KeyDown(object sender, KeyEventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
            {
                if (e.KeyCode == Keys.Delete)
                {
                    cmb_docorig2.SelectedIndex = -1;
                    tx_dat_docOr2.Text = "";
                    tx_docsOr2.Text = "";
                    tx_docsOr2.ReadOnly = true;
                    tx_rucEorig2.Text = "";
                    tx_rucEorig2.ReadOnly = true;
                }
            }
        }
        private void cmb_docorig_KeyDown(object sender, KeyEventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
            {
                if (e.KeyCode == Keys.Delete)
                {
                    cmb_docorig.SelectedIndex = -1;
                    tx_dat_docOr.Text = "";
                    tx_docsOr.Text = "";
                    tx_docsOr.ReadOnly = true;
                    tx_rucEorig.Text = "";
                    tx_rucEorig.ReadOnly = true;
                }
            }
        }

        #endregion comboboxes

        #region datagridview
        // nada
        #endregion

        #region impresion
        private bool imprimeA5()
        {
            bool retorna = false;
            // jala los parametros de impresion
            try
            {
                //printDocument1.PrinterSettings.PrinterName = v_impA5;
                //printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 148, 210);
                PrintDocument printDoc = new PrintDocument();
                var paperSize = printDoc.PrinterSettings.PaperSizes.Cast<PaperSize>().FirstOrDefault(e => e.PaperName == "A5");
                printDoc.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                printDocument1.Print();
                retorna = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error en impresion A5");
                retorna = false;
            }
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
                MessageBox.Show(ex.Message, "Error en imprimir TK");
                retorna = false;
            }
            return retorna;
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (vi_formato == "A5")
            {
                imprime_A5(sender, e);
            }
            if (vi_formato == "TK")
            {
                imprime_TK(sender, e);
            }
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
            string numguia = "PRE GUIA NRO. " + tx_serie.Text + "-" + tx_numero.Text;
            float lt = (CentimeterToPixel(21F) - e.Graphics.MeasureString(numguia, lt_titB).Width) / 2;
            puntoF = new PointF(lt, posi);
            e.Graphics.DrawString(numguia, lt_titB, Brushes.Black, puntoF, StringFormat.GenericTypographic);                      // titulo del reporte
            posi = posi + alfi*2;
            PointF ptoimp = new PointF(coli, posi);                     // fecha de emision
            e.Graphics.DrawString("EMITIDO: " + tx_fechope.Text.Substring(0,10), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi + 30.0F;                                         // avance de fila
            ptoimp = new PointF(coli, posi);                               // direccion partida
            e.Graphics.DrawString("PARTIDA: " + tx_dirOrigen.Text.Trim(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi + 30.0F;
            ptoimp = new PointF(coli, posi);                      // direccion llegada
            e.Graphics.DrawString("DESTINO: " + tx_dirDestino.Text.Trim(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi + 30.0F;
            ptoimp = new PointF(coli, posi);                                // remitente
            e.Graphics.DrawString("REMITENTE: " + tx_nomRem.Text.Trim(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi;
            ptoimp = new PointF(coli, posi);                       // destinatario
            e.Graphics.DrawString("DESTINATARIO: " + tx_nomDrio.Text.Trim(), lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
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
            {
                ptoimp = new PointF(coli + 20.0F, posi);
                e.Graphics.DrawString("", lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                ptoimp = new PointF(cold, posi);
                e.Graphics.DrawString("", lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                ptoimp = new PointF(cold + 80.0F, posi);
                e.Graphics.DrawString("", lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                ptoimp = new PointF(cold + 400.0F, posi);
                e.Graphics.DrawString("KGs." + "", lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
                posi = posi + alfi;             // avance de fila
            }
            // guias del cliente
            posi = posi + alfi;
            ptoimp = new PointF(coli, posi);
            e.Graphics.DrawString("Docs. de remisión: " + tx_docsOr.Text, lt_tit, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            // imprime el flete
            posi = posi + alfi * 2;
            string gtotal = "FLETE " + cmb_mon.Text + " " + tx_flete.Text;
            lt = (CentimeterToPixel(21F) - e.Graphics.MeasureString(gtotal, lt_titB).Width) / 2;
            ptoimp = new PointF(lt, posi);
            e.Graphics.DrawString(gtotal, lt_titB, Brushes.Black, ptoimp, StringFormat.GenericTypographic);
            posi = posi + alfi;

        }
        private void imprime_TK(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // TIPOS DE LETRA PARA EL DOCUMENTO FORMATO TICKET
            Font lt_gra = new Font("Arial", 10);                // grande
            Font lt_med = new Font("Arial", 9);                 // normal textos
            Font lt_peq = new Font("Arial", 8);                 // pequeño
            //
            float anchTik = 7.8F;                               // ancho del TK en centimetros
            int coli = 5;                                       // columna inicial
            float posi = 20;                                    // posicion x,y inicial
            int alfi = 20;                                      // alto de cada fila
            int copias = int.Parse(vi_copias);                  // cantidad de copias del ticket
            SizeF cuad = new SizeF();
            for (int i = 1; i <= copias; i++)
            {
                float lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(nomclie, lt_gra).Width) / 2;
                PointF puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(nomclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                //string tipdo = "PRE GUIA";
                string serie = tx_serie.Text.Trim();
                string corre = tx_numero.Text.Trim();
                //string nota = tipdo + " " + serie + "-" + corre;
                posi = posi + alfi + 8;
                string titnum = "PRE_GUIA " + serie + " - " + corre;
                lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                puntoF = new PointF(lt, posi);
                e.Graphics.DrawString(titnum, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("EMITIDO ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 65, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 70, posi);
                e.Graphics.DrawString(tx_fechope.Text.Substring(0,10), lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("PARTIDA ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 65, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 70, posi);
                if (tx_dirOrigen.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik) - 80, alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik) - 80, alfi * 1);
                RectangleF recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(tx_dirOrigen.Text, lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                if (tx_dirOrigen.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                else posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("DESTINO ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 65, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 70, posi);
                if (tx_dirDestino.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik) - 80, alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik) - 80, alfi * 1);
                recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(tx_dirDestino.Text, lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                if (tx_dirDestino.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                else posi = posi + alfi;
                puntoF = new PointF(coli, posi);

                e.Graphics.DrawString("REMITENTE ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 75, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 80, posi);
                if (tx_nomRem.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik) - 90, alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik) - 90, alfi * 1);
                recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(tx_nomRem.Text.Trim(), lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                if (tx_nomRem.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                else posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("DESTINAT. ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 75, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 80, posi);
                if (tx_nomDrio.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik) - 90, alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik) - 90, alfi * 1);
                recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(tx_nomDrio.Text.Trim(), lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                if (tx_nomDrio.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                else posi = posi + alfi * 2;

                puntoF = new PointF(coli, posi);
                // **************** detalle del documento ****************//
                e.Graphics.DrawString("DETALLE DEL ENVIO", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                StringFormat alder = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                SizeF siz = new SizeF(70, 15);
                RectangleF recto = new RectangleF(puntoF, siz);
                {
                    puntoF = new PointF(coli + 20.0F, posi);
                    e.Graphics.DrawString(tx_det_cant.Text + " " + tx_det_umed.Text.Trim() + " " + tx_det_desc.Text.Trim(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20.0F, posi);
                    e.Graphics.DrawString("PESO " + tx_det_peso.Text + " " + ((rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                }
                // pie del documento ;
                posi = posi + alfi;
                string flete = "FLETE " + cmb_mon.Text + " " + tx_flete.Text;
                lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(flete, lt_gra).Width) / 2;
                puntoF = new PointF(lt, posi);
                e.Graphics.DrawString(flete, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi * 2;
                // leyenda 4
                string leyenda4 = "Documento sin valor legal";
                lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(leyenda4, lt_med).Width) / 2;
                puntoF = new PointF(lt, posi);
                e.Graphics.DrawString(leyenda4, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic); 
                posi = posi + alfi;
                string locyus = cmb_origen.Text + " - " + tx_user.Text;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(locyus, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // tienda y vendedor
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("Imp. " + DateTime.Now, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi + alfi;
                string despedida ="www.solorsoft.com/transcarga";
                puntoF = new PointF((CentimeterToPixel(anchTik) - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                e.Graphics.DrawString(despedida, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi + alfi;
            }
        }
        private void updateprint(string sn)  // actualiza el campo impreso de la GR = S
        {   // S=si impreso || N=no impreso
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "update cabpregr set impreso=@sn where id=@idr";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@sn", sn);
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.ExecuteNonQuery();
                }
            }
        }
        int CentimeterToPixel(double Centimeter)
        {
            double pixel = -1;
            using (Graphics g = this.CreateGraphics())
            {
                pixel = Centimeter * g.DpiY / 2.54d;
            }
            return (int)pixel;
        }
        #endregion

    }
}
