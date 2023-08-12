using System;
using System.Net;
using Newtonsoft.Json; // using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Drawing.Imaging;

namespace TransCarga
{
    public partial class guiari_e : Form
    {
        static string nomform = "guiari_e";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabguiar";              // cabecera de guias REMITENTE

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
        string gloDeta = "";            // glosa x defecto en el detalle
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        string v_cid = "";              // codigo interno de tipo de documento
        string v_fra1 = "";             // frase de si va o no con clave
        string v_fra2 = "";             // frase 
        string v_sanu = "";             // serie anulacion interna ANU
        string v_CR_gr_ind = "";        // nombre del formato GR individual en CR
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_clte_rem = "";         // variable para marcar si el remitente es cliente nuevo "N" o para actualizar sus datos "E"
        string v_clte_des = "";         // variable para marcar si el destinatario es cliente nuevo "N" o para actualizar sus datos "E"
        string v_igv = "";              // igv
        string caractNo = "";           // caracter prohibido en campos texto, caracter delimitador para los TXT de fact. electronica
        string v_uedo = "";             // usuarios que pueden modificar campo Docs. Origen
        string client_id_sunat = "";    // id del cliente api sunat para guias electrónicas 
        string client_pass_sunat = "";  // clave api sunat para guias electrónicas
        string u_sol_sunat = "";        // usuario sol sunat del cliente
        string c_sol_sunat = "";        // clave sol sunat del cliente
        string scope_sunat = "";        // scope sunat del api
        string cGR_sunat = "";          // codigo sunat para GR transportista
        string usa_gre = "";            // usa GRE en la organización? S/N
        string rutatxt = "";            // ruta para las guias de remision electronicas
        string rutaxml = "";            // ruta para los XML de las guias de remision
        string tipdo = "";              // CODIGO SUNAT tipo de documento guia remision transportista
        string tipoDocEmi = "";         // CODIGO SUNAT tipo de documento RUC/DNI emisor
        string tipoDocRem = "";         // CODIGO SUNAT tipo de documento RUC/DNI remitente de la GRT
        string tipoDocDes = "";         // CODIGO SUNAT tipo de documento RUC/DNI destinatario de la GRT
        string v_urege = "";            // usuarios que pueden regenerar txt
        string v_uagin = "";            // usuarios que pueden hacer anulacion interna
        string webdni = "";             // ruta web del buscador de DNI
        string NoRetGl = "";            // glosa de retorno cuando umasapa no encuentra el dni o ruc
        string wsPostS = "";            // ruta del webservice de sunat para postear el xml/zip
        // GRE
        string v_marGRET = "";          // marca de guía de remisión electrónica
        string v_iniGRET = "";          // sigla, inicicla, marca de las GRE-T
        string logoclt = "";            // logo 
        string otro = "";               // ruta y nombre del png código QR
        string ipeeg = "";              // identificador de proveedor de emisor electrónico 
        string despedida = "Gracias por su confianza en nosotros";
        string firmDocElec = "";        // Firma xml, true=firma, false=no firma
        string rutaCertifc = "";        // Ruta y nombre del certificado .pfx
        string claveCertif = "";        // Clave del certificado
        string despedid2 = "";          // despedida del ticket 2
        string glosa1 = "";             // glosa comprobante final 1
        string glosa2 = "";             // 
        string codtrasli = "";          // codigo de traslado interno entre establacimientos de la misma empresa
        string codtrasit = "";          // codigo de traslado interno ITINERANTE
        string det3dtm = "";            // palabra nombre descriptivo de las guias de remision electronicas de transportista

        double tiempoT = 0;             // Sunat Webservice - contador EN SEGUNDOS de vigencia del token
        string TokenAct = "";           // Sunat Webservice - Token actual vigente
        TimeSpan horaT;                 // Sunat Webservice - Hora de emisión del token
        int plazoT = 0;                 // Sunat Webservice - Cantidad en segundos
        string[] c_t = new string[6] { "", "", "", "", "", ""}; // parametros para generar el token
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string claveSeg = "";                       // clave de seguridad del envío
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string ubiclie = Program.ubidirfis;         // ubigeo direc fiscal
        string asd = Program.vg_user;               // usuario conectado al sistema
        string nRegMTC = Program.regmtc;            // numero registro del MTC
        #endregion

        publico lp = new publico();             // libreria de clases
        acGRE_sunat _Sunat = new acGRE_sunat();

        AutoCompleteStringCollection departamentos = new AutoCompleteStringCollection();// autocompletado departamentos
        AutoCompleteStringCollection provincias = new AutoCompleteStringCollection();   // autocompletado provincias
        AutoCompleteStringCollection distritos = new AutoCompleteStringCollection();    // autocompletado distritos
        AutoCompleteStringCollection desdet = new AutoCompleteStringCollection();       // autompletatado descripcion detalle
        //AutoCompleteStringCollection bultos = new AutoCompleteStringCollection();       // autompletatado bultos del detalle
        DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public static string CadenaConexion = "Data Source=TransCarga.db";  // Data Source=TransCarga;Mode=Memory;Cache=Shared

        DataTable dtu = new DataTable();            // local origen
        DataTable dtd = new DataTable();            // local destino 
        DataTable dttd0 = new DataTable();
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();
        DataTable dttdv = new DataTable();          // tipo documentos guias
        DataTable dtdor = new DataTable();          // tipos de documentos origen 1 de un transporte de mercancia ..segun sunat
        DataTable dtdor2 = new DataTable();         // tipos de documentos origen 2 de un transporte de mercancia ..segun sunat
        DataTable dtmt = new DataTable();           // motivos de traslado 
        DataTable tcfe = new DataTable();           // GRT electronica - cabecera
        DataTable tdfe = new DataTable();           // GRT electronica -detalle
        string[] datosR = { "" };                   // datos del remitente si existe en la B.D.
        string[] datosD = { "" };                   // datos del destinatario si existe en la B.D.
        string[] rl = { "" };                       // datos del NUEVO remitente
        string[] dl = { "" };                       // datos del NUEVO destinatario
        
