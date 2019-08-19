//#define DEBUG_ScreenWrap
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ScreenWrap : MonoBehaviour
{
	private Bounds _bounds;
	public Rigidbody rb;
	[Header("Triggers when object is considered out of bounds")]
	public UnityEvent onWrap;
	[Header("Triggers when object has reentered bounds")]
	public UnityEvent onWrapComplete;
	private bool _inTrigger;

	private void Awake()
	{
		_bounds = GameObject.FindGameObjectWithTag("OnScreenBounds").GetComponent<Collider>().bounds;
		rb = GetComponent<Rigidbody>();
		if (onWrap == null)
		{
			onWrap = new UnityEvent();
		}
		if (onWrapComplete == null)
		{
			onWrapComplete = new UnityEvent();
		}
		InvokeRepeating("InBoundsCheck", 1, 1);
	}
	
	private void OnTriggerExit(Collider collider)
	{
		if (enabled)
		{
			_inTrigger = false;
			InBoundsCheck();
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (enabled)
		{
			onWrapComplete.Invoke();
			_inTrigger = true;
		}
	}

	public void InBoundsCheck()
	{
		if (enabled && !_inTrigger)
		{
			float distanceX = Mathf.Abs(transform.position.x - _bounds.center.x);
			float distanceY = Mathf.Abs(transform.position.y - _bounds.center.y);
			float distanceZ = Mathf.Abs(transform.position.z - _bounds.center.z);

			//so first I get the direction the object of which the object is away from the center of bounds

			float directionX = (transform.position.x - _bounds.center.x) > 0 ? 1 : -1;
			float directionY = (transform.position.y - _bounds.center.y) > 0 ? 1 : -1;
			float directionZ = (transform.position.z - _bounds.center.z) > 0 ? 1 : -1;

			//then here I get the velocity's direction

			float velocityX = rb.velocity.x > 0 ? 1 : -1;
			float velocityY = rb.velocity.y > 0 ? 1 : -1;
			float velocityZ = rb.velocity.z > 0 ? 1 : -1;

			//then I check if the velocity is in the same direction as the way its wrapping to. 

			if (distanceX > _bounds.extents.x && directionX == velocityX)
			{
			#if DEBUG_ScreenWrap
			Debug.Log("GET BACK HERE! (x)");
			#endif
				transform.position = new Vector3(-transform.position.x, transform.position.y, 0);
				onWrap.Invoke();
			}
			if (distanceY > _bounds.extents.y && directionY == velocityY)
			{
			#if DEBUG_ScreenWrap
			Debug.Log("GET BACK HERE! (y)");
			#endif
				transform.position = new Vector3(transform.position.x, -transform.position.y, 0);
				onWrap.Invoke();
			}
			if (distanceZ > _bounds.extents.z && directionZ == velocityZ)
			{
			#if DEBUG_ScreenWrap
			Debug.Log("GET BACK HERE! (z)");
			#endif
				transform.position = new Vector3(transform.position.x, transform.position.y, 0);
				onWrap.Invoke();
			}
		}
	}
}
