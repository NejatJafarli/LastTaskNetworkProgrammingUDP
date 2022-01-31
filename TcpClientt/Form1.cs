using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpClientt
{
    public partial class Form1 : Form
    {

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
        public UdpClient Client { get; set; } = new UdpClient(1111 + 5);

        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

        public Form1()
        {
            InitializeComponent();
        }

        private void btn1_Click(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() =>
            {
                var ip = IPAddress.Loopback;
                var port = 1111;
                TcpClient tcpClientt = new TcpClient();
                tcpClientt.Connect(ip, port);

                List<byte> data = new List<byte>();

                for (int i = 0; i < 10; i++)
                {
                    var bytes1 = Client.Receive(ref ep);
                    var bytes2 = Client.Receive(ref ep);
                    var bytes3 = Client.Receive(ref ep);
                    var bytes4 = Client.Receive(ref ep);

                    var DeCompressedBytes1 = Decompress(bytes1);
                    var DeCompressedBytes2 = Decompress(bytes2);
                    var DeCompressedBytes3 = Decompress(bytes3);
                    var DeCompressedBytes4 = Decompress(bytes4);

                    List<byte> vs = new List<byte>();
                    vs.AddRange(DeCompressedBytes1);
                    vs.AddRange(DeCompressedBytes2);
                    vs.AddRange(DeCompressedBytes3);
                    vs.AddRange(DeCompressedBytes4);
                    Image img;

                    using (var ms = new MemoryStream(vs.ToArray()))
                    {
                        img = Image.FromStream(ms);
                    }
                    PB1.Image = img;



                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
