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
            session.Room = this;
            session.ReadyStatus = 0;
            isFireReady = true;
            Host = session.PlayerId;
            Title = title;
            MaxPlayer = max;
            NowPlayer++;
            Stage = "1";
            State = true;
            _sessions.Add(session);
            S_CreateRoomResult pkt = new S_CreateRoomResult();
            // 생성 성공
            pkt.title = this.Title;
            pkt.stage = this.Stage;
            pkt.maxPlayer = this.MaxPlayer;
            pkt.nowPlayer = this.NowPlayer;
            session.Send(pkt.Write());
        }

     

        public void Broadcast(ArraySegment<byte> segment)
        {
            foreach (ClientSession s in _sessions)
                s.Send(segment);
        }
        /// <summary>
        /// 캐릭터가 방에 접속 했을 때 실행
        /// </summary>
        /// <param name="session"></param>
        public void Enter(ClientSession session)
        {
            // 새로들어온 유저에게 룸 객체 지엉
            session.Room = this;
            session.RoomId = this.RoomId;
            session.ReadyStatus = -1;
            ++NowPlayer;
            isWaterReady = true;

            // 방에 입장해 유저에게 방의 현재상태를 알려줄 패킷 생성
            S_EnterRoomOk pkt = new S_EnterRoomOk();
            pkt.title = this.Title;
            pkt.stage = this.Stage;
            pkt.maxPlayer = this.MaxPlayer;
            pkt.nowPlayer = this.NowPlayer;

            S_EnterRoomOk.PlayerReady pr = new S_EnterRoomOk.PlayerReady();            
            foreach (ClientSession s in _sessions)
            {
                pr.playerId = s.PlayerId;
                pr.readyStatus = s.ReadyStatus;
                pkt.playerReadys.Add(pr);
            }
            session.Send(pkt.Write());
            
            // 새로 들어간방의 유저들에게 들어갔음을 알려준다.
            S_BroadCastEnterRoom bEnterRoom = new S_BroadCastEnterRoom();
            bEnterRoom.playerId = session.PlayerId;
            _sessions.Add(session);
            Push(() => Broadcast(bEnterRoom.Write()));
            Console.WriteLine("전체에게 뿌려줌");

        }

        // 사용 안함
        public void EnterRoom(ClientSession session, C_Enter packet) 
        {
            ClientSession Mysession;
            Mysession = _sessions.Find(x => x.SessionId == session.SessionId);
            Mysession.Attr = packet.attr;
            Mysession.PosX = packet.posX;
            Mysession.PosY = packet.posY;
            Mysession.PosZ = packet.posZ;


            _sessions.Remove(_sessions.Find(x => x.SessionId == session.SessionId));
            _sessions.Add(Mysession);

            // 새로 들어온 플레이어에게 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
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

            // 새로 들어온 플레이어의 입장을 모든 플레이어에게 알린다.
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.PlayerId;
            enter.attr = packet.attr;
            enter.posX = packet.posX;
            enter.posY = packet.posY;
            enter.posZ = packet.posZ;            
            Broadcast(enter.Write());
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

        /// <summary>
        /// 플레이어가 레디를 할때 호출 packet.result = 0 : fire요청, packet.result = 1 : water요청
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void Ready(ClientSession session, C_Ready packet)
        {
            S_BroadCastReady pkt = new S_BroadCastReady();
            if (packet.result == 0)
            {
                if (!this.isFireReady)
                {
                    isFireReady = true;
                    session.ReadyStatus = 0;
                    pkt.playerID = session.PlayerId;
                    pkt.result = packet.result;
                }
            }
            else if (packet.result == 1)
            {
                if (!this.isWaterReady)
                {
                    isWaterReady = true;
                    session.ReadyStatus = 1;
                    pkt.playerID = session.PlayerId;
                    pkt.result = packet.result;
                }
            }
            Push(() => session.Send(pkt.Write()));

        }
        public void ReadyCancel(ClientSession session, C_ReadyCancle packet)
        {
            //S_ReadyCancel pkt = new S_ReadyCancel();
            //if (packet.result == 0)
            //{
            //    isFireReady = true;
            //    session.ReadyStatus = 0;
            //    pkt.playerID = session.PlayerId;
            //    pkt.result = packet.result;
            //}
            //else if (packet.result == 1)
            //{

            //}
            
        }

        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;
            Console.WriteLine("in");


            // 모두에게 알린다.
            S_BroadCastMove move = new S_BroadCastMove();
            move.playerId = session.PlayerId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

            Console.WriteLine(move.playerId + move.posX + move.posY + move.posZ);
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
        /// 게임 시작
        /// </summary>
        /// <param name="session"></param>
        public void GameStart(ClientSession session, C_GameStart packet)
        {
            if(session.PlayerId == this.Host)
            {
                if(isFireReady && isWaterReady)
                {
                    S_GameStart pkt = new S_GameStart();
                    pkt.stageCode = packet.stageCode;
                    StartGameTime = DateTime.Now.ToString();
                     Broadcast(pkt.Write());
                    Console.WriteLine($"{Host}의 방 게임 시작");
                }
                else
                {
                    S_GameStart pkt = new S_GameStart();
                    pkt.stageCode = packet.stageCode;
                    StartGameTime = DateTime.Now.ToString();
                    Broadcast(pkt.Write());
                    Console.WriteLine($"레디가 되지않았지만 게임 시작");
                }
            }
            else
            {
                S_GameStartFaild pkt = new S_GameStartFaild();
                pkt.result = 1;
                session.Send(pkt.Write());
                Console.WriteLine($"호스트가 아님당 ");
            }

        }
        /// <summary>
        ///  플레이어가 죽거나 다시시작을 눌렀을 때
        /// </summary>
        /// <param name="session"></param>
        public void GameOver(ClientSession session)
        {
            C_GameOver gameOverPacket = new C_GameOver();
            StartGameTime = DateTime.Now.ToString();            
            Broadcast(gameOverPacket.Write());
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
