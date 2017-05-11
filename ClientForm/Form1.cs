using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private StreamWriter w;
        private StreamReader r;
        private bool isConnected;
        private string Nome;
        private string IP;

        public Form1()
        {
            InitializeComponent();            
        }

        private void ReadData()
        {
            while (isConnected)
            {
                string data = null;
                byte[] buff = null;
                buff = new byte[1024];
                int count = client.GetStream().Read(buff, 0, buff.Length);

                if (count > 0)
                {
                    data = Encoding.UTF8.GetString(buff);
                    this.InvokeEx(t => t.listBox1.Items.Add(data.Trim()));
                }

                isConnected = client.Client.IsBound;
            }
        }


        private void WriteData()
        {
            w = new StreamWriter(client.GetStream(), Encoding.UTF8);
            string data = null;
            if (isConnected && txbInput.Text.Trim().Length > 0)
            {
                this.InvokeEx(t => data = this.Nome + ": " + t.txbInput.Text.Trim());
                this.InvokeEx(t => t.txbInput.Text = "");
                w.WriteLine(data);
                w.Flush();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WriteData();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtIP.Text.Trim().Length > 0 && txtNome.Text.Trim().Length > 0)
            {
                this.IP = txtIP.Text.Trim();
                this.Nome = txtNome.Text.Trim();
                txtNome.Enabled = false;
                button1.Enabled = true;


                try
                {
                    client = new TcpClient();
                    client.Connect(this.IP, 2929);

                    Thread reader = new Thread(ReadData);

                    isConnected = true;
                    reader.Start();

                    lblStatus.Text = "Conectado!";
                    lblStatus.ForeColor = Color.Green;
                }
                catch (Exception)
                {
                    isConnected = false;
                    lblStatus.Text = "Servidor offline.";
                    lblStatus.ForeColor = Color.Red;
                }
            }
        }

        private void txbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
