using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Beat
{
    public class Program
    {
        private static string Server = "127.0.0.1";
        private static int Port = 13000;
        private static bool Run = true;
        private static void Intro()
        {
            Console.Clear();
            Console.Title = "Auram Beat";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("████████████████████████████████████████████████████████████\n██▀▄─██▄─██─▄█▄─▄▄▀██▀▄─██▄─▀█▀─▄███▄─▄─▀█▄─▄▄─██▀▄─██─▄─▄─█\n██─▀─███─██─███─▄─▄██─▀─███─█▄█─█████─▄─▀██─▄█▀██─▀─████─███\n▀▄▄▀▄▄▀▀▄▄▄▄▀▀▄▄▀▄▄▀▄▄▀▄▄▀▄▄▄▀▄▄▄▀▀▀▄▄▄▄▀▀▄▄▄▄▄▀▄▄▀▄▄▀▀▄▄▄▀▀\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Auram Beat");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" by TTMC Corporation ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("TheBlueLines");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Version: v0.1 IGNIS\n");
        }
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
        private static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }
                return mso.ToArray();
            }
        }
        private static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
        private static void Main(string[] args)
        {
            Intro();
            while (true)
            {
                try
                {
                    TcpClient client = Connect(Server, Port);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Connected to Auram server at {0}:{1}", Server, Port);
                    Run = true;
                    while (Run)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(" » ");
                        string? message = Console.ReadLine();
                        if (message == null)
                        {
                            Disconnect(client);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("PROGRAM EXITED");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Run = false;
                            Environment.Exit(0);
                        }
                        else if (message == string.Empty)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine(" « Command can't be empty");
                        }
                        else
                        {
                            if (message.ToUpper() == "EXIT")
                            {
                                Disconnect(client);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("PROGRAM EXITED");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Run = false;
                                Environment.Exit(0);
                            }
                            else if (message.ToUpper() == "CLEAR" || message.ToUpper() == "RESET")
                            {
                                Intro();
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Connected to Auram server at {0}:{1}", Server, Port);
                            }
                            if (message.ToUpper().StartsWith("RUN"))
                            {
                                if (message.Length == 3)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine(" « Usage: RUN <SCRIPT>\n « Example: RUN C:\\Users\\Auram\\Downloads");
                                }
                                else
                                {
                                    if (File.Exists(message[4..]))
                                    {
                                        try
                                        {
                                            foreach (string line in File.ReadLines(message[4..]))
                                            {
                                                if (line != string.Empty)
                                                {
                                                    try
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                                        Console.Write(" « ");
                                                        Console.WriteLine(Message(client, line));
                                                    }
                                                    catch
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                                        Console.Write(" « ");
                                                        Console.WriteLine("ERROR: " + line);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            Console.ForegroundColor = ConsoleColor.DarkGray;
                                            Console.Write(" « ");
                                            Console.WriteLine("Can't run this Auram Script file!");
                                        }
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("FILE NOT FOUND");
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                }
                            }
                            else if (message.Split(' ')[0].ToUpper() == "CONNECT")
                            {
                                if (message.Split(' ').Length == 2)
                                {
                                    string[] x = message.Split(' ');
                                    string[] y = x[1].Split(':');
                                    if (y.Length == 1)
                                    {
                                        try
                                        {

                                            Connect(y[0], Port);
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine(" « CONNECTED");
                                            Server = y[0];
                                        }
                                        catch
                                        {
                                            Console.ForegroundColor = ConsoleColor.DarkGray;
                                            Console.WriteLine(" « Can't connect to server at " + y[0] + ":" + Port);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Intro();
                                            Connect(y[0], int.Parse(y[1]));
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine(" « CONNECTED");
                                            Server = y[0];
                                            Port = int.Parse(y[1]);
                                        }
                                        catch
                                        {
                                            Console.ForegroundColor = ConsoleColor.DarkGray;
                                            Console.WriteLine(" « Can't connect to server at " + y[0] + ":" + y[1]);
                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine(" « Usage: CONNECT <SERVER:PORT>\n « Example: CONNECT " + Server + ":" + Port);
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(" « ");
                                Console.WriteLine(Message(client, message));
                            }
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    Disconnect(client);
                }
                catch
                {
                    Run = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Can't connect to Auram server at {0}:{1}", Server, Port);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Please enter server address");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(" « Usage: <SERVER:PORT>\n « Example: " + Server + ":" + Port);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(" » ");
                    string? tmp = Console.ReadLine();
                    if (tmp != null)
                    {
                        string[] temp = tmp.Split(':');
                        try
                        {
                            Server = temp[0];
                            Port = int.Parse(temp[1]);
                            Intro();
                        }
                        catch
                        {
                            Server = "127.0.0.1";
                            Port = 13000;
                            Intro();
                        }
                    }
                }
            }
        }
        private static void Disconnect(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            stream.Close();
            client.Close();
        }
        private static TcpClient Connect(string server, int port)
        {
            return new TcpClient(server, port);
        }
        private static string Message(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);
                return Encoding.UTF8.GetString(data, 0, bytes);
            }
            catch (ArgumentNullException e)
            {
                return "ArgumentNullException: " + e;
            }
            catch (SocketException e)
            {
                return "SocketException: " + e;
            }
        }
    }
}