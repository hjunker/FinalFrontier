using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalFrontier
{
    public partial class ffinfo : Form
    {
        public ffinfo(Analyzer ana)
        {
            Label tmp;
            InitializeComponent();
            this.label2.Text = ana.senderName + " / " + ana.senderEmailAddress;
            
            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "Whitelisted: " + ana.isWhitelisted;
            flowLayoutPanelMeta.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "Domain Mismatch in Sender Combo: " + ana.domainMismatch;
            flowLayoutPanelMeta.Controls.Add(tmp);

            tmp = new Label();
            tmp.AutoSize = true;
            tmp.Text = "Lookalike: " + ana.isLookalike;
            flowLayoutPanelMeta.Controls.Add(tmp);

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
            flowLayoutPanelMeta.Controls.Add(tmp);

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
            flowLayoutPanelMeta.Controls.Add(tmp);

            if (ana.links != null)
            {
                foreach (HtmlNode node in ana.links)
                {
                    tmp = new Label();
                    tmp.Text = node.GetAttributeValue("href", null);
                    tmp.AutoSize = true;
                    flowLayoutPanelLinks.Controls.Add(tmp);
                    // TODO: Check for unwanted TLDs (.date, ...)
                    // TODO: check for lookalikes (similar domainnames for phishing)
                }
            }

            // TODO: enumerate attachments and their analysis results
            if (ana.attachments != null)
            {
                foreach (Attachment attachment in ana.attachments)
                {
                    tmp = new Label();
                    tmp.Text = attachment.DisplayName + " / " + attachment.FileName + " / " + attachment.Type;
                    tmp.AutoSize = true;
                    flowLayoutPanelAttachments.Controls.Add(tmp);
                }
            }

            
            this.Refresh();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
