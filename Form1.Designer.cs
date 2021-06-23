namespace WINSTLD
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
            this.components = new System.ComponentModel.Container();
            this.btn_process = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lbl_location = new System.Windows.Forms.Label();
            this.lbl_business = new System.Windows.Forms.Label();
            this.Btn_rename = new System.Windows.Forms.Button();
            this.lbl_STLD = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.btn_unzip = new System.Windows.Forms.Button();
            this.lbl_nameval = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BTN_OTEHR = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.BTNPRoductDb = new System.Windows.Forms.Button();
            this.Btn_Convert = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.BTNTIMERNETWORK = new System.Windows.Forms.Button();
            this.BTN_PMIX = new System.Windows.Forms.Button();
            this.pmix_stld = new System.Windows.Forms.Label();
            this.pmix_location = new System.Windows.Forms.Label();
            this.lbl_stld_pmix = new System.Windows.Forms.Label();
            this.pmix_file = new System.Windows.Forms.Label();
            this.pmix_businessdate = new System.Windows.Forms.Label();
            this.pmixcount = new System.Windows.Forms.Label();
            this.btn_renamepmix = new System.Windows.Forms.Button();
            this.lbl_re = new System.Windows.Forms.Label();
            this.btn_way_pmix = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.BTN_CLEARLBLES = new System.Windows.Forms.Button();
            this.btn_Product = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_process
            // 
            this.btn_process.Location = new System.Drawing.Point(12, 189);
            this.btn_process.Name = "btn_process";
            this.btn_process.Size = new System.Drawing.Size(536, 23);
            this.btn_process.TabIndex = 0;
            this.btn_process.Text = "Orginal Process";
            this.btn_process.UseVisualStyleBackColor = true;
            this.btn_process.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 218);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(536, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lbl_location
            // 
            this.lbl_location.AutoSize = true;
            this.lbl_location.Location = new System.Drawing.Point(43, 43);
            this.lbl_location.Name = "lbl_location";
            this.lbl_location.Size = new System.Drawing.Size(48, 13);
            this.lbl_location.TabIndex = 2;
            this.lbl_location.Text = "Location";
            this.lbl_location.Click += new System.EventHandler(this.label1_Click);
            // 
            // lbl_business
            // 
            this.lbl_business.AutoSize = true;
            this.lbl_business.Location = new System.Drawing.Point(43, 66);
            this.lbl_business.Name = "lbl_business";
            this.lbl_business.Size = new System.Drawing.Size(71, 13);
            this.lbl_business.TabIndex = 3;
            this.lbl_business.Text = "Datebusiness";
            // 
            // Btn_rename
            // 
            this.Btn_rename.Location = new System.Drawing.Point(13, 99);
            this.Btn_rename.Name = "Btn_rename";
            this.Btn_rename.Size = new System.Drawing.Size(303, 55);
            this.Btn_rename.TabIndex = 4;
            this.Btn_rename.Text = "Manual Process";
            this.Btn_rename.UseVisualStyleBackColor = true;
            this.Btn_rename.Click += new System.EventHandler(this.Btn_rename_Click);
            // 
            // lbl_STLD
            // 
            this.lbl_STLD.AutoSize = true;
            this.lbl_STLD.Location = new System.Drawing.Point(43, 19);
            this.lbl_STLD.Name = "lbl_STLD";
            this.lbl_STLD.Size = new System.Drawing.Size(63, 13);
            this.lbl_STLD.TabIndex = 5;
            this.lbl_STLD.Text = "STLDName";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(174, 19);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(69, 13);
            this.lblCount.TabIndex = 6;
            this.lblCount.Text = "Recordcount";
            this.lblCount.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // btn_unzip
            // 
            this.btn_unzip.Location = new System.Drawing.Point(321, 99);
            this.btn_unzip.Margin = new System.Windows.Forms.Padding(2);
            this.btn_unzip.Name = "btn_unzip";
            this.btn_unzip.Size = new System.Drawing.Size(227, 55);
            this.btn_unzip.TabIndex = 7;
            this.btn_unzip.Text = "RenameingADC";
            this.btn_unzip.UseVisualStyleBackColor = true;
            this.btn_unzip.Click += new System.EventHandler(this.btn_unzip_Click);
            // 
            // lbl_nameval
            // 
            this.lbl_nameval.AutoSize = true;
            this.lbl_nameval.Location = new System.Drawing.Point(309, 19);
            this.lbl_nameval.Name = "lbl_nameval";
            this.lbl_nameval.Size = new System.Drawing.Size(63, 13);
            this.lbl_nameval.TabIndex = 8;
            this.lbl_nameval.Text = "Filenameval";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(290, 283);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(259, 50);
            this.button1.TabIndex = 9;
            this.button1.Text = "Unzipfiles-Process";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button3
            // 
            this.button3.ForeColor = System.Drawing.Color.Red;
            this.button3.Location = new System.Drawing.Point(13, 283);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(259, 50);
            this.button3.TabIndex = 10;
            this.button3.Text = "Delete All XML";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "label1";
            // 
            // BTN_OTEHR
            // 
            this.BTN_OTEHR.Location = new System.Drawing.Point(554, 99);
            this.BTN_OTEHR.Name = "BTN_OTEHR";
            this.BTN_OTEHR.Size = new System.Drawing.Size(303, 55);
            this.BTN_OTEHR.TabIndex = 12;
            this.BTN_OTEHR.Text = "RenameGTWA STLD";
            this.BTN_OTEHR.UseVisualStyleBackColor = true;
            this.BTN_OTEHR.Click += new System.EventHandler(this.button4_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(554, 185);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(303, 55);
            this.button4.TabIndex = 13;
            this.button4.Text = "Waysation STLD";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // BTNPRoductDb
            // 
            this.BTNPRoductDb.Location = new System.Drawing.Point(554, 283);
            this.BTNPRoductDb.Margin = new System.Windows.Forms.Padding(2);
            this.BTNPRoductDb.Name = "BTNPRoductDb";
            this.BTNPRoductDb.Size = new System.Drawing.Size(302, 50);
            this.BTNPRoductDb.TabIndex = 14;
            this.BTNPRoductDb.Text = "Product DB and Names DB";
            this.BTNPRoductDb.UseVisualStyleBackColor = true;
            this.BTNPRoductDb.Click += new System.EventHandler(this.BTNPRoductDb_Click);
            // 
            // Btn_Convert
            // 
            this.Btn_Convert.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Convert.ForeColor = System.Drawing.Color.Red;
            this.Btn_Convert.Location = new System.Drawing.Point(13, 353);
            this.Btn_Convert.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Convert.Name = "Btn_Convert";
            this.Btn_Convert.Size = new System.Drawing.Size(844, 37);
            this.Btn_Convert.TabIndex = 15;
            this.Btn_Convert.Text = "Start Convert Process";
            this.Btn_Convert.UseVisualStyleBackColor = true;
            this.Btn_Convert.Click += new System.EventHandler(this.Btn_Convert_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 395);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(842, 19);
            this.progressBar1.TabIndex = 16;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 90000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // BTNTIMERNETWORK
            // 
            this.BTNTIMERNETWORK.Location = new System.Drawing.Point(862, 99);
            this.BTNTIMERNETWORK.Margin = new System.Windows.Forms.Padding(2);
            this.BTNTIMERNETWORK.Name = "BTNTIMERNETWORK";
            this.BTNTIMERNETWORK.Size = new System.Drawing.Size(271, 52);
            this.BTNTIMERNETWORK.TabIndex = 17;
            this.BTNTIMERNETWORK.Text = "Rename Work Incase Network issue";
            this.BTNTIMERNETWORK.UseVisualStyleBackColor = true;
            this.BTNTIMERNETWORK.Click += new System.EventHandler(this.BTNTIMERNETWORK_Click);
            // 
            // BTN_PMIX
            // 
            this.BTN_PMIX.Location = new System.Drawing.Point(870, 185);
            this.BTN_PMIX.Margin = new System.Windows.Forms.Padding(2);
            this.BTN_PMIX.Name = "BTN_PMIX";
            this.BTN_PMIX.Size = new System.Drawing.Size(56, 49);
            this.BTN_PMIX.TabIndex = 18;
            this.BTN_PMIX.Text = "Btn_PMIX";
            this.BTN_PMIX.UseVisualStyleBackColor = true;
            this.BTN_PMIX.Click += new System.EventHandler(this.BTN_PMIX_Click);
            // 
            // pmix_stld
            // 
            this.pmix_stld.AutoSize = true;
            this.pmix_stld.Location = new System.Drawing.Point(1033, 314);
            this.pmix_stld.Name = "pmix_stld";
            this.pmix_stld.Size = new System.Drawing.Size(61, 13);
            this.pmix_stld.TabIndex = 20;
            this.pmix_stld.Text = "PMIXSTLD";
            // 
            // pmix_location
            // 
            this.pmix_location.AutoSize = true;
            this.pmix_location.Location = new System.Drawing.Point(959, 314);
            this.pmix_location.Name = "pmix_location";
            this.pmix_location.Size = new System.Drawing.Size(76, 13);
            this.pmix_location.TabIndex = 19;
            this.pmix_location.Text = "Pmix_Location";
            // 
            // lbl_stld_pmix
            // 
            this.lbl_stld_pmix.AutoSize = true;
            this.lbl_stld_pmix.Location = new System.Drawing.Point(1036, 283);
            this.lbl_stld_pmix.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_stld_pmix.Name = "lbl_stld_pmix";
            this.lbl_stld_pmix.Size = new System.Drawing.Size(49, 13);
            this.lbl_stld_pmix.TabIndex = 21;
            this.lbl_stld_pmix.Text = "lbl_PMIX";
            // 
            // pmix_file
            // 
            this.pmix_file.AutoSize = true;
            this.pmix_file.Location = new System.Drawing.Point(1095, 274);
            this.pmix_file.Name = "pmix_file";
            this.pmix_file.Size = new System.Drawing.Size(63, 13);
            this.pmix_file.TabIndex = 22;
            this.pmix_file.Text = "pmix_ name";
            this.pmix_file.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pmix_businessdate
            // 
            this.pmix_businessdate.AutoSize = true;
            this.pmix_businessdate.Location = new System.Drawing.Point(959, 283);
            this.pmix_businessdate.Name = "pmix_businessdate";
            this.pmix_businessdate.Size = new System.Drawing.Size(55, 13);
            this.pmix_businessdate.TabIndex = 23;
            this.pmix_businessdate.Text = "pmix_date";
            // 
            // pmixcount
            // 
            this.pmixcount.AutoSize = true;
            this.pmixcount.Location = new System.Drawing.Point(959, 301);
            this.pmixcount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pmixcount.Name = "pmixcount";
            this.pmixcount.Size = new System.Drawing.Size(55, 13);
            this.pmixcount.TabIndex = 24;
            this.pmixcount.Text = "pmixcount";
            // 
            // btn_renamepmix
            // 
            this.btn_renamepmix.Location = new System.Drawing.Point(870, 344);
            this.btn_renamepmix.Margin = new System.Windows.Forms.Padding(2);
            this.btn_renamepmix.Name = "btn_renamepmix";
            this.btn_renamepmix.Size = new System.Drawing.Size(227, 55);
            this.btn_renamepmix.TabIndex = 25;
            this.btn_renamepmix.Text = "renamepmix";
            this.btn_renamepmix.UseVisualStyleBackColor = true;
            this.btn_renamepmix.Click += new System.EventHandler(this.btn_renamepmix_Click);
            // 
            // lbl_re
            // 
            this.lbl_re.AutoSize = true;
            this.lbl_re.Location = new System.Drawing.Point(1095, 301);
            this.lbl_re.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_re.Name = "lbl_re";
            this.lbl_re.Size = new System.Drawing.Size(63, 13);
            this.lbl_re.TabIndex = 26;
            this.lbl_re.Text = "lbl_Rename";
            // 
            // btn_way_pmix
            // 
            this.btn_way_pmix.Location = new System.Drawing.Point(1109, 215);
            this.btn_way_pmix.Margin = new System.Windows.Forms.Padding(2);
            this.btn_way_pmix.Name = "btn_way_pmix";
            this.btn_way_pmix.Size = new System.Drawing.Size(56, 19);
            this.btn_way_pmix.TabIndex = 27;
            this.btn_way_pmix.Text = "Btn_wayPMIX";
            this.btn_way_pmix.UseVisualStyleBackColor = true;
            this.btn_way_pmix.Click += new System.EventHandler(this.btn_way_pmix_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(870, 418);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(227, 55);
            this.button5.TabIndex = 28;
            this.button5.Text = "renameOstld";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // BTN_CLEARLBLES
            // 
            this.BTN_CLEARLBLES.Location = new System.Drawing.Point(862, 19);
            this.BTN_CLEARLBLES.Name = "BTN_CLEARLBLES";
            this.BTN_CLEARLBLES.Size = new System.Drawing.Size(223, 23);
            this.BTN_CLEARLBLES.TabIndex = 29;
            this.BTN_CLEARLBLES.Text = "Clear All Lables and components";
            this.BTN_CLEARLBLES.UseVisualStyleBackColor = true;
            this.BTN_CLEARLBLES.Click += new System.EventHandler(this.BTN_CLEARLBLES_Click);
            // 
            // btn_Product
            // 
            this.btn_Product.Location = new System.Drawing.Point(432, 438);
            this.btn_Product.Name = "btn_Product";
            this.btn_Product.Size = new System.Drawing.Size(186, 23);
            this.btn_Product.TabIndex = 30;
            this.btn_Product.Text = "NP6PRODUCTDB";
            this.btn_Product.UseVisualStyleBackColor = true;
            this.btn_Product.Click += new System.EventHandler(this.btn_Product_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 479);
            this.Controls.Add(this.btn_Product);
            this.Controls.Add(this.BTN_CLEARLBLES);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btn_way_pmix);
            this.Controls.Add(this.lbl_re);
            this.Controls.Add(this.btn_renamepmix);
            this.Controls.Add(this.pmixcount);
            this.Controls.Add(this.pmix_businessdate);
            this.Controls.Add(this.pmix_file);
            this.Controls.Add(this.lbl_stld_pmix);
            this.Controls.Add(this.pmix_stld);
            this.Controls.Add(this.pmix_location);
            this.Controls.Add(this.BTN_PMIX);
            this.Controls.Add(this.BTNTIMERNETWORK);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Btn_Convert);
            this.Controls.Add(this.BTNPRoductDb);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.BTN_OTEHR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbl_nameval);
            this.Controls.Add(this.btn_unzip);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lbl_STLD);
            this.Controls.Add(this.Btn_rename);
            this.Controls.Add(this.lbl_business);
            this.Controls.Add(this.lbl_location);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btn_process);
            this.Name = "Form1";
            this.Text = "Application";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_process;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lbl_location;
        private System.Windows.Forms.Label lbl_business;
        private System.Windows.Forms.Button Btn_rename;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btn_unzip;
        private System.Windows.Forms.Label lbl_nameval;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTN_OTEHR;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lbl_STLD;
        private System.Windows.Forms.Button BTNPRoductDb;
        private System.Windows.Forms.Button Btn_Convert;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button BTNTIMERNETWORK;
        private System.Windows.Forms.Button BTN_PMIX;
        private System.Windows.Forms.Label pmix_stld;
        private System.Windows.Forms.Label pmix_location;
        private System.Windows.Forms.Label lbl_stld_pmix;
        private System.Windows.Forms.Label pmix_file;
        private System.Windows.Forms.Label pmix_businessdate;
        private System.Windows.Forms.Label pmixcount;
        private System.Windows.Forms.Button btn_renamepmix;
        private System.Windows.Forms.Label lbl_re;
        private System.Windows.Forms.Button btn_way_pmix;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button BTN_CLEARLBLES;
        private System.Windows.Forms.Button btn_Product;
    }
}

