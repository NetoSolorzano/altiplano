using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;

namespace TransCarga
{
    public partial class repadmcaja : Form
    {
        static string nomform = "repadmcaja";           // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "cabcobran";            // 

        #region variables
        string asd = TransCarga.Program.vg_user;      // usuario conectado al sistema
        public int totfilgrid, cta;             // variables para impresion
        public string perAg = "";
        public string perMo = "";
        public string perAn = "";
        public string perIm = "";
        //string tipede = "";
        //string tiesta = "";
        string img_btN = "";
        string img_btE = "";
        string img_btP = "";
        string img_btA = "";            // anula = bloquea
        string img_btexc = "";          // exporta a excel
        string img_btq = "";
        string img_grab = "";
        string img_anul = "";
        string img_imprime = "";
        string v_ruta = "";             // ruta para los archivos que se exportan, vacio = ruta del sistema
        string img_preview = "";        // imagen del boton preview e imprimir reporte
        string cliente = Program.cliente;    // razon social para los reportes
        string codAnul = "";            // codigo de documento anulado
        string nomAnul = "";            // texto nombre del estado anulado
        string codGene = "";            // codigo documento nuevo generado
        string v_nccCR = "";            // nombre del formato CR del cuadre de caja
        string v_npcCR = "";            // nombre del formato CR pendientes de cobranza
        string v_rcsCR = "";            // nombre del formato CR reporte cobranzas semanales
        //int pageCount = 1, cuenta = 0;
        #endregion

