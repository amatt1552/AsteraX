using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class ExtentionMethods
{

	/// <summary>
	/// based on this: https://answers.unity.com/questions/1317007/how-to-check-if-a-tag-exists.html
	/// </summary>
	/// <param name="aObj"></param>
	/// <param name="aTag"></param>
	/// <returns></returns>
	public static bool DoesTagExist(this GameObject aObj, string aTag)
	{
		try
		{
			if (GameObject.FindWithTag(aTag))
			{
				return true;
			}
			else
			{
				Debug.LogWarning("No GameObject with the tag " + aTag + " Set. Make sure to add a " + aTag + " tag to a GameObject in inspector.");
				return false;
			}
			
		}
		catch
		{
			Debug.LogError("No " + aTag + " GameObject Set. Make sure to add a " + aTag + " tag to tags");
			return false;
		}
	}
	/// <summary>
	/// this gets all the Toggles under a ToggleGroup if enabled.
	/// Toggles must be children of said ToggleGroup.
	/// </summary>
	/// <param name="aToggleGroup"></param>
	/// <param name="getAll"></param>
	/// <returns></returns>
	public static Toggle[] ActiveToggles(this ToggleGroup aToggleGroup, bool getAll)
	{
		if (getAll)
		{
			return aToggleGroup.GetComponentsInChildren<Toggle>(true);
		}
		return aToggleGroup.ActiveToggles().ToArray();
	}

	/// <summary>
	/// Gets the  active toggle's index in toggle group. returns -1 if null.
	/// </summary>
	/// <param name="aToggleGroup"></param>
	/// <returns></returns>
	public static int GetActiveToggleIndex(this ToggleGroup aToggleGroup)
	{

		if (aToggleGroup != null)
		{
			Toggle[] toggleArray = aToggleGroup.GetComponentsInChildren<Toggle>(true);

			for (int i = 0; i < toggleArray.Length; i++)
			{
				if (toggleArray[i].isOn)
				{
					return i;
				}

			}
		}
		return -1;
	}
}
