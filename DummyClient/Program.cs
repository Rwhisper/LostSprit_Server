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
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

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
						string id = Console.ReadLine();
						string pwd = Console.ReadLine();
						SessionManager.Instance.Login(id, pwd);
						break;
					case "roomList":
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
