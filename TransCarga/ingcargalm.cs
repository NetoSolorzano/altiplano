using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class ingcargalm : Form
    {
        static string nomform = "ingcargalm";           // nombre del formulario
        string colback = TransCarga.Program.colbac;     // color de fondo
        string colpage = TransCarga.Program.colpag;     // color de los pageframes
        string colgrid = TransCarga.Program.colgri;     // color de las grillas
        string colfogr = TransCarga.Program.colfog;     // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;     // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;     // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;     // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabingalm";

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
        string codGene = "";            // codigo documento generado
        string codIngA = "";            // codigo documento recepcionado en almacen
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
        //string v_mfildet = "";          // maximo numero de filas en el detalle, coord. con el formato
        string v_trompa = "";           // codigo interno placa de tracto
        string v_carret = "";           // código interno placa de carreta/furgon
        string v_camion = "";           // código interno placa de camion
        string v_mondef = "";           // moneda por defecto del form
        string vint_A0 = "";            // variable INTERNA para amarrar el codigo anulacion cliente con A0
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
        string[] retorD = { "", "", "", "", "", "", "", "", "", "", "" };      // datos devueltos de busqueda de planlla y GR

        public ingcargalm()
        {
            InitializeComponent();
        }
        private void ingcargalm_KeyDown(object sender, KeyEventArgs e)
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
            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void ingcargalm_Load(object sender, EventArgs e)
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
            tx_serP.MaxLength = 4;
            tx_numP.MaxLength = 8;
            tx_pla_placa.MaxLength = 7;
            tx_pla_carret.MaxLength = 7;
            tx_pla_brevet.MaxLength = 10;
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
            dataGridView1.ColumnCount = 9;
            dataGridView1.Columns[0].Name = "fila";
            dataGridView1.Columns[0].HeaderText = "Fila";
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Name = "serguia";
            dataGridView1.Columns[1].HeaderText = "Ser.GR";
            dataGridView1.Columns[1].ReadOnly = false;
            dataGridView1.Columns[1].Width = 40;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Name = "numguia";
            dataGridView1.Columns[2].HeaderText = "Num.GR";
            dataGridView1.Columns[2].ReadOnly = false;
            dataGridView1.Columns[2].Width = 60;
            dataGridView1.Columns[3].Name = "totcant";
            dataGridView1.Columns[3].HeaderText = "Cant.Bul.";
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[3].Width = 30;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].Name = "nombul";
            dataGridView1.Columns[4].HeaderText = "Embalaje";
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[4].Width = 70;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].Name = "totpeso";
            dataGridView1.Columns[5].HeaderText = "Kgs.";
            dataGridView1.Columns[5].ReadOnly = true;
            dataGridView1.Columns[5].Width = 40;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].Name = "descrip";
            dataGridView1.Columns[6].HeaderText = "Descripcion";
            dataGridView1.Columns[6].ReadOnly = true;
            dataGridView1.Columns[6].Width = 200;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[7].Name = "estado";
            dataGridView1.Columns[7].HeaderText = "Estado";
            dataGridView1.Columns[7].ReadOnly = true;
            dataGridView1.Columns[7].Width = 60;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].Name = "codest";
            dataGridView1.Columns[8].HeaderText = "codest";
            dataGridView1.Columns[8].Visible = false;
            DataGridViewCheckBoxColumn marca = new DataGridViewCheckBoxColumn();
            marca.Name = "Rok";
            marca.HeaderText = "Recibido";
            marca.Width = 50;
            marca.ReadOnly = false;
            marca.FillWeight = 20;
            dataGridView1.Columns.Add(marca);
            DataGridViewTextBoxColumn tobs = new DataGridViewTextBoxColumn();
            tobs.Name = "Orecep";
            tobs.HeaderText = "Obs.Recepción";
            tobs.Width = 180;
            tobs.ReadOnly = false;
            dataGridView1.Columns.Add(tobs);
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
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofa)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
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
                            if (row["param"].ToString() == "estplarecep") codIngA = row["valor"].ToString().Trim();           // estado planilla ingresada a alm.
                        }
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
        private void jalaoc(string campo)        // jala ingreso 
        {
            {
                string parte = "";
                if (campo == "tx_idr")
                {
                    parte = "where a.id=@ida";
                }
                if (campo == "sernum")
                {
                    parte = "where a.serdocuin=@ser and a.numdocuin=@num";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "";
                    consulta = "select a.id,a.fechope,a.tipoingre,a.serdocuin,a.numdocuin,a.locorigen,a.locdestin,a.platracto,a.placarret,a.brevchofe,a.cantfilas," +
                        "a.cantotpla,a.pestotpla,a.obsplacar,a.estadoser,ifnull(b.nomchofe,'') as nomchofe,o.descrizionerid as nomorig,d.descrizionerid as nomdest," +
                        "a.userc,a.fechc,a.userm,a.fechm,a.usera,a.fecha," +
                        "ifnull(o2.descrizionerid,'') as nomorig,ifnull(d2.descrizionerid,'') as nomdesg,ifnull(g.locorigen,'') as codlor,ifnull(g.locdestin,'') as codlde " +
                        "FROM cabingalm a left join (select brevchofe,nomchofe from cabplacar group by upper(brevchofe)) b on b.brevchofe=a.brevchofe " +
                        "left join desc_loc o on o.idcodice=a.locorigen left join desc_loc d on d.idcodice=a.locdestin " +
                        "left join cabguiai g on g.sergui=a.serdocuin and g.numgui=a.numdocuin and a.tipoingre='G' " +
                        "left join desc_loc o2 on o2.idcodice=g.locorigen left join desc_loc d2 on d2.idcodice=a.locdestin " +
                        parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    if (campo == "tx_idr") micon.Parameters.AddWithValue("@ida", tx_idr.Text);
                    if (campo == "sernum")
                    {
                        micon.Parameters.AddWithValue("@ser", (rb_plani.Checked == true)? tx_serP.Text : tx_serGR.Text);
                        micon.Parameters.AddWithValue("@num", (rb_plani.Checked == true)? tx_numP.Text : tx_numGR.Text);
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
                            tx_dat_estad.Text = dr.GetString("estadoser");
                            if (dr.GetString("tipoingre") == "P")
                            {
                                rb_plani.Checked = true;
                                tx_serP.Text = dr.GetString("serdocuin");
                                tx_numP.Text = dr.GetString("numdocuin");
                                tx_origen.Text = dr.GetString("nomorig");
                                tx_destino.Text = dr.GetString("nomdest");
                                tx_pla_placa.Text = dr.GetString("platracto");
                                tx_pla_brevet.Text = dr.GetString("brevchofe");
                                tx_pla_carret.Text = dr.GetString("placarret");
                                tx_pla_nomcho.Text = dr.GetString("nomchofe");
                                tx_dat_orig.Text = dr.GetString("locorigen");
                                tx_dat_dest.Text = dr.GetString("locdestin");
                                //tx_dat_idplan.Text = dr.GetString("");
                            }
                            if (dr.GetString("tipoingre") == "G")
                            {
                                rb_manual.Checked = true;
                                tx_serGR.Text = dr.GetString("serdocuin");
                                tx_numGR.Text = dr.GetString("numdocuin");
                                tx_origen.Text = dr.GetString("nomorig");
                                tx_destino.Text = dr.GetString("nomdesg");
                                tx_dat_orig.Text = dr.GetString("codlor");
                                tx_dat_dest.Text = dr.GetString("codlde");
                            }
                            tx_obser1.Text = dr.GetString("obsplacar");
                            tx_tfil.Text = dr.GetString("cantfilas");
                            tx_totcant.Text = dr.GetString("cantotpla");
                            tx_totpes.Text = dr.GetString("pestotpla");
                        }
                        tx_estado.Text = lib.nomstat(tx_dat_estad.Text);
                        // si el documento esta ANULADO o un estado que no permite EDICION, se pone todo en sololee (ANULADO O RECIBIDO)
                        if (tx_dat_estad.Text != codGene)
                        {
                            sololee();
                            dataGridView1.ReadOnly = true;
                            MessageBox.Show("Este documento no puede ser editado/anulado", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {

                        }
                        button1.Enabled = true;
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
        private void jaladet(string idr)         // jala el detalle del ingreso
        {
            string jalad = "";
            if (rb_plani.Checked == true)
            {
                jalad = "select a.idrecep,a.serplacar,a.numplacar,a.fila,a.serguia,a.numguia,a.totcant,floor(a.totpeso) as totpeso," +
                    "e.descrizionerid,g.estadoser,'X' as marca,a.id,a.nombult,g.descprodi,a.marcaR,a.obsrecep " +
                    "from detplacar a " +
                    "left join detguiai g on g.sergui = a.serguia and g.numgui = a.numguia " +
                    "left join desc_est e on e.idcodice=a.estadoser " +
                    "where a.idrecep=@idr";
            }
            if (rb_manual.Checked == true)  // jala guia individual
            {
                jalad = "SELECT b.fechopegr,b.locorigen,b.locdestin,'1',a.sergui,a.numgui,a.cantprodi,a.pesoprodi,e.descrizionerid,b.estadoser,space(1) as esp1," +
                    "space(1) as esp2,a.unimedpro,a.descprodi,'S',b.obsrecep " +
                    "FROM detguiai a LEFT JOIN cabguiai b ON b.id=a.idc " +
                    "left join desc_est e on e.idcodice=a.estadoser " +
                    "left join cabalmac c on c.gremtra=concat(a.sergui,a.numgui) " +
                    "left join desc_loc l on l.idcodice=c.almacen " +
                    "WHERE a.sergui = @serg AND a.numgui = @numg";
            }
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                using (MySqlCommand micon = new MySqlCommand(jalad, conn))
                {
                    if (rb_plani.Checked == true) micon.Parameters.AddWithValue("@idr", idr);
                    if (rb_manual.Checked == true)
                    {
                        micon.Parameters.AddWithValue("@serg", tx_serGR.Text);
                        micon.Parameters.AddWithValue("@numg", tx_numGR.Text);
                    }
                    using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.Rows.Clear();
                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridView1.Rows.Add(
                                row[3].ToString(),
                                row[4].ToString(),
                                row[5].ToString(),
                                row[6].ToString(),
                                row[12].ToString(),
                                row[7].ToString(),
                                row[13].ToString(),
                                row[8].ToString(),
                                row[9].ToString(),
                                (row[14].ToString() == "S")? true : false,
                                row[15].ToString()
                                );
                        }
                        dt.Dispose();
                    }
                }
            }
            operaciones();
        }
        private void dataload()                  // jala datos para los combos 
        {
            /*
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                MessageBox.Show("No se pudo conectar con el servidor", "Error de conexión");
                Application.Exit();
                return;
            }
            conn.Close();
            */
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (codIngA == "")          // codigo documento INGRESADO AL ALMACEN
            {
                lib.messagebox("Código de planilla INGRESADA");
                retorna = false;
            }
            if (v_clu == "")            // codigo del local del usuario
            {
                lib.messagebox("Código local del usuario");
                retorna = false;
            }
            if (vint_A0 == "")
            {
                lib.messagebox("Cód. Interno enlace Anulado: A0");
                retorna = false;
            }
            return retorna;
        }
        private string[] ValPlaCarr(string pc,string codigo)    // pc=G ó P, codigo=serie+numero
        {
            retorD[0] = ""; retorD[1] = ""; retorD[2] = ""; retorD[3] = ""; retorD[4] = ""; retorD[5] = ""; retorD[6] = ""; 
            retorD[7] = ""; retorD[8] = ""; retorD[9] = ""; retorD[10] = "";
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                string consulta = "";
                if (pc == "P")
                { 
                    consulta = "select b1.descrizione,b2.descrizione,a.platracto,a.placarret,a.brevchofe,a.nomchofe,a.estadoser,space(1) as estalma," +
                        "a.locorigen,a.locdestin,a.id " + 
                        "from cabplacar a left join desc_loc b1 on b1.idcodice=a.locorigen left join desc_loc b2 on b2.idcodice=a.locdestin " +
                        "where concat(a.serplacar,a.numplacar)=@codigo";
                }
                if (pc == "G")
                {
                    consulta = "select b1.descrizione,b2.descrizione,a.plaplagri,a.plaplar2,a.breplagri,space(1 ) as nomchofe,a.estadoser,d.estalma," +
                        "a.locorigen,a.locdestin,'0' " +
                        "from cabguiai a left join desc_loc b1 on b1.idcodice=a.locorigen left join desc_loc b2 on b2.idcodice=a.locdestin " +
                        "left join controlg d on d.serguitra=a.sergui and d.numguitra=a.numgui " +
                        "where concat(a.sergui,a.numgui)=@codigo";
                }
                using (MySqlCommand micon = new MySqlCommand(consulta,conn))
                {
                    micon.Parameters.AddWithValue("@codigo", codigo);
                    MySqlDataReader dr = micon.ExecuteReader();
                    while (dr.Read())
                    {
                        retorD[0] = dr.GetString(0);   // origen
                        retorD[1] = dr.GetString(1);   // destino
                        retorD[2] = dr.GetString(2);   // placa
                        retorD[3] = dr.GetString(3);   // carreta
                        retorD[4] = dr.GetString(4);   // brevete
                        retorD[5] = dr.GetString(5);   // nombre chofer
                        retorD[6] = dr.GetString(6);   // estado documento
                        retorD[7] = dr.GetString(7);   // estado almacen
                        retorD[8] = dr.GetString(8);   // codigo origen
                        retorD[9] = dr.GetString(9);   // codigo destino
                        retorD[10] = dr.GetString(10); // id planilla de carga
                    }
                    dr.Dispose();
                }
            }
            return retorD;
        }
        private void operaciones()              // recalcula los totales de la grilla
        {
            int totfil = 0;
            int totcant = 0;
            decimal totpes = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[9].Value != null && dataGridView1.Rows[i].Cells[9].Value.ToString() == "True")
                {
                    if (dataGridView1.Rows[i].Cells[3].Value != null)
                    {
                        totcant = totcant + int.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString());
                        totfil += 1;
                    }
                    if (dataGridView1.Rows[i].Cells[5].Value != null)
                    {
                        totpes = totpes + decimal.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString());
                    }
                }
            }
            tx_filas.Text = dataGridView1.Rows.Count.ToString();
            tx_totcant.Text = totcant.ToString();
            tx_totpes.Text = totpes.ToString("0.00");
            tx_tfil.Text = totfil.ToString();
            dataGridView1.AllowUserToAddRows = true;
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

        }
        private void limpia_combos()
        {
            lp.limpia_cmb(this);
        }
        #endregion limpiadores_modos;

        #region boton_form GRABA EDITA ANULA
        private void bt_Agr_Click(object sender, EventArgs e)
        {
            if (rb_plani.Checked == true && (tx_serP.Text.Trim() == "" || tx_numP.Text.Trim() == ""))
            {
                MessageBox.Show("Ingrese correctamente la planilla de carga", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_serP.Focus();
                return;
            }
            if (rb_manual.Checked == true && (tx_serGR.Text.Trim() == "" || tx_numGR.Text.Trim() == ""))
            {
                MessageBox.Show("Ingrese correctamente la guía de remisión", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tx_serGR.Focus();
                return;
            }
            if (rb_plani.Checked == true && tx_numP.Text.Trim() != "" && tx_serP.Text.Trim() != "")
            {
                if (tx_dat_dest.Text != v_clu)
                {
                    MessageBox.Show("La planilla de carga tiene destino diferente","Error en Almacén",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
            }
            if (rb_manual.Checked == true && tx_numGR.Text.Trim() != "" && tx_serGR.Text.Trim() != "")
            {
                if (tx_dat_dest.Text != v_clu)
                {
                    var aa= MessageBox.Show("La Guía tiene destino diferente" + Environment.NewLine +
                        "Desea ingresarlo de todas formas?", "Error en Almacén", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.No)
                    {
                        return;
                    }
                }
            }
            if (tx_serP.Text.Trim() != "")
            {
                // jalamos igual que edicion de planillas de carga
                string jalad = "select a.idc,a.serplacar,a.numplacar,a.fila,a.serguia,a.numguia,a.totcant,floor(a.totpeso) as totpeso," +
                "a.estadoser,'X' as marca,a.id,a.nombult,g.descprodi,e.descrizionerid,g.estadoser,c.iding,l.descrizionerid,c.fecingalm " +
                "from detplacar a " +
                "left join detguiai g on g.sergui = a.serguia and g.numgui = a.numguia " +
                "left join desc_est e on e.idcodice=g.estadoser " +
                "left join cabalmac c on c.idplan=a.idc " +
                "left join desc_loc l on l.idcodice=c.almacen " +
                "where a.serplacar=@serp and a.numplacar=@nump";
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    using (MySqlCommand micon = new MySqlCommand(jalad, conn))
                    {
                        micon.Parameters.AddWithValue("@serp", tx_serP.Text);
                        micon.Parameters.AddWithValue("@nump", tx_numP.Text);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.Rows.Clear();
                            if (dt.Rows[0].ItemArray[15].ToString().Trim() == "")
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    dataGridView1.Rows.Add(
                                        row[3].ToString(),
                                        row[4].ToString(),
                                        row[5].ToString(),
                                        row[6].ToString(),
                                        row[11].ToString(),
                                        row[7].ToString(),
                                        row[12].ToString(),
                                        row[13].ToString(),
                                        row[14].ToString(),
                                        true
                                        );
                                }
                            }
                            else
                            {
                                MessageBox.Show("La planilla de carga ya esta ingresada en el álmacen" + Environment.NewLine +
                                       dt.Rows[0].ItemArray[16].ToString() + " con fecha " + dt.Rows[0].ItemArray[17].ToString().Substring(0,10));
                            }
                            dt.Dispose();
                        }
                    }
                }
            }
            if (tx_serGR.Text.Trim() != "")
            {
                string jalag = "SELECT a.idc,a.sergui,a.numgui,b.fechopegr,b.locorigen,b.locdestin,a.cantprodi,a.unimedpro,a.codiprodi,a.descprodi,a.pesoprodi," +
                    "e.descrizionerid,b.estadoser,c.iding,l.descrizionerid,c.fecingalm " +
                    "FROM detguiai a LEFT JOIN cabguiai b ON b.id=a.idc " +
                    "left join desc_est e on e.idcodice=a.estadoser " +
                    "left join cabalmac c on c.gremtra=concat(a.sergui,a.numgui) " +
                    "left join desc_loc l on l.idcodice=c.almacen " +
                    "WHERE a.sergui = @serg AND a.numgui = @numg";
                using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
                {
                    conn.Open();
                    using (MySqlCommand micon = new MySqlCommand(jalag, conn))
                    {
                        micon.Parameters.AddWithValue("@serg", tx_serGR.Text);
                        micon.Parameters.AddWithValue("@numg", tx_numGR.Text);
                        using (MySqlDataAdapter da = new MySqlDataAdapter(micon))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.Rows.Clear();
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row[13].ToString().Trim() == "")
                                {
                                    dataGridView1.Rows.Add(
                                        "1",
                                        row[1].ToString(),
                                        row[2].ToString(),
                                        row[6].ToString(),
                                        row[7].ToString(),
                                        row[10].ToString(),
                                        row[9].ToString(),
                                        row[11].ToString(),
                                        row[12].ToString(),
                                        true
                                        );
                                }
                                else
                                {
                                    MessageBox.Show("La guía ya esta ingresada en el álmacen" + Environment.NewLine + 
                                        row[14].ToString() + " con fecha " + row[15].ToString().Substring(0,10));
                                }
                            }
                            dt.Dispose();
                        }
                    }
                }
            }
            operaciones();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            #region validaciones
            if (tx_tfil.Text == "0" || tx_tfil.Text.Trim() == "")   // tx_serP.Text.Trim() == ""
            {
                MessageBox.Show("Ingrese los datos de planilla o guía", "Complete la información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tx_serP.Focus();
                return;
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
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el ingreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (graba() == true)
                        {
                            /*
                            var bb = MessageBox.Show("Desea imprimir la planilla?" + Environment.NewLine +
                                "El formato actual es " + vi_formato, "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (bb == DialogResult.Yes)
                            {
                                Bt_print.PerformClick();
                            }
                            */
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
                if (true)   // de momento no validamos mas
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar el ingreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                if (tx_dat_estad.Text != codAnul)
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea ANULAR el ingreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                int vdif = int.Parse(tx_filas.Text) - int.Parse(tx_tfil.Text);
                string inserta = "insert into cabingalm (" +
                    "fechope,tipoingre,serdocuin,numdocuin,locorigen,locdestin,platracto,placarret,brevchofe,cantfilas,cantotpla,pestotpla,obsplacar," +
                    "estadoser,almacen,obsllega," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                    "values (@fecho,@tipin,@serpl,@numpl,@locor,@locde,@pltra,@plcar,@brech,@cantf,@canto,@pesto,@obspl," +
                    "@estse,@almaR,@obsll," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@fecho", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@tipin", (rb_plani.Checked == true)? "P" : "G");
                    micon.Parameters.AddWithValue("@serpl", (rb_plani.Checked == true) ? tx_serP.Text : tx_serGR.Text);
                    micon.Parameters.AddWithValue("@numpl", (rb_plani.Checked == true) ? tx_numP.Text : tx_numGR.Text);
                    micon.Parameters.AddWithValue("@locor", tx_dat_orig.Text);
                    micon.Parameters.AddWithValue("@locde", tx_dat_dest.Text);
                    micon.Parameters.AddWithValue("@pltra", tx_pla_placa.Text);
                    micon.Parameters.AddWithValue("@plcar", tx_pla_carret.Text);
                    micon.Parameters.AddWithValue("@brech", tx_pla_brevet.Text);
                    micon.Parameters.AddWithValue("@cantf", tx_tfil.Text);      // cantidad filas detalle
                    micon.Parameters.AddWithValue("@canto", tx_totcant.Text);   // cant total de bultos
                    micon.Parameters.AddWithValue("@pesto", tx_totpes.Text);    // peso total
                    micon.Parameters.AddWithValue("@obspl", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@estse", tx_dat_estad.Text);
                    micon.Parameters.AddWithValue("@almaR", v_clu);
                    micon.Parameters.AddWithValue("@obsll", (vdif == 0)? "0" : "1");
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
                                string inserd2 = "insert into cabalmac (iding,almacen,fecingalm,tipingalm,idplan,locorigen,locdestin," +
                                    "preguia,gremtra,estadgrt,cantbul,fleteMN,pesokgr,nombult,descrip,comIng,estalma," +
                                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) " +
                                    "values (@idin,@codalm,@fecho,@orire,@idori,@locor,@locde," +
                                    "@numpr,@senug,@estgr,@totca,@totfl,@totpe,@nombu,@descr,@codmo,@estad," +
                                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                                using (MySqlCommand micon = new MySqlCommand(inserd2, conn))
                                {
                                    micon.Parameters.AddWithValue("@idin", tx_idr.Text);
                                    micon.Parameters.AddWithValue("@codalm", v_clu);
                                    micon.Parameters.AddWithValue("@fecho", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                                    micon.Parameters.AddWithValue("@orire", (rb_plani.Checked == true) ? "P" : "G");
                                    micon.Parameters.AddWithValue("@idori", (rb_plani.Checked == true) ? tx_dat_idplan.Text : "0");
                                    micon.Parameters.AddWithValue("@locor", tx_dat_orig.Text);
                                    micon.Parameters.AddWithValue("@locde", tx_dat_dest.Text);
                                    micon.Parameters.AddWithValue("@numpr", "");
                                    micon.Parameters.AddWithValue("@senug", dataGridView1.Rows[i].Cells[1].Value.ToString() + dataGridView1.Rows[i].Cells[2].Value.ToString());
                                    micon.Parameters.AddWithValue("@estgr", dataGridView1.Rows[i].Cells[8].Value.ToString());
                                    micon.Parameters.AddWithValue("@totca", dataGridView1.Rows[i].Cells[3].Value.ToString());
                                    micon.Parameters.AddWithValue("@totpe", dataGridView1.Rows[i].Cells[5].Value.ToString());
                                    micon.Parameters.AddWithValue("@totfl", 0);
                                    micon.Parameters.AddWithValue("@nombu", dataGridView1.Rows[i].Cells[4].Value.ToString());
                                    micon.Parameters.AddWithValue("@descr", dataGridView1.Rows[i].Cells[6].Value.ToString());
                                    micon.Parameters.AddWithValue("@codmo", (dataGridView1.Rows[i].Cells[10].Value == null) ? "" : dataGridView1.Rows[i].Cells[10].Value.ToString());
                                    micon.Parameters.AddWithValue("@estad", codIngA);
                                    micon.Parameters.AddWithValue("@verApp", verapp);
                                    micon.Parameters.AddWithValue("@asd", asd);
                                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                                    micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                                    //
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
                    if (tx_dat_estad.Text == codGene)
                    {
                        string actua = "update cabingalm set obsplacar=@obspl," +
                            "verApp=@verApp,userm=@asd,fechm=now(),diriplan4=@iplan,diripwan4=@ipwan,netbname=@nbnam " +
                            "where id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@obspl", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        retorna = true;
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error en modificar ingreso");
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
            // .... NO VA ...
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update ?? set estadoser=@estser,usera=@asd,fecha=now()," +
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
            if (Tx_modo.Text != "NUEVO" && tx_idr.Text != "")
            {
                jalaoc("tx_idr");
                jaladet(tx_idr.Text);
            }
        }
        private void rb_plani_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_plani.Checked == true)
            {
                tx_serP.ReadOnly = false;
                tx_numP.ReadOnly = false;
                tx_serGR.ReadOnly = true;
                tx_serGR.Text = "";
                tx_numGR.ReadOnly = true;
                tx_numGR.Text = "";
                tx_serP.Focus();
            }
        }
        private void rb_manual_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_manual.Checked == true)
            {
                tx_serP.ReadOnly = true;
                tx_serP.Text = "";
                tx_numP.ReadOnly = true;
                tx_numP.Text = "";
                tx_serGR.ReadOnly = false;
                tx_numGR.ReadOnly = false;
                tx_serGR.Focus();
            }
        }
        private void tx_serP_Leave(object sender, EventArgs e)
        {
            if (tx_serP.Text.Trim() != "") tx_serP.Text = lib.Right("0000" + tx_serP.Text.Trim(),4);
        }
        private void tx_numP_Leave(object sender, EventArgs e)
        {
            if (tx_numP.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                tx_numP.Text = lib.Right("00000000" + tx_numP.Text.Trim(), 8);
                ValPlaCarr("P",tx_serP.Text + tx_numP.Text);
                if (retorD[6].ToString() == codAnul || retorD[6].ToString() == codIngA || retorD[6].ToString() == codGene)
                {
                    MessageBox.Show("Planilla de carga esta Abierta, Anulada o Recibida","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    tx_origen.Text = retorD[0];
                    tx_destino.Text = retorD[1];
                    tx_pla_placa.Text = retorD[2];
                    tx_pla_carret.Text = retorD[3];
                    tx_pla_brevet.Text = retorD[4];
                    tx_pla_nomcho.Text = retorD[5];
                    tx_dat_orig.Text = retorD[8];
                    tx_dat_dest.Text = retorD[9];
                    tx_dat_idplan.Text = retorD[10];
                    bt_agr.Focus();
                }
            }
            if (tx_numP.Text.Trim() != "" && Tx_modo.Text != "NUEVO")
            {
                tx_numP.Text = lib.Right("00000000" + tx_numP.Text.Trim(), 8);
                jalaoc("sernum");
                jaladet(tx_idr.Text);
            }
        }
        private void tx_serGR_Leave(object sender, EventArgs e)
        {
            if (tx_serGR.Text.Trim() != "") tx_serGR.Text = lib.Right("0000" + tx_serGR.Text.Trim(), 4);
        }
        private void tx_numGR_Leave(object sender, EventArgs e)
        {
            if (tx_numGR.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                tx_numGR.Text = lib.Right("00000000" + tx_numGR.Text.Trim(), 8);
                ValPlaCarr("G", tx_serGR.Text + tx_numGR.Text);
                if (retorD[6].ToString() == codAnul || retorD[7].ToString() == codIngA)
                {
                    MessageBox.Show("La Guía se encuentra Anulada o ya fue ingresada","Atención",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                else
                {
                    tx_origen.Text = retorD[0];
                    tx_destino.Text = retorD[1];
                    tx_pla_placa.Text = retorD[2];
                    tx_pla_carret.Text = retorD[3];
                    tx_pla_brevet.Text = retorD[4];
                    tx_pla_nomcho.Text = retorD[5];
                    tx_dat_orig.Text = retorD[8];
                    tx_dat_dest.Text = retorD[9];
                    tx_dat_idplan.Text = retorD[10];
                    bt_agr.Focus();
                }
            }
            if (tx_numGR.Text.Trim() != "" && Tx_modo.Text != "NUEVO")
            {
                tx_numGR.Text = lib.Right("00000000" + tx_numGR.Text.Trim(), 8);
                jalaoc("sernum");
                jaladet(tx_idr.Text);
            }
        }
        #endregion

        #region botones_de_comando
        public void toolboton()
        {
            Bt_add.Visible = false;
            Bt_edit.Visible = false;
            Bt_anul.Visible = false;
            Bt_ver.Visible = false;
            Bt_print.Visible = false;
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
                if (Convert.ToString(row["btn1"]) == "S") Bt_add.Visible = true;
                else Bt_add.Visible = false;
                if (Convert.ToString(row["btn2"]) == "S") Bt_edit.Visible = true;
                else Bt_edit.Visible = false;
                if (Convert.ToString(row["btn3"]) == "S") Bt_anul.Visible = true;
                else Bt_anul.Visible = false;
                if (Convert.ToString(row["btn4"]) == "S") Bt_ver.Visible = true;
                else Bt_ver.Visible = false;
                if (Convert.ToString(row["btn5"]) == "S") Bt_print.Visible = true;
                else Bt_print.Visible = false;
                if (Convert.ToString(row["btn6"]) == "S") Bt_close.Visible = true;
                else Bt_close.Visible = false;
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
            button1.Enabled = true;
            initIngreso();
            sololee(); //escribe();
            tx_serP.Text = "";
            tx_numP.Text = "";
            tx_serGR.Text = "";
            tx_numGR.Text = "";
            tx_serP.ReadOnly = true;
            tx_numP.ReadOnly = true;
            tx_serGR.ReadOnly = true;
            tx_numGR.ReadOnly = true;
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            tx_tfil.Text = "0";
            tx_totcant.Text = "0";
            tx_totpes.Text = "0";
            rb_plani.Focus();
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
            sololee();
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            rb_plani.Focus();
        }
        private void Bt_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Bt_print_Click(object sender, EventArgs e)
        {
            // Impresion ó Re-impresion ??
            //if (tx_impreso.Text == "S")
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            /*
            sololee();
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            Bt_ini.Enabled = true;
            Bt_sig.Enabled = true;
            Bt_ret.Enabled = true;
            Bt_fin.Enabled = true;
            //
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            initIngreso();
            tx_serP.ReadOnly = false;
            tx_numP.ReadOnly = false;
            tx_serP.Focus();
            */
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
            //
            tx_serP.Text = "";
            tx_numP.Text = "";
            tx_serGR.Text = "";
            tx_numGR.Text = "";
            tx_serP.ReadOnly = true;
            tx_numP.ReadOnly = true;
            tx_serGR.ReadOnly = true;
            tx_numGR.ReadOnly = true;
            tx_tfil.Text = "0";
            tx_totcant.Text = "0";
            tx_totpes.Text = "0";
            rb_plani.Focus();
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
        #endregion botones_de_comando  ;

        #region comboboxes

        #endregion comboboxes

        #region impresion
            //
        #endregion

        #region crystal
        private void llenaDataSet()
        {
            //
        }
        private conClie generaReporte()
        {
            conClie PlaniC = new conClie();
            //
            return PlaniC;
        }
        #endregion

        #region datagridview
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 7 && e.FormattedValue.ToString() == "False")
            {
                operaciones();
            }
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            //
        }
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            operaciones();
        }
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //
        }
        #endregion

    }
}
