using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraEditSettings : MonoBehaviour
{
	public enum Shape
	{
		sphere,
		box,
		custom
	}
	public Shape currentShape;
	public float radius = 1;
	//add editor script to hide more later
	public Vector3 boxSize = Vector3.one;
	public Vector3 newCameraOffset;

	BoxCollider _boxCollider;
	SphereCollider _sphereCollider;

    void Update()
    {
		switch (currentShape)
		{
			case Shape.sphere:
				if (_boxCollider != null)
				{
					DestroyImmediate(_boxCollider);
				}
				if (_sphereCollider == null)
				{
					_sphereCollider = GetComponent<SphereCollider>();
					if (_sphereCollider == null)
					{
						_sphereCollider = gameObject.AddComponent<SphereCollider>();
					}
				}
				_sphereCollider.radius = radius;

				break;

			case Shape.box:
				if (_sphereCollider != null)
				{
					DestroyImmediate(_sphereCollider);
				}
				if (_boxCollider == null)
				{
					_boxCollider = GetComponent<BoxCollider>();
					if (_boxCollider == null)
					{
						_boxCollider = gameObject.AddComponent<BoxCollider>();
					}
				}
				_boxCollider.size = boxSize;

				break;

			case Shape.custom:
				break;
		}
    }
}
