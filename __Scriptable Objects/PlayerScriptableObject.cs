using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player")]
[System.Serializable]

public class PlayerScriptableObject : ScriptableObject
{
	private static PlayerScriptableObject _S;
	public PlayerScriptableObject()
	{
		_S = this;
	}

	public GameObject[] shipTurrets;
	public GameObject[] shipBodies;
	[Tooltip("Particle systems you wish to use for jumps")]
	public List<ParticleSystem> jumpEffects;


	public static GameObject GetTurret(uint index)
	{
		if (index < _S.shipTurrets.Length && _S.shipTurrets != null)
		{
			return _S.shipTurrets[index];
		}
		else
		{
			Debug.LogError("No turret Found at index. Make sure its defined.");
			return null;
		}
	}

	public static GameObject GetBody(uint index)
	{
		if (index < _S.shipBodies.Length && _S.shipBodies != null)
		{
			return _S.shipBodies[index];
		}
		else
		{
			Debug.LogError("No body Found at index. Make sure its defined.");
			return null;
		}
	}

	public static ParticleSystem GetRandomJumpEffect()
	{
		return _S.jumpEffects[Random.Range(0, _S.jumpEffects.Count)];
	}
}
