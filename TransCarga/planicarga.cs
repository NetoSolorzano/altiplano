using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class planicarga : Form
    {
        static string nomform = "planicarga";           // nombre del formulario
        string colback = TransCarga.Program.colbac;     // color de fondo
        string colpage = TransCarga.Program.colpag;     // color de los pageframes
        string colgrid = TransCarga.Program.colgri;     // color de las grillas
        string colfogr = TransCarga.Program.colfog;     // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;     // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;     // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;     // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabplacar";

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
        string codAnul = "";            // codigo de documento anulado
        string codGene = "";            // codigo documento nuevo generado
        string codCier = "";            // codigo planilla cerrada
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        string v_impA4 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        string vtc_flete = "";          // el detalle va con el flete impreso ?? SI || NO
        string v_cid = "";              // codigo interno de tipo de documento
        string v_fra1 = "";             // frase de si va o no con clave
        string v_fra2 = "";             // frase 
        string v_sanu = "";             // serie anulacion interna ANU
        string v_CR_gr_ind = "";        // nombre del formato en CR
        string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string v_trompa = "";           // codigo interno placa de tracto
        string v_carret = "";           // código interno placa de carreta/furgon
        string v_camion = "";           // código interno placa de camion
        string v_mondef = "";           // moneda por defecto del form
        string vint_A0 = "";            // variable INTERNA para amarrar el codigo anulacion cliente con A0
        int v_cdrp = 0;                 // cantidad de días para reabrir una planilla de carga
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        #endregion

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";
        DataTable dtu = new DataTable();
        DataTable dtd = new DataTable();
        DataTable dtm = new DataTable();
        DataTable dtf = new DataTable();    // formatos de impresion CR
        public planicarga()
        {
            InitializeComponent();
        }
        private void planicarga_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)    // F1
        {
            string para1 = "";
            string para2 = "";
            string para3 = "";
            if (keyData == Keys.F1 && tx_pla_ruc.Focused == true)    // Tx_modo.Text == "NUEVO" && 
            {
                para1 = "Proveedores";
                para2 = "";
                para3 = "";
                ayuda3 ayu3 = new ayuda3(para1, para2, para3);
                var result = ayu3.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    tx_pla_ruc.Text = ayu3.ReturnValue1;
                    tx_pla_propiet.Text = ayu3.ReturnValue2;
                    tx_pla_propiet.Tag = ayu3.ReturnValue2;
                }
                return true;    // indicate that you handled this keystroke
            }
            if (keyData == Keys.F1 && tx_pla_placa.Focused == true)   // Tx_modo.Text != "NUEVO" && 
            {
                para1 = "Placas";
                para2 = (rb_propio.Checked == true) ? Program.ruc : tx_pla_ruc.Text;
                para3 = v_trompa + "|" + v_camion;   // codigo tipo camion
                ayuda3 ayu3 = new ayuda3(para1, para2, para3);
                var result = ayu3.ShowDialog();     //ayu1.Show();
                if (result == DialogResult.Cancel)  // deberia ser OK, pero que chuuu
                {
                    tx_pla_placa.Text = ayu3.ReturnValue1;
                    tx_pla_marca.Text = ayu3.ReturnValueA[1];
                    tx_pla_modelo.Text = ayu3.ReturnValueA[2];
                    tx_pla_confv.Text = ayu3.ReturnValueA[3];
                    tx_pla_autor.Text = ayu3.ReturnValueA[4];
                }
                return true;    // indicate that you handled this keystroke
            }
            if (keyData == Keys.F1 && tx_pla_carret.Focused == true)
            {
                para1 = "Placas";
                para2 = (rb_propio.Checked == true) ? Program.ruc : tx_pla_ruc.Text;
                para3 = v_carret;   // codigo tipo carreta
                ayuda3 ayu3 = new ayuda3(para1, para2, para3);
                var result = ayu3.ShowDialog();     //ayu1.Show();
                if (result == DialogResult.Cancel)  // deberia ser OK, pero que chuuu
                {
                    tx_pla_carret.Text = ayu3.ReturnValue1;
                    tx_carret_marca.Text = ayu3.ReturnValueA[1];
                    tx_carret_modelo.Text = ayu3.ReturnValueA[2];
                    tx_carret_conf.Text = ayu3.ReturnValueA[3];
                    tx_carret_autoriz.Text = ayu3.ReturnValueA[4];
                }
                return true;    // indicate that you handled this keystroke
            }
            if (keyData == Keys.F1 && tx_pla_brevet.Focused == true)
            {
                para1 = "Brevete";
                para2 = "";
                para3 = "";
                ayuda3 ayu3 = new ayuda3(para1, para2, para3);
                var result = ayu3.ShowDialog();
                if (result == DialogResult.Cancel)  // deberia ser OK, pero que chuuu
                {
                    tx_pla_brevet.Text = ayu3.ReturnValue1;
                    tx_pla_nomcho.Text = ayu3.ReturnValueA[1];
                }
                return true;
            }
            if (keyData == Keys.F1 && tx_pla_ayud.Focused == true)
            {
                para1 = "Brevete";
                para2 = "";
                para3 = "";
                ayuda3 ayu3 = new ayuda3(para1, para2, para3);
                var result = ayu3.ShowDialog();
                if (result == DialogResult.Cancel)  // deberia ser OK, pero que chuuu
                {
                    tx_pla_ayud.Text = ayu3.ReturnValue1;
                    tx_pla_nomayu.Text = ayu3.ReturnValue2;
                }
                return true;
            }
            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void planicarga_Load(object sender, EventArgs e)
        {
            this.Focus();
            jalainfo();
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            if (valiVars() == false)
            {
                Application.Exit();
                return;
            }
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            splitContainer1.Panel1.BackColor = Color.FromName(colpage);
            splitContainer1.Panel2.BackColor = Color.FromName(colpage);
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //
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
            // longitudes maximas de campos
            tx_car3ro_ruc.MaxLength = 11;
            tx_car_3ro_nombre.MaxLength = 100;
            tx_serie.MaxLength = 4;
            tx_numero.MaxLength = 8;
            tx_pla_placa.MaxLength = 7;
            tx_pla_carret.MaxLength = 7;
            tx_pla_brevet.MaxLength = 10;
            tx_pla_autor.MaxLength = 10;
            tx_pla_confv.MaxLength = 10;
            tx_pla_nomcho.MaxLength = 100;
            tx_pla_propiet.MaxLength = 100;
            tx_pla_ruc.MaxLength = 11;
            tx_obser1.MaxLength = 150;
            // campos en mayusculas
            tx_pla_placa.CharacterCasing = CharacterCasing.Upper;
            tx_pla_carret.CharacterCasing = CharacterCasing.Upper;
            // grilla
            armagrilla();
            // todo desabilidado
            sololee();
            // prueba evitar busquedas en base de datos inecesarias
            tx_pla_propiet.Tag = "x";
        }
        private void armagrilla()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.ColumnCount = 19;
            dataGridView1.Columns[0].Name = "fila";
            dataGridView1.Columns[0].HeaderText = "Fila";
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Name = "numpreg";
            dataGridView1.Columns[1].HeaderText = "Pre-GR";
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[1].Width = 80;
            dataGridView1.Columns[2].Name = "serguia";
            dataGridView1.Columns[2].HeaderText = "Ser.GR";
            dataGridView1.Columns[2].ReadOnly = false;
            dataGridView1.Columns[2].Width = 60;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].Name = "numguia";
            dataGridView1.Columns[3].HeaderText = "Num.GR";
            dataGridView1.Columns[3].ReadOnly = false;
            dataGridView1.Columns[3].Width = 80;
            dataGridView1.Columns[4].Name = "totcant";
            dataGridView1.Columns[4].HeaderText = "Bultos";
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[4].Width = 40;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].Name = "totpeso";
            dataGridView1.Columns[5].HeaderText = "Peso";
            dataGridView1.Columns[5].ReadOnly = true;
            dataGridView1.Columns[5].Width = 70;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].Name = "MON";
            dataGridView1.Columns[6].HeaderText = "Mon";
            dataGridView1.Columns[6].ReadOnly = true;
            dataGridView1.Columns[6].Width = 50;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].Name = "totflet";
            dataGridView1.Columns[7].HeaderText = "Flete";
            dataGridView1.Columns[7].ReadOnly = true;
            dataGridView1.Columns[7].Width = 80;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[8].Visible = false;   // valor pagado de la guia
            dataGridView1.Columns[9].Visible = false;   // valor por cobrar de la guia
            dataGridView1.Columns[10].Visible = false;  // codigo moneda
            dataGridView1.Columns[11].Visible = false;  // marca para edicion
            dataGridView1.Columns[12].Visible = false;  // id de la fila
            dataGridView1.Columns[13].Visible = true;  // nombre destinatario
            dataGridView1.Columns[13].ReadOnly = true;
            dataGridView1.Columns[14].Visible = true;  // direccion destinat
            dataGridView1.Columns[14].ReadOnly = true;
            dataGridView1.Columns[15].Visible = false;  // telef destinatario
            dataGridView1.Columns[15].ReadOnly = true;
            dataGridView1.Columns[16].Name = "nombul";
            dataGridView1.Columns[16].HeaderText = "Nombul";
            dataGridView1.Columns[16].ReadOnly = true;
            dataGridView1.Columns[16].Visible = false;
            dataGridView1.Columns[17].Name = "docvta";
            dataGridView1.Columns[17].HeaderText = "Docvta";
            dataGridView1.Columns[17].Visible = false;
            dataGridView1.Columns[18].Visible = false;
            if (Tx_modo.Text == "EDITAR")
            {
                DataGridViewCheckBoxColumn marca = new DataGridViewCheckBoxColumn();
                marca.Name = "Borra";
                marca.HeaderText = "Borra";
                marca.Width = 50;
                marca.ReadOnly = false;
                marca.FillWeight = 20;
                dataGridView1.Columns.Add(marca);
            }
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            armagrilla();
            tx_fechope.Text = DateTime.Today.ToString("dd/MM/yyyy");
            tx_digit.Text = v_nbu;
            tx_dat_estad.Text = codGene;
            tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
            rb_orden_gr.Checked = true;
            cmb_forimp.Enabled = true;
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofi,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
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
                            if (row["param"].ToString() == "cerrado") codCier = row["valor"].ToString().Trim();        // codigo planilla cerrada
                        }
                    }
                    if (row["formulario"].ToString() == nomform)
                    {
                        if (row["campo"].ToString() == "documento")
                        {
                            if (row["param"].ToString() == "flete") vtc_flete = row["valor"].ToString().Trim();           // imprime precio del flete ?
                            if (row["param"].ToString() == "c_int") v_cid = row["valor"].ToString().Trim();               // codigo interno pre guias
                            if (row["param"].ToString() == "frase1") v_fra1 = row["valor"].ToString().Trim();               // frase de si va con clave la guia
                            if (row["param"].ToString() == "frase2") v_fra2 = row["valor"].ToString().Trim();               // frase otro dato
                            if (row["param"].ToString() == "serieAnu") v_sanu = row["valor"].ToString().Trim();             // serie anulacion interna
                            if (row["param"].ToString() == "reabre") v_cdrp = int.Parse(row["valor"].ToString());           // cantidad de días para re-abrir un manifiesto
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "filasDet") v_mfildet = row["valor"].ToString().Trim();       // maxima cant de filas de detalle
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impMatris") v_impA4 = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "nomGRi_cr") v_CR_gr_ind = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "carguero")
                        {
                            if (row["param"].ToString() == "codTrackto") v_trompa = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "codCarreta") v_carret = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "codCamion") v_camion = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") v_mondef = row["valor"].ToString().Trim();
                    }
                    if (row["formulario"].ToString() == "interno")  // variables configuracion interna, campos especiales de base de datos
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
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
        private void jalaoc(string campo)        // jala planilla de carga
        {
            {
                string parte = "";
                if (campo == "tx_idr")
                {
                    parte = "where a.id=@ida";
                }
                if (campo == "sernum")
                {
                    parte = "where a.serplacar=@ser and a.numplacar=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "select a.id,a.fechope,a.serplacar,a.numplacar,a.locorigen,a.locdestin,a.obsplacar,a.cantfilas,a.cantotpla,a.pestotpla,a.tipmonpla," +
                        "a.tipcampla,a.subtotpla,a.igvplacar,a.totplacar,a.totpagado,a.salxpagar,a.estadoser,a.impreso,a.fleteimp,a.platracto,a.placarret,a.autorizac," +
                        "a.confvehic,a.brevchofe,a.nomchofe,a.brevayuda,a.nomayuda,a.rucpropie,a.tipoplani,a.userc,a.userm,a.usera,ifnull(b.razonsocial,'') as razonsocial," +
                        "a.marcaTrac,a.modeloTrac,a.marcaCarret,a.modelCarret,a.autorCarret,a.confvCarret " +
                        "FROM cabplacar a left join anag_for b on a.rucpropie=b.ruc and b.estado=0 " + parte;
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
                            tx_fechope.Text = dr.GetString("fechope").Substring(0,10);
                            tx_digit.Text = dr.GetString("userc") + " " + dr.GetString("userm") + " " + dr.GetString("usera");
                            //
                            switch (dr.GetString("tipoplani"))
                            {
                                case "1":   // propio
                                    rb_propio.Checked = true;
                                    splitContainer1.SplitterDistance = 560;
                                    break;
                                case "2":   // tercero
                                    rb_3ro.Checked = true;
                                    splitContainer1.SplitterDistance = 560;
                                    break;
                                case "3":   // agencia bus carga
                                    rb_bus.Checked = true;
                                    splitContainer1.SplitterDistance = 30;
                                    break;
                                case "4":   // courier

                                    break;
                            }
                            //
                            tx_dat_estad.Text = dr.GetString("estadoser");
                            tx_serie.Text = dr.GetString("serplacar");
                            tx_numero.Text = dr.GetString("numplacar");
                            tx_pla_fech.Text = dr.GetString("fechope").Substring(0, 10);
                            tx_dat_locori.Text = dr.GetString("locorigen");
                            tx_dat_locdes.Text = dr.GetString("locdestin");
                            tx_obser1.Text = dr.GetString("obsplacar");
                            tx_tfil.Text = dr.GetString("cantfilas");
                            tx_totcant.Text = dr.GetString("cantotpla");
                            tx_totpes.Text = dr.GetString("pestotpla");
                            tx_dat_mone.Text = dr.GetString("tipmonpla");
                            tx_flete.Text = dr.GetString("totplacar");
                            tx_pagado.Text = dr.GetString("totpagado");
                            tx_salxcob.Text = dr.GetString("salxpagar");
                            tx_dat_detflete.Text = dr.GetString("fleteimp");    // determina si en el detalle se muestra e imprime el valor del flete de la guia
                            //
                            tx_pla_placa.Text = dr.GetString("platracto");
                            tx_pla_carret.Text = dr.GetString("placarret");
                            tx_pla_brevet.Text = dr.GetString("brevchofe");
                            tx_pla_nomcho.Text = dr.GetString("nomchofe");
                            tx_pla_ayud.Text = dr.GetString("brevayuda");
                            tx_pla_nomayu.Text = dr.GetString("nomayuda");
                            tx_pla_autor.Text = dr.GetString("autorizac");
                            tx_pla_confv.Text = dr.GetString("confvehic");
                            tx_pla_propiet.Text = dr.GetString("razonsocial");
                            tx_pla_ruc.Text = dr.GetString("rucpropie");
                            if (tx_pla_ruc.Text == rucclie) tx_pla_propiet.Text = nomclie;
                            tx_pla_marca.Text = dr.GetString("marcaTrac");
                            tx_pla_modelo.Text = dr.GetString("modeloTrac");
                            tx_carret_marca.Text = dr.GetString("marcaCarret");
                            tx_carret_modelo.Text = dr.GetString("modelCarret");
                            tx_carret_conf.Text = dr.GetString("confvCarret");
                            tx_carret_autoriz.Text = dr.GetString("autorCarret");
                            //
                            tx_car3ro_ruc.Text = dr.GetString("rucpropie");
                            tx_car_3ro_nombre.Text = dr.GetString("razonsocial");  // falta en consulta
                        }
                        tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                        cmb_destino.SelectedValue = tx_dat_locdes.Text;
                        cmb_origen.SelectedValue = tx_dat_locori.Text;
                        cmb_mon.SelectedValue = tx_dat_mone.Text;
                        // si el documento esta ANULADO o un estado que no permite EDICION, se pone todo en sololee (ANULADO O RECIBIDO)
                        if (tx_dat_estad.Text != codGene)
                        {
                            sololee();
                            splitContainer1.Panel1.Enabled = false;
                            cmb_forimp.Enabled = true;
                            dataGridView1.ReadOnly = true;
                            MessageBox.Show("Este documento no puede ser editado/anulado", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            tx_serie.ReadOnly = true;   // despues de una jalada exitosa
                            tx_numero.ReadOnly = true;  // estos numeros no deben modificarse
                            if (tx_dat_estad.Text == codGene) chk_cierea.Text = "CIERRA PLANILLA";          // SOLAMENTE se cierran o re-abren
                                                                                                            //
                            button1.Enabled = true;
                            // validamos usuario y local para modos EDICION y ANULACION
                            if (("EDITAR,ANULAR").Contains(Tx_modo.Text))
                            {
                                if (tx_dat_locori.Text == v_clu)
                                {
                                    escribe();
                                    dataGridView1.ReadOnly = false;
                                    button1.Enabled = true;
                                }
                                else
                                {
                                    MessageBox.Show("La planilla no puede Editada o Anulada" + Environment.NewLine +
                                        "revise el estado del documento y/o el local", "No puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    sololee();
                                    cmb_forimp.Enabled = true;
                                    button1.Enabled = false;
                                    dataGridView1.ReadOnly = true;
                                }
                            }
                        }
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
        }
        private void jaladet(string idr)         // jala el detalle
        {
            string jalad = "select a.idc,a.serplacar,a.numplacar,a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,floor(a.totpeso) as totpeso,b.descrizionerid as MON,a.totflet," +
                "a.estadoser,a.codmone,'X' as marca,a.id,a.pagado,a.salxcob,g.nombdegri,g.diredegri,g.teledegri,a.nombult,u1.nombre AS distrit,u2.nombre as provin," +
                "concat(d.descrizionerid,'-',if(SUBSTRING(g.serdocvta,1,2)='00',SUBSTRING(g.serdocvta,3,2),g.serdocvta),'-',if(SUBSTRING(g.numdocvta,1,3)='000',SUBSTRING(g.numdocvta,4,5),g.numdocvta))," +
                "g.nombregri " +
                "from detplacar a " +
                "left join desc_mon b on b.idcodice = a.codmone " +
                "left join cabguiai g on g.sergui = a.serguia and g.numgui = a.numguia " +
                "left join desc_tdv d on d.idcodice=g.tipdocvta " + 
                "LEFT JOIN ubigeos u1 ON CONCAT(u1.depart, u1.provin, u1.distri)= g.ubigdegri " +
                "LEFT JOIN(SELECT* FROM ubigeos WHERE depart<>'00' AND provin<>'00' AND distri = '00') u2 ON u2.depart = left(g.ubigdegri, 2) AND u2.provin = concat(substr(g.ubigdegri, 3, 2)) " +
                "where a.idc=@idr";
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
                        dataGridView1.Rows.Clear();
                        foreach (DataRow row in dt.Rows)
                        {
                            if (Tx_modo.Text != "EDITAR")
                            {
                                dataGridView1.Rows.Add(
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString(),
                                    row[7].ToString(),
                                    row[8].ToString(),
                                    row[9].ToString(),
                                    row[10].ToString(),
                                    row[15].ToString(),
                                    row[16].ToString(),
                                    row[12].ToString(),
                                    row[13].ToString(),
                                    row[14].ToString(),
                                    row[17].ToString(),
                                    row[18].ToString() + " - " + row[21].ToString() + " - " + row[22].ToString(),
                                    row[19].ToString(),
                                    row[20].ToString(),
                                    row[23].ToString(),
                                    row[24].ToString()
                                    );
                            }
                            else
                            {
                                dataGridView1.Rows.Add(
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString(),
                                    row[7].ToString(),
                                    row[8].ToString(),
                                    row[9].ToString(),
                                    row[10].ToString(),
                                    row[15].ToString(),
                                    row[16].ToString(),
                                    row[12].ToString(),
                                    row[13].ToString(),
                                    row[14].ToString(),
                                    row[17].ToString(),
                                    row[18].ToString() + " - " + row[21].ToString() + " - " + row[22].ToString(),
                                    row[19].ToString(),
                                    row[20].ToString(),
                                    row[23].ToString(),
                                    row[24].ToString(),
                                    false
                                    );
                            }
                        }
                        dt.Dispose();
                    }
                }
            }
            operaciones();
        }
        private void dataload()                  // jala datos para los combos 
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
            MySqlCommand ccl = new MySqlCommand("select idcodice,descrizionerid,ubidir,marca1 from desc_loc where numero=@bloq", conn);
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
            // datos de formatos de impresion
            cmb_forimp.Items.Clear();
            MySqlCommand cmf = new MySqlCommand("select valor from enlaces where formulario='planicarga' and campo='impresion' and param like '%_cr%'", conn);
            dacu = new MySqlDataAdapter(cmf);
            dtf.Clear();
            dacu.Fill(dtf);
            cmb_forimp.DataSource = dtf;
            cmb_forimp.DisplayMember = "valor";
            cmb_forimp.ValueMember = "valor";
            conn.Close();
        }
        private bool valiGri()                  // valida filas completas en la grilla - 8 columnas
        {
            bool retorna = true;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null &&
                    dataGridView1.Rows[i].Cells[1].Value == null &&
                    dataGridView1.Rows[i].Cells[2].Value == null &&
                    dataGridView1.Rows[i].Cells[3].Value == null &&
                    dataGridView1.Rows[i].Cells[4].Value == null &&
                    dataGridView1.Rows[i].Cells[5].Value == null &&
                    dataGridView1.Rows[i].Cells[6].Value == null &&
                    dataGridView1.Rows[i].Cells[7].Value == null &&
                    dataGridView1.Rows[i].Cells[8].Value == null)
                {
                    // no hay problema
                    retorna = true;
                }
                else
                {
                    if (dataGridView1.Rows[i].Cells[0].Value == null ||
                        dataGridView1.Rows[i].Cells[1].Value == null ||
                        dataGridView1.Rows[i].Cells[2].Value == null ||
                        dataGridView1.Rows[i].Cells[3].Value == null ||
                        dataGridView1.Rows[i].Cells[4].Value == null ||
                        dataGridView1.Rows[i].Cells[5].Value == null ||
                        dataGridView1.Rows[i].Cells[6].Value == null ||
                        dataGridView1.Rows[i].Cells[7].Value == null ||
                        dataGridView1.Rows[i].Cells[8].Value == null)
                    {
                        retorna = false;
                        break;
                    }
                    else
                    {
                        retorna = true;
                    }
                }
            }
            return retorna;
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de GR indivual ANULADA");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de GR indivual GENERADA/NUEVA");
                retorna = false;
            }
            if (v_clu == "")            // codigo del local del usuario
            {
                lib.messagebox("Código local del usuario");
                retorna = false;
            }
            if (v_slu == "")            // serie del local del usuario
            {
                lib.messagebox("Serie general local del usuario");
                retorna = false;
            }
            if (v_nbu == "")            // nombre del usuario
            {
                lib.messagebox("Nombre del usuario");
                retorna = false;
            }
            if (vi_formato == "")       // formato de impresion del documento
            {
                lib.messagebox("formato de impresion de la GR interna");
                retorna = false;
            }
            if (vi_copias == "")        // cant copias impresion
            {
                lib.messagebox("# copias impresas de la GR interna");
                retorna = false;
            }
            if (v_impA4 == "")          // nombre de la impresora matricial
            {
                lib.messagebox("Nombre de impresora matricial");
                retorna = false;
            }
            if (vtc_flete == "")         // el detalle va con flete impreso ?? SI || NO
            {
                lib.messagebox("GR interna imprime valor del flete");
                retorna = false;
            }
            if (v_cid == "")             // codigo interno de tipo de documento
            {
                lib.messagebox("Código interno tipo de documento");
                retorna = false;
            }
            if (v_fra1 == "")            // frase de si va o no con clave
            {
                lib.messagebox("Frase impresa en GR sobre clave");
                retorna = false;
            }
            if (v_sanu == "")           // serie de anulacion del documento
            {
                lib.messagebox("Serie de Anulación interna");
                retorna = false;
            }
            if (v_CR_gr_ind == "")
            {
                lib.messagebox("Nombre formato GR en CR");
                retorna = false;
            }
            if (v_mfildet == "")
            {
                lib.messagebox("Max. filas de detalle");
                retorna = false;
            }
            if (vint_A0 == "")
            {
                lib.messagebox("Cód. Interno enlace Anulado: A0");
                retorna = false;
            }
            return retorna;
        }
        private string[] ValPlaCarr(string pc,string codigo)    // pc=P ó C, codigo=placa de trompa o carreta
        {
            string[] retorna = { "", "", "", "", "" };      // cofig.vehicular, autorizacion, placa asociada, marca, modelo
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string parte0="", parte1="";
                if (pc == "P")
                { 
                    parte0 = "placa = @codigo";
                    parte1 = "tipo = @tipo";    // variable codigo placa
                }
                if (pc == "C")
                {
                    parte0 = "placa = @codigo";
                    parte1 = "tipo = @tipo";    // variable codigo carreta
                }

                string consulta = "select confve,autor1,placAsoc,marca,modelo from vehiculos where status<>@estdes and placa=@codigo"; // and " + parte0 + " and " + parte1;
                using (MySqlCommand micon = new MySqlCommand(consulta,conn))
                {
                    micon.Parameters.AddWithValue("@estdes", codAnul);
                    micon.Parameters.AddWithValue("@codigo", codigo);
                    //micon.Parameters.AddWithValue("@tipo", (pc == "P")? v_trompa : v_carret);
                    MySqlDataReader dr = micon.ExecuteReader();
                    while (dr.Read())
                    {
                        retorna[0] = dr.GetString(0);
                        retorna[1] = dr.GetString(1);
                        retorna[2] = dr.GetString(2);   // (pc == "P") ? dr.GetString(2) : dr.GetString(3);   // carreta retorna marca, placa retorna placa asoc
                        retorna[3] = dr.GetString(3);
                        retorna[4] = dr.GetString(4);
                    }
                    dr.Dispose();
                }
            }
            return retorna;
        }
        private void operaciones()              // recalcula los totales de la grilla
        {
            int totfil = 0;
            int totcant = 0;
            decimal totpes = 0;
            decimal totfle = 0, totpag = 0, totsal = 0;
            //a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,a.totpeso,b.descrizionerid as MON,a.totflet,a.totpag,a.salgri
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Tx_modo.Text == "EDITAR" && tx_dat_estad.Text == codGene)
                {
                    if (dataGridView1.Rows.Count > 0 && dataGridView1.Rows[i].Cells[13].Value != null)
                    {
                        if (dataGridView1.Rows[i].Cells[19].Value.ToString() == "False")
                        {
                            if (dataGridView1.Rows[i].Cells[4].Value != null)
                            {
                                totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                                totfil += 1;
                            }
                            if (dataGridView1.Rows[i].Cells[5].Value != null)
                            {
                                totpes = totpes + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString());
                            }
                            if (dataGridView1.Rows[i].Cells[7].Value != null)
                            {
                                totfle = totfle + decimal.Parse(dataGridView1.Rows[i].Cells[7].Value.ToString());
                                totpag = totpag + decimal.Parse(dataGridView1.Rows[i].Cells[8].Value.ToString());
                                totsal = totsal + decimal.Parse(dataGridView1.Rows[i].Cells[9].Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show(dataGridView1.Rows[i].Cells[13].Value.ToString(),"fila: " + i.ToString());
                        if (dataGridView1.Rows[i].Cells[4].Value != null)
                        {
                            totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                            totfil += 1;
                        }
                        if (dataGridView1.Rows[i].Cells[5].Value != null)
                        {
                            totpes = totpes + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString());
                        }
                        if (dataGridView1.Rows[i].Cells[7].Value != null)
                        {
                            totfle = totfle + decimal.Parse(dataGridView1.Rows[i].Cells[7].Value.ToString());
                            totpag = totpag + decimal.Parse(dataGridView1.Rows[i].Cells[8].Value.ToString());
                            totsal = totsal + decimal.Parse(dataGridView1.Rows[i].Cells[9].Value.ToString());
                        }
                    }
                }
                else
                {
                    if (dataGridView1.Rows[i].Cells[4].Value != null)
                    {
                        totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                        totfil += 1;
                    }
                    if (dataGridView1.Rows[i].Cells[5].Value != null)
                    {
                        totpes = totpes + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString());
                    }
                    if (dataGridView1.Rows[i].Cells[7].Value != null)
                    {
                        totfle = totfle + decimal.Parse(dataGridView1.Rows[i].Cells[7].Value.ToString());
                        totpag = totpag + decimal.Parse(dataGridView1.Rows[i].Cells[8].Value.ToString());
                        totsal = totsal + decimal.Parse(dataGridView1.Rows[i].Cells[9].Value.ToString());
                    }
                }
            }
            tx_totcant.Text = totcant.ToString();
            tx_totpes.Text = totpes.ToString("0.00");
            tx_tfil.Text = totfil.ToString();
            tx_flete.Text = totfle.ToString("0.00");
            tx_pagado.Text = totpag.ToString("0.00");
            tx_salxcob.Text = totsal.ToString("0.00");
            if (int.Parse(tx_tfil.Text) == int.Parse(v_mfildet) && int.Parse(v_mfildet)>0)
            {
                MessageBox.Show("Número máximo de filas en planilla", "El formato no permite mas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataGridView1.AllowUserToAddRows = false;
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;
            }
        }

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
        }
        private void escribe()
        {
            lp.escribe(this);
            tx_pla_marca.ReadOnly = true;
            tx_pla_modelo.ReadOnly = true;
            tx_pla_confv.ReadOnly = true;
            tx_pla_autor.ReadOnly = true;
            tx_pla_propiet.ReadOnly = true;
            tx_car_3ro_nombre.ReadOnly = true;
            tx_carret_marca.ReadOnly = true;
            tx_carret_modelo.ReadOnly = true;
            tx_carret_conf.ReadOnly = true;
            tx_carret_autoriz.ReadOnly = true;
        }
        private void limpiar()
        {
            lp.limpiar(this);
        }
        private void limpia_chk()
        {
            lp.limpia_chk(this);
        }
        private void limpia_otros()
        {
            lp.limpiagbox(gbox_flete);
            lp.limpiagbox(gbox_serie);
            lp.limpiasplit(splitContainer1);
        }
        private void limpia_combos()
        {
            lp.limpia_cmb(this);
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void button1_Click(object sender, EventArgs e)
        {
            #region validaciones
            if (tx_serie.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la serie de la planilla", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tx_serie.Focus();
                return;
            }
            if (tx_pla_fech.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la fecha de la planilla", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tx_pla_fech.Focus();
                return;
            }
            if (tx_dat_locori.Text == "")
            {
                MessageBox.Show("Seleccione el local de origen", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmb_origen.Focus();
                return;
            }
            if (tx_dat_locdes.Text == "")
            {
                MessageBox.Show("Seleccione el local de destino", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmb_destino.Focus();
                return;
            }
            if (tx_pla_ruc.Text.Trim() == "" && tx_car3ro_ruc.Text == "")
            {
                MessageBox.Show("Ingrese el ruc del transportista", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if(rb_propio.Checked == true) tx_pla_ruc.Focus();
                if(rb_3ro.Checked == true) tx_pla_ruc.Focus();
                if (rb_bus.Checked == true) tx_car3ro_ruc.Focus();
                return;
            }
            if (rb_propio.Checked == true)
            {
                // validacion se hace desde funcion en B.D.: ayudante 
                if (tx_pla_autor.Text == "")        // autorizacion circulacion
                {
                    MessageBox.Show("Falta la autorización de circulación", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_autor.Focus();
                    return;
                }
                if (tx_pla_brevet.Text == "")       // brevete chofer
                {
                    MessageBox.Show("Falta el brevete del chofer", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_brevet.Focus();
                    return;
                }
                if (tx_pla_confv.Text == "")        // conf. vehicular
                {
                    MessageBox.Show("Falta la configuración vehicular", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_confv.Focus();
                    return;
                }
                if (tx_pla_nomcho.Text == "")       // nombre chofer
                {
                    MessageBox.Show("Falta el nombre del chofer", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_nomcho.Focus();
                    return;
                }
                if (tx_pla_placa.Text == "")        // placa trompa
                {
                    MessageBox.Show("Ingrese la placa del camión", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_placa.Focus();
                    return;
                }
            }
            #endregion
            // recalculamos totales 
            operaciones();
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            //MessageBox.Show(tx_pla_confv.Text + "-" + tx_carret_conf.Text);
            if (modo == "NUEVO")
            {
                // valida que las filas de la grilla esten completas
                if (valiGri() != true)
                {
                    MessageBox.Show("Complete las filas del detalle", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //dataGridView1.Focus();
                    return;
                }
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear la planilla?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            var bb = MessageBox.Show("Desea imprimir la planilla?" + Environment.NewLine +
                                "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (bb == DialogResult.Yes)
                            {
                                Bt_print.PerformClick();
                            }
                        }
                        else
                        {
                            iserror = "si";
                        }
                    }
                    else
                    {
                        //tx_numDocRem.Focus();
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
                    MessageBox.Show("Ingrese el número de la planilla", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_numero.Focus();
                    return;
                }
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("La planilla esta anulada", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (tx_dat_estad.Text != codGene)
                {
                    //MessageBox.Show("La planilla tiene estado que impide su edición", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //return;
                }
                if (true)   // de momento no validamos mas
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar la planilla?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            if (tx_dat_estad.Text == codCier && chk_cierea.Checked == true) // reabre planilla
                            {
                                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                                conn.Open();
                                if (conn.State == ConnectionState.Open)
                                {
                                    string actua = "update cabplacar set estadoser=@estad,verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam " +
                                        "where serplacar=@serpl and numplacar=@numpl";
                                    using (MySqlCommand micon = new MySqlCommand(actua, conn))
                                    {
                                        micon.Parameters.AddWithValue("@serpl", tx_serie.Text);
                                        micon.Parameters.AddWithValue("@numpl", tx_numero.Text);
                                        micon.Parameters.AddWithValue("@estad", codGene);
                                        micon.Parameters.AddWithValue("@verApp", verapp);
                                        micon.Parameters.AddWithValue("@asd", asd);
                                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                        micon.ExecuteNonQuery();
                                    }
                                }
                                conn.Close();
                            }
                            else
                            {
                                if (tx_dat_estad.Text == codGene)
                                {
                                    if (edita() == true)
                                    {
                                        iserror = "no";
                                    }
                                    else
                                    {
                                        iserror = "si";
                                    }
                                }
                            }
                        }
                        else
                        {
                            //tx_dat_tdRem.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("La Planilla ya debe existir para editar", "Debe ser edición", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
            }
            if (modo == "ANULAR")
            {
                // EN ESTE FORM, LA ANULACION ES FISICA PORQUE SU NUMERACION ES AUTOMATICA
                // si se anula, se tiene que desenlazar en todas sus guías y en control

                if (tx_dat_estad.Text != codAnul)   // (tx_pla_plani.Text.Trim() == "") && tx_impreso.Text == "N"
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea ANULAR la planilla?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            if (anula() == true)
                            {
                                // todo bien
                            }
                            else
                            {
                                iserror = "si";
                            }
                        }
                        else
                        {
                            //tx_dat_tdRem.Focus();
                            return;
                        }
                    }
                }
            }
            if (iserror == "no")
            {
                string resulta = lib.ult_mov(nomform, nomtab, asd);
                if (resulta != "OK")                                        // actualizamos la tabla usuarios
                {
                    MessageBox.Show(resulta, "Error en actualización de tabla usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                initIngreso();          // limpiamos todo para volver a empesar
                armagrilla();
                cmb_origen.Focus();
                return;
            }
        }
        private bool graba()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if(conn.State == ConnectionState.Open)
            {
                int vtip = 0;
                if (rb_propio.Checked == true) vtip = 1;
                if (rb_3ro.Checked == true) vtip = 2;
                if (rb_bus.Checked == true) vtip = 3;
                string inserta = "insert into cabplacar (" +
                    "fechope,serplacar,locorigen,locdestin,obsplacar,cantfilas,cantotpla,pestotpla,tipmonpla,tipcampla,subtotpla," +
                    "igvplacar,totplacar,totpagado,salxpagar,estadoser,fleteimp,platracto,placarret,autorizac,confvehic,brevchofe," +
                    "brevayuda,rucpropie,tipoplani,nomchofe,nomayuda,marcaTrac,modeloTrac,marcaCarret,modelCarret,autorCarret,confvCarret," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fecho,@serpl,@locor,@locde,@obspl,@cantf,@canto,@pesto,@tipmo,@tipca,@subto," +
                    "@igvpl,@totpl,@totpa,@salxp,@estad,@fleim,@platr,@placa,@autor,@confv,@brevc," +
                    "@breva,@rucpr,@tipop,@nocho,@noayu,@marca,@model,@marCarr,@modCarr,@autCarr,@conCarr," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)"; // 
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fecho", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@serpl", tx_serie.Text);
                    micon.Parameters.AddWithValue("@locor", tx_dat_locori.Text);
                    micon.Parameters.AddWithValue("@locde", tx_dat_locdes.Text);
                    micon.Parameters.AddWithValue("@obspl", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@cantf", tx_tfil.Text);      // cantidad filas detalle
                    micon.Parameters.AddWithValue("@canto", tx_totcant.Text);   // cant total de bultos
                    micon.Parameters.AddWithValue("@pesto", tx_totpes.Text);    // peso total
                    micon.Parameters.AddWithValue("@tipmo", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@tipca", "0.00");
                    micon.Parameters.AddWithValue("@subto", "0.00");
                    micon.Parameters.AddWithValue("@igvpl", "0.00");
                    micon.Parameters.AddWithValue("@totpl", tx_flete.Text);
                    micon.Parameters.AddWithValue("@totpa", tx_pagado.Text);
                    micon.Parameters.AddWithValue("@salxp", tx_salxcob.Text);   // saldo por cobrar al momento de grabar la planilla
                    micon.Parameters.AddWithValue("@estad", tx_dat_estad.Text);
                    micon.Parameters.AddWithValue("@fleim", tx_dat_detflete.Text);      // variable si detalle lleva valores flete guias
                    micon.Parameters.AddWithValue("@platr", tx_pla_placa.Text);
                    micon.Parameters.AddWithValue("@placa", tx_pla_carret.Text);
                    micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                    micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text + "-" + tx_carret_conf.Text);
                    micon.Parameters.AddWithValue("@marCarr", tx_carret_marca.Text);
                    micon.Parameters.AddWithValue("@modCarr", tx_carret_modelo.Text);
                    micon.Parameters.AddWithValue("@autCarr", tx_carret_autoriz.Text);
                    micon.Parameters.AddWithValue("@conCarr", tx_carret_conf.Text);
                    micon.Parameters.AddWithValue("@brevc", tx_pla_brevet.Text);
                    micon.Parameters.AddWithValue("@nocho", tx_pla_nomcho.Text);           // nombre del chofer
                    micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                    micon.Parameters.AddWithValue("@noayu", tx_pla_nomayu.Text);           // nombre del ayudante
                    micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "")? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                    micon.Parameters.AddWithValue("@tipop", vtip);              // tipo planilla, tipo transporte/transportista
                    micon.Parameters.AddWithValue("@marca", tx_pla_marca.Text);
                    micon.Parameters.AddWithValue("@model", tx_pla_modelo.Text);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    try
                    {
                        micon.ExecuteNonQuery();
                    }
                    catch(MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Validación Interna", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return retorna;
                    }
                }
                using (MySqlCommand micon = new MySqlCommand("select last_insert_id()", conn))
                {
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // numplacar numeracion automatica estilo pre guias
                            tx_numero.Text = lib.Right("0000000" + dr.GetString(0), 8);
                            tx_idr.Text = dr.GetString(0);
                            retorna = true;
                        }
                    }
                }
                // detalle
                if (dataGridView1.Rows.Count > 0)
                {
                    int fila = 1;
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value != null)
                        {
                            if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                            {
                                string inserd2 = "insert into detplacar (idc,serplacar,numplacar,fila,numpreg,serguia,numguia,totcant,totpeso,totflet,codmone,estadoser,origreg," +
                                    "verApp,userc,fechc,diriplan4,diripwan4,netbname,platracto,placarret,autorizac,confvehic,brevchofe,brevayuda,rucpropiet,fechope,pagado,salxcob) " +
                                    "values (@idr,@serpl,@numpl,@fila,@numpr,@sergu,@numgu,@totca,@totpe,@totfl,@codmo,@estad,@orireg," +
                                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam,@platr,@placa,@autor,@confv,@brevc,@breva,@rucpr,@fecho,@paga,@xcob)";
                                using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                                {
                                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                    micon.Parameters.AddWithValue("@fecho", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                                    micon.Parameters.AddWithValue("@serpl", tx_serie.Text);
                                    micon.Parameters.AddWithValue("@numpl", tx_numero.Text);
                                    micon.Parameters.AddWithValue("@fila", fila);
                                    micon.Parameters.AddWithValue("@numpr", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                    micon.Parameters.AddWithValue("@sergu", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                    micon.Parameters.AddWithValue("@numgu", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                    micon.Parameters.AddWithValue("@totca", dataGridView1.Rows[i].Cells[4].Value.ToString());
                                    micon.Parameters.AddWithValue("@totpe", dataGridView1.Rows[i].Cells[5].Value.ToString());
                                    micon.Parameters.AddWithValue("@totfl", dataGridView1.Rows[i].Cells[7].Value.ToString());
                                    micon.Parameters.AddWithValue("@codmo", dataGridView1.Rows[i].Cells[10].Value.ToString());
                                    micon.Parameters.AddWithValue("@estad", tx_dat_estad.Text);
                                    micon.Parameters.AddWithValue("@orireg", "M");              // origen del registro manual, cuando viene desde el form de guias es A
                                    micon.Parameters.AddWithValue("@verApp", verapp);
                                    micon.Parameters.AddWithValue("@asd", asd);
                                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                    micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                    micon.Parameters.AddWithValue("@platr", tx_pla_placa.Text);
                                    micon.Parameters.AddWithValue("@placa", tx_pla_carret.Text);
                                    micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                                    micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text  + "-" + tx_carret_conf.Text);
                                    micon.Parameters.AddWithValue("@brevc", tx_pla_brevet.Text);
                                    micon.Parameters.AddWithValue("", tx_pla_nomcho.Text);           // nombre del chofer
                                    micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                                    micon.Parameters.AddWithValue("", tx_pla_nomayu.Text);           // nombre del ayudante
                                    micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "") ? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                                    micon.Parameters.AddWithValue("@paga", dataGridView1.Rows[i].Cells[8].Value.ToString());    // 
                                    micon.Parameters.AddWithValue("@xcob", dataGridView1.Rows[i].Cells[9].Value.ToString());    // 
                                    //a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,a.totpeso,b.descrizionerid as MON,a.totflet,a.totpag,a.salgri,a.codmon
                                    micon.ExecuteNonQuery();
                                    fila += 1;
                                    retorna = true;         // no hubo errores!
                                }
                            }
                        }
                    }
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
        private bool edita()
        {
            bool retorna = false;
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                //try
                {
                    if (tx_dat_estad.Text == codGene || tx_dat_estad.Text == codCier)   // solo edita estado GENERADO/CERRADO, otro estado no se edita
                    {                                               // El estado cambia solo cuando: SE CIERRA MANUALMENTE ó CUANDO SE RECEPCIONA LA PLANILLA
                        int vtip = 0;                               // los datos que NO SE EDITAN son: serie,numero,origen y destino
                        if (rb_propio.Checked == true) vtip = 1;    // los totales filas ,peso y bultos si cambian con la edicion
                        if (rb_3ro.Checked == true) vtip = 2;       // los fletes y saldos de cada guía NO CAMBIAN al editar, salvo si se borra y vuelte a registrar la GR
                        if (rb_bus.Checked == true) vtip = 3;       // locorigen=@locor,locdestin=@locde,
                        string actua = "update cabplacar set " +
                            "fechope=@fecho,obsplacar=@obspl,cantfilas=@cantf,cantotpla=@canto,pestotpla=@pesto,tipmonpla=@tipmo," +
                            "tipcampla=@tipca,subtotpla=@subto,igvplacar=@igvpl,totplacar=@totpl,totpagado=@totpa,salxpagar=@salxp,fleteimp=@fleim," +
                            "platracto=@platr,placarret=@placa,autorizac=@autor,confvehic=@confv,brevchofe=@brevc,brevayuda=@breva,rucpropie=@rucpr,tipoplani=@tipop," +
                            "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam,nomchofe=@nocho,nomayuda=@noayu,estadoser=@estad," +
                            "marcaCarret=@marCarr,modelCarret=@modCarr,autorCarret=@autCarr,confvCarret=@conCarr," +
                            "marcaTrac=@marca,modeloTrac=@model " +
                            "where serplacar=@serpl and numplacar=@numpl";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@fecho", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@serpl", tx_serie.Text);
                        micon.Parameters.AddWithValue("@numpl", tx_numero.Text);
                        //micon.Parameters.AddWithValue("@locor", tx_dat_locori.Text);
                        //micon.Parameters.AddWithValue("@locde", tx_dat_locdes.Text);
                        micon.Parameters.AddWithValue("@obspl", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@cantf", tx_tfil.Text);      // cantidad filas detalle
                        micon.Parameters.AddWithValue("@canto", tx_totcant.Text);   // cant total de bultos
                        micon.Parameters.AddWithValue("@pesto", tx_totpes.Text);    // peso total
                        micon.Parameters.AddWithValue("@tipmo", tx_dat_mone.Text);
                        micon.Parameters.AddWithValue("@tipca", "0.00");
                        micon.Parameters.AddWithValue("@subto", "0.00");
                        micon.Parameters.AddWithValue("@igvpl", "0.00");
                        micon.Parameters.AddWithValue("@totpl", tx_flete.Text);
                        micon.Parameters.AddWithValue("@totpa", tx_pagado.Text);
                        micon.Parameters.AddWithValue("@salxp", tx_salxcob.Text);   // saldo por cobrar al momento de grabar la planilla
                        micon.Parameters.AddWithValue("@fleim", tx_dat_detflete.Text);      // variable si detalle lleva valores flete guias
                        micon.Parameters.AddWithValue("@platr", tx_pla_placa.Text);
                        micon.Parameters.AddWithValue("@placa", tx_pla_carret.Text);
                        micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                        micon.Parameters.AddWithValue("@confv", (tx_pla_confv.Text.Trim().Length > 3) ? tx_pla_confv.Text : tx_pla_confv.Text + tx_carret_conf.Text);
                        micon.Parameters.AddWithValue("@marCarr", tx_carret_marca.Text);
                        micon.Parameters.AddWithValue("@modCarr", tx_carret_modelo.Text);
                        micon.Parameters.AddWithValue("@autCarr", tx_carret_autoriz.Text);
                        micon.Parameters.AddWithValue("@conCarr", tx_carret_conf.Text);
                        micon.Parameters.AddWithValue("@brevc", tx_pla_brevet.Text);
                        micon.Parameters.AddWithValue("@nocho", tx_pla_nomcho.Text);           // nombre del chofer
                        micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                        micon.Parameters.AddWithValue("@noayu", tx_pla_nomayu.Text);           // nombre del ayudante
                        micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "") ? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                        micon.Parameters.AddWithValue("@tipop", vtip);              // tipo planilla, tipo transporte/transportista
                        if (tx_dat_estad.Text == codGene && chk_cierea.Checked == true)     // Planilla abierta y checkeado ==> CIERRA LA PLANILLA
                        {
                            micon.Parameters.AddWithValue("@estad", codCier);
                        }
                        else
                        {
                            if (tx_dat_estad.Text == codCier && chk_cierea.Checked == true)     // planilla cerrada y con check ==> REABRE PLANILLA
                            {
                                micon.Parameters.AddWithValue("@estad", codGene);
                            }
                            else
                            {
                                micon.Parameters.AddWithValue("@estad", codGene);
                            }
                        }
                        micon.Parameters.AddWithValue("@marca", tx_pla_marca.Text);
                        micon.Parameters.AddWithValue("@model", tx_pla_modelo.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE 
                        /*
                            Las filas marcadas SE BORRAN
                            Las filas NUEVAS SE INSERTAN
                            Las filas cambiasas NO HACE CASO O NO PERMITE EL CAMBIO, solo se permite borrar o agregar filas
                        */
                        int fila = 0;
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[19].Value != null)   // fila marcada para borrar
                            {
                                // saca la guia de detplacar
                                if (dataGridView1.Rows[i].Cells[19].Value.ToString() == "True")
                                {
                                    string consulta = "borraseguro";
                                    using (MySqlCommand comed = new MySqlCommand(consulta, conn))
                                    {
                                        comed.CommandType = CommandType.StoredProcedure;
                                        comed.Parameters.AddWithValue("@tabla", "detplacar");
                                        comed.Parameters.AddWithValue("@vidr", int.Parse(dataGridView1.Rows[i].Cells[12].Value.ToString()));
                                        comed.Parameters.AddWithValue("@vidc", 0);
                                        try
                                        {
                                            comed.ExecuteNonQuery();
                                            // trigger borra los campos en cabguiai  ... el triger esta comentado no se porque 23/06/2021
                                            // trigger borra los campos en controlg  ... el triger esta comentado no se porque 23/06/2021
                                        }
                                        catch (MySqlException ex)
                                        {
                                            MessageBox.Show("Ocurrió un error en el proceso de borrar la guía de la planilla" + Environment.NewLine +
                                                "la GR aun esta en la planilla! " + Environment.NewLine +
                                                ex.Message, "Alerta proceso no concluido!");
                                        }
                                    }
                                    //
                                    try
                                    {
                                        string actor1 = "UPDATE cabguiai SET serplagri='',numplagri='',plaplagri='',carplagri='',autplagri='',confvegri='',breplagri='',proplagri='',idplani=0,fechplani=null " +
                                            "WHERE sergui=@OLDS AND numgui=@OLDN";
                                        string actor2 = "UPDATE controlg SET fecplacar=null,serplacar='',numplacar='',placamcar='',chocamcar='' " +
                                            "WHERE serguitra=@OLDS AND numguitra=@OLDN";
                                        using (MySqlCommand comm = new MySqlCommand(actor1, conn))
                                        {
                                            comm.Parameters.AddWithValue("@OLDS", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                            comm.Parameters.AddWithValue("@OLDN", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                            comm.ExecuteNonQuery();
                                        }
                                        using (MySqlCommand comm = new MySqlCommand(actor2, conn))
                                        {
                                            comm.Parameters.AddWithValue("@OLDS", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                            comm.Parameters.AddWithValue("@OLDN", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                            comm.ExecuteNonQuery();
                                        }
                                    }
                                    catch (MySqlException ex)
                                    {
                                        MessageBox.Show("Ocurrió un error en el proceso de actualizar la guía" + Environment.NewLine +
                                                "no se borraron los datos de la planilla " + Environment.NewLine +
                                                ex.Message, "Alerta proceso no concluido!");
                                    }
                                }
                            }
                            if (dataGridView1.Rows[i].Cells[11].Value == null)   // fila nueva, se inserta  || .ToString() != "X"
                            {
                                string inserd2 = "insert into detplacar (idc,serplacar,numplacar,fila,numpreg,serguia,numguia,totcant,totpeso,totflet,codmone,estadoser,origreg," +
                                "verApp,userc,fechc,diriplan4,diripwan4,netbname,nombult," +
                                "platracto,placarret,autorizac,confvehic,brevchofe,brevayuda,rucpropiet,fechope,pagado,salxcob) " +
                                "values (@idr,@serpl,@numpl,@fila,@numpr,@sergu,@numgu,@totca,@totpe,@totfl,@codmo,@estad,@orireg," +
                                "@verApp,@asd,now(),@iplan,@ipwan,@nbnam,@nombu," +
                                "@platr,@placa,@autor,@confv,@brevc,@breva,@rucpr,@fecho,@paga,@xcob)";
                                micon = new MySqlCommand(inserd2, conn);
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@serpl", tx_serie.Text);
                                micon.Parameters.AddWithValue("@numpl", tx_numero.Text);
                                micon.Parameters.AddWithValue("@fila", fila);
                                micon.Parameters.AddWithValue("@numpr", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@sergu", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                micon.Parameters.AddWithValue("@numgu", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                micon.Parameters.AddWithValue("@totca", dataGridView1.Rows[i].Cells[4].Value.ToString());
                                micon.Parameters.AddWithValue("@nombu", dataGridView1.Rows[i].Cells[16].Value.ToString());
                                micon.Parameters.AddWithValue("@totpe", dataGridView1.Rows[i].Cells[5].Value.ToString());
                                micon.Parameters.AddWithValue("@totfl", dataGridView1.Rows[i].Cells[7].Value.ToString());
                                micon.Parameters.AddWithValue("@codmo", tx_dat_mone.Text);
                                micon.Parameters.AddWithValue("@estad", tx_dat_estad.Text);
                                micon.Parameters.AddWithValue("@orireg", "M");              // origen del registro manual, cuando viene desde el form de guias es A
                                micon.Parameters.AddWithValue("@verApp", verapp);
                                micon.Parameters.AddWithValue("@asd", asd);
                                micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                                micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                micon.Parameters.AddWithValue("@platr", tx_pla_placa.Text);
                                micon.Parameters.AddWithValue("@placa", tx_pla_carret.Text);
                                micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                                micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text + "-" + tx_carret_conf.Text);
                                micon.Parameters.AddWithValue("@brevc", tx_pla_brevet.Text);
                                micon.Parameters.AddWithValue("", tx_pla_nomcho.Text);           // nombre del chofer
                                micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                                micon.Parameters.AddWithValue("", tx_pla_nomayu.Text);           // nombre del ayudante
                                micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "") ? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                                micon.Parameters.AddWithValue("@fecho", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                                micon.Parameters.AddWithValue("@paga", dataGridView1.Rows[i].Cells[8].Value.ToString());    // 
                                micon.Parameters.AddWithValue("@xcob", dataGridView1.Rows[i].Cells[9].Value.ToString());    // 
                                micon.ExecuteNonQuery();
                            }
                        }
                        micon.Dispose();
                        string conupd = "numdetpla";                                    // numeramos las filas de la planilla
                        using (MySqlCommand comup = new MySqlCommand(conupd, conn))     // secuencialmente del 1 al infinito
                        {
                            comup.CommandType = CommandType.StoredProcedure;
                            comup.Parameters.AddWithValue("@vseri", tx_serie.Text);
                            comup.Parameters.AddWithValue("@vnume", tx_numero.Text);
                            comup.ExecuteNonQuery();
                        }
                        retorna = true;
                    }
                    conn.Close();
                }
                /*catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar la planilla");
                    Application.Exit();
                    return retorna;
                }*/
            }
            else
            {
                MessageBox.Show("No fue posible conectarse al servidor de datos");
                Application.Exit();
                return retorna;
            }
            return retorna;
        }
        private bool anula()
        {
            bool retorna = false;
            // cambia estado a ANULADO en cabecera
            // el trigger after_update debe cambiar estado ANULADO en detalle
            // el trigger after_update debe borrar los campos de enlace en cabguiai y controlg
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabplacar set estadoser=@estser,usera=@asd,fecha=now()," +
                        "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp,estintreg=@eirA0 " +
                        "where id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
                        micon.Parameters.AddWithValue("@eirA0", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
                        micon.ExecuteNonQuery();
                        retorna = true;
                    }
                }
            }
            return retorna;
        }
        #endregion boton_form;

        #region leaves y checks
        private void tx_idr_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "" && tx_numero.Text.Trim() == "")
            {
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
                if (Tx_modo.Text == "EDITAR" && tx_dat_estad.Text == codGene) dataGridView1.ReadOnly = false;
                else dataGridView1.ReadOnly = true;
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            if (tx_serie.Text.Trim() != "" && Tx_modo.Text != "NUEVO")
            {
                tx_serie.Text = lib.Right("000" + tx_serie.Text.Trim(), 4);
            }
        }
        private void tx_numero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Tx_modo.Text != "NUEVO" && tx_numero.Text.Trim() != "")
            {
                if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab)
                {
                    tx_numero.Text = lib.Right("0000000" + tx_numero.Text.Trim(), 8);
                    jalaoc("sernum");
                    jaladet(tx_idr.Text);
                    dataGridView1.ReadOnly = true;
                    int tfil = 0;
                    int.TryParse(tx_tfil.Text, out tfil);
                    if (int.Parse(tx_tfil.Text) > 0 && Tx_modo.Text == "EDITAR")
                    {
                        splitContainer1.Panel1.Enabled = false;
                    }
                    if (Tx_modo.Text == "EDITAR" && tx_dat_estad.Text == codCier)        // si es del local del usuario y de la fecha actual, permite re-abrir la planilla 08/06/2021
                    {
                        // tx_fechope.Text == tx_fechact.Text.Substring(0, 10) && tx_dat_locori.Text == v_clu
                        if ((DateTime.Parse(tx_fechact.Text) - DateTime.Parse(tx_fechope.Text)).Days <= v_cdrp && tx_dat_locori.Text == v_clu)
                        {
                            chk_cierea.Text = "RE ABRE LA PLANILLA";
                            chk_cierea.Enabled = true;
                        }
                    }
                    if (Tx_modo.Text == "EDITAR" && tx_dat_estad.Text == codGene)
                    {
                        dataGridView1.ReadOnly = false;
                    }
                }
            }
        }
        private void rb_propio_Click(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 560;
            splitContainer1.Enabled = true;
            splitContainer1.Panel1.Enabled = true;
            tx_car3ro_ruc.Text = "";
            tx_car_3ro_nombre.Text = "";
            tx_pla_ruc.Text = "";
            tx_pla_propiet.Text = "";
            splitContainer1.Panel2.Enabled = false;
            tx_pla_ruc.Text = rucclie;
            tx_pla_propiet.Text = nomclie;
            tx_pla_confv.ReadOnly = true;
            tx_pla_autor.ReadOnly = true;
            splitContainer1.Panel1.Focus();
        }
        private void rb_3ro_Click(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 560;
            splitContainer1.Enabled = true;
            splitContainer1.Panel1.Enabled = true;
            tx_car3ro_ruc.Text = "";
            tx_car_3ro_nombre.Text = "";
            tx_pla_ruc.Text = "";
            tx_pla_propiet.Text = "";
            splitContainer1.Panel2.Enabled = false;
            tx_pla_confv.ReadOnly = false;
            tx_pla_autor.ReadOnly = false;
            splitContainer1.Panel1.Focus();
        }
        private void rb_bus_Click(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 30;
            splitContainer1.Enabled = true;
            splitContainer1.Panel1.Enabled = false;
            tx_pla_brevet.Text = "";
            tx_pla_nomcho.Text = "";
            tx_pla_ayud.Text = "";
            tx_pla_nomayu.Text = "";
            tx_pla_ruc.Text = "";
            tx_pla_propiet.Text = "";
            tx_pla_placa.Text = "";
            tx_pla_carret.Text = "";
            tx_pla_confv.Text = "";
            tx_pla_autor.Text = "";
            splitContainer1.Panel2.Enabled = true;
            splitContainer1.Panel2.Focus();
        }
        private void brev_chof_Leave(object sender, EventArgs e)
        {
            if (rb_propio.Checked == true)
            {
                // aca se debe validar en RR.HH..... cuando  haya modulo
            }
        }
        private void brev_ayud_Leave(object sender, EventArgs e)
        {
            if (rb_propio.Checked == true)
            {
                // aca se debe validar en RR.HH..... cuando  haya modulo
            }
        }
        private void ruc_transp_Leave(object sender, EventArgs e)
        {
            if (rb_3ro.Checked == true || rb_bus.Checked == true)
            {
                if (tx_pla_propiet.Text != tx_pla_propiet.Tag.ToString())
                {
                    // validamos que el ruc ingresado se encuentre en la anag_for
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        string cruc = (tx_pla_ruc.Text.Trim() == "") ? tx_car3ro_ruc.Text : tx_pla_ruc.Text;
                        string consulta = "select razonsocial from anag_for where ruc=@ruc";
                        using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                        {
                            micon.Parameters.AddWithValue("@ruc", cruc);
                            MySqlDataReader dr = micon.ExecuteReader();
                            if (dr.HasRows)
                            {
                                if (dr.Read())
                                {
                                    if (rb_3ro.Checked == true) tx_pla_propiet.Text = dr.GetString(0);
                                    else tx_car_3ro_nombre.Text = dr.GetString(0);
                                }
                            }
                            dr.Dispose();
                        }
                    }
                }
                tx_pla_propiet.Tag = "x";
            }
        }
        private void placa_Leave(object sender, EventArgs e)
        {
            // valida existencia en vehiculos como trompa
            // si existe pone la conf. vehicular y autorizacion
            if (rb_propio.Checked == true && tx_pla_placa.Text.Trim() != "")
            {
                string[] datos = ValPlaCarr("P", tx_pla_placa.Text);
                if (datos[0].Length < 1)
                {
                    MessageBox.Show("No existe la placa", "Error en ingreso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_placa.Text = "";
                    tx_pla_placa.Focus();
                    return;
                }
                else
                {
                    tx_pla_confv.Text = datos[0];
                    tx_pla_autor.Text = datos[1];
                    tx_pla_carret.Text = datos[2];
                    tx_pla_marca.Text = datos[3]; // ayu3.ReturnValueA[1];
                    tx_pla_modelo.Text = datos[4]; //ayu3.ReturnValueA[2];
                    if (tx_pla_carret.Text.Trim() != "")
                    {
                        carreta_Leave(null,null);
                    }
                }
            }
        }
        private void carreta_Leave(object sender, EventArgs e)
        {
            // valida existencia en vehiculos como CARRETA
            // si existe CONCATENA la conf. vehicular
            if (rb_propio.Checked == true && tx_pla_carret.Text.Trim() != "")
            {
                string[] datos = ValPlaCarr("C", tx_pla_carret.Text);
                if (datos[0].Length < 1)
                {
                    MessageBox.Show("No existe la carreta", "Error en ingreso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tx_pla_carret.Text = "";
                    tx_pla_carret.Focus();
                    return;
                }
                else
                {
                    //tx_pla_confv.Text = tx_pla_confv.Text.Trim()  + " " + datos[0].Trim();
                    tx_carret_conf.Text = datos[0].Trim();
                    tx_carret_autoriz.Text = datos[1].Trim();
                    tx_carret_marca.Text = datos[3].Trim();
                    tx_carret_modelo.Text = datos[4].Trim();
                }
            }
        }
        //...
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
            Tx_modo.Text = "NUEVO";
            button1.Image = Image.FromFile(img_grab);
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            chk_cierea.Visible = false;
            //
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            button1.Enabled = true;
            initIngreso();
            escribe();
            splitContainer1.Enabled = true;
            tx_serie.ReadOnly = true;
            tx_numero.ReadOnly = true;
            tx_pla_fech.Text = DateTime.Today.ToString().Substring(0,10);
            tx_flete.Text = "0";
            tx_pagado.Text = "0";
            tx_salxcob.Text = "0";
            tx_tfil.Text = "0";
            tx_totcant.Text = "0";
            tx_totpes.Text = "0";
            tx_dat_mone.Text = v_mondef;
            cmb_mon.SelectedValue = v_mondef;
            rb_propio.PerformClick();
            cmb_origen.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            chk_cierea.Visible = true;
            chk_cierea.Text = "";
            //
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            initIngreso();
            splitContainer1.Enabled = true;
            escribe();
            tx_serie.ReadOnly = false;      // cambia a true una ves jalado los datos
            tx_numero.ReadOnly = false;     // cambia a true una ves jalado los datos
            tx_pla_fech.ReadOnly = false;
            cmb_origen.Enabled = false;
            cmb_destino.Enabled = false;
            rb_propio.PerformClick();
            tx_dat_mone.Text = v_mondef;
            cmb_mon.SelectedValue = v_mondef;
            tx_serie.Focus();
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            // Impresion ó Re-impresion ??
            //if (tx_impreso.Text == "S")
            {
                var aa = MessageBox.Show("Desea imprimir el documento?", "Confirme por favor", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
                    v_CR_gr_ind = cmb_forimp.Text;
                    if (vi_formato == "A4")            // Seleccion de formato ... A4
                    {
                        if (imprimeA4() == true) updateprint("S");
                    }
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
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            chk_cierea.Visible = false;
            //
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            initIngreso();
            splitContainer1.Enabled = false;
            gbox_serie.Enabled = true;
            tx_pla_fech.ReadOnly = true;
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_serie.Focus();
        }
        private void Bt_ver_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "VISUALIZAR";
            button1.Image = Image.FromFile(img_ver);
            initIngreso();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            chk_cierea.Visible = false;
            chk_cierea.Text = "";
            //
            gbox_serie.Enabled = true;
            tx_serie.ReadOnly = false;
            tx_numero.ReadOnly = false;
            tx_pla_fech.ReadOnly = true;
            tx_serie.Focus();
        }
        private void Bt_first_Click(object sender, EventArgs e)
        {
            limpiar();
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
                limpiar();
                limpia_chk();
                limpia_combos();
                limpia_otros();
                tx_idr.Text = aca.ToString();
                tx_idr_Leave(null, null);
            }
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            if (tx_idr.Text.Trim() != "")
            {
                int aca = int.Parse(tx_idr.Text) + 1;
                limpiar();
                limpia_chk();
                limpia_combos();
                limpia_otros();
                tx_idr.Text = aca.ToString();
                tx_idr_Leave(null, null);
            }
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar();
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
                //tx_dirOrigen.Text = lib.dirloca(lib.codloc(asd));
                //tx_serie.Text = v_slu;
                if (Tx_modo.Text == "NUEVO" && tx_dat_locdes.Text.Trim() != "")
                {
                    string consul = "SELECT tipdoc,serie,actual,final,format,glosaser,dir_pe,ubigeo," +
                        "imp_ini,imp_fec,imp_det,imp_dtr,imp_pie " +
                        "FROM series WHERE STATUS<> @ean and " +
                        "tipdoc = @td AND sede = @ori AND zona = (SELECT zona FROM desc_loc WHERE idcodice = @des)";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        using (MySqlCommand micon = new MySqlCommand(consul, conn))
                        {
                            micon.Parameters.AddWithValue("@ean", codAnul);
                            micon.Parameters.AddWithValue("@td", v_cid);
                            micon.Parameters.AddWithValue("@ori", tx_dat_locori.Text);
                            micon.Parameters.AddWithValue("@des", tx_dat_locdes.Text);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    tx_serie.Text = dr.GetString(1);
                                }
                            }
                        }
                    }
                }
            }
            if (tx_dat_locori.Text.Trim() != "")
            {
                DataRow[] fila = dtu.Select("idcodice='" + tx_dat_locori.Text + "'");
                //tx_ubigO.Text = fila[0][2].ToString();
            }
        }
        private void cmb_destino_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmb_destino.SelectedIndex > -1)
            {
                tx_dat_locdes.Text = cmb_destino.SelectedValue.ToString();
                //tx_dirDestino.Text = lib.dirloca(tx_dat_locdes.Text);
                if (Tx_modo.Text == "NUEVO")
                {
                    // vamos por la serie
                    string consul = "SELECT tipdoc,serie,actual,final,format,glosaser,dir_pe,ubigeo," +
                        "imp_ini,imp_fec,imp_det,imp_dtr,imp_pie " +
                        "FROM series WHERE STATUS<> @ean and " +
                        "tipdoc = @td AND sede = @ori AND zona = (SELECT zona FROM desc_loc WHERE idcodice = @des)";
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        using (MySqlCommand micon = new MySqlCommand(consul, conn))
                        {
                            micon.Parameters.AddWithValue("@ean", codAnul);
                            micon.Parameters.AddWithValue("@td", v_cid);
                            micon.Parameters.AddWithValue("@ori", tx_dat_locori.Text);
                            micon.Parameters.AddWithValue("@des", tx_dat_locdes.Text);
                            using (MySqlDataReader dr = micon.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    tx_serie.Text = dr.GetString(1);
                                }
                            }
                        }
                    }
                }
            }
            if (tx_dat_locdes.Text.Trim() != "")
            {
                DataRow[] fila = dtd.Select("idcodice='" + tx_dat_locdes.Text + "'");
                //tx_ubigD.Text = fila[0][2].ToString();
            }
        }
        #endregion comboboxes

        #region impresion
        private bool imprimeA4()
        {
            bool retorna = false;
            llenaDataSet();                         // metemos los datos al dataset de la impresion
            return retorna;
        }
        private bool imprimeA5()
        {
            bool retorna = false;
            //
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            // 
            return retorna;
        }
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (vi_formato == "A4")
            {
                imprime_A4(sender, e);
            }
            if (vi_formato == "A5")
            {
                imprime_A5(sender, e);
            }
            if (vi_formato == "TK")
            {
                imprime_TK(sender, e);
            }
        }
        private void imprime_A4(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }
        private void imprime_A5(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float alfi = 20.0F;     // alto de cada fila
            float alin = 50.0F;     // alto inicial
            float posi = 80.0F;     // posición de impresión
            float coli = 20.0F;     // columna mas a la izquierda
            //float cold = 80.0F;
            Font lt_tit = new Font("Arial", 11);
            Font lt_titB = new Font("Arial", 11, FontStyle.Bold);
            PointF puntoF = new PointF(coli, alin);
            e.Graphics.DrawString(nomclie, lt_titB, Brushes.Black, puntoF, StringFormat.GenericTypographic);                      // titulo del reporte
            posi = posi + alfi;
            posi = posi + alfi;

        }
        private void imprime_TK(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // no hay guias en TK
        }
        private void updateprint(string sn)  // actualiza el campo impreso de la GR = S
        {   // S=si impreso || N=no impreso
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "update cabguiai set impreso=@sn where id=@idr";
                using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                {
                    micon.Parameters.AddWithValue("@sn", sn);
                    micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                    micon.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region crystal
        private void llenaDataSet()
        {
            try
            {
                if (v_CR_gr_ind.Trim() == "")
                {
                    MessageBox.Show("Seleccione formato de impresión", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmb_forimp.Focus();
                    return;
                }
                conClie data = generaReporte();
                ReportDocument repo = new ReportDocument();
                repo.Load(v_CR_gr_ind);
                repo.SetDataSource(data);
                repo.PrintOptions.PrinterName = v_impA4;
                repo.PrintToPrinter(int.Parse(vi_copias), false, 0, 0);    // ,1,1
            }
            catch (Exception ex)
            {
                MessageBox.Show("Confirme su configuración de impresión" + Environment.NewLine + 
                    ex.Message,"Error en Impresión");
                return;
            }
        }
        private conClie generaReporte()
        {
            conClie PlaniC = new conClie();
            // CABECERA
            conClie.placar_cabRow rowcabeza = PlaniC.placar_cab.Newplacar_cabRow();
            rowcabeza.rucEmisor = rucclie;
            rowcabeza.nomEmisor = nomclie;
            rowcabeza.dirEmisor = Program.dirfisc;  // + " " + Program.distfis + " " + Program.provfis + " " + Program.depfisc;
            rowcabeza.id = tx_idr.Text;
            rowcabeza.autoriz = tx_pla_autor.Text;
            rowcabeza.brevAyudante = tx_pla_ayud.Text;
            rowcabeza.brevChofer = tx_pla_brevet.Text;
            rowcabeza.camion = tx_pla_carret.Text;
            rowcabeza.confvehi = tx_pla_confv.Text;
            rowcabeza.direDest = "";
            rowcabeza.direOrigen = "";
            rowcabeza.fechope = tx_pla_fech.Text;    // tx_fechope.Text.Substring(0, 10);
            rowcabeza.marcaModelo = "";
            rowcabeza.nomAyudante = tx_pla_nomayu.Text;
            rowcabeza.nomChofer = tx_pla_nomcho.Text;
            rowcabeza.nomDest = cmb_destino.Text;
            rowcabeza.nomOrigen = cmb_origen.Text;
            rowcabeza.nomPropiet = tx_pla_propiet.Text;
            rowcabeza.numpla = tx_numero.Text;
            rowcabeza.placa = tx_pla_placa.Text;
            rowcabeza.rucPropiet = tx_pla_ruc.Text;
            rowcabeza.serpla = tx_serie.Text;
            rowcabeza.fechSalida = "";
            rowcabeza.fechLlegada = "";
            rowcabeza.estado = tx_estado.Text;
            rowcabeza.tituloF = Program.tituloF;
            PlaniC.placar_cab.Addplacar_cabRow(rowcabeza);
            //
            // DETALLE  
            if (rb_orden_gr.Checked == true) dataGridView1.Sort(dataGridView1.Columns["numguia"], System.ComponentModel.ListSortDirection.Ascending);
            if (rb_orden_dir.Checked == true) dataGridView1.Sort(dataGridView1.Columns[14], System.ComponentModel.ListSortDirection.Ascending);
            if (rb_orden_des.Checked == true) dataGridView1.Sort(dataGridView1.Columns[13], System.ComponentModel.ListSortDirection.Ascending);
            int i = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    i = i + 1;
                    conClie.placar_detRow rowdetalle = PlaniC.placar_det.Newplacar_detRow();
                    rowdetalle.fila = i.ToString();  // row.Cells["fila"].Value.ToString();
                    rowdetalle.id = tx_idr.Text;
                    rowdetalle.idc = "";
                    rowdetalle.moneda = row.Cells["MON"].Value.ToString();
                    rowdetalle.numguia = row.Cells["numguia"].Value.ToString();
                    rowdetalle.pagado = double.Parse(row.Cells[8].Value.ToString());
                    rowdetalle.salxcob = double.Parse(row.Cells[9].Value.ToString());
                    rowdetalle.serguia = row.Cells["serguia"].Value.ToString();
                    rowdetalle.totcant = Int16.Parse(row.Cells["totcant"].Value.ToString());
                    rowdetalle.totflete = Double.Parse(row.Cells["totflet"].Value.ToString());
                    rowdetalle.totpeso = int.Parse(row.Cells["totpeso"].Value.ToString());
                    rowdetalle.nomdest = row.Cells[13].Value.ToString();
                    rowdetalle.dirdest = row.Cells[14].Value.ToString();
                    rowdetalle.teldest = row.Cells[15].Value.ToString();
                    rowdetalle.nombulto = row.Cells[16].Value.ToString();
                    rowdetalle.nomremi = "";    // row.Cells[].Value.ToString();
                    rowdetalle.docvta = row.Cells[17].Value.ToString();
                    rowdetalle.nomremi = row.Cells[18].Value.ToString();
                    PlaniC.placar_det.Addplacar_detRow(rowdetalle);
                }
            }
            //
            return PlaniC;
        }
        #endregion

        #region datagridview
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)     // jala datos de cabecera guias
        {
            if (e.ColumnIndex == 1 && e.FormattedValue.ToString().Trim() != "") // pre guia
            {
                // las planillas de carga solo se llenan con guias individuales
            }
            if (e.ColumnIndex == 2 && e.FormattedValue.ToString().Trim() != "") // serie guia
            {
                // validamos que la serie de la guia corresponda al local de la planilla, serie de la planilla
                if (e.FormattedValue.ToString() != tx_serie.Text)
                {
                    if (dataGridView1.EditingControl != null) dataGridView1.EditingControl.Text = tx_serie.Text;
                }
            }
            if (e.ColumnIndex == 3 && e.FormattedValue.ToString().Trim() != "") // numero gúia
            {
                string completo = "";
                if (e.FormattedValue.ToString().Trim().Length > 0)
                {
                    completo = lib.Right("0000000" + e.FormattedValue, 8);
                    if (dataGridView1.EditingControl != null) dataGridView1.EditingControl.Text = completo;
                }
                if (completo.Length == 8 && dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString().Trim().Length == 4 && 
                    dataGridView1.Rows[e.RowIndex].Cells[11].Value == null)
                {
                    using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                    {
                        conn.Open();
                        string consulta = "select a.numpregui,a.cantotgri,a.pestotgri,b.descrizionerid as MON,a.totgri,a.totpag,a.salgri,a.tipmongri,a.numplagri," +
                            "c.unimedpro,ifnull(" +
                            "concat(d.descrizionerid,'-',if(SUBSTRING(a.serdocvta,1,2)='00',SUBSTRING(a.serdocvta,3,2),a.serdocvta),'-',if(SUBSTRING(a.numdocvta,1,3)='000',SUBSTRING(a.numdocvta,4,5),a.numdocvta)),'')" + 
                            "from cabguiai a left join desc_mon b on b.idcodice=a.tipmongri " +
                            "left join detguiai c on c.idc=a.id " +
                            "left join desc_tdv d on d.idcodice=a.tipdocvta " +
                            "where a.sergui=@ser and a.numgui=@num limit 1 ";
                        using (MySqlCommand micon = new MySqlCommand(consulta, conn))
                        {
                            micon.Parameters.AddWithValue("@ser", dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString().Trim());
                            micon.Parameters.AddWithValue("@num", completo);
                            MySqlDataReader dr = micon.ExecuteReader();
                            if (dr.HasRows)
                            {
                                if (dr.Read())
                                {
                                    if (dr.GetString(8).Trim() == "")
                                    {
                                        dataGridView1.Rows[e.RowIndex].Cells[0].Value = e.RowIndex + 1;
                                        dataGridView1.Rows[e.RowIndex].Cells[1].Value = dr.GetString(0);
                                        dataGridView1.Rows[e.RowIndex].Cells[4].Value = dr.GetString(1);
                                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = dr.GetString(2);
                                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = dr.GetString(3);
                                        dataGridView1.Rows[e.RowIndex].Cells[7].Value = dr.GetString(4);
                                        dataGridView1.Rows[e.RowIndex].Cells[8].Value = dr.GetString(5);
                                        dataGridView1.Rows[e.RowIndex].Cells[9].Value = dr.GetString(6);
                                        dataGridView1.Rows[e.RowIndex].Cells[10].Value = dr.GetString(7);
                                        dataGridView1.Rows[e.RowIndex].Cells[16].Value = dr.GetString(9);
                                        dataGridView1.Rows[e.RowIndex].Cells[17].Value = dr.GetString(10);
                                    }
                                    else
                                    {
                                        MessageBox.Show("La Guía ingresada ya está registrada" + Environment.NewLine +
                                            "Planilla: " + dr.GetString(8).Trim(), "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        e.Cancel = true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("La Guía ingresada no existe", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                e.Cancel = true;
                            }
                            dr.Dispose();
                        }
                    }
                }
            }
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)       // cursor a la derecha o siguiente fila ... NO FUNCA
        {
           if (e.KeyCode == Keys.Enter)
            {
                //if (dataGridView1.CurrentCell.ColumnIndex == 2)
                {
                    e.SuppressKeyPress = true;
                    SendKeys.Send("{TAB}");
                }
                //if (dataGridView1.CurrentCell.ColumnIndex == 3)
                //{
                //    dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[2].Selected = true;
                //}
            }
        }
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            operaciones();
        }
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = tx_serie.Text;
            }
        }
        // evento click en el checkbox de la coumna 14
        #endregion

    }
}
