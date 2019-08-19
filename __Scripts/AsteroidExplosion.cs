using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AsteroidExplosion : MonoBehaviour
{
	const int _FRACTLE = 50;
	void Start ()
	{
		ParticleSystem particle = GetComponent<ParticleSystem>();
		particle.Emit(_FRACTLE);
		Destroy(gameObject, 1);
	}
	
}
