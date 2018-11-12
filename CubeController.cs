
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollisionHelp))]
public class CubeController : MonoBehaviour
{
	GameObject _player;
	Rigidbody _rigidbody;
	BoxCollisionHelp _bCH;
	const float _collisionBias = 0.98f;

	public Transform respawnPoint;
	public bool moving;
	public LayerMask moveLayerMask;
	public LayerMask groundLayerMask;
	
	bool _deadReset;
	public bool dead;

	private void Awake()
	{
		SetPlayerGameObjectAndRigidbody();
		_bCH = GetComponent<BoxCollisionHelp>();
	}



	/// <summary>
	/// sets the player and rigidbody. gets attached gameObject if empty
	/// </summary>
	/// <param name="player"></param>
	public void SetPlayerGameObjectAndRigidbody(GameObject player = null)
	{
		//look into the ?? later..
		_player = player ?? gameObject;
		_rigidbody = _player.GetComponent<Rigidbody>();
	}
	
	/// <summary>
	/// basic movement based on the mass that excludes changing the y.
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="direction"></param>
	public void Move(float speed, Vector3 direction)
	{
		if (!_bCH.CollidingFromDirection(direction, moveLayerMask, _collisionBias))
		{
			//I dont want this to change the Y at all in this case so setting to be the current velocity.
			moving = true;
			Vector3 newDirection = speed * direction;

			newDirection.y = _rigidbody.velocity.y;

			_rigidbody.velocity = newDirection / _rigidbody.mass;
		}
		else
		{
			moving = false;
		}
	}

	
	public void MoveToPoint(Vector3 position)
	{
		_rigidbody.position = position;
	}

	public bool Ground()
	{
		return _bCH.CollidingFromDirection(-transform.up, groundLayerMask, 0.98f, 0);
	}

	/// <summary>
	/// continues moving up if jumpTime is greater than zero.
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="jumpTime"></param>
	public void Jump(float speed, float jumpTime)
	{
		if (jumpTime < 0)
		{
			jumpTime = 0;
		}

		if (jumpTime > 0)
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, speed, _rigidbody.velocity.z);
		}
	}

	public void Dead(float deathTime)
	{
		if (!_deadReset)
		{
			if (!dead)
			{
				_deadReset = true;
			}
			dead = true;
			StartCoroutine("RealDead",deathTime);
		}
	}
	IEnumerator RealDead(float deathTime)
	{
		yield return new WaitForSeconds(deathTime);
		_rigidbody.velocity = Vector3.zero;
		Respawn();
		_deadReset = false;
		dead = false;
	}

	public void Respawn()
	{
		transform.position = respawnPoint.position;
	}

	public void UseGravity(bool enabled)
	{
		_rigidbody.useGravity = enabled;
	}

	/// <summary>
	/// sets velocity to 0
	/// </summary>
	public void IsKinematic(bool kinematic)
	{
		_rigidbody.isKinematic = kinematic;
	}
}
