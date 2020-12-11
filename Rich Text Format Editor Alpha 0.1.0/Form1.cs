using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;

namespace Notepad_Alpha_0._1._0
{
    public partial class Form1 : Form
    {
        bool IsFileChange = false;
        string path = ""; //文件目录
        string filetype;
        string capture = "Rich Text Format File Editor Alpha 0.1.0";
        string default_path;
        string program_path;
        int width, height;
        public static RichTextBox rtb;
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        //读取ini文件 section表示ini文件中的节点名，key表示键名 def没有查到的话返回的默认值 filePath文件路径
        public string Read(string section, string key, string def, string filePath)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, sb, 1024, filePath);
            return sb.ToString();
        }

        //写入ini文件 section表示ini文件中的节点名，key表示键名 value写入的值 filePath文件路径
        public int Write(string section, string key, string value, string filePath)
        {
            //CheckPath(filePath);
            return WritePrivateProfileString(section, key, value, filePath);
        }


        //删除section 
        public int DeleteSection(string section, string filePath)
        {
            return Write(section, null, null, filePath);
        }


        //删除键
        public int DeleteKey(string section, string key, string filePath)
        {
            return Write(section, key, null, filePath);
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] dstr;
            openFileDialog1.InitialDirectory = default_path;
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
            {
                path = openFileDialog1.FileName;
                dstr = path.Split('.');
                filetype = dstr[dstr.Length - 1].ToLower();
                if (filetype == "rtf")
                {
                    richTextBox1.LoadFile(path, RichTextBoxStreamType.RichText);
                    this.Text = path + " - " + capture;
                    IsFileChange = false;
                }
                else
                {
                    //MessageBox.Show(this, "文件类型错误！", "错误：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    richTextBox1.LoadFile(path, RichTextBoxStreamType.PlainText);
                    this.Text = path + " - " + capture;
                    IsFileChange = false;
                }
                //richTextBox1.LoadFile(path);
                dstr = null;
                dstr = path.Split('\\');
                if (default_path != null)
                {
                    default_path = null;
                }
                for (int i = 0;i < dstr.Length - 1; i++)
                {
                    default_path = default_path + dstr[i] + "\\";
                }
                //MessageBox.Show(default_path);
                Write("General", "default_path", default_path, program_path + "\\config.ini");
                Write("General", "filetype", filetype, program_path + "\\config.ini");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string tmp_w, tmp_h;
            richTextBox1.Multiline = true;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.Text = capture;
            program_path = Application.StartupPath;
            default_path = Read("General", "default_path", program_path + "\\", program_path + "\\config.ini");
            filetype = Read("General", "filetype", "rtf", program_path + "\\config.ini");
            tmp_w = Read("Size", "width", "818", program_path + "\\config.ini");
            tmp_h = Read("Size", "height", "497", program_path + "\\config.ini");
            width = int.Parse(tmp_w);
            height = int.Parse(tmp_h);
            openFileDialog1.Filter = "RTF文件|*.rtf|文本文件|*.txt|所有文件|*.*";
            saveFileDialog1.Filter = "RTF文件|*.rtf|文本文件|*.txt|所有文件|*.*";
            openFileDialog2.Filter = "图像文件|*.jpg,*.jpeg,*.png,*.bmp";
            查找上一个VToolStripMenuItem.Enabled = false;
            查找下一个NToolStripMenuItem.Enabled = false;
            this.Width = width;
            this.Height = height;
            rtb = richTextBox1;
            Form1_SizeChanged(sender, e);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Form f = (Form)sender;
            richTextBox1.Top = 30;
            richTextBox1.Left = 0;
            richTextBox1.Height = this.Height - 30 - statusStrip1.Height - 48;
            richTextBox1.Width = this.Width - 19;
            width = f.Width;
            height = f.Height;
        }

        private void 撤销UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void 粘贴pToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                粘贴pToolStripMenuItem.Enabled = true;
                richTextBox1.Paste();
            }
            else
            {
                粘贴pToolStripMenuItem.Enabled = false;
            }
        }

        private void 重做RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void 剪切CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
            {
                richTextBox1.Cut();
            }
        }

        private void 复制CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Equals("")) return;
            Clipboard.SetDataObject(richTextBox1.SelectedText, true);
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                粘贴pToolStripMenuItem.Enabled = true;
            }
            else
            {
                粘贴pToolStripMenuItem.Enabled = false;
            }
        }

        private void 删除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
                richTextBox1.SelectedText = "";
        }

        private void 全选AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            IsFileChange = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr;
            if (IsFileChange)
            {
                dr = MessageBox.Show(this, "文件已经修改，是否保存？", "提示：", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dr == DialogResult.OK)
                {
                    保存SToolStripMenuItem_Click(sender, e);
                }
            }
            Write("Size", "width", width.ToString(), program_path + "\\config.ini");
            Write("Size", "height", height.ToString(), program_path + "\\config.ini");
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] dstr;
            saveFileDialog1.Filter = "RTF文件|*.rtf|文本文件|*.txt|所有文件|*.*";
            if (path == "")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Length > 0) 
                {
                    path = saveFileDialog1.FileName;
                    dstr = path.Split('.');
                    filetype = dstr[dstr.Length - 1];
                    if (filetype == "rtf")
                    {
                        richTextBox1.SaveFile(path, RichTextBoxStreamType.RichText);
                        this.Text = path + " - " + capture;
                        IsFileChange = false;
                    }
                    else
                    {
                        richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
                        this.Text = path + " - " + capture;
                        IsFileChange = false;
                    }
                }
            }
            else
            {
                if (filetype == "rtf")
                {
                    richTextBox1.SaveFile(path, RichTextBoxStreamType.RichText);
                    this.Text = path + " - " + capture;
                    IsFileChange = false;
                }
                else
                {
                    richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
                    this.Text = path + " - " + capture;
                    IsFileChange = false;
                }
            }
        }

        private void 退出CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFileChange)
            {
                DialogResult dr = MessageBox.Show(this, "文件已修改，是否需要保存？", "信息：", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    保存SToolStripMenuItem_Click(sender, e);
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
            else this.Close();
        }

        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFileChange)
            {
                DialogResult dr = MessageBox.Show(this, "文件已修改，是否需要保存？", "信息：", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    保存SToolStripMenuItem_Click(sender, e);
                    this.Text = capture;
                    richTextBox1.Clear();
                }
                else if (dr == DialogResult.No) 
                {
                    richTextBox1.Clear();
                }
            }
        }

        private void 更改文本字体FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
            {
                DialogResult dr;
                dr = fontDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    richTextBox1.SelectionFont = fontDialog1.Font;
                }
            }
        }

        private void gitHubGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string target = "https://github.com/Amekiri-Studio/Rich-Text-Format-Editior-Alpha-0.1.0";
            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void 发送反馈FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "无法找到相关文件！请检查程序及相关文件是否完整！", "错误：", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 查看帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "无法找到相关文件！请检查程序及相关文件是否完整！", "错误：", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void 更改文字颜色CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
            {
                DialogResult dr;
                colorDialog1.AllowFullOpen = true;
                dr = colorDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    richTextBox1.SelectionColor = colorDialog1.Color;
                }
            }
        }

        private void 更改文字背景颜色BToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
            {
                DialogResult dr;
                colorDialog1.AllowFullOpen = true;
                dr = colorDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    richTextBox1.SelectionBackColor = colorDialog1.Color;
                }
            }
        }

        private void 插入图片PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = openFileDialog2.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Clipboard.SetDataObject(Image.FromFile(openFileDialog2.FileName), false);
                richTextBox1.Paste();
            }
        }

        private void 查找SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new SearchReplaceForm();
            f.Show();
        }

        /*public static string GetIERunString()
{
   //string IEString;
   //Registry regKey = Registry.ClassesRoot;
   //regKey = regKey.OpenSubKey();
   //return IEString;
}*/
        
    }
}
