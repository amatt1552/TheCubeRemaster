using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
	static GameObject _staticGameObject;
	static bool _CREATED;
	void Start ()
	{
		if (!_CREATED)
		{
			DontDestroyOnLoad(gameObject);
			_staticGameObject = gameObject;
			_CREATED = true;
		}

		if (gameObject != _staticGameObject)
		{
			Destroy(gameObject);
		}
		
		
	}
	
}
