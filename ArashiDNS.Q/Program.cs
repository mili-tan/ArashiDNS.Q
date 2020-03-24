using System;
using System.ComponentModel;
using System.Net;
using QuicNet;
using ARSoft.Tools.Net.Dns;

namespace ArashiDNS.Q
{
    class Program
    {
        static void Main(string[] args)
        {
            DNSEncoder.Init();
            var listener = new QuicListener(11000);
            listener.Start();
            while (true)
            {
                var client = listener.AcceptQuicClient();

                client.OnDataReceived += c => {
                    var msg = c.Data;
                    using (var bgWorker = new BackgroundWorker())
                    {
                        bgWorker.DoWork += (sender, eventArgs) =>
                        {
                            var dnsQMsg = DnsMessage.Parse(msg);
                            var dnsRMsg =
                                new DnsClient(IPAddress.Parse("8.8.8.8"), 1000).SendMessage(dnsQMsg);
                            c.Send(DNSEncoder.Encode(dnsRMsg));

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            dnsQMsg.Questions.ForEach(o => Console.WriteLine("Qu:" + o));
                            Console.ForegroundColor = ConsoleColor.Green;
                            dnsRMsg.AnswerRecords.ForEach(o => Console.WriteLine("An:" + o));
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            dnsRMsg.AuthorityRecords.ForEach(o => Console.WriteLine("Au:" + o));
                            Console.ForegroundColor = ConsoleColor.Green;
                        };
                        bgWorker.RunWorkerAsync();
                    }
                };
            }
        }
    }
}
