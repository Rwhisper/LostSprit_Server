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
		public DataBase db = null;

		

		int _sessionId = 0;
		
		/// <summary>
		/// 서버에 접속한 유저 정보
		/// </summary>
		Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
		int _roomId = 0;
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
				// 로그인 시도 결과 반환 패킷
				S_LoginResult pkt = new S_LoginResult();
				db = new DataBase();
				// 아이디 비번 있음
				if(db.Loing(packet.id, packet.pwd) == 1)
                {
					if (_loginSession.ContainsKey(packet.id))
					{
						// 로그인 실패
						pkt.result = -1;
						// 널값을 참조하지 않도록 객체 초기화
						pkt.id = "";
					}
					else            // 먼저 같은 아이디로 로그인 한 객체가 없음 
					{
						// 세션의 아이디 지정
						session.PlayerId = packet.id;
						// 로그인 세션에 넣어준다.
						_loginSession.Add(packet.id, session);
						// 패킷에 접속한 아이디 지정
						pkt.id = packet.id;
						// 로그인 성공
						pkt.result = 1;
					}					
                }
                else
                {
					// 비밀번호가 일치 하지 않거나 아이디가 다릅니다.
					pkt.result = 0;
					pkt.id = "";
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

				foreach (GameRoom room in _gameRoom.Values)
				{
					S_RoomList.Room gm = new S_RoomList.Room();
					gm.host = room.Host;
					gm.nowPlayer = room.NowPlayer;
					gm.maxPlayer = room.MaxPlayer;
					gm.title = room.Title;
					gm.stage = room.Stage;
					gm.state = room.State;

					if (room.State)
					{
						_roomList.Add(gm);
						Console.WriteLine("방 있음" + gm.host + gm.maxPlayer + gm.nowPlayer + gm.stage + gm.state + gm.title);
						Console.WriteLine(_roomList.Count);
						cnt++;
					}
					else
					{
						Console.WriteLine("닫힌 방" + gm.host + gm.maxPlayer + gm.nowPlayer + gm.stage + gm.state + gm.title);
					}
				}
				if (cnt != 0)
				{
					Console.WriteLine("방 전송");

					S_RoomList pkt = new S_RoomList();
					pkt.rooms = _roomList;
					session.Send(pkt.Write());
				}
				else    // 들어갈수있는 방이 없을때 출력
				{
					S_RoomConnFaild pkt = new S_RoomConnFaild();
					pkt.result = 1;
					session.Send(pkt.Write());
				}

			}
		}

		public void LeaveGame(ClientSession session)
        {
            lock (_lock)
            {
				if(session.RoomId != 0)
                {
					if(_gameRoom.TryGetValue(session.RoomId, out GameRoom gr))
                    {
						if (session.RoomHost == session.PlayerId)
						{
							gr.Push(() => gr.LeaveHost(session));
							
						}
                        else
                        {
							gr.Push(() => gr.Leave(session));
                        }
					}
					_gameRoom.Remove(session.RoomId);
				}
				if (session.PlayerId != null)
				{
					_loginSession.Remove(session.PlayerId);
				}				
				_sessions.Remove(session.SessionId);
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
				int roomId = _roomId++;
				// 룸 아이디 저장
				session.RoomId = roomId;
				GameRoom createRoom = new GameRoom();
				// 새로 만든 룸 객체 초기화 하고 호스트 넣어줌
				createRoom.Push(() => createRoom.CreateRoom(session, packet.maxUser, packet.title));
				// 룸 리스트에 룸 추가
				_gameRoom.Add(roomId, createRoom);
				if(_gameRoom.TryGetValue(roomId, out GameRoom r)) 
                {
                    Console.WriteLine("CreateRoom : " + r.Host + r.Stage + r.NowPlayer + r.MaxPlayer + r.Title);
					//S_CreateRoomResult pkt = new S_CreateRoomResult();
					//pkt.title = r.Title;
					//pkt.stage = r.Stage;
					//pkt.maxPlayer = r.MaxPlayer;
					//pkt.nowPlayer = r.NowPlayer;
					//session.Send(pkt.Write());
				}
				
				//Console.WriteLine($"CreateRoom : {roomId}, Host : {session.PlayerId} , Title{}");
			}
		}

		// 
		public void RoomInfo(ClientSession session)
        {
            lock (_lock)
            {
				if(_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
                {
					room.Push(() => room.ShowRoomInfo(session));
                }
			}
        }

		public void StartEnterRoom(ClientSession session, C_Enter packet)
        {
            lock (_lock)
            {
				if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
                {
					room.Push(() => room.EnterRoom(session, packet));
                }
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
				if (_gameRoom.TryGetValue(packet.roomId, out GameRoom room)) // roomId 에 맞는 방이 존재할떄
				{
					if (room.MaxPlayer > room.NowPlayer) // 룸 안에 현재 인원이 최대인원보다 적다면
					{
						// 룸에 넣어준다.
						room.Push(() => room.Enter(session));

					}
					else // 룸 안에 현재인원이 최대인원보다 같거나 많으면 
					{
						// 룸 접속 실패 패킷 보내준다.
						S_RoomConnFaild pkt = new S_RoomConnFaild();
						pkt.result = 2;
						session.Send(pkt.Write());
					}
				}
				else // 방이 존재 하지 않습니다.
				{
					S_RoomConnFaild pkt = new S_RoomConnFaild();
					pkt.result = 3;
					//session.RoomId = packet.roomId;
					session.Send(pkt.Write());
				}
			}
		}
		/// <summary>
		/// 방 나가기
		/// </summary>
		/// <param name="session"></param>
		public void LeaveRoom(ClientSession session)
        {
			if(_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
            {
				if (session.PlayerId == room.Host)
				{
					room.Push(() => room.LeaveHost(session));
					_gameRoom.Remove(session.Room.RoomId);
				}
				else
				{
					room.Push(() => room.Leave(session));
				}
			}
        }
		/// <summary>
		/// 게임 시작
		/// </summary>
		/// <param name="session"></param>
		public void GameStart(ClientSession session, C_GameStart pkt)
		{
			if(_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
            {
				room.Push(() => room.GameStart(session, pkt));
            }
        }
        /// <summary>
        /// 게임 오버 요청
        /// </summary>
        /// <param name="session"></param>
        public void GameOver(ClientSession session)
        {
            if (!_gameRoom.ContainsKey(session.RoomId))
            {
				return;
            }
            if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
            {
                room.Push(() => room.GameOver(session));
            }
        }


        /// <summary>
        /// 게임 클리어
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void GameClear(ClientSession session, C_GameClear packet)
		{
			lock (_lock)
			{
				if (session.Room == null || session.PlayerId != session.Room.Host)
				{
					return;
				}
				if(_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
                {
					if (room.Host != session.PlayerId)
						return;
					string clearStage = room.Stage;
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
						if (db.Insertranking(clearStage, cs.PlayerId, diffTime))
						{
							player.playerId = cs.PlayerId;
							playerList.Add(player);
						}
					}
					room.Push(() => room.Broadcast(pkt.Write()));
				}
			}
		}
		
		// 게임 나가기 요청
		//public void LeaveGame(ClientSession session)
		//{
		//	lock (_lock)
		//	{
		//		if (session.RoomId != 0)
		//		{
		//			if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
		//			{
		//				if (session.PlayerId == room.Host)
		//				{
		//					room.Push(() => room.LeaveHost(session));
		//					_gameRoom.Remove(session.RoomId);
		//				}
		//				else
		//				{
		//					room.Push(() => room.Leave(session));
		//				}
		//			}
		//		}
		//	}
		//}
		
		/// <summary>
		/// 랭킹 리스트 요청
		/// </summary>
		/// <param name="session"></param>
		/// <param name="packet"></param>
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
		/// <summary>
		/// 게임 룸 내에서 캐릭터 움직임
		/// </summary>
		/// <param name="session"></param>
		/// <param name="packet"></param>
		public void Move(ClientSession session, C_Move packet)
        {
			if(session.RoomId != 0)
            {
                if (_gameRoom.ContainsKey(session.RoomId))
                {
					if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
					{
						room.Push(() => room.Move(session, packet));
					}
				}
            } 
			
            
        }
		public void Rot(ClientSession session, C_Rot packet)
		{
			if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
			{
				room.Push(() => room.Rot(session, packet));
			}
		}
		public void DropItem(ClientSession session, C_DropItem packet)
		{
			if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
			{
				room.Push(() => room.DropItem(session, packet));
			}
		}
		public void DestroyItem(ClientSession session, C_DestroyItem packet)
		{
			if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
			{
				room.Push(() => room.DestroyItem(session, packet));
			}
		}
		public void Ready(ClientSession session, C_Ready packet)
		{
			if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
			{
				room.Push(() => room.Ready(session, packet));
			}
		}
		public void ReadyCancel(ClientSession session, C_ReadyCancle packet)
		{
			if (_gameRoom.TryGetValue(session.RoomId, out GameRoom room))
			{
				room.Push(() => room.ReadyCancel(session, packet));
			}
		}

	}
}
