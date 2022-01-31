using System.Drawing;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

class Program
{
    static public void Split<T>(T[] array, int index, out T[] first, out T[] second)
    {
        first = array.Take(index).ToArray();
        second = array.Skip(index).ToArray();
    }
    static public void SplitMidPoint<T>(T[] array, out T[] first, out T[] second)
    {
        Split(array, array.Length / 2, out first, out second);
    }
    static void Main(string[] args)
    {
        var ip = IPAddress.Loopback;
        var port = 1111;
        var TcpServer = new TcpListener(ip, port);

        Console.WriteLine("Lisening ", ip);

        TcpServer.Start(100);

        for (int i = 0; i < 10; i++)
        {

            var client = TcpServer.AcceptTcpClient();

            UdpClient MyUdpClient = new UdpClient();
            var ep = new IPEndPoint(ip, port + 5);
            Console.WriteLine($"{client.Client.RemoteEndPoint} Client Connected");
            while (true)
            {
                Thread.Sleep(250);
                using var bitmap = new Bitmap(1280, 720);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0,
                        bitmap.Size, CopyPixelOperation.SourceCopy);
                }


                Image img = (Image)bitmap;

                byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));

                byte[] bytes1;
                byte[] bytes2;

                SplitMidPoint(bytes,out bytes1,out bytes2);

                byte[] bytes1_1;
                byte[] bytes1_2;

                SplitMidPoint(bytes1, out bytes1_1, out bytes1_2);
                byte[] bytes2_1;
                byte[] bytes2_2;
                SplitMidPoint(bytes2, out bytes2_1, out bytes2_2);

                var CompressedBytes1=Compress(bytes1_1);
                var CompressedBytes2=Compress(bytes1_2);
                var CompressedBytes3=Compress(bytes2_1);
                var CompressedBytes4=Compress(bytes2_2);

                MyUdpClient.Send(CompressedBytes1, CompressedBytes1.Length,ep);
                MyUdpClient.Send(CompressedBytes2, CompressedBytes2.Length,ep);
                MyUdpClient.Send(CompressedBytes3, CompressedBytes3.Length,ep);
                MyUdpClient.Send(CompressedBytes4, CompressedBytes4.Length,ep);

            }



        }
    }
    public static byte[] Compress(byte[] data)
    {
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.SmallestSize))
        {
            dstream.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

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
}