using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace FinalProject
{
    internal class Client
    {
        private static Client instance = new Client();
        private const short port = 8888;
        private MainProgram main = null;
        private TcpClient client;

        //Security
        private ConnectionMode connectionMode;

        private RSASecurity rsa;
        private AESSecurity aes;

        public MainProgram Main { get => main; set => main = value; }

        private Client()
        {
            client = default(TcpClient);
        }

        public void Connect(IPAddress ip, string username)
        {
            for (short r = 0; r < 3; r++)
            {
                try
                {
                    // Connect to host
                    client = new TcpClient(ip.ToString(), port);
                    Write("Connected!");
                    Program.running = true;
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }
            if (client != null)
            {
                if (client.Connected)
                {
                    // If client is secure, tell server that we want secure connection
                    if (MainProgram.Secure)
                    {
                        try
                        {
                            // Get Stream and send connection mode
                            var stream = client.GetStream();
                            byte[] buffer = new byte[1];
                            buffer[0] = Convert.ToByte(true);
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Flush();
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                        //todo: Establish secure connection...
                        {
                            var stream = client.GetStream();
                            var size = client.ReceiveBufferSize;
                            var buffer = new byte[size];
                            stream.Read(buffer, 0, size);
                            var str = Encoding.ASCII.GetString(buffer);
                        }
                    }
                    try
                    {
                        // Get Stream and send the username
                        {
                            var stream = client.GetStream();
                            var buffer = Encoding.ASCII.GetBytes(username + "</usr>");
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Flush();
                        }

                        // Create a thread to handle incoming messages
                        Thread clientThread = new Thread(GetMessage);
                        clientThread.Start();

                        // A Thread to ping the client, needed to refresh client.Connected
                        Thread keepAlive = new Thread(KeepAlive);
                        keepAlive.Start();
                    }
                    catch (Exception e)
                    {
                        Write(e.Message);
                        Console.WriteLine(e);
                    }
                }
            }
            else
            {
                Write("Unable to connect to " + ip.ToString() + " after 3 retries...");
            }
        }

        private void GetMessage()
        {
            try
            {
                while (Program.running & client.Connected)
                {
                    var stream = client.GetStream();
                    var size = client.ReceiveBufferSize;
                    var buffer = new byte[size];
                    stream.Read(buffer, 0, size);
                    var str = Encoding.ASCII.GetString(buffer);
                    if (str.Contains("<stop>"))
                    {
                        Write("Server has been shut down");
                        break;
                    }
                    else
                    {
                        Write(str);
                    }
                }
                Write("Disconnected from server...");
                client.Close();
            }
            catch (Exception e)
            {
                Write(e.Message);
                Console.WriteLine(e);
            }
        }

        public static Client GetInstance()
        {
            return instance;
        }

        private void Write(string message)
        {
            main.Write(Environment.NewLine + message);
        }

        public void Send(string message)
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    try
                    {
                        var stream = client.GetStream();
                        var buffer = Encoding.ASCII.GetBytes(message + "</msg>");
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                    }
                    catch (Exception e)
                    {
                        Write(e.Message);
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    Write("Cannot send, no connection to server");
                }
            }
        }

        public void Stop()
        {
            Send("<stop>");
            Program.running = false;
        }

        private void KeepAlive()
        {
            while (client.Connected)
            {
                Thread.Sleep(1000);
            }
        }
    }
}