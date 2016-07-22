using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	private BulletSpawnManager bulletSpwnManager;

	void Start()
	{
		bulletSpwnManager = GameObject.Find("BulletSpawnManager").GetComponent<BulletSpawnManager> ();
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdFire();
		}
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

		if (isServer && isClient)
		{
			GetComponent<MeshRenderer>().material.color = Color.green;
		}
		else
		{
			GetComponent<MeshRenderer>().material.color = Color.blue;
		}
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		if (isLocalPlayer)
			return;

		GetComponent<MeshRenderer>().material.color = Color.yellow;
	}

	[Command]
	private void CmdFire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = bulletSpwnManager.GetFromPool(bulletSpawn.position);
		bullet.transform.rotation = bulletSpawn.rotation;
		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

		// Spawn the bullet on clients.
		NetworkServer.Spawn(bullet, bulletSpwnManager.assetID);
	}
}
