using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MessageTypes
{
	public const short PlayerMode = MsgType.Highest + 1;
	public const short PlayerPrefab = MsgType.Highest + 2;

	public class PlayerModeMsg : MessageBase
	{
		public short controllerID;
		public PlayerMode mode;
	}

	public class PlayerPrefabMsg : MessageBase
	{
		public short controllerID;    
		public short prefabIndex;
	}
}
