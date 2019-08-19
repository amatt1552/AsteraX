using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TrailRenderer))]
public class TrailSnapFix : MonoBehaviour
{
	private TrailRenderer _trail;
	Vector3 _oldPosition;
	Vector3 _newPosition;
	[Tooltip("how close the trail renderer has to be to the new position before working")]
	public float maxDistance = 1;
	void Start()
	{
		_trail = GetComponent<TrailRenderer>();
		_oldPosition = _newPosition = transform.position;
	}

	void LateUpdate()
	{
		_oldPosition = _newPosition;
		_newPosition = transform.position;
		if (Vector3.Distance(_oldPosition, _newPosition) < maxDistance)
		{

			_trail.emitting = true;
		}
		else
		{
			_trail.Clear();
			_trail.emitting = false;
		}
	}
}
