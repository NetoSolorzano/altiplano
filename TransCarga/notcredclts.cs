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
        string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        string v_cid = "";              // codigo interno de tipo de documento
        string v_fra2 = "";             // frase que va en obs de cobranza cuando se cancela desde el doc.vta.
        string v_sanu = "";             // serie anulacion interna ANU
        string v_mpag = "";             // medio de pago automatico x defecto para las cobranzas
        string v_codcob = "";           // codigo del documento cobranza
        string v_CR_gr_ind = "";        // nombre del formato FT/BV en CR
        string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_codidv = "";           // variable codifo interno de documento de venta en vista TDV
        string codfact = "";            // idcodice de factura
        string v_igv = "";              // valor igv %
        string v_estcaj = "";           // estado de la caja
        string v_idcaj = "";            // id de la caja actual
        string codAbie = "";            // codigo estado de caja abierta
        string logoclt = "";            // ruta y nombre archivo logo
        string fshoy = "";              // fecha hoy del servidor en formato ansi
        string codppc = "";             // codigo del plazo de pago por defecto para fact a crédito
        string v_codnot = "";           // codigo tipo de documento nota de credito
        //
        string rutatxt = "";            // ruta de los txt para la fact. electronica
        string tipdo = "";              // CODIGO SUNAT tipo de documento de venta
        string tipoDocEmi = "";         // CODIGO SUNAT tipo de documento RUC/DNI
        string tipoMoneda = "";         // CODIGO SUNAT tipo de moneda
        string glosdet = "";            // glosa para las operaciones con detraccion
        string glosser = "";            // glosa que va en el detalle del doc. de venta
        string restexto = "xxx";        // texto resolucion sunat autorizando prov. fact electronica
        string autoriz_OSE_PSE = "yyy"; // numero resolucion sunat autorizando prov. fact electronica
        string despedida = "";          // texto para mensajes al cliente al final de la impresión del doc.vta. 
        string webose = "";             // direccion web del ose o pse para la descarga del 
        string correo_gen = "";         // correo generico del emisor cuando el cliente no tiene correo propio
        //
        static libreria lib = new libreria();   // libreria de procedimientos
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
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";

        DataTable dtu = new DataTable();        // detalle del documento
        DataTable dttd0 = new DataTable();
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();        // moneda
        DataTable dttdn = new DataTable();      // tip doc notas cred
        string[] datcltsD = { "", "", "", "", "", "", "", "", "" };
        string[] datguias = { "", "", "", "", "", "", "", "", "", "", "" };

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
                tx_dat_tnota.Text = v_codnot;
                cmb_tnota.SelectedValue = v_codnot;
                cmb_tnota.Enabled = false;
                tx_serie.ReadOnly = true;
            }
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofi,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                micon.Parameters.AddWithValue("@nofi", "clients");
                //micon.Parameters.AddWithValue("@noco", "cobranzas");
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
                            if (row["param"].ToString() == "fe_txt") rutatxt = row["valor"].ToString().Trim();         // ruta de los txt para la fact. electronica
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
                            //if (row["param"].ToString() == "nomfor_cr") v_CR_gr_ind = row["valor"].ToString().Trim(); // le ose/pse imprime desde su portal
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();      // moneda por defecto
                        if (row["campo"].ToString() == "detraccion" && row["param"].ToString() == "glosa") glosdet = row["valor"].ToString().Trim();    // glosa detraccion
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "codinDV" && row["param"].ToString() == "DV") v_codidv = row["valor"].ToString().Trim();           // codigo de dov.vta en tabla TDV
                        if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
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
                {
                    string consulta = "select a.id,a.fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,b.descrizionerid as nomest " +
                        "from cabdebcred a left join desc_est b on b.idcodice=a.estdvta " +
                        parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    if (campo == "tx_idr")
                    {
                        micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    }
                    if (campo == "sernum")
                    {
                        micon.Parameters.AddWithValue("@tnot", tx_dat_tdv.Text);
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
                            /*
                            tx_dat_tdv.Text = dr.GetString("tipdvta");
                            tx_serie.Text = dr.GetString("serdvta");
                            tx_numero.Text = dr.GetString("numdvta");
                            tx_dat_tdRem.Text = dr.GetString("tidoclt");
                            tx_numDocRem.Text = dr.GetString("nudoclt");
                            tx_nomRem.Text = dr.GetString("nombclt");
                            tx_dirRem.Text = dr.GetString("direclt");
                            tx_dptoRtt.Text = dr.GetString("dptoclt");
                            tx_provRtt.Text = dr.GetString("provclt");
                            tx_distRtt.Text = dr.GetString("distclt");
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
                            tx_impreso.Text = dr.GetString("impreso");
                            tx_idcob.Text = dr.GetString("cobra");              // id de cobranza
                            //
                            cmb_tdv.SelectedValue = tx_dat_tdv.Text;
                            cmb_tdv_SelectedIndexChanged(null, null);
                            tx_numero.Text = dr.GetString("numdvta");       // al cambiar el indice en el combox se borra numero, por eso lo volvemos a jalar
                            cmb_mon.SelectedValue = tx_dat_mone.Text;
                            tx_estado.Text = dr.GetString("nomest");   // lib.nomstat(tx_dat_estad.Text);
                            if (dr.GetString("userm") == "") tx_digit.Text = lib.nomuser(dr.GetString("userc"));
                            else tx_digit.Text = lib.nomuser(dr.GetString("userm"));
                            */
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
            string jalad = "select filadet,codgror,cantbul,unimedp,descpro,pesogro,codmogr,totalgr " +
                "from detdebcred where idc=@idr";
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
                using (MySqlCommand cdv = new MySqlCommand("select distinct a.idcodice,a.descrizionerid,a.enlace1,a.codsunat,b.glosaser from desc_tdv a LEFT JOIN series b ON b.tipdoc = a.IDCodice where numero=@bloq and codigo=@codv", conn))
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
                // datos para combo notas cred/deb
                cmb_tnota.Items.Clear();
                using (MySqlCommand cdv = new MySqlCommand("select distinct a.idcodice,a.descrizionerid,a.enlace1,a.codsunat,b.glosaser from desc_tdv a LEFT JOIN series b ON b.tipdoc = a.IDCodice where numero=@bloq and codigo=@codn", conn))
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
        private bool validGR()    // validamos y devolvemos datos
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
                            "a.corrclt,a.teleclt,a.mondvta,a.subtota,a.igvtota,a.porcigv,a.totdvta,a.saldvta,a.subtMN,a.igvtMN,a.totdvMN,b.descrizionerid as docC " +
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
                                    tx_dat_tdRem.Text = dr.GetString("docC");
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
                                    retorna = true;
                                }
                            }
                        }
                        consulta = "SELECT a.codgror,a.cantbul,a.unimedp,a.descpro,a.totalgr,a.codMN,a.totalgrMN "+
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
                                    dataGridView1.Rows.Add(row[0], row[3], row[1], row[5], row[4], row[6], "", "", "", "");
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
            if (provee == "Horizont")
            {
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
            }
            return retorna;
        }
        private bool crearTXT(string tipdo, string serie, string corre, string file_path)
        {
            bool retorna;
            retorna = false;

            string _fecemi = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);   // fecha de emision   yyyy-mm-dd
            string Prazsoc = nomclie.Trim();                                            // razon social del emisor
            string Pnomcom = "";                                                        // nombre comercial del emisor
            string ubigEmi = ubiclie;                                                   // UBIGEO DOMICILIO FISCAL
            string Pdf_dir = Program.dirfisc.Trim();                                    // DOMICILIO FISCAL - direccion
            string Pdf_urb = "-";                                                       // DOMICILIO FISCAL - Urbanizacion
            string Pdf_pro = Program.provfis.Trim();                                    // DOMICILIO FISCAL - provincia
            string Pdf_dep = Program.depfisc.Trim();                                    // DOMICILIO FISCAL - departamento
            string Pdf_dis = Program.distfis.Trim();                                    // DOMICILIO FISCAL - distrito
            string paisEmi = "PE";                                                      // DOMICILIO FISCAL - código de país
            string Ptelef1 = Program.telclte1.Trim();                                   // teléfono del emisor
            string Pweb1 = "";                                                          // página web del emisor
            string Prucpro = Program.ruc;                                               // Ruc del emisor
            string Pcrupro = "6";                                                       // codigo Ruc emisor
            string _tipdoc = int.Parse(tipdo).ToString();                               // Tipo de documento de venta - 1 car
            string _moneda = tipoMoneda;                                                // Moneda del doc. de venta - 3 car
            string _sercor = serie + "-" + corre;                                       // Serie y correlat concatenado F001-00000001 - 13 car
            string Cnumdoc = tx_numDocRem.Text;                                         // numero de doc. del cliente - 15 car
            string Ctipdoc = tipoDocEmi;                                                // tipo de doc. del cliente - 1 car
            string Cnomcli = tx_nomRem.Text.Trim();                                     // nombre del cliente - 100 car
            string dir1Adq = tx_dirRem.Text.Trim();                                     // direccion del adquiriente 1
            string dir2Adq = "";                                                        // direccion del adquiriente 2
            string provAdq = tx_provRtt.Text.Trim();                                    // provincia del adquiriente
            string depaAdq = tx_dptoRtt.Text.Trim();                                    // departamento del adquiriente
            string distAdq = tx_distRtt.Text.Trim();                                    // distrito del adquiriente
            string paisAdq = "PE";                                                      // pais del adquiriente
            string _totoin = "0.00";                                                       // total operaciones inafectas
            string _totoex = "0.00";                                                       // total operaciones exoneradas
            string _toisc = "0.00";                                                        // total impuesto selectivo consumo
            string _totogr = tx_flete.Text;                                             // Total valor venta operaciones grabadas n(12,2)  15
            string _totven = tx_subt.Text;                                              // Importe total de la venta n(12,2)             15
            string tipOper = "0101";                                                    // tipo de operacion - 4 car
            string codLocE = Program.codlocsunat;                                       // codigo local emisor
            string conPago = "01";                                                      // condicion de pago
            string _codgui = "31";                                                      // Código de la guia de remision TRANSPORTISTA
            string _scotro = dataGridView1.Rows[0].Cells[0].Value.ToString();           // serie y numero concatenado de la guia
            string obser1 = tx_obser1.Text.Trim();                                      // observacion del documento
            string obser2 = "";                                                         // mas observaciones
            string maiAdq = tx_email.Text.Trim();                                       // correo del adquiriente
            string totImp = tx_igv.Text;                                                // total impuestos del documento
            string codImp = "1000";                                                     // codigo impuesto
            string nomImp = "IGV";                                                      // nombre del tipo de impuesto
            string tipTri = "VAT";                                                      // tipo de tributo
            string monLet = tx_fletLetras.Text.Trim();                                  // monto en letras
            string _horemi = "";                                                        // hora de emision del doc.venta
            string _fvcmto = "";                                                        // fecha de vencimiento del doc.venta
            string corclie = Program.mailclte;                                          // correo del emisor
            string _morefD = "";                                                        // moneda de refencia para el tipo de cambio
            string _monobj = "";                                                        // moneda objetivo del tipo de cambio
            string _tipcam = "";                                                        // tipo de cambio con 3 decimales
            string _fechca = "";                                                        // fecha del tipo de cambio

            string d_medpa = "";                                                        // medio de pago de la detraccion (001 = deposito en cuenta)
            string d_monde = "";                                                        // moneda de la detraccion
            string d_conpa = "";                                                        // condicion de pago
            double totdet = 0;
            string d_porde = "";                                                        // porcentaje de detraccion
            string d_valde = "";                                                        // valor de la detraccion
            string d_codse = "";                                                        // codigo de servicio
            string d_ctade = "";                                                        // cuenta detraccion BN
            string d_valre = "";                                                        // valor referencial
            string d_numre = "";                                                        // numero registro mtc del camion
            string d_confv = "";                                                        // config. vehicular del camion
            string d_ptori = "";                                                        // Pto de origen
            string d_ptode = "";                                                        // Pto de destino
            string d_vrepr = "";                                                        // valor referencial preliminar
            string codleyt = "1000";                                                    // codigoLeyenda 1 - valor en letras
            string codleyd = "";                                                        // codigo leyenda detraccion
            string codobs = "107";                                                      // codigo del ose para las observaciones, caso carrion documentos origen del remitente
            string _forpa = "";                                                         // glosa de forma de pago SUNAT
            string _valcr = "";                                                         // valor credito
            string _fechc = "";                                                         // fecha programada del pago credito
            if (tx_dat_mone.Text != MonDeft)
            {
                _morefD = tx_dat_monsunat.Text;                                      // moneda de refencia para el tipo de cambio
                _monobj = "PEN";        //tipoMoneda;                                // moneda objetivo del tipo de cambio
                _tipcam = tx_tipcam.Text;                                            // tipo de cambio con 3 decimales
                //_fechca = string.Format("{0:yyyy-MM-dd}", tx_fechope.Text);          // fecha del tipo de cambio
                _fechca = tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2);
                if (double.Parse(tx_flete.Text) > (double.Parse(Program.valdetra) / double.Parse(tx_tipcam.Text)) && tx_dat_tdv.Text == codfact)
                {
                    d_medpa = "001";                                    // medio de pago de la detraccion (001 = deposito en cuenta)
                    d_monde = "PEN";                                    // moneda de la detraccion SIEMPRE ES PEN moneda nacional
                    d_conpa = "CONTADO";                                // condicion de pago
                    d_porde = Program.pordetra;                         // porcentaje de detraccion
                    d_valde = Program.valdetra;                         // valor de la detraccion
                    d_codse = Program.coddetra;                         // codigo de servicio
                    d_ctade = Program.ctadetra;                         // cuenta detraccion BN
                    d_valre = "0";                                      // valor referencial
                    d_numre = "";                // numero registro mtc del camion
                    d_confv = "";                // config. vehicular del camion
                    d_ptori = "";                // Pto de origen
                    d_ptode = "";                // Pto de destino
                    d_vrepr = "0";               // valor referencial preliminar
                    codleyt = "1000";            // codigoLeyenda 1 - valor en letras
                    codleyd = "2006";
                    tipOper = "1001";
                    totdet = Math.Round(double.Parse(tx_fletMN.Text) * double.Parse(Program.pordetra) / 100,2);    // totalDetraccion
                }
            }
            /* ********************************************** GENERAMOS EL TXT    ************************************* */
            string sep = "|";    // char sep = (char)31;
            StreamWriter writer;
            file_path = file_path + ".txt";
            writer = new StreamWriter(file_path);
            writer.WriteLine("V|2.1|2.0||");
            writer.WriteLine("G" + sep +
                tipdo + sep +                   // Tipo de Comprobante Electrónico
                serie + sep +                   // Serie del Comprobante Electrónico
                corre + sep +                   // Numeración de Comprobante Electrónico
                _fecemi + sep +                 // Fecha de emisión
                _horemi + sep +                 // hora de emisión
                _moneda + sep +                 // Tipo de moneda
                _fvcmto + sep +                 // fecha de vencimiento del doc.venta
                Pcrupro + sep +                 // tipo de documento del emisor
                Prucpro + sep +                 // ruc emisor
                Prazsoc + sep +                 // razon social emisor
                Pnomcom + sep +                 // nombre comercial emisor
                Pdf_dir + sep +                 // Dirección detallada completa
                ubigEmi + sep +                 // ubigeo del emisor
                Pdf_dep + sep +                 // Departamento
                Pdf_pro + sep +                 // Provincia
                Pdf_urb + sep +                 // Urbanización
                Pdf_dis + sep +                 // Distrito
                paisEmi + sep +                 // pais del emisor
                codLocE + sep +                 // codigo sunat del local emisor
                corclie + sep +                 // Correo-Emisor
                Ptelef1 + sep +                 // telefono emisor
                Pweb1 + sep +                   // sitio web
                "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep +    // lugar de entrega/venta itinerante
                Ctipdoc + sep +                 // Tipo de documento del cliente
                Cnumdoc + sep +                 // Nro. Documento del cliente
                Cnomcli + sep +                 // Razón social del cliente
                dir1Adq + sep +                 // Dirección
                depaAdq + sep +                 // Departamento
                provAdq + sep +                 // Provincia
                "" + sep +                      // Urbanización   dir2Adq
                distAdq + sep +                 // Distrito
                paisAdq + sep +                 // Código país
                "" + sep +                      // codigo establecimiento adquiriente
                maiAdq + sep +                  // Correo-Receptor
                "" + sep +                      // sitio web del arquiriente/receptor
                "" + sep + "" + sep +           // datos del comprador
                totImp + sep +                  // Total IGV
                "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep +   // exportaciones, inafectas, exoneradas, gratuitas, etc
                _totven + sep +                 // Total operaciones gravadas
                totImp + sep +                  // total tributos grabados
                "" + sep + "" + sep + "" + sep + "" + sep + "" + sep + "" + sep +       // ivap, isc, otros tributos
                "" + sep + "" + sep +           // total descuentos, total otros cargos
                _totogr + sep +                 // Importe total de la venta
                _totven + sep +                 // total valor venta
                _totogr + sep +                 // total precio venta
                "" + sep +                      // redondeo del importe total
                "" + sep +                      // total anticipos
                tipOper + sep +                 // Tipo de Operación
                "" + sep +                      // orden de compra
                _morefD + sep +                 // TIPO DE CAMBIO - moneda a cambiar
                _monobj + sep +                 // TIPO DE CAMBIO - moneda destino cambiada, osea MN
                _tipcam + sep +                 // TIPO DE CAMBIO - tipo de cambio
                _fechca + sep +                 // TIPO DE CAMBIO - fecha del tipo de cambio
                d_codse + sep +                 // DETRACCION - codigo de servicio
                d_ctade + sep +                 // DETRACCION - cuenta detraccion BN
                d_medpa + sep +                 // DETRACCION - medio de pago
                totdet + sep +                  // DETRACCION - valor
                d_porde + sep +                 // DETRACCION - porcentaje
                d_monde + sep +                 // DETRACCION - moneda
                d_conpa + sep +                 // DETRACCION - condicion de pago
                "" + sep +                      // FERROVIARIO
                "" + sep +                      // FERROVIARIO
                "" + sep +                      // FERROVIARIO
                "" + sep +                      // FERROVIARIO
                "" + sep +                      // DOCUMENTOS MODIFICA
                "" + sep +                      // DOCUMENTOS MODIFICA
                "" + sep +                      // DOCUMENTOS MODIFICA
                "" + sep +                      // DOCUMENTOS MODIFICA
                "" + sep +                      // DOCUMENTOS MODIFICA
                "" + sep +                      // INCOTERMS
                "" + sep +                      // INCOTERMS
                "" + sep +                      // IMPUESTO ICBPER
                _forpa + sep +                  // INF.ADICIONAL FORMA DE PAGO
                _valcr + sep                    // INF.ADICIONAL FORMA DE PAGO
            );
            for (int s = 0; s < dataGridView1.Rows.Count - 1; s++)  // DETALLE
            {
                double _msigv = double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) / (1 + (double.Parse(v_igv) / 100));
                string Ipreuni = double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()).ToString("#0.00");     // Precio de venta unitario CON IGV
                if (tx_dat_mone.Text != MonDeft && dataGridView1.Rows[s].Cells["codmondoc"].Value.ToString() == MonDeft)   // 
                {
                    _msigv = Math.Round(_msigv / double.Parse(tx_tipcam.Text),2);
                    Ipreuni = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString())/ double.Parse(tx_tipcam.Text), 2).ToString("#0.00");
                }
                if (tx_dat_mone.Text == MonDeft && dataGridView1.Rows[s].Cells["codmondoc"].Value.ToString() != MonDeft)
                {
                    _msigv = Math.Round(_msigv * double.Parse(tx_tipcam.Text), 2);
                    Ipreuni = Math.Round(double.Parse(dataGridView1.Rows[s].Cells["valor"].Value.ToString()) * double.Parse(tx_tipcam.Text), 2).ToString("#0.00");
                }
                string Inumord = (s + 1).ToString();                                        // numero de orden del item             5
                string Iumeded = "ZZ";                                                      // Unidad de medida                     3
                string Icantid = "1.00";                                                    // Cantidad de items   n(12,3)         16
                string Icodprd = "-";                                                       // codigo del producto del cliente
                string Icodpro = "";                                                        // codigo del producto SUNAT                          30
                string Icodgs1 = "";                                                        // codigo del producto GS1
                string Icogtin = "";                                                        // tipo de producto GTIN
                string Inplaca = "";                                                        // numero placa de vehiculo
                string Idescri = glosser + " " + dataGridView1.Rows[s].Cells["Descrip"].Value.ToString();   // Descripcion
                string Ivaluni = _msigv.ToString("#0.00");                                  // Valor unitario del item SIN IMPUESTO 
                string Ivalref = "";                                                        // valor referencial del item cuando la venta es gratuita
                string Iigvite = Math.Round(double.Parse(Ipreuni) - double.Parse(Ivaluni),2).ToString("#0.00");     // monto IGV del item
                string Imonbas = Ivaluni;                                                   // monto base (valor sin igv * cantidad)
                string Isumigv = Iigvite;                                                   // Sumatoria de igv
                string Itasigv = Math.Round(double.Parse(v_igv), 2).ToString("#0.00");      // tasa del igv
                string Icatigv = "10";                                                      // Codigo afectacion al igv                    2
                string Iindgra = "";                                                        // indicador de gratuito
                string Iiscmba = "";                                                        // ISC monto base
                string Iiscmon = "";                                                        // ISC monto del tributo
                string Iisctas = "";                                                        // ISC tasa del tributo
                string Iisctip = "";                                                        // ISC tipo de sistema
                string Iotrtri = "";                                                        // otros tributos monto base
                string Iotrlin = "";                                                        // otros tributos monto unitario
                string Iotrtas = "";                                                        // otros tributos tasa del tributo
                string Iotrsis = "";                                                        // otros tributos tipo de sistema
                string Ivalvta = Ivaluni;                                                   // Valor de venta del ítem
                //
                writer.WriteLine("I" + sep +
                    Inumord + sep +     // orden
                    Iumeded + sep +     // unidad de medida ...... servicio ZZ
                    Icantid + sep +     // cantidad 1 servicio de transporte
                    Icodprd + sep +     // codigo del producto o servicio
                    Icodpro + sep +     // codigo del producto sunat
                    Icodgs1 + sep +     // codigo de producto GS1
                    Icogtin + sep +     // tipo de producto GTIN
                    Inplaca + sep +     // numero placa de vehiculo
                    Idescri + sep +     // descripcion del servicio
                    Ivaluni + sep +     // Valor unitario por ítem - SIN IGV
                    Ipreuni + sep +     // Precio de venta unitario por ítem - CON IGV
                    Ivalref + sep +     // valor referencial del item cuando la venta es gratuita
                    Iigvite + sep +     // Monto IGV
                    Imonbas + sep +     // monto base (valor sin igv * cantidad)
                    Isumigv + sep +     // monto igv (valor igv * cantidad)
                    Itasigv + sep +     // tasa del igv
                    Icatigv + sep +     // Codigo afectacion al igv
                    Iindgra + sep +     // indicador de gratuidad
                    Iiscmba + sep +     // ISC monto base
                    Iiscmon + sep +     // ISC monto del tributo
                    Iisctas + sep +     // ISC tasa del tributo
                    Iisctip + sep +     // ISC tipo de sistema
                    Iotrtri + sep +     // otros tributos monto base
                    Iotrlin + sep +     // otros tributos monto unitario
                    Iotrtas + sep +     // otros tributos tasa del tributo
                    Iotrsis + sep +     // otros tributos tipo de sistema
                    Ivalvta + sep +     // Valor de venta del ítem
                    "" + sep + "" + sep + "" + sep + "" + sep +         // CARGO, codigo, factor, etc.
                    "" + sep + "" + sep + "" + sep + "" + sep +         // DESCUENTO, codigo, factor, etc
                    "" + sep + "" + sep + "" + sep                      // BOLSAS DE PLASTICO
                );
            }
            for (int s = 0; s < dataGridView1.Rows.Count - 1; s++)
            {
                writer.WriteLine("T" + sep +
                    "31" + sep +
                    dataGridView1.Rows[s].Cells["guias"].Value.ToString() + sep +
                    dataGridView1.Rows[s].Cells["fechaGR"].Value.ToString() + sep
                );
            }
            writer.WriteLine("L" + sep +
                codleyt + sep +         // codigo leyenda monto en letras
                monLet + sep            // Leyenda: Monto expresado en Letras
            );
            if (_forpa == "Credito")
            {
                writer.WriteLine("F" + sep +
                "Cuota001" + sep +
                _valcr + sep +
                _fechc + sep);
            }
            if (codleyd != "")
            {
                writer.WriteLine("L" + sep +
                codleyd + sep +         // codigo leyenda monto en letras
                glosdet + sep);            // Leyenda: Monto expresado en Letras
            }
            for (int s = 0; s < dataGridView1.Rows.Count - 1; s++)
            {
                writer.WriteLine("E" + sep +
                codobs + sep +
                dataGridView1.Rows[s].Cells["guiasclte"].Value.ToString() + sep);
            }
            writer.Flush();
            writer.Close();
            retorna = true;
            return retorna;
        }
        #endregion

        #region autocompletados

        #endregion autocompletados

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
                    MessageBox.Show("La Factura no existe o esta anulada", "Error en Factura", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tx_numGR.Text = "";
                    tx_numGR.Focus();
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
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // valida pago y calcula
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
                        MessageBox.Show("El valor a facturar no puede ser diferente al valor de la(s) GR");
                        tx_flete.Focus();
                        return;
                    }
                }
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el documento?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            if (factElec("Horizont", "txt", "alta", 0) == true)       // facturacion electrónica
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
                                MessageBox.Show("No se puede generar el documento de venta electrónico", "Error en proveedor de Fact.Electrónica");
                                iserror = "si";
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se puede grabar el documento de venta electrónico","Error en conexión");
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
                    micon.Parameters.AddWithValue("td", tx_dat_tdv.Text);
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
                string inserta = "insert into cab???? (" +
                    "fechope,martdve,tipdvta,serdvta,numdvta,ticltgr,tidoclt,nudoclt,nombclt,direclt,dptoclt,provclt,distclt,ubigclt,corrclt,teleclt," +
                    "locorig,dirorig,ubiorig,obsdvta,canfidt,canbudt,mondvta,tcadvta,subtota,igvtota,porcigv,totdvta,totpags,saldvta,estdvta,frase01," +
                    "tipoclt,m1clien,tippago,ferecep,impreso,codMN,subtMN,igvtMN,totdvMN,pagauto,tipdcob,idcaja,plazocred," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                    "@fechop,@mtdvta,@ctdvta,@serdv,@numdv,@tcdvta,@tdcrem,@ndcrem,@nomrem,@dircre,@dptocl,@provcl,@distcl,@ubicre,@mailcl,@telecl," +
                    "@ldcpgr,@didegr,@ubdegr,@obsprg,@canfil,@totcpr,@monppr,@tcoper,@subpgr,@igvpgr,@porcigv,@totpgr,@pagpgr,@salxpa,@estpgr,@frase1," +
                    "@ticlre,@m1clte,@tipacc,@feredv,@impSN,@codMN,@subMN,@igvMN,@totMN,@pagaut,@tipdco,@idcaj,@plazc," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@mtdvta", cmb_tnota.Text.Substring(0,1));
                    micon.Parameters.AddWithValue("@ctdvta", tx_dat_tdv.Text);
                    micon.Parameters.AddWithValue("@serdv", tx_serie.Text);
                    micon.Parameters.AddWithValue("@numdv", tx_numero.Text);
                    micon.Parameters.AddWithValue("@tdcrem", tx_dat_tdRem.Text);
                    micon.Parameters.AddWithValue("@ndcrem", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@nomrem", tx_nomRem.Text);
                    micon.Parameters.AddWithValue("@dircre", tx_dirRem.Text);
                    micon.Parameters.AddWithValue("@dptocl", tx_dptoRtt.Text);
                    micon.Parameters.AddWithValue("@provcl", tx_provRtt.Text);
                    micon.Parameters.AddWithValue("@distcl", tx_distRtt.Text);
                    micon.Parameters.AddWithValue("@mailcl", tx_email.Text);
                    micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                    micon.Parameters.AddWithValue("@didegr", dirloc);                             // direccion origen
                    micon.Parameters.AddWithValue("@ubdegr", ubiloc);                             // ubigeo origen
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@canfil", tx_tfil.Text);     // cantidad de filas de detalle
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@tcoper", tx_tipcam.Text);                   // TIPO DE CAMBIO
                    micon.Parameters.AddWithValue("@subpgr", tx_subt.Text);                     // sub total
                    micon.Parameters.AddWithValue("@igvpgr", tx_igv.Text);                      // igv
                    micon.Parameters.AddWithValue("@porcigv", v_igv);                           // porcentaje en numeros de IGV
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);                    // total inc. igv
                    micon.Parameters.AddWithValue("@pagpgr", (tx_pagado.Text == "") ? "0" : tx_pagado.Text);
                    micon.Parameters.AddWithValue("@salxpa", (tx_salxcob.Text == "") ? "0" : tx_salxcob.Text);
                    micon.Parameters.AddWithValue("@estpgr", (tx_pagado.Text == "" || tx_pagado.Text == "0.00") ? tx_dat_estad.Text : codCanc); // estado
                    micon.Parameters.AddWithValue("@frase1", "");                   // no hay nada que poner 19/11/2020
                    micon.Parameters.AddWithValue("@tipacc", v_mpag);                   // pago del documento x defecto si nace la fact pagada
                    micon.Parameters.AddWithValue("@feredv", DBNull.Value);         // si es pago contado la fecha de recep del doc. es la misma fecha
                    micon.Parameters.AddWithValue("@impSN", "N");
                    micon.Parameters.AddWithValue("@codMN", MonDeft);               // codigo moneda local
                    micon.Parameters.AddWithValue("@subMN", subtMN);
                    micon.Parameters.AddWithValue("@igvMN", igvtMN);
                    micon.Parameters.AddWithValue("@totMN", fletMN);
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
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                        {

                            string inserd2 = "update det???? set " +
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
                        string actua = "update cab??? a set obsdvta=@obsprg," +
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
            /*
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
                        if (Math.Round(decimal.Parse(tx_tfmn.Text),1) != Math.Round(decimal.Parse(tx_fletMN.Text),1))
                        {
                            MessageBox.Show("No coinciden los valores!","Error en calculo",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            tx_flete.Text = "";
                            tx_flete.Focus();
                            return;
                        }
                    }
                }
                DataRow[] row = dtm.Select("idcodice='" + tx_dat_mone.Text + "'");
                NumLetra numLetra = new NumLetra();
                tx_fletLetras.Text = numLetra.Convertir(tx_flete.Text,true) + row[0][3].ToString().Trim();
                button1.Focus();
            }
            */
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
            tx_serGR.Focus();
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
        private void cmb_mon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")    //  || Tx_modo.Text == "EDITAR"
            {   // lo de totcant es para accionar solo cuando el detalle de la GR se haya cargado
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    DataRow[] row = dtm.Select("idcodice='"+ tx_dat_mone.Text+"'");
                    tx_dat_monsunat.Text = row[0][2].ToString();
                    tipcambio(tx_dat_mone.Text);
                    if (tx_flete.Text != "" && tx_flete.Text != "0.00") calculos(decimal.Parse(tx_flete.Text));
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
                    tx_dat_tdec.Text = row[0].ItemArray[2].ToString();
                    //glosser = row[0].ItemArray[4].ToString();
                    tx_serGR.Text = "";
                    tx_numGR.Text = "";
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

        }
        private void imprime_TK(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // se imprime desde portal del ose
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
