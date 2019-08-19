using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class AsteraX : MonoBehaviour
{
	private static AsteraX _S;
	public AsteroidScriptableObject asteroidInfo;
	public PlayerScriptableObject playerInfo;
	private PlayerController _playerController;
	public static List<Asteroid> ASTEROIDS;
	private bool _pause;
	private bool _levelCompleteOneShot;
	private bool _useLevelCompleteWait;
	public static bool DEAD;
	public static bool JUMPING;
	private int _currentLevel = 0;
	public UnityEvent onLevelComplete;
	private bool _gameStarted;

	private void Awake()
	{
		if (_S == null)
		{
			_S = this;
		}
		if (onLevelComplete == null)
		{
			onLevelComplete = new UnityEvent();
		}
		ASTEROIDS = new List<Asteroid>();
	}

	void Start ()
	{
		RemoteSettings.Updated += RemoteSettingsUpdated;
		if (gameObject.DoesTagExist("Player"))
		{
			_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		}
	}
	private void Update()
	{
		DEAD = _playerController.dead;
		JUMPING = _playerController.jumping;
		if (ASTEROIDS.Count == 0 && _gameStarted && !DEAD && !JUMPING && !_levelCompleteOneShot)
		{
			//this is so that the first level starts immediately
			if (_useLevelCompleteWait)
			{
				_levelCompleteOneShot = true;
				StartCoroutine("LevelCompleteWait", 0.5f);
			}
			else
			{
				_useLevelCompleteWait = true;
				LevelComplete();
			}
		}
	}

	/// <summary>
	/// creates Asteroids based on info from AsteroidScriptableObject. Best to be called only once per level or on event.
	/// </summary>
	public void CreateAsteroids()
	{
		if (asteroidInfo != null && asteroidInfo.asteroidList != null)
		{
			for (int i = 0;i < asteroidInfo.targetAsteroidCount;)
			{
				//gets a random position within the bounds
				Vector3 randomPosition = ScreenBounds.RANDOM_ON_SCREEN_LOC;
				//gets a random rotation
				Quaternion randomRotation = Random.rotation;

				if (Vector3.Distance(randomPosition, _playerController.transform.position) > asteroidInfo.spawnDeadZone)
				{
					GameObject asteroid = Instantiate(asteroidInfo.GetRandomAsteroidPrefab(), randomPosition , randomRotation);
					//in case we forget to add the asteroid script
					if (asteroid.GetComponent<Asteroid>())
					{
						AddAsteroid(asteroid.GetComponent<Asteroid>());
					}
					else
					{
						Debug.LogError("ASTEROID SCRIPT NOT IS ON THE "+ asteroid.name + " PREFAB! fix it.");
					}
					i++;
				}
			}
		}
	}

	public static void AddAsteroid(Asteroid asteroid)
	{
		if (ASTEROIDS.IndexOf(asteroid) == -1)
		{
			ASTEROIDS.Add(asteroid);
		}
	}
	public static void RemoveAsteroid(Asteroid asteroid)
	{
		if (ASTEROIDS.IndexOf(asteroid) != -1)
		{
			ASTEROIDS.Remove(asteroid);
		}
	}
	public static int GetLevel()
	{
		return _S._currentLevel;
	}
	public void RestartAfterTime(float time)
	{
		StartCoroutine("_RestartAfterTime", time);
	}
	IEnumerator _RestartAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void StartGame()
	{
		_gameStarted = true;
	}

	public void LevelComplete()
	{
		_currentLevel++;
		Achievements.AchievementCheck();
		asteroidInfo.SetCurrentAsteroidCount();
		UIScript.SetLevelSettings(asteroidInfo.targetAsteroidCount, (int)asteroidInfo.targetChildCount);
		if (_currentLevel > 10)
		{
			RestartAfterTime(0);
			return;
		}
		CustomAnalytics.SendLevelStart(_currentLevel);
		onLevelComplete.Invoke();
		string tag = "Bullet";
		if (gameObject.DoesTagExist(tag))
		{
			foreach (GameObject bullet in GameObject.FindGameObjectsWithTag(tag))
			{
				Destroy(bullet);
			}
		}
		CreateAsteroids();

		_levelCompleteOneShot = false;
	}
	/// <summary>
	/// waits for a time before considered being complete
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	IEnumerator LevelCompleteWait(float time)
	{
		yield return new WaitForSeconds(time);
		LevelComplete();
	}
	
	//effects

	//maybe make explosion effect script
	public static void ExplosionEffect(Transform startTransform, float scale)
	{
		ParticleSystem particle = Instantiate(_S.asteroidInfo.GetRandomAsteroidExplosion(), startTransform.position, startTransform.rotation);

		particle.transform.localScale = Vector3.one * scale;
		particle.Play();
		Destroy(particle.gameObject, 1);
	}
	
	//pause
	public void SetPause()
	{
		_pause = UIScript.Paused();
		if (_pause)
		{
			Time.timeScale = 0;
			Achievements.AchievementCheck();
			UIScript.ShowPause(true);
		}
		else
		{
			Time.timeScale = 1;
			UIScript.ShowPause(false);
		}
		
	}
	public bool GetPause()
	{
		return _pause;
	}
	//remote Settings

	void RemoteSettingsUpdated()
	{
		string newLevelProgression = RemoteSettings.GetString("levelProgression", "");
		if (newLevelProgression != "")
		{
			asteroidInfo.SetLevelProgression(newLevelProgression);
			Debug.Log("Remote Setting mission complete.");
		}
		else
		{
			Debug.Log("Remote Setting mission failed. we'll get them next time..");
		}
	}

}
