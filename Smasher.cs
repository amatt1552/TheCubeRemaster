using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Smasher : MonoBehaviour
{
	enum eSmasherState
	{
		Smashing, Retracting, Waiting
	}
	eSmasherState _smasherState;

	Rigidbody _rb;
	public Transform topTransform, bottomTransform;
	const float _smashSpeed = 20;
	public float smashRetractPause = 4;
	const float _smashRetractSpeed = 5;
	const float _closeToPoint = 0.1f;
	const float _smashPause = 1;
	//makes sure the Wait function runs only once at a time
	bool _enumOneShot;

	void Awake ()
	{
		_rb = GetComponent<Rigidbody>();
		_smasherState = eSmasherState.Retracting;
	}
	
	void FixedUpdate ()
	{
		if (topTransform == null || bottomTransform == null)
		{
			Debug.LogError("top or bottom Transforms not set.");
			return;
		}

		switch ((int)_smasherState)
		{
			case 0:
				Smash();
				break;
			case 1:
				Retract();
				break;
			case 2:
				break;
			default:
				break;
		}

	}

	void Smash()
	{
		_rb.MovePosition(Vector3.MoveTowards(transform.position, bottomTransform.position, _smashSpeed * Time.deltaTime));
		
		if (Vector3.Distance(transform.position, bottomTransform.position) < _closeToPoint && !_enumOneShot)
		{
			_enumOneShot = true;
			StartCoroutine(Wait(_smashPause, eSmasherState.Retracting));
		}
	}

	void Retract()
	{
		_rb.MovePosition(Vector3.MoveTowards(transform.position, topTransform.position, _smashRetractSpeed * Time.deltaTime));
		print("retracting");
		if (Vector3.Distance(transform.position, topTransform.position) < _closeToPoint && !_enumOneShot)
		{
			print("retractingDone");
			_enumOneShot = true;
			StartCoroutine(Wait(smashRetractPause, eSmasherState.Smashing));
		}
	}
	
	//waits for time then sets state to be targeted state.

	IEnumerator Wait(float time, eSmasherState targetState)
	{
		_smasherState = eSmasherState.Waiting;
		yield return new WaitForSeconds(time);
		_smasherState = targetState;
		_enumOneShot = false;
	}
}
