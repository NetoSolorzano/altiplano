using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;     // ok
using MySql.Data.MySqlClient;   // ok
using System.Configuration;     // ok
using ClosedXML.Excel;          // ok
using System.Collections;       // ok

namespace TransCarga
{
    public partial class almgestion : Form
    {
        string valant = "";                 // valor celda antes de cambio 
        string valnue = "";                 // valor celda despues de cambio
        string codant = "";                 // codigo antes de cambio de valor de celda
        string codnue = "";                 // codigo despues de cambio de valor de celda
        libreria lib = new libreria();
        publico pub = new publico();
        DataTable dt = new DataTable();
        DataView dv = new DataView();
        List<bool> marcas = new List<bool>();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        #region variables
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colstrp = TransCarga.Program.colstr;   // color del strip
        string asd = TransCarga.Program.vg_user;   // usuario conectado al sistema
        // para la impresion
        StringFormat strFormat;                                 // Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();             // Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();            // Used to save column widths
        int iCellHeight = 0;                                    // Used to get/set the datagridview cell height
        int iTotalWidth = 0;                                    //
        int iRow = 0;                                           // Used as counter
        bool bFirstPage = false;                                // Used to check whether we are printing first page
        bool bNewPage = false;                                  // Used to check whether we are printing a new page
        int iHeaderHeight = 0;                                  // Used for the header height
        int totcolv = 0;                                        // total columnas visibles
        string nomform = "almgestion";                          //
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
        string img_grab = "";
        string img_anul = "";
        string vcear = "";                                   // cosdigo estado de almacen en reparto
        string vcead = "";                                  // codigo de estado almacen recepcionado
        string nfCRgr = "";                                 // nombre formato CR para visualizacion de guias
        #endregion

        public almgestion()
        {
            InitializeComponent();
        }

