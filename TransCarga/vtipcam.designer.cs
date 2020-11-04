namespace TransCarga
{
    partial class vtipcam
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vtipcam));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tx_newVal = new TransCarga.NumericTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tx_fecha = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tx_codmon = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tx_tipcam = new TransCarga.NumericTextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(757, 252);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 27);
            this.button1.TabIndex = 3;
            this.button1.Text = "Aceptar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(92, 148);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(170, 49);
            this.button2.TabIndex = 2;
            this.button2.Text = "GRABA";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button1_Click);
            // 
            // tx_newVal
            // 
            this.tx_newVal.AllowSpace = false;
            this.tx_newVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx_newVal.Location = new System.Drawing.Point(137, 109);
            this.tx_newVal.Name = "tx_newVal";
            this.tx_newVal.ReadOnly = true;
            this.tx_newVal.Size = new System.Drawing.Size(89, 29);
            this.tx_newVal.TabIndex = 1;
            this.tx_newVal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(42, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 18);
            this.label1.TabIndex = 7;
            this.label1.Text = "Tipo de Cambio - Fecha: ";
            // 
            // tx_fecha
            // 
            this.tx_fecha.Location = new System.Drawing.Point(228, 19);
            this.tx_fecha.Name = "tx_fecha";
            this.tx_fecha.ReadOnly = true;
            this.tx_fecha.Size = new System.Drawing.Size(77, 20);
            this.tx_fecha.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(42, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 18);
            this.label2.TabIndex = 9;
            this.label2.Text = "Tipo de Cambio - Moneda:";
            // 
            // tx_codmon
            // 
            this.tx_codmon.Location = new System.Drawing.Point(228, 42);
            this.tx_codmon.Name = "tx_codmon";
            this.tx_codmon.ReadOnly = true;
            this.tx_codmon.Size = new System.Drawing.Size(77, 20);
            this.tx_codmon.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(42, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "Tipo de Cambio - Valor:";
            // 
            // tx_tipcam
            // 
            this.tx_tipcam.AllowSpace = false;
            this.tx_tipcam.Location = new System.Drawing.Point(228, 65);
            this.tx_tipcam.Name = "tx_tipcam";
            this.tx_tipcam.Size = new System.Drawing.Size(77, 20);
            this.tx_tipcam.TabIndex = 12;
            // 
            // vtipcam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 209);
            this.Controls.Add(this.tx_tipcam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tx_codmon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tx_fecha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tx_newVal);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "vtipcam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ENVIO SEGURO";
            this.Load += new System.EventHandler(this.vtipcam_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private NumericTextBox tx_newVal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tx_fecha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tx_codmon;
        private System.Windows.Forms.Label label3;
        private NumericTextBox tx_tipcam;
    }
}