        libreria lib = new libreria();
        DataTable dtcuad = new DataTable();
        DataTable dt = new DataTable();
        DataTable dtestad = new DataTable();
        DataTable dttaller = new DataTable();
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        public repadmcaja()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)    // F1
        {
            // en este form no usamos
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void repadmcaja_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
        }
        private void repadmcaja_Load(object sender, EventArgs e)
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
            dataload("todos");
            jalainfo();
            init();
            toolboton();
            KeyPreview = true;
            tabControl1.Enabled = false;
        }
        private void init()
        {
            tabControl1.BackColor = Color.FromName(TransCarga.Program.colgri);
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            dgv_ccaja.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dgv_resumen.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dgv_resumen.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dgv_resumen.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //
            dgv_vtas.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_guias.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_plan.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_pend.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            //Bt_ver.Image = Image.FromFile(img_btV);
            Bt_print.Image = Image.FromFile(img_btP);
            Bt_close.Image = Image.FromFile(img_btq);
            bt_exc.Image = Image.FromFile(img_btexc);
            Bt_close.Image = Image.FromFile(img_btq);
        }
        private void jalainfo()                                     // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in(@nofo,@ped,@caj)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@ped", nomform);
                micon.Parameters.AddWithValue("@caj", "ayccaja");
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["campo"].ToString() == "imagenes" && row["formulario"].ToString() == "main")
                    {
                        if (row["param"].ToString() == "img_btN") img_btN = row["valor"].ToString().Trim();         // imagen del boton de accion NUEVO
                        if (row["param"].ToString() == "img_btE") img_btE = row["valor"].ToString().Trim();         // imagen del boton de accion EDITAR
                        if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                        if (row["param"].ToString() == "img_btexc") img_btexc = row["valor"].ToString().Trim();     // imagen del boton exporta a excel
                        if (row["param"].ToString() == "img_btQ") img_btq = row["valor"].ToString().Trim();         // imagen del boton de accion SALIR
                        //if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();        // imagen del boton de accion IMPRIMIR
                        if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                        if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                        if (row["param"].ToString() == "img_imprime") img_imprime = row["valor"].ToString().Trim();  // imagen del boton IMPRIMIR REPORTE
                        if (row["param"].ToString() == "img_pre") img_preview = row["valor"].ToString().Trim();  // imagen del boton VISTA PRELIMINAR
                    }
                    if (row["campo"].ToString() == "estado" && row["formulario"].ToString() == "main")
                    {
                        if (row["param"].ToString() == "anulado") codAnul = row["valor"].ToString().Trim();         // codigo doc anulado
                        if (row["param"].ToString() == "generado") codGene = row["valor"].ToString().Trim();        // codigo doc generado
                        DataRow[] fila = dtestad.Select("idcodice='" + codAnul + "'");
                        nomAnul = fila[0][0].ToString();
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "exporta" && row["param"].ToString() == "ruta") v_ruta = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "pendcob") v_npcCR = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "repCobSem" && row["param"].ToString() == "nomfor_cr") v_rcsCR = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "ayccaja" && row["campo"].ToString() == "impresion" && row["param"].ToString() == "nomfor_cr")
                    {
                        v_nccCR = row["valor"].ToString().Trim();
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
        public void dataload(string quien)                          // jala datos para los combos y la grilla
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            if (quien == "todos")
            {
                // ***************** seleccion de la sede 
                string parte = "";
                if (("NIV002,NIV003").Contains(TransCarga.Program.vg_nius))
                {
                    parte = parte + "and idcodice='" + TransCarga.Program.vg_luse + "' ";
                }
                string contaller = "select descrizionerid,idcodice,codigo from desc_loc " +
                                       "where numero=1 " + parte + "order by idcodice";
                MySqlCommand cmd = new MySqlCommand(contaller, conn);
                MySqlDataAdapter dataller = new MySqlDataAdapter(cmd);
                // PANEL CUADRE CAJA
                dataller.Fill(dttaller);
                cmb_sedeCaj.DataSource = dttaller;
                cmb_sedeCaj.DisplayMember = "descrizionerid";
                cmb_sedeCaj.ValueMember = "idcodice";
                // panel COBRANZAS
                cmb_vtasloc.DataSource = dttaller;
                cmb_vtasloc.DisplayMember = "descrizionerid";
                cmb_vtasloc.ValueMember = "idcodice";
                // PANEL EGRESOS
                cmb_sede_guias.DataSource = dttaller;
                cmb_sede_guias.DisplayMember = "descrizionerid";
                cmb_sede_guias.ValueMember = "idcodice";
                // PANEL INGRESOS VARIOS
                cmb_sede_plan.DataSource = dttaller;
                cmb_sede_plan.DisplayMember = "descrizionerid"; ;
                cmb_sede_plan.ValueMember = "idcodice";
                // PANEL PENDIENTES DE COBRANZAS
                cmb_sede_pend.DataSource = dttaller;
                cmb_sede_pend.DisplayMember = "descrizionerid"; ;
                cmb_sede_pend.ValueMember = "idcodice";
                // ***************** seleccion de estado de servicios
                string conestad = "select descrizionerid,idcodice,codigo from desc_est " +
                                       "where numero=1 order by idcodice";
                cmd = new MySqlCommand(conestad, conn);
                MySqlDataAdapter daestad = new MySqlDataAdapter(cmd);
                daestad.Fill(dtestad);
                // PANEL CUADRE DE CAJA
                cmb_estCaj.DataSource = dtestad;
                cmb_estCaj.DisplayMember = "descrizionerid";
                cmb_estCaj.ValueMember = "idcodice";
                // PANEL COBRANZAS
                cmb_estad.DataSource = dtestad;
                cmb_estad.DisplayMember = "descrizionerid";
                cmb_estad.ValueMember = "idcodice";
                // PANEL EGRESOS
                cmb_estad_guias.DataSource = dtestad;
                cmb_estad_guias.DisplayMember = "descrizionerid";
                cmb_estad_guias.ValueMember = "idcodice";
                // PANEL INGRESOS VARIOS
                cmb_estad_plan.DataSource = dtestad;
                cmb_estad_plan.DisplayMember = "descrizionerid";
                cmb_estad_plan.ValueMember = "idcodice";
                // PANEL DE PENDIENTES DE COB
                // no hay ...
            }
            conn.Close();
        }
        private void grilla(string dgv)                             // arma las grillas
        {
            Font tiplg = new Font("Arial", 7, FontStyle.Bold);
            int b;
            switch (dgv)
            {
                case "dgv_ccaja":
                    dgv_ccaja.Font = tiplg;
                    dgv_ccaja.DefaultCellStyle.Font = tiplg;
                    dgv_ccaja.RowTemplate.Height = 15;
                    //dgv_ccaja.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    dgv_ccaja.AllowUserToAddRows = false;
                    if (dgv_ccaja.DataSource == null) dgv_ccaja.ColumnCount = 1;
                    /*
                    dgv_ccaja.Width = Parent.Width - 50;    // 1015;
                    if (dgv_ccaja.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_ccaja.Columns.Count; i++)
                        {
                            dgv_ccaja.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_ccaja.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_ccaja.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_ccaja.Columns.Count; i++)
                        {
                            int a = dgv_ccaja.Columns[i].Width;
                            b += a;
                            dgv_ccaja.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_ccaja.Columns[i].Width = a;
                        }
                        if (b < dgv_ccaja.Width) dgv_ccaja.Width = b - 20;
                        dgv_ccaja.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_ccaja");
                    break;
                case "dgv_vtas":                                    // COBRANZAS
                    dgv_vtas.Font = tiplg;
                    dgv_vtas.DefaultCellStyle.Font = tiplg;
                    dgv_vtas.RowTemplate.Height = 15;
                    //dgv_vtas.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    dgv_vtas.AllowUserToAddRows = false;
                    if (dgv_vtas.DataSource == null) dgv_vtas.ColumnCount = 11;
                    /*
                    dgv_vtas.Width = Parent.Width - 50; // 1015;
                    if (dgv_vtas.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_vtas.Columns.Count; i++)
                        {
                            dgv_vtas.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_vtas.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_vtas.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_vtas.Columns.Count; i++)
                        {
                            int a = dgv_vtas.Columns[i].Width;
                            b += a;
                            dgv_vtas.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_vtas.Columns[i].Width = a;
                        }
                        //if (b < dgv_vtas.Width) dgv_vtas.Width = b - 20;
                        dgv_vtas.Width = b + 50;
                        dgv_vtas.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_vtas");
                    break;
                case "dgv_guias":                                   // EGRESOS
                    dgv_guias.Font = tiplg;
                    dgv_guias.DefaultCellStyle.Font = tiplg;
                    dgv_guias.RowTemplate.Height = 15;
                    dgv_guias.AllowUserToAddRows = false;
                    if (dgv_guias.DataSource == null) dgv_guias.ColumnCount = 11;
                    /*
                    dgv_guias.Width = Parent.Width - 50; // 1015;
                    if (dgv_guias.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_guias.Columns.Count; i++)
                        {
                            dgv_guias.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_guias.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_guias.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_guias.Columns.Count; i++)
                        {
                            int a = dgv_guias.Columns[i].Width;
                            b += a;
                            dgv_guias.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_guias.Columns[i].Width = a;
                        }
                        if (b < dgv_guias.Width) dgv_guias.Width = b - 20;
                        dgv_guias.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_guias");
                    break;
                case "dgv_plan":                                // INGRESOS VARIOS
                    dgv_plan.Font = tiplg;
                    dgv_plan.DefaultCellStyle.Font = tiplg;
                    dgv_plan.RowTemplate.Height = 15;
                    dgv_plan.AllowUserToAddRows = false;
                    if (dgv_plan.DataSource == null) dgv_plan.ColumnCount = 11;
                    /*
                    dgv_plan.Width = Parent.Width - 50; // 1015;
                    if (dgv_plan.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_plan.Columns.Count; i++)
                        {
                            dgv_plan.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_plan.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_plan.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_plan.Columns.Count; i++)
                        {
                            int a = dgv_plan.Columns[i].Width;
                            b += a;
                            dgv_plan.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_plan.Columns[i].Width = a;
                        }
                        if (b < dgv_plan.Width) dgv_plan.Width = b - 20;
                        dgv_plan.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_plan");
                    break;
                case "dgv_pend":
                    dgv_pend.Font = tiplg;
                    dgv_pend.DefaultCellStyle.Font = tiplg;
                    dgv_pend.RowTemplate.Height = 15;
                    dgv_pend.AllowUserToAddRows = false;
                    if (dgv_pend.DataSource == null) dgv_pend.ColumnCount = 11;
                    /*
                    dgv_pend.Width = Parent.Width - 50;
                    if (dgv_pend.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_pend.Columns.Count; i++)
                        {
                            dgv_pend.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_pend.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_pend.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_pend.Columns.Count; i++)
                        {
                            int a = dgv_pend.Columns[i].Width;
                            b += a;
                            dgv_pend.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_pend.Columns[i].Width = a;
                        }
                        if (b < dgv_pend.Width) dgv_pend.Width = b - 20;
                        dgv_pend.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_pend");
                    break;
            }
        }
        private void suma_grilla(string dgv)
        {
            DataRow[] row = dtestad.Select("idcodice='" + codAnul + "'");   // dtestad
            string etiq_anulado = row[0].ItemArray[0].ToString();
            int cr = 0; // dgv_facts.Rows.Count;
            double tvv = 0, tva = 0, tvb = 0;
            switch (dgv)
            {
                case "dgv_pend":            // grilla pendientes de cobranza
                    for (int i = 0; i < dgv_pend.Rows.Count; i++)
                    {
                        tvv = tvv + Convert.ToDouble(dgv_pend.Rows[i].Cells["SALDO"].Value);
                        cr = cr + 1;
                    }
                    tx_tfi_p.Text = cr.ToString();
                    tx_totpend.Text = tvv.ToString("#0.00");
                    break;
                case "dgv_plan":            // grilla ingresos varios
                    for (int i = 0; i < dgv_plan.Rows.Count; i++)
                    {
                        if (dgv_plan.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_plan.Rows[i].Cells["PAGADO"].Value);
                            cr = cr + 1;
                        }
                        else
                        {
                            dgv_plan.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        }
                    }
                    tx_tf_iv.Text = cr.ToString();
                    tx_ingv.Text = tvv.ToString("#0.00");
                    break;
                case "dgv_guias":           // grilla egresos
                    for (int i = 0; i < dgv_guias.Rows.Count; i++)
                    {
                        if (dgv_guias.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_guias.Rows[i].Cells["PAGADO"].Value);
                            cr = cr + 1;
                        }
                        else
                        {
                            dgv_guias.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        }
                    }
                    tx_tf_e.Text = cr.ToString();
                    tx_tegre.Text = tvv.ToString("#0.00");
                    break;
                case "dgv_vtas":            // grilla cobranzas
                    if (rb_resumen.Checked == true && chk_semana.Checked == true)
                    {
                        for (int i = 0; i < dgv_vtas.Rows.Count; i++)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_vtas.Rows[i].Cells["TOT_SEM"].Value);
                            cr = cr + 1;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dgv_vtas.Rows.Count; i++)
                        {
                            if (dgv_vtas.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                            {
                                tvv = tvv + Convert.ToDouble(dgv_vtas.Rows[i].Cells["PAGADO"].Value);
                                cr = cr + 1;
                            }
                            else
                            {
                                dgv_vtas.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                            }
                        }
                    }
                    tx_tf_c.Text = cr.ToString();
                    tx_tcob.Text = tvv.ToString("#0.00");
                    break;
                case "dgv_ccaja":           // grilla cuadre caja
                    for (int i = 0; i < dgv_ccaja.Rows.Count; i++)
                    {
                        if (dgv_ccaja.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_ccaja.Rows[i].Cells["T_COB"].Value);       // Cobranzas
                            tva = tva + Convert.ToDouble(dgv_ccaja.Rows[i].Cells["T_EGRESOS"].Value);    // Egresos
                            tvb = tvb + Convert.ToDouble(dgv_ccaja.Rows[i].Cells["T_ING_VAR"].Value);    // Ing. varios
                            cr = cr + 1;
                        }
                        else
                        {
                            dgv_ccaja.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        }
                    }
                    tx_tfc.Text = cr.ToString();
                    tx_tcob_c.Text = tvv.ToString("#0.00");
                    tx_tegr_c.Text = tva.ToString("#0.00");
                    tx_ting_c.Text = tvb.ToString("#0.00");
                    break;
            }
        }
        private void bt_caja_Click(object sender, EventArgs e)          // CUADRE DE CAJA
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_adm_caj1";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@loca", (tx_dat_sedecaj.Text != "") ? tx_dat_sedecaj.Text : "");
                    micon.Parameters.AddWithValue("@fecini", dtp_iniCaj.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_finCaj.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", (tx_dat_estCaj.Text != "") ? tx_dat_estCaj.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_excCaj.Checked == true) ? "1" : "0");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_ccaja.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_ccaja.DataSource = dt;
                        grilla("dgv_ccaja");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_prev_caja_Click(object sender, EventArgs e)     // impresion cuadre de caja
        {
            if (dgv_ccaja.Rows.Count > 0 && dgv_ccaja.CurrentRow.Index > -1)
            {
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    if (lib.procConn(conn) == true)
                    {
                        using (MySqlCommand micon = new MySqlCommand("rep_cuadre_sede", conn))
                        {
                            micon.CommandType = CommandType.StoredProcedure;
                            micon.CommandTimeout = 300;
                            micon.Parameters.AddWithValue("@idc", dgv_ccaja.CurrentRow.Cells[0].Value.ToString());
                            using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                            {
                                dtcuad.Rows.Clear(); 
                                da.Fill(dtcuad);
                                setParaCrystal("cuadre_caja");
                            }
                        }
                    }
                }
            }
        }
        private void bt_prev_pend_Click(object sender, EventArgs e)     // impresion pendientes de cobranza
        {
            setParaCrystal("pend_cob");
        }
        private void bt_vtasfiltra_Click(object sender, EventArgs e)    // COBRANZAS
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (rb_listado.Checked == true)
                {
                    string consulta = "rep_adm_cob1";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@loca", (tx_dat_vtasloc.Text != "") ? tx_dat_vtasloc.Text : "");
                        micon.Parameters.AddWithValue("@fecini", dtp_vtasfini.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fecfin", dtp_vtasfina.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@esta", (tx_dat_estad.Text != "") ? tx_dat_estad.Text : "");
                        micon.Parameters.AddWithValue("@excl", (chk_excluye.Checked == true) ? "1" : "0");
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            dgv_vtas.DataSource = null;
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgv_vtas.DataSource = dt;
                            grilla("dgv_vtas");
                        }
                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                        if (resulta != "OK")                                        // actualizamos la tabla usuarios
                        {
                            MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                if (rb_resumen.Checked == true && chk_semana.Checked == false)
                {
                    string consulta = "rep_adm_cob1";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@loca", "resume");
                        micon.Parameters.AddWithValue("@fecini", dtp_vtasfini.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fecfin", dtp_vtasfina.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@esta", "");
                        micon.Parameters.AddWithValue("@excl", "0");
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            dgv_vtas.DataSource = null;
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgv_vtas.DataSource = dt;
                            grilla("dgv_vtas");
                        }
                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                        if (resulta != "OK")                                        // actualizamos la tabla usuarios
                        {
                            MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                if (rb_resumen.Checked == true && chk_semana.Checked == true)
                {
                    string consulta = "res_sem_cob";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@feini", dtp_vtasfini.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fefin", dtp_vtasfina.Value.ToString("yyyy-MM-dd"));
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            dgv_vtas.DataSource = null;
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgv_vtas.DataSource = dt;
                            grilla("dgv_vtas");
                        }
                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                        if (resulta != "OK")                                        // actualizamos la tabla usuarios
                        {
                            MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        private void bt_guias_Click(object sender, EventArgs e)         // EGRESOS
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_adm_egre1";
                using (MySqlCommand micon = new MySqlCommand(consulta,conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@loca", (tx_sede_guias.Text != "") ? tx_sede_guias.Text : "");
                    micon.Parameters.AddWithValue("@fecini", dtp_ini_guias.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fin_guias.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", (tx_estad_guias.Text != "") ? tx_estad_guias.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_excl_guias.Checked == true) ? "1" : "0");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_guias.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_guias.DataSource = dt;
                        grilla("dgv_guias");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_plan_Click(object sender, EventArgs e)          // INGRESOS VARIOS
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_adm_ingv1";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@fecini", dtp_fini_plan.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fter_plan.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@loca", (tx_dat_sede_plan.Text != "") ? tx_dat_sede_plan.Text : "");
                    micon.Parameters.AddWithValue("@esta", (tx_dat_estad_plan.Text != "") ? tx_dat_estad_plan.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_exclu_plan.Checked == true)? "1" : "0");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_plan.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_plan.DataSource = dt;
                        grilla("dgv_plan");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_pend_Click(object sender, EventArgs e)          // PENDIENTES DE COBRANZA
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_adm_pendcob1";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@loca", (tx_dat_sed_pend.Text != "") ? tx_dat_sed_pend.Text : "");
                    micon.Parameters.AddWithValue("@fecini", dtp_fini_pend.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fina_pend.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", codAnul);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_pend.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_pend.DataSource = dt;
                        grilla("dgv_pend");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #region combos
        private void cmb_sedeCaj_SelectionChangeCommitted(object sender, EventArgs e)             // CAJA - Sede
        {
            if (cmb_sedeCaj.SelectedValue != null) tx_dat_sedecaj.Text = cmb_sedeCaj.SelectedValue.ToString();
            else tx_dat_sedecaj.Text = "";
        }
        private void cmb_estCaj_SelectionChangeCommitted(object sender, EventArgs e)              // CAJA - Estados
        {
            if (cmb_estCaj.SelectedValue != null) tx_dat_estCaj.Text = cmb_estCaj.SelectedValue.ToString();
            else tx_dat_estCaj.Text = "";
        }
        private void cmb_sedeCaj_KeyDown(object sender, KeyEventArgs e)                           // CAJA - Sede delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sedeCaj.SelectedIndex = -1;
                tx_dat_sedecaj.Text = "";
            }
        }
        private void cmb_estCaj_KeyDown(object sender, KeyEventArgs e)                            // CAJA - Estados delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estCaj.SelectedIndex = -1;
                tx_dat_estCaj.Text = "";
            }
        }
        private void cmb_estad_ing_SelectionChangeCommitted(object sender, EventArgs e)           // COBRANZAS - Estados
        {
            if (cmb_estad.SelectedValue != null) tx_dat_estad.Text = cmb_estad.SelectedValue.ToString();
            else
            {
                tx_dat_estad.Text = "";    // cmb_estad.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
                chk_excluye.Checked = false;
            }
        }
        private void cmb_vtasloc_SelectionChangeCommitted(object sender, EventArgs e)             // COBRANZAS - Sede
        {
            if (cmb_vtasloc.SelectedValue != null) tx_dat_vtasloc.Text = cmb_vtasloc.SelectedValue.ToString();
            else tx_dat_vtasloc.Text = ""; // cmb_vtasloc.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_estad_ing_KeyDown(object sender, KeyEventArgs e)                         // COBRANZAS - Estados delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad.SelectedIndex = -1;
                tx_dat_estad.Text = "";
            }
        }
        private void cmb_vtasloc_KeyDown(object sender, KeyEventArgs e)                           // COBRANZAS - Sede delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_vtasloc.SelectedIndex = -1;
                tx_dat_vtasloc.Text = "";
            }
        }
        private void cmb_sede_plan_SelectionChangeCommitted(object sender, EventArgs e)           // INGRESOS VARIOS - Sede
        {
            if (cmb_sede_plan.SelectedValue != null) tx_dat_sede_plan.Text = cmb_sede_plan.SelectedValue.ToString();
            else tx_dat_sede_plan.Text = "";
        }
        private void cmb_sede_plan_KeyDown(object sender, KeyEventArgs e)                         // INGRESOS VARIOS - Sede delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_plan.SelectedIndex = -1;
                tx_dat_sede_plan.Text = "";
            }
        }
        private void cmb_estad_plan_SelectionChangeCommitted(object sender, EventArgs e)          // INGRESOS VARIOS - Estados
        {
            if (cmb_estad_plan.SelectedValue != null) tx_dat_estad_plan.Text = cmb_estad_plan.SelectedValue.ToString();
            else tx_dat_estad_plan.Text = "";
        }
        private void cmb_estad_plan_KeyDown(object sender, KeyEventArgs e)                        // INGRESOS VARIOS - Estados delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad_plan.SelectedIndex = -1;
                tx_dat_estad_plan.Text = "";
            }
        }
        private void cmb_sede_guias_SelectionChangeCommitted(object sender, EventArgs e)          // EGRESOS - Sede
        {
            if (cmb_sede_guias.SelectedValue != null) tx_sede_guias.Text = cmb_sede_guias.SelectedValue.ToString();
            else tx_sede_guias.Text = "";
        }
        private void cmb_sede_guias_KeyDown(object sender, KeyEventArgs e)                        // EGRESOS - Sede delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_guias.SelectedIndex = -1;
                tx_sede_guias.Text = "";
            }
        }
        private void cmb_estad_guias_SelectionChangeCommitted(object sender, EventArgs e)         // EGRESOS - Estados
        {
            if (cmb_estad_guias.SelectedValue != null) tx_estad_guias.Text = cmb_estad_guias.SelectedValue.ToString();
            else tx_estad_guias.Text = "";
        }
        private void cmb_estad_guias_KeyDown(object sender, KeyEventArgs e)                       // EGRESOS - Estados delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad_guias.SelectedIndex = -1;
                tx_estad_guias.Text = "";
            }
        }
        private void cmb_sede_pend_SelectionChangeCommitted(object sender, EventArgs e)           // PENDIENTES - sedes
        {
            if (cmb_sede_pend.SelectedValue != null) tx_dat_sed_pend.Text = cmb_sede_pend.SelectedValue.ToString();
            else tx_dat_sed_pend.Text = "";
        }
        private void cmb_sede_pend_KeyDown(object sender, KeyEventArgs e)                         // PENDIENTES - delete
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_pend.SelectedIndex = -1;
                tx_dat_sed_pend.Text = "";
            }
        }
        #endregion

        #region botones de comando
        public void toolboton()
        {
            Bt_add.Visible = false;
            Bt_edit.Visible = false;
            Bt_anul.Visible = false;
            Bt_print.Visible = false;
            bt_exc.Visible = false;
            Bt_ini.Visible = false;
            Bt_sig.Visible = false;
            Bt_ret.Visible = false;
            Bt_fin.Visible = false;
            //
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
                if (Convert.ToString(row["btn1"]) == "S")               // nuevo ... ok
                {
                    this.Bt_add.Visible = true;
                }
                else { this.Bt_add.Visible = false; }
                if (Convert.ToString(row["btn2"]) == "S")               // editar ... ok
                {
                    this.Bt_edit.Visible = true;
                }
                else { this.Bt_edit.Visible = false; }
                if (Convert.ToString(row["btn3"]) == "S")               // anular ... ok
                {
                    this.Bt_anul.Visible = true;
                }
                else { this.Bt_anul.Visible = false; }
                /*if (Convert.ToString(row["btn4"]) == "S")               // visualizar ... ok
                {
                    this.bt_view.Visible = true;
                }
                else { this.bt_view.Visible = false; }*/
                if (Convert.ToString(row["btn5"]) == "S")               // imprimir ... ok
                {
                    this.Bt_print.Visible = true;
                }
                else { this.Bt_print.Visible = false; }
                /*if (Convert.ToString(row["btn7"]) == "S")               // vista preliminar ... ok
                {
                    this.bt_prev.Visible = true;
                }
                else { this.bt_prev.Visible = false; }*/
                if (Convert.ToString(row["btn8"]) == "S")               // exporta xlsx  .. ok
                {
                    this.bt_exc.Visible = true;
                }
                else { this.bt_exc.Visible = false; }
                if (Convert.ToString(row["btn6"]) == "S")               // salir del form ... ok
                {
                    this.Bt_close.Visible = true;
                }
                else { this.Bt_close.Visible = false; }
            }
        }
        private void Bt_add_Click(object sender, EventArgs e)
        {
            // nothing to do
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            // nothing to do
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "IMPRIMIR";
            tabControl1.Enabled = true;
            // caja
            cmb_sedeCaj.SelectedIndex = -1;
            cmb_estCaj.SelectedIndex = -1;
            chk_excCaj.Checked = false;
            // cobranzas
            cmb_estad.SelectedIndex = -1;
            cmb_vtasloc.SelectedIndex = -1;
            chk_excluye.Checked = false;
            rb_listado.Checked = true;
            // egresos
            cmb_sede_guias.SelectedIndex = -1;
            cmb_estad_guias.SelectedIndex = -1;
            chk_excl_guias.Checked = false;
            // ingresos varios
            cmb_sede_plan.SelectedIndex = -1;
            cmb_estad_plan.SelectedIndex = -1;
            chk_exclu_plan.Checked = false;
            // pendientes de cob
            cmb_sede_pend.SelectedIndex = -1;
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            // nothing to do
        }
        private void bt_exc_Click(object sender, EventArgs e)
        {
            if (tabControl1.Enabled == false) return;
            // CAJA
            string nombre = "";
            if (tabControl1.SelectedTab == tabres && dgv_ccaja.Rows.Count > 0)
            {
                nombre = "reporte_cajas_" + "<local>" +"_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_ccaja.DataSource;
                    wb.Worksheets.Add(dt, "Cajas");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            // COBRANZAS
            if (tabControl1.SelectedTab == tabvtas && dgv_vtas.Rows.Count > 0)
            {
                nombre = "Reportes_Cobranzas_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_vtas.DataSource;
                    wb.Worksheets.Add(dt, "Cobranzas");
                    wb.SaveAs(v_ruta + nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            // EGRESOS
            if (tabControl1.SelectedTab == tabgrti && dgv_guias.Rows.Count > 0)
            {
                nombre = "Reportes_Egresos_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_guias.DataSource;
                    wb.Worksheets.Add(dt, "Egresos");
                    wb.SaveAs(v_ruta + nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            // INGRESOS VARIOS
            if (tabControl1.SelectedTab == tabplacar && dgv_plan.Rows.Count > 0)
            {
                nombre = "Reportes_IngVarios_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_plan.DataSource;
                    wb.Worksheets.Add(dt, "IngVarios");
                    wb.SaveAs(v_ruta + nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            // PENDIENTES DE PAGO
            if (tabControl1.SelectedTab == tabpend && dgv_pend.Rows.Count > 0)
            {
                nombre = "Pendientes_Cobranza_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_pend.DataSource;
                    wb.Worksheets.Add(dt, "Pendientes");
                    wb.SaveAs(v_ruta + nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
        }
        #endregion

        #region crystal
        private void button4_Click(object sender, EventArgs e)      // reporte de cobranzas
        {
            if (rb_resumen.Checked == true && chk_semana.Checked == true)
            {
                setParaCrystal("rescobsem");
            }
        }
        private void setParaCrystal(string repo)                    // genera el set para el reporte de crystal
        {
            if (repo== "cuadre_caja")
            {
                conClie datos = generacuadre();                        // conClie = dataset de impresion de contrato   
                frmvizoper visualizador = new frmvizoper(datos);        // POR ESO SE CREO ESTE FORM frmvizcont PARA MOSTRAR AHI. ES MEJOR ASI.  
                visualizador.Show();
            }
            if (repo == "pend_cob")
            {
                conClie datos = generareppend();
                frmvizoper visualizador = new frmvizoper(datos);
                visualizador.Show();
            }
            if (repo == "rescobsem")
            {
                conClie datos = generarepcobsem();
                frmvizoper visualizador = new frmvizoper(datos);
                visualizador.Show();
            }
        }
        private conClie generarepcobsem()                           // resumen de cobranzas semanales
        {
            conClie rrepcobs = new conClie();                        // xsd
            for(int i=0; i<dgv_vtas.Rows.Count-1; i++)
            {
                conClie.repCobsemRow cabrow = rrepcobs.repCobsem.NewrepCobsemRow();
                DataGridViewRow row = dgv_vtas.Rows[i];
                if (row.Cells["semana"].Value != null && row.Cells["semana"].Value.ToString().Trim() != "")
                {
                    cabrow.rucEmisor = Program.ruc;
                    cabrow.nomEmisor = Program.cliente;
                    cabrow.dirEmisor = Program.dirfisc;
                    cabrow.fechRep = DateTime.Now;
                    cabrow.fecini = dtp_vtasfini.Value.ToString("dd/MM/yyyy");
                    cabrow.fecfin = dtp_vtasfina.Value.ToString("dd/MM/yyyy");
                    cabrow.nomSemana = row.Cells[0].Value.ToString();
                    cabrow.sede1 = decimal.Parse(row.Cells[1].Value.ToString());
                    cabrow.sede2 = decimal.Parse(row.Cells[2].Value.ToString());
                    cabrow.sede3 = decimal.Parse(row.Cells[3].Value.ToString());
                    cabrow.totalSem = decimal.Parse(row.Cells[4].Value.ToString());
                    cabrow.formatoRPT = v_rcsCR;
                    cabrow.numsem = i.ToString();
                    cabrow.nomsed1 = row.Cells[1].OwningColumn.Name;
                    cabrow.nomsed2 = row.Cells[2].OwningColumn.Name;
                    cabrow.nomsed3 = row.Cells[3].OwningColumn.Name;
                    rrepcobs.repCobsem.AddrepCobsemRow(cabrow);
                }
            }
            return rrepcobs;
        }
        private conClie generareppend()                             // genera el set para los pendientes de cobranza
        {
            conClie repPend = new conClie();
            int cta = 0;
            foreach (DataGridViewRow row in dgv_pend.Rows)
            {
                cta = cta + 1;
                conClie.pendCobRow pendrow = repPend.pendCob.NewpendCobRow();
                pendrow.rucEmisor = Program.ruc;
                pendrow.dirEmisor = Program.dirfisc;
                pendrow.nomEmisor = Program.cliente;
                pendrow.fecini = dtp_fini_pend.Text.Substring(0, 10);
                pendrow.fecfin = dtp_fina_pend.Text.Substring(0, 10);
                pendrow.cta = cta.ToString();
                pendrow.sede = cmb_sede_pend.Text;
                pendrow.fechRep = DateTime.Now.ToString();
                pendrow.origen = row.Cells["ORIGEN"].Value.ToString();
                pendrow.destino = row.Cells["DESTINO"].Value.ToString();
                pendrow.fecha = row.Cells["FECHA"].Value.ToString();
                pendrow.serie = row.Cells["SER"].Value.ToString();
                pendrow.numero = row.Cells["NUMERO"].Value.ToString();
                pendrow.docr = row.Cells["DOCR"].Value.ToString();
                pendrow.remitente = row.Cells["REMITENTE"].Value.ToString();
                pendrow.nombrer = row.Cells["NOMBRER"].Value.ToString();
                pendrow.docd = row.Cells["DOCD"].Value.ToString();
                pendrow.destinat = row.Cells["DESTINAT"].Value.ToString();
                pendrow.nombred = row.Cells["NOMBRED"].Value.ToString();
                pendrow.mon = row.Cells["MON"].Value.ToString();
                pendrow.flete = double.Parse(row.Cells["FLETE"].Value.ToString());
                pendrow.tdv = row.Cells["TDV"].Value.ToString();
                pendrow.servta = row.Cells["SERVTA"].Value.ToString();
                pendrow.numvta = row.Cells["NUMVTA"].Value.ToString();
                pendrow.pagado = double.Parse(row.Cells["PAGADO"].Value.ToString());
                pendrow.saldo = double.Parse(row.Cells["SALDO"].Value.ToString());
                pendrow.atraso = int.Parse(row.Cells["ATRASO"].Value.ToString());
                pendrow.tituloF = Program.tituloF;
                pendrow.formatoRPT = v_npcCR;
                repPend.pendCob.AddpendCobRow(pendrow);
            }
            return repPend;
        }
        private conClie generacuadre()                              // genera cuadre de caja
        {
            conClie cuadre = new conClie();                                    // dataset
            conClie.cuadreCaja_cabRow rowcabeza = cuadre.cuadreCaja_cab.NewcuadreCaja_cabRow(); // rescont.rescont_cab.Newrescont_cabRow();
            //
            rowcabeza.rucEmisor = Program.ruc;
            rowcabeza.nomEmisor = Program.cliente;
            rowcabeza.dirEmisor = Program.dirfisc;
            rowcabeza.formatoRPT = v_nccCR;
            rowcabeza.id = dtcuad.Rows[0].ItemArray[3].ToString();
            rowcabeza.serie = dtcuad.Rows[0].ItemArray[8].ToString();
            rowcabeza.corre = dtcuad.Rows[0].ItemArray[9].ToString();
            rowcabeza.cajeroA = dtcuad.Rows[0].ItemArray[28].ToString();
            rowcabeza.cajeroC = dtcuad.Rows[0].ItemArray[30].ToString();
            rowcabeza.codloc = dtcuad.Rows[0].ItemArray[1].ToString();
            rowcabeza.corre = dtcuad.Rows[0].ItemArray[9].ToString();
            rowcabeza.dircloc = ""; // dtcuad.Rows[0].ItemArray[].ToString();
            rowcabeza.estado = dtcuad.Rows[0].ItemArray[6].ToString();
            rowcabeza.fechAbier = dtcuad.Rows[0].ItemArray[4].ToString().Substring(0, 10);
            rowcabeza.fechCierr = (dtcuad.Rows[0].ItemArray[5].ToString().Trim() == "")? "" : dtcuad.Rows[0].ItemArray[5].ToString().Substring(0, 10);
            rowcabeza.nomCajA = dtcuad.Rows[0].ItemArray[29].ToString();
            rowcabeza.nomCajC = dtcuad.Rows[0].ItemArray[31].ToString();
            rowcabeza.nomloc = dtcuad.Rows[0].ItemArray[2].ToString();
            rowcabeza.cobranzas = Double.Parse(dtcuad.Rows[0].ItemArray[21].ToString());
            rowcabeza.ingvarios = Double.Parse(dtcuad.Rows[0].ItemArray[22].ToString());
            rowcabeza.egresos = Double.Parse(dtcuad.Rows[0].ItemArray[23].ToString());

            //MessageBox.Show(Double.Parse(dtcuad.Rows[0].ItemArray[26].ToString()).ToString());
            
            rowcabeza.saldoAnt = Double.Parse(dtcuad.Rows[0].ItemArray[26].ToString());
            //MessageBox.Show(rowcabeza.saldoAnt.ToString());

            rowcabeza.saldofinal = Double.Parse(dtcuad.Rows[0].ItemArray[27].ToString());
            rowcabeza.serie = dtcuad.Rows[0].ItemArray[8].ToString();
            rowcabeza.tituloF = Program.tituloF;
            cuadre.cuadreCaja_cab.AddcuadreCaja_cabRow(rowcabeza);    //rescont.rescont_cab.Addrescont_cabRow(rowcabeza);
            // detalle
            foreach (DataRow row in dtcuad.Rows)
            {
                if (true)
                {
                    conClie.cuadreCaja_detRow rowdetalle = cuadre.cuadreCaja_det.NewcuadreCaja_detRow();
                    rowdetalle.segmento = row.ItemArray[0].ToString();       // nombre del segmento
                    rowdetalle.id = row.ItemArray[3].ToString();             // id de la caja
                    rowdetalle.fecha = row.ItemArray[4].ToString().Substring(0, 10);          // fecha del doc del segmento
                    rowdetalle.estado = row.ItemArray[6].ToString();         // estado del doc del segmento
                    rowdetalle.nomEst = row.ItemArray[7].ToString();         // nombre del estado
                    rowdetalle.serSeg = row.ItemArray[8].ToString();         // serie del doc del segmento
                    rowdetalle.numSeg = row.ItemArray[9].ToString();         // numero del doc del segmento
                    rowdetalle.tipDoc = row.ItemArray[10].ToString();        // tipo del documento
                    rowdetalle.nomTdoc = row.ItemArray[11].ToString();       // nombre del tipo de doc
                    rowdetalle.serDoc = row.ItemArray[12].ToString();        // serie del documento
                    rowdetalle.numDoc = row.ItemArray[13].ToString();        // numero del documento
                    rowdetalle.tmepag = row.ItemArray[14].ToString();        // codigo moneda del documento
                    rowdetalle.nomMond = row.ItemArray[15].ToString();       // nombre moneda del documento
                    rowdetalle.codTipg = row.ItemArray[16].ToString();       // codigo tipo pago/cobranza
                    rowdetalle.nomTipg = row.ItemArray[17].ToString();       // nombre del tipo
                    rowdetalle.codCtag = row.ItemArray[18].ToString();       // codigo cuenta depositos
                    rowdetalle.nomCtag = row.ItemArray[19].ToString();       // nombre de cuenta
                    rowdetalle.refpago = row.ItemArray[20].ToString();       // referencia de pago/deposito/ingreso
                    rowdetalle.totdoco = double.Parse(row.ItemArray[21].ToString());       // total del documento
                    rowdetalle.totpags = double.Parse(row.ItemArray[22].ToString());       // total pagado
                    rowdetalle.saldvta = Double.Parse(row.ItemArray[23].ToString());       // saldo del doc
                    rowdetalle.codmopa = row.ItemArray[24].ToString();       // codigo moneda de pago
                    rowdetalle.nomMonp = row.ItemArray[25].ToString();       // nombre de la moneda de pago
                    rowdetalle.totpago = Double.Parse(row.ItemArray[26].ToString());       // total pagado/cobrado
                    rowdetalle.totpaMN = Double.Parse(row.ItemArray[27].ToString());       // total pagado/cobrado en MN
                    rowdetalle.nomclie = row.ItemArray[29].ToString();       // nombre del cliente en cobranzas 
                    cuadre.cuadreCaja_det.AddcuadreCaja_detRow(rowdetalle);
                }
            }
            return cuadre;
        }
        #endregion

        #region leaves y enter
        private void tabvtas_Enter(object sender, EventArgs e)           // COBRANZAS
        {
            cmb_vtasloc.Focus();
        }
        private void tabres_Enter(object sender, EventArgs e)            // CAJA
        {
            cmb_sedeCaj.Focus();
        }
        private void tabgrti_Enter(object sender, EventArgs e)           // EGRESOS
        {
            cmb_sede_guias.Focus();
        }
        private void tabplacar_Enter(object sender, EventArgs e)         // INGRESOS VARIOS
        {
            cmb_sede_plan.Focus();
        }
        private void rb_listado_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_listado.Checked == true)
            {
                chk_semana.Checked = false;
                chk_semana.Enabled = false;
                cmb_vtasloc.SelectedIndex = -1;
                cmb_vtasloc.Enabled = true;
                cmb_estad.SelectedIndex = -1;
                cmb_estad.Enabled = true;
                button4.Enabled = false;
            }
            else
            {
                chk_semana.Checked = false;
                chk_semana.Enabled = true;
                //rb_listado.Enabled = false;
                cmb_vtasloc.SelectedIndex = -1;
                cmb_vtasloc.Enabled = false;
                cmb_estad.SelectedIndex = -1;
                cmb_estad.Enabled = false;
            }
        }
        private void rb_resumen_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_resumen.Checked == true)
            {
                chk_semana.Checked = false;
                chk_semana.Enabled = true;
                cmb_vtasloc.SelectedIndex = -1;
                cmb_vtasloc.Enabled = false;
                cmb_estad.SelectedIndex = -1;
                cmb_estad.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                chk_semana.Checked = false;
                chk_semana.Enabled = false;
                //rb_listado.Enabled = true;
                cmb_vtasloc.SelectedIndex = -1;
                cmb_vtasloc.Enabled = true;
                cmb_estad.SelectedIndex = -1;
                cmb_estad.Enabled = true;
            }
        }
        private void chk_semana_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_semana.Checked == true && rb_resumen.Checked == true) button4.Enabled = true;
            else button4.Enabled = false;
        }
        #endregion

        #region advancedatagridview
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)                  // filtro COBRANZAS
        {
            DataTable dtg = (DataTable)dgv_vtas.DataSource;
            dtg.DefaultView.RowFilter = dgv_vtas.FilterString;
            suma_grilla("dgv_vtas");
        }
        private void dgv_vtas_SortStringChanged(object sender, EventArgs e)                                 // sort cobranzas
        {
            DataTable dtg = (DataTable)dgv_vtas.DataSource;
            dtg.DefaultView.Sort = dgv_vtas.SortString;
        }
        private void advancedDataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)            // no usamos
        {
            //advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }
        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)      // no usamos
        {
            /*if(e.ColumnIndex == 1)
            {
                //string codu = "";
                string idr = "";
                idr = advancedDataGridView1.CurrentRow.Cells[0].Value.ToString();
                tx_rind.Text = advancedDataGridView1.CurrentRow.Index.ToString();
                tabControl1.SelectedTab = tabreg;
                limpiar(this);
                limpia_otros();
                limpia_combos();
                tx_idr.Text = idr;
                jalaoc("tx_idr");
            }*/
        }
        private void dgv_guias_FilterStringChanged(object sender, EventArgs e)                              // filtro EGRESOS
        {
            DataTable dtg = (DataTable)dgv_guias.DataSource;
            dtg.DefaultView.RowFilter = dgv_guias.FilterString;
            suma_grilla("dgv_guias");
        }
        private void dgv_guias_SortStringChanged(object sender, EventArgs e)                                // sort Egresos
        {
            DataTable dtg = (DataTable)dgv_guias.DataSource;
            dtg.DefaultView.Sort = dgv_guias.SortString;
        }
        private void dgv_plan_FilterStringChanged(object sender, EventArgs e)                               // filtro INGRESOS VARIOS
        {
            DataTable dtg = (DataTable)dgv_plan.DataSource;
            dtg.DefaultView.RowFilter = dgv_plan.FilterString;
            suma_grilla("dgv_plan");
        }
        private void dgv_plan_SortStringChanged(object sender, EventArgs e)                                 // sort ingresos varios
        {
            DataTable dtg = (DataTable)dgv_plan.DataSource;
            dtg.DefaultView.Sort = dgv_plan.SortString;
        }
        private void dgv_pend_FilterStringChanged(object sender, EventArgs e)                               // filtro PENDIENTES
        {
            DataTable dtg = (DataTable)dgv_pend.DataSource;
            dtg.DefaultView.RowFilter = dgv_pend.FilterString;
            suma_grilla("dgv_pend");
        }
        private void dgv_pend_SortStringChanged(object sender, EventArgs e)                                 // sort PENDIENTES
        {
            DataTable dtg = (DataTable)dgv_pend.DataSource;
            dtg.DefaultView.Sort = dgv_pend.SortString;
        }
        private void advancedDataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) // no usamos
        {
            /*if (e.RowIndex > -1 && e.ColumnIndex > 0 
                && advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != e.FormattedValue.ToString())
            {
                string campo = advancedDataGridView1.Columns[e.ColumnIndex].Name.ToString();
                string[] noeta = equivinter(advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString());    // retorna la tabla segun el titulo de la columna

                var aaa = MessageBox.Show("Confirma que desea cambiar el valor?",
                    "Columna: " + advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString(),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aaa == DialogResult.Yes)
                {
                    if(advancedDataGridView1.Columns[e.ColumnIndex].Tag.ToString() == "validaSI")   // la columna se valida?
                    {
                        // valida si el dato ingresado es valido en la columna
                        if (lib.validac(noeta[0], noeta[1], e.FormattedValue.ToString()) == true)
                        {
                            // llama a libreria con los datos para el update - tabla,id,campo,nuevo valor
                            lib.actuac(nomtab, campo, e.FormattedValue.ToString(),advancedDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                        }
                        else
                        {
                            MessageBox.Show("El valor no es válido para la columna", "Atención - Corrija");
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        // llama a libreria con los datos para el update - tabla,id,campo,nuevo valor
                        lib.actuac(nomtab, campo, e.FormattedValue.ToString(), advancedDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }*/
        }
        #endregion
    }
}
