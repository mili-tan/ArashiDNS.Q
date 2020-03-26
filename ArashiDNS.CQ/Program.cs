using System;
using System.Net;
using System.Threading.Tasks;
using ARSoft.Tools.Net.Dns;
using QuicNet;

namespace ArashiDNS.CQ
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dnsServer = new DnsServer(IPAddress.Any, 10, 10))
            {
                DNSEncoder.Init();
                var client = new QuicClient();
                var connection = client.Connect("127.0.0.1", 11000);
                var stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);
                async Task OnDnsServerOnQueryReceived(object sender, QueryReceivedEventArgs e)
                {
                    if (!(e.Query is DnsMessage query)) return;
                    await Task.Run(() =>
                    {
                        stream.Send(DNSEncoder.Encode(query));
                        e.Response = DnsMessage.Parse(stream.Receive());
                    });
                }

                dnsServer.QueryReceived += OnDnsServerOnQueryReceived;
                dnsServer.Start();
                Console.WriteLine(@"-------ARASHI Q DNS------");
                Console.WriteLine(@"-------DOTNET ALPHA------");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"ArashiDNS Server Running");

                Console.WriteLine(@"Press any key to stop dns server");
                Console.WriteLine("------------------------");
                Console.ReadLine();
                Console.WriteLine("------------------------");
            }
        }
    }
}
