using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void C_LoginHandler(PacketSession session, IPacket packet)
	{

	}
	public static void C_LogoutHandler(PacketSession session, IPacket packet)
	{

	}

	public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
	{
		ClientSession clientSession = session as ClientSession;

		if (clientSession.Room == null)
			return;

		GameRoom room = clientSession.Room;
		room.Push(
			() => room.Leave(clientSession)
		); 
	}
	public static void C_MoveHandler(PacketSession session, IPacket packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		if (clientSession.Room == null)
			return;

		Console.WriteLine($"위치 : {movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");
		GameRoom room = clientSession.Room;
		room.Push(
			() => room.Move(clientSession, movePacket)
		);
	}
	public static void C_RotHandler(PacketSession session, IPacket packet)
    {
		C_Rot rotPacket = packet as C_Rot;
		ClientSession clientSession = session as ClientSession;

		if (clientSession.Room == null)
			return;
        //float x, y, z;
        //if (movePacket.posX == null)
        //	movePacket.posX == 
        Console.WriteLine($"각도 : {rotPacket.rotX}, {rotPacket.rotY}, {rotPacket.rotZ}, {rotPacket.rotW}");
        GameRoom room = clientSession.Room;
		room.Push(
			() => room.Rot(clientSession, rotPacket)
		);
	}
	public static void C_EnterHandler(PacketSession session, IPacket packet)
	{
		
		C_Enter enterPacket = packet as C_Enter;
		ClientSession clientSession = session as ClientSession;

				
		GameRoom room = clientSession.Room;
		room.Push(
			() => room.EnterRoom(clientSession, enterPacket)
		);
	}
	public static void C_DestroyItemHandler(PacketSession session, IPacket packet)
	{

		C_DestroyItem destroyItem = packet as C_DestroyItem;
		ClientSession clientSession = session as ClientSession;


		GameRoom room = clientSession.Room;
		room.Push(
			() => room.DestroyItem(clientSession, destroyItem)
		);
	}
	public static void C_GameOverHandler(PacketSession session, IPacket packet)
	{

		C_GameOver enterPacket = packet as C_GameOver;
		ClientSession clientSession = session as ClientSession;


	}
	public static void C_DropItemHandler(PacketSession session, IPacket packet)
	{

		C_DropItem enterPacket = packet as C_DropItem;
		ClientSession clientSession = session as ClientSession;



	}
	public static void C_RoomListHandler(PacketSession session, IPacket packet)
	{

	}
	public static void C_RoomRefreshHandler(PacketSession session, IPacket packet)
	{

	}
	public static void C_RoomEnterHandler(PacketSession session, IPacket packet)
	{

	}
	public static void C_LeaveRoomHandler(PacketSession session, IPacket packet)
	{

	}
	public static void C_RankListHandler(PacketSession session, IPacket packet)
	{

	}
	
}
