using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNav : MonoBehaviour
{
	[Header("If rigidbody is found then will move with")]
	[Space(-10)]
	[Header("MovePosition, else will move the transform.")]
	public Waypoints waypoints;
	public int speed = 1;
	public float waitTime = 1;
	int _target;
	const float distanceIsCloseTo = 0.1f;
	Rigidbody _rb;
	bool _waiting = false;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		
	}

	private void Start()
	{
		if (_rb == null)
		{
			Invoke("Move",0);
		}
	}

	void FixedUpdate()
	{
		if (_rb != null)
		{
			MoveRb();
		}
	}

	private void Update()
	{
		if (_rb == null)
		{
			Move();
		}
	}

	void Move()
	{

		if (Vector3.Distance(transform.position, waypoints.connectedPoints[_target].position) < distanceIsCloseTo && !_waiting)
		{
			StartCoroutine("Wait");
			MoveToNext();
		}
		else if (!_waiting)
		{
			transform.position = Vector3.MoveTowards(transform.position, waypoints.connectedPoints[_target].position, speed * Time.deltaTime);
		}
	}

	void MoveRb()
	{
		if (Vector3.Distance(transform.position, waypoints.connectedPoints[_target].position) < distanceIsCloseTo && !_waiting)
		{

			StartCoroutine("Wait");
			MoveToNext();
		}
		else if(!_waiting)
		{
			_rb.MovePosition(Vector3.MoveTowards(transform.position, waypoints.connectedPoints[_target].position, speed * Time.deltaTime));
		}
	}

	IEnumerator Wait()
	{
		_waiting = true;
		yield return new WaitForSeconds(waitTime);
		_waiting = false;
	}

	void MoveToNext()
	{
		_target++;
		if (_target >= waypoints.connectedPoints.Length)
		{
			_target = 0;
		}
	}

	public Vector3 CurrentDirection()
	{
		Vector3 direction = transform.position - waypoints.connectedPoints[_target].position;
		return direction.normalized;
	}

	public bool Waiting()
	{

		return _waiting;
	}
}
