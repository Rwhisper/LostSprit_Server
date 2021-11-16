using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
	S_EnterGame = 1,
	S_LoginResult = 2,
	S_BroadcastEnterGame = 3,
	S_BroadcastLeaveGame = 4,
	S_PlayerList = 5,
	S_BroadCastRot = 6,
	S_BroadCastMove = 7,
	S_BroadCastDestroyItem = 8,
	S_BroadCastGameOver = 9,
	S_BroadCastDropItem = 10,
	S_BroadCastReady = 11,
	S_BroadCastItemEvent = 12,
	S_RoomList = 13,
	S_RankList = 14,
	S_CreateRoomResult = 15,
	S_EnterRoomOk = 16,
	S_RoomConnFaild = 17,
	S_BroadCastEnterRoom = 18,
	S_NewRanking = 19,
	S_Ready = 20,
	S_ReadyCancel = 21,
	S_GameStart = 22,
	S_GameClear = 23,
	S_GameOver = 24,
	S_GameStartFaild = 25,
	C_Login = 26,
	C_Logout = 27,
	C_LeaveGame = 28,
	C_GameStart = 29,
	C_Move = 30,
	C_Rot = 31,
	C_Enter = 32,
	C_DestroyItem = 33,
	C_GameOver = 34,
	C_DropItem = 35,
	C_RoomList = 36,
	C_CreateRoom = 37,
	C_RoomRefresh = 38,
	C_RoomEnter = 39,
	C_RankList = 40,
	C_LeaveRoom = 41,
	C_Ready = 42,
	C_ReadyCancle = 43,
	C_GameClear = 44,
	C_GameRestart = 45,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class S_EnterGame : IPacket
{
	public int sessionId;

	public ushort Protocol { get { return (ushort)PacketID.S_EnterGame; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.sessionId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_EnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.sessionId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_LoginResult : IPacket
{
	public int result;
	public string id;

	public ushort Protocol { get { return (ushort)PacketID.S_LoginResult; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_LoginResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadcastEnterGame : IPacket
{
	public string playerId;
	public string attr;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEnterGame; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		ushort attrLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.attr = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, attrLen);
		count += attrLen;
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		ushort attrLen = (ushort)Encoding.Unicode.GetBytes(this.attr, 0, this.attr.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(attrLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += attrLen;
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadcastLeaveGame : IPacket
{
	public string playerId;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_PlayerList : IPacket
{
	public class Player
	{
		public string attr;
		public bool isSelf;
		public string playerId;
		public float posX;
		public float posY;
		public float posZ;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort attrLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.attr = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, attrLen);
			count += attrLen;
			this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort attrLen = (ushort)Encoding.Unicode.GetBytes(this.attr, 0, this.attr.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(attrLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += attrLen;
			Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			return success;
		}	
	}
	public List<Player> players = new List<Player>();

	public ushort Protocol { get { return (ushort)PacketID.S_PlayerList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerLen; i++)
		{
			Player player = new Player();
			player.Read(segment, ref count);
			players.Add(player);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Player player in this.players)
			player.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastRot : IPacket
{
	public string playerId;
	public float rotX;
	public float rotY;
	public float rotZ;
	public float rotW;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastRot; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		this.rotX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotW = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastRot), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		Array.Copy(BitConverter.GetBytes(this.rotX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotW), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastMove : IPacket
{
	public string playerId;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastMove; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastDestroyItem : IPacket
{
	public int itemId;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastDestroyItem; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastDestroyItem), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.itemId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastGameOver : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastGameOver; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastGameOver), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastDropItem : IPacket
{
	public int itemId;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastDropItem; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastDropItem), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.itemId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastReady : IPacket
{
	public string playerID;
	public int result;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastReady; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort playerIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIDLen);
		count += playerIDLen;
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastReady), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort playerIDLen = (ushort)Encoding.Unicode.GetBytes(this.playerID, 0, this.playerID.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIDLen;
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastItemEvent : IPacket
{
	public int itemId;
	public int itemEvent;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastItemEvent; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.itemEvent = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastItemEvent), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.itemId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.itemEvent), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_RoomList : IPacket
{
	public class Room
	{
		public int roomId;
		public string host;
		public bool state;
		public int maxPlayer;
		public int nowPlayer;
		public string title;
		public string stage;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort hostLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.host = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, hostLen);
			count += hostLen;
			this.state = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.maxPlayer = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.nowPlayer = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort titleLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.title = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, titleLen);
			count += titleLen;
			ushort stageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.stage = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, stageLen);
			count += stageLen;
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort hostLen = (ushort)Encoding.Unicode.GetBytes(this.host, 0, this.host.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(hostLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += hostLen;
			Array.Copy(BitConverter.GetBytes(this.state), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.maxPlayer), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.nowPlayer), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort titleLen = (ushort)Encoding.Unicode.GetBytes(this.title, 0, this.title.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(titleLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += titleLen;
			ushort stageLen = (ushort)Encoding.Unicode.GetBytes(this.stage, 0, this.stage.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(stageLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += stageLen;
			return success;
		}	
	}
	public List<Room> rooms = new List<Room>();

	public ushort Protocol { get { return (ushort)PacketID.S_RoomList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.rooms.Clear();
		ushort roomLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < roomLen; i++)
		{
			Room room = new Room();
			room.Read(segment, ref count);
			rooms.Add(room);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_RoomList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.rooms.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Room room in this.rooms)
			room.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_RankList : IPacket
{
	public class Rank
	{
		public string stage;
		public string id;
		public string clearTime;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort stageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.stage = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, stageLen);
			count += stageLen;
			ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
			count += idLen;
			ushort clearTimeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.clearTime = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, clearTimeLen);
			count += clearTimeLen;
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort stageLen = (ushort)Encoding.Unicode.GetBytes(this.stage, 0, this.stage.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(stageLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += stageLen;
			ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += idLen;
			ushort clearTimeLen = (ushort)Encoding.Unicode.GetBytes(this.clearTime, 0, this.clearTime.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(clearTimeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += clearTimeLen;
			return success;
		}	
	}
	public List<Rank> ranks = new List<Rank>();

	public ushort Protocol { get { return (ushort)PacketID.S_RankList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.ranks.Clear();
		ushort rankLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < rankLen; i++)
		{
			Rank rank = new Rank();
			rank.Read(segment, ref count);
			ranks.Add(rank);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_RankList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.ranks.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Rank rank in this.ranks)
			rank.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_CreateRoomResult : IPacket
{
	public int result;

	public ushort Protocol { get { return (ushort)PacketID.S_CreateRoomResult; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_CreateRoomResult), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_EnterRoomOk : IPacket
{
	public string title;
	public string stage;
	public int maxPlayer;
	public int nowPlayer;
	public class PlayerReady
	{
		public string playerId;
		public int readyStatus;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
			this.readyStatus = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			Array.Copy(BitConverter.GetBytes(this.readyStatus), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<PlayerReady> playerReadys = new List<PlayerReady>();

	public ushort Protocol { get { return (ushort)PacketID.S_EnterRoomOk; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort titleLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.title = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, titleLen);
		count += titleLen;
		ushort stageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.stage = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, stageLen);
		count += stageLen;
		this.maxPlayer = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.nowPlayer = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.playerReadys.Clear();
		ushort playerReadyLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerReadyLen; i++)
		{
			PlayerReady playerReady = new PlayerReady();
			playerReady.Read(segment, ref count);
			playerReadys.Add(playerReady);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_EnterRoomOk), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort titleLen = (ushort)Encoding.Unicode.GetBytes(this.title, 0, this.title.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(titleLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += titleLen;
		ushort stageLen = (ushort)Encoding.Unicode.GetBytes(this.stage, 0, this.stage.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(stageLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += stageLen;
		Array.Copy(BitConverter.GetBytes(this.maxPlayer), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.nowPlayer), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes((ushort)this.playerReadys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (PlayerReady playerReady in this.playerReadys)
			playerReady.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_RoomConnFaild : IPacket
{
	public int result;

	public ushort Protocol { get { return (ushort)PacketID.S_RoomConnFaild; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_RoomConnFaild), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_BroadCastEnterRoom : IPacket
{
	public string playerId;

	public ushort Protocol { get { return (ushort)PacketID.S_BroadCastEnterRoom; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
		count += playerIdLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadCastEnterRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += playerIdLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_NewRanking : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.S_NewRanking; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_NewRanking), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_Ready : IPacket
{
	public class PlayerReady
	{
		public string playerId;
		public int Readystatus;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
			this.Readystatus = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			Array.Copy(BitConverter.GetBytes(this.Readystatus), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<PlayerReady> playerReadys = new List<PlayerReady>();

	public ushort Protocol { get { return (ushort)PacketID.S_Ready; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerReadys.Clear();
		ushort playerReadyLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerReadyLen; i++)
		{
			PlayerReady playerReady = new PlayerReady();
			playerReady.Read(segment, ref count);
			playerReadys.Add(playerReady);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Ready), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.playerReadys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (PlayerReady playerReady in this.playerReadys)
			playerReady.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_ReadyCancel : IPacket
{
	public class PlayerReady
	{
		public string playerId;
		public int Readystatus;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
			this.Readystatus = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			Array.Copy(BitConverter.GetBytes(this.Readystatus), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			return success;
		}	
	}
	public List<PlayerReady> playerReadys = new List<PlayerReady>();

	public ushort Protocol { get { return (ushort)PacketID.S_ReadyCancel; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.playerReadys.Clear();
		ushort playerReadyLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerReadyLen; i++)
		{
			PlayerReady playerReady = new PlayerReady();
			playerReady.Read(segment, ref count);
			playerReadys.Add(playerReady);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_ReadyCancel), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)this.playerReadys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (PlayerReady playerReady in this.playerReadys)
			playerReady.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_GameStart : IPacket
{
	public string stageCode;
	public class PlayerReady
	{
		public string playerId;
		public string status;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
			ushort statusLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.status = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, statusLen);
			count += statusLen;
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			ushort statusLen = (ushort)Encoding.Unicode.GetBytes(this.status, 0, this.status.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(statusLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += statusLen;
			return success;
		}	
	}
	public List<PlayerReady> playerReadys = new List<PlayerReady>();

	public ushort Protocol { get { return (ushort)PacketID.S_GameStart; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort stageCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.stageCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, stageCodeLen);
		count += stageCodeLen;
		this.playerReadys.Clear();
		ushort playerReadyLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerReadyLen; i++)
		{
			PlayerReady playerReady = new PlayerReady();
			playerReady.Read(segment, ref count);
			playerReadys.Add(playerReady);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GameStart), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort stageCodeLen = (ushort)Encoding.Unicode.GetBytes(this.stageCode, 0, this.stageCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(stageCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += stageCodeLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.playerReadys.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (PlayerReady playerReady in this.playerReadys)
			playerReady.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_GameClear : IPacket
{
	public string clearTime;
	public string stage;
	public class Player
	{
		public string playerId;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			ushort playerIdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.playerId = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, playerIdLen);
			count += playerIdLen;
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			ushort playerIdLen = (ushort)Encoding.Unicode.GetBytes(this.playerId, 0, this.playerId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(playerIdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += playerIdLen;
			return success;
		}	
	}
	public List<Player> players = new List<Player>();

	public ushort Protocol { get { return (ushort)PacketID.S_GameClear; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort clearTimeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.clearTime = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, clearTimeLen);
		count += clearTimeLen;
		ushort stageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.stage = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, stageLen);
		count += stageLen;
		this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < playerLen; i++)
		{
			Player player = new Player();
			player.Read(segment, ref count);
			players.Add(player);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GameClear), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort clearTimeLen = (ushort)Encoding.Unicode.GetBytes(this.clearTime, 0, this.clearTime.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(clearTimeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += clearTimeLen;
		ushort stageLen = (ushort)Encoding.Unicode.GetBytes(this.stage, 0, this.stage.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(stageLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += stageLen;
		Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Player player in this.players)
			player.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_GameOver : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.S_GameOver; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GameOver), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_GameStartFaild : IPacket
{
	public int result;

	public ushort Protocol { get { return (ushort)PacketID.S_GameStartFaild; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_GameStartFaild), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Login : IPacket
{
	public string id;
	public string pwd;

	public ushort Protocol { get { return (ushort)PacketID.C_Login; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort idLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, idLen);
		count += idLen;
		ushort pwdLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.pwd = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, pwdLen);
		count += pwdLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Login), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort idLen = (ushort)Encoding.Unicode.GetBytes(this.id, 0, this.id.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(idLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += idLen;
		ushort pwdLen = (ushort)Encoding.Unicode.GetBytes(this.pwd, 0, this.pwd.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(pwdLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += pwdLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Logout : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_Logout; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Logout), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_LeaveGame : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_GameStart : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_GameStart; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_GameStart), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Move : IPacket
{
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.C_Move; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Rot : IPacket
{
	public float rotX;
	public float rotY;
	public float rotZ;
	public float rotW;

	public ushort Protocol { get { return (ushort)PacketID.C_Rot; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.rotX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.rotW = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Rot), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.rotX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.rotW), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Enter : IPacket
{
	public string attr;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.C_Enter; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort attrLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.attr = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, attrLen);
		count += attrLen;
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Enter), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort attrLen = (ushort)Encoding.Unicode.GetBytes(this.attr, 0, this.attr.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(attrLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += attrLen;
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_DestroyItem : IPacket
{
	public int itemId;

	public ushort Protocol { get { return (ushort)PacketID.C_DestroyItem; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_DestroyItem), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.itemId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_GameOver : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_GameOver; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_GameOver), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_DropItem : IPacket
{
	public int itemId;
	public float posX;
	public float posY;
	public float posZ;

	public ushort Protocol { get { return (ushort)PacketID.C_DropItem; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.itemId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_DropItem), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.itemId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_RoomList : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_RoomList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RoomList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_CreateRoom : IPacket
{
	public string title;
	public int maxUser;

	public ushort Protocol { get { return (ushort)PacketID.C_CreateRoom; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort titleLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.title = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, titleLen);
		count += titleLen;
		this.maxUser = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_CreateRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort titleLen = (ushort)Encoding.Unicode.GetBytes(this.title, 0, this.title.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(titleLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += titleLen;
		Array.Copy(BitConverter.GetBytes(this.maxUser), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_RoomRefresh : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_RoomRefresh; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RoomRefresh), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_RoomEnter : IPacket
{
	public int roomId;

	public ushort Protocol { get { return (ushort)PacketID.C_RoomEnter; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.roomId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RoomEnter), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_RankList : IPacket
{
	public string stageCode;

	public ushort Protocol { get { return (ushort)PacketID.C_RankList; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort stageCodeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.stageCode = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, stageCodeLen);
		count += stageCodeLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_RankList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort stageCodeLen = (ushort)Encoding.Unicode.GetBytes(this.stageCode, 0, this.stageCode.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(stageCodeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += stageCodeLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_LeaveRoom : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_LeaveRoom; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveRoom), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_Ready : IPacket
{
	public int result;

	public ushort Protocol { get { return (ushort)PacketID.C_Ready; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Ready), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_ReadyCancle : IPacket
{
	public int result;

	public ushort Protocol { get { return (ushort)PacketID.C_ReadyCancle; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.result = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_ReadyCancle), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.result), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_GameClear : IPacket
{
	public string clearTime;

	public ushort Protocol { get { return (ushort)PacketID.C_GameClear; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort clearTimeLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.clearTime = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, clearTimeLen);
		count += clearTimeLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_GameClear), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort clearTimeLen = (ushort)Encoding.Unicode.GetBytes(this.clearTime, 0, this.clearTime.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(clearTimeLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += clearTimeLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_GameRestart : IPacket
{
	

	public ushort Protocol { get { return (ushort)PacketID.C_GameRestart; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_GameRestart), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

