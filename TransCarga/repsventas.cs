﻿using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Text;
using RestSharp;                    // para consulta de CDR

namespace TransCarga
{
    public partial class repsventas : Form
    {
        static string nomform = "repsventas";           // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "cabfactu";            // 
        
        #region variables
        string asd = TransCarga.Program.vg_user;      // usuario conectado al sistema
        public int totfilgrid, cta;             // variables para impresion
        public string perAg = "";
        public string perMo = "";
        public string perAn = "";
        public string perIm = "";
        string codfact = "";
        string codBole = "";            // codigo de Boleta de venta
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
        string rutatxt = "";            // ruta para las guias de remision electronicas
        string rutaxml = "";            // ruta para los XML de las guias de remision
        string[] c_t = new string[6] { "", "", "", "", "", "" }; // parametros para generar el token
        string client_id_sunat = "";    // id del cliente api sunat para guias electrónicas 
        string client_pass_sunat = "";  // clave api sunat para guias electrónicas
        string u_sol_sunat = "";        // usuario sol sunat del cliente
        string c_sol_sunat = "";        // clave sol sunat del cliente
        string scope_sunat = "";        // scope sunat del api
        string glosdetra = "";          // glosa original para las detracciones en tabla enlaces
        string nipfe = "";              // nombre identificador del proveedor de fact electronica
        string restexto = "xxx";        // texto resolucion sunat autorizando prov. fact electronica
        string autoriz_OSE_PSE = "yyy"; // numero resolucion sunat autorizando prov. fact electronica
        string despedida = "";          // texto para mensajes al cliente al final de la impresión del doc.vta. 
        string webose = "";             // direccion web del ose o pse para la descarga del 
        string logoclt = "";            // ruta y nombre archivo logo
        string glosser = "";            // glosa que va en el detalle del doc. de venta
        string vi_formato = "";         // formato de impresion del documento
        string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string vi_copias = "";          // cant copias impresion
        string v_impTK = "";            // nombre de la ticketera
        #endregion

        libreria lib = new libreria();
        acGRE_sunat _E = new acGRE_sunat();           // instanciamos la clase 
        NumLetra nlet = new NumLetra();
        DataTable dtestad = new DataTable();
        DataTable dttaller = new DataTable();
        DataTable dtsunatE = new DataTable();       // comprobantes elec - estados sunat
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public static string CadenaConexion = "Data Source=TransCarga.db";  // Data Source=TransCarga;Mode=Memory;Cache=Shared

