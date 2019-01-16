using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
	public Transform[] connectedPoints;
	public bool debugging = true;
	private void OnDrawGizmos()
	{
		if (debugging)
		{
			for (int i = 0; i < connectedPoints.Length; i++)
			{
				if (i + 1 < connectedPoints.Length)
				{
					Debug.DrawLine(connectedPoints[i].position, connectedPoints[i + 1].position);
				}
			}
		}
	}
}
