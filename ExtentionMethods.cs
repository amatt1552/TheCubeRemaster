using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethods
{
	/// <summary>
	/// multiplies each component of the this Vector by the multiplied Vector.
	/// </summary>
	/// <param name="thisVector"></param>
	/// <param name="multipliedVector"></param>
	/// <returns></returns>
	public static Vector3 Multiply(this Vector3 thisVector, Vector3 multipliedVector)
	{
		Vector3 returnedVector = new Vector3(thisVector.x * multipliedVector.x, thisVector.y * multipliedVector.y, thisVector.z * multipliedVector.z);
		return returnedVector;
	}
}
