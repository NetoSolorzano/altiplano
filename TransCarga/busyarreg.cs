using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Windows.Forms;     // ok
using MySql.Data.MySqlClient;   // ok
using System.Collections;       // ok

namespace TransCarga
{
    public partial class busyarreg : Form
    {
        string valant = "";                 // valor celda antes de cambio 
        string valnue = "";                 // valor celda despues de cambio
        string codant = "";                 // codigo antes de cambio de valor de celda
        string codnue = "";                 // codigo despues de cambio de valor de celda
        libreria lib = new libreria();
        publico pub = new publico();
        DataTable dt = new DataTable();
        DataView dv = new DataView();
        //List<bool> marcas = new List<bool>();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        #region variables
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colstrp = TransCarga.Program.colstr;   // color del strip
        string asd = TransCarga.Program.vg_user;   // usuario conectado al sistema
        // para la impresion
        string nomform = "busyarreg";                          //
        string img_btN = "";
        string img_btE = "";
        string img_btP = "";
        string img_btA = "";            // anula = bloquea
        string img_btexc = "";          // exporta a excel
        string img_bti = "";
        string img_bts = "";
        string img_btr = "";
        string img_btf = "";
        string img_btq = "";
        string nfCRgr = "";             // nombre formato CR para visualizacion de guias
        string vcestper = "";           // nombres de estados que si se permite el arreglo o modificacion
        #endregion

