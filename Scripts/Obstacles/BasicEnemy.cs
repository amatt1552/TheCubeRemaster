using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
	public WaypointNav nav;

	void Awake()
    {
		if (nav == null)
		{
			nav = GetComponent<WaypointNav>();
		}
    }
	
    void Update()
    {
		if (nav != null)
		{
			if (!nav.Waiting())
			{
				//clamps to 0, 1, and -1
				float rotationDirection;
				if (nav.CurrentDirection().x != 0)
				{
					rotationDirection = nav.CurrentDirection().x / Mathf.Abs(nav.CurrentDirection().x);
				}
				else
				{
					rotationDirection = 0;
				}
				transform.Rotate(transform.forward * nav.speed * rotationDirection);
			}
		}
    }
}
