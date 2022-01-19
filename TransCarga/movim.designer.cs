namespace TransCarga
{
    partial class movim
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lb_titulo = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tx_ndr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tx_dat_dest = new System.Windows.Forms.TextBox();
            this.cmb_dest = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtp_fsal = new System.Windows.Forms.DateTimePicker();
            this.rb_ajuste = new System.Windows.Forms.RadioButton();
            this.rb_mov = new System.Windows.Forms.RadioButton();
            this.tx_comsal = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tx_evento = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.bt_close = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Crimson;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(2, 381);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(615, 31);
            this.panel1.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.BackColor = System.Drawing.Color.Red;
            this.button1.Location = new System.Drawing.Point(529, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Graba";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Crimson;
            this.panel2.Controls.Add(this.lb_titulo);
            this.panel2.Controls.Add(this.bt_close);
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(626, 26);
            this.panel2.TabIndex = 16;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            // 
            // lb_titulo
            // 
            this.lb_titulo.AutoSize = true;
            this.lb_titulo.Location = new System.Drawing.Point(280, 7);
            this.lb_titulo.Name = "lb_titulo";
            this.lb_titulo.Size = new System.Drawing.Size(73, 13);
            this.lb_titulo.TabIndex = 15;
            this.lb_titulo.Text = "Titulo del form";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 132);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(614, 248);
            this.dataGridView1.TabIndex = 20;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tx_ndr);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.tx_dat_dest);
            this.panel4.Controls.Add(this.cmb_dest);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.dtp_fsal);
            this.panel4.Controls.Add(this.rb_ajuste);
            this.panel4.Controls.Add(this.rb_mov);
            this.panel4.Controls.Add(this.tx_comsal);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.tx_evento);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Location = new System.Drawing.Point(2, 29);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(616, 102);
            this.panel4.TabIndex = 0;
            // 
            // tx_ndr
            // 
            this.tx_ndr.Location = new System.Drawing.Point(78, 52);
            this.tx_ndr.Name = "tx_ndr";
            this.tx_ndr.Size = new System.Drawing.Size(112, 20);
            this.tx_ndr.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(194, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Nombre";
            // 
            // tx_dat_dest
            // 
            this.tx_dat_dest.Location = new System.Drawing.Point(398, 29);
            this.tx_dat_dest.Name = "tx_dat_dest";
            this.tx_dat_dest.Size = new System.Drawing.Size(36, 20);
            this.tx_dat_dest.TabIndex = 14;
            this.tx_dat_dest.Visible = false;
            // 
            // cmb_dest
            // 
            this.cmb_dest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_dest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmb_dest.FormattingEnabled = true;
            this.cmb_dest.Location = new System.Drawing.Point(514, 28);
            this.cmb_dest.Name = "cmb_dest";
            this.cmb_dest.Size = new System.Drawing.Size(98, 21);
            this.cmb_dest.TabIndex = 13;
            this.cmb_dest.Visible = false;
            this.cmb_dest.SelectedIndexChanged += new System.EventHandler(this.cmb_dest_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(440, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Alm. Destino";
            this.label5.Visible = false;
            // 
            // dtp_fsal
            // 
            this.dtp_fsal.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp_fsal.Location = new System.Drawing.Point(514, 4);
            this.dtp_fsal.Name = "dtp_fsal";
            this.dtp_fsal.Size = new System.Drawing.Size(99, 20);
            this.dtp_fsal.TabIndex = 2;
            // 
            // rb_ajuste
            // 
            this.rb_ajuste.AutoSize = true;
            this.rb_ajuste.Location = new System.Drawing.Point(98, 23);
            this.rb_ajuste.Name = "rb_ajuste";
            this.rb_ajuste.Size = new System.Drawing.Size(118, 17);
            this.rb_ajuste.TabIndex = 1;
            this.rb_ajuste.TabStop = true;
            this.rb_ajuste.Text = "Entrega en Reparto";
            this.rb_ajuste.UseVisualStyleBackColor = true;
            this.rb_ajuste.CheckedChanged += new System.EventHandler(this.rb_ajuste_CheckedChanged);
            // 
            // rb_mov
            // 
            this.rb_mov.AutoSize = true;
            this.rb_mov.Location = new System.Drawing.Point(98, 4);
            this.rb_mov.Name = "rb_mov";
            this.rb_mov.Size = new System.Drawing.Size(113, 17);
            this.rb_mov.TabIndex = 0;
            this.rb_mov.TabStop = true;
            this.rb_mov.Text = "Entrega en Oficina";
            this.rb_mov.UseVisualStyleBackColor = true;
            this.rb_mov.CheckedChanged += new System.EventHandler(this.rb_mov_CheckedChanged);
            // 
            // tx_comsal
            // 
            this.tx_comsal.Location = new System.Drawing.Point(78, 75);
            this.tx_comsal.Name = "tx_comsal";
            this.tx_comsal.Size = new System.Drawing.Size(535, 20);
            this.tx_comsal.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Comentario";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(440, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Fecha Salida";
            // 
            // tx_evento
            // 
            this.tx_evento.Location = new System.Drawing.Point(240, 52);
            this.tx_evento.Name = "tx_evento";
            this.tx_evento.Size = new System.Drawing.Size(373, 20);
            this.tx_evento.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Receptor ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "TIPO SALIDA";
            // 
            // bt_close
            // 
            this.bt_close.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bt_close.FlatAppearance.BorderSize = 0;
            this.bt_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_close.ForeColor = System.Drawing.Color.White;
            this.bt_close.Image = global::TransCarga.Properties.Resources.close_square;
            this.bt_close.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_close.Location = new System.Drawing.Point(588, 4);
            this.bt_close.Name = "bt_close";
            this.bt_close.Size = new System.Drawing.Size(23, 18);
            this.bt_close.TabIndex = 14;
            this.bt_close.UseVisualStyleBackColor = true;
            this.bt_close.Click += new System.EventHandler(this.bt_close_Click);
            // 
            // movim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(615, 411);
            this.ControlBox = false;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "movim";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.movim_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bt_close;
        private System.Windows.Forms.Label lb_titulo;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox tx_comsal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tx_evento;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rb_ajuste;
        private System.Windows.Forms.RadioButton rb_mov;
        private System.Windows.Forms.DateTimePicker dtp_fsal;
        private System.Windows.Forms.ComboBox cmb_dest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tx_dat_dest;
        private System.Windows.Forms.TextBox tx_ndr;
        private System.Windows.Forms.Label label1;
    }
}