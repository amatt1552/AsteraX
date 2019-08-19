using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	public float bulletSpeed = 20;
	public float lifetime = 2;
	private Rigidbody _rbBullet;
	public TrailRenderer trail;
	public bool bulletWrapped;

	void Start ()
	{ 
		_rbBullet = GetComponent<Rigidbody>();
		_rbBullet.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
		Destroy(gameObject, lifetime);
	}

	public void SetBulletWraped()
	{
		bulletWrapped = true;
	}
}
