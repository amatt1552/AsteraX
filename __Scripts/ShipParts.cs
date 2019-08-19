using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipParts : MonoBehaviour
{
	public GameObject turret;
	public GameObject turretParent;
	public GameObject body;
	public GameObject bodyParent;

	void Update ()
	{
		if (!AsteraX.JUMPING)
		{
			SetTurret((uint)Achievements.GetProgress().currentTurret);
			SetBody((uint)Achievements.GetProgress().currentBody);
		}
	}

	public void SetTurret(uint index)
	{
		if (turret != null  && PlayerScriptableObject.GetTurret(index) != null)
		{
			if (turret.name != PlayerScriptableObject.GetTurret(index).name)
			{
				Destroy(turret);
			}
		}
		//log here
		if (turret == null)
		{
			turret = Instantiate(PlayerScriptableObject.GetTurret(index));

			if (turretParent.transform != null)
			{
				turret.transform.parent = turretParent.transform;
				turret.transform.localPosition = Vector3.zero;
				turret.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
				turret.name = PlayerScriptableObject.GetTurret(index).name;
				SaveGameManager.Save();
			}
		}
		//log here
		
	}
	public void SetBody(uint index)
	{
		if (body != null && PlayerScriptableObject.GetBody(index) != null)
		{
			if(body.name != PlayerScriptableObject.GetBody(index).name)
			Destroy(body);
		}
		//log here
		if (body == null)
		{
			body = Instantiate(PlayerScriptableObject.GetBody(index));

			if (bodyParent.transform != null)
			{
				body.transform.parent = bodyParent.transform;
				body.transform.localPosition = Vector3.zero;
				body.transform.localRotation = Quaternion.identity;
				body.name = PlayerScriptableObject.GetBody(index).name;
				SaveGameManager.Save();
			}
		}
		//log here
	}
}
