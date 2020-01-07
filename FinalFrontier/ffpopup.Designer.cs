namespace FinalFrontier
{
    partial class ffpopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ffpopup));
            this.ffpopuppanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.seclevel_id_label = new System.Windows.Forms.Label();
            this.seclevel_doc_label = new System.Windows.Forms.Label();
            this.seclevel_link_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ffpopuppanel
            // 
            this.ffpopuppanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ffpopuppanel.Location = new System.Drawing.Point(12, 264);
            this.ffpopuppanel.Name = "ffpopuppanel";
            this.ffpopuppanel.Size = new System.Drawing.Size(619, 416);
            this.ffpopuppanel.TabIndex = 0;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::FinalFrontier.Properties.Resources.icon_link;
            this.pictureBox3.InitialImage = global::FinalFrontier.Properties.Resources.icon_link;
            this.pictureBox3.Location = new System.Drawing.Point(431, 12);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(200, 200);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 3;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::FinalFrontier.Properties.Resources.icon_doc;
            this.pictureBox2.InitialImage = global::FinalFrontier.Properties.Resources.icon_doc;
            this.pictureBox2.Location = new System.Drawing.Point(218, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(200, 200);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FinalFrontier.Properties.Resources.icon_id;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // seclevel_id_label
            // 
            this.seclevel_id_label.Location = new System.Drawing.Point(13, 219);
            this.seclevel_id_label.Name = "seclevel_id_label";
            this.seclevel_id_label.Size = new System.Drawing.Size(200, 25);
            this.seclevel_id_label.TabIndex = 4;
            this.seclevel_id_label.Text = "label1";
            this.seclevel_id_label.Click += new System.EventHandler(this.seclevel_id_label_Click);
            // 
            // seclevel_doc_label
            // 
            this.seclevel_doc_label.Location = new System.Drawing.Point(218, 219);
            this.seclevel_doc_label.Name = "seclevel_doc_label";
            this.seclevel_doc_label.Size = new System.Drawing.Size(200, 25);
            this.seclevel_doc_label.TabIndex = 5;
            this.seclevel_doc_label.Text = "label2";
            // 
            // seclevel_link_label
            // 
            this.seclevel_link_label.Location = new System.Drawing.Point(431, 219);
            this.seclevel_link_label.Name = "seclevel_link_label";
            this.seclevel_link_label.Size = new System.Drawing.Size(200, 25);
            this.seclevel_link_label.TabIndex = 6;
            this.seclevel_link_label.Text = "label3";
            // 
            // ffpopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 681);
            this.Controls.Add(this.seclevel_link_label);
            this.Controls.Add(this.seclevel_doc_label);
            this.Controls.Add(this.seclevel_id_label);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ffpopuppanel);
            this.Name = "ffpopup";
            this.Text = "ffpopup";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ffpopuppanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label seclevel_id_label;
        private System.Windows.Forms.Label seclevel_doc_label;
        private System.Windows.Forms.Label seclevel_link_label;
    }
}