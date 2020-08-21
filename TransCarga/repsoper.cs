using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;

namespace TransCarga
{
    public partial class repsoper : Form
    {
        static string nomform = "repsoper";           // nombre del formulario
        string asd = TransCarga.Program.vg_user;      // usuario conectado al sistema
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colstrp = TransCarga.Program.colstr;   // color del strip
        static string nomtab = "contrat";         // 
        public int totfilgrid, cta;             // variables para impresion
        public string perAg = "";
        public string perMo = "";
        public string perAn = "";
        public string perIm = "";
        string tipede = "";
        string tiesta = "";
        string img_btN = "";
        string img_btE = "";
        string img_btP = "";
        string img_btA = "";            // anula = bloquea
        string img_btexc = "";          // exporta a excel
        string img_btq = "";
        string img_grab = "";
        string img_anul = "";
        string img_imprime = "", img_preview = "";        // imagen del boton preview e imprimir reporte
        string letpied = "";            // letra indentificadora de piedra en detalle 2
        string cliente = Program.cliente;    // razon social para los reportes
        int pageCount = 1, cuenta = 0;
        libreria lib = new libreria();
        // string de conexion
        //static string serv = ConfigurationManager.AppSettings["serv"].ToString();
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        //static string usua = ConfigurationManager.AppSettings["user"].ToString();
        //static string cont = ConfigurationManager.AppSettings["pass"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";

