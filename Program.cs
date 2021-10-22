using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace FinalProject
{
    internal static class Program
    {
        public static string path = Environment.CurrentDirectory + "\\" + "Log_" + DateTime.Now.ToFileTime() + ".txt";
        public static string localAddress = string.Empty;
        public static bool running = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var i in host.AddressList)
            {
                if (i.AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddress = i.ToString();
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainProgram());
        }
    }
}