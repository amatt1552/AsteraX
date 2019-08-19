//#define MOBILE_INPUT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[CreateAssetMenu(fileName = "AsteroidData", menuName = "Asteroid")]
[System.Serializable]

public class AsteroidScriptableObject : ScriptableObject
{
	static public AsteroidScriptableObject S;
	public AsteroidScriptableObject()
	{
		S = this;
	}
	
	[Header("Asteroid Settings")]
	public float gravityScale = 0;
	public bool useAutoMass = true;

	[Header("Asteroid Spawning Settings")]
	public int size = 3;

	private string _levelProgression = "1:3/2,2:4/2,3:3/3,4:4/3,5:5/3,6:3/4,7:4/4,8:5/4,9:6/4,10:3/5";

	[Tooltip("How much smaller they get")]
	public float scaleOffset = 0.75f;

	/// <summary>
	/// How many children you get per parent
	/// </summary>
	[Tooltip("How many children you get per parent")]
	[HideInInspector]
	public float targetChildCount;
	[HideInInspector]
	public int targetAsteroidCount;
	public int minVelocity = 5;
	public int maxVelocity = 10;
	public int maxAngularVelocity = 10;

	[Tooltip("Disance from the player where the Asteroids can't spawn")]
	public int spawnDeadZone = 4;
	public List<GameObject> asteroidList;
	public List<GameObject> simpleAsteroidList;
	[Tooltip("Particle systems you wish to use for explosions")]
	public List<ParticleSystem> asteroidExplosions;

	//the forgetten vars..
	//public List<int> asteroidCountPerLevel;
	//public int exponent = 1;
	//public DifficultyScaling difficultyScaling;
	/*
	/// <summary>
	/// Scales Difficulty from inspector
	/// </summary>
	/// <param name="Exponential"> Scales based on level to power of Exponent times the StartCount </param>>
	/// <param name="Custom"> use another script or set each level in inspector to handle scaling </param>>
	public enum DifficultyScaling
	{
		Exponential,
		Custom
	}
	*/

	public GameObject GetRandomAsteroidPrefab()
	{
#if MOBILE_INPUT
		return simpleAsteroidList[Random.Range(0, asteroidList.Count)];
#else

		Debug.LogWarning("Mobile Input not available.");
		return asteroidList[Random.Range(0, asteroidList.Count)];
#endif
	}

	public ParticleSystem GetRandomAsteroidExplosion()
	{
		return asteroidExplosions[Random.Range(0, asteroidExplosions.Count)];
	}
	public void SetLevelProgression(string value)
	{
		_levelProgression = value;
	}
	public void SetCurrentAsteroidCount()
	{
		string[] splitAsteroidArray = _levelProgression.Split(',');
		
		string splitAsteroid = splitAsteroidArray[AsteraX.GetLevel() - 1];
		string ignoreLevel = splitAsteroidArray[AsteraX.GetLevel() - 1].Split(':')[1];
		string[] childrenAndParents = ignoreLevel.Split('/');
		targetAsteroidCount = int.Parse(childrenAndParents[0].ToString());
		targetChildCount = int.Parse(childrenAndParents[1].ToString());
	}
}
