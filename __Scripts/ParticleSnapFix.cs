using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSnapFix : MonoBehaviour
{
	private ParticleSystem _particle;
	Vector3 _oldPosition;
	Vector3 _newPosition;
	[Tooltip("how close the trail renderer has to be to the new position before working")]
	public float maxDistance = 1;
	const float _MIN_DISTANCE = 0.01f;
	void Start()
	{
		_particle = GetComponent<ParticleSystem>();
		_oldPosition = _newPosition = transform.position;
	}

	void LateUpdate()
	{
		CheckForSnap();
		
	}

	void CheckForSnap()
	{
		ParticleSystem.EmissionModule emission = _particle.emission;
		_oldPosition = _newPosition;
		_newPosition = transform.position;
		if (Vector3.Distance(_oldPosition, _newPosition) < maxDistance && Vector3.Distance(_oldPosition, _newPosition) > _MIN_DISTANCE)
		{

			emission.enabled = true;
		}
		else
		{
			
			emission.enabled = false;
		}
	}
}
