using UnityEngine;
using UnityEngine.Networking;

public class LevelSpawner : NetworkBehaviour
{
	public GameObject ColorGridGO;

	public override void OnStartServer()
	{
		GameObject colorGrid = (GameObject) Instantiate(ColorGridGO, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn(colorGrid);
	}
}
