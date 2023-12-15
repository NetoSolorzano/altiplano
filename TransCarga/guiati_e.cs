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
using System.Text;
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
    public partial class guiati_e : Form
    {
        static string nomform = "guiati_e";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabguiai";              // cabecera de guias INDIVIDUALES

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
        string v_impMat = "";           // nombre de la impresora matricial
        string v_impA5 = "";            // nombre de la impresora de tinta o laser para las GRE
        string v_impTK = "";            // nombre de la ticketera
        string v_cid = "";              // codigo interno de tipo de documento
        string v_fra1 = "";             // frase de si va o no con clave
        string v_fra2 = "";             // frase 
        string v_sanu = "";             // serie anulacion interna ANU
        string v_CR_gr_ind = "";        // nombre del formato GR individual en CR
        string v_CR_gre_A4 = "";        // nombre del formato CR para las guias electrónicas de carga única
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
        string v_uagin = "";            // usuarios que pueden hacer anulaciones internas
        string webdni = "";             // ruta web del buscador de DNI
        string NoRetGl = "";            // glosa de retorno cuando umasapa no encuentra el dni o ruc
        string wsPostS = "";            // ruta del webswervice de sunat para postear el xml/zip
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
        string det3dtm = "";            // palabra nombre descriptivo de las guias de remision electronicas de transportista
        // uso de pre-guias, tipo de uso registro manual del número, solo para marcar los bultos
        string usoPGm = "";             // en blanco -> no se usa preguias manuales, "manual" -> Si usa pre-guias (orden servicio) manuales solo numero para marcar bultos
        string rutaQR = "";             // ruta donde se grabará el png del QR de la GRE
        string nomImgQR = "";           // nombre del archivo imagen incluyendo la extensión del QR
        string s_d_r = "";              // variable Sin Datos Relacionados 
        double tiempoT = 0;             // Sunat Webservice - contador EN SEGUNDOS de vigencia del token
        string TokenAct = "";           // Sunat Webservice - Token actual vigente
        TimeSpan horaT;                 // Sunat Webservice - Hora de emisión del token
        int plazoT = 0;                 // Sunat Webservice - Cantidad en segundos
        string[] c_t = new string[6] { "", "", "", "", "", ""}; // parametros para generar el token
        //
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string claveSeg = "";                       // clave de seguridad del envío
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string ubiclie = Program.ubidirfis;         // ubigeo direc fiscal
        string asd = Program.vg_user;               // usuario conectado al sistema
        string nRegMTC = Program.regmtc;            // numero registro del MTC
        #endregion

        acGRE_sunat _Sunat = new acGRE_sunat();
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases

        AutoCompleteStringCollection departamentos = new AutoCompleteStringCollection();// autocompletado departamentos
        AutoCompleteStringCollection provincias = new AutoCompleteStringCollection();   // autocompletado provincias
        AutoCompleteStringCollection distritos = new AutoCompleteStringCollection();    // autocompletado distritos
        AutoCompleteStringCollection desdet = new AutoCompleteStringCollection();       // autompletatado descripcion detalle

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public static string CadenaConexion = "Data Source=TransCarga.db";  // Data Source=TransCarga;Mode=Memory;Cache=Shared

        DataTable dttdv = new DataTable();          // tipo documentos guias
        DataTable tcfe = new DataTable();           // GRT electronica - cabecera
        DataTable dttd0 = new DataTable();          // tipo documento del remitente
        string[] datosR = { "" };                   // datos del remitente si existe en la B.D.
        string[] datosD = { "" };                   // datos del destinatario si existe en la B.D.
        string[] rl = { "" };                       // datos del NUEVO remitente
        string[] dl = { "" };                       // datos del NUEVO destinatario
        
        public guiati_e()
        {
            InitializeComponent();
        }
        private void guiati_e_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void guiati_e_Load(object sender, EventArgs e)
        {
            this.Focus();
            jalainfo();
            toolStrip1.BackColor = Color.FromName(colstrp);
            gbox_docvta.BackColor = Color.FromName(colsfon);
            //
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            autodepa();                                     // autocompleta departamentos
            armagret();
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
            tx_det_desc.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_det_desc.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_det_desc.AutoCompleteCustomSource = desdet; //;
            // longitudes maximas de campos
            tx_det_umed.MaxLength = 14;
            tx_det_desc.MaxLength = 95;     // no ampliar porque la descripcion a grabar en la tabla = glosa + tx_det_desc.text
            //
            tx_pregr_num.MaxLength = 8;     // numero de la pre-guia
            tx_serie.MaxLength = 4;         // serie guia
            tx_numero.MaxLength = 8;        // numero guia
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
            tx_docsOr.MaxLength = 20;           // documento origen de la GRT
            tx_docsOr2.MaxLength = 20;
            tx_rucEorig.MaxLength = 11;         // ruc del emisor del documento origen
            tx_rucEorig2.MaxLength = 11;
            tx_numDocRem.MaxLength = 15;        // 8 dni, 11 ruc, 15 maximo otros documentos
            tx_numDocDes.MaxLength = 15;        // 8 dni, 11 ruc, 15 maximo otros documentos
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
            tx_serie.CharacterCasing = CharacterCasing.Upper;
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
            if (usoPGm == "manual")
            {
                tx_pregr_num.Enabled = true;
                tx_pregr_num.ReadOnly = ("NUEVO".Contains(Tx_modo.Text)) ? false : true;
                tx_pregr_num.Text = "";
                bt_preg.Tag = "";
            }
            else
            {
                bt_preg.Tag = "Crealo";     // En el caso de Altiplano, al ser recontra automáticas todas las GRE-T se crean con su pre-guia
            }
            lb_glodeta.Text = gloDeta;
            tx_flete.Text = "";
            tx_pagado.Text = "";
            tx_salxcob.Text = "";
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
            cmb_mon.SelectedValue = tx_dat_mone.Text;
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_digit.Text = v_nbu;
            tx_dat_estad.Text = codGene;
            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
            chk_man.Checked = false;        // checked=false ==> si se manifiesta, checked=true NO se manifiesta
            chk_man.Enabled = false;        // solo se habilita en modo NUEVO y cuando el destino de la GR tiene manifiesto
            rb_kg.Checked = true;
            cmb_docorig2.Enabled = false;   // solo se permite por defecto un solo documento origen relacionado
            // solo 1 excepto hasta 2 si el primero tiene el código "31", "65", "66", "67", "68", "69", o "09"  ... OJO que si es 09 sunat permite muchos mas pero no lo implemente aun.
            if (Tx_modo.Text == "NUEVO") rb_pDes.Checked = true;
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            for (int t = 0; t < Program.dt_enlaces.Rows.Count; t++)
            {
                DataRow row = Program.dt_enlaces.Rows[t];
                if (row["formulario"].ToString() == "main")
                {
                    if (row["campo"].ToString() == "imagenes")     // lite.GetString(1).ToString() == "imagenes"
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
                    if (row["campo"].ToString() == "sunat")
                    {
                        if (row["param"].ToString() == "usa_gre") usa_gre = row["valor"].ToString().Trim();                   // se usa GRE? S/N
                        if (row["param"].ToString() == "client_id") client_id_sunat = row["valor"].ToString().Trim();         // id del api sunat
                        if (row["param"].ToString() == "client_pass") client_pass_sunat = row["valor"].ToString().Trim();     // password del api sunat
                        if (row["param"].ToString() == "user_sol") u_sol_sunat = row["valor"].ToString().Trim();              // usuario sol portal sunat del cliente 
                        if (row["param"].ToString() == "clave_sol") c_sol_sunat = row["valor"].ToString().Trim();             // clave sol portal sunat del cliente 
                        if (row["param"].ToString() == "scope") scope_sunat = row["valor"].ToString().Trim();                 // scope del api sunat
                        if (row["param"].ToString() == "codgre") cGR_sunat = row["valor"].ToString().Trim();                 // codigo sunat para GR transportista
                        //  "true" + " " + "certificado.pfx" + " " + "190969Sorol"
                        if (row["param"].ToString() == "firmDocElec") firmDocElec = row["valor"].ToString().Trim();            // Firma xml, true=firma, false=no firma
                        if (row["param"].ToString() == "rutaCertifc") rutaCertifc = row["valor"].ToString().Trim();            // Ruta y nombre del certificado .pfx
                        if (row["param"].ToString() == "claveCertif") claveCertif = row["valor"].ToString().Trim();            // Clave del certificado
                        if (row["param"].ToString() == "wsPostSunatG") wsPostS = row["valor"].ToString().Trim();               // ruta api sunat para postear
                    }
                    if (row["campo"].ToString() == "rutas")
                    {
                        if (row["param"].ToString() == "grt_txt") rutatxt = row["valor"].ToString().Trim();         // ruta de los txt para las guías elect
                        if (row["param"].ToString() == "web_dni") webdni = row["valor"].ToString().Trim();         // web para busqueda de dni 
                        if (row["param"].ToString() == "grt_xml") rutaxml = row["valor"].ToString().Trim();         // 
                    }
                    if (row["campo"].ToString() == "conector")
                    {
                        if (row["param"].ToString() == "noRetGlosa") NoRetGl = row["valor"].ToString().Trim();          // glosa que retorna umasapa cuando no encuentra dato
                    }
                }
                if (row["formulario"].ToString() == "clients" && row["campo"].ToString() == "documento")
                {
                    if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                    if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                    if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
                }
                if (row["formulario"].ToString() == "facelect")
                {
                    if (row["campo"].ToString() == "factelect")
                    {
                        if (row["param"].ToString() == "caracterNo") caractNo = row["valor"].ToString().Trim();
                    }
                }
                if (row["formulario"].ToString() == "guiati_e")    // guias de remision electrónicas de transportista
                {
                    if (row["campo"].ToString() == "documento")
                    {
                        if (row["param"].ToString() == "c_int") v_cid = row["valor"].ToString().Trim();                 // codigo interno guias de remision
                        if (row["param"].ToString() == "frase1") v_fra1 = row["valor"].ToString().Trim();               // frase para documento anulado
                        if (row["param"].ToString() == "frase2") v_fra2 = row["valor"].ToString().Trim();               // frase de si va con clave la guia
                        if (row["param"].ToString() == "serieAnu") v_sanu = row["valor"].ToString().Trim();             // serie anulacion interna
                        if (row["param"].ToString() == "usediDrem") v_uedo = row["valor"].ToString().Trim();            // usuarios que pueden modificar documentos del remitente
                        if (row["param"].ToString() == "marca") v_marGRET = row["valor"].ToString().Trim();             // marca de guía transportista electrónica
                        if (row["param"].ToString() == "ini_GRET") v_iniGRET = row["valor"].ToString().Trim();          // inicial (sigla) de las GRE-T
                        if (row["param"].ToString() == "UsuRegen") v_urege = row["valor"].ToString().Trim();            // usuarios que pueden regenerar txt
                        if (row["param"].ToString() == "UsuAnuInt") v_uagin = row["valor"].ToString().Trim();           // usuarios que pueden hacer anulaciones internas
                        if (row["param"].ToString() == "usoPGm") usoPGm = row["valor"].ToString().Trim();               // uso de pre-guias manuales para el marcado de bultos = "manual"
                    }
                    if (row["campo"].ToString() == "impresion")
                    {
                        if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "impMatris") v_impMat = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "impA5") v_impA5 = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "nomGRE_cr") v_CR_gr_ind = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "nomGRE_A4") v_CR_gre_A4 = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "rutaQR") rutaQR = row["valor"].ToString().Trim();      // "C:\temp\"
                        if (row["param"].ToString() == "nomImgQR") nomImgQR = row["valor"].ToString().Trim();    // "imgQR.png"
                    }
                    if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                    if (row["campo"].ToString() == "detalle" && row["param"].ToString() == "glosa") gloDeta = row["valor"].ToString().Trim();             // glosa del detalle
                    if (row["campo"].ToString() == "electronico" && row["param"].ToString() == "proveedor") ipeeg = row["valor"].ToString().Trim();      // identificador del emisor electrónico
                    if (row["campo"].ToString() == "glosas")
                    {
                        if (row["param"].ToString() == "glosa1") glosa1 = row["valor"].ToString().Trim();          // glosa final del ticket 1
                        if (row["param"].ToString() == "glosa2") glosa2 = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "nomGRET") det3dtm = row["valor"].ToString().Trim();         // nombre detalle DTM de las GRE-Transportista
                    }
                    if (row["campo"].ToString() == "despedida")
                    {
                        if (row["param"].ToString() == "desped1") despedida = row["valor"].ToString().Trim();          // glosa despedida del ticket 1
                        if (row["param"].ToString() == "desped2") despedid2 = row["valor"].ToString().Trim();
                    }
                }
                if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                {
                    if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
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

            //ipeeg = "API_SUNAT";            // borrame 05/12/2023     
        }
        private void jalaoc(string campo)       // jala guia individual
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
                    parte = "where a.marca_gre=@marGR and a.sergui=@ser and a.numgui=@num";
                }
                if (campo == "preg")
                {
                    parte = "where a.marca_gre=@marGR and a.sergui=@ser and a.numpregui=@num and a.estadoser<>'" + codAnul + "'";  // 18/10/2023 Solo jalamos directamente desde la PG si la guía no esta anulada 
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select a.id,a.fechopegr,a.sergui,a.numgui,a.numpregui,a.tidodegri,a.nudodegri,a.nombdegri,a.diredegri," +
                        "a.ubigdegri,a.tidoregri,a.nudoregri,a.nombregri,a.direregri,a.ubigregri,a.locorigen,a.dirorigen,a.ubiorigen," +
                        "a.locdestin,a.dirdestin,a.ubidestin,a.docsremit,a.obspregri,a.clifingri,a.cantotgri,a.pestotgri," +
                        "a.tipmongri,a.tipcamgri,a.subtotgri,a.igvgri,a.totgri,a.totpag,a.salgri,a.estadoser,a.impreso," +
                        "a.frase1,a.frase2,a.fleteimp,a.tipintrem,a.tipintdes,a.tippagpre,a.seguroE,a.userc,a.userm,a.usera," +
                        "a.serplagri,a.numplagri,a.plaplagri,a.carplagri,a.autplagri,a.confvegri,a.breplagri,a.proplagri," +
                        "ifnull(p.nomchofe,'') as chocamcar,ifnull(p.nregtrackto,'') as nregtrackto,ifnull(p.nregcarreta,'') as nregcarreta," +
                        "ifnull(p.brevayuda,'') as brevayuda,ifnull(p.nomayuda,'') as nomayuda,ifnull(p.dnichofer,'') as dnichofer,ifnull(p.dniayudante,'') as dniayudante," +
                        "ifnull(p.tipdocpri,'') as tipdocpri,ifnull(p.tipdocayu,'') as tipdocayu," +
                        "ifnull(b.fecplacar,'') as fecplacar,ifnull(b.fecdocvta,'') as fecdocvta,ifnull(f.descrizionerid,'') as tipdocvta," +
                        "ifnull(b.serdocvta,'') as serdocvta,ifnull(b.numdocvta,'') as numdocvta,ifnull(b.codmonvta,'') as codmonvta," +
                        "ifnull(b.totdocvta,0) as totdocvta,ifnull(b.codmonpag,'') as codmonpag,ifnull(b.totpagado,0) as totpagado,ifnull(b.saldofina,0) as saldofina," +
                        "ifnull(b.feculpago,'') as feculpago,ifnull(b.estadoser,'') as estadoser,ifnull(c.razonsocial,'') as razonsocial,a.grinumaut," +
                        "ifnull(d.marca,'') as marca,ifnull(d.modelo,'') as modelo,ifnull(r.marca,'') as marCarret,ifnull(r.confve,'') as confvCarret,ifnull(r.autor1,'') as autCarret," +
                        "ifnull(er.numerotel1,'') as telrem,ifnull(ed.numerotel1,'') as teldes,ifnull(t.nombclt,'') as clifact," +
                        "a.marca_gre,a.tidocor,a.rucDorig,a.lpagop,a.pesoKT,a.tidocor2,a.rucDorig2,a.docsremit2,a.marca1," +
                        "ifnull(ad.nticket,'') as nticket,ifnull(ad.estadoS,'') as estadoS, ifnull(ad.cdr,'') as cdr,ifnull(ad.cdrgener,'') as cdrgener," +
                        "ifnull(ad.textoQR,'') as textoQR,ifnull(ad.fticket,'') as fticket,a.idplani,concat(l.deta1,'-',l.deta2,'-',l.deta3,'-',l.deta4) as dirlocdes " +
                        "from cabguiai a " +
                        "left join adiguias ad on ad.idg=a.id " +
                        "left join controlg b on b.serguitra=a.sergui and b.numguitra=a.numgui " +
                        "left join desc_tdv f on f.idcodice=b.tipdocvta " +
                        "left join cabfactu t on t.tipdvta=a.tipdocvta and t.serdvta=a.serdocvta and t.numdvta=a.numdocvta " +
                        "left join anag_for c on c.ruc=a.proplagri and c.tipdoc=@tdep " +
                        "left join vehiculos d on d.placa=a.plaplagri " +
                        "left join vehiculos r on r.placa=a.carplagri " +
                        "left join cabplacar p on p.id=a.idplani " +
                        "left join desc_loc l on l.idcodice=a.locdestin " +
                        "left join anag_cli er on er.ruc=a.nudoregri and er.tipdoc=a.tidoregri " +
                        "left join anag_cli ed on ed.ruc=a.nudodegri and ed.tipdoc=a.tidodegri " + parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@tdep", vtc_ruc);
                    micon.Parameters.AddWithValue("@marGR", v_marGRET);
                    if (campo == "tx_idr") micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    if (campo == "sernum")
                    {
                        micon.Parameters.AddWithValue("@ser", tx_serie.Text);
                        micon.Parameters.AddWithValue("@num", tx_numero.Text);
                    }
                    if (campo == "preg")
                    {
                        micon.Parameters.AddWithValue("@ser", tx_serie.Text);
                        micon.Parameters.AddWithValue("@num", tx_pregr_num.Text);
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
                            tx_serie.Text = dr.GetString("sergui");
                            tx_numero.Text = dr.GetString("numgui");
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
                            tx_flete.Text = dr.GetDecimal("totgri").ToString("#.##");
                            tx_pagado.Text = dr.GetDecimal("totpag").ToString("#.##");
                            tx_salxcob.Text = dr.GetDecimal("salgri").ToString("#.##");
                            tx_totcant.Text = dr.GetString("cantotgri");
                            tx_totpes.Text = dr.GetDecimal("pestotgri").ToString("#.#");
                            tx_impreso.Text = dr.GetString("impreso");
                            tx_pregr_num.Text = dr.GetString("numpregui");
                            claveSeg = dr.GetString("seguroE");
                            chk_flete.Checked = (dr.GetString("fleteimp") == "S") ? true : false;
                            tx_n_auto.Text = dr.GetString("grinumaut");     // numeracion de GR autom o manual
                            //
                            tx_idplan.Text = dr.GetString("idplani");
                            tx_marcamion.Text = dr.GetString("marca");
                            tx_pla_fech.Text = dr.GetString("fecplacar");   //.Substring(0, 10);
                            tx_pla_plani.Text = dr.GetString("serplagri") + dr.GetString("numplagri");
                            tx_pla_placa.Text = dr.GetString("plaplagri");
                            tx_pla_carret.Text = dr.GetString("carplagri");
                            tx_pla_autor.Text = dr.GetString("autplagri");
                            tx_aut_carret.Text = dr.GetString("autCarret");
                            tx_marCarret.Text = dr.GetString("marCarret");
                            tx_pla_confv.Text = dr.GetString("confvegri");
                            tx_pla_ruc.Text = dr.GetString("proplagri");
                            tx_pla_propiet.Text = dr.GetString("razonsocial");
                            tx_marCpropio.Text = (tx_pla_ruc.Text.Trim() != "" && tx_pla_ruc.Text != Program.ruc) ? "1" : "0";   // Indicador de transporte subcontratado = true
                            //
                            tx_pla_brevet.Text = dr.GetString("breplagri");     // brevete del chofer principal
                            tx_pla_nomcho.Text = dr.GetString("chocamcar");     // nombre chofer principal
                            tx_pla_dniChof.Text = dr.GetString("dnichofer");    // num doc chofer principal
                            //tx_pla_chofS.Text = dr.GetString("tipdocpri");      // tipo de doc chofer principal 
                            tx_pla_brev2.Text = dr.GetString("brevayuda");      // brevete del ayudante
                            tx_pla_chofer2.Text = dr.GetString("nomayuda");     // nombre del ayudante
                            tx_dat_dniC2.Text = dr.GetString("dniayudante");   // num doc ayudante
                            //tx_dat_dniC2s.Text = dr.GetString("tipdocayu");      // tipo de doc ayudante 
                            tx_fecDV.Text = dr.GetString("fecdocvta");  //.Substring(0,10);
                            tx_DV.Text = dr.GetString("tipdocvta") + "-" + dr.GetString("serdocvta") + "-" + dr.GetString("numdocvta");
                            tx_clteDV.Text = dr.GetString("clifact");
                            //DataRow[] row = dtm.Select("idcodice='" + dr.GetString("codmonvta") + "'");
                            DataRow[] row = Program.dt_definic.Select("idcodice='" + dr.GetString("codmonvta") + "'");
                            lb_impDV.Text = lb_impDV.Text + ((row.Length > 0)? row[0]["DescrizioneRid"].ToString() : "");
                            tx_impDV.Text = dr.GetDecimal("totdocvta").ToString("#.##");
                            // "a.marca_gre,a.tidocor,a.rucDorig,a.lpagop,a.pesoKT " +
                            tx_dat_docOr.Text = dr.GetString("tidocor");
                            tx_rucEorig.Text = dr.GetString("rucDorig");
                            tx_dat_docOr2.Text = dr.GetString("tidocor2");
                            tx_rucEorig2.Text = dr.GetString("rucDorig2");
                            if (dr.GetString("marca1") == "1") chk_cunica.Checked = true;
                            //
                            if (dr.GetString("pesoKT") == "K") rb_kg.Checked = true;
                            else rb_tn.Checked = true;
                            if (dr.GetString("lpagop") == "O") rb_pOri.Checked = true;
                            else rb_pDes.Checked = true;
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
                            chk_cunica.Checked = (dr.GetString("marca1") == "True") ? true : false;
                            cmb_origen.SelectedValue = tx_dat_locori.Text;
                            cmb_origen_SelectionChangeCommitted(null, null);
                            cmb_destino.SelectedValue = tx_dat_locdes.Text;
                            tx_dirDestino.Text = dr.GetString("dirlocdes");    //cmb_destino_SelectionChangeCommitted(null, null);
                            tx_dat_plaNreg.Text = dr.GetString("nregtrackto");
                            tx_dat_carrNreg.Text = dr.GetString("nregcarreta");
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
                            cmb_mon.SelectedValue = tx_dat_mone.Text;
                            tx_tipcam.Text = dr.GetString("tipcamgri");
                            var xxx = dr.GetString("tipdocpri");
                            if (xxx != "")
                            {
                                DataRow[] rows = Program.dt_definic.Select("idcodice='" + xxx + "'");
                                tx_pla_chofS.Text = rows[0]["codsunat"].ToString();           // tipo de doc chofer principal 
                                if (dr.GetString("tipdocayu").Trim() != "")
                                {
                                    rows = Program.dt_definic.Select("idcodice='" + dr.GetString("tipdocayu") + "'");
                                    tx_dat_dniC2s.Text = rows[0]["codsunat"].ToString();      // tipo de doc ayudante 
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No existe el número buscado!", "Atención - dato incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            tx_pregr_num.Text = "";
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
                    if (Tx_modo.Text != "NUEVO" && (tx_estaSunat.Text == "Enviado" || tx_estaSunat.Text == "En proceso"))    // (tx_estaSunat.Text != "Aceptado" && tx_estaSunat.Text != "Rechazado")
                    {
                        // llamada al metodo que consultará el estado del comprobante y actualizara 
                        if (tx_dat_tickSunat.Text != "") _Sunat.consultaC("adiguias", tx_idr.Text, tx_dat_tickSunat.Text, _Sunat.conex_token_(c_t), tx_serie.Text, tx_numero.Text, rutaxml);
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
        }
        private bool jalapg(string serpre, string numpre, MySqlConnection cnn)      // jala datos de la pre guia
        {
            bool retorna = false;
            //
            serpre = "0" + serpre.Substring(1, 3);
            if (true) // lib.procConn(conn) == true
            {
                string jala = "select a.estadoser,a.locorigen,a.ubiorigen,a.locdestin,a.ubidestin," +
                    "a.tidodepre,a.nudodepre,a.nombdepre,a.diredepre,a.ubigdepre," +
                    "a.tidorepre,a.nudorepre,a.nombrepre,a.direrepre,a.ubigrepre," +
                    "a.obspregui,a.clifinpre,a.tipmonpre,a.seguroE,a.totpregui," +
                    "a.tidocor,a.rucDorig,a.docsremit,a.tidocor2,a.rucDorig2,a.docsremit2 " +
                    "from cabpregr a where a.serpregui=@ser and a.numpregui=@num";
                using (MySqlCommand micon = new MySqlCommand(jala, cnn))
                {
                    micon.Parameters.AddWithValue("@num", numpre);
                    micon.Parameters.AddWithValue("@ser", serpre);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            if (dr.GetString("estadoser") == codAnul)
                            {
                                MessageBox.Show("La Pre Guía esta ANULADA", "No puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                tx_pregr_num.Text = "";
                                initIngreso();
                                tx_pregr_num.Focus();
                                return retorna;
                            }
                            else
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
                                tx_obser1.Text = dr.GetString("obspregui");
                                tx_consig.Text = dr.GetString("clifinpre");
                                tx_dat_mone.Text = dr.GetString("tipmonpre");
                                tx_flete.Text = dr.GetDecimal("totpregui").ToString("#.##");
                                claveSeg = dr.GetString("seguroE");
                                tx_docsOr.Text = dr.GetString("docsremit");
                                tx_dat_docOr.Text = dr.GetString("tidocor");
                                tx_rucEorig.Text = dr.GetString("rucDorig");
                                tx_docsOr2.Text = dr.GetString("docsremit2");
                                tx_dat_docOr2.Text = dr.GetString("tidocor2");
                                tx_rucEorig2.Text = dr.GetString("rucDorig2");
                                dr.Dispose();
                                //
                                string jalad = "select cantprodi,unimedpro,codiprodi,descprodi,round(pesoprodi,1),precprodi,totaprodi " +
                                    "from detpregr where serpregui=@ser and numpregui=@num";
                                using (MySqlCommand midet = new MySqlCommand(jalad, cnn))
                                {
                                    midet.Parameters.AddWithValue("@num", numpre);
                                    midet.Parameters.AddWithValue("@ser", serpre);
                                    MySqlDataReader drdt = midet.ExecuteReader();
                                    while (drdt.Read())
                                    {
                                        tx_det_cant.Text = drdt.GetString(0);
                                        tx_det_desc.Text = drdt.GetString(3);
                                        tx_det_peso.Text = drdt.GetString(4);
                                        tx_det_umed.Text = drdt.GetString(1);
                                    }
                                    drdt.Dispose();
                                    tx_det_cant_Leave(null, null);
                                    tx_det_peso_Leave(null, null);
                                    tx_tfil.Text = "1";
                                }
                                // solo para probar 05/08/2023
                                cmb_docorig.SelectedValue = tx_dat_docOr.Text;
                                cmb_docorig_SelectionChangeCommitted(null, null);
                                cmb_docorig2.SelectedValue = tx_dat_docOr2.Text;
                                cmb_docorig2_SelectionChangeCommitted(null, null);

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
                                cmb_mon.SelectedValue = tx_dat_mone.Text;
                                //
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
                                if (claveSeg == "") chk_seguridad.Enabled = true;
                                else
                                {
                                    chk_seguridad.Checked = true;
                                }
                                tx_docsOr.Enabled = true;
                                tx_consig.Enabled = true;
                                tx_obser1.Enabled = true;

                                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_tdRem.Text + "'");
                                foreach (DataRow row in fila)
                                {
                                    tx_mld.Text = row[2].ToString();
                                }
                                fila = Program.dt_definic.Select("idcodice='" + tx_dat_tDdest.Text + "'");
                                foreach (DataRow row in fila)
                                {
                                    tx_mldD.Text = row[2].ToString();
                                }

                                bt_preg.Tag = "No";     // osea que no cree automáticamente porque si existe la PG y estan enlazadas
                                retorna = true;
                            }
                        }
                        else
                        {

                            dr.Dispose();
                            MessageBox.Show("No se pudo generar la busqueda correctamente", "Error en busqueda de pre-guía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bt_preg.Tag = "";
                            tx_pregr_num.Text = "";
                            tx_pregr_num.Focus();
                            return retorna;
                        }
                    }
                    else
                    {
                        var aa = MessageBox.Show("Confirma que es correcto el número y" + Environment.NewLine +
                            "desea enlazarlo automáticamente?", "No existe la pre-guía ingresada", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            retorna = true;
                            bt_preg.Tag = "Crealo";
                        }
                        else
                        {
                            bt_preg.Tag = "";
                            tx_pregr_num.Text = "";
                            tx_pregr_num.Focus();
                        }
                    }
                }
            }
            //
            return retorna;
        }
        private void jaladet(string idr)        // jala el detalle
        {
            string jalad = "select id,sergui,numgui,cantprodi,unimedpro,codiprodi,REPLACE(descprodi,@glodet,'') AS descprodi,round(pesoprodi,1),precprodi,totaprodi " +
                "from detguiai where idc=@idr";
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
            //  datos para los combos de locales origen y destino
            cmb_origen.Items.Clear();
            cmb_origen.DataSource = Program.dt_definic.Select("idtabella='LOC'").CopyToDataTable(); // dtu;
            cmb_origen.DisplayMember = "descrizionerid";
            cmb_origen.ValueMember = "idcodice";
            //
            cmb_destino.Items.Clear();
            cmb_destino.DataSource = Program.dt_definic.Select("idtabella='LOC'").CopyToDataTable(); // dtd;
            cmb_destino.DisplayMember = "descrizionerid";
            cmb_destino.ValueMember = "idcodice";
            //  datos para los combobox de tipo de documento
            cmb_docRem.Items.Clear();
            cmb_docRem.DataSource = Program.dt_definic.Select("idtabella='DOC'").CopyToDataTable(); // dttd0;
            cmb_docRem.DisplayMember = "descrizionerid";
            cmb_docRem.ValueMember = "idcodice";
            //
            cmb_docDes.DataSource = Program.dt_definic.Select("idtabella='DOC'").CopyToDataTable(); // dttd1;
            cmb_docDes.DisplayMember = "descrizionerid";
            cmb_docDes.ValueMember = "idcodice";
            // tipo de documentos             
            dttdv.Clear();
            dttdv = Program.dt_definic.Select("idtabella='TDV' and codigo=''").CopyToDataTable();
            // datos para el combo de moneda
            cmb_mon.Items.Clear();
            cmb_mon.DataSource = Program.dt_definic.Select("idtabella='MON'").CopyToDataTable(); // dtm;
            cmb_mon.DisplayMember = "descrizionerid";
            cmb_mon.ValueMember = "idcodice";
            // documento origen - documentos relacionados con transporte de mercancias
            cmb_docorig.DataSource = Program.dt_definic.Select("idtabella='DTM' and numero=1 and deta3='" + det3dtm + "' or deta4='" + det3dtm + "'").CopyToDataTable(); // dtdor;
            cmb_docorig.DisplayMember = "descrizione";
            cmb_docorig.ValueMember = "idcodice";
            //
            cmb_docorig2.DataSource = Program.dt_definic.Select("idtabella='DTM' and numero=1 and deta3='" + det3dtm + "' or deta4='" + det3dtm + "'").CopyToDataTable(); // dtdor2;
            cmb_docorig2.DisplayMember = "descrizione";
            cmb_docorig2.ValueMember = "idcodice";
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
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
        private bool correlativo()              // coje y aumenta en 1 el correlativo
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
                        micon.Parameters.AddWithValue("td", "TDV001");
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
        private void analizaEdicion()           // dependiendo de criterios permite la edicion total
        {
            if (Tx_modo.Text == "EDITAR" && tx_estaSunat.Text.Trim() == "Rechazado" && v_urege.ToUpper().Contains(asd.ToUpper()))
            {
                escribe();
                gbox_serie.Enabled = false;
            }
            else
            {
                sololee();
            }
        }

        #region autocompletados
        private void autodepa()                             // departamentos
        {
            if (Program.dt_ubigeos == null)
            {
                MessageBox.Show("Problema de comunicación de datos" + Environment.NewLine +
                    "Debe reiniciar el sistema","Error interno",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                Application.Exit();
                return;
            }
            DataRow[] depar = Program.dt_ubigeos.Select("depart<>'00' and provin='00' and distri='00'");
            departamentos.Clear();
            foreach (DataRow row in depar)
            {
                departamentos.Add(row["nombre"].ToString());
            }
        }
        private void autoprov(string marca)                 // se jala despues de ingresado el departamento
        {
            if (marca != "")
            {
                DataRow[] provi = null;
                if (marca == "tx_ubigO")
                {
                    provi = Program.dt_ubigeos.Select("depart='" + tx_ubigO.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                }
                if (marca == "tx_ubigD")
                {
                    provi = Program.dt_ubigeos.Select("depart='" + tx_ubigD.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                }
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
        }
        private void autodist(string marca)                 // se jala despues de ingresado la provincia
        {
            if (marca != "")
            {
                DataRow[] distr = null;
                if (marca == "tx_ubigO")
                {
                    distr = Program.dt_ubigeos.Select("depart='" + tx_ubigO.Text.Substring(0, 2) + "' and provin='" + tx_ubigO.Text.Substring(2, 2) + "' and distri<>'00'");
                }
                if (marca == "tx_ubigD")
                {
                    distr = Program.dt_ubigeos.Select("depart='" + tx_ubigD.Text.Substring(0, 2) + "' and provin='" + tx_ubigD.Text.Substring(2, 2) + "' and distri<>'00'");
                }
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
            gbox_docvta.Enabled = false;
            //
            cmb_origen.Enabled = false;
        }
        private void limpiar()
        {
            lp.limpiar(this);
            tx_pagado.Text = "";
            tx_fecDV.Text = "";
            tx_DV.Text = "";
            tx_clteDV.Text = "";
            tx_impDV.Text = "";
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

        #region guia electronica Sunat metodo directo y PSNet
        static private void CreaTablaLiteGRE()                  // llamado en el load del form, crea las tablas al iniciar
        {
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                string sqlborra = "DROP TABLE IF EXISTS dt_cabgre; DROP TABLE IF EXISTS dt_detgre";
                using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                {
                    cmdB.ExecuteNonQuery();
                }
                string sqlTabla = "create table dt_cabgre (" +
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
                    "DirLLeDir varchar(200) " +
                    ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // ********************* DETALLE ************************ //
                sqlTabla = "create table dt_detgre (" +
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
                string metela = "insert into dt_cabgre (" +
                    "EmisRuc,EmisNom,EmisUbi,EmisDir,EmisDep,EmisPro,EmisDis,EmisUrb,EmisPai,EmisCor,NumGuia,FecEmis,HorEmis,CodGuia,NomGuia,CantBul,PesoTot,CodUnid,FecIniT,CargaUn," +
                    "DocRelnu1,DocRelti1,DocRelnr1,DocRelcs1,DocRelnm1,DocRelnu2,DocRelti2,DocRelnr2,DocRelcs2,DocRelnm2," +
                    "DstTipdoc,DstNomTdo,DstNumdoc,DstNombre,DstDirecc,DstUbigeo," +
                    "RemTipdoc,RemNomTdo,RemNumdoc,RemNombre,RemDirecc,RemUbigeo," +
                    "PagTipdoc,PagNomTip,PagNumdoc,PagNombre," +
                    "SConTipdo,SConNomTi,SConNumdo,SconNombr," +
                    "EnvPlaca1,EnvAutor1,EnvRegis1,EnvCodEn1,EnvNomEn1,EnvPlaca2,EnvAutor2,EnvRegis2,EnvCodEn2,EnvNomEn2," +
                    "ChoTipDi1,ChoNumDi1,ChoNomTi1,ChoNombr1,ChoApell1,ChoLicen1,ChoTipDi2,ChoNumDi2,ChoNomTi2,ChoNombr2,ChoApell2,ChoLicen2," +
                    "DirParUbi,DirParDir,DirLLeUbi,DirLLeDir) " +
                    "values (" +
                    "@EmisRuc,@EmisNom,@EmisUbi,@EmisDir,@EmisDep,@EmisPro,@EmisDis,@EmisUrb,@EmisPai,@EmisCor,@NumGuia,@FecEmis,@HorEmis,@CodGuia,@NomGuia,@CantBul,@PesoTot,@CodUnid,@FecIniT,@CargaUn," +
                    "@DocRelnu1,@DocRelti1,@DocRelnr1,@DocRelcs1,@DocRelnm1,@DocRelnu2,@DocRelti2,@DocRelnr2,@DocRelcs2,@DocRelnm2," +
                    "@DstTipdoc,@DstNomTdo,@DstNumdoc,@DstNombre,@DstDirecc,@DstUbigeo," +
                    "@RemTipdoc,@RemNomTdo,@RemNumdoc,@RemNombre,@RemDirecc,@RemUbigeo," +
                    "@PagTipdoc,@PagNomTip,@PagNumdoc,@PagNombre," +
                    "@SConTipdo,@SConNomTi,@SConNumdo,@SconNombr," +
                    "@EnvPlaca1,@EnvAutor1,@EnvRegis1,@EnvCodEn1,@EnvNomEn1,@EnvPlaca2,@EnvAutor2,@EnvRegis2,@EnvCodEn2,@EnvNomEn2," +
                    "@ChoTipDi1,@ChoNumDi1,@ChoNomTi1,@ChoNombr1,@ChoApell1,@ChoLicen1,@ChoTipDi2,@ChoNumDi2,@ChoNomTi2,@ChoNombr2,@ChoApell2,@ChoLicen2," +
                    "@DirParUbi,@DirParDir,@DirLLeUbi,@DirLLeDir)";
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
                    cmd.Parameters.AddWithValue("@HorEmis", lib.Right("0" + DateTime.Now.Hour, 2) + ":" + lib.Right("0" + DateTime.Now.Minute, 2) + ":" + lib.Right("0" + DateTime.Now.Second, 2));  // "12:21:13"
                    cmd.Parameters.AddWithValue("@CodGuia", "31");
                    cmd.Parameters.AddWithValue("@NomGuia", "GUIA TRANSPORTISTA");
                    cmd.Parameters.AddWithValue("@CantBul", 1);                           // ??? cantidad de bultos = 1 ????? seguro ????????????
                    cmd.Parameters.AddWithValue("@PesoTot", tx_totpes.Text);              // 30
                    cmd.Parameters.AddWithValue("@CodUnid", (rb_tn.Checked != true) ? "KGM" : "TNE");           // "KGM"
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
                    // datos de quien paga el servicio
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
                    // datos de transportista subcontratado (si lo hubiera)
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
                    cmd.Parameters.AddWithValue("@ChoTipDi1", tx_pla_chofS.Text);                       // codigo sunat del tipo de doc del chofer principal
                    cmd.Parameters.AddWithValue("@ChoNumDi1", tx_pla_dniChof.Text);                     // Num doc del chofer principal
                    cmd.Parameters.AddWithValue("@ChoNomTi1", "Documento de Identidad");                // 
                    cmd.Parameters.AddWithValue("@ChoNombr1", partidor(tx_pla_nomcho.Text, " ")[0]);    // tx_pla_nomcho.Text
                    cmd.Parameters.AddWithValue("@ChoApell1", partidor(tx_pla_nomcho.Text, " ")[1]);
                    cmd.Parameters.AddWithValue("@ChoLicen1", tx_pla_brevet.Text);                      // "U46785663"
                    cmd.Parameters.AddWithValue("@ChoTipDi2", tx_dat_dniC2s.Text);                      // codigo sunat del tipo de doc del chofer
                    cmd.Parameters.AddWithValue("@ChoNumDi2", tx_dat_dniC2.Text);                       // Num doc del chofer secundario
                    cmd.Parameters.AddWithValue("@ChoNomTi2", "Documento de Identidad");                // 
                    cmd.Parameters.AddWithValue("@ChoNombr2", partidor(tx_pla_chofer2.Text, " ")[0]);     // tx_pla_chofer2.Text
                    cmd.Parameters.AddWithValue("@ChoApell2", partidor(tx_pla_chofer2.Text, " ")[1]);
                    cmd.Parameters.AddWithValue("@ChoLicen2", tx_pla_brev2.Text);
                    // datos de direcciones partida y llegada
                    cmd.Parameters.AddWithValue("@DirParUbi", tx_ubigRtt.Text);         //  "150115"
                    cmd.Parameters.AddWithValue("@DirParDir", tx_dirRem.Text);
                    cmd.Parameters.AddWithValue("@DirLLeUbi", tx_ubigDtt.Text);
                    cmd.Parameters.AddWithValue("@DirLLeDir", tx_dirDrio.Text);
                    cmd.ExecuteNonQuery();
                }
                // DETALLE
                metela = "insert into dt_detgre (" +
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
                //string[] parametros = new string[] { rutaxml, Program.ruc, tx_serie.Text + "-" + tx_numero.Text };
                //Console.WriteLine(rutaxml + " " + Program.ruc + " " + tx_serie.Text + "-" + tx_numero.Text + " " + firmDocElec + " " + rutaCertifc + " " + claveCertif + " " + "31");
                ProcessStartInfo p = new ProcessStartInfo();
                p.Arguments = rutaxml + " " + Program.ruc + " " + tx_serie.Text + "-" + tx_numero.Text + " " + firmDocElec + " " + rutaCertifc + " " + claveCertif + " " + "31";
                p.FileName = @rutalocal + "/xmlGRE/xmlGRE.exe";
                var proc = Process.Start(p) ;
                proc.WaitForExit();
                if (proc.ExitCode == 1) retorna = true;
                else retorna = false;
                retorna = true;
            }

            return retorna;
        }
        private string apis()                                   // genera el xml
        {
            string retorna = "NO";
            if (ipeeg == "API_SUNAT")                   // Emisión directa consumiendo los servicios web de sunat api-rest
            {
                if (llenaTablaLiteGRE() != true)
                {
                    MessageBox.Show("No se pudo llenar las tablas sqlite" + Environment.NewLine +
                        "debe anular la guía internamente", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (_Sunat.sunat_api("31", "adiguias", c_t, tx_idr.Text, tx_serie.Text, tx_numero.Text, rutaxml, wsPostS) == false)               // sunat_api() == false
                    {
                        MessageBox.Show("Documento Guía inválida, debe anularse internamente", "Error: No se pudo generar GRE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                        {
                            conn.Open();
                            if (lib.procConn(conn) == true)
                            {
                                using (MySqlCommand micon = new MySqlCommand("update adiguias set estadoS='Invalido' where id=@idr"))
                                {
                                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                    micon.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        retorna = "SI";
                    }
                }
            }
            if (ipeeg == "secure")
            {
                if (psnet_api() == false)               // 22/05/2023
                {
                    MessageBox.Show("No se pudo genar el txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else { retorna = "SI"; }
            }
            return retorna;
        }

        #region psnet
        private void armagret()                         // arma cabecera general para todos los metodos
        {
            tcfe.Clear();
            //  DATOS TRIBUTARIOS DEL DOCUMENTO ELECTRÓNICO
            tcfe.Columns.Add("idsistp");                                    // Id del comprobante en ERP del Cliente
            tcfe.Columns.Add("_tipdoc");                                    // Tipo de Comprobante Electrónico
            tcfe.Columns.Add("_sercor");                                    // Numeración de Comprobante Electrónico
            tcfe.Columns.Add("_fecemi");                                    // fecha de emision   yyyy-mm-dd
            tcfe.Columns.Add("observ1");                                    // observacion del documento
                                                                            /* DATOS DEL EMISOR */
            tcfe.Columns.Add("Prucpro");                                    // Ruc del emisor
            tcfe.Columns.Add("Prazsoc");                                    // razon social del emisor
            tcfe.Columns.Add("Pnomcom");                                    // nombre comercial del emisor
            tcfe.Columns.Add("nregMTC");                                    // Número de Registro MTC
            tcfe.Columns.Add("nautEsp");                                    // Número de autorización especial
            tcfe.Columns.Add("entemEs");                                    // Entidad emisora de la autorización especial  
            tcfe.Columns.Add("paisEmi");                                    // Código de país
            tcfe.Columns.Add("ubigEmi");                                    // UBIGEO DOMICILIO FISCAL
            tcfe.Columns.Add("Pdf_dep");                                    // DOMICILIO FISCAL - departamento
            tcfe.Columns.Add("Pdf_pro");                                    // DOMICILIO FISCAL - provincia
            tcfe.Columns.Add("Pdf_dis");                                    // DOMICILIO FISCAL - distrito
            tcfe.Columns.Add("Pdf_urb");                                    // DOMICILIO FISCAL - Urbanizacion
            tcfe.Columns.Add("Pdf_dir");                                    // DOMICILIO FISCAL - direccion
            tcfe.Columns.Add("Ptelef1");                                    // teléfono del emisor
            tcfe.Columns.Add("Ptelef2");                                    // telef o fax del emisor
            tcfe.Columns.Add("correoE");                                    // correo electronico
            /* DOCUMENTO RELACIONADO */
            tcfe.Columns.Add("ctipdre");                                    // Código del tipo de documento
            tcfe.Columns.Add("ndocrel");                                    // Numero de documento
            tcfe.Columns.Add("rucedre");                                    // Número de RUC del emisor del doc 
            //if (tx_dat_docOr2.Text != "")
            {
                tcfe.Columns.Add("ctipdre2");                               // Código del tipo de documento
                tcfe.Columns.Add("ndocrel2");                               // Numero de documento
                tcfe.Columns.Add("rucedre2");                               // Número de RUC del emisor del doc 
            }
            /* DATOS DE ENVÍO */
            tcfe.Columns.Add("Pcrupro");                                    // Tipo de documento de identidad del remitente
            tcfe.Columns.Add("Cnumdoc");                                    // Numero de documento de identidad del remitente
            tcfe.Columns.Add("Cnomcli");                                    // denominacion o razon social del remitente
            tcfe.Columns.Add("Ctipdoc");                                    // Tipo de documento de identidad del destinatario
            tcfe.Columns.Add("Dnumdoc");                                    // Numero de documento de identidad del destinatario
            tcfe.Columns.Add("Dnomcli");                                    // denominacion o razon social del destinatario
            tcfe.Columns.Add("fectras");                                    // fecha inicio del traslado
            tcfe.Columns.Add("pesotot");                                    // Peso bruto total de los bienes
            tcfe.Columns.Add("unimedp");                                    // Unidad de medida del peso bruto
            tcfe.Columns.Add("trastot");                                    // Indicador de traslado total de bienes 0=falso, 1=verdadero
            tcfe.Columns.Add("indret1");                                    // Indicador de retorno de vehículo con envases o embalajes vacíos 0=falso, 1=verdadero
            tcfe.Columns.Add("indretv");                                    // Indicador de retorno de vehículo vacío 0=falso, 1=verdadero
            tcfe.Columns.Add("indtran");                                    // Indicador de transbordo programado 0=falso, 1=verdadero
            tcfe.Columns.Add("indsubc");                                    // Indicador de transporte subcontratado
            tcfe.Columns.Add("tipdsub");                                    // Tipo de documento de identidad del subcontratador, 6=ruc
            tcfe.Columns.Add("numdocs");                                    // Numero de documento de identidad del subcontratador
            tcfe.Columns.Add("nomsubc");                                    // denominacion o razon social del subcontratador
            tcfe.Columns.Add("indpagf");                                    // Indicador de pagador de flete - 1: Pagador de flete Remitente, 2: Pagador de flete Subcontratador, 3: Pagador de flete Tercero
            tcfe.Columns.Add("tipdocp");                                    // Tipo de documento de identidad de quien paga el servicio
            tcfe.Columns.Add("numdocp");                                    // Numero de documento de identidad de de quien paga el servicio
            tcfe.Columns.Add("nompaga");                                    // denominacion o razon social de quien paga el servicio
            tcfe.Columns.Add("ubigpar");                                    // Ubigeo del punto de partida
            tcfe.Columns.Add("direpar");                                    // Direccion completa y detallada del punto de partida
            tcfe.Columns.Add("gepLong");                                    // Punto de georreferencia del punto de partida, Longitud
            tcfe.Columns.Add("geplati");                                    // Punto de georreferencia del punto de partida, Latitud
            tcfe.Columns.Add("ubilleg");                                    // Ubigeo del punto de llegada
            tcfe.Columns.Add("dirlleg");                                    // Direccion completa y detallada del punto de llegada
            tcfe.Columns.Add("gelLong");                                    // Punto de georreferencia del punto de llegada, Longitud
            tcfe.Columns.Add("gellati");                                    // Punto de georreferencia del punto de llegada, Latitud
            /* DATOS DE VEHICULOS TRACKTO*/
            tcfe.Columns.Add("plaTrac");                                    // Número de placa del vehículo
            tcfe.Columns.Add("ntaruni");                                    // Número de la Tarjeta Única deCirculación
            tcfe.Columns.Add("autcirc");                                    // Número de autorización del vehículo emitido por la entidad
            tcfe.Columns.Add("entauto");                                    // Entidad emisora de la autorización
            /* DATOS DE VEHICULO CARRETA */
            tcfe.Columns.Add("plaCarr");                                    // Número de placa de la carreta
            tcfe.Columns.Add("ntarunC");                                    // Número de la Tarjeta Única deCirculación de la carreta
            tcfe.Columns.Add("aucircC");                                    // Autorización del mtc de la carreta
            /* DATOS DE CONDUCTORES  */
            tcfe.Columns.Add("tipdcho");                                    // Tipo de documento de identidad 
            tcfe.Columns.Add("numdcho");                                    // Numero de documento de identidad 
            tcfe.Columns.Add("nomdcho");                                    // Apellidos y nombres
            tcfe.Columns.Add("bredcho");                                    // Número de licencia de conducir
            tcfe.Columns.Add("tipdch2");                                    // Tipo de documento de identidad, chofer 2
            tcfe.Columns.Add("numdch2");                                    // Numero de documento de identidad , chofer 2
            tcfe.Columns.Add("nomdch2");                                    // Apellidos y nombres, chofer 2
            tcfe.Columns.Add("bredch2");                                    // Número de licencia de conducir, chofer 2
            /* BIENES A TRANSPORTAR */
            tcfe.Columns.Add("nordite");                                    // Numero de orden del item
            tcfe.Columns.Add("umedite");                                    // Unidad de medida del item
            tcfe.Columns.Add("cantite");                                    // Cantidad del item
            tcfe.Columns.Add("descite");                                    // Descripcion detallada del item
            tcfe.Columns.Add("codiite");                                    // Codigo del item
            tcfe.Columns.Add("cosuite");                                    // Código producto SUNAT
            tcfe.Columns.Add("cgtnite");                                    // Código GTIN
            tcfe.Columns.Add("paraite");                                    // Partida arancelaria
            tcfe.Columns.Add("nparite");                                    // Nombre del concepto de la partida arancelaria
            tcfe.Columns.Add("cparite");                                    // Código del concepto de la partida arancelaria
            tcfe.Columns.Add("inbnite");                                    // Indicador de bien normalizado

        }
        private bool psnet_api()                        // metodo PSNet
        {
            bool retorna = false;
            //armagret();
            tcfe.Clear();
            if (arma_GRTE_psnet("alta") != "") retorna = true;
            return retorna;
        }
        private string arma_GRTE_psnet(string accion)   // metodo PSNet
        {
            string retorna = "";

            DataRow[] row = dttdv.Select("idcodice='" + v_cid + "'");             // tipo de documento guia remision transportista
            tipdo = row[0][3].ToString();
            string serie = row[0][4].ToString().Substring(0, 1) + lib.Right(tx_serie.Text, 3);
            string corre = tx_numero.Text;
            DataRow[] rowd = dttd0.Select("idcodice='" + tx_dat_tdRem.Text + "'");          // tipo de documento del remitente
            tipoDocRem = rowd[0][3].ToString().Trim();
            rowd = dttd0.Select("idcodice='" + tx_dat_tDdest.Text + "'");          // tipo de documento destinatario
            tipoDocDes = rowd[0][3].ToString().Trim();

            string ruta = rutatxt;
            string archi = "";
            if (accion == "alta")
            {
                archi = rucclie + "-" + tipdo + "-" + serie + "-" + corre;
                if (datosTXT(tipdo, serie, corre, ruta + archi) == true)
                {
                    if (true)
                    {
                        if (generaTxt(tipdo, serie, corre, ruta + archi) == true)
                        {
                            retorna = tipdo + serie + corre;   // que retorno acá ?
                        }
                    }
                }
            }
            return retorna;
        }
        private bool datosTXT(string tipdo, string serie, string corre, string file_path)    // peru secure net
        {
            bool retorna = false;
            tcfe.Rows.Clear();
            DataRow row = tcfe.NewRow();
            try
            {
                //  DATOS TRIBUTARIOS DEL DOCUMENTO ELECTRÓNICO
                row["idsistp"] = "";                                                        // Id del comprobante en ERP del Cliente
                row["_tipdoc"] = tipdo;                                                     // Tipo de Comprobante Electrónico
                row["_sercor"] = serie + "-" + corre;                                       // Numeración de Comprobante Electrónico
                row["_fecemi"] = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
                row["observ1"] = tx_obser1.Text.Trim();                                      // observacion del documento
                /* DATOS DEL EMISOR */
                row["Prucpro"] = Program.ruc;                                               // Ruc del emisor
                row["Prazsoc"] = nomclie.Trim();                                            // razon social del emisor
                row["Pnomcom"] = "";                                                        // nombre comercial del emisor
                row["nregMTC"] = nRegMTC;                                                   // Número de Registro MTC
                row["nautEsp"] = "";                                                        // Número de autorización especial
                row["entemEs"] = "";                                                        // Entidad emisora de la autorización especial  
                row["paisEmi"] = "PE";                                                      // Código de país
                row["ubigEmi"] = ubiclie;                                                   // UBIGEO DOMICILIO FISCAL
                row["Pdf_dep"] = Program.depfisc.Trim();                                    // DOMICILIO FISCAL - departamento
                row["Pdf_pro"] = Program.provfis.Trim();                                    // DOMICILIO FISCAL - provincia
                row["Pdf_dis"] = Program.distfis.Trim();                                    // DOMICILIO FISCAL - distrito
                row["Pdf_urb"] = "-";                                                       // DOMICILIO FISCAL - Urbanizacion
                row["Pdf_dir"] = Program.dirfisc.Trim();                                    // DOMICILIO FISCAL - direccion
                row["Ptelef1"] = Program.telclte1.Trim();                                   // teléfono del emisor
                row["Ptelef2"] = "";                                                        // telef o fax del emisor
                row["correoE"] = Program.mailclte;                                          // correo electrónico del emisor

                /* DOCUMENTO RELACIONADO */
                row["ctipdre"] = tx_dat_dorigS.Text;                                        // Código del tipo de documento
                row["ndocrel"] = tx_docsOr.Text.Replace(" ", "");                           // Numero de documento
                row["rucedre"] = tx_rucEorig.Text;                                          // Número de RUC del emisor del doc 
                if (tx_dat_docOr2.Text.Trim() != "")
                {
                    row["ctipdre2"] = tx_dat_dorigS2.Text;                                  // Código del tipo de documento
                    row["ndocrel2"] = tx_docsOr2.Text.Replace(" ", "");                     // Numero de documento
                    row["rucedre2"] = tx_rucEorig2.Text;                                    // Número de RUC del emisor del doc 
                }

                /* DATOS DE ENVÍO */
                row["Pcrupro"] = tipoDocRem;                                                // Tipo de documento de identidad del remitente
                row["Cnumdoc"] = tx_numDocRem.Text;                                         // Numero de documento de identidad del remitente
                row["Cnomcli"] = tx_nomRem.Text.Trim();                                     // denominacion o razon social del remitente
                row["Ctipdoc"] = tipoDocDes;                                                // Tipo de documento de identidad del destinatario
                row["Dnumdoc"] = tx_numDocDes.Text;                                         // Numero de documento de identidad del destinatario
                row["Dnomcli"] = tx_nomDrio.Text.Trim();                                    // denominacion o razon social del destinatario
                if (Tx_modo.Text == "NUEVO") row["fectras"] = tx_pla_fech.Text.Substring(6, 4) + "-" + tx_pla_fech.Text.Substring(3, 2) + "-" + tx_pla_fech.Text.Substring(0, 2);
                else row["fectras"] = tx_pla_fech.Text;                                          // fecha inicio del traslado
                row["observ1"] = "";                                                        // Anotación opcional sobre los bienes
                row["pesotot"] = tx_totpes.Text;                                            // Peso bruto total de los bienes
                row["unimedp"] = (rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text;          // Unidad de medida del peso bruto
                row["trastot"] = 1;  // (todas las cargas son completas)                    // Indicador de traslado total de bienes 0=falso, 1=verdadero
                row["indret1"] = 0;  // no se retorna nada de esto                          // Indicador de retorno de vehículo con envases o embalajes vacíos 0=falso, 1=verdadero
                row["indretv"] = 0;  // ningún vehículo retorna vacío                       // Indicador de retorno de vehículo vacío 0=falso, 1=verdadero
                row["indtran"] = 0;  // no tenemos esa modalidad                            // Indicador de transbordo programado 0=falso, 1=verdadero
                row["indsubc"] = tx_marCpropio.Text;                                        // Indicador de trans|porte subcontratado
                row["tipdsub"] = (tx_marCpropio.Text == "1") ? "6" : "";                    // Tipo de documento de identidad del subcontratador, 6=ruc
                row["numdocs"] = (tx_marCpropio.Text == "1") ? Program.ruc : "";            // Numero de documento de identidad del subcontratador
                row["nomsubc"] = (tx_marCpropio.Text == "1") ? Program.cliente : "";        // denominacion o razon social del subcontratador
                row["indpagf"] = (rb_pOri.Checked == true) ? 1 : 3;                         // Indicador de pagador de flete - 1: Pagador de flete Remitente, 2: Pagador de flete Subcontratador, 3: Pagador de flete Tercero
                if (rb_pOri.Checked == true)
                {
                    row["tipdocp"] = tipoDocRem;                                            // Tipo de documento de identidad de quien paga el servicio
                    row["numdocp"] = tx_numDocRem.Text;                                     // Numero de documento de identidad de de quien paga el servicio
                    row["nompaga"] = tx_nomRem.Text.Trim();                                 // denominacion o razon social de quien paga el servicio
                }
                else
                {
                    row["tipdocp"] = tipoDocDes;                                            // Tipo de documento de identidad de quien paga el servicio
                    row["numdocp"] = tx_numDocDes.Text;                                     // Numero de documento de identidad de de quien paga el servicio
                    row["nompaga"] = tx_nomDrio.Text.Trim();                                // denominacion o razon social de quien paga el servicio
                }
                row["ubigpar"] = tx_ubigRtt.Text;                                           // Ubigeo del punto de partida
                row["direpar"] = tx_dirRem.Text.Trim() + " " + tx_dptoRtt.Text.Trim() + " " +
                    tx_provRtt.Text.Trim() + " " + tx_distRtt.Text.Trim();                  // Direccion completa y detallada del punto de partida
                row["gepLong"] = "";                                                        // Punto de georreferencia del punto de partida, Longitud
                row["geplati"] = "";                                                        // Punto de georreferencia del punto de partida, Latitud
                row["ubilleg"] = tx_ubigDtt.Text;                                           // Ubigeo del punto de llegada
                row["dirlleg"] = tx_dirDrio.Text.Trim() + " " + tx_dptoDrio.Text.Trim() + " " +
                    tx_proDrio.Text.Trim() + " " + tx_disDrio.Text.Trim();                  // Direccion completa y detallada del punto de llegada
                row["gelLong"] = "";                                                        // Punto de georreferencia del punto de llegada, Longitud
                row["gellati"] = "";                                                        // Punto de georreferencia del punto de llegada, Latitud
                                                                                            /* DATOS DE VEHICULOS - TRACKTO */
                row["plaTrac"] = tx_pla_placa.Text.Replace("-", "");                        // Número de placa del vehículo
                row["ntaruni"] = tx_pla_autor.Text;                                         // Número de la Tarjeta Única deCirculación
                row["autcirc"] = tx_pla_autor.Text;                                         // Número de autorización del vehículo emitido por la entidad
                row["entauto"] = "06";                                                      // Entidad emisora de la autorización MTC=06
                /* DATOS DE VEHICULOS - CARRETA */
                row["plaCarr"] = tx_pla_carret.Text.Replace("-", "");                       // Número de placa de la carreta
                row["ntarunC"] = tx_aut_carret.Text;                                        // Número de la Tarjeta Única deCirculación de la carreta
                row["aucircC"] = tx_aut_carret.Text;                                        // Número de autorización del vehículo de la carreta
                /* DATOS DE CONDUCTORES  */
                row["tipdcho"] = "1";                                                       // Tipo de documento de identidad 
                row["numdcho"] = tx_pla_dniChof.Text;                                       // Numero de documento de identidad 
                row["nomdcho"] = tx_pla_nomcho.Text;                                        // Apellidos y nombres
                row["bredcho"] = tx_pla_brevet.Text.Replace("-", "");                       // Número de licencia de conducir
                /* BIENES A TRANSPORTAR */
                row["nordite"] = "1";                                                       // Numero de orden del item
                row["umedite"] = (rb_kg.Checked == true) ? rb_kg.Text : rb_tn.Text;         // Unidad de medida del item
                row["cantite"] = tx_det_peso.Text;                                          // Cantidad del item
                row["descite"] = lb_glodeta.Text + " " + tx_det_desc.Text;                  // Descripcion detallada del item
                row["codiite"] = "";                                                        // Codigo del item
                row["cosuite"] = "";                                                        // Código producto SUNAT
                row["cgtnite"] = "";                                                        // Código GTIN
                row["paraite"] = "";                                                        // Partida arancelaria
                row["nparite"] = "";                                                        // Nombre del concepto de la partida arancelaria
                row["cparite"] = "";                                                        // Código del concepto de la partida arancelaria
                row["inbnite"] = "0";                                                       // Indicador de bien normalizado, 0=no, 1=bien normalizado

                retorna = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                retorna = false;
            }
            tcfe.Rows.Add(row);

            return retorna;
        }
        private bool generaTxt(string tipdo, string serie, string corre, string file_path)   // peru secure net
        {
            bool retorna = false;
            DataRow row = tcfe.Rows[0];

            char sep = (char)31;
            StreamWriter writer;
            file_path = file_path + ".txt";
            writer = new StreamWriter(file_path);
            writer.WriteLine("CONTROL" + sep + "31001" + sep + asd + sep);
            writer.WriteLine("ENCABEZADO" + sep +
                row["idsistp"] + sep +                                                  // Id del comprobante en ERP del Cliente
                row["_tipdoc"] + sep +                                                  // Tipo de Comprobante Electrónico
                row["_sercor"] + sep +                                                  // Numeración de Comprobante Electrónico
                row["_fecemi"] + sep +                                                  // fecha de emision   yyyy-mm-dd
                row["observ1"] + sep);                                                  // observacion del documento
            /* DATOS DEL EMISOR */
            writer.WriteLine("ENCABEZADO-EMISOR" + sep +
                row["Prucpro"] + sep +                                                  // Ruc del emisor
                row["Prazsoc"] + sep +                                                  // razon social del emisor
                row["Pnomcom"] + sep +                                                  // nombre comercial del emisor
                row["nregMTC"] + sep +                                                  // Número de Registro MTC
                row["nautEsp"] + sep +                                                  // Número de autorización especial
                row["entemEs"] + sep +                                                  // Entidad emisora de la autorización especial  
                row["paisEmi"] + sep +                                                  // Código de país
                row["ubigEmi"] + sep +                                                  // UBIGEO DOMICILIO FISCAL
                row["Pdf_dep"] + sep +                                                  // DOMICILIO FISCAL - departamento
                row["Pdf_pro"] + sep +                                                  // DOMICILIO FISCAL - provincia
                row["Pdf_dis"] + sep +                                                  // DOMICILIO FISCAL - distrito
                row["Pdf_urb"] + sep +                                                  // DOMICILIO FISCAL - Urbanizacion
                row["Pdf_dir"] + sep +                                                  // DOMICILIO FISCAL - direccion
                row["Ptelef1"] + sep +                                                  // teléfono del emisor
                row["Ptelef2"] + sep +                                                  // telef o fax del emisor
                row["correoE"] + sep);                                                  // correo electrónico del emisor
            /* DOCUMENTO RELACIONADO */
            writer.WriteLine("ENCABEZADO-DOCRELACIONADO" + sep +
                row["ctipdre"] + sep +                                                  // Código del tipo de documento
                row["ndocrel"] + sep +                                                  // Numero de documento
                row["rucedre"] + sep);                                                  // Número de RUC del emisor del doc 
            if (tx_dat_docOr2.Text != "")
            {
                writer.WriteLine("ENCABEZADO-DOCRELACIONADO" + sep +
                    row["ctipdre2"] + sep +                                             // Código del tipo de documento
                    row["ndocrel2"] + sep +                                             // Numero de documento
                    row["rucedre2"] + sep);                                             // Número de RUC del emisor del doc 
            }
            /* DATOS DE ENVÍO */
            writer.WriteLine("ENCABEZADO-DATOSENVIO" + sep +
                row["Pcrupro"] + sep +                                                  // Tipo de documento de identidad del remitente
                row["Cnumdoc"] + sep +                                                  // Numero de documento de identidad del remitente
                row["Cnomcli"] + sep +                                                  // denominacion o razon social del remitente
                row["Ctipdoc"] + sep +                                                  // Tipo de documento de identidad del destinatario
                row["Dnumdoc"] + sep +                                                  // Numero de documento de identidad del destinatario
                row["Dnomcli"] + sep +                                                  // denominacion o razon social del destinatario
                row["fectras"] + sep +                                                  // fecha inicio del traslado
                row["observ1"] + sep +                                                  // Anotación opcional sobre los bienes
                row["pesotot"] + sep +                                                  // Peso bruto total de los bienes
                row["unimedp"] + sep +                                                  // Unidad de medida del peso bruto
                row["trastot"] + sep +                                                  // Indicador de traslado total de bienes 0=falso, 1=verdadero
                row["indret1"] + sep +                                                  // Indicador de retorno de vehículo con envases o embalajes vacíos 0=falso, 1=verdadero
                row["indretv"] + sep +                                                  // Indicador de retorno de vehículo vacío 0=falso, 1=verdadero
                row["indtran"] + sep +                                                  // Indicador de transbordo programado 0=falso, 1=verdadero
                row["indsubc"] + sep +                                                  // Indicador de transporte subcontratado
                row["tipdsub"] + sep +                                                  // Tipo de documento de identidad del subcontratador, 6=ruc
                row["numdocs"] + sep +                                                  // Numero de documento de identidad del subcontratador
                row["nomsubc"] + sep +                                                  // denominacion o razon social del subcontratador
                row["indpagf"] + sep +                                                  // Indicador de pagador de flete - 1: Pagador de flete Remitente, 2: Pagador de flete Subcontratador, 3: Pagador de flete Tercero
                row["tipdocp"] + sep +                                                  // Tipo de documento de identidad de quien paga el servicio
                row["numdocp"] + sep +                                                  // Numero de documento de identidad de de quien paga el servicio
                row["nompaga"] + sep +                                                  // denominacion o razon social de quien paga el servicio
                row["ubigpar"] + sep +                                                  // Ubigeo del punto de partida
                row["direpar"] + sep +                                                  // Direccion completa y detallada del punto de partida
                row["gepLong"] + sep +                                                  // Punto de georreferencia del punto de partida, Longitud
                row["geplati"] + sep +                                                  // Punto de georreferencia del punto de partida, Latitud
                row["ubilleg"] + sep +                                                  // Ubigeo del punto de llegada
                row["dirlleg"] + sep +                                                  // Direccion completa y detallada del punto de llegada
                row["gelLong"] + sep +                                                  // Punto de georreferencia del punto de llegada, Longitud
                row["gellati"] + sep);                                                  // Punto de georreferencia del punto de llegada, Latitud
                                                                                        /* DATOS DE VEHICULOS */
            writer.WriteLine("VEHICULOS" + sep +
                row["plaTrac"] + sep +                                                  // Número de placa del vehículo
                row["ntaruni"] + sep +                                                  // Número de la Tarjeta Única deCirculación
                row["autcirc"] + sep +                                                  // Número de autorización del vehículo emitido por la entidad
                row["entauto"] + sep);                                                  // Entidad emisora de la autorización
            if (tx_pla_carret.Text.Trim() != "")
            {
                writer.WriteLine("VEHICULOS" + sep +
                row["plaCarr"] + sep +                                                  // Número de placa del vehículo CARRETA
                row["ntarunC"] + sep +                                                  // Número de la Tarjeta Única deCirculación CARRETA
                row["aucircC"] + sep +                                                  // Número de autorización del vehículo emitido por la entidad CARRETA
                row["entauto"] + sep);                                                  // Entidad emisora de la autorización es la misma que del trackto
            }
            /* DATOS DE CONDUCTORES  */
            writer.WriteLine("CONDUCTORES" + sep +
                row["tipdcho"] + sep +                                                  // Tipo de documento de identidad 
                row["numdcho"] + sep +                                                  // Numero de documento de identidad 
                row["nomdcho"] + sep +                                                  // Apellidos y nombres
                row["bredcho"]);                                                        // Número de licencia de conducir
                                                                                        /* BIENES A TRANSPORTAR */
            writer.WriteLine("ITEM" + sep +
                row["nordite"] + sep +                                                  // Numero de orden del item
                row["umedite"] + sep +                                                  // Unidad de medida del item
                row["cantite"] + sep +                                                  // Cantidad del item
                row["descite"] + sep +                                                  // Descripcion detallada del item
                row["codiite"] + sep +                                                  // Codigo del item
                row["cosuite"] + sep +                                                  // Código producto SUNAT
                row["cgtnite"] + sep +                                                  // Código GTIN
                row["paraite"] + sep +                                                  // Partida arancelaria
                row["nparite"] + sep +                                                  // Nombre del concepto de la partida arancelaria
                row["cparite"] + sep +                                                  // Código del concepto de la partida arancelaria
                row["inbnite"] + sep);                                                  // Indicador de bien normalizado

            writer.Flush();
            writer.Close();
            retorna = true;

            return retorna;
        }
        #endregion

        #endregion Sunat metodo directo

        #region boton_form GRABA EDITA ANULA, boton jala pre-guia
        private void bt_preg_Click(object sender, EventArgs e)
        {
            if ("NUEVO".Contains(Tx_modo.Text) && tx_pregr_num.Text.Trim() != "")
            {
                if (usoPGm != "manual")
                {
                    tx_pregr_num.Text = lib.Right("00000000" + tx_pregr_num.Text, 8);
                }
                // validamos que no se repita este numero, ojo que si esta en blanco no validamos
                MySqlConnection cnn = new MySqlConnection(DB_CONN_STR);
                cnn.Open();
                if (cnn.State == ConnectionState.Open)                 // lib.procConn(cnn) == true
                {
                    using (MySqlCommand micon = new MySqlCommand("select count(id) from cabguiai where sergui=@sgr and numpregui=@npg and estintreg<>'A0'", cnn))
                    {
                        micon.Parameters.AddWithValue("@sgr", tx_serie.Text);
                        micon.Parameters.AddWithValue("@npg", tx_pregr_num.Text);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr.GetInt32(0) == 1)
                                {
                                    MessageBox.Show("Esta repitiendo el número de pre guía" + Environment.NewLine +
                                        "", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    tx_pregr_num.Text = "";
                                    tx_pregr_num.Focus();
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Application.Exit();
                    return;
                }
                //
                tx_obser1.Focus();
                // jalamos datos de la pre-guía
                if (jalapg(tx_serie.Text, tx_pregr_num.Text, cnn) == true)
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
                    if (claveSeg == "") chk_seguridad.Enabled = true;
                    else
                    {
                        chk_seguridad.Checked = true;
                    }
                    tx_docsOr.Enabled = true;
                    tx_consig.Enabled = true;
                    tx_obser1.Enabled = true;
                    tx_obser1.Focus();
                    //dataGridView1_RowLeave(null, null);
                    //dataGridView1.ReadOnly = true;
                }
                cnn.Close();
            }
            if (!"NUEVO".Contains(Tx_modo.Text) && tx_pregr_num.Text.Trim() != "")
            {
                // jalamos la guía desde el numero de la pre-guia
                jalaoc("preg");   // 18/10/2023 Solo jalamos directamente desde la PG si la guía no esta anulada, si la GRE esta anulada nos dice que no existe el numero 
                jaladet(tx_idr.Text);
                chk_seguridad_CheckStateChanged(null, null);
                sololee();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            s_d_r = "Con";      // por defecto las guías van con documento relacionado 20/10/2023
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
            if (tx_flete.Text.Trim() == "" || tx_flete.Text.Trim() == "0")
            {
                MessageBox.Show("Ingrese el valor del flete", " Atención ");
                tx_flete.Focus();
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
                MessageBox.Show("Seleccione el tipo de moneda", " Atención ");
                cmb_mon.Focus();
                return;
            }
            else
            {
                if (tx_dat_mone.Text.Trim() != MonDeft)
                {
                    tx_fletMN.Text = (decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text)).ToString("#0.00");
                }
                else
                {
                    tx_fletMN.Text = tx_flete.Text;
                }
            }
            if (tx_dat_tdRem.Text != vtc_ruc && tx_dat_docOr.Text.Trim() == "")     // tx_dat_docOr.Text.Trim() == ""
            {
                var aa = MessageBox.Show("Debería registrar al menos un documento origen" + Environment.NewLine +
                    "desea continuar sin datos relacionados?", "Confirme documento relacionado",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.No)
                {
                    cmb_docorig.Focus();
                    return;
                }
                else
                {
                    tx_rucEorig.Text = "";
                    tx_docsOr.Text = "";
                    tx_dat_docOr.Text = "";
                    tx_dat_dorigS.Text = "";
                    //
                    tx_rucEorig2.Text = "";
                    tx_docsOr2.Text = "";
                    tx_dat_docOr2.Text = "";
                    tx_dat_dorigS2.Text = "";
                    //
                    s_d_r = "Sin";
                }
            }       // solo remitentes sin ruc se puede permitir sin docs relacionados
            if (tx_dat_tdRem.Text == vtc_ruc && tx_dat_docOr.Text.Trim() == "")
            {
                var aa = MessageBox.Show("Debe registrar al menos un documento origen", "Error, remitente con RUC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                {
                    cmb_docorig.Focus();
                    return;
                }
            }       // remitente con RUC es ogligatorio al menos un doc relacionado
            #endregion
            if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
            {
                #region validaciones GR electrónicas Sunat
                if (tx_pla_dniChof.Text.Trim() == "")
                {
                    MessageBox.Show("No existe DNI del chofer!", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tx_pla_placa.Text == "")
                {
                    MessageBox.Show("Las guías electrónicas de transportista" + Environment.NewLine +
                        "necesitan los datos del vehículo obligatoriamente", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (s_d_r == "Con")     // validaciones SUNAT - Documentos relacionados
                {
                    // documentos relacionados (doc relacionado 2, que esten completos si los hubiera)
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

                    // validaciones SUNAT - formatos del número de documento origen
                    if (tx_dat_dorigS.Text.Trim() != "" && "'01','03','04','09'".Contains(tx_dat_dorigS.Text))
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
                    if (tx_dat_dorigS2.Text != "" && "'01','03','04','09','31'".Contains(tx_dat_dorigS2.Text))
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
                    if (tx_dat_dorigS.Text.Trim() != "" && "'50','52'".Contains(tx_dat_dorigS.Text))
                    {
                        if (tx_docsOr.Text.Length > 18 || tx_docsOr.Text.Length < 8 || lib.repeticiones(tx_docsOr.Text, "-") != 3 ||
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
                    }                                             // Declaración de aduana DAM y de Mudanza
                    if (tx_dat_dorigS2.Text != "" && "'50'.'52'".Contains(tx_dat_dorigS2.Text))
                    {
                        if (tx_docsOr2.Text.Length > 18 || tx_docsOr2.Text.Length < 8 || lib.repeticiones(tx_docsOr2.Text, "-") != 3 ||
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
                    }               // Declaración de aduana DAM y de Mudanza
                    if (tx_dat_dorigS.Text != "" && "80".Contains(tx_dat_dorigS.Text))
                    {
                        if (tx_docsOr.Text.Length > 15 || tx_docsOr.Text.Length < 3 || lib.IsAllDigits(tx_docsOr.Text) == false ||
                            int.Parse(tx_docsOr.Text) <= 0)
                        {
                            MessageBox.Show("El formato de la constancia no es correcto, debe ser:" + Environment.NewLine +
                                    "Solo números no mayor a 15 dígitos > a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }                                                    // Constancia de deposito
                    if (tx_dat_dorigS2.Text != "" && "80".Contains(tx_dat_dorigS2.Text))
                    {
                        if (tx_docsOr2.Text.Length > 15 || tx_docsOr2.Text.Length < 3 || lib.IsAllDigits(tx_docsOr2.Text) == false ||
                            int.Parse(tx_docsOr2.Text) <= 0)
                        {
                            MessageBox.Show("El formato de la constancia no es correcto, debe ser:" + Environment.NewLine +
                                    "Solo números no mayor a 15 dígitos > a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr2.Focus();
                            return;
                        }
                    }                      // Constancia de deposito
                    if (tx_dat_dorigS.Text != "" && "12".Contains(tx_dat_dorigS.Text))
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
                    }                                                    // Ticket, cintas de maquinas registradoras
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
                    }                      // Ticket, cintas de maquinas registradoras
                    if (tx_dat_dorigS.Text != "" && "48".Contains(tx_dat_dorigS.Text))
                    {
                        if (tx_docsOr.Text.Trim().Length < 3 || tx_docsOr.Text.Length > 12 || !tx_docsOr.Text.Contains("-") || lib.repeticiones(tx_docsOr.Text, "-") > 1 ||
                            lib.IsAllDigits(lib.separador(tx_docsOr.Text, '-', 2)) == false || lib.separador(tx_docsOr.Text, '-', 2).Length > 7 ||
                            int.Parse(lib.separador(tx_docsOr.Text, '-', 2)) <= 0)
                        {
                            MessageBox.Show("El formato del comprobante no es correcto, debe ser:" + Environment.NewLine +
                                    "<serie>-<número> con esta estructura [0-9]{1,4}-[0-9]{1,7}" + Environment.NewLine +
                                    "El campo <número> debe ser mayor a cero", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }                                                    // Comprobante de operaciones
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
                    }                      // Comprobante de operaciones
                    if ("'82','65','66','67','68','69'".Contains(tx_dat_dorigS.Text))
                    {
                        // acá se permite todo menos espacios en blanco, saltos de linea y demas comunes
                    }
                    if (tx_dat_dorigS2.Text != "" && "'82','65','66','67','68','69'".Contains(tx_dat_dorigS2.Text))
                    {
                        // acá se permite todo menos espacios en blanco, saltos de linea y demas comunes
                    }
                    if (tx_dat_dorigS.Text != "" && "09".Contains(tx_dat_dorigS.Text) && lib.IsAllDigits(tx_docsOr.Text.Substring(0, 1)) == false)
                    {
                        if (tx_numDocRem.Text != tx_rucEorig.Text)
                        {
                            MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                                    "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr.Focus();
                            return;
                        }
                    }    // RUC emisor doc. relacionado GUIA remitente debe ser igual al ruc del remitente
                    if (tx_dat_dorigS2.Text != "" && "09".Contains(tx_dat_dorigS2.Text) &&
                        lib.IsAllDigits(tx_docsOr2.Text.Substring(0, 1)) == false)
                    {
                        if (tx_numDocRem.Text != tx_rucEorig2.Text)
                        {
                            MessageBox.Show("El número del documento del remitente debe " + Environment.NewLine +
                                    "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_docsOr2.Focus();
                            return;
                        }
                    }                                       // RUC emisor doc. relacionado GUIA remitente debe ser igual al ruc del remitente
                    if (tx_dat_dorigS.Text != "" && "31".Contains(tx_dat_dorigS.Text) && lib.IsAllDigits(tx_docsOr.Text.Substring(0, 1)) == false)
                    {
                        if (tx_pla_ruc.Text != tx_rucEorig.Text)
                        {
                            MessageBox.Show("El número del documento del Transportista debe " + Environment.NewLine +
                                    "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_pla_ruc.Focus();
                            return;
                        }
                    }   // RUC emisor doc. relacionado GUIA Transportista debe ser igual al ruc del dueño del camion que hará el traslado
                    if (tx_dat_dorigS2.Text != "" && "09".Contains(tx_dat_dorigS2.Text) &&
                        lib.IsAllDigits(tx_docsOr2.Text.Substring(0, 1)) == false)
                    {
                        if (tx_pla_ruc.Text != tx_rucEorig2.Text)
                        {
                            MessageBox.Show("El número del documento del Transportista debe " + Environment.NewLine +
                                    "ser igual al del emisor del documento origen", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_pla_ruc.Focus();
                            return;
                        }
                    }                                       // RUC emisor doc. relacionado GUIA Transportista debe ser igual al ruc del dueño del camion que hará el traslado
                }

                // Validaciones SUNAT - Datos del remitente
                if (tx_pla_ruc.Text == tx_numDocRem.Text)
                {
                    MessageBox.Show("El número del documento del remitente NO" + Environment.NewLine +
                            "DEBE ser igual al del transportista", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_numDocRem.Focus();
                    return;
                }
                // Validaciones SUNAT - Datos del Destinatario
                //          todo ok con las validaciones generales
                // Validaciones SUNAT - Vehículos
                if (tx_pla_autor.Text.Trim().Length < 9 || tx_pla_autor.Text.Trim().Length > 16)
                {
                    MessageBox.Show("Las autorizaciones de circulación deben" + Environment.NewLine +
                        "tener entre 10 y 15 caracteres alfanuméricos", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }                   // Aut. Circulación trackto, alfanumérico de 10 a 15 caracteres
                if (tx_aut_carret.Text.Trim() != "" &&
                    (tx_aut_carret.Text.Trim().Length < 9 || tx_aut_carret.Text.Trim().Length > 16))
                {
                    MessageBox.Show("Las autorizaciones de circulación deben" + Environment.NewLine +
                        "tener entre 10 y 15 caracteres alfanuméricos", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }               // Aut. Circulación carreta, alfanumérico de 10 a 15 caracteres
                // Validaciones SUNAT - Choferes
                if (tx_pla_dniChof.Text == "")
                {
                    MessageBox.Show("El número de documento del" + Environment.NewLine +
                        "chofer principal está en vacío", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }                                                                     // Núm doc identidad chofer principal
                if (tx_pla_chofS.Text == "" || tx_pla_chofS.Text.Trim() == "6")
                {
                    MessageBox.Show("El tipo de documento del chofer principal" + Environment.NewLine +
                        "está en vacío o no corresponde", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }                                    // Tipo de documento chofer principal
                if (tx_pla_brev2.Text == "" && tx_dat_dniC2.Text == "" && tx_pla_chofer2.Text == "")
                {
                    // todo ok, pasa nada
                }
                else
                {
                    if (tx_pla_brev2.Text != "" && tx_dat_dniC2.Text != "" && tx_pla_chofer2.Text != "")
                    {
                        // todo ok, datos completos
                    }
                    else
                    {
                        MessageBox.Show("Los datos del chofer secundario" + Environment.NewLine +
                            "no están completos", "Validación Sunat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }            // Tipo de documento chofer secundario                                          // Número de documento de identidad chofer principal y secundario
                             // Validaciones SUNAT - Datos de Envío
                if (chk_cunica.Checked == true)
                {
                    // No estamos considerando este dato en el xml y si deberíamos ... falta implementar
                }
                #endregion
            }
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // validaciones de pre-guia
                if (usoPGm == "manual" && bt_preg.Tag.ToString() == "")
                {
                    MessageBox.Show("Debe ingresar la Pre Guía", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                #region validaciones para nuevo
                if ((tx_pla_carret.Text != "" && tx_dat_carrNreg.Text == "") || tx_dat_plaNreg.Text == "")
                {
                    MessageBox.Show("El número de registro MTC del transportista está" + Environment.NewLine +
                        "faltando en los datos del tracko o de la carreta","Atención, complete los datos del vehículo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
                if (tx_numDocRem.Text == Program.ruc)
                {
                    MessageBox.Show("El remitente de la guía no puede ser " + Environment.NewLine +
                        "la misma empresa emisora de la guía", "Atención, validación de Sunat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tx_numDocRem.Focus();
                    return;
                }
                #endregion

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
                        if (graba() == true)
                        {
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            string todoBien = apis();
                            if (todoBien == "SI")
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
                                                using (MySqlCommand micon = new MySqlCommand("update cabguiai set impreso='S' where id=@idr"))
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
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("La guía esta ANULADA", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tx_numero.Focus();
                    return;
                }
                if (true)
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar la Guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            edita();
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (v_urege.Contains(asd) == true)
                            {
                                var bb = MessageBox.Show("Desea regenerar el XML ?", "Atención", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (bb == DialogResult.Yes)
                                {
                                    apis();
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
                if (tx_DV.Text.Trim().Length < 6 && tx_pagado.Text.Trim() == "")   // tx_impreso.Text == "N"    tx_pla_plani.Text.Trim() == "" && 
                {
                    // no tiene planilla y no esta impreso => se puede modificar todo y SI anular
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
                else
                {
                    sololee();
                    MessageBox.Show("No se puede Anular" + Environment.NewLine +
                        "Tiene Doc.Venta y/o cobranza vigente","Atención " + tx_DV.Text.Trim()+"|", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
                // pre-guia, modo creación automática
                if (bt_preg.Tag.ToString() == "Crealo")
                {
                    string insertaPG = "insert into cabpregr (" +
                        "fechpregr,serpregui,numpregui,tidodepre,nudodepre,nombdepre,diredepre,ubigdepre," +
                        "tidorepre,nudorepre,nombrepre,direrepre,ubigrepre,locorigen,dirorigen,ubiorigen,locdestin," +
                        "dirdestin,ubidestin,obspregui,clifinpre,cantotpre,pestotpre,tipmonpre,tipcampre," +
                        "subtotpre,igvpregui,totpregui,totpagpre,salpregui,estadoser,seguroE,m1cliente,m2cliente," +
                        "tidocor,rucDorig,docsremit,tidocor2,rucDorig2,docsremit2," +
                        "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                        "values (@fechop,@serpgr,@npg,@tdcdes,@ndcdes,@nomdes,@dircde,@ubicde," +
                        "@tdcrem,@ndcrem,@nomrem,@dircre,@ubicre,@locpgr,@dirpgr,@ubopgr,@ldcpgr," +
                        "@didegr,@ubdegr,@obsprg,@conprg,@totcpr,@totppr,@monppr,@tcprgr," +
                        "@subpgr,@igvpgr,@totpgr,@pagpgr,@totpgr,@estpgr,@clavse,@m1clte,@m2clte," +
                        "@tdocor,@rucDor,@dooprg,@tidoc2,@rucDo2,@docsr2," +
                        "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                    using (MySqlCommand micpg = new MySqlCommand(insertaPG, conn))
                    {
                        micpg.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micpg.Parameters.AddWithValue("@serpgr", "0" + lib.Right(tx_serie.Text, 3));
                        micpg.Parameters.AddWithValue("@npg", tx_pregr_num.Text);
                        micpg.Parameters.AddWithValue("@tdcdes", tx_dat_tDdest.Text);
                        micpg.Parameters.AddWithValue("@ndcdes", tx_numDocDes.Text);
                        micpg.Parameters.AddWithValue("@nomdes", tx_nomDrio.Text);
                        micpg.Parameters.AddWithValue("@dircde", tx_dirDrio.Text);
                        micpg.Parameters.AddWithValue("@ubicde", tx_ubigDtt.Text);
                        micpg.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                        micpg.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                        micpg.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                        micpg.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                        micpg.Parameters.AddWithValue("@ubicre", tx_ubigRtt.Text);
                        micpg.Parameters.AddWithValue("@locpgr", tx_dat_locori.Text);
                        micpg.Parameters.AddWithValue("@dirpgr", tx_dirOrigen.Text);
                        micpg.Parameters.AddWithValue("@ubopgr", tx_ubigO.Text);
                        micpg.Parameters.AddWithValue("@ldcpgr", tx_dat_locdes.Text);
                        micpg.Parameters.AddWithValue("@didegr", tx_dirDestino.Text);
                        micpg.Parameters.AddWithValue("@ubdegr", tx_ubigD.Text);
                        micpg.Parameters.AddWithValue("@obsprg", tx_obser1.Text);  // observaciones de la pre guia ... no hay
                        micpg.Parameters.AddWithValue("@conprg", tx_consig.Text);
                        micpg.Parameters.AddWithValue("@totcpr", tx_totcant.Text);
                        micpg.Parameters.AddWithValue("@totppr", tx_totpes.Text);
                        micpg.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                        micpg.Parameters.AddWithValue("@tcprgr", "0.00");  // tipo de cambio ... falta leer de la tabla de cambios
                        micpg.Parameters.AddWithValue("@subpgr", "0"); // sub total de la pre guía
                        micpg.Parameters.AddWithValue("@igvpgr", "0"); // igv
                        micpg.Parameters.AddWithValue("@totpgr", tx_flete.Text); // total inc. igv
                        micpg.Parameters.AddWithValue("@pagpgr", "0");
                        micpg.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                        micpg.Parameters.AddWithValue("@clavse", claveSeg);
                        micpg.Parameters.AddWithValue("@m1clte", v_clte_rem);
                        micpg.Parameters.AddWithValue("@m2clte", v_clte_des);
                        micpg.Parameters.AddWithValue("@tdocor", tx_dat_docOr.Text);                            // tipo del documento origen
                        micpg.Parameters.AddWithValue("@rucDor", tx_rucEorig.Text);                             // ruc del emisor del doc origen
                        micpg.Parameters.AddWithValue("@dooprg", tx_docsOr.Text);
                        micpg.Parameters.AddWithValue("@tidoc2", tx_dat_docOr2.Text);
                        micpg.Parameters.AddWithValue("@rucDo2", tx_rucEorig2.Text);
                        micpg.Parameters.AddWithValue("@docsr2", tx_docsOr2.Text);
                        micpg.Parameters.AddWithValue("@verApp", verapp);
                        micpg.Parameters.AddWithValue("@asd", asd);
                        micpg.Parameters.AddWithValue("@iplan", lib.iplan());
                        micpg.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micpg.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micpg.ExecuteNonQuery();
                    }
                    string lectura = "select last_insert_id()";
                    MySqlCommand mpg = new MySqlCommand(lectura, conn);
                    MySqlDataReader dr = mpg.ExecuteReader();
                    string idPG = "0";
                    if (dr.Read())
                    {
                        idPG = dr.GetString(0);
                    }
                    dr.Close();
                    // actualiza la tabla detalle,
                    string actuaD = "update detpregr set cantprodi=@can,unimedpro=@uni,codiprodi=@cod,descprodi=@des," +
                            "pesoprodi=@pes,precprodi=@preu,totaprodi=@pret " +
                            "where idc=@idr";
                    mpg = new MySqlCommand(actuaD, conn);
                    mpg.Parameters.AddWithValue("@idr", idPG);
                    mpg.Parameters.AddWithValue("@can", tx_det_cant.Text);      // dataGridView1.Rows[0].Cells[0].Value.ToString()
                    mpg.Parameters.AddWithValue("@uni", tx_det_umed.Text);      // dataGridView1.Rows[0].Cells[1].Value.ToString()
                    mpg.Parameters.AddWithValue("@cod", "");
                    mpg.Parameters.AddWithValue("@des", tx_det_desc.Text);      // dataGridView1.Rows[0].Cells[2].Value.ToString()
                    mpg.Parameters.AddWithValue("@pes", tx_det_peso.Text);      // dataGridView1.Rows[0].Cells[3].Value.ToString()
                    mpg.Parameters.AddWithValue("@preu", "0");
                    mpg.Parameters.AddWithValue("@pret", "0");
                    mpg.ExecuteNonQuery();
                    mpg.Dispose();
                    tx_pregr_num.Text = lib.Right(("0000000" + idPG), 8);
                }
                //
                if (v_clte_rem == "N" && rb_car_ofi.Checked == true) v_clte_rem = "P";  // N=nombre y direccion | P=solo nombre
                if (v_clte_des == "N" && rb_ent_ofic.Checked == true) v_clte_des = "P";  // N=nombre y direccion | P=solo nombre
                // asunto de la serie para la zona
                // la zona se jala del desc_loc del destino
                // EL NUMERO DE GUIA SIEMPRE DEBE SER AUTOMÁTICO ... ya no desde el 08/12/2020
                //if (tx_n_auto.Text == "A")  // EN GUIAS ELECTRONICAS SI O SI ES AUTOMATICO Y EL NUMERADOR YA SE CORRIO ANTES DEL TXT  21/03/2023
                if (tx_tipcam.Text.Trim() == "") tx_tipcam.Text = "0";
                decimal subtgr = Math.Round(decimal.Parse(tx_flete.Text) / (decimal.Parse(v_igv) / 100 + 1), 3);
                decimal igvtgr = Math.Round(decimal.Parse(tx_flete.Text) - subtgr, 3);
                if (tx_dat_mone.Text == MonDeft) tx_fletMN.Text = tx_flete.Text;
                else
                {
                    if (tx_tipcam.Text.Trim() == "" || tx_tipcam.Text == "0")
                    {
                        MessageBox.Show("Problema con la moneda o tipo de cambio", "No puede continuar");
                        return retorna;
                    }
                    else
                    {
                        tx_fletMN.Text = Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                }
                decimal subMN = Math.Round(decimal.Parse(tx_fletMN.Text) / (decimal.Parse(v_igv)/100 + 1),3);
                decimal igvMN = Math.Round(decimal.Parse(tx_fletMN.Text) - subMN,3);
                string inserta = "insert into cabguiai (" +
                    "fechopegr,sergui,numgui,numpregui,tidodegri,nudodegri,nombdegri,diredegri,ubigdegri," +
                    "tidoregri,nudoregri,nombregri,direregri,ubigregri,locorigen,dirorigen,ubiorigen," +
                    "locdestin,dirdestin,ubidestin,docsremit,obspregri,clifingri,cantotgri,pestotgri," +
                    "tipmongri,tipcamgri,subtotgri,igvgri,totgri,totpag,salgri,estadoser,cantfilas," +
                    "frase1,frase2,fleteimp,tipintrem,tipintdes,tippagpre,seguroE,m1cliente,m2cliente," +
                    "subtotMN,igvMN,totgrMN,codMN,grinumaut,teleregri,teledegri,igvporc," +
                    "idplani,fechplani,serplagri,numplagri,plaplagri,carplagri,autplagri,confvegri,breplagri,proplagri," +
                    "marca_gre,tidocor,rucDorig,lpagop,pesoKT,tidocor2,rucDorig2,docsremit2,marca1," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fechop,@sergr,@numgr,@npregr,@tdcdes,@ndcdes,@nomdes,@dircde,@ubicde," +
                    "@tdcrem,@ndcrem,@nomrem,@dircre,@ubicre,@locpgr,@dirpgr,@ubopgr," +
                    "@ldcpgr,@didegr,@ubdegr,@dooprg,@obsprg,@conprg,@totcpr,@totppr," +
                    "@monppr,@tcprgr,@subpgr,@igvpgr,@totpgr,@pagpgr,@totpgr,@estpgr,@canfil," +
                    "@frase1,@frase2,@fleimp,@ticlre,@ticlde,@tipacc,@clavse,@m1clte,@m2clte," +
                    "@stMN,@igMN,@tgMN,@codmn,@grinau,@telrem,@teldes,@igvpor," +
                    "@idplan,@fecpla,@serpla,@numpla,@plapla,@carpla,@autpla,@confve,@brepla,@propla," +
                    "@margre,@tdocor,@rucDor,@lpagop,@pesoKT,@tidoc2,@rucDo2,@docsr2,@marCU," +
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
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);            // total inc. igv
                    micon.Parameters.AddWithValue("@pagpgr", "0");
                    micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text);        // estado de la guía
                    micon.Parameters.AddWithValue("@frase1", (claveSeg == "") ? "" : v_fra1);
                    micon.Parameters.AddWithValue("@frase2", (chk_seguridad.Checked == true)? v_fra2 : "");
                    micon.Parameters.AddWithValue("@fleimp", (chk_flete.Checked == true) ? "S" : "N");
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
                    micon.Parameters.AddWithValue("@lpagop", (rb_pOri.Checked == true)? "O" : "D");         // mara de pago en origen o destino
                    micon.Parameters.AddWithValue("@pesoKT", (rb_kg.Checked == true) ? "K" : "T");          // peso en: Kilos o Toneladas
                    micon.Parameters.AddWithValue("@tidoc2", tx_dat_docOr2.Text);
                    micon.Parameters.AddWithValue("@rucDo2", tx_rucEorig2.Text);
                    micon.Parameters.AddWithValue("@docsr2", tx_docsOr2.Text);
                    micon.Parameters.AddWithValue("@marCU", (chk_cunica.Checked == true) ? 1 : 0);          // 0=carga consolidada normal, 1=carga única en el camión
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
                if (ipeeg == "API_SUNAT")       // en otro metodo no usamos la tabla adiguias
                {
                    // adicionales
                    string actag = "insert into adiguias (idg,serie,numero) values (@idg,@seg,@nug)";
                    using (MySqlCommand micon = new MySqlCommand(actag, conn))
                    {
                        micon.Parameters.AddWithValue("@idg", tx_idr.Text);
                        micon.Parameters.AddWithValue("@seg", tx_serie.Text);
                        micon.Parameters.AddWithValue("@nug", tx_numero.Text);
                        micon.ExecuteNonQuery();
                    }
                }
                // detalle
                int fila = 1;
                string inserd2 = "update detguiai set " +
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
                // clientes 
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
            decimal subtgr = Math.Round(decimal.Parse(tx_flete.Text) / (decimal.Parse(v_igv) / 100 + 1), 3);
            decimal igvtgr = Math.Round(decimal.Parse(tx_flete.Text) - subtgr, 3);
            if (tx_dat_mone.Text == MonDeft) tx_fletMN.Text = tx_flete.Text;
            else
            {
                if (tx_tipcam.Text.Trim() == "" || tx_tipcam.Text == "0")
                {
                    MessageBox.Show("Problema con la moneda o tipo de cambio", "No puede continuar");
                    return;
                }
                else
                {
                    tx_fletMN.Text = Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                }
            }
            decimal subMN = Math.Round(decimal.Parse(tx_fletMN.Text) / (decimal.Parse(v_igv) / 100 + 1), 3);
            decimal igvMN = Math.Round(decimal.Parse(tx_fletMN.Text) - subMN, 3);
            //
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    if (Tx_modo.Text == "EDITAR" && tx_estaSunat.Text.Trim() == "Rechazado" && v_urege.ToLower().Contains(asd.ToLower())) // true == true     
                    {
                        // EDICION DE CABECERA ... Al 06/01/2021 solo se permite editar observ y consignatario
                        // EDICION DE CABECERA ... al 05/05/2022 se permite editar docs.origen si eres usuario autorizado
                        // EN GUIAS ELECTRONICAS NO IMPORTA IMPRESO O NO, NO SE EDITA ESTOS VALORES 16/03/2023
                        // 15/11/2023, si Sunat rechazo la GRE entonces permite cambiar los datos para corregir y volver a enviar 15/11/2023
                        string acttotal = "update cabguiai set " +
                            "tidodegri=@tdcdes,nudodegri=@ndcdes,nombdegri=@nomdes,diredegri=@dircde,ubigdegri=@ubicde," +
                            "tidoregri=@tdcrem,nudoregri=@ndcrem,nombregri=@nomrem,direregri=@dircre,ubigregri=@ubicre," +
                            "locorigen=@locpgr,dirorigen=@dirpgr,ubiorigen=@ubopgr," +
                            "locdestin=@ldcpgr,dirdestin=@didegr,ubidestin=@ubdegr,docsremit=@dooprg," +
                            "obspregri=@obsprg,clifingri=@conprg,cantotgri=@totcpr,pestotgri=@totppr," +
                            "tipmongri=@monppr,tipcamgri=@tcprgr,subtotgri=@subpgr,igvgri=@igvpgr,totgri=@totpgr," +
                            "totpag=@pagpgr,salgri=@totpgr,estadoser=@estpgr,cantfilas=@canfil," +
                            "frase1=@frase1,frase2=@frase2,fleteimp=@fleimp,tipintrem=@ticlre,tipintdes=@ticlde," +
                            "tippagpre=@tipacc,seguroE=@clavse,m1cliente=@m1clte,m2cliente=@m2clte," +
                            "subtotMN=@stMN,igvMN=@igMN,totgrMN=@tgMN,codMN=@codmn,grinumaut=@grinau," +
                            "teleregri=@telrem,teledegri=@teldes,igvporc=@igvpor," +
                            "idplani=@idplan,fechplani=@fecpla,serplagri=@serpla,numplagri=@numpla,plaplagri=@plapla," +
                            "carplagri=@carpla,autplagri=@autpla,confvegri=@confve,breplagri=@brepla,proplagri=@propla," +
                            "tidocor=@tdocor,rucDorig=@rucDor,lpagop=@lpagop,pesoKT=@pesoKT,tidocor2=@tidoc2," +
                            "rucDorig2=@rucDo2,docsremit2=@docsr2,marca1=@marCU," +
                            "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam " +
                            "where id=@idr";
                        using (MySqlCommand micon = new MySqlCommand(acttotal, conn))
                        {
                            micon.Parameters.AddWithValue("@idr", tx_idr.Text);
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
                            micon.Parameters.AddWithValue("@canfil", (tx_tfil.Text.Trim() == "") ? "0" : tx_tfil.Text);             // cantidad de filas de detalle
                            micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                            micon.Parameters.AddWithValue("@igvpor", v_igv);                    // igv en porcentaje
                            micon.Parameters.AddWithValue("@tcprgr", tx_tipcam.Text);           // tipo de cambio ... falta leer de la tabla de cambios
                            micon.Parameters.AddWithValue("@subpgr", subtgr.ToString());        // sub total
                            micon.Parameters.AddWithValue("@igvpgr", igvtgr.ToString());        // igv
                            micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);            // total inc. igv
                            micon.Parameters.AddWithValue("@pagpgr", "0");
                            micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text);        // estado de la guía
                            micon.Parameters.AddWithValue("@frase1", (claveSeg == "") ? "" : v_fra1);
                            micon.Parameters.AddWithValue("@frase2", (chk_seguridad.Checked == true) ? v_fra2 : "");
                            micon.Parameters.AddWithValue("@fleimp", (chk_flete.Checked == true) ? "S" : "N");
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
                            else micon.Parameters.AddWithValue("@fecpla", tx_pla_fech.Text);    // tx_pla_fech.Text.Substring(6, 4) + "-" + tx_pla_fech.Text.Substring(3, 2) + "-" + tx_pla_fech.Text.Substring(0, 2)
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
                            micon.Parameters.AddWithValue("@tdocor", tx_dat_docOr.Text);                            // tipo del documento origen
                            micon.Parameters.AddWithValue("@rucDor", tx_rucEorig.Text);                             // ruc del emisor del doc origen
                            micon.Parameters.AddWithValue("@lpagop", (rb_pOri.Checked == true) ? "O" : "D");         // mara de pago en origen o destino
                            micon.Parameters.AddWithValue("@pesoKT", (rb_kg.Checked == true) ? "K" : "T");          // peso en: Kilos o Toneladas
                            micon.Parameters.AddWithValue("@tidoc2", tx_dat_docOr2.Text);
                            micon.Parameters.AddWithValue("@rucDo2", tx_rucEorig2.Text);
                            micon.Parameters.AddWithValue("@docsr2", tx_docsOr2.Text);
                            micon.Parameters.AddWithValue("@marCU", (chk_cunica.Checked == true) ? 1 : 0);          // 0=carga consolidada normal, 1=carga única en el camión
                            //                                                                                        //
                            micon.Parameters.AddWithValue("@verApp", verapp);
                            micon.Parameters.AddWithValue("@asd", asd);
                            micon.Parameters.AddWithValue("@iplan", lib.iplan());
                            micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                            micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                            //
                            micon.ExecuteNonQuery();
                        }
                    }
                    else    // if (true)   // tx_impreso.Text == "S"
                    {
                        // EDICION DE CABECERA ... Al 06/01/2021 solo se permite editar observ y consignatario
                        // EDICION DE CABECERA ... al 05/05/2022 se permite editar docs.origen si eres usuario autorizado
                        string actua = "update cabguiai a set " +
                            "a.obspregri=@obsprg,a.clifingri=@conprg,a.numpregui=@npregr," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@conprg", tx_consig.Text);
                        micon.Parameters.AddWithValue("@npregr", tx_pregr_num.Text);
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
            // En Guías de remisión electrónicas NO HAY ANULACION INTERNA, todas las anulaciones (bajas de comprobante)
            // se hacen DESPUES de haberse hecho en sunat en el portal con clave SOL o en el app emprender 08/03/2023
            // En el caso de que no se haya generado el xml o el comprobante no haya sido enviada a Sunat por cualquier
            // motivo entonces mejor habilitamos la anulación interna 12/07/2023
            string parte = " ";
            var aa = DialogResult.No;
            if (v_uagin.Contains(asd))   // usuario con acceso a anulación interna
            {
                aa = MessageBox.Show("Anulación interna para recuperar el número?" + Environment.NewLine +
                    "Se cambia la serie a ANU", "Atención, confirme por favor",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes) parte = ",a.sergui=@coad,b.serie=@coad ";
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabguiai a left join adiguias b on b.idg=a.id " +
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
                //tx_numero_Leave(null,null);   // comentado el 08/08/2023
                tx_obser1.Enabled = true;
                analizaEdicion();
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento del remitente, jala provincia
        {
            if(tx_dptoRtt.Text.Trim() != "")    //  && TransCarga.Program.vg_conSol == false
            {
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
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and nombre='" + tx_provRtt.Text.Trim() + "' and provin<>'00' and distri='00'");
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
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and nombre='" + tx_distRtt.Text.Trim() + "'");
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
                DataRow[] row = Program.dt_ubigeos.Select("nombre='" + tx_dptoDrio.Text.Trim() + "' and provin='00' and distri='00'");
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
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and nombre='" + tx_proDrio.Text.Trim() + "' and provin<>'00' and distri='00'");
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
                DataRow[] row = Program.dt_ubigeos.Select("depart='" + tx_ubigDtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigDtt.Text.Substring(2, 2) + "' and nombre='" + tx_disDrio.Text.Trim() + "'");
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
                    }
                    if (tx_dat_tdRem.Text == vtc_ruc)
                    {
                        if (encuentra == "no")
                        {
                            if (TransCarga.Program.vg_conSol == true) // conector solorsoft para ruc
                            {
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
                    cmb_docorig.Focus();
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
                if (lib.valientabla("cabguiai", "concat(sergui,numgui)", tx_serie.Text + tx_numero.Text) == "1")
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
                chk_seguridad_CheckStateChanged(null, null);
                analizaEdicion();
                //sololee();
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            string es = tx_serie.Text.Trim();
            if (es.Substring(0, 1) == "V")
            {
                if (es.Length == 2) tx_serie.Text = es.Substring(0, 1) + "00" + es.Substring(1, 1);
                if (es.Length == 3) tx_serie.Text = es.Substring(0, 1) + "0" + es.Substring(1, 2);
            }
            else
            {
                if (es.Substring(0, 1) == "0") tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
                else
                {
                    tx_serie.Text = "";
                }
            }
        }
        private void tx_pregr_num_Leave(object sender, EventArgs e)     // numero pre guía
        {
            // esta parte lo pase al boton de jalar bt_pre
            //bt_preg_Click(null, null);
            if (tx_pregr_num.Text != "") tx_pregr_num.Text = lib.Right("0000000" + tx_pregr_num.Text.Trim(), 8);
        }
        private void tx_flete_Leave(object sender, EventArgs e)
        {
            if ((Tx_modo.Text == "NUEVO" && tx_flete.Text.Trim() != "") || (Tx_modo.Text == "EDITAR" && tx_flete.Enabled == true && tx_flete.ReadOnly == false))
            {
                if (tx_dat_mone.Text == MonDeft)
                {
                    tx_fletMN.Text = tx_flete.Text;
                }
                else
                {
                    if (tx_tipcam.Text.Trim() != "" && tx_tipcam.Text.Trim() != "0")
                    {
                        tx_fletMN.Text = Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                    else
                    {
                        MessageBox.Show("Se requiere tipo de cambio","Atención",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        tx_flete.Text = "";
                        cmb_mon.Focus();
                        return;
                    }
                }
            }
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
        private void rb_ent_ofic_Click(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && rb_ent_ofic.Checked == true)
            {
                if (tx_dat_locdes.Text != "")
                {
                    // idcodice,descrizionerid,ubidir,marca1,marca2,deta1,deta2,deta3,deta4
                    DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locdes.Text + "'");
                    tx_ubigDtt.Text = fila[0]["ubiDir"].ToString();
                    tx_dirDrio.Text = fila[0]["deta1"].ToString();
                    tx_dptoDrio.Text = fila[0]["deta2"].ToString();
                    tx_proDrio.Text = fila[0]["deta3"].ToString();
                    tx_disDrio.Text = fila[0]["deta4"].ToString();
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
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locori.Text + "'");
                tx_ubigRtt.Text = fila[0]["ubiDir"].ToString();
                tx_dirRem.Text = fila[0]["deta1"].ToString();
                tx_dptoRtt.Text = fila[0]["deta2"].ToString();
                tx_provRtt.Text = fila[0]["deta3"].ToString();
                tx_distRtt.Text = fila[0]["deta4"].ToString();
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
                tx_flete.Focus();
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
                tx_idplan.Text = "";
                tx_aut_carret.Text = "";
                // OJO, para volver a manifestar, solo se debe volver a seleccionar el local de destino
                chk_man.Checked = false;
            }
        }
        private void tx_rucEorig2_Leave(object sender, EventArgs e)              // validamos el ruc del emisor documento origen 2
        {
            valiruc(tx_rucEorig2);
        }
        private void tx_rucEorig_Validating(object sender, System.ComponentModel.CancelEventArgs e)              // validamos el ruc del emisor documento origen 1
        {
            valiruc(tx_rucEorig);
            if (Tx_modo.Text == "NUEVO")
            {
                tx_det_cant.Focus();
            }
        }
        #endregion

        #region botones_de_comando
        public void toolboton()
        {
            DataTable mdtb = new DataTable();
            const string consbot = "select * from permisos where formulario=@nomform and upper(usuario)=@use";
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    MySqlCommand consulb = new MySqlCommand(consbot, conn);
                    consulb.Parameters.AddWithValue("@nomform", nomform);
                    consulb.Parameters.AddWithValue("@use", asd.ToUpper());
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
                if (Convert.ToString(row["btn3"]) == "S")
                {
                    this.Bt_anul.Visible = true;
                }
                else { this.Bt_anul.Visible = false; }
                if (Convert.ToString(row["btn4"]) == "S")
                {
                    this.Bt_ver.Visible = true;
                }
                else { this.Bt_ver.Visible = false; }
                if (Convert.ToString(row["btn5"]) == "S")
                {
                    this.Bt_print.Visible = true;
                }
                else { this.Bt_print.Visible = false; }
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
                //tx_pregr_num.Focus();       // 06/12/2023 ya no
                cmb_destino.Focus();
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
                    if (datosImpresion() == true) updateprint("S");
                    /*
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
                    */
                }
            }
            else
            {
                if (Tx_modo.Text == "NUEVO" && tx_idr.Text == "")
                {
                    MessageBox.Show("No se puede imprimir sin grabar!", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (datosImpresion() == true) updateprint("S");
                /*
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
                */
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
                tx_pregr_num.ReadOnly = false;  // 18/10/2023
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
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_tdRem.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mld.Text = row["codigo"].ToString();
                    tx_dat_csrem.Text = row["codsunat"].ToString();
                    tx_dat_nomcsr.Text = row["deta1"].ToString();
                }
            }
        }
        private void cmb_docDes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docDes.SelectedIndex > -1)
            {
                tx_dat_tDdest.Text = cmb_docDes.SelectedValue.ToString();
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_tDdest.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mldD.Text = row["codigo"].ToString();
                    tx_dat_codsu.Text = row["codsunat"].ToString();
                    tx_dat_nomcsd.Text = row["deta1"].ToString();
                }
            }
        }
        private void cmb_mon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")   // ("NUEVO,EDITAR").Contains(Tx_modo.Text.Trim())
            {
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    if (tx_dat_mone.Text != MonDeft)
                    {
                        vtipcam vtipcam = new vtipcam(tx_flete.Text,tx_dat_mone.Text,tx_fechope.Text);
                        var result = vtipcam.ShowDialog();
                        if (vtipcam.ReturnValue1 != "" || vtipcam.ReturnValue1 != "0")
                        {
                            //cmb_mon.SelectedValue = MonDeft;
                            tx_flete.Text = vtipcam.ReturnValue1;
                            tx_fletMN.Text = vtipcam.ReturnValue2;
                            tx_tipcam.Text = vtipcam.ReturnValue3;
                        }
                        else
                        {
                            tx_flete.Text = "";
                            tx_fletMN.Text = "";
                            tx_tipcam.Text = "";
                        }
                    }
                    tx_flete.Focus();
                }
            }
        }
        private void cmb_origen_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_origen.SelectedIndex > -1)
            {
                tx_dat_locori.Text = cmb_origen.SelectedValue.ToString();
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locori.Text + "'");
                tx_dirOrigen.Text = fila[0]["deta1"].ToString() + " - " + fila[0]["deta2"].ToString() +  " - " + fila[0]["deta3"].ToString() + " - " + fila[0]["deta4"].ToString();
                tx_ubigO.Text = fila[0]["ubiDir"].ToString();
            }
            // lo de arriba viene del selectedindexhcnaged
            if (tx_dat_locori.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locori.Text + "'");
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
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locdes.Text + "'");
                tx_dirDestino.Text = fila[0]["deta1"].ToString() + " - " + fila[0]["deta2"].ToString() + " - " + fila[0]["deta3"].ToString() + " - " + fila[0]["deta4"].ToString(); // lib.dirloca(tx_dat_locdes.Text);
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
                            "a.dnichofer,a.dniayudante,a.nregtrackto,a.nregcarreta,a.tipdocpri,a.tipdocayu " +
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
                                    if (nfila > -1)
                                    {
                                        DataRow row = data.Rows[nfila];
                                        tx_idplan.Text = row["id"].ToString();
                                        tx_pla_fech.Text = row["fechope"].ToString().Substring(0, 10);
                                        tx_pla_plani.Text = row["serplacar"].ToString() + row["numplacar"].ToString();
                                        tx_pla_placa.Text = row["platracto"].ToString();
                                        tx_pla_carret.Text = row["placarret"].ToString();
                                        tx_pla_autor.Text = row["autorizac"].ToString();
                                        tx_dat_plaNreg.Text = row["nregtrackto"].ToString();      // num reg mtc del transportista
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
                                        tx_dat_carrNreg.Text = row["nregcarreta"].ToString();   // num reg MTC  a.nregtrackto,a.nregcarreta
                                        tx_marCarret.Text = row["marcaCarret"].ToString();
                                        tx_pla_dniChof.Text = row["dnichofer"].ToString();  //.Trim() == "") ? lib.Right(row["brevchofe"].ToString(), 8) : row["dnichofer"].ToString();
                                        tx_dat_dniC2.Text = row["dniayudante"].ToString();  //.Trim() == "") ? (row["brevayuda"].ToString().Trim() == "") ? "" : lib.Right(row["brevayuda"].ToString(), 8) : row["dniayudante"].ToString();
                                                                                            //
                                        chk_man.Checked = false;
                                        chk_man.Enabled = true;
                                        if (row["tipdocpri"].ToString() != "")
                                        {
                                            //DataRow[] fla = dttd2.Select("idcodice='" + row["tipdocpri"].ToString() + "'");
                                            DataRow[] fla = Program.dt_definic.Select("idcodice='" + row["tipdocpri"].ToString() + "'");
                                            tx_pla_chofS.Text = fla[0]["codsunat"].ToString();
                                            if (row["tipdocayu"] != null && row["tipdocayu"].ToString() != "")
                                            {
                                                fla = Program.dt_definic.Select("idcodice='" + row["tipdocayu"].ToString() + "'");
                                                tx_dat_dniC2s.Text = fla[0]["codsunat"].ToString();
                                            }
                                        }
                                    }
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
                                    tx_dat_dniC2s.Text = "";
                                    tx_pla_chofS.Text = "";
                                    //
                                    chk_man.Checked = false;
                                    chk_man.Enabled = false;
                                }
                            }
                        }
                    }
                    if (tx_pregr_num.Text.Trim() == "")
                    {
                        cmb_docRem.Focus();
                        cmb_docRem.DroppedDown = true;
                    }
                }
            }
            if (tx_dat_locdes.Text.Trim() != "")
            {
                DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_locdes.Text + "'");
                tx_ubigD.Text = fila[0]["ubiDir"].ToString();
            }
            // if(Tx_modo.Text == "NUEVO") rb_ent_clte.PerformClick();
        }
        private void cmb_docorig_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docorig.SelectedIndex > -1)
            {
                tx_dat_docOr.Text = cmb_docorig.SelectedValue.ToString();
                if (tx_dat_docOr.Text.Trim() != "")
                {
                    DataRow[] fila = Program.dt_definic.Select("idcodice='" + tx_dat_docOr.Text + "'");
                    tx_dat_dorigS.Text = fila[0]["codsunat"].ToString();     // codsunat
                    if (fila[0]["marca1"].ToString() == "1")              // sunat permite 2 documntos relacionados 
                    {
                        cmb_docorig2.Enabled = true;
                    }
                    else
                    {
                        cmb_docorig2.SelectedIndex = -1;
                        cmb_docorig2.Enabled = false;
                        tx_docsOr2.Text = "";
                        tx_dat_docOr2.Text = "";
                        tx_dat_dorigS2.Text = "";
                        tx_rucEorig2.Text = "";
                    }
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
                // debe ir en orden, no puede haber un segundo documento si el primero esta vacío
                cmb_docorig2.SelectedIndex = -1;
                tx_dat_docOr2.Text = "";
                tx_dat_dorigS2.Text = "";
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
                    tx_dat_dorigS2.Text = fila[0]["codsunat"].ToString();     // codsunat
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
                    tx_dat_dorigS2.Text = "";
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
                    tx_dat_dorigS.Text = "";
                    tx_docsOr.Text = "";
                    tx_docsOr.ReadOnly = true;
                    tx_rucEorig.Text = "";
                    tx_rucEorig.ReadOnly = true;
                }
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
            bool retorna = false;
            datosImpresion();
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            datosImpresion();
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
               //imprime_TK(sender, e);     // ahora utilizamos la clase impresor
            }
        }
        private void imprime_A4(float pix, float piy, string cliente, float coli, float alin, float posi, float alfi, float deta, float pie, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // no hay en A4, salvo del pdf del ose o sunat
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
                string consulta = "update cabguiai set impreso=@sn where id=@idr";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@sn", sn);
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.ExecuteNonQuery();
                }
            }
        }
        private bool datosImpresion()
        {
            bool retorna = false;

                string[] vs = {"","","","","","","","","","","","","", "", "", "", "", "", "", "",   // 20
                               "", "", "", "", "", "", "", "", "", "", ""};    // 11
                string[] vc = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };   // 16
                string[] va = { "", "", "", "", "", "", "", "", "" };       // 9
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
                if (tx_pla_fech.Text != "") vs[16] = tx_pla_fech.Text.Substring(8, 2) + "/" + tx_pla_fech.Text.Substring(5, 2) + "/" + tx_pla_fech.Text.Substring(0, 4);      // dr.GetString("fechplani")
                else vs[16] = "";
                vs[17] = tx_totpes.Text;                     // dr.GetString("pestotgri")
            if (chk_cunica.Checked == true) vs[18] = (rb_kg.Checked == true) ? "KILOGRAMO" : "TONELADA";    // dr.GetString("pesoKT")    
            else vs[18] = (rb_kg.Checked == true) ? "K" : "T";                        // dr.GetString("pesoKT")
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
                vs[29] = tx_pregr_num.Text;                 // número de pre-guia (orden de servicio)
                vs[30] = tx_flete.Text;                     // flete del servicio, solo para impresion, no debe ir a sunat

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
                va[1] = rutaQR + nomImgQR;                   // "C:\temp\"+"imgQR.png"
                va[2] = despedida;
                va[3] = "";                                  // Varios: segunda linea de despedida
                va[4] = glosa1;
                va[5] = glosa2;
                va[6] = tx_consig.Text.Trim();              // consignatario
                va[7] = tx_telR.Text.Trim();
                va[8] = tx_telD.Text.Trim();

                int y = 0;
                dt[y, 0] = (y + 1).ToString();              // detalle: Num de fila
                dt[y, 1] = tx_det_cant.Text;                // detalle: Cant.
                dt[y, 2] = tx_det_umed.Text;                // detalle: Unidad de medida
                dt[y, 3] = gloDeta + " " + tx_det_desc.Text;                // detalle: Descripción
                dt[y, 4] = tx_det_peso.Text;                // detalle: peso

            if (Tx_modo.Text == "NUEVO")
            {   // si es nuevo, se imprimen 2 copias
                if (chk_cunica.Checked == true)
                {
                    impGRE_T impGRE = new impGRE_T(1, v_impA5, vs, dt, va, vc, "A4", v_CR_gre_A4);
                }
                else
                {
                    if (vi_formato == "A5") { impGRE_T impGRE = new impGRE_T(int.Parse(vi_copias), v_impA5, vs, dt, va, vc, vi_formato, v_CR_gr_ind); }
                    if (vi_formato == "TK") { impGRE_T impGRE = new impGRE_T(int.Parse(vi_copias), v_impTK, vs, dt, va, vc, vi_formato, v_CR_gr_ind); }
                    retorna = true;
                }
            }
            else
            {   // si NO es nuevo, se imprime 1 copia
                if (chk_cunica.Checked == true)
                {
                    impGRE_T impGRE = new impGRE_T(1, v_impA5, vs, dt, va, vc, "A4", v_CR_gre_A4);
                }
                else
                {
                    if (vi_formato == "TK") { impGRE_T impGRE = new impGRE_T(1, v_impTK, vs, dt, va, vc, vi_formato, v_CR_gr_ind); }
                    if (vi_formato == "A5") { impGRE_T impGRE = new impGRE_T(1, v_impA5, vs, dt, va, vc, vi_formato, v_CR_gr_ind); }
                }
                retorna = true;
            }
            return retorna;
        }
        #endregion

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //jalainfo();
        }

    }
}
