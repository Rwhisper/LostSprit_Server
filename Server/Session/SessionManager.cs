using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
/// <summary>
/// 전체 접속한 유저들 관리하는 클래스
/// </summary>
	class SessionManager : IJobQueue
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }


		JobQueue _jobQueue = new JobQueue();

		int _sessionId = 0;
		int _roomId = 1;
		/// <summary>
		/// 서버에 접속한 유저 정보
		/// </summary>
		Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
		/// <summary>
		/// 로그인한 유저들의 정보
		/// </summary>
		Dictionary<string, ClientSession> _loginSession = new Dictionary<string, ClientSession>();
		Dictionary<int, GameRoom> _gameRoom = new Dictionary<int, GameRoom>();


		object _lock = new object();

		public void Push(Action job)
		{
			_jobQueue.Push(job);
		}

		public void Flush()
		{
			
			//// N ^ 2
			//foreach (ClientSession s in _sessions)
			//	s.Send(_pendingList);

			////Console.WriteLine($"Flushed {_pendingList.Count} items");
			//_pendingList.Clear();
		}

		public ClientSession Generate()
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = new ClientSession();
				session.SessionId = sessionId;
				_sessions.Add(sessionId, session);

				Console.WriteLine($"Connected : {sessionId}");

				return session;
			}
		}
		// 유저 찾기
		public ClientSession Find(int id)
		{
			lock (_lock)
			{
				ClientSession session = null;
				_sessions.TryGetValue(id, out session);
				return session;
			}
		}



		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session.SessionId);
			}
		}

		/// <summary>
		/// 로그인 확인 함수
		/// </summary>
		/// <param name="session"></param>
		/// <param name="packet"></param>
		public void Login(ClientSession session, C_Login packet)
        {
            lock (_lock)
            {
				// 이부분은 db연동해서 수정 할 것
				if (packet.id == "test1" || packet.pwd == "1234")
				{
					Console.WriteLine($"{packet.id} 로그인");
					session.PlayerId = packet.id;
					_loginSession.Add(packet.id, session);
					S_LoginResult pkt = new S_LoginResult();
					pkt.result = 1;
					session.Send(pkt.Write());
				}
				else if(packet.id == "test2" || packet.pwd == "1234")
                {
					Console.WriteLine($"{packet.id} 로그인");
					session.PlayerId = packet.id;
					_loginSession.Add(packet.id, session);
					S_LoginResult pkt = new S_LoginResult();
					pkt.result = 1;
					session.Send(pkt.Write());
				}
				else if(packet.id == "test3" || packet.pwd == "1234")
                {
					Console.WriteLine($"{packet.id} 로그인");
					session.PlayerId = packet.id;
					_loginSession.Add(packet.id, session);
					S_LoginResult pkt = new S_LoginResult();
					pkt.result = 1;
					session.Send(pkt.Write());
				}
                else
                {
					S_LoginResult pkt = new S_LoginResult();
					pkt.result = 0;
					session.Send(pkt.Write());
                }

			}
            
        }

		public void Logout(ClientSession session)
        {

			_loginSession.Remove(session.PlayerId);
			if(session.Room.Roomid > 0)
            {
				_gameRoom[session.Room.Roomid].LeaveRoom(session);
			}
			
        }
		/// <summary>
		/// 룸 리스트 요청 패킷이 넘어 올때 실행
		/// </summary>
		public void RoomList()
        {
			List<Room> _roomList = new List<Room>();
			Room gm = new Room();
			foreach (GameRoom room in _gameRoom.Values)
            {
				gm.Roomid = room.Roomid;
				gm.Host = room.Host;
				gm.nowPlayer = room.nowPlayer;
				gm.maxPlayer = room.maxPlayer;
				gm.Roomid = room.Roomid;
				gm.RoomName = room.RoomName;
				_roomList.Add(gm);
            }
			// 룸 리스트 반환 패킷
        }

		/// <summary>
		/// 룸생성 요청을 처리
		/// </summary>
		/// <param name="session"></param>
		/// <param name="packet"></param>
		public void CreateRoom(ClientSession session, C_CreateRoom packet)
        {
			lock (_lock)
			{
				int roomId = ++_roomId;
				// 룸 아이디 저장
				session.Room.Roomid = roomId;
				GameRoom createRoom = new GameRoom();
				// 설정한 최대 유저수
				createRoom.maxPlayer = packet.maxUser;
				// 만든 룸에 호스트 집어넣기
				createRoom.Push(() => createRoom.Enter(session));
				// 룸 리스트에 룸 추가
				_gameRoom.Add(roomId, createRoom);

				Console.WriteLine($"CreateRoom : {roomId}, Host : {session.PlayerId}");
			}
		}
		/// <summary>
		/// 새로이 룸에 들어가기
		/// </summary>
		/// <param name="session"></param>
		/// <param name="packet"></param>
		public void RoomEnter(ClientSession session, C_RoomEnter packet)
        {
			if (_gameRoom.TryGetValue(packet.roomNumber, out GameRoom room))
            {
				if(room.maxPlayer < room.nowPlayer)
                {
					room.Push(() =>room.Enter(session));
				}					
                else
                {
					S_RoomConnFaild pkt = new S_RoomConnFaild();
					pkt.result = 1;
					session.Send(pkt.Write());
				}

			}			
            else
            {
				S_RoomConnFaild pkt = new S_RoomConnFaild();
				pkt.result = 0;
				session.Room.Roomid = packet.roomNumber;
				session.Send(pkt.Write());			
            }
		}

		/// <summary>
		/// 방에서 나가기
		/// </summary>
		/// <param name="session"></param>
		public void LeaveRoomSession(ClientSession session)
		{
			
			if (_gameRoom.TryGetValue(session.Room.Roomid, out GameRoom room))
			{
				room.LeaveRoom(session);
				session.Room.Roomid = 0;
				if(room.nowPlayer <= 0)
                {
					_gameRoom.Remove(session.Room.Roomid);
                }
			}
		}

		/// <summary>
		/// 전체 랭킹 불러오기 (stage)
		/// </summary>
		/// <param name="stage"></param>
		/// <returns></returns>
		public List<Ranking> GetRankingList(string stage)
        {
			List<Ranking> _rankList = new List<Ranking>();


			return _rankList;
		}

		/// <summary>
		/// 한명의 랭킹 불러오기
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public List<Ranking> GetRanking(string id )
        {
			List<Ranking> _rankList = new List<Ranking>();


			return _rankList;
        }

		public void GameStart(ClientSession session)
        {
			GameRoom Room = session.Room;

        }

		public void GameClear()
        {

        }

		public void GameOver()
        {

        }

	}
}
