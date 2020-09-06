﻿using System;
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
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string vi_formato = "";         // formato de impresion del documento
        string vi_copias = "";          // cant copias impresion
        string v_impA5 = "";            // nombre de la impresora matricial
        string v_impTK = "";            // nombre de la ticketera
        string vtc_flete = "";          // el detalle va con el flete impreso ?? SI || NO
        string v_cid = "";              // codigo interno de tipo de documento
        string v_fra1 = "";             // frase de si va o no con clave
        string v_fra2 = "";             // frase 
        string v_sanu = "";             // serie anulacion interna ANU
        string v_CR_gr_ind = "";        // nombre del formato en CR
        string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string v_trompa = "";           // codigo interno placa de tracto/camion
        string v_carret = "";           // código interno placa de carreta/furgon
        string v_mondef = "";           // moneda por defecto del form
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        #endregion

        // string de conexion
        static string port = ConfigurationManager.AppSettings["port"].ToString();
        static string data = ConfigurationManager.AppSettings["data"].ToString();
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + data + ";";
        DataTable dtu = new DataTable();
        DataTable dtd = new DataTable();
        DataTable dttd0 = new DataTable();
        DataTable dttd1 = new DataTable();
        DataTable dtm = new DataTable();
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
        }
        private void armagrilla()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.ColumnCount = 13;
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
        }
        private void jalainfo()                 // obtiene datos de imagenes y variables
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nofa,@nofi)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
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
                            if (row["param"].ToString() == "serieAnu") v_sanu = row["valor"].ToString().Trim();               // serie anulacion interna
                        }
                        if (row["campo"].ToString() == "impresion")
                        {
                            if (row["param"].ToString() == "formato") vi_formato = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "filasDet") v_mfildet = row["valor"].ToString().Trim();       // maxima cant de filas de detalle
                            if (row["param"].ToString() == "copias") vi_copias = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impMatris") v_impA5 = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "impTK") v_impTK = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "nomGRi_cr") v_CR_gr_ind = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "carguero")
                        {
                            if (row["param"].ToString() == "codTrackto") v_trompa = row["valor"].ToString().Trim();
                            if (row["param"].ToString() == "codCarreta") v_carret = row["valor"].ToString().Trim();
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") v_mondef = row["valor"].ToString().Trim();
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
                        "a.confvehic,a.brevchofe,a.nomchofe,a.brevayuda,a.nomayuda,a.rucpropie,a.tipoplani,a.userc,a.userm,a.usera,ifnull(b.razonsocial,'') as razonsocial " +
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
                            //
                            tx_car3ro_ruc.Text = dr.GetString("rucpropie");
                            tx_car_3ro_nombre.Text = dr.GetString("razonsocial");  // falta en consulta
                        }
                        tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                        cmb_destino.SelectedValue = tx_dat_locdes.Text;
                        cmb_origen.SelectedValue = tx_dat_locori.Text;
                        cmb_mon.SelectedValue = tx_dat_mone.Text;
                        // si el documento esta ANULADO o un estado que no permite EDICION, se pone todo en sololee
                        if (tx_dat_estad.Text != codGene)
                        {
                            sololee();
                            MessageBox.Show("Este documento no puede ser editado", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            tx_serie.ReadOnly = true;   // despues de una jalada exitosa
                            tx_numero.ReadOnly = true;  // estos numeros no deben modificarse
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
            string jalad = "select a.idc,a.serplacar,a.numplacar,a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,a.totpeso,b.descrizionerid as MON,a.totflet," +
                "a.estadoser,a.codmone,'X' as marca,a.id,a.pagado,a.salxcob " +
                "from detplacar a left join desc_mon b on b.idcodice=a.codmone where a.idc=@idr";
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
                                    row[14].ToString()
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
                                    false
                                    );
                            }
                        }
                        dt.Dispose();
                    }
                }
            }
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
            conn.Close();
        }
        private bool valiGri()                  // valida filas completas en la grilla - 8 columnas
        {
            bool retorna = false;
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
            if (v_impA5 == "")          // nombre de la impresora matricial
            {
                lib.messagebox("Nombre de impresora matricial");
                retorna = false;
            }
            if (v_impTK == "")           // nombre de la ticketera
            {
                lib.messagebox("Nombre de impresora de Tickets");
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
            return retorna;
        }
        private string[] ValPlaCarr(string pc,string codigo)    // pc=P ó C, codigo=placa de trompa o carreta
        {
            string[] retorna = { "", "" };      // cofig.vehicular, autorizacion
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

                string consulta = "select confve,autor1 from vehiculos where " + parte0 + " and " + parte1;
                using (MySqlCommand micon = new MySqlCommand(consulta,conn))
                {
                    micon.Parameters.AddWithValue("@codigo", codigo);
                    micon.Parameters.AddWithValue("@tipo", (pc == "P")? v_trompa : v_carret);
                    MySqlDataReader dr = micon.ExecuteReader();
                    while (dr.Read())
                    {
                        retorna[0] = dr.GetString(0);
                        retorna[1] = dr.GetString(1);
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
                if (Tx_modo.Text == "EDITAR")
                {
                    if (dataGridView1.Rows[i].Cells[13].Value != null)
                    {
                        if (dataGridView1.Rows[i].Cells[13].Value.ToString() == "False")
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
            if (int.Parse(tx_tfil.Text) == int.Parse(v_mfildet))
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
            tx_pla_propiet.ReadOnly = true;
            tx_car_3ro_nombre.ReadOnly = true;
            tx_pla_confv.ReadOnly = true;
            tx_pla_autor.ReadOnly = true;
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
            if(tx_serie.Text.Trim() == "")
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
                    MessageBox.Show("La planilla tiene estado que impide su edición", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (true)   // de momento no validamos mas
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar la planilla?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            if (edita() == true)
                            {
                                // 
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

                if (true)   // (tx_pla_plani.Text.Trim() == "") && tx_impreso.Text == "N"
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
                    "brevayuda,rucpropie,tipoplani,nomchofe,nomayuda," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fecho,@serpl,@locor,@locde,@obspl,@cantf,@canto,@pesto,@tipmo,@tipca,@subto," +
                    "@igvpl,@totpl,@totpa,@salxp,@estad,@fleim,@platr,@placa,@autor,@confv,@brevc," +
                    "@breva,@rucpr,@tipop,@nocho,@noayu," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
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
                    micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text);
                    micon.Parameters.AddWithValue("@brevc", tx_pla_brevet.Text);
                    micon.Parameters.AddWithValue("@nocho", tx_pla_nomcho.Text);           // nombre del chofer
                    micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                    micon.Parameters.AddWithValue("@noayu", tx_pla_nomayu.Text);           // nombre del ayudante
                    micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "")? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                    micon.Parameters.AddWithValue("@tipop", vtip);              // tipo planilla, tipo transporte/transportista
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
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
                                string inserd2 = "insert into detplacar (idc,serplacar,numplacar,fila,numpreg,serguia,numguia,totcant,totpeso,totflet,codmone,estadoser," +
                                    "verApp,userc,fechc,diriplan4,diripwan4,netbname,platracto,placarret,autorizac,confvehic,brevchofe,brevayuda,rucpropiet,fechope,pagado,salxcob) " +
                                    "values (@idr,@serpl,@numpl,@fila,@numpr,@sergu,@numgu,@totca,@totpe,@totfl,@codmo,@estad," +
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
                                    micon.Parameters.AddWithValue("@verApp", verapp);
                                    micon.Parameters.AddWithValue("@asd", asd);
                                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                    micon.Parameters.AddWithValue("@platr", tx_pla_placa.Text);
                                    micon.Parameters.AddWithValue("@placa", tx_pla_carret.Text);
                                    micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                                    micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text);
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
                try
                {
                    if (tx_dat_estad.Text == codGene)               // solo edita estado GENERADO, otro estado no se edita
                    {                                               // El estado cambia solo cuando: SE CIERRA MANUALMENTE ó CUANDO SE RECEPCIONA LA PLANILLA
                        int vtip = 0;                               // los datos que NO SE EDITAN son: serie,numero,origen y destino
                        if (rb_propio.Checked == true) vtip = 1;    // los totales filas ,peso y bultos si cambian con la edicion
                        if (rb_3ro.Checked == true) vtip = 2;       // los fletes y saldos de cada guía NO CAMBIAN al editar, salvo si se borra y vuelte a registrar la GR
                        if (rb_bus.Checked == true) vtip = 3;   // locorigen=@locor,locdestin=@locde,estadoser=@estad,
                        string actua = "update cabplacar set " +
                            "fechope=@fecho,obsplacar=@obspl,cantfilas=@cantf,cantotpla=@canto,pestotpla=@pesto,tipmonpla=@tipmo," +
                            "tipcampla=@tipca,subtotpla=@subto,igvplacar=@igvpl,totplacar=@totpl,totpagado=@totpa,salxpagar=@salxp,fleteimp=@fleim," +
                            "platracto=@platr,placarret=@placa,autorizac=@autor,confvehic=@confv,brevchofe=@brevc,brevayuda=@breva,rucpropie=@rucpr,tipoplani=@tipop," +
                            "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam,nomchofe=@nocho,nomayuda=@noayu " +
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
                        //micon.Parameters.AddWithValue("@estad", tx_dat_estad.Text);
                        micon.Parameters.AddWithValue("@fleim", tx_dat_detflete.Text);      // variable si detalle lleva valores flete guias
                        micon.Parameters.AddWithValue("@platr", tx_pla_placa.Text);
                        micon.Parameters.AddWithValue("@placa", tx_pla_carret.Text);
                        micon.Parameters.AddWithValue("@autor", tx_pla_autor.Text);
                        micon.Parameters.AddWithValue("@confv", tx_pla_confv.Text);
                        micon.Parameters.AddWithValue("@brevc", tx_pla_brevet.Text);
                        micon.Parameters.AddWithValue("@nocho", tx_pla_nomcho.Text);           // nombre del chofer
                        micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                        micon.Parameters.AddWithValue("@noayu", tx_pla_nomayu.Text);           // nombre del ayudante
                        micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "") ? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                        micon.Parameters.AddWithValue("@tipop", vtip);              // tipo planilla, tipo transporte/transportista
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
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
                            if (dataGridView1.Rows[i].Cells[13].Value != null)   // fila marcada para borrar
                            {
                                // saca la guia de detplacar
                                if (dataGridView1.Rows[i].Cells[13].Value.ToString() == "True")
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
                                            // trigger borra los campos en cabguiai
                                            // trigger borra los campos en controlg
                                        }
                                        catch (MySqlException ex)
                                        {
                                            MessageBox.Show("Ocurrió un error en el proceso de borrar la guía de la planilla" + Environment.NewLine +
                                                "y / o en la actualización posterior en Guías y Control " + Environment.NewLine +
                                                ex.Message, "Alerta proceso no concluido!");
                                        }
                                    }
                                }
                            }
                            if (dataGridView1.Rows[i].Cells[11].Value == null)   // fila nueva, se inserta  || .ToString() != "X"
                            {
                                string inserd2 = "insert into detplacar (idc,serplacar,numplacar,fila,numpreg,serguia,numguia,totcant,totpeso,totflet,codmone,estadoser," +
                                "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                                "values (@idr,@serpl,@numpl,@fila,@numpr,@sergu,@numgu,@totca,@totpe,@totfl,@codmo,@estad," +
                                "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                                micon = new MySqlCommand(inserd2, conn);
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@serpl", tx_serie.Text);
                                micon.Parameters.AddWithValue("@numpl", tx_numero.Text);
                                micon.Parameters.AddWithValue("@fila", fila);
                                micon.Parameters.AddWithValue("@numpr", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@sergu", dataGridView1.Rows[i].Cells[2].Value.ToString());
                                micon.Parameters.AddWithValue("@numgu", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                micon.Parameters.AddWithValue("@totca", dataGridView1.Rows[i].Cells[4].Value.ToString());
                                micon.Parameters.AddWithValue("@totpe", dataGridView1.Rows[i].Cells[5].Value.ToString());
                                micon.Parameters.AddWithValue("@totfl", dataGridView1.Rows[i].Cells[7].Value.ToString());
                                micon.Parameters.AddWithValue("@codmo", tx_dat_mone.Text);
                                micon.Parameters.AddWithValue("@estad", tx_dat_estad.Text);
                                micon.Parameters.AddWithValue("@verApp", verapp);
                                micon.Parameters.AddWithValue("@asd", asd);
                                micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                                micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                micon.ExecuteNonQuery();
                            }
                        }
                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[13].Value == null)
                            {
                                // actualiza contador fila en detplacar
                                fila += 1;
                                string consulta = "update detplacar set fila=@fi where serguia=@ser and numguia=@num";
                                using (MySqlCommand comup = new MySqlCommand(consulta , conn))
                                {
                                    comup.Parameters.AddWithValue("@fi", fila);
                                    comup.Parameters.AddWithValue("@ser", dataGridView1.Rows[i].Cells[5].Value.ToString());
                                    comup.Parameters.AddWithValue("@num", dataGridView1.Rows[i].Cells[6].Value.ToString());
                                    comup.ExecuteNonQuery();
                                }
                            }
                        }
                        retorna = true;
                        micon.Dispose();
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar la planilla");
                    Application.Exit();
                    return retorna;
                }
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
            // el trigger before_update debe cambiar estado ANULADO en detalle
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update ??? set estadoser=@estser,usera=@asd,fecha=now()," +
                        "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp " +
                        "where id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
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
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
            }
        }
        private void tx_serie_Leave(object sender, EventArgs e)
        {
            if (tx_serie.Text.Trim() != "" && Tx_modo.Text != "NUEVO")
            {
                tx_serie.Text = lib.Right("000" + tx_serie.Text.Trim(), 4);
            }
        }
        private void tx_numero_Leave(object sender, EventArgs e)
        {
            if (tx_numero.Text.Trim() != "" && Tx_modo.Text != "NUEVO")
            {
                tx_numero.Text = lib.Right("0000000" + tx_numero.Text.Trim(), 8);
                jalaoc("sernum");
                jaladet(tx_idr.Text);
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
                    tx_pla_confv.Text = tx_pla_confv.Text.Trim()  + " " + datos[0].Trim();
                    //tx_pla_autor.Text = datos[1];
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
            //
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            initIngreso();
            escribe();
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
            //
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            initIngreso();
            //armagrilla();
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
                var aa = MessageBox.Show("Desea re imprimir el documento?", "Confirme por favor", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (aa == DialogResult.Yes)
                {
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
            initIngreso();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
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
            int aca = int.Parse(tx_idr.Text) + 1;
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idr.Text = aca.ToString();
            tx_idr_Leave(null, null);
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

            return retorna;
        }
        private bool imprimeA5()
        {
            bool retorna = false;
            llenaDataSet();                         // metemos los datos al dataset de la impresion
            return retorna;
        }
        private bool imprimeTK()
        {
            bool retorna = false;
            try
            {
                printDocument1.PrinterSettings.PrinterName = v_impTK;
                printDocument1.Print();
                retorna = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error en imprimir TK");
                retorna = false;
            }
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
            float cold = 80.0F;
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
            conClie data = generaReporte();
            ReportDocument repo = new ReportDocument();
            repo.Load(v_CR_gr_ind);
            repo.SetDataSource(data);
            repo.PrintOptions.PrinterName = v_impA5;
            repo.PrintToPrinter(int.Parse(vi_copias),false,1,1);
        }
        private conClie generaReporte()
        {
            conClie guiaT = new conClie();
            conClie.gr_ind_cabRow rowcabeza = guiaT.gr_ind_cab.Newgr_ind_cabRow();
            //
            // CABECERA
            rowcabeza.id = tx_idr.Text;
            rowcabeza.estadoser = tx_estado.Text;
            rowcabeza.fechope = tx_fechope.Text;
            rowcabeza.frase1 = "";  // no hay campo
            rowcabeza.frase2 = "";  // no hay campo
            // origen - destino
            rowcabeza.dptoDestino = ""; // no hay campo
            rowcabeza.provDestino = "";
            rowcabeza.distDestino = ""; // no hay campo
            rowcabeza.dptoOrigen = "";  // no hay campo
            rowcabeza.provOrigen = "";
            rowcabeza.distOrigen = "";  // no hay campo
            // importes
            rowcabeza.igv = "";         // no hay campo
            rowcabeza.subtotal = "";    // no hay campo
            // pie
            rowcabeza.brevAyuda = "";   // falta este campo
            //
            guiaT.gr_ind_cab.Addgr_ind_cabRow(rowcabeza);
            //
            // DETALLE  
            // ...
            return guiaT;
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
                        string consulta = "select a.numpregui,a.cantotgri,a.pestotgri,b.descrizionerid as MON,a.totgri,a.totpag,a.salgri,a.tipmongri,a.numplagri " +
                            "from cabguiai a left join desc_mon b on b.idcodice=a.tipmongri where a.sergui=@ser and a.numgui=@num";
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
