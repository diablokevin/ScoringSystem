using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EpServerEngine.cs;

namespace TimerControl
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm,  INetworkServerAcceptor, INetworkServerCallback, INetworkSocketCallback
    {
        INetworkServer m_server = new IocpTcpServer();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(m_server.IsServerStarted)
            {
                m_server.StopServer();
                
                
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ServerOps ops = new ServerOps(this, "12345", this);
            m_server.StartServer(ops);


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public bool OnAccept(INetworkServer server, IPInfo ipInfo)
        {
            return true;
        }

        public INetworkSocketCallback GetSocketCallback()
        {
            return this;
        }

        public void OnServerStarted(INetworkServer server, StartStatus status)
        {
            SetTipMessage("connect!");
            
        }

        public void OnServerAccepted(INetworkServer server, INetworkSocket socket)
        {
            throw new NotImplementedException();
        }

        public void OnServerStopped(INetworkServer server)
        {
            
        }


        List<INetworkSocket> m_socketList = new List<INetworkSocket>();
        public void OnNewConnection(INetworkSocket socket)
        {
            m_socketList.Add(socket);
            string sendString = "**New user(" + socket.IPInfo.IPAddress + ")connected,port:" + socket.IPInfo.IPEndPoint.Port;
            AddMsg(sendString);
        }
        delegate void AddMsg_Involk(string message);
        public void AddMsg(string message)
        {
            if(textBox1.InvokeRequired)
            {
                AddMsg_Involk CI = new AddMsg_Involk(AddMsg);
                textBox1.Invoke(CI, message);
            }
            else
            {
                textBox1.Text += message + "\r\n";
            }
        }

        public void SetTipMessage(string message)
        {
            if(label1.InvokeRequired)
            {
                AddMsg_Involk CI = new AddMsg_Involk(SetTipMessage);
                label1.Invoke(CI, message);
            }
            else
            {
                label1.Text = message;
            }
        }
        

        public void OnReceived(INetworkSocket socket, Packet receivedPacket)
        {
            string sendString = "User(" + socket.IPInfo.IPAddress + "):" + StringFromByteArr(receivedPacket.PacketRaw);
            AddMsg(sendString);
            foreach (var socketObj in m_socketList)
            {
                if (socketObj != socket)
                {
                    socketObj.Send(receivedPacket);
                }
            }
        }

        public void OnSent(INetworkSocket socket, SendStatus status, Packet sentPacket)
        {
           switch(status)
            {
                case SendStatus.SUCCESS:
                    SetTipMessage("Send success");
                    break;
                case SendStatus.FAIL_CONNECTION_CLOSING:
                    SetTipMessage("Send fail,FAIL_CONNECTION_CLOSING");
                    break;
                case SendStatus.FAIL_INVALID_PACKET:
                    SetTipMessage("Send fail,FAIL_INVALID_PACKET");
                    break;
                case SendStatus.FAIL_NOT_CONNECTED:
                    SetTipMessage("Send fail,FAIL_NOT_CONNECTED");
                    break;
                case SendStatus.FAIL_SOCKET_ERROR:
                    SetTipMessage("Send fail,FAIL_SOCKET_ERROR");
                    break;
           
            }
        }

        public void OnDisconnect(INetworkSocket socket)
        {
            m_socketList.Remove(socket);
            String sendString = "** User(" + socket.IPInfo.IPAddress + ") disconnected!";
            AddMsg(sendString);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sendText = textEdit1.Text.Trim();
            byte[] bytes = BytesFromString(sendText);
            Packet packet = new Packet(bytes, 0, bytes.Count(), false);
            m_server.Broadcast(packet);
        }

 

        String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        byte[] BytesFromString(String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
          
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        byte[] BytesFromString(String str,string encoding)
        {
            byte[] bytes_encoding = Encoding.GetEncoding(encoding).GetBytes(str);            
            return bytes_encoding;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            char[] chr = textEdit1.Text.ToCharArray();
            foreach (char c in chr)
            {
                byte[] bm = Encoding.GetEncoding(textBox2.Text).GetBytes(new char[] { c });
                foreach(byte b in bm)
                {
                    textBox1.Text += b.ToString("X8");
                }
                //int n = (int)bm[0] << 8;
                //n += bm[1];
                
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sendText = textEdit1.Text.Trim();
            byte[] bytes = BytesFromString(sendText,textBox2.Text);
            Packet packet = new Packet(bytes, 0, bytes.Count(), false);
            m_server.Broadcast(packet);
        }
    }
}
