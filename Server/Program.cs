using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
        // 방이 하나일때는 이렇게 사용해도됨
        //public static GameRoom Room = new GameRoom();

        //static void FlushRoom()
        //{
        //    Room.Push(() => Room.Flush());
        //    JobTimer.Instance.Push(FlushRoom, 25);
        //}

        static void Main(string[] args) 
		{
			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
            IPAddress ip = IPAddress.Parse("10.105.1.244");
			IPEndPoint endPoint = new IPEndPoint(ip, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

            //계속해서 뿌려줌FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);
            

            while (true)
			{
                //JobTimer.Instance.Flush();
            }
		}
	}
}
