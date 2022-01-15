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
		_makeFunc.Add((ushort)PacketID.S_EnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)PacketID.S_EnterGame, PacketHandler.S_EnterGameHandler);
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
		_makeFunc.Add((ushort)PacketID.S_BroadCastReady, MakePacket<S_BroadCastReady>);
		_handler.Add((ushort)PacketID.S_BroadCastReady, PacketHandler.S_BroadCastReadyHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastItemEvent, MakePacket<S_BroadCastItemEvent>);
		_handler.Add((ushort)PacketID.S_BroadCastItemEvent, PacketHandler.S_BroadCastItemEventHandler);
		_makeFunc.Add((ushort)PacketID.S_RoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)PacketID.S_RoomList, PacketHandler.S_RoomListHandler);
		_makeFunc.Add((ushort)PacketID.S_RankList, MakePacket<S_RankList>);
		_handler.Add((ushort)PacketID.S_RankList, PacketHandler.S_RankListHandler);
		_makeFunc.Add((ushort)PacketID.S_CreateRoomResult, MakePacket<S_CreateRoomResult>);
		_handler.Add((ushort)PacketID.S_CreateRoomResult, PacketHandler.S_CreateRoomResultHandler);
		_makeFunc.Add((ushort)PacketID.S_EnterRoomOk, MakePacket<S_EnterRoomOk>);
		_handler.Add((ushort)PacketID.S_EnterRoomOk, PacketHandler.S_EnterRoomOkHandler);
		_makeFunc.Add((ushort)PacketID.S_RoomInfo, MakePacket<S_RoomInfo>);
		_handler.Add((ushort)PacketID.S_RoomInfo, PacketHandler.S_RoomInfoHandler);
		_makeFunc.Add((ushort)PacketID.S_RoomConnFaild, MakePacket<S_RoomConnFaild>);
		_handler.Add((ushort)PacketID.S_RoomConnFaild, PacketHandler.S_RoomConnFaildHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastEnterRoom, MakePacket<S_BroadCastEnterRoom>);
		_handler.Add((ushort)PacketID.S_BroadCastEnterRoom, PacketHandler.S_BroadCastEnterRoomHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadCastStageChange, MakePacket<S_BroadCastStageChange>);
		_handler.Add((ushort)PacketID.S_BroadCastStageChange, PacketHandler.S_BroadCastStageChangeHandler);
		_makeFunc.Add((ushort)PacketID.S_NewRanking, MakePacket<S_NewRanking>);
		_handler.Add((ushort)PacketID.S_NewRanking, PacketHandler.S_NewRankingHandler);
		_makeFunc.Add((ushort)PacketID.S_Ready, MakePacket<S_Ready>);
		_handler.Add((ushort)PacketID.S_Ready, PacketHandler.S_ReadyHandler);
		_makeFunc.Add((ushort)PacketID.S_ReadyCancel, MakePacket<S_ReadyCancel>);
		_handler.Add((ushort)PacketID.S_ReadyCancel, PacketHandler.S_ReadyCancelHandler);
		_makeFunc.Add((ushort)PacketID.S_GameStart, MakePacket<S_GameStart>);
		_handler.Add((ushort)PacketID.S_GameStart, PacketHandler.S_GameStartHandler);
		_makeFunc.Add((ushort)PacketID.S_GameClear, MakePacket<S_GameClear>);
		_handler.Add((ushort)PacketID.S_GameClear, PacketHandler.S_GameClearHandler);
		_makeFunc.Add((ushort)PacketID.S_GameOver, MakePacket<S_GameOver>);
		_handler.Add((ushort)PacketID.S_GameOver, PacketHandler.S_GameOverHandler);
		_makeFunc.Add((ushort)PacketID.S_GameStartFaild, MakePacket<S_GameStartFaild>);
		_handler.Add((ushort)PacketID.S_GameStartFaild, PacketHandler.S_GameStartFaildHandler);
		_makeFunc.Add((ushort)PacketID.S_Chat, MakePacket<S_Chat>);
		_handler.Add((ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler);

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