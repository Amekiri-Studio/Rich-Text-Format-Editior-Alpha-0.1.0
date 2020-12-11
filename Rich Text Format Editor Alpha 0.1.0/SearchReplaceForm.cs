using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad_Alpha_0._1._0
{
    public partial class SearchReplaceForm : Form
    {
        public SearchReplaceForm()
        {
            InitializeComponent();
        }

        private void SearchRelpaceForm_Load(object sender, EventArgs e)
        {

        }
        public void FindText(RichTextBox rtb, string text)
        {
            rtb.HideSelection = false;
            int searchStartPosition = rtb.SelectionStart;
            if (rtb.SelectedText.Length > 0)
            {
                searchStartPosition = rtb.SelectionStart + rtb.SelectedText.Length;
            }
            int indexOfText = rtb.Find(text, searchStartPosition, RichTextBoxFinds.None);
            if (indexOfText >= 0)
            {
                searchStartPosition = indexOfText + rtb.SelectionLength;
                rtb.Select(indexOfText, rtb.SelectionLength);
            }
            else
            {
                MessageBox.Show(String.Format("找不到“{0}”...", text));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindText(Form1.rtb, textBox1.Text);
        }
    }
}
