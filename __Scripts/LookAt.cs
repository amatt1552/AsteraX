using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
	public Transform pivot;
	Vector3 mousePos;
	void Start ()
	{
		if (pivot == null)
			pivot = transform;
	}
	
	void Update ()
	{
		Vector3 tempPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos = new Vector3(tempPos.x, tempPos.y, 0);
		if(Time.timeScale != 0)
			QuatLookAt2D(mousePos, Vector3.back);
	}

	void QuatLookAt2D(Vector3 targetPoint, Vector3 axis)
	{
		Vector3 direction = targetPoint - pivot.position;

		pivot.rotation = Quaternion.LookRotation(direction, axis);
	}

	void TransLookAt2D(Vector3 targetPoint)
	{
		transform.LookAt(pivot, targetPoint);
	}
}
