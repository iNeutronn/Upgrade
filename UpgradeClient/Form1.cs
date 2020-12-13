using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Windows.Forms;
using UpgradeServer;

namespace UpgradeClient
{
    
    public partial class Form1 : Form
    {
        static List<Point> points;
        public static string IPAdress;
        public static string NamePlayer;
        const int port= 3234;
        List<Player> players = new List<Player>(); 
        public Form1()
        {
            StreamReader sr = new StreamReader("points.txt");
            points = JsonSerializer.Deserialize<List<Point>>(sr.ReadLine());
            InitializeComponent();
        }

        void GetData()
        {
            string inp = Request("GetData");
            players = JsonSerializer.Deserialize<List<Player>>(inp);

        }
        string Request(string request)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(IPAdress), port));
            socket.Send(Encoding.UTF8.GetBytes(request));
            byte[] data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return builder.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupDialogForm form = new SetupDialogForm();
            var dia =  form.ShowDialog();
            
            Request("Connect "+NamePlayer);
            timer1.Start();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string s = Request("GetLog");
            List<string> Log = JsonSerializer.Deserialize<List<string>>(s);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Log.ToArray());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Request("Disconect "+ NamePlayer);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
           
        }
    }
}
