using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public Transform objectToFollow;
	
	void Update ()
	{
		if (objectToFollow != null)
		{
			transform.position = objectToFollow.position;
		}
	}
}
