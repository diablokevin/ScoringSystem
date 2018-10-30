﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.Entity;
using EpServerEngine.cs;

namespace TimerControl
{
  
    public partial class main : DevExpress.XtraBars.Ribbon.RibbonForm, INetworkServerAcceptor, INetworkServerCallback, INetworkSocketCallback
    {
        INetworkServer m_server = new IocpTcpServer();  //TCP服务器对象
        //public static AllEventViews ListEventViews;
        //public static AllTimerViews ListTimerViews;

        public List<TimerView> TimerViews = new List<TimerView>();  //储存计时器状态的集合
        public List<EventView> EventViews = new List<EventView>();
        TimerControl.TimerContext timerContext = new TimerControl.TimerContext();
        public main()
        {

            InitializeComponent();
            // This line of code is generated by Data Source Configuration Wizard
            serveStatus_start.Visibility = BarItemVisibility.Never;
            serveStatus_stop.Visibility = BarItemVisibility.Always;
            ribbonbtn_serveAct_start.Enabled = true;
            ribbonbtn_serveAct_stop.Enabled = false;
            //ListTimerViews =  new AllTimerViews();
            //ListEventViews = new AllEventViews(timerContext);
            foreach (Event item in timerContext.Events.ToList())
            {
                EventView eventView = new EventView();
                eventView.ChipId = item.ChipId;
                eventView.Name = item.Name;
                eventView.Time_limit = item.TimeLimit;
                eventView.Id = item.Id;

                EventViews.Add(eventView);


            }


          
           
            //gridControl1.DataSource = ListEventViews.list;

            gridControl1.DataSource = EventViews;
            gridControl2.DataSource = TimerViews;
          //  ListTimerViews.list = TimerViews;
           // ListEventViews.list = EventViews;
        
          

            // This line of code is generated by Data Source Configuration Wizard
            lookUpEdit1.Properties.DataSource = EventViews;
            lookUpEdit1.Properties.ValueMember = "Id";
            lookUpEdit1.Properties.DisplayMember= "Name";
            


        }
        List<INetworkSocket> m_socketList = new List<INetworkSocket>();  //TCP客户端集合
        public INetworkSocketCallback GetSocketCallback()
        {
            return this;
        }

        public bool OnAccept(INetworkServer server, IPInfo ipInfo)
        {
            return true;
        }

        public void OnDisconnect(INetworkSocket socket)
        {
            m_socketList.Remove(socket);
            TimerView timerView = FindTimerViaSocket(socket);
            timerView.UnBindToEventView();
            TimerViews.Remove(timerView);//去掉Timer列表里的项目

        }

        

        public void OnNewConnection(INetworkSocket socket)
        {
            m_socketList.Add(socket);           
            //ListTimerViews.list.Add(new TimerView() { IpAddress = socket.IPInfo.IPAddress });
            TimerViews.Add(new TimerView() { IpAddress = socket.IPInfo.IPAddress ,Socket=socket});
        }

        private void RefreshClientList()
        {
           cardView1.RefreshData();

            //tileView1.RefreshData();
        }
        private void RefreshEventList()
        {
            tileView1.RefreshData();
        }

