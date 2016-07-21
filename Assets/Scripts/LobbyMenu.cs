using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum PlayerSpawnType { Normal, Big }

public class LobbyMenu : MonoBehaviour
{
	private PlayerSpawnType mSpawnType = PlayerSpawnType.Normal;
	private CanvasGroup mCG;

	void Awake()
	{
		mCG = GetComponent<CanvasGroup>();
	}

	public void SetLobbyMenuVisible(bool _visible)
	{
		if (_visible)
			mCG.alpha = 1f;
		else
			mCG.alpha = 0f;
		mCG.blocksRaycasts = _visible;
		mCG.interactable = _visible;
	}

	#region Button functions
	public void SetPlayerSpawnType(int _typeID)
	{
		PlayerSpawnType newType = (PlayerSpawnType)_typeID;
		if (newType == mSpawnType)
			return;

		mSpawnType = newType;
		MultiPlayerPrefabNetworkManager.Instance.playerPrefabIndex = (short)_typeID;
	}
	#endregion
}
