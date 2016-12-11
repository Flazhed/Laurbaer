using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SoapClient.laurbaer.loanbroker;

namespace SoapClient
{
    public partial class Form1 : Form
    {
        private LaurbaerServiceSoapClient laurbaer;

        public Form1()
        {
            InitializeComponent();
            laurbaer = new LaurbaerServiceSoapClient();
            //Bad bad, but in this small client its ok. Read: http://stackoverflow.com/questions/10775367/cross-thread-operation-not-valid-control-textbox1-accessed-from-a-thread-othe
            TextBox.CheckForIllegalCrossThreadCalls = false;
            textBoxSsn.Text = RandomString(10);
            textBoxAmount.Text = RandomString(4);
            textBoxDuration.Text = "10";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rate = "Working";

            //Timeout set to 1min.
            //TODO: uncomment
            new Thread(delegate ()
            {
                rate = laurbaer.SoapLoanRequest(textBoxSsn.Text, float.Parse(textBoxAmount.Text),
                    float.Parse(textBoxDuration.Text));

                textBox1.Text = rate;
            }).Start();

            textBox1.Text = rate;
            textBoxSsn.Text = RandomString(10);
            textBoxAmount.Text = RandomString(4);
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "123456789";
            string ran = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (length == 10)
            {
                return ran.Insert(6, "-");
            }

            return ran.Insert(2, ".");
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void process1_Exited(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}