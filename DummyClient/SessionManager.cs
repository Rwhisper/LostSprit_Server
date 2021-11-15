using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		//List<ServerSession> _sessions = new List<ServerSession>();
		public static ServerSession session = new ServerSession();
		object _lock = new object();

		Random _rand = new Random();
		//public void SendForEach()
		//{
		//	lock (_lock)
		//	{
		//		foreach (ServerSession session in _sessions)
		//		{
		//			C_Move movePacket = new C_Move();
					
		//			movePacket.posX = _rand.Next(-50, 50);
		//			movePacket.posY = 0;
		//			movePacket.posZ = _rand.Next(-50, 50);
					
		//			session.Send(movePacket.Write());
		//		}
		//	}
		//}

		public void Login(string id , string pwd)
        {
            lock (_lock)
            {
				C_Login pkt = new C_Login();
				pkt.id = id;
				pkt.pwd = pwd;
				session.Send(pkt.Write());
			}
				
						
        }
		public void Logout()
        {
			C_Logout pkt = new C_Logout();
			session.Send(pkt.Write());
        }
		public void RoomListRequest()
        {
			C_RoomList pkt = new C_RoomList();
			session.Send(pkt.Write());
        }
		public void EnterRoom(int roomId)
        {
			C_RoomEnter pkt = new C_RoomEnter();
			pkt.roomId = roomId;
			session.Send(pkt.Write());
        }
		public void LeaveRoom()
        {
			C_LeaveRoom pkt = new C_LeaveRoom();
			session.Send(pkt.Write());
        }

		public void CreateRoom(string title, int maxPlayer)
        {
            lock (_lock)
            {
				C_CreateRoom pkt = new C_CreateRoom();
				pkt.title = title;
				pkt.maxUser = maxPlayer;
				session.Send(pkt.Write());
			}
			
        }
		public void GameStart()
        {
			C_GameStart pkt = new C_GameStart();
			session.Send(pkt.Write());
        }
		public void GameOver()
        {
			C_GameOver pkt = new C_GameOver();
			session.Send(pkt.Write());
        }
		public void Move()
        {
			C_Move pkt = new C_Move();
			pkt.posX = _rand.Next(-50, 50);
			pkt.posY = 0;
			pkt.posZ = _rand.Next(-50, 50);

            session.Send(pkt.Write());
        }
		public void Rot()
        {
			C_Rot pkt = new C_Rot();
			pkt.rotX = _rand.Next(-50, 50);
			pkt.rotY = 0;
			pkt.rotZ = _rand.Next(-50, 50);
			pkt.rotW = _rand.Next(-50, 50);

			session.Send(pkt.Write());
		}

		public ServerSession Generate()
		{
			lock (_lock)
			{
				 session = new ServerSession();
				return session;
			}
		}
	}
}
