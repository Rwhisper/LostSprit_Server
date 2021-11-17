using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
	Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
		
	public void Register()
	{
		_makeFunc.Add((ushort)PacketID.C_Login, MakePacket<C_Login>);
		_handler.Add((ushort)PacketID.C_Login, PacketHandler.C_LoginHandler);
		_makeFunc.Add((ushort)PacketID.C_Logout, MakePacket<C_Logout>);
		_handler.Add((ushort)PacketID.C_Logout, PacketHandler.C_LogoutHandler);
		_makeFunc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
		_handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);
		_makeFunc.Add((ushort)PacketID.C_GameStart, MakePacket<C_GameStart>);
		_handler.Add((ushort)PacketID.C_GameStart, PacketHandler.C_GameStartHandler);
		_makeFunc.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);
		_handler.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);
		_makeFunc.Add((ushort)PacketID.C_Rot, MakePacket<C_Rot>);
		_handler.Add((ushort)PacketID.C_Rot, PacketHandler.C_RotHandler);
		_makeFunc.Add((ushort)PacketID.C_Enter, MakePacket<C_Enter>);
		_handler.Add((ushort)PacketID.C_Enter, PacketHandler.C_EnterHandler);
		_makeFunc.Add((ushort)PacketID.C_DestroyItem, MakePacket<C_DestroyItem>);
		_handler.Add((ushort)PacketID.C_DestroyItem, PacketHandler.C_DestroyItemHandler);
		_makeFunc.Add((ushort)PacketID.C_GameOver, MakePacket<C_GameOver>);
		_handler.Add((ushort)PacketID.C_GameOver, PacketHandler.C_GameOverHandler);
		_makeFunc.Add((ushort)PacketID.C_DropItem, MakePacket<C_DropItem>);
		_handler.Add((ushort)PacketID.C_DropItem, PacketHandler.C_DropItemHandler);
		_makeFunc.Add((ushort)PacketID.C_RoomList, MakePacket<C_RoomList>);
		_handler.Add((ushort)PacketID.C_RoomList, PacketHandler.C_RoomListHandler);
		_makeFunc.Add((ushort)PacketID.C_CreateRoom, MakePacket<C_CreateRoom>);
		_handler.Add((ushort)PacketID.C_CreateRoom, PacketHandler.C_CreateRoomHandler);
		_makeFunc.Add((ushort)PacketID.C_RoomRefresh, MakePacket<C_RoomRefresh>);
		_handler.Add((ushort)PacketID.C_RoomRefresh, PacketHandler.C_RoomRefreshHandler);
		_makeFunc.Add((ushort)PacketID.C_RoomEnter, MakePacket<C_RoomEnter>);
		_handler.Add((ushort)PacketID.C_RoomEnter, PacketHandler.C_RoomEnterHandler);
		_makeFunc.Add((ushort)PacketID.C_RankList, MakePacket<C_RankList>);
		_handler.Add((ushort)PacketID.C_RankList, PacketHandler.C_RankListHandler);
		_makeFunc.Add((ushort)PacketID.C_LeaveRoom, MakePacket<C_LeaveRoom>);
		_handler.Add((ushort)PacketID.C_LeaveRoom, PacketHandler.C_LeaveRoomHandler);
		_makeFunc.Add((ushort)PacketID.C_Ready, MakePacket<C_Ready>);
		_handler.Add((ushort)PacketID.C_Ready, PacketHandler.C_ReadyHandler);
		_makeFunc.Add((ushort)PacketID.C_ReadyCancle, MakePacket<C_ReadyCancle>);
		_handler.Add((ushort)PacketID.C_ReadyCancle, PacketHandler.C_ReadyCancleHandler);
		_makeFunc.Add((ushort)PacketID.C_GameClear, MakePacket<C_GameClear>);
		_handler.Add((ushort)PacketID.C_GameClear, PacketHandler.C_GameClearHandler);
		_makeFunc.Add((ushort)PacketID.C_GameRestart, MakePacket<C_GameRestart>);
		_handler.Add((ushort)PacketID.C_GameRestart, PacketHandler.C_GameRestartHandler);
		_makeFunc.Add((ushort)PacketID.C_RoomInfo, MakePacket<C_RoomInfo>);
		_handler.Add((ushort)PacketID.C_RoomInfo, PacketHandler.C_RoomInfoHandler);

	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
		if (_makeFunc.TryGetValue(id, out func))
        {
			IPacket packet = func.Invoke(session, buffer);
			if (onRecvCallback != null)
				onRecvCallback.Invoke(session, packet);
			else
			HandlePacket(session, packet);
		}
	}

	T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
	{
		T pkt = new T();
		pkt.Read(buffer);
		return pkt;
	}
	public void HandlePacket(PacketSession session, IPacket packet)
    {
		Action<PacketSession, IPacket> action = null;
		if (_handler.TryGetValue(packet.Protocol, out action))
			action.Invoke(session, packet);
	}

}