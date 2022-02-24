using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
	public class Listener
	{
		Socket _listenSocket;
		Func<Session> _sessionFactory;

		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sessionFactory += sessionFactory;

			// 이 아이피로 받겟다
			_listenSocket.Bind(endPoint);

			// 수신 시작
			// backlog : 최대 대기수
			_listenSocket.Listen(backlog);

			// 이벤트 핸들러 생성
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			// 이벤트 연결
			args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
			// 비동기 연결 시작
			RegisterAccept(args);
			
		}
		/// <summary>
		/// 비동기 accept 함수 accept이 성공함녀 OnAcceptCompleted 함수를 호출한다.
		/// </summary>
		/// <param name="args"></param>
		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			bool pending = _listenSocket.AcceptAsync(args);
			if (pending == false)
				OnAcceptCompleted(null, args);
		}

		// 비동기 accept이 성고하면 호출하는 함수
		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.AcceptSocket);
				session.OnConnected(args.AcceptSocket.RemoteEndPoint);
			}
			else
				Console.WriteLine(args.SocketError.ToString());

			RegisterAccept(args);
		}
	}
}
