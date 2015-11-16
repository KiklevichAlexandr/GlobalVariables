using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalVariables
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          OpenFileDialog filedialog = new OpenFileDialog();
          filedialog.Filter = "Текстовый файл|*.txt";
          filedialog.Title = "Выберите файл с исходным текстом";
            if (filedialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.richTextBox1.Text = File.ReadAllText(filedialog.FileName);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            METRICS metrix = new METRICS(richTextBox1.Text);
            richTextBox2.Text = richTextBox2.Text + "\n" + metrix.CalculateMetrix();
        }
    }
}
