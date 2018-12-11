using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Bouncer : MonoBehaviour
{
	public float targetHeight;

	public void Bounce(Rigidbody movedRb)
	{
		//v = final velocity
		//u = initial velocity
		//a = acceleration
		//s = distance
		//(v^2 - u^2) / 2a = s 
		//I want u
		//2as - v^2 = u^2
		//sqrt(2as - v^2) = u
		float velocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * targetHeight);
		print(velocity);
		movedRb.velocity = transform.up * velocity;
	}
	void OnGUI()
	{
		Debug.DrawLine(transform.position, transform.position + Vector3.up * targetHeight);
	}
}
