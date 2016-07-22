using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : MonoBehaviour
{
	private const float TimeToDespawn = 2.0f;

	private BulletSpawnManager spwnManager;
	private float mfTimeLeftTillDespawn;

	void Awake()
	{
		spwnManager = GameObject.Find("BulletSpawnManager").GetComponent<BulletSpawnManager> ();
	}

	void OnEnable()
	{
		mfTimeLeftTillDespawn = TimeToDespawn;
	}

	void Update()
	{
		mfTimeLeftTillDespawn -= Time.deltaTime;
		if (mfTimeLeftTillDespawn < 0f)
		{
			ReturnSelfToPool();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		GameObject hit = collision.gameObject;
		Health health = hit.GetComponent<Health>();
		if (health != null)
		{
			health.TakeDamage(10);
		}

		ReturnSelfToPool();
	}

	private void ReturnSelfToPool()
	{
		spwnManager.UnSpawnObject(gameObject);
		NetworkServer.UnSpawn(gameObject);
	}
}