        public busyarreg()
        {
            InitializeComponent();
        }
        private void busyarreg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
        }
        private void busyarreg_Load(object sender, EventArgs e)
        {
            advancedDataGridView1.Enabled = false;
            toolboton();
            init();
            tx_ser.ReadOnly = true;
            dtp_fini.Enabled = false;
            dtp_ffin.Enabled = false;
            bt_caja.Enabled = false;
        }
        private void pan_inicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
        }

        #region funciones priopas del form
        private void jalainfo()                                                         // obtiene datos de imagenes
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@noga,@nofg)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@noga", nomform);
                micon.Parameters.AddWithValue("@nofg", "guiati");
                MySqlDataAdapter da = new MySqlDataAdapter(micon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    DataRow row = dt.Rows[t];
                    if (row["formulario"].ToString() == "main" && row["campo"].ToString() == "imagenes")
                    {
                        if (row["param"].ToString() == "img_btN") img_btN = row["valor"].ToString().Trim();         // imagen del boton de accion NUEVO
                        if (row["param"].ToString() == "img_btE") img_btE = row["valor"].ToString().Trim();         // imagen del boton de accion EDITAR
                        if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        if (row["param"].ToString() == "img_btA") img_btA = row["valor"].ToString().Trim();         // imagen del boton de accion ANULAR/BORRAR
                        if (row["param"].ToString() == "img_btexc") img_btexc = row["valor"].ToString().Trim();     // imagen del boton exporta a excel
                        if (row["param"].ToString() == "img_btQ") img_btq = row["valor"].ToString().Trim();         // imagen del boton de accion SALIR
                        //if (row["param"].ToString() == "img_btP") img_btP = row["valor"].ToString().Trim();         // imagen del boton de accion IMPRIMIR
                        // boton de vista preliminar .... esta por verse su utlidad
                        if (row["param"].ToString() == "img_bti") img_bti = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL INICIO
                        if (row["param"].ToString() == "img_bts") img_bts = row["valor"].ToString().Trim();         // imagen del boton de accion SIGUIENTE
                        if (row["param"].ToString() == "img_btr") img_btr = row["valor"].ToString().Trim();         // imagen del boton de accion RETROCEDE
                        if (row["param"].ToString() == "img_btf") img_btf = row["valor"].ToString().Trim();         // imagen del boton de accion IR AL FINAL
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "estadPer" && row["param"].ToString() == "permitidos") vcestper = row["valor"].ToString().Trim();       // estados permitos para arreglar
                    }
                    if (row["formulario"].ToString() == "guiati")
                    {
                        if (row["campo"].ToString() == "impresion" && row["param"].ToString() == "nomGRir_cr") nfCRgr = row["valor"].ToString().Trim();         // campo de codigo de estado almacen reparto
                        //if (row["campo"].ToString() == "estAlmacen" && row["param"].ToString() == "recepcion") vcead = row["valor"].ToString().Trim();       // estado almacen recepcionado
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
        private void jaladat()                                                          // jala almacen 
        {
            MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
            cn.Open();
            try
            {
                string sqlCmd = "select a.id AS ID,a.fechopegr as FECHA,lo.descrizionerid as SEDE,ld.descrizionerid as DESTINO,a.sergui as SERIE,a.numgui as GUIA,dr.descrizionerid as DR,a.nudoregri as NUMDOCR," +
                    "a.nombregri as REMITENTE,dd.descrizionerid as DD,a.nudodegri as NUMDOCD,a.nombdegri as DESTINATARIO,a.docsremit as DOCSREMIT,mo.descrizionerid as MON,a.totgri as FLETE," +
                    "es.descrizionerid as ESTADO,concat(a.serplagri, '-', a.numplagri) AS MANIFIESTO, ifnull(concat(dv.descrizionerid, '-', a.serdocvta, '-', a.numdocvta), '') as DOC_VTA," +
                    "a.estadoser " +
                    "from cabguiai a " +
                    "left join desc_loc lo on lo.idcodice = a.locorigen " +
                    "left join desc_loc ld on ld.idcodice = a.locdestin " +
                    "left join desc_doc dr on dr.idcodice = a.tidoregri " +
                    "left join desc_doc dd on dd.idcodice = a.tidodegri " +
                    "left join desc_mon mo on mo.idcodice = a.tipmongri " +
                    "left join desc_est es on es.idcodice = a.estadoser " +
                    "left join desc_tdv dv on dv.idcodice = a.tipdocvta " +
                    "where a.fechopegr between @fini and @ffin and a.sergui = @ser " +
                    "order by a.sergui,a.numgui";
                MySqlCommand micon = new MySqlCommand(sqlCmd, cn);
                micon.Parameters.AddWithValue("@ser", tx_ser.Text);
                micon.Parameters.AddWithValue("@fini", dtp_fini.Value.ToString("yyyy-MM-dd"));
                micon.Parameters.AddWithValue("@ffin", dtp_ffin.Value.ToString("yyyy-MM-dd"));
                micon.CommandTimeout = 300;
                MySqlDataAdapter adr = new MySqlDataAdapter(micon);
                adr.SelectCommand.CommandType = CommandType.Text;
                adr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en jaladat");
                cn.Dispose(); // return connection to pool
                cn.Close();
                Application.Exit();
            }
            cn.Close();
        }
        private void grilla()                                                           // arma la grilla1
        {
            Font font = new Font("Tahoma", 7);
            advancedDataGridView1.RowHeadersWidth = 19;
            advancedDataGridView1.ColumnHeadersHeight = 19;
            advancedDataGridView1.DefaultCellStyle.Font = font;
            advancedDataGridView1.RowHeadersDefaultCellStyle.Font = font;
            advancedDataGridView1.RowTemplate.Height = 15;
            advancedDataGridView1.AllowUserToAddRows = false;
            advancedDataGridView1.ColumnCount = 19;
            advancedDataGridView1.Columns[0].Width = 40;             // id
            advancedDataGridView1.Columns[0].ReadOnly = true;
            advancedDataGridView1.Columns[0].Name = "ID";
            advancedDataGridView1.Columns[0].HeaderText = "ID";
            advancedDataGridView1.Columns[1].Width = 70;             // fecha ingreso
            advancedDataGridView1.Columns[1].ReadOnly = true;
            advancedDataGridView1.Columns[1].Name = "FECHA";
            advancedDataGridView1.Columns[1].HeaderText = "FECHA";
            advancedDataGridView1.Columns[2].Width = 70;             // nombre sede origen
            advancedDataGridView1.Columns[2].ReadOnly = true;
            advancedDataGridView1.Columns[2].Name = "SEDE";
            advancedDataGridView1.Columns[2].HeaderText = "SEDE";
            advancedDataGridView1.Columns[3].Width = 70;             // nombre sede destino
            advancedDataGridView1.Columns[3].ReadOnly = true;
            advancedDataGridView1.Columns[3].Name = "DESTINO";
            advancedDataGridView1.Columns[3].HeaderText = "DESTINO";
            advancedDataGridView1.Columns[4].Width = 50;             // serie guia transportista
            advancedDataGridView1.Columns[4].ReadOnly = false;
            advancedDataGridView1.Columns[4].Name = "SERIE";
            advancedDataGridView1.Columns[4].HeaderText = "SERIE";
            advancedDataGridView1.Columns[5].Width = 60;             // numero guia transportista
            advancedDataGridView1.Columns[5].ReadOnly = false;
            advancedDataGridView1.Columns[5].Name = "GUIA";
            advancedDataGridView1.Columns[5].HeaderText = "GUIA";
            advancedDataGridView1.Columns[6].Width = 50;             // nombre doc remitente
            advancedDataGridView1.Columns[6].ReadOnly = true;
            advancedDataGridView1.Columns[6].Name = "DR";
            advancedDataGridView1.Columns[6].HeaderText = "DR";
            advancedDataGridView1.Columns[7].Width = 70;             // num doc remitente
            advancedDataGridView1.Columns[7].ReadOnly = true;
            advancedDataGridView1.Columns[7].Name = "NUMDOCR";
            advancedDataGridView1.Columns[7].HeaderText = "NUMDOCR";
            advancedDataGridView1.Columns[8].Width = 170;            // nombre del remitente
            advancedDataGridView1.Columns[8].ReadOnly = true;
            advancedDataGridView1.Columns[8].Name = "REMITENTE";
            advancedDataGridView1.Columns[8].HeaderText = "REMITENTE";
            advancedDataGridView1.Columns[9].Width = 50;              // nombre doc destinatario
            advancedDataGridView1.Columns[9].ReadOnly = true;
            advancedDataGridView1.Columns[9].Name = "DD";
            advancedDataGridView1.Columns[9].HeaderText = "DD";
            advancedDataGridView1.Columns[10].Width = 70;             // numero doc destinat
            advancedDataGridView1.Columns[10].ReadOnly = true;
            advancedDataGridView1.Columns[10].Name = "NUMDOCD";
            advancedDataGridView1.Columns[10].HeaderText = "NUMDOCD";
            advancedDataGridView1.Columns[11].Width = 170;            // nombre del destinat
            advancedDataGridView1.Columns[11].ReadOnly = true;
            advancedDataGridView1.Columns[11].Name = "DESTINATARIO";
            advancedDataGridView1.Columns[11].HeaderText = "DESTINATARIO";
            advancedDataGridView1.Columns[12].Width = 80;            // guias o docs del remitente
            advancedDataGridView1.Columns[12].ReadOnly = true;
            advancedDataGridView1.Columns[12].Name = "DOCSREMIT";
            advancedDataGridView1.Columns[12].HeaderText = "DOCSREMIT";
            advancedDataGridView1.Columns[13].Width = 30;             // simbolo moneda GR
            advancedDataGridView1.Columns[13].ReadOnly = true;
            advancedDataGridView1.Columns[13].Name = "MON";
            advancedDataGridView1.Columns[13].HeaderText = "MON";
            advancedDataGridView1.Columns[14].Width = 70;          // valor flete gr
            advancedDataGridView1.Columns[14].ReadOnly = true;
            advancedDataGridView1.Columns[14].Name = "FLETE";
            advancedDataGridView1.Columns[14].HeaderText = "FLETE";
            advancedDataGridView1.Columns[15].Width = 70;          // estado de la GR
            advancedDataGridView1.Columns[15].ReadOnly = true;
            advancedDataGridView1.Columns[15].Name = "ESTADO";
            advancedDataGridView1.Columns[15].HeaderText = "ESTADO";
            advancedDataGridView1.Columns[16].Width = 100;          // planilla de carga
            advancedDataGridView1.Columns[16].ReadOnly = true;
            advancedDataGridView1.Columns[16].Name = "MANIFIESTO";
            advancedDataGridView1.Columns[16].HeaderText = "MANIFIESTO";
            advancedDataGridView1.Columns[17].Width = 100;          // doc.venta
            advancedDataGridView1.Columns[17].ReadOnly = true;
            advancedDataGridView1.Columns[17].Name = "DOC_VTA";
            advancedDataGridView1.Columns[17].HeaderText = "DOC_VTA";
            advancedDataGridView1.Columns[18].Name = "codEst";
            advancedDataGridView1.Columns[18].Visible = false;
            // ID,FECHA,SEDE,DESTINO,SERIE,GUIA,DR,NUMDOCR,REMITENTE,DD,NUMDOCD,DESTINATARIO,DOCSREMIT,MON,FLETE,ESTADO,MANIFIESTO,DOC_VTA,codEst 
            foreach (DataRow row in dt.Rows)    // if (dt.Rows.Count > 0)   // advancedDataGridView1.Rows.Count > 0
            {
                advancedDataGridView1.Rows.Add(row.ItemArray[0].ToString(),
                    row.ItemArray[1].ToString().Substring(0,10),
                    row.ItemArray[2].ToString(),
                    row.ItemArray[3].ToString(),
                    row.ItemArray[4].ToString(),
                    row.ItemArray[5].ToString(),
                    row.ItemArray[6].ToString(),
                    row.ItemArray[7].ToString(),
                    row.ItemArray[8].ToString(),
                    row.ItemArray[9].ToString(),
                    row.ItemArray[10].ToString(),
                    row.ItemArray[11].ToString(),
                    row.ItemArray[12].ToString(),
                    row.ItemArray[13].ToString(),
                    row.ItemArray[14].ToString(),
                    row.ItemArray[15].ToString(),
                    row.ItemArray[16].ToString(),
                    row.ItemArray[17].ToString(),
                    row.ItemArray[18].ToString()
                    );
            }
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < advancedDataGridView1.Columns.Count; i++)   //
                {
                    //advancedDataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _ = decimal.TryParse(advancedDataGridView1.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                    if (vd != 0) advancedDataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }
        private void init()                                                             // inicializa ancho de columnas grilla de filtros
        {
            this.BackColor = Color.FromName(colback);                               // color de fondo
            advancedDataGridView1.BackgroundColor = Color.FromName(colgrid);        // color de las grillas
            toolStrip1.BackColor = Color.FromName(colstrp);                         // color del strip
            //
            jalainfo();
            Bt_add.Image = Image.FromFile(img_btN);
            Bt_edit.Image = Image.FromFile(img_btE);
            Bt_anul.Image = Image.FromFile(img_btA);
            Bt_close.Image = Image.FromFile(img_btq);
            bt_exc.Image = Image.FromFile(img_btexc);
            bt_prev.Image = Image.FromFile(img_btP);
            Bt_print.Image = Image.FromFile(img_btP);
            Bt_ini.Image = Image.FromFile(img_bti);
            Bt_sig.Image = Image.FromFile(img_bts);
            Bt_ret.Image = Image.FromFile(img_btr);
            Bt_fin.Image = Image.FromFile(img_btf);
        }
        private void cellsum(int ind)                                                   // suma la columna especificada
        {
            tx_tarti.Text = (advancedDataGridView1.Rows.Count).ToString();
        }
        private void grabacam(int idm, string campo, string valor)                      // graba el cambio en la tabla
        {
            // ID,FECHA,SEDE,DESTINO,SERIE,GUIA,DR,NUMDOCR,REMITENTE,DD,NUMDOCD,DESTINATARIO,DOCSREMIT,MON,FLETE,ESTADO,MANIFIESTO,DOC_VTA 
            switch(campo)
            {
                case "FECHA":
                    // ummmm no creo ah
                    break;
                case "SEDE":
                    // aca tampoco ahh
                    break;
                case "SERIE": case "GUIA":
                    using (MySqlConnection cn0 = new MySqlConnection(DB_CONN_STR))
                    {
                        if (lib.procConn(cn0) == true)
                        {
                            string sqlCmd = "arreglaGR";
                            using (MySqlCommand micon = new MySqlCommand(sqlCmd, cn0))
                            {
                                micon.CommandType = CommandType.StoredProcedure;
                                micon.Parameters.AddWithValue("@v_idr", idm);
                                micon.Parameters.AddWithValue("@v_cam", campo);
                                micon.Parameters.AddWithValue("@v_nva", valor);
                                micon.ExecuteNonQuery();
                            }
                        }
                    }
                    break;
            }
        }
        private void bt_caja_Click(object sender, EventArgs e)                          // boton genera
        {
            advancedDataGridView1.DataSource = null;
            advancedDataGridView1.Rows.Clear();
            dt.Rows.Clear();
            jaladat();
            grilla();
            cellsum(0);
        }
        private void tx_ser_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "" && tx_ser.Text.Trim() != "")
            {
                tx_ser.Text = lib.Right("000" + tx_ser.Text,4);
            }
        }
        #endregion

        #region botones_de_comando_y_permisos  
        public void toolboton()
        {
            Bt_add.Visible = false;
            Bt_edit.Visible = false;
            Bt_anul.Visible = false;
            bt_view.Visible = false;
            Bt_print.Visible = false;
            bt_prev.Visible = false;
            bt_exc.Visible = false;
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
                if (Convert.ToString(row["btn4"]) == "S")               // visualizar ... ok
                {
                    this.bt_view.Visible = true;
                }
                else { this.bt_view.Visible = false; }
                if (Convert.ToString(row["btn5"]) == "S")               // imprimir ... ok
                {
                    this.Bt_print.Visible = true;
                }
                else { this.Bt_print.Visible = false; }
                if (Convert.ToString(row["btn7"]) == "S")               // vista preliminar ... ok
                {
                    this.bt_prev.Visible = true;
                }
                else { this.bt_prev.Visible = false; }
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
            Tx_modo.Text = "NUEVO";
            advancedDataGridView1.Enabled = true;
            tx_ser.ReadOnly = false;
            dtp_fini.Enabled = true;
            dtp_ffin.Enabled = true;
            bt_caja.Enabled = true;
            tx_ser.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "EDITAR";
            advancedDataGridView1.Enabled = true;
            tx_ser.ReadOnly = false;
            dtp_fini.Enabled = true;
            dtp_ffin.Enabled = true;
            bt_caja.Enabled = true;
            tx_ser.Focus();
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            //Tx_modo.Text = "IMPRIMIR";
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            //Tx_modo.Text = "ANULAR";
            // no tiene funcion en este form
        }
        private void bt_exc_Click(object sender, EventArgs e)
        {

        }
        private void Bt_first_Click(object sender, EventArgs e)
        {

        }
        private void Bt_back_Click(object sender, EventArgs e)
        {

        }
        private void Bt_next_Click(object sender, EventArgs e)
        {

        }
        private void Bt_last_Click(object sender, EventArgs e)
        {

        }
        #endregion botones_de_comando  ;

        #region advanced
        private void advancedDataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                valant = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                codant = advancedDataGridView1.Rows[e.RowIndex].Cells["GUIA"].Value.ToString();
            }
        }
        // ID,FECHA,SEDE,DESTINO,SERIE,GUIA,DR,NUMDOCR,REMITENTE,DD,NUMDOCD,DESTINATARIO,DOCSREMIT,MON,FLETE,ESTADO,MANIFIESTO,DOC_VTA 
        private void advancedDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                // solo campos SERIE Y GUIA se pueden cambiar
                if (vcestper.Contains(advancedDataGridView1.Rows[e.RowIndex].Cells["codEst"].Value.ToString()) &&
                    advancedDataGridView1.Rows[e.RowIndex].Cells["DOC_VTA"].Value.ToString().Trim() == "")
                {
                    valnue = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    var ao = MessageBox.Show("Modifica el dato " + advancedDataGridView1.Columns[e.ColumnIndex].Name + " ?", "Confirme por favor",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ao == DialogResult.Yes)
                    {
                        grabacam(int.Parse(advancedDataGridView1.Rows[e.RowIndex].Cells[advancedDataGridView1.Columns["ID"].Index].Value.ToString()),
                            advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString(), valnue);
                    }
                    else
                    {
                        advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = valant;
                    }
                }
                else
                {
                    MessageBox.Show("No se puede cambiar valor", "La GR tiene cobranza o Doc.Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = valant;
                }
            }
        }
        private void advancedDataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {

        }
        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        {
            dt.DefaultView.RowFilter = advancedDataGridView1.FilterString;
            cellsum(7);
        }
        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            dt.DefaultView.Sort = advancedDataGridView1.SortString;
        }
        private void advancedDataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (advancedDataGridView1.IsCurrentCellDirty)
            {
                advancedDataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void advancedDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (advancedDataGridView1.Columns[e.ColumnIndex].Name == "GUIA" && !string.IsNullOrEmpty(valant))
            {
                // se hace en cellendedit
            }
        }
        private void advancedDataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                //dataGridView2.HorizontalScrollingOffset = e.NewValue;
                //dataGridView1.HorizontalScrollingOffset = e.NewValue;
            }
        }
        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Tx_modo.Text.Trim() != "" && advancedDataGridView1.Columns[e.ColumnIndex].Name == "SERIE")
            {
                string vser = advancedDataGridView1.Rows[e.RowIndex].Cells["SERIE"].Value.ToString();
                string vnum = advancedDataGridView1.Rows[e.RowIndex].Cells["GUIA"].Value.ToString();
                pub.muestra_gr(vser, vnum, nfCRgr);
            }
        }
        #endregion

    }
}
