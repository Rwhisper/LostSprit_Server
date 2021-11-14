using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;

namespace Server
{
	class ClientSession : PacketSession
	{
		public int SessionId { get; set; }
		public string PlayerId { get; set; }
		public string Attr { get; set; }
		public int ReadyStatus { get; set; }
		public GameRoom Room { get; set; }
		// 위치
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }
		// 각도
		public float RotX { get; set; }
		public float RotY { get; set; }
		public float RotZ { get; set; }
		public float RotW { get; set; }

		public string RoomHost { get; set; }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// 룸 안에 밀어 넣는다.
            //Program.Room.Push(() => Program.Room.Enter(this));
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);
			if (Room != null)
			{ 
				GameRoom room = Room;
				room.Push(() => room.Leave(this));
				Room = null;
			}

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
