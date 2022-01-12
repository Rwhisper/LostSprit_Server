using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    static class Constants
    {
        // Ready State
        public const int Ready_Fire = 0;
        public const int Ready_Water = 1;
        
        // LoginFaild
        public const int LoginFaild_NotFoundID = 0;
        public const int LoginFaild_NotMatchPasswd = 1;
        
        // LoginSucces
        public const int LoginSucces_Succes = 0;
        public const int LoginSucces_FoundLogin = 1;

        // EnterRoomFaild
        public const int EnterRoomFaild_NotFoundRoom = 0;
        public const int EnterRoomFaild_MaxRoom = 1;
        public const int EnterRoomFaild_StartRoom = 2;


    }
}