        private void almgestion_Load(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            dataGridView1.Enabled = false;
            dataGridView2.Enabled = false;
            advancedDataGridView1.Enabled = false;
            bt_reserva.Enabled = false;
            bt_salida.Enabled = false;
            bt_borra.Enabled = false;
            //
            jaladat();
            advancedDataGridView1.DataSource = dt;
            grilla();
            init();
            cellsum(0);
            cvc();
            rb_estan.Checked = true;
            toolboton();
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
                        if (row["param"].ToString() == "img_gra") img_grab = row["valor"].ToString().Trim();         // imagen del boton grabar nuevo
                        if (row["param"].ToString() == "img_anu") img_anul = row["valor"].ToString().Trim();         // imagen del boton grabar anular
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "estAlmacen" && row["param"].ToString() == "reparto") vcear = row["valor"].ToString().Trim();         // campo de codigo de estado almacen reparto
                        if (row["campo"].ToString() == "estAlmacen" && row["param"].ToString() == "recepcion") vcead = row["valor"].ToString().Trim();       // estado almacen recepcionado
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
            {   // a.descrip AS CONTENIDO
                string sqlCmd = "SELECT a.marca,a.id,a.iding AS IDING,a.fecingalm AS F_INGRE,l0.descrizionerid AS ALMACEN,lo.descrizionerid AS ORIGEN," +
                    "ld.descrizionerid AS DESTINO,a.gremtra AS GUIA,eg.descrizionerid AS EST_GR,if(gr.tipdocvta='','',concat(dv.descrizionerid,'-',gr.serdocvta,'-',gr.numdocvta)) as DOCVTA," +
                    "ea.descrizionerid AS EST_ALM,a.cantbul AS CANT_B,a.pesokgr AS PESO,a.nombult AS BULTO,gr.nombdegri as DESTINATARIO," +
                    "a.coming AS OBSERVACION,a.unidadrep AS UNI_REP,a.codigorep AS REPARTIDOR,a.fecsalrep AS F_REPARTO " +
                    "FROM cabalmac a " +
                    "LEFT JOIN desc_loc l0 ON l0.IDCodice = a.almacen " +
                    "LEFT JOIN desc_loc lo ON lo.IDCodice = a.locorigen " +
                    "LEFT JOIN desc_loc ld on ld.IDCodice = a.locdestin " +
                    "LEFT JOIN desc_eal ea ON ea.IDCodice = a.estalma " +
                    "LEFT JOIN cabguiai gr on concat(gr.sergui,gr.numgui) = a.gremtra " +
                    "LEFT JOIN desc_est eg ON eg.IDCodice = gr.estadoser " +
                    "lEFT JOIN desc_tdv dv on dv.idcodice = gr.tipdocvta " +
                    "WHERE a.almacen = @loc";
                MySqlCommand micon = new MySqlCommand(sqlCmd, cn);
                micon.Parameters.AddWithValue("@loc", Program.vg_luse);
                micon.CommandTimeout = 300;
                MySqlDataAdapter adr = new MySqlDataAdapter(micon);
                adr.SelectCommand.CommandType = CommandType.Text;
                adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
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
            Font font = new Font("Tahoma", 8);
            advancedDataGridView1.RowHeadersWidth = 20;
            advancedDataGridView1.ColumnHeadersHeight = 20;
            advancedDataGridView1.DefaultCellStyle.Font = font;
            //
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            DataGridViewCheckBoxColumn checkColum2 = new DataGridViewCheckBoxColumn();
            //DataGridViewCheckBoxColumn checkmarca = new DataGridViewCheckBoxColumn();
            advancedDataGridView1.AllowUserToAddRows = false;
            //
            advancedDataGridView1.Columns[0].Width = 30;            // marca
            advancedDataGridView1.Columns[1].Width = 40;            // id
            advancedDataGridView1.Columns[1].ReadOnly = true;
            advancedDataGridView1.Columns[2].Width = 40;            // id ingreso
            advancedDataGridView1.Columns[2].ReadOnly = true;
            advancedDataGridView1.Columns[3].Width = 70;            // fecha ingreso
            advancedDataGridView1.Columns[3].ReadOnly = true;
            advancedDataGridView1.Columns[4].Width = 70;            // nombre almacen
            advancedDataGridView1.Columns[4].ReadOnly = true;
            advancedDataGridView1.Columns[5].Width = 70;             // origen
            advancedDataGridView1.Columns[5].ReadOnly = true;
            advancedDataGridView1.Columns[6].Width = 70;             // destino
            advancedDataGridView1.Columns[6].ReadOnly = true;
            advancedDataGridView1.Columns[7].Width = 80;             // guia transportista
            advancedDataGridView1.Columns[7].ReadOnly = true;
            advancedDataGridView1.Columns[8].Width = 70;             // nombre estado de la guia
            advancedDataGridView1.Columns[8].ReadOnly = true;
            advancedDataGridView1.Columns[9].Width = 100;             // documento de venta
            advancedDataGridView1.Columns[9].ReadOnly = true;
            advancedDataGridView1.Columns[10].Width = 70;             // nombre estado almacen
            advancedDataGridView1.Columns[10].ReadOnly = true;
            advancedDataGridView1.Columns[11].Width = 50;            // cant. bultos
            advancedDataGridView1.Columns[11].ReadOnly = true;
            advancedDataGridView1.Columns[12].Width = 70;            // peso en kg
            advancedDataGridView1.Columns[13].ReadOnly = true;
            advancedDataGridView1.Columns[13].Width = 80;            // nombre del bulto
            advancedDataGridView1.Columns[13].ReadOnly = true;
            advancedDataGridView1.Columns[14].Width = 200;           // destinatario de la GR
            advancedDataGridView1.Columns[14].ReadOnly = true;
            advancedDataGridView1.Columns[15].Width = 150;            // observ. ingreso
            advancedDataGridView1.Columns[15].ReadOnly = false;
            // columnas vista reducida false
            checkColumn.Name = "chkreserva";
            checkColumn.HeaderText = "";
            checkColumn.Width = 30;
            checkColumn.ReadOnly = false;
            checkColumn.FillWeight = 10;
            advancedDataGridView1.Columns.Insert(16, checkColumn);
            //
            advancedDataGridView1.Columns[17].Width = 70;            // unidad de reparto
            advancedDataGridView1.Columns[17].ReadOnly = true;
            advancedDataGridView1.Columns[18].Width = 70;            // repartidor
            advancedDataGridView1.Columns[18].ReadOnly = true;
            advancedDataGridView1.Columns[19].Width = 70;            // fecha salida a reparto
            advancedDataGridView1.Columns[19].ReadOnly = true;
            //
            checkColum2.Name = "chksalida";
            checkColum2.HeaderText = "";
            checkColum2.Width = 30;
            checkColum2.ReadOnly = false;
            checkColum2.FillWeight = 10;
            advancedDataGridView1.Columns.Insert(20, checkColum2);
            //
        }
        private void init()                                                             // inicializa ancho de columnas grilla de filtros
        {
            this.BackColor = Color.FromName(colback);                               // color de fondo
            panel1.BackColor = Color.FromName(colpage);                             // color de los pageframes
            advancedDataGridView1.BackgroundColor = Color.FromName(colgrid);        // color de las grillas
            dataGridView1.BackgroundColor = Color.FromName(colgrid);
            dataGridView2.BackgroundColor = Color.FromName(colgrid);
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
            //
            dataGridView2.AllowUserToResizeColumns = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView2.ColumnCount = (advancedDataGridView1.Rows.Count > 0) ? advancedDataGridView1.Rows[0].Cells.Count : advancedDataGridView1.ColumnCount;
            dataGridView1.ColumnCount = 0;
            dataGridView2.ColumnHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView2.Rows.Add();
            for (int i = 0; i < ((advancedDataGridView1.Rows.Count > 0) ? advancedDataGridView1.Rows[0].Cells.Count : advancedDataGridView1.Columns.Count); i++)
            {
                dataGridView2.Columns[i].Width = advancedDataGridView1.Columns[i].Width;
                dataGridView2.Columns[i].Name = advancedDataGridView1.Columns[i].Name;
                //
                DataGridViewCheckBoxColumn checkver = new DataGridViewCheckBoxColumn();
                checkver.Name = advancedDataGridView1.Columns[i].Name;    //"vc"+i.ToString();
                checkver.HeaderText = "";
                checkver.Width = advancedDataGridView1.Columns[i].Width;
                checkver.ReadOnly = false;
                checkver.FillWeight = 10;
                dataGridView1.Columns.Insert(i, checkver);
            }
            dataGridView1.Rows.Add();
            dataGridView2.Columns["id"].ReadOnly = true;
        }
        private void cvc()                                                              // checks de visualizacion de columnas
        {
            if (advancedDataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i <= advancedDataGridView1.Rows[0].Cells.Count - 1; i++)  // dataGridView1 -2
                {
                    if (advancedDataGridView1.Columns[i].Visible == true)
                    {
                        dataGridView1.Rows[0].Cells[i].Value = true;
                    }
                    else
                    {
                        dataGridView1.Rows[0].Cells[i].Value = false;
                    }
                }
            }
        }
        private void cellsum(int ind)                                                   // suma la columna especificada
        {
            tx_tarti.Text = (advancedDataGridView1.Rows.Count).ToString();
            decimal b = 0, c = 0;
            string qw = "PESO";
            string qe = "CANT_B";
            foreach (DataGridViewRow r in advancedDataGridView1.Rows)
            {
                if (r.Cells[qw].Value != null && r.Cells[qw].Value != DBNull.Value) b += Convert.ToDecimal(r.Cells[qw].Value);  // total peso
                if (r.Cells[qe].Value != null && r.Cells[qe].Value != DBNull.Value) c += Convert.ToDecimal(r.Cells[qe].Value);  // total bultos
            }
            tx_totprec.Text = b.ToString("###,###,##0.00");
            tx_bultos.Text = c.ToString("###,##0");
        }
        private void filtros(string expres)                                             // filtros de nivel superior
        {
            dv = new DataView(dt);
            dv.RowFilter = expres;
            dt = dv.ToTable();
            //advancedDataGridView1.Columns.Remove("marca");
            advancedDataGridView1.Columns.Remove("chkreserva");
            advancedDataGridView1.Columns.Remove("chksalida");
            advancedDataGridView1.DataSource = dt;
            grilla();
            cellsum(0);
            rb_redu_CheckedChanged(null, null);
            rb_todos_CheckedChanged(null, null);
        }
        private bool vali_alm(string codi)                                              // valida almacen
        {
            bool retorna = false;
            string DB_CONN_STR0 = DB_CONN_STR;
            MySqlConnection cn0 = new MySqlConnection(DB_CONN_STR0);
            cn0.Open();
            try
            {
                string sqlCmd = "select count(*) from desc_loc where idcodice=@valm";
                MySqlCommand micon = new MySqlCommand(sqlCmd, cn0);
                micon.Parameters.AddWithValue("@valm", codi);
                MySqlDataReader dr = micon.ExecuteReader();
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        if (dr.GetInt16(0) > 0)
                        {
                            dr.Close();
                            retorna = true;
                        }
                        else dr.Close();
                    }
                }
                else
                {
                    dr.Close();
                    retorna = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en vali alm");
                cn0.Close();
                cn0.Dispose(); // return connection to pool
                Application.Exit();
            }
            cn0.Close();
            cn0.Dispose(); // return connection to pool
            return retorna;
        }
        private void grabacam(int idm, string campo, string valor, string almac)        // graba el cambio en la tabla cabalmac
        {
            string campoT = "";
            switch(campo)
            {
                case "OBSERVACION":
                    campoT = "coming";
                    break;
                case "ALMACEN":
                    // aca habria que dar salida a la guia del actual almacen
                    // y dar ingreso al nuevo almacen
                    break;
                case "marca":
                    campoT = "marca";
                    break;
            }
            string DB_CONN_STR1 = DB_CONN_STR;
            MySqlConnection cn0 = new MySqlConnection(DB_CONN_STR1);
            cn0.Open();
            try
            {
                string sqlCmd = "update cabalmac set " + campoT + "=@val where id=@idm";   // debería deshabilitarse esto porque cualquier cambio en el codigo
                MySqlCommand micon = new MySqlCommand(sqlCmd, cn0);                     // afecta al kardex porque pasa a ser otro producto!
                micon.Parameters.AddWithValue("@val", valor);
                micon.Parameters.AddWithValue("@idm", idm);
                micon.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en grabacam");
                cn0.Close();
                cn0.Dispose(); // return connection to pool
                Application.Exit();
            }
            cn0.Close();
            cn0.Dispose(); // return connection to pool
        }
        private void busmarcas()                                                        // busca y guarda las marcas de visualizacion vertical
        {
            for (int i = 0; i < dataGridView1.Rows[0].Cells.Count - 2; i++)
            {
                marcas.Add((dataGridView1.Rows[0].Cells[i].Value.ToString() == "True") ? true : false);
            }
        }
        private void restauramar()                                                      // restaura las visualizaciones segun la marca
        {
            for (int i = 0; i <= dataGridView1.Rows[0].Cells.Count - 3; i++)
            {
                if (marcas.ElementAt(i).ToString() == "True")
                {
                    dataGridView1.Rows[0].Cells[i].Value = true;
                    dataGridView1.Columns[i].Visible = true;
                }
                else
                {
                    dataGridView1.Rows[0].Cells[i].Value = false;
                    dataGridView1.Columns[i].Visible = false;
                    dataGridView2.Columns[i].Visible = false;
                    advancedDataGridView1.Columns[i].Visible = false;
                }
            }
        }
        private void selec()                                                            // pone color de seleccion si esta con check
        {
            for (int i = 0; i < advancedDataGridView1.Rows.Count - 1; i++)
            {
                if (advancedDataGridView1.Rows[i].Cells[advancedDataGridView1.Columns["marca"].Index].Value.ToString() == "True")
                {
                    advancedDataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
                }
            }
        }
        private bool quitareserv(string idr)                                            // borramos la asignacion de la GR al repartidor/unidad
        {
            bool retorna = false;   // , string ida, string contra
            using (MySqlConnection cn = new MySqlConnection(DB_CONN_STR))
            {
                if (lib.procConn(cn) == true)
                {
                    string actua = "UPDATE cabalmac a LEFT JOIN controlg c ON CONCAT(c.serguitra,c.numguitra)=a.gremtra " +
                        "SET a.unidadrep = '',a.codigorep = '',a.fecsalrep = NULL,c.estalma = @cea " +
                        "WHERE a.id = @idr";
                    using (MySqlCommand micon = new MySqlCommand(actua, cn))
                    {
                        micon.Parameters.AddWithValue("@idr", idr);
                        micon.Parameters.AddWithValue("@cea", vcead);
                        micon.ExecuteNonQuery();
                        retorna = true;
                    }
                }
            }
            return retorna;
        }
        private void despacho(string tipo, int rowind)
        {
            // primero validamos
            int fi = 0;
            string[,] pasa = new string[10, 8]
            {
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" }
            };
            if (tipo == "masivo")
            {
                for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
                {
                    if (advancedDataGridView1.Rows[i].Cells["marca"].FormattedValue.ToString() == "True")
                    {
                        fi = fi + 1;
                    }
                }
                if (fi > 0 && fi < 11)
                {
                    try
                    {
                        int conta = 0;
                        for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
                        {
                            if (advancedDataGridView1.Rows[i].Cells["marca"].FormattedValue.ToString() == "True")
                            {
                                string id = advancedDataGridView1.Rows[i].Cells["id"].FormattedValue.ToString();
                                string co = advancedDataGridView1.Rows[i].Cells["GUIA"].Value.ToString();
                                string no = advancedDataGridView1.Rows[i].Cells["CANT_B"].FormattedValue.ToString();
                                string al = advancedDataGridView1.Rows[i].Cells["ALMACEN"].FormattedValue.ToString();
                                pasa[conta, 0] = id;
                                pasa[conta, 1] = co;
                                pasa[conta, 2] = no;
                                pasa[conta, 3] = al;
                                pasa[conta, 7] = advancedDataGridView1.Rows[i].Cells["DESTINATARIO"].FormattedValue.ToString();
                                conta = conta + 1;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error - no se pudo insertar");
                        Application.Exit();
                        return;
                    }
                }
            }
            if (tipo == "individual")
            {
                fi = 1;
                string id = advancedDataGridView1.Rows[rowind].Cells["id"].FormattedValue.ToString();
                string co = advancedDataGridView1.Rows[rowind].Cells["GUIA"].Value.ToString();
                string no = advancedDataGridView1.Rows[rowind].Cells["CANT_B"].FormattedValue.ToString();
                string al = advancedDataGridView1.Rows[rowind].Cells["ALMACEN"].FormattedValue.ToString();
                pasa[0, 0] = id;
                pasa[0, 1] = co;
                pasa[0, 2] = no;
                pasa[0, 3] = al;
                pasa[0, 7] = advancedDataGridView1.Rows[rowind].Cells["DESTINATARIO"].FormattedValue.ToString();
            }
            if (fi > 0)
            {
                // vamos a llamar a movimas
                movimas resem = new movimas("reserva", Program.vg_luse, pasa);
                var result = resem.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    if (resem.retorno == true)
                    {
                        MySqlConnection cnx = new MySqlConnection(DB_CONN_STR);
                        if (lib.procConn(cnx) == true)
                        {
                            try
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    DataRow fila = dt.Rows[i];
                                    for (int r = 0; r < 10; r++)
                                    {
                                        if (fila[1].ToString() == resem.para3[r, 0].ToString())
                                        {
                                            dt.Rows[i]["UNI_REP"] = resem.para3[r, 6].ToString();
                                            dt.Rows[i]["REPARTIDOR"] = resem.para3[r, 4].ToString();
                                            dt.Rows[i]["F_REPARTO"] = resem.para3[r, 5].ToString();
                                            // actualizamos 
                                            string actua = "update cabalmac set unidadrep=@ure,codigorep=@res,fecsalrep=@con,marca=0 where id=@idr";
                                            MySqlCommand miact = new MySqlCommand(actua, cnx);
                                            miact.Parameters.AddWithValue("@ure", resem.para3[r, 6].ToString());
                                            miact.Parameters.AddWithValue("@res", resem.para3[r, 4].ToString());
                                            miact.Parameters.AddWithValue("@con", resem.para3[r, 5].ToString().Substring(6, 4) + "-" + resem.para3[r, 5].ToString().Substring(3, 2) + "-" + resem.para3[r, 5].ToString().Substring(0, 2));
                                            miact.Parameters.AddWithValue("@idr", resem.para3[r, 0].ToString());
                                            miact.ExecuteNonQuery();
                                            dt.Rows[i]["marca"] = 0;
                                            miact.Dispose();
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
                        cnx.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una acción individual", "Deben ser entre 1 y 10 Guías");
            }
        }
        #endregion

        #region botones_click
        private void bt_borra_Click(object sender, EventArgs e)                         // Boton para reiniciar toda la grilla
        {
            int udt = 0;
            if (rb_estan.Checked == true)
            {
                udt = 1;
            }
            if (rb_redu.Checked == true) udt = 2;
            if (rb_todos.Checked == true) udt = 3;
            busmarcas();    // visualizacion de columnas
            dt.Rows.Clear();
            dataGridView2.Rows.Clear();
            dt.DefaultView.RowFilter = "";
            advancedDataGridView1.DataSource = null;
            advancedDataGridView1.Rows.Clear();
            //advancedDataGridView1.Columns.Remove("marca");
            advancedDataGridView1.Columns.Remove("chkreserva");
            advancedDataGridView1.Columns.Remove("chksalida");
            jaladat();
            advancedDataGridView1.DataSource = dt;
            grilla();
            init();
            cvc();
            cellsum(0);
            rb_estan.Checked = false;
            rb_redu.Checked = false;
            rb_todos.Checked = false;
            restauramar();
            selec();
            switch (udt)
            {
                case 1:
                    rb_estan.PerformClick();
                    break;
                case 2:
                    rb_redu.PerformClick();
                    break;
                case 3:
                    rb_todos.PerformClick();
                    break;
            }
        }
        private void bt_reserva_Click(object sender, EventArgs e)                       // asignacion masiva a reparto
        {
            despacho("masivo",0);
            /* primero validamos
            int fi = 0;
            for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
            {
                if (advancedDataGridView1.Rows[i].Cells["marca"].FormattedValue.ToString() == "True")
                {
                    fi = fi + 1;
                }
            }
            if (fi > 1 && fi<11)
            {
                string[,] pasa = new string[10, 8]
                {
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" }
                };
                try
                {
                    int conta = 0;
                    for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
                    {
                        if (advancedDataGridView1.Rows[i].Cells["marca"].FormattedValue.ToString() == "True")
                        {
                            string id = advancedDataGridView1.Rows[i].Cells["id"].FormattedValue.ToString();
                            string co = advancedDataGridView1.Rows[i].Cells["GUIA"].Value.ToString();
                            string no = advancedDataGridView1.Rows[i].Cells["CANT_B"].FormattedValue.ToString();
                            string al = advancedDataGridView1.Rows[i].Cells["ALMACEN"].FormattedValue.ToString();
                            pasa[conta, 0] = id;
                            pasa[conta, 1] = co;
                            pasa[conta, 2] = no;
                            pasa[conta, 3] = al;
                            pasa[conta, 7] = advancedDataGridView1.Rows[i].Cells["DESTINATARIO"].FormattedValue.ToString();
                            conta = conta + 1;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error - no se pudo insertar");
                    Application.Exit();
                    return;
                }
                // vamos a llamar a movimas
                movimas resem = new movimas("reserva", "", pasa);
                var result = resem.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    if (resem.retorno == true)
                    {
                        MySqlConnection cnx = new MySqlConnection(DB_CONN_STR);
                        if (lib.procConn(cnx) == true)
                        {
                            try
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    DataRow fila = dt.Rows[i];
                                    for (int r=0; r < 10; r++ )
                                    {
                                        if (fila[1].ToString() == resem.para3[r, 0].ToString())
                                        {
                                            dt.Rows[i]["UNI_REP"] = resem.para3[r, 6].ToString();
                                            dt.Rows[i]["REPARTIDOR"] = resem.para3[r, 4].ToString();
                                            dt.Rows[i]["F_REPARTO"] = resem.para3[r, 5].ToString();
                                            // actualizamos 
                                            string actua = "update cabalmac set unidadrep=@ure,codigorep=@res,fecsalrep=@con,marca=0 where id=@idr";
                                            MySqlCommand miact = new MySqlCommand(actua, cnx);
                                            miact.Parameters.AddWithValue("@ure", resem.para3[r, 6].ToString());
                                            miact.Parameters.AddWithValue("@res", resem.para3[r, 4].ToString());
                                            miact.Parameters.AddWithValue("@con", resem.para3[r, 5].ToString().Substring(6,4) + "-" + resem.para3[r, 5].ToString().Substring(3, 2) + "-" + resem.para3[r, 5].ToString().Substring(0, 2));
                                            miact.Parameters.AddWithValue("@idr", resem.para3[r, 0].ToString());
                                            miact.ExecuteNonQuery();
                                            dt.Rows[i]["marca"] = 0;
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
                        cnx.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una acción individual","Deben ser entre 1 y 10 Guías");
            }
            */
        }
        private void bt_salida_Click(object sender, EventArgs e)                        // salida de mercaderia hacia cliente
        {
            // primero validamos
            int fi = 0;
            for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
            {
                if (advancedDataGridView1.Rows[i].Cells["marca"].FormattedValue.ToString() == "True")
                {
                    fi = fi + 1;
                }
            }
            if (fi > 1)
            {

                MySqlConnection cn = new MySqlConnection(DB_CONN_STR);
                if (lib.procConn(cn) != true)
                {
                    MessageBox.Show("Error de conectividad","Se debe reiniciar");
                    Application.Exit();
                    return;
                }
                string[,] pasa = new string[10, 8]
                {
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" },
                    {"","","","","","","", "" }
                };
                try
                {
                    int conta = 0;
                    for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
                    {
                        if (advancedDataGridView1.Rows[i].Cells["marca"].FormattedValue.ToString() == "True")
                        {
                            string id = advancedDataGridView1.Rows[i].Cells["id"].FormattedValue.ToString();
                            string co = advancedDataGridView1.Rows[i].Cells["GUIA"].Value.ToString();
                            string no = advancedDataGridView1.Rows[i].Cells["CANT_B"].FormattedValue.ToString();
                            string al = advancedDataGridView1.Rows[i].Cells["ALMACEN"].FormattedValue.ToString();
                            pasa[conta, 0] = id;
                            pasa[conta, 1] = co;
                            pasa[conta, 2] = no;
                            pasa[conta, 3] = al;
                            pasa[conta, 4] = advancedDataGridView1.Rows[i].Cells["REPARTIDOR"].FormattedValue.ToString();
                            pasa[conta, 5] = advancedDataGridView1.Rows[i].Cells["F_REPARTO"].FormattedValue.ToString();
                            pasa[conta, 6] = advancedDataGridView1.Rows[i].Cells["UNI_REP"].FormattedValue.ToString();
                            conta = conta + 1;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error - no se pudo insertar");
                    Application.Exit();
                    return;
                }
                // vamos a llamar a movimas
                movimas resem = new movimas("salida", Program.vg_luse, pasa);    // modo,array,libre
                var result = resem.ShowDialog();
                if (result == DialogResult.Cancel)  // deberia ser OK, pero que chuuu
                {
                    if (resem.retorno == true)
                    {
                        // actualizamos el datagridview
                        for (int i = 0; i < advancedDataGridView1.Rows.Count; i++)
                        {
                            for (int x=0; x<10; x++)
                            {
                                if (advancedDataGridView1.Rows[i].Cells["id"].Value.ToString() == pasa[x, 0])
                                {
                                    if (pasa[i, 1] != "")
                                    {
                                        advancedDataGridView1.Rows.RemoveAt(i);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una acción individual");
            }
        }
        private void pan_inicio_Enter(object sender, EventArgs e)                       // llamamos al procedimiento que colorea las filas seleccionadas
        {
            selec();
        }
        private void bt_bmf_Click(object sender, EventArgs e)                           // BORRA LAS MARCAS DE SELECCION DE FILAS
        {
            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
            {
                if (row.Cells["marca"].FormattedValue.ToString() == "True")
                {
                    int mark = 0;
                    row.Cells["marca"].Value = mark;
                    grabacam(int.Parse(row.Cells["id"].Value.ToString()), "marca", mark.ToString(), "");
                }
            }
        }
        private void bt_etiq_Click(object sender, EventArgs e)                          // imprime cargo de entrega a repartidor?
        {
            if(advancedDataGridView1.Enabled == true && advancedDataGridView1.CurrentRow.Index >= 0)
            {
                if (advancedDataGridView1.CurrentRow.Index >= 0)
                {
                    var aa = MessageBox.Show("Impresión de Etiquetas para el artículo" + Environment.NewLine +
                        "Esta listo para la imprimir?", "Rutina de impresión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        string id_mueble = advancedDataGridView1.CurrentRow.Cells["id"].Value.ToString();
                        string tx_cant = " ";
                        string tx_paq = " ";
                        //
                        string cap = advancedDataGridView1.CurrentRow.Cells["capit"].Value.ToString();
                        string mod = advancedDataGridView1.CurrentRow.Cells["model"].Value.ToString();
                        string mad = advancedDataGridView1.CurrentRow.Cells["mader"].Value.ToString();
                        string tip = advancedDataGridView1.CurrentRow.Cells["tipol"].Value.ToString();
                        string dt1 = advancedDataGridView1.CurrentRow.Cells["deta1"].Value.ToString();
                        string aca = advancedDataGridView1.CurrentRow.Cells["acaba"].Value.ToString();
                        string tal = advancedDataGridView1.CurrentRow.Cells["talle"].Value.ToString();
                        string dt2 = advancedDataGridView1.CurrentRow.Cells["deta2"].Value.ToString();
                        string dt3 = advancedDataGridView1.CurrentRow.Cells["deta3"].Value.ToString();
                        string jgo = advancedDataGridView1.CurrentRow.Cells["juego"].Value.ToString();
                        string nom = advancedDataGridView1.CurrentRow.Cells["nombr"].Value.ToString();
                        string med = advancedDataGridView1.CurrentRow.Cells["medid"].Value.ToString();
                        /* llama al form impresor con los valores actuales
                        impresor impetiq = new impresor(cap, mod, mad, tip, dt1, aca, tal,
                            dt2, dt3, jgo, nom, med, tx_cant, tx_paq, int.Parse(id_mueble));
                        impetiq.Show();
                        */
                    }
                }
            }
        }
        #endregion

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                //iCount = 0;
                bFirstPage = true;
                bNewPage = true;

                // Calculating Total Widths
                iTotalWidth = 0;
                totcolv = 0;
                foreach (DataGridViewColumn dgvGridCol in advancedDataGridView1.Columns)
                {
                    if (dgvGridCol.Visible == true && dgvGridCol.IsDataBound == true)
                    {
                        iTotalWidth += dgvGridCol.Width;
                        totcolv += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string lb_titulo = this.Text;
            try
            {
                //Set the left margin
                int iLeftMargin = e.MarginBounds.Left;
                //Set the top margin
                int iTopMargin = e.MarginBounds.Top;
                //Whether more pages have to print or not
                bool bMorePagesToPrint = false;
                int iTmpWidth = 0;
                //For the first page to print set the cell width and header height
                if (bFirstPage)
                {
                    foreach (DataGridViewColumn GridCol in advancedDataGridView1.Columns)
                    {
                        if (GridCol.Visible == true && GridCol.IsDataBound == true)
                        {
                            iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                (double)iTotalWidth * (double)iTotalWidth *
                                ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                            iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                            // Save width and height of headers
                            arrColumnLefts.Add(iLeftMargin);
                            arrColumnWidths.Add(iTmpWidth);
                            iLeftMargin += iTmpWidth;
                        }
                    }
                }
                //Loop till all the grid rows not get printed
                while (iRow <= advancedDataGridView1.Rows.Count - 1)
                {
                    DataGridViewRow GridRow = advancedDataGridView1.Rows[iRow];
                    //Set the cell height
                    iCellHeight = GridRow.Height - 10;       // + 5              ********************************************
                    int iCount = 0;
                    //Check whether the current page settings allows more rows to print
                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        Font titulo = new Font("Arial", 7);// para el titulo de columnas y dentro de la grilla
                        Font normal = new Font("Arial", 6);// para el titulo de columnas y dentro de la grilla
                        if (bNewPage)
                        {
                            //Draw Header
                            e.Graphics.DrawString(lb_titulo,
                                new Font(advancedDataGridView1.Font, FontStyle.Bold),
                                Brushes.Black, e.MarginBounds.Left,
                                e.MarginBounds.Top - e.Graphics.MeasureString(lb_titulo,
                                new Font(dataGridView1.Font, FontStyle.Bold),
                                e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " +
                                DateTime.Now.ToShortTimeString();
                            //Draw Date
                            e.Graphics.DrawString(strDate,
                                new Font(advancedDataGridView1.Font, FontStyle.Bold), Brushes.Black,
                                e.MarginBounds.Left +
                                (e.MarginBounds.Width - e.Graphics.MeasureString(strDate,
                                new Font(advancedDataGridView1.Font, FontStyle.Bold),
                                e.MarginBounds.Width).Width),
                                e.MarginBounds.Top - e.Graphics.MeasureString(lb_titulo,
                                new Font(new Font(advancedDataGridView1.Font, FontStyle.Bold),
                                FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            //Draw Columns                 
                            iTopMargin = e.MarginBounds.Top;
                            foreach (DataGridViewColumn GridCol in advancedDataGridView1.Columns)
                            {
                                if (GridCol.Visible == true && GridCol.IsDataBound == true)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawRectangle(Pens.Black,
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawString(GridCol.Name.ToString(),
                                        titulo,
                                        new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);   // HeaderText
                                    iCount++;
                                }
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;
                        //Draw Columns Contents                
                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {
                            if (Cel.Value != null && Cel.Visible == true)
                            {
                                if (Cel.Value.GetType().ToString() == "System.DateTime")   //Cel.ValueType.ToString() == "System.DateTime"
                                {   // 
                                    e.Graphics.DrawString(Cel.Value.ToString().Substring(0, 10),
                                    normal,
                                    new SolidBrush(Cel.InheritedStyle.ForeColor),
                                    new RectangleF((int)arrColumnLefts[iCount],
                                    (float)iTopMargin,
                                    (int)arrColumnWidths[iCount], (float)iCellHeight)
                                    );
                                }
                                else
                                {
                                    e.Graphics.DrawString(Cel.Value.ToString(),
                                    normal,
                                    new SolidBrush(Cel.InheritedStyle.ForeColor),
                                    new RectangleF((int)arrColumnLefts[iCount],
                                    (float)iTopMargin,
                                    (int)arrColumnWidths[iCount], (float)iCellHeight),
                                    strFormat);
                                }
                                //Drawing Cells Borders 
                                e.Graphics.DrawRectangle(Pens.Black,
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iCellHeight));
                                iCount++;
                            }
                        }
                    }
                    iRow++;
                    iTopMargin += iCellHeight;
                    if (iTopMargin <= e.PageBounds.Height)
                    {
                        e.HasMorePages = false;
                    }
                    else
                    {
                        e.HasMorePages = true;
                    }
                }
                //If more lines exist, print another page.
                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
                return;     // lo acabo de poner 08-03-2018 
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
            bFirstPage = true;
            bNewPage = true;
            iRow = 0;
        }

        #region radiobuttons - checked changed
        private void rb_estan_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_estan.Checked == true && advancedDataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < advancedDataGridView1.Rows[0].Cells.Count; i++)
                {
                    advancedDataGridView1.Columns[i].Visible = true;
                    dataGridView1.Columns[i].Visible = true;
                    dataGridView2.Columns[i].Visible = true;
                }
                advancedDataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                dataGridView2.Columns[5].Visible = false;
                //
                advancedDataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView2.Columns[6].Visible = false;
                //
            }
        }
        private void rb_redu_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_redu.Checked == true && advancedDataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < advancedDataGridView1.Rows[0].Cells.Count; i++)
                {
                    advancedDataGridView1.Columns[i].Visible = true;
                    dataGridView1.Columns[i].Visible = true;
                    dataGridView2.Columns[i].Visible = true;
                }
                advancedDataGridView1.Columns[2].Visible = false;
                dataGridView1.Columns[2].Visible = false;
                dataGridView2.Columns[2].Visible = false;
                //
                advancedDataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[4].Visible = false;
                dataGridView2.Columns[4].Visible = false;
                //
                advancedDataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                dataGridView2.Columns[5].Visible = false;
                //
                advancedDataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView2.Columns[6].Visible = false;
                //
            }
        }
        private void rb_todos_CheckedChanged(object sender, EventArgs e)
        {
            if (advancedDataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.Rows[0].Cells.Count; i++)
                {
                    dataGridView1.Rows[0].Cells[i].Value = true;
                    dataGridView1.Columns[i].Visible = true;
                    dataGridView2.Columns[i].Visible = true;
                    advancedDataGridView1.Columns[i].Visible = true;
                }
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
            //button1.Image = Image.FromFile(img_grab);
            panel1.Enabled = true;
            dataGridView1.Enabled = true;
            dataGridView2.Enabled = true;
            advancedDataGridView1.Enabled = true;
            bt_reserva.Enabled = true;
            bt_salida.Enabled = true;
            bt_borra.Enabled = true;
            rb_redu.Enabled = true;
            rb_todos.Enabled = true;
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "EDITAR";
            panel1.Enabled = true;
            dataGridView1.Enabled = true;
            dataGridView2.Enabled = true;
            advancedDataGridView1.Enabled = true;
            bt_reserva.Enabled = true;
            bt_salida.Enabled = true;
            bt_borra.Enabled = true;
            rb_redu.Enabled = true;
            rb_todos.Enabled = true;
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "IMPRIMIR";
            //Open the print preview dialog
            System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
            pg.Margins.Top = 50;
            pg.Margins.Bottom = 0;
            pg.Margins.Left = 50;
            pg.Margins.Right = 0;
            pg.Landscape = true;
            printDocument1.DefaultPageSettings = pg;

            iRow = 0; // a ver a ver
            PrintPreviewDialog objPPdialog = new PrintPreviewDialog();
            objPPdialog.Document = printDocument1;
            objPPdialog.ShowDialog();
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "ANULAR";
            // no tiene funcion en este form
        }
        private void bt_exc_Click(object sender, EventArgs e)
        {
            string nombre = "Stock_al_" + DateTime.Now.ToShortDateString() + "_.xlsx";
            var aa = MessageBox.Show("Confirma que desea generar la hoja de calculo?",
            "Archivo: " + nombre, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (aa == DialogResult.Yes)
            {
                var wb = new XLWorkbook();
                DataTable datexc = (DataTable)(advancedDataGridView1.DataSource);
                wb.Worksheets.Add(datexc, "Inventario");
                wb.SaveAs(nombre);
                MessageBox.Show("Archivo generado con exito!");
            }
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

        #region grillas 1, 2 y advanced
        private void dataGridView2_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.CurrentCell.Value != null)
            {
                // string frase = dataGridView2.Columns[e.ColumnIndex].Name.ToString() + " like '" + dataGridView2.CurrentCell.Value.ToString() + "*'";
                // filtros(frase);
                string nomcol = dataGridView2.Columns[e.ColumnIndex].Name.ToString();
                string valcol = dataGridView2.CurrentCell.Value.ToString();
                DataRow[] row = dt.Select("[" + nomcol + "] LIKE '%" + valcol + "%'");
                int fila = int.Parse(row[0].ItemArray[1].ToString());
                dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                advancedDataGridView1.Focus();
                for (int i=0; i<advancedDataGridView1.Rows.Count -1; i++)
                {
                    if (advancedDataGridView1.Rows[i].Cells[3].Value.ToString().Equals(fila.ToString()))
                    {
                        advancedDataGridView1.CurrentCell = advancedDataGridView1.Rows[i].Cells[2];
                        break;
                    }
                }
                //advancedDataGridView1.CurrentCell = advancedDataGridView1.Rows[fila].Cells[2];
            }
        }
        private void advancedDataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                valant = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                codant = advancedDataGridView1.Rows[e.RowIndex].Cells["GUIA"].Value.ToString();
            }
        }
        private void advancedDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                valnue = advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int index = advancedDataGridView1.Columns["id"].Index;
                switch (advancedDataGridView1.Columns[e.ColumnIndex].Name)
                {
                    case "marca": 
                        int mark = (advancedDataGridView1.Rows[e.RowIndex].Cells[advancedDataGridView1.Columns["marca"].Index].Value.ToString() == "False") ? 0 : 1;
                        grabacam(int.Parse(advancedDataGridView1.Rows[e.RowIndex].Cells[advancedDataGridView1.Columns["id"].Index].Value.ToString()),
                                        advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString(), mark.ToString(), "");
                        break;
                    case "ALMACEN":
                        if ((advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "" ||
                            advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != null) &&
                            valant != "" && valant != valnue)
                        {
                            if (vali_alm(valnue) == true)
                            {
                                var aa = MessageBox.Show("Desea MOVER el mueble al almacén ingresado?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (aa == DialogResult.Yes)
                                {
                                    // ejecuta el proceso interno de cambio de almacen
                                    grabacam(int.Parse(advancedDataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString()),
                                        advancedDataGridView1.Columns[e.ColumnIndex].Name.ToString(), valnue, "");
                                }
                                else
                                {
                                    // regresa el valor anterior de la columna almacen
                                    advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = valant;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Almacén incorrecto: " + valnue,
                                    "Verifique por favor", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = valant;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("es null o vacio o valant es igual al titulo", dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        }
                        break;
                    case "OBSERVACION":
                        var ao = MessageBox.Show("Modifica la observación?", "Confirme por favor",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ao == DialogResult.Yes)
                        {
                            advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = valnue;
                            grabacam(int.Parse(advancedDataGridView1.Rows[e.RowIndex].Cells[index].Value.ToString()),
                                advancedDataGridView1.Columns[e.ColumnIndex].HeaderText.ToString(), valnue, "");
                        }
                        else
                        {
                            advancedDataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = valant;
                        }
                        break;
                }
            }
        }
        private void advancedDataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (dataGridView2.ColumnCount > 1)
            {
                dataGridView2.Columns[e.Column.Name].Width = e.Column.Width;
                dataGridView1.Columns[e.Column.Name].Width = e.Column.Width;
            }
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
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void advancedDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (advancedDataGridView1.Columns[e.ColumnIndex].Name == "GUIA" && !string.IsNullOrEmpty(valant))
            {
                // se hace en cellendedit
            }
            if (advancedDataGridView1.Columns[e.ColumnIndex].Name == "marca")
            {
                if (advancedDataGridView1.CurrentCell.FormattedValue.ToString() == "True")
                {
                    advancedDataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
                }
                else
                {
                    advancedDataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
            if (advancedDataGridView1.Columns[e.ColumnIndex].Name == "chkreserva")          // salida a despacho
            {
                if (advancedDataGridView1.CurrentCell != null &&
                    advancedDataGridView1.CurrentCell.FormattedValue.ToString() == "True")
                {
                    if (advancedDataGridView1.CurrentCell.FormattedValue.ToString() == "True" && 
                        string.IsNullOrWhiteSpace(advancedDataGridView1.Rows[e.RowIndex].Cells["UNI_REP"].Value.ToString()))
                    {
                        despacho("individual", e.RowIndex);
                    }
                    else
                    {
                        var aa = MessageBox.Show("Realmente retorna la guía y mercadería?", "Confirme por favor",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (aa == DialogResult.Yes)
                        {
                            if (quitareserv(advancedDataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString()) == true)
                            {
                                advancedDataGridView1.Rows[e.RowIndex].Cells["UNI_REP"].Value = "";
                                advancedDataGridView1.Rows[e.RowIndex].Cells["REPARTIDOR"].Value = "";
                                advancedDataGridView1.Rows[e.RowIndex].Cells["F_REPARTO"].Value = "";
                            }
                        }
                        else
                        {
                            advancedDataGridView1.CurrentCell.Value = false;
                        }
                    }
                }
            }
            if (advancedDataGridView1.Columns[e.ColumnIndex].Name == "chksalida")               // entrega al cliente
            {
                if (advancedDataGridView1.CurrentCell != null &&
                    advancedDataGridView1.CurrentCell.FormattedValue.ToString() == "True")
                {
                    if (advancedDataGridView1.CurrentCell.FormattedValue.ToString() == "True")
                    {
                        var aa = MessageBox.Show("Realmente desea entregar la mercadería?", "Confirme por favor",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            string[] pasa = new string[8]
                            {"","","","","","","", "" };
                            pasa[0] = advancedDataGridView1.Rows[e.RowIndex].Cells["id"].FormattedValue.ToString();
                            pasa[1] = advancedDataGridView1.Rows[e.RowIndex].Cells["GUIA"].Value.ToString();
                            pasa[2] = advancedDataGridView1.Rows[e.RowIndex].Cells["CANT_B"].FormattedValue.ToString();
                            pasa[3] = advancedDataGridView1.Rows[e.RowIndex].Cells["ALMACEN"].FormattedValue.ToString();
                            pasa[4] = advancedDataGridView1.Rows[e.RowIndex].Cells["REPARTIDOR"].FormattedValue.ToString();
                            pasa[5] = advancedDataGridView1.Rows[e.RowIndex].Cells["F_REPARTO"].FormattedValue.ToString();
                            pasa[6] = advancedDataGridView1.Rows[e.RowIndex].Cells["UNI_REP"].FormattedValue.ToString();
                            pasa[7] = advancedDataGridView1.Rows[e.RowIndex].Cells["DESTINATARIO"].FormattedValue.ToString();
                            movim rese = new movim("salida",pasa);
                            var result = rese.ShowDialog();
                            if (result == DialogResult.Cancel)  // deberia ser OK, pero que chuuu .... no sea aaa
                            {
                                if (rese.retorno == false) advancedDataGridView1.CurrentCell.Value = false;
                                else
                                {
                                    advancedDataGridView1.CurrentCell.Value = true;
                                    //if (advancedDataGridView1.CurrentRow.Cells["salida"].Value.ToString() == "0")
                                    {
                                        advancedDataGridView1.Rows.RemoveAt(e.RowIndex);
                                    }
                                }
                            }
                        }
                        else
                        {
                            advancedDataGridView1.CurrentCell.Value = false;
                        }
                    }
                    else
                    {
                        /* // UNA VEZ SALIDA LA MERCA ...REGRESA ??? .... NO, por esta opcion NO
                        var aa = MessageBox.Show("Realmente desea BORRAR la autorización de salida?", "Confirme por favor",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);         
                        if (aa == DialogResult.Yes)
                        {
                            if (quitasalida(advancedDataGridView1.Rows[e.RowIndex].Cells["salida"].Value.ToString(),
                                advancedDataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString()
                                ) == true)
                            {
                                // borra las marcas en la grilla
                                advancedDataGridView1.CurrentRow.Cells["salida"].Value = "";
                                advancedDataGridView1.CurrentRow.Cells["evento"].Value = "";
                                advancedDataGridView1.CurrentRow.Cells["almdes"].Value = "";
                                advancedDataGridView1.CurrentRow.Cells["chksalida"].Value = 0;
                            }
                        }
                        else
                        {
                            advancedDataGridView1.CurrentCell.Value = false;
                        }
                        */
                    }
                }
            }
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                if (dataGridView1.CurrentCell.FormattedValue.ToString() == "False")
                {
                    //string noseve = dataGridView1.Columns[dataGridView1.Columns[e.ColumnIndex].Name.ToString()].ToString();
                    string noseve = dataGridView1.Columns[e.ColumnIndex].Name.ToString();
                    dataGridView1.Columns[noseve].Visible = false;
                    dataGridView2.Columns[noseve].Visible = false;
                    advancedDataGridView1.Columns[noseve].Visible = false;
                }
            }
        }
        private void advancedDataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                dataGridView2.HorizontalScrollingOffset = e.NewValue;
                dataGridView1.HorizontalScrollingOffset = e.NewValue;
            }
        }
        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Tx_modo.Text.Trim() != "" && advancedDataGridView1.Columns[e.ColumnIndex].Name == "GUIA")
            {
                string vser = advancedDataGridView1.Rows[e.RowIndex].Cells["GUIA"].Value.ToString().Substring(0, 4);
                string vnum = advancedDataGridView1.Rows[e.RowIndex].Cells["GUIA"].Value.ToString().Substring(4, 8);
                pub.muestra_gr(vser, vnum, nfCRgr, @"C:\temp\imgQR.png", "DICE CONTENER ", "", "A5", "");   // FALTA VARIABILIZAR 27/10/2023
            }
        }
        #endregion

    }
}
