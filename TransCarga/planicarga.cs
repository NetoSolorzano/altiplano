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
            // grilla
            armagrilla();
            // todo desabilidado
            sololee();
        }
        private void armagrilla()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            //a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,a.totpeso,b.descrizionerid as MON,a.totflet
            if (dataGridView1.DataSource == null) dataGridView1.ColumnCount = 1;
            //
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                _ = decimal.TryParse(dataGridView1.Rows[0].Cells[i].Value.ToString(), out decimal vd);
                if (vd != 0) dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            int b = 0;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                int a = dataGridView1.Columns[i].Width;
                b += a;
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridView1.Columns[i].Width = a;
            }
            if (b < dataGridView1.Width) dataGridView1.Width = b + 60;
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
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
            try
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
                    string consulta = "select id,fechope,serplacar,numplacar,locorigen,locdestin,obsplacar,cantfilas,cantotpla,pestotpla,tipmonpla," +
                        "tipcampla,subtotpla,igvplacar,totplacar,totpagado,salxpagar,estadoser,impreso,fleteimp,platracto,placarret,autorizac," +
                        "confvehic,brevchofe,nomchofe,brevayuda,nomayuda,rucpropie,tipoplani,userc,userm,usera,anag_for.razonsocial " +
                        "FROM cabplacar left join anag_for on cabplacar.rucpropie=anag_for.ruc and anag_for.estado=0 " + parte;
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
                                    splitContainer1.Panel1.Width = splitContainer1.Width;
                                    break;
                                case "2":   // tercero
                                    rb_3ro.Checked = true;
                                    splitContainer1.Panel1.Width = splitContainer1.Width;
                                    break;
                                case "3":   // agencia bus carga
                                    rb_bus.Checked = true;
                                    splitContainer1.Panel2.Width = splitContainer1.Width;
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
                            tx_pla_propiet.Text = dr.GetString("razonsocial"); // falta en consulta
                            tx_pla_ruc.Text = dr.GetString("rucpropie");
                            //
                            tx_car3ro_ruc.Text = dr.GetString("rucpropie");
                            tx_car_3ro_nombre.Text = dr.GetString("razonsocial");  // falta en consulta
                        }
                        tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                        cmb_destino.SelectedValue = tx_dat_locdes.Text;
                        cmb_origen.SelectedValue = tx_dat_locori.Text;
                        cmb_mon.SelectedValue = tx_dat_mone.Text;
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Fatal en codigo");
                Application.Exit();
                return;
            }
        }
        private void jaladet(string idr)         // jala el detalle
        {
            string jalad = "select a.idc,a.serplacar,a.numplacar,a.fila,a.numpreg,a.serguia,a.numguia,a.totcant,a.totpeso,b.descrizionerid as MON,a.totflet,a.estadoser " +
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
                            dataGridView1.Rows.Add(
                                row[3].ToString(),
                                row[4].ToString(),
                                row[5].ToString(),
                                row[6].ToString(),
                                row[7].ToString(),
                                row[8].ToString(),
                                row[9].ToString(),
                                row[10].ToString()
                                );
                        }
                        dt.Dispose();
                    }
                }
            }
        }
        public void dataload()                  // jala datos para los combos 
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

        #region limpiadores_modos
        private void sololee()
        {
            lp.sololee(this);
        }
        private void escribe()
        {
            lp.escribe(this);
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
            //
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
            if (tx_numero.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número de la planilla", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tx_numero.Focus();
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
            if (tx_pla_ruc.Text.Trim() == "" || tx_car3ro_ruc.Text == "")
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
                if (tx_dat_estad.Text == codAnul)
                {
                    MessageBox.Show("La planilla esta anulada", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                // debe limpiar los campos y actualizar la grilla
                initIngreso();          // limpiamos todo para volver a empesar
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
                    "brevayuda,rucpropie,tipoplani," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fecho,@serpl,@locor,@locde,@obspl,@cantf,@canto,@pesto,@tipmo,@tipca,@subto," +
                    "@igvpl,@totpl,@totpa,@salxp,@estad,@fleim,@platr,@placa,@autor,@confv,@brevc," +
                    "@breva,@rucpr,@tipop," +
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
                    micon.Parameters.AddWithValue("", tx_pla_nomcho.Text);           // nombre del chofer
                    micon.Parameters.AddWithValue("@breva", tx_pla_ayud.Text);
                    micon.Parameters.AddWithValue("", tx_pla_nomayu.Text);           // nombre del ayudante
                    micon.Parameters.AddWithValue("@rucpr", (tx_pla_ruc.Text.Trim() == "")? tx_car3ro_ruc.Text : tx_pla_ruc.Text);
                    micon.Parameters.AddWithValue("@tipop", vtip);              // tipo planilla, tipo transporte/transportista
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    micon.ExecuteNonQuery();
                }
                using (MySqlCommand micon = new MySqlCommand("select last_insert_id()", conn))
                {
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // numplacar numeracion automatica estilo pre guias
                            tx_idr.Text = dr.GetString(0);
                        }
                    }
                }
                // detalle
                /*
                if (dataGridView1.Rows.Count > 0)
                {
                    int fila = 1;
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                        {

                            string inserd2 = "update detguiai set " +
                                "cantprodi=@can,unimedpro=@uni,codiprodi=@cod,descprodi=@des,pesoprodi=@pes,precprodi=@preu,totaprodi=@pret " +
                                "where idc=@idr and fila=@fila";
                            using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                            {
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@fila", fila);
                                micon.Parameters.AddWithValue("@can", dataGridView1.Rows[i].Cells[0].Value.ToString());
                                micon.Parameters.AddWithValue("@uni", dataGridView1.Rows[i].Cells[1].Value.ToString());
                                micon.Parameters.AddWithValue("@cod", "");
                                micon.Parameters.AddWithValue("@des", gloDeta + dataGridView1.Rows[i].Cells[2].Value.ToString().Trim());
                                micon.Parameters.AddWithValue("@pes", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                micon.Parameters.AddWithValue("@preu", "0");
                                micon.Parameters.AddWithValue("@pret", "0");
                                micon.ExecuteNonQuery();
                                fila += 1;
                                //
                                retorna = true;         // no hubo errores!
                            }
                        }
                    }
                }
                */
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
        private void edita()
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    if (true)     // EDICION DE CABECERA
                    {   // tx_impreso.Text == "N"
                        string actua = "update ??? a set " +
                            "a.fechopegr=@fechop,a.tidodegri=@tdcdes,a.nudodegri=@ndcdes," +
                            "a.nombdegri=@nomdes,a.diredegri=@dircde,a.ubigdegri=@ubicde,a.tidoregri=@tdcrem,a.nudoregri=@ndcrem," + 
                            "a.nombregri=@nomrem,a.direregri=@dircre,a.ubigregri=@ubicre,a.locorigen=@locpgr,a.dirorigen=@dirpgr," +
                            "a.ubiorigen=@ubopgr,a.locdestin=@ldcpgr,a.dirdestin=@didegr,a.ubidestin=@ubdegr,a.docsremit=@dooprg," +
                            "a.obspregri=@obsprg,a.clifingri=@conprg,a.cantotgri=@totcpr,a.pestotgri=@totppr,a.tipmongri=@monppr," +
                            "a.tipcamgri=@tcprgr,a.subtotgri=@subpgr,a.igvgri=@igvpgr,a.totgri=@totpgr,a.totpag=@pagpgr," +
                            "a.salgri=@totpgr,a.estadoser=@estpgr,a.seguroE=@clavse,a.cantfilas=@canfil," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@tcprgr", "0.00");  // tipo de cambio
                        micon.Parameters.AddWithValue("@subpgr", "0"); // sub total de la pre guía
                        micon.Parameters.AddWithValue("@igvpgr", "0"); // igv
                        micon.Parameters.AddWithValue("@pagpgr", "0");
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE 
                        //
                        micon = new MySqlCommand("borraseguro", conn);
                        micon.CommandType = CommandType.StoredProcedure;
                        micon.Parameters.AddWithValue("@tabla", "detguiai");
                        micon.Parameters.AddWithValue("@vidr", "0");
                        micon.Parameters.AddWithValue("@vidc", tx_idr.Text);
                        micon.ExecuteNonQuery();
                        //for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            //if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() != "")
                            {
                                string inserd2 = "insert into detguiai (idc,fila,sergui,numgui," +
                                    "cantprodi,unimedpro,codiprodi,descprodi,pesoprodi,precprodi,totaprodi," +
                                    "estadoser,verApp,userm,fechm,diriplan4,diripwan4,netbname" +
                                    ") values (@idr,@fila,@serpgr,@corpgr," +
                                    "@can,@uni,@cod,@des,@pes,@preu,@pret," +
                                    "@estpgr,@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                                micon = new MySqlCommand(inserd2, conn);
                                micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                                micon.Parameters.AddWithValue("@cod", "");
                                micon.Parameters.AddWithValue("@preu", "0");
                                micon.Parameters.AddWithValue("@pret", "0");
                                micon.Parameters.AddWithValue("@estpgr", tx_dat_estad.Text); // estado de la pre guía
                                micon.Parameters.AddWithValue("@verApp", verapp);
                                micon.Parameters.AddWithValue("@asd", asd);
                                micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                micon.Parameters.AddWithValue("@ipwan", lib.ipwan());
                                micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                micon.ExecuteNonQuery();
                            }
                        }
                        //
                        micon.Dispose();
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar la guía individual");
                    Application.Exit();
                    return;
                }
            }
            else
            {
                MessageBox.Show("No fue posible conectarse al servidor de datos");
                Application.Exit();
                return;
            }
        }
        private void anula()
        {
            // en el caso guias y otros documentos HAY 2: ANULACION FISICA y ANULACION INTERNA (serie ANU)
            // Anulacion fisica se "anula" el numero del documento en sistema y en fisico se tacha, marca anulado 
            // Anulación interna (ANU) el numero se recupera tanto en fisico como en sistema, el anulado internamente pasa a ser serie ANU
            var aa = MessageBox.Show("Anulación interna para recuperar el número?" + Environment.NewLine +
                "Se cambia la serie a ANU", "Atención, confirme por favor",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            string parte = " ";
            if (aa == DialogResult.OK) parte = ",sergui=@coad ";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabguiai set estadoser=@estser,usera=@asd,fecha=now()," +
                        "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp" + parte +
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
                        if (aa == DialogResult.OK) micon.Parameters.AddWithValue("@coad", v_sanu);
                        micon.ExecuteNonQuery();
                    }
                }
            }
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
            // local usa o no pre-guias
            DataRow[] fila = dtu.Select("idcodice='" + v_clu + "'");
            if(fila.Length > 0)
            {
                if (fila[0][3].ToString() == "1")
                {
                    sololee();
                    initIngreso();  // limpiamos/preparamos todo para el ingreso

                }
                if (fila[0][3].ToString() == "0")
                {
                    escribe();
                    initIngreso();  // limpiamos/preparamos todo para el ingreso
                }
            }
            // Guía va con flete impreso?
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            escribe();
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            //
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
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
            // ...
        #endregion comboboxes

        #region datagridview
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

        }
        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

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
    }
}
