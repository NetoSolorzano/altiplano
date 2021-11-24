using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TransCarga
{
    public partial class ingresosv : Form
    {
        static string nomform = "ingresosv";             // nombre del formulario
        string colback = TransCarga.Program.colbac;   // color de fondo
        string colpage = TransCarga.Program.colpag;   // color de los pageframes
        string colgrid = TransCarga.Program.colgri;   // color de las grillas
        string colfogr = TransCarga.Program.colfog;   // color fondo con grillas
        string colsfon = TransCarga.Program.colsbg;   // color fondo seleccion
        string colsfgr = TransCarga.Program.colsfc;   // color seleccion grilla
        string colstrp = TransCarga.Program.colstr;   // color del strip
        bool conectS = TransCarga.Program.vg_conSol;    // usa conector solorsoft? true=si; false=no
        static string nomtab = "cabingresosv";              // cabecera de guias INDIVIDUALES

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
        string MonDeft = "";            // moneda por defecto
        string v_clu = "";              // codigo del local del usuario
        string v_slu = "";              // serie del local del usuario
        string v_nbu = "";              // nombre del usuario
        string v_codsv = "";            // codigos tipo validos para ingresos varios
        string v_ctpe = "";             // codigo tipo de ingreso efectivo
        string v_codd = "";             // codigo tipo documento - deposito en cuenta propia
        string v_nodd = "";             // siglas del documento - deposito en cuenta propia
        string vint_A0 = "";            // variable codigo anulacion interna por BD
        string v_igv = "";              // valor igv %
        string v_estcaj = "";           // estado de la caja
        string v_idcaj = "";            // id de la caja
        string codAbie = "";            // codigo estado de caja abierta
        //
        static libreria lib = new libreria();   // libreria de procedimientos
        publico lp = new publico();             // libreria de clases
        string verapp = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion;
        string nomclie = Program.cliente;           // cliente usuario del sistema
        string rucclie = Program.ruc;               // ruc del cliente usuario del sistema
        string asd = TransCarga.Program.vg_user;    // usuario conectado al sistema
        string dirloc = TransCarga.Program.vg_duse; // direccion completa del local usuario conectado
        string ubiloc = TransCarga.Program.vg_uuse; // ubigeo local del usuario conectado
        #endregion

        // string de conexion
        string DB_CONN_STR = "server=" + login.serv + ";uid=" + login.usua + ";pwd=" + login.cont + ";database=" + login.data + ";";

        DataTable dtm = new DataTable();        // combo moneda
        DataTable dtmpa = new DataTable();      // medio de pago del ingreso
        DataTable dtcom = new DataTable();      // combo documento de compra
        DataTable dtctb = new DataTable();      // cuentas bancarias propias
        public ingresosv()
        {
            InitializeComponent();
        }
        private void ingresosv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{TAB}");
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.N) Bt_add.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.E) Bt_edit.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A) Bt_anul.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.O) Bt_ver.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.P) Bt_print.PerformClick();
            if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.S) Bt_close.PerformClick();
        }
        private void ingresosv_Load(object sender, EventArgs e)
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
            this.Focus();
            jalainfo();
            init();
            dataload();
            toolboton();
            this.KeyPreview = true;
            if (valiVars() == false)
            {
                //Application.Exit();
                //return;
            }
        }
        private void init()
        {
            this.BackColor = Color.FromName(colback);
            toolStrip1.BackColor = Color.FromName(colstrp);
            dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.BackColor = Color.FromName(colgrid);
            //dataGridView1.DefaultCellStyle.ForeColor = Color.FromName(colfogr);
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromName(colsfon);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromName(colsfgr);
            //
            tx_user.Text += asd;
            tx_nomuser.Text = TransCarga.Program.vg_nuse;
            tx_locuser.Text = tx_locuser.Text + " " + TransCarga.Program.vg_nlus; // TransCarga.Program.vg_luse;
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
            tx_serGR.MaxLength = 4;         // serie doc comprobante
            tx_numGR.MaxLength = 8;         // numero doc comprobante
            tx_glosa.MaxLength = 90;        // num.operacion, banco, etc.
            tx_obser1.MaxLength = 245;      // observaciones
            // grilla
            dataGridView1.ReadOnly = true;
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;    // tipo moneda
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;     // monto egresado
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;     // monto en MN
            // todo desabilidado
            sololee();
        }
        private void initIngreso()
        {
            limpiar();
            limpia_chk();
            limpia_otros();
            limpia_combos();
            dataGridView1.Rows.Clear();
            dataGridView1.ReadOnly = true;
            tx_idcaja.ReadOnly = true;
            if (Tx_modo.Text == "NUEVO")
            {
                tx_serGR.Text = v_slu;
                cmb_comp.SelectedIndex = 0;
                tx_dat_mp.Text = dtmpa.Rows[0].ItemArray[0].ToString();
                cmb_mpago.SelectedIndex = 0;    // primer registro predeterminado
            }
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
                string consulta = "select formulario,campo,param,valor from enlaces where formulario in (@nofo,@nfin,@nofa,@nofi)";
                MySqlCommand micon = new MySqlCommand(consulta, conn);
                micon.Parameters.AddWithValue("@nofo", "main");
                micon.Parameters.AddWithValue("@nfin", "interno");
                micon.Parameters.AddWithValue("@nofi", "ayccaja");
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
                            if (row["param"].ToString() == "coddep") v_codd = row["valor"].ToString().Trim();               // codigo tipo depositos
                            if (row["param"].ToString() == "nomdep") v_nodd = row["valor"].ToString().Trim();               // nombre codido depositos
                            if (row["param"].ToString() == "docsval") v_codsv = row["valor"].ToString().Trim();             // tipos validos para ingresos varios
                            if (row["param"].ToString() == "codingef") v_ctpe = row["valor"].ToString().Trim();             // codigo ingreso efectivo
                        }
                        if (row["campo"].ToString() == "moneda" && row["param"].ToString() == "default") MonDeft = row["valor"].ToString().Trim();             // moneda por defecto
                    }
                    if (row["formulario"].ToString() == "ayccaja")
                    {
                        if (row["campo"].ToString() == "estado" && row["param"].ToString() == "abierto") codAbie = row["valor"].ToString().Trim();             // codigo caja abierta
                        //if (row["param"].ToString() == "cerrado") codCier = row["valor"].ToString().Trim();             // codigo caja cerrada
                    }
                    if (row["formulario"].ToString() == "interno")              // codigo enlace interno de anulacion del cliente con en BD A0
                    {
                        if (row["campo"].ToString() == "anulado" && row["param"].ToString() == "A0") vint_A0 = row["valor"].ToString().Trim();
                        if (row["campo"].ToString() == "igv" && row["param"].ToString() == "%") v_igv = row["valor"].ToString().Trim();
                    }
                }
                da.Dispose();
                dt.Dispose();
                // jalamos datos del usuario y local
                v_clu = TransCarga.Program.vg_luse;                // codigo local usuario
                v_slu = lib.serlocs(v_clu);                        // serie local usuario
                v_nbu = TransCarga.Program.vg_nuse;                // nombre del usuario
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión");
                Application.Exit();
                return;
            }
        }
        private void jalaoc(string campo)        // jala egresos
        {
            //try
            {
                string parte = "";
                if (campo == "tx_idcaja")
                {
                    parte = "where a.idcaja=@idcaja";
                }
                MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string consulta = "SELECT a.id,ifnull(f.descrizionerid,'') as tdi,a.fechope,a.seringv,a.numingv,ifnull(d.descrizionerid,'') as mon,a.locingv,a.estingv,a.codting," +
                        "a.tipdoco,a.serdoco,a.numdoco,a.ctaprop,ifnull(c.descrizione,'') as cta,a.refctap," +
                        "ifnull(a.fechdep,'') as fechdep,a.obscobc,a.codmopa,a.totpago,a.timping,a.tcadvta,a.porcigv,a.totpaMN,a.codmoMN,a.userc,b.nom_user,a.idcaja " +
                        "FROM cabingresosv a " +
                        "LEFT JOIN desc_mon d ON d.idcodice = a.codmopa " +
                        "left join desc_ctb c on c.idcodice = a.ctaprop " +
                        "left join desc_tdv f on f.idcodice = a.tipdoco " +
                        "left join usuarios b on b.nom_user = a.userc " + 
                        parte;
                    MySqlCommand micon = new MySqlCommand(consulta, conn);
                    if (campo == "tx_idcaja") micon.Parameters.AddWithValue("@idcaja", tx_idcaja.Text);
                    MySqlDataReader dr = micon.ExecuteReader();
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            // aca llenamos el detalle de los egresos para la caja respectiva 
                            dataGridView1.Rows.Add(
                                    dr.GetString("id"),
                                    dr.GetString("tdi"),
                                    dr.GetString("seringv"),
                                    dr.GetString("numdoco"),
                                    dr.GetString("mon"),
                                    dr.GetString("totpago"),
                                    dr.GetString("totpaMN"),
                                    dr.GetString("cta"),
                                    dr.GetString("refctap"),
                                    dr.GetString("fechdep"),
                                    dr.GetString("obscobc"),
                                    dr.GetString("fechope"),
                                    dr.GetString("timping"),
                                    dr.GetString("codmopa"),
                                    dr.GetString("ctaprop"),
                                    dr.GetString("estingv"),
                                    dr.GetString("userc"),
                                    dr.GetString("nom_user"),
                                    dr.GetString("numingv"),
                                    dr.GetString("tipdoco"),
                                    dr.GetString("tcadvta")
                                    ); ;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No existe el número buscado!", "Atención - dato incorrecto",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    dr.Dispose();
                    micon.Dispose();
                    //
                }
                conn.Close();
            }
        }
        public void dataload()                  // jala datos para los combos 
        {
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                while (true)
                {
                    try
                    {
                        conn.Open();
                        break;
                    }
                    catch (MySqlException ex)
                    {
                        var aa = MessageBox.Show(ex.Message + Environment.NewLine + "No se pudo conectar con el servidor" + Environment.NewLine +
                            "Desea volver a intentarlo?", "Error de conexión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.No)
                        {
                            Application.Exit();
                            return;
                        }
                    }
                }
                // datos para el combo de moneda
                cmb_mon.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mon where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtm.Clear();
                        dacu.Fill(dtm);
                        cmb_mon.DataSource = dtm;
                        cmb_mon.DisplayMember = "descrizionerid";
                        cmb_mon.ValueMember = "idcodice";
                    }
                }
                // datos para el combo de medio de pago
                cmb_mpago.Items.Clear();                    // OJO, solo se admiten medio de pago EFECTIVO
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_mpa where numero=@bloq and cnt=0", conn))  // efectivo
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtmpa.Clear();
                        dacu.Fill(dtmpa);
                        cmb_mpago.DataSource = dtmpa;
                        cmb_mpago.DisplayMember = "descrizionerid";
                        cmb_mpago.ValueMember = "idcodice";
                    }
                }
                // datos para el combo VALE ingreso
                cmb_comp.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid from desc_tdv where numero=@bloq and idcodice in (@vales)", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    cmo.Parameters.AddWithValue("@vales", v_codsv);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtcom.Clear();
                        dacu.Fill(dtcom);
                        cmb_comp.DataSource = dtcom;
                        cmb_comp.DisplayMember = "descrizionerid";
                        cmb_comp.ValueMember = "idcodice";
                    }
                }
                // combo de cuentas bancarias
                cmb_ctaprop.Items.Clear();
                using (MySqlCommand cmo = new MySqlCommand("select idcodice,descrizionerid,descrizione,codigo from desc_ctb where numero=@bloq", conn))
                {
                    cmo.Parameters.AddWithValue("@bloq", 1);
                    using (MySqlDataAdapter dacu = new MySqlDataAdapter(cmo))
                    {
                        dtctb.Clear();
                        dacu.Fill(dtctb);
                        cmb_ctaprop.DataSource = dtctb;
                        cmb_ctaprop.DisplayMember = "descrizione";
                        cmb_ctaprop.ValueMember = "idcodice";
                    }
                }
                // jalamos la caja
                using (MySqlCommand micon = new MySqlCommand("select id,fechope,statusc from cabccaja where loccaja=@luc order by id desc limit 1", conn))
                {
                    micon.Parameters.AddWithValue("@luc", v_clu);
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            v_estcaj = dr.GetString("statusc");
                            v_idcaj = dr.GetString("id");
                        }
                    }
                }
            }
        }
        private bool valiVars()                 // valida existencia de datos en variables del form
        {
            bool retorna = true;
            if (codAnul == "")          // codigo de documento anulado
            {
                lib.messagebox("Código de Ingreso ANULADO");
                retorna = false;
            }
            if (codGene == "")          // codigo documento nuevo generado
            {
                lib.messagebox("Código de Ingreso GENERADA/NUEVA");
                retorna = false;
            }
            if (MonDeft == "")          // moneda por defecto
            {
                lib.messagebox("Moneda por defecto");
                retorna = false;
            }
            if (v_slu == "")            // serie del local del usuario
            {
                lib.messagebox("Serie general local del usuario");
                retorna = false;
            }
            if (vint_A0 == "")
            {
                lib.messagebox("Código interno enlace anulación BD - A0");
                retorna = false;
            }
            // aca falta agregar resto  ...........
            return retorna;
        }
        private void tipcambio(string codmod)                // funcion para calculos con el tipo de cambio
        {
            if (codmod != MonDeft)
            {
                vtipcam vtipcam = new vtipcam(tx_PAGO.Text, codmod, DateTime.Now.Date.ToString());
                var result = vtipcam.ShowDialog();
                if (vtipcam.ReturnValue3 != null)
                {
                    tx_PAGO.Text = vtipcam.ReturnValue1;
                    tx_pagoMN.Text = vtipcam.ReturnValue2;
                    tx_tipcam.Text = vtipcam.ReturnValue3;
                    if (tx_pagoMN.Text.Trim() == "0.00" && (tx_PAGO.Text.Trim() != "" || tx_PAGO.Text.Trim() != "0"))
                    {
                        tx_pagoMN.Text = Math.Round(decimal.Parse(tx_PAGO.Text) * decimal.Parse(tx_tipcam.Text), 2).ToString();
                    }
                }
                else
                {
                    cmb_mon.SelectedValue = MonDeft;
                }
            }
        }
        private void calculos(decimal totDoc)
        {
            decimal tigv = 0;
            decimal tsub = 0;
            if (totDoc > 0)
            {
                tsub = Math.Round(totDoc / (1 + decimal.Parse(v_igv) / 100), 2);
                tigv = Math.Round(totDoc - tsub, 2);
                
            }
        }
        private void sumdet()                   // totalizamos detalle
        {
            tx_tfil.Text = "";
            tx_totcant.Text = "";
            decimal tp = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["valorMN"].Value != null && row.Cells["status"].Value.ToString() != codAnul)
                {
                    tp = tp + decimal.Parse(row.Cells["valorMN"].Value.ToString());  // row["valorMN"].ToString()
                }
            }
            tx_tfil.Text = (dataGridView1.Rows.Count - 1).ToString();
            tx_totcant.Text = tp.ToString();
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
            if (tx_dat_comp.Text == "")
            {
                MessageBox.Show("Seleccione el documento de ingreso", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_comp.Focus();
                return;
            }
            if (tx_serGR.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese la serie del vale de ingreso", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_serGR.Focus();
                return;
            }
            if (tx_numGR.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese el número del vale de ingreso", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_numGR.Focus();
                return;
            }
            if (tx_dat_mone.Text.Trim() == "")
            {
                MessageBox.Show("Seleccione la moneda del ingreso extraordinario", " Atención ");
                cmb_mon.Focus();
                return;
            }
            if (tx_dat_mp.Text == "")
            {
                MessageBox.Show("Seleccione el tipo del ingreso", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmb_mpago.Focus();
                return;
            }
            if (tx_PAGO.Text.Trim() == "")
            {
                MessageBox.Show("Registre el monto del ingreso", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tx_PAGO.Focus();
                return;
            }
            if (tx_dat_mp.Text != v_ctpe)   // ingreso extraordinario no efectivo
            {
                if (tx_dat_cta.Text == "")
                {
                    MessageBox.Show("Seleccione la cuenta hacia donde se ingreso el dinero", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmb_ctaprop.Focus();
                    return;
                }
            }
            if (tx_idcaja.Text == "")
            {
                MessageBox.Show("No existe caja!", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
            {
                if (tx_dat_mone.Text != MonDeft)    //  && tx_tipcam.Text.Trim() == ""
                {
                    decimal tc = 0, vc = 0;
                    decimal.TryParse(tx_tipcam.Text, out tc);
                    decimal.TryParse(tx_pagoMN.Text, out vc);
                    if (tc <= 0 || vc <= 0)
                    {
                        MessageBox.Show("Seleccione la moneda y tipo de cambio", " Atención ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmb_mon.Focus();
                        return;
                    }
                    else
                    {
                        tx_pagoMN.Text = (decimal.Parse(tx_tipcam.Text) * decimal.Parse(tx_PAGO.Text)).ToString("#0.00");
                    }
                }
                else
                {
                    tx_pagoMN.Text = tx_PAGO.Text;
                }
            }
            #endregion
            // grabamos, actualizamos, etc
            string modo = Tx_modo.Text;
            string iserror = "no";
            if (modo == "NUEVO")
            {
                // validaciones de ingresos

                // vamos con todo
                if (tx_idr.Text.Trim() == "")
                {
                    var aa = MessageBox.Show("Confirma que desea crear el Ingreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        if (true)
                        {
                            if (graba() == true)
                            {
                                // actualizamos la tabla seguimiento de usuarios
                                string resulta = lib.ult_mov(nomform, nomtab, asd);
                                if (resulta != "OK")
                                {
                                    MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                jalaoc("tx_idcaja");
                            }
                            else
                            {
                                iserror = "si";
                            }
                        }
                    }
                    else
                    {
                        //rb_pago.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Los datos no son nuevos en egresos", "Verifique duplicidad", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            if (modo == "EDITAR")
            {
                // validaciones

                if (true)
                {
                    if (tx_idr.Text.Trim() != "")
                    {
                        var aa = MessageBox.Show("Confirma que desea modificar el Ingreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (aa == DialogResult.Yes)
                        {
                            edita();    // modificacion total
                            // actualizamos la tabla seguimiento de usuarios
                            string resulta = lib.ult_mov(nomform, nomtab, asd);
                            if (resulta != "OK")
                            {
                                MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            //tx_serie.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("El documento ya debe existir para editar", "Debe ser edición", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
            }
            if (modo == "ANULAR")
            {
                // validaciones

                // SOLO USUARIOS AUTORIZADOS DEBEN ACCEDER A ESTA OPCIÓN
                // SE ANULA EL DOCUMENTO Y LOS MOVIMIENTOS INTERNOS se hacen por B.D.
                // anulacion procede siempre y cuando sea de la fecha y del usuario
                if (asd != tx_dat_userdoc.Text) // falta validar caja abierta
                {
                    MessageBox.Show("No se puede ANULAR Ingresos Varios fuera de fecha" + Environment.NewLine +
                        "o que sean de otro local/usuario","Atención",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    return;
                }
                if (tx_idr.Text.Trim() != "")
                {
                    var aa = MessageBox.Show("Confirma que desea ANULAR el Ingreso?", "Confirme por favor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (aa == DialogResult.Yes)
                    {
                        anula();
                        string resulta = lib.ult_mov(nomform, nomtab, asd);
                        if (resulta != "OK")
                        {
                            MessageBox.Show(resulta, "Error en actualización de seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        //tx_serie.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("El documento debe existir para poder anular!", "No esta el Id del registro", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
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
                string inserta = "insert into cabingresosv (" +
                    "idcaja,fechope,seringv,locingv,estingv,codting,tipdoco,serdoco,numdoco," +
                    "ctaprop,refctap,fechdep,obscobc,codmopa,totpago,timping,tcadvta,porcigv,totpaMN,codmoMN," +
                    "verApp,userc,fechc,diriplan4,diripwan4,netbname) values (" +
                    "@idcaja,@fechop,@servin,@ldcpgr,@estado,@tip,@tipdoc,@serdoc,@numdoc," +
                    "@ctapro,@refcta,@fechde,@obsprg,@monppr,@totpag,@timepa,@tcoper,@porcig,@totMN,@codMN," +
                    "@verApp,@asd,now(),@iplan,@ipwan,@nbnam)";
                using (MySqlCommand micon = new MySqlCommand(inserta, conn))
                {
                    micon.Parameters.AddWithValue("@idcaja", tx_idcaja.Text);
                    micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@servin", tx_serGR.Text);
                    //micon.Parameters.AddWithValue("@numvin", );   // automatico en BD
                    micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                    micon.Parameters.AddWithValue("@estado", tx_dat_estad.Text);
                    micon.Parameters.AddWithValue("@tip", "");      // creo esta por las puras
                    micon.Parameters.AddWithValue("@tipdoc", tx_dat_comp.Text);
                    micon.Parameters.AddWithValue("@serdoc", tx_serGR.Text);
                    micon.Parameters.AddWithValue("@numdoc", tx_numGR.Text);
                    micon.Parameters.AddWithValue("@ctapro", tx_dat_cta.Text);
                    micon.Parameters.AddWithValue("@refcta", tx_glosa.Text);
                    micon.Parameters.AddWithValue("@fechde", (tx_fecdep.Text.Trim().Length < 8)? null : tx_fecdep.Text.Substring(6, 4) + "-" + tx_fecdep.Text.Substring(3, 2) + "-" + tx_fecdep.Text.Substring(0, 2));
                    micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                    micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                    micon.Parameters.AddWithValue("@totpag", tx_PAGO.Text);
                    micon.Parameters.AddWithValue("@timepa", tx_dat_mp.Text);
                    micon.Parameters.AddWithValue("@tcoper", (tx_tipcam.Text == "") ? "0" : tx_tipcam.Text);                   // TIPO DE CAMBIO                    
                    micon.Parameters.AddWithValue("@porcig", v_igv);                            // porcentaje en numeros de IGV
                    micon.Parameters.AddWithValue("@totMN", tx_pagoMN.Text);
                    micon.Parameters.AddWithValue("@codMN", MonDeft);
                    micon.Parameters.AddWithValue("@verApp", verapp);
                    micon.Parameters.AddWithValue("@asd", asd);
                    micon.Parameters.AddWithValue("@iplan", lib.iplan());
                    micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                    micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                    try
                    {
                        micon.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Number.ToString() + Environment.NewLine +
                            "Revise que el vale/recibo no este repetido","Error en insertar");
                        retorna = false;
                        return retorna;
                    }
                }
                using (MySqlCommand micon = new MySqlCommand("select last_insert_id()", conn))
                {
                    using (MySqlDataReader dr = micon.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            tx_idr.Text = dr.GetString(0);
                            //tx_numero.Text = lib.Right(tx_idr.Text, 8);
                            retorna = true;
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
        private void edita()
        {
            MySqlConnection conn = new MySqlConnection(DB_CONN_STR);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    if (true)
                    {
                        string actua = "update cabingresosv a set " +
                            "a.idcaja=@idcaja,a.fechope=@fechop,a.seringv=@servin,a.locingv=@ldcpgr,a.estingv=@estado,a.codting=@tip,a.tipdoco=@tipdoc,a.serdoco=@serdoc," +
                            "a.numdoco=@numdoc,a.ctaprop=@ctapro,a.refctap=@refcta,a.fechdep=@fechde,a.obscobc=@obsprg,a.codmopa=@monppr,a.totpago=@totpag," +
                            "a.timping=@timepa,a.tcadvta=@tcoper,a.porcigv=@porcig,a.totpaMN=@totMN,a.codmoMN=@codMN," +
                            "a.verApp=@verApp,a.userm=@asd,a.fechm=now(),a.diriplan4=@iplan,a.diripwan4=@ipwan,a.netbname=@nbnam " +
                            "where a.id=@idr";
                        MySqlCommand micon = new MySqlCommand(actua, conn);
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@idcaja", tx_idcaja.Text);
                        micon.Parameters.AddWithValue("@fechop", tx_fechope.Text.Substring(6, 4) + "-" + tx_fechope.Text.Substring(3, 2) + "-" + tx_fechope.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@servin", tx_serGR.Text);
                        micon.Parameters.AddWithValue("@ldcpgr", TransCarga.Program.almuser);         // local origen
                        micon.Parameters.AddWithValue("@estado", tx_dat_estad.Text);
                        micon.Parameters.AddWithValue("@tip", "");      // creo esta por las puras
                        micon.Parameters.AddWithValue("@tipdoc", tx_dat_comp.Text);
                        micon.Parameters.AddWithValue("@serdoc", tx_serGR.Text);
                        micon.Parameters.AddWithValue("@numdoc", tx_numGR.Text);
                        micon.Parameters.AddWithValue("@ctapro", tx_dat_cta.Text);
                        micon.Parameters.AddWithValue("@refcta", tx_glosa.Text);
                        micon.Parameters.AddWithValue("@fechde", (tx_fecdep.Text.Trim().Length < 8) ? null : tx_fecdep.Text.Substring(6, 4) + "-" + tx_fecdep.Text.Substring(3, 2) + "-" + tx_fecdep.Text.Substring(0, 2));
                        micon.Parameters.AddWithValue("@obsprg", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@monppr", tx_dat_mone.Text);
                        micon.Parameters.AddWithValue("@totpag", tx_PAGO.Text);
                        micon.Parameters.AddWithValue("@timepa", tx_dat_mp.Text);
                        micon.Parameters.AddWithValue("@tcoper", (tx_tipcam.Text == "") ? "0" : tx_tipcam.Text);                   // TIPO DE CAMBIO                    
                        micon.Parameters.AddWithValue("@porcig", v_igv);                            // porcentaje en numeros de IGV
                        micon.Parameters.AddWithValue("@totMN", tx_pagoMN.Text);
                        micon.Parameters.AddWithValue("@codMN", MonDeft);
                        micon.Parameters.AddWithValue("@verApp", verapp);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@iplan", lib.iplan());
                        micon.Parameters.AddWithValue("@ipwan", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnam", Environment.MachineName);
                        micon.ExecuteNonQuery();
                        //
                        // EDICION DEL DETALLE .... no hay detalle
                        micon.Dispose();
                    }
                    conn.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine +
                        "Confirme que no este repitiendo número de vale," + Environment.NewLine +
                        "falta algún dato, esté incompleto o es incorrecto", "Error modificando - NO SE GRABO");
                    //Application.Exit();
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
            // en este caso solo hay ANULACION FISICA
            // Anulacion fisica se "anula" el numero del documento en sistema y
            // se borran todos los enlaces mediante triggers en la B.D.
            using (MySqlConnection conn = new MySqlConnection(DB_CONN_STR))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string canul = "update cabingresosv set estingv=@estser,obscobc=@obse,usera=@asd,fecha=now()," +
                        "verApp=@veap,diriplan4=@dil4,diripwan4=@diw4,netbname=@nbnp,estintreg=@eiar " +
                        "where id=@idr";
                    using (MySqlCommand micon = new MySqlCommand(canul, conn))
                    {
                        micon.Parameters.AddWithValue("@idr", tx_idr.Text);
                        micon.Parameters.AddWithValue("@estser", codAnul);
                        micon.Parameters.AddWithValue("@obse", tx_obser1.Text);
                        micon.Parameters.AddWithValue("@asd", asd);
                        micon.Parameters.AddWithValue("@dil4", lib.iplan());
                        micon.Parameters.AddWithValue("@diw4", TransCarga.Program.vg_ipwan);
                        micon.Parameters.AddWithValue("@nbnp", Environment.MachineName);
                        micon.Parameters.AddWithValue("@veap", verapp);
                        micon.Parameters.AddWithValue("@eiar", (vint_A0 == codAnul) ? "A0" : "");  // codigo anulacion interna en DB A0
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
                dataGridView1.Rows.Clear();
                jalaoc("tx_idcaja");
            }
        }
        private void tx_idcaja_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "VISUALIZAR" && tx_idcaja.Text != "")
            {
                dataGridView1.Rows.Clear();
                jalaoc("tx_idcaja");
            }
        }
        private void tx_serGR_Leave(object sender, EventArgs e)
        {
            tx_serGR.Text = lib.Right("0000" + tx_serGR.Text, 4);
            tx_numGR.Focus();
        }
        private void tx_numGR_Leave(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" && tx_serGR.Text.Trim() != "" && tx_numGR.Text.Trim() != "")
            {
                tx_numGR.Text = lib.Right("00000000" + tx_numGR.Text, 8);
            }
        }
        private void tx_pago_Leave(object sender, EventArgs e)
        {
            if (tx_PAGO.Text.Trim() != "" && Tx_modo.Text == "NUEVO")
            {
                decimal vpag = decimal.Parse(tx_PAGO.Text);
                if (vpag <= 0)
                {
                    MessageBox.Show("El monto a pagar debe ser mayor a cero", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    tx_PAGO.Text = "";
                    tx_PAGO.Focus();
                    return;
                }
                if (tx_dat_mone.Text != MonDeft)   // tipo de cambio si moneda <> local
                {
                    calculos(decimal.Parse(tx_PAGO.Text));
                }
                else
                {
                    tx_pagoMN.Text = tx_PAGO.Text;
                    calculos(decimal.Parse(tx_PAGO.Text));
                }
            }
        }
        private void cmb_mon_Enter(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO") cmb_mon.DroppedDown = true;
        }
        private void cmb_mpago_Enter(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO") cmb_mpago.DroppedDown = true;
        }
        private void cmb_comp_Enter(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO") cmb_comp.DroppedDown = true;
        }
        private void cmb_ctapro_Enter(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO") cmb_ctaprop.DroppedDown = true;
        }
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
            escribe();
            // 
            Bt_ini.Enabled = false;
            Bt_sig.Enabled = false;
            Bt_ret.Enabled = false;
            Bt_fin.Enabled = false;
            //
            initIngreso();
            tx_idcaja.Text = "";
            if (v_estcaj == codAbie)    // valida existencia de caja abierta en fecha y sede
            {
                // validamos la fecha de la caja
                string fhoy = lib.fechaServ("ansi");
                if (fhoy != TransCarga.Program.vg_fcaj)  // ambas fecahs formato yyyy-mm-dd
                {
                    MessageBox.Show("Debe cerrar la caja anterior!", "Caja fuera de fecha", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                else
                {
                    tx_idcaja.Text = v_idcaj;   // aca debe ir el verdadero id de la caja abierta
                }
            }
            else
            {
                MessageBox.Show("No existe caja abierta!","Atención",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            jalaoc("tx_idcaja");
            tx_serGR.ReadOnly = true;
            cmb_comp.Focus();
        }
        private void Bt_edit_Click(object sender, EventArgs e)
        {
            sololee();          
            Tx_modo.Text = "EDITAR";
            button1.Image = Image.FromFile(img_grab);
            initIngreso();
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            tx_idcaja.Text = "";
            if (v_estcaj == codAbie)    // valida existencia de caja abierta en fecha y sede
            {
                tx_idcaja.Text = v_idcaj;   // aca debe ir el verdadero id de la caja abierta
            }
            jalaoc("tx_idcaja");
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
        }
        private void Bt_anul_Click(object sender, EventArgs e)
        {
            sololee();
            Tx_modo.Text = "ANULAR";
            button1.Image = Image.FromFile(img_anul);
            initIngreso();
            tx_obser1.Enabled = true;
            tx_obser1.ReadOnly = false;
            tx_idcaja.Text = "";
            if (v_estcaj == codAbie)    // valida existencia de caja abierta en fecha y sede
            {
                tx_idcaja.Text = v_idcaj;   // aca debe ir el verdadero id de la caja abierta
            }
            jalaoc("tx_idcaja");
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
            tx_idcaja.Enabled = true;
            tx_idcaja.ReadOnly = false;
            tx_idcaja.Focus();
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
            tx_idcaja.Text = lib.gofirts("cabccaja");  // nomtab
            tx_idcaja_Leave(null, null);
        }
        private void Bt_back_Click(object sender, EventArgs e)
        {
            if(tx_idcaja.Text.Trim() != "")    // tx_idr.Text.Trim() != ""
            {
                int aca = int.Parse(tx_idcaja.Text) - 1;
                limpiar();
                limpia_chk();
                limpia_combos();
                limpia_otros();
                tx_idcaja.Text = aca.ToString();   // tx_idr.Text = aca.ToString();
                tx_idcaja_Leave(null, null);
            }
        }
        private void Bt_next_Click(object sender, EventArgs e)
        {
            int aca = int.Parse(tx_idcaja.Text) + 1;
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idcaja.Text = aca.ToString();
            tx_idcaja_Leave(null, null);
        }
        private void Bt_last_Click(object sender, EventArgs e)
        {
            limpiar();
            limpia_chk();
            limpia_combos();
            limpia_otros();
            tx_idcaja.Text = lib.golast("cabccaja");     // nomtab
            tx_idcaja_Leave(null, null);
        }
        #endregion botones;
        // proveed para habilitar los botones de comando
        #endregion botones_de_comando  ;

        #region comboboxes
        private void cmb_mon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Tx_modo.Text == "NUEVO" || Tx_modo.Text == "EDITAR")
            {
                if (cmb_mon.SelectedIndex > -1)
                {
                    tx_dat_mone.Text = cmb_mon.SelectedValue.ToString();
                    if (tx_dat_mone.Text != MonDeft)
                    {
                        tipcambio(tx_dat_mone.Text);
                    }
                    else
                    {
                        tx_pagoMN.Text = tx_PAGO.Text;
                    }
                }
            }
        }
        private void cmb_mpago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_mpago.SelectedIndex > -1)
            {
                tx_dat_mp.Text = cmb_mpago.SelectedValue.ToString();
                //
                if (cmb_mpago.SelectedValue.ToString() == v_ctpe)    // efectivo
                {
                    cmb_ctaprop.Enabled = false;
                    tx_dat_cta.Text = "";
                    tx_glosa.Text = "";
                    tx_glosa.ReadOnly = true;
                    tx_fecdep.Text = "";
                    tx_fecdep.ReadOnly = true;
                    tx_obser1.Focus();
                }
                else
                {
                    cmb_ctaprop.Enabled = true;
                    tx_glosa.ReadOnly = false;
                    tx_fecdep.ReadOnly = false;
                    tx_fecdep.Text = tx_fechope.Text;
                    cmb_ctaprop.Focus();
                }
            }
        }
        private void cmb_mpago_SelectionChangeCommitted(object sender, EventArgs e)
        {
            /*
            if (cmb_mpago.SelectedValue.ToString() == v_ctpe)    // efectivo
            {
                cmb_ctaprop.Enabled = false;
                tx_dat_cta.Text = "";
                tx_glosa.Text = "";
                tx_glosa.ReadOnly = true;
                tx_fecdep.Text = "";
                tx_fecdep.ReadOnly = true;
                tx_obser1.Focus();
            }
            else
            {
                cmb_ctaprop.Enabled = true;
                tx_glosa.ReadOnly = false;
                tx_fecdep.ReadOnly = false;
                tx_fecdep.Text = tx_fechope.Text;
                cmb_ctaprop.Focus();
            }*/
        }
        private void cmb_comp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_comp.SelectedIndex > -1)
            {
                tx_dat_comp.Text = cmb_comp.SelectedValue.ToString();
            }
        }
        private void cmb_ctaprop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_ctaprop.SelectedIndex > -1)
            {
                tx_dat_cta.Text = cmb_ctaprop.SelectedValue.ToString();
            }
        }
        #endregion comboboxes

        #region grilla
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            sumdet();
            if (dataGridView1.Rows[e.RowIndex].Cells["status"].Value != null)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["status"].Value.ToString() == codAnul)
                   dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.IndianRed;
            }
        }
        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            sumdet();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Tx_modo.Text == "EDITAR" || Tx_modo.Text == "ANULAR")
            {
                if (e.RowIndex > -1 && e.RowIndex < dataGridView1.Rows.Count)   // - 1
                {
                    tx_idr.Text = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();
                    tx_fechope.Text = dataGridView1.Rows[e.RowIndex].Cells["fechope"].Value.ToString().Substring(0,10);
                    tx_dat_userdoc.Text = dataGridView1.Rows[e.RowIndex].Cells["userc"].Value.ToString();
                    tx_digit.Text = dataGridView1.Rows[e.RowIndex].Cells["nom_user"].Value.ToString();
                    tx_dat_estad.Text = codGene;
                    tx_dat_comp.Text = dataGridView1.Rows[e.RowIndex].Cells["tipdoco"].Value.ToString(); ;
                    // tx_idcaja
                    tx_serGR.Text = dataGridView1.Rows[e.RowIndex].Cells["serie"].Value.ToString();
                    tx_numGR.Text = dataGridView1.Rows[e.RowIndex].Cells["numero"].Value.ToString();
                    tx_nuingv.Text = dataGridView1.Rows[e.RowIndex].Cells["numingv"].Value.ToString();
                    tx_dat_mone.Text = dataGridView1.Rows[e.RowIndex].Cells["codmopa"].Value.ToString();
                    tx_PAGO.Text = dataGridView1.Rows[e.RowIndex].Cells["monto"].Value.ToString();
                    tx_dat_mp.Text = dataGridView1.Rows[e.RowIndex].Cells["mpago"].Value.ToString();
                    tx_dat_cta.Text = dataGridView1.Rows[e.RowIndex].Cells["ctaprop"].Value.ToString();
                    tx_glosa.Text = dataGridView1.Rows[e.RowIndex].Cells["glosa"].Value.ToString();
                    if (dataGridView1.Rows[e.RowIndex].Cells["fechdep"].Value.ToString().Trim() != "")
                    {
                        tx_fecdep.Text = dataGridView1.Rows[e.RowIndex].Cells["fechdep"].Value.ToString().Substring(8, 2) +
                            dataGridView1.Rows[e.RowIndex].Cells["fechdep"].Value.ToString().Substring(5, 2) +
                            dataGridView1.Rows[e.RowIndex].Cells["fechdep"].Value.ToString().Substring(0, 4);
                    }
                    else
                    {
                        tx_fecdep.Text = dataGridView1.Rows[e.RowIndex].Cells["fechdep"].Value.ToString();
                    }
                    tx_obser1.Text = dataGridView1.Rows[e.RowIndex].Cells["observaciones"].Value.ToString();
                    tx_tipcam.Text = "";
                    tx_pagoMN.Text = dataGridView1.Rows[e.RowIndex].Cells["valorMN"].Value.ToString();
                    cmb_comp.SelectedValue = tx_dat_comp.Text;
                    cmb_mon.SelectedValue = tx_dat_mone.Text;
                    cmb_mpago.SelectedValue = tx_dat_mp.Text;
                    cmb_ctaprop.SelectedValue = tx_dat_cta.Text;
                    tx_tipcam.Text = dataGridView1.Rows[e.RowIndex].Cells["tipcam"].Value.ToString();
                    //
                    if (Tx_modo.Text == "EDITAR")
                    {
                        escribe();
                        //tx_serie.ReadOnly = true;
                        //tx_numero.ReadOnly = true;
                    }
                }
            }
        }
        #endregion
    }
}
