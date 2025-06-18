namespace WindowsFormsApp1
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtinputlink = new System.Windows.Forms.TextBox();
            this.BTProcessing = new System.Windows.Forms.Button();
            this.BTGetLinks = new System.Windows.Forms.Button();
            this.lblinks = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtcssclass = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lbnumber = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtpathsavefile = new System.Windows.Forms.TextBox();
            this.BTBrowser = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.BTSavelinks = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtnumscroll = new System.Windows.Forms.TextBox();
            this.lbnumsave = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtcssshowmore = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtnextpaging = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtnumclick = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdpaging = new System.Windows.Forms.RadioButton();
            this.rdShowmore = new System.Windows.Forms.RadioButton();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.BTLoadlinks = new System.Windows.Forms.Button();
            this.BTPreventbot = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtdelay = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input link";
            // 
            // txtinputlink
            // 
            this.txtinputlink.Location = new System.Drawing.Point(190, 10);
            this.txtinputlink.Margin = new System.Windows.Forms.Padding(2);
            this.txtinputlink.Name = "txtinputlink";
            this.txtinputlink.Size = new System.Drawing.Size(995, 23);
            this.txtinputlink.TabIndex = 0;
            this.txtinputlink.Leave += new System.EventHandler(this.txtinputlink_Leave);
            // 
            // BTProcessing
            // 
            this.BTProcessing.Location = new System.Drawing.Point(436, 107);
            this.BTProcessing.Margin = new System.Windows.Forms.Padding(2);
            this.BTProcessing.Name = "BTProcessing";
            this.BTProcessing.Size = new System.Drawing.Size(169, 34);
            this.BTProcessing.TabIndex = 7;
            this.BTProcessing.Text = "Proccessing download";
            this.BTProcessing.UseVisualStyleBackColor = true;
            this.BTProcessing.Click += new System.EventHandler(this.BTProcessing_Click);
            // 
            // BTGetLinks
            // 
            this.BTGetLinks.Location = new System.Drawing.Point(42, 74);
            this.BTGetLinks.Margin = new System.Windows.Forms.Padding(2);
            this.BTGetLinks.Name = "BTGetLinks";
            this.BTGetLinks.Size = new System.Drawing.Size(115, 34);
            this.BTGetLinks.TabIndex = 5;
            this.BTGetLinks.Text = "Get links";
            this.BTGetLinks.UseVisualStyleBackColor = true;
            this.BTGetLinks.Click += new System.EventHandler(this.BTGetLinks_Click);
            // 
            // lblinks
            // 
            this.lblinks.FormattingEnabled = true;
            this.lblinks.ItemHeight = 16;
            this.lblinks.Location = new System.Drawing.Point(32, 170);
            this.lblinks.Margin = new System.Windows.Forms.Padding(2);
            this.lblinks.Name = "lblinks";
            this.lblinks.Size = new System.Drawing.Size(370, 612);
            this.lblinks.TabIndex = 44;
            this.lblinks.SelectedIndexChanged += new System.EventHandler(this.lblinks_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 147);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 17);
            this.label2.TabIndex = 45;
            this.label2.Text = "List links";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(406, 151);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 46;
            this.label3.Text = "Webpage Render";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 42);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "CSS Class link";
            // 
            // txtcssclass
            // 
            this.txtcssclass.Location = new System.Drawing.Point(190, 39);
            this.txtcssclass.Margin = new System.Windows.Forms.Padding(2);
            this.txtcssclass.Name = "txtcssclass";
            this.txtcssclass.Size = new System.Drawing.Size(504, 23);
            this.txtcssclass.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 145);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 17);
            this.label5.TabIndex = 47;
            this.label5.Text = "Number:";
            // 
            // lbnumber
            // 
            this.lbnumber.AutoSize = true;
            this.lbnumber.Location = new System.Drawing.Point(226, 146);
            this.lbnumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbnumber.Name = "lbnumber";
            this.lbnumber.Size = new System.Drawing.Size(0, 17);
            this.lbnumber.TabIndex = 48;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(741, 42);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 17);
            this.label6.TabIndex = 49;
            this.label6.Text = "Path save file html";
            // 
            // txtpathsavefile
            // 
            this.txtpathsavefile.Location = new System.Drawing.Point(839, 40);
            this.txtpathsavefile.Margin = new System.Windows.Forms.Padding(2);
            this.txtpathsavefile.Name = "txtpathsavefile";
            this.txtpathsavefile.ReadOnly = true;
            this.txtpathsavefile.Size = new System.Drawing.Size(346, 23);
            this.txtpathsavefile.TabIndex = 3;
            // 
            // BTBrowser
            // 
            this.BTBrowser.Location = new System.Drawing.Point(1189, 37);
            this.BTBrowser.Margin = new System.Windows.Forms.Padding(2);
            this.BTBrowser.Name = "BTBrowser";
            this.BTBrowser.Size = new System.Drawing.Size(122, 28);
            this.BTBrowser.TabIndex = 4;
            this.BTBrowser.Text = "Browse";
            this.BTBrowser.UseVisualStyleBackColor = true;
            this.BTBrowser.Click += new System.EventHandler(this.BTBrowser_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(667, 149);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 17);
            this.label7.TabIndex = 52;
            this.label7.Text = "Number save: ";
            // 
            // BTSavelinks
            // 
            this.BTSavelinks.Location = new System.Drawing.Point(177, 75);
            this.BTSavelinks.Margin = new System.Windows.Forms.Padding(2);
            this.BTSavelinks.Name = "BTSavelinks";
            this.BTSavelinks.Size = new System.Drawing.Size(99, 34);
            this.BTSavelinks.TabIndex = 6;
            this.BTSavelinks.Text = "Save links";
            this.BTSavelinks.UseVisualStyleBackColor = true;
            this.BTSavelinks.Click += new System.EventHandler(this.BTSavelinks_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1204, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 54;
            this.label8.Text = "Num scroll";
            // 
            // txtnumscroll
            // 
            this.txtnumscroll.Location = new System.Drawing.Point(1271, 10);
            this.txtnumscroll.Name = "txtnumscroll";
            this.txtnumscroll.Size = new System.Drawing.Size(48, 23);
            this.txtnumscroll.TabIndex = 55;
            this.txtnumscroll.Text = "5";
            // 
            // lbnumsave
            // 
            this.lbnumsave.AutoSize = true;
            this.lbnumsave.Location = new System.Drawing.Point(772, 149);
            this.lbnumsave.Name = "lbnumsave";
            this.lbnumsave.Size = new System.Drawing.Size(0, 17);
            this.lbnumsave.TabIndex = 56;
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
            // txtcssshowmore
            // 
            this.txtcssshowmore.Location = new System.Drawing.Point(143, 36);
            this.txtcssshowmore.Name = "txtcssshowmore";
            this.txtcssshowmore.ReadOnly = true;
            this.txtcssshowmore.Size = new System.Drawing.Size(317, 23);
            this.txtcssshowmore.TabIndex = 58;
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(465, 36);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 17);
            this.label11.TabIndex = 59;
            this.label11.Text = "Num click";
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
            this.groupBox1.Location = new System.Drawing.Point(856, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(544, 94);
            this.groupBox1.TabIndex = 61;
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
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(406, 170);
            this.webView21.Margin = new System.Windows.Forms.Padding(2);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(998, 611);
            this.webView21.TabIndex = 43;
            this.webView21.ZoomFactor = 1D;
            // 
            // BTLoadlinks
            // 
            this.BTLoadlinks.Location = new System.Drawing.Point(289, 74);
            this.BTLoadlinks.Margin = new System.Windows.Forms.Padding(2);
            this.BTLoadlinks.Name = "BTLoadlinks";
            this.BTLoadlinks.Size = new System.Drawing.Size(113, 34);
            this.BTLoadlinks.TabIndex = 62;
            this.BTLoadlinks.Text = "Load links";
            this.BTLoadlinks.UseVisualStyleBackColor = true;
            this.BTLoadlinks.Click += new System.EventHandler(this.BTLoadlinks_Click);
            // 
            // BTPreventbot
            // 
            this.BTPreventbot.Location = new System.Drawing.Point(27, 26);
            this.BTPreventbot.Margin = new System.Windows.Forms.Padding(2);
            this.BTPreventbot.Name = "BTPreventbot";
            this.BTPreventbot.Size = new System.Drawing.Size(169, 34);
            this.BTPreventbot.TabIndex = 63;
            this.BTPreventbot.Text = "Proccessing crawl";
            this.BTPreventbot.UseVisualStyleBackColor = true;
            this.BTPreventbot.Click += new System.EventHandler(this.BTPreventbot_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BTPreventbot);
            this.groupBox2.Location = new System.Drawing.Point(623, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 74);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Prevent bot";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(551, 77);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 17);
            this.label13.TabIndex = 66;
            this.label13.Text = "second";
            // 
            // txtdelay
            // 
            this.txtdelay.Location = new System.Drawing.Point(509, 73);
            this.txtdelay.Name = "txtdelay";
            this.txtdelay.Size = new System.Drawing.Size(48, 23);
            this.txtdelay.TabIndex = 65;
            this.txtdelay.Text = "5";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(469, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 17);
            this.label12.TabIndex = 64;
            this.label12.Text = "Delay";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1414, 802);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtdelay);
            this.Controls.Add(this.BTLoadlinks);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbnumsave);
            this.Controls.Add(this.txtnumscroll);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.BTSavelinks);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BTBrowser);
            this.Controls.Add(this.txtpathsavefile);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbnumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblinks);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.BTGetLinks);
            this.Controls.Add(this.BTProcessing);
            this.Controls.Add(this.txtcssclass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtinputlink);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Auto Crawl HTML";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtinputlink;
        private System.Windows.Forms.Button BTProcessing;
        private System.Windows.Forms.Button BTGetLinks;
        private System.Windows.Forms.ListBox lblinks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtcssclass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbnumber;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtpathsavefile;
        private System.Windows.Forms.Button BTBrowser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BTSavelinks;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtnumscroll;
        private System.Windows.Forms.Label lbnumsave;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtcssshowmore;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtnextpaging;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtnumclick;
        private System.Windows.Forms.GroupBox groupBox1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.RadioButton rdpaging;
        private System.Windows.Forms.RadioButton rdShowmore;
        private System.Windows.Forms.Button BTLoadlinks;
        private System.Windows.Forms.Button BTPreventbot;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtdelay;
        private System.Windows.Forms.Label label12;
    }
}

