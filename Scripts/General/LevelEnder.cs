using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class LevelEnder : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Collider>().isTrigger = true;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			TheCubeGameManager.LoadNextLevel();
		}
	}
}
