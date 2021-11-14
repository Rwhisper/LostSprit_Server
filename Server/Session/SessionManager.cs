using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
/// <summary>
/// 전체 접속한 유저들 관리하는 클래스
/// </summary>
	class SessionManager 
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }
		DataBase db;

		

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
	

		public void Flush()
		{
			
			//// N ^ 2
			//foreach (ClientSession s in _sessions)
			//	s.Send(_pendingList);

			////Console.WriteLine($"Flushed {_pendingList.Count} items");
			//_pendingList.Clear();
		}
		/// <summary>
		/// 처음 서버에 접속했을때 고유 번호 부여
		/// </summary>
		/// <returns></returns>
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


		/// <summary>
		/// 세션풀에서 세션 삭제
		/// </summary>
		/// <param name="session"></param>
		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session.SessionId);
			}
		}

		/// <summary>
		/// 로그인 확인 함수 result = 1 : 성공 , result = 2 : 실패, result =3 : 이미 로그인한 사람이 있어서 로그인 할 수 없음
		/// </summary>
		/// <param name="session"></param>
		/// <param name="packet"></param>
		public void Login(ClientSession session, C_Login packet)
        {
            lock (_lock)
            {
				S_LoginResult pkt = new S_LoginResult();
				db = new DataBase();
				if(db.Loing(packet.id, packet.pwd) == 1)
                {
					if(_loginSession.TryGetValue(packet.id, out ClientSession s))
                    {
						pkt.result = -1;
                    }
                    else
					{
						session.PlayerId = packet.id;
						_loginSession.Add(packet.id, session);
						pkt.result = 1;
					}					
                }
                else
                {
					pkt.result = 0;
                }
				session.Send(pkt.Write());
			}
            
        }
		
		/// <summary>
		/// 로그아웃 로그아웃 
		/// </summary>
		/// <param name="session"></param>
		public void Logout(ClientSession session)
        {
            lock (_lock)
            {
				if (_loginSession.TryGetValue(session.PlayerId, out ClientSession s))
				{
					_loginSession.Remove(session.PlayerId);
				}
			}			
			
        }
		/// <summary>
		/// 룸 리스트 요청 패킷이 넘어 올때 실행
		/// </summary>
		public void RoomList(ClientSession session)
        {			
			lock (_lock)
            {
				int cnt = 0;
				List<S_RoomList.Room> _roomList = new List<S_RoomList.Room>();
				S_RoomList.Room gm = new S_RoomList.Room();				
				foreach (GameRoom room in _gameRoom.Values)
				{
					gm.host = room.Host;
					gm.nowPlayer = room.NowPlayer;
					gm.maxPlayer = room.MaxPlayer;
					gm.title = room.Title;
					gm.stage = room.Stage;
					gm.state = room.State;
                    if (!room.State)
                    {
						cnt++;
						_roomList.Add(gm);
					}
				}
				if(cnt != 0)
                {
					S_RoomList pkt = new S_RoomList();
					pkt.rooms = _roomList;
					session.Send(pkt.Write());
				}
                else
                {
					S_RoomConnFaild pkt = new S_RoomConnFaild();
					pkt.result = 1;
					session.Send(pkt.Write());
                }
               
			}			
        }

		public void RankingLIst(ClientSession session, C_RankList packet)
        {
            lock (_lock)
            {
				List<Ranking> dbRankList = new List<Ranking>();
				List<S_RankList.Rank> pktRankList = new List<S_RankList.Rank>();
				dbRankList = db.Selectranking(packet.stageCode);
				foreach (Ranking r in dbRankList)
				{
					S_RankList.Rank pktRank = new S_RankList.Rank();
					pktRank.clearTime = r.cleartime;
					pktRank.id = r.userid;
					pktRank.stage = r.stagecode;
					pktRankList.Add(pktRank);
				}
				S_RankList pkt = new S_RankList();
				pkt.ranks = pktRankList;
				session.Send(pkt.Write());
			}			
		}
		/// <summary>
		/// 게임 나가기 요청
		/// </summary>
		/// <param name="session"></param>
		public void LeaveGame(ClientSession session)
        {
            lock (_lock)
            {
				if (session.Room != null)
				{
					if (_gameRoom.TryGetValue(session.Room.Roomid, out GameRoom room))
					{
						if (session.PlayerId == room.Host)
						{
							room.Push(()=>room.LeaveHost(session));
							_gameRoom.Remove(session.Room.Roomid);
						}
						else
						{
							room.Push(()=>room.Leave(session));
						}
					}
				}
			}

			
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
				createRoom.MaxPlayer = packet.maxUser;
				// 만든 룸에 호스트 집어넣기
				createRoom.Push(() => createRoom.CreateRoom(session, packet.maxUser));
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
		public void EnterRoom(ClientSession session, C_RoomEnter packet)
        {
            lock (_lock)
            {
				if (_gameRoom.TryGetValue(packet.roomId, out GameRoom room))
				{
					if (room.MaxPlayer > room.NowPlayer)
					{
						room.Push(() => room.Enter(session));
						
					}
					else
					{
						S_RoomConnFaild pkt = new S_RoomConnFaild();
						pkt.result = 2;
						session.Send(pkt.Write());
					}
				}
				else
				{
					S_RoomConnFaild pkt = new S_RoomConnFaild();
					pkt.result = 3;
					session.Room.Roomid = packet.roomId;
					session.Send(pkt.Write()); 
				}
			}			
		}

		/// <summary>
		/// 방에서 나가기
		/// </summary>
		/// <param name="session"></param>
	

		public void GameClear(ClientSession session, C_GameClear packet)
        {
            lock (_lock)
            {
				if(session.Room == null || session.PlayerId != session.Room.Host)
                {
					return;
                }
				
				GameRoom room = session.Room;				
				string clearStage = session.Room.Stage;
				string diffTime = GetClearTime(room.StartGameTime);
				List<ClientSession> sessions = room.GetSessions();
				S_GameClear pkt = new S_GameClear();
				S_GameClear.Player player = new S_GameClear.Player();
				List<S_GameClear.Player> playerList = new List<S_GameClear.Player>();
				pkt.clearTime = diffTime;
				pkt.stage = clearStage;
				foreach (ClientSession cs in sessions)
                {
					db = new DataBase();
					if(db.Insertranking(clearStage, cs.PlayerId, diffTime))
                    {
						player.playerId = cs.PlayerId;
						playerList.Add(player);
					}
                }
				room.Push(() => room.Broadcast(pkt.Write()));
				
			}
			
        }	
		/// <summary>
		/// 게임 클리어시간 계산해주는 함수
		/// </summary>
		/// <param name="startGameTime"></param>
		/// <returns></returns>
		public string GetClearTime(string startGameTime)
        {
			string result = null;
			DateTime startTime = Convert.ToDateTime(startGameTime);
			DateTime endTime = DateTime.Now;
			TimeSpan dateDiff = endTime - startTime;
			int diffHour = dateDiff.Hours;
			int diffMinute = dateDiff.Minutes;
			int diffSecond = dateDiff.Seconds;
			result = diffHour + ":" + diffMinute + ":" + diffSecond;
			return result;
        }


	}
}
