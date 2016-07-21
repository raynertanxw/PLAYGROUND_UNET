using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LocalPlayerManager : NetworkBehaviour
{
	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

//		GameObject.Find("Lobby Panel").GetComponent<LobbyMenu>().SetLobbyMenuVisible(false);
	}

	public override void OnNetworkDestroy()
	{
		base.OnNetworkDestroy();

//		GameObject.Find("Lobby Panel").GetComponent<LobbyMenu>().SetLobbyMenuVisible(true);
	}



	private ColorGrid mLocalColorGrid;

	void Update()
	{
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown(KeyCode.E))
		{
			CmdETriggered();
		}
	}

	[Command] private void CmdETriggered()
	{
		GameObject.Find("ColorGrid").GetComponent<ColorGrid>().CmdRandColor();
	}
}
