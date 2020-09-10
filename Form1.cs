using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace NonogramResolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            textBox1.Visible = false;
            string html;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Consts.url + textBox1.Text);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            Nonogram nonogram = new Nonogram();
            nonogram.CreateNonogram(html, this);
            Refresh();
            nonogram.Solve(this);
        }
    }
}