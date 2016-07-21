using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ColorGrid : NetworkBehaviour
{
	[SyncVar (hook = "OnChangeGridWireframeCol")] private Color gridWireframeCol = Color.cyan;
	private Material cubeMat;
	public GameObject cubeGO;

	public override void OnStartServer()
	{
		base.OnStartServer();

		GameObject cube = (GameObject) Instantiate(cubeGO, Vector3.zero, Quaternion.identity);
		cubeMat = cube.GetComponent<MeshRenderer>().material;

		NetworkServer.Spawn(cube);
	}

	[Command] public void CmdRandColor()
	{
		gridWireframeCol = Random.ColorHSV(0.5f, 1.0f);
	}

	void OnChangeGridWireframeCol(Color _newCol)
	{
		cubeMat.color = _newCol;
	}
}
