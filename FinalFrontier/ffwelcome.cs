using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalFrontier
{
    public partial class ffwelcome : Form
    {
        public ffwelcome(Microsoft.Office.Interop.Outlook.Folder root)
        {
            InitializeComponent();
            ArrayList folders = new ArrayList();
            foreach (Microsoft.Office.Interop.Outlook.Folder childFolder in root.Folders)
            {
                folders.Add(childFolder.FolderPath);
            }
            listBox1.DataSource = folders;
        }

        private void ffwelcome_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
