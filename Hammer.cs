using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Hammer : MonoBehaviour
{
	Rigidbody rb;
	public float speed = 10;

	[Tooltip("Sets the angle where it will change directions. If set to 180 will continue to rotate")]
	[Range(0, 180)]
	public float angleLimit = 50;

	[Tooltip("if true then directionChangeSpeed will be determined by dividing speed by directionChangeSpeed. In this case lower values make it faster.")]
	public bool directionChangeSpeedIsBasedOnSpeed;

	[Tooltip("how fast it changes direction")]
	public float directionChangeSpeed = 10;
	
	//target direction
	float _targetDirection = 1;
	//current direction
	float _currentDirection;
	bool flipReset;

	void Awake ()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
	{
		Vector3 baseDirection, currentDirection;
		float angle;
		float newDirectionChangeSpeed = directionChangeSpeed;

		baseDirection = -transform.root.up;
		currentDirection = (transform.position -transform.up) - transform.position;
		angle = Vector3.Angle(currentDirection, baseDirection);

		if (angle > angleLimit && !flipReset)
		{
			_targetDirection *= -1;
			flipReset = true;
		}
		else if (angle < 5)
		{
			flipReset = false;
		}

		if (directionChangeSpeedIsBasedOnSpeed)
		{
			newDirectionChangeSpeed = Mathf.Abs(speed / directionChangeSpeed);
		}

		_currentDirection += _targetDirection * newDirectionChangeSpeed * Time.deltaTime;
		_currentDirection = Mathf.Clamp(_currentDirection,-1,1);
		print(transform.root.name + " " + _currentDirection);
		rb.angularVelocity = transform.root.right * _currentDirection * speed;
	}
}
