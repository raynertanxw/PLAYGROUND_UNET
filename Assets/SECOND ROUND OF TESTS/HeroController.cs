using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class HeroController : NetworkBehaviour
{
	private CustomNetworkManager customNetworkManager = null;
	private short playerControllerId = short.MaxValue;
	public short PlayerControllerId { get { return playerControllerId; } }
	private int connectionId;

	NetworkClient client;
	List<int> clientConnectionIds;

	void InitializeClient()
	{
		client = new NetworkClient();
		client.RegisterHandler(MessageTypes.UpdatePosNRot, OnClientRespondTpUpdatePosMsg);

		client.Connect("localhost", 7777);

		Debug.Log(GetInstanceID());
	}

	public override void OnStartServer()
	{
		base.OnStartServer();

		clientConnectionIds = new List<int>();
		NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
		
		NetworkServer.RegisterHandler(MessageTypes.UpdatePosNRot, OnServerRespondToUpdatePosMsg);
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

		customNetworkManager = GameObject.FindObjectOfType<CustomNetworkManager>();
		playerControllerId = GetComponent<NetworkIdentity>().playerControllerId;
		connectionId = customNetworkManager.client.connection.connectionId;
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		InitializeClient();
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		MessageTypes.UpdatePosNRotMsg msg = new MessageTypes.UpdatePosNRotMsg();
		msg.playerControllerId = playerControllerId;
		msg.position = transform.position;
		msg.rotation = transform.rotation;
		customNetworkManager.client.Send(MessageTypes.UpdatePosNRot, msg);
	}

	public void UpdatePosNRot(Vector3 _newPos, Quaternion _newRot)
	{
		transform.position = _newPos;
		transform.rotation = _newRot;
	}


	#region Message Handlers
	private void OnServerConnect(NetworkMessage netMsg)
	{
		clientConnectionIds.Add(netMsg.conn.connectionId);

		NetworkServer.SetClientReady(netMsg.conn);
	}

	private void OnServerRespondToUpdatePosMsg(NetworkMessage netMsg)
	{
		MessageTypes.UpdatePosNRotMsg msg = netMsg.ReadMessage<MessageTypes.UpdatePosNRotMsg>();

		if (msg.playerControllerId == PlayerControllerId)
			return;

//		foreach (int connId in clientConnectionIds)
//		{
//			NetworkServer.SendToClient(connId, MessageTypes.UpdatePosNRot, msg);
//		}

		NetworkServer.SendToAll(MessageTypes.UpdatePosNRot, msg);
	}

	private void OnClientRespondTpUpdatePosMsg(NetworkMessage netMsg)
	{
		MessageTypes.UpdatePosNRotMsg msg = netMsg.ReadMessage<MessageTypes.UpdatePosNRotMsg>();

		if (isLocalPlayer)
			return;

		transform.position = msg.position;
		transform.rotation = msg.rotation;
	}
	#endregion
}
