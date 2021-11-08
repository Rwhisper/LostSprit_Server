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
		_makeFunc.Add((ushort)PacketID.S_LoginResult, MakePacket<S_LoginResult>);
		_handler.Add((ushort)PacketID.S_LoginResult, PacketHandler.S_LoginResultHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
		_handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
		_handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
		_makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
		_handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastRot, MakePacket<S_BroadCastRot>);
		_handler.Add((ushort)PacketID.S_BroadCastRot, PacketHandler.S_BroadCastRotHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastMove, MakePacket<S_BroadCastMove>);
		_handler.Add((ushort)PacketID.S_BroadCastMove, PacketHandler.S_BroadCastMoveHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastDestroyItem, MakePacket<S_BroadCastDestroyItem>);
		_handler.Add((ushort)PacketID.S_BroadCastDestroyItem, PacketHandler.S_BroadCastDestroyItemHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastGameOver, MakePacket<S_BroadCastGameOver>);
		_handler.Add((ushort)PacketID.S_BroadCastGameOver, PacketHandler.S_BroadCastGameOverHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastDropItem, MakePacket<S_BroadCastDropItem>);
		_handler.Add((ushort)PacketID.S_BroadCastDropItem, PacketHandler.S_BroadCastDropItemHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastItemEvent, MakePacket<S_BroadCastItemEvent>);
		_handler.Add((ushort)PacketID.S_BroadCastItemEvent, PacketHandler.S_BroadCastItemEventHandler);
		_makeFunc.Add((ushort)PacketID.S_RoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)PacketID.S_RoomList, PacketHandler.S_RoomListHandler);
		_makeFunc.Add((ushort)PacketID.S_RankList, MakePacket<S_RankList>);
		_handler.Add((ushort)PacketID.S_RankList, PacketHandler.S_RankListHandler);
		_makeFunc.Add((ushort)PacketID.S_RoomConnFaild, MakePacket<S_RoomConnFaild>);
		_handler.Add((ushort)PacketID.S_RoomConnFaild, PacketHandler.S_RoomConnFaildHandler);
		_makeFunc.Add((ushort)PacketID.S_GameStart, MakePacket<S_GameStart>);
		_handler.Add((ushort)PacketID.S_GameStart, PacketHandler.S_GameStartHandler);
		_makeFunc.Add((ushort)PacketID.S_GameClear, MakePacket<S_GameClear>);
		_handler.Add((ushort)PacketID.S_GameClear, PacketHandler.S_GameClearHandler);

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