        public guiari_e()
        {
            InitializeComponent();
        }
        private void guiari_e_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void guiari_e_Load(object sender, EventArgs e)
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
            //backgroundWorker1.RunWorkerAsync();     // 08/03/2023
            //
            //this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            //dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //gbox_planilla.BackColor = Color.FromName(colpage);
            //
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            autodepa();                                     // autocompleta departamentos
            //armagret();
            CreaTablaLiteGRE();                             // creamos la tabla sqlite para las guías metodo SFS 1.9
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
            //tx_det_umed.AutoCompleteMode = AutoCompleteMode.Suggest;
            //tx_det_umed.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //tx_det_umed.AutoCompleteCustomSource = bultos; //;
            tx_det_desc.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_det_desc.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_det_desc.AutoCompleteCustomSource = desdet; //;
            // longitudes maximas de campos
            tx_det_umed.MaxLength = 14;
            tx_det_desc.MaxLength = 100;
            //
            tx_pregr_num.MaxLength = 8;
            tx_serie.MaxLength = 4;         // serie pre guia
            tx_numero.MaxLength = 8;        // numero pre guia
            tx_dirRem.MaxLength = 100;
            tx_nomRem.MaxLength = 100;           // nombre remitente
            tx_distRtt.MaxLength = 45;
            tx_provRtt.MaxLength = 45;
            tx_dptoRtt.MaxLength = 45;
            tx_nomDrio.MaxLength = 100;           // nombre destinatario
            tx_dirDrio.MaxLength = 100;
            tx_disDrio.MaxLength = 45;
            tx_proDrio.MaxLength = 45;
            tx_dptoDrio.MaxLength = 45;
            tx_docsOr.MaxLength = 100;          // documentos origen del traslado
            tx_consig.MaxLength = 100;
            tx_obser1.MaxLength = 150;
            tx_telD.MaxLength = 19;
            tx_telR.MaxLength = 19;
            tx_docsOr.MaxLength = 45;           // documento origen de la GRT
            tx_docsOr2.MaxLength = 45;
            tx_rucEorig.MaxLength = 11;         // ruc del emisor del documento origen
            tx_rucEorig2.MaxLength = 11;
            tx_motras.MaxLength = 100;          // motivo de traslado, limite de sunat es 100 caracteres 18/07/2023
            tx_dat_plaNreg.MaxLength = 20;
            tx_dat_carrNreg.MaxLength = 20;
            tx_pla_autor.MaxLength = 15;
            tx_aut_carret.MaxLength = 15;
            // 
            tx_nomRem.CharacterCasing = CharacterCasing.Upper;
            tx_dirRem.CharacterCasing = CharacterCasing.Upper;
            tx_dptoRtt.CharacterCasing = CharacterCasing.Upper;
            tx_provRtt.CharacterCasing = CharacterCasing.Upper;
            tx_distRtt.CharacterCasing = CharacterCasing.Upper;
            tx_nomDrio.CharacterCasing = CharacterCasing.Upper;
            tx_dirDrio.CharacterCasing = CharacterCasing.Upper;
            tx_dptoDrio.CharacterCasing = CharacterCasing.Upper;
            tx_proDrio.CharacterCasing = CharacterCasing.Upper;
            tx_disDrio.CharacterCasing = CharacterCasing.Upper;
            tx_docsOr.CharacterCasing = CharacterCasing.Upper;
            tx_consig.CharacterCasing = CharacterCasing.Upper;
            tx_det_umed.CharacterCasing = CharacterCasing.Upper;
            tx_det_desc.CharacterCasing = CharacterCasing.Upper;
            // todo desabilidado
            rb_ent_clte.Checked = true;
            rb_car_ofi.Checked = true;
            sololee();

        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            Array.Clear(rl, 0, rl.Length);
            Array.Clear(dl, 0, dl.Length);
            claveSeg = "";
            //dataGridView1.Rows.Clear();
            //if (Tx_modo.Text == "NUEVO") dataGridView1.ReadOnly = false;
            //else dataGridView1.ReadOnly = true;
            lb_glodeta.Text = gloDeta;
            tx_numero.Text = "";
            tx_totcant.Text = "";
            tx_totpes.Text = "";
            tx_serie.Text = v_iniGRET + lib.Right(v_slu,3);     // inicial GRE-T + serie en 3 digitos
            tx_numero.ReadOnly = true;
            tx_dat_locori.Text = v_clu;
            rb_car_ofi.Checked = true;
            rb_ent_clte.Checked = true;
            cmb_origen.SelectedValue = tx_dat_locori.Text;
            cmb_origen_SelectionChangeCommitted(null, null);
            tx_dat_mone.Text = MonDeft;
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_digit.Text = v_nbu;
            tx_dat_estad.Text = codGene;
            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
            chk_man.Checked = false;        // checked=false ==> si se manifiesta, checked=true NO se manifiesta
            chk_man.Enabled = false;        // solo se habilita en modo NUEVO y cuando el destino de la GR tiene manifiesto
            rb_kg.Checked = true;
            tx_dat_tdRem.Text = vtc_ruc;
            cmb_docRem.SelectedValue = vtc_ruc;
            cmb_docRem_SelectionChangeCommitted(null, null);
            tx_numDocRem.Text = Program.ruc;
            cmb_docRem.Enabled = false;
            tx_numDocRem.Enabled = false;
            tx_nomRem.Text = Program.cliente;
            tx_nomRem.Enabled = false;
            rb_tn.Enabled = false;          // sunat solo admite KGM 18/07/2023
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
                {
                    cnx.Open();
                    string consulta = "select formulario,campo,param,valor from dt_enlaces where formulario in (@nofo,@nfin,@nofa,@nofi,@nofe)";
                    using (SqliteCommand micon = new SqliteCommand(consulta, cnx))
                    {
                        micon.Parameters.AddWithValue("@nofo", "main");
                        micon.Parameters.AddWithValue("@nfin", "interno");
                        micon.Parameters.AddWithValue("@nofi", "clients");
                        micon.Parameters.AddWithValue("@nofe", "facelect");
                        micon.Parameters.AddWithValue("@nofa", "guiari_e");
                        SqliteDataReader lite = micon.ExecuteReader();
                        if (lite.HasRows == true)
                        {
                            while(lite.Read())
                            {
                                lite.GetString(0).ToString();
                                if (lite.GetString(0).ToString() == "main")
                                {
                                    if (lite.GetString(1).ToString() == "imagenes")
                                    {
                                        if (lite.GetString(2).ToString() == "img_btN") img_btN = lite.GetString(3).ToString().Trim();         // imagen del boton de accion NUEVO
                                        if (lite.GetString(2).ToString() == "img_btE") img_btE = lite.GetString(3).ToString().Trim();         // imagen del boton de accion EDITAR
                                        if (lite.GetString(2).ToString() == "img_btA") img_btA = lite.GetString(3).ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                                        if (lite.GetString(2).ToString() == "img_btQ") img_btq = lite.GetString(3).ToString().Trim();         // imagen del boton de accion SALIR
                                        if (lite.GetString(2).ToString() == "img_btP") img_btP = lite.GetString(3).ToString().Trim();         // imagen del boton de accion IMPRIMIR
                                        if (lite.GetString(2).ToString() == "img_btV") img_btV = lite.GetString(3).ToString().Trim();         // imagen del boton de accion visualizar
                                        if (lite.GetString(2).ToString() == "img_bti") img_bti = lite.GetString(3).ToString().Trim();         // imagen del boton de accion IR AL INICIO
                                        if (lite.GetString(2).ToString() == "img_bts") img_bts = lite.GetString(3).ToString().Trim();         // imagen del boton de accion SIGUIENTE
                                        if (lite.GetString(2).ToString() == "img_btr") img_btr = lite.GetString(3).ToString().Trim();         // imagen del boton de accion RETROCEDE
                                        if (lite.GetString(2).ToString() == "img_btf") img_btf = lite.GetString(3).ToString().Trim();         // imagen del boton de accion IR AL FINAL
                                        if (lite.GetString(2).ToString() == "img_gra") img_grab = lite.GetString(3).ToString().Trim();         // imagen del boton grabar nuevo
                                        if (lite.GetString(2).ToString() == "img_anu") img_anul = lite.GetString(3).ToString().Trim();         // imagen del boton grabar anular
                                        if (lite.GetString(2).ToString() == "img_preview") img_ver = lite.GetString(3).ToString().Trim();         // imagen del boton grabar visualizar
                                    }
                                    if (lite.GetString(1).ToString() == "estado")
                                    {
                                        if (lite.GetString(2).ToString() == "anulado") codAnul = lite.GetString(3).ToString().Trim();         // codigo doc anulado
                                        if (lite.GetString(2).ToString() == "generado") codGene = lite.GetString(3).ToString().Trim();        // codigo doc generado
                                    }
                                    if (lite.GetString(1).ToString() == "sunat")
                                    {
                                        if (lite.GetString(2).ToString() == "usa_gre") usa_gre = lite.GetString(3).ToString().Trim();                   // se usa GRE? S/N
                                        if (lite.GetString(2).ToString() == "client_id") client_id_sunat = lite.GetString(3).ToString().Trim();         // id del api sunat
                                        if (lite.GetString(2).ToString() == "client_pass") client_pass_sunat = lite.GetString(3).ToString().Trim();     // password del api sunat
                                        if (lite.GetString(2).ToString() == "user_sol") u_sol_sunat = lite.GetString(3).ToString().Trim();              // usuario sol portal sunat del cliente 
                                        if (lite.GetString(2).ToString() == "clave_sol") c_sol_sunat = lite.GetString(3).ToString().Trim();             // clave sol portal sunat del cliente 
                                        if (lite.GetString(2).ToString() == "scope") scope_sunat = lite.GetString(3).ToString().Trim();                 // scope del api sunat
                                        if (lite.GetString(2).ToString() == "codgre") cGR_sunat = lite.GetString(3).ToString().Trim();                 // codigo sunat para GR transportista
                                        //  "true" + " " + "certificado.pfx" + " " + "190969Sorol"
                                        if (lite.GetString(2).ToString() == "firmDocElec") firmDocElec = lite.GetString(3).ToString().Trim();            // Firma xml, true=firma, false=no firma
                                        if (lite.GetString(2).ToString() == "rutaCertifc") rutaCertifc = lite.GetString(3).ToString().Trim();            // Ruta y nombre del certificado .pfx
                                        if (lite.GetString(2).ToString() == "claveCertif") claveCertif = lite.GetString(3).ToString().Trim();            // Clave del certificado
                                        if (lite.GetString(2).ToString() == "wsPostSunatG") wsPostS = lite.GetString(3).ToString().Trim();               // ruta api sunat para postear
                                    }
                                    if (lite.GetString(1).ToString() == "rutas")
                                    {
                                        if (lite.GetString(2).ToString() == "grt_txt") rutatxt = lite.GetString(3).ToString().Trim();         // ruta de los txt para las guías elect
                                        if (lite.GetString(2).ToString() == "web_dni") webdni = lite.GetString(3).ToString().Trim();         // web para busqueda de dni 
                                        if (lite.GetString(2).ToString() == "grt_xml") rutaxml = lite.GetString(3).ToString().Trim();         // 
                                    }
                                    if (lite.GetString(1).ToString() == "conector")
                                    {
                                        if (lite.GetString(2).ToString() == "noRetGlosa") NoRetGl = lite.GetString(3).ToString().Trim();          // glosa que retorna umasapa cuando no encuentra dato
                                    }
                                }
                                if (lite.GetString(0).ToString() == "clients" && lite.GetString(1).ToString() == "documento")
                                {
                                    if (lite.GetString(2).ToString() == "dni") vtc_dni = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(2).ToString() == "ruc") vtc_ruc = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(2).ToString() == "ext") vtc_ext = lite.GetString(3).ToString().Trim();
                                }
                                if (lite.GetString(0).ToString() == "facelect")
                                {
                                    if (lite.GetString(1).ToString() == "factelect")
                                    {
                                        if (lite.GetString(2).ToString() == "caracterNo") caractNo = lite.GetString(3).ToString().Trim();
                                    }
                                }
                                if (lite.GetString(0).ToString() == "guiari_e")    // guias de remision electrónicas de transportista
                                {
                                    if (lite.GetString(1).ToString() == "documento")
                                    {
                                        if (lite.GetString(2).ToString() == "c_int") v_cid = lite.GetString(3).ToString().Trim();                 // codigo interno guias de remision
                                        if (lite.GetString(2).ToString() == "frase1") v_fra1 = lite.GetString(3).ToString().Trim();               // frase para documento anulado
                                        if (lite.GetString(2).ToString() == "frase2") v_fra2 = lite.GetString(3).ToString().Trim();               // frase de si va con clave la guia
                                        if (lite.GetString(2).ToString() == "serieAnu") v_sanu = lite.GetString(3).ToString().Trim();             // serie anulacion interna
                                        if (lite.GetString(2).ToString() == "usediDrem") v_uedo = lite.GetString(3).ToString().Trim();            // usuarios que pueden modificar documentos del remitente
                                        if (lite.GetString(2).ToString() == "marca") v_marGRET = lite.GetString(3).ToString().Trim();             // marca de guía transportista electrónica
                                        if (lite.GetString(2).ToString() == "ini_GRET") v_iniGRET = lite.GetString(3).ToString().Trim();          // inicial (sigla) de las GRE-T
                                        if (lite.GetString(2).ToString() == "UsuRegen") v_urege = lite.GetString(3).ToString().Trim();            // usuarios que pueden regenerar txt
                                        if (lite.GetString(2).ToString() == "UsuAnuInt") v_uagin = lite.GetString(3).ToString().Trim();           // usuarios que pueden hacer anulaciones internas
                                    }
                                    if (lite.GetString(1).ToString() == "impresion")
                                    {
                                        if (lite.GetString(2).ToString() == "formato") vi_formato = lite.GetString(3).ToString().Trim();
                                        if (lite.GetString(2).ToString() == "copias") vi_copias = lite.GetString(3).ToString().Trim();
                                        if (lite.GetString(2).ToString() == "impMatris") v_impA5 = lite.GetString(3).ToString().Trim();
                                        if (lite.GetString(2).ToString() == "impTK") v_impTK = lite.GetString(3).ToString().Trim();
                                        if (lite.GetString(2).ToString() == "nomGRi_cr") v_CR_gr_ind = lite.GetString(3).ToString().Trim();
                                    }
                                    if (lite.GetString(1).ToString() == "moneda" && lite.GetString(2).ToString() == "default") MonDeft = lite.GetString(3).ToString().Trim();             // moneda por defecto
                                    if (lite.GetString(1).ToString() == "detalle" && lite.GetString(2).ToString() == "glosa") gloDeta = lite.GetString(3).ToString().Trim();             // glosa del detalle
                                    if (lite.GetString(1).ToString() == "electronico" && lite.GetString(2).ToString() == "proveedor") ipeeg = lite.GetString(3).ToString().Trim();      // identificador del emisor electrónico
                                    if (lite.GetString(1).ToString() == "glosas")
                                    {
                                        if (lite.GetString(2).ToString() == "glosa1") glosa1 = lite.GetString(3).ToString();          // glosa final del ticket 1
                                        if (lite.GetString(2).ToString() == "glosa2") glosa2 = lite.GetString(3).ToString();
                                        if (lite.GetString(2).ToString() == "nomGRER") det3dtm = lite.GetString(3).ToString();         // nombre detalle DTM de las GRE-Transportista
                                    }
                                    if (lite.GetString(1).ToString() == "despedida")
                                    {
                                        if (lite.GetString(2).ToString() == "desped1") despedida = lite.GetString(3).ToString();          // glosa despedida del ticket 1
                                        if (lite.GetString(2).ToString() == "desped2") despedid2 = lite.GetString(3).ToString();
                                    }
                                    if (lite.GetString(1).ToString() == "traslado")
                                    {
                                        if (lite.GetString(2).ToString() == "codTraInt") codtrasli = lite.GetString(3).ToString();          // codigo interno de traslado entre establecimientos de una misma empresa
                                        if (lite.GetString(2).ToString() == "codTraIti") codtrasit = lite.GetString(3).ToString();          // codigo interno de traslado ITINERANTE
                                    }
                                }
                                if (lite.GetString(0).ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                                {
                                    if (lite.GetString(1).ToString() == "anulado" && lite.GetString(2).ToString() == "A0") vint_A0 = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "igv" && lite.GetString(2).ToString() == "%") v_igv = lite.GetString(3).ToString().Trim();
                                }
                            }
                        }
                    }
                }
                // jalamos datos del usuario y local
                v_clu = lib.codloc(asd);                // codigo local usuario
                v_slu = lib.serlocs(v_clu);             // serie local usuario
                v_nbu = lib.nomuser(asd);               // nombre del usuario
                // parametros para token
                c_t[0] = client_id_sunat;
                c_t[1] = scope_sunat;
                c_t[2] = client_id_sunat;
                c_t[3] = client_pass_sunat;
                c_t[4] = u_sol_sunat;
                c_t[5] = c_sol_sunat;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void jalaoc(string campo)       // jala guia individual remitente
        {
            //try
            {
                string parte = "";
                if (campo == "tx_idr")
                {
                    parte = "where a.marca_gre=@marGR and a.id=@ida";
                }
                if (campo == "sernum")
                {
                    parte = "where a.marca_gre=@marGR and a.serguir=@ser and a.numguir=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select a.id,a.fechopegr,a.serguir,a.numguir,a.numpregui,a.tidodegri,a.nudodegri,a.nombdegri,a.diredegri," +
                        "a.ubigdegri,a.tidoregri,a.nudoregri,a.nombregri,a.direregri,a.ubigregri,a.locorigen,a.dirorigen,a.ubiorigen," +
                        "a.locdestin,a.dirdestin,a.ubidestin,a.docsremit,a.obspregri,a.clifingri,a.cantotgri,a.pestotgri," +
                        "a.tipmongri,a.tipcamgri,a.subtotgri,a.igvgri,a.totgri,a.totpag,a.salgri,a.estadoser,a.impreso," +
                        "a.frase1,a.frase2,a.fleteimp,a.tipintrem,a.tipintdes,a.tippagpre,a.seguroE,a.userc,a.userm,a.usera," +
                        "a.serplagri,a.numplagri,a.plaplagri,a.carplagri,a.autplagri,a.confvegri,a.breplagri,a.proplagri," +
                        "ifnull(p.nomchofe,'') as chocamcar,ifnull(a.fechplani,'') as fecplacar,'' as fecdocvta,'' as tipdocvta," +    // ifnull(b.fecplacar,'') as fecplacar,ifnull(b.fecdocvta,'') as fecdocvta,ifnull(f.descrizionerid,'') as tipdocvta,
                        "'' as serdocvta,'' as numdocvta,'' as codmonvta," +    // ifnull(b.serdocvta,'') as serdocvta,ifnull(b.numdocvta,'') as numdocvta,ifnull(b.codmonvta,'') as codmonvta,
                        "0 as totdocvta,'' as codmonpag,0 as totpagado,0 as saldofina," +       //ifnull(b.totdocvta,0) as totdocvta,ifnull(b.codmonpag,'') as codmonpag,ifnull(b.totpagado,0) as totpagado,ifnull(b.saldofina,0) as saldofina,
                        "'' as feculpago,ifnull(a.estadoser,'') as estadoser,ifnull(c.razonsocial,'') as razonsocial,a.grinumaut," +        // ifnull(b.feculpago,'') as feculpago,ifnull(b.estadoser,'') as estadoser,
                        "ifnull(d.marca,'') as marca,ifnull(d.modelo,'') as modelo,ifnull(r.marca,'') as marCarret,ifnull(r.confve,'') as confvCarret,ifnull(r.autor1,'') as autCarret," +
                        "ifnull(er.numerotel1,'') as telrem,ifnull(ed.numerotel1,'') as teldes,ifnull(t.nombclt,'') as clifact," +
                        "a.marca_gre,a.tidocor,a.rucDorig,a.lpagop,a.pesoKT,a.tidocor2,a.rucDorig2,a.docsremit2,a.marca1,a.codMTras,a.desMTras," +
                        "ifnull(ad.nticket,'') as nticket,ifnull(ad.estadoS,'') as estadoS,ifnull(ad.cdr,'') as cdr,ifnull(ad.cdrgener,'') as cdrgener," +
                        "ifnull(ad.textoQR,'') as textoQR,ifnull(ad.fticket,'') as fticket " +
                        "from cabguiar a " +
                        "left join adiguiar ad on ad.idg=a.id " +
                        "left join cabfactu t on t.tipdvta=a.tipdocvta and t.serdvta=a.serdocvta and t.numdvta=a.numdocvta " +
                        "left join anag_for c on c.ruc=a.proplagri and c.tipdoc=@tdep " +
                        "left join vehiculos d on d.placa=a.plaplagri " +
                        "left join vehiculos r on r.placa=a.carplagri " +
                        "left join cabplacar p on p.id=a.idplani " +
                        "left join anag_cli er on er.ruc=a.nudoregri and er.tipdoc=a.tidoregri " +
                        "left join anag_cli ed on ed.ruc=a.nudodegri and ed.tipdoc=a.tidodegri " + parte;
                    //  "left join controlg b on b.serguitra=a.sergui and b.numguitra=a.numgui " + LAS GUIAS REMITENTE NO GENERAN CONTROL Y COBRANZA
                    //  "left join desc_tdv f on f.idcodice=b.tipdocvta " +
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@tdep", vtc_ruc);
                    micon.Parameters.AddWithValue("@marGR", v_marGRET);
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
                            tx_fechope.Text = dr.GetString("fechopegr").Substring(0,10);
                            tx_digit.Text = dr.GetString("userc") + " " + dr.GetString("userm") + " " + dr.GetString("usera");
                            tx_dat_estad.Text = dr.GetString("estadoser");
                            tx_serie.Text = dr.GetString("serguir");
                            tx_numero.Text = dr.GetString("numguir");
                            tx_dat_locori.Text = dr.GetString("locorigen");
                            tx_dat_locdes.Text = dr.GetString("locdestin");
                            tx_ubigO.Text = dr.GetString("ubiorigen");
                            tx_ubigD.Text = dr.GetString("ubidestin");
                            tx_dat_tdRem.Text = dr.GetString("tidoregri");
                            tx_numDocRem.Text = dr.GetString("nudoregri");
                            tx_nomRem.Text = dr.GetString("nombregri");
                            tx_dirRem.Text = dr.GetString("direregri");
                            tx_ubigRtt.Text = dr.GetString("ubigregri");
                            tx_telR.Text = dr.GetString("telrem");
                            tx_dat_tDdest.Text = dr.GetString("tidodegri");
                            tx_numDocDes.Text = dr.GetString("nudodegri");
                            tx_nomDrio.Text = dr.GetString("nombdegri");
                            tx_dirDrio.Text = dr.GetString("diredegri");
                            tx_ubigDtt.Text = dr.GetString("ubigdegri");
                            tx_telD.Text = dr.GetString("teldes");
                            tx_docsOr.Text = dr.GetString("docsremit");
                            tx_docsOr2.Text = dr.GetString("docsremit2");
                            tx_obser1.Text = dr.GetString("obspregri");
                            tx_consig.Text = dr.GetString("clifingri");
                            tx_dat_mone.Text = dr.GetString("tipmongri");
                            tx_totcant.Text = dr.GetString("cantotgri");
                            tx_totpes.Text = dr.GetDecimal("pestotgri").ToString("#.#");
                            tx_impreso.Text = dr.GetString("impreso");
                            tx_pregr_num.Text = dr.GetString("numpregui");
                            claveSeg = dr.GetString("seguroE");
                            tx_n_auto.Text = dr.GetString("grinumaut");     // numeracion de GR autom o manual
                            //
                            tx_marcamion.Text = dr.GetString("marca");
                            tx_pla_fech.Text = dr.GetString("fecplacar");   //.Substring(0, 10);
                            tx_pla_plani.Text = dr.GetString("serplagri") + dr.GetString("numplagri");
                            tx_pla_placa.Text = dr.GetString("plaplagri");
                            tx_pla_carret.Text = dr.GetString("carplagri");
                            tx_pla_autor.Text = dr.GetString("autplagri");
                            tx_aut_carret.Text = dr.GetString("autCarret");
                            tx_marCarret.Text = dr.GetString("marCarret");
                            tx_pla_confv.Text = dr.GetString("confvegri");
                            tx_pla_brevet.Text = dr.GetString("breplagri");
                            tx_pla_nomcho.Text = dr.GetString("chocamcar");
                            tx_pla_ruc.Text = dr.GetString("proplagri");
                            tx_pla_propiet.Text = dr.GetString("razonsocial");
                            tx_pla_dniChof.Text = lib.Right(dr.GetString("breplagri").ToString(), 8);         // aca debería ser un campo separado 07/03/2023
                            tx_marCpropio.Text = (tx_pla_ruc.Text.Trim() != "" && tx_pla_ruc.Text != Program.ruc) ? "1" : "0";   // Indicador de transporte subcontratado = true
                            tx_dat_docOr.Text = dr.GetString("tidocor");
                            tx_rucEorig.Text = dr.GetString("rucDorig");
                            tx_dat_docOr2.Text = dr.GetString("tidocor2");
                            tx_rucEorig2.Text = dr.GetString("rucDorig2");
                            if (dr.GetString("marca1") == "1") chk_cunica.Checked = true;
                            tx_dat_motras.Text = dr.GetString("codMTras");
                            tx_motras.Text = dr.GetString("desMTras");
                            //
                            if (dr.GetString("pesoKT") == "K") rb_kg.Checked = true;
                            else rb_tn.Checked = true;
                            cmb_docorig.SelectedValue = tx_dat_docOr.Text;
                            cmb_docorig_SelectionChangeCommitted(null, null);
                            cmb_docorig2.SelectedValue = tx_dat_docOr2.Text;
                            cmb_docorig2_SelectionChangeCommitted(null, null);
                            //
                            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                            tx_dat_tickSunat.Text = dr.GetString("nticket");
                            tx_estaSunat.Text = dr.GetString("estadoS");
                            tx_dat_textoqr.Text = dr.GetString("textoQR");
                            tx_fticket.Text = dr.GetString("fticket");

                            cmb_origen.SelectedValue = tx_dat_locori.Text;
                            cmb_origen_SelectionChangeCommitted(null, null);
                            cmb_destino.SelectedValue = tx_dat_locdes.Text;
                            cmb_destino_SelectionChangeCommitted(null, null);
                            cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
                            cmb_docRem_SelectionChangeCommitted(null, null);
                            string[] du_remit = lib.retDPDubigeo(tx_ubigRtt.Text);
                            tx_dptoRtt.Text = du_remit[0];
                            tx_provRtt.Text = du_remit[1];
                            tx_distRtt.Text = du_remit[2];
                            cmb_docDes.SelectedValue = tx_dat_tDdest.Text;
                            cmb_docDes_SelectionChangeCommitted(null, null);
                            string[] du_desti = lib.retDPDubigeo(tx_ubigDtt.Text);
                            tx_dptoDrio.Text = du_desti[0];
                            tx_proDrio.Text = du_desti[1];
                            tx_disDrio.Text = du_desti[2];
                            tx_dirDrio.Text = dr.GetString("diredegri");
                            tx_tipcam.Text = dr.GetString("tipcamgri");
                            cmb_motras.SelectedValue = tx_dat_motras.Text;
                            cmb_motras_SelectionChangeCommitted(null, null);
                        }
                        else
                        {
                            MessageBox.Show("No existe el número de guía!", "Atención - dato incorrecto",
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
                    if (Tx_modo.Text != "NUEVO" && (tx_estaSunat.Text == "Enviado" || tx_estaSunat.Text == "En proceso"))
                    {
                        // llamada al metodo que consultará el estado del comprobante y actualizara 
                        if (tx_dat_tickSunat.Text != "") _Sunat.consultaC("adiguiar", tx_idr.Text, tx_dat_tickSunat.Text, _Sunat.conex_token_(c_t), tx_serie.Text, tx_numero.Text, rutaxml);
                    }
                    else
                    {
                        // aca no hay nada que hacer ... el campo textoQR para el QR ya tiene info ahí. 
                        //if (Tx_modo.Text != "NUEVO" && (tx_estaSunat.Text == "Aceptado" && dr.GetString("cdrgener") == "1")) convierteCDR(dr.GetString("cdr"));
                    }

                    dr.Dispose();
                    micon.Dispose();
                }
                conn.Close();
            }
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error Fatal en codigo");
            //    Application.Exit();
            //    return;
            //}
        }
        private void jalapg(string numpre)          // jala datos de la pre guia
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string jala = "select a.estadoser,a.locorigen,a.ubiorigen,a.locdestin,a.ubidestin," +
                    "a.tidodepre,a.nudodepre,a.nombdepre,a.diredepre,a.ubigdepre," +
                    "a.tidorepre,a.nudorepre,a.nombrepre,a.direrepre,a.ubigrepre," +
                    "a.docsremit,a.obspregui,a.clifinpre,a.tipmonpre,a.seguroE,a.totpregui " +
                    "from cabpregr a where a.numpregui=@num";
                using (MySqlCommand micon = new MySqlCommand(jala, conn))
                {
                    micon.Parameters.AddWithValue("@num", numpre);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        tx_dat_estad.Text = dr.GetString("estadoser");
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
                        tx_docsOr.Text = dr.GetString("docsremit");
                        tx_obser1.Text = dr.GetString("obspregui");
                        tx_consig.Text = dr.GetString("clifinpre");
                        tx_dat_mone.Text = dr.GetString("tipmonpre");
                        claveSeg = dr.GetString("seguroE");
                    }
                    dr.Dispose();
                }
                string jalad = "select cantprodi,unimedpro,codiprodi,descprodi,round(pesoprodi,1),precprodi,totaprodi " +
                    "from detpregr where numpregui = @num";
                using (MySqlCommand micon = new MySqlCommand(jalad, conn))
                {
                    micon.Parameters.AddWithValue("@num", numpre);
                    MySqlDataReader dr = micon.ExecuteReader();
                    while (dr.Read())
                    {
                        /*dataGridView1.Rows.Add(
                            dr.GetString(0),
                            dr.GetString(1),
                            dr.GetString(3),
                            dr.GetString(4)
                            );
                        */
                        tx_det_cant.Text = dr.GetString(0);
                        tx_det_umed.Text = dr.GetString(1);
                        tx_det_desc.Text = dr.GetString(3);
                        tx_det_peso.Text = dr.GetString(4);
                    }
                    dr.Dispose();
                }
                cmb_origen.SelectedValue = tx_dat_locori.Text;
                cmb_origen_SelectionChangeCommitted(null, null);
                cmb_destino.SelectedValue = tx_dat_locdes.Text;
                cmb_destino_SelectionChangeCommitted(null, null);
                cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
                string[] du_remit = lib.retDPDubigeo(tx_ubigRtt.Text);
                tx_dptoRtt.Text = du_remit[0];
                tx_provRtt.Text = du_remit[1];
                tx_distRtt.Text = du_remit[2];
                cmb_docDes.SelectedValue = tx_dat_tDdest.Text;
                string[] du_desti = lib.retDPDubigeo(tx_ubigDtt.Text);
                tx_dptoDrio.Text = du_desti[0];
                tx_proDrio.Text = du_desti[1];
                tx_disDrio.Text = du_desti[2];
            }
        }
        private void jaladet(string idr)            // jala el detalle
        {
            string jalad = "select id,serguir,numguir,cantprodi,unimedpro,codiprodi,REPLACE(descprodi,@glodet,'') AS descprodi,round(pesoprodi,1),precprodi,totaprodi " +
                "from detguiar where idc=@idr";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand micon = new MySqlCommand(jalad, conn))
                {
                    micon.Parameters.AddWithValue("@idr", idr);
                    micon.Parameters.AddWithValue("@glodet", gloDeta);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            tx_det_cant.Text = row[3].ToString();
                            tx_det_umed.Text = row[4].ToString();
                            tx_det_desc.Text = row[6].ToString();
                            tx_det_peso.Text = row[7].ToString();
                        }
                        dt.Dispose();
                    }
                }
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
            //  datos para los combos de locales origen y destino
            cmb_origen.Items.Clear();
            MySqlCommand ccl = new MySqlCommand("select idcodice,descrizionerid,ubidir,marca1,marca2,deta1,deta2,deta3,deta4 from desc_loc where numero=@bloq",conn);
            ccl.Parameters.AddWithValue("@bloq", 1);
            MySqlDataAdapter dacu = new MySqlDataAdapter(ccl);
            dtu.Clear();
            dacu.Fill(dtu);
            cmb_origen.DataSource = dtu;
            cmb_origen.DisplayMember = "descrizionerid";
            cmb_origen.ValueMember = "idcodice";
            //
            dtd.Clear();
            dacu.Fill(dtd);
            cmb_destino.Items.Clear();
            cmb_destino.DataSource = dtd;
            cmb_destino.DisplayMember = "descrizionerid";
            cmb_destino.ValueMember = "idcodice";
            //  datos para los combobox de tipo de documento
            cmb_docRem.Items.Clear();
            MySqlCommand cdu = new MySqlCommand("select idcodice,descrizionerid,codigo,codsunat,descrizione from desc_doc where numero=@bloq", conn);
            cdu.Parameters.AddWithValue("@bloq", 1);
            MySqlDataAdapter datd = new MySqlDataAdapter(cdu);
            dttd0.Clear();
            datd.Fill(dttd0);
            cmb_docRem.DataSource = dttd0;
            cmb_docRem.DisplayMember = "descrizionerid";
            cmb_docRem.ValueMember = "idcodice";
            //
            dttd1.Clear();
            cmb_docDes.Items.Clear();
            datd.Fill(dttd1);
            cmb_docDes.DataSource = dttd1;
            cmb_docDes.DisplayMember = "descrizionerid";
            cmb_docDes.ValueMember = "idcodice";
            // datos para tipo de documento 
            string consu = "select idcodice,descrizione,descrizionerid,codsunat,deta1 from desc_tdv where codigo=''";
            using (MySqlCommand cdv = new MySqlCommand(consu, conn))
            {
                using (MySqlDataAdapter datv = new MySqlDataAdapter(cdv))
                {
                    dttdv.Clear();
                    datv.Fill(dttdv);
                }
            }
            // documento origen - documentos relacionados con transporte de mercancias
            using (MySqlCommand mydorig = new MySqlCommand("select * from desc_dtm where numero=@bloq AND deta3=@det3 OR deta4=@det3", conn))
            {
                mydorig.Parameters.AddWithValue("@bloq", 1);
                mydorig.Parameters.AddWithValue("@det3", det3dtm);
                using (MySqlDataAdapter da = new MySqlDataAdapter(mydorig))
                {
                    dtdor.Clear();
                    da.Fill(dtdor);
                    cmb_docorig.DataSource = dtdor;
                    cmb_docorig.DisplayMember = "descrizione";
                    cmb_docorig.ValueMember = "idcodice";
                    //
                    dtdor2.Clear();
                    da.Fill(dtdor2);
                    cmb_docorig2.DataSource = dtdor2;
                    cmb_docorig2.DisplayMember = "descrizione";
                    cmb_docorig2.ValueMember = "idcodice";
                }
            }
            // datos para el combo de motivo de traslado
            cmb_motras.Items.Clear();
            MySqlCommand cmotras = new MySqlCommand("select idcodice,descrizionerid,codigo,codsunat from desc_mtr where numero=@bloq", conn);
            cmotras.Parameters.AddWithValue("@bloq", 1);
            dacu = new MySqlDataAdapter(cmotras);
            dtmt.Clear();
            dacu.Fill(dtmt);
            cmb_motras.DataSource = dtmt;
            cmb_motras.DisplayMember = "descrizionerid";
            cmb_motras.ValueMember = "idcodice";
            //
            ccl.Dispose();
            cdu.Dispose();
            dacu.Dispose();
            conn.Close();
        }
        private bool valiGri()                      // valida filas completas en la grilla
        {
            bool retorna = true;
            /*
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                if (dataGridView1.Rows[i].Cells[0].Value == null &&
                    dataGridView1.Rows[i].Cells[1].Value == null &&
                    dataGridView1.Rows[i].Cells[2].Value == null &&
                    dataGridView1.Rows[i].Cells[3].Value == null)
                {
                    // no hay problema
                }
                else
                {
                    if (dataGridView1.Rows[i].Cells[0].Value == null ||
                        dataGridView1.Rows[i].Cells[1].Value == null ||
                        dataGridView1.Rows[i].Cells[2].Value == null ||
                        dataGridView1.Rows[i].Cells[3].Value == null)
                    {
                        //MessageBox.Show("Complete las filas del detalle", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        retorna = false;
                    }
                    else
                    {
                        retorna = true;
                    }
                }
            }
            */
            return retorna;
        }
        private bool valiVars()                         // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (usa_gre != "S")
            {
                lib.messagebox("NO se usan las GRE en esta organización");
                retorna = false;
            }
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
            if (gloDeta == "")          // glosa x defecto en el detalle
            {
                lib.messagebox("Glosa por defecto en detalle");
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
            if (v_fra1 == "")            // frase de si va o no con clave
            {
                lib.messagebox("Frase impresa en GR sobre clave");
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
            if (vint_A0 == "")
            {
                lib.messagebox("Código interno enlace anulación BD - A0");
                retorna = false;
            }
            return retorna;
        }
        private bool correlativo()                          // coje y aumenta en 1 el correlativo
        {
            bool retorna = false;
            string todo = "corre_serie";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    using (MySqlCommand micon = new MySqlCommand(todo, conn))
                    {
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("td", "TDV004");
                        micon.Parameters.AddWithValue("ser", tx_serie.Text);
                        using (MySqlDataReader dr0 = micon.ExecuteReader())
                        {
                            if (dr0.Read())
                            {
                                if (dr0[0] != null && dr0.GetString(0) != "")
                                {
                                    tx_numero.Text = lib.Right("00000000" + dr0.GetString(0), 8);
                                    if (tx_numero.Text != "00000000") retorna = true;
                                }
                                else
                                {
                                    //
                                }

                            }
                        }
                    }
                }
            }
            return retorna;
        }
        private void valiruc(object sender)                     // valida los ruc del documento origen
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
        private string[] partidor(string texto, string marca)       // convierte un texto en un arreglo de 2 filas
        {
            string[] retorna = new string[] { "", "" };

            string[] torna = texto.Split(new string[] { marca }, StringSplitOptions.None);
            int medio = torna.Length;
            for (int i = 0; i < torna.Length; i++)
            {
                if (torna.Length / 2 > i) retorna[0] = retorna[0] + torna[i] + " ";
                //if (partido.Length / 2 == i) Console.WriteLine("");
                if (torna.Length / 2 <= i) retorna[1] = retorna[1] + torna[i] + " ";
            }

            return retorna;
        }

        #region autocompletados
        private void autodepa()                             // departamentos
        {
            if (dataUbig == null)
            {
                MessageBox.Show("Problema de comunicación de datos" + Environment.NewLine +
                    "Debe reiniciar el sistema","Error interno",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                Application.Exit();
                return;
                //DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");
                // aca deberiamos volver a hacer un AddItem de CacheManager
            }
            DataRow[] depar = dataUbig.Select("depart<>'00' and provin='00' and distri='00'");
            departamentos.Clear();
            foreach (DataRow row in depar)
            {
                departamentos.Add(row["nombre"].ToString());
            }
        }
        private void autoprov(string marca)                 // se jala despues de ingresado el departamento
        {
            if (marca != "")   // tx_ubigO.Text.Trim() != ""
            {
                DataRow[] provi = null;
                if (marca == "tx_ubigO")
                {
                    provi = dataUbig.Select("depart='" + tx_ubigO.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                }
                if (marca == "tx_ubigD")
                {
                    provi = dataUbig.Select("depart='" + tx_ubigD.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                }
                if (marca == "tx_ubigRtt")
                {
                    provi = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                }
                if (marca == "tx_ubigDtt")
                {
                    provi = dataUbig.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                }
                provincias.Clear();
                foreach (DataRow row in provi)
                {
                    provincias.Add(row["nombre"].ToString());
                }
                /*MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select nombre from ubigeos where depart=@dep and provin<>'00' and distri='00'";
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    if (marca == "tx_ubigO") micon.Parameters.AddWithValue("@dep", tx_ubigO.Text.Substring(0, 2));
                    if (marca == "tx_ubigD") micon.Parameters.AddWithValue("@dep", tx_ubigD.Text.Substring(0, 2));
                    if (marca == "tx_ubigRtt") micon.Parameters.AddWithValue("@dep", tx_ubigRtt.Text.Substring(0, 2));
                    if (marca == "tx_ubigDtt") micon.Parameters.AddWithValue("@dep", tx_ubigDtt.Text.Substring(0, 2));
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
                }*/
            }
        }
        private void autodist(string marca)                 // se jala despues de ingresado la provincia
        {
            if (marca != "")
            {
                DataRow[] distr = null;
                if (marca == "tx_ubigO")
                {
                    distr = dataUbig.Select("depart='" + tx_ubigO.Text.Substring(0, 2) + "' and provin='" + tx_ubigO.Text.Substring(2, 2) + "' and distri<>'00'");
                }
                if (marca == "tx_ubigD")
                {
                    distr = dataUbig.Select("depart='" + tx_ubigD.Text.Substring(0, 2) + "' and provin='" + tx_ubigD.Text.Substring(2, 2) + "' and distri<>'00'");
                }
                if (marca == "tx_ubigRtt")
                {
                    distr = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and distri<>'00'");
                }
                if (marca == "tx_ubigDtt")
                {
                    distr = dataUbig.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigDtt.Text.Substring(2, 2) + "' and distri<>'00'");
                }
                distritos.Clear();
                foreach (DataRow row in distr)
                {
                    distritos.Add(row["nombre"].ToString());
                }
            }
        }
        private void autobult(string umedi)
        {

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
            tx_dirOrigen.ReadOnly = true;
            tx_dirDestino.ReadOnly = true;
            tx_nomRem.ReadOnly = true;          // los nombres y direcciones en readonly 
            tx_dirRem.ReadOnly = true;         // porque se jalan de la maestra
            tx_dptoRtt.ReadOnly = true;        // si la direccion esta en blanco
            tx_provRtt.ReadOnly = true;        // debe permitir escribir para actualizar la maestra
            tx_distRtt.ReadOnly = true;        // los nombres en readonly porque se jalan con el conector
            tx_nomDrio.ReadOnly = true;         // SE DEBE MARCAR si el cliente es N nuevo
            tx_dirDrio.ReadOnly = true;        // x defecto todo va en readonly=true
            tx_dptoDrio.ReadOnly = true;
            tx_proDrio.ReadOnly = true;
            tx_disDrio.ReadOnly = true;
            gbox_planilla.Enabled = false;
            //
            cmb_origen.Enabled = false;
            tx_docsOr.Enabled = false;
            tx_docsOr2.Enabled = false;
        }
        private void limpiar()
        {
            lp.limpiar(this);
            //
            tx_pla_fech.Text = "";
            tx_pla_plani.Text = "";
            tx_pla_placa.Text = "";
            tx_pla_carret.Text = "";
            tx_marcamion.Text = "";
            tx_pla_autor.Text = "";
            tx_aut_carret.Text = "";
            tx_marCarret.Text = "";
            tx_pla_confv.Text = "";
            tx_pla_brevet.Text = "";
            tx_pla_nomcho.Text = "";
            tx_pla_ruc.Text = "";
            tx_pla_propiet.Text = "";
            //
            tx_det_cant.Text = "";
            tx_det_umed.Text = "";
            tx_det_desc.Text = "";
            tx_det_peso.Text = "";
            //
            tx_dat_motras.Text = "";
            tx_dat_motrasS.Text = "";
            tx_motras.Text = "";
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
            cmb_motras.SelectedIndex = -1;
        }
        #endregion limpiadores_modos;

        #region  guia electronica en sunat y psnet

        #region Sunat metodo directo
        /*private bool sunat_api()                                // SI VAMOS A USAR 26/05/2023 este metodo directo
        {
            bool retorna = false;
            string token = conex_token(c_t);           // este metodo funciona bien .. 26/05/2023
            if (token != null && token != "")
            {
                string aZip = "";
                string aXml = "";
                if (llenaTablaLiteGRE() != true)
                {
                    MessageBox.Show("No se pudo llenar las tablas sqlite", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    aXml = Program.ruc + "-" + "09" + "-" + tx_serie.Text + "-" + tx_numero.Text + ".xml";
                    aZip = Program.ruc + "-" + "09" + "-" + tx_serie.Text + "-" + tx_numero.Text + ".zip";
                }
                if (aXml != "")
                {
                    // - zipear el xml, 
                    if (File.Exists(rutaxml + aZip) == true)
                    {
                        File.Delete(rutaxml + aZip);
                    } 
                    using (ZipArchive zip = ZipFile.Open(rutaxml + aZip, ZipArchiveMode.Create))
                    {
                        string source = rutaxml + aXml;
                        zip.CreateEntryFromFile(source, aXml);
                    }
                    // - byte[]ar el zip, 
                    var bytexml = File.ReadAllBytes(rutaxml + aZip);
                    var base64 = Convert.ToBase64String(bytexml);
                    // - hashear 
                    string hash = "";
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        hash = string.Concat(sha256.ComputeHash(bytexml).Select(x => x.ToString("x2")));
                    }
                    // Postear 
                    string url = "https://api-cpe.sunat.gob.pe/v1/contribuyente/gem/comprobantes/" + aXml.Replace(".xml", "");
                    var oData = new
                    {
                        archivo = new
                        {
                            nomArchivo = aZip,
                            arcGreZip = base64,
                            hashZip = hash
                        }
                    };
                    var json = JsonConvert.SerializeObject(oData);
                    //var Body = new StringContent(json, Encoding.UTF8, "application/json");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var poste = new RestClient(url);
                    poste.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Bearer " + token);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                    //
                    IRestResponse response = poste.Execute(request);
                    var result = JsonConvert.DeserializeObject<Ticket_RptaR>(response.Content);
                    if (response.ResponseStatus.ToString() != "200") 
                    {
                        MessageBox.Show(response.Content.ToString(),"Error " + response.ResponseStatus.ToString());
                        retorna = false;
                    }
                    else
                    {
                        retorna = true;
                    }
                    // actualizamos los campos de la tabla adiguiar
                    //string actua = "insert into adiguiar (idg,serie,numero,nticket,fticket,estadoS,cdr) values (@idg,@seg,@nug,@nti,@fti,@est,@cdr)";
                    string actua = "update adiguiar set nticket=@nti,fticket=@fti,estadoS=@est,cdr=@cdr where idg=@idg";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        using (MySqlCommand micon = new MySqlCommand(actua, conn))
                        {
                            micon.Parameters.AddWithValue("@idg", tx_idr.Text);
                            //micon.Parameters.AddWithValue("@seg", tx_serie.Text);
                            //micon.Parameters.AddWithValue("@nug", tx_numero.Text);
                            micon.Parameters.AddWithValue("@nti", result.numTicket);
                            micon.Parameters.AddWithValue("@fti", result.fecRecepcion);
                            micon.Parameters.AddWithValue("@est", "Enviado");
                            micon.Parameters.AddWithValue("@cdr", "0");
                            micon.ExecuteNonQuery();
                        }
                    }
                }
            }
            return retorna;
        } */
        /* public string consultaC(string ticket, string token)     // consulta comprobante
        {
            string retorna = "";
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://api-cpe.sunat.gob.pe/v1/contribuyente/gem/comprobantes/envios/" + ticket);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);
            if (response.Content.Contains("error"))
            {
                tx_estaSunat.Text = "Error";
                tx_estaSunat.Tag = response.Content.ToString();
                retorna = tx_estaSunat.Text;
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    string actua = "update adiguiar set estadoS=@est,cdr=@cdr where idg=@idg";
                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                    {
                        micon.Parameters.AddWithValue("@est", "Error");
                        micon.Parameters.AddWithValue("@cdr", response.Content.ToString());
                        micon.Parameters.AddWithValue("@idg", tx_idr.Text);
                        micon.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                var Rpta = JsonConvert.DeserializeObject<Rspta_ConsultaR>(response.Content);
                if (Rpta.arcCdr != null)
                {
                    string CodRrpta = Rpta.codRespuesta.ToString();
                    if (CodRrpta == "0" || CodRrpta == "99")
                    {
                        tx_estaSunat.Text = "Aceptado";
                        retorna = tx_estaSunat.Text;
                        // descompone el arcCDR para obtener los datos del QR
                        string cuidado = convierteCDR(Rpta.arcCdr, tx_serie.Text, tx_numero.Text, rutatxt);
                        if (cuidado != null && cuidado != "")
                        {
                            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                            {
                                conn.Open();
                                string actua = "update adiguiar set estadoS=@est,cdr=@cdr,cdrgener=@gen,textoQR=@tqr where idg=@idg";  // ,fticket=@ftk
                                using (MySqlCommand micon = new MySqlCommand(actua, conn))
                                {
                                    micon.Parameters.AddWithValue("@est", "Aceptado");
                                    micon.Parameters.AddWithValue("@cdr", Rpta.arcCdr.ToString());
                                    micon.Parameters.AddWithValue("@gen", Rpta.indCdrGenerado.ToString());
                                    micon.Parameters.AddWithValue("@tqr", cuidado);
                                    //micon.Parameters.AddWithValue("", );
                                    micon.Parameters.AddWithValue("@idg", tx_idr.Text);
                                    micon.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        tx_estaSunat.Text = (CodRrpta == "98") ? "En Proceso" : "Rechazado";
                        retorna = tx_estaSunat.Text;
                        using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                        {
                            conn.Open();
                            string actua = "update adiguiar set estadoS=@est,cdr=@cdr,cdrgener=@gen where idg=@idg";  // (serie, numero, , @seg, @nug, @nti, @fti)";
                            using (MySqlCommand micon = new MySqlCommand(actua, conn))
                            {
                                micon.Parameters.AddWithValue("@est", (CodRrpta == "0") ? "Aceptado" : (CodRrpta == "98") ? "En Proceso" : "Rechazado");
                                micon.Parameters.AddWithValue("@cdr", Rpta.arcCdr.ToString());
                                micon.Parameters.AddWithValue("@gen", Rpta.indCdrGenerado.ToString());
                                micon.Parameters.AddWithValue("@idg", tx_idr.Text);
                                micon.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            return retorna;
        } */
        /* public string conex_token(string[] c_t)                 // obtenemos el token de conexión
        {
            string retorna = "";
            tiempoT = (DateTime.Now.TimeOfDay.Subtract(horaT).TotalSeconds);
            if (tiempoT >= (plazoT-60))             // un minuto antes que venza la vigencia del token
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient("https://api-seguridad.sunat.gob.pe/v1/clientessol/" + c_t[0] + "/oauth2/token/"); // client_id_sunat + "/oauth2/token/"
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Cookie", "TS019e7fc2=014dc399cb268cb3d074c3b37bb5b735ab83b63307dfe5263ff5802065fe226640c58236dcd71073fbe01e3206d01bfa3c513e69c4");
                request.AddParameter("grant_type", "password");
                request.AddParameter("scope", c_t[1]);                          // scope_sunat         "https://api-cpe.sunat.gob.pe"
                request.AddParameter("client_id", c_t[2]);                      // client_id_sunat     "9613540b-a94d-45c6-b201-7521413ed391"
                request.AddParameter("client_secret", c_t[3]);                  // client_pass_sunat   "gmlqIVugA1+Fgd1wUN6Kyg=="
                request.AddParameter("username", c_t[4]);                       // u_sol_sunat         "20430100344PTIONVAL"
                request.AddParameter("password", c_t[5]);                       // c_sol_sunat         "patocralr"
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString() != "OK")
                {
                    MessageBox.Show("NO se pudo obtener el token" + Environment.NewLine +
                        response.StatusDescription, "Error de conexión a Sunat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<TokenR>(response.Content);
                    retorna = result.access_token;
                    plazoT = result.expires_in;
                    horaT = DateTime.Now.TimeOfDay;
                    TokenAct = result.access_token;
                }
            }
            else
            {
                retorna = TokenAct;     // retorna el token actual
            }
            return retorna;
        } */
        static private void CreaTablaLiteGRE()                  // llamado en el load del form, crea las tablas al iniciar
        {
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                string sqlborra = "DROP TABLE IF EXISTS dt_cabgrer; DROP TABLE IF EXISTS dt_detgrer";
                using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                {
                    cmdB.ExecuteNonQuery();
                }
                string sqlTabla = "create table dt_cabgrer (" +
                    // cabecera
                    "id integer primary key autoincrement, " +
                    "EmisRuc varchar(11), " +           // ruc del emisor de la guía
                    "EmisNom varchar(150), " +
                    "EmisUbi varchar(6), " +            // ubigeo del emisor
                    "EmisDir varchar(200), " +
                    "EmisDep varchar(50), " +
                    "EmisPro varchar(50), " +
                    "EmisDis varchar(50), " +
                    "EmisUrb varchar(50), " +           // urbanización, pueblo, localidad
                    "EmisPai varchar(2), " +            // código sunat del país emisor
                    "EmisCor varchar(100), " +          // correo del emisor de la guía
                    "NumGuia varchar(12), " +           // serie+numero
                    "FecEmis varchar(10), " +
                    "HorEmis varchar(8), " +
                    "CodGuia varchar(2), " +            // código sunat de la guía de remisión
                    "NomGuia varchar(50), " +           // glosa, texto o nombre sunat de la guía de remisión
                    "CantBul integer, " +
                    "PesoTot real, " +
                    "CodUnid varchar(3), " +             // código unidad de medida de sunat
                    "FecIniT varchar(10), " +
                    "CargaUn varchar(5), " +            // carga unica si="true", no="false"
                                                        // documentos relacionados
                    "DocRelnu1 varchar(11), " +         // código,número,identificador del documento relacionado 1
                    "DocRelti1 varchar(2), " +          // código sunat para el tipo de documento relacionado 1
                    "DocRelnr1 varchar(11), " +         // número del ruc/dni/etc del emisor del documento relacionado 1
                    "DocRelcs1 varchar(2), " +          // código sunat del tipo de documento del emisor del documento relacionado 1
                    "DocRelnm1 varchar(50), " +         // glosa, texto o nombre sunat del documento relacionado 1
                    "DocRelnu2 varchar(11), " +         // código,número,identificador del documento relacionado 2
                    "DocRelti2 varchar(2), " +          // código sunat para el tipo de documento relacionado 2
                    "DocRelnr2 varchar(11), " +         // número del ruc/dni/etc del emisor del documento relacionado 2
                    "DocRelcs2 varchar(2), " +          // código sunat del tipo de documento del emisor del documento relacionado 2
                    "DocRelnm2 varchar(50), " +         // glosa, texto o nombre sunat del documento relacionado 1
                                                        // datos del destinatario
                    "DstTipdoc varchar(2), " +          // código sunat del tipo de documento del destinatario
                    "DstNomTdo varchar(50), " +         // glosa, texto o nombre sunat del documento del destinatario
                    "DstNumdoc varchar(11), " +         // número del documento del destinatario
                    "DstNombre varchar(150), " +        // nombre o razón social del destinatario
                    "DstDirecc varchar(200), " +
                    "DstUbigeo varchar(6), " +
                    // datos del remitente
                    "RemTipdoc varchar(2), " +
                    "RemNomTdo varchar(50), " +
                    "RemNumdoc varchar(11), " +
                    "RemNombre varchar(150), " +
                    "RemDirecc varchar(200), " +
                    "RemUbigeo varchar(6), " +
                    // datos de quien paga el servicio
                    "PagTipdoc varchar(2), " +          // código del tipo de documento sunat
                    "PagNomTip varchar(50), " +         // glosa, texto o nombre sunat del documento
                    "PagNumdoc varchar(11), " +         // número del documento
                    "PagNombre varchar(150), " +        // nombre o razón social
                                                        // datos de transportista subcontratado (si lo hubiera) 
                    "SConTipdo varchar(2), " +          // código sunat del tipo de documento
                    "SConNomTi varchar(50), " +         // glosa, texto o nombre sunat del documento
                    "SConNumdo varchar(11), " +         // número del documento
                    "SconNombr varchar(150), " +        // nombre o razón social del subcontratado
                                                        // datos del envío del (los) camiones, autorizaciones de trackto y carreta
                    "EnvPlaca1 varchar(6), " +          // placa del vahículo principal (placa sin guión medio)
                    "EnvAutor1 varchar(15), " +         // número o código de autorización de circulación
                    "EnvRegis1 varchar(15), " +         // número o código del registro en la entidad autorizadora
                    "EnvCodEn1 varchar(2), " +          // código sunat de la entidad que da el registro  ( MTC=06 )
                    "EnvNomEn1 varchar(50), " +         // glosa, texto o nombre sunat de la entidad
                    "EnvPlaca2 varchar(6), " +
                    "EnvAutor2 varchar(15), " +
                    "EnvRegis2 varchar(15), " +
                    "EnvCodEn2 varchar(2), " +
                    "EnvNomEn2 varchar(50), " +
                    // datos de los choferes
                    "ChoTipDi1 varchar(1), " +          // código sunat del tipo de documento del chofer 1
                    "ChoNumDi1 varchar(11), " +         // número de documento de identidad
                    "ChoNomTi1 varchar(50), " +         // glosa, texto o nombre sunat del documento
                    "ChoNombr1 varchar(150), " +        // nombres completos del chofer 1
                    "ChoApell1 varchar(150), " +        // apellidos completos del chofer 1
                    "ChoLicen1 varchar(10), " +         // licencia de conducir del chofer 1
                    "ChoTipDi2 varchar(1), " +
                    "ChoNumDi2 varchar(11), " +
                    "ChoNomTi2 varchar(50), " +
                    "ChoNombr2 varchar(150), " +
                    "ChoApell2 varchar(150), " +
                    "ChoLicen2 varchar(10), " +
                    // datos de direcciones partida y llegada
                    "DirParUbi varchar(6), " +
                    "DirParDir varchar(200), " +
                    "DirLLeUbi varchar(6), " +
                    "DirLLeDir varchar(200), " +
                    "MotTrasCo varchar(2), " +          // codigo sunat motivo del traslado
                    "MotTrasDe varchar(200), " +          // detalle del motivo del traslado
                    "CodModTra varchar(2)" +            // código sunat modalidad de transporte
                    ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // ********************* DETALLE ************************ //
                sqlTabla = "create table dt_detgrer (" +
                    "id integer primary key autoincrement, " +
                    "NumGuia varchar(12), " +
                    "clinea integer, " +
                    "cant integer, " +
                    "codigo varchar(3), " +       // código bien o servicio
                    "peso real, " +               // peso de la carga, va unido a la unidad de medida 
                    "umed varchar(3), " +         // codigo unidad de medida de sunat
                    "deta1 varchar(100), " +
                    "deta2 varchar(100))";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private bool llenaTablaLiteGRE()                        // llena tabla con los datos de la guía y llama al app que crea el xml
        {
            bool retorna = false;
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                // CABECERA
                    string metela = "insert into dt_cabgrer (" +
                        "EmisRuc,EmisNom,EmisUbi,EmisDir,EmisDep,EmisPro,EmisDis,EmisUrb,EmisPai,EmisCor,NumGuia,FecEmis,HorEmis,CodGuia,NomGuia,CantBul,PesoTot,CodUnid,FecIniT,CargaUn," +
                        "DocRelnu1,DocRelti1,DocRelnr1,DocRelcs1,DocRelnm1,DocRelnu2,DocRelti2,DocRelnr2,DocRelcs2,DocRelnm2," +
                        "DstTipdoc,DstNomTdo,DstNumdoc,DstNombre,DstDirecc,DstUbigeo," +
                        "RemTipdoc,RemNomTdo,RemNumdoc,RemNombre,RemDirecc,RemUbigeo," +
                        "SConTipdo,SConNomTi,SConNumdo,SconNombr," +
                        "EnvPlaca1,EnvAutor1,EnvRegis1,EnvCodEn1,EnvNomEn1,EnvPlaca2,EnvAutor2,EnvRegis2,EnvCodEn2,EnvNomEn2," +
                        "ChoTipDi1,ChoNumDi1,ChoNomTi1,ChoNombr1,ChoApell1,ChoLicen1,ChoTipDi2,ChoNumDi2,ChoNomTi2,ChoNombr2,ChoApell2,ChoLicen2," +
                        "DirParUbi,DirParDir,DirLLeUbi,DirLLeDir,MotTrasCo,MotTrasDe,CodModTra) " +
                        "values (" +
                        "@EmisRuc,@EmisNom,@EmisUbi,@EmisDir,@EmisDep,@EmisPro,@EmisDis,@EmisUrb,@EmisPai,@EmisCor,@NumGuia,@FecEmis,@HorEmis,@CodGuia,@NomGuia,@CantBul,@PesoTot,@CodUnid,@FecIniT,@CargaUn," +
                        "@DocRelnu1,@DocRelti1,@DocRelnr1,@DocRelcs1,@DocRelnm1,@DocRelnu2,@DocRelti2,@DocRelnr2,@DocRelcs2,@DocRelnm2," +
                        "@DstTipdoc,@DstNomTdo,@DstNumdoc,@DstNombre,@DstDirecc,@DstUbigeo," +
                        "@RemTipdoc,@RemNomTdo,@RemNumdoc,@RemNombre,@RemDirecc,@RemUbigeo," +
                        "@SConTipdo,@SConNomTi,@SConNumdo,@SconNombr," +
                        "@EnvPlaca1,@EnvAutor1,@EnvRegis1,@EnvCodEn1,@EnvNomEn1,@EnvPlaca2,@EnvAutor2,@EnvRegis2,@EnvCodEn2,@EnvNomEn2," +
                        "@ChoTipDi1,@ChoNumDi1,@ChoNomTi1,@ChoNombr1,@ChoApell1,@ChoLicen1,@ChoTipDi2,@ChoNumDi2,@ChoNomTi2,@ChoNombr2,@ChoApell2,@ChoLicen2," +
                        "@DirParUbi,@DirParDir,@DirLLeUbi,@DirLLeDir,@MotTrasCo,@MotTrasDe,@CodModTra)";
                //                        "PagTipdoc,PagNomTip,PagNumdoc,PagNombre," +
                //                        "@PagTipdoc,@PagNomTip,@PagNumdoc,@PagNombre," +
                using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                    {
                        // cabecera
                        cmd.Parameters.AddWithValue("@EmisRuc", Program.ruc);                 // "20430100344"
                        cmd.Parameters.AddWithValue("@EmisNom", Program.cliente);             // "J&L Technology SAC"
                        cmd.Parameters.AddWithValue("@EmisUbi", Program.ubidirfis);           // "070101"
                        cmd.Parameters.AddWithValue("@EmisDir", Program.dirfisc);             // "Calle Sigma Mz.A19 Lt.16 Sector I"
                        cmd.Parameters.AddWithValue("@EmisDep", Program.depfisc);             // "Callao"
                        cmd.Parameters.AddWithValue("@EmisPro", Program.provfis);             // "Callao"
                        cmd.Parameters.AddWithValue("@EmisDis", Program.distfis);             // "Callao"
                        cmd.Parameters.AddWithValue("@EmisUrb", "-");                         // "Bocanegra"
                        cmd.Parameters.AddWithValue("@EmisPai", "PE");                        // país del emisor
                        cmd.Parameters.AddWithValue("@EmisCor", Program.mailclte);            // "neto.solorzano@solorsoft.com"
                        cmd.Parameters.AddWithValue("@NumGuia", tx_serie.Text + "-" + tx_numero.Text);         // "V001-98000006"
                        cmd.Parameters.AddWithValue("@FecEmis", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));              // "2023-05-19"
                        cmd.Parameters.AddWithValue("@HorEmis", lib.Right("0" + DateTime.Now.Hour,2) + ":" + lib.Right("0" + DateTime.Now.Minute,2) + ":" + lib.Right("0" + DateTime.Now.Second,2));  // DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second
                        cmd.Parameters.AddWithValue("@CodGuia", "09");
                        cmd.Parameters.AddWithValue("@NomGuia", "GUIA REMISION REMITENTE");
                        cmd.Parameters.AddWithValue("@CantBul", 1);
                        cmd.Parameters.AddWithValue("@PesoTot", tx_totpes.Text);              // 30
                        cmd.Parameters.AddWithValue("@CodUnid", (rb_tn.Checked == true) ? "TNE" : "KGM");           // "KGM"
                        cmd.Parameters.AddWithValue("@FecIniT", tx_pla_fech.Text);          // "2023-05-19"
                        cmd.Parameters.AddWithValue("@CargaUn", (chk_cunica.Checked == true) ? "true" : "false");   // "true"
                        // documentos relacionados
                        cmd.Parameters.AddWithValue("@DocRelnu1", tx_docsOr.Text);            // "001-00054323" OJO, me esta validando 12 caracteres = SSS-NNNNNNNN | debería ser 13, ver en producción
                        cmd.Parameters.AddWithValue("@DocRelti1", tx_dat_dorigS.Text);        // "09"
                        cmd.Parameters.AddWithValue("@DocRelnr1", tx_rucEorig.Text);          // "20430100344"
                        cmd.Parameters.AddWithValue("@DocRelcs1", "6");                       // sunat 6 = tipo documento ruc
                        cmd.Parameters.AddWithValue("@DocRelnm1", cmb_docorig.Text.ToUpper());    // "GUIA DE REMISION REMITENTE"
                        cmd.Parameters.AddWithValue("@DocRelnu2", tx_docsOr2.Text);
                        cmd.Parameters.AddWithValue("@DocRelti2", tx_dat_dorigS2.Text);
                        cmd.Parameters.AddWithValue("@DocRelnr2", tx_rucEorig2.Text);
                        cmd.Parameters.AddWithValue("@DocRelcs2", "6");                      // como se pide el ruc del emisor  entonces el tipo es 6
                        cmd.Parameters.AddWithValue("@DocRelnm2", cmb_docorig2.Text);
                        // datos del destinatario
                        cmd.Parameters.AddWithValue("@DstTipdoc", tx_dat_codsu.Text);       // "1"
                        cmd.Parameters.AddWithValue("@DstNomTdo", tx_dat_nomcsd.Text);      // "Documento Nacional de Identidad"
                        cmd.Parameters.AddWithValue("@DstNumdoc", tx_numDocDes.Text);       // "09314486"
                        cmd.Parameters.AddWithValue("@DstNombre", tx_nomDrio.Text);
                        cmd.Parameters.AddWithValue("@DstDirecc", tx_dirDrio.Text);
                        cmd.Parameters.AddWithValue("@DstUbigeo", tx_ubigDtt.Text);         // "070101"
                        // datos del remitente
                        cmd.Parameters.AddWithValue("@RemTipdoc", tx_dat_csrem.Text);
                        cmd.Parameters.AddWithValue("@RemNomTdo", tx_dat_nomcsr.Text);        // "Documento Nacional de Identidad"
                        cmd.Parameters.AddWithValue("@RemNumdoc", tx_numDocRem.Text);        // "10401018"
                        cmd.Parameters.AddWithValue("@RemNombre", tx_nomRem.Text);
                        cmd.Parameters.AddWithValue("@RemDirecc", tx_dirRem.Text);         // "Bocanegra sector 1"
                        cmd.Parameters.AddWithValue("@RemUbigeo", tx_ubigRtt.Text);        // "070101"
                        /* datos de quien paga el servicio
                        if (rb_pOri.Checked == true)        // paga remitente
                        {
                            cmd.Parameters.AddWithValue("@PagTipdoc", tx_dat_csrem.Text);
                            cmd.Parameters.AddWithValue("@PagNomTip", tx_dat_nomcsr.Text);
                            cmd.Parameters.AddWithValue("@PagNumdoc", tx_numDocRem.Text);
                            cmd.Parameters.AddWithValue("@PagNombre", tx_nomRem.Text);
                        }
                        if (rb_pDes.Checked == true)        // paga destinatario
                        {
                            cmd.Parameters.AddWithValue("@PagTipdoc", tx_dat_codsu.Text);
                            cmd.Parameters.AddWithValue("@PagNomTip", tx_dat_nomcsd.Text);
                            cmd.Parameters.AddWithValue("@PagNumdoc", tx_numDocDes.Text);
                            cmd.Parameters.AddWithValue("@PagNombre", tx_nomDrio.Text);
                        }
                        */
                        // datos de transportista 
                        if (tx_pla_ruc.Text != Program.ruc)     // valida si es carro contratado
                        {
                            cmd.Parameters.AddWithValue("@SConTipdo", "6");     // por defecto el tipo es 6 = Ruc
                            cmd.Parameters.AddWithValue("@SConNomTi", "Registro Unico de Contributentes");
                            cmd.Parameters.AddWithValue("@SConNumdo", tx_pla_ruc.Text);
                            cmd.Parameters.AddWithValue("@SconNombr", tx_pla_propiet.Text);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@SConTipdo", "");
                            cmd.Parameters.AddWithValue("@SConNomTi", "");
                            cmd.Parameters.AddWithValue("@SConNumdo", "");
                            cmd.Parameters.AddWithValue("@SconNombr", "");
                        }
                        // datos del envío del (los) camiones, autorizaciones de trackto y carreta
                        cmd.Parameters.AddWithValue("@EnvPlaca1", tx_pla_placa.Text);        // "F2N714"
                        cmd.Parameters.AddWithValue("@EnvAutor1", tx_pla_autor.Text);        // Certificado de habilitación
                        cmd.Parameters.AddWithValue("@EnvRegis1", tx_dat_plaNreg.Text);        // "1550877CNG"
                        cmd.Parameters.AddWithValue("@EnvCodEn1", "06");
                        cmd.Parameters.AddWithValue("@EnvNomEn1", "Ministerio de Transportes y Comunicaciones");
                        cmd.Parameters.AddWithValue("@EnvPlaca2", tx_pla_carret.Text);        // "AYS991"
                        cmd.Parameters.AddWithValue("@EnvAutor2", tx_aut_carret.Text);        // "15M21028161E"
                        cmd.Parameters.AddWithValue("@EnvRegis2", tx_dat_carrNreg.Text);      // número de registro/autorización
                        cmd.Parameters.AddWithValue("@EnvCodEn2", "06");
                        cmd.Parameters.AddWithValue("@EnvNomEn2", "Ministerio de Transportes y Comunicaciones");
                        // datos de los choferes
                        cmd.Parameters.AddWithValue("@ChoTipDi1", "1");
                        cmd.Parameters.AddWithValue("@ChoNumDi1", tx_pla_dniChof.Text);
                        cmd.Parameters.AddWithValue("@ChoNomTi1", "Documento Nacional de Identidad");
                        cmd.Parameters.AddWithValue("@ChoNombr1", partidor(tx_pla_nomcho.Text, " ")[0]);        // tx_pla_nomcho.Text
                        cmd.Parameters.AddWithValue("@ChoApell1", partidor(tx_pla_nomcho.Text, " ")[1]);
                        cmd.Parameters.AddWithValue("@ChoLicen1", tx_pla_brevet.Text);        // "U46785663"
                        cmd.Parameters.AddWithValue("@ChoTipDi2", (tx_dat_dniC2.Text != "") ? "1" : "");
                        cmd.Parameters.AddWithValue("@ChoNumDi2", tx_dat_dniC2.Text);
                        cmd.Parameters.AddWithValue("@ChoNomTi2", (tx_dat_dniC2.Text != "") ? "Documento Nacional de Identidad" : "");
                        cmd.Parameters.AddWithValue("@ChoNombr2", partidor(tx_pla_chofer2.Text, " ")[0]);     // tx_pla_chofer2.Text
                        cmd.Parameters.AddWithValue("@ChoApell2", partidor(tx_pla_chofer2.Text, " ")[1]);
                        cmd.Parameters.AddWithValue("@ChoLicen2", tx_pla_brev2.Text);
                        // datos de direcciones partida y llegada
                        cmd.Parameters.AddWithValue("@DirParUbi", tx_ubigRtt.Text);         //  "150115"
                        cmd.Parameters.AddWithValue("@DirParDir", tx_dirRem.Text);
                        cmd.Parameters.AddWithValue("@DirLLeUbi", tx_ubigDtt.Text);
                        cmd.Parameters.AddWithValue("@DirLLeDir", tx_dirDrio.Text);
                        // motivo de traslado
                        cmd.Parameters.AddWithValue("@MotTrasCo", tx_dat_motrasS.Text);
                        cmd.Parameters.AddWithValue("@MotTrasDe", tx_motras.Text);
                    // modalidad de transporte 
                    cmd.Parameters.AddWithValue("@CodModTra", (Program.ruc == tx_pla_ruc.Text) ? "02" : "01");  // 01->Transp. público, 02->Transp.privado (propio)
                    cmd.ExecuteNonQuery();
                    }
                    
                    // DETALLE
                    metela = "insert into dt_detgrer (" +
                        "NumGuia,clinea,cant,codigo,peso,umed,deta1,deta2) values (" +
                        "@NumGuia,@clinea,@cant,@codigo,@peso,@umed,@deta1,@deta2)";
                using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                {
                    cmd.Parameters.AddWithValue("@NumGuia", tx_serie.Text + "-" + tx_numero.Text);      // "V001-98000006"
                    cmd.Parameters.AddWithValue("@clinea", 1);
                    cmd.Parameters.AddWithValue("@cant", tx_det_cant.Text);                             // 30
                    cmd.Parameters.AddWithValue("@codigo", "ZZ");
                    cmd.Parameters.AddWithValue("@peso", tx_det_peso.Text);                             // 150
                    cmd.Parameters.AddWithValue("@umed", (rb_kg.Checked == true) ? "KGM" : "TNE");       // "KGM"
                    cmd.Parameters.AddWithValue("@deta1", lb_glodeta.Text);    // "Servicio de Transporte de carga terrestre "
                    cmd.Parameters.AddWithValue("@deta2", tx_det_desc.Text);    //"Dice contener Enseres domésticos"

                    cmd.ExecuteNonQuery();                      
                }
                // llamada al programa de generación del xml de la guía
                string rutalocal = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string[] parametros = new string[] { rutaxml, Program.ruc, tx_serie.Text + "-" + tx_numero.Text };
                ProcessStartInfo p = new ProcessStartInfo();
                p.Arguments = rutaxml + " " + Program.ruc + " " + tx_serie.Text + "-" + tx_numero.Text + " " + firmDocElec + " " + rutaCertifc + " " + claveCertif + " " + "09";
                p.FileName = @rutalocal + "/xmlGRE/xmlGRE.exe";
                var proc = Process.Start(p) ;
                proc.WaitForExit();
                if (proc.ExitCode == 1) retorna = true;
                else retorna = false;
                retorna = true;
            }

            return retorna;
        }
        /* public string convierteCDR(string arCdr, string serie, string corre, string ruta)               // genera el cdr a partir de la respuesta de sunat arcCDR del json
        {
            string retorna = "";

            if (File.Exists(ruta + "temporal.zip"))   // @"c:/temp/temporal.zip"
            {
                File.Delete(ruta + "temporal.zip");   // @"c:/temp/temporal.zip"
            }
            string archi = "R-" + Program.ruc + "-" + "09" + "-" + serie + "-" + corre + ".xml";
            if (File.Exists(ruta + archi))           // @"c:/temp/" + archi
            {
                File.Delete(ruta + archi);           // @"c:/temp/" + archi
            }
            // grabamos en memoria el xml y obtenemos el dato del tag <cbc:DocumentDescription> ahí esta el texto a convertir en código QR
            //byte[] xmlbytes = Base64DecodeString(arCdr);
            byte[] xmlbytes = Convert.FromBase64CharArray(arCdr.ToCharArray(), 0, arCdr.Length);
            FileStream fstrm = new FileStream(ruta + "temporal.zip", FileMode.CreateNew, FileAccess.Write);   // @"c:/temp/temporal.zip"
            BinaryWriter writer = new BinaryWriter(fstrm);
            writer.Write(xmlbytes);
            writer.Close();
            fstrm.Close();

            System.IO.Compression.ZipFile.ExtractToDirectory(ruta + "temporal.zip", ruta);        // @"c:/temp/temporal.zip", @"c:/temp/"
            FileStream archiS = new FileStream(ruta + archi, FileMode.Open, FileAccess.Read);        // @"c:/temp/" + archi, FileMode.Open, FileAccess.Read
            XmlDocument archiXml = new XmlDocument();
            archiXml.Load(archiS);
            XmlNode fqr = archiXml.GetElementsByTagName("DocumentDescription", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2").Item(0);
            retorna = fqr.InnerText;

            return retorna;
        } */
        #endregion Sunat metodo directo

        #endregion

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            #region validaciones generales del form
            if (tx_serie.Text.Trim() == "")
            {
                tx_serie.Focus();
                return;
            }
            // aca va la validacion de la numeracion
            if (tx_n_auto.Text == "M" && tx_numero.Text.Trim() == "")
            {
                tx_numero.Focus();
                return;
            }
            if (tx_dat_locori.Text.Trim() == "")
            {
                cmb_origen.Focus();
                return;
            }
            if (tx_ubigO.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese ubigeo correcto", " Error en origen! ");
                tx_ubigO.Focus();
                return;
            }
            if (tx_dat_locdes.Text.Trim() == "")
            {
                cmb_destino.Focus();
                return;
            }
            if (tx_ubigD.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese ubigeo correcto", " Error en destino! ");
                tx_ubigD.Focus();
                return;
            }
            if (tx_totcant.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta cantidad ");
                tx_det_cant.Focus();
                return;
            }
            if (tx_totpes.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta peso ");
                tx_det_peso.Focus();
                return;
            }
            if (tx_det_umed.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta unidad medida ");
                tx_det_umed.Focus();
                return;
            }
            if (tx_det_desc.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta detalle");
                tx_det_desc.Focus();
                return;

            }
            if (tx_dat_tdRem.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de documento", " Error en Remitente ");
                tx_dat_tdRem.Focus();
                return;
            }
            if (tx_numDocRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de documento", " Error en Remitente ");
                tx_numDocRem.Focus();
                return;
            }
            if (tx_nomRem.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el nombre o razón social", " Error en Remitente ");
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
                MessageBox.Show("Complete la dirección, departamento, provincia y distrito", "Error en remitente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_dirRem.Focus();
                return;
            }
            if (tx_dat_tDdest.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de documento", " Error en Destinatario ");
                tx_dat_tDdest.Focus();
                return;
            }
            if (tx_numDocDes.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de documento", " Error en Destinatario ");
                tx_numDocDes.Focus();
                return;
            }
            if (tx_nomDrio.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el nombre o razón social", " Error en Destinatario ");
                tx_nomDrio.Focus();
                return;
            }
            if (tx_dirDrio.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la dirección", " Error en Destinatario ");
                tx_dirDrio.Focus();
                return;
            }
            if (tx_dptoDrio.Text.Trim() == "" || tx_proDrio.Text.Trim() == "" || tx_disDrio.Text.Trim() == "")
            {
                MessageBox.Show("Complete la dirección, departamento, provincia y distrito", "Error en destinatario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_dirDrio.Focus();
                return;
            }
            if (tx_ubigRtt.Text.Trim().Length != 6)
            {
                MessageBox.Show("Seleccione correctamente Departamento, Provincia y Distrito","Seleccione en orden",MessageBoxButtons.OK,MessageBoxIcon.Error);
                tx_dptoRtt.Focus();
                return;
            }
            if (tx_ubigDtt.Text.Trim().Length != 6)
            {
                MessageBox.Show("Seleccione correctamente Departamento, Provincia y Distrito", "Seleccione en orden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tx_dptoDrio.Focus();
                return;
            }
            if (tx_dat_mone.Text.Trim() == "")
            {
                // por compatibilidad con la B.D. ponemos moneda local por defecto
                tx_dat_mone.Text = MonDeft;
            }
            #endregion
            #region validaciones GR electrónicas Sunat
            // documentos relacionados
            if (tx_dat_docOr.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione un documento origen","Faltan datos",MessageBoxButtons.OK,MessageBoxIcon.Information);
                cmb_docorig.Focus();
                return;
            }
            if (tx_docsOr.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el documento origen", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_docsOr.Focus();
                return;
            }
            if (tx_rucEorig.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el ruc del emisor del documento origen", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_rucEorig.Focus();
                return;
            }
            if (tx_dat_docOr2.Text.Trim() == "" && tx_docsOr2.Text.Trim() != "")
            {
                MessageBox.Show("Seleccione el tipo de documento origen 2", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmb_docorig2.Focus();
                return;
            }
            if (tx_dat_docOr2.Text.Trim() != "" && tx_docsOr2.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el documento origen 2", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_docsOr2.Focus();
                return;
            }
            if (tx_dat_docOr2.Text.Trim() != "" && tx_rucEorig2.Text == "")
            {
                MessageBox.Show("Ingrese el ruc del documento origen 2", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_rucEorig2.Focus();
                return;
            }
            // validaciones de traslado
            if (tx_dat_motras.Text == "")
            {
                MessageBox.Show("Debe seleccionar un motivo de traslado", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmb_motras.Focus();
                return;
            }
            if (tx_motras.Text.Trim() == "" || tx_motras.Text.Trim().Length < 5)
            {
                MessageBox.Show("Debe ingresar un detalle del traslado", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_motras.Focus();
                return;
            }
            // validaciones SUNAT - motivos de traslado importacion 08 / exportacion 09 y sus tipos de documentos relacionados
            if ("'08','09'".Contains(tx_dat_motrasS.Text))
            {
                if (!("'50','52'").Contains(tx_dat_dorigS.Text))
                {
                    MessageBox.Show("No se ha ingresado el tipo Declaracion Aduanera de Mercancias (DAM) o la " + Environment.NewLine +
                        "Declaracion Simplificada (DS) para el motivo de traslado selecionado", "Validación Sunat", MessageBoxButtons.OK,MessageBoxIcon.Information);
                    cmb_docorig.Focus();
                    return;
                }
                if (!"'09','49','80',''".Contains(tx_dat_dorigS2.Text))
                {
                    MessageBox.Show("El tipo de documento relacionado no corresponde para el motivo de traslado seleccionado", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmb_docorig2.Focus();
                    return;
                }
            }
            if (!"'08','09','13'".Contains(tx_dat_motrasS.Text))
            {
                if (("'50','52'").Contains(tx_dat_dorigS.Text))
                {
                    MessageBox.Show("El tipo de documento relacionado no corresponde con el motivo de traslado seleccionado", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmb_docorig.Focus();
                    return;
                }
                if (tx_dat_dorigS2.Text != "" && ("'50','52'").Contains(tx_dat_dorigS2.Text))
                {
                    MessageBox.Show("El tipo de documento relacionado no corresponde con el motivo de traslado seleccionado", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmb_docorig2.Focus();
                    return;
                }
            }
            // validaciones SUNAT - partida arancelaria en detalle cuando hay documentos relacionados 49 u 80
            if (("'49','80'").Contains(tx_dat_dorigS.Text) || (tx_dat_dorigS2.Text != "" && ("'49','80'").Contains(tx_dat_dorigS2.Text))) 
            {
                var aa = MessageBox.Show("Asegurese de tener en el detalle un item con Partida Arancelaria" + Environment.NewLine +
                    "Confirma que lo tiene?", "Validación Sunat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.No) return;
            }
            // validaciones SUNAT - formatos del número de documento origen
            if ("'01','03','04','09'".Contains(tx_dat_dorigS.Text))
            {
                if (tx_docsOr.Text.Length > 13 || tx_docsOr.Text.Length < 3 || !tx_docsOr.Text.Contains("-") || lib.repeticiones(tx_docsOr.Text, "-") > 1 ||
                    lib.separador(tx_docsOr.Text, '-', 1).Length > 4 || lib.separador(tx_docsOr.Text, '-', 1).Length < 1 ||
                    lib.separador(tx_docsOr.Text, '-', 2).Length > 8 || lib.separador(tx_docsOr.Text, '-', 2).Length < 1 ||
                    lib.IsAllDigits(lib.separador(tx_docsOr.Text, '-', 2)) == false || int.Parse(lib.separador(tx_docsOr.Text, '-', 2)) <= 0)
                {
                    MessageBox.Show("El formato del comprobante no es correcto, debe ser:" + Environment.NewLine +
                        "<serie(4 caracteres)>-<número(8 números)" + Environment.NewLine +
                        "El campo <Numero> debe ser mayor a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                   // facturas,boletas,liq.de compras, guias de remision
            if (tx_dat_dorigS2.Text != "" && "'01','03','04','09'".Contains(tx_dat_dorigS2.Text)) 
            {
                if (tx_docsOr2.Text.Length > 13 || tx_docsOr2.Text.Length < 3 || !tx_docsOr2.Text.Contains("-") || lib.repeticiones(tx_docsOr2.Text, "-") > 1 ||
                    lib.separador(tx_docsOr2.Text, '-', 1).Length > 4 || lib.separador(tx_docsOr2.Text, '-', 1).Length < 1 ||
                    lib.separador(tx_docsOr2.Text, '-', 2).Length > 8 || lib.separador(tx_docsOr2.Text, '-', 2).Length < 1 ||
                    lib.IsAllDigits(lib.separador(tx_docsOr2.Text, '-', 2)) == false || int.Parse(lib.separador(tx_docsOr2.Text, '-', 2)) <= 0)
                {
                    MessageBox.Show("El formato del comprobante no es correcto, debe ser:" + Environment.NewLine +
                        "<serie(4 caracteres)>-<número(8 números)" + Environment.NewLine +
                        "El campo <Numero> debe ser mayor a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }     // facturas,boletas,liq.de compras, guias de remision
            if ("12".Contains(tx_dat_dorigS.Text))
            {
                if (tx_docsOr.Text.Length < 3 || tx_docsOr.Text.Length > 41 || !tx_docsOr.Text.Contains("-") || lib.repeticiones(tx_docsOr.Text, "-") > 1 ||
                    lib.separador(tx_docsOr.Text, '-', 1).Length > 20 || lib.separador(tx_docsOr.Text, '-', 1).Length < 1 || 
                    lib.separador(tx_docsOr.Text, '-', 2).Length > 20 || lib.separador(tx_docsOr.Text, '-', 2).Length < 1)
                {
                    MessageBox.Show("El formato del ticket/cinta no es correcto, debe ser:" + Environment.NewLine +
                            "<serie>-<número> con la siguiente estructura:" + Environment.NewLine +
                            "[a-zA-Z0-9]{1,20}-[a-zA-Z0-9]{1,20}", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                                   // Ticket, cintas de maquinas registradoras
            if (tx_dat_dorigS2.Text != "" && "12".Contains(tx_dat_dorigS2.Text))
            {
                if (tx_docsOr2.Text.Length < 3 || tx_docsOr2.Text.Length > 41 || !tx_docsOr2.Text.Contains("-") || lib.repeticiones(tx_docsOr2.Text, "-") > 1 ||
                    lib.separador(tx_docsOr2.Text, '-', 1).Length > 20 || lib.separador(tx_docsOr2.Text, '-', 1).Length < 1 ||
                    lib.separador(tx_docsOr2.Text, '-', 2).Length > 20 || lib.separador(tx_docsOr2.Text, '-', 2).Length < 1)
                {
                    MessageBox.Show("El formato del ticket/cinta no es correcto, debe ser:" + Environment.NewLine +
                            "<serie>-<número> con la siguiente estructura:" + Environment.NewLine +
                            "[a-zA-Z0-9]{1,20}-[a-zA-Z0-9]{1,20}", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }                   // Ticket, cintas de maquinas registradoras
            if ("48".Contains(tx_dat_dorigS.Text))
            {
                if (tx_docsOr.Text.Trim().Length < 3 || tx_docsOr.Text.Length > 12 || !tx_docsOr.Text.Contains("-") || lib.repeticiones(tx_docsOr.Text,"-") > 1 ||
                    lib.IsAllDigits(lib.separador(tx_docsOr.Text, '-', 2)) == false || lib.separador(tx_docsOr.Text, '-', 2).Length > 7 ||
                    int.Parse(lib.separador(tx_docsOr.Text, '-', 2)) <= 0)
                {
                    MessageBox.Show("El formato del comprobante no es correcto, debe ser:" + Environment.NewLine +
                            "<serie>-<número> con esta estructura [0-9]{1,4}-[0-9]{1,7}" + Environment.NewLine +
                            "El campo <número> debe ser mayor a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                                   // Comprobante de operaciones
            if (tx_dat_dorigS2.Text != "" && "48".Contains(tx_dat_dorigS2.Text))
            {
                if (tx_docsOr2.Text.Trim().Length < 3 || tx_docsOr2.Text.Length > 12 || !tx_docsOr2.Text.Contains("-") || lib.repeticiones(tx_docsOr2.Text, "-") > 1 ||
                    lib.IsAllDigits(lib.separador(tx_docsOr2.Text, '-', 2)) == false || lib.separador(tx_docsOr2.Text, '-', 2).Length > 7 || 
                    int.Parse(lib.separador(tx_docsOr2.Text, '-', 2)) <= 0)
                {
                    MessageBox.Show("El formato del comprobante no es correcto, debe ser:" + Environment.NewLine +
                            "<serie>-<número> con esta estructura [0-9]{1,4}-[0-9]{1,7}" + Environment.NewLine +
                            "El campo <número> debe ser mayor a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }                   // Comprobante de operaciones
            if ("'49','80'".Contains(tx_dat_dorigS.Text))
            {
                if (tx_docsOr.Text.Length > 15 || tx_docsOr.Text.Length < 3 || lib.IsAllDigits(tx_docsOr.Text) == false || 
                    int.Parse(tx_docsOr.Text) <= 0)
                {
                    MessageBox.Show("El formato de la constancia no es correcto, debe ser:" + Environment.NewLine +
                            "Solo números no mayor a 15 dígitos > a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                             // Constancia de deposito
            if (tx_dat_dorigS2.Text != "" && "'49','80'".Contains(tx_dat_dorigS2.Text))
            {
                if (tx_docsOr2.Text.Length > 15 || tx_docsOr2.Text.Length < 3 || lib.IsAllDigits(tx_docsOr2.Text) == false ||
                    int.Parse(tx_docsOr2.Text) <= 0)
                {
                    MessageBox.Show("El formato de la constancia no es correcto, debe ser:" + Environment.NewLine +
                            "Solo números no mayor a 15 dígitos > a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }               // Constancia de deposito
            if ("81".Contains(tx_dat_dorigS.Text))
            {
                if (tx_docsOr.Text.Length > 20 || tx_docsOr.Text.Length < 3)
                {
                    MessageBox.Show("El formato de la autorización SCOP no es correcto, debe ser:" + Environment.NewLine +
                            "Solo letras y números no mayor a 20 caracteres y > a 2", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                                    // Código de autorización SCOP
            if (tx_dat_dorigS2.Text != "" && "81".Contains(tx_dat_dorigS2.Text))
            {
                if (tx_docsOr2.Text.Length > 20 || tx_docsOr2.Text.Length < 3)
                {
                    MessageBox.Show("El formato de la autorización SCOP no es correcto, debe ser:" + Environment.NewLine +
                            "Solo letras y números no mayor a 20 caracteres y > a 2", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }                      // Código de autorización SCOP
            if ("50".Contains(tx_dat_dorigS.Text))
            {
                if (!"'08','09'".Contains(tx_dat_motrasS.Text))
                {
                    // el motivo de traslado NO es 08 ó 09
                    if (tx_docsOr.Text.Length > 18 || tx_docsOr.Text.Length < 13 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
                        lib.separador(tx_docsOr.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr.Text, '-', 3).Length != 2)
                    {
                        MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-{2}-{6}, Ejemplo: 123-2023-99-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, [0-9]{2} Régimen aduanero, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx_docsOr.Focus();
                        return;
                    }
                }
                else
                {
                    if (tx_dat_motrasS.Text == "08")        // si es motivos de traslado 08 ó 09
                    {
                        if (tx_docsOr.Text.Length > 26 || tx_docsOr.Text.Length < 12 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
                        lib.separador(tx_docsOr.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-10-{6}, Ejemplo: 123-2023-10-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 10, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }
                    if (tx_dat_motrasS.Text == "09")        // tx_dat_motrasS.Text == "09"
                    {
                        if (tx_docsOr.Text.Length > 26 || tx_docsOr.Text.Length < 12 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
                        lib.separador(tx_docsOr.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-40-{6}, Ejemplo: 123-2023-10-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 40, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }
                }
            }                                                   // Declaración de aduana DAM
            if (tx_dat_dorigS2.Text != "" && "50".Contains(tx_dat_dorigS2.Text))
            {
                if (!"'08','09'".Contains(tx_dat_motrasS.Text))
                {
                    // el motivo de traslado NO es 08 ó 09
                    if (tx_docsOr2.Text.Length > 18 || tx_docsOr2.Text.Length < 13 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
                        lib.separador(tx_docsOr2.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr2.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr2.Text, '-', 3).Length != 2)
                    {
                        MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-{2}-{6}, Ejemplo: 123-2023-99-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, [0-9]{2} Régimen aduanero, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx_docsOr2.Focus();
                        return;
                    }
                }
                else
                {

                    if (tx_dat_motrasS.Text == "08")        // si es motivos de traslado 08 ó 09
                    {
                        if (tx_docsOr2.Text.Length > 26 || tx_docsOr2.Text.Length < 12 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
                        lib.separador(tx_docsOr2.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr2.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr2.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr2.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr2.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-10-{6}, Ejemplo: 123-2023-10-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 10, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr2.Focus();
                            return;
                        }
                    }
                    if (tx_dat_motrasS.Text == "09")        // tx_dat_motrasS.Text == "09"
                    {
                        if (tx_docsOr2.Text.Length > 26 || tx_docsOr2.Text.Length < 12 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
                        lib.separador(tx_docsOr2.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr2.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr2.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr2.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr2.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-40-{6}, Ejemplo: 123-2023-10-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 40, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr2.Focus();
                            return;
                        }
                    }
                }
            }                       // Declaración de aduana DAM
            if ("52".Contains(tx_dat_dorigS.Text))
            {
                if (!"'08','09'".Contains(tx_dat_motrasS.Text))
                {
                    // el motivo de traslado NO es 08 ó 09
                    if (tx_docsOr.Text.Length > 18 || tx_docsOr.Text.Length < 13 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
                        lib.separador(tx_docsOr.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr.Text, '-', 3).Length != 2)
                    {
                        MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-{2}-{6}, Ejemplo: 123-2023-99-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, [0-9]{2} Régimen aduanero, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx_docsOr.Focus();
                        return;
                    }
                }
                else
                {
                    if (tx_dat_motrasS.Text == "08")        // si es motivos de traslado 08 ó 09
                    {
                        if (tx_docsOr.Text.Length > 26 || tx_docsOr.Text.Length < 12 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
                        lib.separador(tx_docsOr.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-18-{6}, Ejemplo: 123-2023-18-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 18, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }
                    if (tx_dat_motrasS.Text == "09")        // tx_dat_motrasS.Text == "09"
                    {
                        if (tx_docsOr.Text.Length > 26 || tx_docsOr.Text.Length < 12 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
                        lib.separador(tx_docsOr.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-48-{6}, Ejemplo: 123-2023-48-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 48, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }
                }
            }                                                   // Declaración jurada de Mudanza
            if (tx_dat_dorigS2.Text != "" && "52".Contains(tx_dat_dorigS2.Text))
            {
                if (!"'08','09'".Contains(tx_dat_motrasS.Text))
                {
                    // el motivo de traslado NO es 08 ó 09
                    if (tx_docsOr2.Text.Length > 18 || tx_docsOr2.Text.Length < 13 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
                        lib.separador(tx_docsOr2.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr2.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr2.Text, '-', 3).Length != 2)
                    {
                        MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-{2}-{6}, Ejemplo: 123-2023-99-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, [0-9]{2} Régimen aduanero, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx_docsOr2.Focus();
                        return;
                    }
                }
                else
                {
                    if (tx_dat_motrasS.Text == "08")        // si es motivos de traslado 08 ó 09
                    {
                        if (tx_docsOr2.Text.Length > 26 || tx_docsOr2.Text.Length < 12 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
                        lib.separador(tx_docsOr2.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr2.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr2.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr2.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr2.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-18-{6}, Ejemplo: 123-2023-18-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 18, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr2.Focus();
                            return;
                        }
                    }
                    if (tx_dat_motrasS.Text == "09")        // tx_dat_motrasS.Text == "09"
                    {
                        if (tx_docsOr2.Text.Length > 26 || tx_docsOr2.Text.Length < 12 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
                        lib.separador(tx_docsOr2.Text, '-', 1).Length != 3 || lib.separador(tx_docsOr2.Text, '-', 2).Length != 4 ||
                        lib.separador(tx_docsOr2.Text, '-', 3).Length != 2 || lib.separador(tx_docsOr2.Text, '-', 4).Length < 1 || int.Parse(lib.separador(tx_docsOr2.Text, '-', 4)) == 0)
                        {
                            MessageBox.Show("El formato de la declaración no es correcto, debe ser:" + Environment.NewLine +
                                " {3}-{4}-48-{6}, Ejemplo: 123-2023-48-1234" + Environment.NewLine +
                                " [0-9]{3}: Código de la Aduana, [0-9]{4}: Año, 48, [0-9]{1,6} Correlativo" + Environment.NewLine +
                                "El campo Correlativo debe ser > 0", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr2.Focus();
                            return;
                        }
                    }
                }
            }                     // Declaración jurada de Mudanza
            if ("'71','72','73','74','75','76','77','78'".Contains(tx_dat_dorigS.Text))
            {
                // acá se permite todo menos espacios en blanco, saltos de linea y demas comunes
            }
            if (tx_dat_dorigS2.Text != "" && "'71','72','73','74','75','76','77','78'".Contains(tx_dat_dorigS2.Text))
            {
                // acá se permite todo menos espacios en blanco, saltos de linea y demas comunes
            }
            // Validaciones SUNAT - ruc del emisor del documento origen
            if ("'01','03','12'".Contains(tx_dat_dorigS.Text))
            {
                if ("'01','03'".Contains(tx_dat_motrasS.Text) && tx_rucEorig.Text != tx_numDocRem.Text)
                {
                    MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                            "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                       // RUC emisor doc. relacionado no corresponde por traslado x ventas
            if (tx_dat_dorigS2.Text != "" && "'01','03','12'".Contains(tx_dat_dorigS2.Text))
            {
                if ("'01','03'".Contains(tx_dat_motrasS.Text) && tx_rucEorig2.Text != tx_numDocRem.Text)
                {
                    MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                            "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }         // RUC emisor doc. relacionado no corresponde por traslado x ventas
            if ("09".Contains(tx_dat_dorigS.Text))
            {
                if (tx_numDocRem.Text != tx_rucEorig.Text)
                {
                    MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                            "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                           // RUC emisor doc. relacionado GUIA debe ser igual al ruc del remitente
            if (tx_dat_dorigS2.Text != "" && "09".Contains(tx_dat_dorigS.Text))
            {
                if (tx_numDocRem.Text != tx_rucEorig2.Text)
                {
                    MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                            "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }              // RUC emisor doc. relacionado GUIA debe ser igual al ruc del remitente
            if ("'04','48'".Contains(tx_dat_dorigS.Text))
            {
                if ("02".Contains(tx_dat_motrasS.Text) && tx_numDocRem.Text != tx_rucEorig.Text)
                {
                    MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                            "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                                   // RUC emisor doc. relacionado GUIA debe ser igual al ruc del remitente, motivo compra 
            if ("'04','48'".Contains(tx_dat_dorigS2.Text))
            {
                if ("02".Contains(tx_dat_motrasS.Text) && tx_numDocRem.Text != tx_rucEorig2.Text)
                {
                    MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                            "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }                                   // RUC emisor doc. relacionado GUIA debe ser igual al ruc del remitente, motivo compra 
            if ("'01','03','12'".Contains(tx_dat_dorigS.Text))
            {
                if ("06".Contains(tx_dat_motrasS.Text) && tx_numDocDes.Text != tx_rucEorig.Text)
                {
                    MessageBox.Show("El número del documento del destinatario debe " + Environment.NewLine +
                           "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr.Focus();
                    return;
                }
            }                            // RUC emisor doc. relacionado x DEVOLUcION debe ser igual al DESTINATARIO 
            if ("'01','03','12'".Contains(tx_dat_dorigS2.Text))
            {
                if ("06".Contains(tx_dat_motrasS.Text) && tx_numDocDes.Text != tx_rucEorig2.Text)
                {
                    MessageBox.Show("El número del documento del destinatario debe " + Environment.NewLine +
                           "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_docsOr2.Focus();
                    return;
                }
            }                           // RUC emisor doc. relacionado x DEVOLUcION debe ser igual al DESTINATARIO 
            // Validaciones SUNAT - Datos del Destinatario
            if ("'06','17'".Contains(tx_dat_motrasS.Text) && tx_dat_codsu.Text.Trim() != "6")
            {
                MessageBox.Show("Para el motivo de traslado: 06 y 17 " + Environment.NewLine +
                    "Devolución y Traslado de bienes para transformación" + Environment.NewLine +
                    "el tipo de documento del destinatario debe ser 6-RUC",
                    "Validación Sunat", MessageBoxButtons.OK,MessageBoxIcon.Information);
                cmb_docDes.Focus();
                return;
            }      // Documento de Destinatario debe ser RUC para traslados 06 y 17
            if ("'01','03','05','06','09','14','17'".Contains(tx_dat_motrasS.Text) && 
                tx_numDocDes.Text == tx_numDocRem.Text)
            {
                MessageBox.Show("Para el motivo de traslado seleccionado, el número de " + Environment.NewLine +
                    "documento del destinatario no debe ser igual al del remitente",
                    "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_numDocDes.Focus();
                return;
            }                                           // Destinatario no debe ser igual al remitente.
            if ("'02','04','07'".Contains(tx_dat_motrasS.Text) && 
                tx_numDocDes.Text != tx_numDocRem.Text)
            {
                MessageBox.Show("Para el motivo de traslado seleccionado, el número de " + Environment.NewLine +
                    "documento del destinatario DEBE ser igual al del remitente",
                    "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_numDocDes.Focus();
                return;
            }                                           // Para el motivo de traslado ingresado el Destinatario debe ser igual al remitente.
            // Validaciones SUNAT - Datos del Remitente ... todo ok
            // Validaciones SUNAT - Datos de Envío
            if ("'08','09'".Contains(tx_dat_motrasS.Text) && chk_cunica.Checked == false)
            {
                MessageBox.Show("Para el motivo de traslado importación/exportación, debe" + Environment.NewLine +
                    "estar marcado el check de carga única",
                    "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmb_motras.Focus();
                return;
            }        // Si el motivo de traslado es Importacion o Exportacion y no se trata de un traslado total de la DAM o DS, debe indicar el Peso bruto total de los items seleccionados
            if (!"'08','09'".Contains(tx_dat_motrasS.Text) && chk_cunica.Checked == true)
            {
                MessageBox.Show("Si el motivo de traslado no es importación/exportación," + Environment.NewLine +
                    "NO debe estar marcado el check de carga única",
                    "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmb_motras.Focus();
                return;
            }        // Si no es traslado Importacion o Exportacion no debe estar marcado como carga única 
            // Validaciones SUNAT - Si es modalidad de traslado privado ==> NO VAN DATOS DEL TRANSPORTISTA, xmlGRE maneja este hilo 19/07/2023

            if (tx_dat_tdRem.Text == tx_dat_tDdest.Text && tx_numDocDes.Text == tx_numDocRem.Text)
            {
                // mismo remitente y destinatario, osea es traslado entre establecimientos de la misma empresa
                if (tx_dat_motras.Text != codtrasli)     // codigo mot.traslado establecimientos de la misma empresa 
                {
                    MessageBox.Show("El Remitente y el Destinatario son LOS MISMOS!" + Environment.NewLine +
                        "Este caso solo es válido si el motivo de traslado es:" + Environment.NewLine +
                        "Traslado entre establecimientos de una misma empresa", "Atención, corrija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmb_motras.Focus();
                    return;
                }
            }
            if (tx_dat_tDdest.Text == vtc_dni)
            {
                // destinatario dni, el motivo de traslado debe ser ITINIRANTE -> porque el punto de llegada no tiene anexo local y ruc
                if (tx_dat_motras.Text != codtrasit)     // codigo traslado itinerante
                {
                    MessageBox.Show("El Destinatario tiene DNI" + Environment.NewLine +
                        "En este caso solo es válido el motivo de traslado:" + Environment.NewLine +
                        "     Traslado emisor itinerante     ", "Atención, corrija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmb_motras.Focus();
                    return;
                }
            }
            // datos de planilla de carga, si no hay datos el traslado se hará en un vehículo menor
            if (chk_man.Checked == true)
            {
                if (tx_pla_dniChof.Text.Trim() == "")
                {
                    MessageBox.Show("No existe DNI del chofer!", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tx_fechope.Text != tx_pla_fech.Text)
                {
                    MessageBox.Show("La fecha de la Guía y la del traslado deben ser iguales", "Atención, corrija", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            #endregion
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
                        if (correlativo() == false)                  // corretalivo del local y serie
                        {
                            MessageBox.Show("No tiene configurado su serie","Falta config local",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            return;
                        }

                        if (true)                       // 
                        {
                            if (graba() == true)        // graba en las tablas de la BD
                            {
                                // actualizamos la tabla seguimiento de usuarios
                                string resulta = lib.ult_mov(nomform, nomtab, asd);
                                if (resulta != "OK")
                                {
                                    MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                                if (ipeeg == "SFS")                         // Facturador Sunat - SFS
                                {
                                    if (llenaTablaLiteGRE() == false)       // 22/05/2023
                                    {
                                        MessageBox.Show("No se pudo genar el txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                if (ipeeg == "API_SUNAT")                   // Emisión directa consumiendo los servicios web de sunat api-rest
                                {
                                    if (llenaTablaLiteGRE() != true)
                                    {
                                        MessageBox.Show("No se pudo llenar las tablas sqlite", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        if (_Sunat.sunat_api("09", "adiguiar", c_t, tx_idr.Text, tx_serie.Text, tx_numero.Text, rutaxml, wsPostS) == false)               // sunat_api() == false
                                        {
                                            MessageBox.Show("Documento Guía inválida, debe anularse internamente", "Error: No se pudo generar GRE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                                            {
                                                conn.Open();
                                                if (lib.procConn(conn) == true)
                                                {
                                                    using (MySqlCommand micon = new MySqlCommand("update adiguiar set estadoS='Invalido' where id=@idr"))
                                                    {
                                                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                                        micon.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var bb = MessageBox.Show("Desea imprimir la Guía?" + Environment.NewLine +
                                                "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                            if (bb == DialogResult.Yes)
                                            {
                                                try
                                                {
                                                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                                                    {
                                                        conn.Open();
                                                        if (lib.procConn(conn) == true)
                                                        {
                                                            using (MySqlCommand micon = new MySqlCommand("update cabguiar set impreso='S' where id=@idr"))
                                                            {
                                                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                                                micon.ExecuteNonQuery();
                                                            }
                                                        }
                                                    }
                                                    Bt_print.PerformClick();
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.Message, "Error en proceso de impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

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
                    MessageBox.Show("Ingrese el número de la guía", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                /* if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("La guía esta ANULADA", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Focus();
                    return;
                } */
                if ((tx_pregr_num.Text.Trim() == "") && tx_impreso.Text == "S")
                {
                    // no tiene guía y SI esta impreso => NO se puede modificar y SI anular
                    //sololee();
                    MessageBox.Show("Se modifica observaciones y consignatario", "La Guía esta impresa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //tx_dat_tdRem.Focus();
                    //return;
                }
                if ((tx_pregr_num.Text.Trim() != "") && tx_impreso.Text == "N")
                {
                    // si tiene guía y no esta impreso => NO se puede modificar NO anular
                    sololee();
                    MessageBox.Show("No se puede Modificar", "Tiene guía enlazada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
                if ((tx_pregr_num.Text.Trim() != "") && tx_impreso.Text == "S")
                {
                    // si tiene guía y si esta impreso => NO se puede modificar NO anular
                    sololee();
                    MessageBox.Show("No se puede Modificar", "Tiene guía enlazada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_dat_tdRem.Focus();
                    return;
                }
                if (true)   // (tx_pregr_num.Text.Trim() == "") && tx_impreso.Text == "N"
                {
                    // SI ESTA IMPRESO NO SE PUEDE MODIFICAR, SOLO ANULAR, SALVO LOS COMENTARIOS Y CONSIGNADO
                    // no tiene pre guía y no esta impreso => se puede modificar todo y SI anular
                    // si tiene pre guía y no esta impreso => se modifica parcial y SI anular
                    // si tiene planilla y no esta impreso => NO modifica parcial y NO anular
                    // no tiene planilla y no esta impreso => se modifica parcial y NO anular
                    // si tiene doc.venta y no esta impreso => NO modifica y NO anula
                    // si tiene cobranza y no esta impreso => NO modifica y NO anula
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar la Guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            edita();    // modificacion total
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (v_urege.Contains(asd) == true)
                            {
                                var bb = MessageBox.Show("Desea regenerar el xml?", "Atención", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (bb == DialogResult.Yes)
                                {
                                    if (ipeeg == "SFS")         // Facturador Sunat - SFS
                                    {
                                        if (llenaTablaLiteGRE() == false)         // 22/05/2023
                                        {
                                            MessageBox.Show("No se pudo regenar el txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    if (ipeeg == "API_SUNAT")                   // Emisión directa consumiendo los servicios web de sunat api-rest
                                    {
                                        if (llenaTablaLiteGRE() != true)
                                        {
                                            MessageBox.Show("No se pudo llenar las tablas sqlite", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        else
                                        {
                                            if (_Sunat.sunat_api("09", "adiguiar", c_t, tx_idr.Text, tx_serie.Text, tx_numero.Text, rutaxml, wsPostS) == false)               // sunat_api() == false
                                            {
                                                MessageBox.Show("Documento Guía inválida, debe anularse internamente", "Error: No se pudo generar GRE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                                                {
                                                    conn.Open();
                                                    if (lib.procConn(conn) == true)
                                                    {
                                                        using (MySqlCommand micon = new MySqlCommand("update adiguiar set estadoS='Invalido' where id=@idr"))
                                                        {
                                                            micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                                            micon.ExecuteNonQuery();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
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
                        MessageBox.Show("La Guía ya debe existir para editar", "Debe ser edición", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
            }
            if (modo == "ANULAR")
            {
                if (tx_numero.Text.Trim() == "")
                {
                    tx_numero.Focus();
                    MessageBox.Show("Ingrese el número de la guía", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (true)
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea ANULAR la guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            anula();
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
                if (v_clte_rem == "N" && rb_car_ofi.Checked == true) v_clte_rem = "P";  // N=nombre y direccion | P=solo nombre
                if (v_clte_des == "N" && rb_ent_ofic.Checked == true) v_clte_des = "P";  // N=nombre y direccion | P=solo nombre
                // EL NUMERO DE GUIA SIEMPRE DEBE SER AUTOMÁTICO 
                if (tx_tipcam.Text.Trim() == "") tx_tipcam.Text = "0";
                decimal subtgr = 0; // Math.Round(decimal.Parse(tx_flete.Text) / (decimal.Parse(v_igv) / 100 + 1), 3);
                decimal igvtgr = 0; // Math.Round(decimal.Parse(tx_flete.Text) - subtgr, 3);
                if (tx_dat_mone.Text == MonDeft) tx_fletMN.Text = "0";
                else
                {
                    if (tx_tipcam.Text.Trim() == "" || tx_tipcam.Text == "0")   // tx_fletMN.Text.Trim() == "" || tx_fletMN.Text.Trim() == "0"
                    {
                        MessageBox.Show("Problema con la moneda o tipo de cambio", "No puede continuar");
                        return retorna;
                    }
                    else
                    {
                        tx_fletMN.Text = "0";     // Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                }
                decimal subMN = 0;  // Math.Round(decimal.Parse(tx_fletMN.Text) / (decimal.Parse(v_igv)/100 + 1),3);
                decimal igvMN = 0;  // Math.Round(decimal.Parse(tx_fletMN.Text) - subMN,3);
                string inserta = "insert into cabguiar (" +
                    "fechopegr,serguir,numguir,numpregui,tidodegri,nudodegri,nombdegri,diredegri,ubigdegri," +
                    "tidoregri,nudoregri,nombregri,direregri,ubigregri,locorigen,dirorigen,ubiorigen," +
                    "locdestin,dirdestin,ubidestin,docsremit,obspregri,clifingri,cantotgri,pestotgri," +
                    "tipmongri,tipcamgri,subtotgri,igvgri,totgri,totpag,salgri,estadoser,cantfilas," +
                    "frase1,frase2,fleteimp,tipintrem,tipintdes,tippagpre,seguroE,m1cliente,m2cliente," +
                    "subtotMN,igvMN,totgrMN,codMN,grinumaut,teleregri,teledegri,igvporc," +
                    "idplani,fechplani,serplagri,numplagri,plaplagri,carplagri,autplagri,confvegri,breplagri,proplagri," +
                    "marca_gre,tidocor,rucDorig,lpagop,pesoKT,tidocor2,rucDorig2,docsremit2,marca1,codMTras,desMTras," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fechop,@sergr,@numgr,@npregr,@tdcdes,@ndcdes,@nomdes,@dircde,@ubicde," +
                    "@tdcrem,@ndcrem,@nomrem,@dircre,@ubicre,@locpgr,@dirpgr,@ubopgr," +
                    "@ldcpgr,@didegr,@ubdegr,@dooprg,@obsprg,@conprg,@totcpr,@totppr," +
                    "@monppr,@tcprgr,@subpgr,@igvpgr,@totpgr,@pagpgr,@totpgr,@estpgr,@canfil," +
                    "@frase1,@frase2,@fleimp,@ticlre,@ticlde,@tipacc,@clavse,@m1clte,@m2clte," +
                    "@stMN,@igMN,@tgMN,@codmn,@grinau,@telrem,@teldes,@igvpor," +
                    "@idplan,@fecpla,@serpla,@numpla,@plapla,@carpla,@autpla,@confve,@brepla,@propla," +
                    "@margre,@tdocor,@rucDor,@lpagop,@pesoKT,@tidoc2,@rucDo2,@docsr2,@marCU,@codMTr,@desMTr," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@sergr", tx_serie.Text);
                    micon.Parameters.AddWithValue("@numgr", tx_numero.Text);
                    micon.Parameters.AddWithValue("@npregr", tx_pregr_num.Text);
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
                    micon.Parameters.AddWithValue("@dooprg", tx_docsOr.Text);
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@conprg", tx_consig.Text);
                    micon.Parameters.AddWithValue("@totcpr", tx_totcant.Text);
                    micon.Parameters.AddWithValue("@totppr", tx_totpes.Text);
                    micon.Parameters.AddWithValue("@canfil", tx_tfil.Text);             // cantidad de filas de detalle
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@igvpor", v_igv);                    // igv en porcentaje
                    micon.Parameters.AddWithValue("@tcprgr", tx_tipcam.Text);           // tipo de cambio ... falta leer de la tabla de cambios
                    micon.Parameters.AddWithValue("@subpgr", subtgr.ToString());        // sub total
                    micon.Parameters.AddWithValue("@igvpgr", igvtgr.ToString());        // igv
                    micon.Parameters.AddWithValue("@totpgr", "0");            // total inc. igv
                    micon.Parameters.AddWithValue("@pagpgr", "0");
                    micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text);        // estado de la guía
                    micon.Parameters.AddWithValue("@frase1", (claveSeg == "") ? "" : v_fra1);
                    micon.Parameters.AddWithValue("@frase2", "");
                    micon.Parameters.AddWithValue("@fleimp", "N");
                    micon.Parameters.AddWithValue("@ticlre", tx_dat_tcr.Text);   // tipo de cliente remitente, credito o contado
                    micon.Parameters.AddWithValue("@ticlde", tx_dat_tcd.Text);   // tipo de cliente destinatario, credito o contado
                    micon.Parameters.AddWithValue("@tipacc", "");               // guía a credito o contra entrega
                    micon.Parameters.AddWithValue("@clavse", claveSeg);
                    micon.Parameters.AddWithValue("@m1clte", v_clte_rem);
                    micon.Parameters.AddWithValue("@m2clte", v_clte_des);
                    micon.Parameters.AddWithValue("@stMN", subMN.ToString());
                    micon.Parameters.AddWithValue("@igMN", igvMN.ToString());
                    micon.Parameters.AddWithValue("@tgMN", tx_fletMN.Text);
                    micon.Parameters.AddWithValue("@codmn", MonDeft);           // codigo moneda local es la moneda por defecto 08/11/2020
                    micon.Parameters.AddWithValue("@grinau", tx_n_auto.Text);
                    micon.Parameters.AddWithValue("@telrem", tx_telR.Text);
                    micon.Parameters.AddWithValue("@teldes", tx_telD.Text);
                    micon.Parameters.AddWithValue("@idplan", (tx_idplan.Text.Trim() == "") ? "0" : tx_idplan.Text);
                    if (tx_idplan.Text.Trim() == "") micon.Parameters.AddWithValue("@fecpla", null);
                    else micon.Parameters.AddWithValue("@fecpla", tx_pla_fech.Text.Substring(6, 4) + "-" + tx_pla_fech.Text.Substring(3, 2) + "-" + tx_pla_fech.Text.Substring(0, 2));
                    if (tx_idplan.Text.Trim() == "") micon.Parameters.AddWithValue("@serpla", "");
                    else micon.Parameters.AddWithValue("@serpla", tx_pla_plani.Text.Substring(0, 4));
                    if (tx_idplan.Text.Trim() == "") micon.Parameters.AddWithValue("@numpla", "");
                    else micon.Parameters.AddWithValue("@numpla", tx_pla_plani.Text.Substring(4, 8));
                    micon.Parameters.AddWithValue("@plapla", tx_pla_placa.Text);
                    micon.Parameters.AddWithValue("@carpla", tx_pla_carret.Text);
                    micon.Parameters.AddWithValue("@autpla", tx_pla_autor.Text);
                    micon.Parameters.AddWithValue("@confve", tx_pla_confv.Text);
                    micon.Parameters.AddWithValue("@brepla", tx_pla_brevet.Text);
                    micon.Parameters.AddWithValue("@propla", tx_pla_ruc.Text);
                    // 
                    micon.Parameters.AddWithValue("@margre", v_marGRET);                                    // marca de Guía de Remisión Electrónica
                    micon.Parameters.AddWithValue("@tdocor", tx_dat_docOr.Text);                            // tipo del documento origen
                    micon.Parameters.AddWithValue("@rucDor", tx_rucEorig.Text);                             // ruc del emisor del doc origen
                    micon.Parameters.AddWithValue("@lpagop", "D");                                          // mara de pago en origen o destino
                    micon.Parameters.AddWithValue("@pesoKT", (rb_kg.Checked == true) ? "K" : "T");          // peso en: Kilos o Toneladas
                    micon.Parameters.AddWithValue("@tidoc2", tx_dat_docOr2.Text);
                    micon.Parameters.AddWithValue("@rucDo2", tx_rucEorig2.Text);
                    micon.Parameters.AddWithValue("@docsr2", tx_docsOr2.Text);
                    micon.Parameters.AddWithValue("@marCU", (chk_cunica.Checked == true) ? 1 : 0);          // 0=carga consolidada normal, 1=carga única en el camión
                    micon.Parameters.AddWithValue("@codMTr", tx_dat_motras.Text);
                    micon.Parameters.AddWithValue("@desMTr", tx_motras.Text);
                    //
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
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
                // adicionales
                string actag = "insert into adiguiar (idg,serie,numero) values (@idg,@seg,@nug)";
                using (MySqlCommand micon = new MySqlCommand(actag, conn))
                {
                    micon.Parameters.AddWithValue("@idg", tx_idr.Text);
                    micon.Parameters.AddWithValue("@seg", tx_serie.Text);
                    micon.Parameters.AddWithValue("@nug", tx_numero.Text);
                    micon.ExecuteNonQuery();
                }
                // detalle
                int fila = 1;
                string inserd2 = "update detguiar set " +
                                "cantprodi=@can,unimedpro=@uni,codiprodi=@cod,descprodi=@des,pesoprodi=@pes,precprodi=@preu,totaprodi=@pret " +
                                "where idc=@idr and fila=@fila";
                using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                {
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.Parameters.AddWithValue("@fila", fila);
                    micon.Parameters.AddWithValue("@can", tx_det_cant.Text); // dataGridView1.Rows[i].Cells[0].Value.ToString());
                    micon.Parameters.AddWithValue("@uni", tx_det_umed.Text); // dataGridView1.Rows[i].Cells[1].Value.ToString());
                    micon.Parameters.AddWithValue("@cod", "");
                    micon.Parameters.AddWithValue("@des", gloDeta + " " + tx_det_desc.Text);    // dataGridView1.Rows[i].Cells[2].Value.ToString().Trim());
                    micon.Parameters.AddWithValue("@pes", tx_det_peso.Text);    // dataGridView1.Rows[i].Cells[3].Value.ToString());
                    micon.Parameters.AddWithValue("@preu", "0");
                    micon.Parameters.AddWithValue("@pret", "0");
                    micon.ExecuteNonQuery();
                }
                //
                string actua = "update anagrafiche set Direcc1=@ndir,ubigeo=@ubig,Localidad=@dist,Provincia=@prov,depart=@depa," +
                    "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,nbname=@nbnam " +
                    "where IDCategoria='CLI' AND tipdoc=@tdc1 AND RUC=@ndc1 AND id> 0";
                if (v_clte_rem == "P" && tx_dat_tdRem.Text == vtc_ruc && tx_numDocRem.Text.Substring(0,2) == "20")
                {
                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                    {
                        micon.Parameters.AddWithValue("@tdc1", tx_dat_tdRem.Text);
                        micon.Parameters.AddWithValue("@ndc1", tx_numDocRem.Text);
                        micon.Parameters.AddWithValue("@ndir", rl[2]);
                        micon.Parameters.AddWithValue("@ubig", rl[1]);
                        micon.Parameters.AddWithValue("@dist", rl[5]);
                        micon.Parameters.AddWithValue("@prov", rl[4]);
                        micon.Parameters.AddWithValue("@depa", rl[3]);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                    }
                }
                if (v_clte_des == "P" && tx_dat_tDdest.Text == vtc_ruc && tx_numDocDes.Text.Substring(0,2) == "20")
                {
                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                    {
                        micon.Parameters.AddWithValue("@tdc1", tx_dat_tDdest.Text);
                        micon.Parameters.AddWithValue("@ndc1", tx_numDocDes.Text);
                        micon.Parameters.AddWithValue("@ndir", dl[2]);
                        micon.Parameters.AddWithValue("@ubig", dl[1]);
                        micon.Parameters.AddWithValue("@dist", dl[5]);
                        micon.Parameters.AddWithValue("@prov", dl[4]);
                        micon.Parameters.AddWithValue("@depa", dl[3]);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                    }
                }
                if (v_clte_des == "N" && rb_ent_clte.Checked == false && tx_dat_tDdest.Text == vtc_ruc && tx_numDocDes.Text.Substring(0, 2) == "20")
                {
                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                    {
                        micon.Parameters.AddWithValue("@tdc1", tx_dat_tDdest.Text);
                        micon.Parameters.AddWithValue("@ndc1", tx_numDocDes.Text);
                        micon.Parameters.AddWithValue("@ndir", dl[2]);
                        micon.Parameters.AddWithValue("@ubig", dl[1]);
                        micon.Parameters.AddWithValue("@dist", dl[5]);
                        micon.Parameters.AddWithValue("@prov", dl[4]);
                        micon.Parameters.AddWithValue("@depa", dl[3]);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                    }
                }
                //
                retorna = true;         // no hubo errores!
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
                    if (true == true)     
                    {
                        // EN GUIAS ELECTRONICAS NO IMPORTA IMPRESO O NO, NO SE EDITA ESTOS VALORES 16/03/2023
                    }
                    if (true)   // tx_impreso.Text == "S"
                    {
                        // EDICION DE CABECERA ... Al 06/01/2021 solo se permite editar observ y consignatario
                        // EDICION DE CABECERA ... al 05/05/2022 se permite editar docs.origen si eres usuario autorizado
                        string actua = "update cabguiar a set " +
                            "a.docsremit=@dooprg,a.docsremit2=@dooprg2,a.tidocor=@tdocor,a.tidocor2=@tdocor2,a.rucDorig=@rucDor,a.rucDorig2=@rucDor2," +
                            "a.obspregri=@obsprg,a.clifingri=@conprg," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@dooprg", tx_docsOr.Text);
                        micon.Parameters.AddWithValue("@dooprg2", tx_docsOr2.Text);
                        micon.Parameters.AddWithValue("@tdocor", tx_dat_docOr.Text);
                        micon.Parameters.AddWithValue("@tdocor2", tx_dat_docOr2.Text);
                        micon.Parameters.AddWithValue("@rucDor", tx_rucEorig.Text);
                        micon.Parameters.AddWithValue("@rucDor2", tx_rucEorig2.Text);
                        micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@conprg", tx_consig.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        micon.Dispose();
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar la guía individual");
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
            // En Guías de remisión electrónicas SI HAY ANULACION INTERNA cuando da error al momento de generar el xml
            // anulaciones normales se hacen DESPUES de haberse hecho en sunat en el portal con clave SOL o en el app emprender 08/03/2023
            string parte = " ";
            var aa = DialogResult.No;
            if (v_uagin.Contains(asd))   // usuario con acceso a anulación interna
            {
                aa = MessageBox.Show("Anulación interna para recuperar el número?" + Environment.NewLine +
                    "Se cambia la serie a ANU", "Atención, confirme por favor",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes) parte = ",a.serguir=@coad,b.serie=@coad ";
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabguiar a left join adiguiar b on b.idg=a.id " +
                        "set a.obspregri=@obsr1,a.estadoser=@estser,a.usera=@asd,a.fecha=now(),a.idplani=0,a.fechplani=NULL," +
                        "a.serplagri='',a.numplagri='',a.plaplagri='',a.carplagri='',a.autplagri='',a.confvegri='',a.breplagri='',a.proplagri=''," +
                        "a.verApp=@veap,a.diriplan4=@dil4,a.diripwan4=@diw4,a.netbname=@nbnp,a.estintreg=@eiar" + parte + 
                        "where a.id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@obsr1", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        if (aa == DialogResult.Yes) micon.Parameters.AddWithValue("@coad", v_sanu);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
                        micon.Parameters.AddWithValue("@eiar", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
                        micon.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion boton_form;

        #region leaves, checks y BotonRadio
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
                tx_numero_Leave(null,null);
                tx_obser1.Enabled = true;
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
                    autoprov("tx_ubigRtt");
                }
                else tx_dptoRtt.Text = "";
            }
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia del remitente
        {
            if(tx_provRtt.Text != "" && tx_dptoRtt.Text.Trim() != "")   //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and nombre='" + tx_provRtt.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = tx_ubigRtt.Text.Trim().Substring(0,2) + row[0].ItemArray[2].ToString();
                    autodist("tx_ubigRtt");
                }
                else tx_provRtt.Text = "";
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)         // distrito del remitente
        {
            if(tx_distRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "" && tx_dptoRtt.Text.Trim() != "") //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and nombre='" + tx_distRtt.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = tx_ubigRtt.Text.Trim().Substring(0,4) + row[row.Length-1].ItemArray[3].ToString(); // lib.retCodubigeo(tx_distRtt.Text.Trim(),"",tx_ubigRtt.Text.Trim());
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
                DataRow[] row = dataUbig.Select("nombre='" + tx_dptoDrio.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigDtt.Text = row[0].ItemArray[1].ToString(); // lib.retCodubigeo(tx_dptoRtt.Text.Trim(),"","");
                    autoprov("tx_ubigDtt");
                }
                else tx_dptoDrio.Text = "";
            }
        }
        private void tx_proDio_Leave(object sender, EventArgs e)      // provincia del destinatario
        {
            if (tx_proDrio.Text.Trim() != "" && tx_dptoDrio.Text.Trim() != "")  //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and nombre='" + tx_proDrio.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigDtt.Text = tx_ubigDtt.Text.Trim().Substring(0, 2) + row[0].ItemArray[2].ToString();
                    autodist("tx_ubigDtt");
                }
                else tx_proDrio.Text = "";
            }
        }
        private void tx_disDrio_Leave(object sender, EventArgs e)      // distrito del destinatario
        {
            if (tx_proDrio.Text.Trim() != "" && tx_dptoDrio.Text.Trim() != "" && tx_disDrio.Text.Trim() != "")
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigDtt.Text.Substring(2, 2) + "' and nombre='" + tx_disDrio.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_ubigDtt.Text = tx_ubigDtt.Text.Trim().Substring(0, 4) + row[row.Length-1].ItemArray[3].ToString(); // lib.retCodubigeo(tx_distRtt.Text.Trim(),"",tx_ubigRtt.Text.Trim());
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
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() != "" && ("NUEVO,EDITAR").Contains(Tx_modo.Text))
            {
                tx_nomRem.Text = "";
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
                tx_telR.ReadOnly = false;
                string encuentra = "no";
                if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
                {
                    v_clte_rem = "";            // variable cliente remitente
                    if (rb_car_clte.Checked == true)
                    {
                        tx_nomRem.Text = "";
                        tx_dirRem.Text = "";
                        tx_dptoRtt.Text = "";
                        tx_provRtt.Text = "";
                        tx_distRtt.Text = "";
                        tx_ubigRtt.Text = "";
                        tx_telR.Text = "";
                    }
                    datosR = lib.datossn("CLI", tx_dat_tdRem.Text.Trim(), tx_numDocRem.Text.Trim());
                    if (datosR[0] != "")   // datos.Length > 0
                    {
                        tx_nomRem.Text = datosR[0];
                        tx_telR.Text = datosR[6];
                        if (rb_car_clte.Checked == true)
                        {
                            tx_dirRem.Text = datosR[1];
                            tx_dptoRtt.Text = datosR[2];
                            tx_provRtt.Text = datosR[3];
                            tx_distRtt.Text = datosR[4];
                            tx_ubigRtt.Text = datosR[5];
                        }
                        encuentra = "si";
                        //tx_numDocRem.ReadOnly = true;
                    }
                    if (tx_dat_tdRem.Text == vtc_ruc)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                            {
                                //string[] rl = lib.conectorSolorsoft("RUC", tx_numDocRem.Text);
                                rl = lib.conectorSolorsoft("RUC", tx_numDocRem.Text);
                                tx_nomRem.Text = rl[0];      // razon social
                                if (rb_car_clte.Checked == true)
                                {
                                    tx_ubigRtt.Text = rl[1];     // ubigeo
                                    tx_dirRem.Text = rl[2];      // direccion
                                    tx_dptoRtt.Text = rl[3];      // departamento
                                    tx_provRtt.Text = rl[4];      // provincia
                                    tx_distRtt.Text = rl[5];      // distrito
                                }
                                else
                                {
                                    // debe grabar la direccion en la maestra de clientes rl[]
                                }
                                v_clte_rem = "N";             // marca de cliente nuevo  
                            }
                        }
                    }
                    if (tx_dat_tdRem.Text == vtc_dni)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para dni
                            {
                                rl = lib.conectorSolorsoft("DNI", tx_numDocRem.Text);
                                if (rl[0].Replace("\r\n", "") == NoRetGl)
                                {
                                    MessageBox.Show("No encontramos el DNI en la busqueda inicial, estamos abriendo" + Environment.NewLine +
                                    "una página web para que efectúe la busqueda manualmente", "Redirección a web de DNI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    System.Diagnostics.Process.Start(webdni);
                                    tx_nomRem.Enabled = true;
                                    tx_nomRem.ReadOnly = false;
                                }
                                else
                                {
                                    tx_nomRem.Text = rl[0];      // nombre
                                }
                                v_clte_rem = "N";             // marca de cliente nuevo  
                            }
                        }
                    }
                    if (tx_dat_tdRem.Text != vtc_ruc && tx_dat_tdRem.Text != vtc_dni)
                    {
                        if (encuentra == "no")
                        {
                            v_clte_rem = "N";
                        }
                    }
                    if (tx_nomRem.Text.Trim() == "")
                    {
                        tx_nomRem.ReadOnly = false;
                    }
                    // si la direccion esta en blanco, debe permitir escribir
                    if (tx_dirRem.Text.Trim() == "" || tx_dirRem.Text.Trim().Substring(0,3) == "- -")
                    {
                        tx_dirRem.ReadOnly = false;
                        tx_dptoRtt.ReadOnly = false;
                        tx_provRtt.ReadOnly = false;
                        tx_distRtt.ReadOnly = false;
                        tx_telR.ReadOnly = false;
                        //v_clte_rem = "E";
                    }
                }
            }
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() == "")
            {
                cmb_docRem.Focus();
            }
        }
        private void tx_numDocDes_Leave(object sender, EventArgs e)     // numero documento destinatario
        {
            if (tx_numDocDes.Text.Trim() != "" && tx_mldD.Text.Trim() != "" && ("NUEVO,EDITAR").Contains(Tx_modo.Text))
            {
                tx_nomDrio.Text = "";
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
                tx_telD.ReadOnly = false;
                if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
                {
                    v_clte_des = "";                // variable para marcar si destinatario es nuevo
                    if (rb_ent_clte.Checked == true)
                    {
                        tx_nomDrio.Text = "";
                        tx_dirDrio.Text = "";
                        tx_dptoDrio.Text = "";
                        tx_proDrio.Text = "";
                        tx_disDrio.Text = "";
                        tx_ubigDtt.Text = "";
                        tx_telD.Text = "";
                    }
                    datosD = lib.datossn("CLI", tx_dat_tDdest.Text.Trim(), tx_numDocDes.Text.Trim());
                    if (datosD[0] != "")   // datos.Length > 0
                    {
                        tx_nomDrio.Text = datosD[0];
                        tx_telD.Text = datosD[6];
                        if (rb_ent_clte.Checked == true)
                        {
                            tx_dirDrio.Text = datosD[1];
                            tx_dptoDrio.Text = datosD[2];
                            tx_proDrio.Text = datosD[3];
                            tx_disDrio.Text = datosD[4];
                            tx_ubigDtt.Text = datosD[5];
                            
                        }
                        encuentra = "si";
                        tx_nomDrio.ReadOnly = true;
                    }
                    if (tx_dat_tDdest.Text == vtc_ruc)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                            {
                                //string[] rl = lib.conectorSolorsoft("RUC", tx_numDocDes.Text);
                                dl = lib.conectorSolorsoft("RUC", tx_numDocDes.Text);
                                tx_nomDrio.Text = dl[0];      // razon social
                                if (rb_ent_clte.Checked == true)
                                {
                                    tx_ubigDtt.Text = dl[1];     // ubigeo
                                    tx_dirDrio.Text = dl[2];      // direccion
                                    tx_dptoDrio.Text = dl[3];      // departamento
                                    tx_proDrio.Text = dl[4];      // provincia
                                    tx_disDrio.Text = dl[5];      // distrito
                                    v_clte_des = "N";
                                }
                                else
                                {
                                    if (dl[0] != "")
                                    {
                                        v_clte_des = "N";
                                        // Se va a grabar la direccion de la guia
                                        // luego de insertar el registro se debe actualizar la tabla de clientes con los datos de la direccion fiscal
                                    }
                                }
                            }
                        }
                    }
                    if (tx_dat_tDdest.Text == vtc_dni)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para dni
                            {
                                dl = lib.conectorSolorsoft("DNI", tx_numDocDes.Text);
                                if (dl[0].Replace("\r\n", "") == NoRetGl)
                                {
                                    MessageBox.Show("No encontramos el DNI en la busqueda inicial, estamos abriendo" + Environment.NewLine +
                                    "una página web para que efectúe la busqueda manualmente", "Redirección a web de DNI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    System.Diagnostics.Process.Start(webdni);
                                    tx_nomDrio.Enabled = true;
                                    tx_nomDrio.ReadOnly = false;
                                }
                                else
                                {
                                    tx_nomDrio.Text = dl[0];    // nombre
                                }   
                                v_clte_des = "N";
                            }
                        }
                    }
                    if (tx_dat_tDdest.Text != vtc_ruc && tx_dat_tDdest.Text != vtc_dni)
                    {
                        if (encuentra == "no")
                        {
                            v_clte_des = "N";
                        }
                    }
                    if (tx_nomDrio.Text.Trim() == "")
                    {
                        tx_nomDrio.ReadOnly = false;
                    }
                    // si la direccion esta en blanco debe permitir actualizar
                    if (tx_dirDrio.Text.Trim() == "" || tx_dirDrio.Text.Trim().Substring(0,3) == "- -")   // tx_dirDrio.Text.Trim() == ""
                    {
                        tx_dirDrio.ReadOnly = false;
                        tx_dptoDrio.ReadOnly = false;
                        tx_proDrio.ReadOnly = false;
                        tx_disDrio.ReadOnly = false;
                        tx_telD.ReadOnly = false;
                        //v_clte_des = "E";
                    }
                }
            }
            if (tx_numDocDes.Text.Trim() != "" && tx_mldD.Text.Trim() == "")
            {
                cmb_docDes.Focus();
            }
        }
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_numero.Text.Trim() != "")
            {
                tx_numero.Text = lib.Right("00000000" + tx_numero.Text, 8);
                if (lib.valientabla("cabguiar", "concat(sergui,numgui)", tx_serie.Text + tx_numero.Text) == "1")
                {
                    MessageBox.Show("El número de Guía ya Existe!"," Atención ", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    tx_numero.Text = "";
                    tx_numero.Focus();
                    return;
                }
                cmb_destino.Focus();
                cmb_destino.DroppedDown = true;
            }
            if (Tx_modo.Text != "NUEVO" && tx_numero.Text.Trim() != "")
            {
                // en el caso de las pre guias el numero es el mismo que el ID del registro
                tx_numero.Text = lib.Right("00000000" + tx_numero.Text, 8);
                //tx_idr.Text = tx_numero.Text;
                jalaoc("sernum");
                //dataGridView1.Rows.Clear();
                jaladet(tx_idr.Text);
                sololee();
                if (Tx_modo.Text == "EDITAR")
                {
                    if ((tx_pregr_num.Text.Trim() == "") && tx_impreso.Text == "N")
                    {
                        // no tiene guía y no esta impreso => se puede modificar todo y SI anular
                        tx_obser1.Enabled = true;
                        tx_consig.Enabled = true;
                        //tx_docsOr.Enabled = true;
                    }
                    if ((tx_pregr_num.Text.Trim() == "") && tx_impreso.Text == "S")
                    {
                        // no tiene pre guía y SI esta impreso => NO se puede modificar y SI anular
                        tx_obser1.Enabled = true;
                        tx_consig.Enabled = true;
                        //if (v_uedo.ToUpper().Contains(asd.ToUpper()) == true) tx_docsOr.Enabled = true;
                    }
                    if ((tx_pregr_num.Text.Trim() != "") && tx_impreso.Text == "N")
                    {
                        // si tiene pre guía y no esta impreso => NO se puede modificar NO anular
                        tx_obser1.Enabled = true;
                        tx_consig.Enabled = true;
                        //if (v_uedo.ToUpper().Contains(asd.ToUpper()) == true) tx_docsOr.Enabled = true;
                    }
                    if ((tx_pregr_num.Text.Trim() != "") && tx_impreso.Text == "S")
                    {
                        // si tiene pre guía y si esta impreso => NO se puede modificar NO anular
                        tx_obser1.Enabled = true;
                        tx_consig.Enabled = true;
                        //if (v_uedo.ToUpper().Contains(asd.ToUpper()) == true) tx_docsOr.Enabled = true;
                    }
                }
                if (Tx_modo.Text == "ANULAR") tx_obser1.Enabled = true;
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
        }
        private void tx_pregr_num_Leave(object sender, EventArgs e)     // numero pre guía
        {
            if (Tx_modo.Text == "NUEVO" && tx_pregr_num.Text.Trim() != "" && tx_pregr_num.ReadOnly == false)
            {
                tx_pregr_num.Text = lib.Right("00000000" + tx_pregr_num.Text, 8);
                jalapg(tx_pregr_num.Text);
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("La Pre Guía esta ANULADA", "No puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    initIngreso();
                    tx_pregr_num.Focus();
                }
                else
                {
                    if (tx_numDocRem.Text.Trim() == "")
                    {
                        cmb_docRem.Enabled = true;
                        tx_numDocRem.Enabled = true;
                        tx_dirRem.Enabled = true;
                        tx_dptoRtt.Enabled = true;
                        tx_provRtt.Enabled = true;
                        tx_distRtt.Enabled = true;
                        tx_ubigRtt.Enabled = true;
                    }
                    if (tx_numDocDes.Text.Trim() == "")
                    {
                        cmb_docDes.Enabled = true;
                        tx_numDocDes.Enabled = true;
                        tx_dirDrio.Enabled = true;
                        tx_dptoDrio.Enabled = true;
                        tx_proDrio.Enabled = true;
                        tx_disDrio.Enabled = true;
                        tx_ubigDtt.Enabled = true;
                    }
                    tx_docsOr.Enabled = true;
                    tx_consig.Enabled = true;
                    tx_obser1.Enabled = true;
                }
            }
        }
        private void rb_ent_ofic_Click(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && rb_ent_ofic.Checked == true)
            {
                if (tx_dat_locdes.Text != "")
                {
                    // idcodice,descrizionerid,ubidir,marca1,marca2,deta1,deta2,deta3,deta4
                    DataRow[] fila = dtd.Select("idcodice='" + tx_dat_locdes.Text + "'");
                    tx_ubigDtt.Text = fila[0][2].ToString();
                    tx_dirDrio.Text = fila[0][5].ToString();
                    tx_dptoDrio.Text = fila[0][6].ToString();
                    tx_proDrio.Text = fila[0][7].ToString();
                    tx_disDrio.Text = fila[0][8].ToString();
                    tx_ubigDtt.ReadOnly = true;
                    tx_dirDrio.ReadOnly = true;
                    tx_dptoDrio.ReadOnly = true;
                    tx_proDrio.ReadOnly = true;
                    tx_disDrio.ReadOnly = true;
                }
            }
        }
        private void rb_ent_clte_Click(object sender, EventArgs e)
        {
            if (("NUEVO,EDITAR").Contains(Tx_modo.Text) && rb_ent_clte.Checked == true)
            {
                tx_ubigDtt.Text = "";
                tx_dirDrio.Text = "";
                tx_dptoDrio.Text = "";
                tx_proDrio.Text = "";
                tx_disDrio.Text = "";
                tx_dirDrio.ReadOnly = false;
                tx_dptoDrio.ReadOnly = false;
                tx_proDrio.ReadOnly = false;
                tx_disDrio.ReadOnly = false;
                if (datosD[0] != "")
                {
                    tx_dirDrio.Text = datosD[1];
                    tx_dptoDrio.Text = datosD[2];
                    tx_proDrio.Text = datosD[3];
                    tx_disDrio.Text = datosD[4];
                }
            }
        }
        private void rb_car_ofi_Click(object sender, EventArgs e)
        {
            if (tx_dat_locori.Text != "" && Tx_modo.Text == "NUEVO")    // el origen y su direccion solo se ponen en modo NUEVO
            {
                DataRow[] fila = dtu.Select("idcodice='" + tx_dat_locori.Text + "'");
                tx_ubigRtt.Text = fila[0][2].ToString();
                tx_dirRem.Text = fila[0][5].ToString();
                tx_dptoRtt.Text = fila[0][6].ToString();
                tx_provRtt.Text = fila[0][7].ToString();
                tx_distRtt.Text = fila[0][8].ToString();
                tx_ubigRtt.ReadOnly = true;
                tx_dirRem.ReadOnly = true;
                tx_dptoRtt.ReadOnly = true;
                tx_provRtt.ReadOnly = true;
                tx_distRtt.ReadOnly = true;
            }
        }
        private void rb_car_clte_Click(object sender, EventArgs e)
        {
            if (("NUEVO,EDITAR").Contains(Tx_modo.Text))    // la direccion de origen si puede cambiar en EDICION   
            {
                //tx_ubigO.Text = "";
                tx_dirRem.Text = "";
                tx_dptoRtt.Text = "";
                tx_provRtt.Text = "";
                tx_distRtt.Text = "";
                tx_ubigRtt.Text = "";
                //tx_ubigO.ReadOnly = false;
                tx_dirRem.ReadOnly = false;
                tx_dptoRtt.ReadOnly = false;
                tx_provRtt.ReadOnly = false;
                tx_distRtt.ReadOnly = false;
                tx_ubigRtt.ReadOnly = false;
                if (rb_car_clte.Checked == true && datosR[0] != "")
                {
                    tx_dirRem.Text = datosR[1];
                    tx_dptoRtt.Text = datosR[2];
                    tx_provRtt.Text = datosR[3];
                    tx_distRtt.Text = datosR[4];
                    tx_ubigRtt.Text = datosR[5];
                }
            }
        }
        private void tx_docsOr_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_docsOr);
        }
        private void tx_dirDrio_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_dirDrio);
        }
        private void tx_dirRem_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_dirRem);
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
        private void tx_docsOr_Enter(object sender, EventArgs e)
        {
            tx_docsOr.DeselectAll();
            tx_docsOr.SelectionStart = tx_docsOr.Text.Length;
            tx_docsOr.SelectionLength = 0;
        }
        private void tx_det_cant_Leave(object sender, EventArgs e)
        {
            if ((Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR") && tx_det_cant.Text.Trim() != "")
            {
                tx_totcant.Text = tx_det_cant.Text;
            }
        }
        private void tx_det_peso_Leave(object sender, EventArgs e)
        {
            if ((Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR") && tx_det_peso.Text.Trim() != "")
            {
                tx_totpes.Text = tx_det_peso.Text;
            }
        }
        private void tx_det_desc_Leave(object sender, EventArgs e)
        {
            if ((Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR") && tx_det_desc.Text.Trim() != "")
            {
                tx_tfil.Text = "1";
            }
        }
        private void chk_man_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_man.Checked == true && Tx_modo.Text == "NUEVO" && tx_pla_plani.Text.Trim() != "")
            {
                MessageBox.Show("La guía de remisión remitente se transportará en" + Environment.NewLine + 
                    "un vehículo menor L o M1 sin datos de placa y chofer","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
                // si va sin manifiesto significa que el transporte se hara con vehículos L1 o 
                tx_pla_fech.Text = "";
                tx_pla_plani.Text = "";
                tx_pla_placa.Text = "";
                tx_pla_carret.Text = "";
                tx_marcamion.Text = "";
                tx_pla_autor.Text = "";
                tx_aut_carret.Text = "";
                tx_marCarret.Text = "";
                tx_pla_confv.Text = "";
                tx_pla_brevet.Text = "";
                tx_pla_nomcho.Text = "";
                tx_pla_ruc.Text = "";
                tx_pla_propiet.Text = "";
                tx_idplan.Text = "";
                tx_aut_carret.Text = "";
                // OJO, para volver a manifestar, solo se debe volver a seleccionar el local de destino
                chk_man.Checked = false;
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
            Bt_add.Visible = false;
            Bt_anul.Visible = false;
            Bt_close.Visible = true;
            Bt_edit.Visible = false;
            Bt_print.Visible = false;
            Bt_ver.Visible = false;
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
            if (valiVars() == false)
            {
                Bt_close.PerformClick();
            }
            else
            {
                Tx_modo.Text = "NUEVO";
                button1.Image = Image.FromFile(img_grab);
                panel1.Enabled = true;
                panel2.Enabled = true;

                escribe();
                gbox_serie.Enabled = true;
                tx_pregr_num.Enabled = false;
                tx_pregr_num.ReadOnly = true;
                tx_serie.ReadOnly = true;
                tx_numero.ReadOnly = true;
                initIngreso();  // limpiamos/preparamos todo para el ingreso
                tx_n_auto.Text = "A";   // numeracion automatica

                Bt_ini.Enabled = false;
                Bt_sig.Enabled = false;
                Bt_ret.Enabled = false;
                Bt_fin.Enabled = false;
                tx_numero.Focus();              //cmb_destino.Focus();
                tx_docsOr.Enabled = true;
                tx_docsOr2.Enabled = true;
            }
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            if (valiVars() == false)
            {
                Bt_close.PerformClick();
            }
            else
            {
                escribe();
                panel1.Enabled = true;
                panel2.Enabled = true;
                Tx_modo.Text = "EDITAR";
                button1.Image = Image.FromFile(img_grab);
                initIngreso();
                //if (v_uedo.ToUpper().Contains(asd.ToUpper()) == true) tx_docsOr.Enabled = true;
                tx_obser1.Enabled = true;
                tx_pregr_num.Text = "";
                tx_numero.Text = "";
                tx_numero.ReadOnly = false;
                tx_serie.Focus();
                //
                Bt_ini.Enabled = true;
                Bt_sig.Enabled = true;
                Bt_ret.Enabled = true;
                Bt_fin.Enabled = true;
            }
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
                if (Tx_modo.Text == "NUEVO")
                {
                    MessageBox.Show("No se puede imprimir sin grabar!", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (vi_formato == "A4")     // no existe aún
                {
                    if (imprimeA4() == true) updateprint("S");
                }
                if (vi_formato == "A5")     // formato de imprenta "manual"
                {
                    if (imprimeA5() == true) updateprint("S");
                }
                if (vi_formato == "TK")     // Electrónica
                {
                    if (imprimeTK() == true) updateprint("S");
                }
            }
            // Cantidad de copias
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            if (valiVars() == false)
            {
                Bt_close.PerformClick();
            }
            else
            {
                sololee();
                Tx_modo.Text = "ANULAR";
                button1.Image = Image.FromFile(img_anul);
                initIngreso();
                tx_obser1.Enabled = true;
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
        }
        private void Bt_ver_Click(object sender, EventArgs e)
        {
            if (valiVars() == false)
            {
                Bt_close.PerformClick();
            }
            else
            {
                sololee();
                panel1.Enabled = false;
                panel2.Enabled = false;
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
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            limpia_chk();
            tx_idr.Text = lib.gofirts(nomtab, "marca_gre", v_marGRET);
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
                //tx_idr.Text = aca.ToString();
                tx_idr.Text = lib.goback(nomtab, aca.ToString(), "marca_gre", v_marGRET);
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
            //tx_idr.Text = aca.ToString();
            tx_idr.Text = lib.gonext(nomtab, aca.ToString(), "marca_gre", v_marGRET);
            tx_idr_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            //tx_idr.Text = lib.golast(nomtab);
            tx_idr.Text = lib.golast(nomtab, "marca_gre", v_marGRET);
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
                    tx_dat_csrem.Text = row[3].ToString();
                    tx_dat_nomcsr.Text = row[4].ToString();
                }
            }
        }
        private void cmb_docDes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docDes.SelectedIndex > -1)
            {
                tx_dat_tDdest.Text = cmb_docDes.SelectedValue.ToString();
                DataRow[] fila = dttd1.Select("idcodice='" + tx_dat_tDdest.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mldD.Text = row[2].ToString();
                    tx_dat_codsu.Text = row[3].ToString();
                    tx_dat_nomcsd.Text = row[4].ToString();
                }
            }
        }
        private void cmb_origen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_origen.SelectedIndex > -1)
            {
                tx_dat_locori.Text = cmb_origen.SelectedValue.ToString();
                tx_dirOrigen.Text = lib.dirloca(lib.codloc(asd));
                DataRow[] fila = dtu.Select("idcodice='" + tx_dat_locori.Text + "'");
                tx_ubigO.Text = fila[0][2].ToString();
            }
            // lo de arriba viene del selectedindexhcnaged
            if (tx_dat_locori.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                DataRow[] fila = dtu.Select("idcodice='" + tx_dat_locori.Text + "'");
                if (rb_car_ofi.Checked == true)
                {
                    rb_car_ofi.PerformClick();
                }
                else
                {
                    rb_car_clte.PerformClick();
                }
            }
        }
        private void cmb_destino_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_destino.SelectedIndex > -1)
            {
                tx_dat_locdes.Text = cmb_destino.SelectedValue.ToString();
                tx_dirDestino.Text = lib.dirloca(tx_dat_locdes.Text);
                if (Tx_modo.Text == "NUEVO")
                {
                    // vamos por la serie
                    string consul = "SELECT s.tipdoc,s.serie,s.actual,s.final,s.format,s.glosaser,s.dir_pe,s.ubigeo," +
                        "s.imp_ini,s.imp_fec,s.imp_det,s.imp_dtr,s.imp_pie " +
                        "FROM series s " +
                        "WHERE s.STATUS<> @ean and " +
                        "s.tipdoc = @td AND s.sede = @ori AND s.zona = (SELECT zona FROM desc_loc WHERE idcodice = @des)";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        using (MySqlCommand micon = new MySqlCommand(consul, conn))
                        {
                            micon.Parameters.AddWithValue("@ean", codAnul);
                            micon.Parameters.AddWithValue("@td", v_cid);
                            micon.Parameters.AddWithValue("@ori", tx_dat_locori.Text);
                            micon.Parameters.AddWithValue("@des", tx_dat_locdes.Text);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    tx_serie.Text = v_iniGRET + lib.Right(dr.GetString(1),3);
                                    // no se donde pongo el resto
                                    // direccion del pto de emision [tipdoc=preguia][est_anulado][origen][destino]
                                }
                            }
                        }
                        // validamos que exista planilla abierta hacia el mismo destino
                        consul = "SELECT a.id,a.fechope,a.serplacar,a.numplacar,a.platracto,a.placarret,a.autorizac,a.confvehic,a.brevchofe,a.nomchofe,a.brevayuda," +
                            "a.nomayuda,a.rucpropie,b.razonsocial,a.marcaTrac as marca,a.modeloTrac as modelo,a.marcaCarret,a.modelCarret,a.autorCarret,a.confvCarret," +
                            "a.dnichofer,a.dniayudante,a.nregtrackto,a.nregcarreta " +
                            "from cabplacar a left join anag_for b on b.ruc=a.rucpropie and b.tipdoc=@tdruc " +
                            "WHERE a.estadoser = @estab AND a.locorigen = @locor AND a.locdestin = @locde";
                        //                             "left join vehiculos c on c.placa=a.platracto " +
                        using (MySqlCommand micon = new MySqlCommand(consul, conn))
                        {
                            micon.Parameters.AddWithValue("@tdruc", vtc_ruc);
                            micon.Parameters.AddWithValue("@estab", codGene);
                            micon.Parameters.AddWithValue("@locor", tx_dat_locori.Text);
                            micon.Parameters.AddWithValue("@locde", tx_dat_locdes.Text);
                            using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                            {
                                DataTable data = new DataTable();
                                da.Fill(data);
                                if (data.Rows.Count > 0)
                                {
                                    int nfila = 0;
                                    if (data.Rows.Count > 1)
                                    {
                                        /*  MessageBox.Show("Tiene más de una planilla abierta" + Environment.NewLine +
                                            "para el destino seleccionado" + Environment.NewLine +
                                            "El sistema usará la primera planilla", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        */
                                        vplancar manif = new vplancar(data);
                                        var result = manif.ShowDialog();
                                        if (result == DialogResult.Cancel)
                                        {
                                            nfila = manif.ReturnValue1;
                                            // aca seleccionamos la fila que sea de la placa seleccionada
                                        }
                                    }
                                    DataRow row = data.Rows[nfila];
                                    tx_idplan.Text = row["id"].ToString();
                                    tx_pla_fech.Text = row["fechope"].ToString().Substring(0, 10);
                                    tx_pla_plani.Text = row["serplacar"].ToString() + row["numplacar"].ToString();
                                    tx_pla_placa.Text = row["platracto"].ToString();
                                    tx_pla_carret.Text = row["placarret"].ToString();
                                    tx_pla_autor.Text = row["autorizac"].ToString();
                                    tx_dat_plaNreg.Text = row["nregtrackto"].ToString();    // numero reg mtc
                                    tx_pla_confv.Text = row["confvehic"].ToString();
                                    tx_pla_brevet.Text = row["brevchofe"].ToString();
                                    tx_pla_nomcho.Text = row["nomchofe"].ToString();
                                    tx_pla_brev2.Text = row["brevayuda"].ToString();
                                    tx_pla_chofer2.Text = row["nomayuda"].ToString();
                                    tx_marCpropio.Text = "";
                                    if (tx_pla_ruc.Text.Trim() != "" && tx_pla_ruc.Text != Program.ruc) tx_marCpropio.Text = "1";   // Indicador de transporte subcontratado = true
                                    else tx_marCpropio.Text = "0";      // Indicador de transporte subcontratado = false
                                    tx_pla_ruc.Text = row["rucpropie"].ToString();
                                    tx_pla_propiet.Text = row["razonsocial"].ToString();
                                    tx_marcamion.Text = row["marca"].ToString();
                                    tx_aut_carret.Text = row["autorCarret"].ToString();
                                    tx_dat_carrNreg.Text = row["nregcarreta"].ToString();   // num reg. mtc
                                    tx_marCarret.Text = row["marcaCarret"].ToString();
                                    tx_pla_dniChof.Text = (row["dnichofer"].ToString().Trim() == "") ? lib.Right(row["brevchofe"].ToString(), 8) : row["dnichofer"].ToString();
                                    tx_dat_dniC2.Text = (row["dniayudante"].ToString().Trim() == "") ? (row["brevayuda"].ToString().Trim() == "") ? "" : lib.Right(row["brevayuda"].ToString(), 8) : row["dniayudante"].ToString();
                                    //
                                    chk_man.Checked = false;
                                    chk_man.Enabled = true;
                                }
                                else
                                {
                                    MessageBox.Show("No existe planilla de carga abierta" + Environment.NewLine +
                                        "para el destino seleccionado"," Atención ",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                    tx_idplan.Text = "";
                                    tx_pla_fech.Text = "";
                                    tx_pla_plani.Text = "";
                                    tx_pla_placa.Text = "";
                                    tx_pla_carret.Text = "";
                                    tx_marcamion.Text = "";
                                    tx_aut_carret.Text = "";
                                    tx_marCarret.Text = "";
                                    tx_pla_autor.Text = "";
                                    tx_pla_confv.Text = "";
                                    tx_pla_brevet.Text = "";
                                    tx_pla_nomcho.Text = "";
                                    tx_pla_ruc.Text = "";
                                    tx_pla_propiet.Text = "";
                                    tx_dat_plaNreg.Text = "";
                                    tx_pla_brev2.Text = "";
                                    tx_pla_chofer2.Text = "";
                                    tx_marCpropio.Text = "";
                                    tx_dat_carrNreg.Text = "";
                                    tx_pla_dniChof.Text = "";
                                    tx_dat_dniC2.Text = "";
                                    //
                                    chk_man.Checked = false;
                                    chk_man.Enabled = false;
                                }
                            }
                        }
                    }
                    //cmb_docRem.Focus();
                    //cmb_docRem.DroppedDown = true;
                }
                /*
                if (lib.valientabla("cabguiar", "concat(sergui,numgui)", tx_serie.Text + tx_numero.Text) == "1")
                {
                    MessageBox.Show("El número de Guía ya Existe!", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Text = "";
                    tx_numero.Focus();
                    return;
                */
            }
            if (tx_dat_locdes.Text.Trim() != "")
            {
                DataRow[] fila = dtd.Select("idcodice='" + tx_dat_locdes.Text + "'");
                tx_ubigD.Text = fila[0][2].ToString();
            }
            if(Tx_modo.Text == "NUEVO") rb_ent_clte.PerformClick();
        }
        private void cmb_docorig_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docorig.SelectedIndex > -1)
            {
                tx_dat_docOr.Text = cmb_docorig.SelectedValue.ToString();
                if (tx_dat_docOr.Text.Trim() != "")
                {
                    DataRow[] fila = dtdor.Select("idcodice='" + tx_dat_docOr.Text + "'");
                    tx_dat_dorigS.Text = fila[0][8].ToString();     // codsunat
                }
                //
                tx_docsOr.ReadOnly = false;
                tx_rucEorig.ReadOnly = false;
            }
            else
            {
                tx_docsOr.Text = "";
                tx_docsOr.ReadOnly = true;
                tx_rucEorig.Text = "";
                tx_rucEorig.ReadOnly = true;
            }
        }
        private void cmb_docorig2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docorig2.SelectedIndex > -1)
            {
                tx_dat_docOr2.Text = cmb_docorig2.SelectedValue.ToString();
                if (tx_dat_docOr2.Text.Trim() != "")
                {
                    DataRow[] fila = dtdor2.Select("idcodice='" + tx_dat_docOr2.Text + "'");
                    tx_dat_dorigS2.Text = fila[0][8].ToString();     // codsunat
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
                    tx_dat_dorigS2.Text = "";
                    tx_docsOr2.Text = "";
                    tx_docsOr2.ReadOnly = true;
                    tx_rucEorig2.Text = "";
                    tx_rucEorig2.ReadOnly = true;
                }
            }
        }
        private void cmb_motras_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_motras.SelectedIndex > -1)
            {
                tx_dat_motras.Text = cmb_motras.SelectedValue.ToString();
                DataRow[] fila = dtmt.Select("idcodice='" + tx_dat_motras.Text + "'");
                tx_dat_motrasS.Text = fila[0][3].ToString();
            }
            else
            {
                tx_dat_motras.Text = "";
                tx_dat_motrasS.Text = "";
            }
        }
        #endregion comboboxes

        #region datagridview
        // se fue! no hay
        #endregion

        #region impresion
        private bool imprimeA4()
        {
            bool retorna = false;
            try
            {
                printDocument1.PrinterSettings.PrinterName = v_impA5;
                printDocument1.PrinterSettings.Copies = Int16.Parse(vi_copias);
                printDocument1.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible generar el formato e imprimir" + Environment.NewLine +
                    ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                retorna = false;
            }
            return retorna;
        }
        private bool imprimeA5()
        {
            bool retorna = true;
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            try
            {
                string[] vs = {"","","","","","","","","","","","","", "", "", "", "", "", "", "",   // 20
                               "", "", "", "", "", "", "", "", "", ""};    // 10
                string[] vc = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };   // 17
                string[] va = { "", "", "", "", "", "" };       // 6
                string[,] dt = new string[3, 5] { { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" } }; // 5 columnas

                vs[0] = tx_serie.Text;                          // dr.GetString("sergui");
                vs[1] = tx_numero.Text;                         // dr.GetString("numgui")
                vs[2] = tx_fechope.Text;                        // dr.GetString("fechopegr").Substring(0, 10)
                vs[3] = tx_dirOrigen.Text;                      // dr.GetString("dirorigen")
                vs[4] = cmb_docorig.Text;                      // dr.GetString("NomTidor1")
                vs[5] = tx_docsOr.Text;                         // dr.GetString("docsremit")
                vs[6] = tx_rucEorig.Text;                       // dr.GetString("rucDorig")
                vs[7] = cmb_docorig2.Text;                      // dr.GetString("NomTidor2")
                vs[8] = tx_docsOr2.Text;                        // dr.GetString("docsremit2")
                vs[9] = tx_rucEorig2.Text;                      // dr.GetString("rucDorig2")
                vs[10] = cmb_docRem.Text;                       // dr.GetString("NomDocRem")
                vs[11] = tx_numDocRem.Text;                     // dr.GetString("nudoregri")
                vs[12] = tx_nomRem.Text;                        // dr.GetString("nombregri")
                vs[13] = cmb_docDes.Text;                     // dr.GetString("NomDocDes")
                vs[14] = tx_numDocDes.Text;                     // dr.GetString("nudodegri")
                vs[15] = tx_nomDrio.Text;                     // dr.GetString("nombdegri")
                vs[16] = (tx_pla_fech.Text == "") ? "" : tx_pla_fech.Text.Substring(8, 2) + "/" + tx_pla_fech.Text.Substring(5, 2) + "/" + tx_pla_fech.Text.Substring(0, 4);      // dr.GetString("fechplani")
                //vs[16] = tx_pla_fech.Text;
                vs[17] = tx_totpes.Text;                     // dr.GetString("pestotgri")
                vs[18] = (rb_kg.Checked == true) ? "K" : "T";                        // dr.GetString("pesoKT")
                vs[19] = tx_dirRem.Text;                     //  dr.GetString("direregri")
                vs[20] = tx_dptoRtt.Text;                      // dr.GetString("Dpto_Rem")
                vs[21] = tx_provRtt.Text;                      // dr.GetString("Prov_Rem")
                vs[22] = tx_distRtt.Text;                      // dr.GetString("Dist_Rem")
                vs[23] = tx_dirDrio.Text;                     // dr.GetString("diredegri")
                vs[24] = tx_dptoDrio.Text;                      // dr.GetString("Dpto_Des")
                vs[25] = tx_proDrio.Text;                      // dr.GetString("Prov_Des")
                vs[26] = tx_disDrio.Text;                      // dr.GetString("Dist_Des")
                vs[27] = (Tx_modo.Text == "NUEVO") ? asd : tx_digit.Text;   // dr.GetString("userc")
                vs[28] = cmb_origen.Text;                     // dr.GetString("locorigen")
                vs[29] = tx_fticket.Text;
                
                vc[0] = tx_pla_placa.Text;                   // dr.GetString("plaplagri")
                vc[1] = tx_pla_autor.Text;                   // dr.GetString("autplagri")
                vc[2] = (Tx_modo.Text == "NUEVO" && tx_pla_ruc.Text == Program.ruc) ? Program.regmtc : "";      // Num Registro MTC del transportista
                vc[3] = tx_pla_confv.Text;                   // dr.GetString("confvegri")
                vc[4] = tx_pla_carret.Text;                   // Placa carreta
                vc[5] = tx_aut_carret.Text;                   // Autoriz. vehicular
                vc[6] = (Tx_modo.Text == "NUEVO" && tx_pla_ruc.Text == Program.ruc) ? Program.regmtc : "";      // Num Registro MTC de la carreta
                vc[7] = "";                                   // Conf. vehicular de la carreta, ya esta incluido en  tx_pla_confv.Text
                
                vc[8] = tx_pla_dniChof.Text;                   // Choferes - Dni chofer principal
                vc[9] = tx_pla_brevet.Text;                   // Choferes - dr.GetString("breplagri")
                vc[10] = tx_pla_nomcho.Text;                  // Choferes - dr.GetString("chocamcar")
                vc[11] = "";                                  // Choferes - Apellidos (ya esta incluido en tx_pla_nomcho.Text)
                vc[12] = tx_dat_dniC2.Text;                   // Choferes - Dni chofer secundario
                vc[13] = tx_pla_brev2.Text;                   // Choferes - Brevete chofer secundario
                vc[14] = tx_pla_chofer2.Text;                 // Choferes - Nombres
                vc[15] = "";                                  // Choferes - Apellidos (ya esta incluido en el nombre)
                                                              
                va[0] = tx_dat_textoqr.Text;                 // Varios: texto del código QR ->tx_dat_textoqr.Text
                va[1] = "";
                va[2] = despedida;
                va[3] = "";                                  // Varios: segunda linea de despedida
                va[4] = glosa1;
                va[5] = glosa2;

                int y = 0;
                dt[y, 0] = (y + 1).ToString();              // detalle: Num de fila
                dt[y, 1] = tx_det_cant.Text;                // detalle: Cant.
                dt[y, 2] = tx_det_umed.Text;                // detalle: Unidad de medida
                dt[y, 3] = lb_glodeta.Text.Trim() + " " + tx_det_desc.Text;                // detalle: Descripción
                dt[y, 4] = tx_det_peso.Text;                // detalle: peso

                if (Tx_modo.Text == "NUEVO")
                {   // si es nuevo, se imprimen 2 copias
                    impGRE_T impGRE = new impGRE_T(int.Parse(vi_copias), v_impTK, vs, dt, va, vc);
                    /*
                    for (int i = 1; i <= int.Parse(vi_copias); i++)
                    {
                        printDocument1.PrinterSettings.PrinterName = v_impTK;
                        printDocument1.Print();
                    }*/
                }
                else
                {   // si NO es nuevo, se imprime 1 copia
                    //printDocument1.PrinterSettings.PrinterName = v_impTK;
                    //printDocument1.Print();
                    impGRE_T impGRE = new impGRE_T(1, v_impTK, vs, dt, va, vc);
                }
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
            if (vi_formato == "A4")         // san jose del sur
            {
                //imprime_A4(sender, e);
                float pix = 140.0F;     // punto inicial X
                float piy = 30.0F;      // punto inicial Y
                float alfi = 25.0F;     // alto de cada fila
                float alin = 160.0F;    // alto inicial
                float coli = 60.0F;     // columna mas a la izquierda
                float alde = 400.0F;    // alto inicial del detalle
                float alpi = 705.0F;    // alto inicial del pie
                e.PageSettings.Landscape = false;
                imprime_A4(pix, piy, " ", coli, alin, pix, alfi, alde, alpi, e);
            }
            if (vi_formato == "A5")         // altiplano
            {
                //imprime_A5(sender, e);
                float pix = 120.0F;  // punto inicial X
                float piy = 30.0F;  // punto inicial Y
                float alfi = 23.0F;     // alto de cada fila
                float alin = 135.0F;    // alto inicial
                float coli = 90.0F;     // columna mas a la izquierda
                float alde = 320.0F;    // alto inicial del detalle
                float alpi = 480.0F;    // alto inicial del pie
                e.PageSettings.Landscape = false;
                imprime_A5(pix, piy, " ", coli, alin, pix, alfi, alde, alpi, e);
            }
            if (vi_formato == "TK")
            {
               imprime_TK(sender, e);
            }
        }
        private void imprime_A4(float pix, float piy, string cliente, float coli, float alin, float posi, float alfi, float deta, float pie, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // no hay en A4, salvo del pdf del ose o sunat
        }
        private void imprime_TK(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // 07/03/2023
            {
                // DATOS PARA EL TICKET
                string nomclie = Program.cliente;
                string rasclie = Program.cliente;
                string rucclie = Program.ruc;
                string dirclie = Program.dirfisc;
                // TIPOS DE LETRA PARA EL DOCUMENTO FORMATO TICKET
                Font lt_gra = new Font("Arial", 11);                // grande
                Font lt_tit = new Font("Lucida Console", 10);       // mediano
                Font lt_med = new Font("Arial", 9);                // normal textos
                Font lt_peq = new Font("Arial", 8);                 // pequeño
                                                                    //
                float anchTik = 7.8F;                               // ancho del TK en centimetros
                int coli = 5;                                      // columna inicial
                float posi = 20;                                    // posicion x,y inicial
                int alfi = 15;                                      // alto de cada fila
                float ancho = 360.0F;                                // ancho de la impresion
                int copias = 1;                                     // cantidad de copias del ticket

                for (int i = 1; i <= copias; i++)
                {
                    // ************************ código QR *************************** //
                    float lt = 0;
                    PointF puntoF = new PointF(lt, posi);
                    puntoF = new PointF(coli, posi);
                    // imprimimos el NOMBRE Y RUC DEL EMISOR
                    posi = posi + 1;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(rasclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    //lt = (ancho - e.Graphics.MeasureString("RUC: " + rucclie, lt_gra).Width) / 2;
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("RUC: " + rucclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    // imprimimos el titulo del comprobante y el numero
                    string serie = tx_serie.Text;
                    string corre = tx_numero.Text;
                    string titdoc = "Guía de Remisión Electrónica Remitente";
                    posi = posi + alfi + 8;
                    //float lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titdoc, lt_gra).Width) / 2;
                    lt = (ancho - e.Graphics.MeasureString(titdoc, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titdoc, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + 8;
                    string titnum = "Nro. " + serie + " - " + corre;
                    //lt = (lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    lt = (ancho - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titnum, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);

                    if (tx_dat_textoqr.Text != "")
                    {
                        string codigo = tx_dat_textoqr.Text;
                        var rnd = Path.GetRandomFileName();
                        otro = Path.GetFileNameWithoutExtension(rnd);
                        otro = otro + ".png";
                        //
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        var qrCode = qrEncoder.Encode(codigo);
                        var renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        using (var stream = new FileStream(otro, FileMode.Create))
                            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                        Bitmap png = new Bitmap(otro);
                        posi = posi + alfi + 7;
                        lt = (lib.CentimeterToPixel(anchTik) - lib.CentimeterToPixel(3)) / 2 + 20;
                        puntoF = new PointF(lt, posi);
                        SizeF cuadro = new SizeF(lib.CentimeterToPixel(3), lib.CentimeterToPixel(3));    // 5x5 cm
                        RectangleF rec = new RectangleF(puntoF, cuadro);
                        e.Graphics.DrawImage(png, rec);
                        png.Dispose();
                    }
                    
                    posi = posi + alfi * 7;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dom.Fiscal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    SizeF cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (20), alfi * 2);
                    RectangleF recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(dirclie, lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Sucursal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_dirOrigen.Text, lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);

                    // imprimimos los datos de emisión
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos de Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("F. Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(tx_fechope.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Hora Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString(), lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);

                    // imprimimos los documentos relacionados
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Documentos relacionados", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Tipo de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cmb_docorig.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Nro. de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(tx_docsOr.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Ruc del emisor", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(tx_rucEorig.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    if (tx_dat_docOr2.Text != "")
                    {
                        posi = posi + alfi;
                        puntoF = new PointF(coli + 20, posi);
                        e.Graphics.DrawString("Tipo de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 135, posi);
                        e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 140, posi);
                        e.Graphics.DrawString(cmb_docorig2.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi;
                        puntoF = new PointF(coli + 20, posi);
                        e.Graphics.DrawString("Nro. de documento", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 135, posi);
                        e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 140, posi);
                        e.Graphics.DrawString(tx_docsOr2.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi;
                        puntoF = new PointF(coli + 20, posi);
                        e.Graphics.DrawString("Ruc del emisor", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 135, posi);
                        e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 140, posi);
                        e.Graphics.DrawString(tx_rucEorig2.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    }
                    // imprimimos los datos de envio
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos del Envío", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Remitente", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cmb_docRem.Text + " " + tx_numDocRem.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(tx_nomRem.Text.Trim(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Destinatario", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    e.Graphics.DrawString(cmb_docDes.Text + " " + tx_numDocDes.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(tx_nomDrio.Text.Trim(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Fecha de Traslado", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_pla_fech.Text != "") e.Graphics.DrawString(tx_pla_fech.Text.Substring(6, 4) + "-" + tx_pla_fech.Text.Substring(3, 2) + "-" + tx_pla_fech.Text.Substring(0, 2), 
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Peso Bruto", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_totpes.Text.Trim() != "" && tx_totpes.Text.Trim() != "0") e.Graphics.DrawString(tx_totpes.Text + " " + ((rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text), 
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Dirección de Partida", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_dirRem.Text.Trim() + " " + tx_dptoRtt.Text.Trim() + " " + tx_provRtt.Text.Trim() + " " + tx_distRtt.Text.Trim(),
                        lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Dirección de Llegada", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 20), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_dirDrio.Text.Trim() + " " + tx_dptoDrio.Text.Trim() + " " + tx_proDrio.Text.Trim() + " " + tx_disDrio.Text.Trim(),
                        lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);

                    // imprimimos datos del vehiculo
                    posi = posi + alfi * 3;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos del Vehículo", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Placa", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_pla_placa.Text != "") e.Graphics.DrawString(tx_pla_placa.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Autorización", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_pla_autor.Text != "") e.Graphics.DrawString(tx_pla_autor.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);

                    // imprimimos los datos del chofer
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Datos del Chofer", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Licencia", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 135, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 140, posi);
                    if (tx_pla_brevet.Text != "") e.Graphics.DrawString(tx_pla_brevet.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString("Nombre", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    if (tx_pla_nomcho.Text != "") e.Graphics.DrawString(tx_pla_nomcho.Text, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    // row["numdcho"] = tx_pla_dniChof.Text;                                       // Numero de documento de identidad 

                    // imprimimos los bienes a transportar
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Bienes a transportar", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20, posi);
                    e.Graphics.DrawString(tx_det_peso.Text + " " + ((rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text), 
                        lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    string gDetalle = lb_glodeta.Text + " " + tx_det_desc.Text;
                    double xxx = (e.Graphics.MeasureString(gDetalle, lt_peq).Width / lib.CentimeterToPixel(anchTik)) + 1;
                    cuad = new SizeF(lib.CentimeterToPixel(anchTik) - (coli + 10), alfi * (int)xxx);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(gDetalle, lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    // final del comprobante
                    string repre = "Representación impresa sin valor legal de la";
                    lt = (ancho - e.Graphics.MeasureString(repre, lt_med).Width) / 2;
                    posi = posi + alfi * 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(repre, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    string previo = "Guía de Remisión Electrónica de Remitente";
                    lt = (ancho - e.Graphics.MeasureString(previo, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(previo, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi * 2;
                    string locyus = tx_locuser.Text + " - " + tx_user.Text;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(locyus, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Imp. " + DateTime.Now, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF((lib.CentimeterToPixel(anchTik) - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                    puntoF = new PointF((ancho - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                    e.Graphics.DrawString(despedida, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF(coli, posi);
                    //e.Graphics.DrawString(".", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                }
            }
        }
        private void imprime_A5(float pix, float piy, string cliente, float coli, float alin, float posi, float alfi, float deta, float pie, System.Drawing.Printing.PrintPageEventArgs e)
        {
                // no tenemos formato en A5, solo TK y en A4 de sunat o el ose
        }
        private void updateprint(string sn)  // actualiza el campo impreso de la GR = S
        {   // S=si impreso || N=no impreso
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "update cabguiar set impreso=@sn where id=@idr";
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

        #endregion

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //jalainfo();
        }

    }
    public class Ticket_RptaR                            // respuesta del post envio comprobante
    {
        public string numTicket { get; set; }           // código ticket respuesta
        public DateTime fecRecepcion { get; set; }      // fecha hora de la respuesta
    }
    public class Error 
    {
        public string numError { get; set; }
        public string desError { get; set; }
    }
    public class Rspta_ConsultaR                         // respuesta a la consulta de estado de comprobante
    {
        public string codRespuesta { get; set; }
        public string arcCdr { get; set; }
        public string indCdrGenerado { get; set; }
        //public string error { get; set; }
        public Error error { get; set; }

    }
    public class TokenR                                  // token de acceso de 3600 segundos
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public static class DocumentExtensionsR
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
