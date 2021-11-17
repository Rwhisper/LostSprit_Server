using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	// 캐릭터 선택할때 보냄
	public static void S_EnterGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
		ServerSession serverSession = session as ServerSession;
        Console.WriteLine("EnterGame" + pkt.attr, pkt.playerId, pkt.posX, pkt.posY, pkt.posZ);

	}
	// 내가 입장한 상태에서 다른사람이 들어왔을때 추가
	public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
	{
		S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame; 
		ServerSession serverSession = session as ServerSession;
        Console.WriteLine(	"BroadEnter : "+pkt.playerId, pkt.attr);

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

		
	}
	// 누군가가 이동하였을때
	public static void S_BroadCastMoveHandler(PacketSession session, IPacket packet)
	{
		S_BroadCastMove pkt = packet as S_BroadCastMove;
		ServerSession serverSession = session as ServerSession;

		Console.WriteLine(pkt.playerId + pkt.posX + pkt.posY+ pkt.posZ);
	}

	public static void S_ReadyCancelHandler(PacketSession session, IPacket packet)
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
	public static void S_RoomInfoHandler(PacketSession session, IPacket packet)
	{
		S_RoomInfo pkt = packet as S_RoomInfo;
        Console.WriteLine(pkt.maxPlayer.ToString() + " " + pkt.nowPlayer.ToString() + pkt.stage + pkt.title);

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
            Console.WriteLine("방이름 : " + room.title + "방장" +room.host + "최대 인원수 : " + room.maxPlayer + "최소 인원수 : " + room.nowPlayer +"스테이지 : " + room. stage + "현재 게임중인지 아닌지 : " + room.state);
        }
	}
	public static void S_RankListHandler(PacketSession session, IPacket packet){
		ServerSession serverSession = session as ServerSession;
		
	}
	public static void S_CreateRoomResultHandler(PacketSession session, IPacket packet)
	{
		ServerSession serverSession = session as ServerSession;
		S_CreateRoomResult pkt = packet as S_CreateRoomResult;
        Console.WriteLine($"room Create : {pkt.title}");
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

