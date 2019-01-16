using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Tooltip("its default position")]
	public Vector3 defaultOffset = Vector3.zero;
	Vector3 _offset;

	public GameObject FollowedObject;
	public bool smoothMovement;
	public float cameraSpeed = 1;
	bool _followObject = true;
	static float DISTANCE_TO_POINT = 0.5f;

	void Start ()
	{
		if (FollowedObject == null)
		{
			FollowedObject = TheCubeGameManager.player;
		}
		
	}
	
	void Update ()
	{
		FollowObject();
	}

	public void SetToDefault()
	{
		_offset = Vector3.zero;
	}

	void FollowObject()
	{
		//maybe make it not smooth when close enough

		Vector3 targetPosition = FollowedObject.transform.position + defaultOffset + _offset;
		if (Vector3.Distance(transform.position, targetPosition) > DISTANCE_TO_POINT)
		{
			smoothMovement = true;
		}
		else
		{
			smoothMovement = false;
		}
		if (_followObject && FollowedObject != null)
		{
			if (smoothMovement)
			{
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, cameraSpeed * Time.deltaTime);	
			}
			else
			{
				transform.position = FollowedObject.transform.position + defaultOffset + _offset;
			}
		}
	}

	public void SetOffset(float x = 0, float y = 0, float z = 0)
	{
		_offset.x = x;
		_offset.y = y;
		_offset.z = z;
	}
	public void SetOffset(Vector3 newOffset)
	{
		_offset = newOffset;
	}

	public void SnapToObject()
	{
		transform.position = FollowedObject.transform.position + defaultOffset + _offset;
	}
}
