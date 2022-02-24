using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
	

	class Program
	{
		
		static void Main(string[] args)
		{
			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPAddress ip = IPAddress.Parse("0.0.0.0");
			IPEndPoint endPoint = new IPEndPoint(ip, 7777);

			Connector connector = new Connector();

			connector.Connect(endPoint, 
				() => { return SessionManager.Instance.Generate(); },
				1);

			
			while (true)
			{
				string n;
                
				n = Console.ReadLine( );
                switch (n)
                {
					case "Login" :
                        Console.WriteLine("로그인 하세요 id");
						string id = Console.ReadLine();
                        Console.WriteLine("로그인 하세요 pw");
						string pwd = Console.ReadLine();
						SessionManager.Instance.Login(id, pwd);
                        Console.WriteLine("로그인 요청중");
						break;
					case "roomList":
                        Console.WriteLine("룸 리스트 요청 보내기");
						SessionManager.Instance.RoomListRequest();						
						break;
					case "Logout":
						SessionManager.Instance.Logout();
						break;
					case "EnterRoom":
						string i = Console.ReadLine();
						SessionManager.Instance.EnterRoom(int.Parse(i));
						break;
					case "GameStart":
						SessionManager.Instance.GameStart();
						break;
					case "CreateRoom":
						string a = Console.ReadLine();
						string b = Console.ReadLine();
						SessionManager.Instance.CreateRoom(a, int.Parse(b));
						break;
					case "Move":
						SessionManager.Instance.Move();
						break;
					case "RoomInfo":
						SessionManager.Instance.RoomInfo();
						break;
                    case "RankList":
						string stageCode = Console.ReadLine();
						break;
					case "GameOver":

						break;
					case "GameReStart":

						break;
                    case "GameClear":

						break;

				}

				//try
				//{
				//	SessionManager.Instance.SendForEach();
				//}
				//catch (Exception e)
				//{
				//	Console.WriteLine(e.ToString());
				//}

				//Thread.Sleep(250);

			}
		}
	}
}
