using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public enum PlayerMode { Hero = 0, God }

public class CustomNetworkManager : NetworkManager
{
	public GameObject[] PlayerPrefabs;

	[SerializeField] private PlayerMode mPlayerMode = PlayerMode.Hero;

	public override void OnStartServer()
	{
		NetworkServer.RegisterHandler(MessageTypes.PlayerMode, OnServerRespondToPrefabType);
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		MessageTypes.PlayerModeMsg msg = new MessageTypes.PlayerModeMsg();
		msg.controllerID = playerControllerId;
		NetworkServer.SendToClient(conn.connectionId, MessageTypes.PlayerMode, msg);
	}

	// called when connected to a server
	public override void OnClientConnect(NetworkConnection conn)
	{
		client.RegisterHandler(MessageTypes.PlayerMode, OnClientRespondToServerPrefabTypeRequest);
		client.RegisterHandler(MessageTypes.UpdatePosNRot, OnClientEmpty);

		// Base functionality.
		ClientScene.Ready(conn);
		ClientScene.AddPlayer(0);
	}

	public override void OnClientSceneChanged(NetworkConnection conn)
	{
		// OVERRIDE TO MAKE SURE ClientScene.Ready(conn) is only called if scene is not ready.
		if (ClientScene.ready)
			return;
		
		// always become ready.
		ClientScene.Ready(conn);

		if (!autoCreatePlayer)
		{
			return;
		}

		bool addPlayer = (ClientScene.localPlayers.Count == 0);
		bool foundPlayer = false;
		for (int i = 0; i < ClientScene.localPlayers.Count; i++)
		{
			if (ClientScene.localPlayers[i].gameObject != null)
			{
				foundPlayer = true;
				break;
			}
		}
		if (!foundPlayer)
		{
			// there are players, but their game objects have all been deleted
			addPlayer = true;
		}
		if (addPlayer)
		{
			ClientScene.AddPlayer(0);
		}
	}



	#region Message Handlers
	private void OnClientRespondToServerPrefabTypeRequest(NetworkMessage netMsg)
	{
		MessageTypes.PlayerModeMsg msg = new MessageTypes.PlayerModeMsg();
		msg.controllerID = netMsg.ReadMessage<MessageTypes.PlayerModeMsg>().controllerID;
		msg.mode = mPlayerMode;
		client.Send(MessageTypes.PlayerMode, msg);
	}

	private void OnServerRespondToPrefabType(NetworkMessage netMsg)
	{
		MessageTypes.PlayerModeMsg msg = netMsg.ReadMessage<MessageTypes.PlayerModeMsg>();  
		playerPrefab = PlayerPrefabs[(int)msg.mode];
		base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
		Debug.Log(playerPrefab.name + " spawned!");
	}

	private void OnClientEmpty(NetworkMessage netMsg)
	{

	}
	#endregion
}
