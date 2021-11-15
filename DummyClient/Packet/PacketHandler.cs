using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
		ServerSession serverSession = session as ServerSession;

	}
	// 내가 입장한 상태에서 다른사람이 들어왔을때 추가
	public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame; 
		ServerSession serverSession = session as ServerSession;

	}
	// 누군가가 나갔을때
	public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;
		ServerSession serverSession = session as ServerSession;

	}
	// 주변의 플레이어들 리스트를 불러온다
	public static void S_PlayerListHandler(PacketSession session, IPacket packet)
	{
		S_PlayerList pkt = packet as S_PlayerList;
		ServerSession serverSession = session as ServerSession;

		
	}
	// 누군가가 이동하였을때
	public static void S_BroadCastMoveHandler(PacketSession session, IPacket packet)
	{
		S_BroadCastMove pkt = packet as S_BroadCastMove;
		ServerSession serverSession = session as ServerSession;
	}

	public static void S_BoradCastDestroyItemHandler(PacketSession session, IPacket packet)
	{
        ServerSession serverSession = session as ServerSession;

        //PlayerManager.Instance.DestroyItem(pkt);

    }
	public static void S_BroadCastGameOverHandler(PacketSession session, IPacket packet)
	{
		S_BroadCastGameOver pkt = packet as S_BroadCastGameOver;
		ServerSession serverSession = session as ServerSession;

		
	}
	public static void S_LoginResultHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
		S_LoginResult resultPacket = packet as S_LoginResult;
        Console.WriteLine($"{resultPacket.id} 접속 :  {resultPacket.result}");

	}
	public static void S_BroadCastRotHandler(PacketSession session, IPacket packet)
	{
		
	}
	public static void S_BroadCastDestroyItemHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_BroadCastReadyHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_BroadCastDropItemHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_BroadCastItemEventHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_RoomListHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
		S_RoomList pkt = packet as S_RoomList;
		foreach(S_RoomList.Room room in pkt.rooms)
        {
            Console.WriteLine(room.host + room.maxPlayer + room.nowPlayer + room. stage + room.state);
        }
	}
	public static void S_RankListHandler(PacketSession session, IPacket packet){
		ServerSession serverSession = session as ServerSession;
		
	}
	public static void S_CreateRoomResultHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
		S_CreateRoomResult pkt = packet as S_CreateRoomResult;
        Console.WriteLine($"room Create : {pkt.result}");
	}
	public static void S_EnterRoomOkHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_RoomConnFaildHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
		S_RoomConnFaild pkt = packet as S_RoomConnFaild;
        Console.WriteLine(pkt.result);
	}
	public static void S_BroadCastEnterRoomHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_NewRankingHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_ReadyHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_GameStartHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_GameClearHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_GameOverHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	public static void S_GameStartFaildHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
	}
	


}

