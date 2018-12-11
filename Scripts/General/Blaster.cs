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
		StartCoroutine("RotateTowards");
		StartCoroutine("Fire");
	}

	IEnumerator Wait()
	{
		waiting = true;
		//print("waiting");
		yield return new WaitForSeconds(pauseTime);
		Flip();
		waiting = false;
	}
	IEnumerator RotateTowards()
	{
		while (1 == 1)
		{
			Vector3 direction = transform.position - (transform.position - transform.up);
			float angleCheck = Vector3.Angle(-Vector3.down, direction);
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
				currentAngle = currentAngle >= 360 ? 0 : currentAngle;
				currentAngle = currentAngle < 0 ? 360 : currentAngle;
				currentAngle += speed * flipper;
			}
			transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
			yield return null;
		}
	}

	void Flip()
	{
		flipper *= -1;
	}

	IEnumerator Fire()
	{
		while (1 == 1)
		{
			yield return new WaitForSeconds(fireRate);
			GameObject bulletGO = Instantiate(_bullet, blastPos.position, blastPos.rotation);
			
		}
	}
}
