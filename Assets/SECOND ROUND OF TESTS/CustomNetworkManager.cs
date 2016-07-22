using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public enum PlayerMode { Hero = 0, God }

public class CustomNetworkManager : NetworkManager
{
	public GameObject[] PlayerPrefabs;

	[SerializeField]
	private PlayerMode mPlayerMode = PlayerMode.Hero;

	public override void OnStartServer()
	{
		NetworkServer.RegisterHandler(MessageTypes.PlayerMode, OnServerRespondToPrefabType);
		Debug.Log("OnStartServer");
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		Debug.Log("OnServerAddPlayer Custom");

		MessageTypes.PlayerModeMsg msg = new MessageTypes.PlayerModeMsg();
		msg.controllerID = playerControllerId;
		NetworkServer.SendToClient(conn.connectionId, MessageTypes.PlayerMode, msg);

		Debug.Log("Server sent client message");
	}

	// called when connected to a server
	public override void OnClientConnect(NetworkConnection conn)
	{
		client.RegisterHandler(MessageTypes.PlayerMode, OnClientRespondToServerPrefabTypeRequest);

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
	// 
	private void OnClientRespondToServerPrefabTypeRequest(NetworkMessage netMsg)
	{
		Debug.Log("Player Recieved Msg");
		MessageTypes.PlayerModeMsg msg = new MessageTypes.PlayerModeMsg();
		msg.controllerID = netMsg.ReadMessage<MessageTypes.PlayerModeMsg>().controllerID;
		msg.mode = mPlayerMode;
		client.Send(MessageTypes.PlayerMode, msg);
	}

	private void OnServerRespondToPrefabType(NetworkMessage netMsg)
	{
		Debug.Log("Server Recieve Message");
		MessageTypes.PlayerModeMsg msg = netMsg.ReadMessage<MessageTypes.PlayerModeMsg>();  
		playerPrefab = spawnPrefabs[(int)msg.mode];
		base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
		Debug.Log(playerPrefab.name + " spawned!");
	}
	#endregion
}