        public repsoper()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)    // F1
        {
            string para1 = "";
            string para2 = "";
            string para3 = "";
            string para4 = "";
            if (keyData == Keys.F1)
            {
                if (tx_nomclie.Focused == true && rb_listado.Checked == true) 
                {
                    /*
                    para1 = "anag_cli";
                    para2 = "todos";
                    ayuda2 ayu2 = new ayuda2(para1, para2, para3, para4);
                    var result = ayu2.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        if (!string.IsNullOrEmpty(ayu2.ReturnValue1))
                        {
                            tx_doclie.Text = ayu2.ReturnValue1;
                            tx_idclie.Text = ayu2.ReturnValue0;      // id del cliente
                            tx_nomclie.Text = ayu2.ReturnValue2;
                        }
                    }
                    */
                }
                return true;    // indicate that you handled this keystroke
            }
            // Call the base class
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
            init();
            toolboton();
            dataload("todos");
            KeyPreview = true;
            tabControl1.Enabled = false;
            //
            tx_codped.CharacterCasing = CharacterCasing.Upper;
            tx_codped.TextAlign = HorizontalAlignment.Center;
        }
        private void init()
        {
            BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            tabControl1.BackColor = Color.FromName(TransCarga.Program.colgri);

            jalainfo();
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_print.Image = Image.FromFile(img_btP);
            Bt_anul.Image = Image.FromFile(img_btA);
            bt_exc.Image = Image.FromFile(img_btexc);
            Bt_close.Image = Image.FromFile(img_btq);
        }
        private void jalainfo()                                     // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in(@nofo,@ped)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@ped", "xxx");
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
                    if (row["formulario"].ToString() == "xxx")
                    {
                        if (row["campo"].ToString() == "tipoped" && row["param"].ToString() == "almacen") tipede = row["valor"].ToString().Trim();         // tipo de pedido por defecto en almacen
                        if (row["campo"].ToString() == "estado" && row["param"].ToString() == "default") tiesta = row["valor"].ToString().Trim();         // estado del pedido inicial
                        if (row["campo"].ToString() == "detalle2" && row["param"].ToString() == "piedra") letpied = row["valor"].ToString().Trim();         // letra identificadora de Piedra en Detalle2
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
                // seleccion de taller de produccion ... ok
                const string contaller = "select descrizionerid,idcodice,codigo from desc_loc " +
                                       "where numero=1 order by idcodice";
                MySqlCommand cmdtaller = new MySqlCommand(contaller, conn);
                MySqlDataAdapter dataller = new MySqlDataAdapter(cmdtaller);
                DataTable dttaller = new DataTable();
                dataller.Fill(dttaller);
                foreach (DataRow row in dttaller.Rows)
                {
                    //cmb_pedtaller.Items.Add(row.ItemArray[1].ToString().PadRight(6).Substring(0, 6) + " - " + row.ItemArray[0].ToString());
                    //cmb_pedtaller.ValueMember = row.ItemArray[1].ToString();
                }
                // seleccion del almacen de destino ... 
                const string condest = "select descrizionerid,idcodice from desc_alm " +
                                       "where numero=1 order by idcodice";
                MySqlCommand cmddest = new MySqlCommand(condest, conn);
                DataTable dtdest = new DataTable();
                MySqlDataAdapter dadest = new MySqlDataAdapter(cmddest);
                dadest.Fill(dtdest);
                foreach (DataRow row in dtdest.Rows)
                {
                    /*
                    cmb_destino.Items.Add(row.ItemArray[1].ToString() + " - " + row.ItemArray[0].ToString());
                    cmb_destino.ValueMember = row.ItemArray[1].ToString();
                    //
                    cmb_dest_ing.Items.Add(row.ItemArray[1].ToString() + " - " + row.ItemArray[0].ToString());
                    cmb_dest_ing.ValueMember = row.ItemArray[1].ToString();
                    */
                }
                // seleccion del estado
                const string conestado = "select descrizionerid,idcodice from desc_stp " +
                                       "where numero=1 order by idcodice";
                MySqlCommand cmdestado = new MySqlCommand(conestado, conn);
                DataTable dtestado = new DataTable();
                MySqlDataAdapter daestado = new MySqlDataAdapter(cmdestado);
                daestado.Fill(dtestado);
                foreach (DataRow row in dtestado.Rows)
                {
                    /*
                    cmb_estado.Items.Add(row.ItemArray[1].ToString() + " - " + row.ItemArray[0].ToString());
                    cmb_estado.ValueMember = row.ItemArray[1].ToString();
                    //
                    cmb_estad_ing.Items.Add(row.ItemArray[1].ToString() + " - " + row.ItemArray[0].ToString());
                    cmb_estad_ing.ValueMember = row.ItemArray[1].ToString();
                    */
                }
                // seleccion del estado del contrato
                const string conestcont = "select descrizionerid,idcodice from desc_sta " +
                                       "where numero=1 order by idcodice";
                MySqlCommand cmdestcont = new MySqlCommand(conestcont, conn);
                DataTable dtestcont = new DataTable();
                MySqlDataAdapter daestcont = new MySqlDataAdapter(cmdestcont);
                daestcont.Fill(dtestcont);
                foreach (DataRow row in dtestcont.Rows)
                {
                    //cmb_conestado.Items.Add(row.ItemArray[1].ToString() + " - " + row.ItemArray[0].ToString());
                    //cmb_conestado.ValueMember = row.ItemArray[1].ToString();
                }
                // seleccion del local de ventas
                const string conlocven = "select descrizionerid,idcodice from desc_ven " +
                                       "where numero=1 order by idcodice";
                MySqlCommand cmdlocven = new MySqlCommand(conlocven, conn);
                DataTable dtlocven = new DataTable();
                MySqlDataAdapter dalocven = new MySqlDataAdapter(cmdlocven);
                dalocven.Fill(dtlocven);
                foreach (DataRow row in dtlocven.Rows)
                {
                    cmb_vtasloc.Items.Add(row.ItemArray[1].ToString() + " - " + row.ItemArray[0].ToString());
                    cmb_vtasloc.ValueMember = row.ItemArray[1].ToString();
                }
            }
            //
            conn.Close();
        }
        private void grilla(string dgv)                             // arma la grilla salidas
        {
            switch (dgv)
            {
                case "dgv_vtas":
                    Font tiplg = new Font("Arial", 7, FontStyle.Bold);
                    dgv_vtas.Font = tiplg;
                    dgv_vtas.DefaultCellStyle.Font = tiplg;
                    dgv_vtas.RowTemplate.Height = 15;
                    dgv_vtas.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    dgv_vtas.AllowUserToAddRows = false;
                    dgv_vtas.Width = 1015;
                    if (dgv_vtas.DataSource == null) dgv_vtas.ColumnCount = 11;
                    //
                    for (int i = 0; i < dgv_vtas.Columns.Count; i++)
                    {
                        dgv_vtas.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        _ = decimal.TryParse(dgv_vtas.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                        if (vd != 0) dgv_vtas.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    int b = 0;
                    for (int i = 0; i < dgv_vtas.Columns.Count; i++)
                    {
                        int a = dgv_vtas.Columns[i].Width;
                        b += a;
                        dgv_vtas.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dgv_vtas.Columns[i].Width = a;
                    }
                    if (b < dgv_vtas.Width) dgv_vtas.Width = b + 60;
                    break;
            }
        }
        private void grillares()                                    // arma la grilla del resumen de contrato
        {
            Font tiplg = new Font("Arial", 7, FontStyle.Bold);
            dgv_resumen.Font = tiplg;
            dgv_resumen.DefaultCellStyle.Font = tiplg;
            dgv_resumen.RowTemplate.Height = 15;
            dgv_resumen.EnableHeadersVisualStyles = false;
            //dgv_resumen.DefaultCellStyle.BackColor = Color.MediumAquamarine;
            dgv_resumen.AllowUserToAddRows = false;
            if (dgv_resumen.DataSource == null) dgv_resumen.ColumnCount = 22;
            // id,codigo,nombre,madera,CanC,sep_id,sep_fecha,sep_almac,canS,ent_id,ent_fecha,canE,
            // codped,ped_fecha,canP,ing_id,ing_fecha,canI,sal_id,sal_fecha,canA
            // id
            dgv_resumen.Columns[0].Visible = false;
            // codigo
            dgv_resumen.Columns[1].Visible = true;
            dgv_resumen.Columns[1].HeaderText = "Código";    // titulo de la columna
            dgv_resumen.Columns[1].Width = 140;                // ancho
            dgv_resumen.Columns[1].ReadOnly = true;           // lectura o no
            dgv_resumen.Columns[1].Tag = "validaNO";
            dgv_resumen.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // nombre
            dgv_resumen.Columns[2].Visible = true;            // columna visible o no
            dgv_resumen.Columns[2].HeaderText = "Nombre";    // titulo de la columna
            dgv_resumen.Columns[2].Width = 200;                // ancho
            dgv_resumen.Columns[2].ReadOnly = true;           // lectura o no
            dgv_resumen.Columns[2].Tag = "validaNO";
            dgv_resumen.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // madera
            dgv_resumen.Columns[3].Visible = true;
            dgv_resumen.Columns[3].HeaderText = "Madera";    // titulo de la columna
            dgv_resumen.Columns[3].Width = 60;                // ancho
            dgv_resumen.Columns[3].ReadOnly = true;           // lectura o no
            dgv_resumen.Columns[3].Tag = "validaNO";
            //dgv_resumen.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // cantidad contrato
            dgv_resumen.Columns[4].Visible = true;
            dgv_resumen.Columns[4].HeaderText = "Cant";
            dgv_resumen.Columns[4].Width = 50;
            dgv_resumen.Columns[4].ReadOnly = true;          // las celdas de esta columna pueden cambiarse
            dgv_resumen.Columns[4].Tag = "validaNO";          // las celdas de esta columna se SI se validan
            //dgv_resumen.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // SEPARACION - Id
            dgv_resumen.Columns[5].Visible = true;
            dgv_resumen.Columns[5].HeaderText = "Id";
            dgv_resumen.Columns[5].Width = 50;
            dgv_resumen.Columns[5].ReadOnly = true;          // las celdas de esta columna pueden cambiarse
            dgv_resumen.Columns[5].Tag = "validaNO";          // las celdas de esta columna se validan
            dgv_resumen.Columns[5].DefaultCellStyle.BackColor = Color.Aquamarine;
            dgv_resumen.Columns[5].HeaderCell.Style.BackColor = Color.Aquamarine;
            //dgv_resumen.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // SEPARACION - Fecha
            dgv_resumen.Columns[6].Visible = true;
            dgv_resumen.Columns[6].HeaderText = "RESERVA Fecha";
            dgv_resumen.Columns[6].Width = 70;
            dgv_resumen.Columns[6].ReadOnly = true;
            dgv_resumen.Columns[6].Tag = "validaNO";          // las celdas de esta columna se NO se validan
            dgv_resumen.Columns[6].DefaultCellStyle.BackColor = Color.Aquamarine;
            dgv_resumen.Columns[6].HeaderCell.Style.BackColor = Color.Aquamarine;
            //dgv_resumen.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // SEPARACION - Almacen
            dgv_resumen.Columns[7].Visible = true;
            dgv_resumen.Columns[7].HeaderText = "Almacen";
            dgv_resumen.Columns[7].Width = 70;
            dgv_resumen.Columns[7].ReadOnly = true;
            dgv_resumen.Columns[7].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[7].DefaultCellStyle.BackColor = Color.Aquamarine;
            dgv_resumen.Columns[7].HeaderCell.Style.BackColor = Color.Aquamarine;
            //dgv_resumen.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // SEPARACION - Cant
            dgv_resumen.Columns[8].Visible = true;
            dgv_resumen.Columns[8].HeaderText = "Cant";
            dgv_resumen.Columns[8].Width = 50;
            dgv_resumen.Columns[8].ReadOnly = true;
            dgv_resumen.Columns[8].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[8].DefaultCellStyle.BackColor = Color.Aquamarine;
            dgv_resumen.Columns[8].HeaderCell.Style.BackColor = Color.Aquamarine;
            //dgv_resumen.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // ENTREGAS - Id
            dgv_resumen.Columns[9].Visible = true;
            dgv_resumen.Columns[9].HeaderText = "Id";
            dgv_resumen.Columns[9].Width = 50;
            dgv_resumen.Columns[9].ReadOnly = true;
            dgv_resumen.Columns[9].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[9].DefaultCellStyle.BackColor = Color.Beige;
            dgv_resumen.Columns[9].HeaderCell.Style.BackColor = Color.Beige;
            //dgv_resumen.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // ENTREGAS - Fecha
            dgv_resumen.Columns[10].Visible = true;
            dgv_resumen.Columns[10].HeaderText = "ENTREGA Fecha";
            dgv_resumen.Columns[10].Width = 70;
            dgv_resumen.Columns[10].ReadOnly = true;
            dgv_resumen.Columns[10].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[10].DefaultCellStyle.BackColor = Color.Beige;
            dgv_resumen.Columns[10].HeaderCell.Style.BackColor = Color.Beige;
            //dgv_resumen.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // ENTREGAS - Cant
            dgv_resumen.Columns[11].Visible = true;
            dgv_resumen.Columns[11].HeaderText = "Cant";
            dgv_resumen.Columns[11].Width = 50;
            dgv_resumen.Columns[11].ReadOnly = true;
            dgv_resumen.Columns[11].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[11].DefaultCellStyle.BackColor = Color.Beige;
            dgv_resumen.Columns[11].HeaderCell.Style.BackColor = Color.Beige;
            //dgv_resumen.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // PEDIDOS - taller
            dgv_resumen.Columns[12].Visible = true;
            dgv_resumen.Columns[12].HeaderText = "Taller";
            dgv_resumen.Columns[12].Width = 50;
            dgv_resumen.Columns[12].ReadOnly = true;
            dgv_resumen.Columns[12].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[12].DisplayIndex = 12;
            dgv_resumen.Columns[12].DefaultCellStyle.BackColor = Color.Coral;
            dgv_resumen.Columns[12].HeaderCell.Style.BackColor = Color.Coral;
            // PEDIDOS - Pedido
            dgv_resumen.Columns[13].Visible = true;
            dgv_resumen.Columns[13].HeaderText = "Pedido";
            dgv_resumen.Columns[13].Width = 70;
            dgv_resumen.Columns[13].ReadOnly = true;
            dgv_resumen.Columns[13].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[13].DefaultCellStyle.BackColor = Color.Coral;
            dgv_resumen.Columns[13].HeaderCell.Style.BackColor = Color.Coral;
            //dgv_resumen.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // PEDIDOS - Fecha
            dgv_resumen.Columns[14].Visible = true;
            dgv_resumen.Columns[14].HeaderText = "PEDIDOS Fecha";
            dgv_resumen.Columns[14].Width = 70;
            dgv_resumen.Columns[14].ReadOnly = true;
            dgv_resumen.Columns[14].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[14].DefaultCellStyle.BackColor = Color.Coral;
            dgv_resumen.Columns[14].HeaderCell.Style.BackColor = Color.Coral;
            //dgv_resumen.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // PEDIDOS - Cant
            dgv_resumen.Columns[15].Visible = true;
            dgv_resumen.Columns[15].HeaderText = "Cant";
            dgv_resumen.Columns[15].Width = 50;
            dgv_resumen.Columns[15].ReadOnly = true;
            dgv_resumen.Columns[15].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[15].DefaultCellStyle.BackColor = Color.Coral;
            dgv_resumen.Columns[15].HeaderCell.Style.BackColor = Color.Coral;
            // INGRESOS - Id4
            dgv_resumen.Columns[16].Visible = true;
            dgv_resumen.Columns[16].HeaderText = " Id";
            dgv_resumen.Columns[16].Width = 50;
            dgv_resumen.Columns[16].ReadOnly = true;
            dgv_resumen.Columns[16].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[16].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
            dgv_resumen.Columns[16].HeaderCell.Style.BackColor = Color.DeepSkyBlue;
            // INGRESOS - Fecha
            dgv_resumen.Columns[17].Visible = true;
            dgv_resumen.Columns[17].HeaderText = "INGRESOS Fecha";
            dgv_resumen.Columns[17].Width = 70;
            dgv_resumen.Columns[17].ReadOnly = true;
            dgv_resumen.Columns[17].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[17].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
            dgv_resumen.Columns[17].HeaderCell.Style.BackColor = Color.DeepSkyBlue;
            // INGRESOS - Cant
            dgv_resumen.Columns[18].Visible = true;
            dgv_resumen.Columns[18].HeaderText = "Cant";
            dgv_resumen.Columns[18].Width = 50;
            dgv_resumen.Columns[18].ReadOnly = true;
            dgv_resumen.Columns[18].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[18].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
            dgv_resumen.Columns[18].HeaderCell.Style.BackColor = Color.DeepSkyBlue;
            // ENTREGAS - Id
            dgv_resumen.Columns[19].Visible = true;
            dgv_resumen.Columns[19].HeaderText = " Id";
            dgv_resumen.Columns[19].Width = 50;
            dgv_resumen.Columns[19].ReadOnly = true;
            dgv_resumen.Columns[19].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[19].DefaultCellStyle.BackColor = Color.Green;
            dgv_resumen.Columns[19].HeaderCell.Style.BackColor = Color.Green;
            // ENTREGAS - Fecha
            dgv_resumen.Columns[20].Visible = true;
            dgv_resumen.Columns[20].HeaderText = "SALIDAS Fecha";
            dgv_resumen.Columns[20].Width = 70;
            dgv_resumen.Columns[20].ReadOnly = true;
            dgv_resumen.Columns[20].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[20].DefaultCellStyle.BackColor = Color.Green;
            dgv_resumen.Columns[20].HeaderCell.Style.BackColor = Color.Green;
            // ENTREGAS - Cant
            dgv_resumen.Columns[21].Visible = true;
            dgv_resumen.Columns[21].HeaderText = "Cant";
            dgv_resumen.Columns[21].Width = 50;
            dgv_resumen.Columns[21].ReadOnly = true;
            dgv_resumen.Columns[21].Tag = "validaNO";          // las celdas de esta columna SI se validan
            dgv_resumen.Columns[21].DefaultCellStyle.BackColor = Color.Green;
            dgv_resumen.Columns[21].HeaderCell.Style.BackColor = Color.Green;
        }
        //
        private void bt_vtasfiltra_Click(object sender, EventArgs e)    // filtra y muestra ventas
        {
            string consulta = "";
            if (rb_listado.Checked == true && tx_nomclie.Text.Trim() != "")         // reporte de ventas por cliente
            {
                consulta = "repvtasxclt";
                try
                {
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        dgv_vtas.DataSource = null;
                        MySqlCommand micon = new MySqlCommand(consulta, conn);
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@idclte", tx_idclie.Text.Trim());
                        micon.Parameters.AddWithValue("@fecini", dtp_vtasfini.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fecfin", dtp_vtasfina.Value.ToString("yyyy-MM-dd"));
                        MySqlDataAdapter da = new MySqlDataAdapter(micon);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            dgv_vtas.DataSource = dt;
                            grilla("dgv_vtas");
                        }
                        else dgv_vtas.DataSource = null;
                        //dt.Dispose();
                        da.Dispose();
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
                consulta = "repventas";                          // CALL repventas('2019-07-01','2019-08-30','listado','');
                try
                {
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        dgv_vtas.DataSource = null;
                        MySqlCommand micon = new MySqlCommand(consulta, conn);
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@fecini", dtp_vtasfini.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@fecfin", dtp_vtasfina.Value.ToString("yyyy-MM-dd"));
                        micon.Parameters.AddWithValue("@tienda", tx_dat_vtasloc.Text.Trim());
                        if (rb_listado.Checked == true) micon.Parameters.AddWithValue("@modo", "listado");
                        if (rb_resumen.Checked == true) micon.Parameters.AddWithValue("@modo", "resumen");
                        MySqlDataAdapter da = new MySqlDataAdapter(micon);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_vtas.DataSource = dt;
                        if (dt.Rows.Count > 0) 
                        { 
                            dgv_vtas.DataSource = dt;
                            grilla("dgv_vtas");
                        }
                        else dgv_vtas.DataSource = null;
                        da.Dispose();
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
        }
        private void tx_codped_Leave(object sender, EventArgs e)    // valida existencia de contrato
        {
            if(tx_codped.Text != "")
            {
                lib.estcont(tx_codped.Text.Trim());
                try
                {
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string consu = "select a.id,a.fecha,a.tipoes,a.cliente,a.valor,a.status,b.ruc,b.razonsocial,a.entrega " +
                            "from contrat a left join anag_cli b on b.idanagrafica=a.cliente " +
                            "where a.contrato=@ped";
                        MySqlCommand micon = new MySqlCommand(consu, conn);
                        micon.Parameters.AddWithValue("@ped", tx_codped.Text);
                        MySqlDataReader dr = micon.ExecuteReader();
                        if (dr.Read())
                        {
                            if(dr[0] == null)
                            {
                                MessageBox.Show("No existe el contrato!", "Atención verifique", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                tx_codped.Text = "";
                                tx_cliente.Text = "";
                                tx_docu.Text = "";
                                tx_fecha.Text = "";
                                tx_valor.Text = "";
                                tx_tiend.Text = "";
                                tx_estad.Text = "";
                                tx_fent.Text = "";
                                tx_codped.Focus();
                                dr.Close();
                                conn.Close();
                                return;
                            }
                            else
                            {
                                tx_cliente.Text = dr.GetString(7);
                                tx_docu.Text = dr.GetString(6);
                                tx_fecha.Text = dr.GetString(1).Substring(0,10);
                                tx_valor.Text = dr.GetString(4);
                                tx_tiend.Text = dr.GetString(2);
                                tx_estad.Text = dr.GetString(5);
                                if (dr[8] != null && dr[8] != DBNull.Value && dr.GetString(8).Trim() != "") tx_fent.Text = dr.GetString(8).Substring(0, 10);
                                else tx_fent.Text = "";
                                dr.Close();
                            }
                        }
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
        private void bt_resumen_Click(object sender, EventArgs e)   // genera resumen de contrato
        {
            if(tx_codped.Text != "")
            {
                tx_codped_Leave(null, null);
                string consulta = "res_cont";
                try
                {
                    MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        dgv_resumen.DataSource = null;
                        MySqlCommand micon = new MySqlCommand(consulta, conn);
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@cont", tx_codped.Text);
                        MySqlDataAdapter da = new MySqlDataAdapter(micon);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgv_resumen.DataSource = dt;
                        dt.Dispose();
                        da.Dispose();
                        grillares();
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
        }

        #region combos
        private void cmb_taller_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmb_taller.SelectedValue != null) tx_dat_orig.Text = cmb_taller.SelectedValue.ToString();
            //else tx_dat_orig.Text = cmb_taller.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_estado_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmb_estado.SelectedValue != null) tx_dat_estad.Text = cmb_estado.SelectedValue.ToString();
            //else tx_dat_estad.Text = cmb_estado.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_destino_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmb_destino.SelectedValue != null) tx_dat_dest.Text = cmb_destino.SelectedValue.ToString();
            //else tx_dat_dest.Text = cmb_destino.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_tall_ing_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmb_tall_ing.SelectedValue != null) tx_dat_taling.Text = cmb_tall_ing.SelectedValue.ToString();
            //else tx_dat_taling.Text = cmb_tall_ing.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_estad_ing_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmb_estad_ing.SelectedValue != null) tx_dat_esting.Text = cmb_estad_ing.SelectedValue.ToString();
            //else tx_dat_esting.Text = cmb_estad_ing.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_dest_ing_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (cmb_dest_ing.SelectedValue != null) tx_dat_desing.Text = cmb_dest_ing.SelectedValue.ToString();
            //else tx_dat_desing.Text = cmb_dest_ing.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        private void cmb_vtasloc_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_vtasloc.SelectedValue != null) tx_dat_vtasloc.Text = cmb_vtasloc.SelectedValue.ToString();
            else tx_dat_vtasloc.Text = cmb_vtasloc.SelectedItem.ToString().PadRight(6).Substring(0, 6).Trim();
        }
        // 
        private void cmb_estado_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                //cmb_estado.SelectedIndex = -1;
                //tx_dat_estad.Text = "";
            }
        }
        private void cmb_taller_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //cmb_taller.SelectedIndex = -1;
                //tx_dat_orig.Text = "";
            }
        }
        private void cmb_destino_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //cmb_destino.SelectedIndex = -1;
                //tx_dat_dest.Text = "";
            }
        }
        private void cmb_tall_ing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //cmb_tall_ing.SelectedIndex = -1;
                //tx_dat_taling.Text = "";
            }
        }
        private void cmb_estad_ing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //cmb_estad_ing.SelectedIndex = -1;
                //tx_dat_esting.Text = "";
            }
        }
        private void cmb_dest_ing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //cmb_dest_ing.SelectedIndex = -1;
                //tx_dat_desing.Text = "";
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
            //cmb_tall_ing.Enabled = false;
            //cmb_estad_ing.Enabled = false;
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
            if (tabControl1.SelectedTab == tabres && dgv_resumen.Rows.Count > 0)
            {
                nombre = "resumen_contrato_" + tx_codped.Text.Trim() +"_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
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
            if (tabControl1.SelectedTab == tabvtas && dgv_vtas.Rows.Count > 0)
            {
                nombre = "Reportes_ventas_" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".xlsx";
                var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
                    "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    var wb = new XLWorkbook();
                    DataTable dt = (DataTable)dgv_vtas.DataSource;
                    wb.Worksheets.Add(dt, "Ventas");
                    wb.SaveAs(nombre);
                    MessageBox.Show("Archivo generado con exito!");
                    this.Close();
                }
            }
        }
        #endregion

        #region crystal
        private void bt_pedidos_Click(object sender, EventArgs e)
        {
            setParaCrystal("pedidos");
        }
        private void button2_Click(object sender, EventArgs e)      // resumen de contrato
        {
            setParaCrystal("resumen");
        }
        private void button3_Click(object sender, EventArgs e)      // listado de contratos
        {
            setParaCrystal("contratos");
        }
        private void button4_Click(object sender, EventArgs e)      // reporte de ventas
        {
            if (tx_idclie.Text.Trim() == "") setParaCrystal("ventas");
            if (tx_idclie.Text.Trim() != "" && tx_nomclie.Text.Trim() != "" && rb_listado.Checked == true) setParaCrystal("vtasxclte");
        }
        private void bt_ingresos_Click(object sender, EventArgs e)  // reportes de ingresos de pedidos
        {
            setParaCrystal("ingresos");
        }
        private void bt_salidas_Click(object sender, EventArgs e)
        {
            setParaCrystal("salidas");
        }

        private void setParaCrystal(string repo)                    // genera el set para el reporte de crystal
        {
            if (repo== "resumen")
            {
                conClie datos = generareporte();                        // conClie = dataset de impresion de contrato   
                frmvizcont visualizador = new frmvizcont(datos);        // POR ESO SE CREO ESTE FORM frmvizcont PARA MOSTRAR AHI. ES MEJOR ASI.  
                visualizador.Show();
            }
            if (repo == "pedidos")
            {
                pedsclts datos = generarepedidos();
                frmvizcpeds visualizador = new frmvizcpeds(datos);
                visualizador.Show();
            }
            if (repo == "contratos")
            {
                conClie datos = generaliscont();
                frmvizcont visualizador = new frmvizcont(datos);
                visualizador.Show();
            }
            if (repo == "ventas")
            {
                conClie datos = generarepvtas();
                frmvizcont visualizador = new frmvizcont(datos);
                visualizador.Show();
            }
            if (repo == "vtasxclte")
            {
                conClie datos = generarepvtasxclte();
                frmvizcont visualizador = new frmvizcont(datos);
                visualizador.Show();
            }
            if (repo == "ingresos")
            {
                pedsclts datos = generarepingresos();
                frmvizcpeds visualizador = new frmvizcpeds(datos);
                visualizador.Show();
            }
            if (repo == "salidas")
            {
                pedsclts datos = generarepsalidas();
                frmvizcpeds visualizador = new frmvizcpeds(datos);
                visualizador.Show();
            }
        }
        private conClie generarepvtasxclte()
        {
            conClie repvtas = new conClie();                        // xsd
            conClie.repvtas_cabRow cabrow = repvtas.repvtas_cab.Newrepvtas_cabRow();
            cabrow.id = "0";
            cabrow.fecini = dtp_vtasfini.Value.ToString("dd/MM/yyyy");
            cabrow.fecfin = dtp_vtasfina.Value.ToString("dd/MM/yyyy");
            cabrow.nudoclte = tx_doclie.Text.Trim();
            cabrow.nomclie = tx_nomclie.Text.Trim();
            if (rb_listado.Checked == true) cabrow.modo = "listado";
            //if (rb_resumen.Checked == true) cabrow.modo = "resumen";
            repvtas.repvtas_cab.Addrepvtas_cabRow(cabrow);
            // detalle
            foreach (DataGridViewRow row in dgv_vtas.Rows)
            {
                if (rb_listado.Checked == true)                      // 
                {
                    if (row.Cells["item"].Value != null && row.Cells["item"].Value.ToString().Trim() != "")
                    {
                        conClie.repvtas_detRow detrow = repvtas.repvtas_det.Newrepvtas_detRow();
                        detrow.id = "0";    //String.Format("{0:dd/MM/yyyy}",row.Cells["fecha"].Value.ToString()); //.ToString("dd/MM/yyyy"); ... ninguno de estos funciono
                        detrow.tienda = row.Cells["tienda"].Value.ToString();
                        detrow.fecha = row.Cells["fecha"].Value.ToString().Substring(0,2) + "/" + row.Cells["fecha"].Value.ToString().Substring(3, 2) + "/" + row.Cells["fecha"].Value.ToString().Substring(6, 4); 
                        detrow.contrato = row.Cells["contratoh"].Value.ToString();
                        detrow.cant = row.Cells["cant"].Value.ToString().Trim();
                        detrow.codigo = row.Cells["item"].Value.ToString();
                        detrow.nombre = row.Cells["nombre"].Value.ToString().Trim();
                        detrow.medidas = row.Cells["medidas"].Value.ToString().Trim();
                        detrow.madera =  row.Cells["madera"].Value.ToString().Trim();
                        detrow.precio = row.Cells["precio"].Value.ToString().Trim();
                        detrow.total = double.Parse(row.Cells["total"].Value.ToString());
                        detrow.estcont = row.Cells["status"].Value.ToString().Trim();
                        if (row.Cells["fesal"].Value.ToString().Trim() == "") detrow.fecsal = row.Cells["fesal"].Value.ToString().Trim(); // salida de almacen con reserva
                        else detrow.fecsal = row.Cells["fesal"].Value.ToString().Substring(8, 2) + "/" + row.Cells["fesal"].Value.ToString().Substring(5, 2) + "/" + row.Cells["fesal"].Value.ToString().Substring(0, 4);
                        if (row.Cells["fecent"].Value.ToString().Trim() == "") detrow.fecent = row.Cells["fecent"].Value.ToString().Trim(); // fecha del pedido
                        else detrow.fecent = row.Cells["fecent"].Value.ToString().Substring(0,10);  //.Substring(8, 2) + "/" + row.Cells["fecent"].Value.ToString().Substring(5, 2) + "/" + row.Cells["fecent"].Value.ToString().Substring(0, 4);
                        repvtas.repvtas_det.Addrepvtas_detRow(detrow);
                    }
                }
            }
            return repvtas;
        }
        private conClie generarepvtas()
        {
            conClie repvtas = new conClie();                        // xsd
            conClie.repvtas_cabRow cabrow = repvtas.repvtas_cab.Newrepvtas_cabRow();
            cabrow.id = "0";
            cabrow.fecini = dtp_vtasfini.Value.ToString("dd/MM/yyyy");
            cabrow.fecfin = dtp_vtasfina.Value.ToString("dd/MM/yyyy");
            cabrow.tienda = tx_dat_vtasloc.Text.Trim();
            if (rb_listado.Checked == true) cabrow.modo = "listado";
            if (rb_resumen.Checked == true) cabrow.modo = "resumen";
            repvtas.repvtas_cab.Addrepvtas_cabRow(cabrow);
            // detalle
            foreach(DataGridViewRow row in dgv_vtas.Rows)
            {
                if (rb_resumen.Checked == true)
                {
                    if (row.Cells["item"].Value != null && row.Cells["item"].Value.ToString().Trim() != "")
                    {
                        conClie.repvtas_detRow detrow = repvtas.repvtas_det.Newrepvtas_detRow();
                        detrow.id = "0";
                        detrow.tienda = row.Cells["tienda"].Value.ToString();
                        detrow.codigo = row.Cells["item"].Value.ToString();
                        detrow.nombre = row.Cells["nombre"].Value.ToString().Trim();
                        detrow.cant = row.Cells["cant"].Value.ToString().Trim();
                        detrow.madera = row.Cells["madera"].Value.ToString().Trim();
                        detrow.medidas = row.Cells["medidas"].Value.ToString().Trim();
                        detrow.total = double.Parse(row.Cells["total"].Value.ToString());
                        repvtas.repvtas_det.Addrepvtas_detRow(detrow);
                    }
                }
                if (rb_listado.Checked == true)                      // tienda,b.fecha,a.contratoh,cliente,a.cant,a.item,a.nombre,a.medidas,a.madera,a.precio,a.total,PEDIDO,STOCK
                {
                    if (row.Cells["item"].Value != null && row.Cells["item"].Value.ToString().Trim() != "")
                    {
                        conClie.repvtas_detRow detrow = repvtas.repvtas_det.Newrepvtas_detRow();
                        detrow.id = "0";
                        detrow.tienda = row.Cells["tienda"].Value.ToString();
                        detrow.fecha = row.Cells["fecha"].Value.ToString().Substring(0,10);
                        detrow.contrato = row.Cells["contratoh"].Value.ToString();
                        detrow.cliente = row.Cells["cliente"].Value.ToString();
                        detrow.cant = row.Cells["cant"].Value.ToString().Trim();
                        detrow.codigo = row.Cells["item"].Value.ToString();
                        detrow.nombre = row.Cells["nombre"].Value.ToString().Trim();
                        detrow.medidas = row.Cells["medidas"].Value.ToString().Trim();
                        detrow.madera = row.Cells["madera"].Value.ToString().Trim();
                        detrow.precio = row.Cells["precio"].Value.ToString().Trim();
                        detrow.total = double.Parse(row.Cells["total"].Value.ToString());
                        detrow.pedido = row.Cells["PEDIDO"].Value.ToString().Trim();
                        detrow.fecsal = row.Cells["DESPACHO"].Value.ToString().Trim();
                        detrow.stock = row.Cells["RESERVA"].Value.ToString().Trim();
                        repvtas.repvtas_det.Addrepvtas_detRow(detrow);
                    }
                }
            }
            return repvtas;
        }
        private conClie generareporte()                             // procedimiento para meter los datos del formulario hacia las tablas del dataset del reporte en crystal
        {
            conClie rescont = new conClie();                                    // dataset
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
            return rescont;
        }
        private pedsclts generarepedidos()
        {
            pedsclts pedset = new pedsclts();
            pedsclts.cab_lispedidosRow rowcab = pedset.cab_lispedidos.Newcab_lispedidosRow();
            rowcab.id = "0";
            rowcab.fecfin = dtp_pedfini.Value.ToString("dd/MM/yyyy");
            rowcab.fecini = dtp_pedfina.Value.ToString("dd/MM/yyyy");
            rowcab.taller = cmb_pedtaller.Text.Trim();
            rowcab.situacion = (rb_ped_todos.Checked == true)? "TODOS":(rb_ped_xllegar.Checked == true)? "X LLEGAR":"INGRESADOS";
            rowcab.tipofechas = (rb_ped_fped.Checked == true)? rb_ped_fped.Text:rb_ped_fentrega.Text;
            pedset.cab_lispedidos.Addcab_lispedidosRow(rowcab);
            // 
            foreach (DataGridViewRow row in dgv_pedidos.Rows)
            {
                if (row.Cells["codped"].Value != null && row.Cells["codped"].Value.ToString().Trim() != "")
                {
                    pedsclts.det_lispedidosRow rowdet = pedset.det_lispedidos.Newdet_lispedidosRow();
                    rowdet.id = "0";
                    rowdet.lugar = row.Cells["destino"].Value.ToString();
                    rowdet.pedido = row.Cells["codped"].Value.ToString();
                    rowdet.cliente = row.Cells["cliente"].Value.ToString();
                    rowdet.codigo = row.Cells["item"].Value.ToString();
                    rowdet.nombre = row.Cells["nombre"].Value.ToString().Trim() + " - " + row.Cells["coment"].Value.ToString().Trim();
                    rowdet.coment = "";
                    rowdet.medidas = row.Cells["medidas"].Value.ToString();
                    rowdet.madera = row.Cells["madera"].Value.ToString();
                    rowdet.cant = row.Cells["cant"].Value.ToString();
                    rowdet.taller = row.Cells["taller"].Value.ToString();
                    rowdet.estado = row.Cells["estado"].Value.ToString();
                    rowdet.fecped = row.Cells["fecped"].Value.ToString().Substring(0,10);
                    rowdet.fecing = row.Cells["fecing"].Value.ToString().PadRight(10).Substring(0, 10);
                    rowdet.fecsal = row.Cells["fecent"].Value.ToString().PadRight(10).Substring(0, 10);
                    rowdet.fececon = row.Cells["feencon"].Value.ToString().PadRight(10).Substring(0, 10);
                    rowdet.origen = row.Cells["origen"].Value.ToString();
                    pedset.det_lispedidos.Adddet_lispedidosRow(rowdet);
                }
            }
            return pedset;
        }

        private pedsclts generarepingresos()
        {   // a.idmovim,a.fechain,tipo,a.pedido,a.origen,a.destino,a.cant,a.articulo,nomad,med1,tipoes,madera,cliente,nomitem
            pedsclts pedset = new pedsclts();
            pedsclts.cab_repingRow rowcab = pedset.cab_reping.Newcab_repingRow();
            rowcab.id = "0";
            rowcab.fini = dtp_fini_sal.Value.ToString().Substring(0, 10);
            rowcab.fina = dtp_final_sal.Value.ToString().Substring(0, 10);
            pedset.cab_reping.Addcab_repingRow(rowcab);
            //
            foreach(DataGridViewRow row in dgv_ingresos.Rows)
            {
                if (row.Cells["pedido"].Value != null && row.Cells["pedido"].Value.ToString().Trim() != "")
                {
                    pedsclts.det_repingRow rowdet = pedset.det_reping.Newdet_repingRow();
                    rowdet.id = "0";
                    rowdet.fecha = row.Cells["fechain"].Value.ToString().Substring(0, 10);
                    rowdet.tipo = row.Cells["tipo"].Value.ToString();
                    rowdet.pedido = row.Cells["pedido"].Value.ToString();
                    rowdet.origen = row.Cells["origen"].Value.ToString();
                    rowdet.destino = row.Cells["destino"].Value.ToString();
                    rowdet.cant = row.Cells["cant"].Value.ToString();
                    rowdet.articulo = row.Cells["articulo"].Value.ToString();
                    rowdet.nonmad = row.Cells["nomad"].Value.ToString();
                    rowdet.medidas = row.Cells["med1"].Value.ToString();
                    rowdet.tipoes = row.Cells["tipoes"].Value.ToString();
                    rowdet.madera = row.Cells["madera"].Value.ToString();
                    rowdet.cliente = row.Cells["cliente"].Value.ToString();
                    rowdet.nomitem = row.Cells["nomitem"].Value.ToString();
                    pedset.det_reping.Adddet_repingRow(rowdet);
                }
            }
            return pedset;
        }
        private pedsclts generarepsalidas()                         // salidas de pedidos de clientes (entregas)
        {
            //a.fecha,a.tipo,a.pedido,cliente,a.uantes,a.uactual,a.coment,b.item,b.cant,b.medidas,b.madera
            pedsclts pedset = new pedsclts();
            pedsclts.cab_repsalRow rowcab = pedset.cab_repsal.Newcab_repsalRow();
            rowcab.id = "0";
            rowcab.fini = dtp_fini_sal.Value.ToString().Substring(0, 10);
            rowcab.fina = dtp_final_sal.Value.ToString().Substring(0, 10);
            pedset.cab_repsal.Addcab_repsalRow(rowcab);
            //
            foreach(DataGridViewRow row in dgv_salidas.Rows)
            {
                if (row.Cells["tipo"].Value != null && row.Cells["tipo"].Value.ToString().Trim() != "")
                {
                    pedsclts.det_repsalRow rowdet = pedset.det_repsal.Newdet_repsalRow();
                    rowdet.id = "0";
                    rowdet.fecha = row.Cells["fecha"].Value.ToString().Substring(0, 10);
                    rowdet.tipo = row.Cells["tipo"].Value.ToString();
                    rowdet.pedido = row.Cells["pedido"].Value.ToString();
                    rowdet.cliente = row.Cells["cliente"].Value.ToString();
                    rowdet.uantes = row.Cells["uantes"].Value.ToString();
                    rowdet.uactual = row.Cells["uactual"].Value.ToString();
                    rowdet.coment = row.Cells["coment"].Value.ToString();
                    rowdet.item = row.Cells["item"].Value.ToString();
                    rowdet.cant = row.Cells["cant"].Value.ToString();
                    rowdet.medidas = row.Cells["medidas"].Value.ToString();
                    rowdet.madera = row.Cells["madera"].Value.ToString();
                    pedset.det_repsal.Adddet_repsalRow(rowdet);
                }
            }
            return pedset;
        }
        private conClie generaliscont()                             // procedimiento para generar los datos del listado de contratos en el dataset
        {
            conClie liscont = new conClie();
            conClie.liscont_cabRow rowcabeza = liscont.liscont_cab.Newliscont_cabRow();
            rowcabeza.id = "0";
            rowcabeza.fechini = dtp_confini.Value.ToString("dd/MM/yyyy");   // yyyy-MM-dd
            rowcabeza.fechfin = dtp_confina.Value.ToString("dd/MM/yyyy");   // 
            rowcabeza.estado = tx_dat_conestado.Text.Trim();
            rowcabeza.ran = (rb_fcont.Checked == true) ? "C" : "E";
            liscont.liscont_cab.Addliscont_cabRow(rowcabeza);
            //
            foreach(DataGridViewRow row in dgv_contratos.Rows)
            {
                if (row.Cells["fecha"].Value != null && row.Cells["fecha"].Value.ToString().Trim() != "")
                {
                    conClie.liscont_detRow rowdetalle = liscont.liscont_det.Newliscont_detRow();
                    rowdetalle.id = "0";
                    rowdetalle.fecha = row.Cells[0].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.tienda = row.Cells[1].Value.ToString();
                    rowdetalle.contrato = row.Cells[2].Value.ToString();
                    rowdetalle.cliente = row.Cells[3].Value.ToString();
                    rowdetalle.coment = row.Cells[4].Value.ToString();
                    rowdetalle.fentrega = row.Cells[5].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.fenreal = row.Cells[6].Value.ToString().PadRight(10).Substring(0,10);
                    rowdetalle.estado = row.Cells[7].Value.ToString();
                    liscont.liscont_det.Addliscont_detRow(rowdetalle);
                }
            }
            return liscont;
        }
        #endregion
    }
}
