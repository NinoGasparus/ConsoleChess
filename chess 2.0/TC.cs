using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using static referenc.referenc;
namespace chess_2._0
{
    internal class TC
    {
        public static string initialsetupH(int port)  //as host listens to first 2m messages ip and name
        {
            string message2;
            while (true)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                listener.Start();
                Console.WriteLine("Connecting... ");
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);


                bytesRead = stream.Read(buffer, 0, buffer.Length);
                message2 = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine(message + " joined from " + message2);
                Thread.Sleep(1000);

                listener.Stop();
                stream.Close();
                break;
            }

            return message2;
        }


        public static void initialsetupC(string ip, int port, string username)
        {
            try
            {
                while (true)
                {
                    TcpClient client = new TcpClient();
                    client.Connect(ip, port);
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(username);
                    stream.Write(data, 0, data.Length);
                    Thread.Sleep(150);

                    data = Encoding.UTF8.GetBytes(TC.getIP());
                    stream.Write(data, 0, data.Length);


                    stream.Close();
                    client.Close();
                    gameReady = true;
                    break;
                }
            }catch(Exception error) {
                Thread.Sleep(2000);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Failed to Connect: Invalid IP address or game port");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(3000);
                
            }




        }

        public static void clientMoveSequence(string move, string move2, string ip, int port) //moves the pice and sends data about the move to the host
        {
            chessboard.movePice(move, move2);
            chessboard.Queue[0] = move;
            chessboard.Queue[1] = move2;
          //  Console.WriteLine("bout to send first message");
            while (true)
            {
                TcpClient client2 = new TcpClient();
                client2.Connect(ip, port);
                NetworkStream stream2 = client2.GetStream();


                byte[] data = Encoding.UTF8.GetBytes(move);
                stream2.Write(data, 0, data.Length);

                stream2.Close();
                client2.Close();
                break;

            }
          //  Console.WriteLine("send first mesage");
            Thread.Sleep(20);
         //   Console.WriteLine("bout to send second message");
            while (true)
            {
                TcpClient client2 = new TcpClient();
                client2.Connect(ip, port);
                NetworkStream stream2 = client2.GetStream();


                byte[] data = Encoding.UTF8.GetBytes(move2);
                stream2.Write(data, 0, data.Length);

                stream2.Close();
                client2.Close();
                break;

            }
        //    Console.WriteLine("second mesasge sent");

        }

        public static string[] clientRecieveMove(int port)
        {
            string[] Innbox = { "", "" };
            Console.WriteLine("not your turn");
            //Console.ReadKey();

          //  Console.WriteLine("entered listen loop");
            while (true)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port - 1);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                listener.Start();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
              //  Console.WriteLine(message);
                Innbox[0] = message;
                listener.Stop();
                stream.Close();
                break;
            }
           // Console.WriteLine("entered listen loop 2");
            while (true)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port - 1);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                listener.Start();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
              //  Console.WriteLine(message);
                Innbox[1] = message;
                listener.Stop();
                stream.Close();
                break;
            }
            // Console.WriteLine("x");
            string x = chessboard.caseFormatMove(Innbox[0], false);
            // Console.WriteLine("y");
            string y = chessboard.caseFormatMove(Innbox[1], false);
            //  Console.WriteLine("z");
            string[] returnString = { x, y };

            return returnString;
        }

        public static void hostMoveSequence(string move, string move2, string ip, int port)
        {
            while (true)
            {
                TcpClient client2 = new TcpClient();
                client2.Connect(ip, port-1);
                NetworkStream stream2 = client2.GetStream();


                byte[] data = Encoding.UTF8.GetBytes(move);
                stream2.Write(data, 0, data.Length);

                stream2.Close();
                client2.Close();
                break;

            }
            // Console.WriteLine("send first mesage");
            Thread.Sleep(20);
            //  Console.WriteLine("bout to send second message");
            while (true)
            {
                TcpClient client2 = new TcpClient();
                client2.Connect(ip, port - 1);
                NetworkStream stream2 = client2.GetStream();


                byte[] data = Encoding.UTF8.GetBytes(move2);
                stream2.Write(data, 0, data.Length);

                stream2.Close();
                client2.Close();
                break;

            }
            //Console.WriteLine("second mesasge sent");
        }

        public static string[] hostRecieveMove(int port)
        {
            string[] Innbox = { "", "" };
            while (true)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                listener.Start();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
             //   Console.WriteLine(message);
                Innbox[0] = message;
                listener.Stop();
                stream.Close();
                break;
            }
            // Console.WriteLine("entered listen loop 2");
            while (true)
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                listener.Start();

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
             //   Console.WriteLine(message);
                Innbox[1] = message;
                listener.Stop();
                stream.Close();
                break;
            }

            return Innbox;
        }
        
        public static  string getIP()
        {
            string activeIPAddress = null;

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties ipProp = nic.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipInfo in ipProp.UnicastAddresses)
                    {
                        if (ipInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            activeIPAddress = ipInfo.Address.ToString();
                            break;
                        }
                    }
                }
            }

            return activeIPAddress;

        }
    }
}
