namespace FinalFrontier
{
    partial class ffinfo
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
            this.flowLayoutPanelMeta = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.flowLayoutPanelLinks = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanelAttachments = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flowLayoutPanelMeta
            // 
            this.flowLayoutPanelMeta.AutoSize = true;
            this.flowLayoutPanelMeta.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelMeta.Location = new System.Drawing.Point(12, 149);
            this.flowLayoutPanelMeta.Name = "flowLayoutPanelMeta";
            this.flowLayoutPanelMeta.Size = new System.Drawing.Size(1027, 105);
            this.flowLayoutPanelMeta.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sender";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "TODO";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "Metadata Analysis";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 456);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "Attachment Analysis";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 275);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "Link Analysis";
            // 
            // flowLayoutPanelLinks
            // 
            this.flowLayoutPanelLinks.AutoSize = true;
            this.flowLayoutPanelLinks.Location = new System.Drawing.Point(12, 323);
            this.flowLayoutPanelLinks.Name = "flowLayoutPanelLinks";
            this.flowLayoutPanelLinks.Size = new System.Drawing.Size(1027, 100);
            this.flowLayoutPanelLinks.TabIndex = 6;
            // 
            // flowLayoutPanelAttachments
            // 
            this.flowLayoutPanelAttachments.AutoSize = true;
            this.flowLayoutPanelAttachments.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelAttachments.Location = new System.Drawing.Point(12, 500);
            this.flowLayoutPanelAttachments.Name = "flowLayoutPanelAttachments";
            this.flowLayoutPanelAttachments.Size = new System.Drawing.Size(1027, 100);
            this.flowLayoutPanelAttachments.TabIndex = 7;
            // 
            // ffinfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1051, 684);
            this.Controls.Add(this.flowLayoutPanelAttachments);
            this.Controls.Add(this.flowLayoutPanelLinks);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.flowLayoutPanelMeta);
            this.Name = "ffinfo";
            this.Text = "FinalFrontier Security Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMeta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLinks;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelAttachments;
    }
}