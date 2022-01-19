using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;

namespace TransCarga
{
    public partial class repsalmac : Form
    {
        static string nomform = "repsalmac";           // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "cabalmac";            // 

        #region variables
        string asd = TransCarga.Program.vg_user;      // usuario conectado al sistema
        public int totfilgrid, cta;             // variables para impresion
        public string perAg = "";
        public string perMo = "";
        public string perAn = "";
        public string perIm = "";
        string codfact = "";
        string coddni = "";
        string codruc = "";
        string codmon = "";
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
        string img_preview = "";        // imagen del boton preview e imprimir reporte
        string cliente = Program.cliente;    // razon social para los reportes
        string codAnul = "";            // codigo de documento anulado
        string nomAnul = "";            // texto nombre del estado anulado
        string codGene = "";            // codigo documento nuevo generado
        //int pageCount = 1, cuenta = 0;
        #endregion

        libreria lib = new libreria();

        DataTable dtesalm = new DataTable();        // estados de almacen
        DataTable dtestad = new DataTable();        // estados del servicio
        DataTable dttaller = new DataTable();       // locales
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        public repsalmac()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)    // F1
        {
            // en este form no usamos
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void repsalmac_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
        }
        private void repsalmac_Load(object sender, EventArgs e)
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
            //
        }
        private void init()
        {
            tabControl1.BackColor = Color.FromName(TransCarga.Program.colgri);
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            //
            dgv_stock.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_dspachs.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            //Bt_ver.Image = Image.FromFile(img_btV);
            Bt_print.Image = Image.FromFile(img_btP);
            Bt_close.Image = Image.FromFile(img_btq);
            bt_exc.Image = Image.FromFile(img_btexc);
            Bt_close.Image = Image.FromFile(img_btq);
            // 
        }
        private void jalainfo()                                     // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in(@nofo,@clie)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@clie","clients");
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
                    if (row["formulario"].ToString() == "clients")
                    {
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "dni") coddni = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "ruc") codruc = row["valor"].ToString().Trim();
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
                dataller.Fill(dttaller);
                // PANEL STOCK
                cmb_sede_stk.DataSource = dttaller;
                cmb_sede_stk.DisplayMember = "descrizionerid";
                cmb_sede_stk.ValueMember = "idcodice";
                // PANEL DESPACHOS
                cmb_sede_desp.DataSource = dttaller;
                cmb_sede_desp.DisplayMember = "descrizionerid";
                cmb_sede_desp.ValueMember = "idcodice";
                // ingresos almacen
                cmb_ingalm.DataSource = dttaller;
                cmb_ingalm.DisplayMember = "descrizionerid";
                cmb_ingalm.ValueMember = "idcodice";

                // ***************** seleccion de estado de servicios
                string conestad = "select descrizionerid,idcodice,codigo from desc_est " +
                                       "where numero=1 order by idcodice";
                cmd = new MySqlCommand(conestad, conn);
                MySqlDataAdapter daestad = new MySqlDataAdapter(cmd);
                daestad.Fill(dtestad);
                // PANEL DESPACHOS
                cmb_estad_desp.DataSource = dtestad;
                cmb_estad_desp.DisplayMember = "descrizionerid";
                cmb_estad_desp.ValueMember = "idcodice";

                // **************** seleccion estados de almacen
                string conesalm = "select descrizionerid,idcodice,codigo from desc_eal " +
                                       "where numero=1 order by idcodice";
                cmd = new MySqlCommand(conesalm, conn);
                MySqlDataAdapter daesalm = new MySqlDataAdapter(cmd);
                daesalm.Fill(dtesalm);
                // PANEL STOCK
                cmb_estad_stk.DataSource = dtesalm;
                cmb_estad_stk.DisplayMember = "descrizionerid";
                cmb_estad_stk.ValueMember = "idcodice";
            }
            conn.Close();
        }
        private void grilla(string dgv)                             // 
        {
            Font tiplg = new Font("Arial", 7, FontStyle.Bold);
            int b;
            switch (dgv)
            {
                case "dgv_stock":
                    dgv_stock.Font = tiplg;
                    dgv_stock.DefaultCellStyle.Font = tiplg;
                    dgv_stock.RowTemplate.Height = 15;
                    dgv_stock.AllowUserToAddRows = false;
                    dgv_stock.Width = Parent.Width - 70; // 1015;
                    if (dgv_stock.DataSource == null) dgv_stock.ColumnCount = 11;
                    if (dgv_stock.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_stock.Columns.Count; i++)
                        {
                            dgv_stock.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_stock.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_stock.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_stock.Columns.Count; i++)
                        {
                            int a = dgv_stock.Columns[i].Width;
                            b += a;
                            dgv_stock.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_stock.Columns[i].Width = a;
                        }
                        if (b < dgv_stock.Width) dgv_stock.Width = b - 20;  // b + 60;
                        dgv_stock.ReadOnly = true;
                    }
                    suma_grilla("dgv_stock");
                    break;
                case "dgv_dspachs":
                    dgv_dspachs.Font = tiplg;
                    dgv_dspachs.DefaultCellStyle.Font = tiplg;
                    dgv_dspachs.RowTemplate.Height = 15;
                    dgv_dspachs.AllowUserToAddRows = false;
                    dgv_stock.Width = Parent.Width - 70; // 1015;
                    if (dgv_dspachs.DataSource == null) dgv_dspachs.ColumnCount = 11;
                    if (dgv_dspachs.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_dspachs.Columns.Count; i++)
                        {
                            dgv_dspachs.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_dspachs.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_dspachs.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_dspachs.Columns.Count; i++)
                        {
                            int a = dgv_dspachs.Columns[i].Width;
                            b += a;
                            dgv_dspachs.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_dspachs.Columns[i].Width = a;
                        }
                        if (b < dgv_dspachs.Width) dgv_dspachs.Width = b - 20;    // b + 60 ;
                        dgv_dspachs.ReadOnly = true;
                    }
                    suma_grilla("dgv_dspachs");
                    break;
                case "dgv_claves":
                    dgv_claves.Font = tiplg;
                    dgv_claves.DefaultCellStyle.Font = tiplg;
                    dgv_claves.RowTemplate.Height = 15;
                    dgv_claves.AllowUserToAddRows = false;
                    dgv_claves.Width = Parent.Width - 70; // 1015;
                    if (dgv_claves.DataSource == null) dgv_claves.ColumnCount = 11;
                    if (dgv_claves.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_claves.Columns.Count; i++)
                        {
                            dgv_claves.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_claves.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_claves.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_claves.Columns.Count; i++)
                        {
                            int a = dgv_claves.Columns[i].Width;
                            b += a;
                            dgv_claves.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_claves.Columns[i].Width = a;
                        }
                        if (b < dgv_claves.Width) dgv_claves.Width = b - 20;    // b + 60 ;
                        dgv_claves.ReadOnly = true;
                    }
                    suma_grilla("dgv_claves");
                    break;
                case "dgv_ingre":
                    dgv_ingre.Font = tiplg;
                    dgv_ingre.DefaultCellStyle.Font = tiplg;
                    dgv_ingre.RowTemplate.Height = 15;
                    dgv_ingre.AllowUserToAddRows = false;
                    dgv_ingre.Width = Parent.Width - 70; // 1015;
                    if (dgv_ingre.DataSource == null) dgv_ingre.ColumnCount = 11;
                    if (dgv_ingre.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_ingre.Columns.Count; i++)
                        {
                            dgv_ingre.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_ingre.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_ingre.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_ingre.Columns.Count; i++)
                        {
                            int a = dgv_ingre.Columns[i].Width;
                            b += a;
                            dgv_ingre.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_ingre.Columns[i].Width = a;
                        }
                        if (b < dgv_ingre.Width) dgv_ingre.Width = b - 20;    // b + 60 ;
                        dgv_ingre.ReadOnly = true;
                    }
                    break;
            }
        }
        private void bt_guias_Click(object sender, EventArgs e)         // genera reporte STOCK
        {
            if (tx_sede_stk.Text.Trim() == "")
            {
                MessageBox.Show("Debe seleccionar un almacén","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
                cmb_sede_stk.Focus();
                return;
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_alm_stock";
                using (MySqlCommand micon = new MySqlCommand(consulta,conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@loca", (tx_sede_stk.Text != "") ? tx_sede_stk.Text : "");
                    //micon.Parameters.AddWithValue("@fecini", dtp_ini_stk.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fin_stk.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", (tx_estad_stk.Text != "") ? tx_estad_stk.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_excl_stk.Checked == true) ? "1" : "0");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_stock.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_stock.DataSource = dt;
                        grilla("dgv_stock");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_plan_Click(object sender, EventArgs e)          // genera reporte DESPACHOS
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_alm_salidas";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@loca", (tx_dat_sede_desp.Text != "") ? tx_dat_sede_desp.Text : "");
                    micon.Parameters.AddWithValue("@fecini", dtp_fini_desp.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fter_desp.Value.ToString("yyyy-MM-dd"));
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_dspachs.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_dspachs.DataSource = dt;
                        grilla("dgv_dspachs");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_claves_Click(object sender, EventArgs e)        // genera historico de use de claves de seguridad
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_alm_claves1";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@fecini", dtp_ini_claves.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fin_claves.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@loca", (tx_dat_sede_claves.Text != "") ? tx_dat_sede_claves.Text : "");
                    micon.Parameters.AddWithValue("@esta", (tx_dat_estad_claves.Text != "") ? tx_dat_estad_claves.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_exclu_claves.Checked == true) ? "1" : "0");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_claves.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_claves.DataSource = dt;
                        grilla("dgv_claves");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_ingre_Click(object sender, EventArgs e)
        {
            if (tx_dat_seding.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione un almacén","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            string coning = "rep_alm_ingresos";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    using (MySqlCommand micon = new MySqlCommand(coning,conn))
                    {
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@loca", tx_dat_seding.Text);
                        micon.Parameters.AddWithValue("@fecini", dtp_fini_ing.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fecfin", dtp_fina_ing.Value.ToString("yyyy-MM-dd"));
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            {
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                dgv_ingre.DataSource = null;
                                dgv_ingre.DataSource = dt;
                                grilla("dgv_ingre");
                                //dt.Dispose();
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
        }
        private void suma_grilla(string dgv)
        {
            DataRow[] row = dtestad.Select("idcodice='" + codAnul + "'");   // dtestad
            string etiq_anulado = row[0].ItemArray[0].ToString();
            int cr = 0, ca = 0; // dgv_facts.Rows.Count;
            double tvv = 0, tva = 0;
            switch (dgv)
            {
                case "dgv_stock":
                    for (int i=0; i < dgv_stock.Rows.Count; i++)
                    {
                        cr = cr + 1;
                        tvv = tvv + Convert.ToDouble(dgv_stock.Rows[i].Cells["CANT_B"].Value);
                        tva = tva + Convert.ToDouble(dgv_stock.Rows[i].Cells["PESO"].Value);
                    }
                    tx_tfi_f.Text = cr.ToString();
                    tx_totval.Text = tvv.ToString("#0");
                    tx_totkgs.Text = tva.ToString("#0.00");
                    break;
                case "dgv_dspachs":
                    for (int i = 0; i < dgv_dspachs.Rows.Count; i++)
                    {
                        cr = cr + 1;
                        ca = ca + 1;
                    }
                    tx_tfi_n.Text = cr.ToString();
                    tx_totval_n.Text = tvv.ToString("#0.00");
                    break;
            }
        }

        #region combos
        private void cmb_sede_plan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_sede_desp.SelectedValue != null) tx_dat_sede_desp.Text = cmb_sede_desp.SelectedValue.ToString();
            else tx_dat_sede_desp.Text = "";
        }
        private void cmb_sede_plan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_desp.SelectedIndex = -1;
                tx_dat_sede_desp.Text = "";
            }
        }
        private void cmb_estad_plan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_estad_desp.SelectedValue != null) tx_dat_estad_desp.Text = cmb_estad_desp.SelectedValue.ToString();
            else tx_dat_estad_desp.Text = "";
        }
        private void cmb_estad_plan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad_desp.SelectedIndex = -1;
                tx_dat_estad_desp.Text = "";
            }
        }
        private void cmb_sede_guias_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_sede_stk.SelectedValue != null) tx_sede_stk.Text = cmb_sede_stk.SelectedValue.ToString();
            else tx_sede_stk.Text = "";
        }
        private void cmb_sede_guias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_stk.SelectedIndex = -1;
                tx_sede_stk.Text = "";
            }
        }
        private void cmb_estad_guias_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_estad_stk.SelectedValue != null) tx_estad_stk.Text = cmb_estad_stk.SelectedValue.ToString();
            else tx_estad_stk.Text = "";
        }
        private void cmb_estad_guias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad_stk.SelectedIndex = -1;
                tx_estad_stk.Text = "";
            }
        }
        private void cmb_ingalm_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_ingalm.SelectedValue != null) tx_dat_seding.Text = cmb_ingalm.SelectedValue.ToString();
            else tx_dat_seding.Text = "";
        }
        private void cmb_ingalm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_ingalm.SelectedIndex = -1;
                tx_dat_seding.Text = "";
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
            //
            cmb_sede_stk.SelectedIndex = -1;
            cmb_estad_stk.SelectedIndex = -1;
            cmb_ingalm.SelectedIndex = -1;
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            // nothing to do
        }
        private void bt_exc_Click(object sender, EventArgs e)
        {
            // segun la pestanha activa debe exportar
            string nombre = "";
            if (tabControl1.Enabled == false) return;
            if (tabControl1.SelectedTab == tabstock && dgv_stock.Rows.Count > 0)
            {
                nombre = "Reportes_stock_" + cmb_sede_stk.Text.Trim() +"_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_stock.DataSource;
                    wb.Worksheets.Add(dt, "Almacén");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabdspachs && dgv_dspachs.Rows.Count > 0)
            {
                nombre = "Reportes_Despachos_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_dspachs.DataSource;
                    wb.Worksheets.Add(dt, "Despachos");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
        }
        #endregion

        #region crystal
        private void button2_Click(object sender, EventArgs e)      // resumen de contrato
        {
            setParaCrystal("resumen");
        }
        private void button4_Click(object sender, EventArgs e)      // reporte de ventas
        {
            //if (rb_listado.Checked == true) setParaCrystal("vtasxclte");
            //else setParaCrystal("ventas");
        }

        private void setParaCrystal(string repo)                    // genera el set para el reporte de crystal
        {
            if (repo== "resumen")
            {
                //conClie datos = generareporte();                        // conClie = dataset de impresion de contrato   
                //frmvizcont visualizador = new frmvizcont(datos);        // POR ESO SE CREO ESTE FORM frmvizcont PARA MOSTRAR AHI. ES MEJOR ASI.  
                //visualizador.Show();
            }
            if (repo == "ventas")
            {
                //conClie datos = generarepvtas();
                //frmvizcont visualizador = new frmvizcont(datos);
                //visualizador.Show();
            }
            if (repo == "vtasxclte")
            {
                //conClie datos = generarepvtasxclte();
                //frmvizoper visualizador = new frmvizoper(datos);
                //visualizador.Show();
            }
        }
        private conClie generareporte()
        {
            conClie rescont = new conClie();                                    // dataset
            /*
            conClie.rescont_cabRow rowcabeza = rescont.rescont_cab.Newrescont_cabRow();
            
            rowcabeza.id = "0";
            rowcabeza.contrato = tx_codped.Text;
            rowcabeza.doccli = tx_docu.Text;
            rowcabeza.nomcli = tx_cliente.Text.Trim();
            rowcabeza.estado = tx_estad.Text;
            rowcabeza.fecha = tx_fecha.Text;
            rowcabeza.tienda = tx_tiend.Text;
            rowcabeza.valor = tx_valor.Text;
            rowcabeza.fent = tx_fent.Text;
            rescont.rescont_cab.Addrescont_cabRow(rowcabeza);
            // detalle
            foreach(DataGridViewRow row in dgv_resumen.Rows)
            {
                if (row.Cells["codigo"].Value != null && row.Cells["codigo"].Value.ToString().Trim() != "")
                {
                    conClie.rescont_detRow rowdetalle = rescont.rescont_det.Newrescont_detRow();
                    rowdetalle.id = row.Cells["id"].Value.ToString();
                    rowdetalle.codigo = row.Cells["codigo"].Value.ToString();
                    rowdetalle.nombre = row.Cells["nombre"].Value.ToString();
                    rowdetalle.madera = row.Cells["madera"].Value.ToString();
                    rowdetalle.cantC = row.Cells["CanC"].Value.ToString();
                    rowdetalle.sep_id = row.Cells["sep_id"].Value.ToString();
                    rowdetalle.sep_fecha = row.Cells["sep_fecha"].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.sep_almac = row.Cells["sep_almac"].Value.ToString();
                    rowdetalle.sep_cant = row.Cells["canS"].Value.ToString();
                    rowdetalle.ent_id = row.Cells["ent_id"].Value.ToString();
                    rowdetalle.ent_fecha = row.Cells["ent_fecha"].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.ent_cant = row.Cells["canE"].Value.ToString();
                    rowdetalle.tallerped = row.Cells["tallerped"].Value.ToString();
                    rowdetalle.ped_pedido = row.Cells["codped"].Value.ToString();
                    rowdetalle.ped_fecha = row.Cells["ped_fecha"].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.ped_cant = row.Cells["canP"].Value.ToString();
                    rowdetalle.ing_id = row.Cells["ing_id"].Value.ToString();
                    rowdetalle.ing_fecha = row.Cells["ing_fecha"].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.ing_cant = row.Cells["canI"].Value.ToString();
                    rowdetalle.sal_id = row.Cells["sal_id"].Value.ToString();
                    rowdetalle.sal_fecha = row.Cells["sal_fecha"].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.sal_cant = row.Cells["canA"].Value.ToString();
                    rescont.rescont_det.Addrescont_detRow(rowdetalle);
                }
            }
            */
            return rescont;
        }
        #endregion

        #region leaves y enter
        private void tabvtas_Enter(object sender, EventArgs e)
        {
            //cmb_vtasloc.Focus();
        }
        private void tabres_Enter(object sender, EventArgs e)
        {
            //cmb_tidoc.Focus();
        }
        #endregion

        #region advancedatagridview
        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "tabstock")
            {
                DataTable dtg = (DataTable)dgv_stock.DataSource;
                dtg.DefaultView.Sort = dgv_stock.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabdspachs")
            {
                DataTable dtg = (DataTable)dgv_dspachs.DataSource;
                dtg.DefaultView.Sort = dgv_dspachs.SortString;
            }
        }
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)                  // filtro de las columnas
        {
            if (tabControl1.SelectedTab.Name == "tabstock")
            {
                DataTable dtg = (DataTable)dgv_stock.DataSource;
                dtg.DefaultView.RowFilter = dgv_stock.FilterString;
                suma_grilla("dgv_facts");
            }
            if (tabControl1.SelectedTab.Name == "tabdspachs")
            {
                DataTable dtg = (DataTable)dgv_dspachs.DataSource;
                dtg.DefaultView.RowFilter = dgv_dspachs.FilterString;
                suma_grilla("dgv_notcre");
            }
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

        private void cmb_estad_desp_SelectedIndexChanged(object sender, EventArgs e)
        {

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
