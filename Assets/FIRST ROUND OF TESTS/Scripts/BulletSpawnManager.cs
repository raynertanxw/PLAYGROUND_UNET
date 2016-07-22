using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BulletSpawnManager : MonoBehaviour
{
	public int m_ObjectPoolSize = 20;
	public GameObject m_BulletPrefab;
	public GameObject[] m_BulletPool;

	public NetworkHash128 assetID { get; set; }

	public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
	public delegate void UnSpawnDelegate(GameObject spawned);

	void Start()
	{
		assetID = m_BulletPrefab.GetComponent<NetworkIdentity>().assetId;
		m_BulletPool = new GameObject[m_ObjectPoolSize];
		for (int i = 0; i < m_BulletPool.Length; i++)
		{
			m_BulletPool[i] = (GameObject)Instantiate(m_BulletPrefab, Vector3.zero, Quaternion.identity);
			m_BulletPool[i].name = "PoolObject" + i;
			m_BulletPool[i].SetActive(false);
		}

		ClientScene.RegisterSpawnHandler(assetID, SpawnObject, UnSpawnObject);
	}

	public GameObject GetFromPool(Vector3 position)
	{
		for (int i = 0; i < m_BulletPool.Length; i++)
		{
			if (!m_BulletPool[i].activeInHierarchy)
			{
				Debug.Log("Activating object " + m_BulletPool[i].name + " at " + position);
				m_BulletPool[i].transform.position = position;
				m_BulletPool[i].SetActive(true);
				return m_BulletPool[i];
			}
		}

		Debug.LogError ("Could not grab object from pool, nothing available");
		return null;
	}

	public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
	{
		return GetFromPool(position);
	}

	public void UnSpawnObject(GameObject spawned)
	{
		Debug.Log ("Re-pooling object " + spawned.name);
		spawned.SetActive (false);
	}
}
