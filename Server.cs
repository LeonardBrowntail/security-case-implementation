using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace FinalProject
{
    /* Logger class to handle server logging */
    public static class Logger
    {
        private static readonly object _log_lock = new object();

        public static void Log(string message)
        {
            lock (_log_lock)
            {
                using (StreamWriter stream = File.AppendText(Program.path))
                {
                    stream.WriteLineAsync(message);
                }
            }
        }

        public static void LogStart()
        {
            using (StreamWriter stream = File.CreateText(Program.path))
            {
                stream.WriteLine("Log " + DateTime.Now.ToString());
            }
        }
    }

    internal class Server
    {
        // Lock for thread safety
        private static readonly object _lock = new object();

        // Connected client list
        public static Hashtable clientList;

        // Server instance
        private static Server instance = new Server();

        // Client listener
        private TcpListener server;

        // Main reference
        private MainProgram main;

        // RSA container
        private RSACryptoServiceProvider key;
        private static Hashtable clientKeys;

        public MainProgram Main { get => main; set => main = value; }

        // Create a server with local addresss
        private Server()
        {
            server = new TcpListener(IPAddress.Parse(Program.localAddress), 8888);
            clientList = new Hashtable();
        }

        private void Listen()
        {
            try
            {
                Write("-----------------------");
                Write("Server started [ " + server.LocalEndpoint.ToString() + " ] Waiting for connection...");
                Write("-----------------------");
                TcpClient client = default(TcpClient);

                // A Thread to ping the client, needed to refresh client.Connected
                Thread keepAlive = new Thread(KeepAlive);
                keepAlive.Start();

                while (Program.running)
                {
                    //Perform blocking call and wait for connection
                    if (server.Pending())
                    {
                        client = server.AcceptTcpClient();

                        if (MainProgram.Secure)
                        {
                            var stream = client.GetStream();
                            var buffer = 
                        }

                        /* This part is to retrieve the username */
                        // Get incoming stream from client
                        NetworkStream stream = client.GetStream();
                        int bufferSize = client.ReceiveBufferSize;
                        byte[] buffer = new byte[bufferSize];
                        string username = string.Empty;

                        // Read the stream and store data into buffer
                        stream.Read(buffer, 0, bufferSize);

                        // Turn the block of data into string
                        username = Encoding.ASCII.GetString(buffer);

                        // Cut the terminator
                        username = username.Substring(0, username.IndexOf("</usr>"));

                        // This part is to prevent same usernames
                        lock (_lock)
                        {
                            short num = 1;
                            while (true)
                            {
                                if (clientList.ContainsKey(username))
                                {
                                    username += " (" + num + ")";
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        // Inform clients about the new client
                        Broadcast(username + " has joined the chat!");
                        // Show data in chatlog
                        Write("<>");
                        Write("<> " + username + " [" + client.Client.RemoteEndPoint.ToString() + "] connected!");
                        Write("<>");
                        // Insert the new client into a hashtable
                        // Using mutex to stop other threads from accessing it
                        lock (_lock) clientList.Add(username, client);

                        // Create a new thread to handle clients in separate threads
                        Thread clientHandler = new Thread(() => ClientHandle(username));
                        clientHandler.Start();
                    }
                }
            }
            catch (Exception e)
            {
                Write("Error :" + e.Message.ToString());
                Console.WriteLine(e);
            }
            Broadcast("<stop>");
            Write("-----------------------");
            Write(">> Server is turned off");
            Write("-----------------------");
            server.Stop();
            clientList.Clear();
        }

        /* This function would broadcast a message to every client connected to the server */

        public static void Broadcast(string message)
        {
            // Get the size of the message
            var size = message.Length;
            // Using mutex to avoid access from other threads
            lock (_lock)
            {
                foreach (DictionaryEntry item in clientList)
                {
                    // Get client from dictionary
                    var destClient = (TcpClient)item.Value;
                    // Get client's stream
                    var stream = destClient.GetStream();
                    // Encode the message as bytes and writes it into the stream
                    stream.Write(Encoding.ASCII.GetBytes(message), 0, size);
                    // Flush the stream
                    stream.Flush();
                }
            }
        }

        /* This function would handle incoming data from clients */

        private void ClientHandle(string usr)
        {
            TcpClient client;
            // Mutex to avoid access from other threads
            lock (_lock)
            {
                client = (TcpClient)clientList[usr];
            }

            if (client != null)
            {
                while (Program.running & client.Connected)
                {
                    try
                    {
                        // Getting stream from client
                        var stream = client.GetStream();
                        var size = client.ReceiveBufferSize;
                        var buffer = new byte[size];
                        // Reading the buffer
                        stream.Read(buffer, 0, size);
                        // Convert the data block into string
                        var str = Encoding.ASCII.GetString(buffer);

                        // Determine if the client disconnects
                        // Clients will send a <stop> code to indicate that they are disconnecting
                        if (!str.Contains("<stop>"))
                        {
                            if (str.Contains("</msg>"))
                            {
                                // Log the string to server log
                                str = str.Substring(0, str.IndexOf("</msg>"));
                                Write("[" + DateTime.Now.ToString() + "] " + usr + ": " + str);
                                // Broadcast the data to every client
                                Broadcast(usr + ": " + str);
                            }
                        }
                        else
                        {
                            // Breaks if client disconnected
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Write("Error: " + e.ToString());
                        Console.WriteLine(e);
                    }
                }
                Write("><");
                Write(">< " + usr + " Disconnected...");
                Write("><");
                clientList.Remove(usr);
                Broadcast(usr + " Disconnected...");
                client.Close();
            }
            else
            {
                Write("Unable to retrieve client...");
            }
        }

        public static Server GetInstance()
        {
            return instance;
        }

        private void Write(string message)
        {
            main.Write(message);
            Logger.Log(message);
        }

        public void Start()
        {
            Program.running = true;
            server.Start();
            Logger.LogStart();
            Thread listen = new Thread(Listen);
            listen.Start();
        }

        public void Stop()
        {
            Program.running = false;
        }

        private void KeepAlive()
        {
            while (Program.running)
            {
                // Mutex to avoid access from other threads
                lock (_lock)
                {
                    foreach (DictionaryEntry item in clientList)
                    {
                        var client = (TcpClient)item.Value;
                        var temp = client.Connected;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void GenerateKey()
        {

        }

        private void Encrypt(string message)
        {

        }
    }
}