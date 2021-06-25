using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class cobranzas : Form
    {
        static string nomform = "cobranzas";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabcobran";              // cabecera de guias INDIVIDUALES

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
        string codAnul = "";            // codigo de documento anulado
        string codGene = "";            // codigo documento nuevo generado
        string MonDeft = "";            // moneda por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
//        string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
//        string v_cid = "";              // codigo interno de tipo de documento
        string v_codc = "";             // codigo tipo documento cobranza
        string v_noco = "";             // sigla del codigo cobranza
        string v_CR_gr_ind = "";        // nombre del formato FT/BV en CR
 //       string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string vint_A0 = "";            // variable codigo anulacion interna por BD
//        string v_codidv = "";           // variable codifo interno de documento de venta en vista TDV
        string v_igv = "";              // valor igv %
        string v_tip1 = "";             // cobranza desde pre guia
        string v_tip2 = "";             // cobranza desde guia transportista
        string v_tip3 = "";             // cobranza desde BOLETA
        string v_tip4 = "";             // cobranza desde FACTURA
        string v_estcaj = "";           // estado de la caja del local
        string codAbie = "";            // codigo caja abierta
        string codCier = "";            // codigo caja cerrada
        string v_idcaj = "";            // id de la caja actual
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        string dirloc = TransCarga.Program.vg_duse; // direccion completa del local usuario conectado
        string ubiloc = TransCarga.Program.vg_uuse; // ubigeo local del usuario conectado
        #endregion

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        DataTable dtm = new DataTable();
        DataTable dtmpa = new DataTable();
        public cobranzas()
        {
            InitializeComponent();
        }
        private void cobranzas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void cobranzas_Load(object sender, EventArgs e)
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
            tx_nomuser.Text = TransCarga.Program.vg_nuse;
            tx_locuser.Text = tx_locuser.Text + " " + TransCarga.Program.vg_nlus; // TransCarga.Program.vg_luse;
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
            // longitudes maximas de campos
            tx_serie.MaxLength = 4;         // serie doc vta
            tx_numero.MaxLength = 8;        // numero doc vta
            tx_serGR.MaxLength = 4;         // serie guia
            tx_numGR.MaxLength = 8;         // numero guia
            tx_detpago.MaxLength = 90;      // detalle del pago, num operacion, banco, etc.
            tx_cajero.MaxLength = 90;       // nombre del trabajador que recibe el dinero
            tx_obser1.MaxLength = 245;      // observaciones
            tx_fb.MaxLength = 1;            // F ó B
            tx_fb.CharacterCasing = CharacterCasing.Upper;
            tx_fb.Visible = false;
            // grilla
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // todo desabilidado
            sololee();
            tx_dat_tdv.Text = v_codc;
            tx_noco.Text = v_noco;
            tx_serGR.CharacterCasing = CharacterCasing.Upper;
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            dataGridView1.Rows.Clear();
            dataGridView1.ReadOnly = true;
            tx_flete.Text = "";
            tx_igv.Text = "";
            tx_subt.Text = "";
            tx_pagado.Text = "";
            tx_salxcob.Text = "";
            tx_numero.Text = "";
            tx_serie.Text = v_slu;
            tx_numero.ReadOnly = true;
            // ser_gr, num_gry resto se limpian solos
            //tx_dat_mone.Text = MonDeft;
            //cmb_mon.SelectedValue = tx_dat_mone.Text;
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_digit.Text = v_nbu;
            tx_dat_estad.Text = codGene;
            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
            tx_idcaja.ReadOnly = true;
            if (Tx_modo.Text == "NUEVO")
            {
                tx_cajero.Text = tx_nomuser.Text;
                tx_idcaja.Text = v_idcaj;
                tx_dat_mp.Text = dtmpa.Rows[0].ItemArray[0].ToString();
                cmb_mpago.SelectedIndex = 0;    // primer registro del medio de pago
            }
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
                micon.Parameters.AddWithValue("@nofi", "ayccaja");
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
                    if (row["formulario"].ToString() == "ayccaja" && row["campo"].ToString() == "estado")
                    {
                        if (row["param"].ToString() == "abierto") codAbie = row["valor"].ToString().Trim();             // codigo caja abierta
                        if (row["param"].ToString() == "cerrado") codCier = row["valor"].ToString().Trim();             // codigo caja cerrada
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "codigo") v_codc = row["valor"].ToString().Trim();               // codigo tipo documento
                            if (row["param"].ToString() == "nomcod") v_noco = row["valor"].ToString().Trim();               // nombre codido cobranza
                        }
                        if (row["campo"].ToString() == "cobranza")
                        {
                            if (row["param"].ToString() == "tipo1") v_tip1 = row["valor"].ToString().Trim();               // cobranza desde pre guia
                            if (row["param"].ToString() == "tipo2") v_tip2 = row["valor"].ToString().Trim();               // cobranza desde guia transp.
                            if (row["param"].ToString() == "tipo3") v_tip3 = row["valor"].ToString().Trim();               // cobranza desde BOLETA
                            if (row["param"].ToString() == "tipo4") v_tip4 = row["valor"].ToString().Trim();               // cobranza desde FACTURA
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "nomfor_cr") v_CR_gr_ind = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
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
                    parte = "where a.sercobc=@ser AND a.numcobc=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "SELECT a.id,a.fechope,a.tipdcob,a.sercobc,a.numcobc,a.loccobc,a.estdcob,a.tidoorc,a.martdve,a.tipdoco,a.serdoco,a.numdoco," +
                        "a.timepag,a.refpagc,a.cobrdor,a.obscobc,a.mondoco,a.totdoco,a.totpags,a.saldvta,a.subdoco,a.igvdoco,a.codmopa,a.totpago,a.tcadvta," +
                        "a.porcigv,a.totpaMN,a.codmoMN,a.impreso,a.cltdoco,a.dcltdoco,d.Descrizione AS nctmG,a.userc,u.nombre,e.descrizionerid as nomest," +
                        "b.descrizionerid AS ntdc,c.razonsocial,concat(c.Direcc1, ' ', c.Direcc2) AS direc,c.depart,c.Provincia,c.Localidad,a.idcaja " +
                        "FROM cabcobran a " +
                        "LEFT JOIN desc_doc b ON b.idcodice = a.cltdoco " +
                        "LEFT JOIN anag_cli c ON c.tipdoc = a.cltdoco AND c.RUC = a.dcltdoco " +
                        "LEFT JOIN desc_mon d ON d.idcodice = a.codmopa " +
                        "left join usuarios u on u.nom_user = a.userc " +
                        "left join desc_est e on e.idcodice = a.estdcob " +
                        parte;
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
                            tx_fechope.Text = dr.GetString("fechope").Substring(0, 10);
                            tx_dat_tdv.Text = dr.GetString("tipdcob");
                            tx_serie.Text = dr.GetString("sercobc");
                            tx_numero.Text = dr.GetString("numcobc");
                            tx_dat_estad.Text = dr.GetString("estdcob");
                            tx_estado.Text = dr.GetString("nomest");
                            rb_PG.Checked = (dr.GetString("tidoorc") == "1") ? true : false;
                            rb_GR.Checked = (dr.GetString("tidoorc") == "2") ? true : false;
                            rb_DV.Checked = (dr.GetString("tidoorc") == "3") ? true : false;
                            tx_fb.Text = dr.GetString("martdve");
                            tx_dat_tidoor.Text = dr.GetString("tipdoco");
                            tx_serGR.Text = dr.GetString("serdoco");
                            tx_numGR.Text = dr.GetString("numdoco");
                            tx_dat_mp.Text = dr.GetString("timepag");
                            tx_detpago.Text = dr.GetString("refpagc");
                            tx_cajero.Text = dr.GetString("cobrdor");
                            tx_obser1.Text = dr.GetString("obscobc");
                            tx_dat_mod.Text = dr.GetString("mondoco");
                            tx_flete.Text = string.Format("{0:0.00}", dr.GetDecimal("totdoco"));
                            tx_pagado.Text = string.Format("{0:0.00}", dr.GetDecimal("totpags"));
                            tx_salxcob.Text = string.Format("{0:0.00}", dr.GetDecimal("saldvta"));
                            tx_subt.Text = string.Format("{0:0.00}", dr.GetDecimal("subdoco"));
                            tx_igv.Text = string.Format("{0:0.00}" ,dr.GetDecimal("igvdoco"));
                            tx_dat_mone.Text = dr.GetString("codmopa");
                            tx_PAGO.Text = string.Format("{0:0.00}", dr.GetDecimal("totpago"));
                            tx_tipcam.Text = dr.GetString("tcadvta");
                            tx_fletMN.Text = string.Format("{0:0.00}", dr.GetDecimal("totpaMN"));
                            tx_impreso.Text = dr.GetString("impreso");
                            lb_moneda.Text = dr.GetString("nctmG");
                            tx_digit.Text = dr.GetString("nombre");
                            tx_dat_userdoc.Text = dr.GetString("userc");
                            // 
                            tx_dat_tdRem.Text = dr.GetString("ntdc");
                            tx_numDocRem.Text = dr.GetString("dcltdoco");
                            tx_nomRem.Text = dr.GetString("razonsocial");
                            tx_dirRem.Text = dr.GetString("direc");
                            tx_dptoRtt.Text = dr.GetString("depart");
                            tx_provRtt.Text = dr.GetString("Provincia");
                            tx_distRtt.Text = dr.GetString("Localidad");
                            //
                            cmb_mpago.SelectedValue = tx_dat_mp.Text;
                            cmb_mon.SelectedValue = tx_dat_mone.Text;
                            //
                            if (rb_GR.Checked == true) jaladet("guia", tx_serGR.Text, tx_numGR.Text);
                            if (rb_DV.Checked == true) jaladet("docvta", tx_serGR.Text, tx_numGR.Text);
                            // 
                            if (v_idcaj != dr.GetString("idcaja"))
                            {
                                tx_numero.Text = "";
                                MessageBox.Show("La Caja del documento esta cerrada!","No puede continuar",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                initIngreso();
                            }
                            else
                            {
                                if (v_estcaj != codAbie)
                                {
                                    tx_numero.Text = "";
                                    MessageBox.Show("La Caja esta cerrada!", "No puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    initIngreso();
                                }
                                else
                                {
                                    tx_idcaja.Text = dr.GetString("idcaja");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No existe el código de cobranza!", "Atención - dato incorrecto",
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
                // datos para el combo de medio de pago
                cmb_mpago.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mpa where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtmpa.Clear();
                        dacu.Fill(dtmpa);
                        cmb_mpago.DataSource = dtmpa;
                        cmb_mpago.DisplayMember = "descrizionerid";
                        cmb_mpago.ValueMember = "idcodice";
                    }
                }
                // jalamos la caja
                using (MySqlCommand micon = new MySqlCommand("select id,fechope,statusc from cabccaja where loccaja=@luc order by id desc limit 1" , conn))
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
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de Cobranza ANULADA");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de Cobranza GENERADA/NUEVA");
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
                lib.messagebox("formato de impresion de la cobranza");
                retorna = false;
            }
            if (vi_copias == "")        // cant copias impresion
            {
                lib.messagebox("# copias impresas de la cobranza");
                retorna = false;
            }
            if (v_impTK == "")           // nombre de la ticketera
            {
                lib.messagebox("Nombre de impresora de Tickets");
                retorna = false;
            }
            if (v_CR_gr_ind == "")
            {
                lib.messagebox("Nombre formato Cobranzas en CR");
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
                // validamos que el DOC: 1.exista, 2.No este cobrada, 3.No este anulada
                // si es fact o boleta de varias Guias debe validarse guía x guia
                string hay = "no";
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    lib.procConn(conn);
                    string cons = "???";
                    if (rb_PG.Checked == true)          // Pre guia
                    {
                        cons = "SELECT a.fecpregui,a.serpregui,a.numpregui,a.codmonpre,a.totpregui,a.tidodepre,a.nudodepre,a.tidorepre,a.nudorepre,a.estadoser,d.Descrizione AS nctmG," +
                            "a.fecguitra,a.serguitra,a.numguitra,a.codmongui,a.totguitra," +
                            "a.fecdocvta,a.tipdocvta,a.serdocvta,a.numdocvta,a.codmonvta,a.totdocvta," +
                            "a.codmonpag,a.totpagado,a.saldofina,a.feculpago,a.estadoser," +
                            "ifnull(b.descrizionerid,'') AS ntdcD,c.razonsocial as razsocialD,concat(c.Direcc1, ' ', c.Direcc2) AS direcD, c.depart as depD,c.Provincia as proD,c.Localidad as disD," +
                            "ifnull(b1.descrizionerid,'') AS ntdcR,c1.razonsocial as razsocialR,CONCAT(c1.Direcc1,' ',c1.Direcc2) AS direcR,c1.depart as depR,c1.Provincia as proR,c1.Localidad as disR " +
                            "from controlg a " +
                            "LEFT JOIN desc_doc b ON b.idcodice = a.tidodepre " +
                            "LEFT JOIN anag_cli c ON c.tipdoc = a.tidodepre AND c.RUC = a.nudodepre " +
                            "LEFT JOIN desc_doc b1 ON b1.idcodice = a.tidorepre " +
                            "LEFT JOIN anag_cli c1 ON c1.tipdoc = a.tidorepre AND c1.RUC = a.nudorepre " +
                            "LEFT JOIN desc_mon d ON d.idcodice = a.codmonpre " +
                            "WHERE a.serpregui = @ser AND a.numpregui = @num";
                        using (MySqlCommand micon = new MySqlCommand(cons, conn))
                        {
                            micon.Parameters.AddWithValue("@ser", serie);
                            micon.Parameters.AddWithValue("@num", corre);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.HasRows)
                                {
                                    if (dr.Read())
                                    {
                                        if (string.IsNullOrEmpty(dr.GetString("serpregui")) || dr.GetString("estadoser") == codAnul || dr.GetDecimal("saldofina") <= 0)
                                        {
                                            hay = "no";
                                        }
                                        else
                                        {
                                            if (dr.GetString("serguitra").Trim() != "")
                                            {
                                                MessageBox.Show("La Pre-Guía tiene una Guía de Remisión asociada" + Environment.NewLine +
                                                    "debe pagar desde la guía " + dr.GetString("serguitra") + "-" + dr.GetString("numguitra"), "Atención", MessageBoxButtons.OK,MessageBoxIcon.Hand);
                                                hay = "no";
                                            }
                                            else
                                            {
                                                hay = "si";
                                                if (dr.GetString("ntdcD") != "")
                                                {
                                                    tx_dat_tdRem.Text = dr.GetString("ntdcD");
                                                    tx_numDocRem.Text = dr.GetString("nudodepre");
                                                    tx_nomRem.Text = dr.GetString("razsocialD");
                                                    tx_dirRem.Text = dr.GetString("direcD");
                                                    tx_dptoRtt.Text = dr.GetString("depD");
                                                    tx_provRtt.Text = dr.GetString("proD");
                                                    tx_distRtt.Text = dr.GetString("disD");
                                                }
                                                else
                                                {
                                                    tx_dat_tdRem.Text = dr.GetString("ntdcR");
                                                    tx_numDocRem.Text = dr.GetString("nudorepre");
                                                    tx_nomRem.Text = dr.GetString("razsocialR");
                                                    tx_dirRem.Text = dr.GetString("direcR");
                                                    tx_dptoRtt.Text = dr.GetString("depR");
                                                    tx_provRtt.Text = dr.GetString("proR");
                                                    tx_distRtt.Text = dr.GetString("disR");
                                                }
                                                tx_flete.Text = string.Format("{0:0.00}", dr.GetDecimal("totpregui"));
                                                tx_salxcob.Text = string.Format("{0:0.00}", dr.GetDecimal("saldofina"));
                                                tx_pagado.Text = string.Format("{0:0.00}", dr.GetDecimal("totpagado"));
                                                tx_dat_mod.Text = dr.GetString("codmonpre");
                                                lb_moneda.Text = dr.GetString("nctmG");
                                                tx_dat_mone.Text = dr.GetString("codmonpre");
                                                cmb_mon.SelectedValue = tx_dat_mone.Text;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    hay = "no"; // no existe el doc
                                }
                            }
                        }
                        if (hay == "si")
                        {
                            dataGridView1.Rows.Clear();
                            string consdet = "SELECT cantprodi,unimedpro,codiprodi,descprodi,pesoprodi,precprodi,totaprodi " +
                                    "FROM detpregr WHERE serpregui = @ser AND numpregui = @num";
                            using (MySqlCommand midet = new MySqlCommand(consdet, conn))
                            {
                                midet.Parameters.AddWithValue("@ser", serie);
                                midet.Parameters.AddWithValue("@num", corre);
                                using (MySqlDataReader drD = midet.ExecuteReader())
                                {
                                    while (drD.Read())
                                    {
                                        dataGridView1.Rows.Add(
                                            drD.GetString("unimedpro"),
                                            drD.GetString("descprodi"),
                                            drD.GetString("cantprodi"),
                                            drD.GetString("pesoprodi"),
                                            drD.GetString("totaprodi")
                                            );
                                    }
                                }
                            }
                            calculos(decimal.Parse(tx_flete.Text));    // calculamos el subtotal e IGV
                            retorna = true;
                        }
                    }
                    if (rb_GR.Checked == true)          // guia
                    {
                        cons = "SELECT a.fecpregui,a.serpregui,a.numpregui,a.codmonpre,a.totpregui,a.fecguitra,a.serguitra,a.numguitra,a.tidodegui,a.nudodegui,a.codmongui,a.totguitra," +
                            "a.fecdocvta,a.tipdocvta,a.serdocvta,a.numdocvta,a.codmonvta,a.totdocvta,a.codmonpag,a.totpagado,a.saldofina,a.feculpago,a.estadoser," +
                            "b.descrizionerid AS ntdc,c.razonsocial,concat(c.Direcc1, ' ', c.Direcc2) AS direc,c.depart,c.Provincia,c.Localidad,d.Descrizione AS nctmG,ifnull(e.descrizione,'') as nctmV " +
                            "from controlg a " +
                            "LEFT JOIN desc_doc b ON b.idcodice = a.tidodegui " +
                            "LEFT JOIN anag_cli c ON c.tipdoc = a.tidodegui AND c.RUC = a.nudodegui " +
                            "LEFT JOIN desc_mon d ON d.idcodice = a.codmongui " +
                            "left join desc_mon e on e.idcodice = a.codmonvta " +
                            "WHERE a.serguitra = @ser AND a.numguitra = @num";
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
                                        if (dr.GetString("numguitra").Trim() == "")
                                        {
                                            hay = "nohay";
                                        }
                                        else
                                        {
                                            //if (dr.GetString("tipdocvta").Trim() != "")    // YA NO 16/04/2021 .. SI SE COBRA DESDE GR DEBE RESTARSE SALDO DE LA FT
                                            //{
                                                // MessageBox.Show("La Guía tiene documento de venta" + Environment.NewLine +
                                                //    "Debe cobrar desde " + dr.GetString("tipdocvta") + dr.GetString("serdocvta") + "-" + dr.GetString("numdocvta"), "Atención", 
                                                //    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                // hay = "nohay";
                                            //}
                                            //else
                                            {
                                                if (dr.GetString("estadoser") == codAnul || dr.GetDouble("saldofina") <= 0)
                                                {
                                                    MessageBox.Show("La Guía esta anulada o ya esta pagada", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                    hay = "nohay";
                                                }
                                                else
                                                {
                                                    tx_dat_clte.Text = dr.GetString("tidodegui");
                                                    tx_dat_tdRem.Text = dr.GetString("ntdc");
                                                    tx_numDocRem.Text = dr.GetString("nudodegui");
                                                    tx_nomRem.Text = dr.GetString("razonsocial");
                                                    tx_dirRem.Text = dr.GetString("direc");
                                                    tx_dptoRtt.Text = dr.GetString("depart");
                                                    tx_provRtt.Text = dr.GetString("Provincia");
                                                    tx_distRtt.Text = dr.GetString("Localidad");
                                                    tx_flete.Text = string.Format("{0:0.00}", dr.GetDecimal("totguitra"));
                                                    tx_salxcob.Text = string.Format("{0:0.00}", dr.GetDecimal("saldofina"));
                                                    tx_pagado.Text = string.Format("{0:0.00}", dr.GetDecimal("totpagado"));
                                                    // a.codmonpag
                                                    if (!string.IsNullOrEmpty(dr.GetString("codmonvta").Trim()))    //  != ""
                                                    {
                                                        tx_dat_mod.Text = dr.GetString("codmonvta");
                                                        lb_moneda.Text = dr.GetString("nctmV");
                                                    }
                                                    else
                                                    {
                                                        tx_dat_mod.Text = dr.GetString("codmongui");
                                                        lb_moneda.Text = dr.GetString("nctmG");
                                                    }
                                                    tx_dat_mone.Text = tx_dat_mod.Text;
                                                    cmb_mon.SelectedValue = tx_dat_mod.Text;
                                                    hay = "si";
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    hay = "no"; // no existe la guía
                                }
                            }
                            if (hay == "si")
                            {
                                jaladet("guia",serie,corre);
                                calculos(decimal.Parse(tx_flete.Text));    // calculamos el subtotal e IGV
                                retorna = true;
                            }
                        }
                    }
                    if (rb_DV.Checked == true)          // doc. venta
                    {
                        cons = "SELECT a.fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt," +
                            "a.mondvta,a.subtota,a.igvtota,a.totdvta,a.totpags,a.saldvta,a.estdvta," +
                            "b.descrizionerid AS ntdc,ifnull(e.descrizione, '') as nctmV " +
                            "FROM cabfactu a " +
                            "LEFT JOIN desc_doc b ON b.idcodice = a.tidoclt " +
                            "left join desc_mon e on e.idcodice = a.mondvta " +
                            "WHERE a.martdve = @tip and a.serdvta = @ser and a.numdvta = @num";
                        // "left join controlg g on g.tipdocvta=a.tipdvta and g.serdocvta=a.serdvta and g.numdocvta=a.numdvta " +
                        // "g.fecguitra,g.serguitra,g.numguitra,g.codmongui,g.totguitra " +
                        using (MySqlCommand micon = new MySqlCommand(cons, conn))
                        {
                            micon.Parameters.AddWithValue("@tip", tx_fb.Text);
                            micon.Parameters.AddWithValue("@ser", serie);
                            micon.Parameters.AddWithValue("@num", corre);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.HasRows)
                                {
                                    if (dr.Read())
                                    {
                                        if (string.IsNullOrEmpty(dr.GetString("estdvta")) || dr.GetString("estdvta") == codAnul || dr.GetDecimal("saldvta") <= 0)
                                        {
                                            hay = "no";
                                        }
                                        else
                                        {
                                            hay = "si";
                                            tx_dat_clte.Text = dr.GetString("tidoclt");
                                            tx_dat_tdRem.Text = dr.GetString("ntdc");
                                            tx_numDocRem.Text = dr.GetString("nudoclt");
                                            tx_nomRem.Text = dr.GetString("nombclt");
                                            tx_dirRem.Text = dr.GetString("direclt");
                                            tx_dptoRtt.Text = dr.GetString("dptoclt");
                                            tx_provRtt.Text = dr.GetString("provclt");
                                            tx_distRtt.Text = dr.GetString("distclt");
                                            tx_flete.Text = string.Format("{0:0.00}", dr.GetDecimal("totdvta"));
                                            tx_salxcob.Text = string.Format("{0:0.00}", dr.GetDecimal("saldvta"));
                                            tx_pagado.Text = string.Format("{0:0.00}", dr.GetDecimal("totpags"));
                                            tx_dat_mod.Text = dr.GetString("mondvta");
                                            lb_moneda.Text = dr.GetString("nctmV");
                                            cmb_mon.SelectedValue = tx_dat_mod.Text;
                                        }
                                    }
                                }
                                else
                                {
                                    hay = "no"; // no existe el doc
                                }
                            }
                        }
                        if (hay == "si")
                        {
                            jaladet("docvta", serie,corre);   
                            calculos(decimal.Parse(tx_flete.Text));    // calculamos el subtotal e IGV
                            retorna = true;
                        }
                    }
                }
            }
            return retorna;
        }
        private void tipcambio(string codmod)                // funcion para calculos con el tipo de cambio
        {
            if (codmod != MonDeft)   // codmod != ""
            {
                vtipcam vtipcam = new vtipcam(tx_PAGO.Text, codmod, DateTime.Now.Date.ToString());
                var result = vtipcam.ShowDialog();
                if (vtipcam.ReturnValue3 != null)
                {
                    tx_PAGO.Text = vtipcam.ReturnValue1;
                    tx_fletMN.Text = vtipcam.ReturnValue2;
                    tx_tipcam.Text = vtipcam.ReturnValue3;
                    if (tx_fletMN.Text.Trim() == "0.00" && (tx_PAGO.Text.Trim() != "" || tx_PAGO.Text.Trim() != "0"))
                    {
                        tx_fletMN.Text = Math.Round(decimal.Parse(tx_PAGO.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                }
                else
                {
                    cmb_mon.SelectedValue = MonDeft;
                }
            }
            //}
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
        private void jaladet(string tipo,string ser,string cor)
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                dataGridView1.Rows.Clear();
                lib.procConn(conn);
                if (tipo == "guia")
                {
                    string consdet = "SELECT a.fila,a.sergui,a.numgui,a.cantprodi,a.unimedpro,a.codiprodi,a.descprodi,a.pesoprodi,a.precprodi,a.totaprodi," +
                        "b.saldofina,c.descrizionerid as mon,b.totguitra " +
                        "FROM detguiai a left join controlg b on b.serguitra=a.sergui and b.numguitra=a.numgui left join desc_mon c on c.idcodice=b.codmongui " +
                        "WHERE a.sergui = @ser AND a.numgui = @num";
                    using (MySqlCommand midet = new MySqlCommand(consdet, conn))
                    {
                        midet.Parameters.AddWithValue("@ser", ser);
                        midet.Parameters.AddWithValue("@num", cor);
                        using (MySqlDataReader drD = midet.ExecuteReader())
                        {
                            while (drD.Read())
                            {
                                dataGridView1.Rows.Add(
                                    ser + "-" + cor,
                                    drD.GetString("descprodi"),
                                    drD.GetString("cantprodi"),
                                    drD.GetString("mon"),
                                    drD.GetString("totguitra"),
                                    "",
                                    "",
                                    drD.GetString("saldofina")
                                    );
                                //                                     drD.GetString("pesoprodi"),
                            }
                        }
                    }
                }
                if (tipo == "docvta")
                {
                    string consdet = "SELECT a.codgror,a.cantbul,a.unimedp,a.descpro,a.pesogro,a.codmogr,a.totalgr,b.saldofina " +
                                    "FROM detfactu a left join controlg b on concat(b.serguitra,'-',b.numguitra)=a.codgror " +
                                    "WHERE a.martdve = @tip and a.serdvta = @ser and a.numdvta = @num";
                    using (MySqlCommand midet = new MySqlCommand(consdet, conn))
                    {
                        midet.Parameters.AddWithValue("@tip", tx_fb.Text);
                        midet.Parameters.AddWithValue("@ser", ser);
                        midet.Parameters.AddWithValue("@num", cor);
                        using (MySqlDataReader drD = midet.ExecuteReader())
                        {
                            while (drD.Read())
                            {
                                dataGridView1.Rows.Add(
                                    drD.GetString("codgror"),
                                    drD.GetString("descpro"),
                                    drD.GetString("cantbul"),
                                    drD.GetString("pesogro"),
                                    drD.GetString("totalgr"),
                                    "",
                                    "",
                                    drD.GetString("saldofina")
                                    );
                            }
                        }
                    }
                }
            }
        }

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
        }
        private void escribe()
        {
            lp.escribe(this);
            tx_nomRem.ReadOnly = true;
            tx_dat_tdRem.ReadOnly = true;
            tx_numDocRem.ReadOnly = true;
            tx_nomRem.ReadOnly = true;
            tx_dirRem.ReadOnly = true;
            tx_dptoRtt.ReadOnly = true;
            tx_provRtt.ReadOnly = true;
            tx_distRtt.ReadOnly = true;
            //
            tx_flete.ReadOnly = true;
            tx_igv.ReadOnly = true;
            tx_subt.ReadOnly = true;
            tx_salxcob.ReadOnly = true;
            tx_pagado.ReadOnly = true;
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
                    if (rb_PG.Checked == true) MessageBox.Show("La Pre-Guía no existe, esta anulada o ya esta pagada", "Error en Pre Guía", MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    if (rb_GR.Checked == true) MessageBox.Show("La GR no existe, esta anulada o ya esta pagada", "Error en Guía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (rb_DV.Checked == true) MessageBox.Show("El documento de venta, no existe" + Environment.NewLine +
                        "esta anulada o esta cancelada", "Error en Documento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //
                    tx_numGR.Text = "";
                    initIngreso();
                    tx_dat_tidoor.Text = v_tip2;
                    if (rb_GR.Checked == true) rb_GR.PerformClick();
                    if (rb_PG.Checked == true) rb_PG.PerformClick();
                    if (rb_DV.Checked == true) rb_DV.PerformClick();
                    tx_numGR.Focus();
                    return;
                }
                else
                {
                    double tg = 0, ts = 0;
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        tg = tg + double.Parse(dataGridView1.Rows[i].Cells["valor"].Value.ToString());
                        ts = ts + double.Parse(dataGridView1.Rows[i].Cells["saldo"].Value.ToString());
                    }
                    if (ts != tg && rb_DV.Checked == true)
                    {
                        MessageBox.Show("El documento de venta, debe cobrarse" + Environment.NewLine +
                        "desde sus guías individualmente", "Error en saldo del Documento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tx_numGR.Text = "";
                        initIngreso();
                        tx_dat_tidoor.Text = v_tip2;
                        if (rb_GR.Checked == true) rb_GR.PerformClick();
                        if (rb_PG.Checked == true) rb_PG.PerformClick();
                        if (rb_DV.Checked == true) rb_DV.PerformClick();
                        tx_numGR.Focus();
                        return;
                    }
                    else
                    {
                        cmb_mpago.Focus();
                        cmb_mpago.DroppedDown = true;
                    }
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
                MessageBox.Show("Seleccione el tipo de moneda de pago", " Atención ");
                cmb_mon.Focus();
                return;
            }
            if (tx_flete.Text.Trim() == "" || tx_flete.Text.Trim() == "0")
            {
                MessageBox.Show("No existe valor del documento", " Atención ");
                tx_serGR.Focus();
                return;
            }
            if (tx_dat_tdRem.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione un documento a cobrar", " Atención ");
                tx_serGR.Focus();
                return;
            }
            if (tx_dat_mp.Text == "")
            {
                MessageBox.Show("Seleccione un tipo de pago", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_mpago.Focus();
                return;
            }
            if (tx_dat_mone.Text != MonDeft && tx_tipcam.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione la moneda de pago y tipo de cambio", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_mon.Focus();
                return;
            }
            if (tx_PAGO.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el monto del pago", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_PAGO.Focus();
                return;
            }
            if (tx_idcaja.Text.Trim() == "")
            {
                MessageBox.Show("No existe Caja!","No puede continuar",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear la cobranza?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (true)
                        {
                            if (graba() == true)
                            {
                                // actualizamos la tabla seguimiento de usuarios
                                string resulta = lib.ult_mov(nomform, nomtab, asd);
                                if (resulta != "OK")
                                {
                                    MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                var xx = MessageBox.Show("Desea imprimir registro de cobranza?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (xx == DialogResult.Yes)
                                {
                                    Bt_print.PerformClick();
                                }
                            }
                        }
                    }
                    else
                    {
                        tx_serGR.Focus();
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
                        var aa = MessageBox.Show("Confirma que desea modificar la cobranza?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                // SOLO USUARIOS AUTORIZADOS DEBEN ACCEDER A ESTA OPCIÓN
                // SE ANULA EL DOCUMENTO Y LOS MOVIMIENTOS INTERNOS se hacen por B.D.
                // anulacion procede siempre y cuando sea de la fecha y del usuario
                if (asd != tx_dat_userdoc.Text || DateTime.Now.Date.ToString().Substring(0,10) != tx_fechope.Text)
                {
                    MessageBox.Show("No se puede ANULAR cobranzas fuera de fecha" + Environment.NewLine +
                        "o que sean de otro local/usuario","Atención",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    return;
                }
                if (tx_idr.Text.Trim() != "")
                {
                    var aa = MessageBox.Show("Confirma que desea ANULAR la cobranza?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        anula();
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
                    MessageBox.Show("El documento debe existir para poder anular!", "No esta el Id del registro", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                tx_dat_tidoor.Text = v_tip2;
            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                string inserta = "insert into cabcobran (" +
                    "fechope,tipdcob,sercobc,loccobc,estdcob,tidoorc,martdve,tipdoco,serdoco,numdoco,timepag,refpagc,cobrdor,obscobc,mondoco,totdoco,totpags," +
                    "saldvta,subdoco,igvdoco,codmopa,totpago,tcadvta,porcigv,totpaMN,codmoMN,impreso,cltdoco,dcltdoco,idcaja," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                    "@fechop,@ctdvta,@serdv,@ldcpgr,@estado,@tidoor,@martdv,@tipdoc,@serdoc,@numdoc,@timepa,@refpag,@cobrdo,@obsprg,@mondoc,@totpgr,@pagpgr," +
                    "@salxpa,@subpgr,@igvpgr,@monppr,@totpag,@tcoper,@porcig,@totMN,@codMN,@impSN,@cltdoc,@dcltdo,@idc," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@ctdvta", tx_dat_tdv.Text);
                    micon.Parameters.AddWithValue("@serdv", tx_serie.Text);
                    //micon.Parameters.AddWithValue("@numdv", tx_numero.Text);  // numero NO porque se autogenera en la BD
                    micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                    micon.Parameters.AddWithValue("@estado", tx_dat_estad.Text);
                    micon.Parameters.AddWithValue("@tidoor", (rb_PG.Checked == true)? "1":(rb_GR.Checked == true)? "2":"3");
                    micon.Parameters.AddWithValue("@martdv", tx_fb.Text);
                    micon.Parameters.AddWithValue("@tipdoc", tx_dat_tidoor.Text);
                    micon.Parameters.AddWithValue("@serdoc", tx_serGR.Text);
                    micon.Parameters.AddWithValue("@numdoc", tx_numGR.Text);
                    micon.Parameters.AddWithValue("@timepa", tx_dat_mp.Text);
                    micon.Parameters.AddWithValue("@refpag", tx_detpago.Text);
                    micon.Parameters.AddWithValue("@cobrdo", tx_cajero.Text);
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@mondoc", tx_dat_mod.Text);
                    micon.Parameters.AddWithValue("@totpgr", tx_flete.Text);                    // total doc origen
                    micon.Parameters.AddWithValue("@pagpgr", (tx_pagado.Text == "") ? "0" : tx_pagado.Text);
                    micon.Parameters.AddWithValue("@salxpa", (tx_salxcob.Text == "") ? "0" : tx_salxcob.Text);
                    micon.Parameters.AddWithValue("@subpgr", tx_subt.Text);                     // sub total
                    micon.Parameters.AddWithValue("@igvpgr", tx_igv.Text);                      // igv
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@totpag", tx_PAGO.Text);
                    micon.Parameters.AddWithValue("@tcoper", (tx_tipcam.Text == "")? "0" : tx_tipcam.Text);                   // TIPO DE CAMBIO
                    micon.Parameters.AddWithValue("@porcig", v_igv);                            // porcentaje en numeros de IGV
                    micon.Parameters.AddWithValue("@totMN", tx_fletMN.Text);
                    micon.Parameters.AddWithValue("@codMN", MonDeft);                           // codigo moneda local
                    micon.Parameters.AddWithValue("@impSN", tx_impreso.Text);
                    micon.Parameters.AddWithValue("@cltdoc", tx_dat_clte.Text); // tx_dat_tdRem.Text.PadRight(6).Substring(0,6)
                    micon.Parameters.AddWithValue("@dcltdo", tx_numDocRem.Text);
                    micon.Parameters.AddWithValue("@idc", tx_idcaja.Text);
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
                            tx_numero.Text = lib.Right(tx_idr.Text, 8);
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
                        string actua = "update cabcobran a set a.obscobc=@obsprg," +
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
                        // EDICION DEL DETALLE .... no hay detalle
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
        private void anula()
        {
            // en este caso solo hay ANULACION FISICA
            // Anulacion fisica se "anula" el numero del documento en sistema y
            // se borran todos los enlaces mediante triggers en la B.D.
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabcobran set estdcob=@estser,obscobc=@obse,usera=@asd,fecha=now()," +
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
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento del remitente, jala provincia
        {
            if(tx_dptoRtt.Text.Trim() != "")
            {
                // nada
            }
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia del remitente
        {
            if(tx_provRtt.Text != "" && tx_dptoRtt.Text.Trim() != "")
            {
                // naaaa
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)         // distrito
        {
            if(tx_distRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "" && tx_dptoRtt.Text.Trim() != "")
            {
                // menos aun
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
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            tx_serie.Text = lib.Right("0000" + tx_serie.Text, 4);
            if (Tx_modo.Text == "NUEVO") tx_serGR.Focus();
        }
        private void tx_serGR_Leave(object sender, EventArgs e)
        {
            tx_serGR.Text = lib.Right("0000" + tx_serGR.Text, 4);
            tx_numGR.Focus();
        }
        private void tx_numGR_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_serGR.Text.Trim() != "" && tx_numGR.Text.Trim() != "")
            {
                tx_numGR.Text = lib.Right("00000000" + tx_numGR.Text, 8);
            }
        }
        private void tx_pago_Leave(object sender, EventArgs e)
        {
            if (tx_PAGO.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                decimal vpag = decimal.Parse(tx_PAGO.Text);
                decimal vsal = decimal.Parse(tx_salxcob.Text);
                if (vpag <= 0)
                {
                    MessageBox.Show("El monto a pagar debe ser mayor a cero", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_PAGO.Text = "";
                    tx_PAGO.Focus();
                    return;
                }
                if (tx_dat_mone.Text == tx_dat_mod.Text)    // moneda del doc y moneda de pago son iguales?
                {
                    if (vpag > vsal)
                    {
                        MessageBox.Show("El monto a pagar no puede ser mayor al saldo", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        tx_PAGO.Text = "";
                        tx_PAGO.Focus();
                        return;
                    }
                    if (vpag < vsal && rb_GR.Checked == true)
                    {
                        var aa = MessageBox.Show("El valor pagado es MENOR al saldo del documento" + Environment.NewLine +
                            "Confirma que la operación es correcta?", "Atención - confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.No)
                        {
                            tx_PAGO.Text = "";
                            tx_PAGO.Focus();
                            return;
                        }
                    }
                    if (vpag < vsal && rb_DV.Checked == true)
                    {
                        MessageBox.Show("El pago de Docs. Venta deben ser cancelatorios","Atención",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                        tx_PAGO.Text = tx_salxcob.Text;
                        tx_PAGO.Focus();
                        return;
                    }
                    // calculos en moneda local
                    if (tx_dat_mone.Text == MonDeft)
                    {
                        tx_fletMN.Text = tx_PAGO.Text;
                    }
                    else
                    {
                        if (tx_tipcam.Text.Trim() == "")
                        {
                            MessageBox.Show("Se requiere tipo de cambio para operaciones" + Environment.NewLine +
                                "que no son en moneda local", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            cmb_mon.Focus();
                            return;
                        }
                        else
                        {
                            tx_fletMN.Text = Math.Round(decimal.Parse(tx_PAGO.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                        }
                    }
                }
                else
                {
                    // las monedas no son iguales
                    // SOLO SE ACEPTAN PAGOS EN LA MONEDA DEL DOCUMENTO
                    MessageBox.Show("Problema con la moneda e importes","Confirme moneda");
                    tx_PAGO.Text = "";
                    tx_PAGO.Focus();
                    return;
                }
            }
        }
        private void tx_fb_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tx_fb.Text != "B" && tx_fb.Text != "F")
            {
                e.Cancel = true;
            }
        }
        private void tx_fb_Leave(object sender, EventArgs e)
        {
            if (tx_fb.Text == "B") tx_dat_tidoor.Text = v_tip3;
            else tx_dat_tidoor.Text = v_tip4;
            tx_serGR.Focus();
        }
        private void rb_PG_Click(object sender, EventArgs e)            // boton pre guia
        {
            tx_fb.Visible = false;
            tx_fb.Text = "";
            tx_dat_tidoor.Text = v_tip1;
            tx_serGR.Focus();
        }
        private void rb_GR_Click(object sender, EventArgs e)            // boton guias
        {
            tx_fb.Visible = false;
            tx_fb.Text = "";
            tx_dat_tidoor.Text = v_tip2;
            tx_serGR.Focus();
        }
        private void rb_DV_Click(object sender, EventArgs e)            // boton doc. venta
        {
            tx_fb.Visible = true;
            tx_fb.Text = "B";
            tx_fb.Focus();
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
            // validamos caja abierta
            if (v_estcaj == codCier || v_estcaj == "")
            {
                MessageBox.Show("Debe aperturar caja para poder cobrar","Caja no abierta",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                return;
            }
            // validamos la fecha de la caja
            string fhoy = lib.fechaServ("ansi");
            if (fhoy != TransCarga.Program.vg_fcaj)  // ambas fecahs formato yyyy-mm-dd
            {
                MessageBox.Show("Debe cerrar la caja anterior!", "Caja fuera de fecha", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
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
            initIngreso();
            tx_numero.Text = "";
            tx_numero.ReadOnly = true;
            tx_noco.ReadOnly = true;
            tx_serie.ReadOnly = true;
            rb_GR.PerformClick();    // rb_GR.Checked = true;
            tx_cajero.Text = tx_nomuser.Text;
            tx_serGR.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            sololee();          
            Tx_modo.Text = "EDITAR";                    // solo puede editarse la observacion 28/10/2020
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            gbox_serie.Enabled = true;
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
            if (v_estcaj == codCier || v_estcaj == "")
            {
                MessageBox.Show("La caja debe estar abierta para" + Environment.NewLine +
                    "poder continuar!", "Caja no abierta", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
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
            // lo moví a change commitment
        }
        private void cmb_mon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO")    //  || Tx_modo.Text == "EDITAR"
            {
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    if (tx_dat_mone.Text == tx_dat_mod.Text)
                    {
                        tipcambio(tx_dat_mone.Text);
                    }
                    else
                    {
                        MessageBox.Show("La moneda de pago debe ser igual" + Environment.NewLine +
                            "a la moneda del documento", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        cmb_mon.SelectedValue = tx_dat_mod.Text;
                    }
                }
            }
        }
        private void cmb_mpago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_mpago.SelectedIndex > -1)
            {
                tx_dat_mp.Text = cmb_mpago.SelectedValue.ToString();
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
                string consulta = "update cabcobran set impreso=@sn where id=@idr";
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
