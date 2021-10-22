using System;
using System.Net;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class MainProgram : Form
    {
        private Server server = null;
        private Client client = null;
        public string stream = string.Empty;
        private static bool secure = false;

        public static bool Secure { get => secure; }

        public MainProgram()
        {
            InitializeComponent();
            Write("Local address: " + Program.localAddress);
            Write("Please choose program mode in Option -> Behaviour...");
        }

        //Server mode selected
        private void select_server(object sender, EventArgs e)
        {
            try
            {
                server = Server.GetInstance();
                server.Main = this;
            }
            catch (Exception error)
            {
                Write("Error: " + error.Message.ToString());
                Console.WriteLine(e);
            }
            if (server != null)
            {
                Write("Server mode selected, press start to start listening to TCP connections");
                ActiveForm.Text += " (Server mode)";
                serverCheck.Checked = true;
                serverCheck.Enabled = false;
                clientCheck.Enabled = false;
                serverStart.Enabled = true;
            }
        }

        //Client mode selected
        private void select_client(object sender, EventArgs e)
        {
            try
            {
                client = Client.GetInstance();
                client.Main = this;
            }
            catch (Exception error)
            {
                Write("Error: " + error.ToString());
            }
            if (client != null)
            {
                Write("Client mode selected, please input the username and host destination...");
                ActiveForm.Text += " (Client mode)";
                serverCheck.Enabled = false;
                clientCheck.Enabled = false;
                clientCheck.Checked = true;
                usernameBox.Enabled = true;
                destIPBox.Enabled = true;
                clientConnect.Enabled = true;
            }
            else
            {
                Write("Client initialization failed...");
            }
        }

        private void RunClient(object sender, EventArgs e)
        {
            IPAddress ip;
            if (IPAddress.TryParse(destIPBox.Text, out ip))
            {
                client.Connect(ip, usernameBox.Text);
                if (Program.running)
                {
                    clientConnect.Enabled = false;
                    usernameBox.Enabled = false;
                    destIPBox.Enabled = false;
                    clientDisconnect.Enabled = true;
                    inputBox.Enabled = true;
                    sendButton.Enabled = true;
                    secureBox.Enabled = false;
                }
            }
            else
            {
                destIPBox.Text = string.Empty;
                Write("Invalid host address, please input a valid address...");
            }
        }

        private void RunServer(object sender, EventArgs e)
        {
            server.Start();
            if (Program.running)
            {
                serverStart.Enabled = false;
                serverStop.Enabled = true;
                secureBox.Enabled = false;
            }
        }

        private void StopClient(object sender, EventArgs e)
        {
            client.Stop();
            if (!Program.running)
            {
                clientConnect.Enabled = true;
                clientDisconnect.Enabled = false;
                inputBox.Enabled = false;
                sendButton.Enabled = false;
                secureBox.Enabled = true;
            }
        }

        private void StopServer(object sender, EventArgs e)
        {
            server.Stop();
            if (!Program.running)
            {
                serverStart.Enabled = true;
                serverStop.Enabled = false;
                secureBox.Enabled = true;
            }
        }

        private void SendMessage(object sender, EventArgs e)
        {
            if (inputBox.Text.Length != 0)
            {
                client.Send(inputBox.Text);
                inputBox.Text = string.Empty;
            }
        }

        public void Write(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => Write(message)));
            }
            else
            {
                chatBox.Text += message + Environment.NewLine;
            }
        }

        private void destIPBox_OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                RunClient(sender, e);
            }
        }

        private void inputBox_OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SendMessage(sender, e);
            }
        }

        private void OnCloseProgram(object sender, FormClosingEventArgs e)
        {
            if (server != null)
            {
                server.Stop();
            }
            if (client != null)
            {
                client.Stop();
            }
        }

        private void secureBox_CheckedChanged(object sender, EventArgs e)
        {
            secure = secureBox.Checked;
        }
    }
}