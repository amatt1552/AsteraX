using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{
	private MeshRenderer[] _meshRenderers;
	
	public void Hide()
	{
		_meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mesh in _meshRenderers)
		{
			mesh.enabled = false;
		}
	}

	public void Show()
	{
		_meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mesh in _meshRenderers)
		{
			mesh.enabled = true;
		}
	}
}