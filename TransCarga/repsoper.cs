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
        string ruta_logo = "";          // ruta y nombre del logo
        string impriLogi = "";          // Imprime logo o no en el formato guia simple - notita
        string client_id_sunat = "";    // id del cliente api sunat para guias electrónicas 
        string client_pass_sunat = "";  // clave api sunat para guias electrónicas
        string u_sol_sunat = "";        // usuario sol sunat del cliente
        string c_sol_sunat = "";        // clave sol sunat del cliente
        string scope_sunat = "";        // scope sunat del api
        string cGR_sunat = "";          // codigo sunat para GR transportista
        string usa_gre = "";            // usa GRE en la organización? S/N
        string firmDocElec = "";        // Firma xml, true=firma, false=no firma
        string rutaCertifc = "";        // Ruta y nombre del certificado .pfx
        string claveCertif = "";        // Clave del certificado
        string rutatxt = "";            // ruta de los txt para las guías elect
        string rutaxml = "";            // ruta para los xml de las guías electrónicas
        string vtc_dni = "";            // codigo dni
        string vtc_ruc = "";            // codigo ruc
        string vtc_ext = "";            // codigo carne extranjería
        string despedid1 = "";          // despedida del ticket 1
        string despedid2 = "";          // despedida del ticket 2
        string glosa1 = "";             // glosa comprobante final 1
        string glosa2 = "";             // 

        string[] c_t = new string[6] { "", "", "", "", "", "" }; // parametros para generar el token
        //int pageCount = 1, cuenta = 0;
        #endregion

        libreria lib = new libreria();
        guiati_e _E = new guiati_e();               // instanciamos el form de guias para usar sus metodos
        DataTable dt = new DataTable();
        DataTable dtestad = new DataTable();
        DataTable dttaller = new DataTable();
        DataTable dtplanCab = new DataTable();      // planilla de carga - cabecera
        DataTable dtplanDet = new DataTable();      // planilla de carga - detalle
        DataTable dtgrtcab = new DataTable();       // guia rem transpor - cabecera
        DataTable dtgrtdet = new DataTable();       // guia rem transpor - detalle
        DataTable dtsunatE = new DataTable();       // guías transp elec - estados
        string[] filaimp = {"","","","","","","","","","","","","" };
        DataGridViewCheckBoxColumn chkc = new DataGridViewCheckBoxColumn()
        { 
            Name = "chck",
            HeaderText = " ",
            Width = 30,
            ReadOnly = false,
            FillWeight = 10
        };
        DataGridViewCheckBoxColumn chkGRE = new DataGridViewCheckBoxColumn()
        {
            Name = "chkGRE",
            HeaderText = " ",
            Width = 30,
            ReadOnly = false,
            FillWeight = 10
        };
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        public repsoper()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) 
        {
            string para1 = "";
            string para2 = "";
            string para3 = "";
            if (keyData == Keys.Enter && tx_cliente.Focused == true && tx_cliente.Text.Trim() != "")
            {
                para1 = "Clientes";
                para2 = tx_cliente.Text.Trim();
                para3 = "";
                ayuda3 ayu3 = new ayuda3(para1, para2, para3);
                var result = ayu3.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    tx_dat_tido.Text = ayu3.ReturnValueA[3];       // codigo tipo doc
                    tx_docu.Text = ayu3.ReturnValueA[3];       // codigo tipo doc
                    cmb_tidoc.Enabled = true;
                    cmb_tidoc.SelectedValue = ayu3.ReturnValue0;
                    tx_codped.Text = ayu3.ReturnValue1;         // nume doc
                    tx_cliente.Text = ayu3.ReturnValue2;       // nombre cliente
                    //
                    dtp_ser_fini.Focus();
                }
                return true;    // indicate that you handled this keystroke
            }
            // 
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
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in(@nofo,@pla,@clie,@grt,@nofi,@gret)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@pla", "planicarga");
                micon.Parameters.AddWithValue("@clie", "clients");
                micon.Parameters.AddWithValue("@grt", "guiati");
                micon.Parameters.AddWithValue("@nofi", nomform);
                micon.Parameters.AddWithValue("@gret", "guiati_e");
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
                            if (row["param"].ToString() == "img_pre") img_preview = row["valor"].ToString().Trim();     // imagen del boton VISTA PRELIMINAR
                            if (row["param"].ToString() == "logoPrin") ruta_logo = row["valor"].ToString().Trim();      // logo de la empresa a imprimir
                        }
                        if (row["campo"].ToString() == "estado")
                        {
                            if (row["param"].ToString() == "anulado") codAnul = row["valor"].ToString().Trim();         // codigo doc anulado
                            if (row["param"].ToString() == "generado") codGene = row["valor"].ToString().Trim();        // codigo doc generado
                            DataRow[] fila = dtestad.Select("idcodice='" + codAnul + "'");
                            nomAnul = fila[0][0].ToString();
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
                            if (row["param"].ToString() == "firmDocElec") firmDocElec = row["valor"].ToString().Trim();           // Firma xml, true=firma, false=no firma
                            if (row["param"].ToString() == "rutaCertifc") rutaCertifc = row["valor"].ToString().Trim();           // Ruta y nombre del certificado .pfx
                            if (row["param"].ToString() == "claveCertif") claveCertif = row["valor"].ToString().Trim();           // Clave del certificado
                        }
                        if (row["campo"].ToString() == "rutas")
                        {
                            if (row["param"].ToString() == "grt_txt") rutatxt = row["valor"].ToString().Trim();         // ruta de los txt para las guías elect
                            if (row["param"].ToString() == "grt_xml") rutaxml = row["valor"].ToString().Trim();         // ruta para los xml de las guías electrónicas
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
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "ruc") v_tipdocR = row["valor"].ToString().Trim();         // tipo documento RUC
                            if (row["param"].ToString() == "dni") vtc_dni = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ruc") vtc_ruc = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "ext") vtc_ext = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "ctacte_cr") v_CR_ctacte = row["valor"].ToString().Trim(); // 
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "impLogo")
                        {
                            if (row["param"].ToString() == "grSimple") impriLogi = row["valor"].ToString().Trim();         // SI= imprime logo | NO=no imprime logo
                        }
                    }
                    if (row["formulario"].ToString() == "guiati_e")
                    {
                        if (row["campo"].ToString() == "glosas")
                        {
                            if (row["param"].ToString() == "glosa1") glosa1 = row["valor"].ToString();          // glosa final del ticket 1
                            if (row["param"].ToString() == "glosa2") glosa2 = row["valor"].ToString();
                        }
                        if (row["campo"].ToString() == "despedida")
                        {
                            if (row["param"].ToString() == "desped1") despedid1 = row["valor"].ToString();          // glosa despedida del ticket 1
                            if (row["param"].ToString() == "desped2") despedid2 = row["valor"].ToString();

                        }
                        //if (row["campo"].ToString() == "impTK") v_impTK = row["valor"].ToString();
                    }
                }
                da.Dispose();
                dt.Dispose();
                conn.Close();
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
                    if (dgv_vtas.DataSource == null) dgv_vtas.ColumnCount = 11;
                    /*
                    dgv_vtas.Width = this.Parent.Width - 50; // 1015;
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
                    */
                    break;
                case "dgv_guias":
                    dgv_guias.Font = tiplg;
                    dgv_guias.DefaultCellStyle.Font = tiplg;
                    dgv_guias.RowTemplate.Height = 15;
                    dgv_guias.AllowUserToAddRows = false;
                    if (dgv_guias.DataSource == null) dgv_guias.ColumnCount = 11;
                    /*
                    dgv_guias.Width = Parent.Width - 50; // 1015;
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
                    */
                    sumaGrilla("dgv_guias");
                    break;
                case "dgv_plan":
                    dgv_plan.Font = tiplg;
                    dgv_plan.DefaultCellStyle.Font = tiplg;
                    dgv_plan.RowTemplate.Height = 15;
                    dgv_plan.AllowUserToAddRows = false;
                    if (dgv_plan.DataSource == null) dgv_plan.ColumnCount = 11;
                    /*
                    dgv_guias.Width = Parent.Width - 50; // 1015;
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
                    sumaGrilla("dgv_plan");
                    break;
                case "dgv_reval":
                    dgv_reval.Font = tiplg;
                    dgv_reval.DefaultCellStyle.Font = tiplg;
                    dgv_reval.RowTemplate.Height = 15;
                    dgv_reval.AllowUserToAddRows = false;
                    if (dgv_reval.DataSource == null) dgv_reval.ColumnCount = 11;
                    /*
                    dgv_reval.Width = Parent.Width - 50; // 1015;
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
                    */
                    sumaGrilla("dgv_reval");
                    break;
                case "dgv_histGR":
                    dgv_histGR.Font = tiplg;
                    dgv_histGR.DefaultCellStyle.Font = tiplg;
                    dgv_histGR.RowTemplate.Height = 15;
                    dgv_histGR.AllowUserToAddRows = false;
                    if (dgv_histGR.DataSource == null) dgv_histGR.ColumnCount = 8;
                    /*
                    dgv_histGR.Width = Parent.Width - 50; // 1015;
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
                    */
                    break;
                case "dgv_GRE_est":
                    dgv_GRE_est.Font = tiplg;
                    dgv_GRE_est.DefaultCellStyle.Font = tiplg;
                    dgv_GRE_est.RowTemplate.Height = 18;
                    dgv_GRE_est.AllowUserToAddRows = false;
                    if (dgv_GRE_est.DataSource == null) dgv_GRE_est.ColumnCount = 7;
                    dgv_GRE_est.Width = Parent.Width - 50; // 1015;

                    Padding padding = new Padding();
                    padding.Left = 16;
                    padding.Right = 16;
                    padding.Top = 0;
                    padding.Bottom = 0;

                    Font chiq = new Font("Arial",6,FontStyle.Bold);

                    DataGridViewButtonColumn btnTk = new DataGridViewButtonColumn();
                    btnTk.HeaderText = "iTK";
                    //btnTk.UseColumnTextForButtonValue = true;
                    btnTk.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnTk.Name = "iTK";
                    btnTk.Width = 60;
                    btnTk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnTk.DefaultCellStyle.Padding = padding;
                    btnTk.DefaultCellStyle.Font = chiq;
                    btnTk.DefaultCellStyle.SelectionBackColor = Color.White;

                    DataGridViewButtonColumn btnCDR = new DataGridViewButtonColumn();
                    btnCDR.HeaderText = "CDR";
                    btnCDR.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnCDR.Name = "cdr";
                    btnCDR.Width = 60;
                    btnCDR.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnCDR.DefaultCellStyle.Padding = padding;
                    btnCDR.DefaultCellStyle.Font = chiq;
                    btnCDR.DefaultCellStyle.SelectionBackColor = Color.White;

                    DataGridViewButtonColumn btnPDF = new DataGridViewButtonColumn();
                    btnPDF.HeaderText = "PDF";
                    btnPDF.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnPDF.Name = "pdf";
                    btnPDF.Width = 60;
                    btnPDF.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnPDF.DefaultCellStyle.Padding = padding;
                    btnPDF.DefaultCellStyle.Font = chiq;
                    btnPDF.DefaultCellStyle.SelectionBackColor = Color.White;

                    DataGridViewButtonColumn btnAct = new DataGridViewButtonColumn();
                    btnAct.HeaderText = "Sunat"; // ACTUALIZA
                    btnAct.Text = "...Actualiza...";
                    btnAct.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnAct.Name = "consulta";
                    btnAct.Width = 140;
                    btnAct.UseColumnTextForButtonValue = true;
                    btnAct.DefaultCellStyle.Padding = padding;

                    // EMISION,GUIA_ELEC,ORIGEN,DESTINO,ESTADO,SUNAT,CDR_GEN,.........,ad.cdr,ad.textoQR,ad.nticket,g.cantfilas
                    //     0        1       2      3       4     5      6     7 8 9 10    11      12         13        14
                    //dgv_GRE_est.CellPainting += grid_CellPainting;        // no funciona bien, no se adecua
                    dgv_GRE_est.CellClick += DataGridView1_CellClick;
                    dgv_GRE_est.Columns.Insert(7, btnTk);   
                    dgv_GRE_est.Columns.Insert(8, btnPDF);   // .Add(btnPDF);
                    dgv_GRE_est.Columns.Insert(9, btnCDR);   // .Add(btnCDR);
                    dgv_GRE_est.Columns.Insert(10, btnAct);   // .Add(btnAct);
                    dgv_GRE_est.Columns[11].Visible = false;
                    dgv_GRE_est.Columns[12].Visible = false;
                    dgv_GRE_est.Columns[13].Visible = false;
                    dgv_GRE_est.Columns[14].Visible = false;
                    if (dgv_GRE_est.Rows.Count > 0)         // autosize filas
                    {
                        for (int i = 0; i < dgv_GRE_est.Columns.Count - 8; i++)
                        {
                            dgv_GRE_est.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_GRE_est.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_GRE_est.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_GRE_est.Columns.Count - 8; i++)
                        {
                            int a = dgv_GRE_est.Columns[i].Width;
                            b += a;
                            dgv_GRE_est.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_GRE_est.Columns[i].Width = a;
                        }
                        if (b < dgv_GRE_est.Width) dgv_GRE_est.Width = dgv_GRE_est.Width - 10;
                        dgv_GRE_est.ReadOnly = true;
                    }
                    if (dgv_GRE_est.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_GRE_est.Rows.Count; i++)
                        {
                            dgv_GRE_est.Rows[i].Cells[7].Value = "TK";
                            if (dgv_GRE_est.Rows[i].Cells[6].Value != null)
                            {
                                if (dgv_GRE_est.Rows[i].Cells[6].Value.ToString() == "1")
                                {
                                    dgv_GRE_est.Rows[i].Cells[8].ReadOnly = false;
                                    dgv_GRE_est.Rows[i].Cells[8].Value = "PDF";
                                    dgv_GRE_est.Rows[i].Cells[9].ReadOnly = false;
                                    dgv_GRE_est.Rows[i].Cells[9].Value = "CDR";
                                    dgv_GRE_est.Rows[i].Cells[10].ReadOnly = true;
                                }
                                else
                                {
                                    dgv_GRE_est.Rows[i].Cells[8].ReadOnly = true;
                                    dgv_GRE_est.Rows[i].Cells[8].Value = "";
                                    dgv_GRE_est.Rows[i].Cells[9].ReadOnly = true;
                                    dgv_GRE_est.Rows[i].Cells[9].Value = "";
                                    dgv_GRE_est.Rows[i].Cells[10].ReadOnly = false;
                                }
                            }
                        }
                    }
                    sumaGrilla("dgv_GRE_est");
                    break;
            }
        }
        private string consultaE(string ticket, int rowIndex)       // consulta estado en Sunat
        {
            string retorna = "";

            if (ticket == "") return retorna;

            string token = _E.conex_token(c_t);
            string resCon = _E.consultaC(ticket, token);
            if (resCon == "Aceptado" || resCon == "Rechazado" || resCon == "Error")
            {
                // consultamos y actualizamos la grilla
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    //EMISION,GUIA_ELEC,ORIGEN,DESTINO,ESTADO,SUNAT,CDR_GEN,.........,ad.cdr,ad.textoQR,ad.nticket,g.cantfilas
                    //    0        1       2      3       4     5      6    7 8 9 10    11      12         13         14

                    string xxx = "select * from adiguias where nticket=@ntk";
                    using (MySqlCommand micon = new MySqlCommand(xxx, conn))
                    {
                        micon.Parameters.AddWithValue("@ntk", ticket);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (dr[8] != null)
                                {
                                    dgv_GRE_est.Rows[rowIndex].Cells[6].Value = dr.GetString(8);
                                    dgv_GRE_est.Rows[rowIndex].Cells[5].Value = dr.GetString(6);
                                    dgv_GRE_est.Rows[rowIndex].Cells[11].Value = dr.GetString(7);
                                    dgv_GRE_est.Rows[rowIndex].Cells[12].Value = dr.GetString(9);
                                }
                                else
                                {
                                    // nada, hubo un error interno o no responde sunat
                                }
                            }
                        }
                    }
                }
            }

            return retorna;
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
                            //
                            for (int i = 0; i < dgv_resumen.Rows.Count; i++)
                            {
                                if (dgv_resumen.Rows[i].Cells["ESTADO"].Value.ToString() == etiq_anulado)
                                {
                                    dgv_resumen.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                }
                            }
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
                    case "dgv_GRE_est":
                        for (int i=0; i < dgv_GRE_est.Rows.Count; i++)
                        {
                            if (dgv_GRE_est.Rows[i].Cells[4].Value.ToString() != etiq_anulado)
                            {
                                cr = cr + 1;
                            }
                            else
                            {
                                dgv_GRE_est.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                ca = ca + 1;
                            }
                        }
                        tx_GRE_fa.Text = ca.ToString();
                        tx_GRE_fv.Text = cr.ToString();
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
            if (dgv_resumen.Rows.Count > 0)
            {
                sumaGrilla("grillares");
            }
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
                if (("tabgrti,tabres").Contains(tabControl1.SelectedTab.Name))
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
                        filaimp[12] = row.Cells["NOMBRE2"].Value.ToString();           // nombre del remitente
                        for (int i = 1; i <= vi_copias; i++)
                        {
                            printDocument1.PrinterSettings.PrinterName = v_impTK;
                            printDocument1.PrinterSettings.Copies = 2;      // esto debería estar en una variable
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
        private void bt_greEst_Click(object sender, EventArgs e)        // Guías de Remisión Electrónicas - Estados
        {
            chk_GRE_imp.Checked = false;
            dtsunatE.Rows.Clear();
            dtsunatE.Columns.Clear();
            string consulta = "SELECT g.fechopegr AS EMISION,concat(g.sergui,'-',g.numgui) AS GUIA_ELEC,lo.descrizionerid AS ORIGEN,ld.DescrizioneRid AS DESTINO," +
                "es.DescrizioneRid AS ESTADO,ad.estadoS AS SUNAT,ad.cdrgener AS CDR_GEN,ad.cdr,ad.textoQR,ad.nticket,g.cantfilas " +
                "FROM cabguiai g LEFT JOIN adiguias ad ON ad.idg = g.id " +
                "LEFT JOIN desc_loc lo ON lo.IDCodice = g.locorigen " +
                "LEFT JOIN desc_loc ld ON ld.IDCodice = g.locdestin " +
                "LEFT JOIN desc_est es ON es.IDCodice = g.estadoser " +
                "WHERE marca_gre<>'' AND g.fechopegr between @fecini and @fecfin";
            string parte = "";
            if (tx_dat_GRE_sede.Text != "" && rb_GRE_orig.Checked == true) parte = parte + " and g.locorigen=@loca";
            if (tx_dat_GRE_sede.Text != "" && rb_GRE_dest.Checked == true) parte = parte + " and g.locdestin=@loca";
            if (tx_dat_GRE_est.Text != "") parte = parte + " and ad.estadoS=@esta";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR)                )
            {
                conn.Open();
                using (MySqlCommand micon = new MySqlCommand(consulta + parte, conn))
                {
                    micon.Parameters.AddWithValue("@loca", (tx_dat_GRE_sede.Text != "") ? tx_dat_GRE_sede.Text : "");
                    micon.Parameters.AddWithValue("@fecini", dtp_GRE_fini.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_GRE_fter.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", (tx_dat_GRE_est.Text != "") ? tx_dat_GRE_est.Text : "");
                    //micon.Parameters.AddWithValue("@excl", (chk_GRE.Checked == true) ? "1" : "0");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_GRE_est.DataSource = null;
                        dgv_GRE_est.Columns.Clear();
                        dgv_GRE_est.Rows.Clear();
                        //
                        da.Fill(dtsunatE);
                        dgv_GRE_est.DataSource = dtsunatE;
                        grilla("dgv_GRE_est");
                    }
                }
            }

        }
        private void bt_consMas_Click(object sender, EventArgs e)       // hace la consulta de todas las GRE de la grilla
        {
            dgv_GRE_est.Enabled = false;
            bt_consMas.Enabled = false;
            // hacemos las consultas
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                for (int i=0; i < dgv_GRE_est.Rows.Count; i++)
                {
                    if (dgv_GRE_est.Rows[i].Cells[5].Value.ToString() != "Aceptado" && dgv_GRE_est.Rows[i].Cells[5].Value.ToString() != "Rechazado" &&
                        (dgv_GRE_est.Rows[i].Cells[6].Value.ToString() == "0" || dgv_GRE_est.Rows[i].Cells[6].Value.ToString().Trim() == ""))
                    {
                        consultaE(dgv_GRE_est.Rows[i].Cells[12].Value.ToString(), i);
                    }
                }
            }
            // terminado todo ...
            dgv_GRE_est.Enabled = true;
            bt_consMas.Enabled = true;
        }
        private void marca_check(string etiqueta, CheckBox check)       // marca columna 0 de la grilla dgv_GRE_est
        {
            // EMISION,GUIA_ELEC,ORIGEN,DESTINO,ESTADO,SUNAT,CDR_GEN,.....,ad.cdr,ad.textoQR,ad.nticket
            //    0        1         2     3      4      5      6    7 8 9    10     11           12
            if (check.CheckState == CheckState.Checked)
            {
                for (int i = 0; i < dgv_GRE_est.Rows.Count; i++)
                {
                    if (dgv_GRE_est.Rows[i].Cells[6].Value.ToString() == etiqueta)
                    {
                        dgv_GRE_est.Rows[i].Cells[0].Value = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < dgv_GRE_est.Rows.Count; i++)
                {
                    if (dgv_GRE_est.Rows[i].Cells[6].Value.ToString() == etiqueta)
                    {
                        dgv_GRE_est.Rows[i].Cells[0].Value = false;
                    }
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
            //
            rb_busDoc.Checked = true;
            //
            chk_GRE_iAcep.Visible = false;
            chk_GRE_iEnpr.Visible = false;
            chk_GRE_iEnvia.Visible = false;
            bt_GRE_impri.Visible = false;
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
                rowdet.fechgr = rowd.Cells["F_GUIA"].Value.ToString().Substring(0,10);
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
                rowdet.docsclte = rowd.Cells["DOCSCLTE"].Value.ToString();
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
                dgv_guias.Columns.Insert(0, chkc);
                dgv_guias.Enabled = true;
                dgv_guias.ReadOnly = false;
                dgv_guias.Columns[0].ReadOnly = false;
                for (int i=1;i<dgv_guias.Columns.Count;i++)
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
                panel4.ForeColor = Color.FromArgb(32,178,170);
            }
            else
            {
                for (int i = 0; i < dgv_guias.Rows.Count; i++)
                {
                    dgv_guias.Rows[i].Cells[0].Value = false;
                }
                dgv_guias.Columns.Remove(chkc);
                rb_imComp.Visible = false;
                rb_imSimp.Visible = false;
                bt_dale.Visible = false;
                dgv_guias.ReadOnly = true;
                panel4.ForeColor = Color.FromArgb(255, 255, 255);
            }
        }
        private void rb_busDoc_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_busDoc.Checked == true)
            {
                tx_cliente.Text = "";
                tx_cliente.ReadOnly = true;

                cmb_tidoc.Enabled = true;
                tx_codped.ReadOnly = false;
            }
        }
        private void rb_busNom_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_busNom.Checked == true)
            {
                cmb_tidoc.SelectedIndex = -1;
                cmb_tidoc.Enabled = false;
                tx_dat_tido.Text = "";
                tx_codped.ReadOnly = true;
                tx_codped.Text = "";

                tx_cliente.ReadOnly = false;
            }
        }
        private void tx_cliente_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tx_cliente.Text.Trim() != "")
            {
                // nada
            }
        }
        private void chk_GRE_imp_CheckStateChanged(object sender, EventArgs e)
        {
            if (dgv_GRE_est.Rows.Count > 0)
            {
                if (chk_GRE_imp.CheckState == CheckState.Checked)
                {
                    dgv_GRE_est.Columns.Insert(0, chkGRE);
                    dgv_GRE_est.Enabled = true;
                    dgv_GRE_est.ReadOnly = false;
                    dgv_GRE_est.Columns[0].ReadOnly = false;
                    for (int i = 1; i < dgv_GRE_est.Columns.Count - 6; i++)
                    {
                        dgv_GRE_est.Columns[i].ReadOnly = true;
                    }
                    for (int i = 0; i < dgv_GRE_est.Rows.Count; i++)
                    {
                       // dgv_GRE_est.Rows[i].Cells[0].Value = true;
                    }

                    chk_GRE_iAcep.Visible = true;
                    chk_GRE_iEnpr.Visible = true;
                    chk_GRE_iEnvia.Visible = true;
                    bt_GRE_impri.Visible = true;
                    panel4.ForeColor = Color.FromArgb(32, 178, 170);
                }
                else
                {
                    for (int i = 0; i < dgv_GRE_est.Rows.Count; i++)
                    {
                        dgv_GRE_est.Rows[i].Cells[0].Value = false;
                    }
                    chk_GRE_iAcep.Checked = false;
                    chk_GRE_iAcep.Visible = false;
                    chk_GRE_iEnpr.Checked = false;
                    chk_GRE_iEnpr.Visible = false;
                    chk_GRE_iEnvia.Checked = false;
                    chk_GRE_iEnvia.Visible = false;
                    bt_GRE_impri.Visible = false;
                    dgv_GRE_est.Columns.Remove(chkGRE);
                    panel4.ForeColor = Color.FromArgb(255, 255, 255);
                }
            }
        }
        private void chk_GRE_iAcep_CheckStateChanged(object sender, EventArgs e)
        {
            marca_check("Aceptado", chk_GRE_iAcep);
        }
        private void chk_GRE_iEnpr_CheckStateChanged(object sender, EventArgs e)
        {
            marca_check("En Proceso", chk_GRE_iEnpr);
        }
        private void chk_GRE_iEnvia_CheckStateChanged(object sender, EventArgs e)
        {
            marca_check("", chk_GRE_iEnvia);    
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
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)        // Click en las columnas boton
        {
            if (dgv_GRE_est.Columns[e.ColumnIndex].Name.ToString() == "consulta")
            {
                if (true)   // dgv_GRE_est.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly == false 
                {
                    if (dgv_GRE_est.Rows[e.RowIndex].Cells[6].Value.ToString() == "0" ||
                        dgv_GRE_est.Rows[e.RowIndex].Cells[6].Value.ToString().Trim() == "")
                    {
                        dgv_GRE_est.Rows[e.RowIndex].Cells[8].ReadOnly = true;
                        dgv_GRE_est.Rows[e.RowIndex].Cells[9].ReadOnly = true;
                        consultaE(dgv_GRE_est.Rows[e.RowIndex].Cells[13].Value.ToString(), e.RowIndex);
                    }
                }
            }
            if (dgv_GRE_est.Columns[e.ColumnIndex].Name.ToString() == "pdf")                    // columna PDF
            {
                if (dgv_GRE_est.Rows[e.RowIndex].Cells[6].Value.ToString() == "1")
                {
                    string urlPdf = dgv_GRE_est.Rows[e.RowIndex].Cells[12].Value.ToString();
                    System.Diagnostics.Process.Start(urlPdf);
                }
            }
            if (dgv_GRE_est.Columns[e.ColumnIndex].Name.ToString() == "cdr")                    // columna CDR
            {
                if (dgv_GRE_est.Rows[e.RowIndex].Cells[6].Value.ToString() == "1")
                {
                    string cdrbyte = dgv_GRE_est.Rows[e.RowIndex].Cells[11].Value.ToString();
                    string serie = dgv_GRE_est.Rows[e.RowIndex].Cells[1].Value.ToString().Substring(0, 4);
                    string corre = dgv_GRE_est.Rows[e.RowIndex].Cells[1].Value.ToString().Substring(5, 8);
                    _E.convierteCDR(cdrbyte, serie, corre, rutatxt);
                }
            }
            if (dgv_GRE_est.Columns[e.ColumnIndex].Name.ToString() == "iTK")
            {
                imprime(dgv_GRE_est.Rows[e.RowIndex].Cells[1].Value.ToString().Substring(0, 4),
                        dgv_GRE_est.Rows[e.RowIndex].Cells[1].Value.ToString().Substring(5, 8));
            }
        }
        private void grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)      // no estamos usando porque no sirve
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex < 0)
                return;

            // pintar una imagen en alguna celda, acá pondremos el icono el pdf y cdr en los respectivos botones
            if (dgv_GRE_est.Columns[e.ColumnIndex].Name == "pdf")
            {
                if (dgv_GRE_est.CurrentRow.Cells[6].Value.ToString() == "1")
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    var w = Properties.Resources.pdf_logo_24x11.Width;
                    var h = Properties.Resources.pdf_logo_24x11.Height;
                    var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                    var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                    e.Graphics.DrawImage(Properties.Resources.pdf_logo_24x11, new Rectangle(x, y, w, h));
                    e.Handled = true;
                }
            }
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
                PointF puntoF = new PointF();
                if (impriLogi == "SI")   // va con logo o no?
                {
                    puntoF = new PointF(coli, posi);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(ruta_logo);
                    //Point loc = new Point(100, 100);
                    e.Graphics.DrawImage(img, puntoF);
                    posi = posi + alfi * 5;
                }
                puntoF = new PointF(coli, posi);
                e.Graphics.DrawString("CONTROL", lt_gra, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 30, posi + 5.0F);
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
                e.Graphics.DrawString("REMITENTE", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm, posi);
                e.Graphics.DrawString(":", lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
                puntoF = new PointF(colm + 10, posi);
                e.Graphics.DrawString(filaimp[12], lt_med, Brushes.Black, puntoF, StringFormat.GenericTypographic);
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

        private void bt_GRE_impri_Click(object sender, EventArgs e)
        {
            // hacemos un ciclo recorriendo fila x fila y jalamos los datos de la guia
            for (int i=0; i < dgv_GRE_est.Rows.Count; i++)
            {
                if (dgv_GRE_est.Rows[i].Cells[0].Value != null && dgv_GRE_est.Rows[i].Cells[0].Value.ToString() == "True")
                {
                    imprime(dgv_GRE_est.Rows[i].Cells[2].Value.ToString().Substring(0, 4),
                        dgv_GRE_est.Rows[i].Cells[2].Value.ToString().Substring(5, 8));
                }
            }
        }
        private void imprime(string serie, string numero)
        {
            // Jalamos los datos que nos falta y los ponemos en sus arreglos
            string[] vs = {"","","","","","","","","","","","","", "", "", "", "", "", "", "",   // 20
                               "", "", "", "", "", "", "", "", "", ""};    // 10
            string[] vc = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };   // 16
            string[] va = { "", "", "", "", "", "" };       // 6
            string[,] dt = new string[3, 5] { { "", "", "", "", "" }, { "", "", "", "", "" }, { "", "", "", "", "" } }; // 5 columnas

            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consdeta = "select a.cantprodi,a.unimedpro,a.descprodi,a.pesoprodi " +
                        "from detguiai a where a.sergui = @ser AND a.numgui = @num";

                    string consulta = "SELECT a.sergui,a.numgui,a.fechopegr,a.dirorigen,a.userc,substring(a.fechc,12,5) as 'fechc',loc.DescrizioneRid as 'locorigen'," +
                        "a.tidocor,dd1.DescrizioneRid AS 'NomTidor1',a.docsremit,a.rucDorig,ifnull(a.tidocor2, '') AS 'tidocor2',ifnull(dd2.DescrizioneRid, '') AS 'NomTidor2',ifnull(a.docsremit2, '') AS 'docsremit2',ifnull(a.rucDorig2, '') AS 'rucDorig2'," +
                        "a.tidoregri,dr1.DescrizioneRid AS 'NomDocRem',a.nudoregri,a.nombregri,a.direregri,a.ubigregri," +
                        "ud1.nombre AS 'dept_ure',up1.nombre AS 'prov_ure',ub1.nombre AS 'dist_ure'," +
                        "a.tidodegri,dr2.DescrizioneRid AS 'NomDocDes',a.nudodegri,a.nombdegri,a.diredegri,a.ubigdegri," +
                        "ud2.nombre AS 'dept_ude',up2.nombre AS 'prov_ude',ub2.nombre AS 'dist_ude'," +
                        "a.fechplani,a.pestotgri,a.pesoKT," +
                        "a.serplagri,a.numplagri,a.plaplagri,a.carplagri,a.autplagri,a.confvegri,a.breplagri,a.proplagri," +
                        "ifnull(c.razonsocial, '') as razonsocial,ifnull(d.marca, '') as marca, ifnull(d.modelo, '') as modelo,ifnull(r.marca, '') as marCarret," +
                        "ifnull(r.confve, '') as confvCarret,ifnull(r.autor1, '') as autCarret,ifnull(p.nomchofe, '') as chocamcar " +
                        "FROM cabguiai a " +
                        "LEFT JOIN desc_dtm dd1 ON dd1.IDCodice = a.tidocor " +
                        "LEFT JOIN desc_dtm dd2 ON dd2.IDCodice = a.tidocor2 " +
                        "LEFT JOIN desc_doc dr1 ON dr1.IDCodice = a.tidoregri " +
                        "LEFT JOIN desc_doc dr2 ON dr2.IDCodice = a.tidodegri " +
                        "LEFT JOIN desc_loc loc ON loc.IDCodice = a.locorigen " +
                        "left join anag_for c on c.ruc = a.proplagri and c.tipdoc = @tdep " +
                        "left join vehiculos d on d.placa = a.plaplagri " +
                        "left join vehiculos r on r.placa = a.carplagri " +
                        "left join cabplacar p on p.id = a.idplani " +
                        "LEFT JOIN ubi_dep ud1 ON ud1.depart = LEFT(a.ubigregri, 2) " +
                        "LEFT join ubi_pro up1 ON concat(up1.depart, up1.provin)= SUBSTRING(a.ubigregri, 1, 4) " +
                        "LEFT JOIN ubigeos ub1 ON concat(ub1.depart, ub1.provin, ub1.distri)= a.ubigregri " +
                        "LEFT JOIN ubi_dep ud2 ON ud2.depart = LEFT(a.ubigdegri, 2) " +
                        "LEFT join ubi_pro up2 ON concat(up2.depart, up2.provin)= SUBSTRING(a.ubigdegri, 1, 4) " +
                        "LEFT JOIN ubigeos ub2 ON concat(ub2.depart, ub2.provin, ub2.distri)= a.ubigdegri " +
                        "where a.sergui = @ser AND a.numgui = @num";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@ser", serie);
                        micon.Parameters.AddWithValue("@num", numero);
                        micon.Parameters.AddWithValue("@tdep", vtc_ruc);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                if (dr.Read())
                                {
                                    vs[0] = dr.GetString("sergui");                         // 0
                                    vs[1] = dr.GetString("numgui");                         // 1
                                    vs[2] = dr.GetString("fechopegr").Substring(0, 10);     // 2
                                    vs[3] = dr.GetString("dirorigen");                      // 3
                                    vs[4] = dr.GetString("NomTidor1");                      // 4
                                    vs[5] = dr.GetString("docsremit");                      // 5
                                    vs[6] = dr.GetString("rucDorig");                       // 6
                                    vs[7] = dr.GetString("NomTidor2");                      // 7
                                    vs[8] = dr.GetString("docsremit2");                     // 8
                                    vs[9] = dr.GetString("rucDorig2");                      // 9
                                    vs[10] = dr.GetString("NomDocRem");                     // 10
                                    vs[11] = dr.GetString("nudoregri");                     // 11
                                    vs[12] = dr.GetString("nombregri");                     // 12
                                    vs[13] = dr.GetString("NomDocDes");                     // 13
                                    vs[14] = dr.GetString("nudodegri");                     // 14
                                    vs[15] = dr.GetString("nombdegri");                     // 15
                                    vs[16] = dr.GetString("fechplani");                     // 16
                                    vs[17] = dr.GetString("pestotgri");                     // 17
                                    vs[18] = dr.GetString("pesoKT");                        // 18
                                    vs[19] = dr.GetString("direregri");                     // 19
                                    vs[20] = dr.GetString("dept_ure");                      // 20
                                    vs[21] = dr.GetString("prov_ure");                      // 21
                                    vs[22] = dr.GetString("dist_ure");                      // 22
                                    vs[23] = dr.GetString("diredegri");                     // 23
                                    vs[24] = dr.GetString("dept_ude");                      // 24
                                    vs[25] = dr.GetString("prov_ude");                      // 25
                                    vs[26] = dr.GetString("dist_ude");                      // 26
                                    vs[27] = dr.GetString("userc");                         // 27
                                    vs[28] = dr.GetString("locorigen");                     // 28
                                    vs[29] = dr.GetString("fechc");                         // 29

                                    vc[0] = dr.GetString("plaplagri");                   // Placa veh principal
                                    vc[1] = dr.GetString("autplagri");                   // Autoriz. vehicular
                                    vc[2] = "";                                          // Num Registro MTC 
                                    vc[3] = dr.GetString("confvegri");                   // Conf. vehicular
                                    vc[4] = dr.GetString("carplagri");                   // Placa carreta
                                    vc[5] = dr.GetString("autCarret");                   // Autoriz. vehicular
                                    vc[6] = "";                                          // Num Registro MTC
                                    vc[7] = dr.GetString("confvCarret");                 // Conf. vehicular 
                                    vc[8] = "";                                          // Choferes - Dni chofer principal
                                    vc[9] = dr.GetString("breplagri");                   // Choferes - Brevete chofer principal
                                    vc[10] = dr.GetString("chocamcar");                  // Choferes - Nombres 
                                    vc[11] = "";                                         // Choferes - Apellidos
                                    vc[12] = "";                                         // Choferes - Dni chofer secundario
                                    vc[13] = "";                                        // Choferes - Brevete chofer secundario
                                    vc[14] = "";                                        // Choferes - Nombres
                                    vc[15] = "";                                        // Choferes - Apellidos
                                }
                                else
                                {
                                    MessageBox.Show("No existe el número de guía!", "Atención - Error interno",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("No existen datos!", "Atención - Error interno2",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            // varios
                            va[0] = "";
                            va[1] = "";
                            va[2] = despedid1;
                            va[3] = despedid2;
                            va[4] = glosa1;
                            va[5] = glosa2;
                        }
                    }
                    // detalle de la guía
                    int y = 0;
                    using (MySqlCommand micomd = new MySqlCommand(consdeta, conn))
                    {
                        micomd.Parameters.AddWithValue("@ser", serie);   // dgv_GRE_est.Rows[i].Cells[2].Value.ToString().Substring(0, 4)
                        micomd.Parameters.AddWithValue("@num", numero);   // dgv_GRE_est.Rows[i].Cells[2].Value.ToString().Substring(5, 8)
                        using (MySqlDataReader drg = micomd.ExecuteReader())
                        {
                            while (drg.Read())  // #fila,a.cantprodi,a.unimedpro,a.descprodi,a.pesoprodi
                            {
                                dt[y, 0] = (y + 1).ToString();
                                dt[y, 1] = drg.GetString(0);
                                dt[y, 2] = drg.GetString(1);
                                dt[y, 3] = drg.GetString(2);
                                dt[y, 4] = drg.GetString(3);
                                y += 1;
                            }

                        }

                    }
                    // llamamos a la clase que imprime
                    impGRE_T imprime = new impGRE_T(1, v_impTK, vs, dt, va, vc);
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
    }
}
