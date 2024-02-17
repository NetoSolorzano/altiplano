using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Linq;
using System.Net;
using RestSharp;
using System.Xml;
using System.Xml.Serialization;

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
        string codCanc = "";            // codigo documento cancelado (pagado 100%)
        string MonDeft = "";            // moneda por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        //string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        //string v_cid = "";              // codigo interno de tipo de documento
        string v_fra2 = "";             // frase que va en obs de cobranza cuando se cancela desde el doc.vta.
        string v_sanu = "";             // serie anulacion interna ANU
        string v_mpag = "";             // medio de pago automatico x defecto para las cobranzas
        string v_codcob = "";           // codigo del documento cobranza
        string v_CR_gr_ind = "";        // nombre del formato FT/BV/NV en CR
        string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_codidv = "";           // variable codifo interno de documento de venta en vista TDV
        string codfact = "";            // idcodice de factura
        string codBole = "";            // id codice de Boleta de venta
        string v_igv = "";              // valor igv %
        string v_estcaj = "";           // estado de la caja
        string v_idcaj = "";            // id de la caja actual
        string codAbie = "";            // codigo estado de caja abierta
        string logoclt = "";            // ruta y nombre archivo logo
        string fshoy = "";              // fecha hoy del servidor en formato ansi
        string codppc = "";             // codigo del plazo de pago por defecto para fact a crédito
        string codcont = "";            // codigo plazo contraentrega o efectivo no credito
        string codsuser_cu = "";        // usuarios autorizados a crear Ft de cargas unicas
        int v_cdpa = 0;                 // cantidad de días despues de emitida la fact. en que un usuario normal puede anular
        string vint_gg = "";            // glosa del detalle inicial de la guía "sin verificar contenido"
        string v_habpago = "";          // se habilitan pagos en el formulario o no, default NO
        //
        string rutatxt = "";            // ruta de los txt para la fact. electronica
        string tipdo = "";              // CODIGO SUNAT tipo de documento de venta
        string tipoDocEmi = "";         // CODIGO SUNAT tipo de documento RUC/DNI
        string tipoMoneda = "";         // CODIGO SUNAT tipo de moneda
        string glosdetra = "";          // glosa original para las detracciones en tabla enlaces
        string glosdet = "";            // glosa para las operaciones con detraccion en el txt
        string glosser = "";            // glosa que va en el detalle del doc. de venta
        string glosser2 = "";           // glosa 2 que va despues de la glosa principal
        string restexto = "xxx";        // texto resolucion sunat autorizando prov. fact electronica
        string autoriz_OSE_PSE = "yyy"; // numero resolucion sunat autorizando prov. fact electronica
        string despedida = "";          // texto para mensajes al cliente al final de la impresión del doc.vta. 
        string webose = "";             // direccion web del ose o pse para la descarga del 
        string correo_gen = "";         // correo generico del emisor cuando el cliente no tiene correo propio
        string codusanu = "";           // usuarios que pueden anular fuera de plazo
        string cusdscto = "";           // usuarios que pueden hacer descuentos
        string otro = "";               // ruta y nombre del png código QR
        string caractNo = "";           // caracter prohibido en campos texto, caracter delimitador para los TXT
        string nipfe = "";              // nombre identificador del proveedor de fact electronica
        string glosaAnul = "";          // texto motivo de baja/anulacion en los TXT para el pse/ose
        string tipdocAnu = "";          // Tipos de documentos que se pueden dar de baja
        string tdocsBol = "";           // tipos de documentos de clientes que permiten boletas
        string tdocsFac = "";           // tipos de documentos de clientes que permiten facturas
        string texmotran = "";          // texto modalidad de transporte
        string codtxmotran = "";        // codigo motivo de traslado de bienes
        int ccf_pdf = 0;                // cantidad de caracteres por fila limite para detalle de cargas unicas
        string rucsEmcoper = "";        // rucs separados por comas para el modelo especial de pdf de Emcoper coordinado con PSnet 07/12/2022 
        string webdni = "";             // ruta web del buscador de DNI
        string NoRetGl = "";            // glosa de retorno cuando umasapa no encuentra el dni o ruc
        string rutaxml = "";            // ruta para los XML de las guias de remision
        string client_id_sunat = "";    // id del cliente api sunat para guias electrónicas 
        string scope_sunat = "";        // scope sunat del api
        string client_pass_sunat = "";  // clave api sunat para guias electrónicas
        string u_sol_sunat = "";        // usuario sol sunat del cliente
        string c_sol_sunat = "";        // clave sol sunat del cliente
        string rutaCertifc = "";        // Ruta y nombre del certificado .pfx
        string wsPostS = "";          // Ruta post webservice Sunat
        string claveCertif = "";        // Clave del certificado
        string[] c_t = new string[6] { "", "", "", "", "", "" }; // parametros para generar el token sunat
        //
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string ubiclie = Program.ubidirfis;         // ubigeo direc fiscal
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        string dirloc = TransCarga.Program.vg_duse; // direccion completa del local usuario conectado
        string ubiloc = TransCarga.Program.vg_uuse; // ubigeo local del usuario conectado
        #endregion

        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        acGRE_sunat _Sunat = new acGRE_sunat();

        AutoCompleteStringCollection departamentos = new AutoCompleteStringCollection();// autocompletado departamentos
        AutoCompleteStringCollection provincias = new AutoCompleteStringCollection();   // autocompletado provincias
        AutoCompleteStringCollection distritos = new AutoCompleteStringCollection();    // autocompletado distritos
        DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        static string CadenaConexion = "Data Source=TransCarga.db";

        DataTable dttd0 = new DataTable();
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();        // moneda
        DataTable dtp = new DataTable();        // plazo de credito 
        DataTable tcfe = new DataTable();       // facturacion electronica - cabecera
        DataTable tdfe = new DataTable();       // facturacion electronica -detalle
        string[] datcltsR = { "", "", "", "", "", "", "", "", "" };
        string[] datcltsD = { "", "", "", "", "", "", "", "", "" };
        string[] datguias = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }; // 18
        string[] datcargu = { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };    // 14
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
            armacfe();
            armadfe();
            if (valiVars() == false)
            {
                Application.Exit();
                return;
            }
            if (nipfe == "factDirecta")         // si el generador electrónico es Fact. directa desde sistema del contribuyente
            {
                CreaTablaLiteDV();              // llama al creador de las tablas en sqlite
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
            tx_dptoRtt.AutoCompleteMode = AutoCompleteMode.Suggest;           // departamentos
            tx_dptoRtt.AutoCompleteSource = AutoCompleteSource.CustomSource;  // departamentos
            tx_dptoRtt.AutoCompleteCustomSource = departamentos;              // departamentos
            tx_provRtt.AutoCompleteMode = AutoCompleteMode.Suggest;           // provincias
            tx_provRtt.AutoCompleteSource = AutoCompleteSource.CustomSource;  // provincias
            tx_provRtt.AutoCompleteCustomSource = provincias;                 // provincias
            tx_distRtt.AutoCompleteMode = AutoCompleteMode.Suggest;           // distritos
            tx_distRtt.AutoCompleteSource = AutoCompleteSource.CustomSource;  // distritos
            tx_distRtt.AutoCompleteCustomSource = distritos;                  // distritos
            tx_dp_dep.AutoCompleteMode = AutoCompleteMode.Suggest;            // punto de partida - cargas unicas
            tx_dp_dep.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_dp_dep.AutoCompleteCustomSource = departamentos;
            tx_dp_pro.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_dp_pro.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_dp_pro.AutoCompleteCustomSource = provincias;
            tx_dp_dis.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_dp_dis.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_dp_dis.AutoCompleteCustomSource = distritos;
            tx_dd_dep.AutoCompleteMode = AutoCompleteMode.Suggest;          // punto llegada - cargas unicas
            tx_dd_dep.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_dd_dep.AutoCompleteCustomSource = departamentos;
            tx_dd_pro.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_dd_pro.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_dd_pro.AutoCompleteCustomSource = provincias;
            tx_dd_dis.AutoCompleteMode = AutoCompleteMode.Suggest;
            tx_dd_dis.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tx_dd_dis.AutoCompleteCustomSource = distritos;
            // longitudes maximas de campos
            tx_serie.MaxLength = 4;         // serie doc vta
            tx_serie.CharacterCasing = CharacterCasing.Upper;
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
            tx_fletLetras.MaxLength = 249;
            tx_dat_dpo.MaxLength = 100;
            tx_dat_dpd.MaxLength = 100;
            tx_pla_placa.MaxLength = 7;
            tx_pla_confv.MaxLength = 15;
            tx_pla_autor.MaxLength = 15;
            tx_dniChof.MaxLength = 8;
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
            cmb_tdv.SelectedIndex = -1;
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
            tx_idcaja.ReadOnly = true;
            tx_idcaja.Text = "";
            tx_fletLetras.ReadOnly = true;
            if (Tx_modo.Text == "NUEVO" && v_estcaj == codAbie)      // caja esta abierta?
            {
                if (fshoy != TransCarga.Program.vg_fcaj)  // fecha de la caja vs fecha de hoy
                {
                    MessageBox.Show("Las fechas no coinciden" + Environment.NewLine +
                        "Fecha de caja vs Fecha actual", "Caja fuera de fecha", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //return;
                }
                else
                {
                    tx_idcaja.Text = v_idcaj;
                }
            }
            if (Tx_modo.Text == "EDITAR")
            {
                fshoy = tx_fechope.Text;
            }
            if (Tx_modo.Text == "NUEVO")
            {
                if (v_habpago == "SI")
                {
                    rb_si.Enabled = true;
                    rb_no.Enabled = true;
                }
                else
                {
                    rb_si.Enabled = false;
                    rb_no.Checked = true;
                    rb_no.Enabled = true;
                }
                if (codsuser_cu.Contains(asd)) chk_cunica.Enabled = true;
                else chk_cunica.Enabled = false;
                if (cusdscto.Contains(asd)) tx_flete.ReadOnly = false;
                else tx_flete.ReadOnly = true;
            }
            tx_dat_nombd.Text = "Bultos";
            tx_dat_nombd.ReadOnly = true;
            glosdet = "";
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofa,@nofi,@noca,@noco,@nocg)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                micon.Parameters.AddWithValue("@nofi", "clients");
                micon.Parameters.AddWithValue("@noco", "cobranzas");
                micon.Parameters.AddWithValue("@noca", "ayccaja");
                micon.Parameters.AddWithValue("@nocg", "guiati_a");
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
                            if (row["param"].ToString() == "web_dni") webdni = row["valor"].ToString().Trim();         // web para busqueda de dni 
                        }
                        if (row["campo"].ToString() == "conector")
                        {
                            if (row["param"].ToString() == "noRetGlosa") NoRetGl = row["valor"].ToString().Trim();          // glosa que retorna umasapa cuando no encuentra dato
                        }
                        if (row["campo"].ToString() == "sunat")
                        {
                            if (row["param"].ToString() == "client_id") client_id_sunat = row["valor"].ToString().Trim();         // id del api sunat
                            if (row["param"].ToString() == "client_pass") client_pass_sunat = row["valor"].ToString().Trim();     // password del api sunat
                            if (row["param"].ToString() == "user_sol") u_sol_sunat = row["valor"].ToString().Trim();              // usuario sol portal sunat del cliente 
                            if (row["param"].ToString() == "clave_sol") c_sol_sunat = row["valor"].ToString().Trim();             // clave sol portal sunat del cliente 
                            if (row["param"].ToString() == "scope") scope_sunat = row["valor"].ToString().Trim();                 // scope del api sunat
                            if (row["param"].ToString() == "rutaCertifc") rutaCertifc = row["valor"].ToString().Trim();           // Ruta y nombre del certificado .pfx
                            if (row["param"].ToString() == "claveCertif") claveCertif = row["valor"].ToString().Trim();           // Clave del certificado
                            if (row["param"].ToString() == "wsPostSunatF") wsPostS = row["valor"].ToString().Trim();               // ruta api sunat para postear
                        }
                    }
                    if (row["formulario"].ToString() == "clients" && row["campo"].ToString() == "documento")
                    {
                        if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "cobranzas" && row["campo"].ToString() == "documento")
                    {
                        if (row["param"].ToString() == "codigo") v_codcob = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "frase2") v_fra2 = row["valor"].ToString().Trim();               // frase cuando se cancela el doc.vta.
                            if (row["param"].ToString() == "serieAnu") v_sanu = row["valor"].ToString().Trim();               // serie anulacion interna
                            if (row["param"].ToString() == "mpagdef") v_mpag = row["valor"].ToString().Trim();               // medio de pago x defecto para cobranzas
                            if (row["param"].ToString() == "factura") codfact = row["valor"].ToString().Trim();               // codigo doc.venta factura
                            if (row["param"].ToString() == "boleta") codBole = row["valor"].ToString().Trim();               // codigo doc.venta BOLETA
                            if (row["param"].ToString() == "plazocred") codppc = row["valor"].ToString().Trim();               // codigo plazo de pago x defecto para fact. a CREDITO
                            if (row["param"].ToString() == "plzoCont") codcont = row["valor"].ToString().Trim();               // codigo de plazo contado o efectivo o contraentrega
                            if (row["param"].ToString() == "usercar_unic") codsuser_cu = row["valor"].ToString().Trim();       // usuarios autorizados a crear Ft de cargas unicas
                            if (row["param"].ToString() == "diasanul") v_cdpa = int.Parse(row["valor"].ToString());            // cant dias en que usuario normal puede anular 
                            if (row["param"].ToString() == "useranul") codusanu = row["valor"].ToString();                      // usuarios autorizados a anular fuera de plazo 
                            if (row["param"].ToString() == "userdscto") cusdscto = row["valor"].ToString();                 // usuarios que pueden hacer descuentos
                            if (row["param"].ToString() == "cltesBol") tdocsBol = row["valor"].ToString();                  // tipos de documento de clientes para boletas
                            if (row["param"].ToString() == "cltesFac") tdocsFac = row["valor"].ToString();
                            if (row["param"].ToString() == "pagaSN") v_habpago = row["valor"].ToString();                     // permite cancelar en el form, SI o NO
                            if (row["param"].ToString() == "emcoper") rucsEmcoper = row["valor"].ToString();                  // rucs separados por comas, formato Emcoper del pdf coordinado con PeruSecurenet 07/12/2022
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "filasDet") v_mfildet = row["valor"].ToString().Trim();       // maxima cant de filas de detalle
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "nomfor_cr") v_CR_gr_ind = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();      // moneda por defecto
                        if (row["campo"].ToString() == "detraccion" && row["param"].ToString() == "glosa") glosdetra = row["valor"].ToString().Trim();    // glosa detraccion
                        if (row["campo"].ToString() == "factelect")
                        {
                            if (row["param"].ToString() == "textaut") restexto = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "autoriz") autoriz_OSE_PSE = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "despedi") despedida = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "webose") webose = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "correo_c1") correo_gen = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "caracterNo") caractNo = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ose-pse") nipfe = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "motivoBaja") glosaAnul = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "tipsDocbaja") tipdocAnu = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "modTran") texmotran = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "codmotTran") codtxmotran = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "cantcarl") ccf_pdf = int.Parse(row["valor"].ToString());
                        }
                    }
                    if (row["formulario"].ToString() == "ayccaja" && row["campo"].ToString() == "estado")
                    {
                        if (row["param"].ToString() == "abierto") codAbie = row["valor"].ToString().Trim();             // codigo caja abierta
                        //if (row["param"].ToString() == "cerrado") codCier = row["valor"].ToString().Trim();             // codigo caja cerrada
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "codinDV" && row["param"].ToString() == "DV") v_codidv = row["valor"].ToString().Trim();           // codigo de dov.vta en tabla TDV
                        if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "guiati_a")
                    {
                        if (row["campo"].ToString() == "detalle" && row["param"].ToString() == "glosa") vint_gg = row["valor"].ToString().Trim();
                    } 
                }
                da.Dispose();
                dt.Dispose();
                // jalamos datos del usuario y local
                v_clu = TransCarga.Program.vg_luse;                // codigo local usuario
                v_slu = lib.serlocs(v_clu);                        // serie local usuario
                v_nbu = TransCarga.Program.vg_nuse;                // nombre del usuario
                // parametros para token
                c_t[0] = client_id_sunat;
                c_t[1] = scope_sunat;
                c_t[2] = client_id_sunat;
                c_t[3] = client_pass_sunat;
                c_t[4] = u_sol_sunat;
                c_t[5] = c_sol_sunat;
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
                    parte = "where a.tipdvta=@tdv and a.serdvta=@ser and a.numdvta=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select a.id,a.fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,a.ticltgr,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt,a.ubigclt,a.corrclt,a.teleclt," +
                        "a.locorig,a.dirorig,a.ubiorig,a.obsdvta,a.canfidt,a.canbudt,a.mondvta,a.tcadvta,a.subtota,a.igvtota,a.porcigv,a.totdvta,a.totpags,a.saldvta,a.estdvta,a.frase01,a.impreso," +
                        "a.tipoclt,a.m1clien,a.tippago,a.ferecep,a.userc,a.fechc,a.userm,a.fechm,b.descrizionerid as nomest,ifnull(c.id,'') as cobra,a.idcaja,a.plazocred,a.totdvMN," +
                        "a.cargaunica,a.porcendscto,a.valordscto,a.conPago,a.pagauto,ifnull(ad.placa,'') as placa,ifnull(ad.confv,'') as confv,ifnull(ad.autoriz,'') as autoriz," +
                        "ifnull(ad.cargaEf,0) as cargaEf,ifnull(ad.cargaUt,0) as cargaUt,ifnull(ad.rucTrans,'') as rucTrans,ifnull(ad.nomTrans,'') as nomTrans,ifnull(date_format(ad.fecIniTras,'%Y-%m-%d'),'') as fecIniTras," +
                        "ifnull(ad.dirPartida,'') as dirPartida,ifnull(ad.ubiPartida,'') as ubiPartida,ifnull(ad.dirDestin,'') as dirDestin,ifnull(ad.ubiDestin,'') as ubiDestin,ifnull(ad.dniChof,'') as dniChof," +
                        "ifnull(ad.brevete,'') as brevete,ifnull(ad.valRefViaje,0) as valRefViaje,ifnull(ad.valRefVehic,0) as valRefVehic,ifnull(ad.valRefTon,0) as valRefTon " +
                        "from cabfactu a " +
                        "left join adifactu ad on ad.idc=a.id and ad.tipoAd=1 " +
                        "left join desc_est b on b.idcodice=a.estdvta " +
                        "left join cabcobran c on c.tipdoco=a.tipdvta and c.serdoco=a.serdvta and c.numdoco=a.numdvta and c.estdcob<>@coda "
                        + parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    micon.Parameters.AddWithValue("@tdep", vtc_ruc);
                    micon.Parameters.AddWithValue("@coda", codAnul);
                    if (campo == "tx_idr")
                    {
                        micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    }
                    if (campo == "sernum")
                    {
                        micon.Parameters.AddWithValue("@tdv", tx_dat_tdv.Text);
                        micon.Parameters.AddWithValue("@ser", tx_serie.Text);
                        micon.Parameters.AddWithValue("@num", tx_numero.Text);
                    }
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr != null)
                    {
                        if (dr.Read())
                        {
                            tx_idr.Text = dr.GetString("id");
                            tx_idcaja.Text = dr.GetString("idcaja");
                            tx_fechope.Text = dr.GetString("fechope").Substring(0, 10);
                            //.Text = dr.GetString("martdve");
                            tx_dat_tdv.Text = dr.GetString("tipdvta");
                            tx_serie.Text = dr.GetString("serdvta");
                            tx_numero.Text = dr.GetString("numdvta");
                            rb_remGR.Checked = (dr.GetString("ticltgr") == "1")? true : false;
                            rb_desGR.Checked = (dr.GetString("ticltgr") == "2") ? true : false;
                            rb_otro.Checked = (dr.GetString("ticltgr") == "3") ? true : false;
                            tx_dat_tdRem.Text = dr.GetString("tidoclt");
                            tx_numDocRem.Text = dr.GetString("nudoclt");
                            tx_nomRem.Text = dr.GetString("nombclt");
                            tx_dirRem.Text = dr.GetString("direclt");
                            tx_dptoRtt.Text = dr.GetString("dptoclt");
                            tx_provRtt.Text = dr.GetString("provclt");
                            tx_distRtt.Text = dr.GetString("distclt");
                            tx_ubigRtt.Text = dr.GetString("ubigclt");
                            tx_email.Text = dr.GetString("corrclt");
                            tx_telc1.Text = dr.GetString("teleclt");
                            //locorig,dirorig,ubiorig
                            tx_obser1.Text = dr.GetString("obsdvta");
                            tx_tfil.Text = dr.GetString("canfidt");
                            tx_totcant.Text = dr.GetString("canbudt");  // total bultos
                            tx_dat_mone.Text = dr.GetString("mondvta");
                            tx_tipcam.Text = dr.GetString("tcadvta");
                            tx_subt.Text = Math.Round(dr.GetDecimal("subtota"),2).ToString();
                            tx_igv.Text = Math.Round(dr.GetDecimal("igvtota"), 2).ToString();
                            //,,,porcigv
                            tx_flete.Text = Math.Round(dr.GetDecimal("totdvta"),2).ToString();           // total inc. igv
                            tx_pagado.Text = dr.GetString("totpags");
                            tx_salxcob.Text = dr.GetString("saldvta");
                            tx_dat_estad.Text = dr.GetString("estdvta");        // estado
                            tx_dat_tcr.Text = dr.GetString("tipoclt");          // tipo de cliente credito o contado
                            tx_dat_m1clte.Text = dr.GetString("m1clien");
                            tx_impreso.Text = dr.GetString("impreso");
                            tx_idcob.Text = dr.GetString("cobra");              // id de cobranza
                            //
                            cmb_tdv.SelectedValue = tx_dat_tdv.Text;
                            cmb_tdv_SelectedIndexChanged(null, null);
                            tx_numero.Text = dr.GetString("numdvta");       // al cambiar el indice en el combox se borra numero, por eso lo volvemos a jalar
                            cmb_docRem.SelectedValue = tx_dat_tdRem.Text;
                            cmb_mon.SelectedValue = tx_dat_mone.Text;
                            tx_estado.Text = dr.GetString("nomest");   // lib.nomstat(tx_dat_estad.Text);
                            if (dr.GetString("userm") == "") tx_digit.Text = lib.nomuser(dr.GetString("userc"));
                            else tx_digit.Text = lib.nomuser(dr.GetString("userm"));
                            if (dr.GetString("conPago") != "")
                            {
                                if (dr.GetString("conPago") == "0") rb_contado.Checked = true;
                                if (dr.GetString("conPago") == "1") rb_credito.Checked = true;
                                if (dr.GetString("pagauto") == "S") rb_si.Checked = true;
                                else rb_no.Checked = true;
                            }
                            else
                            {
                                if (dr.GetString("pagauto") == "S")
                                {
                                    rb_contado.Checked = true;
                                    rb_si.Checked = true;
                                }
                                else
                                {
                                    if (dr.GetString("pagauto") == "N" && dr.GetString("tippago") == v_mpag)
                                    {
                                        //rb_contado.Checked = false;
                                        rb_si.Checked = true;
                                    }
                                    else
                                    {
                                        rb_no.Checked = true;
                                        rb_credito.Checked = true;
                                    }
                                }
                            }
                            tx_valdscto.Text = dr.GetString("valordscto");
                            tx_dat_porcDscto.Text = dr.GetString("porcendscto");
                            tx_dat_plazo.Text = dr.GetString("plazocred");
                            tx_fletMN.Text = Math.Round(dr.GetDecimal("totdvMN"), 2).ToString();
                            // campos de carga unica
                            // a.placa,a.confveh,a.autoriz,a.detPeso,a.detputil,a.detMon1,a.detMon2,a.detMon3,a.dirporig,a.ubiporig,a.dirpdest,a.ubipdest,
                            // ad.placa,ad.confv,ad.autoriz,ad.cargaEf,ad.cargaUt,ad.rucTrans,ad.nomTrans,ad.fecIniTras,ad.dirPartida,ad.ubiPartida,ad.dirDestin,ad.ubiDestin,ad.dniChof,ad.brevete,ad.valRefViaje,ad.valRefVehic,ad.valRefTon "
                            if (true)       // dr.GetInt16("cargaunica") == 1  ... 16/02/2024
                            {
                                tx_pla_placa.Text = dr.GetString("placa");
                                tx_pla_confv.Text = dr.GetString("confv");
                                tx_pla_autor.Text = dr.GetString("autoriz");
                                tx_cetm.Text = dr.GetString("cargaEf");
                                tx_cutm.Text = dr.GetString("cargaUt");
                                tx_rucT.Text = dr.GetString("rucTrans");
                                tx_razonS.Text = dr.GetString("nomTrans");
                                tx_fecini.Text = dr.GetString("fecIniTras");
                                tx_dat_dpo.Text = dr.GetString("dirPartida");
                                tx_dat_upo.Text = dr.GetString("ubiPartida");
                                tx_dat_dpd.Text = dr.GetString("dirDestin");
                                tx_dat_upd.Text = dr.GetString("ubiDestin");
                                tx_dniChof.Text = dr.GetString("dniChof");
                                // brevete
                                tx_valref1.Text = dr.GetString("valRefViaje");
                                tx_valref2.Text = dr.GetString("valRefVehic");
                                tx_valref3.Text = dr.GetString("valRefTon");

                                if (dr.GetInt16("cargaunica") == 1) chk_cunica.Checked = true;
                                string[] retub = lib.retDPDubigeo(tx_dat_upo.Text);
                                tx_dp_dep.Text = retub[0];
                                tx_dp_pro.Text = retub[1];
                                tx_dp_dis.Text = retub[2];
                                string[] retud = lib.retDPDubigeo(tx_dat_upd.Text);
                                tx_dd_dep.Text = retud[0];
                                tx_dd_pro.Text = retud[1];
                                tx_dd_dis.Text = retud[2];
                            }
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
                    if (decimal.Parse(tx_valdscto.Text) > 0)
                    {
                        lin_dscto.Visible = true;
                        lb_dscto.Visible = true;
                        tx_valdscto.Visible = true;
                    }
                    else
                    {
                        lin_dscto.Visible = false;
                        lb_dscto.Visible = false;
                        tx_valdscto.Visible = false;
                    }
                    //
                    DataRow[] row = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");
                    NumLetra nel = new NumLetra();
                    tx_fletLetras.Text = nel.Convertir(tx_flete.Text, true) + row[0][3].ToString().Trim();
                    //
                    if (tx_dat_plazo.Text.Trim() != "" && tx_dat_plazo.Text != codcont)    // osea que no seas contado -> osea es credito 
                    {
                        cmb_plazoc.SelectedValue = tx_dat_plazo.Text;
                    }
                }
                conn.Close();
            }
        }
        private void jaladet(string idr)         // jala el detalle
        {
            string jalad = "select a.filadet,a.codgror,a.cantbul,d.unimedpro,a.descpro,a.pesogro,a.codmogr,a.totalgr," +
                "g.totgrMN,g.codMN,g.fechopegr,g.docsremit,g.tipmongri,concat(lo.descrizionerid,' - ',ld.descrizionerid) as orides," +
                "b.porcendscto,b.valordscto,d.unimedpro " +
                "from detfactu a left join cabguiai g on concat(g.sergui,'-',g.numgui)=a.codgror " +
                "left join detguiai d on d.idc=g.id " +
                "left join desc_loc lo on lo.idcodice=g.locorigen " +
                "left join desc_loc ld on ld.idcodice=g.locdestin " +
                "left join cabfactu b on b.id=a.idc " +
                "where a.idc=@idr";
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
                            var a = row[10].ToString().Substring(0, 10);
                            string valorel = "";
                            if (row[14].ToString().Trim() != "" && row[14].ToString().Trim().Substring(0,4) != "0.00")
                            {
                                decimal vdf = Math.Truncate((decimal.Parse(row[7].ToString()) * decimal.Parse(row[14].ToString())) / 100);
                                //dataGridView1.Rows[i].Cells[12].Value = (decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) - vdf).ToString("#0.00");
                                valorel = vdf.ToString("#0.00");
                            }
                            dataGridView1.Rows.Add(
                                row[1].ToString(),      // guias
                                row[4].ToString(),      // descrip
                                row[2].ToString(),      // Cant (cant bultos)
                                row[6].ToString(),      // moneda (nombre)
                                row[7].ToString(),      // valor 
                                row[8].ToString(),     // valorMN
                                row[9].ToString(),     // codmonloc
                                a.Substring(6, 4) + "-" + a.Substring(3, 2) + "-" + a.Substring(0, 2),     // fechaGR
                                row[11].ToString(),     // guiasclte
                                row[12].ToString(),     // codmondoc
                                row[13].ToString(),     // OriDest
                                "",                     // saldo
                                valorel,               // valorel
                                row[16].ToString());    // unidad de medida
                            tx_dat_nombd.Text = row[3].ToString();
                            //glosser2 = dataGridView1.Rows[0].Cells["OriDest"].Value.ToString() + " - " + tx_totcant.Text.Trim() + " " + tx_dat_nombd.Text;
                            glosser2 = row[13].ToString() + " - " + tx_totcant.Text.Trim() + " " + tx_dat_nombd.Text;
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
                string consu = "select distinct a.idcodice,a.descrizionerid,a.enlace1,a.codsunat,b.glosaser,b.serie " +
                    "from desc_tdv a LEFT JOIN series b ON b.tipdoc = a.IDCodice where a.numero=@bloq and a.codigo=@codv and b.sede=@loca";
                using (MySqlCommand cdv = new MySqlCommand(consu, conn))
                {
                    cdv.Parameters.AddWithValue("@bloq", 1);
                    cdv.Parameters.AddWithValue("@codv", v_codidv);
                    cdv.Parameters.AddWithValue("@loca", v_clu);
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
                using (MySqlCommand cdu = new MySqlCommand("select idcodice,descrizionerid,codigo,codsunat from desc_doc where numero=@bloq", conn))
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
                // datos del combo plazo de pago creditos
                using (MySqlCommand compla = new MySqlCommand("select idcodice,descrizionerid,codsunat,marca1 from desc_tpa where numero=@bloq", conn))
                {
                    compla.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dapla = new MySqlDataAdapter(compla))
                    {
                        dtp.Clear();
                        dapla.Fill(dtp);
                        cmb_plazoc.DataSource = dtp;
                        cmb_plazoc.DisplayMember = "descrizionerid";
                        cmb_plazoc.ValueMember = "idcodice";
                    }
                }
                // jalamos la caja
                using (MySqlCommand micon = new MySqlCommand("select id,fechope,statusc from cabccaja where loccaja=@luc order by id desc limit 1", conn))
                {
                    micon.Parameters.AddWithValue("@luc", v_clu);
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            v_estcaj = dr.GetString("statusc");
                            v_idcaj = dr.GetString("id");
                        }
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
            if (vi_formato == "")       // formato de impresion del documento
            {
                lib.messagebox("formato de impresion del Doc.Venta");
                retorna = false;
            }
            if (vi_copias == "")        // cant copias impresion
            {
                lib.messagebox("# copias impresas del Doc.Venta");
                retorna = false;
            }
            if (v_impTK == "")           // nombre de la ticketera
            {
                lib.messagebox("Nombre de impresora de Tickets");
                retorna = false;
            }
            if (v_sanu == "")           // serie de anulacion del documento
            {
                lib.messagebox("Serie de Anulación interna");
                retorna = false;
            }
            if (v_CR_gr_ind == "")
            {
                lib.messagebox("Nombre formato Doc.Venta en CR");
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
                datcltsR[8] = "";
                //
                datcltsD[0] = "";
                datcltsD[1] = "";
                datcltsD[2] = "";
                datcltsD[3] = "";
                datcltsD[4] = "";
                datcltsD[5] = "";
                datcltsD[6] = "";
                datcltsD[7] = "";
                datcltsD[8] = "";
                //
                datguias[0] = "";   // num GR
                datguias[1] = "";   // descrip
                datguias[2] = "";   // cant bultos
                datguias[3] = "";   // nombre de la moneda de la GR
                datguias[4] = "";   // valor de la guía en su moneda
                datguias[5] = "";   // valor en moneda local
                datguias[6] = "";   // codigo moneda local
                datguias[7] = "";   // codigo moneda de la guia
                datguias[8] = "";   // tipo de cambio
                datguias[9] = "";   // fecha de la GR
                datguias[10] = "";  // guia del cliente, sustento del cliente
                datguias[11] = "";   // placa
                datguias[12] = "";   // carreta
                datguias[13] = "";   // autoriz circulacion
                datguias[14] = "";   // conf. vehicular
                datguias[15] = "";  // local origen-destino
                datguias[16] = "";  // saldo de la GR
                datguias[17] = "";  // unidad medida 
                //
                datcargu[0] = "";   // direc. partida
                datcargu[1] = "";   // depart. pto. partida
                datcargu[2] = "";   // provin. pto. partida
                datcargu[3] = "";   // distri. pto. partida
                datcargu[4] = "";   // ubigeo punto partida
                datcargu[5] = "";   // direc. llegada
                datcargu[6] = "";   // depart. pto. llegada
                datcargu[7] = "";   // provin. pto. llegada
                datcargu[8] = "";   // distri. pto. llegada
                datcargu[9] = "";   // ubigeo punto llegada
                datcargu[10] = "";  // ruc del camion
                datcargu[11] = "";  // razon social del ruc
                datcargu[12] = "";  // fecha inicio del traslado

                // validamos que la GR: 1.exista, 2.No este facturada, 3.No este anulada
                // y devolvemos una fila con los datos del remitente y otra fila los datos del destinatario
                string hay = "no";
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    lib.procConn(conn);
                    string cons = "select fecguitra,totguitra,estadoser,fecdocvta,tipdocvta,serdocvta,numdocvta,codmonvta,totdocvta,saldofina " +
                        "from controlg where serguitra=@ser and numguitra=@num";
                    using (MySqlCommand mic1 = new MySqlCommand(cons, conn))
                    {
                        mic1.Parameters.AddWithValue("@ser", serie);
                        mic1.Parameters.AddWithValue("@num", corre);
                        using (MySqlDataReader dr = mic1.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                if (dr.Read())
                                {
                                    if (dr.GetString("numdocvta").Trim() != "") hay = "sif"; // si hay guía pero ya esta facturado
                                    else hay = "sin";    // si hay guía y no tiene factura
                                    if (dr.GetString("saldofina") != dr.GetString("totguitra") && dr.GetDecimal("saldofina") > 0)
                                    {
                                        MessageBox.Show("No esta permitido generar un documento" + Environment.NewLine + 
                                            "de venta de una guía que tiene pago parcial","Atención - no puede continuar");
                                        hay = "no";
                                    }
                                }
                            }
                            else
                            {
                                hay = "no"; // no existe la guía
                            }
                        }
                    }
                    if (hay == "sin")
                    {
                        string consulta = "SELECT a.tidoregri,a.nudoregri,b1.razonsocial as nombregri,b1.direcc1 as direregri,b1.ubigeo as ubigregri,ifnull(b1.email,'') as emailR,ifnull(b1.numerotel1,'') as numtel1R," +
                            "ifnull(b1.numerotel2,'') as numtel2R,a.tidodegri,a.nudodegri,b2.razonsocial as nombdegri,b2.direcc1 as diredegri,b2.ubigeo as ubigdegri,ifnull(b2.email,'') as emailD," +
                            "ifnull(b2.numerotel1,'') as numtel1D,ifnull(b2.numerotel2,'') as numtel2D,a.tipmongri,a.totgri,a.salgri,SUM(d.cantprodi) AS bultos,date(a.fechopegr) as fechopegr,a.tipcamgri," +
                            "max(d.descprodi) AS descrip,ifnull(m.descrizionerid,'') as mon,a.totgrMN,a.codMN,c.fecdocvta,b1.tiposocio as tipsrem,b2.tiposocio as tipsdes,a.docsremit," +
                            "a.plaplagri,a.carplagri,a.autplagri,a.confvegri,concat(lo.descrizionerid,' - ',ld.descrizionerid) as orides,c.saldofina,a.direregri as dirpartida," +
                            "a.ubigregri as ubigpartida,a.diredegri as dirllegada,a.ubigdegri as ubigllegada,ifnull(a.fechplani,'') as fechplani,a.proplagri,ifnull(p.RazonSocial,'') as RazonSocial,d.unimedpro,a.docsremit2 " +
                            "from cabguiai a left join detguiai d on d.idc=a.id " +
                            "LEFT JOIN controlg c ON c.serguitra = a.sergui AND c.numguitra = a.numgui " +
                            "left join anag_for p on p.ruc=a.proplagri " +
                            "left join anag_cli b1 on b1.tipdoc=a.tidoregri and b1.ruc=a.nudoregri " +
                            "left join anag_cli b2 on b2.tipdoc=a.tidodegri and b2.ruc=a.nudodegri " +
                            "left join desc_mon m on m.idcodice=a.tipmongri " +
                            "left join desc_loc lo on lo.idcodice=a.locorigen " +
                            "left join desc_loc ld on ld.idcodice=a.locdestin " +
                            "WHERE a.sergui = @ser AND a.numgui = @num AND a.estadoser not IN(@est) AND c.fecdocvta IS NULL";
                        using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                        {
                            micon.Parameters.AddWithValue("@ser", serie);
                            micon.Parameters.AddWithValue("@num", corre);
                            micon.Parameters.AddWithValue("@est", codAnul);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    if (!dr.IsDBNull(0))    //  && dr[24] == DBNull.Value
                                    {
                                        datcltsR[0] = dr.GetString("tidoregri");        // datos del remitente de la GR
                                        datcltsR[1] = dr.GetString("nudoregri");
                                        datcltsR[2] = dr.GetString("nombregri");
                                        datcltsR[3] = dr.GetString("direregri");
                                        datcltsR[4] = dr.GetString("ubigregri");
                                        datcltsR[5] = dr.GetString("emailR");
                                        datcltsR[6] = dr.GetString("numtel1R");
                                        datcltsR[7] = dr.GetString("numtel2R");
                                        datcltsR[8] = dr.GetString("tipsrem");
                                        //
                                        datcltsD[0] = dr.GetString("tidodegri");        // datos del destinatario de la GR
                                        datcltsD[1] = dr.GetString("nudodegri");
                                        datcltsD[2] = dr.GetString("nombdegri");
                                        datcltsD[3] = dr.GetString("diredegri");
                                        datcltsD[4] = dr.GetString("ubigdegri");
                                        datcltsD[5] = dr.GetString("emailD");
                                        datcltsD[6] = dr.GetString("numtel1D");
                                        datcltsD[7] = dr.GetString("numtel2D");
                                        datcltsD[8] = dr.GetString("tipsdes");
                                        //
                                        datguias[0] = serie + "-" + corre;                 // GR
                                        datguias[1] = (dr.IsDBNull(20)) ? "" : dr.GetString("descrip");         // descrip
                                        datguias[2] = (dr.IsDBNull(19)) ? "0" : dr.GetString("bultos");          // cant bultos
                                        datguias[3] = dr.GetString("mon");             // nombre moneda de la GR
                                        datguias[4] = dr.GetString("totgri");          // valor GR en su moneda
                                        datguias[5] = dr.GetString("totgrMN");         // valor GR en moneda local
                                        datguias[6] = dr.GetString("codMN");            // codigo moneda local
                                        datguias[7] = dr.GetString("tipmongri");        // codigo moneda de la guía
                                        datguias[8] = dr.GetString("tipcamgri");     // tipo de cambio de la GR
                                        var a = dr.GetString("fechopegr").Substring(0, 10);
                                        datguias[9] = a.Substring(6,4) + "-" + a.Substring(3,2) + "-" + a.Substring(0,2);     // fecha de la GR
                                        datguias[10] = dr.GetString("docsremit") + " " + dr.GetString("docsremit2");
                                        datguias[11] = dr.GetString("plaplagri"); 
                                        datguias[12] = dr.GetString("carplagri");
                                        datguias[13] = dr.GetString("autplagri");
                                        datguias[14] = dr.GetString("confvegri");
                                        datguias[15] = dr.GetString("orides");
                                        datguias[16] = dr.GetString("salgri");
                                        datguias[17] = dr.GetString("unimedpro");
                                        //
                                        datcargu[0] = dr.GetString("dirpartida");
                                        datcargu[4] = dr.GetString("ubigpartida");   // ubigeo punto partida
                                        string[] aa = lib.retDPDubigeo(datcargu[4]);
                                        datcargu[1] = aa[0];   // depart. pto. partida
                                        datcargu[2] = aa[1];   // provin. pto. partida
                                        datcargu[3] = aa[2];   // distri. pto. partida
                                        datcargu[5] = dr.GetString("dirllegada");   // direc. llegada
                                        datcargu[9] = dr.GetString("ubigllegada");   // ubigeo punto llegada
                                        aa = lib.retDPDubigeo(datcargu[9]);
                                        datcargu[6] = aa[0];   // depart. pto. llegada
                                        datcargu[7] = aa[1];   // provin. pto. llegada
                                        datcargu[8] = aa[2];   // distri. pto. llegada
                                        datcargu[10] = dr.GetString("proplagri");  // ruc del camion
                                        datcargu[11] = dr.GetString("RazonSocial");  // razon social del ruc
                                        datcargu[12] = dr.GetString("fechplani");    // fecha inicio traslado
                                        //
                                        tx_dat_saldoGR.Text = dr.GetString("salgri");
                                        retorna = true;
                                    }
                                }
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
        private void calculos(decimal totDoc)
        {
            decimal tigv = 0;
            decimal tsub = 0;
            if (totDoc > 0)
            {
                tsub = Math.Round(totDoc / (1 + decimal.Parse(v_igv) / 100), 2);
                tigv = Math.Round(totDoc - tsub, 2);
                
            }
            tx_igv.Text = tigv.ToString("#0.00");
            tx_subt.Text = tsub.ToString("#0.00");
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
        private void cargaunica()               // campos de carga unica
        {
            if (true)  // chk_cunica.Checked == true  ... 16/02/2024
            {
                //panel2.Enabled = true;
                tx_dat_dpo.Enabled = true;
                tx_dat_dpd.Enabled = true;
                if (dataGridView1.Rows[0].Cells[0].Value != null)
                {
                    tx_pla_placa.Text = datguias[11].ToString();
                    tx_pla_confv.Text = datguias[14].ToString();
                    tx_pla_autor.Text = datguias[13].ToString();
                    tx_rucT.Text = datcargu[10].ToString();
                    tx_razonS.Text = datcargu[11].ToString();
                    tx_fecini.Text = (datcargu[12].ToString().Length < 10) ? "" : datcargu[12].ToString().Substring(0, 10);
                    tx_cetm.Text = "";
                    tx_cutm.Text = "";
                    tx_valref1.Text = "";
                    tx_valref2.Text = "";
                    tx_valref3.Text = "";
                    tx_dat_dpo.Text = datcargu[0].ToString();       // datcltsR[3].ToString();
                    tx_dp_dep.Text = datcargu[1].ToString();
                    tx_dp_pro.Text = datcargu[2].ToString();
                    tx_dp_dis.Text = datcargu[3].ToString();
                    tx_dat_upo.Text = datcargu[4].ToString();     // datcltsR[4].ToString();
                    tx_dat_dpd.Text = datcargu[5].ToString();       // datcltsD[3].ToString();
                    tx_dd_dep.Text = datcargu[6].ToString();
                    tx_dd_pro.Text = datcargu[7].ToString();
                    tx_dd_dis.Text = datcargu[8].ToString();
                    tx_dat_upd.Text = datcargu[9].ToString();     // datcltsD[4].ToString();
                }
                tx_dat_nombd.ReadOnly = false;
            }
        }
        private void armacfe()                  // arma cabecera de fact elect.
        {
            tcfe.Clear();
            tcfe.Columns.Add("_fecemi");    // fecha de emision   yyyy-mm-dd
            tcfe.Columns.Add("Prazsoc");    // razon social del emisor
            tcfe.Columns.Add("Pnomcom");    // nombre comercial del emisor
            tcfe.Columns.Add("ubigEmi");    // UBIGEO DOMICILIO FISCAL
            tcfe.Columns.Add("Pdf_dir");    // DOMICILIO FISCAL - direccion
            tcfe.Columns.Add("Pdf_urb");    // DOMICILIO FISCAL - Urbanizacion
            tcfe.Columns.Add("Pdf_pro");    // DOMICILIO FISCAL - provincia
            tcfe.Columns.Add("Pdf_dep");    // DOMICILIO FISCAL - departamento
            tcfe.Columns.Add("Pdf_dis");    // DOMICILIO FISCAL - distrito
            tcfe.Columns.Add("paisEmi");    // DOMICILIO FISCAL - código de país
            tcfe.Columns.Add("Ptelef1");    // teléfono del emisor
            tcfe.Columns.Add("Pweb1");      // página web del emisor
            tcfe.Columns.Add("Prucpro");    // Ruc del emisor
            tcfe.Columns.Add("Pcrupro");    // codigo Ruc emisor
            tcfe.Columns.Add("_tipdoc");    // Tipo de documento de venta - 1 car
            tcfe.Columns.Add("_moneda");    // Moneda del doc. de venta - 3 car
            tcfe.Columns.Add("_sercor");    // Serie y correlat concatenado F001-00000001 - 13 car
            tcfe.Columns.Add("Cnumdoc");    // numero de doc. del cliente - 15 car
            tcfe.Columns.Add("Ctipdoc");    // tipo de doc. del cliente - 1 car
            tcfe.Columns.Add("Cnomcli");    // nombre del cliente - 100 car
            tcfe.Columns.Add("ubigAdq");    // ubigeo del adquiriente - 6 car
            tcfe.Columns.Add("dir1Adq");    // direccion del adquiriente 1
            tcfe.Columns.Add("dir2Adq");    // direccion del adquiriente 2
            tcfe.Columns.Add("provAdq");    // provincia del adquiriente
            tcfe.Columns.Add("depaAdq");    // departamento del adquiriente
            tcfe.Columns.Add("distAdq");    // distrito del adquiriente
            tcfe.Columns.Add("paisAdq");    // pais del adquiriente
            tcfe.Columns.Add("_totoin");    // total operaciones inafectas
            tcfe.Columns.Add("_totoex");    // total operaciones exoneradas
            tcfe.Columns.Add("_toisc");     // total impuesto selectivo consumo
            tcfe.Columns.Add("_totogr");    // Total valor venta operaciones grabadas n(12,2)  15
            tcfe.Columns.Add("_totven");    // Importe total de la venta n(12,2)             15
            tcfe.Columns.Add("tipOper");    // tipo de operacion - 4 car
            tcfe.Columns.Add("codLocE");    // codigo local emisor
            tcfe.Columns.Add("conPago");    // condicion de pago
            tcfe.Columns.Add("plaPago");    // plazo de pago en días
            tcfe.Columns.Add("fvencto");    // fecha de vencimiento de la fact credito yyyy-mm-dd
            tcfe.Columns.Add("_codgui");    // Código de la guia de remision TRANSPORTISTA
            tcfe.Columns.Add("_scotro");    // serie y numero concatenado de la guia
            tcfe.Columns.Add("codgrem");
            tcfe.Columns.Add("scogrem");
            tcfe.Columns.Add("obser1");     // observacion del documento
            tcfe.Columns.Add("obser2");     // mas observaciones
            tcfe.Columns.Add("maiAdq");     // correo del adquiriente
            tcfe.Columns.Add("teladq");     // telefono del adquiriente
            tcfe.Columns.Add("totImp");     // total impuestos del documento
            tcfe.Columns.Add("codImp");     // codigo impuesto
            tcfe.Columns.Add("nomImp");     // nombre del tipo de impuesto
            tcfe.Columns.Add("tipTri");     // tipo de tributo
            tcfe.Columns.Add("monLet");     // monto en letras
            tcfe.Columns.Add("_horemi");    // hora de emision del doc.venta
            tcfe.Columns.Add("_fvcmto");    // fecha de vencimiento del doc.venta
            tcfe.Columns.Add("corclie");    // correo del emisor
            tcfe.Columns.Add("_morefD");    // moneda de refencia para el tipo de cambio
            tcfe.Columns.Add("_monobj");    // moneda objetivo del tipo de cambio
            tcfe.Columns.Add("_tipcam");    // tipo de cambio con 3 decimales
            tcfe.Columns.Add("_fechca");    // fecha del tipo de cambio
            tcfe.Columns.Add("d_conpa");    // condicion de pago
            tcfe.Columns.Add("d_valre");    // valor referencial
            tcfe.Columns.Add("d_numre");    // numero registro mtc del camion
            tcfe.Columns.Add("d_confv");    // config. vehicular del camion
            tcfe.Columns.Add("d_ptori");    // Pto de origen
            tcfe.Columns.Add("d_ptode");    // Pto de destino
            tcfe.Columns.Add("d_vrepr");    // valor referencial preliminar
            tcfe.Columns.Add("codleyt");    // codigoLeyenda 1 - valor en letras
            tcfe.Columns.Add("codobs");     // codigo del ose para las observaciones, caso carrion documentos origen del remitente
            tcfe.Columns.Add("_forpa");     // glosa de forma de pago SUNAT
            tcfe.Columns.Add("_valcr");     // valor credito
            tcfe.Columns.Add("_fechc");     // fecha programada del pago credito
            // detraccion
            tcfe.Columns.Add("d_porde");                    // 2 Porcentaje de detracción
            tcfe.Columns.Add("d_valde");                    // 3 Monto de la detracción
            tcfe.Columns.Add("d_codse");                    // 4 Código del Bien o Servicio Sujeto a Detracción
            tcfe.Columns.Add("d_ctade");                    // 5 Número del cta en el bco de la nación
            tcfe.Columns.Add("d_medpa");                    // 6 medio de pago de la detraccion (001 = deposito en cuenta)
            tcfe.Columns.Add("glosdet");                    // 7 Leyenda: Detracción        300
            tcfe.Columns.Add("totdet", typeof(double));     // total detraccion
            tcfe.Columns.Add("codleyd");                    // codigo leyenda detraccion
            tcfe.Columns.Add("d_monde");                    // moneda de la detraccion
            // carga unica - traslado de bienes
            tcfe.Columns.Add("cu_cpapp");                   // 02    codigo pais de origen ... osea PE
            tcfe.Columns.Add("cu_ubipp");                   // 03    Ubigeo del punto de partida 
            tcfe.Columns.Add("cu_deppp");                   // 04    Departamento del punto de partida
            tcfe.Columns.Add("cu_propp");                   // 05    Provincia del punto de partida
            tcfe.Columns.Add("cu_dispp");                   // 06    Distrito del punto de partida
            tcfe.Columns.Add("cu_urbpp");                   // 07    Urbanización del punto de partida
            tcfe.Columns.Add("cu_dirpp");                   // 08    Dirección detallada del punto de partida
            tcfe.Columns.Add("cu_cppll");                   // 09    Código país del punto de llegada
            tcfe.Columns.Add("cu_ubpll");                   // 10    Ubigeo del punto de llegada
            tcfe.Columns.Add("cu_depll");                   // 11    Departamento del punto de llegada
            tcfe.Columns.Add("cu_prpll");                   // 12    Provincia del punto de llegada
            tcfe.Columns.Add("cu_dipll");                   // 13    Distrito del punto de llegada
            tcfe.Columns.Add("cu_urpll");                   // 14    Urbanización del punto de llegada
            tcfe.Columns.Add("cu_ddpll");                   // 15    Dirección detallada del punto de llegada
            tcfe.Columns.Add("cu_placa");                   // 16    Placa del Vehículo
            tcfe.Columns.Add("cu_coins");                   // 17    Constancia de inscripción del vehículo o certificado de habilitación vehicular
            tcfe.Columns.Add("cu_marca");                   // 18    Marca del Vehículo
            tcfe.Columns.Add("cu_breve");                   // 19    Nro.de licencia de conducir
            tcfe.Columns.Add("cu_ructr");                   // 20    RUC del transportista
            tcfe.Columns.Add("cu_nomtr");                   // 21    Razón social del Transportista
            tcfe.Columns.Add("cu_modtr");                   // 22    Modalidad de Transporte
            tcfe.Columns.Add("cu_pesbr");                   // 23    Total Peso Bruto
            tcfe.Columns.Add("cu_motra");                   // 24    Código de Motivo de Traslado
            tcfe.Columns.Add("cu_fechi");                   // 25    Fecha de Inicio de Traslado
            tcfe.Columns.Add("cu_remtc");                   // 26    Registro MTC
            tcfe.Columns.Add("cu_nudch");                   // 27    Nro.Documento del conductor
            tcfe.Columns.Add("cu_tidch");                   // 28    Tipo de Documento del conductor
            tcfe.Columns.Add("cu_plac2");                   // 29    Placa del Vehículo secundario
            tcfe.Columns.Add("cu_insub");                   // 30   Indicador de subcontratación
        }
        private void armadfe()                  // arma detalle de fact elect.
        {
            tdfe.Clear();
            tdfe.Columns.Add("Inumord");                    // 2 numero de orden del item           
            tdfe.Columns.Add("Idatper");                    // 3 Datos personilazados del item      
            tdfe.Columns.Add("Iumeded");                    // 4 Unidad de medida                    3
            tdfe.Columns.Add("Icantid");                    // 5 Cantidad de items             n(12,2)
            tdfe.Columns.Add("Idescri");                    // 6 Descripcion                       500
            tdfe.Columns.Add("Idesglo");                    // 7 descricion de la glosa del item   250
            tdfe.Columns.Add("Icodprd");                    // 8 codigo del producto del cliente    30
            tdfe.Columns.Add("Icodpro");                    // 9 codigo del producto SUNAT           8
            tdfe.Columns.Add("Icodgs1");                    // 10 codigo del producto GS1           14
            tdfe.Columns.Add("Icogtin");                    // 11 tipo de producto GTIN             14
            tdfe.Columns.Add("Inplaca");                    // 12 numero placa de vehiculo
            tdfe.Columns.Add("Ivaluni");                    // 13 Valor unitario del item SIN IMPUESTO 
            tdfe.Columns.Add("Ipreuni");                    // 14 Precio de venta unitario CON IGV
            tdfe.Columns.Add("Ivalref");                    // 15 valor referencial del item cuando la venta es gratuita
            tdfe.Columns.Add("_msigv", typeof(double));     // 16 monto igv
            tdfe.Columns.Add("Icatigv");                    // 17 tipo/codigo de afectacion igv
            tdfe.Columns.Add("Itasigv");                    // 18 tasa del igv
            tdfe.Columns.Add("Iigvite");                    // 19 monto IGV del item
            tdfe.Columns.Add("Icodtri");                    // 20 codigo del tributo por item
            tdfe.Columns.Add("Iiscmba");                    // 21 ISC monto base
            tdfe.Columns.Add("Iisctas");                    // 22 ISC tasa del tributo
            tdfe.Columns.Add("Iisctip");                    // 23 ISC tipo de afectacion
            tdfe.Columns.Add("Iiscmon");                    // 24 ISC monto del tributo
            tdfe.Columns.Add("Icbper1");                    // 25 indicador de afecto a ICBPER
            tdfe.Columns.Add("Icbper2");                    // 26 monto unitario de ICBPER
            tdfe.Columns.Add("Icbper3");                    // 27 monto total ICBPER del item
            tdfe.Columns.Add("Iotrtri");                    // 28 otros tributos monto base
            tdfe.Columns.Add("Iotrtas");                    // 29 otros tributos tasa del tributo
            tdfe.Columns.Add("Iotrlin");                    // 30 otros tributos monto unitario
            tdfe.Columns.Add("Itdscto");                    // 31 Descuentos por ítem
            tdfe.Columns.Add("Iincard");                    // 32 indicador de cargo/descuento
            tdfe.Columns.Add("Icodcde");                    // 33 codigo de cargo/descuento
            tdfe.Columns.Add("Ifcades");                    // 34 Factor de cargo/descuento
            tdfe.Columns.Add("Imoncde");                    // 35 Monto de cargo/descuento
            tdfe.Columns.Add("Imobacd");                    // 36 Monto base del cargo/descuento
            tdfe.Columns.Add("Ivalvta");                    // 37 Valor de venta del ítem

            //tdfe.Columns.Add("Iotrsis");                    // otros tributos tipo de sistema
            //tdfe.Columns.Add("Imonbas");                    // monto base (valor sin igv * cantidad)
            //tdfe.Columns.Add("Isumigv");                    // Sumatoria de igv
            //tdfe.Columns.Add("Iindgra");                    // indicador de gratuito
        }
        private string[] busqueda_clt_conector(string tipoD, string numeD)  // retorna datos del cliente del conector externo
        {
            string[] retorna = new string[8];
            retorna[0] = ""; retorna[1] = ""; retorna[2] = ""; retorna[3] = "";
            retorna[4] = ""; retorna[5] = ""; retorna[6] = ""; retorna[7] = "";

            if (tx_dat_tdRem.Text == vtc_ruc)
            {
                if (lib.valiruc(tx_numDocRem.Text, tx_dat_tdRem.Text) == false)
                {
                    MessageBox.Show("Número de ruc es inválido", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_numDocRem.Text = "";
                    tx_nomRem.Text = "";
                    return retorna;
                }
            }
            // si no hay Y SI DOCUMENTO ES RUC O DNI, vamos al conector a buscarlo por ahí
            string[] biene = lib.conectorSolorsoft(cmb_docRem.Text.ToUpper().Trim(), tx_numDocRem.Text);
            string myStr = biene[0].Replace("\r\n", "");
            if (biene[0] == "" || myStr == NoRetGl)  // compara retorno vacio o glosa cuando no encuentra el dato
            {
                if (tx_dat_tdRem.Text == vtc_ruc)
                {
                    var aa = MessageBox.Show(" No encontramos el documento en ningún registro. " + Environment.NewLine +
                    " Deberá ingresarlo manualmente si esta seguro(a) " + Environment.NewLine +
                    " de la validez del número y documento. " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "Confirma que desea ingresarlo manualmente?", "Atención", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.No)
                    {
                        tx_numDocRem.Text = "";
                        tx_numDocRem.Focus();
                        return retorna;
                    }
                }
                if (tx_dat_tdRem.Text == vtc_dni)
                {
                    MessageBox.Show("No encontramos el DNI en la busqueda inicial, estamos abriendo" + Environment.NewLine +
                                    "una página web para que efectúe la busqueda manualmente", "Redirección a web de DNI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(webdni);    // "https://eldni.com/pe/buscar-por-dni"
                    tx_nomRem.Enabled = true;
                    tx_nomRem.ReadOnly = false;
                }
            }
            else
            {
                if (tx_dat_tdRem.Text == vtc_ruc)
                {
                    if (biene[6] != "ACTIVO" || biene[7] != "HABIDO")
                    {
                        var aa = MessageBox.Show("No debería generar el comprobante" + Environment.NewLine +
                            "el ruc tiene el estado o condición no correcto" + Environment.NewLine + Environment.NewLine +
                            "Condición: " + biene[7] + Environment.NewLine +
                            "Estado: " + biene[6] + Environment.NewLine + Environment.NewLine +
                            "CONFIRMA QUE DESEA CONTINUAR?", "Alerta - no debería continuar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.No)
                        {
                            tx_numDocRem.Text = "";
                            tx_numDocRem.Focus();
                            return retorna;
                        }
                    }
                    retorna[0] = biene[0];     // razon social; 
                    retorna[1] = biene[1];     // ubigeo
                    retorna[2] = biene[2];     // direccion
                    retorna[3] = biene[3];     // departamento
                    retorna[4] = biene[4];     // provincia
                    retorna[5] = biene[5];     // distrito
                    retorna[6] = biene[6];     // estado del contrib.
                    retorna[7] = biene[7];     // situación de domicilio
                }
                if (tx_dat_tdRem.Text == vtc_dni)
                {
                    //tx_nombre.Text = biene[0];   // razon social
                    retorna[0] = biene[0];     // razon social; 
                }
            }
            return retorna;
        }

        #region facturacion electronica
        private bool factElec(string provee, string tipo, string accion, int ctab)                 // conexion a facturacion electrónica provee=proveedor | tipo=txt ó json
        {
            bool retorna = false;
            
            DataRow[] row = dttd1.Select("idcodice='"+tx_dat_tdv.Text+"'");             // tipo de documento venta
            tipdo = row[0][3].ToString();
            string serie = row[0][1].ToString().Substring(0,1) + lib.Right(tx_serie.Text,3);
            string corre = tx_numero.Text;
            DataRow[] rowd = dttd0.Select("idcodice='"+tx_dat_tdRem.Text+"'");          // tipo de documento del cliente
            tipoDocEmi = rowd[0][3].ToString().Trim();
            DataRow[] rowm = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");         // tipo de moneda
            tipoMoneda = rowm[0][2].ToString().Trim();
            //
            if (provee == "Horizont")       // INCOMPLETO .. NO USAR ...
            {
                /*
                string ruta = rutatxt + "TXT/";
                string archi = "";
                if (accion == "alta")
                {
                    archi = rucclie + "-" + tipdo + "-" + serie + "-" + corre;
                    if (crearTXT(tipdo, serie, corre, ruta + archi) == true)
                    {
                        retorna = true;
                    }
                }
                if (accion == "baja")
                {
                    //string _fecemi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   
                    string _fecemi = tx_fechact.Text.Substring(6, 4) + "-" + tx_fechact.Text.Substring(3, 2) + "-" + tx_fechact.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
                    string _secuen = lib.Right("00" + ctab.ToString(), 3);
                    string _codbaj = "RA" + "-" + tx_fechact.Text.Substring(6, 4) + tx_fechact.Text.Substring(3, 2) + tx_fechact.Text.Substring(0, 2);  // codigo comunicacion de baja
                    archi = rucclie + "-" + _codbaj + "-" + _secuen;
                    if (bajaTXT(tipdo, _fecemi, _codbaj, _secuen, ruta + archi, ctab, serie, corre) == true) retorna = true;
                }
                */
            }
            if (provee == "secure")
            {
                string ruta = rutatxt;
                string archi = "";
                if (accion == "alta")
                {
                    archi = rucclie + "-" + tipdo + "-" + serie + "-" + corre;
                    if (datosTXT(tipdo, serie, corre, ruta + archi) == true)
                    {
                        if (datDetxt(tipdo, serie, corre) == true)
                        {
                            if (generaTxt(tipdo, serie, corre, ruta + archi) == true)
                            {
                                retorna = true;
                            }
                        }
                    }
                }
                if (accion == "baja")
                {
                    if (tipdocAnu.Contains(tipdo))  // este pse no permite hacer bajas de Boletas .... que monses !!
                    {
                        string _fecemi = tx_fechact.Text.Substring(6, 4) + "-" + tx_fechact.Text.Substring(3, 2) + "-" + tx_fechact.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
                        string _secuen = lib.Right("00" + ctab.ToString(), 3);
                        string _codbaj = "RA" + "-" + tx_fechact.Text.Substring(6, 4) + tx_fechact.Text.Substring(3, 2) + tx_fechact.Text.Substring(0, 2);  // codigo comunicacion de baja
                        archi = rucclie + "-" + _codbaj + "-" + _secuen;
                        if (baja2TXT(tipdo, _fecemi, _codbaj, _secuen, ruta + archi, ctab, serie, corre) == true) retorna = true;
                    }
                }
            }
            if (provee == "factSunat")
            {
                string ruta = rutatxt + "DATA/";           //  rutatxt = %dirSistema%/sunat_archivos/sfs/ + "DATA/"
                string archi = "";
                string sep = "|";    // char sep = (char)31;
                int tfg = dataGridView1.Rows.Count - 1;

                if (accion == "alta")
                {
                    archi = rucclie + "-" + tipdo + "-" + serie + "-" + corre;
                    double vsubt = double.Parse(tx_subt.Text);      // sub total
                    double vigvt = double.Parse(tx_igv.Text);       // igv
                    double vflet = double.Parse(tx_flete.Text);     // total
                    double monDet = 0;

                    if (tx_dat_mone.Text == MonDeft)
                    {
                        if (double.Parse(tx_flete.Text) > (double.Parse(Program.valdetra)))
                        {
                            monDet = Math.Round(double.Parse(tx_flete.Text) * double.Parse(Program.pordetra) / 100, 2);
                        }
                    }
                    else
                    {
                        if (double.Parse(tx_flete.Text) > (double.Parse(Program.valdetra) / double.Parse(tx_tipcam.Text)))
                        {
                            // OJO, la detracción es en SOLES, la cuenta detracción en el BN es en soles
                            monDet = Math.Round((double.Parse(tx_flete.Text) * double.Parse(Program.pordetra) / 100) / double.Parse(tx_tipcam.Text), 2);
                        }
                    }
                    // 
                    if (generaCAB(tipdo, serie, corre, ruta + archi, sep, vsubt, vigvt, vflet, monDet) == false)  // Archivo: Cabecera (RRRRRRRRRRR-CC-XXXX-99999999.CAB)
                    {
                        MessageBox.Show("Error en cabecera del archivo plano","Error en CAB",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return retorna;
                    }
                    if (generaDET(tipdo, serie, corre, ruta + archi, sep, tfg) == false)    // Archivo: Detalle (RRRRRRRRRRR-CC-XXXX-999999999.DET)
                    {
                        MessageBox.Show("Error en detalle del archivo plano", "Error en DET", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return retorna;
                    }
                    if (generaTRI(tipdo, serie, corre, ruta + archi, sep, vsubt, vigvt, vflet) == false)    // Archivo: Tributos Generales (RRRRRRRRRRR-CC-XXXX-999999999.TRI)
                    {
                        MessageBox.Show("Error en tributos del archivo plano", "Error en TRI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return retorna;
                    }
                    if (generaLEY(tipdo, serie, corre, ruta + archi, sep) == false)    // Archivo: Leyendas (RRRRRRRRRRR-CC-XXXX-999999999.LEY)
                    {
                        MessageBox.Show("Error en leyendas del archivo plano", "Error en LEY", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return retorna;
                    }
                    if (tx_dat_tdv.Text == codfact)
                    {
                        if (generaPAG(tipdo, serie, corre, ruta + archi, sep) == false)    // Archivo: Datos de la forma de pago (RRRRRRRRRRR-CC-XXXX-999999999.PAG)
                        {
                            MessageBox.Show("Error en pagos del archivo plano", "Error en PAG", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (rb_credito.Checked == true)
                        {
                            if (generaDPA(tipdo, serie, corre, ruta + archi, sep) == false)    // Archivo: Detalles de la forma de pago al crédito (RRRRRRRRRRR-CC-XXXX-999999999.DPA)
                            {
                                MessageBox.Show("Error en pagos al crédito del archivo plano", "Error en DPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return retorna;
                            }
                        }
                    }
                    if (generaACA(tipdo, serie, corre, ruta + archi, sep, monDet) == false)    // Archivo: Adicionales de cabecera (RRRRRRRRRRR-CC-XXXX-999999999.ACA)
                    {
                        MessageBox.Show("Error en adicionales de CAB del archivo plano", "Error en ACA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return retorna;
                    }
                    if (chk_cunica.Checked == true)
                    {
                        if (generaSTC(tipdo, serie, corre, ruta + archi, sep, monDet) == false)    // Archivo: Detracciones - Servicio de transporte de Carga (RRRRRRRRRRR-CC-XXXX-99999999.STC)
                        {
                            MessageBox.Show("Error en detraciones del archivo plano", "Error en STC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (generaREL(tipdo, serie, corre, ruta + archi, sep, tfg) == false)    // Archivo: Documentos relacionados (RRRRRRRRRRR-CC-XXXX-999999999.REL)
                        {
                            MessageBox.Show("Error en docs relacionados del archivo plano", "Error en REL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (generaADE(tipdo, serie, corre, ruta + archi, sep) == false)    // Archivo: Adicionales de detalle (RRRRRRRRRRR-CC-XXXX-999999999.ADE)
                        {
                            MessageBox.Show("Error en adicionales de DET del archivo plano", "Error en ADE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (generaACV(tipdo, serie, corre, ruta + archi, sep) == false)    // Archivo: Adicionales de Cabecera Variable (RRRRRRRRRRR-CC-XXXX-999999999.ACV)
                        {
                            MessageBox.Show("Error en adicionales de ACV del archivo plano", "Error en ACV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (generaTRA(tipdo, serie, corre, ruta + archi, sep, monDet) == false)    // Archivo: Detracciones - Servicio de transporte de Carga - Detalle de tramos (De corresponder)   (RRRRRRRRRRR-CC-XXXX-99999999.TRA)
                        {
                            MessageBox.Show("Error en detraciones tramos del archivo plano", "Error en TRA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                        if (generaVEH(tipdo, serie, corre, ruta + archi, sep, monDet) == false)    // Archivo: Detracciones - Servicio de transporte de Carga - Servicio de transporte de Carga - Detalle del(os) Vehículo(s)  (RRRRRRRRRRR-CC-XXXX-99999999.VEH)
                        {
                            MessageBox.Show("Error en detraciones vehiculos del archivo plano", "Error en VEH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return retorna;
                        }
                    }
                    if (generaRTN(tipdo, serie, corre, ruta + archi, sep) == false)    // Archivo: Datos de la Retención del IGV (RRRRRRRRRRR-CC-XXXX-999999999.RTN)
                    {
                        MessageBox.Show("Error en retenciones del archivo plano", "Error en RTN", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return retorna;
                    }
                    //
                    retorna = true;
                }
                if (accion == "baja")
                {
                    //string _fecemi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   
                    string _fecemi = tx_fechact.Text.Substring(6, 4) + "-" + tx_fechact.Text.Substring(3, 2) + "-" + tx_fechact.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
                    string _secuen = lib.Right("00" + ctab.ToString(), 3);
                    string _codbaj = "RA" + "-" + tx_fechact.Text.Substring(6, 4) + tx_fechact.Text.Substring(3, 2) + tx_fechact.Text.Substring(0, 2);  // codigo comunicacion de baja
                    archi = rucclie + "-" + _codbaj + "-" + _secuen;
                    if (bajaTXT(tipdo, _fecemi, _codbaj, _secuen, ruta + archi, ctab, serie, corre) == true) retorna = true;
                }
            }
            if (provee == "factDirecta")
            {
                if (sunat_api(tipdo, tipoMoneda, tipoDocEmi) == true) retorna = true;
                else retorna = false;
            }
            return retorna;
        }

        #region peruSecure
        private bool bajaTXT(string tipdo, string _fecemi, string _codbaj, string _secuen, string file_path, int cuenta, string serie, string corre)    // horizont
        {
            bool retorna = false;
            string Prazsoc = nomclie.Trim();                                            // razon social del emisor
            string Prucpro = Program.ruc;                                               // Ruc del emisor
            string Pcrupro = "6";                                                       // codigo Ruc emisor
            string motivo = glosaAnul;          // "ANULACION";
            string fecdoc = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
            /* ********************************************** GENERAMOS EL TXT    ************************************* */
            string sep = "|";    // char sep = (char)31;
            StreamWriter writer;
            file_path = file_path + ".txt";
            writer = new StreamWriter(file_path);
            writer.WriteLine("G" + sep +
                Pcrupro + sep +                 // tipo de documento del emisor
                Prucpro + sep +                 // ruc emisor
                Prazsoc + sep +                 // razon social emisor
                fecdoc + sep +                 // fecha del documento dado de baja
                _codbaj + "-" + _secuen + sep +       // codigo identificador de la baja, secuencial dentro de cada día
                _fecemi + sep                   // fecha de la baja
            );
            writer.WriteLine("I" + sep +
                "1" + sep +
                tipdo + sep +
                serie + sep +
                corre + sep +
                motivo + sep
            );
            writer.Flush();
            writer.Close();
            retorna = true;
            return retorna;
        }
        private bool datosTXT(string tipdo, string serie, string corre, string file_path)       // peru secure
        {
            bool retorna = false;
            tcfe.Rows.Clear();
            DataRow row = tcfe.NewRow();
            row["_fecemi"] = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
            row["Prazsoc"] = nomclie.Trim();                                            // razon social del emisor
            row["Pnomcom"] = "";                                                        // nombre comercial del emisor
            row["ubigEmi"] = ubiclie;                                                   // UBIGEO DOMICILIO FISCAL
            row["Pdf_dir"] = Program.dirfisc.Trim();                                    // DOMICILIO FISCAL - direccion
            row["Pdf_urb"] = "-";                                                       // DOMICILIO FISCAL - Urbanizacion
            row["Pdf_pro"] = Program.provfis.Trim();                                    // DOMICILIO FISCAL - provincia
            row["Pdf_dep"] = Program.depfisc.Trim();                                    // DOMICILIO FISCAL - departamento
            row["Pdf_dis"] = Program.distfis.Trim();                                    // DOMICILIO FISCAL - distrito
            row["paisEmi"] = "PE";                                                      // DOMICILIO FISCAL - código de país
            row["Ptelef1"] = Program.telclte1.Trim();                                   // teléfono del emisor
            row["Pweb1"] = "";                                                          // página web del emisor
            row["Prucpro"] = Program.ruc;                                               // Ruc del emisor
            row["Pcrupro"] = "6";                                                       // codigo Ruc emisor
            row["_tipdoc"] = tipdo;                                                     // Tipo de documento de venta - 1 car
            row["_moneda"] = tipoMoneda;                                                // Moneda del doc. de venta - 3 car
            row["_sercor"] = serie + "-" + corre;                                       // Serie y correlat concatenado F001-00000001 - 13 car
            row["Cnumdoc"] = tx_numDocRem.Text;                                         // numero de doc. del cliente - 15 car
            row["Ctipdoc"] = tipoDocEmi;                                                // tipo de doc. del cliente - 1 car
            row["Cnomcli"] = tx_nomRem.Text.Trim();                                     // nombre del cliente - 100 car
            row["ubigAdq"] = tx_ubigRtt.Text;                                           // ubigeo del adquiriente - 6 car
            row["dir1Adq"] = tx_dirRem.Text.Trim();                                     // direccion del adquiriente 1
            row["dir2Adq"] = "";                                                        // direccion del adquiriente 2
            row["provAdq"] = tx_provRtt.Text.Trim();                                    // provincia del adquiriente
            row["depaAdq"] = tx_dptoRtt.Text.Trim();                                    // departamento del adquiriente
            row["distAdq"] = tx_distRtt.Text.Trim();                                    // distrito del adquiriente
            row["paisAdq"] = "PE";  // y si es boliviano o veneco???                    // pais del adquiriente
            row["_totoin"] = "0.00";                                                       // total operaciones inafectas
            row["_totoex"] = "0.00";                                                       // total operaciones exoneradas
            row["_toisc"] = "";                                                         // total impuesto selectivo consumo
            row["_totogr"] = tx_subt.Text;                                              // Total valor venta operaciones grabadas n(12,2)  15
            row["_totven"] = tx_flete.Text;                                             // Importe total de la venta n(12,2)             15
            row["tipOper"] = "0101";                                                    // tipo de operacion - 4 car
            row["codLocE"] = Program.codlocsunat;                                       // codigo local emisor
            //row["conPago"] = "01";                                                      // condicion de pago
            row["_codgui"] = "31";                                                      // Código de la guia de remision TRANSPORTISTA
            row["_scotro"] = dataGridView1.Rows[0].Cells[0].Value.ToString();           // serie y numero concatenado de la guia
            if (chk_cunica.Checked == true && rucsEmcoper.Contains(tx_numDocRem.Text))  // caso especial emcoper
            {
                row["codgrem"] = "99";                                                      // Código de la guia de remision REMITENTE -------------| esto aplica
                row["scogrem"] = dataGridView1.Rows[0].Cells[8].Value.ToString().Trim();           // serie y numero concatenado de la guia del remitente--| a cargas unicas
            }
            else
            {
                row["codgrem"] = "";
                row["scogrem"] = "";
            }
            row["obser1"] = tx_obser1.Text.Trim();                                      // observacion del documento
            //row["obser2"] = "";                                                         // mas observaciones
            row["maiAdq"] = tx_email.Text.Trim();                                       // correo del adquiriente
            row["teladq"] = tx_telc1.Text;                                              // telefono del adquiriente
            row["totImp"] = tx_igv.Text;                                                // total impuestos del documento
            //row["codImp"] = "1000";                                                     // codigo impuesto
            //row["nomImp"] = "IGV";                                                      // nombre del tipo de impuesto
            //row["tipTri"] = "VAT";                                                      // tipo de tributo
            row["monLet"] = tx_fletLetras.Text.Trim();                                  // monto en letras
            row["_horemi"] = "";                                                        // hora de emision del doc.venta
            row["_fvcmto"] = "";                                                        // fecha de vencimiento del doc.venta
            row["plaPago"] = "";                                                        // plazo de pago cuando es credito
            row["corclie"] = Program.mailclte;                                          // correo del emisor
            row["_morefD"] = "";                                                        // moneda de refencia para el tipo de cambio
            row["_monobj"] = "";                                                        // moneda objetivo del tipo de cambio
            row["_tipcam"] = "";                                                        // tipo de cambio con 3 decimales
            row["_fechca"] = "";                                                        // fecha del tipo de cambio
            row["d_medpa"] = "";                                                        // medio de pago de la detraccion (001 = deposito en cuenta)
            row["d_monde"] = "";                                                        // moneda de la detraccion
            row["d_conpa"] = "";                                                        // condicion de pago
            row["totdet"] = 0;                                                          // total detraccion
            row["d_porde"] = "";                                                        // porcentaje de detraccion
            row["d_valde"] = "";                                                        // valor de la detraccion
            row["d_codse"] = "";                                                        // codigo de servicio
            row["d_ctade"] = "";                                                        // cuenta detraccion BN
            //row["d_valre"] = "";                                                        // valor referencial
            //row["d_numre"] = "";                                                        // numero registro mtc del camion
            //row["d_confv"] = "";                                                        // config. vehicular del camion
            //row["d_ptori"] = "";                                                        // Pto de origen
            //row["d_ptode"] = "";                                                        // Pto de destino
            //row["d_vrepr"] = "";                                                        // valor referencial preliminar
            row["codleyt"] = "1000";                                                    // codigoLeyenda 1 - valor en letras
            row["codleyd"] = "";                                                        // codigo leyenda detraccion
            row["codobs"] = "107";                                                      // codigo del ose para las observaciones, caso carrion documentos origen del remitente
            row["_forpa"] = "";                                                         // glosa de forma de pago SUNAT
            row["_valcr"] = "";                                                         // valor credito
            row["_fechc"] = "";                                                         // fecha programada del pago credito
            if (tx_dat_tdv.Text == codfact)                          // campos solo para facturas "formas de pago"
            {
                if (rb_contado.Checked == true)
                {
                    row["conPago"] = "01";
                    row["_forpa"] = "Contado";
                    row["_valcr"] = "";
                    row["_fechc"] = row["_fecemi"];
                }
                else
                {
                    if (rb_credito.Checked == true)  // rb_no.Checked == true
                    {
                        if (tx_dat_dpla.Text.Trim() == "") tx_dat_dpla.Text = "7";
                        string fansi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
                        row["_fechc"] = DateTime.Parse(fansi).AddDays(double.Parse(tx_dat_dpla.Text)).Date.ToString("yyyy-MM-dd");        // fecha de emision + dias plazo credito
                        row["conPago"] = "02";
                        row["_forpa"] = "Credito";
                        row["_valcr"] = tx_flete.Text;
                        row["plaPago"] = int.Parse(tx_dat_dpla.Text).ToString();
                        row["_fvcmto"] = row["_fechc"];
                        row["fvencto"] = row["_fechc"];
                    }
                    else
                    {   // SI NO ESTA CHECK EN CONTADO TAMPOCO ESTA EN CREDITO, ES UN REGISTRO ANTERIOR A LA ADECUACION DEL 09/05/22 Y SE ....
                        if (rb_si.Checked == true)
                        {
                            row["conPago"] = "01";
                            row["_forpa"] = "Contado";
                            row["_valcr"] = "";
                            row["_fechc"] = row["_fecemi"];
                        }
                        else
                        {
                            if (tx_dat_dpla.Text.Trim() == "") tx_dat_dpla.Text = "7";
                            string fansi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
                            row["_fechc"] = DateTime.Parse(fansi).AddDays(double.Parse(tx_dat_dpla.Text)).Date.ToString("yyyy-MM-dd");        // fecha de emision + dias plazo credito
                            row["conPago"] = "02";
                            row["_forpa"] = "Credito";
                            row["_valcr"] = tx_flete.Text;
                            row["plaPago"] = int.Parse(tx_dat_dpla.Text).ToString();
                            row["_fvcmto"] = row["_fechc"];
                            row["fvencto"] = row["_fechc"];
                        }
                    }
                }
            }
            if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)     // factura de cargas unicas ... 
            {
                row["cu_cpapp"] = "PE";
                row["cu_ubipp"] = tx_dat_upo.Text;                   // 03    Ubigeo del punto de partida 
                row["cu_deppp"] = tx_dp_dep.Text;                   // 04    Departamento del punto de partida
                row["cu_propp"] = tx_dp_pro.Text;                   // 05    Provincia del punto de partida
                row["cu_dispp"] = tx_dp_dis.Text;                   // 06    Distrito del punto de partida
                row["cu_urbpp"] = "-";                              // 07    Urbanización del punto de partida
                row["cu_dirpp"] = tx_dat_dpo.Text;                   // 08    Dirección detallada del punto de partida
                row["cu_cppll"] = "PE";                              // 09    Código país del punto de llegada
                row["cu_ubpll"] = tx_dat_upd.Text;                   // 10    Ubigeo del punto de llegada
                row["cu_depll"] = tx_dd_dep.Text;                   // 11    Departamento del punto de llegada
                row["cu_prpll"] = tx_dd_pro.Text;                   // 12    Provincia del punto de llegada
                row["cu_dipll"] = tx_dd_dis.Text;                   // 13    Distrito del punto de llegada
                //row["cu_urbpl"] = "-";                              // 14    Urbanización del punto de llegada
                row["cu_ddpll"] = tx_dat_dpd.Text;                   // 15    Dirección detallada del punto de llegada
                row["cu_placa"] = tx_pla_placa.Text;                   // 16    Placa del Vehículo
                row["cu_coins"] = tx_pla_autor.Text;                   // 17    Constancia de inscripción del vehículo o certificado de habilitación vehicular
                row["cu_marca"] = "";                   // 18    Marca del Vehículo 
                row["cu_breve"] = "";                   // 19    Nro.de licencia de conducir
                row["cu_ructr"] = tx_rucT.Text;                   // 20    RUC del transportista
                row["cu_nomtr"] = tx_razonS.Text;                   // 21    Razón social del Transportista
                row["cu_modtr"] = texmotran;                    // 22    Modalidad de Transporte
                row["cu_pesbr"] = "";   // tx_cetm.Text;        // 23    Total Peso Bruto
                row["cu_motra"] = codtxmotran;                   // 24    Código de Motivo de Traslado
                row["cu_fechi"] = tx_fecini.Text;               // 25    Fecha de Inicio de Traslado
                row["cu_remtc"] = "";                           // 26    Registro MTC
                row["cu_nudch"] = tx_dniChof.Text;              // 27    Nro.Documento del conductor
                row["cu_tidch"] = "1";                          // 28    Tipo de Documento del conductor
                row["cu_plac2"] = "";                           // 29    Placa del Vehículo secundario
                row["cu_insub"] = "";                           // 30   Indicador de subcontratación
            }
            /* *********************   calculo y campos de detracciones   ****************************** */
            if (double.Parse(tx_flete.Text) > double.Parse(Program.valdetra) && tx_dat_tdv.Text == codfact && tx_dat_mone.Text == MonDeft)    // soles
            {

                // Están sujetos a las detracciones los servicios de transporte de bienes por vía terrestre gravado con el IGV, 
                // siempre que el importe de la operación o el valor referencial, según corresponda, sea mayor a 
                // S/ 400.00 o su equivalente en dólares ........ DICE SUNAT
                // ctadetra;                                                            // numeroCtaBancoNacion
                // valdetra;                                                            // monto a partir del cual tiene detraccion la operacion
                // coddetra;                                                            // codigoDetraccion
                // pordetra;                                                            // porcentajeDetraccion
                row["d_medpa"] = "001";                                                 // medio de pago de la detraccion (001 = deposito en cuenta)
                row["d_monde"] = "PEN"; // MonDeft;                                  // moneda de la detraccion
                row["d_conpa"] = "CONTADO";                                         // condicion de pago
                row["d_porde"] = Program.pordetra;                         // porcentaje de detraccion
                row["d_valde"] = Program.valdetra;                         // valor de la detraccion
                row["d_codse"] = Program.coddetra;                         // codigo de servicio
                row["d_ctade"] = Program.ctadetra;                         // cuenta detraccion BN
                //d_valre = "0";                                      // valor referencial
                //d_numre = "";                // numero registro mtc del camion
                //d_confv = "";                // config. vehicular del camion
                //d_ptori = "";                // Pto de origen
                //d_ptode = "";                // Pto de destino
                //d_vrepr = "0";               // valor referencial preliminar
                row["codleyt"] = "1000";            // codigoLeyenda 1 - valor en letras
                row["totdet"] = Math.Round(double.Parse(tx_flete.Text) * double.Parse(Program.pordetra) / 100, 2);    // totalDetraccion
                row["codleyd"] = "2006";
                row["tipOper"] = "1001";
                glosdet = glosdet + " " + row["d_ctade"];                // leyenda de la detración
                row["glosdet"] = glosdet;
            }
            if (tx_dat_mone.Text != MonDeft)
            {
                row["_morefD"] = tx_dat_monsunat.Text;                                      // moneda de refencia para el tipo de cambio
                row["_monobj"] = "PEN";        //tipoMoneda;                                // moneda objetivo del tipo de cambio
                row["_tipcam"] = tx_tipcam.Text;                                            // tipo de cambio con 3 decimales
                //_fechca = string.Format("{0:yyyy-MM-dd}", tx_fechope.Text);          // fecha del tipo de cambio
                row["_fechca"] = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
                if (double.Parse(tx_flete.Text) > (double.Parse(Program.valdetra) / double.Parse(tx_tipcam.Text)) && tx_dat_tdv.Text == codfact)
                {
                    row["d_medpa"] = "001";                                    // medio de pago de la detraccion (001 = deposito en cuenta)
                    row["d_monde"] = "PEN";                                    // moneda de la detraccion SIEMPRE ES PEN moneda nacional
                    row["d_conpa"] = "CONTADO";                                // condicion de pago
                    row["d_porde"] = Program.pordetra;                         // porcentaje de detraccion
                    row["d_valde"] = Program.valdetra;                         // valor de la detraccion
                    row["d_codse"] = Program.coddetra;                         // codigo de servicio
                    row["d_ctade"] = Program.ctadetra;                         // cuenta detraccion BN
                    //d_valre = "0";                                      // valor referencial
                    //d_numre = "";                // numero registro mtc del camion
                    //d_confv = "";                // config. vehicular del camion
                    //d_ptori = "";                // Pto de origen
                    //d_ptode = "";                // Pto de destino
                    //d_vrepr = "0";               // valor referencial preliminar
                    row["codleyt"] = "1000";            // codigoLeyenda 1 - valor en letras
                    row["codleyd"] = "2006";
                    row["tipOper"] = "1001";
                    if (tx_valref1.Text.Trim() != "" && double.Parse(tx_valref1.Text) > 0)   // usamos en los casos en que el cliente quiere el calculo de la detraccion según el valor referencial o no
                    {
                        row["totdet"] = Math.Round(double.Parse(tx_valref1.Text) * double.Parse(Program.pordetra) / 100, 2);    // totalDetraccion en base al valor referencial
                    }
                    else row["totdet"] = Math.Round(double.Parse(tx_fletMN.Text) * double.Parse(Program.pordetra) / 100, 2);    // totalDetraccion "normal"
                    //
                    glosdet = glosdet + " " + row["d_ctade"];                // leyenda de la detración
                    row["glosdet"] = glosdet;
                }
            }
            retorna = true;
            tcfe.Rows.Add(row);

            return retorna;
        }
        private bool datDetxt(string tipdo, string serie, string corre)                         // peru secure
        {
            bool retorna = false;
            tdfe.Rows.Clear();
            int tfg = ((dataGridView1.Rows.Count -1) == int.Parse(v_mfildet)) ? int.Parse(v_mfildet) : dataGridView1.Rows.Count - 1;
            for (int s = 0; s < tfg; s++)  // int s = 0; s < dataGridView1.Rows.Count - 1; s++
            {
                //glosser2 = dataGridView1.Rows[s].Cells["OriDest"].Value.ToString() + " - " + tx_totcant.Text.Trim() + " " + tx_dat_nombd.Text; // " Bultos"; 
                glosser2 = dataGridView1.Rows[s].Cells["OriDest"].Value.ToString() + " - " +
                    dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + 
                    dataGridView1.Rows[s].Cells["umed"].Value.ToString() + " " + dataGridView1.Rows[s].Cells["guiasclte"].Value.ToString();
                DataRow row = tdfe.NewRow();
                row["Idatper"] = "";                                                        // datos personalizados del item
                row["Idescri"] = glosser + " " + dataGridView1.Rows[s].Cells["Descrip"].Value.ToString() + " " + glosser2;   // Descripcion
                row["Icantid"] = "1.00";                                                    // Cantidad de items   n(12,3)         16
                if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)               // datos para el pdf si es carga unica
                {
                    char saltoL = (char)8;
                    char saltoL5 = (char)5;
                    int ld = 0;
                    if (rucsEmcoper.Contains(tx_numDocRem.Text))    // caso especial Empcoper 27/12/2022
                    {
                        if (dataGridView1.Rows.Count > 2)
                        {
                            row["Idescri"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString();   // Descripcion
                            ld = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Length;
                            if (ld > ccf_pdf)
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(0, ccf_pdf) + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(ccf_pdf, ld - ccf_pdf);   // + saltoL + saltoL5 +
                                    //dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text;
                            }
                            else
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString();   // + saltoL + saltoL5 +
                                    //dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text;
                            }
                            //row["Idescri"] = glosser;
                            row["Icantid"] = Math.Round((double.Parse(dataGridView1.Rows[s].Cells["Cant"].Value.ToString()) * double.Parse(tx_cetm.Text)) / double.Parse(tx_totcant.Text), 2);
                        }
                        else
                        {
                            ld = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Length;
                            if (ld > ccf_pdf)
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(0, ccf_pdf) + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(ccf_pdf, ld - ccf_pdf);   // + saltoL + saltoL5 +
                                    //dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text;
                            }
                            else
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString(); //+ saltoL + saltoL5 +
                                    //dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text;
                            }
                            //row["Idescri"] = glosser;
                            row["Idescri"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString();   // Descripcion
                            row["Icantid"] = Math.Round((double.Parse(dataGridView1.Rows[s].Cells["Cant"].Value.ToString()) * double.Parse(tx_cetm.Text)) / double.Parse(tx_totcant.Text), 2);
                        }
                    }
                    else
                    {
                        if (dataGridView1.Rows.Count > 2)
                        {
                            ld = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Length;
                            if (ld > ccf_pdf)
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(0, ccf_pdf) + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(ccf_pdf, ld - ccf_pdf) + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text + saltoL + saltoL5 +
                                    "GUIA TRANSPORTISTA " + dataGridView1.Rows[s].Cells["guias"].Value.ToString() + saltoL + saltoL5 +
                                    "GUIA REMITENTE " + dataGridView1.Rows[s].Cells["guiasclte"].Value.ToString();  // tx_totcant.Text
                            }
                            else
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString() + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text + saltoL + saltoL5 +
                                    "GUIA TRANSPORTISTA " + dataGridView1.Rows[s].Cells["guias"].Value.ToString() + saltoL + saltoL5 +
                                    "GUIA REMITENTE " + dataGridView1.Rows[s].Cells["guiasclte"].Value.ToString();  // tx_totcant.Text
                            }
                            row["Idescri"] = glosser;
                            row["Icantid"] = Math.Round((double.Parse(dataGridView1.Rows[s].Cells["Cant"].Value.ToString()) * double.Parse(tx_cetm.Text)) / double.Parse(tx_totcant.Text), 2);
                        }
                        else
                        {
                            ld = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Length;
                            if (ld > ccf_pdf)
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(0, ccf_pdf) + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Descrip"].Value.ToString().Substring(ccf_pdf, ld - ccf_pdf) + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text + saltoL + saltoL5 +
                                    "GUIA TRANSPORTISTA " + dataGridView1.Rows[s].Cells["guias"].Value.ToString() + saltoL + saltoL5 +
                                    "GUIA REMITENTE " + dataGridView1.Rows[s].Cells["guiasclte"].Value.ToString();  // tx_totcant.Text
                            }
                            else
                            {
                                row["Idatper"] = dataGridView1.Rows[s].Cells["Descrip"].Value.ToString() + saltoL + saltoL5 +
                                    dataGridView1.Rows[s].Cells["Cant"].Value.ToString() + " " + tx_dat_nombd.Text + saltoL + saltoL5 +
                                    "GUIA REMITENTE " + dataGridView1.Rows[s].Cells["guiasclte"].Value.ToString();  // tx_totcant.Text
                            }
                            row["Idescri"] = glosser;
                            row["Icantid"] = Math.Round((double.Parse(dataGridView1.Rows[s].Cells["Cant"].Value.ToString()) * double.Parse(tx_cetm.Text)) / double.Parse(tx_totcant.Text), 2);
                        }
                    }
                }
                if (dataGridView1.Rows[s].Cells["valorel"].Value == null || dataGridView1.Rows[s].Cells["valorel"].Value.ToString().Trim() == "0.000" || dataGridView1.Rows[s].Cells["valorel"].Value.ToString().Trim() == "")
                {
                    row["_msigv"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) - (double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) / (1 + (double.Parse(v_igv) / 100))), 2);

                    if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)
                    {
                        //row["Ipreuni"] = (double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) / double.Parse(row["Icantid"].ToString())).ToString("#0.0000000000");    // * (1 + (double.Parse(v_igv) / 100)).ToString("#0.0000000000");
                        //row["Ivaluni"] = (double.Parse(row["Ipreuni"].ToString()) / (1 + (double.Parse(v_igv) / 100))).ToString("#0.0000000000");
                        row["Ivaluni"] = ((double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) - (double)row["_msigv"]) / double.Parse(tx_cetm.Text)).ToString("#0.0000000000");
                        row["Ipreuni"] = ((double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) - (double)row["_msigv"]) / double.Parse(tx_cetm.Text) * (1 + (double.Parse(v_igv) / 100))).ToString("#0.0000000000");    // * (1 + (double.Parse(v_igv) / 100)).ToString("#0.0000000000");
                    }
                    else
                    {
                        row["Ivaluni"] = (double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) - (double)row["_msigv"]).ToString("#0.0000000000");
                        row["Ipreuni"] = double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()).ToString("#0.0000000000");     // Precio de venta unitario CON IGV
                    }
                    if (tx_dat_mone.Text != MonDeft && dataGridView1.Rows[s].Cells["codmondoc"].Value.ToString() == MonDeft)   // 
                    {
                        //row["_msigv"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) / (1 + (double.Parse(v_igv) / 100)) / double.Parse(tx_tipcam.Text), 2);
                        row["_msigv"] = Math.Round(((double)row["_msigv"] / double.Parse(tx_tipcam.Text)), 2);
                        row["Ipreuni"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) / double.Parse(tx_tipcam.Text), 2).ToString("#0.0000000000");
                        row["Ivaluni"] = ((double)row["Ivaluni"] / double.Parse(tx_tipcam.Text)).ToString("#0.0000000000");
                    }
                    if (tx_dat_mone.Text == MonDeft && dataGridView1.Rows[s].Cells["codmondoc"].Value.ToString() != MonDeft)
                    {
                        row["_msigv"] = Math.Round((double)row["_msigv"] * double.Parse(tx_tipcam.Text), 2);
                        row["Ipreuni"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) * double.Parse(tx_tipcam.Text), 2).ToString("#0.0000000000");
                        //row["Ivaluni"] = ((double)row["Ivaluni"] * double.Parse(tx_tipcam.Text)).ToString("#0.0000000000"); // 30/06/2023
                        row["Ivaluni"] = (double.Parse(row["Ivaluni"].ToString()) * double.Parse(tx_tipcam.Text)).ToString("#0.0000000000");
                    }
                }
                else
                {       // tiene descuento la fila, aplica valorel
                    row["_msigv"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) - (double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) / (1 + (double.Parse(v_igv) / 100))), 2);
                    if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)
                    {
                        row["Ivaluni"] = ((double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) - (double)row["_msigv"]) / double.Parse(tx_cetm.Text)).ToString("#0.0000000000");
                        row["Ipreuni"] = ((double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) - (double)row["_msigv"]) / double.Parse(tx_cetm.Text) * (1 + (double.Parse(v_igv) / 100))).ToString("#0.0000000000");    // * (1 + (double.Parse(v_igv) / 100)).ToString("#0.0000000000");
                    }
                    else
                    {
                        row["Ipreuni"] = double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()).ToString("#0.0000000000");     // Precio de venta unitario CON IGV
                        row["Ivaluni"] = (double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) - (double)row["_msigv"]).ToString("#0.0000000000");
                    }
                    if (tx_dat_mone.Text != MonDeft && dataGridView1.Rows[s].Cells["codmondoc"].Value.ToString() == MonDeft)   // 
                    {
                        //row["_msigv"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) / (1 + (double.Parse(v_igv) / 100)) / double.Parse(tx_tipcam.Text), 2);
                        row["_msigv"] = Math.Round(((double)row["_msigv"] / double.Parse(tx_tipcam.Text)), 2);
                        row["Ipreuni"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) / double.Parse(tx_tipcam.Text), 2).ToString("#0.0000000000");
                        row["Ivaluni"] = ((double)row["Ivaluni"] / double.Parse(tx_tipcam.Text)).ToString("#0.0000000000");
                    }
                    if (tx_dat_mone.Text == MonDeft && dataGridView1.Rows[s].Cells["codmondoc"].Value.ToString() != MonDeft)
                    {
                        row["_msigv"] = Math.Round((double)row["_msigv"] * double.Parse(tx_tipcam.Text), 2);
                        row["Ipreuni"] = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valorel"].Value.ToString()) * double.Parse(tx_tipcam.Text), 2).ToString("#0.0000000000");
                        row["Ivaluni"] = ((double)row["Ivaluni"] * double.Parse(tx_tipcam.Text)).ToString("#0.0000000000");
                    }
                }
                row["Inumord"] = (s + 1).ToString();                                        // numero de orden del item             5
                row["Iumeded"] = "ZZ";                                                      // Unidad de medida                     3
                row["Icodprd"] = " - ";                                                     // codigo del producto del cliente
                row["Icodpro"] = "";                                                        // codigo del producto SUNAT                          30
                row["Icodgs1"] = "";                                                        // codigo del producto GS1
                row["Icogtin"] = "";                                                        // tipo de producto GTIN
                row["Inplaca"] = "";                                                        // numero placa de vehiculo
                row["Idesglo"] = "";                                                        // descricion de la glosa del item 
                row["Ivalref"] = "";                                                        // valor referencial del item cuando la venta es gratuita
                row["Iigvite"] = row["_msigv"];
                //row["Imonbas"] = row["Ivaluni"];                                            // monto base (valor sin igv * cantidad)
                //row["Isumigv"] = row["Iigvite"];                                            // Sumatoria de igv
                row["Itasigv"] = Math.Round(double.Parse(v_igv), 2).ToString("#0.00");      // tasa del igv
                row["Icatigv"] = "10";                                                      // Codigo afectacion al igv                    2
                row["Icodtri"] = "1000";                                                    // codigo del tributo del item => igv = 1000
                //row["Iindgra"] = "";                                                      // indicador de gratuito
                row["Iiscmba"] = "";                                                        // ISC monto base
                row["Iiscmon"] = "";                                                        // ISC monto del tributo
                row["Icbper1"] = "";
                row["Icbper2"] = "";
                row["Icbper3"] = "";
                row["Iisctas"] = "";                                                        // ISC tasa del tributo
                row["Iisctip"] = "";                                                        // ISC tipo de sistema
                row["Iotrtri"] = "";                                                        // otros tributos monto base
                row["Iotrlin"] = "";                                                        // otros tributos monto unitario
                row["Itdscto"] = "0.00";                                                    // descuento por item
                row["Iincard"] = "2";                                                       // indicador de cargo/descuento => 2=No aplica cargo/descuento
                row["Icodcde"] = "";
                row["Ifcades"] = "";
                row["Imoncde"] = "";
                row["Imobacd"] = "";
                row["Iotrtas"] = "";                                                        // otros tributos tasa del tributo
                //row["Iotrsis"] = "";                                                        // otros tributos tipo de sistema
                //row["Ivalvta"] = Math.Round(double.Parse(row["Ipreuni"].ToString()),10).ToString("#0.00");       // Valor de venta del ítem
                if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)
                {
                    row["Ivalvta"] = Math.Round(double.Parse(tx_cetm.Text) * ((double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) - (double)row["_msigv"]) / double.Parse(tx_cetm.Text)), 2).ToString("#0.00");    // (double)row["Ivaluni"]
                }
                else
                {
                    row["Ivalvta"] = Math.Round(double.Parse(row["Ivaluni"].ToString()), 10).ToString("#0.00");       // Valor de venta del ítem
                }
                retorna = true;
                tdfe.Rows.Add(row);
            }
            return retorna;
        }
        private bool generaTxt(string tipdo, string serie, string corre, string file_path)      // peru secure
        {
            bool retorna = false;
            DataRow row = tcfe.Rows[0];

            char sep = (char)31;
            StreamWriter writer;
            file_path = file_path + ".txt";
            writer = new StreamWriter(file_path);
            writer.WriteLine("CONTROL" + sep + "31007" + sep);
            writer.WriteLine("ENCABEZADO" + sep +
                "" + sep +                                      // 2 id del erp emisor
                row["_tipdoc"] + sep +                          // 3 Tipo de Comprobante Electrónico
                row["_sercor"] + sep +                          // 4 Numeración de Comprobante Electrónico
                row["_fecemi"] + sep +                          // 5 Fecha de emisión
                "" + sep +                                      // 6 Hora de emision V.31006
                row["_moneda"] + sep +                          // 7 Tipo de moneda
                "" + sep + "" + sep + "" + sep +                // 8,9,10, tcambio, vendedor, unidad de negocio
                row["tipOper"] + sep +                          // 11 Tipo de Operación
                "" + sep + "" + sep + "" + sep +                // 12,13,14 monto anticipos, numero, ruc emisor,
                "" + sep + "" + sep + "" + sep +                // 15,16,17 total anticipos
                "" + sep + "" + sep + "" + sep + "" + sep +     // 18,19,20,21 Tipo de nota(Crédito/Débito),Tipo del documento afectado,Numeración de documento afectado,Motivo del documento afectado
                row["conPago"] + sep +                          // 22 Condición de Pago
                row["plaPago"] + sep +                          // 23 Plazo de Pago
                row["fvencto"] + sep +                          // 24 Fecha de vencimiento
                "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep +   // Forma de Pago del 1 al 6
                "" + sep + "" + sep +                           // 31,32 Número del pedido, Número de la orden de compra
                "" + sep + "" + sep + "" + sep + "" + sep +     // 33,34,35,36 sector publico: Numero de Expediente,Código de unidad ejecutora, Nº de contrato,Nº de proceso de selección
                row["_codgui"] + sep + row["_scotro"] + sep +   // 37,38 tipo de guia y serie+numero
                row["codgrem"] + sep + row["scogrem"] + sep +   // 39,40 Tipo otro doc relacionado, numero doc relacionado .. Guía remitente y numero ... SOLO PARA EMCOPER 
                "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep +  // varios campos opcionales
                "" + sep +                                      // 48 pais de uso si es 0201 o 0208 V.3006
                row["obser1"] + sep + row["obser2"] + sep + "" + sep +    // 49,50,51 observaciones del documento 1 y 2
                row["_totogr"] + sep +                          // 52 Total operaciones gravadas
                row["_totoin"] + sep +                          // 53 Total operaciones inafectas
                row["_totoex"] + sep +                          // 54 total operaciones exoneradas
                "0.00" + sep +                                  // 55 Total operaciones exportacion
                "0.00" + sep +                                  // 56 total operaciones gratuitas gratuitas
                "0.00" + sep +                                  // 57 monto impuestos operaciones gratuitas V.3006
                "" + sep +                                      // 58 Monto Fondo Inclusión Social Energético FISE
                row["totImp"] + sep +                           // 59 Total IGV
                row["_toisc"] + sep +                           // 60 Total ISC
                "" + sep + "" + sep + "" + sep + "" + sep + "" + sep +  // 61,62,63,64,65  indicador imp,cod.motivo,factor dscto,monto dscto,monto base
                "" + sep + "0.00" + sep + "0.00" + sep +        // 66,67,68 Total otros tributos,Total otros cargos
                "0.00" + sep +                                  // 69 Descuento Global
                "0.00" + sep +                                  // 70 Total descuento
                row["_totven"] + sep +                          // 71 Importe total de la venta
                "" + sep +                                      // 72 monto para redondeo del importe total V.3006
                row["monLet"] + sep +                           // 73 Leyenda: Monto expresado en Letras
                "" + sep +                                      // 74 Leyenda: Transferencia gratuita o servicio prestado gratuitamente
                "" + sep +                                      // 75 Leyenda: Bienes transferidos en la Amazonía
                "" + sep +                                      // 76 Leyenda: Servicios prestados en la Amazonía
                "" + sep +                                      // 77 Leyenda: Contratos de construcción ejecutados en la Amazonía
                "" + sep + "" + sep + "" + sep);                // 78,79,80 Leyenda: Exoneradas,Leyenda: Inafectas,Leyenda: Emisor itinerante
            if (row["_forpa"].ToString() == "Credito")
            {
                string fansi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
                string _fechc = DateTime.Parse(fansi).AddDays(double.Parse(tx_dat_dpla.Text)).Date.ToString("yyyy-MM-dd");        // fecha de emision + dias plazo credito
                string vneto = "0";
                if (cmb_mon.SelectedIndex == 0) // operacion en soles
                {
                    vneto = Math.Round(double.Parse(row["_totven"].ToString()) - double.Parse(row["totdet"].ToString()),2).ToString("#0.00"); 
                }
                else
                {                               // operacion en dolares
                    vneto = Math.Round(double.Parse(row["_totven"].ToString()) - double.Parse(row["totdet"].ToString()) / double.Parse(tx_tipcam.Text), 2).ToString("#0.00");
                }
                writer.WriteLine("ENCABEZADO-CREDITO" + sep +
                    vneto + sep                        // OJO, las ventas a credito, son totales, no tenemos pagos parciales facturados, factura = pago total
                    );
                //  row["_totven"] + sep
                writer.WriteLine("DETALLE-CREDITO" + sep +
                    "Cuota001" + sep +                          // 2 numero de cuota
                    vneto + sep +                               // 3 monto de la cuota, OJO, monto NETO pendiente, osea precio de venta - detracciones, retenciones, pagos anticipados, etc
                    _fechc + sep                                // 4 fecha del pago
                    );
                // row["_totven"] + sep +
            }
            // datos del traslados de bienes
            if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)     // factura de cargas unicas ... 
            {
                writer.WriteLine("ENCABEZADO-TRASLADOBIENES" + sep +
                    row["cu_cpapp"] + sep +                   // 02    Código país del punto de origen
                    row["cu_ubipp"] + sep +                   // 03    Ubigeo del punto de partida 
                    row["cu_deppp"] + sep +                   // 04    Departamento del punto de partida
                    row["cu_propp"] + sep +                   // 05    Provincia del punto de partida
                    row["cu_dispp"] + sep +                   // 06    Distrito del punto de partida
                    row["cu_urbpp"] + sep +                   // 07    Urbanización del punto de partida
                    row["cu_dirpp"] + sep +                   // 08    Dirección detallada del punto de partida
                    row["cu_cppll"] + sep +                   // 09    Código país del punto de llegada
                    row["cu_ubpll"] + sep +                   // 10    Ubigeo del punto de llegada
                    row["cu_depll"] + sep +                   // 11    Departamento del punto de llegada
                    row["cu_prpll"] + sep +                   // 12    Provincia del punto de llegada
                    row["cu_dipll"] + sep +                   // 13    Distrito del punto de llegada
                    "-" + sep +                               // 14    Urbanización del punto de llegada
                    row["cu_ddpll"] + sep +                   // 15    Dirección detallada del punto de llegada
                    row["cu_placa"] + sep +                   // 16    Placa del Vehículo
                    row["cu_coins"] + sep +                   // 17    Constancia de inscripción del vehículo o certificado de habilitación vehicular
                    row["cu_marca"] + sep +                   // 18    Marca del Vehículo
                    row["cu_breve"] + sep +                   // 19    Nro.de licencia de conducir
                    row["cu_ructr"] + sep +                   // 20    RUC del transportista
                    row["cu_nomtr"] + sep +                   // 21    Razón social del Transportista
                    row["cu_modtr"] + sep +                   // 22    Modalidad de Transporte
                    row["cu_pesbr"] + sep +                   // 23    Total Peso Bruto
                    row["cu_motra"] + sep +                   // 24    Código de Motivo de Traslado
                    row["cu_fechi"] + sep +                   // 25    Fecha de Inicio de Traslado
                    row["cu_remtc"] + sep +                   // 26    Registro MTC
                    row["cu_nudch"] + sep +                   // 27    Nro.Documento del conductor
                    row["cu_tidch"] + sep +                   // 28    Tipo de Documento del conductor
                    row["cu_plac2"] + sep +                   // 29    Placa del Vehículo secundario
                    row["cu_insub"]                           // 30   Indicador de subcontratación
                );
            }
            //
            writer.WriteLine("ENCABEZADO-EMISOR" + sep +
                row["Prucpro"] + sep +                          // 2 ruc emisor
                row["Prazsoc"] + sep +                          // 3 razon social emisor
                row["Pnomcom"] + sep +                          // 4 nombre comercial emisor
                row["paisEmi"] + sep +                          // 5 pais del emisor
                row["ubigEmi"] + sep +                          // 6 ubigeo del emisor
                row["Pdf_dep"] + sep +                          // 7 Departamento
                row["Pdf_pro"] + sep +                          // 8 Provincia
                row["Pdf_dis"] + sep +                          // 9 Distrito
                row["Pdf_urb"] + sep +                          // 10 Urbanización
                row["Pdf_dir"] + sep +                          // 11 Dirección detallada
                "" + sep +                                      // 12 Punto de emisión ... aca deberia ser la serie asignada por sunat al local emisor
                "" + sep +                                      // 13 Dirección de emisión ... aca deberia ir la direc del local emisor
                row["codLocE"] + sep +                          // 14 codigo local anexo sunat
                row["Ptelef1"] + sep +                          // 15 Teléfono
                "" + sep +                                      // 16 Fax
                row["corclie"] + sep);                          // 17 Correo-Emisor
            if (row["Ctipdoc"].ToString() == "0") row["Cnumdoc"] = "";
            writer.WriteLine("ENCABEZADO-RECEPTOR" + sep +
                row["Ctipdoc"] + sep +                          // 2 Tipo de documento del cliente
                row["Cnumdoc"] + sep +                          // 3 Nro. Documento del cliente
                row["Cnomcli"] + sep +                          // 4 Razón social del cliente
                "" + sep +                                      // 5 Identificador del cliente
                "" + sep +                                      // 6 Tipo de documento del receptor  V.3006 
                "" + sep +                                      // 7 Numero de documento del receptor  V.3006 
                row["paisAdq"] + sep +                          // 8 Código país
                row["ubigAdq"] + sep +                          // 9 Ubigeo
                row["depaAdq"] + sep +                          // 10 Departamento
                row["provAdq"] + sep +                          // 11 Provincia
                row["distAdq"] + sep +                          // 12 Distrito
                "" + sep +                                      // 13 Urbanización   dir2Adq
                row["dir1Adq"] + sep +                          // 14 Dirección
                row["maiAdq"] + sep);                           // 15 Correo-Receptor
            //
            // datos de percepcion
            // datos de retencion
            // datos de anticipos
            // 
            if (row["totdet"].ToString() != "0")
            {
                writer.WriteLine("ENCABEZADO-DETRACCION" + sep +
                    row["d_porde"] + sep +                      // 2 porcentaje de detraccion
                    row["totdet"] + sep +                       // 3 valor de la detraccion
                    row["d_codse"] + sep +                      // 4 codigo de servicio
                    row["d_ctade"] + sep +                      // 5 cuenta detraccion BN
                    row["d_medpa"] + sep +                      // 6 medio de pago
                    row["glosdet"] + sep);                      // 7 leyenda de la detración
            }
            // ***** DETALLE ***** //
            if (chk_cunica.Checked == true && tx_dat_tdv.Text == codfact)
            {
                //DataRow rdrow = tdfe.Rows[0];
                // tdfe.Rows[0].ItemArray[4].ToString().Replace(vint_gg," ") + sep +                    // 6 Descripcion 
                // tx_totcant.Text + " " + tdfe.Rows[0].ItemArray[8].ToString() + sep +        // 7 descricion de la glosa del item   250
                if (rucsEmcoper.Contains(tx_numDocRem.Text))
                {
                    foreach (DataRow rdrow in tdfe.Rows)
                    {
                        writer.WriteLine(
                            "ITEM" + sep +
                            rdrow["Inumord"] + sep +        // "1"      // 2 orden
                            "" + sep +                    // 3 Datos personilazados del item       
                            "TNE" + sep +                               // 4 Unidad de medida                    3
                            rdrow["Icantid"] + sep +                    // tx_cetm.Text.Trim()             // 5 Cantidad de items             n(12,2)
                            rdrow["Idatper"] + sep +                    // 6 Descripcion                       500
                            "" + sep +                                  // 7 descricion de la glosa del item   250
                            rdrow["Icodprd"] + sep +                    // 8 codigo del producto del cliente    30
                            rdrow["Icodpro"] + sep +                    // 9 codigo del producto SUNAT           8
                            rdrow["Icodgs1"] + sep +                    // 10 codigo del producto GS1           14
                            rdrow["Icogtin"] + sep +                    // 11 tipo de producto GTIN             14
                            rdrow["Inplaca"] + sep +                    // 12 numero placa de vehiculo
                            rdrow["Ivaluni"] + sep +                    // 13 Valor unitario del item SIN IMPUESTO 
                            rdrow["Ipreuni"] + sep +                    // 14 Precio de venta unitario CON IGV
                            rdrow["Ivalref"] + sep +                    // 15 valor referencial del item cuando la venta es gratuita
                            rdrow["Iigvite"] + sep +                     // 16 monto igv   .. ."_msigv"
                            rdrow["Icatigv"] + sep +                    // 17 tipo/codigo de afectacion igv
                            rdrow["Itasigv"] + sep +                    // 18 tasa del igv
                            rdrow["Iigvite"] + sep +                    // 19 monto IGV del item
                            rdrow["Icodtri"] + sep +                    // 20 codigo del tributo por item
                            rdrow["Iiscmba"] + sep +                    // 21 ISC monto base
                            rdrow["Iisctas"] + sep +                    // 22 ISC tasa del tributo
                            rdrow["Iisctip"] + sep +                    // 23 ISC tipo de afectacion
                            rdrow["Iiscmon"] + sep +                    // 24 ISC monto del tributo
                            rdrow["Icbper1"] + sep +                    // 25 indicador de afecto a ICBPER
                            rdrow["Icbper2"] + sep +                    // 26 monto unitario de ICBPER
                            rdrow["Icbper3"] + sep +                    // 27 monto total ICBPER del item
                            rdrow["Iotrtri"] + sep +                    // 28 otros tributos monto base
                            rdrow["Iotrtas"] + sep +                    // 29 otros tributos tasa del tributo
                            rdrow["Iotrlin"] + sep +                    // 30 otros tributos monto unitario
                            rdrow["Itdscto"] + sep +                    // 31 Descuentos por ítem
                            rdrow["Iincard"] + sep +                    // 32 indicador de cargo/descuento
                            rdrow["Icodcde"] + sep +                    // 33 codigo de cargo/descuento
                            rdrow["Ifcades"] + sep +                    // 34 Factor de cargo/descuento
                            rdrow["Imoncde"] + sep +                    // 35 Monto de cargo/descuento
                            rdrow["Imobacd"] + sep +                    // 36 Monto base del cargo/descuento
                            rdrow["Ivalvta"] + sep);                    // 37 Valor de venta del ítem
                    }
                }
                else
                {
                    foreach (DataRow rdrow in tdfe.Rows)
                    {
                        writer.WriteLine(
                            "ITEM" + sep +
                            rdrow["Inumord"] + sep +        // "1"      // 2 orden
                            rdrow["Idatper"] + sep +                    // 3 Datos personilazados del item       
                            "TNE" + sep +                               // 4 Unidad de medida                    3
                            rdrow["Icantid"] + sep +    // tx_cetm.Text.Trim()             // 5 Cantidad de items             n(12,2)
                            rdrow["Idescri"] + sep +                    // 6 Descripcion                       500
                            "" + sep +                                  // 7 descricion de la glosa del item   250
                            rdrow["Icodprd"] + sep +                    // 8 codigo del producto del cliente    30
                            rdrow["Icodpro"] + sep +                    // 9 codigo del producto SUNAT           8
                            rdrow["Icodgs1"] + sep +                    // 10 codigo del producto GS1           14
                            rdrow["Icogtin"] + sep +                    // 11 tipo de producto GTIN             14
                            rdrow["Inplaca"] + sep +                    // 12 numero placa de vehiculo
                            rdrow["Ivaluni"] + sep +                    // 13 Valor unitario del item SIN IMPUESTO 
                            rdrow["Ipreuni"] + sep +                    // 14 Precio de venta unitario CON IGV
                            rdrow["Ivalref"] + sep +                    // 15 valor referencial del item cuando la venta es gratuita
                            rdrow["Iigvite"] + sep +                     // 16 monto igv   .. ."_msigv"
                            rdrow["Icatigv"] + sep +                    // 17 tipo/codigo de afectacion igv
                            rdrow["Itasigv"] + sep +                    // 18 tasa del igv
                            rdrow["Iigvite"] + sep +                    // 19 monto IGV del item
                            rdrow["Icodtri"] + sep +                    // 20 codigo del tributo por item
                            rdrow["Iiscmba"] + sep +                    // 21 ISC monto base
                            rdrow["Iisctas"] + sep +                    // 22 ISC tasa del tributo
                            rdrow["Iisctip"] + sep +                    // 23 ISC tipo de afectacion
                            rdrow["Iiscmon"] + sep +                    // 24 ISC monto del tributo
                            rdrow["Icbper1"] + sep +                    // 25 indicador de afecto a ICBPER
                            rdrow["Icbper2"] + sep +                    // 26 monto unitario de ICBPER
                            rdrow["Icbper3"] + sep +                    // 27 monto total ICBPER del item
                            rdrow["Iotrtri"] + sep +                    // 28 otros tributos monto base
                            rdrow["Iotrtas"] + sep +                    // 29 otros tributos tasa del tributo
                            rdrow["Iotrlin"] + sep +                    // 30 otros tributos monto unitario
                            rdrow["Itdscto"] + sep +                    // 31 Descuentos por ítem
                            rdrow["Iincard"] + sep +                    // 32 indicador de cargo/descuento
                            rdrow["Icodcde"] + sep +                    // 33 codigo de cargo/descuento
                            rdrow["Ifcades"] + sep +                    // 34 Factor de cargo/descuento
                            rdrow["Imoncde"] + sep +                    // 35 Monto de cargo/descuento
                            rdrow["Imobacd"] + sep +                    // 36 Monto base del cargo/descuento
                            rdrow["Ivalvta"] + sep);                    // 37 Valor de venta del ítem
                    }
                }
            }
            else
            {
                foreach (DataRow rdrow in tdfe.Rows)
                {
                    writer.WriteLine(
                        "ITEM" + sep +
                        rdrow["Inumord"] + sep +                    // 2 orden
                        rdrow["Idatper"] + sep +                    // 3 Datos personilazados del item      
                        rdrow["Iumeded"] + sep +                    // 4 Unidad de medida                    3
                        rdrow["Icantid"] + sep +                    // 5 Cantidad de items             n(12,2)
                        rdrow["Idescri"] + sep +                    // 6 Descripcion                       500
                        rdrow["Idesglo"] + sep +                    // 7 descricion de la glosa del item   250
                        rdrow["Icodprd"] + sep +                    // 8 codigo del producto del cliente    30
                        rdrow["Icodpro"] + sep +                    // 9 codigo del producto SUNAT           8
                        rdrow["Icodgs1"] + sep +                    // 10 codigo del producto GS1           14
                        rdrow["Icogtin"] + sep +                    // 11 tipo de producto GTIN             14
                        rdrow["Inplaca"] + sep +                    // 12 numero placa de vehiculo
                        rdrow["Ivaluni"] + sep +                    // 13 Valor unitario del item SIN IMPUESTO 
                        rdrow["Ipreuni"] + sep +                    // 14 Precio de venta unitario CON IGV
                        rdrow["Ivalref"] + sep +                    // 15 valor referencial del item cuando la venta es gratuita
                        rdrow["Iigvite"] + sep +                    // 16 monto igv   .. ."_msigv"
                        rdrow["Icatigv"] + sep +                    // 17 tipo/codigo de afectacion igv
                        rdrow["Itasigv"] + sep +                    // 18 tasa del igv
                        rdrow["Iigvite"] + sep +                    // 19 monto IGV del item
                        rdrow["Icodtri"] + sep +                    // 20 codigo del tributo por item
                        rdrow["Iiscmba"] + sep +                    // 21 ISC monto base
                        rdrow["Iisctas"] + sep +                    // 22 ISC tasa del tributo
                        rdrow["Iisctip"] + sep +                    // 23 ISC tipo de afectacion
                        rdrow["Iiscmon"] + sep +                    // 24 ISC monto del tributo
                        rdrow["Icbper1"] + sep +                    // 25 indicador de afecto a ICBPER
                        rdrow["Icbper2"] + sep +                    // 26 monto unitario de ICBPER
                        rdrow["Icbper3"] + sep +                    // 27 monto total ICBPER del item
                        rdrow["Iotrtri"] + sep +                    // 28 otros tributos monto base
                        rdrow["Iotrtas"] + sep +                    // 29 otros tributos tasa del tributo
                        rdrow["Iotrlin"] + sep +                    // 30 otros tributos monto unitario
                        rdrow["Itdscto"] + sep +                    // 31 Descuentos por ítem
                        rdrow["Iincard"] + sep +                    // 32 indicador de cargo/descuento
                        rdrow["Icodcde"] + sep +                    // 33 codigo de cargo/descuento
                        rdrow["Ifcades"] + sep +                    // 34 Factor de cargo/descuento
                        rdrow["Imoncde"] + sep +                    // 35 Monto de cargo/descuento
                        rdrow["Imobacd"] + sep +                    // 36 Monto base del cargo/descuento
                        rdrow["Ivalvta"] + sep);                    // 37 Valor de venta del ítem
                }
            }
            writer.Flush();
            writer.Close();
            retorna = true;
            return retorna;
        }
        private bool baja2TXT(string tipdo, string _fecemi, string _codbaj, string _secuen, string file_path, int cuenta, string serie, string corre)   // peru secure
        {
            bool retorna = false;

            string Prazsoc = nomclie.Trim();                                            // razon social del emisor
            string Prucpro = Program.ruc;                                               // Ruc del emisor
            string Pcrupro = "6";                                                       // codigo Ruc emisor
            string motivo = glosaAnul;      // "ANULACION";
            string fecdoc = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
            /* ********************************************** GENERAMOS EL TXT de baja   ************************************* */
            //string sep = "|";
            char sep = (char)31;
            StreamWriter writer;
            file_path = file_path + ".txt";
            writer = new StreamWriter(file_path);
            writer.WriteLine("CONTROL" + sep + "31001");
            writer.WriteLine("ENCABEZADO" + sep +
                "" + sep +                      // 2 Id del comprobante erp emisor
                "RA" + sep +                    // 3 tipo de comprobante
                Prucpro + sep +                 // 4 ruc emisor
                Prazsoc + sep +                 // 5 razon social emisor
                _codbaj + "-" + _secuen + sep +       // 6 codigo identificador de la baja, secuencial dentro de cada día
                _fecemi + sep +                 // 7 fecha de la baja  
                fecdoc + sep +                  // 8 fecha del documento dado de baja
                Program.mailclte +              // 9 correo del emisor
                "" + sep                        // 10 correo del receptor
            );
            writer.WriteLine("ITEM" + sep +
                "1" + sep +
                tipdo + sep +
                serie + sep +
                corre + sep +
                motivo + sep
            );
            writer.Flush();
            writer.Close();
            retorna = true;

            return retorna;
        }
        #endregion

        #region factSunat SFS
        private bool generaCAB(string tipdo, string serie, string corre, string file_path, string sep,
            double vsubt, double vigvt, double vflet, double monDet)
        {
            bool retorna = true;
            string ta = ".CAB";
            string fecemi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
            string horemi = DateTime.Now.ToString("HH:mm:ss");
            string fansi = fecemi;
            string vtipO = "0101";      // codigo tipo operación venta interna SIN DETRACCION
            if (tx_dat_dpla.Text.Trim() != "" && rb_credito.Checked == true)
            {
                fansi = DateTime.Parse(fansi).AddDays(double.Parse(tx_dat_dpla.Text)).Date.ToString("yyyy-MM-dd");        // fecha de emision + dias plazo credito
            }
            if (monDet > 0) vtipO = "1001";      // codigo tipo operación sujeta a detracción
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(
                vtipO + sep +                  // Tipo de operación 
                fecemi + sep +                  // Fecha de emisión
                horemi + sep +                  // Hora de Emisión
                fansi + sep +                   // fecha de vencimiento del doc.venta
                ((Program.codlocsunat == "")? "0000" : Program.codlocsunat) + sep +     // Código del domicilio fiscal o de local anexo del emisor
                tipoDocEmi + sep +              // Tipo de documento de identidad del adquirente o usuario
                tx_numDocRem.Text + sep +       // Número de documento de identidad del adquirente o usuario
                tx_nomRem.Text.Trim() + sep +   // Apellidos y nombres, denominación o razón social del adquirente o usuario 
                tipoMoneda + sep +                  // Tipo de moneda en la cual se emite la factura electrónica
                vigvt.ToString("#0.00") + sep +           // Sumatoria Tributos
                vsubt.ToString("#0.00") + sep +           // Total valor de venta 
                vflet.ToString("#0.00") + sep +           // Total Precio de Venta
                "0" + sep +                               // Total descuentos (no afectan la base imponible del IGV/IVAP)
                "0" + sep +                               // Sumatoria otros Cargos
                "0" + sep +                               // Total Anticipos
                vflet.ToString("#0.00") + sep +           // Importe total de la venta, cesión en uso o del servicio prestado
                "2.1" + sep +                       // Versión UBL
                "2.0" + sep                          // Customization Documento
                );
            writer.Flush();
            writer.Close();
            return retorna;
        }
        private bool generaDET(string tipdo, string serie, string corre, string file_path, string sep, int tfg)
        {
            bool retorna = true;
            string ta = ".DET";
            string fecemi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
            string horemi = DateTime.Now.ToString("HH:mm:ss");
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            for (int s = 0; s < tfg; s++)
            {
                string descrip = dataGridView1.Rows[s].Cells[1].Value.ToString();
                double preunit = double.Parse(dataGridView1.Rows[s].Cells[4].Value.ToString());
                double valunit = double.Parse(dataGridView1.Rows[s].Cells[4].Value.ToString()) / (1 + (double.Parse(v_igv) / 100));
                double sumimpl = double.Parse(dataGridView1.Rows[s].Cells[4].Value.ToString()) - valunit;

                writer.WriteLine(
                    "ZZ" + sep +             // Código de unidad de medida por ítem
                    "1" + sep +              // Cantidad de unidades por ítem
                    "" + sep +               // Código de producto
                    "-" + sep +              // Codigo producto SUNAT
                    descrip.Trim() + sep +          // Descripción detallada del servicio prestado, bien vendido o cedido en uso, indicando las características.
                    valunit.ToString("#0.00") + sep +           // Valor Unitario(cac:InvoiceLine / cac:Price / cbc:PriceAmount)
                    sumimpl.ToString("#0.00") + sep +           // Sumatoria Tributos por item
                    "1000" + sep +           // Tributo: Códigos de tipos de tributos IGV
                    sumimpl.ToString("#0.00") + sep +           // Tributo: Monto de IGV por ítem
                    valunit.ToString("#0.00") + sep +           // Tributo: Base Imponible IGV por Item
                    "IGV" + sep +            // Tributo: Nombre de tributo por item
                    "VAT" + sep +            // Tributo: Código de tipo de tributo por Item
                    "10" + sep +             // Tributo: Afectación al IGV por ítem
                    "18.0" + sep +           // Tributo: Porcentaje de IGV
                    "-" + sep +              // Tributo ISC: Códigos de tipos de tributos ISC
                    "" + sep +               // Tributo ISC: Monto de ISC por ítem
                    "" + sep +               // Tributo ISC: Base Imponible ISC por Item
                    "" + sep +               // Tributo ISC: Nombre de tributo por item
                    "" + sep +               // Tributo ISC: Código de tipo de tributo por Item
                    "" + sep +               // Tributo ISC: Tipo de sistema ISC
                    "" + sep +               // Tributo ISC: Porcentaje de ISC
                    "-" + sep +              // Tributo Otro: Códigos de tipos de tributos OTRO
                    "" + sep +               // Tributo Otro: Monto de tributo OTRO por iItem
                    "" + sep +               // Tributo Otro: Base Imponible de tributo OTRO por Item
                    "" + sep +               // Tributo Otro: Nombre de tributo OTRO por item
                    "" + sep +               // Tributo Otro: Código de tipo de tributo OTRO por Item
                    "" + sep +               // Tributo Otro: Porcentaje de tributo OTRO por Item
                    "-" + sep +              // Tributo ICBPER: Códigos de tipos de tributos ICBPER
                    "" + sep +               // Tributo ICBPER: Monto de tributo ICBPER por iItem
                    "" + sep +               // Tributo ICBPER: Cantidad de bolsas plásticas por Item
                    "" + sep +               // Tributo ICBPER:  Nombre de tributo ICBPER por item
                    "" + sep +               // Tributo ICBPER: Código de tipo de tributo ICBPER por Item
                    "" + sep +               // Tributo ICBPER: Monto de tributo ICBPER por Unidad
                    preunit.ToString("#0.00") + sep +            // Precio de venta unitario cac: InvoiceLine / cac:PricingReference / cac:AlternativeConditionPrice
                    valunit.ToString("#0.00") + sep +            // Valor de venta por Item cac: InvoiceLine / cbc:LineExtensionAmount
                    "0.00" + sep             // Valor REFERENCIAL unitario(gratuitos) cac: InvoiceLine / cac:PricingReference / cac:AlternativeConditionPrice
                );
            }
            writer.Flush();
            writer.Close();

            return retorna;
        }
        private bool generaTRI(string tipdo, string serie, string corre, string file_path, string sep,
            double vsubt, double vigvt, double vflet)
        {
            bool retorna = true;
            string ta = ".TRI";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(
                "1000" + sep +                      // Identificador de tributo
                "IGV" + sep +                       // Nombre de tributo
                "VAT" + sep +                       // Código de tipo de tributo
                vsubt.ToString("#0.00") + sep +     // Base imponible
                vigvt.ToString("#0.00") + sep       // Monto de Tributo
                );
            writer.Flush();
            writer.Close();

            return retorna;
        }
        private bool generaLEY(string tipdo, string serie, string corre, string file_path, string sep)
        {
            bool retorna = true;
            string ta = ".LEY";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(
                "1000" + sep +                              // Código de leyenda
                "SON: " + tx_fletLetras.Text + sep          // "Monto en Letras"
                );
            if (tx_dat_mone.Text == MonDeft)
            {
                if (double.Parse(tx_flete.Text) > (double.Parse(Program.valdetra)))
                {
                    writer.WriteLine(
                        "2006" + sep +
                        glosdetra + " " + Program.ctadetra + sep
                        );
                }
            }
            else
            {
                if (double.Parse(tx_flete.Text) > (double.Parse(Program.valdetra) / double.Parse(tx_tipcam.Text)))
                {
                    writer.WriteLine(
                        "2006" + sep +
                        glosdetra + " " + Program.ctadetra + sep
                        );
                }
            }
            writer.Flush();
            writer.Close();

            return retorna;
        }
        private bool generaREL(string tipdo, string serie, string corre, string file_path, string sep, int tfg)
        {
            bool retorna = true;
            string ta = ".REL";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            for (int s = 0; s < tfg; s++)
            {
                string vg = dataGridView1.Rows[s].Cells[0].Value.ToString();
                writer.WriteLine(
                    "1" + sep +                         // Indicador de documento relacionado (1: Guía
                    "-" + sep +                         // Número identificador del anticipo (solo para el Caso: 2 Anticipo).
                    "31" + sep +                        // Tipo de documento relacionado
                    vg + sep +                          // Serie numero
                    "" + sep +                          // OPCIONAL NO USAMOS Tipo de documento del emisor del documento relacionado, OSEA RUC DEL TRANSPORTISTA
                    "" + sep +                          // OPCIONAL NO USAMOS Número de documento del emisor del documento relacionado
                    "0" + sep                           // OPCIONAL NO USAMOS monto de la guía
                    );
            }
            writer.Flush();
            writer.Close();

            return retorna;
        }
        private bool generaACA(string tipdo, string serie, string corre, string file_path, string sep,
            double monDet)
        {
            bool retorna = true;
            string ta = ".ACA";
            StreamWriter writer;
            file_path = file_path + ta;
            if (monDet > 0)
            {
                writer = new StreamWriter(file_path);
                {
                    writer.WriteLine(
                        Program.ctadetra + sep +                // Cuenta del banco de la nacion (detraccion)
                        "027" + sep +                           // Codigo del bien o producto sujeto a detracción 
                        Program.pordetra + sep +                // Porcentaje de la detracción
                        monDet.ToString("#0.00") + sep +        // Monto de la detracción
                        "001" + sep +                           // Medio de pago
                        "-" + sep + 
                        "-" + sep +
                        "-" + sep +
                        "-" + sep +
                        "-" + sep +
                        "-" + sep
                        );
                }
                writer.Flush();
                writer.Close();
            }
            return retorna;
        }
        private bool generaADE(string tipdo, string serie, string corre, string file_path, string sep)
        {
            bool retorna = true;
            /* 22/03/2023 no tenemos adicionales en detalle
            string ta = ".ADE";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(

                );
            writer.Flush();
            writer.Close();
            */
            return retorna;
        }
        private bool generaACV(string tipdo, string serie, string corre, string file_path, string sep)
        {
            bool retorna = true;
            /* 22/03/2023 no tenemos adicionales de cabecera variable
            string ta = ".ACV";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(

                );
            writer.Flush();
            writer.Close();
            */
            return retorna;
        }
        private bool generaSTC(string tipdo, string serie, string corre, string file_path, string sep,
            double monDet)
        {
            bool retorna = true;
            if (monDet > 0)                 // chk_cunica.Checked == true && 
            {
                string ta = ".STC";
                StreamWriter writer;
                file_path = file_path + ta;
                writer = new StreamWriter(file_path);
                writer.WriteLine(
                    "1" + sep +             // Linea item
                    "" + sep +              // Detalle del Viaje
                    tx_dat_upo.Text + sep + // Código de Ubigeo Origen
                    tx_dat_dpo.Text + sep + // Dirección detallada del origen 
                    tx_dat_upd.Text + sep + // Código de Ubigeo Destino
                    tx_dat_dpd.Text + sep + // Dirección detallada del destino
                    "01" + sep +            // Tipo referencial del servicio 
                    tx_valref1.Text + sep + // Valor referencial del servicio 
                    "02" + sep +            // Tipo referencial sobre la carga efectiva
                    tx_valref2.Text + sep + // Valor referencial sobre la carga efectiva
                    "03" + sep +            // Tipo referencial sobre la carga útil nominal
                    tx_valref3.Text + sep   // Valor referencial sobre la carga útil nominal
                    );
                writer.Flush();
                writer.Close();
            }
            return retorna;
        }
        private bool generaTRA(string tipdo, string serie, string corre, string file_path, string sep,
            double monDet)
        {
            bool retorna = true;
            if (chk_cunica.Checked == true && monDet > 0)
            {
                string ta = ".TRA";
                StreamWriter writer;
                file_path = file_path + ta;
                writer = new StreamWriter(file_path);
                writer.WriteLine(
                    "1" + sep +                         // Linea item
                    "01" + sep +                        // Identificador del tramo
                    "Recorrido total" + sep +           // Descripción del tramo
                    tx_valref2.Text + sep +             // Valor preliminar referencial sobre la Carga Efectiva (Por el tramo virtual recorrido)
                    tx_valref3.Text + sep +             // Valor Preliminar Referencial por Carga Útil Nominal (Tratándose de más de 1 vehículo)
                    tx_dat_upo.Text + sep +             // Código de Ubigeo - ORIGEN
                    tx_dat_upd.Text + sep               // Código de Ubigeo - DESTINO
                    );
                writer.Flush();
                writer.Close();
            }
            return retorna;
        }
        private bool generaVEH(string tipdo, string serie, string corre, string file_path, string sep,
            double monDet)
        {
            bool retorna = true;
            if (chk_cunica.Checked == true && monDet > 0)
            {
                string ta = ".VEH";
                StreamWriter writer;
                file_path = file_path + ta;
                writer = new StreamWriter(file_path);
                writer.WriteLine(
                    "1" + sep +                 // Linea item
                    "01" + sep +                // Identificador del tramo, el mismo que en .TRA
                    tx_pla_confv.Text + sep +   // Configuracion vehicular del vehículo
                    "false" + sep +             // Indica factor de retorno de viaje
                    tx_valRefTM.Text + sep +    // Valor Referencial por TM
                    "02" + sep +                // Carga Efectiva en TM del vehículo - Tipo
                    tx_cetm.Text + sep +        // Carga Efectiva en TM del vehículo - Valor
                    "01" + sep +                // Carga Util en TM del vehículo - Tipo
                    tx_cutm.Text + sep          // Carga Util en TM del vehículo - Valor
                    );
                writer.Flush();
                writer.Close();
            }
            return retorna;
        }
        private bool generaPAG(string tipdo, string serie, string corre, string file_path, string sep)
        {
            bool retorna = true;
            string mp = "";
            string vp = tx_flete.Text;
            if (rb_contado.Checked == true)
            {
                mp = "Contado";
                vp = "-";
            }
            else
            {
                mp = "Credito";
            }
            string ta = ".PAG";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(
                 mp + sep +             // Forma de pago
                 vp + sep +             // Monto neto pendiente de pago
                tipoMoneda + sep        // Tipo de moneda del monto pendiente de pago
                );
            writer.Flush();
            writer.Close();
            
            return retorna;
        }
        private bool generaDPA(string tipdo, string serie, string corre, string file_path, string sep)
        {
            bool retorna = true;
            string vp = tx_flete.Text;
            string _fechc = DateTime.Parse(tx_fechope.Text).AddDays(double.Parse(tx_dat_dpla.Text)).Date.ToString("yyyy-MM-dd");

            string ta = ".DPA";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(
                vp + sep +
                _fechc + sep +
                tipoMoneda + sep
                );
            writer.Flush();
            writer.Close();
            
            return retorna;
        }
        private bool generaRTN(string tipdo, string serie, string corre, string file_path, string sep)
        {
            bool retorna = true;
            /* 23/03/2023 no tengo cliente con retencion de igv ... no usamos
            string ta = ".RTN";
            StreamWriter writer;
            file_path = file_path + ta;
            writer = new StreamWriter(file_path);
            writer.WriteLine(

                );
            writer.Flush();
            writer.Close();
            */
            return retorna;
        }
        #endregion

        #region factDirecta sistema del contribuyente
        private bool sunat_api(string tipdo, string tipoMoneda, string tipoDocEmi)                 // SI VAMOS A USAR 26/05/2023 este metodo directo
        {
            bool retorna = false;
            //guiati_e guiati_E = new guiati_e();
            string token = "noPideToken";  // _Sunat.conex_token_(c_t);           // no pide token para el envío del comprobante en soap
            if (token != null && token != "")
            {
                string aZip = "";
                string aXml = "";
                if (llenaTablaLiteDV(tipdo, tipoMoneda, tipoDocEmi) != true)
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
                        MessageBox.Show(ex.Message,"Error al enviar a Sunat",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                        if (chk_cunica.Checked == false)
                        {
                            // insertamos
                            actua = "insert into adifactu (idc,nticket,fticket,estadoS,cdr,cdrgener,textoQR) values (@idc,@nti,@fti,@est,@cdrt,@cdrg,@tqr)";
                        }
                        else
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

                retorna = true;
            }
            return retorna;
        }
        static private void CreaTablaLiteDV()                  // llamado en el load del form, crea las tablas al iniciar
        {
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                cnx.Open();
                string sqlborra = "DROP TABLE IF EXISTS dt_cabdv; DROP TABLE IF EXISTS dt_detdv; DROP TABLE IF EXISTS dt_docrel";
                using (SqliteCommand cmdB = new SqliteCommand(sqlborra, cnx))
                {
                    cmdB.ExecuteNonQuery();
                }
                string sqlTabla = "create table dt_cabdv (" +
                    // cabecera
                    "id integer primary key autoincrement, " +
                    "EmisRuc varchar(11), " +           // ruc del emisor               - 16
                    "EmisNom varchar(150), " +          // Razón social del emisor      - 15
                    "EmisCom varchar(150), " +          // Nombre Comercial del emisor  - 14
                    "CodLocA varchar(4), " +            // Código local anexo emisor    - 17
                    "EmisUbi varchar(6), " +            // ubigeo del emisor
                    "EmisDir varchar(200), " +
                    "EmisDep varchar(50), " +
                    "EmisPro varchar(50), " +
                    "EmisDis varchar(50), " +
                    "EmisUrb varchar(50), " +           // urbanización, pueblo, localidad
                    "EmisPai varchar(2), " +            // código sunat del país emisor
                    "EmisCor varchar(100), " +          // correo del emisor de la guía
                    "NumDVta varchar(12), " +           // serie+numero
                    "FecEmis varchar(10), " +
                    "HorEmis varchar(8), " +
                    "CodComp varchar(2), " +            // código sunat del comprobante
                    "FecVcto varchar(10), " +           // Fecha de vencimiento del comprobante
                    "TipDocu varchar(2), " +            // SUNAT:Identificador de Tipo de Documento
                    "CodLey1 varchar(4), " +             // codigo sunat de leyenda MONTO EN LETRAS
                    "MonLetr varchar(150), " +           // monto en letras
                    "CodMonS varchar(3)," +              // código internacional de moneda 
                    // datos del destinatario
                    "DstTipdoc varchar(2), " +          // código sunat del tipo de documento del destinatario  - 18
                    "DstNumdoc varchar(11), " +         // número del documento del destinatario                - 18
                    "DstNomTdo varchar(50), " +         // glosa, texto o nombre sunat del doc del destinatario
                    "DstNombre varchar(150), " +        // nombre o razón social del destinatario
                    "DstDirecc varchar(200), " +        // dirección del destinatario                           - 20
                    "DstDepart varchar(50), " +
                    "DstProvin varchar(50), " +
                    "DstDistri varchar(50), " +
                    "DstUrbani varchar(50), " +         // urbanización, pueblo, localidad
                    "DstUbigeo varchar(6), " +          // ubigeo de la direc del cliente                       - 20
                    // Información de descuentos Globales               // no usamos dsctos globales 17/06/2023 - 21
                    
                    // Información de importes 
                    "ImpTotImp decimal(12,2), " +       // Monto total de impuestos                             - 22 TaxAmount
                    "ImpOpeGra decimal(12,2), " +       // Monto las operaciones gravadas                       - 23 TaxableAmount
                    //"ImpOpeExo decimal(12,2), " +     // Monto las operaciones Exoneradas                     - 24
                    //"ImpOpeIna decimal(12,2), " +     // Monto las operaciones inafectas del impuesto         - 25
                    //"ImpOpeGra decimal(12,2), " +     // Monto las operaciones gratuitas                      - 26
                    "ImpIgvTot decimal(12,2), " +       // Sumatoria de IGV                                     - 27
                    //"ImpISCTot decimal(12,2), " +      // Sumatoria de ISC                                     - 28
                    "ImpOtrosT decimal(12,2), " +       // Sumatoria de Otros Tributos                          - 29
                    "IgvCodSun varchar(1), " +          // schemeAgencyID="6"
                    "IgvConInt varchar(4), " +          // 1000
                    "IgvNomSun varchar(4), " +          // IGV
                    "IgvCodInt varchar(4), " +          // VAT
                    "TotValVta decimal(12,2), " +       // Total valor de venta                                 - 30
                    "TotPreVta decimal(12,2), " +       // Total precio de venta (incluye impuestos)            - 31
                    "TotDestos decimal(12,2), " +       // Monto total de descuentos del comprobante            - 32
                    "TotOtrCar decimal(12,2), " +       // Monto total de otros cargos del comprobante          - 33
                    "TotaVenta decimal(12,2), " +        // Importe total de la venta, cesión en uso o del servicio prestado - 34
                    "CanFilDet integer, " +              // Cantidad filas de detalle
                    "CtaDetra varchar(20), " +           // Cta detracción banco de la nación
                    "PorDetra decimal(5,1), " +          // % de la detracción
                    "ImpDetra decimal(12,2), " +         // Importe de la detracción EN SOLES, la cuenta del BN es el soles
                    "GloDetra varchar(200), " +          // Glosa general de la detracción
                    "CodTipDet varchar(3), " +           // Código sunat tipo de detraccion (027 transporte de carga)
                    "CondPago varchar(10), " +           // Condicion de pago
                    "CodTipDoc varchar(2), " +          // Código sunat para el tipo de documento, FT=01, BV=03, etc
                    "CodTipOpe varchar(4), " +           // Código sunat para el tipo de operación, 0101=Vta, interna facturas y boletas
                    // ENCABEZADO-TRASLADOBIENES
                    "cu_cpapp varchar(2), " +            // Código país del punto de origen
                    "cu_ubipp varchar(6), " +            // Ubigeo del punto de partida 
                    "cu_deppp varchar(50), " +           // Departamento del punto de partida
                    "cu_propp varchar(50), " +           // Provincia del punto de partida
                    "cu_dispp varchar(50), " +           // Distrito del punto de partida
                    "cu_urbpp varchar(50), " +           // Urbanización del punto de partida
                    "cu_dirpp varchar(200), " +          // Dirección detallada del punto de partida
                    "cu_cppll varchar(2), " +            // Código país del punto de llegada
                    "cu_ubpll varchar(6), " +            // Ubigeo del punto de llegada
                    "cu_depll varchar(50), " +           // Departamento del punto de llegada
                    "cu_prpll varchar(50), " +           // Provincia del punto de llegada
                    "cu_dipll varchar(50), " +           // Distrito del punto de llegada
                    "cu_ddpll varchar(200), " +          // Dirección detallada del punto de llegada
                    "cu_placa varchar(7), " +            // Placa del Vehículo
                    "cu_confv varchar(7), " +            // Configuracion vehicular
                    "cu_coins varchar(15), " +           // Constancia de inscripción del vehículo o certificado de habilitación vehicular
                    "cu_marca varchar(50), " +           // Marca del Vehículo
                    "cu_breve varchar(15), " +           // Nro.de licencia de conducir
                    "cu_ructr varchar(11), " +           // RUC del transportista
                    "cu_nomtr varchar(200), " +          // Razón social del Transportista
                    "cu_modtr varchar(2), " +            // Modalidad de Transporte
                    "cu_pesbr decimal(10,2), " +         // Total Peso Bruto
                    "cu_motra varchar(2), " +            // Código de Motivo de Traslado
                    "cu_fechi varchar(10), " +           // Fecha de Inicio de Traslado
                    "cu_remtc varchar(15), " +           // Registro MTC
                    "cu_nudch varchar(15), " +           // Nro.Documento del conductor
                    "cu_tidch varchar(2), " +            // Tipo de Documento del conductor
                    "cu_plac2 varchar(7), " +            // Placa del Vehículo secundario
                    "cu_insub varchar(2), " +             // Indicador de subcontratación
                    "cu_marCU varchar(1) " +             // "1"=carga unica, "0"=carga normal
                ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // ********************* DETALLE ************************ //
                sqlTabla = "create table dt_detdv (" +
                    "id integer primary key autoincrement, " +
                    "NumDVta varchar(12), " +
                    "Numline integer, " +            // Número de orden del Ítem                             - 35
                    "Cantprd integer, " +            // Cantidad y Unidad de medida por ítem                 - 36
                    "CodMone varchar(3), " +         // Codigo internacional de moneda                       - 37
                    "ValVtaI decimal(12,2), " +      // Valor de venta del ítem                              - 37
                    "PreVtaU decimal(12,2), " +     // Precio de venta unitario por item y código           - 38
                                                    // Valor referencial unitario por ítem en operaciones no onerosas   - 39
                                                    // Descuentos por Ítem                                  - 40
                                                    // Cargos por item                                      - 41
                    "ValIgvI decimal(12,2), " +      // Afectación al IGV por ítem                           - 42
                                                    // Afectación al ISC por ítem                           - 43
                    "DesDet1 varchar(100), " +      // Descripción detallada                                - 44
                    "DesDet2 varchar(100), " +
                    "CodIntr varchar(50), " +       // Código de producto                                   - 45
                                                    // Código de producto SUNAT                             - 46
                                                    // Propiedades Adicionales del Ítem                     - 47
                    "ValUnit decimal(12,2), " +     // Valor unitario del ítem                              - 48
                    "ValPeso real, " +              // peso de la carga, va unido a la unidad de medida 
                    "UniMedS varchar(3), " +        // codigo unidad de medida de sunat
                    "GuiaTra varchar(13), " +       // numero guía relacionada
                    "CodTipG varchar(2), " +        // codigo sunat tipo de guía relacionada
                    "PorcIgv varchar(2), " +        // % del igv en números (18)
                    "CodSunI varchar(2), " +        // codigo sunat del igv, (10)
                    "CodSunT varchar(4), " +        // codigo sunat del tributo, (1000)
                    "NomSunI varchar(10), " +       // nombre sunat del impuesto, (IGV)
                    "NomIntI varchar(10)" +         // nombre internacional del impuesto, (VAT)
                    ")";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                /* / ********************* GUIAS RELACIONADAS ************************ //
                sqlTabla = "create table dt_docrel (" +
                    "id integer primary key autoincrement, " +
                    "NumGuia varchar(12), " +
                    "clinea integer, " +
                    "codDoc varchar(2), " + 
                    "numDoc varchar(15)";
                using (SqliteCommand cmd = new SqliteCommand(sqlTabla, cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                */
            }
        }
        private bool llenaTablaLiteDV(string tipdo, string tipoMoneda, string tipoDocEmi)          // llena tabla con los datos del comprobante y llama al app que crea el xml
        {
            bool retorna = false;
            using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
            {
                string fecemi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
                string fansi = DateTime.Parse(fecemi).AddDays(double.Parse((tx_dat_dpla.Text == "") ? "0" : tx_dat_dpla.Text)).Date.ToString("yyyy-MM-dd");        // fecha de emision + dias plazo credito
                string cdvta = cmb_tdv.Text.Substring(0, 1) + lib.Right(tx_serie.Text, 3) + "-" + tx_numero.Text;
                
                cnx.Open();
                using (SqliteCommand cmd = new SqliteCommand("delete from dt_cabdv where id>0", cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                using (SqliteCommand cmd = new SqliteCommand("delete from dt_detdv where id>0", cnx))
                {
                    cmd.ExecuteNonQuery();
                }
                // CABECERA
                string metela = "insert into dt_cabdv (" +
                    "EmisRuc,EmisNom,EmisCom,CodLocA,EmisUbi,EmisDir,EmisDep,EmisPro,EmisDis,EmisUrb,EmisPai,EmisCor,NumDVta,FecEmis,HorEmis,CodComp,FecVcto," +
                    "TipDocu,CodLey1,MonLetr,CodMonS,DstTipdoc,DstNumdoc,DstNomTdo,DstNombre,DstDirecc,DstDepart,DstProvin,DstDistri,DstUrbani,DstUbigeo,ImpTotImp," +
                    "ImpOpeGra,ImpIgvTot,ImpOtrosT,IgvCodSun,IgvConInt,IgvNomSun,IgvCodInt,TotValVta,TotPreVta,TotDestos,TotOtrCar,TotaVenta," +
                    "CanFilDet,CtaDetra,PorDetra,ImpDetra,GloDetra,CodTipDet,CondPago,CodTipOpe," +
                    "cu_cpapp,cu_ubipp,cu_deppp,cu_propp,cu_dispp,cu_urbpp,cu_dirpp,cu_cppll,cu_ubpll,cu_depll,cu_prpll,cu_dipll,cu_ddpll,cu_confv," +
                    "cu_placa,cu_coins,cu_marca,cu_breve,cu_ructr,cu_nomtr,cu_modtr,cu_pesbr,cu_motra,cu_fechi,cu_remtc,cu_nudch,cu_tidch,cu_plac2,cu_insub,cu_marCU) " +
                    "values (" +
                    "@EmisRuc,@EmisNom,@EmisCom,@CodLocA,@EmisUbi,@EmisDir,@EmisDep,@EmisPro,@EmisDis,@EmisUrb,@EmisPai,@EmisCor,@NumDVta,@FecEmis,@HorEmis,@CodComp,@FecVcto," +
                    "@TipDocu,@CodLey1,@MonLetr,@CodMonS,@DstTipd,@DstNumd,@DstNomT,@DstNomb,@DstDire,@DstDepa,@DstProv,@DstDist,@DstUrba,@DstUbig,@ImpTotI," +
                    "@ImpOpeG,@ImpIgvT,@ImpOtro,@IgvCodS,@IgvConI,@IgvNomS,@IgvCodI,@TotValV,@TotPreV,@TotDest,@TotOtrC,@TotaVen," +
                    "@CanFilD,@CtaDetr,@PorDetr,@ImpDetr,@GloDetr,@CodTipD,@CondPag,@CodTipO," +
                    "@cu_cpapp,@cu_ubipp,@cu_deppp,@cu_propp,@cu_dispp,@cu_urbpp,@cu_dirpp,@cu_cppll,@cu_ubpll,@cu_depll,@cu_prpll,@cu_dipll,@cu_ddpll,@cu_confv," +
                    "@cu_placa,@cu_coins,@cu_marca,@cu_breve,@cu_ructr,@cu_nomtr,@cu_modtr,@cu_pesbr,@cu_motra,@cu_fechi,@cu_remtc,@cu_nudch,@cu_tidch,@cu_plac2,@cu_insub,@cu_marCU)";
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
                    cmd.Parameters.AddWithValue("@NumDVta", cdvta);         // "V001-98000006"
                    cmd.Parameters.AddWithValue("@FecEmis", fecemi);              // "2023-05-19"
                    cmd.Parameters.AddWithValue("@HorEmis", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);  // "12:21:13"
                    cmd.Parameters.AddWithValue("@CodComp", "");                      // codigo del comprobante
                    cmd.Parameters.AddWithValue("@FecVcto", fansi);

                    cmd.Parameters.AddWithValue("@TipDocu", tipdo);             // SUNAT:Identificador de Tipo de Documento
                    cmd.Parameters.AddWithValue("@CodLey1", "1000");
                    cmd.Parameters.AddWithValue("@MonLetr", "SON: " + tx_fletLetras.Text);
                    cmd.Parameters.AddWithValue("@CodMonS", tipoMoneda);
                    cmd.Parameters.AddWithValue("@DstTipd", tipoDocEmi);
                    cmd.Parameters.AddWithValue("@DstNumd", tx_numDocRem.Text);
                    cmd.Parameters.AddWithValue("@DstNomT", "");                // glosa, texto o nombre sunat del doc del destinatario
                    cmd.Parameters.AddWithValue("@DstNomb", tx_nomRem.Text);    // "<![CDATA[" + tx_nomRem.Text + "]]>"  ... no funca
                    cmd.Parameters.AddWithValue("@DstDire", tx_dirRem.Text);    // "<![CDATA[" + tx_dirRem.Text + "]]>"
                    cmd.Parameters.AddWithValue("@DstDepa", tx_dptoRtt.Text);
                    cmd.Parameters.AddWithValue("@DstProv", tx_provRtt.Text);
                    cmd.Parameters.AddWithValue("@DstDist", tx_distRtt.Text);
                    cmd.Parameters.AddWithValue("@DstUrba", "");
                    cmd.Parameters.AddWithValue("@DstUbig", tx_ubigRtt.Text);
                    cmd.Parameters.AddWithValue("@ImpTotI", tx_igv.Text);       // Monto total de impuestos

                    cmd.Parameters.AddWithValue("@ImpOpeG", tx_subt.Text);      // Monto las operaciones gravadas
                    cmd.Parameters.AddWithValue("@ImpIgvT", tx_igv.Text);       // Sumatoria de IGV
                    cmd.Parameters.AddWithValue("@ImpOtro", "0");               // Sumatoria de Otros Tributos
                    cmd.Parameters.AddWithValue("@IgvCodS", "6");               // schemeAgencyID="6"
                    cmd.Parameters.AddWithValue("@IgvConI", "1000");            // 1000
                    cmd.Parameters.AddWithValue("@IgvNomS", "IGV");             // IGV
                    cmd.Parameters.AddWithValue("@IgvCodI", "VAT");             // VAT
                    cmd.Parameters.AddWithValue("@TotValV", tx_subt.Text);      // Total valor de venta
                    cmd.Parameters.AddWithValue("@TotPreV", tx_flete.Text);     // Total precio de venta (incluye impuestos)
                    cmd.Parameters.AddWithValue("@TotDest", "0");
                    cmd.Parameters.AddWithValue("@TotOtrC", "0");
                    cmd.Parameters.AddWithValue("@TotaVen", tx_flete.Text);
                    string detrac = "no";
                    double vtotdet = 0;
                    if (decimal.Parse(tx_fletMN.Text) > decimal.Parse(Program.valdetra)) 
                    {
                        detrac = "si";
                        //vtotdet = Math.Round(double.Parse(tx_flete.Text) * double.Parse(Program.pordetra) / 100, 2);    // totalDetraccion 
                        vtotdet = Math.Round(double.Parse(tx_fletMN.Text) * double.Parse(Program.pordetra) / 100, 2);    // totalDetraccion 
                    }
                    cmd.Parameters.AddWithValue("@CanFilD", tx_tfil.Text);
                    cmd.Parameters.AddWithValue("@CtaDetr", (detrac == "si") ? Program.ctadetra : "");
                    cmd.Parameters.AddWithValue("@PorDetr", (detrac == "si") ? Program.pordetra : "");
                    cmd.Parameters.AddWithValue("@ImpDetr", (detrac == "si") ? vtotdet : 0);
                    cmd.Parameters.AddWithValue("@GloDetr", (detrac == "si") ? glosdetra + " " + Program.ctadetra : "");
                    cmd.Parameters.AddWithValue("@CodTipD", (detrac == "si") ? Program.coddetra : "");
                    cmd.Parameters.AddWithValue("@CondPag", (rb_contado.Checked == true) ? "Contado" : "Credito");
                    cmd.Parameters.AddWithValue("@CodTipO", (detrac == "si") ? "1004" : "0101");    // 0101=venta interna, 1001=vta interna sujeta a detracción, 1004=Op. Sujeta a Detracción - Servicios de Transporte Carga
                    cmd.Parameters.AddWithValue("@cu_cpapp", "PE");         // Código país del punto de origen
                    cmd.Parameters.AddWithValue("@cu_ubipp", tx_dat_upo.Text);         // Ubigeo del punto de partida 
                    cmd.Parameters.AddWithValue("@cu_deppp", tx_dp_dep.Text);         // Departamento del punto de partida
                    cmd.Parameters.AddWithValue("@cu_propp", tx_dp_pro.Text);         // Provincia del punto de partida 
                    cmd.Parameters.AddWithValue("@cu_dispp", tx_dp_dis.Text);         // Distrito del punto de partida
                    cmd.Parameters.AddWithValue("@cu_urbpp", "");         // Urbanización del punto de partida
                    cmd.Parameters.AddWithValue("@cu_dirpp", tx_dat_dpo.Text);         // Dirección detallada del punto de partida
                    cmd.Parameters.AddWithValue("@cu_cppll", "PE");         // Código país del punto de llegada
                    cmd.Parameters.AddWithValue("@cu_ubpll", tx_dat_upd.Text);         // Ubigeo del punto de llegada
                    cmd.Parameters.AddWithValue("@cu_depll", tx_dd_dep.Text);         // Departamento del punto de llegada
                    cmd.Parameters.AddWithValue("@cu_prpll", tx_dd_pro.Text);         // Provincia del punto de llegada
                    cmd.Parameters.AddWithValue("@cu_dipll", tx_dd_dis.Text);         // Distrito del punto de llegada
                    cmd.Parameters.AddWithValue("@cu_ddpll", tx_dat_dpd.Text);         // Dirección detallada del punto de llegada
                    cmd.Parameters.AddWithValue("@cu_placa", tx_pla_placa.Text);         // Placa del Vehículo
                    cmd.Parameters.AddWithValue("@cu_confv", tx_pla_confv.Text);         // configuración vehicular
                    cmd.Parameters.AddWithValue("@cu_coins", tx_pla_autor.Text);         // Constancia de inscripción del vehículo o certificado de habilitación vehicular
                    cmd.Parameters.AddWithValue("@cu_marca", "");         // Marca del Vehículo  
                    cmd.Parameters.AddWithValue("@cu_breve", "");         // Nro.de licencia de conducir
                    cmd.Parameters.AddWithValue("@cu_ructr", tx_rucT.Text);         // RUC del transportista
                    cmd.Parameters.AddWithValue("@cu_nomtr", tx_razonS.Text);         // Razón social del Transportista
                    cmd.Parameters.AddWithValue("@cu_modtr", texmotran);         // Modalidad de Transporte
                    cmd.Parameters.AddWithValue("@cu_pesbr", tx_cetm.Text);         // Total Peso Bruto    02
                    cmd.Parameters.AddWithValue("@cu_motra", codtxmotran);          // Código de Motivo de Traslado    01
                    cmd.Parameters.AddWithValue("@cu_fechi", tx_fecini.Text);         // Fecha de Inicio de Traslado 
                    cmd.Parameters.AddWithValue("@cu_remtc", (tx_rucT.Text.Trim() != Program.ruc) ? "faltAdecuar" : Program.regmtc);         // Registro MTC
                    cmd.Parameters.AddWithValue("@cu_nudch", tx_dniChof.Text);         // Nro.Documento del conductor 
                    cmd.Parameters.AddWithValue("@cu_tidch", 1);         // Tipo de Documento del conductor
                    cmd.Parameters.AddWithValue("@cu_plac2", "");         // Placa del Vehículo secundario
                    cmd.Parameters.AddWithValue("@cu_insub", (tx_rucT.Text.Trim() != Program.ruc) ? "true" : "false");         // Indicador de subcontratación (true/false)
                    if (chk_cunica.Checked == true)         // 15/02/2024
                    {
                        cmd.Parameters.AddWithValue("@cu_marCU", "1");          // 1=carga unica, 0=carga normal
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@cu_marCU", "0");          // 1=carga unica, 0=carga normal
                    }
                    cmd.ExecuteNonQuery();
                }
                // DETALLE
                for (int i=0; i< dataGridView1.Rows.Count - 1; i++)
                {
                    string glosser2 = "";       // detalle de la linea
                    string descrip = "";        // descripcion del la linea
                    double preunit = 0;         // precio unitario de la linea
                    double valunit = 0;         // valor sin igv de la linea
                    double sumimpl = 0;         // igv de la fila

                    glosser2 = dataGridView1.Rows[i].Cells["OriDest"].Value.ToString() + " - " +
                        dataGridView1.Rows[i].Cells["Cant"].Value.ToString() + " " +
                        dataGridView1.Rows[i].Cells["umed"].Value.ToString() + " " + dataGridView1.Rows[i].Cells["guiasclte"].Value.ToString();
                    descrip = dataGridView1.Rows[i].Cells[1].Value.ToString();

                    if (tx_dat_mone.Text == MonDeft)
                    {
                        preunit = double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                        valunit = double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) / (1 + (double.Parse(v_igv) / 100));
                        sumimpl = double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) - valunit;
                    }
                    else
                    {
                        // solo somos bi moneda , soles y dolares, sino no es soles entonces es dolares
                        preunit = Math.Round(double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) / double.Parse(tx_tipcam.Text), 2);
                        valunit = Math.Round(preunit / (1 + (double.Parse(v_igv) / 100)), 2); // double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) / (1 + (double.Parse(v_igv) / 100));
                        sumimpl = Math.Round(preunit - valunit, 2); // double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) - valunit;
                    }
                    metela = "insert into dt_detdv (" +
                        "NumDVta,Numline,Cantprd,CodMone,ValVtaI,PreVtaU,ValIgvI,DesDet1,DesDet2,CodIntr,ValUnit,ValPeso,UniMedS," +
                        "GuiaTra,CodTipG,PorcIgv,CodSunI,CodSunT,NomSunI,NomIntI) values (" +
                        "@NumGu,@Numli,@Cantp,@CodMo,@ValVt,@PreVt,@ValIg,@DesD1,@DesD2,@CodIn,@ValUn,@ValPe,@UniMe," +
                        "@GuiaT,@CodTG,@PIgvn,@CodSI,@CodST,@NomSI,@NomII)";
                    using (SqliteCommand cmd = new SqliteCommand(metela, cnx))
                    {
                        cmd.Parameters.AddWithValue("@NumGu", cdvta);      // "V001-98000006"
                        cmd.Parameters.AddWithValue("@Numli", i+1.ToString());
                        cmd.Parameters.AddWithValue("@Cantp", "1");    // dataGridView1.Rows[i].Cells[2].Value.ToString()
                        cmd.Parameters.AddWithValue("@CodMo", tipoMoneda);
                        cmd.Parameters.AddWithValue("@ValVt", valunit.ToString());  // valor venta  s/igv
                        cmd.Parameters.AddWithValue("@PreVt", preunit.ToString());  // precio venta c/igv
                        cmd.Parameters.AddWithValue("@ValIg", sumimpl.ToString());  // Afectación al IGV por ítem
                        cmd.Parameters.AddWithValue("@DesD1", glosser + " " + glosser2 + " " + descrip);             // "Servicio de Transporte de carga terrestre "
                        cmd.Parameters.AddWithValue("@DesD2", "");                  //"Dice contener Enseres domésticos"
                        cmd.Parameters.AddWithValue("@CodIn", "");                  // código del item
                        cmd.Parameters.AddWithValue("@ValUn", valunit.ToString());  // Valor unitario del ítem
                        cmd.Parameters.AddWithValue("@ValPe", "");                  // peso
                        cmd.Parameters.AddWithValue("@UniMe", "ZZ");    // dataGridView1.Rows[i].Cells[13].Value.ToString()
                        cmd.Parameters.AddWithValue("@GuiaT", dataGridView1.Rows[i].Cells[0].Value.ToString());     // serie(4)-numero(8)
                        cmd.Parameters.AddWithValue("@CodTG", "31");
                        cmd.Parameters.AddWithValue("@PIgvn", v_igv);
                        cmd.Parameters.AddWithValue("@CodSI", "10");                // Código de tipo de afectación del IGV
                        cmd.Parameters.AddWithValue("@CodST", "1000");              // codigo sunat del tributo, (1000)
                        cmd.Parameters.AddWithValue("@NomSI", "IGV");               // nombre sunat del impuesto
                        cmd.Parameters.AddWithValue("@NomII", "VAT");               // nombre internacional del impuesto
                        cmd.ExecuteNonQuery();
                    }
                }
                // llamada al programa de generación del xml del comprobante
                string rutalocal = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                //string[] parametros = new string[] { rutaxml, Program.ruc, tx_serie.Text + "-" + tx_numero.Text };
                ProcessStartInfo p = new ProcessStartInfo();                                                // true = firma comprobante
                p.Arguments = rutaxml + " " + Program.ruc + " " +
                     cdvta + " " + 
                    true + " " + rutaCertifc + " " + claveCertif + " " + tipdo;
                p.FileName = @rutalocal + "/xmlDocVta/xmlDocVta.exe";
                var proc = Process.Start(p);
                proc.WaitForExit();
                if (proc.ExitCode == 1) retorna = true;
                else retorna = false;

                retorna = true;
            }

            return retorna;
        }
        #endregion

        #endregion

        #region autocompletados
        private void autodepa()                 // se jala en el load
        {
            if (dataUbig == null)
            {
                DataTable dataUbig = (DataTable)CacheManager.GetItem("ubigeos");
            }
            DataRow[] depar = dataUbig.Select("depart<>'00' and provin='00' and distri='00'");
            departamentos.Clear();
            foreach (DataRow row in depar)
            {
                departamentos.Add(row["nombre"].ToString());
            }
        }
        private void autoprov(string donde)                 // se jala despues de ingresado el departamento
        {
            switch(donde)
            {
                case "cliente":
                    if (tx_dptoRtt.Text.Trim() != "")
                    {
                        DataRow[] provi = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                        provincias.Clear();
                        foreach (DataRow row in provi)
                        {
                            provincias.Add(row["nombre"].ToString());
                        }
                    }
                    break;
                case "partida":
                    if (tx_dp_dep.Text.Trim() != "")
                    {
                        DataRow[] provi = dataUbig.Select("depart='" + tx_dat_upo.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                        provincias.Clear();
                        foreach (DataRow row in provi)
                        {
                            provincias.Add(row["nombre"].ToString());
                        }
                    }
                    break;
                case "llegada":
                    if (tx_dd_dep.Text.Trim() != "")
                    {
                        DataRow[] provi = dataUbig.Select("depart='" + tx_dat_upd.Text.Substring(0, 2) + "' and provin<>'00' and distri='00'");
                        provincias.Clear();
                        foreach (DataRow row in provi)
                        {
                            provincias.Add(row["nombre"].ToString());
                        }
                    }
                    break;
            } 
        }
        private void autodist(string donde)                 // se jala despues de ingresado la provincia
        {
            switch (donde)
            {
                case "cliente":
                    if (tx_ubigRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "")
                    {
                        DataRow[] distr = dataUbig.Select("depart='" + tx_ubigRtt.Text.Substring(0, 2) + "' and provin='" + tx_ubigRtt.Text.Substring(2, 2) + "' and distri<>'00'");
                        distritos.Clear();
                        foreach (DataRow row in distr)
                        {
                            distritos.Add(row["nombre"].ToString());
                        }
                    }
                    break;
                case "partida":
                    if (tx_dat_upo.Text.Trim() != "" && tx_dp_pro.Text.Trim() != "")
                    {
                        DataRow[] distr = dataUbig.Select("depart='" + tx_dat_upo.Text.Substring(0, 2) + "' and provin='" + tx_dat_upo.Text.Substring(2, 2) + "' and distri<>'00'");
                        distritos.Clear();
                        foreach (DataRow row in distr)
                        {
                            distritos.Add(row["nombre"].ToString());
                        }
                    }
                    break;
                case "llegada":
                    if (tx_dat_upd.Text.Trim() != "" && tx_dd_pro.Text.Trim() != "")
                    {
                        DataRow[] distr = dataUbig.Select("depart='" + tx_dat_upd.Text.Substring(0, 2) + "' and provin='" + tx_dat_upd.Text.Substring(2, 2) + "' and distri<>'00'");
                        distritos.Clear();
                        foreach (DataRow row in distr)
                        {
                            distritos.Add(row["nombre"].ToString());
                        }
                    }
                    break;
            }
        }
        #endregion autocompletados

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
            panel2.Enabled = false;
        }
        private void escribe()
        {
            lp.escribe(this);
            tx_nomRem.ReadOnly = true;
            tx_serie.ReadOnly = true;
            //tx_dirRem.ReadOnly = true;
            //tx_dptoRtt.ReadOnly = true;
            //tx_provRtt.ReadOnly = true;
            //tx_distRtt.ReadOnly = true;
            panel2.Enabled = true;
        }
        private void limpiar()
        {
            lp.limpiar(this);
            //cargaunica();
            {
                panel2.Enabled = false;
                tx_dat_dpo.Enabled = false;
                tx_dat_dpd.Enabled = false;
                //
                tx_pla_placa.Text = "";
                tx_pla_confv.Text = "";
                tx_pla_autor.Text = "";
                tx_rucT.Text = "";
                tx_razonS.Text = "";
                tx_fecini.Text = "";
                tx_cetm.Text = "";
                tx_cutm.Text = "";
                tx_dniChof.Text = "";
                tx_valref1.Text = "";
                tx_valref2.Text = "";
                tx_valref3.Text = "";
                tx_dat_dpo.Text = "";
                tx_dp_dep.Text = "";
                tx_dp_pro.Text = "";
                tx_dp_dis.Text = "";
                tx_dat_upo.Text = "";
                tx_dat_dpd.Text = "";
                tx_dd_dep.Text = "";
                tx_dd_pro.Text = "";
                tx_dd_dis.Text = "";
                tx_dat_upd.Text = "";
                tx_dat_nombd.Text = "Bultos";
                tx_dat_nombd.ReadOnly = true;
                //
                rb_si.Checked = false;
                rb_no.Checked = false;
                rb_contado.Checked = false;
                rb_credito.Checked = false;
            }
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
            cmb_plazoc.SelectedIndex = -1;
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void bt_agr_Click(object sender, EventArgs e)
        {
            if (tx_serGR.Text.Trim() != "" && tx_numGR.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                // validamos que no se repita la GR
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() == (tx_serGR.Text.Trim() + "-" + tx_numGR.Text.Trim()))
                        {
                            MessageBox.Show("Esta repitiendo la Guía!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tx_numGR.Text = "";
                            tx_numGR.Focus();
                            return;
                        }
                    }
                }
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
                    if (datguias[1].Trim() == "")       // el detalle de la GR tiene descripción??? 
                    {
                        MessageBox.Show("La GR no tiene el detalle completo!", "Error en Guía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx_numGR.Text = "";
                        tx_numGR.Focus();
                        return;
                    }
                    else
                    {
                        rb_desGR.PerformClick();
                    }
                }
                //dataGridView1.Rows.Clear(); nooooo, se puede hacer una fact de varias guias, n guias
                dataGridView1.Rows.Add(datguias[0], datguias[1], datguias[2], datguias[3], datguias[4], datguias[5], datguias[6], datguias[9], datguias[10], datguias[7], datguias[15],datguias[16],datguias[16],datguias[17]);     // insertamos en la grilla los datos de la GR
                int totfil = 0;
                int totcant = 0;
                decimal totflet = 0;    // acumulador en moneda de la GR 
                decimal totflMN = 0;
                tx_dat_mone.Text = datguias[7].ToString();
                cmb_mon.SelectedValue = datguias[7].ToString();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null)
                    {
                        totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());
                        totfil += 1;
                        if (tx_dat_mone.Text != MonDeft)
                        {
                            totflet = totflet + decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()); // VALOR de la GR
                            totflMN = totflMN + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString()); // VALOR DE LA GR EN MONEDA LOCAL
                        }
                        else
                        {
                            totflet = totflet + decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()); // VALOR DE LA GR EN SU MONEDA
                            totflMN = totflMN + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString()); // VALOR DE LA GR EN MONEDA LOCAL
                        }
                    }
                }
                tx_tfmn.Text = totflMN.ToString("#0.00");
                tx_totcant.Text = totcant.ToString();
                tx_tfil.Text = totfil.ToString();
                tx_flete.Text = totflet.ToString("#0.00");
                tx_fletMN.Text = totflMN.ToString("#0.00"); // Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                tx_tipcam.Text = datguias[8].ToString();
                if (tx_dat_mone.Text != MonDeft && datguias[9].ToString().Substring(0,10) != tx_fechope.Text)
                {
                    // llamanos a tipo de cambio
                    vtipcam vtipcam = new vtipcam("", tx_dat_mone.Text, DateTime.Now.Date.ToString());
                    var result = vtipcam.ShowDialog();
                    //tx_flete.Text = vtipcam.ReturnValue1;
                    //tx_fletMN.Text = vtipcam.ReturnValue2;
                    tx_tipcam.Text = (vtipcam.ReturnValue3 == null)? "0" : vtipcam.ReturnValue3;
                    tx_fletMN.Text = Math.Round(decimal.Parse(tx_flete.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                }
                if (int.Parse(tx_tfil.Text) == int.Parse(v_mfildet))
                {
                    MessageBox.Show("Número máximo de filas de detalle", "El formato no permite mas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.AllowUserToAddRows = false;
                }
                else
                {
                    dataGridView1.AllowUserToAddRows = true;
                }
                rb_no.Enabled = true;
                // comprobación de filas de guias, pagos y saldos, si hay + de 1 fila y alguna esta pagada => no se permite cobrar automatico
                if (dataGridView1.Rows.Count >= 3 && decimal.Parse(tx_dat_saldoGR.Text) <= 0)
                {
                    MessageBox.Show("El presente comprobante no se " + Environment.NewLine +
                         "puede cobrar en automático", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rb_si.Checked = false;
                    rb_si.Enabled = false;
                    tx_salxcob.Text = tx_flete.Text;
                    tx_pagado.Text = "0";
                }
                if (dataGridView1.Rows.Count <= 2 && decimal.Parse(tx_dat_saldoGR.Text) <= 0)
                {
                    MessageBox.Show("La GR esta cancelada, el documento de venta"+ Environment.NewLine +
                         "se creará con el estado cancelado","Atención verifique",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    //rb_si.PerformClick();
                    rb_no.Enabled = false;
                    rb_si.Enabled = false;
                    tx_salxcob.Text = "0";
                    tx_pagado.Text = tx_flete.Text;
                }
                else
                {
                    //tx_flete.ReadOnly = true;
                    if (cusdscto.Contains(asd)) tx_flete.ReadOnly = false;
                    else tx_flete.ReadOnly = true;
                }
                tx_flete_Leave(null, null);
                rb_si.Checked = false;
                rb_no.Checked = false;   // true
                cargaunica();               // llamamos a la carga de carga de datos complementarios para la detracción y otros mas  ... 16/02/2024
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            #region validaciones
            if (tx_serie.Text.Trim() == "" || tx_serie.Text.Trim() == "0000")
            {
                MessageBox.Show("Seleccione la serie", " Atención ");
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
            /*
            if (tx_dat_tdec.Text != tx_dat_tdRem.Text)
            {
                MessageBox.Show("Asegurese que el tipo de documento de venta" + Environment.NewLine +
                    "sean coincidente con el tipo de cliente", "Error de tipos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_docRem.Focus();
                return;
            }*/
            if (tx_dat_tdec.Text.Trim() != "" && (tx_dat_tdec.Text != tx_dat_tdRem.Text))   // las notas de venta NO deben tener codigo doc cliente asociado 15/08/2021
            {
                // aca validamos que el tipo de doc de venta se corresponda con el documento del cliente
                if (tx_dat_tdv.Text != codfact)
                {
                    if (!tdocsBol.Contains(tx_dat_tdRem.Text))
                    {
                        MessageBox.Show("Asegurese que el tipo de documento de venta" + Environment.NewLine +
                            "sean coincidente con el tipo de cliente", "Error de tipo Boleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmb_docRem.Focus();
                        return;
                    }
                }
                else
                {
                    if (!tdocsFac.Contains(tx_dat_tdRem.Text))
                    {
                        MessageBox.Show("Asegurese que el tipo de documento de venta" + Environment.NewLine +
                            "sean coincidente con el tipo de cliente", "Error de tipo Factura", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmb_docRem.Focus();
                        return;
                    }
                }
            }
            if (tx_dat_tdv.Text.Trim() == "")
            {
                cmb_tdv.Focus();
                return;
            }
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // valida contado o credito
                if (rb_contado.Checked == false && rb_credito.Checked == false)
                {
                    MessageBox.Show("Seleccione si el comprobante se mitirá" + Environment.NewLine +
                         "al Contado o al Crédito", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // valida pago y calcula
                if (rb_si.Checked == false && rb_no.Checked == false && rb_no.Enabled == true)
                {
                    MessageBox.Show("Seleccione si se cancela la factura o no","Atención - Confirme",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    rb_si.Focus();
                    return;
                }
                // valida que no se pueda hacer BOLETAS al crédito
                if (rb_credito.Checked == true && tx_dat_tdv.Text == codBole)
                {
                    MessageBox.Show("No esta permitido hacer BOLETAS" + Environment.NewLine +
                        "al Crédito!", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rb_contado.Focus();
                    return;
                }
                if (tx_pagado.Text.Trim() == "" && tx_salxcob.Text.Trim() == "")
                {
                    MessageBox.Show("Seleccione si se cancela la factura o no", "Atención - Confirme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rb_si.Focus();
                    return;
                }
                if (tx_dat_mone.Text != MonDeft && tx_tipcam.Text == "" || tx_tipcam.Text == "0")
                {
                    MessageBox.Show("Problemas con el tipo de cambio","Atención",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    cmb_mon.Focus();
                    return;
                }
                if (tx_dat_mone.Text != MonDeft && decimal.Parse(tx_tipcam.Text) > 1)
                {
                    if (Math.Round(decimal.Parse(tx_tfmn.Text), 1) != Math.Round(decimal.Parse(tx_fletMN.Text), 1))
                    {
                        var aa = MessageBox.Show("El valor a facturar no puede ser diferente al valor de la(s) GR","Confirme por favor",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                        if (aa == DialogResult.No)
                        {
                            tx_flete.Focus();
                            return;
                        }
                    }
                }
                if (fshoy != lib.fechCajaLoc(TransCarga.Program.almuser, codGene) && rb_si.Checked == true) // si la caja esta abierta permite cobrar sino NO!
                {
                    MessageBox.Show("No puede cobrar en automático", "No existe caja abierta");
                    rb_no.PerformClick();
                }
                if (chk_cunica.Checked == true)
                {
                    if (tx_cetm.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese el tonelaje","Atención",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        tx_cetm.Focus();
                        return;
                    }
                    else
                    {
                        if (double.Parse(tx_cetm.Text) > 99)
                        {
                            MessageBox.Show("El peso excede la capacidad", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tx_cetm.Focus();
                            return;
                        }
                    }
                    if (tx_dniChof.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese DNI del Chofer", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_dniChof.Focus();
                        return;
                    }
                    if (tx_dat_upo.Text.Trim().Length != 6 || tx_dat_upd.Text.Trim().Length != 6)
                    {
                        MessageBox.Show("Complete los datos de Dpto. Prov. o Dist" + Environment.NewLine + 
                            "en origen o Destino de la carga","Error en Direcciones",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        tx_dat_dpo.Focus();
                        return;
                    }
                    if (tx_pla_placa.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese la placa del vehículo", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_pla_placa.Focus();
                        return;
                    }
                    if (tx_pla_autor.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese la autorización de circulación", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_pla_autor.Focus();
                        return;
                    }
                    if (tx_rucT.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese el Ruc del transportista", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_rucT.Focus();
                        return;
                    }
                    if (tx_razonS.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese el nombre del transportista", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_razonS.Focus();
                        return;
                    }
                    if (tx_fecini.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese el fecha del inicio del traslado", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_fecini.Focus();
                        return;
                    }
                    if (tx_dat_dpo.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese la dirección de partida", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_dat_dpo.Focus();
                        return;
                    }
                    if (tx_dat_dpd.Text.Trim() == "")
                    {
                        MessageBox.Show("Ingrese la dirección de llegada", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tx_dat_dpd.Focus();
                        return;
                    }
                    if (dataGridView1.Rows[0].Cells[1].Value.ToString().Trim().Length > 149)
                    {
                        MessageBox.Show("Longitud de la descripción del detalle es muy larga","Atención",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        dataGridView1.Focus();
                        return;
                    }
                }
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (lib.DirectoryVisible(rutatxt) == true)      // OJO, crear ruta aunque sea para las notas de venta sin fact. electronica 15/08/2021
                        {
                            if (graba() == true)  // 
                            {
                                if (factElec(nipfe, "txt", "alta", 0) == true)       // facturacion electrónica
                                {
                                    // actualizamos la tabla seguimiento de usuarios
                                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                                    if (resulta != "OK")
                                    {
                                        MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    //  TODO DOC.VTA. SE ENVIA A LA ETIQUETERA DE FRENTE ... 28/10/2020
                                    //  AL GRABAR SE ASUME IMPRESA 28/10/2020 ... ya no 13/12/2020
                                    var bb = MessageBox.Show("Desea imprimir el documento?" + Environment.NewLine +
                                        "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (bb == DialogResult.Yes)
                                    {
                                        Bt_print.PerformClick();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No se puede generar el documento de venta electrónico" + Environment.NewLine +
                                        "Se generó una anulación interna para el presente documento", "Error en proveedor de Fact.Electrónica");
                                    iserror = "si";
                                    anula("INT");
                                    /*
                                    MessageBox.Show("No se puede grabar el documento de venta electrónico", "Error en conexión");
                                    iserror = "si";
                                    */
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se puede grabar el documento de venta electrónico", "Error en conexión");
                                iserror = "si";
                            }
                        }
                        else
                        {
                            MessageBox.Show("No existe ruta o no es valida para" + Environment.NewLine +
                                        "generar comprobante - " + rutatxt, "Ruta para Fact.Electrónica", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            iserror = "si";
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
                if (true)
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
                            if (Program.vg_tius == "TPU001" && Program.vg_nius == "NIV000")   // solo todo poderoso puede regenerar txt
                            {
                                var zz = MessageBox.Show("Desea regenerar el TXT?","Confirme por favor",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                                if (zz == DialogResult.Yes)
                                {
                                    if (factElec(nipfe, "txt", "alta", 0) == true)       // facturacion electrónica
                                    {

                                    }
                                    else
                                    {
                                        MessageBox.Show("No se puede generar el documento de venta electrónico" + Environment.NewLine +
                                            "Se generó una anulación interna para el presente documento", "Error en proveedor de Fact.Electrónica");
                                        iserror = "si";
                                        anula("INT");
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
                if (tx_idcob.Text != "")
                {
                    MessageBox.Show("El documento de venta tiene Cobranza activa" + Environment.NewLine +
                        "La cobranza permanece sin cambios", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //tx_numero.Focus();
                    //return;
                }
                // validaciones de fecha para poder anular
                DateTime fedv = DateTime.Parse(tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                TimeSpan span = DateTime.Parse(lib.fechaServ("ansi")) - fedv;
                if (span.Days > v_cdpa)
                {
                    // no se puede anular ... a menos que sea un usuario autorizado
                    if (codusanu.Contains(asd))
                    {
                        // SOLO USUARIOS AUTORIZADOS DEBEN ACCEDER A ESTA OPCIÓN
                        // SE ANULA EL DOCUMENTO Y SE HACEN LOS MOVIMIENTOS INTERNOS
                        // LA ANULACION EN EL PROVEEDOR DE FACT. ELECTRONICA SE HACE A MANO POR EL ENCARGADO ... 28/10/2020 ya no al 09/01/2021
                        // la anulacion debe generar un TXT de comunicacion de baja y guardarse en el directorio del prov. de fact. electronica 09/01/2021
                        if (tx_idr.Text.Trim() != "")
                        {
                            var aa = MessageBox.Show("Confirma que desea ANULAR el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (aa == DialogResult.Yes)
                            {
                                if (lib.DirectoryVisible(rutatxt) == true)
                                {
                                    int cta = anula("FIS");      // cantidad de doc.vtas anuladas en la fecha
                                    if (factElec(nipfe, "txt", "baja", cta) == true)
                                    {
                                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                                        if (resulta != "OK")
                                        {
                                            MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No existe ruta o no es valida para" + Environment.NewLine +
                                        "generar la anulación electrónica","Ruta para Fact.Electrónica",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                                    iserror = "si";
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
                            MessageBox.Show("El documento ya debe existir para anular", "No esta el Id del registro", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se puede anular por estar fuera de plazo","Usuario no permito",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea ANULAR el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            if (lib.DirectoryVisible(rutatxt) == true)
                            {
                                int cta = anula("FIS");      // cantidad de doc.vtas anuladas en la fecha
                                if (factElec(nipfe, "txt", "baja", cta) == true)
                                {
                                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                                    if (resulta != "OK")
                                    {
                                        MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("No existe ruta o no es valida para" + Environment.NewLine +
                                        "generar la anulación electrónica", "Ruta para Fact.Electrónica", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                iserror = "si";
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
                        MessageBox.Show("El documento ya debe existir para anular", "No esta el Id del registro", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
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
            }
            initIngreso();          // limpiamos todo para volver a empesar
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
                    micon.Parameters.AddWithValue("td", tx_dat_tdv.Text);
                    micon.Parameters.AddWithValue("ser", tx_serie.Text);
                    using (MySqlDataReader dr0 = micon.ExecuteReader())
                    {
                        if (dr0.Read())
                        {
                            if (dr0[0] != null && dr0.GetString(0) != "")
                            {
                                tx_numero.Text = lib.Right("00000000" + dr0.GetString(0), 8);
                                //tx_numero.Text = lib.Right("00000000" + "4948", 8);     // ojo OJO ojo CAMBIAR ESTO
                            }
                        }
                    }
                }
                if (tx_tipcam.Text == "") tx_tipcam.Text = "0";
                decimal fletMN = 0;
                decimal subtMN = 0;
                decimal igvtMN = 0;
                if (tx_dat_mone.Text != MonDeft)
                {
                    if (tx_tipcam.Text == "0" || tx_fletMN.Text == "")
                    {
                        MessageBox.Show("Error con el tipo de cambio", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return retorna;
                    }
                    else
                    {
                        fletMN = Math.Round(decimal.Parse(tx_fletMN.Text), 3);
                        subtMN = Math.Round(fletMN / (1 + decimal.Parse(v_igv)/100), 3);
                        igvtMN = Math.Round(fletMN - subtMN, 3);
                    }
                }
                else
                {
                    fletMN = Math.Round(decimal.Parse(tx_flete.Text), 3);
                    subtMN = Math.Round(decimal.Parse(tx_subt.Text), 3);
                    igvtMN = Math.Round(decimal.Parse(tx_igv.Text), 3);
                }
                // comprobamos si los datos del cliente tienen cambios
                if (rb_remGR.Checked == true)
                {
                    if (datcltsR[3].ToString().Trim() != tx_dirRem.Text.Trim() ||
                        datcltsR[6].ToString().Trim() != tx_telc1.Text.Trim() ||
                        datcltsR[5].ToString().Trim() != tx_email.Text.Trim() ||
                        datcltsR[4].ToString().Trim() != tx_ubigRtt.Text.Trim())
                    {
                        tx_dat_m1clte.Text = "E";
                    }
                }
                if (rb_desGR.Checked == true)
                {
                    if (datcltsD[3].ToString().Trim() != tx_dirRem.Text.Trim() ||
                        datcltsD[6].ToString().Trim() != tx_telc1.Text.Trim() ||
                        datcltsD[5].ToString().Trim() != tx_email.Text.Trim() ||
                        datcltsD[4].ToString().Trim() != tx_ubigRtt.Text.Trim())
                    {
                        tx_dat_m1clte.Text = "E";
                    }
                }
                string inserta = "insert into cabfactu (" +
                    "fechope,martdve,tipdvta,serdvta,numdvta,ticltgr,tidoclt,nudoclt,nombclt,direclt,dptoclt,provclt,distclt,ubigclt,corrclt,teleclt," +
                    "locorig,dirorig,ubiorig,obsdvta,canfidt,canbudt,mondvta,tcadvta,subtota,igvtota,porcigv,totdvta,totpags,saldvta,estdvta,frase01," +
                    "tipoclt,m1clien,tippago,ferecep,impreso,codMN,subtMN,igvtMN,totdvMN,pagauto,tipdcob,idcaja,plazocred,porcendscto,valordscto," +
                    "cargaunica,placa,confveh,autoriz,detPeso,detputil,detMon1,detMon2,detMon3,dirporig,ubiporig,dirpdest,ubipdest,conPago," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                    "@fechop,@mtdvta,@ctdvta,@serdv,@numdv,@tcdvta,@tdcrem,@ndcrem,@nomrem,@dircre,@dptocl,@provcl,@distcl,@ubicre,@mailcl,@telecl," +
                    "@ldcpgr,@didegr,@ubdegr,@obsprg,@canfil,@totcpr,@monppr,@tcoper,@subpgr,@igvpgr,@porcigv,@totpgr,@pagpgr,@salxpa,@estpgr,@frase1," +
                    "@ticlre,@m1clte,@tipacc,@feredv,@impSN,@codMN,@subMN,@igvMN,@totMN,@pagaut,@tipdco,@idcaj,@plazc,@pordesc,@valdesc," +
                    "@caruni,@placa,@confv,@autor,@dPeso,@dputil,@dMon1,@dMon2,@dMon3,@dporig,@uporig,@dpdest,@updest,@conPag," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@mtdvta", cmb_tdv.Text.Substring(0,1));
                    micon.Parameters.AddWithValue("@ctdvta", tx_dat_tdv.Text);
                    micon.Parameters.AddWithValue("@serdv", tx_serie.Text);
                    micon.Parameters.AddWithValue("@numdv", tx_numero.Text);
                    micon.Parameters.AddWithValue("@tcdvta", (rb_remGR.Checked == true)? "1" : (rb_desGR.Checked == true)? "2" : "3");
                    micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@dptocl", tx_dptoRtt.Text);
                    micon.Parameters.AddWithValue("@provcl", tx_provRtt.Text);
                    micon.Parameters.AddWithValue("@distcl", tx_distRtt.Text);
                    micon.Parameters.AddWithValue("@ubicre", tx_ubigRtt.Text);
                    micon.Parameters.AddWithValue("@mailcl", tx_email.Text);
                    micon.Parameters.AddWithValue("@telecl", tx_telc1.Text);
                    micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                    micon.Parameters.AddWithValue("@didegr", dirloc);                             // direccion origen
                    micon.Parameters.AddWithValue("@ubdegr", ubiloc);                             // ubigeo origen
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@canfil", tx_tfil.Text);     // cantidad de filas de detalle
                    micon.Parameters.AddWithValue("@totcpr", tx_totcant.Text);  // total bultos
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@tcoper", tx_tipcam.Text);                   // TIPO DE CAMBIO
                    micon.Parameters.AddWithValue("@subpgr", tx_subt.Text);                     // sub total
                    micon.Parameters.AddWithValue("@igvpgr", tx_igv.Text);                      // igv
                    micon.Parameters.AddWithValue("@porcigv", v_igv);                           // porcentaje en numeros de IGV
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);                    // total inc. igv
                    micon.Parameters.AddWithValue("@pagpgr", (rb_si.Checked == true) ? tx_fletMN.Text : "0");  // (tx_pagado.Text == "") ? "0" : tx_pagado.Text);
                    micon.Parameters.AddWithValue("@salxpa", (tx_salxcob.Text == "") ? "0" : tx_salxcob.Text);
                    micon.Parameters.AddWithValue("@estpgr", (tx_pagado.Text == "" || tx_pagado.Text == "0.00") ? tx_dat_estad.Text : codCanc); // estado
                    micon.Parameters.AddWithValue("@frase1", "");                   // no hay nada que poner 19/11/2020
                    micon.Parameters.AddWithValue("@ticlre", tx_dat_tcr.Text);      // tipo de cliente credito o contado
                    micon.Parameters.AddWithValue("@m1clte", tx_dat_m1clte.Text);
                    micon.Parameters.AddWithValue("@tipacc", v_mpag);                   // pago del documento x defecto si nace la fact pagada
                    micon.Parameters.AddWithValue("@feredv", DBNull.Value);         // si es pago contado la fecha de recep del doc. es la misma fecha
                    micon.Parameters.AddWithValue("@impSN", "N");
                    micon.Parameters.AddWithValue("@codMN", MonDeft);               // codigo moneda local
                    micon.Parameters.AddWithValue("@subMN", subtMN);
                    micon.Parameters.AddWithValue("@igvMN", igvtMN);
                    micon.Parameters.AddWithValue("@totMN", fletMN);
                    micon.Parameters.AddWithValue("@pagaut", (rb_si.Checked == true)? "S" : "N");
                    micon.Parameters.AddWithValue("@tipdco", (rb_si.Checked == true)? v_codcob : "");
                    micon.Parameters.AddWithValue("@idcaj", (rb_si.Checked == true)? tx_idcaja.Text : "0");
                    micon.Parameters.AddWithValue("@plazc", (rb_no.Checked == true)? tx_dat_plazo.Text : "");  // (rb_no.Checked == true)? codppc : "");
                    micon.Parameters.AddWithValue("@pordesc", (tx_dat_porcDscto.Text.Trim() == "") ? "0" : tx_dat_porcDscto.Text);
                    micon.Parameters.AddWithValue("@valdesc", (tx_valdscto.Text.Trim() == "") ? "0" : tx_valdscto.Text);
                    micon.Parameters.AddWithValue("@caruni", (chk_cunica.Checked == true)? 1 : 0);
                    micon.Parameters.AddWithValue("@placa", tx_pla_placa.Text);
                    micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text);
                    micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                    micon.Parameters.AddWithValue("@dPeso", (tx_cetm.Text.Trim() == "")? "0" : tx_cetm.Text);
                    micon.Parameters.AddWithValue("@dputil", (tx_cutm.Text.Trim() == "")? "0" : tx_cutm.Text);
                    micon.Parameters.AddWithValue("@dMon1", (tx_valref1.Text.Trim() == "")? "0" : tx_valref1.Text);
                    micon.Parameters.AddWithValue("@dMon2", (tx_valref2.Text.Trim() == "")? "0" : tx_valref2.Text);
                    micon.Parameters.AddWithValue("@dMon3", (tx_valref3.Text.Trim() == "")? "0" : tx_valref3.Text);
                    micon.Parameters.AddWithValue("@dporig", tx_dat_dpo.Text);
                    micon.Parameters.AddWithValue("@uporig", tx_dat_upo.Text);
                    micon.Parameters.AddWithValue("@dpdest", tx_dat_dpd.Text);
                    micon.Parameters.AddWithValue("@updest", tx_dat_upd.Text);
                    micon.Parameters.AddWithValue("@conPag", (rb_contado.Checked == true)? "0" : "1");  // 0=contado, 1=credito
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
                // detalle
                if (dataGridView1.Rows.Count > 0)
                {
                    int fila = 1;
                    int tfg = (dataGridView1.Rows.Count == int.Parse(v_mfildet)) ? int.Parse(v_mfildet) : dataGridView1.Rows.Count - 1;
                    for (int i = 0; i < tfg; i++)  // int i = 0; i < dataGridView1.Rows.Count - 1; i++
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                        {

                            string inserd2 = "update detfactu set " +
                                "codgror=@guia,cantbul=@bult,unimedp=@unim,descpro=@desc,pesogro=@peso,codmogr=@codm,totalgr=@pret,codMN=@cmnn," +
                                "totalgrMN=@tgrmn,pagauto=@pagaut " +
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
                                micon.Parameters.AddWithValue("@pagaut", (rb_si.Checked == true) ? "S" : "N");
                                micon.ExecuteNonQuery();
                                fila += 1;
                                // retorna = true;         // no hubo errores!
                            }
                        }
                    }
                    retorna = true;         // no hubo errores!
                }
                // adicionales a la factura
                if (true)         // chk_cunica.Checked == true  ... 16/02/2024
                {
                    string insert = "insert into adifactu (idc,tipoAd,placa,confv,autoriz,cargaEf,cargaUt,rucTrans,nomTrans,fecIniTras,dirPartida,ubiPartida," +
                        "dirDestin,ubiDestin,dniChof,brevete,valRefViaje,valRefVehic,valRefTon) " +
                        "values (@idr,@tiad,@plac,@conf,@auto,@dPes,@dput,@ruct,@nomt,@feit,@dpor,@upor," +
                        "@dpde,@upde,@dnic,@brec,@dMon1,@dMon2,@dMon3)";
                    using (MySqlCommand micon = new MySqlCommand(insert, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@tiad", (chk_cunica.Checked == true) ? "1" : "0");    // 1=carga unica
                        micon.Parameters.AddWithValue("@plac", tx_pla_placa.Text);
                        micon.Parameters.AddWithValue("@conf", tx_pla_confv.Text);
                        micon.Parameters.AddWithValue("@auto", tx_pla_autor.Text);
                        micon.Parameters.AddWithValue("@dPes", (tx_cetm.Text.Trim() == "") ? "0" : tx_cetm.Text);
                        micon.Parameters.AddWithValue("@dput", (tx_cutm.Text.Trim() == "") ? "0" : tx_cutm.Text);
                        micon.Parameters.AddWithValue("@ruct", tx_rucT.Text);
                        micon.Parameters.AddWithValue("@nomt", tx_razonS.Text);
                        micon.Parameters.AddWithValue("@feit", tx_fecini.Text);
                        micon.Parameters.AddWithValue("@dpor", tx_dat_dpo.Text);
                        micon.Parameters.AddWithValue("@upor", tx_dat_upo.Text);
                        micon.Parameters.AddWithValue("@dpde", tx_dat_dpd.Text);
                        micon.Parameters.AddWithValue("@upde", tx_dat_upd.Text);
                        micon.Parameters.AddWithValue("@dnic", tx_dniChof.Text);
                        micon.Parameters.AddWithValue("@brec", "");
                        micon.Parameters.AddWithValue("@dMon1", (tx_valref1.Text.Trim() == "") ? "0" : tx_valref1.Text);
                        micon.Parameters.AddWithValue("@dMon2", (tx_valref2.Text.Trim() == "") ? "0" : tx_valref2.Text);
                        micon.Parameters.AddWithValue("@dMon3", (tx_valref3.Text.Trim() == "") ? "0" : tx_valref3.Text);
                        //
                        micon.ExecuteNonQuery();
                        //
                        retorna = true;         // no hubo errores!
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
                        string actua = "update cabfactu a set obsdvta=@obsprg," +
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
        private int anula(string tipo)
        {
            int ctanul = 0;
            // en el caso de documentos de venta HAY 1: ANULACION FISICA ... 28/10/2020
            // tambien podría haber ANULACION interna con la serie ANU1 .... 19/11/2020
            // Anulacion fisica se "anula" el numero del documento en sistema y en fisico se tacha y en prov. fact.electronica se da baja de numeracion
            // se borran todos los enlaces mediante triggers en la B.D.
            if (tipo == "FIS")
            {
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string canul = "update cabfactu set estdvta=@estser,obsdvta=@obse,usera=@asd,fecha=now()," +
                            "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp,estintreg=@eiar " +
                            "where id=@idr";
                        using (MySqlCommand micon = new MySqlCommand(canul, conn))
                        {
                            micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                            micon.Parameters.AddWithValue("@estser", codAnul);
                            micon.Parameters.AddWithValue("@obse", tx_obser1.Text);
                            micon.Parameters.AddWithValue("@asd", asd);
                            micon.Parameters.AddWithValue("@dil4", lib.iplan());
                            micon.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                            micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                            micon.Parameters.AddWithValue("@veap", verapp);
                            micon.Parameters.AddWithValue("@eiar", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
                            micon.ExecuteNonQuery();
                        }
                        string consul = "select count(id) from cabfactu where date(fecha)=@fech and estdvta=@estser";
                        using (MySqlCommand micon = new MySqlCommand(consul, conn))
                        {
                            micon.Parameters.AddWithValue("@fech", tx_fechact.Text.Substring(6, 4) + "-" + tx_fechact.Text.Substring(3, 2) + "-" + tx_fechact.Text.Substring(0, 2));
                            micon.Parameters.AddWithValue("@estser", codAnul);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    ctanul = dr.GetInt32(0);
                                }
                            }
                        }
                    }
                }
            }
            if (tipo == "INT")
            {
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string canul = "update cabfactu set serdvta=@sain,estdvta=@estser,obsdvta=@obse,usera=@asd,fecha=now()," +
                            "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp,estintreg=@eiar " +
                            "where id=@idr";
                        using (MySqlCommand micon = new MySqlCommand(canul, conn))
                        {
                            micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                            micon.Parameters.AddWithValue("@sain", v_sanu);
                            micon.Parameters.AddWithValue("@estser", codAnul);
                            micon.Parameters.AddWithValue("@obse", tx_obser1.Text);
                            micon.Parameters.AddWithValue("@asd", asd);
                            micon.Parameters.AddWithValue("@dil4", lib.iplan());
                            micon.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                            micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                            micon.Parameters.AddWithValue("@veap", verapp);
                            micon.Parameters.AddWithValue("@eiar", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
                            micon.ExecuteNonQuery();
                        }
                        /*  05/11/2021
                        string updser = "update series set actual=actual-1 where tipdoc=@tipd AND serie=@serd";
                        using (MySqlCommand micon = new MySqlCommand(updser, conn))
                        {
                            micon.Parameters.AddWithValue("@tipd", tx_dat_tdv.Text);
                            micon.Parameters.AddWithValue("@serd", tx_serie.Text);
                            micon.ExecuteNonQuery();
                        }
                        */
                    }
                }
            }
            return ctanul;
        }
        #endregion boton_form;

        #region leaves y checks
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                var aa = tx_idr.Text;
                limpiar();
                tx_idr.Text = aa;
                dataGridView1.Rows.Clear();
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
            }
        }
        private void tx_nomRem_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_nomRem);
        }
        private void tx_dirRem_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_dirRem);
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento del remitente, jala provincia
        {
            if(tx_dptoRtt.Text.Trim() != "")    //  && TransCarga.Program.vg_conSol == false
            {
                DataRow[] row = dataUbig.Select("nombre='" + tx_dptoRtt.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_ubigRtt.Text = row[0].ItemArray[1].ToString(); // lib.retCodubigeo(tx_dptoRtt.Text.Trim(),"","");
                    autoprov("cliente");
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
                    autodist("cliente");
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
        private void tx_dp_dep_Leave(object sender, EventArgs e)        // departamento del punto de partida
        {
            if (tx_dp_dep.Text.Trim() != "")
            {
                DataRow[] row = dataUbig.Select("nombre='" + tx_dp_dep.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_dat_upo.Text = row[0].ItemArray[1].ToString();
                    autoprov("partida");
                }
                else tx_dp_dep.Text = "";
            }
            if (tx_dp_dep.Text.Trim() == "")
            {
                tx_dat_upo.Text = "";
            }
        }
        private void tx_dp_pro_Leave(object sender, EventArgs e)        // provincia del punto de partida
        {
            // tx_dp_pro.Text.Trim() != "" && tx_dp_dep.Text.Trim() != ""
            if (tx_dat_upo.Text.Trim().Length >= 2)
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_dat_upo.Text.Substring(0, 2) + "' and nombre='" + tx_dp_pro.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_dat_upo.Text = tx_dat_upo.Text.Trim().Substring(0, 2) + row[0].ItemArray[2].ToString();
                    autodist("partida");
                }
                else tx_dp_pro.Text = "";
            }
            else
            {
                tx_dp_pro.Text = "";
            }
            if (tx_dp_pro.Text.Trim() == "")
            {
                tx_dat_upo.Text = "";
            }
        }
        private void tx_dp_dis_Leave(object sender, EventArgs e)        // distrito del punto de partida
        {
            // tx_dp_dis.Text.Trim() != "" && tx_dp_pro.Text.Trim() != "" && tx_dp_dep.Text.Trim() != ""
            if (tx_dat_upo.Text.Trim().Length >= 4)
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_dat_upo.Text.Substring(0, 2) + "' and provin='" + tx_dat_upo.Text.Substring(2, 2) + "' and nombre='" + tx_dp_dis.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_dat_upo.Text = tx_dat_upo.Text.Trim().Substring(0, 4) + row[0].ItemArray[3].ToString();
                }
                else tx_dp_dis.Text = "";
            }
            else
            {
                tx_dp_dis.Text = "";
            }
            if (tx_dp_dis.Text.Trim() == "")
            {
                tx_dat_upo.Text = "";
            }
        }
        private void tx_dd_dep_Leave(object sender, EventArgs e)        // departamento del punto de llegada
        {
            if (tx_dd_dep.Text.Trim() != "")
            {
                DataRow[] row = dataUbig.Select("nombre='" + tx_dd_dep.Text.Trim() + "' and provin='00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_dat_upd.Text = row[0].ItemArray[1].ToString();
                    autoprov("llegada");
                }
                else tx_dd_dep.Text = "";
            }
            if (tx_dd_dep.Text.Trim() == "")
            {
                tx_dat_upd.Text = "";
            }
        }
        private void tx_dd_pro_Leave(object sender, EventArgs e)        // provincia del punto de llegada
        {
            // tx_dd_pro.Text.Trim() != "" && tx_dd_dep.Text.Trim() != ""
            if (tx_dat_upd.Text.Trim().Length >= 2)
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_dat_upd.Text.Substring(0, 2) + "' and nombre='" + tx_dd_pro.Text.Trim() + "' and provin<>'00' and distri='00'");
                if (row.Length > 0)
                {
                    tx_dat_upd.Text = tx_dat_upd.Text.Trim().Substring(0, 2) + row[0].ItemArray[2].ToString();
                    autodist("llegada");
                }
                else tx_dd_pro.Text = "";
            }
            else
            {
                tx_dd_pro.Text = "";
            }
            if (tx_dd_pro.Text.Trim() == "")
            {
                tx_dat_upd.Text = "";
            }
        }
        private void tx_dd_dis_Leave(object sender, EventArgs e)        // distrito del punto de llegada
        {
            // tx_dd_dis.Text.Trim() != "" && tx_dd_pro.Text.Trim() != "" && tx_dd_dep.Text.Trim() != ""
            if (tx_dat_upd.Text.Trim().Length >= 4)
            {
                DataRow[] row = dataUbig.Select("depart='" + tx_dat_upd.Text.Substring(0, 2) + "' and provin='" + tx_dat_upd.Text.Substring(2, 2) + "' and nombre='" + tx_dd_dis.Text.Trim() + "'");
                if (row.Length > 0)
                {
                    tx_dat_upd.Text = tx_dat_upd.Text.Trim().Substring(0, 4) + row[0].ItemArray[3].ToString();
                }
                else tx_dd_dis.Text = "";
            }
            else
            {
                tx_dd_dis.Text = "";
            }
            if (tx_dd_dis.Text.Trim() == "")
            {
                tx_dat_upd.Text = "";
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
                    if (datos[0] != "")  // datos.Length > 0
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
                        tx_email.Text = datos[7];
                        tx_email.Select(0, 0);
                        tx_telc1.Text = datos[6];
                        tx_telc1.Select(0, 0);
                        encuentra = "si";
                        tx_dat_m1clte.Text = "E";
                    }
                    else
                    {
                        {
                            // llamada a la funcion
                            string[] biene = busqueda_clt_conector(tx_dat_tdRem.Text, tx_numDocRem.Text);
                            tx_nomRem.Text = biene[0];   // razon social
                            //biene[1];                  // ubigeo
                            tx_dirRem.Text = biene[2];    // direccion
                            tx_dptoRtt.Text = biene[3];     // departamento
                            tx_provRtt.Text = biene[4];     // provincia
                            tx_distRtt.Text = biene[5];     // distrito
                            tx_dat_m1clte.Text = "N";
                        }
                    }
                    /*
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
                                tx_dat_m1clte.Text = "N";
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
                                tx_dat_m1clte.Text = "N";
                            }
                        }
                    }
                    */
                }
            }
            if (tx_numDocRem.Text.Trim() != "" && tx_mld.Text.Trim() == "")
            {
                cmb_docRem.Focus();
            }
        }
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_numero.Text.Trim() != "")
            {
                // en el caso de las pre guias el numero es el mismo que el ID del registro
                tx_numero.Text = lib.Right("00000000" + tx_numero.Text, 8);
                var aa = tx_numero.Text;
                var bb = tx_dat_tdv.Text;
                var cc = tx_serie.Text;
                limpiar();
                tx_dat_tdv.Text = bb;
                cmb_tdv.SelectedValue = bb;
                tx_serie.Text = cc;
                tx_numero.Text = aa;
                jalaoc("sernum");
                dataGridView1.Rows.Clear();
                jaladet(tx_idr.Text);
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            //tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
            if (Tx_modo.Text == "NUEVO") tx_serGR.Focus();
        }
        private void tx_flete_Leave(object sender, EventArgs e)
        {
            if (tx_flete.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                tx_flete.Text = Math.Round(decimal.Parse(tx_flete.Text), 2).ToString("#0.00");
                calculos(decimal.Parse((tx_flete.Text.Trim() != "") ? tx_flete.Text : "0"));
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
                        if (Math.Round(decimal.Parse(tx_tfmn.Text),1) != Math.Round(decimal.Parse(tx_fletMN.Text),1))   // OJO, no hacemos dscto en moneda diferente al nacional
                        {
                            var aa = MessageBox.Show("El flete en M.N. de la(s) guía(s) no es" + Environment.NewLine + 
                                "igual al flete en M.N. del comprobante actual" + Environment.NewLine +
                                "Continúa?","No coinciden valores en M.N.",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                            if (aa == DialogResult.No)
                            {
                                tx_flete.Text = "";
                                tx_flete.Focus();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // si el valor del flete es menor al valor de tx_tfmn ===> tiene descuento
                    // si tiene descuento, visibiliza campo descuento y calcula monto y %
                    if (Math.Round(decimal.Parse(tx_flete.Text), 1) < Math.Round(decimal.Parse(tx_tfmn.Text), 1))
                    {
                        lin_dscto.Visible = true;
                        lb_dscto.Visible = true;
                        tx_valdscto.Visible = true;
                        // calculos
                        tx_valdscto.Text = (Math.Round(decimal.Parse(tx_tfmn.Text), 1) - Math.Round(decimal.Parse(tx_flete.Text), 1)).ToString("#0.0");
                        tx_dat_porcDscto.Text = ((Math.Round(decimal.Parse(tx_flete.Text), 1) * 100) / Math.Round(decimal.Parse(tx_tfmn.Text), 1)).ToString("#0.00");
                        // calcula detalle
                        int filas = dataGridView1.Rows.Count - 1;
                        // vdf = decimal.Parse(tx_valdscto.Text) / (decimal)filas;
                        for (int i=0; i<dataGridView1.Rows.Count - 1; i++)
                        {
                            decimal vdf = Math.Round((decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) * decimal.Parse(tx_dat_porcDscto.Text))/100,2);
                            //dataGridView1.Rows[i].Cells[12].Value = (decimal.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()) - vdf).ToString("#0.00");
                            dataGridView1.Rows[i].Cells[12].Value = vdf.ToString("#0.00");
                        }
                    }
                    else
                    {
                        if (Math.Round(decimal.Parse(tx_flete.Text), 1) > Math.Round(decimal.Parse(tx_tfmn.Text), 1))
                        {
                            MessageBox.Show("No se permite facturar montos de las guías","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            tx_flete.Text = tx_tfmn.Text;
                        }
                        lin_dscto.Visible = false;
                        lb_dscto.Visible = false;
                        tx_valdscto.Visible = false;
                    }
                }
                DataRow[] row = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");
                NumLetra numLetra = new NumLetra();
                tx_fletLetras.Text = numLetra.Convertir(tx_flete.Text,true) + row[0][3].ToString().Trim();
                button1.Focus();
            }
        }
        private void tx_serGR_Leave(object sender, EventArgs e)
        {
            //tx_serGR.Text = lib.Right("0000" + tx_serGRx.Text, 4);

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
            tx_dptoRtt.Text = "";
            tx_provRtt.Text = "";
            tx_distRtt.Text = "";
            if (datcltsR[4].ToString().Trim() != "")
            {
                if (datcltsR[4].Trim().Length >= 2)
                {
                    DataRow[] row = dataUbig.Select("depart='" + datcltsR[4].Substring(0, 2) + "' and provin='00' and distri='00'");
                    tx_dptoRtt.Text = row[0].ItemArray[4].ToString();
                    if (datcltsR[4].Trim().Length >= 4)
                    {
                        row = dataUbig.Select("depart='" + datcltsR[4].Substring(0, 2) + "' and provin ='" + datcltsR[4].Substring(2, 2) + "' and distri='00'");
                        tx_provRtt.Text = row[0].ItemArray[4].ToString();
                        if (datcltsR[4].Trim().Length >= 5)
                        {
                            row = dataUbig.Select("depart='" + datcltsR[4].Substring(0, 2) + "' and provin ='" + datcltsR[4].Substring(2, 2) + "' and distri='" + datcltsR[4].Substring(4, 2) + "'");
                            tx_distRtt.Text = row[0].ItemArray[4].ToString();
                        }
                    }
                }
                //
                tx_email.Text = datcltsR[5];
                tx_telc1.Text = datcltsR[6];
                tx_telc2.Text = datcltsR[7];
                tx_ubigRtt.Text = datcltsR[4];
            }
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
            tx_dptoRtt.Text = "";
            tx_provRtt.Text = "";
            tx_distRtt.Text = "";
            try
            {
                if (datcltsD[4].ToString().Trim() != "")
                {
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
                    tx_ubigRtt.Text = datcltsD[4];
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error en datos del Destinatario " + Environment.NewLine + ex.Message, "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                if (tx_dat_m1clte.Text != "N") tx_dat_m1clte.Text = "E";
            }
        }
        private void tx_telc1_Leave(object sender, EventArgs e)
        {
            if (tx_telc1.Text.Trim() != "") //  && (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
            {
                val_NoCaracteres(tx_telc1);
                if (tx_dat_m1clte.Text != "N") tx_dat_m1clte.Text = "E";
            }
        }
        private void tx_pla_placa_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_pla_placa);
        }
        private void tx_pla_confv_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_pla_confv);
        }
        private void tx_pla_autor_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_pla_autor);
        }
        private void tx_obser1_Leave(object sender, EventArgs e)
        {
            val_NoCaracteres(tx_obser1);
        }
        private void rb_contado_Click(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")
            {
                if (rb_contado.Checked == true)
                {
                    rb_si.Checked = false;
                    rb_si.Enabled = true;
                    rb_no.Checked = false;
                    rb_no.Enabled = true;
                    cmb_plazoc.SelectedIndex = -1;
                    tx_dat_dpla.Text = "";
                    cmb_plazoc.Enabled = false;
                }
            }
        }
        private void rb_credito_Click(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")
            {
                if (rb_credito.Checked == true)
                {
                    rb_si.Checked = false;
                    rb_si.Enabled = false;
                    rb_no.Checked = true;
                    rb_no.Enabled = true;
                    cmb_plazoc.Enabled = true;
                }
            }
        }
        private void rb_si_Click(object sender, EventArgs e)
        {
            if (tx_idcaja.Text != "")
            {
                // validamos la fecha de la caja
                string fhoy = lib.fechaServ("ansi");
                if (fhoy != TransCarga.Program.vg_fcaj)  // ambas fecahs formato yyyy-mm-dd
                {
                    MessageBox.Show("Debe cerrar la caja anterior!", "Caja fuera de fecha", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    rb_si.Checked = false;
                    rb_no.PerformClick();
                    return;
                }
                else
                {
                    if (tx_dat_saldoGR.Text.Trim() != "")
                    {
                        if (tx_dat_mone.Text == MonDeft)
                        {
                            if (decimal.Parse(tx_dat_saldoGR.Text) > 0)
                            {
                                tx_pagado.Text = tx_fletMN.Text;     // tx_flete.Text;
                                tx_salxcob.Text = "0.00";
                                tx_salxcob.BackColor = Color.Green;
                            }
                            else
                            {
                                tx_salxcob.Text = "0.00";
                                tx_dat_plazo.Text = "";
                                cmb_plazoc.SelectedIndex = -1;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Solo puede cancelar en moneda local","Atención",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            rb_no.PerformClick();
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No existe caja abierta!" + Environment.NewLine +
                    "No puede cobrar hasta aperturar caja", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                rb_si.Checked = false;
                rb_no.PerformClick();
            }
        }
        private void rb_no_Click(object sender, EventArgs e)
        {
            double once = 0;
            for (int i = 0; i<dataGridView1.Rows.Count - 1; i++)
            {
                if (string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[11].Value.ToString())) dataGridView1.Rows[i].Cells[11].Value = "0";
                once = once + double.Parse(dataGridView1.Rows[i].Cells[11].Value.ToString());
            }
            tx_pagado.Text = "0.00";
            tx_salxcob.Text = once.ToString("#0.00"); // tx_flete.Text;
            tx_salxcob.BackColor = Color.Red;
            if (rb_credito.Checked == true)
            {
                cmb_plazoc.Enabled = true;
                cmb_plazoc.SelectedValue = codppc;
                tx_dat_plazo.Text = codppc;
                cmb_plazoc.Enabled = true;
                cmb_plazoc.SelectedValue = codppc;
                tx_dat_plazo.Text = codppc;
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
                //else 
            }
        }
        private void chk_cunica_CheckedChanged(object sender, EventArgs e)
        {
            //cargaunica();
            if (chk_cunica.Checked == true)
            {
                if (Tx_modo.Text != "NUEVO") panel2.Enabled = false;
                else panel2.Enabled = true;
                dataGridView1.ReadOnly = false;
                for(int i=0; i<dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                }
                dataGridView1.Columns[1].ReadOnly = false;
            }
            else
            {
                dataGridView1.ReadOnly = true;
                panel2.Enabled = false;
            }
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
        private void tx_rucT_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR") && tx_rucT.Text != "" && tx_rucT.Text != Program.ruc)
            {
                MessageBox.Show("Falta registrar el registro MTC del 3ro","Falta adecuar");
                tx_rucT.Text = "";
                tx_razonS.Text = "";
                return;
            }
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
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            escribe();
            // 
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            tx_salxcob.BackColor = Color.White;
            // validamos la fecha de la caja
            fshoy = lib.fechaServ("ansi");
            //
            tx_flete.ReadOnly = true;
            initIngreso();
            tx_numero.ReadOnly = true;
            cmb_tdv_SelectedIndexChanged(null, null);
            cmb_tdv.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            sololee();          
            Tx_modo.Text = "EDITAR";                    // solo puede editarse la observacion 28/10/2020
            button1.Image = Image.FromFile(img_grab);
            tx_flete.ReadOnly = true;
            initIngreso();
            gbox_serie.Enabled = true;
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            tx_serie.ReadOnly = false;
            tx_numero.Text = "";
            tx_numero.ReadOnly = false;
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            tx_salxcob.BackColor = Color.White;
            cmb_tdv.Focus();
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
                if (tx_numero.Text.Trim() == "")
                {
                    MessageBox.Show("Debe grabar el comprobante!","Atención",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
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
            cmb_tdv.Focus();
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
            /*if (Tx_modo.Text == "NUEVO" && tx_totcant.Text != "")    //  || Tx_modo.Text == "EDITAR"
            {   // lo de totcant es para accionar solo cuando el detalle de la GR se haya cargado
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    DataRow[] row = dtm.Select("idcodice='"+ tx_dat_mone.Text+"'");
                    tx_dat_monsunat.Text = row[0][2].ToString();
                    tipcambio(tx_dat_mone.Text);
                    if (tx_flete.Text != "" && tx_flete.Text != "0.00") calculos(decimal.Parse(tx_flete.Text));
                    if (rb_no.Checked == true) rb_no_Click(null,null);
                    if (rb_si.Checked == true) rb_si_Click(null, null);
                    if (tx_dat_mone.Text != MonDeft)
                    {
                        tx_flete.ReadOnly = false;
                        tx_flete.Focus();
                    }
                    else
                    {
                        if (decimal.Parse(tx_dat_saldoGR.Text) <= 0)
                        {
                            if (cusdscto.Contains(asd)) tx_flete.ReadOnly = false;
                            else tx_flete.ReadOnly = true;
                        }
                        else
                        {
                            tx_flete.ReadOnly = true;
                        }
                    }
                }
            } */
        }
        private void cmb_tdv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_tdv.SelectedIndex > -1)
            {
                DataRow[] row = dttd1.Select("idcodice='" + cmb_tdv.SelectedValue.ToString() + "'");
                if (row.Length > 0)
                {
                    tx_dat_tdv.Text = row[0].ItemArray[0].ToString();
                    tx_dat_tdec.Text = row[0].ItemArray[2].ToString();
                    glosser = row[0].ItemArray[4].ToString();
                    if (Tx_modo.Text == "NUEVO") tx_serie.Text = row[0].ItemArray[5].ToString();
                    tx_numero.Text = "";
                }
            }
        }
        private void cmb_mon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_totcant.Text != "")    //  || Tx_modo.Text == "EDITAR"
            {   // lo de totcant es para accionar solo cuando el detalle de la GR se haya cargado
                if (cmb_mon.SelectedValue.ToString() != tx_dat_mone.Text) // cmb_mon.SelectedIndex > -1
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    DataRow[] row = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");
                    tx_dat_monsunat.Text = row[0][2].ToString();
                    tipcambio(tx_dat_mone.Text);
                    if (tx_flete.Text != "" && tx_flete.Text != "0.00") calculos(decimal.Parse(tx_flete.Text));
                    if (rb_no.Checked == true) rb_no_Click(null, null);
                    if (rb_si.Checked == true) rb_si_Click(null, null);
                    if (tx_dat_mone.Text != MonDeft)
                    {
                        tx_flete.ReadOnly = false;
                        tx_flete.Focus();
                    }
                    else
                    {
                        if (decimal.Parse(tx_dat_saldoGR.Text) <= 0)
                        {
                            if (cusdscto.Contains(asd)) tx_flete.ReadOnly = false;
                            else tx_flete.ReadOnly = true;
                        }
                        else
                        {
                            tx_flete.ReadOnly = true;
                        }
                    }
                }
            }
        }
        private void cmb_plazoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_plazoc.SelectedIndex > -1)
            {
                tx_dat_plazo.Text = cmb_plazoc.SelectedValue.ToString();
                DataRow[] dias = dtp.Select("idcodice='" + tx_dat_plazo.Text + "'");
                foreach (DataRow row in dias)
                {
                    tx_dat_dpla.Text = row[3].ToString();
                }
            }
            else
            {
                tx_dat_plazo.Text = "";
                tx_dat_dpla.Text = "";
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
            //llenaDataSet();                         // metemos los datos al dataset de la impresion
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            //try
            {
                //printDocument1.PrinterSettings.PrinterName = v_impTK;
                //printDocument1.Print();

                string[] vs = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",      // 20
                               "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};     // 20
                string[] va = { "", "", "", "", "", "", "", "", "" };       // 9
                string[,] dt = new string[10, 9] {
                    { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" },
                    { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }
                }; // 6 columnas, 10 filas
                string[] cu = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };    // 17

                vs[0] = cmb_tdv.Text.Substring(0, 1).ToUpper() + lib.Right(tx_serie.Text, 3);   // serie (F001)
                vs[1] = tx_numero.Text;                 // numero
                vs[2] = tx_dat_tdv.Text;                // tx_dat_tdv.Text, codigo Transcarga del tipo de documento
                vs[3] = Program.dirfisc;                // direccion emisor
                if (tx_dat_tdv.Text == codBole) vs[4] = "Boleta de Venta Electrónica";
                if (tx_dat_tdv.Text == codfact) vs[4] = "Factura Electrónica";
                vs[5] = tx_fechope.Text;                // fecha de emision formato dd/mm/aaaa
                vs[6] = tx_nomRem.Text;                 // nombre del cliente del comprobante
                vs[7] = tx_numDocRem.Text;              // numero documento del cliente
                vs[8] = tx_dirRem.Text;                 // dirección cliente
                vs[9] = tx_distRtt.Text;                // distrito de la direccion
                vs[10] = tx_provRtt.Text;               // provincia de la direccion
                vs[11] = tx_dptoRtt.Text;               // departamento de la dirección
                vs[12] = tx_tfil.Text;                  // cantidad de filas de detalle
                vs[13] = tx_subt.Text;                  // Sub total del comprobante
                vs[14] = tx_igv.Text;                   // igv del comprobante
                vs[15] = tx_flete.Text;                 // importe total del comprobante
                vs[16] = cmb_mon.Text;                  // Simbolo de la moneda
                vs[17] = tx_fletLetras.Text;            // flete en letras
                vs[18] = (rb_credito.Checked == true) ? "CREDITO" : "CONTADO";
                vs[19] = tx_dat_dpla.Text;              // dias de plazo credito
                vs[20] = glosdetra;                     // Glosa para la detracción
                vs[21] = tipdo;                         // codigo sunat tipo comprobante
                vs[22] = tipoDocEmi;                    // CODIGO SUNAT tipo de documento RUC/DNI del cliente
                vs[23] = nipfe;                         // identificador de ose/pse metodo de envío
                vs[24] = restexto;                      // texto del resolucion sunat del ose/pse
                vs[25] = autoriz_OSE_PSE;               // autoriz del ose/pse
                vs[26] = webose;                        // web del ose/pse
                vs[27] = tx_digit.Text.Trim();          // usuario creador
                vs[28] = tx_locuser.Text;               // local de emisión
                vs[29] = despedida;                     // glosa despedida
                // detalle
                int tfg = (dataGridView1.Rows.Count == int.Parse(v_mfildet)) ? int.Parse(v_mfildet) : dataGridView1.Rows.Count - 1;
                for (int l = 0; l < tfg; l++)
                {
                    string textF2 = dataGridView1.Rows[l].Cells["OriDest"].Value.ToString() + " - " +
                        dataGridView1.Rows[l].Cells["Cant"].Value.ToString() + " " + dataGridView1.Rows[l].Cells["umed"].Value.ToString();
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[l].Cells[0].Value.ToString()))
                    {
                        dt[l, 0] = dataGridView1.Rows[l].Cells["OriDest"].Value.ToString();
                        dt[l, 1] = dataGridView1.Rows[l].Cells["Cant"].Value.ToString();
                        dt[l, 2] = dataGridView1.Rows[l].Cells["umed"].Value.ToString();
                        dt[l, 3] = dataGridView1.Rows[l].Cells[0].Value.ToString();             // guia transportista
                        dt[l, 4] = dataGridView1.Rows[l].Cells[1].Value.ToString();             // descripcion de la carga
                        dt[l, 5] = dataGridView1.Rows[l].Cells[8].Value.ToString();             // documento relacionado remitente de la guia transportista
                    }
                }
                // varios
                va[0] = logoclt;         // Ruta y nombre del logo del emisor electrónico
                va[1] = glosser;         // glosa del servicio en facturacion
                va[2] = codfact;         // siglas nombre de tipo de documento Factura 
                va[3] = "";         // 
                va[4] = "";         // 
                va[5] = "";         // 
                va[6] = "";         // 
                va[7] = "";         // 
                va[8] = "";         // 

                impDV impTK = new impDV(1, v_impTK, vs, dt, va, cu, "TK", "");

                if (File.Exists(@otro))
                {
                    //File.Delete(@"C:\test.txt");
                    File.Delete(@otro);
                }

                retorna = true;
            }
            
            /*catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error en Impresora o plazo " + v_impTK);
                retorna = false;
            }*/
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
                // imprime_TK(sender, e);
                string[] vs = {"","","","","","","","","","","","","", "", "", "", "", "", "", "",   // 20
                               "", "", "", "", "", "", "", "", "", ""};    // 10
                string[] va = { "", "", "", "", "", "", "", "", "" };       // 9
                string[,] dt = new string[10, 6] {
                    { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" },
                    { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }
                }; // 6 columnas, 10 filas
                string[] cu = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };    // 17

                vs[0] = cmb_tdv.Text.Substring(0, 1).ToUpper() + lib.Right(tx_serie.Text, 3);   // serie (F001)
                vs[1] = tx_numero.Text;                 // numero
                vs[2] = tx_dat_tdv.Text;                // tx_dat_tdv.Text, codigo Transcarga del tipo de documento
                vs[3] = Program.dirfisc;                // direccion emisor
                if (tx_dat_tdv.Text == codBole) vs[4] = "Boleta de Venta Electrónica";
                if (tx_dat_tdv.Text == codfact) vs[4] = "Factura Electrónica";
                vs[5] = tx_fechope.Text;                // fecha de emision formato dd/mm/aaaa
                vs[6] = tx_nomRem.Text;                 // nombre del cliente del comprobante
                vs[7] = tx_numDocRem.Text;              // numero documento del cliente
                vs[8] = tx_dirRem.Text;                 // dirección cliente
                vs[9] = tx_distRtt.Text;                // distrito de la direccion
                vs[10] = tx_provRtt.Text;               // provincia de la direccion
                vs[11] = tx_dptoRtt.Text;               // departamento de la dirección
                vs[12] = tx_tfil.Text;                  // cantidad de filas de detalle
                vs[13] = tx_subt.Text;                  // Sub total del comprobante
                vs[14] = tx_igv.Text;                   // igv del comprobante
                vs[15] = tx_flete.Text;                 // importe total del comprobante
                vs[16] = cmb_mon.Text;                  // Simbolo de la moneda
                vs[17] = tx_fletLetras.Text;            // flete en letras
                vs[18] = (rb_credito.Checked == true) ? "CREDITO" : "CONTADO";
                vs[19] = tx_dat_dpla.Text;              // dias de plazo credito
                vs[20] = glosdetra;                     // Glosa para la detracción
                vs[21] = tipdo;                         // codigo sunat tipo comprobante
                vs[22] = tipoDocEmi;                    // CODIGO SUNAT tipo de documento RUC/DNI del cliente
                vs[23] = nipfe;                         // identificador de ose/pse metodo de envío
                vs[24] = restexto;                      // texto del resolucion sunat del ose/pse
                vs[25] = autoriz_OSE_PSE;               // autoriz del ose/pse
                vs[26] = webose;                        // web del ose/pse
                vs[27] = tx_digit.Text.Trim();          // usuario creador
                vs[28] = tx_locuser.Text;               // local de emisión
                vs[29] = despedida;                     // glosa despedida
                vs[30] = "";                            // libre 
                // detalle
                int tfg = (dataGridView1.Rows.Count == int.Parse(v_mfildet)) ? int.Parse(v_mfildet) : dataGridView1.Rows.Count - 1;
                for (int l = 0; l < tfg; l++)
                {
                    string textF2 = dataGridView1.Rows[l].Cells["OriDest"].Value.ToString() + " - " +
                        dataGridView1.Rows[l].Cells["Cant"].Value.ToString() + " " + dataGridView1.Rows[l].Cells["umed"].Value.ToString();
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[l].Cells[0].Value.ToString()))
                    {
                        dt[l, 0] = dataGridView1.Rows[l].Cells["OriDest"].Value.ToString();
                        dt[l, 1] = dataGridView1.Rows[l].Cells["Cant"].Value.ToString();
                        dt[l, 2] = dataGridView1.Rows[l].Cells["umed"].Value.ToString();
                        dt[l, 3] = dataGridView1.Rows[l].Cells[0].Value.ToString();             // guia transportista
                        dt[l, 4] = dataGridView1.Rows[l].Cells[1].Value.ToString();             // descripcion de la carga
                        dt[l, 5] = dataGridView1.Rows[l].Cells[8].Value.ToString();             // documento relacionado remitente de la guia transportista
                    }
                }
                // varios
                va[0] = logoclt;         // Ruta y nombre del logo del emisor electrónico
                va[1] = glosser;         // glosa del servicio en facturacion
                va[2] = codfact;         // siglas nombre de tipo de documento Factura 
                va[3] = "";         // 
                va[4] = "";         // 
                va[5] = "";         // 
                va[6] = "";         // 
                va[7] = "";         // 
                va[8] = "";         // 

                impDV impTK = new impDV(1, v_impTK, vs, dt, va, cu, "TK", "");
                
                if (File.Exists(@otro))
                {
                    //File.Delete(@"C:\test.txt");
                    File.Delete(@otro);
                }

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
                Image photo = Image.FromFile(logoclt);
                for (int i = 1; i <= copias; i++)
                {
                    PointF puntoF = new PointF(coli, posi);
                    // imprimimos el logo o el nombre comercial del emisor
                    if (logoclt != "")
                    {
                        SizeF cuadLogo = new SizeF(CentimeterToPixel(anchTik) - 20.0F, alfi * 6);
                        RectangleF reclogo = new RectangleF(puntoF, cuadLogo);
                        e.Graphics.DrawImage(photo, reclogo);
                    }
                    else
                    {
                        e.Graphics.DrawString(nomclie, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // nombre comercial
                    }
                    float lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(nomclie, lt_gra).Width) / 2;
                    posi = posi + alfi * 7;
                    lt = (ancho - e.Graphics.MeasureString(rasclie, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(rasclie, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // razon social
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dom.Fiscal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // direccion emisor
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    SizeF cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    RectangleF recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(dirclie, lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);     // direccion emisor
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Sucursal", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // direccion emisor
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(dirloc, lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);     // direccion emisor
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("RUC ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // ruc de emisor
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    e.Graphics.DrawString(rucclie, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);     // ruc de emisor
                    //string tipdo = cmb_tdv.Text;                                  // tipo de documento
                    string serie = cmb_tdv.Text.Substring(0, 1).ToUpper() + lib.Right(tx_serie.Text,3);                    // serie electrónica
                    string corre = tx_numero.Text;                                // numero del documento electrónico
                    //string nota = tipdo + "-" + serie + "-" + corre;
                    string titdoc = "";
                    if (tx_dat_tdv.Text == codBole) titdoc = "Boleta de Venta Electrónica";
                    if (tx_dat_tdv.Text == codfact) titdoc = "Factura Electrónica";
                    if (tx_dat_tdv.Text != codBole && tx_dat_tdv.Text != codfact) titdoc = "NOTA DE VENTA";
                    posi = posi + alfi + 8;
                    lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titdoc, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titdoc, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // tipo de documento
                    posi = posi + alfi + 8;
                    string titnum = serie + " - " + corre;
                    lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(titnum, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);   // serie y numero
                    posi = posi + alfi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("F. Emisión", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic); // fecha y hora emision
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    e.Graphics.DrawString(tx_fechope.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic); // fecha y hora emision
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Cliente", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // DNI/RUC cliente
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    if (tx_nomRem.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    else cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 1);
                    recdom = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_nomRem.Text.Trim(), lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);                  // DNI/RUC cliente
                    if (tx_nomRem.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                    else posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("RUC", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);    // nombre del cliente
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    e.Graphics.DrawString(tx_numDocRem.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);    // ruc/dni del cliente
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Dirección", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);  // direccion
                    puntoF = new PointF(coli + 65, posi);
                    e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 70, posi);
                    string dipa = tx_dirRem.Text.Trim() + Environment.NewLine + tx_distRtt.Text.Trim() + " - " + tx_provRtt.Text.Trim() + " - " + tx_dptoRtt.Text.Trim();
                    if (dipa.Length < 60) cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                    else cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 3);
                    RectangleF recdir = new RectangleF(puntoF, cuad);
                    e.Graphics.DrawString(tx_dirRem.Text.Trim() + Environment.NewLine +
                        tx_distRtt.Text.Trim() + " - " + tx_provRtt.Text.Trim() + " - " + tx_dptoRtt.Text.Trim(),
                        lt_peq, Brushes.Black, recdir, StringFormat.GenericTypographic);  // direccion
                    if (dipa.Length < 60) posi = posi + alfi + alfi;
                    else posi = posi + alfi + alfi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(" ", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    // **************** detalle del documento ****************//
                    StringFormat alder = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                    SizeF siz = new SizeF(70, 15);
                    RectangleF recto = new RectangleF(puntoF, siz);
                    int tfg = (dataGridView1.Rows.Count == int.Parse(v_mfildet)) ? int.Parse(v_mfildet) : dataGridView1.Rows.Count - 1;
                    for (int l = 0; l < tfg; l++)  // int l = 0; l < dataGridView1.Rows.Count - 1; l++
                    {
                        string textF2 = dataGridView1.Rows[l].Cells["OriDest"].Value.ToString() + " - " + 
                            dataGridView1.Rows[l].Cells["Cant"].Value.ToString() + " " + dataGridView1.Rows[l].Cells["umed"].Value.ToString();
                        if (!string.IsNullOrEmpty(dataGridView1.Rows[l].Cells[0].Value.ToString()))
                        {
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString(glosser, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString(textF2, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            puntoF = new PointF(coli, posi);
                            string qqq = "GRT-" + dataGridView1.Rows[l].Cells[0].Value.ToString() + " " + dataGridView1.Rows[l].Cells[1].Value.ToString();
                            if (qqq.Length > 41) siz = new SizeF(CentimeterToPixel(anchTik), 30);
                            else siz = new SizeF(CentimeterToPixel(anchTik), 15);
                            recto = new RectangleF(puntoF, siz);
                            e.Graphics.DrawString(qqq, lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                            posi = posi + alfi;
                            if (qqq.Length > 41) posi = posi + alfi - 4;
                            puntoF = new PointF(coli, posi);
                            e.Graphics.DrawString("Según doc.cliente: " + dataGridView1.Rows[l].Cells[8].Value.ToString(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                            posi = posi + alfi *2;
                        }
                    }
                    // pie del documento ;
                    siz = new SizeF(70, 15);
                    if (tx_dat_tdv.Text != codfact)
                    {
                        //SizeF siz = new SizeF(70, 15);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. GRAVADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recst = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(tx_subt.Text, lt_peq, Brushes.Black, recst, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. INAFECTA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recig = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recig, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. EXONERADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recex = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recex, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IGV", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recgv = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(tx_igv.Text, lt_peq, Brushes.Black, recgv, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IMPORTE TOTAL " + cmb_mon.Text, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        recto = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(tx_flete.Text, lt_peq, Brushes.Black, recto, alder);
                    }
                    if (tx_dat_tdv.Text == codfact)
                    {
                        //SizeF siz = new SizeF(70, 15);
                        //StringFormat alder = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. GRAVADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recst = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(tx_subt.Text, lt_peq, Brushes.Black, recst, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. INAFECTA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recig = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recig, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("OP. EXONERADA", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recex = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString("0.00", lt_peq, Brushes.Black, recex, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IGV", lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        RectangleF recgv = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(tx_igv.Text, lt_peq, Brushes.Black, recgv, alder);
                        posi = posi + alfi;
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString("IMPORTE TOTAL " + cmb_mon.Text, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        puntoF = new PointF(coli + 190, posi);
                        recto = new RectangleF(puntoF, siz);
                        e.Graphics.DrawString(tx_flete.Text, lt_peq, Brushes.Black, recto, alder);
                    }
                    posi = posi + alfi * 2;
                    puntoF = new PointF(coli, posi);
                    NumLetra nl = new NumLetra();
                    string monlet = "SON: " + tx_fletLetras.Text;
                    if (monlet.Length <= 30) siz = new SizeF(CentimeterToPixel(anchTik), alfi);
                    else siz = new SizeF(CentimeterToPixel(anchTik), alfi * 2);
                    recto = new RectangleF(puntoF, siz);
                    e.Graphics.DrawString(monlet, lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                    if (monlet.Length <= 30) posi = posi + alfi;
                    else posi = posi + alfi + alfi;
                    if (tx_dat_tdv.Text == codfact)
                    {
                        // forma de pago
                        posi = posi + (alfi / 1.5F);
                        string ahiva = "";
                        if (rb_no.Checked == true && rb_credito.Checked == true)    //   rb_si.Checked == true
                        {
                            string _fechc = DateTime.Parse(tx_fechope.Text).AddDays(double.Parse(tx_dat_dpla.Text)).Date.ToString("dd-MM-yyyy");    // "yyyy-MM-dd"
                            ahiva = "- AL CREDITO -" + " 1 CUOTA - VCMTO: " + _fechc;
                        }
                        else
                        {
                            ahiva = "PAGO AL CONTADO " + tx_flete.Text;
                        }
                        puntoF = new PointF(coli, posi);
                        e.Graphics.DrawString(ahiva, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi * 1.5F;
                        // leyenda de detracción
                        if (double.Parse(tx_flete.Text) > double.Parse(Program.valdetra))
                        {
                            siz = new SizeF(CentimeterToPixel(anchTik), 15 * 3);
                            puntoF = new PointF(coli, posi);
                            recto = new RectangleF(puntoF, siz);
                            e.Graphics.DrawString(glosdetra.Trim() + " " + Program.ctadetra.Trim(), lt_peq, Brushes.Black, recto, StringFormat.GenericTypographic);
                            posi = posi + alfi * 3;
                        }
                    }
                    puntoF = new PointF(coli, posi);
                    string repre = "Representación impresa de la";
                    lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(repre, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(repre, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    string previo = "";
                    if (tx_dat_tdv.Text == codBole) previo = "boleta de venta electrónica";
                    if (tx_dat_tdv.Text == codfact) previo = "factura electrónica";
                    if (tx_dat_tdv.Text != codBole && tx_dat_tdv.Text != codfact) previo = "nota de venta";
                    lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(previo, lt_med).Width) / 2;
                    puntoF = new PointF(lt, posi);
                    e.Graphics.DrawString(previo, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    //posi = posi + alfi;
                    string separ = "|";
                    string codigo = rucclie + separ + tipdo + separ +
                        serie + separ + tx_numero.Text + separ +
                        tx_igv.Text + separ + tx_flete.Text + separ +
                        tx_fechope.Text.Substring(6,4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2) + separ + tipoDocEmi + separ +
                        tx_numDocRem.Text + separ;  // string.Format("{0:yyyy-MM-dd}", tx_fechope.Text)
                    //
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
                    lt = (CentimeterToPixel(anchTik) - lib.CentimeterToPixel(3)) / 2;
                    puntoF = new PointF(lt, posi);
                    SizeF cuadro = new SizeF(lib.CentimeterToPixel(3), lib.CentimeterToPixel(3));    // 5x5 cm
                    RectangleF rec = new RectangleF(puntoF, cuadro);
                    e.Graphics.DrawImage(png, rec);
                    png.Dispose();
                    // leyenda 2
                    if (nipfe == "secure" || nipfe == "Horizont")
                    {
                        posi = posi + lib.CentimeterToPixel(3);
                        lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(restexto, lt_med).Width) / 2;
                        puntoF = new PointF(lt, posi);
                        e.Graphics.DrawString(restexto, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        posi = posi + alfi;
                        lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(autoriz_OSE_PSE, lt_med).Width) / 2;
                        puntoF = new PointF(lt, posi);
                        e.Graphics.DrawString(autoriz_OSE_PSE, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                        // centrado en rectangulo   *********************
                        StringFormat sf = new StringFormat();       //  *
                        sf.Alignment = StringAlignment.Center;      //  *
                        posi = posi + alfi + 5;
                        SizeF leyen = new SizeF(CentimeterToPixel(anchTik) - 20, alfi * 3);
                        puntoF = new PointF(coli, posi);
                        leyen = new SizeF(CentimeterToPixel(anchTik) - 20, alfi * 2);
                        RectangleF recley5 = new RectangleF(puntoF, leyen);
                        e.Graphics.DrawString(webose, lt_med, Brushes.Black, recley5, sf);
                    }
                    posi = posi + alfi * 3;
                    string locyus = tx_locuser.Text + " - " + tx_user.Text;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString(locyus, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);                  // tienda y vendedor
                    posi = posi + alfi;
                    puntoF = new PointF(coli, posi);
                    e.Graphics.DrawString("Imp. " + DateTime.Now, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    puntoF = new PointF((CentimeterToPixel(anchTik) - e.Graphics.MeasureString(despedida, lt_med).Width) / 2, posi);
                    e.Graphics.DrawString(despedida, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi + alfi;
                    //puntoF = new PointF(coli, posi);
                    //e.Graphics.DrawString(".", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                }
            }
        }
        private void updateprint(string sn)  // actualiza el campo impreso de la GR = S
        {   // S=si impreso || N=no impreso
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "update cabfactu set impreso=@sn where id=@idr";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@sn", sn);
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.ExecuteNonQuery();
                }
            }
        }
        #endregion

    }
}
