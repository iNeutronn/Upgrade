using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

namespace UpgradeServer
{
    class Program
    {
        static List<Player> players;
        const int port = 3234;
        static List<string> Log = new List<string>();
        static Socket listenSocket;
        static void Main(string[] args)
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            listenSocket.Listen(10);
            Console.WriteLine("Серверс запущений");
            Console.WriteLine();
            Console.WriteLine($"Ваш адрес : {GetOwnIP()}");
            Process();


        }
        private static string GetOwnIP()
        {
#pragma warning disable CS0618 // Тип или член устарел
            IPAddress[] ip = Dns.GetHostByName(Dns.GetHostName()).AddressList;
#pragma warning restore CS0618 // Тип или член устарел
            foreach (var item in ip)
                if (item.AddressFamily == AddressFamily.InterNetwork && item.ToString().Contains("192.168.0"))
                    return item.ToString();
            throw new Exception();


        }
        static  void Process()
        {
            while (true)
            {
                Socket handler = listenSocket.Accept();
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                byte[] data = new byte[256];
                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (handler.Available > 0);
                string resp = builder.ToString();
                if (resp == "GetData")
                {
                    data = Encoding.UTF8.GetBytes(GetData());
                    handler.Send(data);
                }
                else if (resp.StartsWith("Connect"))
                {
                    string Name = "";
                    for (int i = 8; i < resp.Length; i++)
                        Name += resp[i];
                    players.Add(new Player(Name));
                    Log.Add($"\"{Name}\" підключився!({players.Count}/4)");
                    Console.WriteLine($"\"{Name}\" підключився! ({players.Count}/4)");

                }
                else if (resp == "IsReadyToStart")
                {
                    handler.Send(Encoding.UTF8.GetBytes((players.Count == 4).ToString()));
                }
                else if (resp == "GetLog")
                {
                    string outp = JsonSerializer.Serialize<List<string>>(Log);
                    data = Encoding.UTF8.GetBytes(outp);
                    handler.Send(data);
                }
                else if (resp.StartsWith("Disconect"))
                {
                    string Name = "";
                    Log.Add($"\"{Name}\" відключився!({players.Count}/4)");
                    Console.WriteLine($"\"{Name}\" відключився! ({players.Count}/4)");
                    int i;
                    
                    for ( i = 8; i < resp.Length; i++)
                        Name += resp[i];
                    
                    for (i = 0; i < players.Count; i++)
                        if (players[i].Name == Name)
                            break;
                    players.RemoveAt(i);
                    
                }
                       
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        private static string GetData()
        {
            return JsonSerializer.Serialize(players);

        }
    }
}
