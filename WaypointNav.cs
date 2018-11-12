using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNav : MonoBehaviour
{
	public Waypoints waypoints;
	public int speed = 1;
	public float waitTime = 1;
	int target;
	const float distanceIsCloseTo = 0.1f;

	private void Start()
	{
		StartCoroutine("Move");
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

	private void MoveToNext()
	{
		target++;
		if (target >= waypoints.connectedPoints.Length)
		{
			target = 0;
		}
	}
}
