namespace TransCarga
{
    partial class TransMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransMain));
            this.pn_phor = new System.Windows.Forms.Panel();
            this.pn_menu = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.bt_solorsoft = new System.Windows.Forms.Button();
            this.pn_user = new System.Windows.Forms.Panel();
            this.tx_nuser = new System.Windows.Forms.TextBox();
            this.tx_empresa = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tx_user = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pn_pver = new System.Windows.Forms.Panel();
            this.bt_pedidos = new System.Windows.Forms.Button();
            this.bt_maestras = new System.Windows.Forms.Button();
            this.bt_almacen = new System.Windows.Forms.Button();
            this.bt_pcontrol = new System.Windows.Forms.Button();
            this.bt_ventas = new System.Windows.Forms.Button();
            this.bt_facele = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bt_salir = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pn_centro = new System.Windows.Forms.Panel();
            this.pn_phor.SuspendLayout();
            this.pn_menu.SuspendLayout();
            this.pn_user.SuspendLayout();
            this.pn_pver.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pn_phor
            // 
            this.pn_phor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pn_phor.Controls.Add(this.pn_menu);
            this.pn_phor.Controls.Add(this.bt_solorsoft);
            this.pn_phor.Controls.Add(this.pn_user);
            this.pn_phor.Location = new System.Drawing.Point(190, 1);
            this.pn_phor.Name = "pn_phor";
            this.pn_phor.Size = new System.Drawing.Size(945, 53);
            this.pn_phor.TabIndex = 0;
            // 
            // pn_menu
            // 
            this.pn_menu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pn_menu.Controls.Add(this.menuStrip1);
            this.pn_menu.Location = new System.Drawing.Point(-1, 25);
            this.pn_menu.Name = "pn_menu";
            this.pn_menu.Size = new System.Drawing.Size(891, 28);
            this.pn_menu.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Location = new System.Drawing.Point(5, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(100, 26);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // bt_solorsoft
            // 
            this.bt_solorsoft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_solorsoft.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_solorsoft.FlatAppearance.BorderSize = 0;
            this.bt_solorsoft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_solorsoft.Location = new System.Drawing.Point(896, 4);
            this.bt_solorsoft.Name = "bt_solorsoft";
            this.bt_solorsoft.Size = new System.Drawing.Size(45, 39);
            this.bt_solorsoft.TabIndex = 0;
            this.toolTip1.SetToolTip(this.bt_solorsoft, "http://www.solorsoft.com/transcarga/");
            this.bt_solorsoft.UseVisualStyleBackColor = true;
            this.bt_solorsoft.Click += new System.EventHandler(this.bt_solorsoft_Click);
            // 
            // pn_user
            // 
            this.pn_user.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pn_user.AutoSize = true;
            this.pn_user.Controls.Add(this.tx_nuser);
            this.pn_user.Controls.Add(this.tx_empresa);
            this.pn_user.Controls.Add(this.label2);
            this.pn_user.Controls.Add(this.tx_user);
            this.pn_user.Controls.Add(this.label1);
            this.pn_user.Location = new System.Drawing.Point(-1, 0);
            this.pn_user.Name = "pn_user";
            this.pn_user.Size = new System.Drawing.Size(891, 25);
            this.pn_user.TabIndex = 2;
            // 
            // tx_nuser
            // 
            this.tx_nuser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx_nuser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx_nuser.Location = new System.Drawing.Point(123, 1);
            this.tx_nuser.Name = "tx_nuser";
            this.tx_nuser.ReadOnly = true;
            this.tx_nuser.Size = new System.Drawing.Size(193, 21);
            this.tx_nuser.TabIndex = 4;
            // 
            // tx_empresa
            // 
            this.tx_empresa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx_empresa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx_empresa.Location = new System.Drawing.Point(417, 1);
            this.tx_empresa.Name = "tx_empresa";
            this.tx_empresa.ReadOnly = true;
            this.tx_empresa.Size = new System.Drawing.Size(249, 21);
            this.tx_empresa.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(328, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Organización: ";
            // 
            // tx_user
            // 
            this.tx_user.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx_user.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx_user.Location = new System.Drawing.Point(58, 1);
            this.tx_user.Name = "tx_user";
            this.tx_user.ReadOnly = true;
            this.tx_user.Size = new System.Drawing.Size(65, 21);
            this.tx_user.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Usuario: ";
            // 
            // pn_pver
            // 
            this.pn_pver.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pn_pver.Controls.Add(this.bt_pedidos);
            this.pn_pver.Controls.Add(this.bt_maestras);
            this.pn_pver.Controls.Add(this.bt_almacen);
            this.pn_pver.Controls.Add(this.bt_pcontrol);
            this.pn_pver.Controls.Add(this.bt_ventas);
            this.pn_pver.Controls.Add(this.bt_facele);
            this.pn_pver.Controls.Add(this.pictureBox1);
            this.pn_pver.Controls.Add(this.bt_salir);
            this.pn_pver.Location = new System.Drawing.Point(1, 1);
            this.pn_pver.Name = "pn_pver";
            this.pn_pver.Size = new System.Drawing.Size(189, 822);
            this.pn_pver.TabIndex = 1;
            // 
            // bt_pedidos
            // 
            this.bt_pedidos.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_pedidos.FlatAppearance.BorderSize = 0;
            this.bt_pedidos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_pedidos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_pedidos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_pedidos.Location = new System.Drawing.Point(2, 226);
            this.bt_pedidos.Name = "bt_pedidos";
            this.bt_pedidos.Size = new System.Drawing.Size(185, 50);
            this.bt_pedidos.TabIndex = 8;
            this.bt_pedidos.Text = "Administración";
            this.bt_pedidos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_pedidos.UseVisualStyleBackColor = true;
            this.bt_pedidos.Click += new System.EventHandler(this.bt_pedidos_Click);
            // 
            // bt_maestras
            // 
            this.bt_maestras.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_maestras.FlatAppearance.BorderSize = 0;
            this.bt_maestras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_maestras.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_maestras.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_maestras.Location = new System.Drawing.Point(2, 311);
            this.bt_maestras.Name = "bt_maestras";
            this.bt_maestras.Size = new System.Drawing.Size(185, 50);
            this.bt_maestras.TabIndex = 7;
            this.bt_maestras.Text = "Maestras";
            this.bt_maestras.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_maestras.UseVisualStyleBackColor = true;
            this.bt_maestras.Click += new System.EventHandler(this.bt_maestras_Click);
            // 
            // bt_almacen
            // 
            this.bt_almacen.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_almacen.FlatAppearance.BorderSize = 0;
            this.bt_almacen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_almacen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_almacen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_almacen.Location = new System.Drawing.Point(2, 268);
            this.bt_almacen.Name = "bt_almacen";
            this.bt_almacen.Size = new System.Drawing.Size(185, 50);
            this.bt_almacen.TabIndex = 6;
            this.bt_almacen.Text = "Alm. / Reparto";
            this.bt_almacen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_almacen.UseVisualStyleBackColor = true;
            this.bt_almacen.Click += new System.EventHandler(this.bt_almacen_Click);
            // 
            // bt_pcontrol
            // 
            this.bt_pcontrol.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_pcontrol.FlatAppearance.BorderSize = 0;
            this.bt_pcontrol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_pcontrol.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_pcontrol.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_pcontrol.Location = new System.Drawing.Point(2, 356);
            this.bt_pcontrol.Name = "bt_pcontrol";
            this.bt_pcontrol.Size = new System.Drawing.Size(185, 50);
            this.bt_pcontrol.TabIndex = 5;
            this.bt_pcontrol.Text = "Configuración";
            this.bt_pcontrol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_pcontrol.UseVisualStyleBackColor = true;
            this.bt_pcontrol.Click += new System.EventHandler(this.bt_pcontrol_Click);
            // 
            // bt_ventas
            // 
            this.bt_ventas.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_ventas.FlatAppearance.BorderSize = 0;
            this.bt_ventas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_ventas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_ventas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_ventas.Location = new System.Drawing.Point(2, 181);
            this.bt_ventas.Name = "bt_ventas";
            this.bt_ventas.Size = new System.Drawing.Size(185, 50);
            this.bt_ventas.TabIndex = 4;
            this.bt_ventas.Text = "Operaciones";
            this.bt_ventas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_ventas.UseVisualStyleBackColor = true;
            this.bt_ventas.Click += new System.EventHandler(this.bt_ventas_Click);
            // 
            // bt_facele
            // 
            this.bt_facele.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_facele.FlatAppearance.BorderSize = 0;
            this.bt_facele.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_facele.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_facele.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_facele.Location = new System.Drawing.Point(2, 130);
            this.bt_facele.Name = "bt_facele";
            this.bt_facele.Size = new System.Drawing.Size(185, 50);
            this.bt_facele.TabIndex = 3;
            this.bt_facele.Text = "Facturación";
            this.bt_facele.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_facele.UseVisualStyleBackColor = true;
            this.bt_facele.Click += new System.EventHandler(this.bt_facele_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(188, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // bt_salir
            // 
            this.bt_salir.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bt_salir.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.bt_salir.FlatAppearance.BorderSize = 0;
            this.bt_salir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_salir.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_salir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_salir.Location = new System.Drawing.Point(2, 770);
            this.bt_salir.Name = "bt_salir";
            this.bt_salir.Size = new System.Drawing.Size(185, 50);
            this.bt_salir.TabIndex = 2;
            this.bt_salir.Text = "Salir ";
            this.bt_salir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_salir.UseVisualStyleBackColor = true;
            this.bt_salir.Click += new System.EventHandler(this.bt_salir_Click);
            // 
            // pn_centro
            // 
            this.pn_centro.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pn_centro.Location = new System.Drawing.Point(191, 55);
            this.pn_centro.Name = "pn_centro";
            this.pn_centro.Size = new System.Drawing.Size(944, 768);
            this.pn_centro.TabIndex = 2;
            // 
            // TransMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 825);
            this.Controls.Add(this.pn_centro);
            this.Controls.Add(this.pn_pver);
            this.Controls.Add(this.pn_phor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TransMain";
            this.Text = "Solorsoft TransCarga";
            this.Activated += new System.EventHandler(this.TransMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransMain_FormClosing);
            this.Load += new System.EventHandler(this.TransMain_Load);
            this.pn_phor.ResumeLayout(false);
            this.pn_phor.PerformLayout();
            this.pn_menu.ResumeLayout(false);
            this.pn_user.ResumeLayout(false);
            this.pn_user.PerformLayout();
            this.pn_pver.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pn_phor;
        private System.Windows.Forms.Panel pn_pver;
        private System.Windows.Forms.Button bt_salir;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bt_solorsoft;
        private System.Windows.Forms.Button bt_facele;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button bt_ventas;
        private System.Windows.Forms.Panel pn_user;
        private System.Windows.Forms.TextBox tx_user;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tx_nuser;
        private System.Windows.Forms.Button bt_pcontrol;
        private System.Windows.Forms.Panel pn_centro;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel pn_menu;
        private System.Windows.Forms.Button bt_maestras;
        private System.Windows.Forms.Button bt_almacen;
        private System.Windows.Forms.Button bt_pedidos;
        public System.Windows.Forms.TextBox tx_empresa;
    }
}