        public repsventas()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)    // F1
        {
            // en este form no usamos
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void repsventas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
        }
        private void repsventas_Load(object sender, EventArgs e)
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
            dgv_facts.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_notcre.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            dgv_sunat_est.DefaultCellStyle.BackColor = Color.FromName(colgrid);
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
            dtp_yea.Format = DateTimePickerFormat.Custom;
            dtp_yea.CustomFormat = "yyyy";
            dtp_yea.ShowUpDown = true;
            //
            dtp_mes.Format = DateTimePickerFormat.Custom;
            dtp_mes.CustomFormat = "MM";
            dtp_mes.ShowUpDown = true;

        }
        private void jalainfo()                                     // obtiene datos de imagenes
        {
            try
            {
                using (SqliteConnection cnx = new SqliteConnection(CadenaConexion))
                {
                    cnx.Open();
                    string consulta = "select formulario,campo,param,valor from dt_enlaces where formulario in (@nofo,@ped,@clie)";
                    using (SqliteCommand micon = new SqliteCommand(consulta, cnx))
                    {
                        micon.Parameters.AddWithValue("@nofo", "main");
                        micon.Parameters.AddWithValue("@ped", "facelect");
                        micon.Parameters.AddWithValue("@clie", "clients");
                        SqliteDataReader lite = micon.ExecuteReader();
                        if (lite.HasRows == true)
                        {
                            while (lite.Read())
                            {
                                lite.GetString(0).ToString();
                                if (lite.GetString(0).ToString() == "main")
                                {
                                    if (lite.GetString(1).ToString() == "imagenes")
                                    {
                                        if (lite.GetString(2).ToString() == "img_btN") img_btN = lite.GetString(3).ToString().Trim();         // imagen del boton de accion NUEVO
                                        if (lite.GetString(2).ToString() == "img_btE") img_btE = lite.GetString(3).ToString().Trim();         // imagen del boton de accion EDITAR
                                        if (lite.GetString(2).ToString() == "img_btP") img_btP = lite.GetString(3).ToString().Trim();         // imagen del boton de accion IMPRIMIR
                                        if (lite.GetString(2).ToString() == "img_btA") img_btA = lite.GetString(3).ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                                        if (lite.GetString(2).ToString() == "img_btexc") img_btexc = lite.GetString(3).ToString().Trim();     // imagen del boton exporta a excel
                                        if (lite.GetString(2).ToString() == "img_btQ") img_btq = lite.GetString(3).ToString().Trim();         // imagen del boton de accion SALIR
                                        //if (row["param"].ToString() == "img_btP") img_btP = lite.GetString(3).ToString().Trim();        // imagen del boton de accion IMPRIMIR
                                        if (lite.GetString(2).ToString() == "img_gra") img_grab = lite.GetString(3).ToString().Trim();         // imagen del boton grabar nuevo
                                        if (lite.GetString(2).ToString() == "img_anu") img_anul = lite.GetString(3).ToString().Trim();         // imagen del boton grabar anular
                                        if (lite.GetString(2).ToString() == "img_imprime") img_imprime = lite.GetString(3).ToString().Trim();  // imagen del boton IMPRIMIR REPORTE
                                        if (lite.GetString(2).ToString() == "img_pre") img_preview = lite.GetString(3).ToString().Trim();  // imagen del boton VISTA PRELIMINAR
                                        if (lite.GetString(2).ToString() == "logoPrin") logoclt = lite.GetString(3).ToString().Trim();         // logo emisor
                                    }
                                    if (lite.GetString(1).ToString() == "estado")
                                    {
                                        if (lite.GetString(2).ToString() == "anulado") codAnul = lite.GetString(3).ToString().Trim();         // codigo doc anulado
                                        if (lite.GetString(2).ToString() == "generado") codGene = lite.GetString(3).ToString().Trim();        // codigo doc generado
                                        DataRow[] fila = dtestad.Select("idcodice='" + codAnul + "'");
                                        nomAnul = fila[0][0].ToString();
                                    }
                                    if (lite.GetString(1).ToString() == "sunat")
                                    {
                                        if (lite.GetString(2).ToString() == "client_id") client_id_sunat = lite.GetString(3).ToString().Trim();         // id del api sunat
                                        if (lite.GetString(2).ToString() == "client_pass") client_pass_sunat = lite.GetString(3).ToString().Trim();     // password del api sunat
                                        if (lite.GetString(2).ToString() == "user_sol") u_sol_sunat = lite.GetString(3).ToString().Trim();              // usuario sol portal sunat del cliente 
                                        if (lite.GetString(2).ToString() == "clave_sol") c_sol_sunat = lite.GetString(3).ToString().Trim();             // clave sol portal sunat del cliente 
                                        if (lite.GetString(2).ToString() == "scope") scope_sunat = lite.GetString(3).ToString().Trim();                 // scope del api sunat
                                    }

                                    if (lite.GetString(1).ToString() == "rutas")
                                    {
                                        if (lite.GetString(2).ToString() == "grt_txt") rutatxt = lite.GetString(3).ToString().Trim();         // ruta de los txt para las guías elect
                                        if (lite.GetString(2).ToString() == "grt_xml") rutaxml = lite.GetString(3).ToString().Trim();         // 
                                    }
                                }
                                if (lite.GetString(0).ToString() == "facelect")
                                {
                                    if (lite.GetString(1).ToString() == "documento" && lite.GetString(2).ToString() == "factura") codfact = lite.GetString(3).ToString().Trim();         // 
                                    if (lite.GetString(1).ToString() == "documento" && lite.GetString(2).ToString() == "boleta") codBole = lite.GetString(3).ToString().Trim();         // 
                                    if (lite.GetString(1).ToString() == "moneda" && lite.GetString(2).ToString() == "default") codmon = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "detraccion" && lite.GetString(2).ToString() == "glosa") glosdetra = lite.GetString(3).ToString().Trim();    // glosa detraccion
                                    if (lite.GetString(1).ToString() == "factelect" && lite.GetString(2).ToString() == "ose-pse") nipfe = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "factelect" && lite.GetString(2).ToString() == "textaut") restexto = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "factelect" && lite.GetString(2).ToString() == "autoriz") autoriz_OSE_PSE = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "factelect" && lite.GetString(2).ToString() == "despedi") despedida = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "factelect" && lite.GetString(2).ToString() == "webose") webose = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "impresion")
                                    {
                                        if (lite.GetString(2).ToString() == "formato") vi_formato = lite.GetString(3).ToString().Trim();
                                        if (lite.GetString(2).ToString() == "filasDet") v_mfildet = lite.GetString(3).ToString().Trim();       // maxima cant de filas de detalle
                                        if (lite.GetString(2).ToString() == "copias") vi_copias = lite.GetString(3).ToString().Trim();
                                        if (lite.GetString(2).ToString() == "impTK") v_impTK = lite.GetString(3).ToString().Trim();
                                        //if (lite.GetString(2).ToString() == "nomfor_cr") v_CR_gr_ind = lite.GetString(3).ToString().Trim();
                                    }
                                }
                                if (lite.GetString(0).ToString() == "clients")
                                {
                                    if (lite.GetString(1).ToString() == "documento" && lite.GetString(2).ToString() == "dni") coddni = lite.GetString(3).ToString().Trim();
                                    if (lite.GetString(1).ToString() == "documento" && lite.GetString(2).ToString() == "ruc") codruc = lite.GetString(3).ToString().Trim();
                                }

                            }
                            // parametros para token
                            c_t[0] = client_id_sunat;
                            c_t[1] = scope_sunat;
                            c_t[2] = client_id_sunat;
                            c_t[3] = client_pass_sunat;
                            c_t[4] = u_sol_sunat;
                            c_t[5] = c_sol_sunat;
                        }
                    }
                }
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
                // PANEL facturacion
                cmb_sede_guias.DataSource = dttaller;
                cmb_sede_guias.DisplayMember = "descrizionerid";
                cmb_sede_guias.ValueMember = "idcodice";
                // PANEL notas de credito
                cmb_sede_plan.DataSource = dttaller;
                cmb_sede_plan.DisplayMember = "descrizionerid"; ;
                cmb_sede_plan.ValueMember = "idcodice";
                // panel de estatos sunat
                cmb_sunat_sede.DataSource = dttaller;
                cmb_sunat_sede.DisplayMember = "descrizionerid";
                cmb_sunat_sede.ValueMember = "idcodice";
                // ***************** seleccion de estado de servicios
                string conestad = "select descrizionerid,idcodice,codigo from desc_est " +
                                       "where numero=1 order by idcodice";
                cmd = new MySqlCommand(conestad, conn);
                MySqlDataAdapter daestad = new MySqlDataAdapter(cmd);
                daestad.Fill(dtestad);
                // PANEL facturacion
                cmb_estad_guias.DataSource = dtestad;
                cmb_estad_guias.DisplayMember = "descrizionerid";
                cmb_estad_guias.ValueMember = "idcodice";
                // PANEL notas de credito
                cmb_estad_plan.DataSource = dtestad;
                cmb_estad_plan.DisplayMember = "descrizionerid";
                cmb_estad_plan.ValueMember = "idcodice";

                // ----------------- panel de estatos sunat
                string conesu = "select descrizionerid,idcodice from desc_esu where numero=1 order by idcodice";
                cmd = new MySqlCommand(conesu, conn);
                MySqlDataAdapter datesu = new MySqlDataAdapter(cmd);
                DataTable dtesu = new DataTable();
                datesu.Fill(dtesu);
                cmb_sunat_est.DataSource = dtesu;
                cmb_sunat_est.DisplayMember = "descrizionerid";
                cmb_sunat_est.ValueMember = "idcodice";
                datesu.Dispose();
            }
            conn.Close();
        }
        private void grilla(string dgv)                             // 
        {
            Font tiplg = new Font("Arial", 7, FontStyle.Bold);
            int b;
            switch (dgv)
            {
                case "dgv_guias":
                    dgv_facts.Font = tiplg;
                    dgv_facts.DefaultCellStyle.Font = tiplg;
                    dgv_facts.RowTemplate.Height = 15;
                    dgv_facts.AllowUserToAddRows = false;
                    if (dgv_facts.DataSource == null) dgv_facts.ColumnCount = 11;
                    /*
                    dgv_facts.Width = Parent.Width - 70; // 1015;
                    if (dgv_facts.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_facts.Columns.Count; i++)
                        {
                            dgv_facts.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_facts.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_facts.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_facts.Columns.Count; i++)
                        {
                            int a = dgv_facts.Columns[i].Width;
                            b += a;
                            dgv_facts.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_facts.Columns[i].Width = a;
                        }
                        if (b < dgv_facts.Width) dgv_facts.Width = b - 20;  // b + 60;
                        dgv_facts.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_facts");
                    break;
                case "dgv_plan":
                    dgv_notcre.Font = tiplg;
                    dgv_notcre.DefaultCellStyle.Font = tiplg;
                    dgv_notcre.RowTemplate.Height = 15;
                    dgv_notcre.AllowUserToAddRows = false;
                    if (dgv_notcre.DataSource == null) dgv_notcre.ColumnCount = 11;
                    /*
                    dgv_facts.Width = Parent.Width - 70; // 1015;
                    if (dgv_notcre.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_notcre.Columns.Count; i++)
                        {
                            dgv_notcre.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_notcre.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_notcre.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_notcre.Columns.Count; i++)
                        {
                            int a = dgv_notcre.Columns[i].Width;
                            b += a;
                            dgv_notcre.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_notcre.Columns[i].Width = a;
                        }
                        if (b < dgv_notcre.Width) dgv_notcre.Width = b - 20;    // b + 60 ;
                        dgv_notcre.ReadOnly = true;
                    }
                    */
                    suma_grilla("dgv_notcre");
                    break;
                case "dgv_sunat_est":
                    dgv_sunat_est.Font = tiplg;
                    dgv_sunat_est.DefaultCellStyle.Font = tiplg;
                    dgv_sunat_est.RowTemplate.Height = 15;
                    dgv_sunat_est.AllowUserToAddRows = false;
                    suma_grilla("dgv_sunat_est");

                    Padding padding = new Padding();
                    padding.Left = 16;
                    padding.Right = 16;
                    padding.Top = 0;
                    padding.Bottom = 0;

                    Font chiq = new Font("Arial", 6, FontStyle.Bold);

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
                    /*
                    DataGridViewButtonColumn btnPDF = new DataGridViewButtonColumn();
                    btnPDF.HeaderText = "PDF";
                    btnPDF.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnPDF.Name = "pdf";
                    btnPDF.Width = 60;
                    btnPDF.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnPDF.DefaultCellStyle.Padding = padding;
                    btnPDF.DefaultCellStyle.Font = chiq;
                    btnPDF.DefaultCellStyle.SelectionBackColor = Color.White;
                    */
                    DataGridViewButtonColumn btnAct = new DataGridViewButtonColumn();
                    btnAct.HeaderText = "Sunat"; // ACTUALIZA
                    btnAct.Text = "...Consulta...";
                    btnAct.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnAct.Name = "consulta";
                    btnAct.Width = 140;
                    btnAct.UseColumnTextForButtonValue = true;
                    btnAct.DefaultCellStyle.Padding = padding;
                    // EMISION,TIPO,COMPROBANTE,ORIGEN,ESTADO,SUNAT,CDR_GEN,btnTK,btnCDR,btnACT,ad.cdr as Rspta,ad.textoQR,ad.nticket,f.canfidt,f.id
                    //     0  ,  1 ,      2    ,   3  ,  4   ,  5  ,   6   ,  7  ,  8   ,  9   ,  10   ,    11    ,   12     ,   13   , 14
                    dgv_sunat_est.CellClick += DataGridView1_CellClick;
                    dgv_sunat_est.Columns.Insert(7, btnTk);
                    //dgv_sunat_est.Columns.Insert(8, btnPDF);   // .Add(btnPDF);
                    dgv_sunat_est.Columns.Insert(8, btnCDR);   // .Add(btnCDR);
                    dgv_sunat_est.Columns.Insert(9, btnAct);   // .Add(btnAct);
                    dgv_sunat_est.Columns[10].Visible = false;
                    dgv_sunat_est.Columns[11].Visible = false;
                    dgv_sunat_est.Columns[12].Visible = false;
                    dgv_sunat_est.Columns[13].Visible = false;
                    dgv_sunat_est.Columns[14].Visible = false;
                    if (dgv_sunat_est.Rows.Count > 0)         // autosize filas
                    {
                        for (int i = 0; i < dgv_sunat_est.Columns.Count - 10; i++)
                        {
                            dgv_sunat_est.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_sunat_est.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_sunat_est.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < dgv_sunat_est.Columns.Count - 10; i++)
                        {
                            int a = dgv_sunat_est.Columns[i].Width;
                            b += a;
                            dgv_sunat_est.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_sunat_est.Columns[i].Width = a;
                        }
                        if (b < dgv_sunat_est.Width) dgv_sunat_est.Width = dgv_sunat_est.Width - 10;
                        dgv_sunat_est.ReadOnly = true;
                    }
                    if (dgv_sunat_est.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_sunat_est.Rows.Count; i++)
                        {
                            dgv_sunat_est.Rows[i].Cells["iTK"].Value = "TK";
                            if (dgv_sunat_est.Rows[i].Cells["iTK"].Value != null)
                            {
                                if (dgv_sunat_est.Rows[i].Cells["CDR_GEN"].Value.ToString() == "0")
                                {
                                    //dgv_sunat_est.Rows[i].Cells[8].ReadOnly = false;
                                    //dgv_sunat_est.Rows[i].Cells[8].Value = "PDF";
                                    dgv_sunat_est.Rows[i].Cells["cdr"].ReadOnly = false;
                                    dgv_sunat_est.Rows[i].Cells["cdr"].Value = "CDR";
                                    dgv_sunat_est.Rows[i].Cells["cdr"].ReadOnly = true;
                                    dgv_sunat_est.Rows[i].Cells["consulta"].ReadOnly = true;
                                    dgv_sunat_est.Rows[i].Cells["consulta"].Value = "";
                                }
                                else
                                {
                                    dgv_sunat_est.Rows[i].Cells["cdr"].ReadOnly = true;
                                    dgv_sunat_est.Rows[i].Cells["cdr"].Value = "";
                                    dgv_sunat_est.Rows[i].Cells["consulta"].ReadOnly = false;
                                    dgv_sunat_est.Rows[i].Cells["consulta"].Value = "...Consulta...";
                                    //dgv_sunat_est.Rows[i].Cells[10].ReadOnly = false;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        private void bt_guias_Click(object sender, EventArgs e)         // genera reporte guias
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_vtas_fact1";
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
                        dgv_facts.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_facts.DataSource = dt;
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
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "rep_vtas_ncred1";
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
                        dgv_notcre.DataSource = null;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_notcre.DataSource = dt;
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
        private void bt_regvtas_Click(object sender, EventArgs e)       // Registro de ventas
        {
            string consulta = "rep_vtas_regvtas1";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(conn) == true)
                {
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@fini", dtp_yea.Value.Year);
                        micon.Parameters.AddWithValue("@fter", dtp_mes.Value.Month);
                        micon.Parameters.AddWithValue("@vanu", codAnul);
                        micon.Parameters.AddWithValue("@vfac", codfact);
                        micon.Parameters.AddWithValue("@vruc", coddni);
                        micon.Parameters.AddWithValue("@vdni", codruc);
                        micon.Parameters.AddWithValue("@vmon", codmon);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            dgv_regvtas.DataSource = null;
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgv_regvtas.DataSource = dt;
                            grilla("dgv_regvtas");
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
        private void suma_grilla(string dgv)
        {
            DataRow[] row = dtestad.Select("idcodice='" + codAnul + "'");   // dtestad
            string etiq_anulado = row[0].ItemArray[0].ToString();
            int cr = 0, ca = 0; // dgv_facts.Rows.Count;
            double tvv = 0, tva = 0;
            switch (dgv)
            {
                case "dgv_facts":
                    for (int i=0; i < dgv_facts.Rows.Count; i++)
                    {
                        if (dgv_facts.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_facts.Rows[i].Cells["TOTAL_MN"].Value);
                            cr = cr + 1;
                        }
                        else
                        {
                            dgv_facts.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                            ca = ca + 1;
                            tva = tva + Convert.ToDouble(dgv_facts.Rows[i].Cells["TOTAL_MN"].Value);
                        }
                    }
                    tx_tfi_f.Text = cr.ToString();
                    tx_totval.Text = tvv.ToString("#0.00");
                    tx_tfi_a.Text = ca.ToString();
                    tx_totv_a.Text = tva.ToString("#0.00");
                    break;
                case "dgv_notcre":
                    for (int i = 0; i < dgv_notcre.Rows.Count; i++)
                    {
                        if (dgv_notcre.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                        {
                            tvv = tvv + Convert.ToDouble(dgv_notcre.Rows[i].Cells["TOTAL_MN"].Value);
                            cr = cr + 1;
                        }
                        else
                        {
                            dgv_notcre.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                            ca = ca + 1;
                            tva = tva + Convert.ToDouble(dgv_notcre.Rows[i].Cells["TOTAL_MN"].Value);
                        }
                    }
                    tx_tfi_n.Text = cr.ToString();
                    tx_totval_n.Text = tvv.ToString("#0.00");
                    break;
                case "dgv_sunat_est":
                    for (int i = 0; i < dgv_sunat_est.Rows.Count; i++)
                    {
                        if (dgv_sunat_est.Rows[i].Cells["ESTADO"].Value.ToString() != etiq_anulado)
                        {
                            cr = cr + 1;
                        }
                        else
                        {
                            dgv_sunat_est.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                            ca = ca + 1;
                        }
                    }
                    tx_sunat_fa.Text = ca.ToString("#0");
                    tx_sunat_fv.Text = cr.ToString("#0");
                    break;
            }
        }
        private void bt_sunatEst_Click(object sender, EventArgs e)      // estados sunat de comprobantes
        {
            dtsunatE.Rows.Clear();
            dtsunatE.Columns.Clear();
            // validaciones
            if (tx_dat_sunat_sede.Text == "")
            {
                MessageBox.Show("Seleccione el local Origen", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmb_sunat_sede.Focus();
                return;
            }
            string consulta = "";
            string parte = "";
            if (rb_dVtas.Checked == true)   // facturas y boletas
            {
                consulta = "SELECT f.fechope AS EMISION,f.martdve as TIPO,CONCAT(f.serdvta,'-',f.numdvta) AS COMPROBANTE,lo.descrizionerid AS ORIGEN," +
                    "es.DescrizioneRid AS ESTADO,ad.estadoS AS SUNAT,ad.cdrgener AS CDR_GEN,ad.cdr as Rspta,ad.textoQR,ad.nticket,f.canfidt,f.id " + // ,ad.ulterror as ULT_ERROR
                    "FROM cabfactu f LEFT JOIN adifactu ad ON ad.idc = f.id " +
                    "LEFT JOIN desc_loc lo ON lo.IDCodice = f.locorig " +
                    "LEFT JOIN desc_est es ON es.IDCodice = f.estdvta  " +
                    "WHERE f.fechope between @fecini and @fecfin";  // marca_gre<>'' AND 
            }
            if (rb_notaC.Checked == true)   // notas de crédito
            {

            }
            if (tx_dat_sunat_sede.Text != "") parte = parte + " and f.locorig=@loca";
            if (tx_dat_sunat_est.Text != "") parte = parte + " and ad.estadoS=@esta";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand micon = new MySqlCommand(consulta + parte, conn))
                {
                    micon.Parameters.AddWithValue("@loca", tx_dat_sunat_sede.Text);
                    micon.Parameters.AddWithValue("@fecini", dtp_sunat_fini.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@fecfin", dtp_sunat_fter.Value.ToString("yyyy-MM-dd"));
                    micon.Parameters.AddWithValue("@esta", (tx_dat_sunat_est.Text != "") ? tx_dat_sunat_est.Text : "");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        dgv_sunat_est.DataSource = null;
                        dgv_sunat_est.Columns.Clear();
                        dgv_sunat_est.Rows.Clear();
                        //
                        da.Fill(dtsunatE);
                        dgv_sunat_est.DataSource = dtsunatE;
                        grilla("dgv_sunat_est");
                    }
                }
            }

        }

        #region combos
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
        private void cmb_sunat_sede_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_sunat_sede.SelectedValue != null) tx_dat_sunat_sede.Text = cmb_sunat_sede.SelectedValue.ToString();
            else tx_dat_sunat_sede.Text = "";
        }
        private void cmb_sunat_sede_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sunat_sede.SelectedIndex = -1;
                tx_dat_sunat_sede.Text = "";
            }
        }
        private void cmb_sunat_est_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_sunat_est.SelectedValue != null) tx_dat_sunat_est.Text = cmb_sunat_est.SelectedValue.ToString();
            else tx_dat_sunat_est.Text = "";
        }
        private void cmb_sunat_est_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                cmb_sunat_est.SelectedIndex = -1;
                tx_dat_sunat_est.Text = "";
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
            cmb_sede_guias.SelectedIndex = -1;
            cmb_estad_guias.SelectedIndex = -1;
            cmb_estad_plan.SelectedIndex = -1;
            cmb_sede_plan.SelectedIndex = -1;
            cmb_sunat_est.SelectedIndex = -1;
            cmb_sunat_sede.SelectedIndex = -1;
            //
            rb_dVtas.Checked = true;
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
            if (tabControl1.SelectedTab == tabfacts && dgv_facts.Rows.Count > 0)
            {
                nombre = "Reportes_facturacion_" + cmb_sede_guias.Text.Trim() +"_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_facts.DataSource;
                    wb.Worksheets.Add(dt, "Ventas");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabnotas && dgv_notcre.Rows.Count > 0)
            {
                nombre = "Reportes_NotasCred_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_notcre.DataSource;
                    wb.Worksheets.Add(dt, "Notas");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabregvtas && dgv_regvtas.Rows.Count > 0)
            {
                nombre = "Registro_Ventas_" + dtp_yea.Value.Year.ToString() + "-" + dtp_mes.Value.Month.ToString() + "_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_regvtas.DataSource;
                    wb.Worksheets.Add(dt, "RegVtas");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
            if (tabControl1.SelectedTab == tabSunat && dgv_sunat_est.Rows.Count > 0)
            {
                // veremos si conviene exportar o no? 11/09/2023
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
            if (tabControl1.SelectedTab.Name == "tabfacts")
            {
                DataTable dtg = (DataTable)dgv_facts.DataSource;
                dtg.DefaultView.Sort = dgv_facts.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabnotas")
            {
                DataTable dtg = (DataTable)dgv_notcre.DataSource;
                dtg.DefaultView.Sort = dgv_notcre.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabregvtas")
            {
                DataTable dtg = (DataTable)dgv_regvtas.DataSource;
                dtg.DefaultView.Sort = dgv_regvtas.SortString;
            }
            if (tabControl1.SelectedTab.Name == "tabSunat")
            {
                DataTable dtg = (DataTable)dgv_sunat_est.DataSource;
                dtg.DefaultView.Sort = dgv_sunat_est.SortString;
            }
        }
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)                  // filtro de las columnas
        {
            if (tabControl1.SelectedTab.Name == "tabfacts")
            {
                DataTable dtg = (DataTable)dgv_facts.DataSource;
                dtg.DefaultView.RowFilter = dgv_facts.FilterString;
                suma_grilla("dgv_facts");
            }
            if (tabControl1.SelectedTab.Name == "tabnotas")
            {
                DataTable dtg = (DataTable)dgv_notcre.DataSource;
                dtg.DefaultView.RowFilter = dgv_notcre.FilterString;
                suma_grilla("dgv_notcre");
            }
            if (tabControl1.SelectedTab.Name == "tabregvtas")
            {
                DataTable dtg = (DataTable)dgv_regvtas.DataSource;
                dtg.DefaultView.RowFilter = dgv_regvtas.FilterString;
            }
            if (tabControl1.SelectedTab.Name == "tabfacts")
            {
                DataTable dtg = (DataTable)dgv_sunat_est.DataSource;
                dtg.DefaultView.RowFilter = dgv_sunat_est.FilterString;
                suma_grilla("dgv_sunat_est");
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
            if (dgv_sunat_est.Columns[e.ColumnIndex].Name.ToString() == "consulta")
            {
                if (true)
                {
                    if (dgv_sunat_est.Rows[e.RowIndex].Cells[6].Value.ToString() == "0" ||
                        dgv_sunat_est.Rows[e.RowIndex].Cells[6].Value.ToString().Trim() == "")
                    {
                        dgv_sunat_est.Rows[e.RowIndex].Cells[8].ReadOnly = true;
                        dgv_sunat_est.Rows[e.RowIndex].Cells[9].ReadOnly = true;
                        consultaE(dgv_sunat_est.Rows[e.RowIndex].Cells[13].Value.ToString(), e.RowIndex);
                    }
                }
            }
            if (dgv_sunat_est.Columns[e.ColumnIndex].Name.ToString() == "cdr")                    // columna CDR
            {
                if (dgv_sunat_est.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "")
                {
                    // PRIMERO deberíamos buscar el cdr.xml en el directorio respectivo
                    // Si hay, deberia sacar un mensaje indicando la ruta donde esta el xml respuesta
                    // Si NO hay, DEBERIAMOS CONSULTAR EN SUNAT EL CDR DEL COMPROBANTE
                    string archi = "R-" + Program.ruc + "-" + ((dgv_sunat_est.Rows[e.RowIndex].Cells["tipo"].Value.ToString() == "F") ? "01" : "03") + "-" +
                        dgv_sunat_est.Rows[e.RowIndex].Cells["tipo"].Value.ToString() + lib.Right(dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString(), 12) + ".zip";
                    if (File.Exists(@rutaxml + archi) == true)     // si hay el xml
                    {
                        MessageBox.Show("El xml zip de respuesta esta en:" + Environment.NewLine +
                            rutaxml + archi, "El CDR está descargado");
                    }
                    else
                    {
                        // no hay el xml ... armarlo desde el dato guardado en la tabla adifactu
                        if (true)
                        {
                            // OPCION 1: leemos el byte[] de la tabla y lo armamos en el directorio 
                            {
                                Byte[] arCdr = Encoding.ASCII.GetBytes(dgv_sunat_est.Rows[e.RowIndex].Cells["Rspta"].Value.ToString());
                                File.WriteAllBytes("nose", arCdr);
                                FileStream fstrm = new FileStream(@rutaxml + archi, FileMode.CreateNew, FileAccess.Write);
                                //BinaryWriter writer = new BinaryWriter(fstrm);
                                fstrm.Write(arCdr, 0, arCdr.Length);
                                //writer.Write(arCdr);
                                //writer.Close();
                                fstrm.Close();
                                //Esta funcionalidad ... no esta bien 28/09/2023 .... no graba el zip correctamente porque posiblemente el campo de la tabla no tenga el tipo correcto .... no se
                            }
                            {
                                // OPCION 2: jalamos el cdr del webservice soap de consulta
                                string pRuc = Program.ruc;
                                string pTip = ((dgv_sunat_est.Rows[e.RowIndex].Cells["tipo"].Value.ToString() == "F") ? "01" : "03");
                                string pSer = dgv_sunat_est.Rows[e.RowIndex].Cells["tipo"].Value.ToString() + dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(1, 3);
                                int pNum = int.Parse(dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(5, 8));

                                // no me funca esta consulta SOAP, no se como programar la consulta .... 04/10/2023
                                ServiceConsultaCDR.billServiceClient aaa = new ServiceConsultaCDR.billServiceClient();
                                aaa.Endpoint.Name = "BillConsultServicePort";
                                // 29/09/2023 me quede acá
                                string x = aaa.getStatusCdr(pRuc, pTip, pSer, pNum).statusMessage;

                            }
                        }
                        // alternativa 2, hacemos la consulta del CDR al WS de consultas de sunat .. NO FUNCA, EL SERVICIO WEB REST NO RESPONDE, 06/10/2023
                        if (false)
                        {
                            try
                            {
                                string pRuc = Program.ruc;
                                string pTip = ((dgv_sunat_est.Rows[e.RowIndex].Cells["tipo"].Value.ToString() == "F") ? "01" : "03");
                                string pSer = dgv_sunat_est.Rows[e.RowIndex].Cells["tipo"].Value.ToString() + dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(1, 3);
                                string pNum = dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(5, 8);

                                string token = _E.conex_token_(c_t);
                                /* var resCon = _E.consCDR(pRuc, token, pTip, pSer, pNum, rutaxml);
                                if (resCon == null)
                                {
                                    MessageBox.Show("Tenemos problemas con la respuesta", "Error en comprobante", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (resCon.Item1 == "Rechazado" || resCon.Item1 == "Error")
                                    {
                                        MessageBox.Show(resCon.Item2, resCon.Item1, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                */
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error al enviar a Sunat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //return retorna;
                            }
                        }
                    }
                }
            }
            if (dgv_sunat_est.Columns[e.ColumnIndex].Name.ToString() == "iTK")
            {
                string cdtip = (dgv_sunat_est.Rows[e.RowIndex].Cells[1].Value.ToString() == "F") ? codfact : codBole;
                imprime(cdtip,
                    dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(0, 4),
                    dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(5, 8));
            }
        }
        #endregion

        private void imprime(string tipo,string serie, string numero)
        {
            string[] vs = {"","","","","","","","","","","","","", "", "", "", "", "", "", "",   // 20
                               "", "", "", "", "", "", "", "", "", ""};    // 10
            string[] va = { "", "", "", "", "", "", "", "", "" };       // 9
            string[,] dt = new string[10, 6] {
                    { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" },
                    { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }, { "", "", "", "", "", "" }
            }; // 6 columnas, 10 filas
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consdeta = "select a.codgror,a.cantbul,a.unimedp,a.descpro,a.pesogro,b.docsremit " +
                        "from detfactu a left join cabguiai b on concat(b.sergui,'-',b.numgui)=a.codgror " +
                        "where a.tipdocvta=@tdv and a.serdvta=@ser and a.numdvta=@num";

                    string consulta = "select a.id,DATE_FORMAT(a.fechope,'%d/%m/%Y') AS fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,a.ticltgr,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt,a.ubigclt,a.corrclt,a.teleclt," +
                        "a.locorig,a.dirorig,a.ubiorig,a.obsdvta,a.canfidt,a.canbudt,a.mondvta,a.tcadvta,a.subtota,a.igvtota,a.porcigv,a.totdvta,a.totpags,a.saldvta,a.estdvta,a.frase01,a.impreso,d.codsunat as ctdcl," +
                        "a.tipoclt,a.m1clien,a.tippago,a.ferecep,a.userc,a.fechc,a.userm,a.fechm,b.descrizionerid as nomest,ifnull(c.id,'') as cobra,a.idcaja,a.plazocred,a.totdvMN,ifnull(p.marca1,'') as dpc,s.glosaser," +
                        "a.cargaunica,a.porcendscto,a.valordscto,a.conPago,a.pagauto,ifnull(ad.placa,'') as placa,ifnull(ad.confv,'') as confv,ifnull(ad.autoriz,'') as autoriz,m.descrizionerid as nomon,t.codsunat as cdtdv," +
                        "ifnull(ad.cargaEf,0) as cargaEf,ifnull(ad.cargaUt,0) as cargaUt,ifnull(ad.rucTrans,'') as rucTrans,ifnull(ad.nomTrans,'') as nomTrans,ifnull(date_format(ad.fecIniTras,'%Y-%m-%d'),'') as fecIniTras," +
                        "ifnull(ad.dirPartida,'') as dirPartida,ifnull(ad.ubiPartida,'') as ubiPartida,ifnull(ad.dirDestin,'') as dirDestin,ifnull(ad.ubiDestin,'') as ubiDestin,ifnull(ad.dniChof,'') as dniChof," +
                        "ifnull(ad.brevete,'') as brevete,ifnull(ad.valRefViaje,0) as valRefViaje,ifnull(ad.valRefVehic,0) as valRefVehic,ifnull(ad.valRefTon,0) as valRefTon,l.descrizionerid as nomLocO " +
                        "from cabfactu a " +
                        "left join adifactu ad on ad.idc=a.id and ad.tipoAd=1 " +
                        "left join desc_est b on b.idcodice=a.estdvta " +
                        "left join desc_mon m on m.idcodice=a.mondvta " +
                        "left join desc_tpa p on p.idcodice=a.plazocred " +
                        "left join desc_tdv t on t.idcodice=a.tipdvta " +
                        "left join desc_doc d on d.idcodice=a.tidoclt " +
                        "left join desc_loc l on l.idcodice=a.locorig " +
                        "left join series s on s.tipdoc=a.tipdvta and s.serie=a.serdvta " +
                        "left join cabcobran c on c.tipdoco=a.tipdvta and c.serdoco=a.serdvta and c.numdoco=a.numdvta and c.estdcob<>@coda " +
                        "where a.tipdvta=@tdv and a.serdvta=@ser and a.numdvta=@num";
                    using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                    {
                        micon.Parameters.AddWithValue("@ser", serie);
                        micon.Parameters.AddWithValue("@num", numero);
                        micon.Parameters.AddWithValue("@tdv", tipo);
                        micon.Parameters.AddWithValue("@coda", codAnul);
                        using (MySqlDataReader dr = micon.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                if (dr.Read())
                                {
                                    vs[0] = serie;                          // serie (F001)
                                    vs[1] = numero;                         // numero
                                    vs[2] = tipo;                           // tx_dat_tdv.Text, codigo Transcarga del tipo de documento
                                    vs[3] = Program.dirfisc;                // direccion emisor
                                    if (tipo != codfact) vs[4] = "Boleta de Venta Electrónica";
                                    if (tipo == codfact) vs[4] = "Factura Electrónica";
                                    vs[5] = dr.GetString("fechope");        // fecha de emision formato dd/mm/aaaa
                                    vs[6] = dr.GetString("nombclt");        // nombre del cliente del comprobante
                                    vs[7] = dr.GetString("nudoclt");        // numero documento del cliente
                                    vs[8] = dr.GetString("direclt");        // dirección cliente
                                    vs[9] = dr.GetString("distclt");        // distrito de la direccion
                                    vs[10] = dr.GetString("provclt");       // provincia de la direccion
                                    vs[11] = dr.GetString("dptoclt");       // departamento de la dirección
                                    vs[12] = dr.GetString("canfidt");       // cantidad de filas de detalle
                                    vs[13] = dr.GetString("subtota");       // Sub total del comprobante
                                    vs[14] = dr.GetString("igvtota");       // igv del comprobante
                                    vs[15] = dr.GetString("totdvta");       // importe total del comprobante
                                    vs[16] = dr.GetString("nomon"); ;       // Simbolo de la moneda
                                    vs[17] = nlet.Convertir(dr.GetString("totdvta"),true);                  // flete en letras
                                    vs[18] = (dr.GetString("tippago").Trim() != "" && dr.GetString("plazocred").Trim() == "") ? "CONTADO" : "CREDITO";
                                    vs[19] = (dr.GetString("plazocred") != "") ? dr.GetString("dpc") : "";  // dias de plazo credito
                                    vs[20] = glosdetra;                     // Glosa para la detracción
                                    vs[21] = dr.GetString("cdtdv");         // codigo sunat tipo comprobante
                                    vs[22] = dr.GetString("ctdcl");         // CODIGO SUNAT tipo de documento RUC/DNI del cliente
                                    vs[23] = nipfe;                         // identificador de ose/pse metodo de envío
                                    vs[24] = restexto;                      // texto del resolucion sunat del ose/pse
                                    vs[25] = autoriz_OSE_PSE;               // autoriz del ose/pse
                                    vs[26] = webose;                        // web del ose/pse
                                    vs[27] = dr.GetString("userc").Trim();  // usuario creador
                                    vs[28] = dr.GetString("nomLocO").Trim();    // local de emisión
                                    vs[29] = despedida;                     // glosa despedida

                                    // varios
                                    glosser = dr.GetString("glosaser");
                                    va[0] = logoclt;         // Ruta y nombre del logo del emisor electrónico
                                    va[1] = glosser;         // glosa del servicio en facturacion
                                    va[2] = codfact;         // siglas nombre de tipo de documento Factura 
                                    va[3] = "";         // 
                                    va[4] = "";         // 
                                    va[5] = "";         // 
                                    va[6] = "";         // 
                                    va[7] = "";         // 
                                    va[8] = "";         // 
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
                        }
                    }
                    // detalle del comprobante
                    int y = 0;
                    using (MySqlCommand micomd = new MySqlCommand(consdeta, conn))
                    {
                        micomd.Parameters.AddWithValue("@ser", serie);
                        micomd.Parameters.AddWithValue("@num", numero);
                        micomd.Parameters.AddWithValue("@tdv", tipo);
                        using (MySqlDataReader drg = micomd.ExecuteReader())
                        {
                            while (drg.Read())  // #fila,a.cantprodi,a.unimedpro,a.descprodi,a.pesoprodi
                            {
                                //dt[y, 0] = (y + 1).ToString();
                                dt[y, 0] = "OriDest";
                                dt[y, 1] = drg.GetString("cantbul");
                                dt[y, 2] = drg.GetString("unimedp");
                                dt[y, 3] = drg.GetString("codgror");             // guia transportista
                                dt[y, 4] = drg.GetString("descpro");             // descripcion de la carga
                                dt[y, 5] = drg.GetString("docsremit");           // documento relacionado remitente de la guia transportista
                                y += 1;
                            }
                        }
                    }
                    // llamamos a la clase que imprime
                    impDV imp = new impDV(1, v_impTK, vs, dt, va, "TK", "");
                }
            }
        }
        private string consultaE(string ticket, int rowIndex)       // consulta estado en Sunat
        {
            string retorna = "";
            MessageBox.Show("Estamos consultando el comprobante");

            if (ticket == "") return retorna;

            /*string token = _E.conex_token_(c_t);
            var resCon = _E.consultaC((rb_GRE_R.Checked == true) ? "adiguiar" : "adiguias", dgv_GRE_est.Rows[rowIndex].Cells[15].Value.ToString(), ticket, token,
                dgv_GRE_est.Rows[rowIndex].Cells[1].Value.ToString().Substring(0, 4), dgv_GRE_est.Rows[rowIndex].Cells[1].Value.ToString().Substring(5, 8), rutaxml);
            */
            var resCon = "";
            if (resCon == null)
            {
                MessageBox.Show("La respuesta del ticket fue nulo", "Error en consultar ticket", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

            }

            return retorna;
        }

    }
}
