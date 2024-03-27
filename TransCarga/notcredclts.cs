using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing.Imaging;
//using com.tuscomprobantespe.webservice;
using System.Collections.Generic;
using Newtonsoft.Json;
using CrystalDecisions.CrystalReports.Engine;
using System.Xml;
using System.IO.Compression;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace TransCarga
{
    public partial class notcredclts : Form
    {
        static string nomform = "notcredclts";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabdebcred";              // cabecera

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
        //string vtc_dni = "";            // variable tipo cliente natural
        //string vtc_ruc = "";            // variable tipo cliente empresa
        //string vtc_ext = "";            // variable tipo cliente extranjero
        string codAnul = "";            // codigo de documento anulado
        string codGene = "";            // codigo documento nuevo generado
        string codCanc = "";            // codigo documento cancelado (pagado 100%)
        string MonDeft = "";            // moneda por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        string nomImp = "";             // nombre de la impresora grafica
        string v_impTK = "";            // nombre de la ticketera
        //string v_cid = "";              // codigo interno de tipo de documento
        string v_fra2 = "";             // frase que va en obs de cobranza cuando se cancela desde el doc.vta.
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_codidv = "";           // variable codifo interno de documento de venta en vista TDV
        string v_codinc = "";           // codigo interno de notas de credito en vista TDV
        string codfact = "";            // idcodice de factura
        string v_igv = "";              // valor igv %
        string logoclt = "";            // ruta y nombre archivo logo
        //string fshoy = "";              // fecha hoy del servidor en formato ansi
        //string codppc = "";             // codigo del plazo de pago por defecto para fact a crédito
        string v_codnot = "";           // codigo tipo de documento nota de credito
        //
        string rutatxt = "";            // ruta de los txt para la fact. electronica
        string rutaxml = "";            // ruta para los XML
        string tipdo = "";              // CODIGO SUNAT tipo de documento de venta
        string tipoDocEmi = "";         // CODIGO SUNAT tipo de documento RUC/DNI
        string tipoMoneda = "";         // CODIGO SUNAT tipo de moneda
        string glosdet = "";            // glosa para las operaciones con detraccion
        string glosser = "";            // glosa que va en el detalle del doc. de venta
        string restexto = "xxx";        // texto resolucion sunat autorizando prov. fact electronica
        string autoriz_OSE_PSE = "yyy"; // numero resolucion sunat autorizando prov. fact electronica
        string despedida = "";          // texto para mensajes al cliente al final de la impresión del doc.vta. 
        string webose = "";             // direccion web del ose o pse para la descarga del 
        string rutaQR = "";             // ruta donde se trabajan los QR -> "C:\temp\"
        string correo_gen = "";         // correo generico del emisor cuando el cliente no tiene correo propio
        string usuaInteg = "";          // usuario de la integracion con SeenCorp
        string clavInteg = "";          // clave de la integracion con SeenCorp
        string nipfe = "";              // proveedor electrónico
        string rutaCertifc = "";        // Ruta y nombre del certificado .pfx
        string claveCertif = "";        // Clave del certificado
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        static NumLetra numLetra = new NumLetra();
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string ubiclie = Program.ubidirfis;         // ubigeo direc fiscal
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        string dirloc = TransCarga.Program.vg_duse; // direccion completa del local usuario conectado
        string ubiloc = TransCarga.Program.vg_uuse; // ubigeo local del usuario conectado
        #endregion

        DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        static string CadenaConexion = "Data Source=TransCarga.db";

        DataTable dtu = new DataTable();        // detalle del documento
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();        // moneda
        DataTable dttdn = new DataTable();      // tip doc notas cred
        string[] vs = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",      // 20
                           "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};     // 20
        string[] va = { "", "", "", "", "", "", "", "", "", "" };      // 10
        string[,] dt = new string[10, 9] {
                    { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" },
                    { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }
                }; // 6 columnas, 10 filas

        public notcredclts()
        {
            InitializeComponent();
        }
        private void notcredclts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void notcredclts_Load(object sender, EventArgs e)
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
            CreaTablaLiteNC();
            this.KeyPreview = true;
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
            tx_nomuser.Text = TransCarga.Program.vg_nuse;   // lib.nomuser(asd);
            //tx_locuser.Text = TransCarga.Program.vg_luse;  // lib.locuser(asd);
            tx_locuser.Text = tx_locuser.Text + " " + TransCarga.Program.vg_nlus;
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
            // ...
            // longitudes maximas de campos
            tx_serie.MaxLength = 4;         // serie nota de credito
            tx_numero.MaxLength = 8;        // numero nota de credito
            tx_serGR.MaxLength = 4;         // serie factura
            tx_numGR.MaxLength = 8;         // numero factura
            tx_numDocRem.MaxLength = 11;    // ruc o dni cliente
            tx_dirRem.MaxLength = 100;
            tx_nomRem.MaxLength = 100;           // nombre remitente
            tx_distRtt.MaxLength = 25;
            tx_provRtt.MaxLength = 25;
            tx_dptoRtt.MaxLength = 25;
            tx_obser1.MaxLength = 150;
            tx_fletLetras.MaxLength = 249;
            // grilla
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // todo desabilidado
            sololee();
        }
        private void initIngreso()
        {
            string[] vs = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",      // 20
                           "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};     // 20
            string[] va = { "", "", "", "", "", "", "", "", "", "" };      // 10
            string[,] dt = new string[10, 9] {
                    { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" },
                    { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }
                }; // 6 columnas, 10 filas
            string[] cu = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };    // 17

            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            dataGridView1.Rows.Clear();
            dataGridView1.ReadOnly = true;
            tx_igv.Text = "";
            tx_subt.Text = "";
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
            tx_fletLetras.ReadOnly = true;
            //
            if (Tx_modo.Text == "NUEVO")
            {
                gbox_serie.Enabled = true;
                cmb_tnota.Enabled = false;
                tx_serie.ReadOnly = true;
                cmb_tnota_SelectedIndexChanged(null, null);
            }
            tx_dat_tnota.Text = v_codnot;
            cmb_tnota.SelectedValue = v_codnot;
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofi,@nofa,@noco,@nogr)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                micon.Parameters.AddWithValue("@nofi", "clients");
                micon.Parameters.AddWithValue("@noco", "facelect");
                micon.Parameters.AddWithValue("@nofa", nomform);
                micon.Parameters.AddWithValue("@nogr", "guiati_e");
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
                            if (row["param"].ToString() == "img_preview") img_ver = row["valor"].ToString().Trim();      // imagen del boton grabar visualizar
                            if (row["param"].ToString() == "logoPrin") logoclt = row["valor"].ToString().Trim();         // logo emisor
                        }
                        if (row["campo"].ToString() == "estado")
                        {
                            if (row["param"].ToString() == "anulado") codAnul = row["valor"].ToString().Trim();         // codigo doc anulado
                            if (row["param"].ToString() == "generado") codGene = row["valor"].ToString().Trim();        // codigo doc generado
                            if (row["param"].ToString() == "cancelado") codCanc = row["valor"].ToString().Trim();        // codigo doc cancelado
                        }
                        if (row["campo"].ToString() == "rutas")
                        {
                            if (row["param"].ToString() == "grt_xml") rutaxml = row["valor"].ToString().Trim();         // 
                            if (row["param"].ToString() == "fe_txt") rutatxt = row["valor"].ToString().Trim();         // ruta de los txt para la fact. electronica
                        }
                        if (row["campo"].ToString() == "sunat")
                        {
                            //if (row["param"].ToString() == "client_id") client_id_sunat = row["valor"].ToString().Trim();         // id del api sunat
                            //if (row["param"].ToString() == "client_pass") client_pass_sunat = row["valor"].ToString().Trim();     // password del api sunat
                            //if (row["param"].ToString() == "user_sol") u_sol_sunat = row["valor"].ToString().Trim();              // usuario sol portal sunat del cliente 
                            //if (row["param"].ToString() == "clave_sol") c_sol_sunat = row["valor"].ToString().Trim();             // clave sol portal sunat del cliente 
                            //if (row["param"].ToString() == "scope") scope_sunat = row["valor"].ToString().Trim();                 // scope del api sunat
                            if (row["param"].ToString() == "rutaCertifc") rutaCertifc = row["valor"].ToString().Trim();           // Ruta y nombre del certificado .pfx
                            if (row["param"].ToString() == "claveCertif") claveCertif = row["valor"].ToString().Trim();           // Clave del certificado
                            //if (row["param"].ToString() == "wsPostSunatF") wsPostS = row["valor"].ToString().Trim();               // ruta api sunat para postear
                        }
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "frase2") v_fra2 = row["valor"].ToString().Trim();                // frase cuando se cancela el doc.vta.
                            if (row["param"].ToString() == "codigo") v_codnot = row["valor"].ToString().Trim();              // codigo nota de credito
                            if (row["param"].ToString() == "factura") codfact = row["valor"].ToString().Trim();              // codigo doc.venta factura
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            //if (row["param"].ToString() == "nomfor_cr") v_CR_NC1 = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();      // moneda por defecto
                        if (row["campo"].ToString() == "detraccion" && row["param"].ToString() == "glosa") glosdet = row["valor"].ToString().Trim();    // glosa detraccion
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "codinDV" && row["param"].ToString() == "DV") v_codidv = row["valor"].ToString().Trim();           // codigo de dov.vta en tabla TDV
                        if (row["campo"].ToString() == "codinNC" && row["param"].ToString() == "NC") v_codinc = row["valor"].ToString().Trim();           // codigo de nota de credito en tabla TDV
                        if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "facelect")
                    {
                        if (row["campo"].ToString() == "factelect")
                        {
                            if (row["param"].ToString() == "usuarioInteg") usuaInteg = row["valor"].ToString().Trim();     // usuario de la integración con Seencorp
                            if (row["param"].ToString() == "claveInteg") clavInteg = row["valor"].ToString().Trim();        // clave del usuario de la integración con Seencorp
                            if (row["param"].ToString() == "ose-pse") nipfe = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "textaut") restexto = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "autoriz") autoriz_OSE_PSE = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "despedi") despedida = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "webose") webose = row["valor"].ToString().Trim();

                        }
                    }
                    if (row["formulario"].ToString() == "guiati_e")
                    {
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "rutaQR") rutaQR = row["valor"].ToString().Trim();           // "C:\temp\"
                            if (row["param"].ToString() == "impA5") nomImp = row["valor"].ToString().Trim();            // nombre de la impresora grafica A4/A5
                        }
                    }

                }
                da.Dispose();
                dt.Dispose();
                // jalamos datos del usuario y local
                v_clu = TransCarga.Program.vg_luse;                // codigo local usuario
                v_slu = lib.serlocs(v_clu);                        // serie local usuario
                v_nbu = TransCarga.Program.vg_nuse;                // nombre del usuario
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
                    parte = "where a.tipnota=@tnot and a.sernota=@snot and a.numnota=@nnot";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {   //      a.martdve,
                    string consulta = "select a.id,a.fechope,a.tipdvta,a.serdvta,a.numdvta,b.descrizionerid as nomest,a.martnot,a.numnota,a.tipncred," +
                        "a.tipnota,a.sernota,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt,a.ubigclt,a.corrclt,a.teleclt," +
                        "a.locorig,a.dirorig,a.ubiorig,a.obsdvta,a.mondvta,a.tcadvta,a.subtota,a.igvtota,a.porcigv,a.totnota,a.totdvta,a.saldvta," +
                        "a.subtMN,a.igvtMN,a.totdvMN,a.codMN,a.estnota,a.frase01,a.impreso,a.tipncred,a.canfidt,c.descrizionerid as docC,f.fechope as femiFT," +
                        "a.verApp,a.userc,a.fechc,a.userm,a.fechm,a.usera,a.fecha,c.codsunat " +
                        "from cabdebcred a " +
                        "left join cabfactu f on f.tipdvta=a.tipdvta and f.serdvta=a.serdvta and f.numdvta=a.numdvta " +
                        "left join desc_est b on b.idcodice=a.estnota " +
                        "left join desc_doc c on c.idcodice=a.tidoclt " +
                        parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    if (campo == "tx_idr")
                    {
                        micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    }
                    if (campo == "sernum")
                    {
                        micon.Parameters.AddWithValue("@tnot", tx_dat_tnota.Text);
                        micon.Parameters.AddWithValue("@snot", tx_serie.Text);
                        micon.Parameters.AddWithValue("@nnot", tx_numero.Text);
                    }
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr != null)
                    {
                        if (dr.Read())
                        {
                            tx_idr.Text = dr.GetString("id");
                            tx_fechope.Text = dr.GetString("fechope").Substring(0, 10);
                            if (dr.GetString("tipncred") == "ANU") rb_anula.Checked = true;
                            else rb_anula.Checked = false;
                            tx_dat_tnota.Text = dr.GetString("tipnota");
                            tx_serie.Text = dr.GetString("sernota");
                            tx_numero.Text = dr.GetString("numnota");
                            tx_dat_tdRem.Text = dr.GetString("tidoclt");
                            tx_nomtdc.Text = dr.GetString("docC");
                            tx_numDocRem.Text = dr.GetString("nudoclt");
                            tx_nomRem.Text = dr.GetString("nombclt");
                            tx_dirRem.Text = dr.GetString("direclt");
                            tx_dptoRtt.Text = dr.GetString("dptoclt");
                            tx_provRtt.Text = dr.GetString("provclt");
                            tx_distRtt.Text = dr.GetString("distclt");
                            tx_email.Text = dr.GetString("corrclt");
                            //tx_telc1.Text = dr.GetString("teleclt");
                            //locorig,dirorig,ubiorig
                            tx_obser1.Text = dr.GetString("obsdvta");
                            tx_tfil.Text = dr.GetString("canfidt");
                            //tx_totcant.Text = dr.GetString("canbudt");  // total bultos
                            tx_dat_mone.Text = dr.GetString("mondvta");
                            tx_tipcam.Text = dr.GetString("tcadvta");
                            tx_subt.Text = Math.Round(dr.GetDecimal("subtota"), 2).ToString();
                            tx_igv.Text = Math.Round(dr.GetDecimal("igvtota"), 2).ToString();
                            //,,,porcigv
                            tx_flete.Text = Math.Round(dr.GetDecimal("totdvta"), 2).ToString();           // total inc. igv
                            tx_pagado.Text = dr.GetString("totnota");
                            //tx_salxcob.Text = dr.GetString("saldvta");
                            tx_dat_estad.Text = dr.GetString("estnota");        // estado
                            tx_impreso.Text = dr.GetString("impreso");
                            //tx_idcob.Text = dr.GetString("cobra");              // id de cobranza
                            tx_dat_tdv.Text = dr.GetString("tipdvta");
                            cmb_tdv.SelectedValue = tx_dat_tdv.Text;
                            cmb_tdv_SelectedIndexChanged(null, null);
                            tx_serGR.Text = dr.GetString("serdvta");
                            tx_numGR.Text = dr.GetString("numdvta");       // al cambiar el indice en el combox se borra numero, por eso lo volvemos a jalar
                            cmb_mon.SelectedValue = tx_dat_mone.Text;
                            tx_estado.Text = dr.GetString("nomest");   // lib.nomstat(tx_dat_estad.Text);
                            if (dr.GetString("userm") == "") tx_digit.Text = lib.nomuser(dr.GetString("userc"));
                            else tx_digit.Text = lib.nomuser(dr.GetString("userm"));
                            tx_fecemi.Text = dr.GetString("femiFT").Substring(0, 10);
                            tx_fletLetras.Text = numLetra.Convertir(tx_flete.Text, true) + " " + tx_dat_nomon.Text;
                            tx_dat_tdsunat.Text = dr.GetString("codsunat");
                            tx_dat_inot.Text = "C";
                        }
                        else
                        {
                            MessageBox.Show("No existe el número de la nota!", "Atención - dato incorrecto",
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
                }
                conn.Close();
            }
        }
        private void jaladet(string idr)         // jala el detalle
        {
            string jalad = "select a.filadet,a.codgror,a.cantbul,a.unimedp,a.descpro,a.pesogro,a.codmogr,a.totalgr,ifnull(b.fechopegr,''),ifnull(b.docsremit,'') " +
                "from detdebcred a left join cabguiai b on concat(b.sergui,'-',b.numgui)=a.codgror where a.idc=@idr";
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
                                row[1].ToString(),
                                row[4].ToString(),
                                row[2].ToString(),
                                row[6].ToString(),
                                row[7].ToString(),
                                null,
                                null,
                                (row[8].ToString() == "") ? "" : row[8].ToString().Substring(6, 4) + "-" + row[8].ToString().Substring(3, 2) + "-" + row[8].ToString().Substring(0, 2),
                                row[9].ToString(),
                                row[6].ToString());
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
                        var aa = MessageBox.Show(ex.Message + Environment.NewLine + "No se pudo conectar con el servidor" + Environment.NewLine +
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
                string tcon = "select distinct a.idcodice,a.descrizionerid,a.enlace1,a.codsunat,b.glosaser,a.deta1 " +
                    "from desc_tdv a LEFT JOIN series b ON b.tipdoc = a.IDCodice where numero=@bloq and codigo=@codv";  //  or codigo=@conc
                using (MySqlCommand cdv = new MySqlCommand(tcon, conn))
                {
                    cdv.Parameters.AddWithValue("@bloq", 1);
                    cdv.Parameters.AddWithValue("@codv", v_codidv);
                    //cdv.Parameters.AddWithValue("@conc", v_codinc);
                    using (MySqlDataAdapter datv = new MySqlDataAdapter(cdv))
                    {
                        dttd1.Clear();
                        datv.Fill(dttd1);
                        cmb_tdv.DataSource = dttd1;
                        cmb_tdv.DisplayMember = "descrizionerid";
                        cmb_tdv.ValueMember = "idcodice";
                    }
                }
                // datos para combo notas cred/deb
                cmb_tnota.Items.Clear();
                using (MySqlCommand cdv = new MySqlCommand("select distinct a.idcodice,a.descrizionerid,a.enlace1,a.codsunat,b.glosaser,a.deta1 from desc_tdv a LEFT JOIN series b ON b.tipdoc = a.IDCodice where numero=@bloq and codigo=@codn", conn))
                {
                    cdv.Parameters.AddWithValue("@bloq", 1);
                    cdv.Parameters.AddWithValue("@codn", "nota");
                    using (MySqlDataAdapter datv = new MySqlDataAdapter(cdv))
                    {
                        dttdn.Clear();
                        datv.Fill(dttdn);
                        cmb_tnota.DataSource = dttdn;
                        cmb_tnota.DisplayMember = "descrizionerid";
                        cmb_tnota.ValueMember = "idcodice";
                    }
                }
                // datos para el combo de moneda
                cmb_mon.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid,codsunat,deta1 from desc_mon where numero=@bloq", conn))
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
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de Doc.Venta ANULADA");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de Doc.Venta GENERADA/NUEVA");
                retorna = false;
            }
            if (MonDeft == "")          // moneda por defecto
            {
                lib.messagebox("Moneda por defecto");
                retorna = false;
            }
            if (v_slu == "")            // serie del local del usuario
            {
                lib.messagebox("Serie general local del usuario");
                retorna = false;
            }
            if (vint_A0 == "")
            {
                lib.messagebox("Código interno enlace anulación BD - A0");
                retorna = false;
            }
            return retorna;
        }
        private bool validGR()                  // validamos y devolvemos datos
        {
            bool retorna = false;
            if (tx_dat_tdv.Text != "" && tx_serGR.Text != "" && tx_numGR.Text != "")
            {
                // validamos que la Factura: 1.exista, 2.No este anulada
                // y devolvemos una fila con los datos del cliente y otra con los datos de la factura
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    if (lib.procConn(conn) == true)
                    {
                        string consulta = "select a.fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt,a.ubigclt," +
                            "a.corrclt,a.teleclt,a.mondvta,a.subtota,a.igvtota,a.porcigv,a.totdvta,a.saldvta,a.subtMN,a.igvtMN,a.totdvMN,b.descrizionerid as docC,b.codsunat " +
                            "from cabfactu a left join desc_doc b on b.idcodice=a.tidoclt " +
                            "WHERE a.tipdvta = @tdv AND a.serdvta = @ser AND a.numdvta = @num AND a.estdvta<> @coda";
                        using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                        {
                            micon.Parameters.AddWithValue("@tdv", tx_dat_tdv.Text);
                            micon.Parameters.AddWithValue("@ser", tx_serGR.Text);
                            micon.Parameters.AddWithValue("@num", tx_numGR.Text);
                            micon.Parameters.AddWithValue("@coda", codAnul);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    tx_nomtdc.Text = dr.GetString("docC");
                                    tx_dat_tdRem.Text = dr.GetString("tidoclt");
                                    tx_dat_tdsunat.Text = dr.GetString("codsunat");
                                    tx_numDocRem.Text = dr.GetString("nudoclt");
                                    tx_nomRem.Text = dr.GetString("nombclt");
                                    tx_dirRem.Text = dr.GetString("direclt");
                                    tx_dptoRtt.Text = dr.GetString("dptoclt");
                                    tx_provRtt.Text = dr.GetString("provclt");
                                    tx_distRtt.Text = dr.GetString("distclt");
                                    tx_email.Text = dr.GetString("corrclt");
                                    tx_fecemi.Text = dr.GetDateTime("fechope").ToString("dd/MM/yyyy");
                                    tx_dat_mone.Text = dr.GetString("mondvta");
                                    tx_flete.Text = dr.GetString("totdvta");
                                    tx_igv.Text = dr.GetString("igvtota");
                                    tx_subt.Text = dr.GetString("subtota");
                                    tx_salxcob.Text = "";   // esta por verse como calculo el saldo de la factura
                                    //
                                    cmb_mon.SelectedValue = tx_dat_mone.Text;
                                    retorna = true;
                                }
                            }
                        }
                        consulta = "SELECT a.codgror,a.cantbul,a.unimedp,a.descpro,a.totalgr,a.codMN,a.totalgrMN,a.codmovta " +
                            "FROM detfactu a WHERE a.tipdocvta=@tdv AND a.serdvta=@ser AND a.numdvta=@num AND estadoser<>@coda";
                        using (MySqlCommand midet = new MySqlCommand(consulta, conn))
                        {
                            midet.Parameters.AddWithValue("@tdv", tx_dat_tdv.Text);
                            midet.Parameters.AddWithValue("@ser", tx_serGR.Text);
                            midet.Parameters.AddWithValue("@num", tx_numGR.Text);
                            midet.Parameters.AddWithValue("@coda", codAnul);
                            using (MySqlDataAdapter da = new MySqlDataAdapter(midet))
                            {
                                dtu.Clear();
                                da.Fill(dtu);
                                foreach (DataRow row in dtu.Rows)
                                {
                                    dataGridView1.Rows.Add(row[0], row[3], row[1], row[7], row[4], row[6], row[5], "", "", row[7]);
                                }
                            }
                        }
                        cmb_mon_SelectedIndexChanged(null, null);
                    }
                }
            }
            return retorna;
        }
        private bool validnota()                // validamos que el documento tenga nota de credito, tiene=true, no tiene=false
        {
            bool retorna = true;
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    string consulta = "select count(id) from cabdebcred where tipdvta=@tipo and serdvta=@serd and numdvta=@nume";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@tipo", tx_dat_tdv.Text);
                        micon.Parameters.AddWithValue("@serd", tx_serGR.Text);
                        micon.Parameters.AddWithValue("@nume", tx_numGR.Text);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr.GetInt16(0) > 0) retorna = true;
                                else retorna = false;
                            }
                        }
                    }
                }
            }
            return retorna;
        }
        private void tipcambio(string codmod)                // funcion para calculos con el tipo de cambio
        {
            decimal totflet = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null)
                {
                    totflet = totflet + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString()); // VALOR DE LA GR EN MONEDA LOCAL
                }
            }
            // si codmod es moneda local, suma campos totales de moneda local y retorna valor
            if (codmod == MonDeft)
            {
                tx_flete.Text = totflet.ToString("#0.00");
            }
            else
            {
                if (codmod != "")
                {
                    vtipcam vtipcam = new vtipcam(tx_tfmn.Text, codmod, DateTime.Now.Date.ToString());
                    var result = vtipcam.ShowDialog();
                    tx_flete.Text = vtipcam.ReturnValue1;
                    tx_fletMN.Text = vtipcam.ReturnValue2;
                    tx_tipcam.Text = vtipcam.ReturnValue3;
                    tx_flete_Leave(null, null);
                }
            }
        }
        private void calculos(string letra, decimal totDoc)
        {
            decimal tigv = 0;
            decimal tsub = 0;
            if (totDoc > 0)
            {
                tsub = Math.Round(totDoc / (1 + decimal.Parse(v_igv) / 100), 2);
                tigv = Math.Round(totDoc - tsub, 2);

            }
            if (letra == "V")
            {
                tx_igv.Text = tigv.ToString("#0.00");
                tx_subt.Text = tsub.ToString("#0.00");
            }
            if (letra == "N")
            {
                tx_igvNot.Text = tigv.ToString("#0.00");
                tx_subtNot.Text = tsub.ToString("#0.00");
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
        private void llena_matris_FE()
        {
            DataRow[] row = dttd1.Select("idcodice='" + tx_dat_tnota.Text + "'");             // tipo de documento venta
            tipdo = row[0][3].ToString();
            DataRow[] rowm = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");         // tipo de moneda
            tipoMoneda = rowm[0][2].ToString().Trim();
            // 
            vs[0] = cmb_tdv.Text.Substring(0, 1) + "C" + lib.Right(tx_serie.Text, 2);      // dr.GetString("martdve") + lib.Right(serie, 3);
            vs[1] = tx_numero.Text;                                                 // numero;
            vs[2] = cmb_tdv.Text.Substring(0, 1) + "C";                             // tipo;
            vs[3] = Program.dirfisc;                                                // direccion emisor
            vs[4] = "Nota de crédito electrónica";
            vs[5] = tx_fechope.Text;                                                // dr.GetString("fechope");
            vs[6] = tx_nomRem.Text;                                                 // dr.GetString("nombclt");
            vs[7] = tx_numDocRem.Text;                                              // dr.GetString("nudoclt");
            vs[8] = tx_dirRem.Text;                                                 // dr.GetString("direclt");
            vs[9] = tx_distRtt.Text;                                                // dr.GetString("distclt");
            vs[10] = tx_provRtt.Text;                                               // dr.GetString("provclt");
            vs[11] = tx_dptoRtt.Text;                                               // dr.GetString("dptoclt");
            vs[12] = tx_tfil.Text;      // tx_totcant.Text;                                               // dr.GetString("canfidt");
            vs[13] = tx_subt.Text;                                                  // dr.GetString("subtota");
            vs[14] = tx_igv.Text;                                                   // dr.GetString("igvtota");
            vs[15] = tx_flete.Text;                                                 // dr.GetString("totdvta");
            vs[16] = tipoMoneda;                                                  // dr.GetString("inimon");
            vs[17] = tx_fletLetras.Text.Trim();                                     // + ((dr.GetString("mondvta") == codmon) ? " SOLES" : " DOLARES AMERICANOS");
            vs[18] = "";
            vs[19] = "";
            vs[20] = "";
            vs[21] = cmb_tdv.Text.Substring(0, 1) + "C";                            // dr.GetString("cdtdv");
            vs[22] = "";                                                            // dr.GetString("ctdcl");
            vs[23] = nipfe;                                                         // identificador de ose/pse metodo de envío
            vs[24] = restexto;                                                      // texto del resolucion sunat del ose/pse
            vs[25] = autoriz_OSE_PSE;                                               // dr.GetString("autorizPSE");
            vs[26] = webose;                                                        // dr.GetString("webosePSE");
            vs[27] = tx_digit.Text;                                                 // dr.GetString("userc").Trim();
            vs[28] = Program.vg_nlus;                                               // dr.GetString("nomLocO").Trim();
            vs[29] = despedida;                                                     // glosa despedida
            vs[30] = Program.cliente;                                               // nombre del emisor del comprobante
            vs[31] = Program.ruc;                                                   // ruc del emisor
            vs[32] = "Anulación de la Operación";                                   // tipo de nota
            vs[33] = "Anulación de la Operación";                                   // motivo para hacer la nota
            vs[34] = "Transporte Privado";          // modalidad de transporte
            vs[35] = "Venta";                       // motivo de traslado
            vs[36] = tipoMoneda;                    // dr.GetString("nonmone");
            vs[37] = tx_fecemi.Text;                // fecha emision del comprobante que se anula
            vs[38] = cmb_tdv.Text.Substring(0, 1) + lib.Right(tx_serGR.Text, 3) + "-" + tx_numGR.Text;                           // comprobante que se anula
            // varios
            va[0] = logoclt;                    // Ruta y nombre del logo del emisor electrónico
            va[1] = ""; // glosser;                    // glosa del servicio en facturacion
            va[2] = ""; // codfact;                    // Tipo de documento FACTURA
            va[3] = ""; // Program.pordetra;           // porcentaje detracción
            va[4] = ""; // (double.Parse(tx_fletMN.Text) * double.Parse(Program.pordetra) / 100).ToString("#0.00");         // monto detracción
            va[5] = ""; // Program.ctadetra;           // cta. detracción
            va[6] = "";                         // concatenado de Guias Transportista para Formato de cargas unicas
            va[7] = rutaQR + "pngqr";           // ruta y nombre del png codigo QR va[7]
            va[8] = rutaQR + Program.ruc + "-" + tipdo + "-" + vs[0] + "-" + vs[1] + ".pdf";                // ruta y nombre del pdf a subir a seencorp
            va[9] = tx_tipcam.Text;
            // detalle
            // a.codgror,a.descpro,a.cantbul,'',a.totalgr,'','',ifnull(b.fechopegr,''),a.codmogr,   a.unimedp,a.pesogro,ifnull(b.docsremit,'')
            for (int l = 0; l < dataGridView1.Rows.Count - 1; l++)
            {
                if (!string.IsNullOrEmpty(dataGridView1.Rows[l].Cells[0].Value.ToString()))   //  dataGridView1.Rows[l].Cells[0].Value != null
                {
                    decimal pu = Math.Round(decimal.Parse(dataGridView1.Rows[l].Cells[4].Value.ToString()), 2);
                    decimal vu = decimal.Parse(dataGridView1.Rows[l].Cells[4].Value.ToString());
                    vu = Math.Round(vu / (1 + decimal.Parse(v_igv) / 100), 2);

                    dt[l, 0] = (l + 1).ToString();
                    dt[l, 1] = dataGridView1.Rows[l].Cells[2].Value.ToString();     // drg.GetString("cantbul"); 
                    dt[l, 2] = "";     // drg.GetString("unimedp");
                    dt[l, 3] = "";     // drg.GetString("codgror");
                    dt[l, 4] = dataGridView1.Rows[l].Cells[1].Value.ToString();     // drg.GetString("descpro");
                    dt[l, 5] = "";     // drg.GetString("docsremit");
                    dt[l, 6] = vu.ToString();     // drg.GetString("valUni");
                    dt[l, 7] = pu.ToString();     // drg.GetString("preUni");
                    dt[l, 8] = pu.ToString();     // drg.GetString("totalgr");
                }
            }
        }

        #region facturacion electronica
        private bool factElec(string provee, string tipo, string accion, int ctab, string fechOp)                 // conexion a facturacion electrónica provee=proveedor | tipo=txt ó json
        {
            bool retorna = false;

            DataRow[] ron = dttdn.Select("idcodice='" + tx_dat_tnota.Text + "'");               // nota de credito
            tipdo = ron[0][3].ToString();
            string serie = cmb_tdv.Text.Substring(0, 1) + tx_dat_inot.Text.Trim() + lib.Right(tx_serie.Text, 2);
            string corre = tx_numero.Text;

            DataRow[] row = dttd1.Select("idcodice='" + tx_dat_tdv.Text + "'");                     // documento venta
            string tipdv = row[0][3].ToString();                                            // tipo comprobante
            string serdv = cmb_tdv.Text.Substring(0, 1) + lib.Right(tx_serGR.Text, 3);      // serie del comprobante
            string numdv = tx_numGR.Text;                                                   // numero del comprobante
            tipoDocEmi = tx_dat_tdsunat.Text;                                               // codigo sunat tipo comprob

            DataRow[] rowm = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");         // tipo de moneda
            tipoMoneda = rowm[0][2].ToString().Trim();
            //
            string ctnota = "01";                                                       // tipo de nota de credito 01=anulacion
            string ntnota = "Anulación de la operación";                                // nombre del tipo de nota
            string fedoco = tx_fecemi.Text.Substring(6, 4) + "-" +
                tx_fecemi.Text.Substring(3, 2) + "-" + tx_fecemi.Text.Substring(0, 2);  // fecha del documento que se anula

            llena_matris_FE();

            if (provee == "factDirecta")
            {
                if (accion == "alta")
                {
                    string aZip = "";
                    string aXml = "";
                    if (llenaTablaLiteNC(tipdo, tipoMoneda, tipoDocEmi) != true)
                    {
                        MessageBox.Show("No se pudo llenar las tablas sqlite", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        aXml = Program.ruc + "-" + tipdo + "-" + cmb_tdv.Text.Substring(0, 1) + lib.Right(tx_serie.Text, 3) + "-" + tx_numero.Text + ".xml";
                        aZip = Program.ruc + "-" + tipdo + "-" + cmb_tdv.Text.Substring(0, 1) + lib.Right(tx_serie.Text, 3) + "-" + tx_numero.Text + ".zip";
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
                        Byte[] respuesta = null;
                        try
                        {
                            ServiceRefSunat.billServiceClient ws = new ServiceRefSunat.billServiceClient();
                            ws.Open();
                            respuesta = ws.sendBill(aZip, bytexml, "");
                            ws.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error al enviar a Sunat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (File.Exists(rutaxml + "R-" + aZip) == true)
                        {
                            File.Delete(rutaxml + "R-" + aZip);
                        }
                        FileStream fstrm = new FileStream(rutaxml + "R-" + aZip, FileMode.CreateNew, FileAccess.Write);
                        fstrm.Write(respuesta, 0, respuesta.Length);
                        fstrm.Close();
                        // 1) tenemos que abrir el zip, 
                        // 2) leer el xml y obtener:
                        //      <cbc:ID></cbc:ID>
                        //      <cac:DocumentResponse><cac:Response>
                        //          <cbc:ReferenceID>F002-00009074</cbc:ReferenceID>
                        //          <cbc:ResponseCode>0</cbc:ResponseCode>
                        //          <cbc:Description>La Factura numero F002-00009074, ha sido aceptada</cbc:Description>
                        // 3) grabar los valores obtenidos en la tabla de estados

                        if (!Directory.Exists(@"c:/temp/"))
                        {
                            Directory.CreateDirectory(@"c:/temp/");
                        }

                        System.IO.Compression.ZipFile.ExtractToDirectory(rutaxml + "R-" + aZip, @"c:/temp/");        // @"c:/temp/temporal.zip", @"c:/temp/"
                        FileStream archiS = new FileStream(@"c:/temp/" + "R-" + aXml, FileMode.Open, FileAccess.Read);        // @"c:/temp/" + archi, FileMode.Open, FileAccess.Read
                        XmlDocument archiXml = new XmlDocument();
                        archiXml.Load(archiS);
                        XmlNode idx = archiXml.GetElementsByTagName("ID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2").Item(0);
                        XmlNode fex = archiXml.GetElementsByTagName("ResponseDate", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2").Item(0);
                        XmlNode hex = archiXml.GetElementsByTagName("ResponseTime", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2").Item(0);
                        string fhx = fex.InnerText.ToString() + " " + hex.InnerText.ToString();
                        XmlNode fqr = archiXml.GetElementsByTagName("DocumentResponse", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2").Item(0);
                        archiS.Close();
                        File.Delete(@"c:/temp/" + "R-" + aXml);     // borramos el xml del temporal
                        string res2 = "", res3 = "";
                        if (fqr == null)
                        {
                            XmlNode fer = archiXml.GetElementsByTagName("Description", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2").Item(0);
                            // esta parte falta .. cuando el comprobante es rechazado 07/09/23
                            // ...........
                            // acá me quede 11/09/2023
                            // .............
                        }
                        else
                        {
                            //res1 = fqr.FirstChild.ChildNodes.Item(0).InnerText; // <cbc:ReferenceID>F002-00009074</cbc:ReferenceID>
                            res2 = fqr.FirstChild.ChildNodes.Item(1).InnerText; // <cbc:ResponseCode>0</cbc:ResponseCode>
                            res3 = fqr.FirstChild.ChildNodes.Item(2).InnerText; // <cbc:Description>La Factura numero F002-00009074, ha sido aceptada</cbc:Description>
                        }
                        // aca debemos saber si es un NUEVO registro o es una EDICION regenerando el XML
                        if (Tx_modo.Text == "NUEVO")
                        {
                            string actua = "";
                            /*
                            if (chk_cunica.Checked == false)
                            {
                                // insertamos
                                actua = "insert into adifactu (idc,nticket,fticket,estadoS,cdr,cdrgener,textoQR) values (@idc,@nti,@fti,@est,@cdrt,@cdrg,@tqr)";
                            }
                            else
                            */
                            {
                                // actualizamos los campos de la tabla
                                actua = "update adifactu set nticket=@nti,fticket=@fti,estadoS=@est,cdr=@cdrt,cdrgener=@cdrg,textoQR=@tqr " +
                                    "where idc=@idc";
                            }
                            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                            {
                                conn.Open();
                                using (MySqlCommand micon = new MySqlCommand(actua, conn))
                                {
                                    micon.Parameters.AddWithValue("@idc", tx_idr.Text);
                                    micon.Parameters.AddWithValue("@nti", idx.InnerText.ToString());
                                    micon.Parameters.AddWithValue("@fti", fhx);
                                    micon.Parameters.AddWithValue("@est", (res2 == "0") ? "Aceptado" : "Rechazado");
                                    micon.Parameters.AddWithValue("@cdrt", respuesta);
                                    micon.Parameters.AddWithValue("@cdrg", res2);
                                    micon.Parameters.AddWithValue("@tqr", res3);
                                    micon.ExecuteNonQuery();
                                }
                            }
                        }
                        if (Tx_modo.Text == "EDITAR")
                        {
                            // actualizamos los campos de la tabla 
                            string actua = "update adifactu set nticket=@nti,fticket=@fti,estadoS=@est,cdr=@cdrt,cdrgener=@cdrg,textoQR=@tqr " +
                                    "where idc=@idc";
                            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                            {
                                conn.Open();
                                using (MySqlCommand micon = new MySqlCommand(actua, conn))
                                {
                                    micon.Parameters.AddWithValue("@idc", tx_idr.Text);
                                    micon.Parameters.AddWithValue("@nti", idx.InnerText.ToString());
                                    micon.Parameters.AddWithValue("@fti", fhx);
                                    micon.Parameters.AddWithValue("@est", (res2 == "0") ? "Aceptado" : "Rechazado");
                                    micon.Parameters.AddWithValue("@cdrt", respuesta);
                                    micon.Parameters.AddWithValue("@cdrg", res2);
                                    micon.Parameters.AddWithValue("@tqr", res3);
                                    micon.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                }
            }
            return retorna;
        }
        static private void CreaTablaLiteNC()                  // llamado en el load del form, crea las tablas al iniciar
        {
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                string sqlborra = "DROP TABLE IF EXISTS dt_cabnc; DROP TABLE IF EXISTS dt_detnc";
                using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                {
                    cmdB.ExecuteNonQuery();
                }
                string sqlTabla = "create table dt_cabnc (" +
                    // cabecera
                    "id integer primary key autoincrement, " +
                    "EmisRuc varchar(11), " +           // ruc del emisor               - 31
                    "EmisNom varchar(150), " +          // Razón social del emisor      - 30
                    "EmisCom varchar(150), " +          // Nombre Comercial del emisor  - 
                    "CodLocA varchar(4), " +            // Código local anexo emisor    - 
                    "EmisUbi varchar(6), " +            // ubigeo del emisor
                    "EmisDir varchar(200), " +          // direccion fiscal             - 3
                    "EmisDep varchar(50), " +           // departamento del emisor      - 
                    "EmisPro varchar(50), " +           // provincia                    - 
                    "EmisDis varchar(50), " +           // distrito                     - 
                    "EmisUrb varchar(50), " +           // urbanización, localidad      - 
                    "EmisPai varchar(2), " +            // código sunat país emisor     - 
                    "EmisCor varchar(100), " +          // correo del emisor            - 
                    "EmisTel varchar(11), " +           // teléfono del emisor          - 
                    "EmisTDoc varchar(1), " +           // codigo tip doc sunat emisor  - 
                    "SeriNot varchar(4), " +            // serie completa de la nota    - 0
                    "NumeNot varchar(8), " +            // numero de la nota            - 1
                    "TipoNot varchar(2), " +            // Tipo FC o BC                 - 2
                    "NumNotC varchar(12), " +           // serie+numero                 - 0 + 1
                    "IdenNot varchar(100), " +          // nombre identificatorio       - 4
                    "FecEmis varchar(10), " +           // fecha emision de la nota     - 5
                    "HorEmis varchar(8), " +
                    "TipDocu varchar(2), " +            // SUNAT: Tipo de Documento     - 
                    "CodLey1 varchar(4), " +            // cod sunat leyenda MONTO EN LETRAS -
                    "MonLetr varchar(150), " +          // monto en letras              - 17
                    "CodMonS varchar(3), " +            // código sunat de moneda       - 36
                    "NtipNot varchar(50), " +           // nombre del tipo de nota      - 32
                    // datos del cliente
                    "DstTipdoc varchar(2), " +          // cód sunat tip doc cliente    -  
                    "DstNumdoc varchar(11), " +         // número doc cliente           - 7
                    "DstNombre varchar(150), " +        // nombre del cliente           - 6
                    "DstDirecc varchar(200), " +        // dirección del destinatario   - 8 
                    "DstDepart varchar(50), " +         // departamento cliente         - 11
                    "DstProvin varchar(50), " +         // provincia del cliente        - 10
                    "DstDistri varchar(50), " +         // distrito cliente             - 9
                    "DstUrbani varchar(50), " +         // urbanización, localidad      - 
                    "DstUbigeo varchar(6), " +          // ubigeo direc cliente         - 
                    "DstCorre varchar(100), " +         // correo del cliente           - 
                    "DstTelef varchar(11), " +          // teléfono del cliente         -
                    // Información de importes 
                    "ImpTotImp decimal(12,2), " +       // Monto total de impuestos     - 14
                    "ImpOpeGra decimal(12,2), " +       // Monto operaciones gravadas   - 13
                    "ImpIgvTot decimal(12,2), " +       // Sumatoria de IGV             - 14
                    "ImpOtrosT decimal(12,2), " +       // Sumatoria Otros Tributos     - 14
                    "TotValVta decimal(12,2), " +       // Total valor de venta         - 13
                    "TotPreVta decimal(12,2), " +       // Total precio de venta        - 15
                    "TotDestos decimal(12,2), " +       // Total descuentos             - 
                    "TotOtrCar decimal(12,2), " +       // Total otros cargos           - 
                    "TotaVenta decimal(12,2), " +       // Importe total de la venta    - 15
                    "CanFilDet integer, " +             // Cantidad filas de detalle    - 12
                    "CondPago varchar(10), " +          // Condicion de pago            - 
                    "TipoCamb decimal(8,2), " +         // tipo de cambio               - V9
                    // varios
                    "nipfe varchar(15), " +             // identificador del serv       - 23
                    "restexto varchar(200), " +         // texto resolucion emision     - 24
                    "autoriOP varchar(50), " +          // autorizacion sunat           - 25
                    "webose varchar(200), " +           // web ose/pse                  - 26
                    "userCrea varchar(15), " +          // usuario creador              - 27
                    "nomLocC varchar(50), " +           // nombre del local origen      - 28
                    "desped0 varchar(200), " +          // glosa despedida              - 29
                    "motivoA varchar(200), " +          // motivo de la nota            - 33
                    "modTrans varchar(50), " +          // modalidad de transporte      - 34
                    "motiTras varchar(25), " +          // motivo de traslado           - 35
                    "fecEComp varchar(10), " +          // fecha de emision comprob     - 37
                    "Comprob varchar(13), " +           // comprobante relacionado      - 38
                    "rutLogo varchar(200), " +          // ruta y nombre logo           - V0
                    "rutNomQR varchar(200), " +         // ruta y nombre                - V7
                    "rutNoPdf varvhar(200)" +           // ruta y nombre del pdf        - V8
                ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // ********************* DETALLE ************************ //
                sqlTabla = "create table dt_detnc (" +
                    "id integer primary key autoincrement, " +
                    "Numline integer, " +            // Número de orden del Ítem                            - 
                    "Cantprd integer, " +            // Cantidad y Unidad de medida por ítem                - 
                    "DesDet1 varchar(100), " +      // Descripción detallada                                - 
                    "DesDet2 varchar(100), " +
                    "CodIntr varchar(50), " +       // Código de producto                                   - 
                    "ValPeso real, " +              // peso de la carga, va unido a la unidad de medida en TN
                    "UniMedS varchar(3), " +        // codigo unidad de medida de sunat
                    "ValUnit decimal(12,2), " +     // valor unitario
                    "PreUnit decimal(12,2), " +     // precio unitario
                    "Totfila decimal(12,2)" +       // total fila
                    ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private bool llenaTablaLiteNC(string tipdo, string tipoMoneda, string tipoDocEmi)          // llena tabla con los datos del comprobante y llama al app que crea el xml
        {
            bool retorna = false;
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                string fecemi = vs[5].Substring(6, 4) + "-" + vs[5].Substring(3, 2) + "-" + vs[5].Substring(0, 2);
                string cdvta = vs[0] + "-" + vs[1];

                cnx.Open();
                using (SqliteCommand cmd = new SqliteCommand("delete from dt_cabnc where id>0", cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                using (SqliteCommand cmd = new SqliteCommand("delete from dt_detnc where id>0", cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // CABECERA
                string metela = "insert into dt_cabnc (" +
                    "EmisRuc, EmisNom, EmisCom, CodLocA, EmisUbi, EmisDir, EmisDep, EmisPro, EmisDis, EmisUrb, EmisPai, EmisCor, EmisTel, EmisTDoc," +
                    "SeriNot, NumeNot, TipoNot, NumNotC, IdenNot, FecEmis, HorEmis, TipDocu, CodLey1, MonLetr, CodMonS, NtipNot," +
                    "DstTipdoc, DstNumdoc, DstNombre, DstDirecc, DstDepart, DstProvin, DstDistri, DstUrbani, DstUbigeo, DstCorre, DstTelef," +
                    "ImpTotImp, ImpOpeGra, ImpIgvTot, ImpOtrosT, TotValVta, TotPreVta, TotDestos, TotOtrCar, TotaVenta, CanFilDet, CondPago, TipoCamb," +
                    "nipfe, restexto, autoriOP, webose, userCrea, nomLocC, desped0, motivoA, modTrans, motiTras, fecEComp, Comprob, rutLogo, rutNomQR, rutNoPdf) " +
                    "values (" +
                    "@EmisRuc,@EmisNom,@EmisCom,@CodLocA,@EmisUbi,@EmisDir,@EmisDep,@EmisPro,@EmisDis,@EmisUrb,@EmisPai,@EmisCor,@EmisTel,@EmisTDoc," +
                    "@SeriNot,@NumeNot,@TipoNot,@NumNotC,@IdenNot,@FecEmis,@HorEmis,@TipDocu,@CodLey1,@MonLetr,@CodMonS,@NtipNot," +
                    "@DstTipdoc,@DstNumdoc,@DstNombre,@DstDirecc,@DstDepart,@DstProvin,@DstDistri,@DstUrbani,@DstUbigeo,@DstCorre,@DstTelef," +
                    "@ImpTotImp,@ImpOpeGra,@ImpIgvTot,@ImpOtrosT,@TotValVta,@TotPreVta,@TotDestos,@TotOtrCar,@TotaVenta,@CanFilDet,@CondPago,@TipoCamb," +
                    "@nipfe,@restexto,@autoriOP,@webose,@userCrea,@nomLocC,@desped0,@motivoA,@modTrans,@motiTras,@fecEComp,@Comprob,@rutLogo,@rutNomQR,@rutNoPdf)";
                using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                {
                    // cabecera
                    cmd.Parameters.AddWithValue("@EmisRuc", Program.ruc);                 // "20430100344"
                    cmd.Parameters.AddWithValue("@EmisNom", Program.cliente);             // "J&L Technology SAC"
                    cmd.Parameters.AddWithValue("@EmisCom", "");                          // nombre comercial
                    cmd.Parameters.AddWithValue("@CodLocA", Program.codlocsunat);         // codigo sunat local anexo emisor
                    cmd.Parameters.AddWithValue("@EmisUbi", Program.ubidirfis);           // "070101"
                    cmd.Parameters.AddWithValue("@EmisDir", Program.dirfisc);             // "Calle Sigma Mz.A19 Lt.16 Sector I"
                    cmd.Parameters.AddWithValue("@EmisDep", Program.depfisc);             // "Callao"
                    cmd.Parameters.AddWithValue("@EmisPro", Program.provfis);             // "Callao"
                    cmd.Parameters.AddWithValue("@EmisDis", Program.distfis);             // "Callao"
                    cmd.Parameters.AddWithValue("@EmisUrb", "-");                         // "Bocanegra"
                    cmd.Parameters.AddWithValue("@EmisPai", "PE");                        // país del emisor
                    cmd.Parameters.AddWithValue("@EmisCor", Program.mailclte);            // "neto.solorzano@solorsoft.com"
                    cmd.Parameters.AddWithValue("@EmisTel", Program.telclte1);
                    cmd.Parameters.AddWithValue("@EmisTDoc", "6");
                    // 
                    cmd.Parameters.AddWithValue("@SeriNot", vs[0]);
                    cmd.Parameters.AddWithValue("@NumeNot", vs[1]);
                    cmd.Parameters.AddWithValue("@TipoNot", vs[2]);
                    cmd.Parameters.AddWithValue("@NumNotC", vs[0] + "-" + vs[1]);
                    cmd.Parameters.AddWithValue("@IdenNot", vs[4]);
                    cmd.Parameters.AddWithValue("@FecEmis", vs[5]);
                    cmd.Parameters.AddWithValue("@HorEmis", DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
                    cmd.Parameters.AddWithValue("@TipDocu", tipdo);
                    cmd.Parameters.AddWithValue("@CodLey1", "1000");
                    cmd.Parameters.AddWithValue("@MonLetr", "SON: " + vs[17]);
                    cmd.Parameters.AddWithValue("@CodMonS", vs[36]);
                    cmd.Parameters.AddWithValue("@NtipNot", vs[32]);
                    // 
                    cmd.Parameters.AddWithValue("@DstTipdoc", tipoDocEmi);
                    cmd.Parameters.AddWithValue("@DstNumdoc", vs[7]);
                    cmd.Parameters.AddWithValue("@DstNombre", vs[6]);             // "<![CDATA[" + tx_nomRem.Text + "]]>"  ... no funca
                    cmd.Parameters.AddWithValue("@DstDirecc", vs[8]);    // "<![CDATA[" + tx_dirRem.Text + "]]>"
                    cmd.Parameters.AddWithValue("@DstDepart", vs[11]);
                    cmd.Parameters.AddWithValue("@DstProvin", vs[10]);
                    cmd.Parameters.AddWithValue("@DstDistri", vs[9]);
                    cmd.Parameters.AddWithValue("@DstUrbani", "");
                    cmd.Parameters.AddWithValue("@DstUbigeo", "");
                    cmd.Parameters.AddWithValue("@DstCorre", "");
                    cmd.Parameters.AddWithValue("@DstTelef", "");
                    // 
                    cmd.Parameters.AddWithValue("@ImpTotImp", vs[14]);       // Monto total de impuestos
                    cmd.Parameters.AddWithValue("@ImpOpeGra", vs[13]);      // Monto las operaciones gravadas
                    cmd.Parameters.AddWithValue("@ImpIgvTot", vs[14]);       // Sumatoria de IGV
                    cmd.Parameters.AddWithValue("@ImpOtrosT", "0");               // Sumatoria de Otros Tributos
                    cmd.Parameters.AddWithValue("@TotValVta", vs[13]);      // Total valor de venta                    
                    cmd.Parameters.AddWithValue("@TotPreVta", vs[15]);     // Total precio de venta (incluye impuestos)
                    cmd.Parameters.AddWithValue("@TotDestos", "0");
                    cmd.Parameters.AddWithValue("@TotOtrCar", "0");
                    cmd.Parameters.AddWithValue("@TotaVenta", vs[15]);
                    cmd.Parameters.AddWithValue("@CanFilDet", vs[12]);
                    cmd.Parameters.AddWithValue("@CondPago", "");
                    cmd.Parameters.AddWithValue("@TipoCamb", va[9]);
                    // 
                    cmd.Parameters.AddWithValue("@nipfe", vs[23]);
                    cmd.Parameters.AddWithValue("@restexto", vs[24]);
                    cmd.Parameters.AddWithValue("@autoriOP", vs[25]);
                    cmd.Parameters.AddWithValue("@webose", vs[26]);
                    cmd.Parameters.AddWithValue("@userCrea", vs[27]);
                    cmd.Parameters.AddWithValue("@nomLocC", vs[28]);
                    cmd.Parameters.AddWithValue("@desped0", vs[29]);
                    cmd.Parameters.AddWithValue("@motivoA", vs[33]);
                    cmd.Parameters.AddWithValue("@modTrans", vs[34]);
                    cmd.Parameters.AddWithValue("@motiTras", vs[35]);
                    cmd.Parameters.AddWithValue("@fecEComp", vs[37]);
                    cmd.Parameters.AddWithValue("@Comprob", vs[38]);          // comprobante relacionado
                    cmd.Parameters.AddWithValue("@rutLogo", va[0]);
                    cmd.Parameters.AddWithValue("@rutNomQR", va[7]);
                    cmd.Parameters.AddWithValue("@rutNoPdf", va[8]);
                    cmd.ExecuteNonQuery();
                }
                // DETALLE
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    double preunit = double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                    double valunit = double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) / (1 + (double.Parse(v_igv) / 100));

                    metela = "insert into dt_detnc (" +
                        "Numline,Cantprd,DesDet1,DesDet2,CodIntr,ValUnit,ValPeso,UniMedS,ValUnit,PreUnit,Totfila) " +
                        "values (" +
                        "@Numline,@Cantprd,@DesDet1,@DesDet2,@CodIntr,@ValUnit,@ValPeso,@UniMedS,@ValUnit,@PreUnit,@Totfila)";
                    using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                    {
                        cmd.Parameters.AddWithValue("@Numline", i + 1.ToString());
                        cmd.Parameters.AddWithValue("@Cantprd", dataGridView1.Rows[i].Cells[2].Value.ToString());
                        cmd.Parameters.AddWithValue("@DesDet1", dataGridView1.Rows[i].Cells[1].Value.ToString());
                        cmd.Parameters.AddWithValue("@DesDet2", "");                  //"Dice contener Enseres domésticos"
                        cmd.Parameters.AddWithValue("@CodIntr", "");                  // código del item
                        cmd.Parameters.AddWithValue("@ValUnit", valunit.ToString());  // valor venta  s/igv
                        cmd.Parameters.AddWithValue("@ValPeso", dataGridView1.Rows[i].Cells[14].Value.ToString());
                        cmd.Parameters.AddWithValue("@UniMedS", dataGridView1.Rows[i].Cells[13].Value.ToString());
                        cmd.Parameters.AddWithValue("@PreUnit", preunit.ToString());  // precio venta c/igv
                        cmd.Parameters.AddWithValue("@Totfila", preunit.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }
                // llamada al programa de generación del xml del comprobante
                string rutalocal = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                ProcessStartInfo p = new ProcessStartInfo();   
                p.Arguments = rutaxml + " " + Program.ruc + " " +
                     cdvta + " " +
                    true + " " + rutaCertifc + " " + claveCertif + " " + tipdo;
                p.FileName = @rutalocal + "/xmlDocVta/xmlDocVta.exe";
                var proc = Process.Start(p);
                proc.WaitForExit();
                if (proc.ExitCode == 1) retorna = true;
                else retorna = false;
            }
            return retorna;
        }

        #endregion

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
        }
        private void escribe()
        {
            lp.escribe(this);
            //tx_nomRem.ReadOnly = true;
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
                // validamos que la FT: 1.exista, 2.No este anulada
                if (validGR() == false)
                {
                    MessageBox.Show("La Boleta/Factura no existe o esta anulada", "Error en documento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //tx_numGR.Text = "";
                    initIngreso();
                    cmb_tdv.Focus();     // tx_numGR.Focus();
                    return;
                }
                // validamos que el doc de venta no tenga nota de credito
                if (validnota() == true)
                {
                    MessageBox.Show("La Boleta/Factura YA tiene nota de crédito", "Error en documento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //tx_numGR.Text = "";
                    initIngreso();
                    cmb_tdv.Focus();    //tx_numGR.Focus();
                    return;
                }
                int totfil = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                    {
                        totfil += 1;
                    }
                }
                tx_tfil.Text = totfil.ToString();
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
            if (tx_serGR.Text.Trim() == "")
            {
                tx_serGR.Focus();
                return;
            }
            if (tx_numGR.Text.Trim() == "")
            {
                tx_numGR.Focus();
                return;
            }
            if (tx_dat_mone.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione el tipo de moneda", " Atención ");
                cmb_mon.Focus();
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
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // valida y calcula
                if (tx_pagado.Text.Trim() == "" || tx_pagado.Text.Trim() == "0")
                {
                    MessageBox.Show("No existe valor del documento", " Atención ");
                    tx_pagado.Focus();
                    return;
                }
                if (rb_anula.Checked == false && rb_dscto.Checked == false)
                {
                    MessageBox.Show("Seleccione el tipo de nota", "Atención - seleccione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rb_anula.Focus();
                    return;
                }
                if (tx_dat_mone.Text != MonDeft && tx_tipcam.Text == "" || tx_tipcam.Text == "0")
                {
                    MessageBox.Show("Problemas con el tipo de cambio", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmb_mon.Focus();
                    return;
                }
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            string fecOp = tx_fechope.Text.Substring(6, 4) + tx_fechope.Text.Substring(3, 2) + tx_fechope.Text.Substring(0, 2);
                            if (factElec(nipfe, "txt", "alta", 0, fecOp) == true)
                            {
                                // actualizamos la tabla seguimiento de usuarios
                                string resulta = lib.ult_mov(nomform, nomtab, asd);
                                if (resulta != "OK")
                                {
                                    MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                /*var bb = MessageBox.Show("Desea imprimir el documento?" + Environment.NewLine +
                                    "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (bb == DialogResult.Yes)
                                {
                                    Bt_print.PerformClick();
                                } */
                            }
                            else
                            {
                                MessageBox.Show("No se puede generar la Nota de crédito", "Error en proveedor de Fact.Electrónica");
                                iserror = "si";
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se puede grabar la nota de crédito", "Error en conexión");
                            iserror = "si";
                        }
                    }
                    else
                    {
                        tx_obser1.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Los datos no son nuevos en doc.venta", "Verifique duplicidad", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            if (modo == "EDITAR")   // solo observaciones
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
                if (Program.vg_tius == "TPU001" && Program.vg_nius == "NIV000")
                {
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
                            if (Program.vg_tius == "TPU001" && Program.vg_nius == "NIV000")
                            {
                                aa = MessageBox.Show("Re-genera la nota electrónica?", "Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (aa == DialogResult.Yes)
                                {
                                    string fecOp = tx_fechope.Text.Substring(6, 4) + tx_fechope.Text.Substring(3, 2) + tx_fechope.Text.Substring(0, 2);
                                    if (factElec(nipfe, "txt", "alta", 0, fecOp) == true)
                                    {
                                        // tutto finito !
                                    }
                                    else
                                    {
                                        MessageBox.Show("No se puede generar la Nota de crédito", "Error en proveedor de Fact.Electrónica");
                                        iserror = "si";
                                    }
                                }
                            }
                        }
                        else
                        {
                            tx_serie.Focus();
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
                // no se anulan nota de credito
            }
            if (iserror == "no")
            {
                string resulta = lib.ult_mov(nomform, nomtab, asd);
                if (resulta != "OK")                                        // actualizamos la tabla usuarios
                {
                    MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // debe limpiar los campos y actualizar la grilla
                //initIngreso();          // 04/01/2022, mejor salimos del form cada vez que grabamos
                this.Close();
            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                string inserta = "insert into cabdebcred (" +
                    "fechope,martnot,tipnota,sernota,tipdvta,serdvta,numdvta,tidoclt,nudoclt,nombclt,direclt,dptoclt,provclt,distclt,ubigclt," +
                    "corrclt,teleclt,locorig,dirorig,ubiorig,obsdvta,mondvta,tcadvta,subtota,igvtota,porcigv,totnota,totdvta,saldvta," +
                    "subtMN,igvtMN,totdvMN,codMN,estnota,frase01,impreso,tipncred,canfidt," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                    "@fechop,@mtdvta,@tipnot,@sernot,@ctdvta,@serdv,@numdv,@tdcrem,@ndcrem,@nomrem,@dircre,@dptocl,@provcl,@distcl,@ubicre," +
                    "@mailcl,@telecl,@ldcpgr,@didegr,@ubdegr,@obsprg,@monppr,@tcoper,@stonot,@igvnot,@porcigv,@pagpgr,@totpgr,@salxpa," +
                    "@subMN,@igvMN,@totMN,@codMN,@estpgr,@frase1,@impSN,@tipon,@canfi," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@mtdvta", cmb_tdv.Text.Substring(0, 1) + tx_dat_inot.Text.Trim());
                    micon.Parameters.AddWithValue("@tipnot", tx_dat_tnota.Text);
                    micon.Parameters.AddWithValue("@sernot", tx_serie.Text);
                    //micon.Parameters.AddWithValue("@numnot", tx_numero.Text); // lo hace el trigger de la tabla
                    micon.Parameters.AddWithValue("@ctdvta", tx_dat_tdv.Text);
                    micon.Parameters.AddWithValue("@serdv", tx_serGR.Text);
                    micon.Parameters.AddWithValue("@numdv", tx_numGR.Text);
                    micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@dptocl", tx_dptoRtt.Text);
                    micon.Parameters.AddWithValue("@provcl", tx_provRtt.Text);
                    micon.Parameters.AddWithValue("@distcl", tx_distRtt.Text);
                    micon.Parameters.AddWithValue("@ubicre", "");                               // este dato no hay
                    micon.Parameters.AddWithValue("@mailcl", tx_email.Text);
                    micon.Parameters.AddWithValue("@telecl", "");                               // este dato no hay el form
                    micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);       // local origen
                    micon.Parameters.AddWithValue("@didegr", dirloc);                           // direccion origen
                    micon.Parameters.AddWithValue("@ubdegr", "");                               // este dato no hay en form
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@tcoper", (tx_tipcam.Text.Trim() != "") ? tx_tipcam.Text : "0");                   // TIPO DE CAMBIO
                    micon.Parameters.AddWithValue("@stonot", tx_subtNot.Text);
                    micon.Parameters.AddWithValue("@igvnot", tx_igvNot.Text);
                    micon.Parameters.AddWithValue("@porcigv", v_igv);                           // porcentaje en numeros de IGV
                    micon.Parameters.AddWithValue("@pagpgr", tx_pagado.Text);
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);                    // total inc. igv
                    micon.Parameters.AddWithValue("@salxpa", (tx_salxcob.Text == "") ? "0" : tx_salxcob.Text);
                    micon.Parameters.AddWithValue("@subMN", tx_subMN.Text);
                    micon.Parameters.AddWithValue("@igvMN", tx_igvMN.Text);
                    micon.Parameters.AddWithValue("@totMN", tx_pagoMN.Text);
                    micon.Parameters.AddWithValue("@codMN", MonDeft);                // codigo moneda local
                    micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text);     // estado
                    micon.Parameters.AddWithValue("@frase1", "");                   // no hay nada que poner 19/11/2020
                    micon.Parameters.AddWithValue("@impSN", "N");
                    micon.Parameters.AddWithValue("@tipon", (rb_anula.Checked == true) ? "ANU" : "DES");
                    micon.Parameters.AddWithValue("@canfi", tx_tfil.Text);
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
                string vavo = "select numnota from cabdebcred where martnot=@mtdvta and sernota=@sernot order by id desc limit 1";
                using (MySqlCommand micon = new MySqlCommand(vavo, conn))  // select last_insert_id()
                {
                    micon.Parameters.AddWithValue("@mtdvta", cmb_tdv.Text.Substring(0, 1) + tx_dat_inot.Text.Trim());
                    micon.Parameters.AddWithValue("@sernot", tx_serie.Text);
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //tx_idr.Text = dr.GetString(0);
                            tx_numero.Text = lib.Right("0000000" + dr.GetString(0), 8);
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
                            // idc, filadet, marnota, tipnota, sernota, numnota, martdve, tipdocvta, serdvta, numdvta, codmovta, totdvta,fechope, estadoser
                            string inserd2 = "update detdebcred set " +
                                "codgror=@guia,cantbul=@bult,unimedp=@unim,descpro=@desc,pesogro=@peso,codmogr=@codm,totalgr=@pret,codMN=@cmnn," +
                                "totalgrMN=@tgrmn " +
                                "where idc=@idr and filadet=@fila";
                            using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                            {
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@fila", fila);
                                micon.Parameters.AddWithValue("@guia", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                micon.Parameters.AddWithValue("@bult", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                micon.Parameters.AddWithValue("@unim", "");
                                micon.Parameters.AddWithValue("@desc", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@peso", "0");
                                micon.Parameters.AddWithValue("@codm", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                micon.Parameters.AddWithValue("@pret", dataGridView1.Rows[i].Cells[4].Value.ToString());
                                micon.Parameters.AddWithValue("@cmnn", dataGridView1.Rows[i].Cells[6].Value.ToString());
                                micon.Parameters.AddWithValue("@tgrmn", dataGridView1.Rows[i].Cells[5].Value.ToString());
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
                    if (true)     // EDICION DE CABECERA
                    {
                        string actua = "update cabdebcred a set obsdvta=@obsprg," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE .... no hay 28/10/2020
                        micon.Dispose();
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
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_numero.Text.Trim() != "")
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
            if (tx_flete.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                tx_flete.Text = Math.Round(decimal.Parse(tx_flete.Text), 2).ToString("#0.00");
                //calculos(decimal.Parse((tx_flete.Text.Trim() != "") ? tx_flete.Text : "0"));
                //
                if (tx_dat_mone.Text != MonDeft)
                {
                    if (tx_tipcam.Text == "" || tx_tipcam.Text.Trim() == "0")
                    {
                        MessageBox.Show("Se requiere tipo de cambio");
                        tx_flete.Text = "";
                        tx_flete.Focus();
                        return;
                    }
                    else
                    {
                        tx_fletMN.Text = Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                }
                else
                {
                    tx_fletMN.Text = tx_flete.Text;
                }
            }
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
        private void tx_email_Leave(object sender, EventArgs e)
        {
            if (lib.email_bien_escrito(tx_email.Text) == false)
            {
                MessageBox.Show("Formato no correcto", "Error en correo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tx_email.Text = "";
                tx_email.Focus();
                return;
            }
        }
        private void chk_sinco_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_sinco.Checked == true)
            {
                if (tx_email.Text.Trim() != "") chk_sinco.Checked = false;
                else tx_email.Text = correo_gen;
            }
            else
            {
                if (tx_email.Text.Trim() != "") tx_email.Text = "";
            }
        }
        private void rb_anula_CheckedChanged(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && rb_anula.Checked == true)
            {
                //tx_pagado.Enabled = false;
                //tx_pagado.ReadOnly = true;
                gbox_flete.Enabled = false;
                tx_pagado.Text = tx_flete.Text;
                tx_salxcob.Text = "0.00";
                tx_fletLetras.Text = numLetra.Convertir(tx_flete.Text, true) + " " + tx_dat_nomon.Text;
                tx_flete_Leave(null, null);
                tx_pagado_Leave(null, null);
            }
        }
        private void rb_dscto_CheckedChanged(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && rb_dscto.Checked == true)
            {
                gbox_flete.Enabled = true;
                cmb_mon.Enabled = false;
                tx_flete.ReadOnly = true;
                tx_igv.ReadOnly = true;
                tx_subt.ReadOnly = true;
                tx_pagado.Text = "";
                tx_salxcob.Text = "";
                tx_fletLetras.Text = "";
                tx_pagado.Enabled = true;
                tx_pagado.ReadOnly = false;
                tx_pagado.Focus();
            }
        }
        private void tx_pagado_Leave(object sender, EventArgs e)
        {
            if (tx_pagado.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                tx_pagado.Text = Math.Round(decimal.Parse(tx_pagado.Text), 2).ToString("#0.00");
                tx_salxcob.Text = Math.Round(decimal.Parse(tx_flete.Text) - decimal.Parse(tx_pagado.Text), 2).ToString("#0.00");
                calculos("N", decimal.Parse((tx_pagado.Text.Trim() != "") ? tx_pagado.Text : "0"));
                //
                if (tx_dat_mone.Text != MonDeft)
                {
                    if (tx_tipcam.Text == "" || tx_tipcam.Text.Trim() == "0")
                    {
                        MessageBox.Show("Se requiere tipo de cambio");
                        tx_pagado.Text = "";
                        tx_pagado.Focus();
                        return;
                    }
                    else
                    {
                        tx_pagoMN.Text = Math.Round(decimal.Parse(tx_pagado.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                        tx_subMN.Text = Math.Round(decimal.Parse(tx_subtNot.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                        tx_igvMN.Text = Math.Round(decimal.Parse(tx_igvNot.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                        /*if (Math.Round(decimal.Parse(tx_tfmn.Text), 1) != Math.Round(decimal.Parse(tx_pagoMN.Text), 1))
                        {
                            MessageBox.Show("No coinciden los valores!", "Error en calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tx_pagado.Text = "";
                            tx_pagado.Focus();
                            return;
                        }*/
                    }
                }
                else
                {
                    tx_pagoMN.Text = tx_pagado.Text;
                    tx_subMN.Text = tx_subtNot.Text;
                    tx_igvMN.Text = tx_igvNot.Text;
                }
            }
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
            //escribe();
            sololee();
            cmb_tdv.Enabled = true;
            tx_serGR.Enabled = true;
            tx_numGR.Enabled = true;
            tx_obser1.Enabled = true;

            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            initIngreso();
            tx_numero.ReadOnly = true;
            cmb_tdv.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "EDITAR";                    // solo puede editarse la observacion 13/01/2021
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            tx_numero.Text = "";
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            tx_salxcob.BackColor = Color.White;
            tx_serGR.Focus();
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
                        imprimeA4();
                    }
                    if (vi_formato == "A5")            // Seleccion de formato ... A5
                    {
                        //if (imprimeA5() == true) updateprint("S");
                    }
                    if (vi_formato == "TK")            // Seleccion de formato ... Ticket
                    {
                        //if (imprimeTK() == true) updateprint("S");
                    }
                }
            }
            else
            {
                if (vi_formato == "A4")            // Seleccion de formato ... A4
                {
                    imprimeA4();
                }
                if (vi_formato == "A5")
                {
                    //if (imprimeA5() == true) updateprint("S");
                }
                if (vi_formato == "TK")
                {
                    //if (imprimeTK() == true) updateprint("S");
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
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
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
            if (tx_idr.Text.Trim() != "")
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
        private void cmb_mon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")    //  || Tx_modo.Text == "EDITAR"
            {   // lo de totcant es para accionar solo cuando el detalle de la GR se haya cargado
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    DataRow[] row = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");
                    tx_dat_monsunat.Text = row[0][2].ToString();
                    tx_dat_nomon.Text = row[0][3].ToString();
                    tipcambio(tx_dat_mone.Text);
                    if (tx_flete.Text != "" && tx_flete.Text != "0.00") calculos("V", decimal.Parse(tx_flete.Text));
                    tx_flete.Focus();
                }
            }
        }
        private void cmb_tdv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_tdv.SelectedIndex > -1)
            {
                DataRow[] row = dttd1.Select("idcodice='" + cmb_tdv.SelectedValue.ToString() + "'");
                if (row.Length > 0)
                {
                    tx_dat_tdv.Text = row[0].ItemArray[0].ToString();
                    //tx_dat_tdec.Text = row[0].ItemArray[2].ToString();
                    //glosser = row[0].ItemArray[4].ToString();
                    tx_serGR.Text = "";
                    tx_numGR.Text = "";
                    //tx_dat_inot.Text = 
                }
            }
        }
        private void cmb_tnota_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_tnota.SelectedIndex > -1)
            {
                DataRow[] row = dttdn.Select("idcodice='" + cmb_tnota.SelectedValue.ToString() + "'");
                if (row.Length > 0)
                {
                    tx_dat_tnota.Text = row[0].ItemArray[0].ToString();
                    tx_dat_tdec.Text = row[0].ItemArray[2].ToString();
                    glosser = row[0].ItemArray[4].ToString();
                    tx_dat_inot.Text = row[0].ItemArray[5].ToString();
                    //tx_serie.Text = "";
                    //tx_numero.Text = "";
                }
            }
        }
        #endregion comboboxes

        #region impresion
        private bool imprimeA4()
        {
            bool retorna = false;
            printDocument1.PrinterSettings.PrinterName = nomImp;
            printDocument1.PrinterSettings.Copies = 1;
            printDocument1.Print();
            retorna = true;

            return retorna;
        }
        private bool imprimeA5()
        {
            bool retorna = false;
            //llenaDataSet();                         // metemos los datos al dataset de la impresion
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            return retorna;
        }
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (vs[0] == "") llena_matris_FE();
            //impNota imp = new impNota(1, "", vs, dt, va, cu, "A4", v_CR_NC1, false);    // vistas en pantalla
        }
        private void imprime_A5(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }
        private void imprime_TK(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // no hay formato en TK solo en A4
        }
        private void updateprint(string sn)  // actualiza el campo impreso de la GR = S
        {   // S=si impreso || N=no impreso
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "update ?? set impreso=@sn where id=@idr";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@sn", sn);
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.ExecuteNonQuery();
                }
            }
        }
        #endregion
        private conClie generaReporte(string cristalito)
        {
            conClie NC = new conClie();
            conClie.cNot_credRow cabRow = NC.cNot_cred.NewcNot_credRow();
            // CABECERA
            cabRow.formatoRPT = cristalito;
            cabRow.id = "0";
            /*
            cabRow.serie = vs[0];
            cabRow.numero = vs[1];
            cabRow.tipDoc = vs[2];
            cabRow.dirEmisor = vs[3];
            cabRow.nomTipdoc = vs[4].ToUpper();
            cabRow.fecEmi = vs[5];
            cabRow.nomClte = vs[6];
            cabRow.nDocClte = vs[7];
            cabRow.DirClte = vs[8];
            cabRow.distClte = vs[9];
            cabRow.provClte = vs[10];
            cabRow.depaClte = vs[11];
            cabRow.canfdet = vs[12];
            cabRow.subtotal = vs[13];
            cabRow.igv = vs[14];
            cabRow.total = vs[15];
            cabRow.moneda = vs[16];
            cabRow.fleteLetras = vs[17];
            cabRow.provee = vs[23];
            cabRow.resolTex = vs[24];
            cabRow.autorizSunat = vs[25];
            cabRow.webose = vs[26];
            cabRow.userc = vs[27];
            cabRow.localEmi = vs[28];
            cabRow.glosDesped = vs[29];
            cabRow.nomEmisor = vs[30];    // nombre del emisor del comprobante
            cabRow.rucEmisor = vs[31];    // ruc del emisor
            cabRow.nomMone = vs[36];      // nombre de la moneda
            cabRow.ubicapng = va[7];
            cabRow.glosaTipoNot = vs[32];   // glosa tipo de nota
            cabRow.motivoNota = vs[33];     // motivo de la anulacion
            cabRow.fechcVtaorigen = vs[37];       // fecha emision comprobante que se anula
            cabRow.cVtaorigen = vs[38];           // comprobante que se anula
            */
            NC.cNot_cred.AddcNot_credRow(cabRow);

            // DETALLE
            for (int o = 0; o < int.Parse(vs[12]); o++)
            {
                conClie.cNot_detRow detRow = NC.cNot_det.NewcNot_detRow();
                detRow.id = "0";
                /*
                detRow.OriDest = dt[o, 0];      // ["OriDest"]
                detRow.cant = dt[o, 1];         // ["Cant"]
                detRow.umed = (dt[o, 2].Trim() == "") ? "ZZ" : dt[o, 2];         // ["umed"]
                detRow.guiaT = dt[o, 3];        // guia transportista
                detRow.descrip = dt[o, 4].Trim();      //  + " Según " + dt[o, 5].Trim()   descripcion de la carga
                detRow.docRel1 = dt[o, 5];      // documento relacionado remitente de la guia transportista
                detRow.docRel2 = "";            // 
                detRow.valUnit = dt[o, 6];      // valor unitario
                detRow.preUnit = dt[o, 7];      // precio unitario
                detRow.Total = dt[o, 8];        // total fila
                */
                NC.cNot_det.AddcNot_detRow(detRow);
            }

            return NC;
        }
    }    
}
