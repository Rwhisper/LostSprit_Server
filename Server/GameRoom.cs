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
        public int Roomid { get; set; }
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
            Roomid = 0;
            Host = null;
            MaxPlayer = 0;
            NowPlayer = 0;
            State = false;
            Stage = "1";
            isFireReady = false;
            isWaterReady = false;
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
        public void CreateRoom(ClientSession session, int max)
        {
            session.Room = this;
            Host = session.PlayerId;
            MaxPlayer = max;
            NowPlayer++;
            _sessions.Add(session);
            S_CreateRoomResult pkt = new S_CreateRoomResult();
            Push(() => session.Send(pkt.Write()));
        }

     

        public void EnterLobby()
        {

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
            session.Room = this;
            ++NowPlayer;
            _sessions.Add(session);
            S_EnterRoomOk pkt = new S_EnterRoomOk();
            pkt.stage = this.Stage;
            pkt.maxPlayer = this.MaxPlayer;
            pkt.nowPlayer = this.NowPlayer;
            S_EnterRoomOk.PlayerReady pr = new S_EnterRoomOk.PlayerReady();
            S_BroadCastEnterRoom bEnterRoom = new S_BroadCastEnterRoom();
            bEnterRoom.playerId = session.PlayerId;
            Push(() => Broadcast(bEnterRoom.Write()));
            foreach (ClientSession s in _sessions)
            {
                pr.playerId = s.PlayerId;
                pr.readyStatus = s.ReadyStatus;
                pkt.playerReadys.Add(pr);
            }            
            session.Send(pkt.Write());
            Broadcast(bEnterRoom.Write());

        }

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
            else if(packet.result ==1)
            {
                if (!this.isWaterReady)
                {
                    isWaterReady = true;
                    session.ReadyStatus = 0;
                    pkt.playerID = session.PlayerId;
                    pkt.result = packet.result;
                }
            }
            Push(() => session.Send(pkt.Write()));

        }
        public void ReadyCancle(ClientSession session )
        {

        }

        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;


            // 모두에게 알린다.
            S_BroadCastMove move = new S_BroadCastMove();
            move.playerId = session.PlayerId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
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

        public void GameStart(ClientSession session)
        {
            if(session.PlayerId == this.Host)
            {
                if(isFireReady && isWaterReady)
                {
                    S_GameStart pkt = new S_GameStart();
                    Push(() => Broadcast(pkt.Write()));
                }
                else
                {
                    S_GameStartFaild pkt = new S_GameStartFaild();
                    pkt.result = 1;
                    Push(() => session.Send(pkt.Write()));
                }
            }
        }
        public void GameOver(ClientSession session)
        {
            C_GameOver gameOverPacket = new C_GameOver();
            StartGameTime = DateTime.Now.ToString();            
            Broadcast(gameOverPacket.Write());
        }

        public void DestroyItem(ClientSession session, C_DestroyItem packet)
        {
            //S_BoradCastDestroyItem destroy = new S_BoradCastDestroyItem();
            //destroy.item = packet.item;
            //Broadcast(destroy.Write());
        }
    }
}
