﻿using System;
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
        #endregion

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
        DataTable dtd = new DataTable();
        DataTable dttd0 = new DataTable();
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();
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
            autoprov();                                     // autocompleta provincias
            autodist();                                     // autocompleta distritos
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);

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
            tx_dirRem.MaxLength = 100;
            tx_distRtt.MaxLength = 25;
            tx_provRtt.MaxLength = 25;
            tx_dptoRtt.MaxLength = 25;
            tx_dirDrio.MaxLength = 100;
            tx_disDrio.MaxLength = 25;
            tx_proDrio.MaxLength = 25;
            tx_dptoDrio.MaxLength = 25;
            tx_docsOr.MaxLength = 100;          // documentos origen del traslado
            tx_nomRem.MaxLength = 100;           // nombre remitente
            tx_nomDrio.MaxLength = 100;           // nombre destinatario
            // grilla
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // todo desabilidado
            sololee(this);
        }
        private void initIngreso()
        {
            limpiar(this);
            limpia_chk();
            limpia_otros();
            limpia_combos();
            claveSeg = "";
            dataGridView1.Rows.Clear();
            tx_flete.Text = "";
            tx_numero.Text = "";
            tx_totcant.Text = "";
            tx_totpes.Text = "";
            tx_serie.Text = v_slu;
            tx_dat_tdi.Text = codDInt;
            tx_numero.ReadOnly = true;
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
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nofa,@nofi)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
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
                            if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "c_int") codDInt = row["valor"].ToString().Trim();           // codigo interno pre guias
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
                        "dirdestin,ubidestin,docsremit,obspregui,clifinpre,cantotpre,pestotpre,tipmonpre,tipcampre,seguroE," +
                        "subtotpre,igvpregui,totpregui,totpagpre,salpregui,estadoser,impreso,serguitra,numguitra,userc,userm,usera " +
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
                            tx_docsOr.Text = dr.GetString("docsremit");
                            tx_consig.Text = dr.GetString("clifinpre");
                            tx_dat_mone.Text = dr.GetString("tipmonpre");
                            tx_flete.Text = dr.GetDecimal("totpregui").ToString("#.##");
                            tx_totcant.Text = dr.GetString("cantotpre");
                            tx_totpes.Text = dr.GetDecimal("pestotpre").ToString("#.#");
                            tx_impreso.Text = dr.GetString("impreso");
                            tx_sergr.Text = dr.GetString("serguitra");
                            tx_numgr.Text = dr.GetString("numguitra");
                            claveSeg = dr.GetString("seguroE");
                        }
                        tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
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
                        //DataGridViewRow fg = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                        foreach (DataRow row in dt.Rows)
                        {
                            //fg.Cells[0].Value = row[3].ToString();
                            //fg.Cells[1].Value = row[4].ToString();
                            //fg.Cells[2].Value = row[6].ToString();
                            //fg.Cells[3].Value = row[7].ToString();
                            //dataGridView1.Rows.Add(fg);
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
            MySqlCommand ccl = new MySqlCommand("select idcodice,descrizionerid,ubidir from desc_loc where numero=@bloq",conn);
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
            MySqlCommand cdu = new MySqlCommand("select idcodice,descrizionerid,codigo from desc_doc where numero=@bloq", conn);
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
            // datos para el combo de moneda
            cmb_mon.Items.Clear();
            MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mon where numero=@bloq", conn);
            cmo.Parameters.AddWithValue("@bloq", 1);
            dacu = new MySqlDataAdapter(cmo);
            dtm.Clear();
            dacu.Fill(dtm);
            cmb_mon.DataSource = dtm;
            cmb_mon.DisplayMember = "descrizionerid";
            cmb_mon.ValueMember = "idcodice";
            //
            cmo.Dispose();
            ccl.Dispose();
            cdu.Dispose();
            dacu.Dispose();
            conn.Close();
        }
        private bool valiGri()                  // valida filas completas en la grilla
        {
            bool retorna = false;
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
            return retorna;
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
            if (tx_ubigO.Text.Trim() != "" && tx_provRtt.Text.Trim() != "")
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
            //this.checkBox1.Checked = false;
        }
        public void limpia_combos()
        {
            cmb_origen.SelectedIndex = -1;
            cmb_destino.SelectedIndex = -1;
            cmb_docRem.SelectedIndex = -1;
            cmb_docDes.SelectedIndex = -1;
            cmb_mon.SelectedIndex = -1;
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
                dataGridView1.Focus();
                return;
            }
            if (tx_totpes.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el detalle del envío", " Falta peso ");
                dataGridView1.Focus();
                return;
            }
            /*
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
            */
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // valida que las filas de la grilla esten completas
                if (valiGri() != true)
                {
                    MessageBox.Show("Complete las filas del detalle", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.Focus();
                    return;
                }
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear la guía?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            var bb = MessageBox.Show("Desea imprimir la Pre Guía?" + Environment.NewLine +
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
                try
                {
                    string inserta = "insert into cabpregr (" +
                        "fechpregr,serpregui,tidodepre,nudodepre,nombdepre,diredepre,ubigdepre," +
                        "tidorepre,nudorepre,nombrepre,direrepre,ubigrepre,locorigen,dirorigen,ubiorigen,locdestin," +
                        "dirdestin,ubidestin,docsremit,obspregui,clifinpre,cantotpre,pestotpre,tipmonpre,tipcampre," +
                        "subtotpre,igvpregui,totpregui,totpagpre,salpregui,estadoser,seguroE," +
                        "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                        "values (@fechop,@serpgr,@tdcdes,@ndcdes,@nomdes,@dircde,@ubicde," +
                        "@tdcrem,@ndcrem,@nomrem,@dircre,@ubicre,@locpgr,@dirpgr,@ubopgr,@ldcpgr," +
                        "@didegr,@ubdegr,@dooprg,@obsprg,@conprg,@totcpr,@totppr,@monppr,@tcprgr," +
                        "@subpgr,@igvpgr,@totpgr,@pagpgr,@totpgr,@estpgr,@clavse," +
                        "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                    MySqlCommand micon = new MySqlCommand(inserta, conn);
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6,4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@serpgr", tx_serie.Text);
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
                    micon.Parameters.AddWithValue("@obsprg", "");  // observaciones de la pre guia ... no hay
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
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.ExecuteNonQuery();
                    //
                    string lectura = "select last_insert_id()";
                    micon = new MySqlCommand(lectura, conn);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr.Read())
                    {
                        tx_idr.Text = dr.GetString(0);
                        tx_numero.Text = lib.Right("00000000" + dr.GetString(0),8);
                        dr.Close();
                        dr.Dispose();
                        // actualiza la tabla detalle,
                        string actua = "update detpregr set cantprodi=@can,unimedpro=@uni,codiprodi=@cod,descprodi=@des," +
                            "pesoprodi=@pes,precprodi=@preu,totaprodi=@pret " +
                            "where idc=@idr";
                        micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@can", dataGridView1.Rows[0].Cells[0].Value.ToString());
                        micon.Parameters.AddWithValue("@uni", dataGridView1.Rows[0].Cells[1].Value.ToString());
                        micon.Parameters.AddWithValue("@cod", "");
                        micon.Parameters.AddWithValue("@des", dataGridView1.Rows[0].Cells[2].Value.ToString());
                        micon.Parameters.AddWithValue("@pes", dataGridView1.Rows[0].Cells[3].Value.ToString());
                        micon.Parameters.AddWithValue("@preu", "0");
                        micon.Parameters.AddWithValue("@pret", "0");
                        micon.ExecuteNonQuery();
                        //
                        if (dataGridView1.Rows.Count > 2)
                        {
                            for(int i = 1; i < dataGridView1.Rows.Count - 1; i++)
                            {
                                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
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
                                    micon.Parameters.AddWithValue("@can", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                    micon.Parameters.AddWithValue("@uni", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                    micon.Parameters.AddWithValue("@cod", "");
                                    micon.Parameters.AddWithValue("@des", gloDeta + dataGridView1.Rows[i].Cells[2].Value.ToString().Trim());
                                    micon.Parameters.AddWithValue("@pes", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                    micon.Parameters.AddWithValue("@preu", "0");
                                    micon.Parameters.AddWithValue("@pret", "0");
                                    micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                                    micon.Parameters.AddWithValue("@verApp", verapp);
                                    micon.Parameters.AddWithValue("@asd", Program.vg_user);
                                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                    micon.ExecuteNonQuery();
                                }
                            }
                            micon.Dispose();
                        }
                        retorna = true;
                    }
                    dr.Close();
                    conn.Close();
                }
                catch(MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en insertar pre guía");
                    conn.Close();
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
                    if (tx_impreso.Text == "N")     // EDICION DE CABECERA Y CONTROL
                    {
                        string actua = "update cabpregr a, controlg b set " +
                            "a.fechpregr=@fechop,a.tidodepre=@tdcdes,a.nudodepre=@ndcdes," +
                            "a.nombdepre=@nomdes,a.diredepre=@dircde,a.ubigdepre=@ubicde,a.tidorepre=@tdcrem,a.nudorepre=@ndcrem," +
                            "a.nombrepre=@nomrem,a.direrepre=@dircre,a.ubigrepre=@ubicre,a.locorigen=@locpgr,a.dirorigen=@dirpgr," +
                            "a.ubiorigen=@ubopgr,a.locdestin=@ldcpgr,a.dirdestin=@didegr,a.ubidestin=@ubdegr,a.docsremit=@dooprg," +
                            "a.obspregui=@obsprg,a.clifinpre=@conprg,a.cantotpre=@totcpr,a.pestotpre=@totppr,a.tipmonpre=@monppr," +
                            "a.tipcampre=@tcprgr,a.subtotpre=@subpgr,a.igvpregui=@igvpgr,a.totpregui=@totpgr,a.totpagpre=@pagpgr," +
                            "a.salpregui=@totpgr,a.estadoser=@estpgr,a.seguroE=@clavse," +
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
                        micon.Parameters.AddWithValue("@dooprg", tx_docsOr.Text);
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
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE 
                        //
                        micon = new MySqlCommand("delete from detpregr where idc=@idr", conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.ExecuteNonQuery();
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
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
                    string canul = "update cabpregr, detpregr, controlg " +
                        "set cabpregr.estadoser=@estser,cabpregr.usera=@asd,cabpregr.fecha=now(),cabpregr.diriplan4=@dil4," +
                        "cabpregr.diripwan4=@diw4,cabpregr.netbname=@nbnp,cabpregr.verApp=@veap," +
                        "detpregr.estadoser=@estser,detpregr.usera=@asd,detpregr.fecha=now(),detpregr.verApp=@veap," +
                        "detpregr.diriplan4=@dil4,detpregr.diripwan4=@diw4,detpregr.netbname=@nbnp," +
                        "controlg.estadoser=@estser " +
                        "where cabpregr.id=detpregr.idc and cabpregr.id=controlg.numpregui and cabpregr.id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", lib.ipwan());
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
                dataGridView1.Rows.Clear();
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
                chk_seguridad_CheckStateChanged(null,null);
            }
        }
        private void textBox7_Leave(object sender, EventArgs e)         // departamento del remitente, jala provincia
        {
            if(tx_dptoRtt.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                tx_ubigRtt.Text = lib.retCodubigeo(tx_dptoRtt.Text.Trim(),"","");
                autoprov();
            }
        }
        private void textBox8_Leave(object sender, EventArgs e)         // provincia del remitente
        {
            if(tx_provRtt.Text != "" && tx_dptoRtt.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                tx_ubigRtt.Text = lib.retCodubigeo("", tx_provRtt.Text.Trim(), tx_ubigRtt.Text.Trim());
                autodist();
            }
        }
        private void textBox9_Leave(object sender, EventArgs e)         // distrito del remitente
        {
            if(tx_distRtt.Text.Trim() != "" && tx_provRtt.Text.Trim() != "" && tx_dptoRtt.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                tx_ubigRtt.Text = lib.retCodubigeo(tx_distRtt.Text.Trim(),"",tx_ubigRtt.Text.Trim());
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
            if (tx_dptoDrio.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                tx_ubigDtt.Text = lib.retCodubigeo(tx_dptoDrio.Text.Trim(),"","");
                autoprov();
            }
        }
        private void tx_proDio_Leave(object sender, EventArgs e)      // provincia del destinatario
        {
            if (tx_proDrio.Text.Trim() != "" && tx_dptoDrio.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                tx_ubigDtt.Text = lib.retCodubigeo("", tx_proDrio.Text.Trim(), tx_ubigDtt.Text.Trim());
                autodist();
            }
        }
        private void tx_disDrio_Leave(object sender, EventArgs e)      // distrito del destinatario
        {
            if (tx_proDrio.Text.Trim() != "" && tx_dptoDrio.Text.Trim() != "" && tx_disDrio.Text.Trim() != "" && TransCarga.Program.vg_conSol == false)
            {
                tx_ubigDtt.Text = lib.retCodubigeo(tx_disDrio.Text.Trim(), "", tx_ubigDtt.Text.Trim());
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
                    string[] datos = lib.datossn("CLI", tx_dat_tdRem.Text.Trim(), tx_numDocRem.Text.Trim());
                    if (datos[0] != "")   // datos.Length > 0
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
                            }
                        }
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
            if (Tx_modo.Text != "NUEVO")
            {
                // en el caso de las pre guias el numero es el mismo que el ID del registro
                tx_numero.Text = lib.Right("00000000" + tx_numero.Text, 8);
                tx_idr.Text = tx_numero.Text;
                jalaoc("tx_idr");
                dataGridView1.Rows.Clear();
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
            cmb_destino.Focus();
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
        private void cmb_docDes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_docDes.SelectedIndex > -1)
            {
                tx_dat_tDdest.Text = cmb_docDes.SelectedValue.ToString();
                DataRow[] fila = dttd1.Select("idcodice='" + tx_dat_tDdest.Text + "'");
                foreach (DataRow row in fila)
                {
                    tx_mldD.Text = row[2].ToString();
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
                DataRow[] fila = dtu.Select("idcodice='" + tx_dat_locori.Text + "'");
                tx_ubigO.Text = fila[0][2].ToString();
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
                DataRow[] fila = dtd.Select("idcodice='" + tx_dat_locdes.Text + "'");
                tx_ubigD.Text = fila[0][2].ToString();
            }
        }
        #endregion comboboxes

        #region datagridview
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            int totcant = 0;
            decimal totpes = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null)
                {
                    totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
                if (dataGridView1.Rows[i].Cells[3].Value != null)
                {
                    totpes = totpes + decimal.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString());
                }
            }
            tx_totcant.Text = totcant.ToString();
            tx_totpes.Text = totpes.ToString();
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if (dataGridView1.CurrentCell.ColumnIndex == 0 || dataGridView1.CurrentCell.ColumnIndex == 3)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
            if (dataGridView1.CurrentCell.ColumnIndex == 1 || dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                if (e.Control is TextBox)
                {
                    ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Upper;
                }
            }
        }
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            /*
            if (dataGridView1.Rows.Count > 1 && ("NUEVO,EDITAR").Contains(Tx_modo.Text))
            {
                if (e.RowIndex > -1) dataGridView1.CurrentRow.Cells[2].Value = gloDeta + " ";
            }
            */
            // cambie de idea. Al momento de grabar y si es NUEVO se agrega la glosa a cada fila 
        }
        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
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
            Font lt_gra = new Font("Arial", 13);                // grande
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
                string serie = tx_serie.Text;
                string corre = tx_numero.Text;
                //string nota = tipdo + " " + serie + "-" + corre;
                posi = posi + alfi + 8;
                string titnum = "PRE-GUIA " + serie + " - " + corre;
                lt = (CentimeterToPixel(anchTik) - e.Graphics.MeasureString(titnum, lt_gra).Width) / 2;
                puntoF = new PointF(lt, posi);
                e.Graphics.DrawString(titnum, lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("EMITIDO: ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 65, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 70, posi);
                e.Graphics.DrawString(tx_fechope.Text.Substring(0,10), lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("PARTIDA: ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(tx_dirOrigen.Text, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("DESTINO: ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(tx_dirDestino.Text, lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi * 2;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("REMITENTE: ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                if (tx_nomRem.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik), alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik), alfi * 1);
                RectangleF recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(tx_nomRem.Text.Trim(), lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                if (tx_nomRem.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                else posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("DESTINATARIO: ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                if (tx_nomDrio.Text.Trim().Length > 39) cuad = new SizeF(CentimeterToPixel(anchTik), alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik), alfi * 1);
                recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(tx_nomDrio.Text.Trim(), lt_peq, Brushes.Black, recdom, StringFormat.GenericTypographic);
                if (tx_nomRem.Text.Trim().Length > 39) posi = posi + alfi + alfi;
                else posi = posi + alfi * 2;
                puntoF = new PointF(coli, posi);
                /*
                e.Graphics.DrawString("Origen", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 65, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 70, posi);
                string dipa = cmb_origen.Text;
                if (dipa.Length < 60) cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                else cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 3);
                RectangleF recdir = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(dipa,lt_peq, Brushes.Black, recdir, StringFormat.GenericTypographic);
                if (dipa.Length < 60) posi = posi + alfi + alfi;
                else posi = posi + alfi + alfi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("Destino", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 65, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(coli + 70, posi);
                e.Graphics.DrawString(cmb_destino.Text, lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic); 
                posi = posi + alfi;
                */
                // **************** detalle del documento ****************//
                e.Graphics.DrawString("DETALLE DEL ENVIO: ", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                StringFormat alder = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                SizeF siz = new SizeF(70, 15);
                RectangleF recto = new RectangleF(puntoF, siz);
                for (int x=0; x < dataGridView1.Rows.Count - 1; x++)
                {
                    puntoF = new PointF(coli + 20.0F, posi);
                    e.Graphics.DrawString(dataGridView1.Rows[x].Cells[0].Value.ToString() 
                        + " " + dataGridView1.Rows[x].Cells[1].Value.ToString(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20.0F, posi);
                    e.Graphics.DrawString(dataGridView1.Rows[x].Cells[2].Value.ToString(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    posi = posi + alfi;
                    puntoF = new PointF(coli + 20.0F, posi);
                    e.Graphics.DrawString("KGs. " + dataGridView1.Rows[x].Cells[3].Value.ToString(), lt_peq, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                    puntoF = new PointF(coli + 90, posi);
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
