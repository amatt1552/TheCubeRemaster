using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : MonoBehaviour
{
	[Range(10, 179)]
	public float angleLimit = 25;
	public float speed = 2;
	float currentAngle;
	int flipper = 1;
	//makes sure it doesn't try to flip again until reset
	bool flipReset = true;
	public float pauseTime = 1;
	GameObject _bullet;
	//angle where it resets
	static float _RESET_ANGLE = 5;
	bool waiting;
	public Transform blastPos;
	public float fireRate = 2;
	

	void Start ()
	{
		
		_bullet = (GameObject)Resources.Load("Prefabs/Blast");
		if (_bullet == null)
			print("EERREEER");
		if (blastPos == null)
			blastPos = transform;
		InvokeRepeating("Fire", fireRate, fireRate);
	}

	private void Update()
	{
		RotateTowards();
	}

	IEnumerator Wait()
	{
		waiting = true;
		//print("waiting");
		yield return new WaitForSeconds(pauseTime);
		Flip();
		waiting = false;
	}

	void RotateTowards()
	{
		
		Vector3 direction = transform.position - (transform.position - transform.up);
		float angleCheck = Vector3.Angle(-Vector3.down, direction);
		angleCheck = Mathf.Ceil(angleCheck);
		if (angleCheck <= _RESET_ANGLE)
		{
			flipReset = true;
		}
		if (angleCheck >= angleLimit && flipReset)
		{
			StartCoroutine("Wait");
			flipReset = false;
		}
		if (!waiting)
		{
			currentAngle += speed * flipper;
			currentAngle = currentAngle >= 360 ? 0 : currentAngle;
			currentAngle = currentAngle < 0 ? 360 : currentAngle;
			if (currentAngle > angleLimit && currentAngle < 360 - angleLimit)
			{
				print("oops");
				if (flipper > 0)
				{
					currentAngle = 360 - angleLimit;
				}
				else
				{
					currentAngle = angleLimit;
				}
			}
			
			transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
		}
	}

	void Flip()
	{
		flipper *= -1;
	}

	void Fire()
	{
		
		GameObject bulletGO = Instantiate(_bullet, blastPos.position, blastPos.rotation);
			
	}
}
