using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;

namespace FinalFrontier
{
    public partial class ffpopup : Form
    {
        public ffpopup(Analyzer ana)
        {
            InitializeComponent();

            /*
            Image image = Image.FromFile("Resources/icon_id.png");
            pictureBox1.Image = image;
            pictureBox1.Height = image.Height;
            pictureBox1.Width = image.Width;

            image = Image.FromFile("Resources/icon_doc.png");
            pictureBox2.Image = image;
            pictureBox2.Height = image.Height;
            pictureBox2.Width = image.Width;

            image = Image.FromFile("Resources/icon_link.png");
            pictureBox3.Image = image;
            pictureBox3.Height = image.Height;
            pictureBox3.Width = image.Width;
            */
            Label tmp;

            tmp = new Label();
            tmp.AutoSize = true;
            if (ana.score >= 20)
            {
                tmp.Text = "*** OK *** (" + ana.score + ")";
                tmp.BackColor = Color.Green;
            }

            if (ana.score < 20)
            {
                tmp.Text = "*** WARNING *** (" + ana.score + ")";
                tmp.BackColor = Color.Yellow;
            }
            if (ana.score < -20)
            {
                tmp.Text = "*** ALARM *** (" + ana.score + ")";
                tmp.BackColor = Color.Red;
            }

            seclevel_id_label.BackColor = Color.Green;
            seclevel_id_label.Text = "OK";

            if ((ana.senderNameContainsEmail == true) |
                (ana.isBadTldSender == true) |
                //(ana.DictSenderName.ContainsKey(ana.senderName) == false) |
                (ana.DictSenderEmail.ContainsKey(ana.senderEmailAddress) == false))
            {
                seclevel_id_label.BackColor = Color.Yellow;
                seclevel_id_label.Text = "*** WARNING ***";
            }

            if ((ana.domainMismatch == true) |
                (ana.isLookalike == true) 
                //(ana.DictSenderName.ContainsKey(ana.senderName) == false) |
                )
            {
                seclevel_id_label.BackColor = Color.Red;
                seclevel_id_label.Text = "*** ALERT ***";
            }


            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "Whitelisted: " + ana.isWhitelisted;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "SenderNameContainsEmail: " + ana.senderNameContainsEmail;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "Domain Mismatch in Sender Combo: " + ana.domainMismatch;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "BadTldSender: " + ana.isBadTldSender;
            ffpopuppanel.Controls.Add(tmp); 

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "Lookalike: " + ana.isLookalike;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            if (ana.DictSenderName.ContainsKey(ana.senderName))
            {
                tmp.Text += "SenderName seen before " + ana.DictSenderName[ana.senderName] + "x.";
            }
            else
            {
                tmp.Text += "SenderName never seen before.";
            }
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            if (ana.DictSenderEmail.ContainsKey(ana.senderEmailAddress))
            {
                tmp.Text += "SenderEmail seen before " + ana.DictSenderEmail[ana.senderEmailAddress] + "x.";
            }
            else
            {
                tmp.Text += "SenderEmail never seen before.";
            }
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "0 links contained";
            if (ana.links != null) tmp.Text = ana.links.Count + " links contained";
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "linkshorteners: " + ana.hasLinksWithShorteners;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "hasBadTldsInLinks: " + ana.hasBadTldsInLinks;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = ana.attachments.Count + " files attached";
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "hasBadExtensions: " + ana.hasbadextensions;
            ffpopuppanel.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "hasDoubleExtensions: " + ana.hasdoubleextensions;
            ffpopuppanel.Controls.Add(tmp);

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void seclevel_id_label_Click(object sender, EventArgs e)
        {

        }
    }
}
