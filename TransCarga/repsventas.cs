using System;
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
        string glosser2 = "";           // glosa ALTERNATIVA DEL DETALLE DEL DOC.VENTA
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
        string forA4CRn = "";           // ruta y nombre del formato CR de factura/boletas "normales"
        string forA4CRcu = "";          // ruta y nombre del formato CR de facturas de cargas únicas
        string vi_rutaQR = "";          // ruta y nombre del QR 
        string v_igv = "";              // valor del igv actual
        string claveCertif = "";        // clave del certificado de seguridad
        string rutaCertifc = "";        // ruta del certificado
        #endregion

        libreria lib = new libreria();
        acGRE_sunat _E = new acGRE_sunat();           // instanciamos la clase 
        NumLetra nlet = new NumLetra();
        DataTable dtestad = new DataTable();
        DataTable dttaller = new DataTable();
        int cuenta = -1;     // contador de repeticiones de visualizacion en columnas de estados
        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        public static string CadenaConexion = "Data Source=TransCarga.db";  // Data Source=TransCarga;Mode=Memory;Cache=Shared
        string[] varios = { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };       // 14 

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
            for (int t = 0; t < Program.dt_enlaces.Rows.Count; t++)
            {
                DataRow row = Program.dt_enlaces.Rows[t];
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
                        if (row["param"].ToString() == "logoPrin") logoclt = row["valor"].ToString().Trim();         // logo emisor
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
                        if (row["param"].ToString() == "client_id") client_id_sunat = row["valor"].ToString().Trim();         // id del api sunat
                        if (row["param"].ToString() == "client_pass") client_pass_sunat = row["valor"].ToString().Trim();     // password del api sunat
                        if (row["param"].ToString() == "user_sol") u_sol_sunat = row["valor"].ToString().Trim();              // usuario sol portal sunat del cliente 
                        if (row["param"].ToString() == "clave_sol") c_sol_sunat = row["valor"].ToString().Trim();             // clave sol portal sunat del cliente 
                        if (row["param"].ToString() == "scope") scope_sunat = row["valor"].ToString().Trim();                 // scope del api sunat
                        if (row["param"].ToString() == "rutaCertifc") rutaCertifc = row["valor"].ToString().Trim();           // Ruta y nombre del certificado .pfx
                        if (row["param"].ToString() == "claveCertif") claveCertif = row["valor"].ToString().Trim();           // Clave del certificado
                    }
                    if (row["campo"].ToString() == "rutas")
                    {
                        if (row["param"].ToString() == "grt_txt") rutatxt = row["valor"].ToString().Trim();         // ruta de los txt para las guías elect
                        if (row["param"].ToString() == "grt_xml") rutaxml = row["valor"].ToString().Trim();         // 
                    }
                }
                if (row["formulario"].ToString() == "facelect")
                {
                    if (row["campo"].ToString() == "documento" && row["param"].ToString() == "factura") codfact = row["valor"].ToString().Trim();         // 
                    if (row["campo"].ToString() == "documento" && row["param"].ToString() == "boleta") codBole = row["valor"].ToString().Trim();         // 
                    if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") codmon = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "detraccion" && row["param"].ToString() == "glosa") glosdetra = row["valor"].ToString().Trim();    // glosa detraccion
                    if (row["campo"].ToString() == "factelect" && row["param"].ToString() == "ose-pse") nipfe = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "factelect" && row["param"].ToString() == "textaut") restexto = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "factelect" && row["param"].ToString() == "autoriz") autoriz_OSE_PSE = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "factelect" && row["param"].ToString() == "despedi") despedida = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "factelect" && row["param"].ToString() == "webose") webose = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "impresion")
                    {
                        if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "filasDet") v_mfildet = row["valor"].ToString().Trim();       // maxima cant de filas de detalle
                        if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                        if (row["param"].ToString() == "forA4CRn") forA4CRn = row["valor"].ToString().Trim();           // ruta y nombre del formato CR de factura/boletas "normales"
                        if (row["param"].ToString() == "forA4CRcu") forA4CRcu = row["valor"].ToString().Trim();          // ruta y nombre del formato CR de facturas de cargas únicas
                        if (row["param"].ToString() == "rutaQR") vi_rutaQR = row["valor"].ToString().Trim();               // Ruta del archivo imagen del QR
                        if (row["param"].ToString() == "gloserA") glosser2 = row["valor"].ToString().Trim();               // glosa cuando no se jala de la tabla series
                    }
                }
                if (row["formulario"].ToString() == "clients")
                {
                    if (row["campo"].ToString() == "documento" && row["param"].ToString() == "dni") coddni = row["valor"].ToString().Trim();
                    if (row["campo"].ToString() == "documento" && row["param"].ToString() == "ruc") codruc = row["valor"].ToString().Trim();
                }
                if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0   glosser2
                {
                    //if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                    //if (row["campo"].ToString() == "codinDV" && row["param"].ToString() == "DV") v_codidv = row["valor"].ToString().Trim();           // codigo de dov.vta en tabla TDV
                    if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
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

                    DataGridViewButtonColumn btnA4 = new DataGridViewButtonColumn();
                    btnA4.HeaderText = "iA4";
                    btnA4.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnA4.Name = "iA4";
                    btnA4.Width = 60;
                    btnA4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnA4.DefaultCellStyle.Padding = padding;
                    btnA4.DefaultCellStyle.Font = chiq;
                    btnA4.DefaultCellStyle.SelectionBackColor = Color.White;

                    DataGridViewButtonColumn btnXML = new DataGridViewButtonColumn();
                    btnXML.HeaderText = "XML";
                    btnXML.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnXML.Name = "xml";
                    btnXML.Width = 60;
                    btnXML.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnXML.DefaultCellStyle.Padding = padding;
                    btnXML.DefaultCellStyle.Font = chiq;
                    btnXML.DefaultCellStyle.SelectionBackColor = Color.White;
                    /*
                    DataGridViewButtonColumn btnAct = new DataGridViewButtonColumn();
                    btnAct.HeaderText = "Sunat"; // ACTUALIZA
                    btnAct.Text = "...Consulta...";
                    btnAct.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnAct.Name = "consulta";
                    btnAct.Width = 140;
                    btnAct.UseColumnTextForButtonValue = true;
                    btnAct.DefaultCellStyle.Padding = padding;
                    */
                    // EMISION,TIPO,COMPROBANTE,ORIGEN,ESTADO,SUNAT,CDR_GEN,btnTK,btnCDR,btnACT,ad.cdr as Rspta,ad.textoQR,ad.nticket,f.canfidt,f.id,glosaser,f.totdvta,f.totdvMN,d.codgror,tipdvta
                    //     0  ,  1 ,      2    ,   3  ,  4   ,  5  ,   6   ,  7  ,  8   ,  9   ,  10   ,    11    ,   12     ,   13   , 14     ,  15,   16   ,    17   ,    18   ,   19    ,   20
                    dgv_sunat_est.CellClick += DataGridView1_CellClick;
                    dgv_sunat_est.Columns.Insert(7, btnTk);
                    dgv_sunat_est.Columns.Insert(8, btnA4);
                    //dgv_sunat_est.Columns.Insert(8, btnPDF);   // .Add(btnPDF);
                    dgv_sunat_est.Columns.Insert(9, btnCDR);   // .Add(btnCDR);
                    dgv_sunat_est.Columns.Insert(10, btnXML);   // .Add(btnAct);
                    dgv_sunat_est.Columns[11].Visible = false;
                    dgv_sunat_est.Columns[12].Visible = false;
                    dgv_sunat_est.Columns[13].Visible = false;
                    dgv_sunat_est.Columns[14].Visible = false;
                    dgv_sunat_est.Columns[15].Visible = false;
                    dgv_sunat_est.Columns[16].Visible = false;
                    dgv_sunat_est.Columns[17].Visible = false;
                    dgv_sunat_est.Columns[18].Visible = false;
                    dgv_sunat_est.Columns[19].Visible = false;
                    dgv_sunat_est.Columns[20].Visible = false;
                    if (dgv_sunat_est.Rows.Count > 0)         // autosize filas
                    {
                        for (int i = 0; i < 7; i++)     // columnas visibles del 0 al 6
                        {
                            dgv_sunat_est.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            _ = decimal.TryParse(dgv_sunat_est.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                            if (vd != 0) dgv_sunat_est.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        b = 0;
                        for (int i = 0; i < 7; i++)
                        {
                            int a = dgv_sunat_est.Columns[i].Width;
                            b += a;
                            dgv_sunat_est.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgv_sunat_est.Columns[i].Width = a;
                        }
                        if (b < dgv_sunat_est.Width) dgv_sunat_est.Width = dgv_sunat_est.Width - 11;
                        dgv_sunat_est.ReadOnly = true;
                    }
                    if (dgv_sunat_est.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgv_sunat_est.Rows.Count; i++)
                        {
                            dgv_sunat_est.Rows[i].Cells["iTK"].Value = "TK";
                            dgv_sunat_est.Rows[i].Cells["iA4"].Value = "A4";
                            dgv_sunat_est.Rows[i].Cells["xml"].Value = "XML";
                            if (dgv_sunat_est.Rows[i].Cells["iTK"].Value != null)
                            {
                                if (dgv_sunat_est.Rows[i].Cells["CDR_GEN"].Value.ToString() == "0")
                                {
                                    //dgv_sunat_est.Rows[i].Cells[8].ReadOnly = false;
                                    //dgv_sunat_est.Rows[i].Cells[8].Value = "PDF";
                                    dgv_sunat_est.Rows[i].Cells["cdr"].ReadOnly = false;
                                    dgv_sunat_est.Rows[i].Cells["cdr"].Value = "CDR";
                                    dgv_sunat_est.Rows[i].Cells["cdr"].ReadOnly = true;
                                    dgv_sunat_est.Rows[i].Cells["xml"].ReadOnly = true;
                                    dgv_sunat_est.Rows[i].Cells["xml"].Value = "XML";
                                }
                                else
                                {
                                    dgv_sunat_est.Rows[i].Cells["cdr"].ReadOnly = true;
                                    dgv_sunat_est.Rows[i].Cells["cdr"].Value = "";
                                    dgv_sunat_est.Rows[i].Cells["xml"].ReadOnly = false;
                                    dgv_sunat_est.Rows[i].Cells["xml"].Value = "";
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
            //dtsunatE.Rows.Clear();
            //dtsunatE.Columns.Clear();
            DataTable dtsunatE = new DataTable();
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
                    "es.DescrizioneRid AS ESTADO,ad.estadoS AS SUNAT,ad.cdrgener AS CDR_GEN,ad.cdr as Rspta,ad.textoQR,ad.nticket,f.canfidt,f.id," + // ,ad.ulterror as ULT_ERROR
                    "ifnull(s.glosaser,'') as glosaser,f.totdvta,f.totdvMN,'d.codgror',f.tipdvta " +
                    "FROM cabfactu f LEFT JOIN adifactu ad ON ad.idc = f.id " +
                    //"left join detfactu d on d.idc=f.id " +               // d.codgror <- no deberia estar acá, hay fact con varios detalles 12/03/2024
                    "LEFT JOIN desc_loc lo ON lo.IDCodice = f.locorig " +
                    "LEFT JOIN desc_est es ON es.IDCodice = f.estdvta  " +
                    "left join series s on s.tipdoc=f.tipdvta and s.serie=f.serdvta " +
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
                        dgv_sunat_est.CellClick -= null;
                        cuenta = -1;
                        da.Fill(dtsunatE);
                        dgv_sunat_est.DataSource = dtsunatE;
                        grilla("dgv_sunat_est");
                        dtsunatE.Dispose();
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
            if (tabControl1.SelectedTab.Name == "tabSunat")
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
            if (e.ColumnIndex > -1 && cuenta != e.RowIndex)
            {
                if (dgv_sunat_est.Columns[e.ColumnIndex].Name.ToString() == "xml")
                {
                    if (true)
                    {
                        if (dgv_sunat_est.Rows[e.RowIndex].Cells[6].Value.ToString() == "0" ||
                            dgv_sunat_est.Rows[e.RowIndex].Cells[6].Value.ToString().Trim() == "")
                        {
                            dgv_sunat_est.Rows[e.RowIndex].Cells[8].ReadOnly = true;
                            dgv_sunat_est.Rows[e.RowIndex].Cells[9].ReadOnly = true;

                            varios[0] = logoclt;                // Ruta y nombre del logo del emisor electrónico
                            varios[1] = dgv_sunat_est.Rows[e.RowIndex].Cells["glosaser"].Value.ToString();         // glosa del servicio en facturacion
                            varios[2] = codfact;                // Código Transcarga del tipo de documento Factura 
                            varios[3] = Program.pordetra;       // porcentaje detracción
                            if (double.Parse(dgv_sunat_est.Rows[e.RowIndex].Cells["totdvMN"].Value.ToString()) >= double.Parse(Program.valdetra))
                            {
                                varios[4] = (double.Parse(dgv_sunat_est.Rows[e.RowIndex].Cells["totdvMN"].Value.ToString()) * double.Parse(Program.pordetra) / 100).ToString("#0.00");         // monto detracción
                            }
                            varios[5] = Program.ctadetra;         // cta. detracción
                            varios[6] = dgv_sunat_est.Rows[e.RowIndex].Cells["codgror"].Value.ToString();         // concatenado de Guias Transportista para Formato de cargas unicas
                            varios[7] = "";             // ruta y nombre del png codigo QR
                            varios[8] = "";             // 
                            varios[9] = codmon;         // moneda por defecto MN del sistema
                            varios[10] = v_igv;         // valor igv en procentaje 
                            varios[11] = rutaxml;       // rutaxml 
                            varios[12] = rutaCertifc;   // rutaCertifc
                            varios[13] = claveCertif;   // claveCertif

                            xmlComprobantes xmlc = new xmlComprobantes();   // dgv_sunat_est.Rows[e.RowIndex].Cells["TIPO"].Value.ToString()
                            xmlc.llenaTablaLite(dgv_sunat_est.Rows[e.RowIndex].Cells["tipdvta"].Value.ToString(),
                                dgv_sunat_est.Rows[e.RowIndex].Cells["COMPROBANTE"].Value.ToString().Substring(0, 4),
                                lib.Right(dgv_sunat_est.Rows[e.RowIndex].Cells["COMPROBANTE"].Value.ToString(), 8),
                                varios);
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
                        dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(5, 8), "TK");
                }
                if (dgv_sunat_est.Columns[e.ColumnIndex].Name.ToString() == "iA4")
                {
                    string cdtip = (dgv_sunat_est.Rows[e.RowIndex].Cells[1].Value.ToString() == "F") ? codfact : codBole;
                    imprime(cdtip,
                        dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(0, 4),
                        dgv_sunat_est.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(5, 8), "A4");
                }
                cuenta = e.RowIndex;
            }
        }
        private void dgv_sunat_est_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            cuenta = -1;
        }
        #endregion

        private void imprime(string tipo,string serie, string numero, string Formato)
        {
            string[] vs = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",      // 21
                           "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};     // 21
            string[] va = { "", "", "", "", "", "", "", "", "", "", "" };       // 11
            string[,] dt = new string[10, 10] {
                    { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" },
                    { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "", "" }
            }; // 6 columnas, 10 filas
            string[] cu = { "","","","","","","","","","","","","","","","","", ""};    // 18
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                string mcu = "";        // marca de carga unica
                string vce = "";        // carga efectiva
                string gse = "";        // glosa de servicio
                double pigv = 0;
                string mCmpte = "";        // moneda del comprobante
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select a.id,DATE_FORMAT(a.fechope,'%d/%m/%Y') AS fechope,a.martdve,a.tipdvta,a.serdvta,a.numdvta,a.ticltgr,a.tidoclt,a.nudoclt,a.nombclt,a.direclt,a.dptoclt,a.provclt,a.distclt,a.ubigclt,a.corrclt,a.teleclt," +
                        "a.locorig,a.dirorig,a.ubiorig,a.obsdvta,a.canfidt,a.canbudt,a.mondvta,a.tcadvta,round(a.subtota,2) as subtota,round(a.igvtota,2) as igvtota,a.porcigv,round(a.totdvta,2) as totdvta,round(a.totpags,2) as totpags,round(a.saldvta,2) as saldvta,a.estdvta,a.frase01,a.impreso,d.codsunat as ctdcl," +
                        "a.tipoclt,a.m1clien,a.tippago,a.ferecep,a.userc,a.fechc,a.userm,a.fechm,b.descrizionerid as nomest,ifnull(c.id,'') as cobra,a.idcaja,a.plazocred,round(a.totdvMN,2) as totdvMN,ifnull(p.marca1,'') as dpc,ifnull(s.glosaser,'') as glosaser," +
                        "a.cargaunica,a.porcendscto,round(a.valordscto,2) as valordscto,a.conPago,a.pagauto,ifnull(ad.placa,'') as placa,ifnull(ad.confv,'') as confv,ifnull(ad.autoriz,'') as autoriz,m.descrizionerid as inimon,t.codsunat as cdtdv," +
                        "ifnull(ad.cargaEf,0) as cargaEf,ifnull(ad.cargaUt,0) as cargaUt,ifnull(ad.rucTrans,'') as rucTrans,ifnull(ad.nomTrans,'') as nomTrans,ifnull(date_format(ad.fecIniTras,'%Y-%m-%d'),'') as fecIniTras," +
                        "ifnull(ad.dirPartida,'') as dirPartida,ifnull(ad.ubiPartida,'') as ubiPartida,ifnull(ad.dirDestin,'') as dirDestin,ifnull(ad.ubiDestin,'') as ubiDestin,ifnull(ad.dniChof,'') as dniChof," +
                        "ifnull(ad.brevete,'') as brevete,ifnull(ad.valRefViaje,0) as valRefViaje,ifnull(ad.valRefVehic,0) as valRefVehic,ifnull(ad.valRefTon,0) as valRefTon,l.descrizionerid as nomLocO,concat(l.deta1,' ',l.deta4,'-',l.deta3,'-',l.deta2) as dirSuc," +
                        "if(a.plazocred='',DATE_FORMAT(a.fechope,'%d/%m/%Y'),DATE_FORMAT(date_add(a.fechope, interval p.marca1 day),'%d/%m/%Y')) as fvence,if(a.plazocred='','Contado','Credito - N° Cuotas : 1') as condicion," +
                        "m.deta1 as nonmone,a.mpsdet,ifnull(ps.descrizione,'') as mpsTex,ifnull(v.numreg1,'') as numreg1 " +
                        "from cabfactu a " +
                        "left join adifactu ad on ad.idc=a.id and ad.tipoAd=1 " +
                        "left join desc_est b on b.idcodice=a.estdvta " +
                        "left join desc_mon m on m.idcodice=a.mondvta " +
                        "left join desc_tpa p on p.idcodice=a.plazocred " +
                        "left join desc_tdv t on t.idcodice=a.tipdvta " +
                        "left join desc_doc d on d.idcodice=a.tidoclt " +
                        "left join desc_loc l on l.idcodice=a.locorig " +
                        "left join desc_mps ps on ps.idcodice=a.mpsdet " +
                        "left join series s on s.tipdoc=a.tipdvta and s.serie=a.serdvta " +
                        "left join cabcobran c on c.tipdoco=a.tipdvta and c.serdoco=a.serdvta and c.numdoco=a.numdvta and c.estdcob<>@coda " +
                        "left join vehiculos v on v.placa=ad.placa " +
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
                                    vs[0] = dr.GetString("martdve") + lib.Right(serie,3);                          // serie (F001)
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
                                    vs[16] = dr.GetString("inimon"); ;       // Simbolo de la moneda
                                    vs[17] = nlet.Convertir(dr.GetString("totdvta"),true) + ((dr.GetString("mondvta") == codmon) ? " SOLES" : " DOLARES AMERICANOS");                  // flete en letras
                                    vs[18] = (dr.GetString("tippago").Trim() != "" && dr.GetString("plazocred").Trim() == "") ? "CONTADO" : "CREDITO";
                                    vs[19] = (dr.GetString("plazocred") != "") ? dr.GetString("dpc") : "";  // dias de plazo credito
                                    vs[20] = (dr.GetDouble("totdvMN") >= double.Parse(Program.valdetra))? glosdetra : "";   // Glosa para la detracción SI TIENE
                                    vs[21] = dr.GetString("cdtdv");         // codigo sunat tipo comprobante
                                    vs[22] = dr.GetString("ctdcl");         // CODIGO SUNAT tipo de documento RUC/DNI del cliente
                                    vs[23] = nipfe;                         // identificador de ose/pse metodo de envío
                                    vs[24] = restexto;                      // texto del resolucion sunat del ose/pse
                                    vs[25] = autoriz_OSE_PSE;               // autoriz del ose/pse
                                    vs[26] = webose;                        // web del ose/pse
                                    vs[27] = dr.GetString("userc").Trim();  // usuario creador
                                    vs[28] = dr.GetString("nomLocO").Trim();    // local de emisión
                                    vs[29] = despedida;                     // glosa despedida
                                    vs[30] = Program.cliente;               // nombre del emisor del comprobante
                                    vs[31] = Program.ruc;                   // ruc del emisor
                                    vs[32] = dr.GetString("fvence");        // fecha vencimiento del comprob.
                                    vs[33] = dr.GetString("condicion");     // forma de pago incluyendo # de cuotas (siempre es 1 cuota en Transcarga)
                                    vs[34] = "Transporte Privado";          // modalidad de transporte
                                    vs[35] = "Venta";                       // motivo de traslado
                                    vs[36] = dr.GetString("nonmone");       // nombre de la moneda
                                    vs[37] = "0";                           // tot operaciones inafectas
                                    vs[38] = "0";                           // tot operaciones exoneradas
                                    // carga unica
                                    cu[0] = dr.GetString("placa");
                                    cu[1] = dr.GetString("confv");
                                    cu[2] = dr.GetString("autoriz");
                                    cu[3] = dr.GetString("cargaEf");
                                    cu[4] = dr.GetString("cargaUt");
                                    cu[5] = dr.GetString("rucTrans");
                                    cu[6] = dr.GetString("nomTrans");
                                    cu[7] = dr.GetString("fecIniTras");
                                    cu[8] = dr.GetString("dirPartida");
                                    cu[9] = dr.GetString("ubiPartida");
                                    cu[10] = dr.GetString("dirDestin");
                                    cu[11] = dr.GetString("ubiDestin");
                                    cu[12] = dr.GetString("dniChof");
                                    cu[13] = dr.GetString("brevete");
                                    cu[14] = dr.GetString("valRefViaje");
                                    cu[15] = dr.GetString("valRefVehic");
                                    cu[16] = dr.GetString("valRefTon");
                                    cu[17] = dr.GetString("numreg1");
                                    // varios
                                    glosser = dr.GetString("glosaser");
                                    if (glosser == "") glosser = glosser2; 
                                    va[0] = logoclt;                    // Ruta y nombre del logo del emisor electrónico
                                    va[1] = glosser;                    // glosa del servicio en facturacion
                                    va[2] = codfact;                    // Código Transcarga del tipo de documento Factura 
                                    va[3] = Program.pordetra;           // porcentaje detracción
                                    double impDetr = 0;
                                    if (cu[14] == "0.00") impDetr = Math.Round(dr.GetDouble("totdvMN") * double.Parse(Program.pordetra) / 100, 0);               // importe calculado de la detracción
                                    impDetr = Math.Round(double.Parse(cu[14]) * double.Parse(Program.pordetra) / 100, 0);
                                    va[4] = impDetr.ToString("#0.00");
                                    va[5] = Program.ctadetra;         // cta. detracción
                                    va[6] = "";         // concatenado de Guias Transportista para Formato de cargas unicas
                                    va[7] = vi_rutaQR + "pngqr";         // ruta y nombre del png codigo QR
                                    va[8] = dr.GetString("mpsTex");     // medio de pago sunat de la detracción
                                    va[9] = dr.GetString("tcadvta");    // tipo de cambio
                                    va[10] = (dr.GetString("estdvta") == codAnul) ? dr.GetString("nomest") : "";

                                    mcu = dr.GetString("cargaunica");
                                    vce = dr.GetString("cargaEf");
                                    gse = glosser;
                                    pigv = dr.GetDouble("porcigv");
                                    mCmpte = dr.GetString("mondvta");
                                    //
                                    double valCuot = 0;                     // valor de la cuota SI ES CREDITO
                                    if (vs[20] == "" && vs[18] == "CREDITO") valCuot = dr.GetDouble("totdvta");
                                    else
                                    {
                                        if (dr.GetString("mondvta") == codmon)      // comprobante en soles?
                                        {
                                            valCuot = dr.GetDouble("totdvta") - impDetr;
                                        }
                                        else
                                        {
                                            valCuot = Math.Round(dr.GetDouble("totdvta") - (impDetr / double.Parse(va[9])), 2);
                                        }
                                    }
                                    vs[39] = valCuot.ToString("#0.00");
                                    vs[40] = dr.GetString("dirSuc");        // direccion de la sucursal
                                    vs[41] = dr.GetString("obsdvta");       // observaciones del comprobante
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
                    {
                        int y = 0;
                        string consdeta = "";
                        if (mCmpte == codmon)
                        {
                            consdeta = "select a.codgror,a.cantbul,ifnull(b.unimedpro, '') as unimedp,a.descpro,a.pesogro,ifnull(b.docsremit, '') as docsremit," +
                                "round(a.totalgr, 2) as totalgr,round(a.totalgr, 2) as preUni,round(a.totalgr / (1 + (@pigv / 100)), 2) as valUni,ifnull(concat(dl.DescrizioneRid, '-', dd.DescrizioneRid),'') AS orides " +
                                "from detfactu a " +
                                "left JOIN(SELECT x.sergui, x.numgui, x.docsremit, y.unimedpro, x.locorigen, x.locdestin " +
                                "from cabguiai x LEFT JOIN detguiai y ON x.id = y.idc " +
                                "WHERE x.tipdocvta = @tdv AND x.serdocvta = @ser AND x.numdocvta = @num)b on concat(b.sergui, '-', b.numgui) = a.codgror " +
                                "LEFT JOIN desc_loc dl ON dl.IDCodice = b.locorigen " +
                                "LEFT JOIN desc_loc dd ON dd.IDCodice = b.locdestin " +
                                "where a.tipdocvta = @tdv and a.serdvta = @ser and a.numdvta = @num";
                        }
                        else
                        {
                            // EL DETALLE TIENE QUE SER EN DOLARES
                            consdeta = "select a.codgror,a.cantbul,ifnull(b.unimedpro, '') as unimedp,a.descpro,a.pesogro,ifnull(b.docsremit, '') as docsremit," +
                                "round(a.totalgrMN/@tc, 2) as totalgr,round(a.totalgrMN/@tc, 2) as preUni,round(round(a.totalgr/@tc,2) / (1 + (@pigv / 100)),2) as valUni," +
                                "ifnull(concat(dl.DescrizioneRid, '-', dd.DescrizioneRid),'') AS orides " +
                                "from detfactu a " +
                                "left JOIN(SELECT x.sergui, x.numgui, x.docsremit, y.unimedpro, x.locorigen, x.locdestin " +
                                "from cabguiai x LEFT JOIN detguiai y ON x.id = y.idc " +
                                "WHERE x.tipdocvta = @tdv AND x.serdocvta = @ser AND x.numdocvta = @num)b on concat(b.sergui, '-', b.numgui) = a.codgror " +
                                "LEFT JOIN desc_loc dl ON dl.IDCodice = b.locorigen " +
                                "LEFT JOIN desc_loc dd ON dd.IDCodice = b.locdestin " +
                                "where a.tipdocvta = @tdv and a.serdvta = @ser and a.numdvta = @num";
                        }
                        using (MySqlCommand micomd = new MySqlCommand(consdeta, conn))
                        {
                            micomd.Parameters.AddWithValue("@ser", serie);
                            micomd.Parameters.AddWithValue("@num", numero);
                            micomd.Parameters.AddWithValue("@tdv", tipo);
                            micomd.Parameters.AddWithValue("@pigv", pigv);          // % igv del comprobante
                            if (mCmpte != codmon) micomd.Parameters.AddWithValue("@tc", va[9]);
                            using (MySqlDataReader drg = micomd.ExecuteReader())
                            {
                                while (drg.Read())  // #fila,a.cantprodi,a.unimedpro,a.descprodi,a.pesoprodi
                                {
                                    //dt[y, 0] = (y + 1).ToString();
                                    dt[y, 0] = drg.GetString("orides");
                                    dt[y, 1] = drg.GetString("cantbul");
                                    dt[y, 2] = drg.GetString("unimedp");
                                    dt[y, 3] = drg.GetString("codgror");             // guia transportista
                                    dt[y, 4] = drg.GetString("descpro");             // descripcion de la carga
                                    dt[y, 5] = drg.GetString("docsremit");           // documento relacionado remitente de la guia transportista
                                    dt[y, 6] = drg.GetString("valUni");             // valor unitario
                                    dt[y, 7] = drg.GetString("preUni");             // precio unitario
                                    dt[y, 8] = drg.GetString("totalgr");            // total
                                    dt[y, 9] = drg.GetString("pesogro");             // peso
                                    va[6] = va[6] + " " + drg.GetString("codgror");
                                    //
                                    if (mcu == "1" && Formato == "A4")
                                    {
                                        dt[y, 4] = glosser + " " + dt[y, 0] + ", " + dt[y, 1] + " " + dt[y, 2] + " " + dt[y, 4] + " Según doc.cliente: " + dt[y, 5];     // descripcion de la carga
                                        dt[y, 1] = Math.Round(double.Parse(dt[y, 9])/1000,2).ToString("#0.00");   // cantidad
                                        dt[y, 2] = "TONELADA";                          // unidad de medida
                                        double pu = Math.Round(double.Parse(dt[y, 8]) / (double.Parse(dt[y, 9]) / 1000), 2);
                                        dt[y, 6] = (pu / (1 + (double.Parse(v_igv) / 100))).ToString("#0.00000");         // valor unitario 
                                        dt[y, 7] = pu.ToString("#0.00");                // precio unitario
                                    }
                                    y += 1;
                                }
                            }
                        }
                    }
                    // llamamos a la clase que imprime
                    if (Formato == "A4") 
                    {
                        if (cu[0] != "") { impDV imp = new impDV(1, "", vs, dt, va, cu, Formato, forA4CRcu); }  // vistas en pantalla
                        else { impDV imp = new impDV(1, "", vs, dt, va, cu, Formato, forA4CRn); }   // vistas en pantalla
                    }
                    else
                    {
                        impDV imp = new impDV(1, v_impTK, vs, dt, va, cu, Formato, "");
                    }
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
