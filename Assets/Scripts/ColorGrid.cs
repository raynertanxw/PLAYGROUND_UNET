using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ColorGrid : NetworkBehaviour
{
	[SyncVar (hook = "OnChangeGridWireframeCol")] private Color gridWireframeCol = Color.cyan;
	private Material cubeMat;

	void Start()
	{
		if (cubeMat != null)
			return;

		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.GetComponent<MeshRenderer>().material = new Material (Shader.Find(" Diffuse"));
		cubeMat = cube.GetComponent<MeshRenderer>().material;
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
