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
        // 대기방인지 시작중인 방인지 확인하는 변수 true = 게임중, false = 대기중
        public bool state { get; set; }

    }
}
