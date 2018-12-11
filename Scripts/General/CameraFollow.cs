using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Tooltip("the default zoom.")]
	[SerializeField]
	float zoom = -5;
	float _modifiedZoom;
	float _offsetX;
	float _offsetY;
	GameObject player;

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("player");
		_modifiedZoom = zoom;
	}
	
	void Update ()
	{
		Vector3 offset = new Vector3(_offsetX, _offsetY, _modifiedZoom);
		transform.position = player.transform.position + offset;
	}

	void SetToDefault()
	{
		_modifiedZoom = zoom;
		_offsetX = 0;
		_offsetY = 0;
	}
	
	public void SetOffset(float x = 0, float y = 0, float z = 0)
	{
		_offsetX = x;
		_offsetY = y;
		_modifiedZoom = z;
	}
	
}
