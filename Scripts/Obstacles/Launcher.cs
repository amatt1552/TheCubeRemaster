using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class Launcher : MonoBehaviour
{
	//public GameObject targetObject;
	[Range(1,45)]
	public float angle = 45;
	public Transform targetPosition;
	private float _velocity;
	public Vector3 direction;
	ParticleSystem part;
	ParticleSystem.VelocityOverLifetimeModule PartS;

	private void Awake()
	{
		part = GetComponent<ParticleSystem>();
		PartS = part.velocityOverLifetime;
		part.Play();
	}
	private void Update()
	{
		if (Application.isPlaying)
		{

			part.Clear();
			part.Stop();
		}
		else
		{
			
			part = GetComponent<ParticleSystem>();
			PartS = part.velocityOverLifetime;
		}

		CalculateForce();
	}

	void CalculateForce()
	{
		float gravity = Physics.gravity.magnitude;
		Vector3 distance = targetPosition.position - transform.position;
		
		float distanceY = -distance.y;
		distance.y = 0;
		float distanceXZ = distance.magnitude;
		
		float angleRadians = angle * Mathf.Deg2Rad;
		
		//https://answers.unity.com/questions/1353777/calculate-initial-velocity-given-distance-gravity.html
		//velocity = (1/ Mathf.Cos(angle * Mathf.Deg2Rad))*Mathf.Sqrt(0.5F*distanceX*distanceX* gravity /(distanceY + Mathf.Tan(angle * Mathf.Deg2Rad) * distanceX));
		_velocity = (1 / Mathf.Cos(angleRadians)) * Mathf.Sqrt((0.5f * distanceXZ * distanceXZ * gravity) / (distanceY + Mathf.Tan(angleRadians) * distanceXZ));
		//print((Mathf.Cos(angleRadians) * velocityReduction) + 0.3f);
		//cos(θ) = Adjacent / Hypotenuse. I want Adjacent.
		float directionX = Mathf.Cos(angleRadians) * _velocity;
		//sin(θ) = Opposite / Hypotenuse. I want Opposite
		float directionY = Mathf.Sin(angleRadians) * _velocity;
		//just guessing with the Z...
		float directionZ = Mathf.Cos(angleRadians) * _velocity;
		
		//helps me get the direction for the x and z
		Vector3 normalizedDistance = distance.normalized;
		normalizedDistance.y = 1;
		direction = new Vector3(directionX, directionY, directionZ).Multiply(normalizedDistance);
		PartS.x = direction.x;
		PartS.y = direction.y;
		PartS.z = direction.z;

	}

	public void Launch(GameObject launchedObject)
	{
		Rigidbody rb = launchedObject.GetComponent<Rigidbody>();
		
		if (launchedObject != null)
		{
			rb.velocity = direction;
		}
	}

	public float GetDistance()
	{
		Vector3 distance = targetPosition.position - transform.position;
		distance.y = 0;
		return distance.magnitude;
	}
	/*
	Vector3[] CalculateArc()
	{
		Vector3[] arcArray = new Vector3[resolution + 1];
		gravity = Physics.gravity.magnitude;

		radianAngle = angle * Mathf.Deg2Rad;
		float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;
		for (int i = 0; i <= resolution; i++)
		{
			float t = (float)i / resolution;
			arcArray[i] = CalculateArcPoint(t, maxDistance);
		}

		return arcArray;
	}

	Vector3 CalculateArcPoint(float t, float maxDistance)
	{
		float x = t * maxDistance;
		float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
		return new Vector3(x, y);
	}
	*/

}
