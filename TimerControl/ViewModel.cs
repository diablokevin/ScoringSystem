using EpServerEngine.cs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimerControl
{
    public class TimerView
    {
        public enum TimerStatus
        {
            未准备=0,
            手动模式准备=10,
            手动模式计时= 11,
            手动模式停止= 12,
            自动模式准备 = 20,
            自动模式计时 = 21,
            自动模式停止 = 22
        };

        public int? ChipId { get; set; }
        public TimerStatus Status { get; set; } = 0;

        public TimeSpan Time_used { get; set; } = new TimeSpan(0, 0, 0);
        public string IpAddress { get; set; }
  
        public string EventName { get; set; }

        public EventView EventView { get; set; }

        public INetworkSocket Socket { get; set; }

        public void BindToEventView(AllEventViews allEventViews)
        {
            EventView eventView = allEventViews.list.Find(t => t.ChipId == this.ChipId);
            if(eventView!=null)
            {
                eventView.BindTimerView(this);
            }

           

        }

        public void UnBindToEventView()
        {
            if(this.EventView!=null)
            {          
                this.EventView.ClearTimerBinding();
                this.EventView = null;
            }
        }

        public void ChangeEventRegister(int eventId, AllEventViews allEventViews)
        {
            UnBindToEventView();
            EventView eventView= allEventViews.list.Find(t => t.Id == eventId);
            eventView.RegisterChip(this.ChipId??0);
            BindToEventView(allEventViews);

        }

        public void DealWithPacket(Packet receivedPacket, AllEventViews allEventViews)
        {
            string cmd = StringFromByteArr(receivedPacket.PacketRaw);
            string[] s = cmd.Split(',');
            if (s.Count() >= 2 )
            {
                switch (s[0])
                {
                    case "chipid":
                        this.ChipId = Convert.ToInt32(s[1]);
                        this.BindToEventView(allEventViews);
                        break;
                    case "timerstatus":

                        this.Status = (TimerView.TimerStatus)Convert.ToInt32(s[1]);
                        this.Time_used = new TimeSpan(Convert.ToInt64(s[3]) / 1000 * 10000000);

                        break;
                    default:
                        break;
                }
            }

        }
        public void SendMessage(string sendText)
        {
            byte[] bytes = BytesFromString(sendText, "gb2312");
            Packet packet = new Packet(bytes, 0, bytes.Count(), false);
            this.Socket.Send(packet);
        }
        private String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        private byte[] BytesFromString(String str, string encoding)
        {
            byte[] bytes_encoding = Encoding.GetEncoding(encoding).GetBytes(str);
            return bytes_encoding;
        }
    }

    public class AllTimerViews
    {
        public AllTimerViews()
        {

        }
        public List<TimerView> list { get; set; } = new List<TimerView>();

        public TimerView FindTimerViewByIpaddress(string ip)
        {
            return  list.First(t => t.IpAddress == ip)??null;
        }

        public void OnreceivedTimerMessage(INetworkSocket socket, Packet receivedPacket,AllEventViews allEventViews)
        {
            TimerView timerView = FindTimerViewByIpaddress(socket.IPInfo.IPAddress);

            timerView.DealWithPacket(receivedPacket, allEventViews);
        }

        public void OnTimerDisconnect(INetworkSocket socket,AllEventViews allEventViews)
        {
            TimerView timerView = FindTimerViewByIpaddress(socket.IPInfo.IPAddress);

            timerView.UnBindToEventView();         
            this.list.Remove(timerView);//去掉Timer列表里的项目
        }
    }

    public class EventView
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ChipId { get; set; }
        public TimeSpan? Time_limit { get; set; }
        public TimerView Timer { get; set; }
        public bool Checked { get; set; } 
        public bool TimerOnline
        { get
            {
                return this.Timer != null;
            }
        }
        public Competitor Competitor { get; set; }  

        public void ClearTimerBinding()
        {
            this.Timer = null;
        }

        public void BindTimerView(TimerView timerView)
        {
            this.Timer = timerView;
            timerView.EventView = this;
        }

        public void RegisterChip(int chipId)
        {
            this.ChipId = ChipId;
        }

        public void UnRegisterChip(int chipId)
        {
            this.ChipId = null;
        }

    }

    public class AllEventViews
    {
        public List<EventView> list { get; set; }

        public AllEventViews(TimerContext dbContext)
        {
            list = new List<EventView>();
            foreach (Event item in dbContext.Events.ToList())
            {
                EventView eventView = new EventView();
                eventView.ChipId = item.ChipId;
                eventView.Name = item.Name;
                eventView.Time_limit = item.TimeLimit;
                eventView.Id = item.Id;

                list.Add(eventView);

            }
        }

        private void RegisterChip(TimerView timerView)
        {

            foreach (EventView eventView in list.FindAll(t => t.ChipId == timerView.ChipId))
            {
                eventView.Timer = timerView;
                timerView.EventName += eventView.Name + ";";
            }


        }



        private void UnRegisterChip(TimerView timerView)
        {

            foreach (EventView eventView in list.FindAll(t => t.ChipId == timerView.ChipId))
            {
                eventView.Timer = null;
            }

        }
    }

    public class Competitor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Race_num { get; set; }
        public string Company { get; set; }
    }
}
