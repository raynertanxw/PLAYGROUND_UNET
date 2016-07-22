using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiPlayerPrefabNetworkManager : NetworkManager
{
	private static MultiPlayerPrefabNetworkManager sIntance = null;
	public static MultiPlayerPrefabNetworkManager Instance { get { return sIntance; } }

	void Awake()
	{
		if (sIntance == null)
		{
			sIntance = this;
			Setup();
		}
		else if (sIntance != this)
		{
			Destroy(this.gameObject);
		}
	}

	void OnDestroy()
	{
		if (sIntance == this)
			sIntance = null;
	}




	public short playerPrefabIndex;

	private void Setup()
	{
		playerPrefabIndex = 0;
	}

	public override void OnStartServer()
	{
		NetworkServer.RegisterHandler(MessageTypes.PlayerPrefab, OnResponsePrefab);
		base.OnStartServer();
	}

	private void OnRequestPrefab(NetworkMessage netMsg)
	{
		MessageTypes.PlayerPrefabMsg msg = new MessageTypes.PlayerPrefabMsg();
		msg.controllerID = netMsg.ReadMessage<MessageTypes.PlayerPrefabMsg>().controllerID;
		msg.prefabIndex = playerPrefabIndex;
		client.Send(MessageTypes.PlayerPrefab, msg);
	}

	private void OnResponsePrefab(NetworkMessage netMsg)
	{
		MessageTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MessageTypes.PlayerPrefabMsg>();  
		playerPrefab = spawnPrefabs[msg.prefabIndex];
		base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
		Debug.Log(playerPrefab.name + " spawned!");
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		MessageTypes.PlayerPrefabMsg msg = new MessageTypes.PlayerPrefabMsg();
		msg.controllerID = playerControllerId;
		NetworkServer.SendToClient(conn.connectionId, MessageTypes.PlayerPrefab, msg);
	}




	// called when connected to a server
	public override void OnClientConnect(NetworkConnection conn)
	{
		// TO MAKE THE MULTIPLE PREFAB SPAWN WORK
		client.RegisterHandler(MessageTypes.PlayerPrefab, OnRequestPrefab);
		// TO MAKE THE MULTIPLE PREFAB SPAWN WORK

		ClientScene.Ready(conn);
		ClientScene.AddPlayer(0);

		GameObject.Find("Lobby Panel").GetComponent<LobbyMenu>().SetLobbyMenuVisible(false);
	}

	// called when disconnected from a server
	public override void OnClientDisconnect(NetworkConnection conn)
	{
		StopClient();

		GameObject.Find("Lobby Panel").GetComponent<LobbyMenu>().SetLobbyMenuVisible(true);

	}

	public override void OnStopHost()
	{
		base.OnStopHost();

		GameObject.Find("Lobby Panel").GetComponent<LobbyMenu>().SetLobbyMenuVisible(true);
	}
}