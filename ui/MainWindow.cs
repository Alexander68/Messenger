using System;
using System.Collections.Generic; 
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MessageException;
using Messenger.utils;

namespace Messenger.ui
{
    public class MainWindow : Form
    {
        private SocketClient client;
        private SocketServer server;
        private Button send;
        private Button connectBtn;
        private TextBox textInput;
        private TableLayoutPanel mesagePanel;

        private int tab;

        public MainWindow()
        {

            InitializeComponent();
            try
            {
                Properties.load();
                string serverPort = Properties.getServerPort();
                server = new SocketServer("localhost", int.Parse(serverPort));
                Thread serverThread = new Thread(server.Init);
                serverThread.Start();
                server.setWindow(this);
                tab = 10;
            }
            catch (Exception ex)
            {
                write(ex.Message);
            }


        }

        public void write(string msg)
        {
            Label message = new System.Windows.Forms.Label();
            message.AutoSize = true;
            message.Size = new System.Drawing.Size(40, 40);
            message.Text = msg;
            message.Visible = true;
            message.TabIndex = tab++;
            message.Show();
            if (mesagePanel.InvokeRequired)
            {
                mesagePanel.BeginInvoke(new Action<Label>((s)
                    => mesagePanel.Controls.Add(s)), message);
            }
            else
            {
                mesagePanel.Controls.Add(message);
            }
        }

        private void InitializeComponent()
        {
            this.send = new System.Windows.Forms.Button();
            this.textInput = new System.Windows.Forms.TextBox();
            this.mesagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.connectBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(411, 416);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(175, 26);
            this.send.TabIndex = 0;
            this.send.Text = "Отправить";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.sendMessage);
            this.send.Enabled = false;
            // 
            // textInput
            // 
            this.textInput.Location = new System.Drawing.Point(12, 305);
            this.textInput.Multiline = true;
            this.textInput.Name = "textInput";
            this.textInput.Size = new System.Drawing.Size(574, 105);
            this.textInput.TabIndex = 1;
            // 
            // mesagePanel
            // 
            this.mesagePanel.AutoScroll = true;
            this.mesagePanel.AutoScrollMinSize = new System.Drawing.Size(1, 0);
            this.mesagePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mesagePanel.Location = new System.Drawing.Point(12, 12);
            this.mesagePanel.Name = "mesagePanel";
            this.mesagePanel.Size = new System.Drawing.Size(574, 287);
            this.mesagePanel.TabIndex = 2;
            this.mesagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button1
            // 
            this.connectBtn.Location = new System.Drawing.Point(13, 419);
            this.connectBtn.Name = "button1";
            this.connectBtn.Size = new System.Drawing.Size(173, 23);
            this.connectBtn.TabIndex = 3;
            this.connectBtn.Text = "button1";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connect);
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(598, 454);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.mesagePanel);
            this.Controls.Add(this.textInput);
            this.Controls.Add(this.send);
            this.Name = "MainWindow";
            this.Text = "Defender Messenger";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
        }



        private void sendMessage(object sender, EventArgs e)
        {
            try
            {
                string sessionKey = Properties.getSessionKey();
                if (!string.IsNullOrEmpty(sessionKey) && client == null)
                {
                    string ip = Properties.getClientIp();
                    string port = Properties.getClientPort();
                    client = new SocketClient(ip.Trim(), int.Parse(port));
                }

                if (!string.IsNullOrEmpty(textInput.Text))
                {
                    if (!client.SendMessageFromSocket(textInput.Text, true))
                    {
                        write("Сообщение: \"" + textInput.Text + "\" не отправлено.");
                    }
                    else
                    {
                        write(textInput.Text);
                    }

                    textInput.Text = "";
                }
            }
            catch (Exception ex) 
            {
                write(ex.Message);
            }
        }



        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void connect(object sender, EventArgs e)
        {
            try
            {
                string ip = Properties.getClientIp();
                string port = Properties.getClientPort();
                client = new SocketClient(ip.Trim(), int.Parse(port));
                string status = client.connect();
                if (SocketClient.SUCCESS.Equals(status))
                {
                    setEnabledSend();
                }
                write(status);

            }
            catch (Exception ex)
            {
                write(ex.Message);
            }
        }

        public void setEnabledSend()
        {
            connectBtn.Enabled = false;
            send.Enabled = true;
        }

       




    }
}
