using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectServer.ServerConnector
{
    public class ServerObject
    {
        public short ServerCode { get; set; }
        public byte Percent { set; get; }
        public byte PlayType { set; get; }
        public string IP { get; set; }
        public short Port { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
