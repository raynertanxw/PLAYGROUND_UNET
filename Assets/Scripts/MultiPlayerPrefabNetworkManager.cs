using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MsgTypes
{
	public const short PlayerPrefab = MsgType.Highest + 1;

	public class PlayerPrefabMsg : MessageBase
	{
		public short controllerID;    
		public short prefabIndex;
	}
}

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
		NetworkServer.RegisterHandler(MsgTypes.PlayerPrefab, OnResponsePrefab);
		base.OnStartServer();
	}

	private void SpawnFakeAStar()
	{
		GameObject fakeAStar = (GameObject) Instantiate(spawnPrefabs[2]);

		NetworkServer.Spawn(fakeAStar);
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		client.RegisterHandler(MsgTypes.PlayerPrefab, OnRequestPrefab);
		base.OnClientConnect(conn);
	}

	private void OnRequestPrefab(NetworkMessage netMsg)
	{
		MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
		msg.controllerID = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>().controllerID;
		msg.prefabIndex = playerPrefabIndex;
		client.Send(MsgTypes.PlayerPrefab, msg);
	}

	private void OnResponsePrefab(NetworkMessage netMsg)
	{
		MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();  
		playerPrefab = spawnPrefabs[msg.prefabIndex];
		base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
		Debug.Log(playerPrefab.name + " spawned!");
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
		msg.controllerID = playerControllerId;
		NetworkServer.SendToClient(conn.connectionId, MsgTypes.PlayerPrefab, msg);
	}




	public override void OnStopServer()
	{
		GameObject.Find("Lobby Panel").GetComponent<LobbyMenu>().SetLobbyMenuVisible(true);
	}
}