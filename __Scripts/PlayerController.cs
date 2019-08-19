
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(HideObject))]
[RequireComponent(typeof(ShipParts))]
public class PlayerController : MonoBehaviour
{
	//bullet

	public GameObject bullet;
	public Transform firingPoint;

	//movement

	public float speed = 1;
	public float maxTilt = 30;
	private Vector3 axis;
	public bool backwardsTilt;

	//death

	public int maxJumps = 3;
	private int _currentJumps;
	public bool jumping;
	const int _STARTJUMPS = 3;
	public int respawnTime = 2;
	public bool dead;

	//player
	
	const int _MAXSCORE = 100000000;
	int _score;
	private Rigidbody _rbPlayer;
	private CapsuleCollider _playerCollider;
	private ShipParts _shipParts;

	//effects

	public TrailRenderer trail;

	//events
	public UnityEvent OnJump;
	public UnityEvent OnDeath;
	public UnityEvent OnRespawn;
	private void Awake()
	{
		_shipParts = GetComponent<ShipParts>();
		_rbPlayer = GetComponent<Rigidbody>();
		_playerCollider = GetComponent<CapsuleCollider>();
		_currentJumps = _STARTJUMPS;
	}
	void Start()
	{
		//update UI
		UIScript.UpdateJumps(_currentJumps);
		UIScript.UpdateScore(_score);

		//initializing events

		if (OnJump == null)
		{
			OnJump = new UnityEvent();
		}
		if (OnDeath == null)
		{
			OnDeath = new UnityEvent();
		}
		if (OnRespawn == null)
		{
			OnRespawn = new UnityEvent();
		}
	}

	//normally i'd do the updating in another script, but want to save some time.
	void Update()
	{
		axis = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
		bool fire = CrossPlatformInputManager.GetButtonDown("Fire1");
		
		if (fire && !jumping && AsteraX.GetLevel() != 0 && Time.timeScale > 0)
		{
			Fire();
		}
	}

	private void FixedUpdate()
	{
		if (!jumping && AsteraX.GetLevel() != 0)
		{
			SimpleMove(axis, speed);
			TiltWithVelocity(axis);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		jumping = true;
		
		_rbPlayer.velocity = Vector3.zero;
		_rbPlayer.angularVelocity = Vector3.zero;
		StartCoroutine(TryJump());
	}

	/// <summary>
	/// Moves without collision checking.
	/// </summary>
	public void SimpleMove(Vector3 axis, float speed)
	{
		_rbPlayer.velocity = axis * speed;
	}

	public void Fire()
	{
		if (bullet != null && firingPoint != null)
		{
			Achievements.BULLETS_FIRED++;
			Achievements.AchievementCheck();
			Vector3 firingVector = firingPoint.position;
			firingVector.z = 0;
			Instantiate(bullet, firingVector, firingPoint.rotation);
		}
	}

	void TiltWithVelocity(Vector3 axis)
	{
		axis = new Vector3(axis.y, -axis.x, 0);
		if (_shipParts.bodyParent != null)
		{
			_shipParts.bodyParent.transform.rotation = Quaternion.AngleAxis(maxTilt * axis.magnitude, axis);
		}
	}

	public IEnumerator TryJump()
	{
		if (_currentJumps > 0)
		{
			OnJump.Invoke();
			_playerCollider.enabled = false;
			yield return new WaitForSeconds(respawnTime);
			Vector3 randomPosition = ScreenBounds.RANDOM_ON_SCREEN_LOC_HALF;
			List<Asteroid> asteroids = AsteraX.ASTEROIDS;
			int infLoopSave = 0;
			int distance = 5;
			for (int i = 0; i < asteroids.Count;)
			{
				infLoopSave++;
				if (Vector3.Distance(randomPosition, asteroids[i].transform.position) > distance)
				{
					i++;
				}
				else
				{
					//one was in the way so trying again
					i = 0;
					randomPosition = ScreenBounds.RANDOM_ON_SCREEN_LOC_HALF;
					
				}

				//in case it doesn't work I reduce the distance so it will spawn eventually.

				if (infLoopSave > asteroids.Count * 100)
				{
					distance = distance >= 1 ? distance / 2 : 0;
					Debug.Log("Distance set too high, reduced to: " + distance);
					infLoopSave = 0;
				}
			}
			RemoveJumps(1);
			UIScript.UpdateJumps(_currentJumps);
			transform.position = randomPosition;
			OnRespawn.Invoke();
			_playerCollider.enabled = true;
			jumping = false;
		}
		else
		{
			dead = true;
			_playerCollider.enabled = false;
			SaveGameManager.CheckHighScore(_score);
			UIScript.SetFinalScore(_score, AsteraX.GetLevel());
			CustomAnalytics.SendGameOver();
			CustomAnalytics.SendFinalShipPartChoice();
			OnDeath.Invoke();
		}
	}
	
	//get and setters

	public void AddJumps(int amount)
	{
		_currentJumps = _currentJumps + amount < maxJumps ? _currentJumps + amount : maxJumps;
		
	}
	public void RemoveJumps(int amount)
	{
		_currentJumps = _currentJumps - amount > 0 ? _currentJumps - amount : 0;
	}
	public int GetJumps()
	{
		return _currentJumps;
	}

	public void AddPoints(int amount)
	{
		_score = _score + amount < _MAXSCORE ? _score + amount : _MAXSCORE;
		Achievements.SCORE = _score;
		Achievements.AchievementCheck();
	}
	public void RemovePoints(int amount)
	{
		_score = _score - amount > 0 ? _score - amount : 0;
		Achievements.SCORE = _score;
		Achievements.AchievementCheck();
	}
	public int GetScore()
	{
		return _score;
	}

	//jump Effect
	public void InitJumpEffect()
	{
		ParticleSystem particle = Instantiate(PlayerScriptableObject.GetRandomJumpEffect(), transform.position, transform.rotation);

		particle.Play();
		Destroy(particle.gameObject, 4);
	}
}
