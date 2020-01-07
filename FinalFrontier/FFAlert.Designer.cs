namespace FinalFrontier
{
    partial class FFAlert
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
            this.alertHeadline = new System.Windows.Forms.Label();
            this.alertContent = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // alertHeadline
            // 
            this.alertHeadline.AutoSize = true;
            this.alertHeadline.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.14286F);
            this.alertHeadline.Location = new System.Drawing.Point(13, 13);
            this.alertHeadline.Name = "alertHeadline";
            this.alertHeadline.Size = new System.Drawing.Size(109, 39);
            this.alertHeadline.TabIndex = 0;
            this.alertHeadline.Text = "label1";
            // 
            // alertContent
            // 
            this.alertContent.Enabled = false;
            this.alertContent.Location = new System.Drawing.Point(18, 65);
            this.alertContent.Multiline = true;
            this.alertContent.Name = "alertContent";
            this.alertContent.ReadOnly = true;
            this.alertContent.Size = new System.Drawing.Size(403, 159);
            this.alertContent.TabIndex = 1;
            // 
            // FFAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(514, 236);
            this.Controls.Add(this.alertContent);
            this.Controls.Add(this.alertHeadline);
            this.Name = "FFAlert";
            this.Text = "!!!WARNUNG!!! [FinalFrontier]";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label alertHeadline;
        private System.Windows.Forms.TextBox alertContent;
    }
}