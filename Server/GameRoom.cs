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
        Dictionary<string, Queue<string>> _room = new Dictionary<string, Queue<string>>();
        public int Roomid { get; set; }
        public string RoomName { get; set; }
        public string Host { get; set; }
        public int MaxPlayer { get; set; }
        public int NowPlayer { get; set; }
        public bool State { get; set; }
        public string Stage { get; set; }
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
        }

        public void LeaveHost(ClientSession session)
        {
            foreach(ClientSession s in _sessions)
            {
                S_RoomConnFaild pkt = new S_RoomConnFaild();
                pkt.result = 0;
                Broadcast(pkt.Write());
            }
        }

        public void Flush()
        {
            // N ^ 2
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }
        public void CreateRoom(ClientSession session, int max)
        {
            session.Room = this;
            Host = session.PlayerId;
            MaxPlayer = max;
            NowPlayer++;
            _sessions.Add(session);
        }

        public void Login(ClientSession session, C_Login packet)
        {
            if(packet.id == "test1" || packet.pwd == "1234")
            {
                Console.WriteLine("test1 접속 성공");
                session.PlayerId = packet.id;
                _sessions.Add(session);
            }
        }

        public void EnterLobby()
        {

        }
        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            session.Room = this;
            ++NowPlayer;
            _sessions.Add(session);            


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

        public void Leave(ClientSession session)
        {
            // 플레이어를 제거하고
            _sessions.Remove(session);
            --NowPlayer;
            // 모두에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.PlayerId;
            Broadcast(leave.Write());
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

        public void DestroyItem(ClientSession session, C_DestroyItem packet)
        {
            //S_BoradCastDestroyItem destroy = new S_BoradCastDestroyItem();
            //destroy.item = packet.item;
            //Broadcast(destroy.Write());
        }
    }
}
