using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ScreenWrap))]
[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{
	private AsteroidScriptableObject _asteroidInfo;
	private Rigidbody _rb;
	public ScreenWrap wrap;
	public float size;
	private PlayerController playerController;
	[Tooltip("Add Compound Colliders of asteroid here.")]
	public GameObject[] compoundColliders; 

	//events
	[Header("happens right before asteroid is destroyed")]
	public UnityEvent onDestroy;

	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
		_asteroidInfo = AsteroidScriptableObject.S;
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		if (_asteroidInfo != null)
		{
			wrap = GetComponent<ScreenWrap>();
			//checks if has an parent and set values accordingly
			if (transform.parent == null)
			{
				EnableRB();
				size = _asteroidInfo.size > 0 ? _asteroidInfo.size: 1;
				transform.localScale = Vector3.one * size;
				//_rb.mass = size;
				AddForce(Random.onUnitSphere, Random.Range(_asteroidInfo.minVelocity, _asteroidInfo.maxVelocity));
				AddAngularVelocity(Random.insideUnitSphere * _asteroidInfo.maxAngularVelocity);
				
			}
			else
			{
				_rb.mass = size * 3;
				DisableRB();
			}

			AddChildren();
		}

		//initializing events

		if (onDestroy == null)
		{
			onDestroy = new UnityEvent();
		}
		
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_asteroidInfo != null && enabled && transform.parent == null)
		{
			AsteroidHit(collision);
		}
		else if (_asteroidInfo != null && enabled && transform.parent != null)
		{
			//this makes sure it hits even when it was a child that was hit.
			print("Hit Child");
			Transform parent = transform.root;
			if (parent.GetComponent<Asteroid>())
				parent.GetComponent<Asteroid>().AsteroidHit(collision);
		}

	}

	//should probably change so that it adds the score and such on Achievements or AsteraX
	/// <summary>
	/// Handles destruction of the Asteroid when its hit
	/// </summary>
	/// <param name="collision"></param>
	public void AsteroidHit(Collision collision)
	{
		int childCount = transform.childCount;
		//getting position before destroying the bullet
		Vector3 collisionPosition = collision.transform.position;
		//updates score and destroys bullet
		if (collision.gameObject.GetComponent<Bullet>())
		{
			//the score seemed to be like binary so thought I'd just calculate for any size if the player didn't die already.
			//added more to score while dead so fixed that.
			if (!AsteraX.DEAD)
			{
				playerController.AddPoints((int)Mathf.Pow(2, _asteroidInfo.size - size) * 100);
				Achievements.ASTEROIDS_HIT++;
			
				Achievements.AchievementCheck();
				if (collision.gameObject.GetComponent<Bullet>().bulletWrapped)
				{
					Achievements.BULLET_WRAP_COUNT++;
					Achievements.AchievementCheck();
				}
			}
			UIScript.UpdateScore(playerController.GetScore());
			Destroy(collision.gameObject);
		}

		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(0);
			child.SetParent(null, true);

			if (child.GetComponent<Asteroid>())
			{
				Asteroid asteroidComp = child.GetComponent<Asteroid>();
				asteroidComp.EnableRB();
				asteroidComp.AddForceFromPoint(collisionPosition, Random.Range(_asteroidInfo.minVelocity, _asteroidInfo.maxVelocity));
				asteroidComp.AddAngularVelocity(Random.insideUnitSphere * _asteroidInfo.maxAngularVelocity);

			}

		}

		//dont need to check for get component since this is the component its looking for.
		AsteraX.RemoveAsteroid(this);
		//starts onDestroy Event
		onDestroy.Invoke();
		AsteraX.ExplosionEffect(transform, size);
		//destroys this asteroid
#if MOBILE_INPUT
		foreach (GameObject col in compoundColliders)
		{
			Destroy(col);
		}
#endif
		Destroy(gameObject);
	}

	public void EnableRB()
	{
		_rb.isKinematic = false;
		wrap.enabled = true;
		wrap.InBoundsCheck();
	}

	public void DisableRB()
	{
		_rb.isKinematic = true;
		wrap.enabled = false;
	}

	public void AddForce(Vector3 direction, float force)
	{
		_rb.velocity += direction * force / size;
	}

	/// <summary>
	/// Like an explosion effect.
	/// </summary>
	/// <param name="forcePosition"></param>
	/// <param name="force"></param>
	public void AddForceFromPoint(Vector3 forcePosition, float force)
	{
		//_rb.velocity = Vector3.zero;
		Vector3 direction = Vector3.Normalize(_rb.transform.position - forcePosition);
		_rb.velocity += direction * force / size;
	}

	public void AddAngularVelocity(Vector3 angularVelocity)
	{
		_rb.angularVelocity = angularVelocity;
	}

	void AddChildren()
	{
		if (size > 1)
		{
			
			//maybe rename to asteroidComponent later
			Asteroid ast;
			for (int i = 0; i < _asteroidInfo.targetChildCount; i++)
			{
				GameObject asteroid = Instantiate(_asteroidInfo.GetRandomAsteroidPrefab());

				//in case my teamates forget to add the asteroid script
				if (asteroid.GetComponent<Asteroid>())
				{
					ast = asteroid.GetComponent<Asteroid>();
				}
				else
				{
					Debug.LogError("ASTEROID SCRIPT NOT IS ON THE " + asteroid.name + " ASSET! fix it.");
					ast = asteroid.AddComponent<Asteroid>();

				}
				
				ast.size = size - 1;
				AsteraX.AddAsteroid(asteroid.GetComponent<Asteroid>());

				Vector3 relPos = Random.onUnitSphere / 2;
				asteroid.transform.parent = transform;
				asteroid.transform.rotation = Random.rotation;
				asteroid.transform.localPosition = relPos;
				asteroid.transform.localScale = Vector3.one * _asteroidInfo.scaleOffset;
				asteroid.name = gameObject.name + "_" + i.ToString("00");
			}
		}
	}

}
