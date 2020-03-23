using System;
using QuicNet;
using QuicNet.Context;
using System.Text;

namespace ArashiDNS.Q
{
    class Program
    {
        static void Main(string[] args)
        {
            QuicListener listener = new QuicListener(11000);
            listener.Start();
            while (true)
            {
                QuicConnection client = listener.AcceptQuicClient();

                client.OnDataReceived += (c) => {
                    byte[] data = c.Data;
                    Console.WriteLine("Data received: " + Encoding.UTF8.GetString(data));
                    c.Send(Encoding.UTF8.GetBytes("Echo!"));
                };
            }
        }
    }
}
