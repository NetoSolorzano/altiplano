using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using CrystalDecisions.CrystalReports.Engine;

namespace TransCarga
{
    public partial class repsoper : Form
    {
        static string nomform = "repsoper";           // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "cabpregr";            // 
        
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
        string img_preview = "";        // imagen del boton preview e imprimir reporte
        string cliente = Program.cliente;    // razon social para los reportes
        string codAnul = "";            // codigo de documento anulado
        string nomAnul = "";            // texto nombre del estado anulado
        string codGene = "";            // codigo documento nuevo generado
        string rpt_placarga = "";       // ruta y nombre del formato RPT planillas carga
        string v_tipdocR = "";          // tipo de documento ruc
        string rpt_grt = "";            // ruta y nombre del formato RPT guias remit
        string v_CR_gr_simple = "";     // ruta y nombre formato TK guia simple
        int vi_copias = 1;               // cantidad de copias impresion
        string v_impTK = "";            // nombre de la impresora de TK para guias
        string v_CR_ctacte = "";        // ruta y nombre del formato CR para el reporte cta cte clientes
        //int pageCount = 1, cuenta = 0;
        #endregion

        libreria lib = new libreria();

        DataTable dt = new DataTable();
        DataTable dtestad = new DataTable();
        DataTable dttaller = new DataTable();
        DataTable dtplanCab = new DataTable();      // planilla de carga - cabecera
        DataTable dtplanDet = new DataTable();      // planilla de carga - detalle
        DataTable dtgrtcab = new DataTable();       // guia rem transpor - cabecera
        DataTable dtgrtdet = new DataTable();       // guia rem transpor - detalle
        string[] filaimp = {"","","","","","","","","","","","" };
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        public repsoper()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)    // F1
        {
            // en este form no usamos
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void repsoper_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
        }
        private void repsoper_Load(object sender, EventArgs e)
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
            tx_codped.CharacterCasing = CharacterCasing.Upper;
            tx_codped.TextAlign = HorizontalAlignment.Center;
        }
        private void init()
        {
            tabControl1.BackColor = Color.FromName(TransCarga.Program.colgri);
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            dgv_resumen.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dgv_resumen.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dgv_resumen.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dgv_resumen.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //
            dgv_vtas.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_guias.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_plan.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_reval.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //
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
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in(@nofo,@pla,@clie,@grt)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@pla", "planicarga");
                micon.Parameters.AddWithValue("@clie", "clients");
                micon.Parameters.AddWithValue("@grt", "guiati");
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
                            if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                            if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                            if (row["param"].ToString() == "img_btexc") img_btexc = row["valor"].ToString().Trim();     // imagen del boton exporta a excel
                            if (row["param"].ToString() == "img_btQ") img_btq = row["valor"].ToString().Trim();         // imagen del boton de accion SALIR
                            if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                            if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                            if (row["param"].ToString() == "img_imprime") img_imprime = row["valor"].ToString().Trim();  // imagen del boton IMPRIMIR REPORTE
                            if (row["param"].ToString() == "img_pre") img_preview = row["valor"].ToString().Trim();  // imagen del boton VISTA PRELIMINAR
                        }
                        if (row["campo"].ToString() == "estado")
                        {
                            if (row["param"].ToString() == "anulado") codAnul = row["valor"].ToString().Trim();         // codigo doc anulado
                            if (row["param"].ToString() == "generado") codGene = row["valor"].ToString().Trim();        // codigo doc generado
                            DataRow[] fila = dtestad.Select("idcodice='" + codAnul + "'");
                            nomAnul = fila[0][0].ToString();
                        }
                    }
                    if (row["formulario"].ToString() == "planicarga")
                    {
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "nomGRi_cr") rpt_placarga = row["valor"].ToString().Trim();         // ruta Y NOMBRE formato rpt
                    }
                    if (row["formulario"].ToString() == "guiati")
                    {
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "nomGRir_cr") rpt_grt = row["valor"].ToString().Trim();         // ruta y nombre formato rpt
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "GrT_simple_cr") v_CR_gr_simple = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "copias") vi_copias = int.Parse(row["valor"].ToString());
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "clients")
                    {
                        if (row["campo"].ToString() == "documento" && row["param"].ToString() == "ruc") v_tipdocR = row["valor"].ToString().Trim();         // tipo documento RUC
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "ctacte_cr") v_CR_ctacte = row["valor"].ToString().Trim(); // 
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
                // panel PRE GUIAS
                dataller.Fill(dttaller);
                cmb_vtasloc.DataSource = dttaller;
                cmb_vtasloc.DisplayMember = "descrizionerid";
                cmb_vtasloc.ValueMember = "idcodice";
                // PANEL GUIAS
                cmb_sede_guias.DataSource = dttaller;
                cmb_sede_guias.DisplayMember = "descrizionerid";
                cmb_sede_guias.ValueMember = "idcodice";
                // PANEL PLANILLA CARGA
                cmb_sede_plan.DataSource = dttaller;
                cmb_sede_plan.DisplayMember = "descrizionerid"; ;
                cmb_sede_plan.ValueMember = "idcodice";
                // ***************** seleccion de estado de servicios
                string conestad = "select descrizionerid,idcodice,codigo from desc_est " +
                                       "where numero=1 order by idcodice";
                cmd = new MySqlCommand(conestad, conn);
                MySqlDataAdapter daestad = new MySqlDataAdapter(cmd);
                daestad.Fill(dtestad);
                // PANEL GUIAS
                cmb_estad.DataSource = dtestad;
                cmb_estad.DisplayMember = "descrizionerid";
                cmb_estad.ValueMember = "idcodice";
                // PANEL GUIAS
                cmb_estad_guias.DataSource = dtestad;
                cmb_estad_guias.DisplayMember = "descrizionerid";
                cmb_estad_guias.ValueMember = "idcodice";
                // PANEL PLANILLA CARGA
                cmb_estad_plan.DataSource = dtestad;
                cmb_estad_plan.DisplayMember = "descrizionerid";
                cmb_estad_plan.ValueMember = "idcodice";
                // ***************** seleccion del tipo de documento cliente
                const string contidoc = "select descrizionerid,idcodice,codigo from desc_doc " +
                                       "where numero=1 order by idcodice";
                cmd = new MySqlCommand(contidoc, conn);
                MySqlDataAdapter datad = new MySqlDataAdapter(cmd);
                DataTable dttd = new DataTable();
                datad.Fill(dttd);
                cmb_tidoc.DataSource = dttd;
                cmb_tidoc.DisplayMember = "descrizionerid";
                cmb_tidoc.ValueMember = "idcodice";
                datad.Dispose();
                // **************** seleccion de placa 
                string conplac = "select placa from vehiculos order by placa asc";
                cmd = new MySqlCommand(conplac, conn);
                MySqlDataAdapter datpla = new MySqlDataAdapter(cmd);
                DataTable dtpla = new DataTable();
                datpla.Fill(dtpla);
                cmb_placa.DataSource = dtpla;
                cmb_placa.DisplayMember = "placa";
                cmb_placa.ValueMember = "placa";
                datpla.Dispose();
            }
            conn.Close();
        }
        private void grilla(string dgv)                             // 
        {
            Font tiplg = new Font("Arial", 7, FontStyle.Bold);
            int b;
            switch (dgv)
            {
                case "dgv_vtas":
                    dgv_vtas.Font = tiplg;
                    dgv_vtas.DefaultCellStyle.Font = tiplg;
                    dgv_vtas.RowTemplate.Height = 15;
                    //dgv_vtas.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    dgv_vtas.AllowUserToAddRows = false;
                    dgv_vtas.Width = this.Parent.Width - 50; // 1015;
                    if (dgv_vtas.DataSource == null) dgv_vtas.ColumnCount = 11;
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
                        if (b < dgv_vtas.Width) dgv_vtas.Width = b - 20;
                        dgv_vtas.ReadOnly = true;
                    }
                    break;
                case "dgv_guias":
                    dgv_guias.Font = tiplg;
                    dgv_guias.DefaultCellStyle.Font = tiplg;
                    dgv_guias.RowTemplate.Height = 15;
                    dgv_guias.AllowUserToAddRows = false;
                    dgv_guias.Width = Parent.Width - 50; // 1015;
                    //dgv_guias.AutoGenerateColumns = false;                              // aca
                    if (dgv_guias.DataSource == null) dgv_guias.ColumnCount = 11;
                    if (dgv_guias.Rows.Count > 0)
                    {
                        dgv_guias.Columns[0].Width = 30;
                        for (int i = 1; i < dgv_guias.Columns.Count; i++)
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
                    sumaGrilla("dgv_guias");
                    break;
                case "dgv_plan":
                    dgv_plan.Font = tiplg;
                    dgv_plan.DefaultCellStyle.Font = tiplg;
                    dgv_plan.RowTemplate.Height = 15;
                    dgv_plan.AllowUserToAddRows = false;
                    dgv_guias.Width = Parent.Width - 50; // 1015;
                    if (dgv_plan.DataSource == null) dgv_plan.ColumnCount = 11;
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
                    sumaGrilla("dgv_plan");
                    break;
                case "dgv_reval":
                    dgv_reval.Font = tiplg;
                    dgv_reval.DefaultCellStyle.Font = tiplg;
                    dgv_reval.RowTemplate.Height = 15;
                    dgv_reval.AllowUserToAddRows = false;
                    dgv_reval.Width = Parent.Width - 50; // 1015;
                    if (dgv_reval.DataSource == null) dgv_reval.ColumnCount = 11;
                    if (dgv_reval.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_reval.Columns.Count; i++)
                        {
                            dgv_reval.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_reval.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_reval.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_reval.Columns.Count; i++)
                        {
                            int a = dgv_reval.Columns[i].Width;
                            b += a;
                            dgv_reval.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_reval.Columns[i].Width = a;
                        }
                        if (b < dgv_reval.Width) dgv_reval.Width = b - 20;
                        dgv_reval.ReadOnly = true;
                    }
                    sumaGrilla("dgv_reval");
                    break;
                case "dgv_histGR":
                    dgv_histGR.Font = tiplg;
                    dgv_histGR.DefaultCellStyle.Font = tiplg;
                    dgv_histGR.RowTemplate.Height = 15;
                    dgv_histGR.AllowUserToAddRows = false;
                    dgv_histGR.Width = Parent.Width - 50; // 1015;
                    if (dgv_histGR.DataSource == null) dgv_histGR.ColumnCount = 8;
                    if (dgv_histGR.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_histGR.Columns.Count; i++)
                        {
                            dgv_histGR.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_histGR.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_histGR.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_histGR.Columns.Count; i++)
                        {
                            int a = dgv_histGR.Columns[i].Width;
                            b += a;
                            dgv_histGR.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_histGR.Columns[i].Width = a;
                        }
                        if (b < dgv_histGR.Width) dgv_histGR.Width = dgv_histGR.Width - 10;
                        dgv_histGR.ReadOnly = true;
                    }
                    break;
            }
        }
        private void grillares(string modo)                         // modo 0=todo,1=sin preguias
        {
            Font tiplg = new Font("Arial", 7, FontStyle.Bold);
            dgv_resumen.Font = tiplg;
            dgv_resumen.DefaultCellStyle.Font = tiplg;
            dgv_resumen.RowTemplate.Height = 15;
            dgv_resumen.DefaultCellStyle.BackColor = Color.MediumAquamarine;
            dgv_resumen.AllowUserToAddRows = false;
            //dgv_resumen.EnableHeadersVisualStyles = false;
            dgv_resumen.Width = Parent.Width - 50; // 1015;
            if (dgv_resumen.DataSource == null) dgv_resumen.ColumnCount = 11;
            for (int i = 0; i < dgv_resumen.Columns.Count; i++)
            {
                dgv_resumen.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (dgv_resumen.Rows.Count > 0)
                {
                    _ = decimal.TryParse(dgv_resumen.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                    if (vd != 0) dgv_resumen.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            int b = 0;
            for (int i = 0; i < dgv_resumen.Columns.Count; i++)
            {
                int a = dgv_resumen.Columns[i].Width;
                b += a;
                dgv_resumen.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv_resumen.Columns[i].Width = a;
            }
            if (b < dgv_resumen.Width) dgv_resumen.Width = b + 60;
            dgv_resumen.ReadOnly = true;
            //
            if (modo == "1")
            {
                for (int i=1;i<10;i++)
                {
                    dgv_resumen.Columns[i].Visible = false;
                }
            }
        }
        private void sumaGrilla(string grilla)
        {
            if (true)
            {
                DataRow[] row = dtestad.Select("idcodice='" + codAnul + "'");
                string etiq_anulado = row[0].ItemArray[0].ToString();
                int cr = 0, ca = 0, tgr = 0;
                double tvv = 0, tva = 0;
                switch (grilla)
                {
                    case "grillares":
                        if (tx_cliente.Text.Trim() != "")
                        {
                            //object sumPRE, sumGR, sumsaldos;
                            Decimal sumPRE = 0;
                            var sdf = dt.Compute("Sum(TOT_PRE)", "ESTADO <> '" + nomAnul + "' and TOT_GUIA = 0");
                            if (sdf.ToString() != "") sumPRE = decimal.Parse(sdf.ToString());   // string.Empty
                            Decimal sumGR = 0;
                            var spf = dt.Compute("Sum(TOT_GUIA)", "ESTADO <> '" + nomAnul + "' and TOT_PRE < TOT_GUIA");
                            if (spf != null && spf.ToString() != "") sumGR = decimal.Parse(spf.ToString());
                            Decimal sumsaldos = 0;
                            var ssf = dt.Compute("Sum(SALDO)", "ESTADO <> '" + nomAnul + "'").ToString();
                            if (ssf != null && ssf.ToString() != "") sumsaldos = decimal.Parse(ssf.ToString());
                            //
                            tx_valor.Text = (sumPRE + sumGR).ToString();
                            tx_pendien.Text = sumsaldos.ToString();
                            //tx_nser.Text = dt.Rows.Count.ToString();
                            tx_nser.Text = dt.Select("ESTADO <> '" + nomAnul + "'").Length.ToString();
                        }
                        break;
                    case "dgv_guias":
                        for (int i = 0; i < dgv_guias.Rows.Count; i++)
                        {
                            if (dgv_guias.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                            {
                                tvv = tvv + Convert.ToDouble(dgv_guias.Rows[i].Cells["FLETE_MN"].Value);
                                cr = cr + 1;
                            }
                            else
                            {
                                dgv_guias.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                ca = ca + 1;
                                tva = tva + Convert.ToDouble(dgv_guias.Rows[i].Cells["FLETE_MN"].Value);
                            }
                        }
                        tx_tfi_f.Text = cr.ToString();
                        tx_totval.Text = tvv.ToString("#0.00");
                        tx_tfi_a.Text = ca.ToString();
                        tx_totv_a.Text = tva.ToString("#0.00");
                        break;
                    case "dgv_plan":
                        for (int i = 0; i < dgv_plan.Rows.Count; i++)
                        {
                            if (dgv_plan.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                            {
                                tvv = tvv + Convert.ToDouble(dgv_plan.Rows[i].Cells["TOTAL"].Value);
                                tgr = tgr + Convert.ToInt32(dgv_plan.Rows[i].Cells["TGUIAS"].Value);
                                cr = cr + 1;
                            }
                            else
                            {
                                dgv_plan.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                ca = ca + 1;
                                tva = tva + Convert.ToDouble(dgv_plan.Rows[i].Cells["TOTAL"].Value);
                            }
                        }
                        tx_tfp_v.Text = cr.ToString();
                        tx_tflets.Text = tvv.ToString("#0.00");
                        tx_tgrp.Text = tgr.ToString();
                        tx_tfp_a.Text = ca.ToString();
                        break;
                    case "dgv_reval":
                        for (int i = 0; i < dgv_reval.Rows.Count; i++)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_reval.Rows[i].Cells["SAL_GR"].Value);
                            tgr = tgr + Convert.ToInt32(dgv_reval.Rows[i].Cells["NVO_SALDO"].Value);
                            cr = cr + 1;
                        }
                        tx_treval.Text = tgr.ToString("#0.00");
                        tx_trant.Text = tvv.ToString("#0.00");
                        tx_frv.Text = cr.ToString();
                        break;
                }
            }
        }
        private void bt_vtasfiltra_Click(object sender, EventArgs e)    // genera reporte pre guias
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_oper_pregr1";
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
        }
        private void tx_codped_Leave(object sender, EventArgs e)        // RESUMEN CLIENTE valida existencia de # documento
        {
            if(tx_codped.Text != "" && tx_dat_tido.Text != "")
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string consu = "select b.id,b.ruc,b.razonsocial,b.estado,b.tiposocio " +
                            "from anag_cli b " +
                            "where b.tipdoc=@td and ruc=@nd";
                        MySqlCommand micon = new MySqlCommand(consu, conn);
                        micon.Parameters.AddWithValue("@td", tx_dat_tido.Text);
                        micon.Parameters.AddWithValue("@nd", tx_codped.Text.Trim());
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.Read())
                        {
                            if(dr[0] == null)
                            {
                                MessageBox.Show("No existe el cliente", "Atención verifique", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                tx_codped.Text = "";
                                tx_docu.Text = "";
                                tx_cliente.Text = "";
                                tx_valor.Text = "";
                                tx_pendien.Text = "";
                                tx_nser.Text = "";
                                tx_codped.Focus();
                                dr.Close();
                                conn.Close();
                                return;
                            }
                            else
                            {
                                tx_cliente.Text = dr.GetString(2);
                                tx_docu.Text = dr.GetString(1);
                                dr.Close();
                            }
                        }
                        micon.Dispose();
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error de conectividad", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
            }
        }
        private void bt_resumen_Click(object sender, EventArgs e)       // genera resumen de cliente
        {
            if(tx_codped.Text.Trim() != "" && tx_dat_tido.Text != "")
            {
                tx_codped_Leave(null, null);
                dt.Clear();
                //dgv_resumen.Rows.Clear();
                //dgv_resumen.Columns.Clear();
                string consulta = "res_serv_clte";
                try
                {
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        dgv_resumen.DataSource = null;
                        MySqlCommand micon = new MySqlCommand(consulta, conn);
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@tido", tx_dat_tido.Text);
                        micon.Parameters.AddWithValue("@nudo", tx_codped.Text.Trim());
                        micon.Parameters.AddWithValue("@fecini", dtp_ser_fini.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fecfin", dtp_ser_fina.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@tope", (rb_total.Checked == true)? "T" : "P");      // T=todos || P=pendientes de cob
                        MySqlDataAdapter da = new MySqlDataAdapter(micon);
                        da.Fill(dt);
                        dgv_resumen.DataSource = dt;
                        dt.Dispose();
                        da.Dispose();
                        if (checkBox1.Checked == false) grillares("0");
                        else grillares("1");                            // 0=todo,1=sin preGuias
                    }
                    else
                    {
                        conn.Close();
                        MessageBox.Show("No se puede conectar al servidor", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en obtener datos");
                    Application.Exit();
                    return;
                }
            }
            else
            {
                tx_codped.Focus();
            }
            sumaGrilla("grillares");
        }
        private void bt_guias_Click(object sender, EventArgs e)         // genera reporte guias
        {
            if (rb_GR_dest.Checked == false && rb_GR_origen.Checked == false && cmb_sede_guias.SelectedIndex > -1)
            {
                MessageBox.Show("Seleccione origen o destino?","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
                rb_GR_origen.Focus();
                return;
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_oper_guiai1";
                using (MySqlCommand micon = new MySqlCommand(consulta,conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@loca", (tx_sede_guias.Text != "") ? tx_sede_guias.Text : "");
                    micon.Parameters.AddWithValue("@fecini", dtp_ini_guias.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fin_guias.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", (tx_estad_guias.Text != "") ? tx_estad_guias.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_excl_guias.Checked == true) ? "1" : "0");
                    micon.Parameters.AddWithValue("@orides", (rb_GR_origen.Checked == true) ? "O" : "D");   // local -> O=origen || D=destino
                    micon.Parameters.AddWithValue("@placa", cmb_placa.Text.Trim());
                    micon.Parameters.AddWithValue("@orden", (rb_remGR.Checked == true) ? "R" : (rb_desGR.Checked == true)? "D" : "G");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        //dgv_guias.Columns.Remove("chkc");
                        dgv_guias.DataSource = null;
                        dgv_guias.Rows.Clear();
                        dgv_guias.Columns.Clear();
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
        private void bt_plan_Click(object sender, EventArgs e)          // genera reporte planilla de carga
        {
            if (rb_PLA_dest.Checked == false && rb_PLA_origen.Checked == false && cmb_sede_plan.SelectedIndex > -1)
            {
                MessageBox.Show("Seleccione origen o destino?", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                rb_PLA_origen.Focus();
                return;
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_oper_plan1";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@fecini", dtp_fini_plan.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_fter_plan.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@loca", (tx_dat_sede_plan.Text != "") ? tx_dat_sede_plan.Text : "");
                    micon.Parameters.AddWithValue("@esta", (tx_dat_estad_plan.Text != "") ? tx_dat_estad_plan.Text : "");
                    micon.Parameters.AddWithValue("@excl", (chk_exclu_plan.Checked == true)? "1" : "0");
                    micon.Parameters.AddWithValue("@orides", (rb_PLA_origen.Checked == true) ? "O" : "D");   // local -> O=origen || D=destino
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
        private void bt_reval_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_oper_reval1";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@fecini", dtp_rev_fecini.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_rev_fecfin.Value.ToString("yyyy-MM-dd"));
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_reval.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_reval.DataSource = dt;
                        grilla("dgv_reval");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void bt_hisGR_Click(object sender, EventArgs e)         // historial de GR
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_oper_histGR";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.CommandType = CommandType.StoredProcedure;
                    micon.Parameters.AddWithValue("@ser", tx_ser.Text);
                    micon.Parameters.AddWithValue("@num", tx_num.Text);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_histGR.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_histGR.DataSource = dt;
                        grilla("dgv_histGR");
                    }
                    string resulta = lib.ult_mov(nomform, nomtab, asd);
                    if (resulta != "OK")                                        // actualizamos la tabla usuarios
                    {
                        MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void muestra_gr(string ser, string cor)                 // muestra la grt 
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    string consulta = "select a.id,a.fechopegr,a.sergui,a.numgui,a.numpregui,a.tidodegri,a.nudodegri,a.nombdegri,a.diredegri," +
                        "a.ubigdegri,a.tidoregri,a.nudoregri,a.nombregri,a.direregri,a.ubigregri,lo.descrizionerid as ORIGEN,a.dirorigen,a.ubiorigen," +
                        "ld.descrizionerid as DESTINO,a.dirdestin,a.ubidestin,a.docsremit,a.obspregri,a.clifingri,a.cantotgri,a.pestotgri," +
                        "a.tipmongri,a.tipcamgri,a.subtotgri,a.igvgri,round(a.totgri,1) as totgri,a.totpag,a.salgri,s.descrizionerid as ESTADO,a.impreso," +
                        "a.frase1,a.frase2,a.fleteimp,a.tipintrem,a.tipintdes,a.tippagpre,a.seguroE,a.userc,a.userm,a.usera," +
                        "a.serplagri,a.numplagri,a.plaplagri,a.carplagri,a.autplagri,a.confvegri,a.breplagri,a.proplagri," +
                        "ifnull(b.chocamcar,'') as chocamcar,ifnull(b.fecplacar,'') as fecplacar,ifnull(b.fecdocvta,'') as fecdocvta,ifnull(f.descrizionerid,'') as tipdocvta," +
                        "ifnull(b.serdocvta,'') as serdocvta,ifnull(b.numdocvta,'') as numdocvta,ifnull(b.codmonvta,'') as codmonvta," +
                        "ifnull(b.totdocvta,0) as totdocvta,ifnull(b.codmonpag,'') as codmonpag,ifnull(b.totpagado,0) as totpagado,ifnull(b.saldofina,0) as saldofina," +
                        "ifnull(b.feculpago,'') as feculpago,ifnull(b.estadoser,'') as estadoser,ifnull(c.razonsocial,'') as razonsocial,a.grinumaut," +
                        "ifnull(d.marca,'') as marca,ifnull(d.modelo,'') as modelo,ifnull(r.marca,'') as marCarret,ifnull(r.autor1,'') as autCarret," +
                        "a.teleregri as telrem,a.teledegri as teldes,ifnull(t.nombclt,'') as clifact," +
                        "u1.nombre AS distrem,u2.nombre as provrem,u3.nombre as deptrem,v1.nombre as distdes,v2.nombre as provdes,v3.nombre as deptdes,mo.descrizionerid as MON " +
                        "from cabguiai a " +
                        "left join controlg b on b.serguitra=a.sergui and b.numguitra=a.numgui " +
                        "left join desc_tdv f on f.idcodice=b.tipdocvta " +
                        "left join cabfactu t on t.tipdvta=a.tipdocvta and t.serdvta=a.serdocvta and t.numdvta=a.numdocvta " +
                        "left join anag_for c on c.ruc=a.proplagri and c.tipdoc=@tdep " +
                        "left join vehiculos d on d.placa=a.plaplagri " +
                        "left join vehiculos r on r.placa=a.carplagri " +
                        "left join anag_cli er on er.ruc=a.nudoregri and er.tipdoc=a.tidoregri " +
                        "left join anag_cli ed on ed.ruc=a.nudodegri and ed.tipdoc=a.tidodegri " +
                        "left join desc_est s on s.idcodice=a.estadoser " +
                        "left join desc_loc lo on lo.idcodice=a.locorigen " +
                        "left join desc_loc ld on ld.idcodice=a.locdestin " +
                        "left join desc_mon mo on mo.idcodice=a.tipmongri " +
                        "LEFT JOIN ubigeos u1 ON CONCAT(u1.depart, u1.provin, u1.distri)= a.ubigregri " +
                        "LEFT JOIN(SELECT* FROM ubigeos WHERE depart<>'00' AND provin<>'00' AND distri = '00') u2 ON u2.depart = left(a.ubigregri, 2) AND u2.provin = concat(substr(a.ubigregri, 3, 2)) " +
                        "LEFT JOIN (SELECT* FROM ubigeos WHERE depart<>'00' AND provin='00' AND distri = '00') u3 ON u3.depart = left(a.ubigregri, 2) " +
                        "LEFT JOIN ubigeos v1 ON CONCAT(v1.depart, v1.provin, v1.distri)= a.ubigdegri " +
                        "LEFT JOIN (SELECT* FROM ubigeos WHERE depart<>'00' AND provin<>'00' AND distri = '00') v2 ON v2.depart = left(a.ubigdegri, 2) AND v2.provin = concat(substr(a.ubigdegri, 3, 2)) " +
                        "LEFT JOIN (SELECT* FROM ubigeos WHERE depart<>'00' AND provin='00' AND distri = '00') v3 ON v3.depart = left(a.ubigdegri, 2) " +
                        "where a.sergui = @ser and a.numgui = @num";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@ser", ser);
                        micon.Parameters.AddWithValue("@num", cor);
                        micon.Parameters.AddWithValue("@tdep", v_tipdocR);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            dtgrtcab.Clear();
                            da.Fill(dtgrtcab);
                        }
                    }
                    consulta = "select id,sergui,numgui,cantprodi,unimedpro,codiprodi,descprodi,round(pesoprodi,1),precprodi,totaprodi " +
                        "from detguiai where sergui = @ser and numgui = @num";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@ser", ser);
                        micon.Parameters.AddWithValue("@num", cor);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            dtgrtdet.Clear();
                            da.Fill(dtgrtdet);
                        }
                    }
                }
                // llenamos el set
                if (tabControl1.SelectedTab.Name == "tabgrti")
                {
                    setParaCrystal("GRT");
                }
                if (tabControl1.SelectedTab.Name == "tabgrhist")
                {
                    if (rb_simple.Checked == true)
                    {
                        setParaCrystal("GrGrupal");
                    }
                    else
                    {
                        setParaCrystal("GRT");
                    }
                }
            }
        }
        private void bt_dale_Click(object sender, EventArgs e)          // impresion GRUPAL de guias
        {
            if (rb_imSimp.Checked == false && rb_imComp.Checked == false)
            {
                MessageBox.Show("Seleccione formato","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
                rb_imSimp.Focus();
                return;
            }
            if (rb_imSimp.Checked == true)      // formato simple de la GR (TK)
            {
                foreach (DataGridViewRow row in dgv_guias.Rows)
                {
                    if (row.Cells[0].EditedFormattedValue.ToString() == "True")
                    {
                        filaimp[0] = row.Cells["SER"].Value.ToString();    // serie
                        filaimp[1] = row.Cells["NUMERO"].Value.ToString();    // correl
                        filaimp[2] = row.Cells["FECHA"].Value.ToString().Substring(0,10);    // fecha
                        filaimp[3] = row.Cells["NOMBRE"].Value.ToString();    // cliente destin
                        filaimp[4] = row.Cells["DIRDEST"].Value.ToString();    // direccion
                        filaimp[5] = row.Cells["DESTINAT"].Value.ToString();    // dni - ruc
                        filaimp[6] = row.Cells["ORIGEN"].Value.ToString() + " - " + row.Cells["DESTINO"].Value.ToString();    // ruta (origen - destino)
                        filaimp[7] = row.Cells["PLACA"].Value.ToString();    // placa
                        filaimp[8] = row.Cells["CANTIDAD"].Value.ToString() + "  " + row.Cells["U_MEDID"].Value.ToString() + "  " + row.Cells["PESO"].Value.ToString() + " Kgs.";    // detalle fila 1 - cant bulto peso
                        filaimp[9] = row.Cells["DETALLE"].Value.ToString();    // detalle fila 2 - detalle
                        filaimp[10] = "Según doc. cliente" + " " + row.Cells["DOCSREMIT"].Value.ToString();   // detalle fila 3
                        filaimp[11] = "S/ " + row.Cells["FLETE_MN"].Value.ToString();   // flete soles
                        for (int i = 1; i <= vi_copias; i++)
                        {
                            printDocument1.PrinterSettings.PrinterName = v_impTK;
                            printDocument1.PrinterSettings.Copies = 2;
                            printDocument1.Print();
                        }
                    }
                }
            }
            if (rb_imComp.Checked == true)      // formato completo de la GR (2 x A4)
            {


            }
            chk_impGrp.Checked = false;
        }
        private void button6_Click(object sender, EventArgs e)          // vista previa de guias completa o simple
        {
            if (rb_complet.Checked == true)
            {
                if (tx_ser.Text.Trim() != "" && tx_num.Text.Trim() != "")
                {
                    muestra_gr(tx_ser.Text, tx_num.Text);
                }
                else
                {
                    tx_ser.Focus();
                    return;
                }
            }
            else
            {
                if (rb_simple.Checked == true)
                {
                    if (tx_ser.Text.Trim() != "" && tx_num.Text.Trim() != "")
                    {
                        muestra_gr(tx_ser.Text, tx_num.Text);

                    }
                    else
                    {
                        tx_ser.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un tipo de impresion de guía","Atención - seleccione",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    return;
                }
            }
        }

        #region combos
        private void cmb_estad_ing_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_estad.SelectedValue != null) tx_dat_estad.Text = cmb_estad.SelectedValue.ToString();
            else
            {
                tx_dat_estad.Text = "";    // cmb_estad.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
                chk_excluye.Checked = false;
            }
        }
        private void cmb_vtasloc_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_vtasloc.SelectedValue != null) tx_dat_vtasloc.Text = cmb_vtasloc.SelectedValue.ToString();
            else tx_dat_vtasloc.Text = ""; // cmb_vtasloc.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_estad_ing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad.SelectedIndex = -1;
                tx_dat_estad.Text = "";
            }
        }
        private void cmb_vtasloc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_vtasloc.SelectedIndex = -1;
                tx_dat_vtasloc.Text = "";
            }
        }
        private void cmb_tidoc_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_tidoc.SelectedValue != null) tx_dat_tido.Text = cmb_tidoc.SelectedValue.ToString();
            else tx_dat_tido.Text = "";
        }
        private void cmb_sede_plan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_sede_plan.SelectedValue != null) tx_dat_sede_plan.Text = cmb_sede_plan.SelectedValue.ToString();
            else tx_dat_sede_plan.Text = "";
        }
        private void cmb_sede_plan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_plan.SelectedIndex = -1;
                tx_dat_sede_plan.Text = "";
            }
        }
        private void cmb_estad_plan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_estad_plan.SelectedValue != null) tx_dat_estad_plan.Text = cmb_estad_plan.SelectedValue.ToString();
            else tx_dat_estad_plan.Text = "";
        }
        private void cmb_estad_plan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad_plan.SelectedIndex = -1;
                tx_dat_estad_plan.Text = "";
            }
        }
        private void cmb_sede_guias_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_sede_guias.SelectedValue != null) tx_sede_guias.Text = cmb_sede_guias.SelectedValue.ToString();
            else tx_sede_guias.Text = "";
        }
        private void cmb_sede_guias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sede_guias.SelectedIndex = -1;
                tx_sede_guias.Text = "";
            }
        }
        private void cmb_estad_guias_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_estad_guias.SelectedValue != null) tx_estad_guias.Text = cmb_estad_guias.SelectedValue.ToString();
            else tx_estad_guias.Text = "";
        }
        private void cmb_estad_guias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_estad_guias.SelectedIndex = -1;
                tx_estad_guias.Text = "";
            }
        }
        private void cmb_placa_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // tranquiiilo ... 
        }
        private void cmb_placa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_placa.SelectedIndex = -1;
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
            cmb_estad.SelectedIndex = -1;
            cmb_vtasloc.SelectedIndex = -1;
            cmb_tidoc.SelectedIndex = -1;
            chk_excluye.Checked = false;
            //
            cmb_sede_guias.SelectedIndex = -1;
            cmb_estad_guias.SelectedIndex = -1;
            cmb_placa.SelectedIndex = -1;
            //
            rb_imComp.Visible = false;
            rb_imSimp.Visible = false;
            bt_dale.Visible = false;
            //
            checkBox1.Checked = true;
            rb_total.Checked = true;
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
            if (tabControl1.SelectedTab == tabres && dgv_resumen.Rows.Count > 0)        // resumen de cliente
            {
                nombre = "resumen_cliente_" + tx_codped.Text.Trim() +"_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_resumen.DataSource;
                    wb.Worksheets.Add(dt, "Resumen");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabvtas && dgv_vtas.Rows.Count > 0)          // pre guias
            {
                nombre = "Reportes_PreGuias_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_vtas.DataSource;
                    wb.Worksheets.Add(dt, "PreGuias");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabgrti && dgv_guias.Rows.Count > 0)         // guias remision transportista
            {
                nombre = "Reportes_GuiasTransportista_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_guias.DataSource;
                    wb.Worksheets.Add(dt, "GuiasTransp");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabplacar && dgv_plan.Rows.Count > 0)        // planilla de carga
            {
                nombre = "Reportes_PlanillasCarga_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_plan.DataSource;
                    wb.Worksheets.Add(dt, "PlanillasC");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabreval && dgv_reval.Rows.Count > 0)        // revalorizaciones
            {
                nombre = "Reportes_Revalorizaciones_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_reval.DataSource;
                    wb.Worksheets.Add(dt, "Revalorizaciones");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabgrhist && dgv_histGR.Rows.Count > 0)      // seguimiento por guía
            {
                nombre = "Seguimiento_GuiasTransp_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_histGR.DataSource;
                    wb.Worksheets.Add(dt, "Seguimiento");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
        }
        #endregion

        #region crystal
        private void button2_Click(object sender, EventArgs e)      // 
        {
            setParaCrystal("resumen");
        }
        private void button4_Click(object sender, EventArgs e)      // 
        {
            if (rb_listado.Checked == true) setParaCrystal("vtasxclte");
            else setParaCrystal("ventas");
        }
        private void setParaCrystal(string repo)                    // genera el set para el reporte de crystal
        {
            if (repo == "GrGrupal")
            {
                conClie data = generareporte();
                frmvizoper visualizador = new frmvizoper(data);
                visualizador.Show();
            }
            if (repo == "GRT")
            {
                conClie datos = generarepgrt();
                frmvizoper visualizador = new frmvizoper(datos);
                visualizador.Show();
            }
            if (repo == "planC")
            {
                conClie datos = generarepplanC();
                frmvizoper visualizador = new frmvizoper(datos);
                visualizador.Show();
            }
            if (repo == "resumen")
            {
                conClie datos = generarepctacte();
                frmvizoper visualizador = new frmvizoper(datos);
                visualizador.Show();
            }
        }
        private conClie generarepplanC()
        {
            conClie PlaniC = new conClie();
            // CABECERA
            conClie.placar_cabRow rowcabeza = PlaniC.placar_cab.Newplacar_cabRow();
            rowcabeza.formatoRPT = rpt_placarga; // "formatos/plancarga2.rpt";
            rowcabeza.rucEmisor = Program.ruc;
            rowcabeza.nomEmisor = Program.cliente;
            rowcabeza.dirEmisor = Program.dirfisc;
            rowcabeza.id = dtplanCab.Rows[0].ItemArray[0].ToString();
            rowcabeza.autoriz = dtplanCab.Rows[0].ItemArray[22].ToString();
            rowcabeza.brevAyudante = dtplanCab.Rows[0].ItemArray[26].ToString();
            rowcabeza.brevChofer = dtplanCab.Rows[0].ItemArray[24].ToString();
            rowcabeza.camion = dtplanCab.Rows[0].ItemArray[21].ToString();            // placa de la carreta
            rowcabeza.confvehi = dtplanCab.Rows[0].ItemArray[23].ToString();
            rowcabeza.direDest = "";
            rowcabeza.direOrigen = "";
            rowcabeza.fechope = dtplanCab.Rows[0].ItemArray[1].ToString();
            rowcabeza.marcaModelo = "";
            rowcabeza.nomAyudante = dtplanCab.Rows[0].ItemArray[27].ToString();
            rowcabeza.nomChofer = dtplanCab.Rows[0].ItemArray[25].ToString();
            rowcabeza.nomDest = dtplanCab.Rows[0].ItemArray[37].ToString();
            rowcabeza.nomOrigen = dtplanCab.Rows[0].ItemArray[36].ToString();
            rowcabeza.nomPropiet = dtplanCab.Rows[0].ItemArray[33].ToString();
            rowcabeza.numpla = dtplanCab.Rows[0].ItemArray[3].ToString();
            rowcabeza.placa = dtplanCab.Rows[0].ItemArray[20].ToString();
            rowcabeza.rucPropiet = dtplanCab.Rows[0].ItemArray[28].ToString();
            rowcabeza.serpla = dtplanCab.Rows[0].ItemArray[2].ToString();
            rowcabeza.fechSalida = "";
            rowcabeza.fechLlegada = "";
            rowcabeza.estado = dtplanCab.Rows[0].ItemArray[38].ToString();
            rowcabeza.tituloF = Program.tituloF;
            PlaniC.placar_cab.Addplacar_cabRow(rowcabeza);
            // DETALLE  
            // if (rb_orden_gr.Checked == true) dataGridView1.Sort(dataGridView1.Columns["numguia"], System.ComponentModel.ListSortDirection.Ascending);
            // if (rb_orden_dir.Checked == true) dataGridView1.Sort(dataGridView1.Columns[14], System.ComponentModel.ListSortDirection.Ascending);
            int i = 0;
            foreach (DataRow row in dtplanDet.Rows)
            {
                if (row.ItemArray[0] != null)
                {
                    i = i + 1;
                    conClie.placar_detRow rowdetalle = PlaniC.placar_det.Newplacar_detRow();
                    rowdetalle.fila = i.ToString();
                    rowdetalle.id = row.ItemArray[0].ToString();
                    rowdetalle.idc = "";
                    rowdetalle.moneda = row.ItemArray[9].ToString();
                    rowdetalle.numguia = row.ItemArray[6].ToString();
                    rowdetalle.pagado = double.Parse(row.ItemArray[15].ToString());
                    rowdetalle.salxcob = double.Parse(row.ItemArray[16].ToString());
                    rowdetalle.serguia = row.ItemArray[5].ToString();
                    rowdetalle.totcant = Int16.Parse(row.ItemArray[7].ToString());
                    rowdetalle.totflete = Double.Parse(row.ItemArray[10].ToString());
                    rowdetalle.totpeso = int.Parse(row.ItemArray[8].ToString());
                    rowdetalle.nomdest = row.ItemArray[17].ToString();
                    rowdetalle.dirdest = row.ItemArray[18].ToString();
                    rowdetalle.teldest = row.ItemArray[19].ToString();
                    rowdetalle.nombulto = row.ItemArray[20].ToString();
                    rowdetalle.nomremi = "";
                    rowdetalle.docvta = row.ItemArray[23].ToString();
                    PlaniC.placar_det.Addplacar_detRow(rowdetalle);
                }
            }
            //
            return PlaniC;
        }
        private conClie generarepgrt()
        {
            conClie guiaT = new conClie();
            conClie.gr_ind_cabRow rowcabeza = guiaT.gr_ind_cab.Newgr_ind_cabRow();
            // CABECERA
            DataRow row = dtgrtcab.Rows[0];
            rowcabeza.formatoRPT = rpt_grt;
            rowcabeza.id = row["id"].ToString(); // tx_idr.Text;
            rowcabeza.estadoser = row["ESTADO"].ToString(); // tx_estado.Text;
            rowcabeza.sergui = row["sergui"].ToString(); // tx_serie.Text;
            rowcabeza.numgui = row["numgui"].ToString(); // tx_numero.Text;
            rowcabeza.numpregui = row["numpregui"].ToString(); // tx_pregr_num.Text;
            rowcabeza.fechope = row["fechopegr"].ToString().Substring(0, 10); // tx_fechope.Text;
            if (row["fecplacar"].ToString() == "") rowcabeza.fechTraslado = "";
            else rowcabeza.fechTraslado = row["fecplacar"].ToString().Substring(8,2) + "/" + row["fecplacar"].ToString().Substring(5, 2) + "/" + row["fecplacar"].ToString().Substring(0, 4); // tx_pla_fech.Text;
            rowcabeza.frase1 = row["ESTADO"].ToString(); //(tx_dat_estad.Text == codAnul) ? v_fra1 : "";  // campo para etiqueta "ANULADO"
            rowcabeza.frase2 = row["frase2"].ToString(); // (chk_seguridad.Checked == true) ? v_fra2 : "";  // campo para etiqueta "TIENE CLAVE"
            // origen - destino
            rowcabeza.nomDestino = row["DESTINO"].ToString(); // cmb_destino.Text;
            rowcabeza.direDestino = row["dirdestin"].ToString(); // tx_dirDestino.Text;
            rowcabeza.dptoDestino = ""; // 
            rowcabeza.provDestino = "";
            rowcabeza.distDestino = ""; // 
            rowcabeza.nomOrigen = row["ORIGEN"].ToString(); // cmb_origen.Text;
            rowcabeza.direOrigen = row["dirorigen"].ToString(); // tx_dirOrigen.Text;
            rowcabeza.dptoOrigen = "";  // no hay campo
            rowcabeza.provOrigen = "";
            rowcabeza.distOrigen = "";  // no hay campo
            // remitente
            rowcabeza.docRemit = "";    // cmb_docRem.Text;
            rowcabeza.numRemit = row["nudoregri"].ToString();    // tx_numDocRem.Text;
            rowcabeza.nomRemit = row["nombregri"].ToString();    // tx_nomRem.Text;
            rowcabeza.direRemit = row["direregri"].ToString();    // tx_dirRem.Text;
            rowcabeza.dptoRemit = row["deptrem"].ToString();   // row[""].ToString();    // tx_dptoRtt.Text;
            rowcabeza.provRemit = row["provrem"].ToString();    // tx_provRtt.Text;
            rowcabeza.distRemit = row["distrem"].ToString();    // tx_distRtt.Text;
            rowcabeza.telremit = row["telrem"].ToString();    // tx_telR.Text;
            // destinatario  
            rowcabeza.docDestinat = ""; // cmb_docDes.Text;
            rowcabeza.numDestinat = row["nudodegri"].ToString(); // tx_numDocDes.Text;
            rowcabeza.nomDestinat = row["nombdegri"].ToString(); // tx_nomDrio.Text;
            rowcabeza.direDestinat = row["diredegri"].ToString(); // tx_dirDrio.Text;
            rowcabeza.distDestinat = row["distdes"].ToString(); // tx_disDrio.Text;
            rowcabeza.provDestinat = row["provdes"].ToString(); // tx_proDrio.Text;
            rowcabeza.dptoDestinat = row["deptdes"].ToString(); // tx_dptoDrio.Text;
            rowcabeza.teldesti = row["teldes"].ToString(); // tx_telD.Text;
            // importes 
            rowcabeza.nomMoneda = row["MON"].ToString(); // cmb_mon.Text;
            rowcabeza.igv = row["igvgri"].ToString();         // no hay campo
            rowcabeza.subtotal = row["subtotgri"].ToString();    // no hay campo
            rowcabeza.total = row["totgri"].ToString(); // (chk_flete.Checked == true) ? tx_flete.Text : "";
            rowcabeza.docscarga = row["docsremit"].ToString(); // tx_docsOr.Text;
            rowcabeza.consignat = row["clifingri"].ToString(); // tx_consig.Text;
            // pie
            rowcabeza.marcamodelo = row["marca"].ToString();    // + " / " + row["modelo"].ToString(); // tx_marcamion.Text;
            rowcabeza.autoriz = row["autplagri"].ToString(); // tx_pla_autor.Text;
            rowcabeza.brevAyuda = "";   // falta este campo
            rowcabeza.brevChofer = row["breplagri"].ToString(); // tx_pla_brevet.Text;
            rowcabeza.nomChofer = row["chocamcar"].ToString(); // tx_pla_nomcho.Text;
            rowcabeza.placa = row["plaplagri"].ToString(); // tx_pla_placa.Text;
            rowcabeza.camion = row["carplagri"].ToString(); // tx_pla_carret.Text;
            rowcabeza.confvehi = row["confvegri"].ToString(); // tx_pla_confv.Text;
            rowcabeza.marcaCarret = row["marCarret"].ToString(); // 
            rowcabeza.autorCarret = row["autCarret"].ToString();
            rowcabeza.rucPropiet = row["proplagri"].ToString(); // tx_pla_ruc.Text;
            rowcabeza.nomPropiet = row["razonsocial"].ToString(); // tx_pla_propiet.Text;
            rowcabeza.fechora_imp = DateTime.Now.ToString();    // fecha de la "reimpresion" en el preview, No de la impresion en papel .. ojo
            rowcabeza.userc = (row["usera"].ToString() != "")? row["usera"].ToString(): (row["userm"].ToString() != "")? row["userm"].ToString(): row["userc"].ToString();
            //
            guiaT.gr_ind_cab.Addgr_ind_cabRow(rowcabeza);
            //
            // DETALLE  
            for (int i = 0; i < dtgrtdet.Rows.Count; i++)
            {
                conClie.gr_ind_detRow rowdetalle = guiaT.gr_ind_det.Newgr_ind_detRow();
                rowdetalle.fila = "";       // no estamos usando
                rowdetalle.cant = dtgrtdet.Rows[0].ItemArray[3].ToString(); // dataGridView1.Rows[i].Cells[0].Value.ToString();
                rowdetalle.codigo = "";     // no estamos usando
                rowdetalle.umed = dtgrtdet.Rows[0].ItemArray[4].ToString(); // dataGridView1.Rows[i].Cells[1].Value.ToString();
                rowdetalle.descrip = dtgrtdet.Rows[0].ItemArray[6].ToString(); // dataGridView1.Rows[i].Cells[2].Value.ToString();
                rowdetalle.precio = "";     // no estamos usando
                rowdetalle.total = "";      // no estamos usando
                rowdetalle.peso = string.Format("{0:#0.0}", dtgrtdet.Rows[0].ItemArray[7].ToString());  // dataGridView1.Rows[i].Cells[3].Value.ToString() + "Kg."
                guiaT.gr_ind_det.Addgr_ind_detRow(rowdetalle);
            }
            //
            return guiaT;
        }
        private conClie generareporte()
        {
            /*
                a.id,a.fechopegr,a.sergui,a.numgui,a.numpregui,a.tidodegri,a.nudodegri,a.nombdegri,a.diredegri,
                a.ubigdegri,a.tidoregri,a.nudoregri,a.nombregri,a.direregri,a.ubigregri,ORIGEN,a.dirorigen,a.ubiorigen,
                DESTINO,a.dirdestin,a.ubidestin,a.docsremit,a.obspregri,a.clifingri,a.cantotgri,a.pestotgri,
                a.tipmongri,a.tipcamgri,a.subtotgri,a.igvgri,totgri,a.totpag,a.salgri,ESTADO,a.impreso,
                a.frase1,a.frase2,a.fleteimp,a.tipintrem,a.tipintdes,a.tippagpre,a.seguroE,a.userc,a.userm,a.usera,
                a.serplagri,a.numplagri,a.plaplagri,a.carplagri,a.autplagri,a.confvegri,a.breplagri,a.proplagri,
                chocamcar,fecplacar,fecdocvta,tipdocvta,serdocvta,numdocvta,codmonvta,totdocvta,
                codmonpag,totpagado,saldofina,feculpago,estadoser,razonsocial,a.grinumaut,marca,modelo,
                marCarret,autCarret,telrem,teldes,clifact,distrem,provrem,deptrem,distdes,provdes,deptdes,MON
            */
            conClie guiaT = new conClie();
            guiaT.Clear();
            conClie.gr_ind_cabRow rowcabeza = guiaT.gr_ind_cab.Newgr_ind_cabRow();
            // CABECERA
            //DataGridViewRow row = dgv_guias.Rows[rowi];
            DataRow row = dtgrtcab.Rows[0];    // Cabecera
            rowcabeza.formatoRPT = v_CR_gr_simple;
            rowcabeza.id = "0"; // tx_idr.Text;
            rowcabeza.estadoser = row.ItemArray[33].ToString();         // row.Cells["ESTADO"].Value.ToString();
            rowcabeza.sergui = row.ItemArray[2].ToString();             // row.Cells["SER"].Value.ToString(); // tx_serie.Text;
            rowcabeza.numgui = row.ItemArray[3].ToString();             // row.Cells["NUMERO"].Value.ToString();
            rowcabeza.numpregui = "";
            rowcabeza.fechope = row.ItemArray[1].ToString().Substring(0, 10);            // row.Cells["FECHA"].Value.ToString().Substring(0, 10); // tx_fechope.Text;
            rowcabeza.fechTraslado = "";
            rowcabeza.frase1 = "";
            rowcabeza.frase2 = "";
            // origen - destino
            rowcabeza.nomDestino = row.ItemArray[18].ToString();          // row.Cells["DESTINO"].Value.ToString();
            rowcabeza.direDestino = row.ItemArray[19].ToString();         // row.Cells["DIRDEST"].Value.ToString();
            rowcabeza.dptoDestino = ""; // 
            rowcabeza.provDestino = "";
            rowcabeza.distDestino = ""; // 
            rowcabeza.nomOrigen = row.ItemArray[15].ToString();          // row.Cells["ORIGEN"].Value.ToString();
            rowcabeza.direOrigen = "";
            rowcabeza.dptoOrigen = "";  // no hay campo
            rowcabeza.provOrigen = "";
            rowcabeza.distOrigen = "";  // no hay campo
            // remitente
            rowcabeza.docRemit = "";    // cmb_docRem.Text;
            rowcabeza.numRemit = row.ItemArray[11].ToString();          // row.Cells["REMITENTE"].Value.ToString();
            rowcabeza.nomRemit = row.ItemArray[12].ToString();          // row.Cells["NOMBRE2"].Value.ToString();
            rowcabeza.direRemit = "";
            rowcabeza.dptoRemit = "";
            rowcabeza.provRemit = "";
            rowcabeza.distRemit = "";
            rowcabeza.telremit = "";
            // destinatario  
            rowcabeza.docDestinat = "";
            rowcabeza.numDestinat = row.ItemArray[6].ToString();    // row.Cells["DESTINAT"].Value.ToString();
            rowcabeza.nomDestinat = row.ItemArray[7].ToString();    // row.Cells["NOMBRE"].Value.ToString();
            rowcabeza.direDestinat = "";
            rowcabeza.distDestinat = "";
            rowcabeza.provDestinat = "";
            rowcabeza.dptoDestinat = "";
            rowcabeza.teldesti = "";
            // importes 
            rowcabeza.nomMoneda = row.ItemArray[81].ToString();     // row.Cells["MON"].Value.ToString();
            rowcabeza.igv = "";
            rowcabeza.subtotal = "";
            rowcabeza.total = row.ItemArray[30].ToString();         // row.Cells["FLETE_GR"].Value.ToString();
            rowcabeza.docscarga = row.ItemArray[21].ToString();     // row.Cells["DOCSREMIT"].Value.ToString(); ;   // docs del remitente 
            rowcabeza.consignat = "";   // 
            // pie
            rowcabeza.marcamodelo = "";
            rowcabeza.autoriz = "";
            rowcabeza.brevAyuda = "";   // falta este campo
            rowcabeza.brevChofer = "";
            rowcabeza.nomChofer = "";
            rowcabeza.placa = row.ItemArray[47].ToString();         // row.Cells["PLACA"].Value.ToString();
            rowcabeza.camion = "";      // placa carreta
            rowcabeza.confvehi = "";
            rowcabeza.rucPropiet = "";
            rowcabeza.nomPropiet = "";
            rowcabeza.fechora_imp = "";
            rowcabeza.userc = "";
            //
            guiaT.gr_ind_cab.Addgr_ind_cabRow(rowcabeza);
            //
            // DETALLE  
            DataRow rowd = dtgrtdet.Rows[0];    // Detalle
            {
                conClie.gr_ind_detRow rowdetalle = guiaT.gr_ind_det.Newgr_ind_detRow();
                rowdetalle.fila = "";       // no estamos usando
                rowdetalle.cant = row.ItemArray[24].ToString();              // row.Cells["CANTIDAD"].Value.ToString();
                rowdetalle.codigo = "";     // no estamos usando
                rowdetalle.umed = rowd.ItemArray[4].ToString();              // row.Cells["U_MEDID"].Value.ToString();
                rowdetalle.descrip = rowd.ItemArray[6].ToString();           // row.Cells["DETALLE"].Value.ToString();
                rowdetalle.precio = "";     // no estamos usando
                rowdetalle.total = "";      // no estamos usando
                rowdetalle.peso = string.Format("{0:#0}", row.ItemArray[25].ToString());    // string.Format("{0:#0}", row.Cells["PESO"].Value.ToString());
                guiaT.gr_ind_det.Addgr_ind_detRow(rowdetalle);
            }
            return guiaT;
        }
        private conClie generarepctacte()
        {
            conClie ctacte = new conClie();

            conClie.ctacteclteRow rowcab = ctacte.ctacteclte.NewctacteclteRow();
            DataGridViewRow row = dgv_resumen.Rows[0];
            rowcab.formatoRPT = v_CR_ctacte;
            rowcab.rucEmisor = Program.ruc;
            rowcab.nomEmisor = Program.cliente;
            rowcab.dirEmisor = Program.dirfisc;
            rowcab.fecfin = dtp_ser_fini.Value.Date.ToString();
            rowcab.fecini = dtp_ser_fina.Value.Date.ToString();
            rowcab.id = "0";
            rowcab.nomcliente = tx_cliente.Text;
            rowcab.numdoc = tx_docu.Text;
            rowcab.tipdoc = cmb_tidoc.Text;
            rowcab.tot_pend = (rb_pend.Checked == true) ? "P" : "T";
            ctacte.ctacteclte.AddctacteclteRow(rowcab);
            //
            foreach (DataGridViewRow rowd in dgv_resumen.Rows)
            {
                conClie.detctacteRow rowdet = ctacte.detctacte.NewdetctacteRow();
                rowdet.id = "0";
                rowdet.estado = rowd.Cells["ESTADO"].Value.ToString();
                rowdet.fechgr = rowd.Cells["F_GUIA"].Value.ToString();
                rowdet.guia = rowd.Cells["GUIA"].Value.ToString();
                rowdet.mongr = rowd.Cells["MON"].Value.ToString();  // moneda GR
                rowdet.flete = double.Parse(rowd.Cells["TOT_GUIA"].Value.ToString());
                rowdet.origen = rowd.Cells["ORIGEN"].Value.ToString();
                rowdet.destino = rowd.Cells["DESTINO"].Value.ToString();
                rowdet.tdrem = rowd.Cells["TD_REM"].Value.ToString();  // tipo doc remiten
                rowdet.ndrem = rowd.Cells["ND_REM"].Value.ToString();
                rowdet.nomrem = rowd.Cells["REMITENTE"].Value.ToString();
                rowdet.tddes = rowd.Cells["TD_DES"].Value.ToString();  // tipo doc destinat
                rowdet.nddes = rowd.Cells["ND_DES"].Value.ToString();
                rowdet.nomdes = rowd.Cells["DESTINAT"].Value.ToString();
                rowdet.fecdv = rowd.Cells["F_VTA"].Value.ToString();
                rowdet.docvta = rowd.Cells["DOC_VTA"].Value.ToString();
                rowdet.monvta = rowd.Cells["MON_VTA"].Value.ToString();
                rowdet.totvta = double.Parse(rowd.Cells["TOT_VTA"].Value.ToString());
                rowdet.fecpag = rowd.Cells["F_PAGO"].Value.ToString();
                rowdet.nompag = rowd.Cells["MON_PAG"].Value.ToString(); // moneda pago
                rowdet.totpag = double.Parse(rowd.Cells["PAGADO"].Value.ToString());  // total pagos
                rowdet.saldo = double.Parse(rowd.Cells["SALDO"].Value.ToString());
                rowdet.fecpla = rowd.Cells["F_PAGO"].Value.ToString();
                rowdet.planilla = rowd.Cells["PLANILLA"].Value.ToString();
                rowdet.placa = rowd.Cells["PLACA"].Value.ToString();
                ctacte.detctacte.AdddetctacteRow(rowdet);
            }
            //
            return ctacte;
        }
        #endregion

        #region leaves y enter
        private void tabvtas_Enter(object sender, EventArgs e)
        {
            cmb_vtasloc.Focus();
        }
        private void tabres_Enter(object sender, EventArgs e)
        {
            cmb_tidoc.Focus();
        }
        private void tx_ser_Leave(object sender, EventArgs e)
        {
            tx_ser.Text = lib.Right("000" + tx_ser.Text, 4);
        }
        private void tx_num_Leave(object sender, EventArgs e)
        {
            tx_num.Text = lib.Right("0000000" + tx_num.Text, 8);
        }
        private void chk_impGrp_CheckStateChanged(object sender, EventArgs e)
        {
            if (chk_impGrp.CheckState == CheckState.Checked)
            {
                DataGridViewCheckBoxColumn chkc = new DataGridViewCheckBoxColumn();
                chkc.Name = "chkc";
                chkc.HeaderText = " ";
                chkc.Width = 30;
                chkc.ReadOnly = false;
                chkc.FillWeight = 10;
                dgv_guias.Columns.Insert(0, chkc);
                dgv_guias.Enabled = true;
                dgv_guias.ReadOnly = false;
                dgv_guias.Columns[0].ReadOnly = false;
                for (int i=1;i<dgv_guias.Columns.Count;i++)     // NO SALE EL CHECK, NO SE VE
                {
                    dgv_guias.Columns[i].ReadOnly = true;
                }
                for (int i=0;i<dgv_guias.Rows.Count;i++)
                {
                    dgv_guias.Rows[i].Cells[0].Value = true;
                }
                rb_imComp.Visible = true;
                rb_imSimp.Visible = true;
                bt_dale.Visible = true;
            }
            else
            {
                for (int i = 0; i < dgv_guias.Rows.Count; i++)
                {
                    dgv_guias.Rows[i].Cells[0].Value = false;
                }
                dgv_guias.Columns.Remove("chkc");
                rb_imComp.Visible = false;
                rb_imSimp.Visible = false;
                bt_dale.Visible = false;
                dgv_guias.ReadOnly = true;
            }
        }

        #endregion

        #region advancedatagridview
        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "tabres")
            {
                DataTable dtg = (DataTable)dgv_resumen.DataSource;
                dtg.DefaultView.Sort = dgv_resumen.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabgrti")
            {
                DataTable dtg = (DataTable)dgv_guias.DataSource;
                dtg.DefaultView.Sort = dgv_guias.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabvtas")
            {
                DataTable dtg = (DataTable)dgv_vtas.DataSource;
                dtg.DefaultView.Sort = dgv_vtas.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabplacar")
            {
                DataTable dtg = (DataTable)dgv_plan.DataSource;
                dtg.DefaultView.Sort = dgv_plan.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabreval")
            {
                DataTable dtg = (DataTable)dgv_reval.DataSource;
                dtg.DefaultView.Sort = dgv_reval.SortString;
            }
        }
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)                  // filtro de las columnas
        {
            if (tabControl1.SelectedTab.Name == "tabres")
            {
                DataTable dtg = (DataTable)dgv_resumen.DataSource;
                dtg.DefaultView.RowFilter = dgv_resumen.FilterString;
            }
            if (tabControl1.SelectedTab.Name == "tabvtas")
            {
                DataTable dtg = (DataTable)dgv_vtas.DataSource;
                dtg.DefaultView.RowFilter = dgv_vtas.FilterString;
            }
            if (tabControl1.SelectedTab.Name == "tabgrti")
            {
                DataTable dtg = (DataTable)dgv_guias.DataSource;
                dtg.DefaultView.RowFilter = dgv_guias.FilterString;
                sumaGrilla("dgv_guias");
            }
            if (tabControl1.SelectedTab.Name == "tabplacar")
            {
                DataTable dtg = (DataTable)dgv_plan.DataSource;
                dtg.DefaultView.RowFilter = dgv_plan.FilterString;
                sumaGrilla("dgv_plan");
            }
            if (tabControl1.SelectedTab.Name == "tabreval")
            {
                DataTable dtg = (DataTable)dgv_reval.DataSource;
                dtg.DefaultView.RowFilter = dgv_reval.FilterString;
                sumaGrilla("dgv_reval");
            }
        }
        private void advancedDataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)            // no usamos
        {
            //advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }
        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)      // no usamos
        {
            if (tabControl1.SelectedTab.Name == "tabres")
            {
                if (dgv_resumen.Columns[e.ColumnIndex].Name == "GUIA")
                {
                    string ser = dgv_resumen.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(0, 4);
                    string num = dgv_resumen.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Substring(5, 8);
                    muestra_gr(ser,num);
                }
            }
            if (tabControl1.SelectedTab.Name == "tabvtas")
            {
                
            }
            if (tabControl1.SelectedTab.Name == "tabgrti")
            {
                if (dgv_guias.Columns[0].Name.ToString() == "chkc")
                {
                    if (e.ColumnIndex == 2)
                    {
                        muestra_gr(dgv_guias.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString(), dgv_guias.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                }
                else
                {
                    if (e.ColumnIndex == 1)
                    {
                        muestra_gr(dgv_guias.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString(), dgv_guias.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    }
                }
            }
            if (tabControl1.SelectedTab.Name == "tabplacar")
            {
                if (e.ColumnIndex == 2)
                {
                    using (MySqlConnection con = new MySqlConnection(DB_CONN_STR))
                    {
                        if (lib.procConn(con) == true)
                        {
                            string consulta = "select a.id,a.fechope,a.serplacar,a.numplacar,a.locorigen,a.locdestin,a.obsplacar,a.cantfilas,a.cantotpla,a.pestotpla,a.tipmonpla," +
                                "a.tipcampla,a.subtotpla,a.igvplacar,a.totplacar,a.totpagado,a.salxpagar,a.estadoser,a.impreso,a.fleteimp,a.platracto,a.placarret,a.autorizac," +
                                "a.confvehic,a.brevchofe,a.nomchofe,a.brevayuda,a.nomayuda,a.rucpropie,a.tipoplani,a.userc,a.userm,a.usera,ifnull(b.razonsocial,'') as razonsocial," +
                                "a.marcaTrac,a.modeloTrac,c.descrizionerid as nomorigen,d.descrizionerid as nomdestin,e.descrizionerid as nomestad " +
                                "FROM cabplacar a left join anag_for b on a.rucpropie=b.ruc and b.estado=0 " +
                                "left join desc_loc c on c.idcodice=a.locorigen " +
                                "left join desc_loc d on d.idcodice=a.locdestin " +
                                "left join desc_est e on e.idcodice=a.estadoser " + 
                                "where a.serplacar=@ser and a.numplacar=@num";
                            using (MySqlCommand micon = new MySqlCommand(consulta, con))
                            {
                                micon.Parameters.AddWithValue("@ser", dgv_plan.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString());
                                micon.Parameters.AddWithValue("@num", dgv_plan.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                                using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                                {
                                    dtplanCab.Clear();
                                    da.Fill(dtplanCab);
                                }
                            }
                            // detalle
                            consulta = "select a.idc,a.serplacar,a.numplacar,a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,floor(a.totpeso) as totpeso,b.descrizionerid as MON,a.totflet," +
                                "a.estadoser,a.codmone,'X' as marca,a.id,a.pagado,a.salxcob,g.nombdegri,g.diredegri,g.teledegri,a.nombult,u1.nombre AS distrit," +
                                "u2.nombre as provin,concat(d.descrizionerid,'-',if(SUBSTRING(g.serdocvta,1,2)='00',SUBSTRING(g.serdocvta,3,2),g.serdocvta),'-',if(SUBSTRING(g.numdocvta,1,3)='000',SUBSTRING(g.numdocvta,4,5),g.numdocvta)) " +
                                "from detplacar a " +
                                "left join desc_mon b on b.idcodice = a.codmone " +
                                "left join cabguiai g on g.sergui = a.serguia and g.numgui = a.numguia " +
                                "left join desc_tdv d on d.idcodice=g.tipdocvta " +
                                "LEFT JOIN ubigeos u1 ON CONCAT(u1.depart, u1.provin, u1.distri)= g.ubigdegri " +
                                "LEFT JOIN(SELECT* FROM ubigeos WHERE depart<>'00' AND provin<>'00' AND distri = '00') u2 ON u2.depart = left(g.ubigdegri, 2) AND u2.provin = concat(substr(g.ubigdegri, 3, 2)) " +
                                "where a.serplacar=@ser and a.numplacar=@num";
                            using (MySqlCommand micon = new MySqlCommand(consulta, con))
                            {
                                micon.Parameters.AddWithValue("@ser", dgv_plan.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString());
                                micon.Parameters.AddWithValue("@num", dgv_plan.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                                using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                                {
                                    dtplanDet.Clear();
                                    da.Fill(dtplanDet);
                                }
                            }
                        }
                        // llenamos el set
                        setParaCrystal("planC");
                    }
                }
            }
            if (tabControl1.SelectedTab.Name == "tabreval")
            {
                
            }
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

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // TIPOS DE LETRA PARA EL DOCUMENTO FORMATO TICKET
            Font lt_gra = new Font("Arial", 13);                // grande
            Font lt_tit = new Font("Lucida Console", 10);       // mediano
            Font lt_med = new Font("Arial", 9);                // normal textos
            Font lt_peq = new Font("Arial", 8);                 // pequeño
            //
            float anchTik = 7.8F;                               // ancho del TK en centimetros
            int coli = 5;                                      // columna inicial
            int colm = 80;
            float posi = 20;                                    // posicion x,y inicial
            int alfi = 20;                                      // alto de cada fila
            float ancho = 360.0F;                                // ancho de la impresion
            {
                //lt = (ancho - e.Graphics.MeasureString(rasclie, lt_gra).Width) / 2;
                PointF puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("CONTROL", lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[0] + "-" + filaimp[1], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("FECHA", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[2], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("CLIENTE", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[3], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("DIRECC", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                SizeF cuad = new SizeF(CentimeterToPixel(anchTik) - (coli + 70), alfi * 2);
                RectangleF recdom = new RectangleF(puntoF, cuad);
                e.Graphics.DrawString(filaimp[4], lt_med, Brushes.Black, recdom, StringFormat.GenericTypographic);
                posi = posi + alfi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("DNI/RUC", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[5], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("RUTA", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[6], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("PLACA", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[7], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(filaimp[8], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(filaimp[9], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(filaimp[10], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                posi = posi + alfi;
                posi = posi + alfi;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("TOTAL", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[11], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi * 9;
                puntoF = new PointF(coli + 40, posi);
                e.Graphics.DrawString("---------------------------------", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi;
                puntoF = new PointF(coli + 40, posi);
                e.Graphics.DrawString("   RECIBI CONFORME", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                posi = posi + alfi * 2;
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString(".", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
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
    }
}