        private void RefreshClientList(int row)
        {
            cardView1.RefreshRow(row);

        }
        public void OnReceived(INetworkSocket socket, Packet receivedPacket)
        {

            //ListTimerViews.OnreceivedTimerMessage(socket, receivedPacket, ListEventViews);

            TimerView timerView = FindTimerViaSocket(socket);
            //timerView.DealWithPacket(receivedPacket, EventViews);
            try
            {
                string cmd = StringFromByteArr(receivedPacket.PacketRaw);
                string[] s = cmd.Split(',');

                if (s.Count() >= 2 && timerView != null)
                {
                    switch (s[0])
                    {
                        case "chipid":
                            timerView.ChipId = Convert.ToInt32(s[1]);
                            BindChip(timerView);
                            break;
                        case "timerstatus":
                            if (timerView.ChipId != null)
                            {
                                timerView.Status = (TimerView.TimerStatus)Convert.ToInt32(s[1]);
                                timerView.Time_used = new TimeSpan(Convert.ToInt64(s[3]) / 1000 * 10000000);

                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                SetTipMessage(e.Message);
            }
           


        }


        private void BindChip(TimerView timerView)
        {

            EventView eventView = EventViews.First(t => t.ChipId == timerView.ChipId);           
            eventView.Timer = timerView;
            timerView.EventName = eventView.Name;
            SendMessageToSocket("setdata,eventname,"+eventView.Name,timerView.Socket);
            SendMessageToSocket("setdata,timelimit," + eventView.Time_limit.GetValueOrDefault().TotalSeconds, timerView.Socket);


        }



        private void UnBindChip(TimerView timerView)
        {

            foreach (EventView eventView in EventViews.FindAll(t => t.ChipId == timerView.ChipId))
            {
                eventView.Timer = null;
            }

        }

        public TimerView FindTimerViaSocket(INetworkSocket socket)
        {
            return TimerViews.Find(t => t.IpAddress == socket.IPInfo.IPAddress);
        }



        public void OnSent(INetworkSocket socket, SendStatus status, Packet sentPacket)
        {
            switch (status)
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
        public void SetTipMessage(string message)
        {
            barStatusTxt.Caption = message;
        }
        public void OnServerAccepted(INetworkServer server, INetworkSocket socket)
        {
           // throw new NotImplementedException();
        }

        public void OnServerStarted(INetworkServer server, StartStatus status)
        {
            //TCP服务器启动
            serveStatus_start.Visibility = BarItemVisibility.Always;
            serveStatus_stop.Visibility = BarItemVisibility.Never;

            ribbonbtn_serveAct_start.Enabled = false;
            ribbonbtn_serveAct_stop.Enabled = true;

        }

        public void OnServerStopped(INetworkServer server)
        {
            serveStatus_start.Visibility = BarItemVisibility.Never;
            serveStatus_stop.Visibility = BarItemVisibility.Always;
            ribbonbtn_serveAct_start.Enabled = true;
            ribbonbtn_serveAct_stop.Enabled = false;
        }


        String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


        byte[] BytesFromString(String str, string encoding)
        {
            byte[] bytes_encoding = Encoding.GetEncoding(encoding).GetBytes(str);
            return bytes_encoding;
        }


        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ServerOps ops = new ServerOps(this, ribbontxt_port.EditValue.ToString(), this);
            //ListEventViews.list.Clear(); //初始化计时器集合
            TimerViews.Clear();
            m_server.StartServer(ops);
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            EventEdit form = new EventEdit();
            form.Show();
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_server.IsServerStarted)
            {
               
                m_server.StopServer();

            }
        }

        private void barButtonItem1_ItemClick_1(object sender, ItemClickEventArgs e)
        {
         
       

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form1 fr = new Form1();
            fr.Show();
        }

        private void barButtonItem2_ItemClick_1(object sender, ItemClickEventArgs e)
        {
           BroadcastMessage("timerstart");
           
        }

        private void BroadcastMessage(string sendText)
        {
            byte[] bytes = BytesFromString(sendText, "gb2312");
            Packet packet = new Packet(bytes, 0, bytes.Count(), false);
            m_server.Broadcast(packet);
        }

        private void SendMessageToSocket(string sendText, INetworkSocket socket)
        {
            byte[] bytes = BytesFromString(sendText, "gb2312");
            Packet packet = new Packet(bytes, 0, bytes.Count(), false);
            socket.Send(packet);
        }


        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            BroadcastMessage("timerstop");
        }





        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            BroadcastMessage("timerreset");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshClientList();
            RefreshEventList();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if(lookUpEdit1.EditValue!=null)   //选择了项目再继续
            {
                int[] rowhandles = cardView1.GetSelectedRows(); //确定选择了哪行
                if (rowhandles.Count() > 0)
                {
                    foreach (int rowhandle in rowhandles)  //可以处理多选
                    {
                        try
                        {
                            int chipid = Convert.ToInt32(cardView1.GetRowCellValue(rowhandle, "ChipId"));  //查找view里的chipid
                            int eventid = Convert.ToInt32(lookUpEdit1.EditValue);  //查找对应的比赛项目id
                            ChangeEventRegister(chipid, eventid);

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }


                    }

                }
            }
            else
            {
                MessageBox.Show("请选择一个项目");
            }
            
        }

