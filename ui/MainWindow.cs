using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Messenger.ui
{
    public class MainWindow: Form
    {
        private SocketClient client;
        private SocketServer server;
        private Button send;
        private TextBox textBox1;
        private TableLayoutPanel mesagePanel;
        private Button button1;
        private int tab = 10;


        public MainWindow() 
        {

            InitializeComponent();
            try
            {
                server = new SocketServer("localhost", 9999);    
                Thread serverThr = new Thread(server.init); 
                serverThr.Start();
                server.setWindow(this);
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.mesagePanel = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
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
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 305);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(574, 105);
            this.textBox1.TabIndex = 1;
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
            this.button1.Location = new System.Drawing.Point(13, 419);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(598, 454);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mesagePanel);
            this.Controls.Add(this.textBox1);
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
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                if (!client.SendMessageFromSocket(textBox1.Text))
                {
                    write("Сообщение: \"" + textBox1.Text + "\" не отправлено.");
                }
                else
                {
                    write(textBox1.Text);
                }
               
                textBox1.Text = "";
            }
        }

         

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                client = new SocketClient("localhost",4578 );
            }
            catch (Exception ex)
            {
                write(ex.Message);
            }
        }
         
    }
}
