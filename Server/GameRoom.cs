using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        public int RoomId { get; set; }
        public string Title { get; set; }
        public string Host { get; set; }
        public int MaxPlayer { get; set; }
        public int NowPlayer { get; set; }
        public bool State { get; set; }
        public string Stage { get; set; }
        public string StartGameTime { get; set; }
        public string EndGameTime { get; set; }
        public bool isFireReady { get; set; }
        public bool isWaterReady { get; set; }

        int loddingCnt;
        public List<ClientSession> GetSessions()
        {
            List<ClientSession> s = new List<ClientSession>();
            s = this._sessions;

            return s;
        }
        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public GameRoom()     
        {
            RoomId = 0;
            Host = null;
            MaxPlayer = 0;
            NowPlayer = 0;
            State = false;
            Stage = "1";
            isFireReady = false;
            isWaterReady = false;
            StartGameTime = null;
            EndGameTime = null;
        }
        public GameRoom(int _roomId, int _maxPalyer, string _host, string _title)
        {
            Title = _title;
            RoomId = _roomId;
            Host = _host;
            MaxPlayer = _maxPalyer;
            NowPlayer = 0;
            State = false;
            Stage = "1";
            isFireReady = false;
            isWaterReady = false;
            StartGameTime = null;
            EndGameTime = null;
            loddingCnt = 0;
        }

        // 지속적으로 패킷 모아서 보내기
        //public void Flush()
        //{
        //    // N ^ 2
        //    foreach (ClientSession s in _sessions)
        //        s.Send(_pendingList);

        //    //Console.WriteLine($"Flushed {_pendingList.Count} items");
        //    _pendingList.Clear();
        //}
        
        public void CreateRoom(ClientSession session, int max, string title)
        {
            session.ReadyStatus = 0;  
            Host = session.PlayerId;
            Title = title;
            MaxPlayer = max;
            NowPlayer++;
            Stage = "1";
            State = true;
            session.Attr = "fire";
            _sessions.Add(session);

            S_CreateRoomResult pkt = new S_CreateRoomResult();
            // 생성 성공
            pkt.title = this.Title;
            pkt.stage = this.Stage;
            pkt.maxPlayer = this.MaxPlayer;
            pkt.nowPlayer = this.NowPlayer;
            session.Send(pkt.Write());
        }    

        // 방의 모든 인원에게 패킷 보내기
        public void Broadcast(ArraySegment<byte> segment)
        {
            foreach (ClientSession s in _sessions)
                s.Send(segment);
        }

        // Ready 요청이 왔을때 혹시 요청하는 속성이 같은 방의 누군가가 이미 선택한 속성이라면 false 반환
        bool GetReadyStatus(int n)
        {
            foreach(var s in _sessions)
            {
                if (s.ReadyStatus == n)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 모든 유저들이 Ready가 되어있음을 확인하여 반환하는 함수
        /// </summary>
        /// <returns></returns>
        bool AllReady()
        {
            foreach (var s in _sessions)
            {
                if (s.ReadyStatus == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 캐릭터가 방에 접속 했을 때 실행
        /// </summary>
        /// <param name="session"></param>
        public void Enter(ClientSession session)
        {
            //// 새로들어온 유저에게 룸 객체 지엉
            //session.Room = this;
            session.RoomId = this.RoomId;
            //session.ReadyStatus = -1;
            ++NowPlayer;
            //isWaterReady = true;

            // 방에 입장해 유저에게 방의 현재상태를 알려줄 패킷 생성
            //ShowRoomInfo(session);

            //새로 들어온 플레이어에게 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();            
            
            session.Attr = "water";
            session.PosX = 0;
            session.PosY = 1;
            session.PosZ = 5;
            foreach (ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.PlayerId,
                    attr = s.Attr,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
                Console.WriteLine($"{s.Attr}, {s.PosX}, {s.PosY}, {s.PosZ}");
            }
            session.Send(players.Write());
            session.RoomId = RoomId;

            S_EnterRoomOk pkt = new S_EnterRoomOk();
            session.Send(pkt.Write());
            _sessions.Add(session);
            Console.WriteLine("입장 성공");
            //session.Send(players.Write());           
        }
        /// <summary>
        /// 채팅 처리 함수
        /// </summary>
        /// <param name="c_chat"></param>
        /// <param name="playerID"></param>
        public void Chatting(C_Chat c_chat, string playerID)
        {
            S_Chat pkt = new S_Chat();
            pkt.playerId = playerID;
            pkt.chat = c_chat.chat;

            Broadcast(pkt.Write());
        }
        public void ShowRoomInfo(ClientSession  session)
        {
            S_RoomInfo pkt = new S_RoomInfo();
            pkt.title = this.Title;
            pkt.stage = this.Stage;
            pkt.maxPlayer = this.MaxPlayer;
            pkt.nowPlayer = this.NowPlayer;

            S_RoomInfo.PlayerReady pr = new S_RoomInfo.PlayerReady();
            foreach (ClientSession s in _sessions)
            {
                pr.playerId = s.PlayerId;
                pr.readyStatus = s.ReadyStatus;
                pkt.playerReadys.Add(pr);
            }
            session.Send(pkt.Write());
        }


        // 사용 함
        public void EnterRoom(ClientSession session) 
        {

            // 이부분 수정하면 좋을것 같음
            S_BroadCastEnterRoom enterRoom_packet = new S_BroadCastEnterRoom();
            enterRoom_packet.playerId = session.PlayerId;
            Broadcast(enterRoom_packet.Write());

            _sessions.Add(session);
            this.NowPlayer++;
            EnterRoomOk(session);

        }

        public void EnterRoomOk(ClientSession session)
        {
            S_EnterRoomOk enterOk_packet = new S_EnterRoomOk();
            enterOk_packet.title = this.Title;
            enterOk_packet.host = this.Host;
            enterOk_packet.maxPlayer = this.MaxPlayer;
            enterOk_packet.nowPlayer = this.NowPlayer;
            enterOk_packet.stage = this.Stage;
            foreach(var s in _sessions)
            {
                S_EnterRoomOk.Player players = new S_EnterRoomOk.Player();
                players.playerID = s.PlayerId;
                players.ready = s.ReadyStatus;
                enterOk_packet.players.Add(players);
            }

            session.Send(enterOk_packet.Write());
        }


        /// <summary>
        /// 호스트가 나가면 실행
        /// </summary>
        /// <param name="session"></param>
        public void LeaveHost(ClientSession session)
        {
            foreach (ClientSession s in _sessions)
            {
                S_RoomConnFaild pkt = new S_RoomConnFaild();
                pkt.result = 0;
                s.Room = null;
                Broadcast(pkt.Write());
            }
        }
        public void Leave(ClientSession session)
        {
            session.Room = null;
            session.RoomId = 0;
            // 플레이어를 제거하고
            _sessions.Remove(session);
            --NowPlayer;
            // 모두에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.PlayerId;
            Broadcast(leave.Write());
        }

        private void ModifyReady(ClientSession session, int ready)
        {
            for(int i =0; i < _sessions.Count; i++)
            {
                if(_sessions[i].PlayerId == session.PlayerId)
                {
                    Console.WriteLine("방에서 Ready요청에 해당하는 유저 찾음");
                    _sessions[i].ReadyStatus = ready;
                    return;
                }
            }
        }
        /// <summary>
        /// 플레이어가 레디를 할때 호출 packet.result = 0 : fire요청, packet.result = 1 : water요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void Ready(ClientSession session, C_Ready packet)
        {
            S_BroadCastReady pkt = new S_BroadCastReady();
            if(GetReadyStatus(packet.result))
            {
                ModifyReady(session, packet.result);
                pkt.playerID = session.PlayerId;
                pkt.result = packet.result;
                Console.WriteLine(session.PlayerId + "캐릭터 선택" + pkt.result);
            }
            else
            {
                // 이미 선택한 유저가 있기때문에 실행 안됨
                // 클라쪽에서 애초에 막겠지만 혹시 모를 상황을 대비하여
                Console.WriteLine("이미 선택한 유저가 있습니다.");
            }

            session.Send(pkt.Write());

        }
        /// <summary>
        /// Ready취소 함수
        /// </summary>
        /// <param name="session"></param>
        public void ReadyCancel(ClientSession session)
        {
            S_ReadyCancel pkt = new S_ReadyCancel();
            pkt.playerId = session.PlayerId;
            Broadcast(pkt.Write());

            Console.WriteLine(pkt.playerId + "의 Ready Cancle ");
        }

        /// <summary>
        /// 스테이지 수정 함수
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void StageChange(ClientSession session, C_StageChange packet)
        {
            if (session.PlayerId != Host)
                return;
            this.Stage = packet.stageCode;
            S_BroadCastStageChange pkt = new S_BroadCastStageChange();
            pkt.stageCode = packet.stageCode;
            Console.WriteLine($"{RoomId} 방 스테이지 체인지");
            Broadcast(pkt.Write());
        }

        
        /// <summary>
        /// 캐릭터 움직임 함수 ()
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;
            //Console.WriteLine("in");


            // 모두에게 알린다.
            S_BroadCastMove move = new S_BroadCastMove();
            move.playerId = session.PlayerId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

            //Console.WriteLine(move.playerId + move.posX + move.posY + move.posZ);
            Broadcast(move.Write());
        }

        public void Rot(ClientSession session, C_Rot packet)
        {
            session.RotX = packet.rotX;
            session.RotY = packet.rotY;
            session.RotZ = packet.rotZ;
            session.RotW = packet.rotW;

            S_BroadCastRot rot = new S_BroadCastRot();
            rot.playerId = session.PlayerId;
            rot.rotX = session.RotX;
            rot.rotY = session.RotY;
            rot.rotZ = session.RotX;
            rot.rotW = session.RotW;
            Broadcast(rot.Write());

        }
        /// <summary>
        /// 로딩이 완료되었을때 호출하는 함수 (클라이언트에서 로딩완료 요청 보냄)
        /// </summary>
        /// <param name="session"></param>
        public void Lodding_Complete(ClientSession session)
        {
            for(int i = 0; i < _sessions.Count; i++)
            {
                if(_sessions[i].PlayerId == session.PlayerId)
                {
                    _sessions[i].lodding = true;
                    Console.WriteLine($"{session.PlayerId} Lodding완료");
                }
            }
        }

        /// <summary>
        /// 방의 모든 인원이 로딩이 되었는지 확인하는 함수
        /// </summary>
        /// <returns></returns>
        private bool AlllLodding()
        {
            foreach(var s in _sessions)
            {
                if (s.lodding != false)
                    return false;
            }
            return true;
        }
        public void EnterGame(ClientSession session)
        {
            if(Host != session.PlayerId)
            {
                Console.WriteLine("호스트가 아닙니다.");
                return;
            }
            Console.WriteLine("게임 시작 요청");
            // 유저 Ready 상태 확인
            if(!AllReady())
            {
                Console.WriteLine("전체 유저가 레디가 되어있지 않습니다.");
                return;
            }
            S_EnterGame pkt = new S_EnterGame();
            pkt.stage = Stage;
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        /// <param name="session"></param>
        public void GameStart(ClientSession session, C_GameStart packet)
        {
            this.Stage = packet.stageCode;
            if(!AlllLodding())
            {
                Console.WriteLine("전체 로딩 안됨 Please Wait");
                return;
            }
            S_GameStart pkt = new S_GameStart();
            pkt.stageCode = packet.stageCode;
            StartGameTime = DateTime.Now.ToString();
            Broadcast(pkt.Write());

            Console.WriteLine($"{Title}방 게임 시작");
            
        }
        /// <summary>
        ///  플레이어가 죽거나 다시시작을 눌렀을 때
        /// </summary>
        /// <param name="session"></param>
        public void GameOver(ClientSession session)
        {
            Console.WriteLine(Host + "게임오버");
            S_GameOver pkt = new S_GameOver();
            StartGameTime = null;            
            Broadcast(pkt.Write());
        }
       

        public void DropItem(ClientSession session , C_DropItem packet)
        {
            S_BroadCastDropItem pkt = new S_BroadCastDropItem();
            pkt.itemId = packet.itemId;
            pkt.posX = packet.posX;
            pkt.posY = packet.posY;
            pkt.posZ = packet.posZ;
            Broadcast(pkt.Write());
        }

        public void DestroyItem(ClientSession session, C_DestroyItem packet)
        {
            S_BroadCastDestroyItem pkt = new S_BroadCastDestroyItem();
            pkt.itemId = packet.itemId;
            Broadcast(pkt.Write());
        }
    }
}