        private void ChangeEventRegister(int chipid, int eventid)
        {
            TimerView timer = TimerViews.First(t => t.ChipId == chipid); //查找对应的timer对象
                                                                         //timer.ChangeEventRegister(eventid, ListEventViews);
            UnBindChip(timer);//先取消与此timer有关的eventView注册
            UnRegisterChip(chipid);

            EventView eventView = EventViews.Find(t => t.Id == eventid);  //查找对应的eventView对象
            eventView.ChipId = chipid;  //设置内存里EventView里的chipid
            BindChip(timer); //注册修改后的timer对象到eventView中
            timerContext.Events.First(t => t.Id == eventid).ChipId = chipid;//修改数据库里的event的chipid
            timerContext.SaveChanges();
        }

        private void UnRegisterChip(int chipid)
        {
            foreach (EventView eventView in EventViews.FindAll(t => t.ChipId == chipid))
            {
                eventView.ChipId = null;
                timerContext.Events.Find(eventView.Id).ChipId = null;
            }
            timerContext.SaveChanges();
  
        }

        private void tileView1_ItemCustomize(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventArgs e)
        {
           
        
      


                if ((bool)tileView1.GetRowCellValue(e.RowHandle, timer_online))
                {
                    e.Item.Elements[4].Text = "";
                    e.Item.Elements[4].Appearance.Normal.BackColor = Color.Transparent;

                    TimerView.TimerStatus status = (TimerView.TimerStatus)tileView1.GetRowCellValue(e.RowHandle, timer_status);
                    switch (status)
                    {
                        case TimerView.TimerStatus.未准备:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.Gray;
                            break;
                        case TimerView.TimerStatus.手动模式准备:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.Orange;
                            break;
                        case TimerView.TimerStatus.手动模式计时:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.LawnGreen;
                            break;
                        case TimerView.TimerStatus.手动模式停止:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.Red;
                            break;
                        case TimerView.TimerStatus.自动模式准备:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.Orange;
                            break;
                        case TimerView.TimerStatus.自动模式计时:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.LawnGreen;
                            break;
                        case TimerView.TimerStatus.自动模式停止:
                            e.Item.Elements[5].Appearance.Normal.BackColor = Color.Red;
                            break;
                        default:
                            break;
                    }
                }
          
          
           
        }

        private void tileView1_ItemRightClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
          
        }

        private void barButton_SelectedStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (EventView eventView in EventViews.FindAll(t => t.Checked == true))
            {
                eventView.Timer.SendMessage("timerstop");

            }
        }

        private void barButton_SelectedStart_ItemClick(object sender, ItemClickEventArgs e)
        {

            foreach (EventView eventView in EventViews.FindAll(t => t.Checked == true))
            {
                eventView.Timer.SendMessage("timerstart");

            }
        }

        private void barButton_SelectedReset_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (EventView eventView in EventViews.FindAll(t=>t.Checked==true))
            {
               eventView.Timer.SendMessage("timerreset");
           
            }
        }

        private void tileView1_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            
            
            int eventId = (int)tileView1.GetRowCellValue(e.Item.RowHandle, Id);
            EventView eventView = EventViews.First(t => t.Id == eventId);
            eventView.Checked = !eventView.Checked;
        }

        private void barButton_CheckClear_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (EventView eventView in EventViews)
            {
                eventView.Checked = false;

            }
        }
    }
}