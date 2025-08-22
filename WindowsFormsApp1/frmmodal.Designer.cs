namespace WindowsFormsApp1
{
    partial class frmmodal
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
            this.rdShowmore = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdpaging = new System.Windows.Forms.RadioButton();
            this.txtcssshowmore = new System.Windows.Forms.TextBox();
            this.txtnumclick = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtnextpaging = new System.Windows.Forms.TextBox();
            this.lbnumsave = new System.Windows.Forms.Label();
            this.txtnumscroll = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BTBrowser = new System.Windows.Forms.Button();
            this.txtpathsavefile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.BTProcessing = new System.Windows.Forms.Button();
            this.txtcssclass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtinputlink = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.SuspendLayout();
            // 
            // rdShowmore
            // 
            this.rdShowmore.AutoSize = true;
            this.rdShowmore.Location = new System.Drawing.Point(172, 11);
            this.rdShowmore.Name = "rdShowmore";
            this.rdShowmore.Size = new System.Drawing.Size(99, 21);
            this.rdShowmore.TabIndex = 61;
            this.rdShowmore.TabStop = true;
            this.rdShowmore.Text = "Show more";
            this.rdShowmore.UseVisualStyleBackColor = true;
            this.rdShowmore.CheckedChanged += new System.EventHandler(this.rdShowmore_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdpaging);
            this.groupBox1.Controls.Add(this.rdShowmore);
            this.groupBox1.Controls.Add(this.txtcssshowmore);
            this.groupBox1.Controls.Add(this.txtnumclick);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtnextpaging);
            this.groupBox1.Location = new System.Drawing.Point(674, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(544, 94);
            this.groupBox1.TabIndex = 83;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show more or Paging";
            // 
            // rdpaging
            // 
            this.rdpaging.AutoSize = true;
            this.rdpaging.Location = new System.Drawing.Point(301, 11);
            this.rdpaging.Name = "rdpaging";
            this.rdpaging.Size = new System.Drawing.Size(73, 21);
            this.rdpaging.TabIndex = 61;
            this.rdpaging.TabStop = true;
            this.rdpaging.Text = "Paging";
            this.rdpaging.UseVisualStyleBackColor = true;
            this.rdpaging.CheckedChanged += new System.EventHandler(this.rdpaging_CheckedChanged);
            // 
            // txtcssshowmore
            // 
            this.txtcssshowmore.Location = new System.Drawing.Point(143, 36);
            this.txtcssshowmore.Name = "txtcssshowmore";
            this.txtcssshowmore.ReadOnly = true;
            this.txtcssshowmore.Size = new System.Drawing.Size(317, 23);
            this.txtcssshowmore.TabIndex = 58;
            // 
            // txtnumclick
            // 
            this.txtnumclick.Location = new System.Drawing.Point(477, 62);
            this.txtnumclick.Name = "txtnumclick";
            this.txtnumclick.ReadOnly = true;
            this.txtnumclick.Size = new System.Drawing.Size(56, 23);
            this.txtnumclick.TabIndex = 60;
            this.txtnumclick.Text = "2";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(109, 17);
            this.label9.TabIndex = 57;
            this.label9.Text = "CSS Show more";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(465, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 17);
            this.label11.TabIndex = 59;
            this.label11.Text = "Num click";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(114, 17);
            this.label10.TabIndex = 57;
            this.label10.Text = "CSS Next paging";
            // 
            // txtnextpaging
            // 
            this.txtnextpaging.Location = new System.Drawing.Point(143, 65);
            this.txtnextpaging.Name = "txtnextpaging";
            this.txtnextpaging.ReadOnly = true;
            this.txtnextpaging.Size = new System.Drawing.Size(317, 23);
            this.txtnextpaging.TabIndex = 58;
            // 
            // lbnumsave
            // 
            this.lbnumsave.AutoSize = true;
            this.lbnumsave.Location = new System.Drawing.Point(446, 153);
            this.lbnumsave.Name = "lbnumsave";
            this.lbnumsave.Size = new System.Drawing.Size(0, 17);
            this.lbnumsave.TabIndex = 82;
            // 
            // txtnumscroll
            // 
            this.txtnumscroll.Location = new System.Drawing.Point(1261, 15);
            this.txtnumscroll.Name = "txtnumscroll";
            this.txtnumscroll.Size = new System.Drawing.Size(48, 23);
            this.txtnumscroll.TabIndex = 81;
            this.txtnumscroll.Text = "2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1194, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 80;
            this.label8.Text = "Num scroll";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(341, 153);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 17);
            this.label7.TabIndex = 79;
            this.label7.Text = "Number save: ";
            // 
            // BTBrowser
            // 
            this.BTBrowser.Location = new System.Drawing.Point(1179, 42);
            this.BTBrowser.Margin = new System.Windows.Forms.Padding(2);
            this.BTBrowser.Name = "BTBrowser";
            this.BTBrowser.Size = new System.Drawing.Size(122, 28);
            this.BTBrowser.TabIndex = 68;
            this.BTBrowser.Text = "Browse";
            this.BTBrowser.UseVisualStyleBackColor = true;
            this.BTBrowser.Click += new System.EventHandler(this.BTBrowser_Click);
            // 
            // txtpathsavefile
            // 
            this.txtpathsavefile.Location = new System.Drawing.Point(829, 45);
            this.txtpathsavefile.Margin = new System.Windows.Forms.Padding(2);
            this.txtpathsavefile.Name = "txtpathsavefile";
            this.txtpathsavefile.ReadOnly = true;
            this.txtpathsavefile.Size = new System.Drawing.Size(346, 23);
            this.txtpathsavefile.TabIndex = 67;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(731, 47);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 17);
            this.label6.TabIndex = 78;
            this.label6.Text = "Path save file html";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 153);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 75;
            this.label3.Text = "Webpage Render";
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(11, 177);
            this.webView21.Margin = new System.Windows.Forms.Padding(2);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(1392, 611);
            this.webView21.TabIndex = 72;
            this.webView21.ZoomFactor = 1D;
            // 
            // BTProcessing
            // 
            this.BTProcessing.Location = new System.Drawing.Point(180, 95);
            this.BTProcessing.Margin = new System.Windows.Forms.Padding(2);
            this.BTProcessing.Name = "BTProcessing";
            this.BTProcessing.Size = new System.Drawing.Size(169, 34);
            this.BTProcessing.TabIndex = 71;
            this.BTProcessing.Text = "Proccessing download";
            this.BTProcessing.UseVisualStyleBackColor = true;
            this.BTProcessing.Click += new System.EventHandler(this.BTProcessing_Click);
            // 
            // txtcssclass
            // 
            this.txtcssclass.Location = new System.Drawing.Point(180, 44);
            this.txtcssclass.Margin = new System.Windows.Forms.Padding(2);
            this.txtcssclass.Name = "txtcssclass";
            this.txtcssclass.Size = new System.Drawing.Size(504, 23);
            this.txtcssclass.TabIndex = 66;
            this.txtcssclass.Text = "uitk-card-link";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 47);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 17);
            this.label4.TabIndex = 64;
            this.label4.Text = "CSS Class item";
            // 
            // txtinputlink
            // 
            this.txtinputlink.Location = new System.Drawing.Point(180, 15);
            this.txtinputlink.Margin = new System.Windows.Forms.Padding(2);
            this.txtinputlink.Name = "txtinputlink";
            this.txtinputlink.Size = new System.Drawing.Size(995, 23);
            this.txtinputlink.TabIndex = 65;
            this.txtinputlink.TextChanged += new System.EventHandler(this.txtinputlink_TextChanged);
            this.txtinputlink.Leave += new System.EventHandler(this.txtinputlink_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 63;
            this.label1.Text = "Input link";
            // 
            // frmmodal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1414, 802);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbnumsave);
            this.Controls.Add(this.txtnumscroll);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BTBrowser);
            this.Controls.Add(this.txtpathsavefile);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.BTProcessing);
            this.Controls.Add(this.txtcssclass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtinputlink);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmmodal";
            this.Text = "frmmodal";
            this.Load += new System.EventHandler(this.frmmodal_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton rdShowmore;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdpaging;
        private System.Windows.Forms.TextBox txtcssshowmore;
        private System.Windows.Forms.TextBox txtnumclick;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtnextpaging;
        private System.Windows.Forms.Label lbnumsave;
        private System.Windows.Forms.TextBox txtnumscroll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BTBrowser;
        private System.Windows.Forms.TextBox txtpathsavefile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Button BTProcessing;
        private System.Windows.Forms.TextBox txtcssclass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtinputlink;
        private System.Windows.Forms.Label label1;
    }
}