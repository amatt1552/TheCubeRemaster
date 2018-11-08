using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
	public Transform targetPosition;
	const float closeToPoint = 0.01f;
	public float elevatorSpeed = 2;
	GameObject _player;

	void Start ()
	{
		if (targetPosition != null)
		{
			StartCoroutine("MoveToPoint");
		}
	}

	IEnumerator MoveToPoint()
	{
		while (Vector3.Distance(targetPosition.position, transform.position) < closeToPoint)
		{
			Vector3.MoveTowards(transform.position, targetPosition.position, elevatorSpeed);
			yield return null;
		}
	}
}
