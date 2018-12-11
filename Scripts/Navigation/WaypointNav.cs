using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNav : MonoBehaviour
{
	[Header("if rigidbody is found then will move with MovePosition, else will move the transform.")]
	public Waypoints waypoints;
	public int speed = 1;
	public float waitTime = 1;
	int target;
	const float distanceIsCloseTo = 0.1f;
	Rigidbody _rb;
	bool waitRbComplete = true;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		
	}

	private void Start()
	{
		if (_rb == null)
		{
			StartCoroutine("Move");
		}
	}

	void FixedUpdate()
	{
		if (_rb != null)
		{
			MoveRb();
		}
	}

	IEnumerator Move()
	{
		while (0 != 1)
		{
			if (Vector3.Distance(transform.position, waypoints.connectedPoints[target].position) < distanceIsCloseTo)
			{
				yield return new WaitForSeconds(waitTime);
				MoveToNext();
			}
			transform.position = Vector3.MoveTowards(transform.position, waypoints.connectedPoints[target].position, speed * Time.deltaTime);
			yield return null;
		}
	}

	void MoveRb()
	{
		
		if (Vector3.Distance(transform.position, waypoints.connectedPoints[target].position) < distanceIsCloseTo)
		{
			if (waitRbComplete)
			{
				StartCoroutine("WaitRb");
				
			}
			
		}
		_rb.MovePosition(Vector3.MoveTowards(transform.position, waypoints.connectedPoints[target].position, speed * Time.deltaTime));
			
		
	}
	IEnumerator WaitRb()
	{
		waitRbComplete = false;
		yield return new WaitForSeconds(waitTime);
		MoveToNext();
		waitRbComplete = true;
	}

	private void MoveToNext()
	{
		target++;
		if (target >= waypoints.connectedPoints.Length)
		{
			target = 0;
		}
	}
}
