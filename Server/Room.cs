using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Room
    {
        public int Roomid { get; set; }
        public string RoomName { get; set; }
        public string Host { get; set; }
        public int maxPlayer { get; set; }
        public int nowPlayer { get; set; }
    }
}
